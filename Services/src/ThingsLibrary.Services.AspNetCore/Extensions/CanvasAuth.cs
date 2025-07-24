// ================================================================================
// <copyright file="CanvasAuth.cs" company="Starlight Software Co">
//    Copyright (c) Starlight Software Co. All rights reserved.
//    Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
// </copyright>
// ================================================================================

using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Microsoft.Identity.Web;
using Microsoft.IdentityModel.JsonWebTokens;
using Microsoft.IdentityModel.Protocols.OpenIdConnect;
using Microsoft.IdentityModel.Tokens;
using Serilog;
using ThingsLibrary.Schema.Canvas;


namespace ThingsLibrary.Services.AspNetCore.Extensions
{
    public static class AuthExtensions
    {
        public static void AddCanvasAuthAzureAd(this IServiceCollection services, IConfiguration configuration)
        {
            var azureAdSection = configuration.GetSection("AzureAD");

            var authBuilder = services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme);

            // ====================================================================================================
            // Azure AD integration?
            // ====================================================================================================            
            if (azureAdSection.Exists())
            {
                Log.Debug("+ {AppCapability}", "Microsoft Identity");
                authBuilder.AddMicrosoftIdentityWebApp(options =>
                {
                    configuration.Bind("AzureAd", options);
                    //options.Events = new JwtBearerEvents
                    //{
                    //    OnTokenValidated = context =>
                    //    {
                    //        // Add custom logic here if needed
                    //        Console.WriteLine("TokenValidated");
                    //        return Task.CompletedTask;
                    //    },
                    //    OnAuthenticationFailed = context =>
                    //    {
                    //        // Add custom logic here if needed
                    //        Console.WriteLine("AuthenticationFailed" + context.Exception.Message);
                    //        return Task.CompletedTask;
                    //    }
                    //};

                    options.TokenValidationParameters = new TokenValidationParameters
                    {                        
                        ValidateAudience = false,
                        // When set to false, the JWT's issuer will not be validated
                        ValidateIssuer = true,
                        // When set to false, JWTs without an 'exp' claim in their payload will be rejected
                        RequireExpirationTime = true,
                        // When set to false, the JWT's signature will not be validated
                        ValidateIssuerSigningKey = false,

                        // fool the validation logic since we aren't validating tokens
                        SignatureValidator = delegate (string token, TokenValidationParameters parameters)
                        {
                            return new JsonWebToken(token);
                        },

                        // When set to false, the JWT's lifetime (as given by the 'exp' claim) will not be checked
                        ValidateLifetime = true,
                    };
                }, options =>
                {
                    configuration.Bind("AzureAd", options);
                });
            }

        }


        public static AuthenticationBuilder AddCanvasAuth(this IServiceCollection services, IConfiguration configuration)
        {
            // require jwt section
            if (App.Service.Canvas?.Auth == null) { throw new ArgumentException("Canvas Auth must be defined."); }
            if (App.Service.Canvas.Auth?.Jwt == null) { throw new ArgumentException("JWT must be included to add auth security."); }

            var auth = App.Service.Canvas.Auth;
            if (auth.OpenId != null && auth.Cookie == null) { throw new ArgumentException("OpenID must include a Cookie definition."); }

            // ================================================================================
            // AUTHENTICATION
            JwtSecurityTokenHandler.DefaultMapInboundClaims = false;

            var azureAdSection = configuration.GetSection("AzureAD");

            AuthenticationBuilder authBuilder;

            if ((auth.OpenId != null || azureAdSection.Exists()) && auth.Jwt != null)
            {
                authBuilder = services.AddAuthentication(options =>
                {
                    options.DefaultAuthenticateScheme = "JWT_OR_OIDC";
                    options.DefaultChallengeScheme = "JWT_OR_OIDC";
                    options.DefaultScheme = "JWT_OR_OIDC";
                });

                authBuilder.AddPolicyScheme("JWT_OR_OIDC", "JWT_OR_OIDC", options =>
                {
                    // runs on each request
                    options.ForwardDefaultSelector = context =>
                    {
                        // filter by auth type
                        string authorization = $"{context.Request.Headers[Microsoft.Net.Http.Headers.HeaderNames.Authorization]}";
                        if (!string.IsNullOrEmpty(authorization) && authorization.StartsWith("Bearer "))
                        {
                            return JwtBearerDefaults.AuthenticationScheme;
                        }
                        else
                        {
                            // otherwise always check for cookie auth
                            return OpenIdConnectDefaults.AuthenticationScheme;
                        }
                    };
                });
            }
            else if (auth.OpenId != null)
            {
                Log.Debug("+ {AppCapability}", "OpenID Authentication (OIDC)");
                authBuilder = services.AddAuthentication(OpenIdConnectDefaults.AuthenticationScheme);
            }
            else// if(auth.Jwt != null)
            {
                Log.Debug("+ {AppCapability}", "JWT Authentication");
                authBuilder = services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme);
            }

