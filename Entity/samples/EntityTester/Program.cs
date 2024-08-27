using Microsoft.Extensions.Configuration;

namespace EntityTester
{
    internal static class Program
    {
        /// <summary>
        ///  The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            var configuration = new ConfigurationBuilder()
               .SetBasePath(AppDomain.CurrentDomain.BaseDirectory)
               .AddJsonFile("appsettings.json")
               .AddUserSecrets(System.Reflection.Assembly.GetExecutingAssembly())       // Source: %APPDATA%\Microsoft\UserSecrets\<user_secrets_id>\secrets.json
               .Build();

            var settings = new AppSettings(configuration);
            settings.Load();
                        
            // To customize application configuration such as set high DPI settings or default font,
            // see https://aka.ms/applicationconfiguration.
            ApplicationConfiguration.Initialize();
            Application.Run(new frmMain(settings));
        }
    }
}
