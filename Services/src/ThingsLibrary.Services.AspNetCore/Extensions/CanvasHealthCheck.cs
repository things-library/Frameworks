// ================================================================================
// <copyright file="CanvasHealthCheck.cs" company="Starlight Software Co">
//    Copyright (c) 2025 Starlight Software Co. All rights reserved.
//    Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
// </copyright>
// ================================================================================

using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Serilog;
using ThingsLibrary.Services.AspNetCore.HealthChecks;

namespace ThingsLibrary.Services.AspNetCore.Extensions
{
    /// <summary>
    /// Canvas Health Check Extensions
    /// </summary>
    public static class CanvasHealthCheckExtensions
    {
        /// <summary>
        /// Adds Canvas Health Checks
        /// </summary>        
        /// <param name="services"><see cref="IServiceCollection"/></param>
        /// <returns><see cref="IServiceCollection"/> for call chaining</returns>
        /// <exception cref="ArgumentNullException">Builder not defined</exception>
        /// <exception cref="ArgumentException">Service Canvas Not Defined</exception>
        public static IServiceCollection AddCanvasHealthChecks(this IServiceCollection services)
        {
            var builder = services.AddHealthChecks();

            // self check (always return healthy when no other health checks are added)            
            Log.Debug("+ Health Checks: startup, live, ready...");
            builder.AddCheck("self", () => HealthCheckResult.Healthy(), tags: new[] { "startup", "live", "ready" });

            return services;
        }

        /// <summary>
        /// Configures canvas health checks
        /// </summary>
        /// <param name="app">Web Application</param>        
        /// <returns>Web Application for chaining calls</returns>
        public static IApplicationBuilder UseCanvasHealthChecks(this IApplicationBuilder app)
        {
            // Static Endpoints:
            //   "LivenessUrl": "/health",
            //   "ReadinessUrl": "/health/ready",
            //   "StartupUrl": "/health/startup"
                        
            Log.Debug("Health Probe Endpoints:");

            // LIVENESS
            Log.Debug("= Live: {AppHealthLive}", "/health");
            app.UseHealthChecks("/health", new HealthCheckOptions
            {
                Predicate = x => x.Tags.Contains("live"),
                ResponseWriter = HealthCheck.Writer
            });

            // READINESS
            Log.Debug("= Ready: {AppHealthReady}", "/health/ready");
            app.UseHealthChecks("/health/ready", new HealthCheckOptions
            {
                Predicate = x => x.Tags.Contains("ready"),
                ResponseWriter = HealthCheck.Writer
            });

            // STARTUP
            Log.Debug("= Startup: {AppHealthStartup}", "/health/startup");
            app.UseHealthChecks("/health/startup", new HealthCheckOptions
            {
                Predicate = x => x.Tags.Contains("startup"),
                ResponseWriter = HealthCheck.Writer,                
            });

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapHealthChecks("/health").AllowAnonymous();
                endpoints.MapHealthChecks("/health/ready").AllowAnonymous();
                endpoints.MapHealthChecks("/health/startup").AllowAnonymous();
            });

            return app;
        }

        #region --- Health Checks ---

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
    }
}
