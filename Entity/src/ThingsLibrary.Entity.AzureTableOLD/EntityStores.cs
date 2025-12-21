using Azure.Data.Tables;

namespace Starlight.Entity.AzureTable;

public class EntityStores
{        
    private string StorageConnectionString { get; set; }

    private Az.TableServiceClient TableServiceClient { get; set; }

    /// <summary>
    /// Constructor
    /// </summary>
    /// <param name="storageConnectionString"></param>
    public EntityStores(string storageConnectionString)
    {
        this.StorageConnectionString = storageConnectionString;
        this.TableServiceClient = new Az.TableServiceClient(this.StorageConnectionString);            
    }

    /// <inheritdoc />
    public TableClient GetStore(string name)
    {
        // validate name
        EntityStores.ValidateTableName(name);

        // create the vendor specific object
        var tableClient = new Az.TableClient(this.StorageConnectionString, name);

        // ensure that the table exists
        tableClient.CreateIfNotExists();

        return tableClient;
    }

    /// <inheritdoc />
    public IEnumerable<string> GetStoreList(CancellationToken cancellationToken = default)
    {
        var results = this.TableServiceClient.Query(filter: "", cancellationToken: cancellationToken);

        return results.Select(x => x.Name).ToList();            
    }

    /// <inheritdoc />
    public void DeleteStore(string name, CancellationToken cancellationToken = default)
    {
        this.TableServiceClient.DeleteTable(name, cancellationToken);
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
