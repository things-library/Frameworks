// ================================================================================
// <copyright file="Revision.cs" company="Starlight Software Co">
//    Copyright (c) 2025 Starlight Software Co. All rights reserved.
//    Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
// </copyright>
// ================================================================================

using Json.Patch;

namespace ThingsLibrary.ItemStore.Entities
{
    [Table("item-revisions")]
    //[Index("EntityId")]
    //[Index("EntityType", "Key", "RevisionNumber", IsUnique = true)]
    public class Revision
    {
        [Key, Required]
        public string Id { get; set; } = Guid.CreateVersion7().ToString();

        [Column("type"), Required]
        public string EntityType { get; set; } = string.Empty;

        /// <summary>
        /// Partitioning Key and where the global types and attributes are located
        /// </summary>
        [PartitionKey]
        [Column("partition_key"), Required]
        public string PartitionKey { get; set; } = string.Empty;

        /// <summary>
        /// Record ID
        /// </summary>
        [Required]
        public string EntityId { get; set; } = string.Empty;

        /// <summary>
        /// JSON Patch Details
        /// </summary>
        [Column("changes")]//, TypeName = "jsonb")]
        public JsonPatch Changes { get; set; } = new JsonPatch();

        /// <summary>
        /// Revision
        /// </summary>
        [RevisionNumber]
        [Column("rev"), Required]
        public int RevisionNumber { get; set; } = 0;

        /// <summary>
        /// Created Date
        /// </summary>        
        [Column("created"), Required]
        public DateTime CreatedOn { get; set; } = DateTime.UtcNow;

        /// <summary>
        /// System Metadata
        /// </summary>
        [Column("meta")]
        public IDictionary<string, string> SystemMeta { get; set; } = new Dictionary<string, string>();
    }


    //public class RevisionConfiguration : IEntityTypeConfiguration<Revision>
    //{
    //    public void Configure(EntityTypeBuilder<Revision> builder)
    //    {
    //        builder
    //            .Property(r => r.Changes)
    //            .HasConversion(
    //                v => JsonSerializer.Serialize(v),
    //                v => JsonSerializer.Deserialize<JsonPatch>(v)!);

    //    }
    //}
}