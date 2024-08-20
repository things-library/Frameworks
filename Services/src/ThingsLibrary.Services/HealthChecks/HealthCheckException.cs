namespace ThingsLibrary.Services.HealthChecks
{
    /// <summary>
    /// Exception occurred during a health check event
    /// </summary>
    public class HealthCheckException : Exception
    {
        /// <summary>
        /// Exception occurred during a health check event
        /// </summary>
        public HealthCheckException()
        {
            //nothing
        }

        /// <summary>
        /// Exception occurred during a health check event
        /// </summary>
        /// <param name="message">Error Message</param>
        public HealthCheckException(string message) : base(message)
        {
            //nothing
        }

        /// <summary>
        /// Exception occurred during a health check event
        /// </summary>
        /// <param name="message">Error Message</param>
        /// <param name="inner">Inner Exception</param>
        public HealthCheckException(string message, Exception inner) : base(message, inner)
        {
            //nothing
        }
    }
}

