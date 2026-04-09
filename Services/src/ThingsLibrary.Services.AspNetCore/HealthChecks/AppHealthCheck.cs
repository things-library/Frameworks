// ================================================================================
// <copyright file="AppHealthCheck.cs" company="Starlight Software Co">
//    Copyright (c) 2025 Starlight Software Co. All rights reserved.
//    Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
// </copyright>
// ================================================================================

using Microsoft.Extensions.Diagnostics.HealthChecks;

namespace ThingsLibrary.Services.AspNetCore.HealthChecks
{
    /// <summary>
    /// Configuration Variables:  HealthCheck.App.IsCurrent
    /// </summary>
    public class AppHealthCheck : IHealthCheck
    {
        /// <summary>
        /// Constructor
        /// </summary>        
        public AppHealthCheck()
        {
            //nothing   
        }

        /// <summary>
        /// Check the health
        /// </summary>
        /// <param name="context">Health Check Context</param>
        /// <param name="cancellationToken">Cancel Token</param>
        /// <returns>Health Status</returns>
        public Task<HealthCheckResult> CheckHealthAsync(HealthCheckContext context, CancellationToken cancellationToken = default)
        {
            try
            {
                var description = "Reports degraded status when:";

                // build the payload
                var data = new Dictionary<string, object>()
                {
                    { "file_version", App.Service.Assembly.FileVersionStr() },
                    { "ready", App.Service.IsReady }
                };

                var status = HealthStatus.Healthy;
                if (!App.Service.IsReady)
                {
                    status = HealthStatus.Unhealthy;
                    description = " application not started up";
                }

                return Task.FromResult(new HealthCheckResult(
                    status: status,
                    description: description,
                    exception: null,
                    data: data)
                );
            }
            catch (Exception ex)
            {
                return Task.FromResult(new HealthCheckResult(
                    status: HealthStatus.Unhealthy,
                    description: ex.Message,
                    exception: ex,
                    data: null)
                );
            }
        }       
    }
}