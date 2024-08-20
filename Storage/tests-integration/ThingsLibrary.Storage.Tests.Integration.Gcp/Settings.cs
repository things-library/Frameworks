namespace Starlight.Cloud.File.Tests.Integration.Gcp
{
    [ExcludeFromCodeCoverage]
    public static class Settings
    {
        public static IConfigurationRoot GetConfigurationRoot()
        {
            return new ConfigurationBuilder()
                .AddJsonFile("appsettings.json", optional: false)
                .AddEnvironmentVariables()
                .AddUserSecrets("8a8b69bd-47d4-4cd9-93a8-23e396b747a3") //from the project file
                .Build();
        }
    }
}
