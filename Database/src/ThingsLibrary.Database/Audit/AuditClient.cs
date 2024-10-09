// ================================================================================
// <copyright file="AuditClient.cs" company="Starlight Software Co">
//    Copyright (c) Starlight Software Co. All rights reserved.
//    Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
// </copyright>
// ================================================================================

namespace ThingsLibrary.Database.Audit
{
    [System.Diagnostics.DebuggerDisplay("{Name} ({Issuer} {ObjectId})")]
    [Table("AuditClient")]
    internal class AuditClient
    {
        /// <summary>
        /// Data Partition Key
        /// </summary>
        [Column("PartitionKey"), Required]
        public string PartitionKey { get; set; } = string.Empty;

        /// <summary>
        /// Client Id / Key
        /// </summary>
        [Column("Key"), Required]
        public string Key { get; set; } = string.Empty;

        /// <summary>
        /// Client Name
        /// </summary>
        [Column("Name"), Required]
        public string Name { get; set; } = string.Empty;

        /// <summary>
        /// Version
        /// </summary>
        [Column("Version"), Required]
        public string Version { get; set; } = string.Empty;

        /// <summary>
        /// Last Updated On
        /// </summary>
        [Column("LastUpdatedOn"), LastEditDate, Required]
        public DateTimeOffset LastUpdatedOn { get; set; } = DateTimeOffset.UtcNow;

        /// <summary>
        /// Created On (timestamp)
        /// </summary>
        [Column("CreatedOn"), Required]
        public DateTimeOffset CreatedOn { get; set; } = DateTimeOffset.UtcNow;
    }
}
