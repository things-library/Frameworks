using Microsoft.Extensions.Configuration;

using System.Diagnostics;
using ThingsLibrary.Entity.Types;

namespace EntityTester
{
    public class AppSettings
    {
        private IConfigurationRoot Configuration { get; set; }

        public List<EntityStoreOptions> EntityStoreOptions { get; set; }

        public AppSettings(IConfigurationRoot configuration)
        {
            this.Configuration = configuration;
        }

        public void Load()
        {
            var section = this.Configuration.GetSection("StoreEntities");
            this.EntityStoreOptions = section.Get<List<EntityStoreOptions>>();

            // go get the secrets
            foreach(var options in this.EntityStoreOptions)
            {
                options.ConnectionString = Configuration.GetConnectionString(options.ConnectionStringVariable);
            }
        }
    }

    [DebuggerDisplay("{Type} ({TableName})")]
    public class EntityStoreOptions
    {        
        public string Type { get; set; }
             
        public string TableName { get; set; }
             
        public string ConnectionStringVariable { get; set; }

        public string ConnectionString { get; set; }

        public override string ToString() => $"{this.Type} ({this.TableName})";        
    }
}
