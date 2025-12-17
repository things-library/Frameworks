// ================================================================================
// <copyright file="TestEntity.cs" company="Starlight Software Co">
//    Copyright (c) 2025 Starlight Software Co. All rights reserved.
//    Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
// </copyright>
// ================================================================================

using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

using ThingsLibrary.Entity;
using ThingsLibrary.Schema.Library;

namespace EntityTester
{
    public class TestEntity : Entity
    {
        #region --- System Audit Tracking ---
                
        /// <summary>
        /// System Controlled Item?
        /// </summary>
        [Column("system"), Required]
        public bool IsSystem { get; set; } = false;

        /// <summary>
        /// Deleted Flag
        /// </summary>
        [Column("deleted"), Required]
        public bool IsDeleted { get; set; } = false;

        /// <summary>
        /// Original creation date of the record.
        /// </summary>        
        [Column("created"), Required]
        public DateTime CreatedOn { get; set; } = DateTime.UtcNow;

        #endregion

        /// <summary>
        /// Type of library
        /// </summary>
        [Column("type"), Required]
        public string Type { get; init; } = string.Empty;

        /// <summary>
        /// If the item is a root level item (outside of library root)
        /// </summary>
        [Column("root")]
        public bool IsRootItem { get; set; }

        /// <summary>
        /// Item Details
        /// </summary>        
        [Column("item"), Required]
        public ItemDto Data { get; set; } = new ItemDto();

        /// <summary>
        /// System Metadata
        /// </summary>
        [Column("meta")]
        public IDictionary<string, string> Meta { get; set; } = new Dictionary<string, string>();

        /// <summary>
        /// Constructor
        /// </summary>
        public TestEntity()
        {
            //nothing
        }

        /// <summary>
        /// Constructor
        /// </summary>
        public TestEntity(string partitionKey, string resourceKey)
        {
            this.PartitionKey = partitionKey;
            this.ResourceKey = resourceKey;
        }

        public string? this[string key]
        {
            get => this.Meta.ContainsKey(key) ? this.Meta[key] : null;
        }
    }
}
