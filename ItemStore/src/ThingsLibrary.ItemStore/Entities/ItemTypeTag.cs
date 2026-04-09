// ================================================================================
// <copyright file="ItemTypeTag.cs" company="Starlight Software Co">
//    Copyright (c) 2025 Starlight Software Co. All rights reserved.
//    Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
// </copyright>
// ================================================================================

namespace ThingsLibrary.ItemStore.Entities
{    
    //[Index("Key")]
    public class ItemTypeTag
    {
        /// <summary>
        /// Library Unique Resource Key
        /// </summary>
        [Key]
        [Column("key"), Required]
        public string Key { get; set; } = string.Empty;

        /// <summary>
        /// Type Name
        /// </summary>
        [Column("name"), Required]
        public string Name { get; set; } = string.Empty;

        /// <summary>
        /// Tag Data Type 
        /// </summary>
        [Column("type"), Required]
        public string Type { get; set; } = "string";

        /// <summary>
        /// Units
        /// </summary>
        [Column("units")]
        public string? Units { get; set; }

        /// <summary>
        /// Sequence Order
        /// </summary>
        [Column("seq")]
        public short? Sequence { get; set; }
        
        /// <summary>
        /// Picklist Values
        /// </summary>
        [Column("values")]
        public IDictionary<string, string> Values { get; set; } = new Dictionary<string, string>();

        /// <summary>
        /// Metadata
        /// </summary>
        [Column("meta")]
        public IDictionary<string, string> Meta { get; set; } = new Dictionary<string, string>();

        #region --- Meta Fields ---

        /// <summary>
        /// Specifies if this tag is considered a summary field to show on a listing page
        /// </summary>
        [Column("list")]
        public bool IsListView { get; set; }

        /// <summary>
        /// Specifies if the tag is required
        /// </summary>
        [Column("required")]
        public bool IsRequired { get; set; }

        #endregion

        /// <summary>
        /// Constructor
        /// </summary>
        public ItemTypeTag()
        {
            //nothing
        }
    }
}
