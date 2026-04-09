using System.Collections.Concurrent;

using ThingsLibrary.Entity.Interfaces;
using ThingsLibrary.Entity.Types;

namespace ThingsLibrary.Entity.Mongo;

/// <summary>
/// Mongo Collection Wrapper
/// </summary>
/// <typeparam name="T"></typeparam>
public class EntityStore<T> : IEntityStore<T> where T : class
{
    /// <summary>
    /// Entity Store Type
    /// </summary>
    public EntityStoreType StoreType => EntityStoreType.MongoDb;

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
    public PropertyInfo? Timestamp { get; }

    /// <summary>
    /// Database Name
    /// </summary>
    public string DatabaseName { get; init; }

    /// <summary>
    /// Table name
    /// </summary>
    public string Name { get; init; }

    /// <summary>
    /// Mongo Client
    /// </summary>
    public IMongoClient Client { get; init; }

    /// <summary>
    /// Mongo Database
    /// </summary>
    public IMongoDatabase Database { get; init; }

    /// <summary>
    /// Mongo Collection of items
    /// </summary>
    public IMongoCollection<T> Collection { get; init; }

    /// <summary>
    /// Constructor
    /// </summary>
    /// <param name="connectionString">Storage Connection String</param>
    /// <param name="databaseName">Database Name</param>
    /// <param name="name">Table Name</param>
    /// <exception cref="ArgumentNullException">Storage connection string, table name must not be null.</exception>
    /// <exception cref="ArgumentException">Table name must strat with a latter, be alphanumeric and be 3-63 characters</exception>
    public EntityStore(string connectionString, string databaseName, string name)
    {
        Log.Information("Init MongoDB Database: {DatabaseName}, Store: {StoreName}", databaseName, name);

        // validate parameters (name is validated in ValidateTableName)
        if (string.IsNullOrEmpty(connectionString)) { throw new ArgumentNullException(nameof(connectionString)); }
        if (string.IsNullOrEmpty(databaseName)) { throw new ArgumentNullException(nameof(databaseName)); }
        if (string.IsNullOrEmpty(name)) { throw new ArgumentNullException(nameof(name)); }

        // validate name
        EntityStores.ValidateCollectionName(name);

        // validate the generic entity (good to do this once on store creation instead of each Insert/Update, etc)
        this.Type = typeof(T);
        this.Key = this.Type.GetProperties().FirstOrDefault(x => x.GetCustomAttributes(typeof(System.ComponentModel.DataAnnotations.KeyAttribute), false).Any()) ?? throw new ArgumentException($"Entity {typeof(T).Namespace}.{typeof(T).Name} has no [Key] attribute defined.");
        
        //Mongo requires a field to be named 'id' in order to serialize and deserialize easily
        if (this.Type.GetProperty("Id") == null)
        {
            throw new ArgumentException("Mongo entity records require a 'Id' field even if not used as the primary key.");
        }

        this.PartitionKey = this.Type.GetProperties().FirstOrDefault(x => x.GetCustomAttributes(typeof(Attributes.PartitionKeyAttribute), false).Any());
        this.Timestamp = this.Type.GetProperties().FirstOrDefault(x => x.GetCustomAttributes(typeof(System.ComponentModel.DataAnnotations.TimestampAttribute), false).Any());

        // set the core properties            
        this.DatabaseName = databaseName;
        this.Name = name;

        // create the vendor specific object
        Log.Debug("Getting MongoDB Client...");
        this.Client = new MongoClient(connectionString);

        Log.Debug("Getting database {DatabaseName}", this.DatabaseName);
        this.Database = this.Client.GetDatabase(this.DatabaseName);
        
        Log.Debug("Getting Collection: {CollectionName}", this.Name);
        this.Collection = this.Database.GetCollection<T>(this.Name);

        // create any indexes that we should have already
        Log.Debug("Creating or verifying indexes...");
        this.CreateIndexes();

        this.LogIndexes();
    }

    #region --- Indexes ---

    private void LogIndexes()
    {
        Log.Debug("Indexes:");
        var indexes = this.Collection.Indexes.List().ToList();
        foreach (var index in indexes)
        {
            Log.Debug("- {IndexName} ({IndexKey})", index.GetElement("name").Value, index.GetElement("key").Value);
        }
    }

