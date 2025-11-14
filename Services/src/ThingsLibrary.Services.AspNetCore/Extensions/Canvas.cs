// ================================================================================
// <copyright file="Canvas.cs" company="Starlight Software Co">
//    Copyright (c) Starlight Software Co. All rights reserved.
//    Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
// </copyright>
// ================================================================================

using Serilog;

namespace ThingsLibrary.Services.AspNetCore.Extensions
{
    /// <summary>
    /// Service Canvas Extensions
    /// </summary>
    public static class CanvasExtensions
    {
        #region --- Old Canvas ---

        ///// <summary>
        ///// Get the service canvas from IConfiguration, register it, auth and authJwt as a singletons.
        ///// </summary>
        ///// <param name="context"><see cref="HostBuilderContext"/></param>
        ///// <param name="services"><see cref="IServiceCollection"/></param>
        ///// <returns><see cref="HostBuilderContext"/> for chaining</returns>
        ///// <exception cref="ArgumentException"></exception>
        //public static CanvasRoot AddServiceCanvas(this IServiceCollection services, IConfiguration configuration)
        //{
        //    // see if this has already been called
        //    if (App.Service.ServiceCanvas != null) { return App.Service.ServiceCanvas; }

        //    Log.Debug("+ Service Canvas from configuration...");

        //    var canvasSection = configuration.GetSection("ServiceCanvas");
        //    if (!canvasSection.Exists()) { throw new ArgumentException("Missing 'ServiceCanvas' section in appSettings / Configuration"); }

        //    var serviceCanvas = canvasSection.Get<CanvasRoot>() ?? throw new ArgumentException("Unable to deserialize 'ServiceCanvas' section in appSettings / Configuration");

        //    // validate the service canvas object
        //    serviceCanvas.ThrowIfInvalid();

        //    App.Service.Init(serviceCanvas);

        //    // register our main canvas sections for DI            
        //    Log.Debug("+ Canvas");
        //    services.AddSingleton<CanvasRoot>(serviceCanvas);

        //    if (serviceCanvas.Auth != null)
        //    {
        //        Log.Debug("+ CanvasAuth");
        //        services.AddSingleton<CanvasAuth>(serviceCanvas.Auth);
        //    }

        //    if (serviceCanvas.Auth?.Jwt != null)
        //    {
        //        Log.Debug("+ CanvasAuthJwt");
        //        services.AddSingleton<CanvasAuthJwt>(serviceCanvas.Auth.Jwt);
        //    }


        //    //IConfigurationSection section = context.Configuration.GetSection("ServiceCanvas");
        //    //if (!section.Exists()) { throw new ArgumentException("Missing 'ServiceCanvas' section in appSettings / Configuration"); }

        //    //var canvas = section.Get<ItemDto>() ?? throw new ArgumentException("Unable to deserialize 'ServiceCanvas' section in appSettings / Configuration");

        //    // allow chaining
        //    return serviceCanvas;
        //}

        ///// <summary>
        ///// Validate service canvas and throw argument exception if invalid
        ///// </summary>
        ///// <param name="canvas"></param>
        ///// <exception cref="ArgumentException">When invalid canvas</exception>
        //public static void ThrowIfInvalid(this CanvasRoot canvas)
        //{
        //    var results = canvas.ToValidationResults(true);
        //    if (results.Count == 0) { return; }   //no errors

        //    if (Debugger.IsAttached)
        //    {
        //        Debug.WriteLine("================================================================================");
        //        Debug.WriteLine(" Service Canvas Validation Errors");
        //        Debug.WriteLine("================================================================================");
        //        foreach (var error in results)
        //        {
        //            Debug.WriteLine("Members:  " + string.Join("; ", error.MemberNames));
        //            Debug.WriteLine("Error: " + error.ErrorMessage);
        //            Debug.WriteLine("");
        //        }
        //    }

        //    throw new ArgumentException("Service Canvas validation errors!  Check debug output for details.");
        //}

