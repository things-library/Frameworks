using ThingsLibrary.Entity.Types;

namespace ThingsLibrary.Entity.Interfaces;

public interface IEntityStoreFactory
{
    /// <summary>
    /// Type of entity store (Azure_Table, GCP_DataStore)
    /// </summary>
    public EntityStoreType StoreType { get; }

    /// <summary>
    /// Get/Create Entity Store
    /// </summary>        
    /// <returns>List of matching entities</returns>
    public IEntityStore<T> GetStore<T>(string name) where T : class;

    /// <summary>
    /// Get a listing of stores
    /// </summary>
    /// <param name="cancellationToken">Cancellation Token</param>
    /// <returns></returns>
    public Task<IEnumerable<string>> GetStoreListAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Delete Store
    /// </summary>
    /// <param name="name">Store Name</param>
    /// <param name="cancellationToken">Cancellation Token</param>
    public Task DeleteStoreAsync(string name, CancellationToken cancellationToken = default);    
}
