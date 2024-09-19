using System.Diagnostics;

namespace ThingsLibrary.Database.Audit
{
    /// <summary>
    /// Audit Event
    /// </summary>
    [System.Diagnostics.DebuggerDisplay("{EntityName} ({EventType})")]
    [Index(nameof(EntityId), nameof(Revision), IsUnique = true)]
    [Index(nameof(PartitionKey), nameof(AuditUserId))]
    [Table("AuditEvent")]
    public class AuditEvent
    {
        /// <summary>
        /// Sequence Id
        /// </summary>    
        [Column("SequenceId"), Key, Required]
        public int SequenceId { get; init; } = default;

        /// <summary>
        /// Data Partition Key
        /// </summary>
        [Column("PartitionKey"), Required]
        public string PartitionKey { get; set; } = string.Empty;

        /// <summary>
        /// Audit Event Id
        /// </summary>
        [Column("Id"), Key, Required]
        public Guid Id { get; set; } = SequentialGuid.NewGuid();

        /// <summary>
        /// Entity Type Name
        /// </summary>
        [Column("EntityTypeName"), Required]
        public string EntityName { get; set; } = string.Empty;

        /// <summary>
        /// Entity Id
        /// </summary>        
        [Column("EntityId"), Required]
        public Guid EntityId { get; set; }

        /// <summary>
        /// Entity's Revision Number
        /// </summary>
        [Column("EntityRevision")]
        public int Revision { get; set; }
        
        /// <summary>
        /// Trace Identifier (HttpContext.TraceIdentifier) for the specific request and response
        /// </summary>
        [Column("TraceId"), StringLength(50)]
        public string? TraceId { get; set; } = Activity.Current?.Id;

        /// <summary>
        /// Event Type (C = Created, U = Updated, D = Deleted, X = UnDeleted)
        /// </summary>
        [Column("EventType"), Required]
        public char EventType { get; set; }       //C = Create, U = update, D = delete

        /// <summary>
        /// This is the unique user id (claim: OID) of who caused the event
        /// </summary>
        [Column("AuditUserId"), Required]
        public Guid AuditUserId { get; set; }

        /// <summary>
        /// Integration / client (plus version) doing the action through  (IE: Website/1.0.0.1)
        /// </summary>
        [Column("ClientId")]
        public string ClientId { get; set; } = string.Empty;

        /// <summary>
        /// New values
        /// </summary>
        [Column("NewValues"), Required]
        public Dictionary<string, object?> NewValues { get; set; } = new Dictionary<string, object?>();

        /// <summary>
        /// Old values
        /// </summary>
        [Column("OldValues"), Required]
        public Dictionary<string, object?> OldValues { get; set; } = new Dictionary<string, object?>();

        /// <summary>
        /// Event Occured On (timestamp)
        /// </summary>
        [Column("EventOn"), Required]
        public DateTime EventOn { get; set; } = DateTime.UtcNow;
        
        /// <summary>
        /// Constructor
        /// </summary>
        public AuditEvent()
        {
            //nothing
        }
    }
}
