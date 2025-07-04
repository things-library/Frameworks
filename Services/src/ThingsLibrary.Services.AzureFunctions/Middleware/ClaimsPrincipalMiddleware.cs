// ================================================================================
// <copyright file="ClaimsPrincipalMiddleware.cs" company="Starlight Software Co">
//    Copyright (c) Starlight Software Co. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
// </copyright>
// ================================================================================

//using System.IdentityModel.Tokens.Jwt;
//using System.Security.Claims;

//using Microsoft.Azure.Functions.Worker.Middleware;
//using Microsoft.Extensions.DependencyInjection;

//using ThingsLibrary.Security.OAuth2;
//using ThingsLibrary.Services.Canvas;

//namespace ThingsLibrary.Services.AzureFunctions.Middleware
//{
//    public class ClaimsPrincipalMiddleware : IFunctionsWorkerMiddleware
//    {
//        /// <summary>
//        /// OAuth2 Client
//        /// </summary>
//        public OAuth2Client? OAuth2Client { get; set; }

//        public ClaimsPrincipalMiddleware(CanvasAuthJwt options)
//        {
//            if (options == null) { throw new ArgumentNullException(nameof(options)); }

//            this.OAuth2Client = new OAuth2Client
//            {
//                Authority = options.Authority,
//                Issuer = options.Issuer,
//                Audience = options.Audience,

//                ClientId = options.ClientId,
//                ClientSecret = options.ClientSecret,

//                RoleClaimType = options.RoleClaimType ?? "roles",
//                NameClaimType = options.NameClaimType ?? "name",

//                ShowValidationErrors = options.ShowValidationErrors,
//                DisableValidation = options.DisableValidation
//            };

//            // fetch the configuration before we actually need to use it
//            if (this.OAuth2Client.OpenIdConfigUri != null)
//            {
//                _ = Task.Run(() => this.OAuth2Client.FetchOpenIdDefinitionsAsync());
//            }
//        }

//        //public ClaimsPrincipalMiddleware(CanvasAuth canvasAuth)
//        //{
//        //    if (canvasAuth == null) { throw new ArgumentException("Service Canvas Auth Section not initialized for DI."); }
//        //    //if (App.Service.Canvas.Auth == null) { throw new ArgumentNullException(nameof(App.Service.Canvas.Auth)); }

//        //    if (canvasAuth.Jwt != null)
//        //    {
//        //        this.OAuth2Client = new OAuth2Client
//        //        {
//        //            OpenIdConfigUri = canvasAuth.Jwt.OpenIdConfigUri,

//        //            Issuer = canvasAuth.Jwt.Issuer,
//        //            Audience = canvasAuth.Jwt.Audience,

//        //            ClientId = canvasAuth.Jwt.ClientId,
//        //            ClientSecret = canvasAuth.Jwt.ClientSecret,

//        //            RoleClaimType = canvasAuth.Jwt.RoleClaimType ?? "roles",
//        //            NameClaimType = canvasAuth.Jwt.NameClaimType ?? "name",

//        //            ShowValidationErrors = canvasAuth.Jwt.ShowValidationErrors,
//        //            DisableValidation = canvasAuth.Jwt.DisableValidation
//        //        };

//        //        // fetch the configuration before we actually need to use it
//        //        if (this.OAuth2Client.OpenIdConfigUri != null)
//        //        {
//        //            _ = Task.Run(() => this.OAuth2Client.FetchOpenIdDefinitionsAsync());
//        //        }
//        //    }
//        //    else
//        //    {
//        //        Log.Warning("No JWT Configuration.");
//        //    }
//        //}

//        public async Task Invoke(FunctionContext context, FunctionExecutionDelegate next)
//        {
//            var request = await context.GetHttpRequestDataAsync();

//            // only http requests will have a object
//            if (request != null)
//            {
//                var authenticationResponse = this.AuthenticateRequest(request);
//                if (authenticationResponse.Data == null)
//                {
//                    var response = request!.CreateResponse();
//                    await response.WriteAsJsonAsync(authenticationResponse, authenticationResponse.StatusCode);

//                    context.GetInvocationResult().Value = response;
//                    return; // end of the road
//                }

//                var claimsPrincipal = authenticationResponse.Data;


//                //TODO: check Roles/Policy enrollment

//                var accessor = context.InstanceServices.GetRequiredService<ThingsLibrary.Security.OAuth2.Interfaces.IClaimsPrincipalAccessor>();
//                accessor.Principal = claimsPrincipal;
//                if(authenticationResponse?.Errors?.Any() == true)
//                {
//                    accessor.ValidationErrors.AddRange(authenticationResponse.Errors);
//                }                
//            }

