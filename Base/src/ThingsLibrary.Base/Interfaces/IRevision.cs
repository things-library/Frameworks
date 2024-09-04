namespace ThingsLibrary.Interfaces
{
    /// <summary>
    /// Basic Interface making sure the class has a standard 'PartitionKey' field
    /// </summary>
    public interface IRevisionNumber
    {
        /// <summary>
        /// Data Partition Key
        /// </summary>
        public long Revision { get; set; }
    }
}
