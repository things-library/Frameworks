using Microsoft.Extensions.DependencyInjection;

namespace Starlight.Entity.AzureTable
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddEntityStores(this IServiceCollection services, string connectionString)
        {            
            // Register lib services here...
            services.AddSingleton<Interfaces.IEntityStoreFactory>(new EntityStoreFactory(connectionString));

            return services;
        }
    }
}
