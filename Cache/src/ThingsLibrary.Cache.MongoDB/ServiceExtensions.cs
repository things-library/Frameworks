// ================================================================================
// <copyright file="ServiceExtensions.cs" company="Starlight Software Co">
//    Copyright (c) Starlight Software Co. All rights reserved.
//    Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
// </copyright>
// ================================================================================

using System.Text.Json;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Options;

using MongoDB.Driver;

using ThingsLibrary.Schema.Library;
using ThingsLibrary.Services.Extensions;

namespace ThingsLibrary.Cache.MongoDB
{
    public static partial class ServicesExtensions
    {
        public static IServiceCollection AddCacheCosmosStore(this IServiceCollection services, IMongoClient mongoClient, ItemDto configOptions, JsonSerializerOptions jsonSerializerOptions)
        {
            ArgumentNullException.ThrowIfNull(services);
            //services.AddMongoDbCache((CosmosCacheOptions cacheOptions) =>
            //{
            //    cacheOptions.CosmosClient = comosClient;
            //    cacheOptions.DatabaseName = configOptions["database_name"] ?? throw new ArgumentException("'database_name' missing from cache options.");
            //    cacheOptions.ContainerName = configOptions["container_name"] ?? throw new ArgumentException("'container_name' missing from cache options.");
            //    cacheOptions.CreateIfNotExists = true;
            //});

            services.Add(ServiceDescriptor.Singleton<IDistributedCache, MongoCache>((IServiceProvider provider) =>
            {
                var optionsMonitor = provider.GetService<IOptionsMonitor<MongoCacheOptions>>();
                if (optionsMonitor != null)
                {
                    return new MongoCache(optionsMonitor);
                }

                var options = provider.GetRequiredService<IOptions<MongoCacheOptions>>();
                return new MongoCache(options);
            }));


            //services.TryAddSingleton<JsonSerializerOptions>(jsonSerializerOptions);   //required 
            services.AddTransient<ICacheStore, CacheStore>();

            return services;
        }

        public static IServiceCollection AddCacheCosmosStore(this IServiceCollection services, IConfiguration configuration, ItemDto configOptions, JsonSerializerOptions jsonSerializerOptions)
        {
            //services.AddMongoDbCache((CosmosCacheOptions cacheOptions) =>
            //{
            //    var connectionStringVariable = configOptions["connection_string_variable"] ?? throw new ArgumentException("'connection_string_variable' missing from cache options");

            //    cacheOptions.ClientBuilder = new CosmosClientBuilder(configuration.TryGetConnectionString(connectionStringVariable));
            //    cacheOptions.DatabaseName = configOptions["database_name"] ?? throw new ArgumentException("'database_name' missing from cache options.");
            //    cacheOptions.ContainerName = configOptions["container_name"] ?? throw new ArgumentException("'container_name' missing from cache options.");
            //    cacheOptions.CreateIfNotExists = true;
            //});
                        
            services.TryAddSingleton<JsonSerializerOptions>(jsonSerializerOptions);   //required 
            services.AddTransient<ICacheStore, CacheStore>();

            return services;
        }
    }
}