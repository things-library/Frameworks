namespace ThingsLibrary.Database.Audit
{
    /// <summary>
    /// Audit User
    /// </summary>
    [System.Diagnostics.DebuggerDisplay("{Id} ({Issuer} {ObjectId})")]
    [Table("AuditUser")]
    [Index(nameof(IssuerObjectId))]
    public class AuditUser
    {
        /// <summary>
        /// Data Partition Key
        /// </summary>
        [Column("PartitionKey"), Required]
        public string PartitionKey { get; set; } = string.Empty;

        /// <summary>
        /// Unique ID
        /// </summary>
        [Column("Id"), Key, Required]
        public Guid Id { get; set; } = SequentialGuid.NewGuid();

        /// <summary>
        /// Identity Provider / Issuer
        /// </summary>
        /// <remarks>JWT Claim: iss</remarks>
        [Column("Issuer")]
        public string Issuer { get; set; } = string.Empty;

        /// <summary>
        /// Issuer Object ID
        /// </summary>
        /// <remarks>JWT Claim: sub or oid</remarks>
        [Column("IssuerObjectId"), Required]
        public string IssuerObjectId { get; set; } = string.Empty;

        /// <summary>
        /// Some sort of display name
        /// </summary>
        [Column("Name"), Required]
        public string Name { get; set; } = string.Empty;

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