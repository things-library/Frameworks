using Azure;
using Azure.Data.Tables;

using Starlight.Entity.Interfaces;
using Starlight.Entity.Types;


namespace Starlight.Entity.AzureTable;

/// <summary>
/// Azure Table implementation 
/// </summary>
/// <remarks>
/// Azure Table does not support: Byte, Decimal, INT16, Timespan and Float.
/// </remarks>
/// <typeparam name="T"></typeparam>
public class EntityStore<T> : IEntityStore<T> where T : class
{
    #region --- General ---

    /// <summary>
    /// Entity Store Type
    /// </summary>
    public EntityStoreType StoreType => EntityStoreType.Azure_Table;

    /// <summary>
    /// Entity's Type
    /// </summary>
    public Type Type { get; init; }

    /// <summary>
    /// If the underlying type is the native type to this system (IE: ITableEntity)
    /// </summary>
    public bool IsNativeType { get; init; }

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
    /// Entity Properties (public / instance)
    /// </summary>
    public Dictionary<string, PropertyInfo> Properties { get; init; }
    
    /// <summary>
    /// Table name
    /// </summary>
    public string Name { get; init; }

    /// <summary>
    /// Fields that are part of the class but that map to system fields, so we don't duplicate data in storage
    /// </summary>
    public List<string> ExcludedFields { get; set; } = new List<string> { "PartitionKey", "RowKey", "odata.etag", "Timestamp" };

    #endregion

    #region --- Store Specific ---

    // VENDOR SPECIFIC (if you aren't using the interface methods) allow for exposure of vendor specific objects
    public Az.TableClient AzureCloudTable { get; init; }

    #endregion

    /// <summary>
    /// Constructor
    /// </summary>
    /// <param name="storageConnectionString">Storage Connection String</param>
    /// <param name="name">Table Name</param>
    /// <exception cref="ArgumentNullException">Storage connection string, table name must not be null.</exception>
    /// <exception cref="ArgumentException">Table name must strat with a latter, be alphanumeric and be 3-63 characters</exception>
    public EntityStore(string storageConnectionString, string name)
    {
        // validate name
        EntityStoreFactory.ValidateTableName(name);

        // validate the generic entity (good to do this once on store creation instead of each Insert/Update, etc)
        this.Type = typeof(T);        
        this.IsNativeType = (typeof(ITableEntity).IsAssignableFrom(typeof(T)));

        // get the key
        if (this.IsNativeType)
        {
            this.Key = this.Type.GetProperty("RowKey");
            this.PartitionKey = this.Type.GetProperty("PartitionKey");
            this.Timestamp = this.Type.GetProperty("Timestamp");

            //Not supported (currently): Byte, Decimal, INT16, Timespan and Float.
            if (this.Type.GetProperties(BindingFlags.Public | BindingFlags.Instance).Any(x => 
                x.PropertyType == typeof(Byte) || 
                x.PropertyType == typeof(Int16) || 
                x.PropertyType == typeof(Decimal)))
            {
                throw new ArgumentException("Azure Tables does not support data types: Byte, Decimal, INT16, Timespan or Float");
            }
        }
        else
        {
            this.Key = this.Type.GetProperties().FirstOrDefault(x => x.GetCustomAttributes(typeof(System.ComponentModel.DataAnnotations.KeyAttribute), false).Any());
            if (this.Key == null) { throw new ArgumentException($"Entity {typeof(T).Namespace}.{typeof(T).Name} has no [Key] attribute defined."); }
            
            this.PartitionKey = this.Type.GetProperties().FirstOrDefault(x => x.GetCustomAttributes(typeof(Attributes.PartitionKeyAttribute), false).Any());
            this.Timestamp = this.Type.GetProperties().FirstOrDefault(x => x.GetCustomAttributes(typeof(System.ComponentModel.DataAnnotations.TimestampAttribute), false).Any());
        }
        
        this.Properties = this.Type.GetProperties(BindingFlags.Public | BindingFlags.Instance).ToDictionary(x => x.Name, x => x);    //include 'key' fields as well!

        // add the three fields that are system fields so we aren't duplicating up data in storage
        if (!this.ExcludedFields.Contains(this.Key.Name)) { this.ExcludedFields.Add(this.Key.Name); }
        if (this.PartitionKey != null && !this.ExcludedFields.Contains(this.PartitionKey.Name)) { this.ExcludedFields.Add(this.PartitionKey.Name); }
        if (this.Timestamp != null && !this.ExcludedFields.Contains(this.Timestamp.Name)) { this.ExcludedFields.Add(this.Timestamp.Name); }

        // validate parameters (name is validated in ValidateTableName)
        if (string.IsNullOrEmpty(storageConnectionString)) { throw new ArgumentNullException(nameof(storageConnectionString)); }
                    
        // set the core properties            
        this.Name = name;

        // create the vendor specific object
        this.AzureCloudTable = new Az.TableClient(storageConnectionString, name);

        // ensure that the table exists
        this.AzureCloudTable.CreateIfNotExists();
    }

