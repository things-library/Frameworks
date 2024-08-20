using ThingsLibrary.Entity.Interfaces;
using ThingsLibrary.Entity.Types;

namespace ThingsLibrary.Entity.AzureTable;

public class EntityStoreFactory : IEntityStoreFactory
{
    /// <inheritdoc />
    public EntityStoreType StoreType => EntityStoreType.Azure_Table;


    private string StorageConnectionString { get; set; }

    /// <summary>
    /// Azure Table Service Client
    /// </summary>
    public Az.TableServiceClient TableServiceClient { get; set; }

    /// <summary>
    /// Constructor
    /// </summary>
    /// <param name="storageConnectionString">Connection String</param>
    public EntityStoreFactory(string storageConnectionString)
    {
        this.StorageConnectionString = storageConnectionString;
        this.TableServiceClient = new Az.TableServiceClient(this.StorageConnectionString);            
    }

    /// <inheritdoc />
    public IEntityStore<T> GetStore<T>(string name) where T : class
    {
        return new EntityStore<T>(this.StorageConnectionString, name);        
    }

    /// <inheritdoc />
    public Task<IEnumerable<string>> GetStoreListAsync(CancellationToken cancellationToken = default)
    {
        var results = this.TableServiceClient.Query(filter: "", cancellationToken: cancellationToken);

        var list = results.Select(x => x.Name).AsEnumerable<string>();

        return Task.FromResult(list);
    }

    /// <inheritdoc />
    public async Task DeleteStoreAsync(string name, CancellationToken cancellationToken = default)
    {
        var result = await this.TableServiceClient.DeleteTableAsync(name, cancellationToken);
        if (result.IsError) { throw new ArgumentException($"Unable to delete store '{name}'."); }
    }

    /// <summary>
    /// Validates the name to be used 
    /// </summary>
    /// <param name="name"></param>
    /// <exception cref="ArgumentException"></exception>
    /// <remarks>
    /// Naming Policies:
    /// 1. Not Empty
    /// 2. Must be between 3 and 63 characters
    /// 3. Must be alphanumeric
    /// 4. Cannot begin with a number
    /// </remarks>
    public static void ValidateTableName(string name)
    {
        if (string.IsNullOrEmpty(name)) { throw new ArgumentNullException(nameof(name)); }

        // validate name
        if (!name.All(c => char.IsLetterOrDigit(c))) { throw new ArgumentException($"Table name '{name}' can only be alphanumeric characters.", nameof(name)); }
        if (name.Length < 3 || name.Length > 63) { throw new ArgumentException($"Table name '{name}' must be between 3 and 63 characters long.", nameof(name)); }
        if (!char.IsLetter(name[0])) { throw new ArgumentException($"Table name '{name}' must start with a letter", nameof(name)); }
    }
}
