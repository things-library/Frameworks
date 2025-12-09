// ================================================================================
// <copyright file="ServiceCollection.cs" company="Starlight Software Co">
//    Copyright (c) 2025 Starlight Software Co. All rights reserved.
//    Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
// </copyright>
// ================================================================================

using Microsoft.Extensions.Localization;
using Microsoft.Extensions.Options;
using ThingsLibrary.Schema.Library;
using ThingsLibrary.Services.DataType;

namespace ThingsLibrary.Services.Extensions
{
    public static class IServiceCollectionExtensions
    {
        /// <summary>
        /// Register a tracked localizer factory that monitors missing localization keys.
        /// </summary>
        /// <param name="services"></param>
        /// <param name="resourceDirectoryPath"></param>
        /// <returns></returns>
        public static IServiceCollection AddTrackedLocalizer(this IServiceCollection services, string? resourceDirectoryPath)
        {
            // Decorate the default factory with TrackingStringLocalizerFactory
            services.AddSingleton<IStringLocalizerFactory>(serviceProvider =>
            {
                // Get the default factory
                var defaultFactory = new ResourceManagerStringLocalizerFactory(
                    serviceProvider.GetRequiredService<IOptions<LocalizationOptions>>(),
                    serviceProvider.GetRequiredService<ILoggerFactory>());

                // Wrap it with our tracking factory
                return new TrackingStringLocalizerFactory(defaultFactory, resourceDirectoryPath);
            });

            return services;
        }
    }
}
