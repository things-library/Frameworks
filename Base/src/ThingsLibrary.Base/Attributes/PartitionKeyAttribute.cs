namespace ThingsLibrary.Attributes
{
    /// <summary>
    /// Specifies which class property is the partition key
    /// </summary>
    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Field, AllowMultiple = false, Inherited = true)]
    public sealed class PartitionKeyAttribute : Attribute
    {
        /// <summary>
        /// Partition Key
        /// </summary>
        public PartitionKeyAttribute() { }
    }
}