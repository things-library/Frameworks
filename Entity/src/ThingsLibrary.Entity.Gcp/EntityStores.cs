namespace Starlight.Cloud.Entity.Gcp
{
    public class EntityStores : IEntityStores
    {
        public EntityStoreType StoreType => EntityStoreType.GCP_DataStore;

        private string StorageConnectionString { get; set; }

        public EntityStores(string storageConnectionString)
        {
            this.StorageConnectionString = storageConnectionString;
        }

        public IEntityStore<T> GetStore<T>(string name) where T : class
        {
            return new EntityStore<T>(this.StorageConnectionString, name);            
        }
    }
}
