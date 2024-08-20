using Core.Cloud.Storage.File;
using Microsoft.Extensions.Configuration;

namespace CloudFileTester
{
    public class AppSettings
    {
        private IConfigurationRoot Configuration { get; set; }

        public List<FileStoreSettings> FileStoreSettings { get; set; }

        public AppSettings(IConfigurationRoot configuration)
        {
            this.Configuration = configuration;
        }

        public void Load()
        {
            var section = this.Configuration.GetSection("FileStores");
            this.FileStoreSettings = section.Get<List<FileStoreSettings>>();
        }
    }

    public class FileStoreSettings
    {
        public FileStoreType Type { get; set; }
        public string BucketName { get; set; }
        public string Connection { get; set; }

        public override string ToString() => $"{this.Type} ({this.BucketName})";
    }
}
