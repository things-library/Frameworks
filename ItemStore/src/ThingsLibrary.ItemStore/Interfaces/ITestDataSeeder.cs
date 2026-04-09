namespace ThingsLibrary.Entity.Interfaces
{
    /// <summary>
    /// Test Data Seeder
    /// </summary>
    public interface ITestDataSeeder
    {
        /// <summary>
        /// Seed Data in database
        /// </summary>
        /// <param name="entityStores"><see cref="IEntityStores"/> or inherited version</param>    
        public void Seed(IEntityStores entityStores);
    }
}