namespace ThingsLibrary.Entity.Local
{
    public class EntityStoreFactory : IEntityStoreFactory
    {
        public EntityStoreType StoreType => EntityStoreType.Local;
    
        private string StorageConnectionString { get; set; }
        private LiteDatabase DataContext { get; set; }

        /// <inheritdoc />
        public EntityStoreFactory(string storageConnectionString, string databaseName)
        {
            this.StorageConnectionString = storageConnectionString;
            this.DataContext = new LiteDatabase(this.StorageConnectionString);
        }

        /// <inheritdoc />
        public IEntityStore<T> GetStore<T>(string name) where T : class
        {
            return new EntityStore<T>(this.StorageConnectionString, name);            
        }

        /// <inheritdoc />
        public Task<IEnumerable<string>> GetStoreListAsync(CancellationToken cancellationToken = default)
        {
            return Task.FromResult(this.DataContext.GetCollectionNames());            
        }

        /// <inheritdoc />
        public Task DeleteStoreAsync(string name, CancellationToken cancellationToken = default)
        {
            this.DataContext.DropCollection(name);     
        
            return Task.CompletedTask;
        }


        /// <summary>
        /// Validates the name to be used 
        /// </summary>
        /// <param name="name">Collection Name</param>
        /// <exception cref="ArgumentException"></exception>
        public static void ValidateCollectionName(string name)
        {
            //Contains only letters, numbers and _
            //Collection names are case insensitive
            //Collection names starting with _ are reserved for internal storage use
            //Collection names starting with $ are reserved for internal system/virtual collections

            // validate name            
            if (!name.All(c => char.IsLetterOrDigit(c))) { throw new ArgumentException("Table names can only be alphanumeric characters.", nameof(name)); }
            if (name.Length < 3 || name.Length > 63) { throw new ArgumentException("Table names must be between 3 and 63 characters long.", nameof(name)); }
            if (!char.IsLetter(name[0])) { throw new ArgumentException("Table names must start with a letter", nameof(name)); }
        }
    }
}
