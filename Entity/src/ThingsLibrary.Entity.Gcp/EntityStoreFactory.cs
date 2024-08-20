
namespace ThingsLibrary.Entity.Gcp
{
    public class EntityStoreFactory : IEntityStoreFactory
    {
        public EntityStoreType StoreType => EntityStoreType.GCP_DataStore;

        private string StorageConnectionString { get; set; }

        public EntityStoreFactory(string storageConnectionString)
        {
            this.StorageConnectionString = storageConnectionString;
        }

        public IEntityStore<T> GetStore<T>(string name) where T : class
        {
            return new EntityStore<T>(this.StorageConnectionString, name);            
        }


        public Task<IEnumerable<string>> GetStoreListAsync(CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException();
        }

        public Task DeleteStoreAsync(string name, CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException();
        }
    }
}
