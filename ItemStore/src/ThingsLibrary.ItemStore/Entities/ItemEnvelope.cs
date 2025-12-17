// ================================================================================
// <copyright file="ItemEnvelope.cs" company="Starlight Software Co">
//    Copyright (c) 2025 Starlight Software Co. All rights reserved.
//    Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
// </copyright>
// ================================================================================

using System.Text.Json.Serialization;

namespace ThingsLibrary.ItemStore.Entities
{
    [Table("items")]
    //[Index("PartitionKey", "IsRootItem", "Type")]
    //[Index("ResourceKey")]
    public class ItemEnvelope
    {
        #region --- System Audit Tracking ---
                
        /// <summary>
        /// System Controlled Item?
        /// </summary>
        [Column("system"), Required]
        [JsonPropertyName("system")]
        public bool IsSystem { get; set; } = false;

        /// <summary>
        /// Deleted Flag
        /// </summary>
        [Column("deleted"), Required]
        [JsonPropertyName("deleted")]
        public bool IsDeleted { get; set; } = false;

        /// <summary>
        /// Original creation date of the record.
        /// </summary>        
        [Column("created"), Required]
        [JsonPropertyName("created")]
        public DateTime CreatedOn { get; set; } = DateTime.UtcNow;

        /// <summary>
        /// Date when the revision was created
        /// </summary>
        [LastEditDate]
        [Column("updated")]
        [JsonPropertyName("updated")]
        public DateTime UpdatedOn { get; set; } = DateTime.UtcNow;

        /// <summary>
        /// Revision
        /// </summary>
        [RevisionNumber]
        [Column("rev"), Required]
        [JsonPropertyName("rev")]
        public int Revision { get; set; } = 0;

        #endregion

        /// <summary>
        /// Globally Unique Sequential ID
        /// </summary>
        [Key]
        [Column("id"), Required]
        [JsonPropertyName("id")]
        public string Id { get; init; } = Guid.CreateVersion7().ToString();

        /// <summary>
        /// Partitioning Key and where the global types and attributes are located
        /// </summary>
        [PartitionKey]
        [Column("partition"), Required]
        [JsonPropertyName("partition")]
        public string Partition { get; init; } = string.Empty;

        /// <summary>
        /// Globally Unique Sequential ID
        /// </summary>
        [Key]
        [Column("key"), Required]
        [JsonPropertyName("key")]
        public string ResourceKey { get; init; } = string.Empty;

        /// <summary>
        /// Type of item
        /// </summary>
        [Column("type"), Required]
        [JsonPropertyName("type")]
        public string Type { get; init; } = string.Empty;

        /// <summary>
        /// If the item is a root level item (outside of library root)
        /// </summary>
        [Column("root"), Required]
        [JsonPropertyName("root")]
        public bool IsRootItem { get; set; }

        /// <summary>
        /// Item Details
        /// </summary>        
        [Column("item"), Required]
        [JsonPropertyName("item")]
        public Item Data { get; set; } = new Item();

        /// <summary>
        /// System Metadata
        /// </summary>
        [Column("meta")]
        [JsonPropertyName("meta")]
        public IDictionary<string, string> SystemMeta { get; set; } = new Dictionary<string, string>();

        /// <summary>
        /// Constructor
        /// </summary>
        public ItemEnvelope()
        {
            //nothing
        }

        /// <summary>
        /// Constructor
        /// </summary>
        public ItemEnvelope(string partitionKey, string resourceKey)
        {
            this.Partition = partitionKey;
            this.ResourceKey = resourceKey;
        }


        /// <summary>
        /// Increment the Revision Number
        /// </summary>
        public void IncrementRevision()
        {
            this.Revision++;
            this.UpdatedOn = DateTime.UtcNow;
        }

        public string? this[string key]
        {
            get => this.SystemMeta.ContainsKey(key) ? this.SystemMeta[key] : null;
        }
    }
}

