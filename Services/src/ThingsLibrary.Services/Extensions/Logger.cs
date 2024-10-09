// ================================================================================
// <copyright file="LoggerExtensions.cs" company="Starlight Software Co">
//    Copyright (c) Starlight Software Co. All rights reserved.
//    Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
// </copyright>
// ================================================================================

namespace ThingsLibrary.Services.Extensions
{
    public static class LoggerExtensions
    {
        /// <summary>
        /// Initialize a long term logger 
        /// </summary>
        public static IServiceCollection AddSeriLogging(this IServiceCollection services, IConfiguration configuration)
        {
            // configure the logger as as possible
            if (configuration.GetSection("Serilog").Exists())
            {
                Log.Logger = new LoggerConfiguration()
                    .ReadFrom.Configuration(configuration)
                    .CreateLogger();
            }
            else
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("WARNING: No 'serilog' section found in AppSettings.");
                Console.ForegroundColor = ConsoleColor.Gray;

                Log.Logger = new LoggerConfiguration()
                    .WriteTo.Console()
                    .CreateLogger();

                Log.Warning("Default Serilog Logger Initalized.");
            }

            services.AddLogging(loggerBuilder => loggerBuilder.AddSerilog(Log.Logger));

            // for chaining
            return services;
        }
    }
}
