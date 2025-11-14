// ================================================================================
// <copyright file="CanvasAuth.cs" company="Starlight Software Co">
//    Copyright (c) Starlight Software Co. All rights reserved.
//    Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
// </copyright>
// ================================================================================

using System.IdentityModel.Tokens.Jwt;

using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.Identity.Web;
using Microsoft.IdentityModel.JsonWebTokens;
using Microsoft.IdentityModel.Protocols.OpenIdConnect;
using Microsoft.IdentityModel.Tokens;

using Serilog;

namespace ThingsLibrary.Services.AspNetCore.Extensions
{
    public static class AuthExtensions
    {
        public static AuthenticationBuilder AddCanvasAuth(this IServiceCollection services, IConfiguration configuration, ItemDto authenticationOptions)
        {
            if (authenticationOptions.Type != "authentication") { throw new ArgumentException($"Invalid type '{authenticationOptions.Type}', expecting 'auth' for auth options."); }

            if (authenticationOptions.Items.ContainsKey("open_id") && !authenticationOptions.Items.ContainsKey("cookie")) { throw new ArgumentException("OpenID must include a Cookie definition."); }

            // ================================================================================
            // AUTHENTICATION
            JwtSecurityTokenHandler.DefaultMapInboundClaims = false;

            AuthenticationBuilder authBuilder;

            if ((authenticationOptions.Items.ContainsKey("open_id") || authenticationOptions.Items.ContainsKey("azure_ad")) && authenticationOptions.Items.Any(x => x.Value.Type == "jwt"))
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
            else if (authenticationOptions.Items.ContainsKey("open_id"))
            {
                Log.Debug("+ {AppCapability}", "OpenID Authentication (OIDC)");
                authBuilder = services.AddAuthentication(options =>
                {
                    options.DefaultScheme = CookieAuthenticationDefaults.AuthenticationScheme;
                    options.DefaultChallengeScheme = OpenIdConnectDefaults.AuthenticationScheme;
                });
            }
            else// if(auth.Jwt != null)
            {
                Log.Debug("+ {AppCapability}", "JWT Authentication");
                authBuilder = services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme);
            }

            // ====================================================================================================
            // cookie
            // ====================================================================================================
            if (authenticationOptions.Items.ContainsKey("cookie"))
            {
                services.AddCanvasAuthCookie(authBuilder, authenticationOptions);
            }

            // ====================================================================================================
            // JWT
            // ====================================================================================================
            if (authenticationOptions.Items.TryGetValue("jwt", out var jwtOptions))
            {
                services.AddCanvasAuthJwt(authBuilder, jwtOptions);                
            }

            //foreach (var jwtOptions in authenticationOptions.Items.Values.Where(x => x.Type == "jwt"))
            //{
            //    services.AddCanvasAuthJwt(authBuilder, jwtOptions);
            //}

            // ====================================================================================================
            // OPEN ID CONNECT Config
            // ====================================================================================================
            if (authenticationOptions.Items.TryGetValue("open_id", out var openIdOptions))
            {
                services.AddCanvasAuthOpenId(authBuilder, openIdOptions);
            }

            // ====================================================================================================
            // Azure AD integration?
            // ====================================================================================================            
            if (authenticationOptions.Items.TryGetValue("azure_ad", out var azureAdOptions))
            {
                services.AddCanvasAuthAzureAd(authBuilder, azureAdOptions);
            }

            return authBuilder;
        }

