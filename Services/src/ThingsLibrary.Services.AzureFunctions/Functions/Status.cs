// ================================================================================
// <copyright file="Status.cs" company="Starlight Software Co">
//    Copyright (c) Starlight Software Co. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
// </copyright>
// ================================================================================

using Microsoft.Extensions.Diagnostics.HealthChecks;
using ThingsLibrary.DataType;
using ThingsLibrary.Security.OAuth2.Services;

namespace ThingsLibrary.Services.AzureFunctions.Functions
{
    /// <summary>
    /// These are a set of troubleshooting status endpoints that help troubleshoot common issues like auth 
    /// </summary>
    public class Status : Base.BaseUserFunctions
    {    
        public Status(IClaimsPrincipalAccessor claimsPrincipalAccessor) : base(claimsPrincipalAccessor)
        {        
            //nothing
        }

        /// <summary>
        /// Simple echo response of the client headers since gateways can add additional headers like x-ms-clientprincipal
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [Function(nameof(Status_Headers))]
        public async Task<HttpResponseData> Status_Headers([HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "status/headers")] HttpRequestData request, FunctionContext context)
        {   
            var response = request.CreateResponse();

            var headers = request.Headers.ToDictionary(x => x.Key, x => string.Join(", ", x.Value));

            await response.WriteAsJsonAsync(new ActionResponse<Dictionary<string, string>>(data: headers), context.CancellationToken);

            return response;
        }

        ///// <summary>
        ///// Get the instance details
        ///// </summary>
        ///// <param name="request"></param>
        ///// <param name="context"></param>
        ///// <returns>Instance details</returns>    
        //[Function(nameof(Status_Instance))]
        //public async Task<HttpResponseData> Status_Instance([HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "status/instance")] HttpRequestData request, FunctionContext context)
        //{        
        //    var response = App.Service.GetInstanceStatus();
        
        //    return await request.CreateResponseAsync<ServiceStatusDto>(new JsonResponse<ServiceStatusDto>(response), context.CancellationToken);        
        //}

        /// <summary>
        /// Generate a health check report
        /// </summary>
        /// <param name="request"></param>
        /// <param name="context"></param>
        /// <returns></returns>
        [Function(nameof(Status_HealthCheck))]
        public async Task<HttpResponseData> Status_HealthCheck([HttpTrigger(AuthorizationLevel.System, "get", Route = "status/health")] HttpRequestData request, FunctionContext context)
        {
            var healthCheckService = context.InstanceServices.GetRequiredService<HealthCheckService>();

            //var healthReport = await healthCheckService.CheckHealthAsync
            //(
            //    (check) => check.Tags.Contains("ready"), 
            //    context.CancellationToken
            //);

            var healthReport = await healthCheckService.CheckHealthAsync(context.CancellationToken);

            return await request.CreateResponseAsync<HealthReport>(new ActionResponse<HealthReport>(healthReport), context.CancellationToken);
        }
    }

}
