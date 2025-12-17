// ================================================================================
// <copyright file="Program.cs" company="Starlight Software Co">
//    Copyright (c) 2025 Starlight Software Co. All rights reserved.
//    Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
// </copyright>
// ================================================================================

using Microsoft.Extensions.Configuration;
using ThingsLibrary.Services;
using ThingsLibrary.Services.Extensions;

namespace CloudEntityTester
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

            _ = configuration.InitCanvas();
            

            // To customize application configuration such as set high DPI settings or default font,
            // see https://aka.ms/applicationconfiguration.
            ApplicationConfiguration.Initialize();
            Application.Run(new frmMain());
        }
    }
}
