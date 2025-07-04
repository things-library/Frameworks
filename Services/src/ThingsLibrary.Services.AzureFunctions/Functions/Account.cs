// ================================================================================
// <copyright file="Account.cs" company="Starlight Software Co">
//    Copyright (c) Starlight Software Co. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
// </copyright>
// ================================================================================

using ThingsLibrary.DataType;

namespace ThingsLibrary.Services.AzureFunctions.Functions
{
    /// <summary>
    /// These are a set of troubleshooting status endpoints that help troubleshoot common issues like auth 
    /// </summary>
    public class Account : Base.BaseUserFunctions
    {
        public Account() : base()
        {
            //nothing
        }

        [Function(nameof(Status_Whoami))]
        public async Task<HttpResponseData> Status_Whoami([HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "account/whoami")] HttpRequestData request, FunctionContext context)
        {
            // validate request (we don't care if validation failed as we check in the whoami)
            if (this.ClaimsPrincipal == null)
            {
                var authResponse = new ActionResponse<WhoamiDto>(HttpStatusCode.Unauthorized, "Unauthorized")
                {
                    ErrorMessage = "Validation errors occured. No claims principal found.",
                    Errors = this.ClaimsPrincipalAccessor.ValidationErrors
                };

                return await request.CreateResponseAsync<WhoamiDto>(authResponse, context.CancellationToken);
            }

            if (this.ClaimsPrincipal.Identity == null)
            {
                var authResponse = new ActionResponse<WhoamiDto>(HttpStatusCode.OK, "Unauthorized")
                {
                    ErrorMessage = "No identities provided.",
                    Errors = this.ClaimsPrincipalAccessor.ValidationErrors
                };

                return await request.CreateResponseAsync<WhoamiDto>(authResponse, context.CancellationToken);
            }

            var whoami = App.Service.GetWhoami(this.ClaimsPrincipal);

            // create response
            return await request.CreateResponseAsync<WhoamiDto>(new ActionResponse<WhoamiDto>(whoami)
            {
                ErrorMessage = (this.ClaimsPrincipalAccessor.ValidationErrors.Any() ? "Validation errors have occured." : null),
                Errors = this.ClaimsPrincipalAccessor.ValidationErrors
            }, context.CancellationToken);
        }

        [Function(nameof(Status_ClaimsPrincipal))]
        public async Task<HttpResponseData> Status_ClaimsPrincipal([HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "account/claims")] HttpRequestData request, FunctionContext context)
        {
            var response = request.CreateResponse();

            var claimsPrincipal = this.ClaimsPrincipal;
            if (claimsPrincipal?.Identity != null)
            {
                await response.WriteAsJsonAsync(new ActionResponse<ClaimsPrincipalDto>(claimsPrincipal.ToDto()), context.CancellationToken);
            }
            else
            {
                Log.Debug("No claims principal identity object found.");

                await response.WriteAsJsonAsync(new ActionResponse("No Claims Principal object."), context.CancellationToken);
            }

            return response;
        }
    }

}
