// ================================================================================
// <copyright file="Configuration.cs" company="Starlight Software Co">
//    Copyright (c) Starlight Software Co. All rights reserved.
//    Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
// </copyright>
// ================================================================================

using Serilog;

namespace ThingsLibrary.Services.AspNetCore.Extensions
{
    public static class ConfigurationExtensions
    {
        /// <summary>
        /// Add the various app settings (base, env, env variables, secrets)
        /// </summary>
        /// <param name="context"></param>
        /// <param name="configurationBuilder"></param>
        /// <returns></returns>
        public static HostBuilderContext ConfigureAppConfiguration(this HostBuilderContext context, IConfigurationBuilder configurationBuilder)
        {            
            configurationBuilder.AddJsonFile(Path.Combine(context.HostingEnvironment.ContentRootPath, $"appsettings.json"), optional: false, reloadOnChange: false);
            configurationBuilder.AddJsonFile(Path.Combine(context.HostingEnvironment.ContentRootPath, $"appsettings.{context.HostingEnvironment.EnvironmentName}.json"), optional: true, reloadOnChange: false);
            configurationBuilder.AddEnvironmentVariables();
            configurationBuilder.AddUserSecrets(System.Reflection.Assembly.GetEntryAssembly()!, true);

            // for chaining
            return context;
        }
    }
}
