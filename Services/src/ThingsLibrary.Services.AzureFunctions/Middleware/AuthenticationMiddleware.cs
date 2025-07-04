// ================================================================================
// <copyright file="AuthenticationMiddleware.cs" company="Starlight Software Co">
//    Copyright (c) Starlight Software Co. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
// </copyright>
// ================================================================================

//using System.IdentityModel.Tokens.Jwt;

using System.IdentityModel.Tokens.Jwt;
using Microsoft.Azure.Functions.Worker.Middleware;
using Starlight.Security.OAuth2;
using ThingsLibrary.DataType;
using ThingsLibrary.Security.OAuth2;
using ThingsLibrary.Security.OAuth2.Services;

////using ThingsLibrary.Security.OAuth2;
////using ThingsLibrary.Services.Canvas;

namespace ThingsLibrary.Services.AzureFunctions.Middleware
{
//    /// <summary>
//    /// Authentication (AuthN) middleware validates that the user is who they claim to be.
//    /// </summary>
//    /// <remarks>
//    /// This middleware processes auth headers such as Azure's X-MS-CLIENT-PRINCIPAL or a Bearer authentication header and validates it if required and makes it accessible
//    /// </remarks>
    public class AuthenticationMiddleware : IFunctionsWorkerMiddleware
    {
        ///Example:  https://github.com/jp1482/AzureFunctionsMiddleware/blob/master/SomeBasicSettings/Middleware/AuthorizationMiddleware.cs

        /// <summary>
        /// OAuth2 Client
        /// </summary>
        public OAuth2Client OAuth2Client { get; set; }

        public AuthenticationMiddleware() //CanvasAuthJwt options)
        {
            //if (options == null) { throw new ArgumentNullException(nameof(options)); }

            //this.OAuth2Client = new OAuth2Client
            //{
            //    Authority = options.Authority,
            //    Issuer = options.Issuer,
            //    Audience = options.Audience,

            //    RoleClaimType = options.RoleClaimType ?? "roles",
            //    NameClaimType = options.NameClaimType ?? "name",

            //    ShowValidationErrors = options.ShowValidationErrors,
            //    DisableValidation = options.DisableValidation
            //};

            //// fetch the configuration before we actually need to use it
            //if (this.OAuth2Client.OpenIdConfigUri != null)
            //{
            //    _ = Task.Run(() => this.OAuth2Client.FetchOpenIdDefinitionsAsync());
            //}
        }

        public async Task Invoke(FunctionContext context, FunctionExecutionDelegate next)
        {
            var request = await context.GetHttpRequestDataAsync();

            // only http requests will have a object
            if (request != null)
            {
                var accessor = context.InstanceServices.GetRequiredService<IClaimsPrincipalAccessor>();

                var authenticationResponse = this.AuthenticateRequest(request);

                // if any validation errors then add them to our accessor listing
                if (authenticationResponse.Errors?.Any() == true)
                {
                    accessor.ValidationErrors.AddRange(authenticationResponse.Errors);
                }

                // see if we found and validated a principal.. if so then add it
                if (authenticationResponse.Data != null)
                {
                    accessor.Principal = authenticationResponse.Data;
                }
                else
                {
                    // principal should always have a principal.. just might not be IsAuthenticated
                    accessor.Principal = new ClaimsPrincipal();

                    //var response = request!.CreateResponse();
                    //await response.WriteAsJsonAsync(authenticationResponse, authenticationResponse.StatusCode);

                    //context.GetInvocationResult().Value = response;
                    //return; // end of the road
                }
            }

            await next(context);
        }

