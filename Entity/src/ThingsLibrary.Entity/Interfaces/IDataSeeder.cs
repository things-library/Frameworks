namespace ThingsLibrary.Entity.Interfaces
{
    /// <summary>
    /// Data Seeder Interface
    /// </summary>
    public interface IDataSeeder
    {
        /// <summary>
        /// Seed Data in database
        /// </summary>
        /// <param name="entityStores"><see cref="IEntityStoreFactory"/> or inherited version</param>           
        public void Seed(IEntityStoreFactory entityStores);
    }
}
