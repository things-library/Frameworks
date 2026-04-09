namespace Starlight.Entity.Tests.Integration.Mongo
{
    [ExcludeFromCodeCoverage]
    public static class Settings
    {
        public static IConfigurationRoot GetConfigurationRoot()
        {
            return new ConfigurationBuilder()
                .AddJsonFile("appsettings.json", optional: false)
                .AddUserSecrets("e8ecb3bc-51b9-41d3-b36a-80c2e121a108") //from the project file
                .Build();
        }
    }
}
