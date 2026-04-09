// ================================================================================
// <copyright file="Startup.cs" company="Starlight Software Co">
//    Copyright (c) Starlight Software Co. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
// </copyright>
// ================================================================================

using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using System.Text.Json.Serialization;
using System.Text.Json;

namespace ThingsLibrary.Services.AzureFunctions
{
    /// <summary>
    /// Azure Functions specific formal templated startup process
    /// </summary>
    public abstract class Startup : Services.Startup
    {
        /// <summary>
        /// Health Check Builder
        /// </summary>
        public IHealthChecksBuilder? HealthBuilder { get; private set; }
     
        protected Startup() : base()
        {           
                     
                        
        }

        /// <summary>
        /// Run/launch web application
        /// </summary>
        public async Task RunAsync()
        {
            try
            {
                Log.Debug("======================================================================");
                Log.Debug("CONFIGURE APP BUILDER");
                Log.Debug("======================================================================");
                this.ConfigureAppBuilder(this.Builder);

                Log.Debug("======================================================================");
                Log.Debug("CONFIGURE APP");
                Log.Debug("======================================================================");
                this.Builder.ConfigureAppConfiguration((context, config) =>
                {
                    //this.InitLogger(context.Configuration);

                    //this.Builder.UseSerilog(Log.Logger);

                    this.ConfigureAppConfiguration(context, config);
                });

                Log.Debug("======================================================================");
                Log.Debug("CONFIGURE SERVICES");
                Log.Debug("======================================================================");
                this.Builder.ConfigureServices((context, services) =>
                {
                    //services.AddSerilog();

                    // application insights
                    services.AddApplicationInsightsTelemetryWorkerService();
                    services.ConfigureFunctionsApplicationInsights();

                    // Register the service canvas singletons and as a static instance
                    services.AddCanvasServices(context.Configuration);

                    Log.Debug("");
                    Log.Debug("======================================================================");
                    Log.Information(" CONFIGURE SERVICES");
                    Log.Debug("======================================================================");
                    this.ConfigureServices(context, services);

                    Log.Debug("");
                    Log.Debug("======================================================================");
                    Log.Debug(" CONFIGURE Health SERVICES...");
                    Log.Debug("======================================================================");
                    Log.Debug("+ Health Check Service...");
                    this.HealthBuilder = services.AddHealthChecks();
                    this.ConfigureHealthServices();
                });

                Log.Debug("======================================================================");
                Log.Debug("HOST BUILD");
                Log.Debug("======================================================================");
                var host = this.Builder.Build();

                Log.Debug("======================================================================");
                Log.Debug("PRE-LAUNCH CHECKS");
                Log.Debug("======================================================================");
                this.PreChecks();

                // okay, we are good to go if we made it here
                App.Service.IsReady = true;

                Log.Debug("======================================================================");
                Log.Information("Launching @ {AppStartOn} (A:{StartupDuration})...", DateTime.UtcNow.ToString("o"), this.AppWatch.Elapsed.ToClock());
                Log.Debug("======================================================================");
                await host.RunAsync(App.Service.CancellationToken);

                if (App.Service.CancellationToken.IsCancellationRequested)
                {
                    Log.Debug("Cancellation Requested.");
                }

                // APPLICATION ENDED!!!                
                App.Service.IsReady = false;

                // APPLICATION ENDED!!!                
                Log.Debug("======================================================================");
                Log.Information(" APPLICATION ENDED @ {AppEndTime}, (A:{AppDuration})", DateTime.UtcNow.ToString("o"), this.AppWatch.Elapsed.ToClock());
                Log.Debug("======================================================================");

            }
            catch (TaskCanceledException)
            {
                //nothing we expect this.. fix later
                Log.Information("Task cancelled exception.");
            }
            catch (Exception ex)
            {
                Log.Fatal(ex, "Host terminated unexpectedly");
            }
            finally
            {
                Log.CloseAndFlush();
            }
        }
                
        /// <summary>
        /// Configure the health services
        /// </summary>
        public virtual void ConfigureHealthServices()
        {
            //no other health checks specified yet
        }

        /// <summary>
        /// Configure App Builder
        /// </summary>
        /// <param name="builder"></param>
        public virtual void ConfigureAppBuilder(IHostBuilder builder)
        {
            // Azure Function Middleware and Defaults
            this.Builder.ConfigureFunctionsWorkerDefaults((context, builder) => ConfigureFunctionsWorkerDefaults(context, builder));                     
        }


        /// <summary>
        /// Configure IFunctions application elements such as middleware
        /// </summary>
        /// <param name="context"><see cref="HostBuilderContext"/></param>
        /// <param name="builder"><see cref="IFunctionsWorkerApplicationBuilder"/></param>
        public virtual void ConfigureFunctionsWorkerDefaults(HostBuilderContext context, IFunctionsWorkerApplicationBuilder builder)
        {
            builder.Services.Configure<JsonSerializerOptions>(options =>
            {
                options.AllowTrailingCommas = true;
                options.DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull;
                options.PropertyNamingPolicy = JsonNamingPolicy.CamelCase;
                options.PropertyNameCaseInsensitive = true;
            });

            //nothing
        }

        /// <summary>
        /// Configure the app configuration components such as what IConfiguration sources/providers
        /// </summary>
        /// <param name="context"><see cref="HostBuilderContext"/></param>
        /// <param name="configurationBuilder"><see cref="ConfigurationBuilder"/></param>
        /// <remarks>
        /// Adds the following as IConfiguration sources:
        /// <list type="bullet">
        ///     <item><description>appsettings.json</description></item>
        ///     <item><description>appsettings.{environmentName}.json</description></item>    
        ///     <item><description>secrets.json</description></item>    
        /// </list>    
        /// </remarks>
        public virtual void ConfigureAppConfiguration(HostBuilderContext context, IConfigurationBuilder configurationBuilder)
        {            
            configurationBuilder.AddJsonFile(Path.Combine(context.HostingEnvironment.ContentRootPath, $"appsettings.json"), optional: false, reloadOnChange: false);
            configurationBuilder.AddJsonFile(Path.Combine(context.HostingEnvironment.ContentRootPath, $"appsettings.{context.HostingEnvironment.EnvironmentName}.json"), optional: true, reloadOnChange: false);
            configurationBuilder.AddEnvironmentVariables();
            configurationBuilder.AddUserSecrets(System.Reflection.Assembly.GetEntryAssembly()!, true);

            // log the configuration sources
            this.LogConfigurationSources(configurationBuilder);
        }

        ///// <summary>
        ///// Do all pre-launch checks as well as cach loading, etc
        ///// </summary>
        ///// <returns>Startup for chaining commands</returns>
        ///// <remarks>
        ///// Base Checks:
        ///// - Health Checks ('live' probe)
        ///// - Database Migrations Check (if Db dependency)
        ///// </remarks>
        //public override void PreChecks()
        //{
        //    base.PreChecks();
        //}        
    }
}
