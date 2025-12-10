using Serilog;
using ThingsLibrary.Entity.Local;
using ThingsLibrary.DataType.Extensions;
using ThingsLibrary.Entity.Interfaces;
using ThingsLibrary.Entity.Types;

namespace ThingsLibrary.Entity.Local;

public class EntityStore<T> : IEntityStore<T> where T : class
{
    #region --- General ---

    /// <summary>
    /// Entity's Type
    /// </summary>
    public Type Type { get; init; }

    /// <summary>
    /// Entity Keys
    /// </summary>
    public PropertyInfo Key { get; init; }

    /// <summary>
    /// Partition Key (if exists)
    /// </summary>
    public PropertyInfo? PartitionKey { get; init; }

    /// <summary>
    /// Timestamp Field
    /// </summary>
    public PropertyInfo Timestamp { get; }

    /// <summary>
    /// Entity Properties (public / instance)
    /// </summary>
    public Dictionary<string, PropertyInfo> Properties { get; init; }

    /// <summary>
    /// Entity Store Type
    /// </summary>
    public EntityStoreType StoreType => EntityStoreType.Local;

    /// <summary>
    /// Table name
    /// </summary>
    public string Name => this.Collection.Name;

    #endregion

    #region --- Store Specific ---

    public LiteDatabase Client { get; init; }
    public ILiteCollection<T> Collection { get; init; }

    public string FilePath { get; init; }
    public string TableDirPath { get; init; }

    public bool IsMemoryDb => string.IsNullOrEmpty(this.FilePath);
    public bool IsFileSaving { get; init; } = false;        

    #endregion

