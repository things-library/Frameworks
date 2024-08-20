using System.Security.Authentication;

using ThingsLibrary.Entity.Interfaces;
using ThingsLibrary.Entity.Mongo.Models;
using ThingsLibrary.Entity.Types;

namespace ThingsLibrary.Entity.Mongo;

/// <summary>
/// Mongo Entity Store Wrapper
/// </summary>
/// <remarks>
/// Reference: https://www.mongodb.com/blog/post/quick-start-c-sharp-and-mongodb-starting-and-setup
/// </remarks>
public class EntityStoreFactory : IEntityStoreFactory
{
    /// <summary>
    /// Type of entity store
    /// </summary>
    public EntityStoreType StoreType => EntityStoreType.MongoDb;

    /// <summary>
    /// Entity Store Options
    /// </summary>
    public Models.EntityStoreOptions Options { get; init; }
    
    /// <summary>
    /// Constructor
    /// </summary>
    /// <param name="connectionString">Connection String</param>
    /// <param name="defaultDatabaseName">Database Name</param>
    public EntityStoreFactory(Models.EntityStoreOptions options)
    {
        ArgumentNullException.ThrowIfNullOrWhiteSpace(options.ConnectionString);

        ValidateDatabaseName(options.DatabaseName);

        this.Options = options;
    }

    /// <inheritdoc />
    public virtual IEntityStore<T> GetStore<T>(string name) where T : class
    {
        return new EntityStore<T>(this.Options, name);
    }

    public virtual EntityStore<T> GetStoreObj<T>(string name) where T : class
    {
        return new EntityStore<T>(this.Options, name);
    }

    /// <summary>
    /// Get store for database name
    /// </summary>
    /// <typeparam name="T">Stored Entity</typeparam>
    /// <param name="databaseName">Database Name</param>
    /// <param name="name">Store Name</param>
    /// <returns></returns>
    /// <exception cref="ArgumentNullException"></exception>
    public virtual IEntityStore<T> GetStore<T>(string databaseName, string name) where T : class
    {
        _ = Guard.Argument(name, nameof(name))
           .NotEmpty()
           .NotNull();

        ValidateDatabaseName(databaseName);

        var options = new EntityStoreOptions(this.Options.ConnectionString, databaseName);
        
        return new EntityStore<T>(options, name);
    }

    /// <inheritdoc />
    public async Task<IEnumerable<string>> GetStoreListAsync(CancellationToken cancellationToken = default)
    {
        return await this.GetStoreListAsync(this.Options.DatabaseName);
    }

    /// <summary>
    /// Get the store list for database
    /// </summary>
    /// <param name="databaseName">Database Name</param>
    /// <param name="cancellationToken">Cancellation Token</param>
    /// <returns></returns>
    public async Task<IEnumerable<string>> GetStoreListAsync(string databaseName, CancellationToken cancellationToken = default)
    {
        var client = this.GetClient();

        var database = client.GetDatabase(databaseName);

        var response = await database.ListCollectionNamesAsync(null, cancellationToken);

        return response.ToList();
    }

    /// <inheritdoc />
    public async Task DeleteStoreAsync(string name, CancellationToken cancellationToken = default)
    {
        await this.DeleteStoreAsync(this.Options.DatabaseName, name, cancellationToken);        
    }

    /// <inheritdoc />
    public async Task DeleteStoreAsync(string databaseName, string name, CancellationToken cancellationToken = default)
    {
        _ = Guard.Argument(databaseName, nameof(databaseName))
           .NotEmpty()
           .NotNull();

        _ = Guard.Argument(name, nameof(name))
           .NotEmpty()
           .NotNull();

        var client = this.GetClient();

        var database = client.GetDatabase(databaseName);

        await database.DropCollectionAsync(name, cancellationToken);
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
        return new MongoClient(this.Options.ConnectionString);        
    }


    /// <summary>
    /// Validate the database name
    /// </summary>
    /// <param name="name">Database Name</param>
    /// <exception cref="ArgumentException">When name is not valid</exception>
    public static void ValidateDatabaseName(string name)
    {
        _ = Guard.Argument(name, nameof(name))
            .NotEmpty()
            .NotNull();

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
        _ = Guard.Argument(name, nameof(name))
            .NotEmpty()
            .NotNull()
            .ThrowIf(argument => argument.Value.StartsWith("system."), argument => new ArgumentException("Name can not begin with 'system.'"));
                
        // validate name        
        //if (!name.All(c => char.IsLetterOrDigit(c))) { throw new ArgumentException($"Table name '{name}' can only be alphanumeric characters.", nameof(name)); }
        //if (name.Length < 3 || name.Length > 63) { throw new ArgumentException($"Table name '{name}' must be between 3 and 63 characters long.", nameof(name)); }
        //if (!char.IsLetter(name[0])) { throw new ArgumentException($"Table name '{name}' must start with a letter", nameof(name)); }
    }
}