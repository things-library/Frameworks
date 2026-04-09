// ================================================================================
// <copyright file="ServiceProvider.cs" company="Starlight Software Co">
//    Copyright (c) 2025 Starlight Software Co. All rights reserved.
//    Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
// </copyright>
// ================================================================================

using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.DependencyInjection;

namespace ThingsLibrary.Cache.Memory.Extensions
{
    public static partial class ServicesExtensions
    {
        /// <summary>
        /// Verify that the service can be reached and is operational
        /// </summary>
        /// <param name="services">Service Provider</param>        
        public static void UseCache(this IServiceProvider services)
        {
            using (var serviceScope = services.CreateScope())
            {
                var cache = serviceScope.ServiceProvider.GetRequiredService<IDistributedCache>();

                // CREATE A CACHE ENTRY, GET CACHE ENTRY
                var testKey = "~TEST_KEY";
                var testString = "~TEST_VALUE";

                // ADD
                cache.SetString(testKey, testString, new DistributedCacheEntryOptions
                {
                    AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(1)
                });

                // FETCH
                var value = cache.GetString(testKey);
                if (value != testString) { throw new IOException("Memory Cache test failed."); }
            }
        }
    }
}