using Microsoft.Extensions.DependencyInjection;

namespace Starlight.Entity.AzureCosmos.Extensions
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddEntityStores(this IServiceCollection services, string connectionString, string databaseName)
        {            
            // Register lib services here...
            services.AddSingleton<Interfaces.IEntityStoreFactory>(new EntityStoreFactory(connectionString, databaseName));

            return services;
        }
    }
}
