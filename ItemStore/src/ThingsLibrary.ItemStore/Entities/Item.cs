// ================================================================================
// <copyright file="Item.cs" company="Starlight Software Co">
//    Copyright (c) 2025 Starlight Software Co. All rights reserved.
//    Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
// </copyright>
// ================================================================================

using System.Text.Json.Serialization;

namespace ThingsLibrary.ItemStore.Entities
{
    public class Item
    {
        /// <summary>
        /// Name
        /// </summary>
        [Column("name"), Required]
        [JsonPropertyName("name")]
        public string Name { get; set; } = string.Empty;

        /// <summary>
        /// Date Event Occured
        /// </summary>
        [Column("date")]
        [JsonPropertyName("date")]
        public DateTimeOffset? Date { get; set; }
                
        /// <summary>
        /// Attribute Tags
        /// </summary>
        [Column("tags")]
        [JsonPropertyName("tags")]
        public IDictionary<string, string> Tags { get; set; } = new Dictionary<string, string>();

        /// <summary>
        /// Metadata
        /// </summary>
        [Column("meta")]
        [JsonPropertyName("meta")]
        public IDictionary<string, string> Meta { get; set; } = new Dictionary<string, string>();

        /// <summary>
        /// Constructor
        /// </summary>
        public Item()
        {
            //nothing
        }

        /// <summary>
        /// Constructor
        /// </summary>
        public Item(string name, DateTimeOffset? date)
        {
            this.Name = name;
            this.Date = date;
        }
    }
}
