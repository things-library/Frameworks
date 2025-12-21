using Azure.Data.Tables;
using System.ComponentModel.DataAnnotations;
using Starlight.Attributes;

namespace Starlight.Entity.AzureTable;

public class EntityItem : ITableEntity
{
    #region --- Table Entity Fields ---
        
    [PartitionKey, Required]
    public string PartitionKey { get; set; } = string.Empty;

    [Key, Required]
    public string RowKey { get; set; } 

    [Timestamp]
    public DateTimeOffset? Timestamp { get; set; }

    public Azure.ETag ETag { get; set; }

    #endregion
}
