using Microsoft.Extensions.DependencyInjection;

namespace ThingsLibrary.Entity.Mongo.Extensions
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddEntityStoreFactory(this IServiceCollection services, string connectionString, string databaseName)
        {
            ArgumentNullException.ThrowIfNullOrWhiteSpace(connectionString);
            ArgumentNullException.ThrowIfNullOrWhiteSpace(databaseName);

            var entityStoreoptions = new Models.EntityStoreOptions(connectionString, databaseName);
            services.AddSingleton<Models.EntityStoreOptions>(entityStoreoptions);
                        
            // Register scopes
            services.AddScoped<Interfaces.IEntityStoreFactory, EntityStoreFactory>();
            services.AddScoped<EntityStoreFactory>();

            return services;
        }
    }
}