    private void CreateIndexes()
    {
        // KEY INDEX (if not 'Id' field name, aka: existing index)        
        this.CreateKeyIndex();

        // Partition index.. how the data is partitioned
        this.CreatePartitionIndex();

        // create indexes for the Indexes tagged on the entity
        var indexAttributes = (Attributes.IndexAttribute[])Attribute.GetCustomAttributes(this.Type, typeof(Attributes.IndexAttribute));
        foreach (var indexAttribute in indexAttributes)
        {
            this.CreateIndex(indexAttribute);
        }
    }

    private void CreateKeyIndex()
    {
        // easy exit? (key is named id and id always has a index)
        if (string.Compare(this.Key.Name, "id", true) == 0) { return; }

        try
        {
            Log.Debug("+ Key Index: {IndexName}", this.Key.Name);

            IndexKeysDefinition<T> keys = "{ " + this.Key.Name + ": 1 }";
            var indexModel = new CreateIndexModel<T>(keys, new CreateIndexOptions
            {
                Name = this.Key.Name,
                Unique = true
            });

            this.Collection.Indexes.CreateOne(indexModel);
        }
        catch (Exception ex)
        {
            Log.Error(ex, ex.Message);
        }
    }

    private void CreatePartitionIndex()
    {
        // easy exit?
        if (this.PartitionKey == null) { return; }

        try
        {
            // Create PartitionKey index
            // ======================================================================
            Log.Debug("+ Partition Index: {IndexName}", this.PartitionKey.Name);

            IndexKeysDefinition<T> keys = "{ " + this.PartitionKey.Name + ": 1 }";
            var indexModel = new CreateIndexModel<T>(keys, new CreateIndexOptions
            {
                Name = this.PartitionKey.Name,
                Unique = false
            });

            this.Collection.Indexes.CreateOne(indexModel);

            //// Create PartitionKey + ID unique index
            //// ======================================================================
            //var indexName = $"{this.PartitionKey.Name}_{this.Key.Name}";

            //Log.Debug("+ Partition Index: {IndexName} (unique)", indexName);

            //keys = $"{{ {this.PartitionKey.Name}: 1, {this.Key.Name}: 1 }}";
            //indexModel = new CreateIndexModel<T>(keys, new CreateIndexOptions
            //{
            //    Name = indexName,
            //    Unique = true
            //});

            //this.Collection.Indexes.CreateOne(indexModel);
        }
        catch (Exception ex)
        {
            Log.Error(ex, ex.Message);
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
            bool[] isDescending = indexAttribute.IsDescending ?? new bool[indexAttribute.PropertyNames.Count];

            int i = 0;
            List<string> keys = new();
            foreach (var propertyName in indexAttribute.PropertyNames)
            {
                keys.Add($"{propertyName}: {(isDescending[i] ? "-1" : "1")}");
                i++;
            }

            // set a name for this index using a naming schema that we expect
            if (string.IsNullOrEmpty(indexAttribute.Name))
            {
                indexAttribute.Name = string.Join("_", indexAttribute.PropertyNames);
            }

            IndexKeysDefinition<T> keyDef = "{ " + string.Join(", ", keys) + " }";
            var indexModel = new CreateIndexModel<T>(keyDef, new CreateIndexOptions
            {
                Unique = (indexAttribute.IsUniqueHasValue && indexAttribute.IsUnique),
                Name = indexAttribute.Name
            });

            Log.Debug("+ Index: {IndexName}", indexAttribute.Name);
            _ = this.Collection.Indexes.CreateOne(indexModel);
        }
        catch (Exception ex)
        {
            Log.Error(ex, ex.Message);
        }
    }

    #endregion

    private FilterDefinition<T> GetKeyFilter(object key, object? partitionKey)
    {
        if (key == default) { throw new ArgumentNullException(nameof(key)); }

        var filter = Builders<T>.Filter.Eq(this.Key.Name, key);
        if (this.PartitionKey != null)
        {
            filter &= Builders<T>.Filter.Eq(this.PartitionKey.Name, partitionKey);
        }

        return filter;
    }

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

