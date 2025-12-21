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
        /// Register an Entity Framework Cosmos DB store for the specified DbContext    
        /// </summary>
        /// <typeparam name="TContext"></typeparam>
        /// <param name="services"></param>
        /// <param name="connectionString"></param>
        /// <param name="databaseName"></param>
        /// <returns></returns>
        public static IServiceCollection AddEntityStoreCosmos<TContext>(this IServiceCollection services, string connectionString, string databaseName) where TContext : DbContext
        {
            ArgumentException.ThrowIfNullOrEmpty(connectionString);
            ArgumentException.ThrowIfNullOrEmpty(databaseName);

            services.AddDbContext<TContext>(options =>
            {   
                options.UseCosmos(connectionString, databaseName);
            });

            return services;
        }


        public static IServiceCollection AddEntityStores(this IServiceCollection services, string connectionString, string databaseName)
        {
            // Register lib services here...
            services.AddSingleton<Interfaces.IEntityStores>(new EntityStores(connectionString, databaseName));

            return services;
        }
    }
}

