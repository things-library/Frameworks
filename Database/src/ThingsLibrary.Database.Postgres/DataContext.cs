namespace ThingsLibrary.Database.Postgres
{
    public static class DataContextUtils
    {
        public static TContext Create<TContext>(string connectionString) where TContext : Database.DataContext
        {
            ArgumentNullException.ThrowIfNullOrWhiteSpace(connectionString);

            var contextBuilder = Parse<TContext>(connectionString);

            return new TContext(contextBuilder);
        }

        public static DbContextOptions<TContext> Parse<TContext>(string connectionString) where TContext : DbContext
        {
            ArgumentNullException.ThrowIfNullOrWhiteSpace(connectionString);

            return new DbContextOptionsBuilder<TContext>().UseNpgsql(connectionString, builder =>
            {
                builder.MigrationsAssembly(typeof(TContext).Assembly.FullName);
                builder.CommandTimeout((int)TimeSpan.FromSeconds(30).TotalSeconds);

                builder.UseQuerySplittingBehavior(QuerySplittingBehavior.SplitQuery);
                builder.EnableRetryOnFailure(maxRetryCount: 15, maxRetryDelay: TimeSpan.FromSeconds(30), errorCodesToAdd: null);
            }).Options;
        }
    }
}
