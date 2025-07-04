// ================================================================================
// <copyright file="AuthorizationMiddleware.cs" company="Starlight Software Co">
//    Copyright (c) Starlight Software Co. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
// </copyright>
// ================================================================================

using Microsoft.Azure.Functions.Worker.Middleware;

namespace ThingsLibrary.Services.AzureFunctions.Middleware
{
    /// <summary>
    /// Authorization (AuthZ) determins what the user can see and do.
    /// </summary>
    public class AuthorizationMiddleware : IFunctionsWorkerMiddleware
    {        
        public AuthorizationMiddleware()
        {

        }

        //public ClaimsPrincipalMiddleware(CanvasAuth canvasAuth)
        //{
        //    if (canvasAuth == null) { throw new ArgumentException("Service Canvas Auth Section not initialized for DI."); }
        //    //if (App.Service.Canvas.Auth == null) { throw new ArgumentNullException(nameof(App.Service.Canvas.Auth)); }

        //    if (canvasAuth.Jwt != null)
        //    {
        //        this.OAuth2Client = new OAuth2Client
        //        {
        //            OpenIdConfigUri = canvasAuth.Jwt.OpenIdConfigUri,

        //            Issuer = canvasAuth.Jwt.Issuer,
        //            Audience = canvasAuth.Jwt.Audience,

        //            ClientId = canvasAuth.Jwt.ClientId,
        //            ClientSecret = canvasAuth.Jwt.ClientSecret,

        //            RoleClaimType = canvasAuth.Jwt.RoleClaimType ?? "roles",
        //            NameClaimType = canvasAuth.Jwt.NameClaimType ?? "name",

        //            ShowValidationErrors = canvasAuth.Jwt.ShowValidationErrors,
        //            DisableValidation = canvasAuth.Jwt.DisableValidation
        //        };

        //        // fetch the configuration before we actually need to use it
        //        if (this.OAuth2Client.OpenIdConfigUri != null)
        //        {
        //            _ = Task.Run(() => this.OAuth2Client.FetchOpenIdDefinitionsAsync());
        //        }
        //    }
        //    else
        //    {
        //        Log.Warning("No JWT Configuration.");
        //    }
        //}

        public async Task Invoke(FunctionContext context, FunctionExecutionDelegate next)
        {  

            await next(context);
        }        
    }
}
