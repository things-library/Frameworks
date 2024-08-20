namespace ThingsLibrary.Services.Extensions
{
    public static class ConfigurationExtensions
    {
        /// <summary>
        /// Add the various app settings (base, env, env variables, secrets)
        /// </summary>
        /// <param name="context"></param>
        /// <param name="configurationBuilder"></param>
        /// <returns></returns>
        public static HostBuilderContext ConfigureAppConfiguration(this HostBuilderContext context, IConfigurationBuilder configurationBuilder)
        {            
            configurationBuilder.AddJsonFile(Path.Combine(context.HostingEnvironment.ContentRootPath, $"appsettings.json"), optional: false, reloadOnChange: false);
            configurationBuilder.AddJsonFile(Path.Combine(context.HostingEnvironment.ContentRootPath, $"appsettings.{context.HostingEnvironment.EnvironmentName}.json"), optional: true, reloadOnChange: false);
            configurationBuilder.AddEnvironmentVariables();
            configurationBuilder.AddUserSecrets(System.Reflection.Assembly.GetEntryAssembly()!, true);

            // for chaining
            return context;
        }

        /// <summary>
        /// Get connection string in the various places it could be.
        /// </summary>
        /// <param name="configuration">Configuration</param>
        /// <param name="connectionStringName">Connection String Name</param>
        /// <param name="isRequired">If it is required to be found</param>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException"></exception>
        /// <exception cref="ArgumentException"></exception>
        public static string TryGetConnectionString(this IConfiguration configuration, string? connectionStringName, bool isRequired = true)
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