    /// <inheritdoc />
    public bool Exists(object key, object partitionKey)
    {
        if (key == default) { throw new ArgumentException("Unable to find non-default key value."); }

        var filter = this.GetKeyFilter(key, partitionKey);

        return this.Collection.Find(filter).Any();
    }

    /// <inheritdoc />
    public async Task<bool> ExistsAsync(object key, object partitionKey, CancellationToken cancellationToken = default)
    {
        if (key == default) { throw new ArgumentException("Unable to find non-default key value."); }

        var filter = this.GetKeyFilter(key, partitionKey);

        return await this.Collection.Find(filter).AnyAsync(cancellationToken);
    }

    /// <inheritdoc />
    public T GetEntity(object key, object partitionKey)
    {
        Log.Information("GET Entity: {EntityKey}", key);

        var filter = this.GetKeyFilter(key, partitionKey);

        return this.Collection.Find(filter).FirstOrDefault();
    }

    /// <inheritdoc />
    public async Task<T> GetEntityAsync(object key, object partitionKey, CancellationToken cancellationToken = default)
    {
        Log.Information("GET Entity: {EntityKey}", key);

        var filter = this.GetKeyFilter(key, partitionKey);

        return await this.Collection.Find(filter).FirstOrDefaultAsync(cancellationToken);
    }

    /// <inheritdoc />
    public IEnumerable<T> GetEntities(DateTime? sinceDate = default)
    {
        if (sinceDate == null)
        {
            Log.Information("GET All Entities");

            return this.Collection.Find(_ => true).ToEnumerable();
        }
        else
        {
            if (this.Timestamp == default) { throw new ArgumentException("No timestamp attribute defined."); }

            Log.Information("GET Entities Since {SinceDate}", sinceDate);

            var filter = Builders<T>.Filter.Gte(this.Timestamp.Name, new BsonDateTime(sinceDate.Value));

            return this.Collection.Find<T>(filter).ToEnumerable();
        }
    }

    /// <inheritdoc />
    public async Task<IEnumerable<T>> GetEntitiesAsync(DateTime? sinceDate = default, CancellationToken cancellationToken = default)
    {
        if (sinceDate == null)
        {
            Log.Information("GET All Entities");

            var items = await this.Collection.FindAsync(_ => true);
            
            return items.ToEnumerable();
        }
        else
        {
            if (this.Timestamp == default) { throw new ArgumentException("No timestamp attribute defined."); }

            Log.Information("Getting Entities Since {SinceDate}", sinceDate);

            var filter = Builders<T>.Filter.Gte(this.Timestamp.Name, new BsonDateTime(sinceDate.Value));
                        
            var items = await this.Collection.FindAsync<T>(filter, null, cancellationToken);
            
            return items.ToEnumerable();
        }
    }


    /// <inheritdoc />
    public void InsertEntity(T entity)
    {
        if (entity == default) { throw new ArgumentNullException(nameof(entity)); }

        var key = this.Key.GetValue(entity);
        if (key == null) { throw new ArgumentException("Unable to find non-default key value."); }

        Log.Information("+ Entity {EntityKey}.", key);

        this.SetTimestamp(entity);

        try
        {
            this.Collection.InsertOne(entity);
        }
        catch (MongoWriteException ex)
        {
            throw new IOException(ex.Message, ex);
        }
        catch (InvalidOperationException ex)
        {
            throw new IOException(ex.Message, ex);
        }
    }

    /// <inheritdoc />
    public async Task InsertEntityAsync(T entity, CancellationToken cancellationToken = default)
    {
        if (entity == default) { throw new ArgumentNullException(nameof(entity)); }

        var key = this.Key.GetValue(entity);
        if (key == null) { throw new ArgumentException("Unable to find non-default key value."); }

        Log.Information("+ Entity {EntityKey}.", key);

        this.SetTimestamp(entity);

        try
        {
            var options = new InsertOneOptions();
            await this.Collection.InsertOneAsync(entity, options, cancellationToken);
        }
        catch (MongoWriteException ex)
        {
            throw new IOException(ex.Message, ex);
        }
    }


