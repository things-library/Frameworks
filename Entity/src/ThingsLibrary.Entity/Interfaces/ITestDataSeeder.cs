namespace ThingsLibrary.Entity.Interfaces;

/// <summary>
/// Test Data Seeder
/// </summary>
public interface ITestDataSeeder
{
    /// <summary>
    /// Seed Data in database
    /// </summary>
    /// <param name="entityStores"><see cref="IEntityStoreFactory"/> or inherited version</param>    
    public void Seed(IEntityStoreFactory entityStores);
}
