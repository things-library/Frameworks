namespace ThingsLibrary.Interfaces
{
    /// <summary>
    /// Basic Interface making sure the class has a standard 'Key' field
    /// </summary>
    public interface IKey
    {
        /// <summary>
        /// Record Key
        /// </summary>
        public string Key { get; set; }
    }
}