    #region --- Entitites ---    

    //public bool Exists(string partitionKey, string rowKey)
    //{
    //    var result = this.AzureCloudTable.QueryAsync(TableOperation.Retrieve<TableEntity>(partitionKey, rowKey)).;
    //    return result.Result != null; // or result.HttpStatusCode != 404
    //}


    public bool Exists(object key, object partitionKey)
    {
        //TODO: need to test that this actually works
        if(this.PartitionKey == null)
        {
            return this.AzureCloudTable.Query<TableEntity>(x => x.RowKey == key.ToString()).Any();
        }
        else
        {
            return this.AzureCloudTable.Query<TableEntity>(x => x.RowKey == key.ToString() && x.PartitionKey == partitionKey.ToString()).Any();
        }        
    }

    public Task<bool> ExistsAsync(object key, object partitionKey, CancellationToken cancellationToken = default)
    {
        ////TODO: need to test that this actually works
        //var query = this.AzureCloudTable.QueryAsync<TableEntity>(x => x.RowKey == key.ToString(), cancellationToken: cancellationToken);

        //return query.Any();

        throw new NotImplementedException();
    }

    /// <inheritdoc />
    public T GetEntity(object key, object partitionKey)
    {
        var nullableResponse = this.AzureCloudTable.GetEntityIfExists<Az.TableEntity>($"{partitionKey}", key.ToString());
        if (nullableResponse.HasValue)
        {
            return this.ToEntity(nullableResponse.Value);
        }
        else
        {
            return null;
        }
    }

    /// <inheritdoc />
    public async Task<T> GetEntityAsync(object key, object partitionKey, CancellationToken cancellationToken = default)
    {
        Response<Az.TableEntity> response;
        if (this.PartitionKey == null)
        {
            response = await this.AzureCloudTable.GetEntityAsync<Az.TableEntity>(string.Empty, key.ToString(), null, cancellationToken);
        }
        else
        {
            response = await this.AzureCloudTable.GetEntityAsync<Az.TableEntity>(partitionKey.ToString(), key.ToString(), null, cancellationToken);
        }

        return this.ToEntity(response);
    }


    /// <inheritdoc />
    public IEnumerable<T> GetEntities(DateTime? sinceDate = null)
    {
        Pageable<TableEntity> pagedList;

        if (sinceDate != null)
        {
            pagedList = this.AzureCloudTable.Query<Az.TableEntity>($"Timestamp ge datetime'{sinceDate:O}'", maxPerPage: 1000);            
        }
        else
        {
            pagedList = this.AzureCloudTable.Query<Az.TableEntity>(maxPerPage: 1000);            
        }

        List<T> items = new List<T>();

        foreach (Page<TableEntity> page in pagedList.AsPages())
        {
            foreach (TableEntity qEntity in page.Values)
            {
                items.Add(ToEntity(qEntity));
            }
        }

        return items;
    }

