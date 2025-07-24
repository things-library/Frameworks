﻿// ================================================================================
// <copyright file="Canvas.cs" company="Starlight Software Co">
//    Copyright (c) Starlight Software Co. All rights reserved.
//    Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
// </copyright>
// ================================================================================

using Serilog;
using ThingsLibrary.Schema.Canvas;

namespace ThingsLibrary.Services.AspNetCore.Extensions
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
        public static CanvasRoot AddServiceCanvas(this IServiceCollection services, IConfiguration configuration)
        {
            // see if this has already been called
            if (App.Service.Canvas != null) { return App.Service.Canvas; }

            Log.Debug("+ Service Canvas from configuration...");

            var canvasSection = configuration.GetSection("ServiceCanvas");
            if (!canvasSection.Exists()) { throw new ArgumentException("Missing 'ServiceCanvas' section in appSettings / Configuration"); }

            var canvas = canvasSection.Get<CanvasRoot>() ?? throw new ArgumentException("Unable to deserialize 'ServiceCanvas' section in appSettings / Configuration");

            // validate the service canvas object
            canvas.ThrowIfInvalid();
            
            App.Service.Init(canvas);

            // register our main canvas sections for DI            
            Log.Debug("+ Canvas");
            services.AddSingleton<CanvasRoot>(canvas);

            if (canvas.Auth != null)
            {
                Log.Debug("+ CanvasAuth");
                services.AddSingleton<CanvasAuth>(canvas.Auth);
            }

            if (canvas.Auth?.Jwt != null)
            {
                Log.Debug("+ CanvasAuthJwt");
                services.AddSingleton<CanvasAuthJwt>(canvas.Auth.Jwt);
            }

            // allow chaining
            return canvas;
        }


        /// <summary>
        /// Validate service canvas and throw argument exception if invalid
        /// </summary>
        /// <param name="canvas"></param>
        /// <exception cref="ArgumentException">When invalid canvas</exception>
        public static void ThrowIfInvalid(this CanvasRoot canvas)
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
        public static void Init(this AppService appService, CanvasRoot canvas)
        {
            if(appService.Canvas != null) { throw new ArgumentException("AppService.Canvas already initialized.");  }

            // set the canvas environment name
            var hostUri = canvas.Info.Host;
            if (hostUri != null)
            {
                appService.HostUri = (hostUri.AbsolutePath.EndsWith('/') ? hostUri : new Uri($"{hostUri.OriginalString}/"));  // the host uri MUST be a path where it ends with / otherwise the last folder is considered a file and appending relative paths won't append correctly.
                Log.Information("+ Host Uri: {HostUri}", appService.HostUri);
            }
            appService.EnvironmentName = canvas.Info.Environment;
            Log.Information("+ Environment: {EnvironmentName}", appService.EnvironmentName);

            appService.Canvas = canvas;
        }        
    }
}
