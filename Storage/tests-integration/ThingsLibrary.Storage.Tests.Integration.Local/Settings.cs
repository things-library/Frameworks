namespace ThingsLibrary.Storage.Tests.Integration.Local
{
    [ExcludeFromCodeCoverage]
    public static class Settings
    {
        public static IConfigurationRoot GetConfigurationRoot()
        {
            return new ConfigurationBuilder()
                .AddJsonFile("appsettings.json", optional: false)
                .AddEnvironmentVariables()
                .AddUserSecrets("4139a283-ebdb-4671-b2bc-282d2564bb39") //from the project file
                .Build();
        }
    }
}
