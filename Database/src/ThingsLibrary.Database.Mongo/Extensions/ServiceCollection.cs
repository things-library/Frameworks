// ================================================================================
// <copyright file="ServiceCollection.cs" company="Starlight Software Co">
//    Copyright (c) 2025 Starlight Software Co. All rights reserved.
//    Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
// </copyright>
// ================================================================================

namespace ThingsLibrary.Database.Mongo.Extensions
{
    public static class ServiceCollectionExtensions
    {
        /// <summary>
        /// Add Mongo Database Context
        /// </summary>
        /// <typeparam name="TContext">Data Context</typeparam>
        /// <param name="services">Services</param>
        /// <param name="connectionString">Connection String</param>
        /// <param name="databaseName">Database Name</param>
        /// <returns></returns>
        public static IServiceCollection AddDatabaseMongo<TContext>(this IServiceCollection services, string connectionString, string databaseName) where TContext : Database.DataContext
        {
            ArgumentException.ThrowIfNullOrEmpty(connectionString);
            ArgumentException.ThrowIfNullOrEmpty(databaseName);

            services.AddDbContext<TContext>(options =>
            {
                options.UseQueryTrackingBehavior(QueryTrackingBehavior.NoTracking);

                options.EnableSensitiveDataLogging(!App.Service.IsProduction());
                options.EnableDetailedErrors(!App.Service.IsProduction());

                options.UseMongoDB(connectionString, databaseName, contextOptions =>
                {
                    //nothing
                });
            });
            
            return services;
        }

        /// <summary>
        /// Add SQL Server Database from Canvas definition
        /// </summary>
        /// <typeparam name="TContext">DataContext</typeparam>
        /// <param name="services">Service Collection</param>
        /// <param name="canvas">Service Canvas</param>
        /// <param name="canvasResourceKey">Canvas Resource Key</param>
        /// <param name="configuration">Configuration</param>
        /// <exception cref="ArgumentException"></exception>
        public static void AddDatabaseMongo<TContext>(this IServiceCollection services, ItemDto canvas, string canvasResourceKey, IConfiguration configuration) where TContext : Database.DataContext
        {
            string? databaseName;
            if(!canvas.TryGetItemTag(canvasResourceKey, "database_name", out databaseName))
            {
                throw new ArgumentException($"Service canvas missing 'database_name' at '{canvasResourceKey}'");
            }

            if(!canvas.TryGetItemTag(canvasResourceKey, "connection_string_variable", out var connectionStringKey))
            {              
                throw new ArgumentException($"Service canvas missing 'connection_string_variable' at '{canvasResourceKey}'");
            }
                        
            var connectionString = configuration.TryGetConnectionString(connectionStringKey);

            services.AddDatabaseMongo<TContext>(connectionString, databaseName);
        }
    }
}
