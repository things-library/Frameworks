// ================================================================================
// <copyright file="DatabaseHealthCheck.cs" company="Starlight Software Co">
//    Copyright (c) 2025 Starlight Software Co. All rights reserved.
//    Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
// </copyright>
// ================================================================================

using Microsoft.Extensions.Diagnostics.HealthChecks;

namespace ThingsLibrary.Entity.HealthChecks
{
    /// <summary>
    /// Configuration Variables: HealthCheck.Database.Response
    /// </summary>
    public class DatabaseHealthCheck : IHealthCheck
    {
        private DbContext DataContext { get; set; }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="context">Data Context</param>
        public DatabaseHealthCheck(DbContext context)
        {
            this.DataContext = context;
        }

        /// <summary>
        /// Check the health
        /// </summary>
        /// <param name="context">Health Check Context</param>
        /// <param name="cancellationToken">Cancel Token</param>
        /// <returns>Health Status</returns>
        public async Task<HealthCheckResult> CheckHealthAsync(HealthCheckContext context, CancellationToken cancellationToken = default)
        {
            try
            {
                // Perform a simple query to check database connectivity
                if (!await this.DataContext.Database.CanConnectAsync(cancellationToken))
                {
                    return HealthCheckResult.Unhealthy("Database is not reachable.");
                }

                return HealthCheckResult.Healthy("Database is reachable.");
            }
            catch (Exception ex)
            {
                return HealthCheckResult.Unhealthy("Database check failed.", ex);
            }
        }
    }
}
