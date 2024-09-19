namespace ThingsLibrary.Database.Postgres.Extensions
{
    public static class DatabaseExtensions
    {
        /// <summary>
        /// Add Mongo Database
        /// </summary>
        /// <param name="services">Service Collection</param>
        /// <param name="configuration">Configuration</param>
        /// <param name="environmentName">Environment name for connection string prefix</param>
        /// <returns></returns>
        /// <exception cref="ArgumentException"></exception>
        public static IServiceCollection AddDatabasePostgres<TContext>(this IServiceCollection services, IConfiguration configuration, string environmentName) where TContext : DbContext
        {
            var parameterName = $"{environmentName}_MongoDatabase";
            var connectionString = configuration.TryGetConnectionString(parameterName, false);

            if (string.IsNullOrEmpty(connectionString)) { throw new ArgumentException($"Unable to find {parameterName}"); }

            // verify a SQL connection can be established before continuing            
            using (var connection = new NpgsqlConnection(connectionString))
            {
                Log.Debug("Testing SQL Connection to {SqlDatabaseServer}...", connection.DataSource);
                connection.Open();
                
                Log.Information("+ PostgreSQL Server: {SqlDatabaseServer} ", connection.DataSource);
                Log.Information("+ Database: {SqlDatabaseName}", connection.Database);
            }

            services.AddDbContext<TContext>(options => DataContext.Parse<TContext>(connectionString));

            // allow chaining
            return services;
        }
    }
}