    /// <inheritdoc />
    public void UpdateEntity(T entity)
    {
        if (entity == default) { throw new ArgumentNullException(nameof(entity)); }

        var key = this.Key.GetValue(entity);
        if (key == null) { throw new ArgumentException("Unable to find non-default key value."); }

        Log.Information("~ Entity {EntityKey}.", key);

        object? partitionKey = null;
        if (this.PartitionKey != null)
        {
            partitionKey = this.PartitionKey.GetValue(entity);
        }

        var filter = this.GetKeyFilter(key, partitionKey);

        this.SetTimestamp(entity);
                
        var response = this.Collection.ReplaceOne(filter, entity, new ReplaceOptions()
        {
            IsUpsert = false
        });

        if (response.MatchedCount != 1)
        {
            throw new IOException("No matching record to update/replace.");
        }
    }

    /// <inheritdoc />
    public async Task UpdateEntityAsync(T entity, CancellationToken cancellationToken = default)
    {
        if (entity == default) { throw new ArgumentNullException(nameof(entity)); }

        var key = this.Key.GetValue(entity);
        if (key == null) { throw new ArgumentException("Unable to find non-default key value."); }

        Log.Information("+ Entity {EntityKey}.", key);
        
        object? partitionKey = null;
        if (this.PartitionKey != null)
        {
            partitionKey = this.PartitionKey.GetValue(entity);
        }

        var filter = this.GetKeyFilter(key, partitionKey);

        this.SetTimestamp(entity);
                
        try
        {
            var response = await this.Collection.ReplaceOneAsync(filter, entity, new ReplaceOptions()
            {
                IsUpsert = false
            }, cancellationToken);

            if (response.MatchedCount != 1)
            {
                throw new IOException("No matching record to update/replace.");
            }
        }
        catch (MongoWriteException ex)
        {
            throw new IOException(ex.Message, ex);
        }
    }

    /// <inheritdoc />
    public void UpsertEntity(T entity)
    {
        if (entity == default) { throw new ArgumentNullException(nameof(entity)); }

        var key = this.Key.GetValue(entity);
        if (key == default) { throw new ArgumentException("Unable to find non-default key value."); }

        Log.Information("+ Entity {EntityKey}.", key);

        object? partitionKey = null;
        if (this.PartitionKey != null)
        {
            partitionKey = this.PartitionKey.GetValue(entity);
        }

        var filter = this.GetKeyFilter(key, partitionKey);

        this.SetTimestamp(entity);

        try
        {
            _ = this.Collection.ReplaceOne(filter, entity, new ReplaceOptions()
            {
                IsUpsert = true
            });
        }
        catch (MongoWriteException ex)
        {
            throw new IOException(ex.Message, ex);
        }
    }

    /// <inheritdoc />
    public async Task UpsertEntityAsync(T entity, CancellationToken cancellationToken = default)
    {
        if (entity == default) { throw new ArgumentNullException(nameof(entity)); }

        var key = this.Key.GetValue(entity);
        if (key == default) { throw new ArgumentException("Unable to find non-default key value."); }

        object? partitionKey = null;
        if (this.PartitionKey != null)
        {
            partitionKey = this.PartitionKey.GetValue(entity);
        }

        var filter = this.GetKeyFilter(key, partitionKey);

        this.SetTimestamp(entity);

        try
        {
            _ = await this.Collection.ReplaceOneAsync(filter, entity, new ReplaceOptions()
            {
                IsUpsert = true
            }, cancellationToken);
        }
        catch (MongoWriteException ex)
        {
            throw new IOException(ex.Message, ex);
        }
    }

    /// <inheritdoc />
    public void BulkUpsert(IEnumerable<T> records)
    {
        if (records == null) { throw new ArgumentNullException(nameof(records)); }
        if (!records.Any()) { return; } //nothing to do

        Log.Information("++ Bulk Upserting {UpsertCount} Entity Records.", records.Count());

        var bulkOps = new ConcurrentBag<WriteModel<T>>();

        Parallel.ForEach(records, record =>
        {
            var filter = Builders<T>.Filter.Eq(this.Key.Name, this.Key.GetValue(record));
            if (this.PartitionKey != null)
            {
                filter &= Builders<T>.Filter.Eq(this.PartitionKey.Name, this.PartitionKey.GetValue(record));
            }

            var upsertOne = new ReplaceOneModel<T>(filter, record) { IsUpsert = true };
            bulkOps.Add(upsertOne);

            // make sure to timestamp every record
            this.SetTimestamp(record);
        });

        try
        {
            var results = this.Collection.BulkWrite(bulkOps);

            Log.Information("Bulk Upsert Result:");
            Log.Information("+ Upserts: {BulkUpsertCount}", results.Upserts.Count);
            Log.Information("~ Modified: {BulkModifiedCount}", results.ModifiedCount);
            Log.Information("= Matched: {BulkMatchedCount}", results.MatchedCount);
        }
        catch (MongoWriteException ex)
        {
            throw new IOException(ex.Message, ex);
        }
    }

