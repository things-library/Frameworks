namespace ThingsLibrary.Database.Base
{
    public interface IEntityBase
    {
        /// <summary>
        /// Data Partition Key
        /// </summary>        
        public string PartitionKey { get; }

        /// <summary>
        /// Record ID
        /// </summary>        
        public Guid Id { get; }

        /// <summary>
        /// Record Updated DateTime
        /// </summary>        
        public DateTimeOffset UpdatedOn { get; }

        /// <summary>
        /// Record Created DateTime
        /// </summary>
        public DateTimeOffset CreatedOn { get; }
    }
}
