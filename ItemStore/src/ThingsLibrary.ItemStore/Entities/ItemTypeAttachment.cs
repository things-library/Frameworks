// ================================================================================
// <copyright file="ItemTypeAttachment.cs" company="Starlight Software Co">
//    Copyright (c) 2025 Starlight Software Co. All rights reserved.
//    Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
// </copyright>
// ================================================================================

namespace ThingsLibrary.ItemStore.Entities
{   
    public class ItemTypeAttachment
    {
        /// <summary>
        /// Library Unique Resource Key
        /// </summary>
        [Key]
        [Column("key"), Required]
        public string Key { get; set; } = string.Empty;

        /// <summary>
        /// Item Type 
        /// </summary>
        [Column("type"), Required]
        public string Type { get; set; } = string.Empty;

        /// <summary>
        /// Type Name
        /// </summary>
        [Column("name"), Required]
        public string Name { get; set; } = string.Empty;

        /// <summary>
        /// Description
        /// </summary>
        [Column("description")]
        public string? Description { get; set; }

        /// <summary>
        /// Sequence Order
        /// </summary>
        [Column("seq")]
        public short Sequence { get; set; } = 0;
        
        /// <summary>
        /// Metadata
        /// </summary>
        [Column("meta")]
        public IDictionary<string, string> Meta { get; set; } = new Dictionary<string, string>();
                     
        
        /// <summary>
        /// Constructor
        /// </summary>
        public ItemTypeAttachment()
        {
            //nothing
        }
    }
}
