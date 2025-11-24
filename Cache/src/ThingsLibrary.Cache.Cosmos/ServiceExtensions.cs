// ================================================================================
// <copyright file="ServiceExtensions.cs" company="Starlight Software Co">
//    Copyright (c) Starlight Software Co. All rights reserved.
//    Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
// </copyright>
// ================================================================================

using Microsoft.Azure.Cosmos;
using Microsoft.Azure.Cosmos.Fluent;
using Microsoft.Extensions.Caching.Cosmos;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

using ThingsLibrary.Schema.Library;
using ThingsLibrary.Services.Extensions;

namespace ThingsLibrary.Cache.Cosmost
{
    public static partial class ServicesExtensions
    {
        public static IServiceCollection AddCacheStore(this IServiceCollection services, CosmosClient comosClient, ItemDto configOptions)
        {
            services.AddCosmosCache((CosmosCacheOptions cacheOptions) =>
            {
                cacheOptions.CosmosClient = comosClient;
                cacheOptions.DatabaseName = configOptions["database_name"] ?? throw new ArgumentException("'database_name' missing from cache options.");
                cacheOptions.ContainerName = configOptions["container_name"] ?? throw new ArgumentException("'container_name' missing from cache options.");
                cacheOptions.CreateIfNotExists = true;
            });

            //var c = new CosmosClientBuilder()

            //services.TryAddSingleton<JsonSerializerOptions>(jsonSerializerOptions);   //required 
            services.AddTransient<ICacheStore, CacheStore>();

            return services;
        }

        public static IServiceCollection AddCacheStore(this IServiceCollection services, IConfiguration configuration, ItemDto configOptions)
        {
            services.AddCosmosCache((CosmosCacheOptions cacheOptions) =>
            {
                var connectionStringVariable = configOptions["connection_variable"] ?? throw new ArgumentException("'connection_variable' missing from cache options");

                cacheOptions.ClientBuilder = new CosmosClientBuilder(configuration.TryGetConnectionString(connectionStringVariable));
                cacheOptions.DatabaseName = configOptions["database_name"] ?? throw new ArgumentException("'database_name' missing from cache options.");
                cacheOptions.ContainerName = configOptions["container_name"] ?? throw new ArgumentException("'container_name' missing from cache options.");
                cacheOptions.CreateIfNotExists = true;
            });
                        
            //services.TryAddSingleton<JsonSerializerOptions>(jsonSerializerOptions);   //required 
            services.AddTransient<ICacheStore, CacheStore>();

            return services;
        }
    }
}