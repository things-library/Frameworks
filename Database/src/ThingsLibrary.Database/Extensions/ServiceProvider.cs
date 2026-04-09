// ================================================================================
// <copyright file="ServiceProvider.cs" company="Starlight Software Co">
//    Copyright (c) 2025 Starlight Software Co. All rights reserved.
//    Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
// </copyright>
// ================================================================================

using Microsoft.Extensions.DependencyInjection;

namespace ThingsLibrary.Entity.Extensions
{
    public static class ServiceProviderExtensions
    {
        /// <summary>
        /// Test connection to database and create if not exists
        /// </summary>
        /// <typeparam name="TContext"></typeparam>
        /// <param name="services"></param>
        public static void UseDataContext<TContext>(this IServiceProvider services) where TContext : Database.DataContext
        {
            // make sure database has all indexes created 
            using (var serviceScope = services.CreateScope())
            {
                var dbContext = serviceScope.ServiceProvider.GetRequiredService<TContext>();
                dbContext.Database.EnsureCreated();

                // SEED DATA?
            }
        }
    }
}

