namespace ThingsLibrary.Interfaces
{
    /// <summary>
    /// Basic Interface making sure the class has a standard 'PartitionKey' field
    /// </summary>
    public interface ILastEditDate
    {
        /// <summary>
        /// DateTime (UTC) of last record edit
        /// </summary>
        public DateTimeOffset UpdatedOn { get; set; }
    }
}
