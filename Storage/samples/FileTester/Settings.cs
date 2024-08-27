using Microsoft.Extensions.Configuration;

namespace FileTester
{
    public class AppSettings
    {
        private IConfigurationRoot Configuration { get; set; }

        public List<FileStoreOptions> FileStoreSettings { get; set; }

        public AppSettings(IConfigurationRoot configuration)
        {
            this.Configuration = configuration;
        }

        public void Load()
        {
            var section = this.Configuration.GetSection("FileStores");
            this.FileStoreSettings = section.Get<List<FileStoreOptions>>();

            foreach(var options in this.FileStoreSettings)
            {
                options.ConnectionString = this.Configuration.GetConnectionString(options.ConnectionStringVariable);
            }
        }
    }

    public class FileStoreOptions
    {
        public string Type { get; set; }
        public string BucketName { get; set; }
        public string ConnectionStringVariable { get; set; }

        public string ConnectionString { get; set; }

        public override string ToString() => $"{this.Type} ({this.BucketName})";
    }
}
