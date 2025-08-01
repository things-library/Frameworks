﻿namespace Starlight.Cloud.Entity.Tests.Integration.Gcp
{
    [ExcludeFromCodeCoverage]
    public static class Settings
    {
        public static IConfigurationRoot GetConfigurationRoot()
        {
            return new ConfigurationBuilder()
                .AddJsonFile("appsettings.json", optional: false)
                .AddUserSecrets("f461dbad-3aa0-4d6e-b896-302df0f9143f") //from the project file
                .Build();
        }
    }
}
