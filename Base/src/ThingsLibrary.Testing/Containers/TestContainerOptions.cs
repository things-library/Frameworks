namespace ThingsLibrary.Testing.Containers
{
    /// <summary>
    /// Simple wrapper for the Testcontainers objects to allow appSettings style configuration of a container instead of code only approach
    /// </summary>
    /// <remarks><see href="https://github.com/testcontainers/testcontainers-dotnet"/></remarks>
    public class TestContainerOptions
    {
        /// <summary>
        /// Image Path to container image
        /// </summary>        
        public string Image { get; init; } = string.Empty;

        /// <summary>
        /// Container Name
        /// </summary>        
        public string Name { get; init; } = string.Empty;

        /// <summary>
        /// Exposed and bound ports
        /// </summary>        
        public IEnumerable<string> Ports { get; init; } = new List<string>();

        /// <summary>
        /// Environment Variables
        /// </summary>        
        public Dictionary<string, string> Environment { get; init; } = new Dictionary<string, string>();
    }
}
