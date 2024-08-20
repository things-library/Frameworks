using System.ComponentModel.DataAnnotations;

using Azure;
using Azure.Data.Tables;

namespace ThingsLibrary.Entity.AzureTable;

public class EntityItem : ITableEntity
{
    #region --- Table Entity Fields ---
        
    [PartitionKey, Required]
    public string PartitionKey { get; set; } = string.Empty;

    [Key, Required]
    public string RowKey { get; set; } 

    [Timestamp]
    public DateTimeOffset? Timestamp { get; set; }

    public ETag ETag { get; set; }
        
    #endregion
}
