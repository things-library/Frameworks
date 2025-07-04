﻿// ================================================================================
// <copyright file="ServiceExtensions.cs" company="Starlight Software Co">
//    Copyright (c) Starlight Software Co. All rights reserved.
//    Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
// </copyright>
// ================================================================================

namespace ThingsLibrary.Cache.NCache
{
    using System.Text.Json;
    using Alachisoft.NCache.Caching.Distributed;
    using Microsoft.Extensions.DependencyInjection;
    using Microsoft.Extensions.DependencyInjection.Extensions;

    namespace ThingsLibrary.Cache.Memory
    {
        public static partial class ServicesExtensions
        {
            public static IServiceCollection AddCacheStore(this IServiceCollection services, string cacheName, JsonSerializerOptions jsonSerializerOptions)
            {
                services.AddNCacheDistributedCache(options =>
                {
                    options.CacheName = cacheName;                 
                });

                services.TryAddSingleton<JsonSerializerOptions>(jsonSerializerOptions);   //required 
                services.AddTransient<ICacheStore, CacheStore>();

                return services;
            }

        }
    }
}
