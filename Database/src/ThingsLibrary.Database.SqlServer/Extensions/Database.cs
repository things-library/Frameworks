// ================================================================================
// <copyright file="Database.cs" company="Starlight Software Co">
//    Copyright (c) Starlight Software Co. All rights reserved.
//    Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
// </copyright>
// ================================================================================

using Serilog;
using ThingsLibrary.Services.Extensions;

namespace ThingsLibrary.Database.SqlServer.Extensions
{
    public static class DatabaseExtensions
    {        
        /// <summary>
        /// Add SQL Server Database
        /// </summary>
        /// <param name="services">Service Collection</param>
        /// <param name="configuration">Configuration</param>        
        /// <param name="parameterName">Full Parameter Name</param>
        /// <returns></returns>
        /// <exception cref="ArgumentException"></exception>
        public static IServiceCollection AddDatabaseSqlServer<TContext>(this IServiceCollection services, IConfiguration configuration, string parameterName) where TContext : Database.DataContext
        {
            ArgumentNullException.ThrowIfNullOrEmpty(parameterName);

            var connectionString = configuration.TryGetConnectionString(parameterName);

            services.AddDatabaseSqlServer<TContext>(connectionString);

            return services;
        }

        /// <summary>
        /// Add SQL Server Database
        /// </summary>
        /// <param name="services">Service Collection</param>
        /// <param name="connectionString">Connection String</param>        
        /// <returns></returns>
        /// <exception cref="ArgumentException"></exception>
        public static IServiceCollection AddDatabaseSqlServer<TContext>(this IServiceCollection services, string connectionString) where TContext : Database.DataContext
        {            
            // verify a SQL connection can be established before continuing            
            using (var connection = new SqlConnection(connectionString))
            {
                Log.Information("Testing SQL Connection to {DatabaseServer}...", connection.DataSource);
                connection.Open();

                Log.Information("+ SQL Server: {DatabaseServer}", connection.DataSource);
                Log.Information("+ SQL Database: {DatabaseName}", connection.Database);
            }

            services.AddDbContext<TContext>(builder => builder.Configure<TContext>(connectionString));

            // allow chaining
            return services;
        }

        /// <summary>
        /// Configure the DataContext based on the connection string
        /// </summary>
        /// <typeparam name="TContext">Data Context</typeparam>
        /// <param name="builder">Data Context Options Builder</param>
        /// <param name="connectionString">Connection String</param>
        public static void Configure<TContext>(this DbContextOptionsBuilder builder, string connectionString) where TContext : Database.DataContext
        {
            ArgumentNullException.ThrowIfNullOrWhiteSpace(connectionString);

            builder
                .UseQueryTrackingBehavior(QueryTrackingBehavior.NoTracking)
                .UseSqlServer(connectionString, builder =>
                {
                    builder.MigrationsAssembly(typeof(TContext).Assembly.FullName);
                    builder.CommandTimeout((int)TimeSpan.FromSeconds(30).TotalSeconds);

                    builder.EnableRetryOnFailure(maxRetryCount: 15, maxRetryDelay: TimeSpan.FromSeconds(30), errorNumbersToAdd: null);
                });
        }
    }
}
