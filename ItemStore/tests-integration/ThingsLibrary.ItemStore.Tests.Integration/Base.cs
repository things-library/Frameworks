// ================================================================================
// <copyright file="Base.cs" company="Starlight Software Co">
//    Copyright (c) 2025 Starlight Software Co. All rights reserved.
//    Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
// </copyright>
// ================================================================================

using Microsoft.Extensions.Configuration;
using Serilog;

namespace ThingsLibrary.Entity.Tests
{
    [TestClass, ExcludeFromCodeCoverage]
    public class Base
    {        
        public static IConfiguration Configuration { get; set; }
        public static CancellationToken CancellationToken { get; set; } = new CancellationToken();

        
        [AssemblyInitialize]
        public static void AssemblyInit(TestContext context)
        {
            Log.Logger = new LoggerConfiguration().CreateLogger(); // new LoggerConfiguration()
                
            Base.Configuration = new ConfigurationBuilder()
               .SetBasePath(AppDomain.CurrentDomain.BaseDirectory)
               .AddJsonFile("appsettings.json")
               .AddUserSecrets(System.Reflection.Assembly.GetExecutingAssembly())       // Source: %APPDATA%\Microsoft\UserSecrets\<user_secrets_id>\secrets.json
               .Build();

            _ = Base.Configuration.InitCanvas();
        }

        [AssemblyCleanup]
        public static void AssemblyCleanup()
        {
            // This method is called once for the test assembly, after all tests are run.
        }

        [TestMethod]
        public void Canvas_Test()
        {
            Assert.IsNotNull(App.Service.Canvas);
        }
    }
}

