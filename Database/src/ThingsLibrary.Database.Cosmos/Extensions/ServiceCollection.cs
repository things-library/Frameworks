// ================================================================================
// <copyright file="ServiceCollection.cs" company="Starlight Software Co">
//    Copyright (c) 2025 Starlight Software Co. All rights reserved.
//    Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
// </copyright>
// ================================================================================



namespace ThingsLibrary.Entity.Cosmos.Extensions
{
    public static class ServiceCollectionExtensions
    {
        /// <summary>
        /// Adds a Cosmos DB database context to the service collection.
        /// </summary>
        /// <typeparam name="TContext"></typeparam>
        /// <param name="services"></param>
        /// <param name="connectionString"></param>
        /// <param name="databaseName"></param>
        /// <returns></returns>
        public static IServiceCollection AddDatabaseCosmos<TContext>(this IServiceCollection services, string connectionString, string databaseName) where TContext : Database.DataContext
        {
            ArgumentException.ThrowIfNullOrEmpty(connectionString);
            ArgumentException.ThrowIfNullOrEmpty(databaseName);

            services.AddDbContext<TContext>(options =>
            {
                options.UseQueryTrackingBehavior(QueryTrackingBehavior.NoTracking);

                options.EnableSensitiveDataLogging(!App.Service.IsProduction());
                options.EnableDetailedErrors(!App.Service.IsProduction());

                options.UseCosmos(connectionString, databaseName, contextOptions =>
                {
                    contextOptions.RequestTimeout(TimeSpan.FromSeconds(30));
                });
            });

            return services;
        }
    }
}

