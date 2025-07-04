using Microsoft.Extensions.Diagnostics.HealthChecks;
using ThingsLibrary.Services;
using ThingsLibrary.Services.HealthChecks;

namespace Starlight.Services.AspNetCore
{
    /// <summary>
    /// Startup Helper Class
    /// </summary>
    /// <remarks>
    /// Order of Execution:
    /// - Configure Services
    /// - Configure Health Services
    /// - Build App
    /// - Configure App
    /// - Pre-Launch Checks
    /// - Run
    /// </remarks>
    public abstract class Startup : ThingsLibrary.Services.Startup
    {
        /// <summary>
        /// Web Application Builder
        /// </summary>
        public WebApplicationBuilder Builder { get; init; }

        /// <summary>
        /// Configuration
        /// </summary>
        public IConfiguration Configuration => this.Builder.Configuration;

        /// <summary>
        /// Health Check Builder
        /// </summary>
        public IHealthChecksBuilder? HealthBuilder { get; private set; }

        /// <summary>
        /// Web Application
        /// </summary>
        public WebApplication? WebApp { get; private set; }

        public AppService App { get; set; }

        /// <summary>
        /// Startup Constructor
        /// </summary>
        /// <param name="args">Command Line Arguments</param>
        protected Startup(string[] args)
        {
            // so we always have a logger running from as soon as possible
            this.InitBootstrapLogger();

            // show something ASAP so we know processing has started
            Log.Information($"Startup(args[{args.Length}])...");
            Log.Information("======================================================================");
            Log.Information($" {this.App.Assembly.Name()} @ ({DateTime.UtcNow.ToString("o")})");
            Log.Information("======================================================================");

            // build the web application
            Log.Debug("Initializing HostBuilder...");
            this.Builder = WebApplication.CreateBuilder(args);
                        
            // CONFIGURATION SOURCES            
            this.Builder.Configuration.AddUserSecrets(System.Reflection.Assembly.GetEntryAssembly()!, true);

            // if there are command arguments we want to log them
            //this.LogCommandArguments(args);

            // log the configuration sources
            this.LogConfigurationSources(this.Builder.Configuration);
        }

        /// <summary>
        /// Run Background Service
        /// </summary>
        /// <typeparam name="T"><see cref="BackgroundService"/></typeparam>        
        public async Task Run<T>() where T : BackgroundService
        {
            Log.Information("======================================================================");
            Log.Information("+ Hosted Service: {BackgroundService}", typeof(T).FullName);
            this.Builder.Services.AddHostedService<T>();

            await RunAsync();
        }

        /// <summary>
        /// Run/launch web application
        /// </summary>
        public async Task RunAsync()
        {
            try
            {
                // set up the host Ilogger
                this.InitLogger(this.Builder.Configuration);
                //this.Builder.Host.UseSerilog();

                // configure servides
                this.Builder.Host.ConfigureServices((context, services) =>
                {
                    services.AddSerilog();

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

                Log.Debug("");
                Log.Debug("======================================================================");
                Log.Information("BUILD");
                Log.Debug("======================================================================");                
                this.WebApp = this.Builder.Build();
                App.Service.Host = this.WebApp;

                Log.Debug("");
                Log.Debug("======================================================================");
                Log.Information("CONFIGURE");
                Log.Debug("======================================================================");
                //this.WebApp.UseCanvasServices();
                this.ConfigureApp();

                Log.Debug("");
                Log.Debug("======================================================================");
                Log.Information("PRE-LAUNCH CHECKS");
                Log.Debug("======================================================================");
                this.PreChecks();

                // okay, we are good to go if we made it here
                Log.Information("App Ready!");
                App.Service.IsReady = true;

                Log.Debug("");
                Log.Debug("======================================================================");
                Log.Information("Launching @ {AppStartOn} (Dur:{StartupDuration})...", DateTime.UtcNow.ToString("o"), this.AppWatch.Elapsed.ToClock());
                Log.Debug("======================================================================");

                await this.WebApp.RunAsync(App.Service.CancellationToken);

                // APPLICATION ENDED!!!                                
                this.AppWatch.Stop();
                App.Service.IsReady = false;
                
                // APPLICATION ENDED!!!
                Log.Debug("");
                Log.Debug("======================================================================");
                Log.Information(" APPLICATION ENDED @ {AppEndTime}, (Dur:{AppDuration})", DateTime.UtcNow.ToString("o"), this.AppWatch.Elapsed.ToClock());
                Log.Debug("======================================================================");
            }
            catch (TaskCanceledException)
            {
                //nothing we expect this.. fix later
                Log.Warning("Task Cancelled");
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
        /// Configure services
        /// </summary>
        public abstract void ConfigureServices(HostBuilderContext context, IServiceCollection services);


        /// <summary>
        /// Configure the health services
        /// </summary>
        public virtual void ConfigureHealthServices()
        {
            //no other health checks specified yet
        }

        /// <summary>
        /// Configure web application
        /// </summary>
        /// <returns>Startup for chaining commands</returns>
        public abstract void ConfigureApp();

        /// <summary>
        /// Do all pre-launch checks
        /// </summary>
        /// <returns>Startup for chaining commands</returns>
        /// <remarks>
        /// Base Checks:
        /// - Health Checks ('live' probe)
        /// - Database Migrations Check (if Db dependency)
        /// </remarks>
        public virtual void PreChecks()
        {
            this.CheckHealth();


        }

        /// <summary>
        /// Do a health check, if it is degrated do another just to make sure.
        /// </summary>
        /// <exception cref="ApplicationException"></exception>
        public void CheckHealth()
        {
            Log.Debug("Performing Initial 'Startup' Health Checks...");
            var healthCheck = this.WebApp!.Services.GetService<HealthCheckService>();

            if (healthCheck == null)
            {
                Log.Warning("No health check service to check.");
                return;
            }

            // only run the startup checks which at this point should be good to go
            var healthReport = healthCheck.CheckHealthAsync(x => x.Tags.Contains("live")).Result;
            if (healthReport.Status != HealthStatus.Healthy)
            {
                // sometimes the first db connection takes longer to establish and so make sure we get two bad health reports in a row before reporting bad health
                healthReport = healthCheck.CheckHealthAsync(x => x.Tags.Contains("live")).Result;
                if (healthReport.Status != HealthStatus.Healthy) { throw new HealthCheckException("Application Unhealthy."); }
            }
        }
     
    }
}