    /// <summary>
    /// Get all entities for a partition
    /// </summary>
    /// <param name="partitionKey">Partition Key</param>
    /// <returns></returns>
    public IEnumerable<T> GetEntities(string partitionKey)
    {
        var pagedList = this.AzureCloudTable.Query<Az.TableEntity>(x => x.PartitionKey == partitionKey);
        
        return pagedList.Select(x => ToEntity(x));
    }

    /// <inheritdoc />
    public IEnumerable<T> GetEntities<TProp>(string propertyName, TProp value)
    {
        // Acceptable Types: String, Boolean, Binary, DateTime, Double, Guid, Int32, Int64

        var propertyInfo = this.Type.GetProperty(propertyName);

        if (propertyInfo.PropertyType != typeof(TProp)) { throw new ArgumentException($"Value type does not match '{propertyName}' property's type."); }

        string valueStr;

        switch (value)
        {
            case Guid x:
                {
                    valueStr = $"'{x}'";
                    break;
                }

            case DateTime:
                {
                    valueStr = $"{value:O}";
                    break;
                }

            case string:
                {
                    valueStr = $"'{value}'";
                    break;
                }


            default:
                {
                    valueStr = $"{value}";
                    break;
                }
        }

        var pagedList = this.AzureCloudTable.Query<Az.TableEntity>($"{propertyInfo.Name} eq {valueStr}", maxPerPage: 1000);

        return pagedList.Select(x => this.ToEntity(x));
    }


    /// <inheritdoc />
    public async Task<IEnumerable<T>> GetEntitiesAsync(DateTime? sinceDate = null, CancellationToken cancellationToken = default)
    {
        AsyncPageable<TableEntity> pagedList;

        if (sinceDate != null)
        {
            pagedList = this.AzureCloudTable.QueryAsync<Az.TableEntity>($"Timestamp ge datetime'{sinceDate:O}'", maxPerPage: 1000);
        }
        else
        {
            pagedList = this.AzureCloudTable.QueryAsync<Az.TableEntity>(maxPerPage: 1000);
        }

        List<T> items = new List<T>();

        await foreach (Page<TableEntity> page in pagedList.AsPages())
        {
            foreach (TableEntity qEntity in page.Values)
            {
                items.Add(ToEntity(qEntity));
            }
        }

        return items;
    }

    /// <inheritdoc />
    public void InsertEntity(T entity)
    {
        if (entity == null) { throw new ArgumentNullException(nameof(entity)); }

        var azureEntity = this.ToTableEntity(entity);

        try
        {
            this.AzureCloudTable.AddEntity(azureEntity);
        }
        catch (RequestFailedException ex)
        {
            throw new IOException(ex.Message);
        }
    }

    /// <inheritdoc />
    public async Task InsertEntityAsync(T entity, CancellationToken cancellationToken = default)
    {
        if (entity == null) { throw new ArgumentNullException(nameof(entity)); }

        var azureEntity = this.ToTableEntity(entity);

        try
        {
            await this.AzureCloudTable.AddEntityAsync(azureEntity, cancellationToken);
        }
        catch (RequestFailedException ex)
        {
            throw new IOException(ex.Message);
        }
    }

    /// <inheritdoc />
    public void UpdateEntity(T entity)
    {
        if (entity == null) { throw new ArgumentNullException(nameof(entity)); }

        try
        {
            var tableEntity = this.ToTableEntity(entity);

            // creates (if missing) or replaces entity            
            this.AzureCloudTable.UpdateEntity(tableEntity, ETag.All, Az.TableUpdateMode.Replace);
        }
        catch (RequestFailedException ex)
        {
            throw new IOException(ex.Message);
        }        
    }

    /// <inheritdoc />
    public async Task UpdateEntityAsync(T entity, CancellationToken cancellationToken = default)
    {
        if (entity == null) { throw new ArgumentNullException(nameof(entity)); }

        var tableEntity = this.ToTableEntity(entity);

        try
        {
            // creates (if missing) or replaces entity            
            await this.AzureCloudTable.UpdateEntityAsync(tableEntity, ETag.All, Az.TableUpdateMode.Replace, cancellationToken);
        }
        catch (RequestFailedException ex)
        {
            throw new IOException(ex.Message);
        }
    }


