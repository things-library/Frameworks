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
