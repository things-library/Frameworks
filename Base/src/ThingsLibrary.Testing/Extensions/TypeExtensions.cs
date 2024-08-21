using Microsoft.Extensions.Configuration;

namespace ThingsLibrary.Storage.Tests
{
    public static class TypeExtensions
    {
        /// <summary>
        /// Gets the configuration root with appSettings.json and user secrets
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public static IConfigurationRoot GetConfigurationRoot(this Type type)
        {
            return new ConfigurationBuilder()
                .AddJsonFile("appsettings.json", optional: false)
                .AddUserSecrets(Assembly.GetAssembly(type)!)
                .Build();
        }
    }
}

