namespace ThingsLibrary.Storage.Azure.Extensions
{
    public static class ServiceCollectionExtensions
    {
        /// <summary>
        /// Add Mongo Database
        /// </summary>
        /// <param name="services">Service Collection</param>
        /// <param name="configuration">Configuration</param>        
        /// <param name="parameterName">Full Parameter Name</param>
        /// <returns></returns>
        /// <exception cref="ArgumentException"></exception>
        public static IServiceCollection AddStorageAzureFileStoreService(this IServiceCollection services, IConfiguration configuration, string parameterName)
        {
            ArgumentNullException.ThrowIfNullOrEmpty(parameterName);

            var connectionString = configuration.TryGetConnectionString(parameterName);

            if (string.IsNullOrEmpty(connectionString)) { throw new ArgumentException($"Connection string '{parameterName}' not found."); }

            var fileStores = new FileStores(connectionString);

            // Register lib service
            services.AddSingleton<IFileStores>(fileStores);

            // allow chaining
            return services;
        }
    }
}
