// ================================================================================
// <copyright file="EntityBase.cs" company="Starlight Software Co">
//    Copyright (c) 2025 Starlight Software Co. All rights reserved.
//    Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
// </copyright>
// ================================================================================

namespace ThingsLibrary.Database.Base
{
    /// <summary>
    /// Base set of properties to standarize some basic foundational data.
    /// </summary>
    public class EntityBase : IEntityBase
    {
        //NOTE: Primitive types must have a setter not INIT to work with Mongo

        /// <summary>
        /// Data Partition Key
        /// </summary>
        [Column("PartitionKey"), PartitionKey]        
        public string PartitionKey { get; set; } = string.Empty;

        /// <summary>
        /// UUID Key
        /// </summary>        
        [Column("Id"), Key]
        public Guid Id { get; set; } = SequentialGuid.NewGuid();

        /// <summary>
        /// Revision Number
        /// </summary>
        [Column("Revision"), RevisionNumber]
        public int Revision { get; set; } = 0;

        /// <summary>
        /// Record Timestamp
        /// </summary>
        [Column("LastUpdatedOn"), LastEditDate, Required]
        public DateTime UpdatedOn { get; set; } = DateTime.UtcNow;

        /// <summary>
        /// Record Created Timestamp
        /// </summary>
        [Column("CreatedOn"), Required]
        public DateTime CreatedOn { get; set; } = DateTime.UtcNow;
    }
}
