using System.Diagnostics.CodeAnalysis;

namespace ThingsLibrary.Database.Postgres
{
    [ExcludeFromCodeCoverage]
    public class DataContext : Database.DataContext
    {
        public DataContext(DbContextOptions options) : base(options)
        {
            //nothing
        }

        #region --- Static --- 

        public static DataContext Create<TContext>(string connectionString) where TContext : DbContext
        {
            ArgumentNullException.ThrowIfNullOrWhiteSpace(connectionString);

            var contextBuilder = DataContext.Parse<TContext>(connectionString);

            return new(contextBuilder.Options);
        }

        public static DbContextOptionsBuilder Parse<TContext>(string connectionString) where TContext : DbContext
        {
            ArgumentNullException.ThrowIfNullOrWhiteSpace(connectionString);

            return new DbContextOptionsBuilder<TContext>().UseNpgsql(connectionString, builder =>
            {
                builder.MigrationsAssembly(typeof(TContext).Assembly.FullName);
                builder.CommandTimeout((int)TimeSpan.FromSeconds(30).TotalSeconds);

                builder.UseQuerySplittingBehavior(QuerySplittingBehavior.SplitQuery);                
                builder.EnableRetryOnFailure(maxRetryCount: 15, maxRetryDelay: TimeSpan.FromSeconds(30), errorCodesToAdd: null);
            });
        }

        #endregion
    }
}
