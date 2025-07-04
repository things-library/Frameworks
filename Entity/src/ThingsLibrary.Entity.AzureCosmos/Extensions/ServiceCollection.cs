// ================================================================================
// <copyright file="ServiceCollection.cs" company="Starlight Software Co">
//    Copyright (c) Starlight Software Co. All rights reserved.
//    Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
// </copyright>
// ================================================================================

namespace ThingsLibrary.Entity.Cosmos.Extensions
{
    public static class ServiceCollectionExtensions
    {
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

        public static void UseEntityStoreCosmos<TContext>(this IServiceProvider services) where TContext : DbContext
        {
            // make sure database has all indexes created 
            using (var serviceScope = services.CreateScope())
            {
                var dbContext = serviceScope.ServiceProvider.GetRequiredService<TContext>();
                dbContext.Database.EnsureCreated();
            }
        }
    }
}

