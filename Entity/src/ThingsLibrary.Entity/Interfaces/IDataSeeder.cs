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
        /// <param name="entityStores"><see cref="IEntityStores"/> or inherited version</param>           
        public void Seed(IEntityStores entityStores);
    }
}