        private ActionResponse<ClaimsPrincipal?> AuthenticateRequest(HttpRequestData request)
        {
            // X-MS-CLIENT-PRINCIPAL-ID --User ID
            // X-MS-CLIENT-PRINCIPAL-NAME --User Name
            // X-MS-CLIENT-PRINCIPAL-IDP --Identity Provider's ID
            // X-MS-CLIENT-PRINCIPAL --Claims

            if (request.Headers.Contains("x-ms-client-principal"))
            {
                string? clientPrincipalHeader = request.Headers.GetValues("x-ms-client-principal").FirstOrDefault();

                //this.Logger.LogDebug("API Gateway Authenticated.  Parsing header token...");
                return this.AuthenticateByClientPrincipalHeader(clientPrincipalHeader).Result;
            }
            else if (request.Headers.Contains("Authorization"))
            {
                // check the authorization header                
                string? authorizationHeader = request.Headers.GetValues("Authorization").FirstOrDefault();

                // get pretty reasons for the validation failures
                //this.Logger.LogDebug("Authorization Header Provided. Validating...");
                return this.AuthenticateByAuthorizationHeader(authorizationHeader).Result;
            }
            else
            {
                return new ActionResponse<ClaimsPrincipal?>(HttpStatusCode.Unauthorized, "Unauthorized.", "No authorization header provided.");
            }
        }

        /// <summary>
        /// Authenticates and returns the claims principal object
        /// </summary>
        /// <param name="clientPrincipalHeader">Claims principal stored in the header x-ms-client-principal</param>
        /// <returns></returns>
        private async Task<ActionResponse<ClaimsPrincipal?>> AuthenticateByClientPrincipalHeader(string? clientPrincipalHeader)
        {
            // X-MS-CLIENT-PRINCIPAL-ID --User ID
            // X-MS-CLIENT-PRINCIPAL-NAME --User Name
            // X-MS-CLIENT-PRINCIPAL-IDP --Identity Provider's ID
            // X-MS-CLIENT-PRINCIPAL --Claims

            if (string.IsNullOrEmpty(clientPrincipalHeader)) { return new ActionResponse<ClaimsPrincipal?>(HttpStatusCode.Unauthorized, "Unauthorized, no header principal provided."); }
            if (this.OAuth2Client == null) { return new ActionResponse<ClaimsPrincipal?>(HttpStatusCode.BadRequest, "No OAuth2 Client defined to authorize against."); }

            var claimsPrincipal = ClaimsPrincipalParser.Parse(clientPrincipalHeader);

            // see if we should generate pretty validation errors
            Dictionary<string, string>? validationResults = new Dictionary<string, string>();
            if (this.OAuth2Client.ShowValidationErrors)
            {
                var jwtToken = new JwtSecurityToken(claims: claimsPrincipal.Claims);

                // just in case it is a Bearer token                
                validationResults = await this.OAuth2Client.ValidateJwtAsync(jwtToken);
            }

            return new ActionResponse<ClaimsPrincipal?>(validationResults, HttpStatusCode.Unauthorized)
            {
                Data = claimsPrincipal
            };
        }

        /// <summary>
        /// Authenticates base on the authorization header Bearer token
        /// </summary>
        /// <param name="authorizationHeader"></param>
        /// <returns></returns>
        private async Task<ActionResponse<ClaimsPrincipal?>> AuthenticateByAuthorizationHeader(string? authorizationHeader)
        {
            if (string.IsNullOrEmpty(authorizationHeader)) { return new ActionResponse<ClaimsPrincipal?>(HttpStatusCode.Unauthorized, "Unauthorized, no authorization header provided."); }
            if (this.OAuth2Client == null) { return new ActionResponse<ClaimsPrincipal?>(HttpStatusCode.BadRequest, "No OAuth2 Client defined to authorize against."); }

            var claimsPrincipal = await this.OAuth2Client.ParseAndValidateAsync(authorizationHeader);

            // see if we should generate pretty validation errors
            Dictionary<string, string>? validationResults = null;
            if (this.OAuth2Client.ShowValidationErrors)
            {
                // just in case it is a Bearer token                
                validationResults = await this.OAuth2Client.ValidateJwtAsync(authorizationHeader);
            }

            return new ActionResponse<ClaimsPrincipal?>(validationResults)
            {
                Data = claimsPrincipal,
                StatusCode = HttpStatusCode.Unauthorized
            };
        }
    }
}
