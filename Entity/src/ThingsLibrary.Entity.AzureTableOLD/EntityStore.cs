using Azure.Data.Tables;
using Azure;
using System.ComponentModel.DataAnnotations;
using Starlight.Entity.AzureTable.Extensions;

namespace Starlight.Entity.AzureTable;

public class EntityStore<T> : Az.TableClient where T : class, ITableEntity
{
    public Az.TableClient TableClient { get; init; }

    /// <summary>
    /// Table Name
    /// </summary>
    public new string Name => this.TableClient.Name;

    public EntityStore(Az.TableClient tableClient)
    {
        this.TableClient = tableClient;
    }

    /// <inheritdoc />
    public T GetEntity(object key, string partitionKey = "")
    {
        throw new NotImplementedException();
            
        //return this.GetEntity(key, partitionKey);

        //if (!string.IsNullOrEmpty(partitionKey))
        //{
        //    return this.TableClient.GetEntity<T>(partitionKey, key.ToString()).Value;
        //}
        //else
        //{
        //    return this.TableClient.QueryFirstOrDefault<T>(filter: TableClient.CreateQueryFilter($"RowKey eq {key}"));
        //}
    }


    public void InsertEntity(T entity)
    {
        if (entity == null) { throw new ArgumentNullException(nameof(entity)); }
                    
        try
        {
            this.TableClient.AddEntity<T>(entity);
        }
        catch (RequestFailedException ex)
        {
            throw new InvalidOperationException(ex.Message);
        }
    }

    /// <inheritdoc />
    public void UpdateEntity(T entity)
    {
        if (entity == null) { throw new ArgumentNullException(nameof(entity)); }
                    
        // creates (if missing) or replaces entity            
        this.TableClient.UpdateEntity<T>(entity, ETag.All, Az.TableUpdateMode.Merge);
    }

    /// <inheritdoc />
    public void UpsertEntity(T entity)
    {
        if (entity == null) { throw new ArgumentNullException(nameof(entity)); }

        // creates (if missing) or updates entity (if fields are missing then those just don't get updated)  So in case of mis-match of existing properties and current
        this.TableClient.UpsertEntity<T>(entity, Az.TableUpdateMode.Merge);
    }

    /// <inheritdoc />
    public void DeleteEntity(object key, string partitionKey = "")
    {
        this.TableClient.DeleteEntity(partitionKey, key.ToString());
    }
}