    /// <inheritdoc />
    public async Task BulkUpsertAsync(IEnumerable<T> records, CancellationToken cancellationToken = default)
    {
        // nothing to insert?
        if (records == null) { throw new ArgumentNullException(nameof(records)); }
        if (!records.Any()) { return; } //nothing to do

        Log.Information("++ Bulk Upserting {UpsertCount} Entity Records.", records.Count());

        var bulkOps = new ConcurrentBag<WriteModel<T>>();

        Parallel.ForEach(records, record =>
        {
            var filter = Builders<T>.Filter.Eq(this.Key.Name, this.Key.GetValue(record));
            if (this.PartitionKey != null)
            {
                filter &= Builders<T>.Filter.Eq(this.PartitionKey.Name, this.PartitionKey.GetValue(record));
            }

            var upsertOne = new ReplaceOneModel<T>(filter, record) { IsUpsert = true };

            bulkOps.Add(upsertOne);

            // make sure to timestamp every record
            this.SetTimestamp(record);
        });

        try
        {
            var results = await this.Collection.BulkWriteAsync(bulkOps, null, cancellationToken);

            Log.Information("Bulk Upsert Result:");
            Log.Information("+ Upserts: {BulkUpsertCount}", results.Upserts.Count);
            Log.Information("~ Modified: {BulkModifiedCount}", results.ModifiedCount);
            Log.Information("= Matched: {BulkMatchedCount}", results.MatchedCount);
        }
        catch (MongoWriteException ex)
        {
            throw new IOException(ex.Message, ex);
        }        
    }

    /// <inheritdoc />
    public void DeleteEntity(object key, object partitionKey)
    {
        if (key == default) { throw new ArgumentNullException(nameof(key)); }

        this.DeleteEntity(key, partitionKey, false);
    }

    /// <inheritdoc />
    public void DeleteEntity(object key, object partitionKey, bool errorIfMissing)
    {
        Log.Information("- DELETE Entity {EntityKey}.", key);

        var filter = this.GetKeyFilter(key, partitionKey);

        var response = this.Collection.DeleteOne(filter);
        if (errorIfMissing && response.DeletedCount == 0)
        {
            Log.Information("No entity found to delete for id: {EntityKey}", key);

            throw new ArgumentException("No entity found to delete for id: {key}");
        }
    }

    /// <inheritdoc />
    public async Task DeleteEntityAsync(object key, object partitionKey, CancellationToken cancellationToken = default)
    {
        if (key == default) { throw new ArgumentNullException(nameof(key)); }

        await this.DeleteEntityAsync(key, partitionKey, false, cancellationToken);
    }

    /// <inheritdoc />
    public async Task DeleteEntityAsync(object key, object partitionKey, bool errorIfMissing, CancellationToken cancellationToken = default)
    {
        Log.Information("- DELETE Entity {EntityKey}.", key);

        var filter = this.GetKeyFilter(key, partitionKey);

        var response = await this.Collection.DeleteOneAsync(filter, cancellationToken);
        if (errorIfMissing && response.DeletedCount == 0)
        {
            Log.Information("No entity found to delete for id: {EntityKey}", key);

            throw new ArgumentException($"No entity found to delete for id: {key}");
        }
    }

    /// <inheritdoc />
    public void DeleteStore()
    {
        Log.Information("- DELETE Entity Store {StoreName}.", this.Name);

        this.Database.DropCollection(this.Name);
    }

    /// <inheritdoc />
    public async Task DeleteStoreAsync(CancellationToken cancellationToken = default)
    {
        await this.Database.DropCollectionAsync(this.Name, cancellationToken);
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
        }
    }

    ~EntityStore()
    {
        this.Dispose(false);
    }

    #endregion
}