// ================================================================================
// <copyright file="Configuration.cs" company="Starlight Software Co">
//    Copyright (c) Starlight Software Co. All rights reserved.
//    Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
// </copyright>
// ================================================================================

namespace ThingsLibrary.Services.Extensions
{
    public static class ConfigurationExtensions
    {        
        /// <summary>
        /// Get connection string in the various places it could be.
        /// </summary>
        /// <param name="configuration">Configuration</param>
        /// <param name="connectionStringName">Connection String Name</param>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException"></exception>
        /// <exception cref="ArgumentException"></exception>
        public static string TryGetConnectionString(this IConfiguration configuration, string connectionStringName)
        {
            if (string.IsNullOrEmpty(connectionStringName)) { throw new ArgumentNullException(nameof(connectionStringName)); }

            Console.WriteLine($"Getting connection string '{connectionStringName}'...");
            var connectionString = configuration.GetConnectionString(connectionStringName);
            if (string.IsNullOrEmpty(connectionString))
            {
                Console.WriteLine($"Not Found; Trying to get 'CUSTOMCONNSTR_{connectionStringName}' with Prefix...");
                connectionString = configuration.GetConnectionString($"CUSTOMCONNSTR_{connectionStringName}");
            }
            
            if (string.IsNullOrEmpty(connectionString))
            {
                Console.WriteLine($"Not Found; Trying to get '{connectionStringName}' from environment variables...");
                connectionString = Environment.GetEnvironmentVariable(connectionStringName);
            }

            if (string.IsNullOrEmpty(connectionString))
            {
                Console.WriteLine($"Not Found; Trying to get '{connectionStringName}' from environment variables with prefix...");
                connectionString = Environment.GetEnvironmentVariable($"CUSTOMCONNSTR_{connectionStringName}");
            }

            // STILL NOT FOUND??!?!?
            if (string.IsNullOrEmpty(connectionString))
            {
                Console.WriteLine($"Connection string '{connectionStringName}' not found!");
                throw new ArgumentException($"Unable to find connection string '{connectionStringName}'");
            }


            return connectionString;
        }
    }
}
