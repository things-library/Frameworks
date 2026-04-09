using Microsoft.Extensions.Configuration;

using Core.Cloud.Data.Entity;
using System.Text.Json.Serialization;
using System.Diagnostics;

namespace CloudEntityTester
{
    public class AppSettings
    {
        private IConfigurationRoot Configuration { get; set; }

        public List<CloudEntitySettings> CloudEntitySettings { get; set; }

        public AppSettings(IConfigurationRoot configuration)
        {
            this.Configuration = configuration;
        }

        public void Load()
        {
            var section = this.Configuration.GetSection("CloudEntities");
            this.CloudEntitySettings = section.Get<List<CloudEntitySettings>>();
        }
    }

    [DebuggerDisplay("{Type} ({TableName})")]
    public class CloudEntitySettings
    {        
        public EntityStoreType Type { get; set; }
             
        public string TableName { get; set; }
             
        public string Connection { get; set; }

        public override string ToString() => $"{this.Type} ({this.TableName})";
    }
}
