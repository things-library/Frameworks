namespace ThingsLibrary.Database.Cosmos.Extensions
{
    public static class DatabaseExtensions
    {
        /// <summary>
        /// Configure Cosmos Database
        /// </summary>
        /// <param name="services">Service Collection</param>
        /// <param name="configuration">Configuration</param>
        /// <param name="environmentName">Environment name for connection string prefix</param>
        /// <param name="name">Root Parameter Name</param>
        /// <returns></returns>
        /// <exception cref="ArgumentException"></exception>
        public static IServiceCollection AddDatabaseCosmos<TContext>(this IServiceCollection services, IConfiguration configuration, string environmentName, string name) where TContext : DbContext
        {
            ArgumentNullException.ThrowIfNullOrEmpty(environmentName);
            ArgumentNullException.ThrowIfNullOrEmpty(name);

            var parameterName = $"{environmentName}_{name}";

            // allow chaining
            return services.AddDatabaseCosmos<TContext>(configuration, parameterName);
        }

        /// <summary>
        /// Add Cosmos Database
        /// </summary>
        /// <param name="services">Service Collection</param>
        /// <param name="configuration">Configuration</param>
        /// <param name="parameterName">Full Parameter Name</param>
        /// <returns></returns>
        /// <exception cref="ArgumentException"></exception>
        public static IServiceCollection AddDatabaseCosmos<TContext>(this IServiceCollection services, IConfiguration configuration, string parameterName) where TContext : DbContext
        {
            ArgumentNullException.ThrowIfNullOrEmpty(parameterName);

            var connectionString = configuration.TryGetConnectionString(parameterName);

            // verify a SQL connection can be established before continuing            
            //using (var connection = new SqlConnection(connectionString))
            //{
            //    Log.Debug("Testing SQL Connection to {SqlDatabaseServer}...", connection.DataSource);
            //    connection.Open();

            //    Log.Information("+ SQL Server: {SqlDatabaseServer} ", connection.DataSource);
            //    Log.Information("+ SQL Database: {SqlDatabaseName}", connection.Database);
            //}

            var databaseName = "";

            Log.Information("+ {AddService}", "Mongo Database");
            services.AddDbContext<TContext>(options => DataContext.Parse<TContext>(connectionString, databaseName));
            
            // allow chaining
            return services;
        }
    }
}