            // ====================================================================================================
            // COOKIE
            // ====================================================================================================            
            if (auth.Cookie != null)
            {
                Log.Debug("======================================================================");
                Log.Debug("= Cookies");
                Log.Debug("--- Name: '{CookieName}'", auth.Cookie.Name);
                Log.Debug("--- Path: '{CookiePath}'", auth.Cookie.Path);
                Log.Debug("--- Domain: '{CookieDomain}'", auth.Cookie.Domain);

                // add the cookie 
                authBuilder.AddCookie(CookieAuthenticationDefaults.AuthenticationScheme, options =>
                {
                    options.Cookie.Name = auth.Cookie.Name;
                    options.Cookie.Path = auth.Cookie.Path;
                    options.Cookie.Domain = auth.Cookie.Domain;

                    options.Cookie.HttpOnly = true;
                    options.Cookie.SecurePolicy = CookieSecurePolicy.SameAsRequest;     //TODO: change to Canvas driven
                    options.Cookie.SameSite = SameSiteMode.Lax;                         //TODO: change to Canvas driven

                    options.LogoutPath = auth.LogoutUrl;
                    options.LoginPath = auth.LogoutUrl;
                    options.AccessDeniedPath = auth.AccessDeniedUrl;

                    // ReturnUrlParameter requires 
                    options.ReturnUrlParameter = auth.ReturnUrlParameter;
                    options.ExpireTimeSpan = auth.ExpireTimeSpan;
                    options.SlidingExpiration = auth.SlidingExpiration;
                });

                services.AddAntiforgery(options => {
                    options.Cookie.SameSite = SameSiteMode.Lax;                 //TODO: change to Canvas driven 
                });
            }

            // ====================================================================================================
            // JWT Config
            // ====================================================================================================
            if (auth.Jwt != null)
            {
                services.AddCanvasAuthJwt(authBuilder);
            }

            // ====================================================================================================
            // OPEN ID CONNECT Config
            // ====================================================================================================
            if (auth.OpenId != null)
            {
                services.AddCanvasAuthOpenId(auth.OpenId, authBuilder);
            }

            // ====================================================================================================
            // Azure AD integration?
            // ====================================================================================================            
            if (azureAdSection.Exists())
            {
                Log.Debug("+ {AppCapability}", "Microsoft Identity");
                authBuilder.AddMicrosoftIdentityWebApp(azureAdSection);
            }

            // ================================================================================
            // AUTHORIZATION
            // ====================================================================================================
            // AUTHORIZATION Config
            // ====================================================================================================
            Log.Debug("======================================================================");
            Log.Debug("Service Authentication");
            Log.Debug("- App Roles:");
            auth.AppRoles.ForEach(role =>
            {
                Log.Debug("--- {AppRole}", role);
            });

            services.AddAuthorization(options =>
            {
                // add all the role -> claim matches            
                Log.Debug("- Policies:");
                auth.PolicyClaimsMap.ForEach(map =>
                {
                    Log.Debug("--- Policy: {AppPolicy}, Roles: {AppPolicyRoles}", map.Key, string.Join(", ", map.Value));

                    // Make sure all the roles specified are in the expected app roles listing
                    foreach (var role in map.Value)
                    {
                        if (!auth.AppRoles.Contains(role)) { throw new ArgumentException($"Policy Claims Map in Canvas:Auth references missing role '{role}'"); }
                    }
                    options.AddPolicy(map.Key, policy => policy.RequireClaim(App.Service.Canvas.Auth.Jwt!.RoleClaimType, map.Value));
                });
                
                // By default, all incoming requests will be authorized according to the default policy.
                options.FallbackPolicy = options.DefaultPolicy;
            });

