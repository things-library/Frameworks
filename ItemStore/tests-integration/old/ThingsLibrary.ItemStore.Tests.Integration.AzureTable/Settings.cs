namespace Starlight.Entity.Tests.Integration.AzureTable
{
    [ExcludeFromCodeCoverage]
    public static class Settings
    {
        public static IConfigurationRoot GetConfigurationRoot()
        {
            return new ConfigurationBuilder()
                .AddJsonFile("appsettings.json", optional: false)
                .AddUserSecrets("7d4bea3b-fef5-4648-b2cc-a2243050f81d") //from the project file
                .Build();
        }
    }
}
