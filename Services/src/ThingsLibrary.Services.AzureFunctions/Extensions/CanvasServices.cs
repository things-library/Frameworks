using Microsoft.Extensions.Configuration;
using System.Diagnostics;
using System.Text.Json;
using System.Text.Json.Serialization;

using ThingsLibrary.Services.Extensions;

namespace ThingsLibrary.Services.AzureFunctions.Extensions
{
    /// <summary>
    /// Canvas Service Extensions
    /// </summary>
    public static class CanvasServicesExtensions
    {
        /// <summary>
        /// Adds Canvas services like health checks, and registers the AppService object
        /// </summary>
        /// <param name="services"><see cref="IServiceCollection"/></param>
        /// <returns><see cref="IServiceCollection"/></returns>
        /// <remarks>
        /// Adds and configures services based on the Service Canvas settings.
        /// <list type="bullet">
        ///     <item><description>App Metrics/Insights</description></item>
        ///     <item><description>Caching</description></item>
        ///     <item><description>Auth Security</description></item>
        ///     <item><description>OpenAPI Docs</description></item>
        ///     <item><description>Http Client Accessor</description></item>
        ///     <item><description>Response Compression</description></item>
        ///     <item><description>Routing</description></item>
        ///     <item><description>MVC</description></item>
        ///     <item><description>Razor Pages</description></item>
        ///     <item><description>Serverside Blazor</description></item>
        ///     <item><description>Controllers</description></item>
        /// </list>
        /// </remarks>
        public static IServiceCollection AddCanvasServices(this IServiceCollection services, IConfiguration configuration)
        {
            //services.AddServiceCanvas(configuration);

            // Application Insights / Logging            
            //services.AddCanvasAppMetrics();

            // Distributed Cache
            //services.AddCanvasCache();

            // required for getting tokens, traceid, etc
            //Log.Debug("+ {AppCapability}", "Http Context Accessor");
            //services.AddHttpContextAccessor();

            // AUTH SECURITY / COOKIES            
            //services.AddCanvasAuthSecurity();
                        
            // add some pretty serialization if debugger is attached
            services.ConfigureJsonSerializerOptions();

            Log.Debug("======================================================================");
            Log.Debug("Capabilities");

            // add distributed cache
            //services.AddCanvasCache(configuration);

            // add any registered endpoints
            //services.AddCanvasApiEndpoints();

            // ======================================================================
            // Other stuff
            // ======================================================================
            var capabilities = App.Service.Canvas.Info.Capabilities;

            //if (capabilities.HealthChecks)
            //{
            //    services.AddCanvasHealthChecks();
            //}

            //// OpenAPI / Documentation
            //if (capabilities.Swagger)
            //{
            //    services.AddCanvasOpenApi();
            //}   

            //if (capabilities.ResponseCompression)
            //{
            //    Log.Debug("+ {AppCapability}", "Response Compression");
            //    services.AddResponseCompression(x => x.EnableForHttps = true);
            //}

            //Log.Debug("+ {AppCapability}", "Routing");
            //services.AddRouting(options => options.LowercaseUrls = true);

            //if (capabilities.Mvc)
            //{
            //    Log.Debug("+ {AppCapability}", "MVC");
            //    services.AddMvc(o => o.EnableEndpointRouting = false);
            //}

            //// Add services to the container.            
            //if (capabilities.RazorPages)
            //{
            //    Log.Debug("+ {AppCapability}", "Razor Pages");
            //    if (App.Service.IsDebug)
            //    {
            //        Log.Debug("+ {AppCapability}", "Razor Runtime Compilation");
            //        services.AddRazorPages().AddRazorRuntimeCompilation();
            //    }
            //    else
            //    {
            //        services.AddRazorPages();
            //    }
            //}

            //if (capabilities.ServerSideBlazor)
            //{
            //    Log.Debug("+ {AppCapability}", "Server Side Blazor");
            //    services.AddServerSideBlazor();
            //}

            //if (capabilities.Controllers)
            //{
            //    Log.Debug("+ {AppCapability}", "Controllers");
            //    services.AddControllers();
            //}

            return services;
        }

        ///// <summary>
        ///// Configure Canvas Services 
        ///// </summary>
        ///// <param name="app"><see cref="IApplicationBuilder"/></param>        
        ///// <returns><see cref="IApplicationBuilder"/> for chaining</returns>
        ///// <remarks>
        ///// <list type="bullet">
        /////     <item><description>Canvas Auth</description></item>
        /////     <item><description>OpenApi</description></item>
        /////     <item><description>HealthChecks</description></item>        
        ///// </list>         
        ///// </remarks>
        //public static IApplicationBuilder UseCanvasServices(this IApplicationBuilder app)
        //{
        //    if (App.Service.Canvas == null) { throw new ArgumentException("No Service Canvas Defined."); }

        //    var capabilities = App.Service.Canvas.Info.Capabilities;

