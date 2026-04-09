// ================================================================================
// <copyright file="Canvas.cs" company="Starlight Software Co">
//    Copyright (c) 2025 Starlight Software Co. All rights reserved.
//    Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
// </copyright>
// ================================================================================

namespace ThingsLibrary.Services.Extensions
{
    /// <summary>
    /// Service Canvas Extensions
    /// </summary>
    public static class CanvasExtensions
    {
        /// <summary>
        /// Get the service canvas from IConfiguration, register it, auth and authJwt as a singletons.
        /// </summary>
        /// <param name="context"><see cref="HostBuilderContext"/></param>
        /// <param name="services"><see cref="IServiceCollection"/></param>
        /// <returns><see cref="HostBuilderContext"/> for chaining</returns>
        /// <exception cref="ArgumentException"></exception>
        public static ItemDto InitCanvas(this IConfiguration configuration)
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
                if (!string.IsNullOrEmpty(host))
                {
                    appService.HostUri = new Uri(host.EndsWith('/') ? host : $"{host}/");  // the host uri MUST be a path where it ends with / otherwise the last folder is considered a file and appending relative paths won't append correctly.
                    Log.Information("+ Host Uri: {HostUri}", appService.HostUri);
                }
            }

            appService.EnvironmentName = canvas["environment"] ?? throw new ArgumentException("No environment provided in service canvas");
            Log.Information("+ Environment: {EnvironmentName}", appService.EnvironmentName);

            appService.Canvas = canvas;
        }
    }
}
