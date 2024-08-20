namespace ThingsLibrary.Storage.Interfaces
{
    public interface ICloudFileStores
    {
        /// <summary>
        /// Type of entity store (Azure_Table, GCP_DataStore)
        /// </summary>
        public CloudFileStoreType StoreType { get; }

        /// <summary>
        /// Get/Create Cloud File Store
        /// </summary>        
        /// <returns><see cref="CloudFileStore"/></returns>
        public ICloudFileStore GetStore(string bucketName);
    }
}