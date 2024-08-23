namespace ThingsLibrary.Testing.Environment
{
    /// <summary>
    /// Various elements of what is the makeup of the test environment
    /// </summary>
    public class TestEnvironmentOptions
    {
        /// <summary>
        /// Connection String Variable to use
        /// </summary>
        public string ConnectionStringVariable { get; init; } = string.Empty;

        /// <summary>
        /// Test Container Definition
        /// </summary>
        public TestContainerOptions? TestContainer { get; init; }

        /// <summary>
        /// If test container should be used/started
        /// </summary>
        public bool UseExistingContainer { get; init; } = false;
    }
}
