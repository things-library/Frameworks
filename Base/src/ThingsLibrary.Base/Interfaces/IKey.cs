namespace ThingsLibrary.Interfaces
{
    /// <summary>
    /// Basic Interface making sure the class has a standard 'Name' field
    /// </summary>
    public interface IKey
    {
        /// <summary>
        /// Record Key
        /// </summary>
        public string Key { get; set; }
    }
}
