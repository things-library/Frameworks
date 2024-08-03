namespace ThingsLibrary.Attributes
{
    /// <summary>
    /// Specifies which class property is the revision number
    /// </summary>
    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Field, AllowMultiple = false, Inherited = true)]
    public sealed class RevisionNumberAttribute : Attribute
    {
        /// <summary>
        /// Partition Key
        /// </summary>
        public RevisionNumberAttribute() { }
    }
}