    /// <summary>
    /// Constructor
    /// </summary>
    /// <param name="storageConnectionString">Storage Connection String</param>
    /// <param name="name">Table Name</param>
    /// <exception cref="ArgumentNullException">Storage connection string, table name must not be null, 'storage_dir_path' must be in connection string.</exception>
    /// <exception cref="ArgumentException">Table name must strat with a latter, be alphanumeric and be 3-63 characters</exception>
    public EntityStore(string storageConnectionString, string name)
    {
        //Connection String: http://www.litedb.org/docs/connection-string/
        //  "Filename=C:\database.db;Password=1234;Upgrade=true;"
        //  "Filename=C:\database.db;Password=1234;Upgrade=true;Files=true"     //Extra property 'files' creates local json files that represent the entities
        //  "Filename=C:\database.db;Password=1234;Upgrade=true;Files=true;RootTableDir=c:\root"     //Extra property 'files' creates local json files that represent the entities
        //  "Filename=:memory:"

        // validate the generic entity
        this.Type = typeof(T);

        // get the key
        this.Key = this.Type.GetProperties().FirstOrDefault(x => x.GetCustomAttributes(typeof(System.ComponentModel.DataAnnotations.KeyAttribute), false).Any());
        if (this.Key == null) { throw new ArgumentException($"Entity {typeof(T).Namespace}.{typeof(T).Name} has no [Key] attribute defined."); }
        if (string.Compare(this.Key.Name, "Id", true) != 0) { throw new ArgumentException("Entity records require a key field named 'id'"); }

        this.PartitionKey = this.Type.GetProperties().FirstOrDefault(x => x.GetCustomAttributes(typeof(Attributes.PartitionKeyAttribute), false).Any());
        this.Timestamp = this.Type.GetProperties().FirstOrDefault(x => x.GetCustomAttributes(typeof(System.ComponentModel.DataAnnotations.TimestampAttribute), false).Any());
        this.Properties = this.Type.GetProperties(BindingFlags.Public | BindingFlags.Instance).Where(x => this.Key != x).ToDictionary(x => x.Name, x => x);

        // validate the parameters
        if (string.IsNullOrEmpty(storageConnectionString)) { throw new ArgumentNullException(nameof(storageConnectionString)); }
        if (string.IsNullOrEmpty(name)) { throw new ArgumentNullException(nameof(name)); }

        // validate name
        EntityStoreFactory.ValidateCollectionName(name);

        // parse out the ProjectId and InstanceId from connectionstring            
        var builder = new System.Data.Common.DbConnectionStringBuilder
        {
            ConnectionString = storageConnectionString
        };

        // validate file name property
        var filePath = builder.GetValue<string>("FileName", null);
        if (string.IsNullOrEmpty(filePath)) { throw new ArgumentException("No 'FileName' found in connection string."); }

        if (!filePath.StartsWith(":"))
        {
            this.FilePath = System.IO.Path.GetFullPath(filePath);
        }

        // validate name            
        if (!name.All(c => char.IsLetterOrDigit(c))) { throw new ArgumentException("Table names can only be alphanumeric characters.", nameof(name)); }
        if (name.Length < 3 || name.Length > 63) { throw new ArgumentException("Table names must be between 3 and 63 characters long.", nameof(name)); }
        if (!char.IsLetter(name[0])) { throw new ArgumentException("Table names must start with a letter", nameof(name)); }

        // connect to database
        Log.Debug("Getting LiteDatabase Client...");
        this.Client = new LiteDatabase(storageConnectionString);

        Log.Debug("Getting Collection: {CollectionName}", name);
        this.Collection = this.Client.GetCollection<T>(name);

        // dates will always be UTC
        this.Client.UtcDate = true;

        // create any indexes that we should have already
        Log.Debug("Creating or verifying indexes...");
        this.CreateIndexes();
                
        // The parameter passed into the expression (myClassRef) in your code
        var parameter = Expression.Parameter(typeof(T), this.Key.Name);

        // Access the property described by the PropertyInfo 'prop' on the parameter
        var propertyAccess = Expression.Property(parameter, this.Key);
        // Since we're returning an 'object', we'll need to make sure we box value types.
        var box = Expression.Convert(propertyAccess, typeof(object));
        // Construct the whole lambda
        var lambda = Expression.Lambda<Func<T, object>>(box, parameter);

        var mapper = BsonMapper.Global;
        mapper.Entity<T>().Id(lambda);
                    
        // see if we are going to be saving out files
        this.IsFileSaving = builder.GetValue<bool>("Files", false);
        if (this.IsFileSaving)
        {
            // validate storage path  
            if (this.IsMemoryDb) { throw new InvalidDataException("In-Memory or temp databases can not be used with file saving."); }

            var rootTableDir = builder.GetValue<string>("RootTableDir", null);
            if (!string.IsNullOrEmpty(rootTableDir))
            {
                rootTableDir = System.IO.Path.GetFullPath(rootTableDir);
            }
            else
            {
                // get the pathing to the table
                rootTableDir = Path.GetDirectoryName(this.FilePath);                    
            }

            this.TableDirPath = Path.Combine(rootTableDir, this.Name);

            // create the root table named folder in the same folder as the database file
            IO.Directory.VerifyPath(this.TableDirPath);
            if(!System.IO.Directory.Exists(this.TableDirPath)) { throw new IOException($"Unable to create table directory '{this.TableDirPath}'."); }
        }            
    }

    #region --- Indexes ---

    private void CreateIndexes()
    {
        // KEY INDEX (if not 'Id' field name, aka: existing index)                
        if (string.Compare(this.Key.Name, "id", true) != 0) 
        {
            Log.Debug("+ Key Index: {IndexName}", this.Key.Name);
            this.Collection.EnsureIndex(this.Key.Name, unique: true);
        }

        // Partition index.. how the data is partitioned
        if (this.PartitionKey != null)
        {
            // Create PartitionKey index
            // ======================================================================
            Log.Debug("+ Partition Index: {IndexName}", this.PartitionKey.Name);
            this.Collection.EnsureIndex(this.PartitionKey.Name);
        }
        
        // create indexes for the Indexes tagged on the entity
        var indexAttributes = (Attributes.IndexAttribute[])Attribute.GetCustomAttributes(this.Type, typeof(Attributes.IndexAttribute));
        foreach (var indexAttribute in indexAttributes)
        {
            this.CreateIndex(indexAttribute);
        }
    }
        