            return authBuilder;
        }

        public static void AddCanvasAuthJwt(this IServiceCollection services, AuthenticationBuilder? authBuilder)
        {
            if (App.Service?.Canvas?.Auth?.Jwt == null) { throw new ArgumentException("Service Canvas Auth JWT Not Defined."); }

            var authJwt = App.Service.Canvas.Auth.Jwt;

            if (authBuilder == null)
            {
                Log.Debug("+ {AppCapability}", "JWT Authentication");
                authBuilder = services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme);
            }

            // make sure higher environments don't have disabled validation
            if (authJwt.DisableValidation && App.Service.IsProduction())
            {
                throw new ArgumentException("JWT Disable Validation not allowed in production environments!");
            }

            Log.Debug("======================================================================");
            Log.Debug("= JWT Configuration:");
            if (!authJwt.DisableValidation)
            {
                Log.Debug("--- Issuer: {JwtIssuer}", (!string.IsNullOrEmpty(authJwt.Issuer) ? authJwt.Issuer : "<< ANY >>"));
                Log.Debug("--- Audience: {JwtAudience}", (!string.IsNullOrEmpty(authJwt.Audience) ? authJwt.Audience : "<< ANY >>"));
            }
            else
            {
                Log.Warning("--- VALIDATION DISABLED!");
            }

            // this fires later
            authBuilder.AddJwtBearer(JwtBearerDefaults.AuthenticationScheme, options =>
            {
                options.Authority = authJwt.Authority!.OriginalString;
                options.Audience = (!string.IsNullOrEmpty(authJwt.Audience) ? authJwt.Audience : null);

                if (!authJwt.DisableValidation)
                {
                    //options.TokenValidationParameters = App.Service.OAuth2Client!.GetTokenValidationParametersAsync().Result;
                }
                else
                {
                    // since this config will prevent a request even getting to the service layers we need a auth that only makes sure a token was provided.. we can do token validation checks later
                    options.TokenValidationParameters = new TokenValidationParameters
                    {
                        ValidateIssuer = false,
                        ValidateAudience = false,
                        RequireAudience = false,

                        ValidateLifetime = false,
                        RequireSignedTokens = false,
                        ValidateIssuerSigningKey = false,

                        // fool the validation logic since we aren't validating tokens
                        SignatureValidator = delegate (string token, TokenValidationParameters parameters)
                        {
                            return new JsonWebToken(token);
                        },

                        NameClaimType = authJwt.NameClaimType,
                        RoleClaimType = authJwt.RoleClaimType
                    };
                }

                options.Events ??= new JwtBearerEvents();                
            });
        }
        
        public static void AddCanvasAuthOpenId(this IServiceCollection services, CanvasAuthOpenId authOpenId, AuthenticationBuilder? authBuilder)
        {
            if (string.IsNullOrEmpty(authOpenId.ClientId)) { throw new ArgumentException("Missing 'Auth:Openid:ClientId' configuration setting."); }
            if (string.IsNullOrEmpty(authOpenId.ClientSecret)) { throw new ArgumentException("Missing 'Auth:Openid:ClientSecret' configuration setting."); }
            if (authOpenId.Authority == null) { throw new ArgumentException("Missing 'Auth:Openid:Authority' configuration setting."); }

            if (!authOpenId.Scope.Any()) { throw new ArgumentException("Auth Missing OpenId Scopes configuration setting."); }

            Log.Debug("======================================================================");
            Log.Debug("= OpenID Connect:");
            Log.Debug("--- Authority: {OpenIdAuthority}", authOpenId.Authority);
            Log.Debug("--- ClientID: {OpenIdClientId}", authOpenId.ClientId);
            Log.Debug("--- Scope: {OpenIdScope}", string.Join(", ", authOpenId.Scope));

            if (authBuilder == null)
            {
                Log.Debug("+ {AppCapability}", "OpenID Authentication (OIDC)");
                authBuilder = services.AddAuthentication(OpenIdConnectDefaults.AuthenticationScheme);
            }


            authBuilder.AddOpenIdConnect(OpenIdConnectDefaults.AuthenticationScheme, options =>
            {
                options.Authority = authOpenId.Authority.OriginalString;
                options.ClientId = authOpenId.ClientId;
                options.ClientSecret = authOpenId.ClientSecret;
                options.CallbackPath = !string.IsNullOrEmpty(authOpenId.CallbackUrl) ? authOpenId.CallbackUrl : "/signin-oidc";

                options.Scope.Clear();
                authOpenId.Scope.ForEach(scope => options.Scope.Add(scope));

                options.SignInScheme = CookieAuthenticationDefaults.AuthenticationScheme;
                options.SignOutScheme = CookieAuthenticationDefaults.AuthenticationScheme;
                options.ResponseType = OpenIdConnectResponseType.Code;
                options.ResponseMode = OpenIdConnectResponseMode.Query;
                options.GetClaimsFromUserInfoEndpoint = authOpenId.UseUserInfoForClaims;
                options.UsePkce = authOpenId.UsePkce;
                options.SaveTokens = true;

                options.TokenValidationParameters = new TokenValidationParameters
                {
                    NameClaimType = "name",
                    RoleClaimType = "roles"
                };                
            });
        }


        /// <summary>
        /// Use Auth Security
        /// </summary>
        /// <param name="app"><see cref="IApplicationBuilder"/></param>        
        /// <returns><see cref="IApplicationBuilder"/> for chaining calls</returns>
        /// <remarks>Configures App with: UseAuthentication, UseAuthorization</remarks>
        public static IApplicationBuilder UseCanvasAuthSecurity(this IApplicationBuilder app)
        {
            if (App.Service?.Canvas == null) { throw new ArgumentException("Service Canvas Not Defined."); }
            if (App.Service.Canvas.Auth == null) { return app; }

            Log.Debug("Configuring Canvas Auth Security...");

            app.UseAuthentication();            
            app.UseAuthorization();

            return app;
        }
    }
}
