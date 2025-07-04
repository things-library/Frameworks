// ================================================================================
// <copyright file="ServiceCollection.cs" company="Starlight Software Co">
//    Copyright (c) Starlight Software Co. All rights reserved.
//    Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
// </copyright>
// ================================================================================

namespace ThingsLibrary.Entity.Mongo.Extensions
{
    public static class ServiceCollectionExtensions
    {
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
    }
}
