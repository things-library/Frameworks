// ================================================================================
// <copyright file="IServiceCollection.cs" company="Starlight Software Co">
//    Copyright (c) Starlight Software Co. All rights reserved.
//    Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
// </copyright>
// ================================================================================

using Microsoft.Extensions.DependencyInjection;

namespace ThingsLibrary.Database.Audit.EntityFrameworkCore.Extensions
{
    /// <summary>
    /// Audit Service Extensions
    /// </summary>
    public static class IServiceCollectionExtensions
    {
        /// <summary>
        /// Set up the audit services and tie to the inherited db context
        /// </summary>
        /// <typeparam name="T">Context that inherits <see cref="DataContext"/></typeparam>
        /// <param name="services"><see cref="IServiceCollection"/></param>
        /// <returns><see cref="IServiceCollection"/></returns>
        public static IServiceCollection AddAuditService<T>(this IServiceCollection services) where T : DataContext
        {
            // add all the audit table services details (must have a DataContext FIRST)
            services.AddScoped<DataContext, T>();

            services.AddScoped<Interfaces.IAuditService, Services.AuditService>();
            services.AddScoped<Interfaces.IAuditRepository, Repositories.AuditRepository>();

            return services;
        }
    }
}
