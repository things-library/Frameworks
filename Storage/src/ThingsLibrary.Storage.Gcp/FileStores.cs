using ThingsLibrary.Storage.Interfaces;

namespace ThingsLibrary.Storage.Gcp
{
    public class FileStores : IFileStores
    {
        /// <inheritdoc />
        public FileStoreType StoreType => FileStoreType.GCP_Storage;

        private string StorageConnectionString { get; set; }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="storageConnectionString"></param>
        public FileStores(string storageConnectionString)
        {
            this.StorageConnectionString = storageConnectionString;
        }

        /// <inheritdoc />
        public IFileStore GetStore(string name)
        {
            return new FileStore(this.StorageConnectionString, name);            
        }
    }
}