        public static void AddCanvasAuthOpenId(this IServiceCollection services, AuthenticationBuilder authBuilder, ItemDto openIdOptions)
        {
            if (openIdOptions.Type != "open_id") { throw new ArgumentException($"Invalid type '{openIdOptions.Type}', expecting 'open_id' for Open ID auth options."); }

            if (string.IsNullOrEmpty(openIdOptions["authority"])) { throw new ArgumentException("Missing 'Auth:Openid:Authority' configuration setting."); }
            if (string.IsNullOrEmpty(openIdOptions["client_id"])) { throw new ArgumentException("Missing 'Auth:Openid:ClientId' configuration setting."); }
            if (string.IsNullOrEmpty(openIdOptions["client_scret"])) { throw new ArgumentException("Missing 'Auth:Openid:ClientSecret' configuration setting."); }
            if (string.IsNullOrEmpty(openIdOptions["scope"])) { throw new ArgumentException("Auth Missing OpenId Scopes configuration setting."); }

            Log.Debug("======================================================================");
            Log.Debug("= OpenID Connect:");
            Log.Debug("--- Authority: {OpenIdAuthority}", openIdOptions["authority"]);
            Log.Debug("--- ClientID: {OpenIdClientId}", openIdOptions["client_id"]);
            Log.Debug("--- Scope: {OpenIdScope}", string.Join(", ", openIdOptions["scope"]));

            if (authBuilder == null)
            {
                Log.Debug("+ {AppCapability}", "OpenID Authentication (OIDC)");
                authBuilder = services.AddAuthentication(OpenIdConnectDefaults.AuthenticationScheme);
            }

            authBuilder.AddOpenIdConnect(OpenIdConnectDefaults.AuthenticationScheme, options =>
            {
                options.Authority = openIdOptions["authority"];
                options.ClientId = openIdOptions["client_id"];
                options.ClientSecret = openIdOptions["client_secret"];
                options.CallbackPath = openIdOptions["callback_url"] ?? "/signin-oidc";

                options.Scope.Clear();
                var scopes = openIdOptions["scope"]?.Split('|', StringSplitOptions.RemoveEmptyEntries) ?? new string[0];
                scopes.ForEach(scope => options.Scope.Add(scope));

                options.SignInScheme = CookieAuthenticationDefaults.AuthenticationScheme;
                options.SignOutScheme = CookieAuthenticationDefaults.AuthenticationScheme;
                options.ResponseType = OpenIdConnectResponseType.Code;
                options.ResponseMode = OpenIdConnectResponseMode.Query;
                options.GetClaimsFromUserInfoEndpoint = openIdOptions.Tags.GetValue<bool>("use_user_info_for_claims", false);
                options.UsePkce = openIdOptions.Tags.GetValue<bool>("use_pkce", false);
                options.SaveTokens = true;

                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidateIssuerSigningKey = true,
                    ValidateAudience = true,
                    ValidateLifetime = true,

                    NameClaimType = "name",
                    RoleClaimType = "roles"
                };

                //Additional config snipped
                //options.Events = new OpenIdConnectEvents
                //{
                //    OnTokenValidated = async context =>
                //    {
                //        //var identity = context.Principal.Identity as ClaimsIdentity;

                //        //if (identity != null && IsEmployee()
                //        //{
                //        //    identity.AddClaim(new Claim("roles", "Employee"));
                //        //}
                //    }
                //};
            });
        }

        public static void AddCanvasAuthAzureAd(this IServiceCollection services, AuthenticationBuilder authBuilder, ItemDto azureAdOptions)
        {
            //{
            //    "authority": "https://c57be6f1-b43d-4a29-97a3-f1c6e9a727a4.ciamlogin.com/c57be6f1-b43d-4a29-97a3-f1c6e9a727a4/v2.0",
            //    "domain": "signodeconnectusers.onmicrosoft.com",
            //    "client_id": "4f62a04e-86aa-4c0f-9449-411b4b69503e",
            //    "callback_path": "/signin-oidc",
            //    "signed_out_callback_path ": "/signout-callback-oidc",
            //    "scopes": "api://4f62a04e-86aa-4c0f-9449-411b4b69503e/default",
            //    "name_claim_type": "name",
            //    "role_claim_type": "roles",
            //}

            if (azureAdOptions.Type != "azure_ad") { throw new ArgumentException($"Invalid type '{azureAdOptions.Type}', expecting 'azure_ad' for Azure AD auth options."); }


            Log.Debug("+ {AppCapability}", "Microsoft Identity");

            authBuilder.AddMicrosoftIdentityWebApp(options =>
            {
                options.Authority = azureAdOptions["authority"];
                options.Domain = azureAdOptions["domain"];
                options.ClientId = azureAdOptions["client_id"];
                options.ClientSecret = azureAdOptions["client_secret"];
                options.CallbackPath = azureAdOptions["callback_path"];
                options.SignedOutCallbackPath = azureAdOptions["signed_out_callback_path"];

                if (azureAdOptions.Items.ContainsKey("scopes"))
                {
                    var scopes = azureAdOptions["scopes"].Split(' ', StringSplitOptions.RemoveEmptyEntries);
                    scopes.ForEach<string>(x => options.Scope.Add(x));
                }
            });

            services.PostConfigure<OpenIdConnectOptions>(OpenIdConnectDefaults.AuthenticationScheme, options =>
            {
                // The claim in the Jwt token where App roles are available.
                options.TokenValidationParameters.NameClaimType = azureAdOptions["name_claim_type"] ?? "name";
                options.TokenValidationParameters.RoleClaimType = azureAdOptions["role_claim_type"] ?? "roles";
            });
        }

        public static void AddCanvasAuthorization(this IServiceCollection services, AuthenticationBuilder authBuilder, ItemDto authorizationOptions)
        {
            if (authorizationOptions.Type != "authorization") { throw new ArgumentException($"Invalid type '{authorizationOptions.Type}', expecting 'authorization' for authorization options."); }

            Log.Debug("======================================================================");
            Log.Debug("Service Authorization");
            Log.Debug("- App Roles:");            
            services.AddAuthorization(options =>
            {
                // add all the role -> claim matches            
                Log.Debug("- Policies:");
                var policies = authorizationOptions.Items.Where(x => x.Value.Type == "policy");

                foreach(var policy in policies)
                {
                    var roles = policy.Value["roles"]?.Split('|', StringSplitOptions.RemoveEmptyEntries) ?? new string[0];
                    
                    Log.Debug("--- Policy: {AppPolicy}, Roles: {AppPolicyRoles}", policy.Key, string.Join(", ", roles));

                    // Make sure all the roles specified are in the expected app roles listing
                    
                    options.AddPolicy(policy.Key, policy => policy.RequireClaim("role", roles));
                };

                // ================================================================================
                // REQUIRE EVERYONE TO LOGIN                
                // By default, all incoming requests will be authorized according to the default policy.
                //options.FallbackPolicy = new AuthorizationPolicyBuilder().RequireAuthenticatedUser().Build(); //options.DefaultPolicy;
            });         
        }
        
        public static void AddCanvasAuthJwt(this IServiceCollection services, AuthenticationBuilder authBuilder, ItemDto jwtOptions)
        {
            //{            
            //    "authority": "https://signodeconnectusers.ciamlogin.com/c57be6f1-b43d-4a29-97a3-f1c6e9a727a4/v2.0",
            //    "issuer": "https://c57be6f1-b43d-4a29-97a3-f1c6e9a727a4.ciamlogin.com/c57be6f1-b43d-4a29-97a3-f1c6e9a727a4/v2.0",
            //    "issuers": "https://sts.windows.net/c57be6f1-b43d-4a29-97a3-f1c6e9a727a4/|https://sts.windows.net/7db30361-ce9f-4244-8d28-60553bffff38/",
            //    "audience": "portal",
            //    "client_id": "4f62a04e-86aa-4c0f-9449-411b4b69503e",
            //    "name_claim_type": "name",
            //    "role_claim_type": "roles",
            //    "disable_validation": "true",
            //    "show_validation_errors": "true"
            //}

            if (jwtOptions.Type != "jwt") { throw new ArgumentException($"Invalid type '{jwtOptions.Type}', expecting 'jwt' for JWT auth options."); }

            //if (authBuilder == null)
            //{
            //    Log.Debug("+ {AppCapability}", "JWT Authentication");
            //    authBuilder = services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme);
            //}

            // make sure higher environments don't have disabled validation
            //if (jwtOptions.Tags.GetValue<bool>("disable_validation", false))
            //{
            //    throw new ArgumentException("JWT Disable Validation not allowed in production environments!");
            //}

            Log.Debug("======================================================================");
            Log.Debug("= JWT Configuration:");
            if (!jwtOptions.Tags.GetValue<bool>("disable_validation", false))
            {
                Log.Debug("--- Issuer: {JwtIssuer}", (!string.IsNullOrEmpty(jwtOptions["issuer"]) ? jwtOptions["issuer"] : "<< ANY >>"));
                Log.Debug("--- Audience: {JwtAudience}", (!string.IsNullOrEmpty(jwtOptions["audience"]) ? jwtOptions["audience"] : "<< ANY >>"));
            }
            else
            {
                Log.Warning("--- VALIDATION DISABLED!");
            }

            // this fires later
            authBuilder.AddJwtBearer(JwtBearerDefaults.AuthenticationScheme, options =>
            {
                options.Authority = jwtOptions["authority"];
                options.Audience = (!string.IsNullOrEmpty(jwtOptions["audience"]) ? jwtOptions["audience"] : null);
                
                if (!jwtOptions.Tags.GetValue<bool>("disable_validation", false))
                {
                    //options.TokenValidationParameters = App.Service.OAuth2Client!.GetTokenValidationParametersAsync().Result;
                    options.TokenValidationParameters = new TokenValidationParameters
                    {
                        ValidateIssuer = true,
                        ValidIssuer = jwtOptions["issuer"],
                        ValidIssuers = jwtOptions["issuers"]?.Split('|', StringSplitOptions.RemoveEmptyEntries),
                        
                        ValidateAudience = true,
                        ValidAudience = jwtOptions["audience"],

                        ValidateLifetime = true,

                        NameClaimType = jwtOptions["name_claim_type"],
                        RoleClaimType = jwtOptions["role_claim_type"]
                    };
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

                        NameClaimType = jwtOptions["name_claim_type"],
                        RoleClaimType = jwtOptions["role_claim_type"]
                    };
                }
            });
        }

        public static void AddCanvasAuthCookie(this IServiceCollection services, AuthenticationBuilder authBuilder, ItemDto authOptions)
        {
            //{
            //  "cookie_name": "CookieAuth",
            //  "domain": "",
            //  "path": "/",
            //  "ttl": "480"
            //}

            if(authOptions.Type != "authentication") { throw new ArgumentException($"Invalid type '{authOptions.Type}', expecting 'authentication' for auth options."); }
            if(!authOptions.Items.ContainsKey("cookie")) { return; }

            var cookieOptions = authOptions.Items["cookie"];

            Log.Debug("======================================================================");
            Log.Debug("= Cookies");
            Log.Debug("--- Name: '{CookieName}'", cookieOptions["cookie_name"]);
            Log.Debug("--- Path: '{CookiePath}'", cookieOptions["path"]);
            Log.Debug("--- Domain: '{CookieDomain}'", cookieOptions["domain"]);

            // add the cookie 
            authBuilder.AddCookie(CookieAuthenticationDefaults.AuthenticationScheme, options =>
            {
                options.Cookie.Name = cookieOptions["cookie_name"];
                options.Cookie.Path = cookieOptions["path"];
                options.Cookie.Domain = cookieOptions["domain"];

                options.Cookie.HttpOnly = true;
                options.Cookie.SecurePolicy = CookieSecurePolicy.SameAsRequest;     //TODO: change to Canvas driven
                options.Cookie.SameSite = SameSiteMode.Lax;                         //TODO: change to Canvas driven

                options.LogoutPath = authOptions["logout_url"];
                options.LoginPath = authOptions["login_url"];
                options.AccessDeniedPath = authOptions["access_denied_url"];

                // ReturnUrlParameter requires 
                options.ReturnUrlParameter = authOptions["return_url_parameter"] ?? "redirect_uri";
                options.ExpireTimeSpan = TimeSpan.Parse(authOptions["expiration_timespan"] ?? "01:00");
                options.SlidingExpiration = authOptions.Tags.GetValue<bool>("sliding_expiration", false);
            });

            services.AddAntiforgery(options => {
                options.Cookie.SameSite = SameSiteMode.Lax;                 //TODO: change to Canvas driven 
            });
        }

        #region --- Old Canvas ---

        //public static AuthenticationBuilder AddCanvasAuth(this IServiceCollection services, IConfiguration configuration)
        //{
        //    // require jwt section
        //    if (App.Service.ServiceCanvas?.Auth == null) { throw new ArgumentException("Canvas Auth must be defined."); }
        //    //if (App.Service.Canvas.Auth?.Jwt == null) { throw new ArgumentException("JWT must be included to add auth security."); }

        //    var auth = App.Service.ServiceCanvas.Auth;
        //    if (auth.OpenId != null && auth.Cookie == null) { throw new ArgumentException("OpenID must include a Cookie definition."); }

        //    // ================================================================================
        //    // AUTHENTICATION
        //    JwtSecurityTokenHandler.DefaultMapInboundClaims = false;

        //    var azureAdSection = configuration.GetSection("AzureAD");

        //    AuthenticationBuilder authBuilder;

        //    if ((auth.OpenId != null || azureAdSection.Exists()) && auth.Jwt != null)
        //    {
        //        authBuilder = services.AddAuthentication(options =>
        //        {
        //            options.DefaultAuthenticateScheme = "JWT_OR_OIDC";
        //            options.DefaultChallengeScheme = "JWT_OR_OIDC";
        //            options.DefaultScheme = "JWT_OR_OIDC";
        //        });

        //        authBuilder.AddPolicyScheme("JWT_OR_OIDC", "JWT_OR_OIDC", options =>
        //        {
        //            // runs on each request
        //            options.ForwardDefaultSelector = context =>
        //            {
        //                // filter by auth type
        //                string authorization = $"{context.Request.Headers[Microsoft.Net.Http.Headers.HeaderNames.Authorization]}";
        //                if (!string.IsNullOrEmpty(authorization) && authorization.StartsWith("Bearer "))
        //                {
        //                    return JwtBearerDefaults.AuthenticationScheme;
        //                }
        //                else
        //                {
        //                    // otherwise always check for cookie auth
        //                    return OpenIdConnectDefaults.AuthenticationScheme;
        //                }
        //            };
        //        });
        //    }
        //    else if (auth.OpenId != null)
        //    {
        //        Log.Debug("+ {AppCapability}", "OpenID Authentication (OIDC)");
        //        authBuilder = services.AddAuthentication(options =>
        //        {
        //            options.DefaultScheme = CookieAuthenticationDefaults.AuthenticationScheme;
        //            options.DefaultChallengeScheme = OpenIdConnectDefaults.AuthenticationScheme;
        //        });
        //    }
        //    else// if(auth.Jwt != null)
        //    {
        //        Log.Debug("+ {AppCapability}", "JWT Authentication");
        //        authBuilder = services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme);
        //    }

        //    // ====================================================================================================
        //    // COOKIE
        //    // ====================================================================================================            
        //    if (auth.Cookie != null)
        //    {
        //        Log.Debug("======================================================================");
        //        Log.Debug("= Cookies");
        //        Log.Debug("--- Name: '{CookieName}'", auth.Cookie.Name);
        //        Log.Debug("--- Path: '{CookiePath}'", auth.Cookie.Path);
        //        Log.Debug("--- Domain: '{CookieDomain}'", auth.Cookie.Domain);

        //        // add the cookie 
        //        authBuilder.AddCookie(CookieAuthenticationDefaults.AuthenticationScheme, options =>
        //        {
        //            options.Cookie.Name = auth.Cookie.Name;
        //            options.Cookie.Path = auth.Cookie.Path;
        //            options.Cookie.Domain = auth.Cookie.Domain;

        //            options.Cookie.HttpOnly = true;
        //            options.Cookie.SecurePolicy = CookieSecurePolicy.SameAsRequest;     //TODO: change to Canvas driven
        //            options.Cookie.SameSite = SameSiteMode.Lax;                         //TODO: change to Canvas driven

        //            options.LogoutPath = auth.LogoutUrl;
        //            options.LoginPath = auth.LogoutUrl;
        //            options.AccessDeniedPath = auth.AccessDeniedUrl;

        //            // ReturnUrlParameter requires 
        //            options.ReturnUrlParameter = auth.ReturnUrlParameter;
        //            options.ExpireTimeSpan = auth.ExpireTimeSpan;
        //            options.SlidingExpiration = auth.SlidingExpiration;
        //        });

        //        services.AddAntiforgery(options =>
        //        {
        //            options.Cookie.SameSite = SameSiteMode.Lax;                 //TODO: change to Canvas driven 
        //        });
        //    }

        //    // ====================================================================================================
        //    // JWT Config
        //    // ====================================================================================================
        //    if (auth.Jwt != null)
        //    {
        //        services.AddCanvasAuthJwt(authBuilder);
        //    }

        //    // ====================================================================================================
        //    // OPEN ID CONNECT Config
        //    // ====================================================================================================
        //    if (auth.OpenId != null)
        //    {
        //        services.AddCanvasAuthOpenId(auth.OpenId, authBuilder);
        //    }

        //    // ====================================================================================================
        //    // Azure AD integration?
        //    // ====================================================================================================            
        //    if (azureAdSection.Exists())
        //    {
        //        Log.Debug("+ {AppCapability}", "Microsoft Identity");
        //        authBuilder.AddMicrosoftIdentityWebApp(azureAdSection);

        //        services.PostConfigure<OpenIdConnectOptions>(OpenIdConnectDefaults.AuthenticationScheme, options =>
        //        {
        //            // The claim in the Jwt token where App roles are available.
        //            options.TokenValidationParameters.NameClaimType = "name";
        //            options.TokenValidationParameters.RoleClaimType = "roles";
        //        });
        //    }

        //    // ====================================================================================================            
        //    // AUTHORIZATION
        //    // ====================================================================================================            
        //    Log.Debug("======================================================================");
        //    Log.Debug("Service Authentication");
        //    Log.Debug("- App Roles:");
        //    auth.AppRoles.ForEach(role =>
        //    {
        //        Log.Debug("--- {AppRole}", role);
        //    });

        //    services.AddAuthorization(options =>
        //    {
        //        // add all the role -> claim matches            
        //        Log.Debug("- Policies:");
        //        auth.PolicyClaimsMap.ForEach(map =>
        //        {
        //            Log.Debug("--- Policy: {AppPolicy}, Roles: {AppPolicyRoles}", map.Key, string.Join(", ", map.Value));

        //            // Make sure all the roles specified are in the expected app roles listing
        //            foreach (var role in map.Value)
        //            {
        //                if (!auth.AppRoles.Contains(role)) { throw new ArgumentException($"Policy Claims Map in Canvas:Auth references missing role '{role}'"); }
        //            }
        //            options.AddPolicy(map.Key, policy => policy.RequireClaim("role", map.Value));
        //        });

        //        // ================================================================================
        //        // REQUIRE EVERYONE TO LOGIN                
        //        // By default, all incoming requests will be authorized according to the default policy.
        //        options.FallbackPolicy = new AuthorizationPolicyBuilder().RequireAuthenticatedUser().Build(); //options.DefaultPolicy;
        //    });

        //    return authBuilder;
        //}

        //public static void AddCanvasAuthJwt(this IServiceCollection services, AuthenticationBuilder? authBuilder)
        //{
        //    if (App.Service?.ServiceCanvas?.Auth?.Jwt == null) { throw new ArgumentException("Service Canvas Auth JWT Not Defined."); }

        //    var authJwt = App.Service.ServiceCanvas.Auth.Jwt;

        //    if (authBuilder == null)
        //    {
        //        Log.Debug("+ {AppCapability}", "JWT Authentication");
        //        authBuilder = services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme);
        //    }

        //    // make sure higher environments don't have disabled validation
        //    if (authJwt.DisableValidation && App.Service.IsProduction())
        //    {
        //        throw new ArgumentException("JWT Disable Validation not allowed in production environments!");
        //    }

        //    Log.Debug("======================================================================");
        //    Log.Debug("= JWT Configuration:");
        //    if (!authJwt.DisableValidation)
        //    {
        //        Log.Debug("--- Issuer: {JwtIssuer}", (!string.IsNullOrEmpty(authJwt.Issuer) ? authJwt.Issuer : "<< ANY >>"));
        //        Log.Debug("--- Audience: {JwtAudience}", (!string.IsNullOrEmpty(authJwt.Audience) ? authJwt.Audience : "<< ANY >>"));
        //    }
        //    else
        //    {
        //        Log.Warning("--- VALIDATION DISABLED!");
        //    }

        //    // this fires later
        //    authBuilder.AddJwtBearer(JwtBearerDefaults.AuthenticationScheme, options =>
        //    {
        //        options.Authority = authJwt.Authority!.OriginalString;
        //        options.Audience = (!string.IsNullOrEmpty(authJwt.Audience) ? authJwt.Audience : null);

        //        if (!authJwt.DisableValidation)
        //        {
        //            //options.TokenValidationParameters = App.Service.OAuth2Client!.GetTokenValidationParametersAsync().Result;
        //            options.TokenValidationParameters = new TokenValidationParameters
        //            {
        //                NameClaimType = authJwt.NameClaimType,
        //                RoleClaimType = authJwt.RoleClaimType
        //            };
        //        }
        //        else
        //        {
        //            // since this config will prevent a request even getting to the service layers we need a auth that only makes sure a token was provided.. we can do token validation checks later
        //            options.TokenValidationParameters = new TokenValidationParameters
        //            {
        //                ValidateIssuer = false,
        //                ValidateAudience = false,
        //                RequireAudience = false,

        //                ValidateLifetime = false,
        //                RequireSignedTokens = false,
        //                ValidateIssuerSigningKey = false,

        //                // fool the validation logic since we aren't validating tokens
        //                SignatureValidator = delegate (string token, TokenValidationParameters parameters)
        //                {
        //                    return new JsonWebToken(token);
        //                },

        //                NameClaimType = authJwt.NameClaimType,
        //                RoleClaimType = authJwt.RoleClaimType
        //            };
        //        }
        //    });
        //}

        //public static void AddCanvasAuthOpenId(this IServiceCollection services, CanvasAuthOpenId authOpenId, AuthenticationBuilder? authBuilder)
        //{
        //    if (string.IsNullOrEmpty(authOpenId.ClientId)) { throw new ArgumentException("Missing 'Auth:Openid:ClientId' configuration setting."); }
        //    if (string.IsNullOrEmpty(authOpenId.ClientSecret)) { throw new ArgumentException("Missing 'Auth:Openid:ClientSecret' configuration setting."); }
        //    if (authOpenId.Authority == null) { throw new ArgumentException("Missing 'Auth:Openid:Authority' configuration setting."); }

        //    if (!authOpenId.Scope.Any()) { throw new ArgumentException("Auth Missing OpenId Scopes configuration setting."); }

        //    Log.Debug("======================================================================");
        //    Log.Debug("= OpenID Connect:");
        //    Log.Debug("--- Authority: {OpenIdAuthority}", authOpenId.Authority);
        //    Log.Debug("--- ClientID: {OpenIdClientId}", authOpenId.ClientId);
        //    Log.Debug("--- Scope: {OpenIdScope}", string.Join(", ", authOpenId.Scope));

        //    if (authBuilder == null)
        //    {
        //        Log.Debug("+ {AppCapability}", "OpenID Authentication (OIDC)");
        //        authBuilder = services.AddAuthentication(OpenIdConnectDefaults.AuthenticationScheme);
        //    }

        //    authBuilder.AddOpenIdConnect(OpenIdConnectDefaults.AuthenticationScheme, options =>
        //    {
        //        options.Authority = authOpenId.Authority.OriginalString;
        //        options.ClientId = authOpenId.ClientId;
        //        options.ClientSecret = authOpenId.ClientSecret;
        //        options.CallbackPath = !string.IsNullOrEmpty(authOpenId.CallbackUrl) ? authOpenId.CallbackUrl : "/signin-oidc";

        //        options.Scope.Clear();
        //        authOpenId.Scope.ForEach(scope => options.Scope.Add(scope));

        //        options.SignInScheme = CookieAuthenticationDefaults.AuthenticationScheme;
        //        options.SignOutScheme = CookieAuthenticationDefaults.AuthenticationScheme;
        //        options.ResponseType = OpenIdConnectResponseType.Code;
        //        options.ResponseMode = OpenIdConnectResponseMode.Query;
        //        options.GetClaimsFromUserInfoEndpoint = authOpenId.UseUserInfoForClaims;
        //        options.UsePkce = authOpenId.UsePkce;
        //        options.SaveTokens = true;

        //        options.TokenValidationParameters = new TokenValidationParameters
        //        {
        //            ValidateIssuer = true,
        //            ValidateIssuerSigningKey = true,
        //            ValidateAudience = true,
        //            ValidateLifetime = true,

        //            NameClaimType = "name",
        //            RoleClaimType = "roles"
        //        };

        //        //Additional config snipped
        //        //options.Events = new OpenIdConnectEvents
        //        //{
        //        //    OnTokenValidated = async context =>
        //        //    {
        //        //        //var identity = context.Principal.Identity as ClaimsIdentity;

        //        //        //if (identity != null && IsEmployee()
        //        //        //{
        //        //        //    identity.AddClaim(new Claim("roles", "Employee"));
        //        //        //}
        //        //    }
        //        //};
        //    });
        //}

        #endregion


        /// <summary>
        /// Use Auth Security
        /// </summary>
        /// <param name="app"><see cref="IApplicationBuilder"/></param>        
        /// <returns><see cref="IApplicationBuilder"/> for chaining calls</returns>
        /// <remarks>Configures App with: UseAuthentication, UseAuthorization</remarks>
        public static IApplicationBuilder UseCanvasAuthSecurity(this IApplicationBuilder app)
        {            
            Log.Debug("Configuring Canvas Auth Security...");

            app.UseAuthentication();            
            app.UseAuthorization();

            return app;
        }
    }
}
