// ================================================================================
// <copyright file="ServiceCollection.cs" company="Starlight Software Co">
//    Copyright (c) 2025 Starlight Software Co. All rights reserved.
//    Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
// </copyright>
// ================================================================================

namespace ThingsLibrary.Entity.Mongo.Extensions
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
        public static IServiceCollection AddEntityStoreMongo<TContext>(this IServiceCollection services, string connectionString, string databaseName) where TContext : DbContext
        {
            ArgumentException.ThrowIfNullOrEmpty(connectionString);
            ArgumentException.ThrowIfNullOrEmpty(databaseName);

            services.AddDbContext<TContext>(options =>
            {
                options.UseMongoDB(connectionString, databaseName);
            });

            return services;
        }




        public static IServiceCollection AddEntityStores(this IServiceCollection services, string connectionString, string databaseName)
        {
            // Register lib services here...
            services.AddSingleton<Interfaces.IEntityStores>(new EntityStores(connectionString, databaseName));

            return services;
        }

        public static IServiceCollection AddEntityStores(this IServiceCollection services, Uri connectionStringUri, string databaseName)
        {
            // Register lib services here...
            services.AddSingleton<Interfaces.IEntityStores>(new EntityStores(connectionStringUri, databaseName));

            return services;
        }
    }
}