    /// <inheritdoc />
    public void UpsertEntity(T entity)
    {
        if (entity == null) { throw new ArgumentNullException(nameof(entity)); }

        var tableEntity = this.ToTableEntity(entity);

        try
        {
            // creates (if missing) or updates entity (if fields are missing then those just don't get updated)  So in case of mis-match of existing properties and current
            this.AzureCloudTable.UpsertEntity(tableEntity, Az.TableUpdateMode.Merge);
        }
        catch (RequestFailedException ex)
        {
            throw new IOException(ex.Message);
        }
    }

    /// <inheritdoc />
    public async Task UpsertEntityAsync(T entity, CancellationToken cancellationToken = default)
    {
        if (entity == null) { throw new ArgumentNullException(nameof(entity)); }

        ITableEntity tableEntity = this.ToTableEntity(entity);

        

        try
        {
            // creates (if missing) or updates entity (if fields are missing then those just don't get updated)  So in case of mis-match of existing properties and current
            await this.AzureCloudTable.UpsertEntityAsync(tableEntity, Az.TableUpdateMode.Merge, cancellationToken);
        }
        catch (RequestFailedException ex)
        {
            throw new IOException(ex.Message);
        }
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
      
    /// <summary>
    /// Delete Key + Partition Key entity
    /// </summary>
    /// <param name="key"></param>
    /// <param name="partitionKey"></param>
    public void DeleteEntity(object key, object partitionKey)
    {
        if(this.PartitionKey == null)
        {
            this.AzureCloudTable.DeleteEntity(partitionKey: string.Empty, rowKey: key.ToString());
        }
        else
        {
            this.AzureCloudTable.DeleteEntity(partitionKey: partitionKey.ToString(), rowKey: key.ToString());
        }
        
    }

    /// <summary>
    /// Delete Key + Partition Key entity
    /// </summary>
    /// <param name="key"></param>
    /// <param name="partitionKey"></param>
    public async Task DeleteEntityAsync(object key, object partitionKey, CancellationToken cancellationToken = default)
    {
        if(this.PartitionKey == null)
        {
            await this.AzureCloudTable.DeleteEntityAsync(partitionKey: string.Empty, rowKey: key.ToString());
        }
        else
        {
            await this.AzureCloudTable.DeleteEntityAsync(partitionKey: partitionKey.ToString(), rowKey: key.ToString());
        }        
    }

    #endregion

    #region --- Static Conversions ---

    private Az.ITableEntity ToTableEntity(T fromEntity)
    {
        if (this.IsNativeType)
        {
            return (ITableEntity)fromEntity;
        }

        // if you send null you get empty
        if (fromEntity == null) { return default; }
                    
        var toEntity = new Az.TableEntity()
        {
            PartitionKey = (this.PartitionKey != null ? this.PartitionKey.GetValue(fromEntity).ToString() : string.Empty),
            RowKey = this.Key.GetValue(fromEntity).ToString()
        };
        
        // map all properties to dictionary records
        foreach (var fromProperty in this.Properties.Values)
        {
            // do not maintain system keys as properties
            if (this.ExcludedFields.Contains(fromProperty.Name)) { continue; }

            var value = fromProperty.GetValue(fromEntity);
            if (value == null) { continue; }

            if (value is DateTime dateTime)
            {                    
                if (dateTime.Kind == DateTimeKind.Unspecified) { dateTime = DateTime.SpecifyKind(dateTime, DateTimeKind.Utc); }

                toEntity[fromProperty.Name] = dateTime;
            }
            else if (value is byte bValue)
            {                    
                toEntity[fromProperty.Name] = Convert.ToInt32(bValue);
            }
            else if (value is short sValue)
            {
                toEntity[fromProperty.Name] = Convert.ToInt32(sValue);
            }
            else if (value is float fValue)
            {
                toEntity[fromProperty.Name] = Convert.ToDouble(fValue);
            }
            else 
            {
                toEntity[fromProperty.Name] = value;
            }
        }

        return toEntity;
    }

    private T ToEntity(Az.TableEntity fromEntity)
    {
        if (fromEntity == null) {  throw new ArgumentNullException(nameof(fromEntity)); }

        //if (this.IsNativeType)
        //{
        //    return fromEntity as T;
        //}
                    
        var toEntity = (T)Activator.CreateInstance(typeof(T)) ?? throw new ArgumentException($"Unable to create instance of '{nameof(T)}'");

        // KEY - set the RowKey key to whatever is the '[Key]' field
        this.SetValue(toEntity, this.Key, fromEntity.RowKey);

        // Partition key - if specified
        if (this.PartitionKey != null)
        {
            this.SetValue(toEntity, this.PartitionKey, fromEntity.PartitionKey);                
        }

        // Timestamp - if there is one and it is specified
        if (this.Timestamp != null && fromEntity.Timestamp != null)
        {
            this.SetValue(toEntity, this.Timestamp, fromEntity.Timestamp);                            
        }

        // all properties
        foreach (var fromProperty in fromEntity)
        {
            // do not set excluded keys this way as we already did above
            if (this.ExcludedFields.Contains(fromProperty.Key)) { continue; }
            if (!this.Properties.ContainsKey(fromProperty.Key)) { continue; }

            var toProperty = this.Properties[fromProperty.Key];

            this.SetValue(toEntity, toProperty, fromProperty.Value);
        }

        return toEntity;
    }

    private void SetValue(T entity, PropertyInfo entityProperty, object value)
    {
        // direct conversion? (most likely the case)
        if (value.GetType() == entityProperty.PropertyType)
        {
            entityProperty.SetValue(entity, value);
        }            
        else if (entityProperty.PropertyType == typeof(byte) || entityProperty.PropertyType == typeof(byte?))
        {
            var toValue = Convert.ToByte(value);
            entityProperty.SetValue(entity, toValue);
        }
        else if (entityProperty.PropertyType == typeof(short) || entityProperty.PropertyType == typeof(short?))
        {
            var toValue = Convert.ToInt16(value);
            entityProperty.SetValue(entity, toValue);
        }
        else if (entityProperty.PropertyType == typeof(int) || entityProperty.PropertyType == typeof(int?))
        {
            var toValue = Convert.ToInt32(value);
            entityProperty.SetValue(entity, toValue);
        }
        else if (entityProperty.PropertyType == typeof(float) || entityProperty.PropertyType == typeof(float?))
        {
            var toValue = Convert.ToSingle(value);
            entityProperty.SetValue(entity, toValue);
        }
        else if (entityProperty.PropertyType == typeof(DateTime) || entityProperty.PropertyType == typeof(DateTime?))
        {
            var fromValue = (DateTimeOffset)value;
            entityProperty.SetValue(entity, fromValue.UtcDateTime);
        }
        else if (entityProperty.PropertyType == typeof(Guid) || entityProperty.PropertyType == typeof(Guid?))
        {
            var toValue = Guid.Parse($"{value}");
            entityProperty.SetValue(entity, toValue);
        }
        else
        {                
            entityProperty.SetValue(entity, value);
        }
    }

    #endregion

    /// <inheritdoc />
    public void DeleteStore()
    {
        var response = this.AzureCloudTable.Delete();
        if (response == null) { return; }        // weird.. no table exists.. this is sus

        if (response.IsError) { throw new IOException("Unable to delete store."); }
    }

    /// <inheritdoc />
    public async Task DeleteStoreAsync(CancellationToken cancellationToken = default)
    {
        var response = await this.AzureCloudTable.DeleteAsync(cancellationToken);
        if (response == null) { return; }        // weird.. no table exists.. this is sus

        if (response.IsError) { throw new IOException("Unable to delete store."); }
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
