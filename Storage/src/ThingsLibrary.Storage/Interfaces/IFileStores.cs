namespace ThingsLibrary.Storage.Interfaces
{
    public interface IFileStores
    {
        /// <summary>
        /// Type of entity store (Azure_Table, GCP_DataStore)
        /// </summary>
        public FileStoreType StoreType { get; }

        /// <summary>
        /// Get/Create Cloud File Store
        /// </summary>        
        /// <returns><see cref="FileStore"/></returns>
        public IFileStore GetStore(string bucketName);
    }
}