    private void CreateIndex(Attributes.IndexAttribute indexAttribute)
    {
        // don't create a index for a field we already would have a unique key for
        if (indexAttribute.PropertyNames.Count == 1)
        {
            if (string.Compare(indexAttribute.PropertyNames[0], this.Key.Name, true) == 0) { return; }
            if (string.Compare(indexAttribute.PropertyNames[0], "Id", true) == 0) { return; }
            if (this.PartitionKey != null && string.Compare(indexAttribute.PropertyNames[0], this.PartitionKey.Name, true) == 0) { return; }
        }

        try
        {            
            if(indexAttribute.PropertyNames.Count > 1)
            {
                Log.Warning("LiteDB does not support multi-field indexes.  Creating one index per field instead!");
            }

            indexAttribute.PropertyNames.ForEach(propertyName =>
            {
                Log.Debug("+ Index: {PropertyName}", propertyName);
                this.Collection.EnsureIndex(propertyName);
            });            
        }
        catch (Exception ex)
        {
            Log.Error(ex, ex.Message);
        }
    }

    #endregion

    #region --- Entitites ---

    private BsonValue GetKeyFilter(T entity)
    {
        if (entity == default) { throw new ArgumentNullException(nameof(entity)); }

        var key = this.Key.GetValue(entity);
        
        return new BsonValue(key);        
    }

    private BsonValue GetKeyFilter(object key, object partitionKey)
    {
        if (key == default) { throw new ArgumentNullException(nameof(key)); }

        return new BsonValue(key);        
    }

    /// <inheritdoc />
    public IEnumerable<T> GetEntities(DateTime? sinceDate = null)
    {
        if(sinceDate != null)
        {
            return this.Collection.Find(Query.GTE("Timestamp", sinceDate));
        }
        else
        {
            return this.Collection.FindAll();
        }            
    }

    /// <inheritdoc />
    public T GetEntity(object key, object partitionKey)
    {
        var lookupId = this.GetKeyFilter(key, partitionKey);

        return this.Collection.FindById(lookupId);
    }

    /// <inheritdoc />
    public void InsertEntity(T entity)
    {
        var lookupId = this.GetKeyFilter(entity);

        // set timestamp variable
        this.SetTimestamp(entity);

        try
        {
            this.Collection.Insert(lookupId, entity);
        }
        catch (LiteException ex)
        {
            throw new IOException(ex.Message);
        }

        //save out entity?
        if (this.IsFileSaving)
        {
            this.SaveEntityFile(entity);
        }
    }

    /// <inheritdoc />
    public void UpdateEntity(T entity)
    {
        var lookupId = this.GetKeyFilter(entity);

        // set timestamp variable
        this.SetTimestamp(entity);

        if (this.Collection.FindById(lookupId) == null)
        {
            throw new IOException("No existing record to update.");
        }

        // creates (if missing) or replaces entity
        this.Collection.Update(lookupId, entity);

        //save out entity?
        if (this.IsFileSaving)
        {
            this.SaveEntityFile(entity);
        }
    }

    /// <inheritdoc />
    public void UpsertEntity(T entity)
    {
        var lookupId = this.GetKeyFilter(entity);

        // set timestamp variable
        this.SetTimestamp(entity);
        
        // creates (if missing) or replaces entity
        this.Collection.Upsert(lookupId, entity);

        //save out entity?
        if (this.IsFileSaving)
        {
            this.SaveEntityFile(entity);
        }
    }

    /// <inheritdoc />
    public void DeleteEntity(object key, object partitionKey)
    {
        var lookupId = this.GetKeyFilter(key, partitionKey);

        this.Collection.Delete(lookupId);
    }


