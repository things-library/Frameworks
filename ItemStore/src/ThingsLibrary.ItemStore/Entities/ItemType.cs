// ================================================================================
// <copyright file="ItemType.cs" company="Starlight Software Co">
//    Copyright (c) 2025 Starlight Software Co. All rights reserved.
//    Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
// </copyright>
// ================================================================================

namespace ThingsLibrary.ItemStore.Entities
{
    public class ItemType
    {                
        /// <summary>
        /// Type Name
        /// </summary>
        [Column("name"), Required]
        public string Name { get; set; } = string.Empty;

        /// <summary>
        /// Type Name
        /// </summary>
        [Column("description")]
        public string? Description { get; set; } = string.Empty;

        /// <summary>
        /// Type Tags
        /// </summary>
        [Column("tags")]
        public List<ItemTypeTag> Tags { get; set; } = new List<ItemTypeTag>();

        /// <summary>
        /// Item Attachments
        /// </summary>
        [Column("items")]
        public List<ItemTypeAttachment> Items { get; set; } = new List<ItemTypeAttachment>();

        /// <summary>
        /// Metadata
        /// </summary>
        [Column("meta")]
        public IDictionary<string, string> Meta { get; set; } = new Dictionary<string, string>();
                     
        
        /// <summary>
        /// Constructor
        /// </summary>
        public ItemType()
        {
            //nothing
        }
    }
}
