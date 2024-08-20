using Microsoft.Extensions.DependencyInjection;

using MongoDB.Bson.Serialization;
using MongoDB.Bson.Serialization.Serializers;

namespace Starlight.Entity.MongoAudit.Extensions
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddAuditEntityStoreFactory(this IServiceCollection services, string connectionString, string databaseName)
        {
            _ = Guard.Argument(connectionString, nameof(connectionString))
            .NotEmpty()
            .NotNull();

            _ = Guard.Argument(databaseName, nameof(databaseName))
            .NotEmpty()
            .NotNull();

            var entityStoreoptions = new Mongo.Models.EntityStoreOptions(connectionString, databaseName);
            services.AddSingleton<Mongo.Models.EntityStoreOptions>(entityStoreoptions);
                        
            // Register scopes
            services.AddScoped<Interfaces.IEntityStoreFactory, AuditEntityStoreFactory>();
            services.AddScoped<AuditEntityStoreFactory>();

            // map the classes
            MapClasses();

            return services;
        }

        private static void MapClasses()
        {
            // ================================================================================
            // Audit Event
            // ================================================================================
            BsonClassMap.RegisterClassMap<Models.AuditEvent>(classMap =>
            {
                classMap.AutoMap();
                classMap.SetIgnoreExtraElements(true);
                classMap.MapMember(c => c.EventType).SetSerializer(new EnumSerializer<Models.AuditType>(BsonType.String));
            });


        }
    }
}
