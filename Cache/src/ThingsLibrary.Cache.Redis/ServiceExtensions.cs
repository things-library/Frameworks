// ================================================================================
// <copyright file="ServiceExtensions.cs" company="Starlight Software Co">
//    Copyright (c) Starlight Software Co. All rights reserved.
//    Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
// </copyright>
// ================================================================================

using System.Text.Json;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace ThingsLibrary.Cache.Redis
{
    public static partial class ServicesExtensions
    {
        public static IServiceCollection AddCacheStore(this IServiceCollection services, string connectionString, JsonSerializerOptions jsonSerializerOptions)
        {            
            services.AddStackExchangeRedisCache(options =>
            {
                options.Configuration = connectionString;
            });

            services.TryAddSingleton<JsonSerializerOptions>(jsonSerializerOptions);   //required 
            services.AddTransient<ICacheStore, CacheStore>();

            return services;
        }

    }
}