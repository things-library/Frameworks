// ================================================================================
// <copyright file="Startup.cs" company="Starlight Software Co">
//    Copyright (c) 2025 Starlight Software Co. All rights reserved.
//    Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
// </copyright>
// ================================================================================

using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration.Json;
using Microsoft.Extensions.Diagnostics.HealthChecks;

using Serilog;
using Serilog.Events;

using ThingsLibrary.Services.AspNetCore.HealthChecks;

namespace ThingsLibrary.Services.AspNetCore
{
    /// <summary>
    /// Standarized start up process that auto configures all of the cross cutting concerns by leveraging the ServiceCanvas strongly typed settings.
    /// 
    /// The core configure sections can be overridden to override the base configuration or add to.
    /// </summary>
    public abstract class Startup
    {        
        /// <summary>
        /// Web Application Builder
        /// </summary>
        private WebApplicationBuilder Builder { get; init; }

        /// <summary>
        /// App Duration Stopwatch
        /// </summary>
        public Stopwatch AppWatch { get; init; } = Stopwatch.StartNew();
                
        public ItemDto Canvas { get; private set; } = new();
        
        protected Startup()
        {
            // so we always have a logger running from as soon as possible
            this.InitBootstrapLogger();
            
            // show something ASAP so we know processing has started
            Log.Information("Startup()...");
            Log.Information("======================================================================");
            Log.Information(" {AppName} @ ({AppStartOn})", App.Service.Assembly.Name(), DateTime.UtcNow.ToString("o"));
            Log.Information("======================================================================");

            // build the web application
            Log.Debug("Initializing HostBuilder...");
            this.Builder = WebApplication.CreateBuilder();
        }

        public abstract void ConfigureHostBuilder();

        /// <summary>
        /// Run/launch web application
        /// </summary>
        public async Task RunAsync()
        {
            this.ConfigureHostBuilder();

            // Override the enviroment settings?
            var environmentName = Environment.GetEnvironmentVariable("DOTNET_ENVIRONMENT");
            if (!string.IsNullOrEmpty(environmentName)) { this.Builder.Host.UseEnvironment(environmentName); }

            // configure servides
            this.Builder.Host.ConfigureServices((context, services) =>
            {
                services.AddSeriLogging(context.Configuration);
                
                // Register the service canvas singletons and as a static instance
                //this.ServiceCanvas = services.AddServiceCanvas(context.Configuration);                
                this.Canvas = services.AddCanvas(context.Configuration);
                                
                Log.Debug("======================================================================");
                Log.Information(" CONFIGURE SERVICES");
                Log.Debug("======================================================================");
                this.ConfigureServices(context, services);
                                
                Log.Debug("======================================================================");
                Log.Debug(" CONFIGURE Health SERVICES...");
                Log.Debug("======================================================================");
                Log.Debug("+ Health Check Service...");
                this.ConfigureHealthServices(context, services);
            });

            Log.Debug("");
            Log.Debug("======================================================================");
            Log.Information("BUILD");
            Log.Debug("======================================================================");
            var app = this.Builder.Build();
            
            Log.Debug("");
            Log.Debug("======================================================================");
            Log.Information("CONFIGURE");
            Log.Debug("======================================================================");
            this.ConfigureApp(app);
            
            Log.Debug("");
            Log.Debug("======================================================================");
            Log.Information("PRE-LAUNCH CHECKS");
            Log.Debug("======================================================================");            
            this.PreChecks(app.Services);

            // okay, we are good to go if we made it here
            Log.Information("App Ready!");
            App.Service.IsReady = true;

            Log.Debug("");
            Log.Debug("======================================================================");
            Log.Information("Launching @ {AppStartOn} (Dur:{StartupDuration})...", DateTime.UtcNow.ToString("o"), DateTimeOffset.UtcNow.Subtract(App.Service.StartedOn).ToClock());
            Log.Debug("======================================================================");
            await app.RunAsync(App.Service.CancellationToken);
            
            // APPLICATION ENDED!!!                                
            App.Service.IsReady = false;

            // APPLICATION ENDED!!!
            Log.Debug("");
            Log.Debug("======================================================================");
            Log.Information(" APPLICATION ENDED @ {AppEndTime}, (Dur:{AppDuration})", DateTime.UtcNow.ToString("o"), DateTimeOffset.UtcNow.Subtract(App.Service.StartedOn).ToClock());
            Log.Debug("======================================================================");

            // flush log
            await Log.CloseAndFlushAsync();
        }

        /// <summary>
        /// Configure Services
        /// </summary>
        /// <param name="context">Host Builder Context</param>
        /// <param name="services">Service Collection</param>
        public abstract void ConfigureServices(HostBuilderContext context, IServiceCollection services);

        /// <summary>
        /// Configure middleware and other app services
        /// </summary>
        public abstract void ConfigureApp(WebApplication app);
                
        /// <summary>
        /// Do all pre-launch checks
        /// </summary>
        /// <returns>Startup for chaining commands</returns>
        /// <remarks>
        /// Base Checks:
        /// - Health Checks ('live' probe)
        /// - Database Migrations Check (if Db dependency)
        /// </remarks>
        public virtual void PreChecks(IServiceProvider services)
        {
            services.CheckHealth();
        }


        #region --- Health Checks ---

        /// <summary>
        /// Configures Healthchecks (startup, live, ready)
        /// </summary>
        /// <param name="context">Host Builder Context</param>
        /// <param name="services">Service Collection</param>
        private void ConfigureHealthServices(HostBuilderContext context, IServiceCollection services)
        {
            var healthCheckBuilder = services.AddHealthChecks();

            // self check (always return healthy when no other health checks are added)            
            Log.Debug("+ Health Checks: startup, live, ready...");
            healthCheckBuilder.AddCheck("self", () => HealthCheckResult.Healthy(), tags: new[] { "startup", "live", "ready" });

            // invoke the main configure health 
            this.ConfigureHealthServices(context, services, healthCheckBuilder);
        }

        /// <summary>
        /// Configure Health Services
        /// </summary>
        /// <param name="context"></param>
        /// <param name="services"></param>
        public abstract void ConfigureHealthServices(HostBuilderContext context, IServiceCollection services, IHealthChecksBuilder healthCheckBuilder);

        #endregion

        #region --- Logger --- 

        /// <summary>
        /// Initialize an initial logger so we have something from the start
        /// </summary>
        protected void InitBootstrapLogger()
        {
            // Set up logging ASAP
            Log.Logger = new LoggerConfiguration()
                .MinimumLevel.Debug()
                .MinimumLevel.Override("Microsoft", LogEventLevel.Information)
                .Enrich.FromLogContext()
                .WriteTo.Console()
                .CreateBootstrapLogger();
        }

        /// <summary>
        /// Logs the configuration sources/providers
        /// </summary>
        /// <param name="configurationBuilder">Configuration Builder</param>
        protected void LogConfigurationSources(IConfigurationBuilder configurationBuilder)
        {
            Log.Information("Configuration Sources:");
            foreach (var source in configurationBuilder.Sources)
            {
                if (source is JsonConfigurationSource jsonSource)
                {
                    Log.Information("= {AppConfigSource} (Path: {AppConfigSourcePath})", source, jsonSource.Path);
                }
                else
                {
                    Log.Information("= {AppConfigSource}", source);
                }
            }
        }

        #endregion

    }
}