        ///// <summary>
        ///// Initialize the AppService with various canvas related fields
        ///// </summary>
        ///// <param name="appService"></param>
        ///// <param name="canvas"></param>
        ///// <exception cref="ArgumentException"></exception>
        //public static void Init(this AppService appService, CanvasRoot canvas)
        //{
        //    if (appService.ServiceCanvas != null) { throw new ArgumentException("AppService.Canvas already initialized."); }

        //    // set the canvas environment name
        //    var hostUri = canvas.Info.Host;
        //    if (hostUri != null)
        //    {
        //        appService.HostUri = (hostUri.AbsolutePath.EndsWith('/') ? hostUri : new Uri($"{hostUri.OriginalString}/"));  // the host uri MUST be a path where it ends with / otherwise the last folder is considered a file and appending relative paths won't append correctly.
        //        Log.Information("+ Host Uri: {HostUri}", appService.HostUri);
        //    }
        //    appService.EnvironmentName = canvas.Info.Environment;
        //    Log.Information("+ Environment: {EnvironmentName}", appService.EnvironmentName);

        //    appService.ServiceCanvas = canvas;
        //}

        #endregion

        #region --- New Canvas --- 

        /// <summary>
        /// Get the service canvas from IConfiguration, register it, auth and authJwt as a singletons.
        /// </summary>
        /// <param name="context"><see cref="HostBuilderContext"/></param>
        /// <param name="services"><see cref="IServiceCollection"/></param>
        /// <returns><see cref="HostBuilderContext"/> for chaining</returns>
        /// <exception cref="ArgumentException"></exception>
        public static ItemDto AddCanvas(this IServiceCollection services, IConfiguration configuration)
        {            
            Log.Debug("+ Service Canvas from configuration...");

            var canvasSection = configuration.GetSection("Canvas");
            if (!canvasSection.Exists()) { throw new ArgumentException("Missing 'Canvas' section in appSettings / Configuration"); }

            var canvas = canvasSection.Get<ItemDto>() ?? throw new ArgumentException("Unable to deserialize 'Canvas' section in appSettings / Configuration");

            // validate the service canvas object
            canvas.ThrowIfInvalid();

            App.Service.Init(canvas);
                        
            // allow chaining
            return canvas;
        }

        /// <summary>
        /// Validate service canvas and throw argument exception if invalid
        /// </summary>
        /// <param name="canvas"></param>
        /// <exception cref="ArgumentException">When invalid canvas</exception>
        public static void ThrowIfInvalid(this ItemDto canvas)
        {
            var results = canvas.ToValidationResults(true);
            if (results.Count == 0) { return; }   //no errors

            if (Debugger.IsAttached)
            {
                Debug.WriteLine("================================================================================");
                Debug.WriteLine(" Service Canvas Validation Errors");
                Debug.WriteLine("================================================================================");
                foreach (var error in results)
                {
                    Debug.WriteLine("Members:  " + string.Join("; ", error.MemberNames));
                    Debug.WriteLine("Error: " + error.ErrorMessage);
                    Debug.WriteLine("");
                }
            }

            throw new ArgumentException("Service Canvas validation errors!  Check debug output for details.");
        }

        /// <summary>
        /// Initialize the AppService with various canvas related fields
        /// </summary>
        /// <param name="appService"></param>
        /// <param name="canvas"></param>
        /// <exception cref="ArgumentException"></exception>
        public static void Init(this AppService appService, ItemDto canvas)
        {
            if (appService.Canvas != null) { throw new ArgumentException("AppService.Canvas already initialized."); }

            // set the canvas environment name            
            if(canvas.Tags.TryGetValue("host", out var host))
            {                
                appService.HostUri = new Uri(host.EndsWith('/') ? host : $"{host}/");  // the host uri MUST be a path where it ends with / otherwise the last folder is considered a file and appending relative paths won't append correctly.
                Log.Information("+ Host Uri: {HostUri}", appService.HostUri);
            }

            appService.EnvironmentName = canvas["environment"] ?? throw new ArgumentException("No environment provided in service canvas");
            Log.Information("+ Environment: {EnvironmentName}", appService.EnvironmentName);

            appService.Canvas = canvas;
        }

        #endregion
    }
}
