namespace ThingsLibrary.Attributes
{
    /// <summary>
    /// Specifies which class property is the edit timestamp key
    /// </summary>
    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Field, AllowMultiple = false, Inherited = true)]
    public sealed class LastEditDateAttribute : Attribute
    {
        /// <summary>
        /// Last Record Edit Date Timestamp
        /// </summary>
        public LastEditDateAttribute() { }
    }
}