        //    // figure out the app environment
        //    Log.Debug("Configuring Middleware...");

        //    if (App.Service.IsDebug && !App.Service.IsProduction())
        //    {
        //        Log.Debug("= {AppCapability}", "Developer Exception Page");
        //        app.UseDeveloperExceptionPage();
        //    }
        //    else
        //    {
        //        // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
        //        Log.Debug("= {AppCapability}", "HSTS (Strict-Transport-Security Header)");
        //        app.UseHsts();
        //        app.UseExceptionHandler("/error");
        //    }

        //    if (capabilities.HttpRedirection)
        //    {
        //        Log.Debug("= {AppCapability}", "HTTP Redirection");
        //        app.UseHttpsRedirection();
        //    }

        //    // see if we need to activate the policies
        //    app.UseCanvasCors();

        //    if (capabilities.StaticFiles)
        //    {
        //        Log.Debug("= {AppCapability}", "Static Files");
        //        app.UseStaticFiles();
        //    }

        //    if (!string.IsNullOrEmpty(App.Service.Canvas.Info.RoutePrefix))
        //    {
        //        //TODO: add or remove a '/' if needed

        //        Log.Debug("= Routing PathBase: {AppBaseUrl}", App.Service.Canvas.Info.RoutePrefix);
        //        app.UsePathBase(App.Service.Canvas.Info.RoutePrefix);
        //        app.UseRouting();
        //    }
        //    else
        //    {
        //        Log.Debug("= Routing");
        //        app.UseRouting();
        //    }

        //    // AUTH SECURITY / COOKIES
        //    app.UseCanvasAuthSecurity();

        //    // configure documentation
        //    if (capabilities.Swagger)
        //    {
        //        Log.Debug("= {AppCapability}", "Swagger");
        //        app.UseCanvasOpenApi();
        //    }

        //    // add health checks startup, ready, live
        //    if (capabilities.HealthChecks)
        //    {
        //        Log.Debug("= {AppCapability}", "HealthChecks");
        //        app.UseCanvasHealthChecks();
        //    }            

        //    if (capabilities.Mvc)
        //    {
        //        Log.Debug("= {AppCapability}", "MVC");
        //        app.UseMvc();
        //        app.UseMvcWithDefaultRoute();
        //    }

        //    app.UseEndpoints(endpoints =>
        //    {
        //        if (capabilities.Controllers)
        //        {
        //            Log.Debug("= {AppCapability}", "Controllers");
        //            endpoints.MapControllers();
        //        }

        //        if (capabilities.RazorPages)
        //        {   
        //            Log.Debug("= {AppCapability}", "Razor Pages");
        //            endpoints.MapRazorPages();
        //        }

        //        if (capabilities.ServerSideBlazor)
        //        {
        //            Log.Debug("= {AppCapability}", "Endpoint Blazor Hub");
        //            endpoints.MapBlazorHub();
        //            endpoints.MapFallbackToPage("/_Host");
        //        }
        //    });

        //    Log.Debug("Services Configured.");

        //    return app;
        //}

        /// <summary>
        /// Configure the base pretty json serialization options that we prefer
        /// </summary>
        /// <param name="services"></param>
        /// <returns></returns>
        public static IServiceCollection ConfigureJsonSerializerOptions(this IServiceCollection services)
        {
            // ================================================================================
            // JSON SERIALIZER OPTIONS
            // ================================================================================
            var jsonOptions = new JsonSerializerOptions()
            {
                AllowTrailingCommas = true,
                WriteIndented = Debugger.IsAttached,
                DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingDefault,
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
                PropertyNameCaseInsensitive = true
            };

            services.Configure<JsonSerializerOptions>(options =>
            {
                options.AllowTrailingCommas = jsonOptions.AllowTrailingCommas;
                options.WriteIndented = jsonOptions.WriteIndented;
                options.DefaultIgnoreCondition = jsonOptions.DefaultIgnoreCondition;
                options.PropertyNamingPolicy = jsonOptions.PropertyNamingPolicy;
                options.PropertyNameCaseInsensitive = jsonOptions.PropertyNameCaseInsensitive;                                
            });
            
            //services.Configure<JsonOptions>(options =>
            //{
            //    options.JsonSerializerOptions.AllowTrailingCommas = jsonOptions.AllowTrailingCommas;
            //    options.JsonSerializerOptions.WriteIndented = jsonOptions.WriteIndented;
            //    options.JsonSerializerOptions.DefaultIgnoreCondition = jsonOptions.DefaultIgnoreCondition;
            //    options.JsonSerializerOptions.PropertyNamingPolicy = jsonOptions.PropertyNamingPolicy;
            //    options.JsonSerializerOptions.PropertyNameCaseInsensitive = jsonOptions.PropertyNameCaseInsensitive;
            //});

            return services;
        }
    }
}

