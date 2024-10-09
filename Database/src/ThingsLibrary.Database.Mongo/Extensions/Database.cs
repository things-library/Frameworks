// ================================================================================
// <copyright file="Database.cs" company="Starlight Software Co">
//    Copyright (c) Starlight Software Co. All rights reserved.
//    Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
// </copyright>
// ================================================================================

using MongoDB.Bson;

namespace ThingsLibrary.Database.Mongo.Extensions
{
    public static class DatabaseExtensions
    {        
        /// <summary>
        /// Add Mongo Database
        /// </summary>
        /// <param name="services">Service Collection</param>
        /// <param name="configuration">Configuration</param>        
        /// <param name="parameterName">Full Parameter Name</param>
        /// <returns></returns>
        /// <exception cref="ArgumentException"></exception>
        public static IServiceCollection AddDatabaseMongo<TContext>(this IServiceCollection services, IConfiguration configuration, string parameterName) where TContext : Database.DataContext
        {
            ArgumentNullException.ThrowIfNullOrEmpty(parameterName);

            var connectionString = configuration.TryGetConnectionString(parameterName);

            var databaseName = MongoUrl.Create(connectionString).DatabaseName;            
            if (string.IsNullOrEmpty(databaseName)) { throw new ArgumentException("Unable to find MongoDB database name part of connection string."); }

            var mongoClient = new MongoClient(connectionString);
            var mongoDatabase = mongoClient.GetDatabase(databaseName);

            // try to connect to data source
            Log.Information("Testing Mongo Database connection to {DatabaseName}...", databaseName);
            var isValid = mongoDatabase.RunCommandAsync((Command<BsonDocument>)"{ping:1}").Wait(1000);
            if (!isValid) { throw new ArgumentException("Unable to connect to MongoBD server."); }

            Log.Information("+ {ServiceName}", "IMongoDatabase");
            services.AddSingleton<IMongoDatabase>(s => mongoDatabase);
            
            Log.Information("+ {ServiceName}", "Mongo DataContext");            
            services.AddDbContext<TContext>(builder => builder.Configure<TContext>(connectionString, databaseName));

            // allow chaining
            return services;
        }

        /// <summary>
        /// Parse connection string and return data contect options
        /// </summary>        
        /// <param name="connectionString">Connection String</param>        
        /// <exception cref="ArgumentException">Missing arguments</exception>
        public static void Configure<TContext>(this DbContextOptionsBuilder builder, string connectionString) where TContext : Database.DataContext
        {
            ArgumentNullException.ThrowIfNullOrWhiteSpace(connectionString);

            // parse out the database name from the connection string
            var databaseName = MongoUrl.Create(connectionString).DatabaseName;
            if (string.IsNullOrEmpty(databaseName)) { throw new ArgumentException("Unable to find MongoDB database name."); }

            Configure<TContext>(builder, connectionString, databaseName);
        }

        /// <summary>
        /// Parse connection string and return data contect options
        /// </summary>        
        /// <param name="connectionString">Connection String</param>
        /// <param name="databaseName">Database Name</param>        
        public static void Configure<TContext>(this DbContextOptionsBuilder builder, string connectionString, string databaseName) where TContext : Database.DataContext
        {
            ArgumentNullException.ThrowIfNullOrWhiteSpace(connectionString);

            builder
                .UseQueryTrackingBehavior(QueryTrackingBehavior.NoTracking)
                .UseMongoDB(connectionString, databaseName);
        }
    }
}