//            await next(context);
//        }

//        private JsonResponse<ClaimsPrincipal?> AuthenticateRequest(HttpRequestData request)
//        {
//            // X-MS-CLIENT-PRINCIPAL-ID --User ID
//            // X-MS-CLIENT-PRINCIPAL-NAME --User Name
//            // X-MS-CLIENT-PRINCIPAL-IDP --Identity Provider's ID
//            // X-MS-CLIENT-PRINCIPAL --Claims

//            if (request.Headers.Contains("x-ms-client-principal"))
//            {
//                string? clientPrincipalHeader = request.Headers.GetValues("x-ms-client-principal").FirstOrDefault();

//                //this.Logger.LogDebug("API Gateway Authenticated.  Parsing header token...");
//                return this.AuthenticateByClientPrincipalHeader(clientPrincipalHeader).Result;
//            }
//            else if (request.Headers.Contains("Authorization"))
//            {
//                // check the authorization header                
//                string? authorizationHeader = request.Headers.GetValues("Authorization").FirstOrDefault();

//                // get pretty reasons for the validation failures
//                //this.Logger.LogDebug("Authorization Header Provided. Validating...");
//                return this.AuthenticateByAuthorizationHeader(authorizationHeader).Result;
//            }
//            else
//            {
//                return new JsonResponse<ClaimsPrincipal?>(HttpStatusCode.Unauthorized, "Unauthorized.", "No authorization header provided.");
//            }            
//        }

//        /// <summary>
//        /// Authenticates and returns the claims principal object
//        /// </summary>
//        /// <param name="clientPrincipalHeader">Claims principal stored in the header x-ms-client-principal</param>
//        /// <returns></returns>
//        private async Task<JsonResponse<ClaimsPrincipal?>> AuthenticateByClientPrincipalHeader(string? clientPrincipalHeader)
//        {
//            // X-MS-CLIENT-PRINCIPAL-ID --User ID
//            // X-MS-CLIENT-PRINCIPAL-NAME --User Name
//            // X-MS-CLIENT-PRINCIPAL-IDP --Identity Provider's ID
//            // X-MS-CLIENT-PRINCIPAL --Claims

//            if (string.IsNullOrEmpty(clientPrincipalHeader)) { return new JsonResponse<ClaimsPrincipal?>(HttpStatusCode.Unauthorized, "Unauthorized, no header principal provided."); }
//            if (this.OAuth2Client == null) { return new JsonResponse<ClaimsPrincipal?>(HttpStatusCode.BadRequest, "No OAuth2 Client defined to authorize against."); }

//            var claimsPrincipal = ClaimsPrincipalParser.Parse(clientPrincipalHeader);

//            // see if we should generate pretty validation errors
//            Dictionary<string, string>? validationResults = new Dictionary<string, string>();
//            if (this.OAuth2Client.ShowValidationErrors)
//            {
//                var jwtToken = new JwtSecurityToken(claims: claimsPrincipal.Claims);

//                // just in case it is a Bearer token                
//                validationResults = await this.OAuth2Client.ValidateJwtAsync(jwtToken);
//            }

//            return new JsonResponse<ClaimsPrincipal?>(validationResults, HttpStatusCode.Unauthorized)
//            {
//                Data = claimsPrincipal
//            };

//        }

//        /// <summary>
//        /// Authenticates base on the authorization header Bearer token
//        /// </summary>
//        /// <param name="authorizationHeader"></param>
//        /// <returns></returns>
//        private async Task<JsonResponse<ClaimsPrincipal?>> AuthenticateByAuthorizationHeader(string? authorizationHeader)
//        {
//            if (string.IsNullOrEmpty(authorizationHeader)) { return new JsonResponse<ClaimsPrincipal?>(HttpStatusCode.Unauthorized, "Unauthorized, no authorization header provided."); }
//            if (this.OAuth2Client == null) { return new JsonResponse<ClaimsPrincipal?>(HttpStatusCode.BadRequest, "No OAuth2 Client defined to authorize against."); }

//            var claimsPrincipal = await this.OAuth2Client.ParseAndValidateAsync(authorizationHeader);

//            // see if we should generate pretty validation errors
//            Dictionary<string, string>? validationResults = null;
//            if (this.OAuth2Client.ShowValidationErrors)
//            {
//                // just in case it is a Bearer token                
//                validationResults = await this.OAuth2Client.ValidateJwtAsync(authorizationHeader);
//            }

//            return new JsonResponse<ClaimsPrincipal?>(validationResults, HttpStatusCode.Unauthorized)
//            {
//                Data = claimsPrincipal
//            };
//        }
//    }
//}
