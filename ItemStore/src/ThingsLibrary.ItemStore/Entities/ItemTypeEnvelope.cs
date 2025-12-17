// ================================================================================
// <copyright file="ItemTypeEnvelope.cs" company="Starlight Software Co">
//    Copyright (c) 2025 Starlight Software Co. All rights reserved.
//    Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
// </copyright>
// ================================================================================

using ThingsLibrary.Attributes;
using ThingsLibrary.Interfaces;

namespace ThingsLibrary.ItemStore.Entities
{
    [Table("item-types")]
    //[Index("PartitionKey", "IsRootType")]
    //[Index("ResourceKey")]
    public class ItemTypeEnvelope
    {
        #region --- System Properties ---
                
        /// <summary>
        /// Partitioning Key and where the global types and attributes are located
        /// </summary>
        [PartitionKey]
        [Column("partition"), Required]
        public string PartitionKey { get; init; } = string.Empty;

        /// <summary>
        /// Globally Unique Sequential ID
        /// </summary>
        [Key]
        [Column("id"), Required]
        public string Id { get; init; } = string.Empty;

        /// <summary>
        /// Root Type - if this type is considered a root item type to be used on root items
        /// </summary>
        [Column("root"), Required]
        public bool IsRootType { get; set; } = false;

        /// <summary>
        /// System Controlled Item?
        /// </summary>
        [Column("system"), Required]
        public bool IsSystem { get; set; } = false;

        /// <summary>
        /// Library Deleted Flag
        /// </summary>
        [Column("deleted"), Required]
        public bool IsDeleted { get; set; } = false;

        /// <summary>
        /// Created Date
        /// </summary>        
        [Column("created"), Required]
        public DateTime CreatedOn { get; init; } = DateTime.UtcNow;

        /// <summary>
        /// Updated Date
        /// </summary>
        [LastEditDate]
        [Column("updated")]
        public DateTime UpdatedOn { get; private set; } = DateTime.UtcNow;

        /// <summary>
        /// Revision
        /// </summary>
        [RevisionNumber]
        [Column("rev"), Required]
        public int Revision { get; set; } = 0;

        #endregion

        [Column("type")]
        public ItemType Data { get; set; } = new ItemType();

        /// <summary>
        /// Metadata
        /// </summary>
        [Column("meta")]
        public IDictionary<string, string> SystemMeta { get; set; } = new Dictionary<string, string>();


        /// <summary>
        /// Constructor
        /// </summary>
        public ItemTypeEnvelope()
        {
            //nothing
        }

        /// <summary>
        /// Constructor
        /// </summary>
        public ItemTypeEnvelope(string partitionKey, string resourceKey)
        {
            this.PartitionKey = partitionKey;
            this.Id = resourceKey;
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
            get => this.SystemMeta.ContainsKey(key) ? SystemMeta[key] : null;
        }
    }
}
