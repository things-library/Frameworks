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
        public DateTimeOffset UpdatedOn { get; set; } = DateTimeOffset.UtcNow;

        /// <summary>
        /// Record Created Timestamp
        /// </summary>
        [Column("CreatedOn"), Required]
        public DateTimeOffset CreatedOn { get; set; } = DateTimeOffset.UtcNow;
    }
}
