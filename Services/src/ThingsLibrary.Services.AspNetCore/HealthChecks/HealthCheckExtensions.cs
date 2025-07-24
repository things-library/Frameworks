// ================================================================================
// <copyright file="HealthCheckExtensions.cs" company="Starlight Software Co">
//    Copyright (c) Starlight Software Co. All rights reserved.
//    Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
// </copyright>
// ================================================================================

using Microsoft.Extensions.Diagnostics.HealthChecks;
using Serilog;

namespace ThingsLibrary.Services.AspNetCore.HealthChecks
{
    /// <summary>
    /// Canvas Health Check Extensions
    /// </summary>
    public static class HealthChecksExtensions
    {
        #region --- Health Checks (Register) ---

        /// <summary>
        /// Add Startup health check
        /// </summary>
        /// <typeparam name="T">Health check class</typeparam>
        /// <param name="services">Services Collection</param>
        /// <returns>IServicesCollection for chaining calls</returns>
        public static IServiceCollection AddStartupHealthCheck<T>(this IServiceCollection services) where T : class, IHealthCheck
        {
            services.AddHealthChecks().AddCheck<T>(nameof(T), tags: new[] { "startup" });

            return services;
        }

        /// <summary>
        /// Add Liviness health check
        /// </summary>
        /// <typeparam name="T">Health check class</typeparam>
        /// <param name="services">Services Collection</param>
        /// <returns>IServicesCollection for chaining calls</returns>
        public static IServiceCollection AddLiveHealthCheck<T>(this IServiceCollection services) where T : class, IHealthCheck
        {
            services.AddHealthChecks().AddCheck<T>(nameof(T), tags: new[] { "live" });

            return services;
        }

        /// <summary>
        /// Add Ready health check
        /// </summary>
        /// <typeparam name="T">Health check class</typeparam>
        /// <param name="services">Services Collection</param>
        /// <returns>IServicesCollection for chaining calls</returns>
        public static IServiceCollection AddReadyHealthCheck<T>(this IServiceCollection services) where T : class, IHealthCheck
        {
            services.AddHealthChecks().AddCheck<T>(nameof(T), tags: new[] { "ready" });

            return services;
        }

        #endregion

        /// <summary>
        /// Do a health check, if it is degrated do another just to make sure.
        /// </summary>
        /// <exception cref="ApplicationException"></exception>
        public static HealthReport? CheckHealth(this IServiceProvider services)
        {
            // we can only do this if health checks have been registered
            var healthCheck = services.GetService<HealthCheckService>();
            if (healthCheck == null)
            {
                Log.Warning("No Health checks defined.");
                return null;
            }

            Log.Information("Performing Initial 'Startup' Health Checks...");

            // only run the startup checks which at this point should be good to go
            var healthReport = healthCheck.CheckHealthAsync(x => x.Tags.Contains("live")).Result;
            if (healthReport.Status != HealthStatus.Healthy)
            {
                // sometimes the first db connection takes longer to establish and so make sure we get two bad health reports in a row before reporting bad health
                healthReport = healthCheck.CheckHealthAsync(x => x.Tags.Contains("live")).Result;
                if (healthReport.Status != HealthStatus.Healthy) { throw new TimeoutException("Application Unhealthy."); }
            }

            return healthReport;
        }
    }
}
