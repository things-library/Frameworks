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
        public static IServiceCollection AddDatabasePostgres<TContext>(this IServiceCollection services, IConfiguration configuration, string parameterName) where TContext : Database.DataContext
        {
            ArgumentNullException.ThrowIfNullOrEmpty(parameterName);

            var connectionString = configuration.TryGetConnectionString(parameterName);

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
                .UseNpgsql(connectionString, builder =>
                {
                    builder.MigrationsAssembly(typeof(TContext).Assembly.FullName);
                    builder.CommandTimeout((int)TimeSpan.FromSeconds(30).TotalSeconds);

                    builder.EnableRetryOnFailure(maxRetryCount: 15, maxRetryDelay: TimeSpan.FromSeconds(30), errorCodesToAdd: null);
                });
        }
    }
}
