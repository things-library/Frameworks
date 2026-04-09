// ================================================================================
// <copyright file="ServiceCollection.cs" company="Starlight Software Co">
//    Copyright (c) 2025 Starlight Software Co. All rights reserved.
//    Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
// </copyright>
// ================================================================================

using System.Text.Json;

using Microsoft.Azure.Cosmos;
using Microsoft.Azure.Cosmos.Fluent;
using Microsoft.Extensions.Caching.Cosmos;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

using ThingsLibrary.Schema.Library;
using ThingsLibrary.Services.Extensions;

namespace ThingsLibrary.Cache.Cosmos.Extensions
{
    public static partial class ServiceCollectionExtensions
    {
        /// <summary>
        /// Add Cosmos DB distributed cache
        /// </summary>
        /// <param name="services">IServiceCollection</param>
        /// <param name="configOptions">Config Options</param>
        /// <param name="jsonSerializerOptions">JSON Serializer Options</param>
        /// <param name="configuration">Configuration</param>
        /// <returns></returns>
        /// <exception cref="ArgumentException"></exception>
        public static IServiceCollection AddCacheCosmos(this IServiceCollection services, ItemDto configOptions, JsonSerializerOptions jsonSerializerOptions, IConfiguration configuration)
        {
            var connectionStringVariable = configOptions["connection_string_variable"] ?? throw new ArgumentException("'connection_string_variable' missing from cache options");
            var builder = new CosmosClientBuilder(configuration.TryGetConnectionString(connectionStringVariable));

            return services.AddCacheCosmos(configOptions, jsonSerializerOptions, null, builder);
        }


        /// <summary>
        /// Add Cosmos DB distributed cache
        /// </summary>
        /// <param name="services">IServiceCollection</param>
        /// <param name="configOptions">Config Options</param>
        /// <param name="jsonSerializerOptions">JSON Serializer Options</param>
        /// <param name="cosmosClient">Cosmos Client</param>
        /// <param name="cosmosBuilder">Cosmos Client Builder</param>
        /// <returns></returns>
        /// <exception cref="ArgumentException"></exception>
        public static IServiceCollection AddCacheCosmos(this IServiceCollection services, ItemDto configOptions, JsonSerializerOptions jsonSerializerOptions, CosmosClient? cosmosClient, CosmosClientBuilder? cosmosBuilder)
        {
            services.AddCosmosCache((CosmosCacheOptions cacheOptions) =>
            {
                if(cosmosClient != null)
                {
                    cacheOptions.CosmosClient = cosmosClient;
                }
                else if(cosmosBuilder != null)
                {
                    cacheOptions.ClientBuilder = cosmosBuilder;
                }
                else
                { 
                    throw new ArgumentException("Either a CosmosClient or CosmosClientBuilder must be provided."); 
                }

                cacheOptions.DatabaseName = configOptions["database_name"] ?? throw new ArgumentException("'database_name' missing from cache options.");
                cacheOptions.ContainerName = configOptions["container_name"] ?? throw new ArgumentException("'container_name' missing from cache options.");
                cacheOptions.CreateIfNotExists = true;
            });

            services.TryAddSingleton<JsonSerializerOptions>(jsonSerializerOptions);   //required 
            services.AddTransient<ICacheStore, CacheStore>();

            return services;
        }        
    }
}