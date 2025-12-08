// ================================================================================
// <copyright file="ServiceCollection.cs" company="Starlight Software Co">
//    Copyright (c) 2025 Starlight Software Co. All rights reserved.
//    Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
// </copyright>
// ================================================================================

using System.Text.Json;

using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace ThingsLibrary.Cache.Memory.Extensions
{
    /// <summary>
    /// Service Collection Extensions for Memory Cache Store
    /// </summary>
    public static partial class ServiceCollectionExtensions
    {
        /// <summary>
        /// Register the Memory Cache Store services
        /// </summary>
        /// <param name="services">Services</param>
        /// <param name="jsonSerializerOptions">JSON Serializer Options</param>
        /// <returns></returns>
        public static IServiceCollection AddCacheMemory(this IServiceCollection services, JsonSerializerOptions jsonSerializerOptions)
        {
            services.AddDistributedMemoryCache();

            services.TryAddSingleton<JsonSerializerOptions>(jsonSerializerOptions);   //required 
            services.AddTransient<ICacheStore, CacheStore>();

            return services;
        }        
    }
}