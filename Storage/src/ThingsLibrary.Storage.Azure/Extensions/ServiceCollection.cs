using Microsoft.Extensions.DependencyInjection;

namespace ThingsLibrary.Storage.Azure.Extensions
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddFileStores(this IServiceCollection services, string connectionString)
        {
            ArgumentNullException.ThrowIfNullOrEmpty(connectionString);
            
            var fileStores = new FileStores(connectionString);

            // Register lib services here...
            services.AddSingleton<ICloudFileStores>(fileStores);

            return services;
        }
    }
}
