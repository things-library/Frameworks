using System.Security.Authentication;
using ThingsLibrary.Entity.Interfaces;
using ThingsLibrary.Entity.Types;

namespace ThingsLibrary.Entity.Mongo;

/// <summary>
/// Mongo Entity Store Wrapper
/// </summary>
/// <remarks>
/// Reference: https://www.mongodb.com/blog/post/quick-start-c-sharp-and-mongodb-starting-and-setup
/// </remarks>
public class EntityStores : IEntityStores
{
    /// <summary>
    /// Type of entity store
    /// </summary>
    public EntityStoreType StoreType => EntityStoreType.MongoDb;

    private string ConnectionString { get; init; }
    private bool IsUrl { get; init; }

    /// <summary>
    /// Default Database Name
    /// </summary>
    public string DefaultDatabaseName { get; init; }

    
    /// <summary>
    /// Constructor
    /// </summary>
    /// <param name="connectionStringUrl">Connection String Url</param>
    /// <param name="defaultDatabaseName">Database Name</param>
    public EntityStores(Uri connectionStringUrl, string defaultDatabaseName)
    {
        if (connectionStringUrl == null) { throw new ArgumentNullException(nameof(connectionStringUrl)); }
        if (string.IsNullOrEmpty(defaultDatabaseName)) { throw new ArgumentNullException(nameof(defaultDatabaseName)); }

        ValidateDatabaseName(defaultDatabaseName);

        this.ConnectionString = connectionStringUrl.OriginalString;
        this.IsUrl = true;

        this.DefaultDatabaseName = defaultDatabaseName;
    }

    /// <summary>
    /// Constructor
    /// </summary>
    /// <param name="connectionString">Connection String</param>
    /// <param name="defaultDatabaseName">Database Name</param>
    public EntityStores(string connectionString, string defaultDatabaseName)
    {
        if (string.IsNullOrEmpty(connectionString)) { throw new ArgumentNullException(nameof(connectionString)); }
        if (string.IsNullOrEmpty(defaultDatabaseName)) { throw new ArgumentNullException(nameof(defaultDatabaseName)); }

        ValidateDatabaseName(defaultDatabaseName);

        this.ConnectionString = connectionString;
        this.DefaultDatabaseName = defaultDatabaseName;                
    }

    /// <inheritdoc />
    public IEntityStore<T> GetStore<T>(string name) where T : class
    {
        return new EntityStore<T>(this.ConnectionString, this.DefaultDatabaseName, name);
    }

    /// <summary>
    /// Get store for database name
    /// </summary>
    /// <typeparam name="T">Stored Entity</typeparam>
    /// <param name="databaseName">Database Name</param>
    /// <param name="name">Store Name</param>
    /// <returns></returns>
    /// <exception cref="ArgumentNullException"></exception>
    public IEntityStore<T> GetStore<T>(string databaseName, string name) where T : class
    {
        if (string.IsNullOrEmpty(databaseName)) { throw new ArgumentNullException(nameof(databaseName)); }
        if (string.IsNullOrEmpty(name)) { throw new ArgumentNullException(nameof(name)); }

        ValidateDatabaseName(databaseName);

        return new EntityStore<T>(this.ConnectionString, databaseName, name);
    }

    /// <inheritdoc />
    public IEnumerable<string> GetStoreList(CancellationToken cancellationToken = default)
    {
        return this.GetStoreList(this.DefaultDatabaseName);
    }

    /// <summary>
    /// Get the store list for database
    /// </summary>
    /// <param name="databaseName">Database Name</param>
    /// <param name="cancellationToken">Cancellation Token</param>
    /// <returns></returns>
    public IEnumerable<string> GetStoreList(string databaseName, CancellationToken cancellationToken = default)
    {
        var client = this.GetClient();

        var database = client.GetDatabase(databaseName);

        return database.ListCollectionNamesAsync().Result.ToList();
    }

    /// <inheritdoc />
    public void DeleteStore(string name, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrEmpty(name)) { throw new ArgumentNullException(nameof(name)); }

        this.DeleteStore(this.DefaultDatabaseName, name, cancellationToken);        
    }

    /// <inheritdoc />
    public void DeleteStore(string databaseName, string name, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrEmpty(databaseName)) { throw new ArgumentNullException(nameof(databaseName)); }
        if (string.IsNullOrEmpty(name)) { throw new ArgumentNullException(nameof(name)); }

        var client = this.GetClient();

        var database = client.GetDatabase(databaseName);

        database.DropCollection(name, cancellationToken);
    }

    #region --- Database ---

    public List<string> GetDatabaseList(CancellationToken cancellationToken = default)
    {
        var client = this.GetClient();

        return client.ListDatabaseNames().ToList();
    }

    /// <summary>
    /// Delete database from server
    /// </summary>
    /// <param name="databaseName">Database name</param>
    /// <param name="cancellationToken">Cancellation Token</param>
    /// <exception cref="ArgumentNullException">Invalid parameters</exception>
    public void DeleteDatabase(string databaseName, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrEmpty(databaseName)) { throw new ArgumentNullException(nameof(databaseName)); }

        var client = this.GetClient();

        client.DropDatabase(databaseName, cancellationToken);
    }

    #endregion

    private MongoClient GetClient()
    {
        if (this.IsUrl)
        {
            var settings = MongoClientSettings.FromUrl(new MongoUrl(this.ConnectionString));
            settings.SslSettings = new SslSettings() { EnabledSslProtocols = SslProtocols.Tls12 };

            return new MongoClient(settings);
        }
        else
        {
            return new MongoClient(this.ConnectionString);
        }
    }


    /// <summary>
    /// Validate the database name
    /// </summary>
    /// <param name="name">Database Name</param>
    /// <exception cref="ArgumentException">When name is not valid</exception>
    public static void ValidateDatabaseName(string name)
    {
        // Invalid Characters: /."*< >:|?$

        // make sure the database name doesn't violate Mongo's naming
        if (name.IndexOfAny("/\\. \"$*<>:|?".ToCharArray()) != -1)
        {
            throw new ArgumentException("Database name can not include any of these characters: /\\. \"$*<>:|?");
        }

        //// make sure the database name doesn't violate Mongo's naming
        //if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux) && name.IndexOfAny("/\\. \"$*<>:|?".ToCharArray()) != -1)
        //{
        //    throw new ArgumentException("Database name can not include any of these characters: /\\. \"$*<>:|?");
        //}
        //else if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows) && name.IndexOfAny("/\\. \"$".ToCharArray()) != -1)
        //{
        //    throw new ArgumentException("Database name can not include any of these characters: /\\. \"$");
        //}
    }

    /// <summary>
    /// Validates the name to be used 
    /// </summary>
    /// <param name="name">Collection Name</param>
    /// <exception cref="ArgumentException"></exception>
    /// <remarks>
    /// Naming Policies:   
    /// </remarks>
    public static void ValidateCollectionName(string name)
    {
        if (string.IsNullOrEmpty(name)) { throw new ArgumentNullException(nameof(name)); }

        if (name.StartsWith("system.")) { throw new ArgumentException("Name can not begin with 'system.'"); }

        // validate name        
        //if (!name.All(c => char.IsLetterOrDigit(c))) { throw new ArgumentException($"Table name '{name}' can only be alphanumeric characters.", nameof(name)); }
        //if (name.Length < 3 || name.Length > 63) { throw new ArgumentException($"Table name '{name}' must be between 3 and 63 characters long.", nameof(name)); }
        //if (!char.IsLetter(name[0])) { throw new ArgumentException($"Table name '{name}' must start with a letter", nameof(name)); }
    }
}