namespace ThingsLibrary.Interfaces
{
    /// <summary>
    /// Standard Interface to force a standard on ID in a class
    /// </summary>
    public interface IId
    {
        /// <summary>
        /// ID Field
        /// </summary>        
        public Guid Id { get; }
    }

    /// <summary>
    /// Standard Interface to force a standard on ID in a class
    /// </summary>
    public interface IIdShort
    {
        /// <summary>
        /// ID Field
        /// </summary>        
        public short Id { get; }
    }
}
