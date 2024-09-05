namespace ThingsLibrary.Database.Extensions
{
    public static class IConfigurationExtensions
    {
        public static string TryGetConnectionString(this IConfiguration configuration, string connectionStringName, bool isRequired)
        {
            if (string.IsNullOrEmpty(connectionStringName)) { throw new ArgumentNullException(nameof(connectionStringName)); }

            Log.Debug("Getting connection string '{ConnectionStringName}'...", connectionStringName);
            var connectionString = configuration.GetConnectionString(connectionStringName);
            if (string.IsNullOrEmpty(connectionString))
            {
                Log.Debug("Not Found; Trying to get '{ConnectionStringName}' with Prefix...", $"CUSTOMCONNSTR_{connectionStringName}");
                connectionString = configuration.GetConnectionString($"CUSTOMCONNSTR_{connectionStringName}");
            }

            if (string.IsNullOrEmpty(connectionString))
            {
                Log.Debug("Not Found; Trying to get '{ConnectionStringName}' from environment variables...", connectionStringName);
                connectionString = Environment.GetEnvironmentVariable(connectionStringName);
            }

            if (string.IsNullOrEmpty(connectionString))
            {
                Log.Debug("Not Found; Trying to get '{ConnectionStringName}' from environment variables with prefix...", connectionStringName);
                connectionString = Environment.GetEnvironmentVariable($"CUSTOMCONNSTR_{connectionStringName}");
            }

            // STILL NOT FOUND??!?!?
            if (string.IsNullOrEmpty(connectionString))
            {
                Log.Warning("Connection string '{ConnectionStringName}' not found!", connectionStringName);

                if (isRequired)
                {
                    throw new ArgumentException($"Unable to find connection string '{connectionStringName}'");
                }
                else
                {
                    return string.Empty;
                }
            }
            else
            {
                return connectionString;
            }
        }
    }
}
