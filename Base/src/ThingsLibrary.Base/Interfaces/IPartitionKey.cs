namespace ThingsLibrary.Interfaces
{
    /// <summary>
    /// Basic Interface making sure the class has a standard 'PartitionKey' field
    /// </summary>
    public interface IPartitionKey
    {
        /// <summary>
        /// Data Partition Key
        /// </summary>
        public string PartitionKey { get; }
    }
}
