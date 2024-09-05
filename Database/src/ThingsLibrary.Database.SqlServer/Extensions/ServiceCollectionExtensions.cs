using ThingsLibrary.Database.Extensions;

namespace ThingsLibrary.Database.Mongo.Extensions
{
    public static class ServiceCollectionExtensions
    {
        /// <summary>
        /// Add Mongo Database
        /// </summary>
        /// <param name="services">Service Collection</param>
        /// <param name="configuration">Configuration</param>
        /// <param name="environmentName">Environment name for connection string prefix</param>
        /// <returns></returns>
        /// <exception cref="ArgumentException"></exception>
        public static IServiceCollection ConfigureMongoDatabase<TContext>(this IServiceCollection services, IConfiguration configuration, string environmentName) where TContext : DbContext
        {
            //var parameterName = $"{environmentName}_MongoDatabase";
            //var connectionString = configuration.TryGetConnectionString(parameterName, false);
                        
            //if (string.IsNullOrEmpty(connectionString)) { throw new ArgumentException($"Unable to find {parameterName}"); }
            
            //var mongoDbName = MongoUrl.Create(connectionString).DatabaseName;            
            //if (string.IsNullOrEmpty(mongoDbName)) { throw new ArgumentException("Unable to find MongoDB database name."); }

            //var mongoClient = new MongoClient(connectionString);
            //var mongoDatabase = mongoClient.GetDatabase(mongoDbName);

            //Log.Debug("+ {AddService}", "MongoDbContext");
            //services.AddSingleton<IMongoDatabase>(s => mongoDatabase);
            //services.AddSingleton<TContext>();

            // allow chaining
            return services;
        }
    }
}
