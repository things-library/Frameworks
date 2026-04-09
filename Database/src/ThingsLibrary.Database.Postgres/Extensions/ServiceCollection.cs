// ================================================================================
// <copyright file="ServiceCollection.cs" company="Starlight Software Co">
//    Copyright (c) 2025 Starlight Software Co. All rights reserved.
//    Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
// </copyright>
// ================================================================================

namespace ThingsLibrary.Database.Postgres.Extensions
{
    public static class ServiceCollectionExtensions
    {
        /// <summary>
        /// Register the MongoDB Entity Framework Core store
        /// </summary>
        /// <typeparam name="TContext">Data Context</typeparam>
        /// <param name="services">Services</param>
        /// <param name="connectionString">Connection String</param>
        /// <param name="databaseName">Database Name</param>
        /// <returns></returns>
        public static IServiceCollection AddDatabaseSqlServer<TContext>(this IServiceCollection services, string connectionString) where TContext : Database.DataContext
        {
            ArgumentException.ThrowIfNullOrEmpty(connectionString);

            services.AddDbContext<TContext>(options =>
            {
                options.UseQueryTrackingBehavior(QueryTrackingBehavior.NoTracking);
                
                options.EnableSensitiveDataLogging(!App.Service.IsProduction());
                options.EnableDetailedErrors(!App.Service.IsProduction());
                                
                options.UseNpgsql(connectionString, builder =>
                {
                    builder.MigrationsAssembly(typeof(TContext).Assembly.FullName);
                    builder.CommandTimeout((int)TimeSpan.FromSeconds(30).TotalSeconds);

                    builder.EnableRetryOnFailure(maxRetryCount: 15, maxRetryDelay: TimeSpan.FromSeconds(30), errorCodesToAdd: null);
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
        public static void AddDatabaseSqlServer<TContext>(this IServiceCollection services, ItemDto canvas, string canvasResourceKey, IConfiguration configuration) where TContext : Database.DataContext
        {
            if (canvas.TryGetItemTag(canvasResourceKey, "connection_string_variable", out var connectionStringKey))
            {
                Log.Information("+ Catalog Services");
                var connectionString = configuration.TryGetConnectionString(connectionStringKey);

                services.AddDatabaseSqlServer<TContext>(connectionString);
            }
            else
            {
                throw new ArgumentException($"Service canvas missing 'connection_string_variable' at '{canvasResourceKey}'");
            }
        }
    }
}