    /// <inheritdoc />
    private void SetTimestamp(T entity)
    {
        //nothing to do?
        if (this.Timestamp == null) { return; }

        if (this.Timestamp.PropertyType == typeof(DateTime))
        {
            this.Timestamp.SetValue(entity, DateTime.UtcNow);
        }
        else if (this.Timestamp.PropertyType == typeof(DateTimeOffset))
        {
            this.Timestamp.SetValue(entity, DateTimeOffset.UtcNow);
        }
        else if (this.Timestamp.PropertyType == typeof(short))
        {
            // EPOCH (DAY)
            this.Timestamp.SetValue(entity, DataType.UnitConvert.ToUnixDay(DateTime.UtcNow));
        }
        else if (this.Timestamp.PropertyType == typeof(long))
        {
            // EPOCH (SECONDS)
            this.Timestamp.SetValue(entity, DateTimeOffset.UtcNow.ToUnixTimeSeconds());
        }
        else
        {
            throw new ArgumentException("Timestamp not DateTime or DateTimeOffset");
        }
    }

    /// <summary>
    /// Save out the entity as a file into the file system
    /// </summary>
    /// <param name="entity"></param>
    /// <remarks>
    /// File Pathing: {TableDirPath}\{Key}.json;         
    //  </remarks>
    private void SaveEntityFile(T entity)
    {        
        var key = this.Key.GetValue(entity);

        var filePath = Path.Combine($"{this.TableDirPath}", $"{key}.json");

        var options = new JsonSerializerOptions { WriteIndented = true };
                   
        var json = System.Text.Json.JsonSerializer.Serialize(entity, options);

        //save out entity
        File.WriteAllText(filePath, json);
    }

    #endregion

    /// <inheritdoc />       
    public void DeleteStore()
    {
        this.DeleteStore(true);
    }

    /// <summary>
    /// Removes the entity store
    /// </summary>        
    /// <param name="includeTableDir">If the working table directory</param>
    public void DeleteStore(bool includeTableDir)
    {
        this.Client.DropCollection(this.Name);

        if (this.IsFileSaving && !string.IsNullOrEmpty(this.TableDirPath))
        {                
            // remove the directory itself
            ThingsLibrary.IO.Directory.TryDeleteDirectory(this.TableDirPath);
            ThingsLibrary.IO.Directory.VerifyPath(this.TableDirPath);    //restore the base folder
        }
    }

    /// <inheritdoc />       
    public Task DeleteStoreAsync(CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }

    /// <inheritdoc />       
    public bool Exists(object key, object partitionKey)
    {
        var lookupId = this.GetKeyFilter(key, partitionKey);

        return (this.Collection.FindById(lookupId) != null);
    }

    /// <inheritdoc />       
    public Task<bool> ExistsAsync(object key, object partitionKey, CancellationToken cancellationToken = default)
    {
        var lookupId = this.GetKeyFilter(key, partitionKey);

        return Task.FromResult<bool>(this.Collection.FindById(lookupId) != null);        
    }

    /// <inheritdoc />       
    public Task<T> GetEntityAsync(object key, object partitionKey, CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }

    /// <inheritdoc />       
    public Task<IEnumerable<T>> GetEntitiesAsync(DateTime? sinceDate, CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }

    /// <inheritdoc />       
    public Task InsertEntityAsync(T entity, CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }

    /// <inheritdoc />       
    public Task UpdateEntityAsync(T entity, CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }

    /// <inheritdoc />       
    public Task UpsertEntityAsync(T entity, CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }

    /// <inheritdoc />       
    public void BulkUpsert(IEnumerable<T> records)
    {
        throw new NotImplementedException();
    }

    /// <inheritdoc />       
    public Task BulkUpsertAsync(IEnumerable<T> records, CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }

    /// <inheritdoc />       
    public Task DeleteEntityAsync(object key, object partitionKey, CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }

    #region --- IDisposable ---

    private bool _disposed = false;

    public void Dispose()
    {
        this.Dispose(true);
        GC.SuppressFinalize(this);
    }

    protected virtual void Dispose(bool disposing)
    {
        // Cleanup
        if (_disposed) { return; }

        _disposed = true;   // we are handling the disposing in this thread so don't let others do the same thing

        if (disposing)
        {
            // free up objects

            // set up the GCP objects
            this.Client.Dispose();                
        }
    }    

    ~EntityStore()
    {
        this.Dispose(false);
    }

    #endregion
}
