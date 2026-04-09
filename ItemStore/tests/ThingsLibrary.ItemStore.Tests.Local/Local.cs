// ================================================================================
// <copyright file="Local.cs" company="Starlight Software Co">
//    Copyright (c) 2025 Starlight Software Co. All rights reserved.
//    Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
// </copyright>
// ================================================================================

using System.Runtime.CompilerServices;
using Microsoft.Extensions.Configuration;

using Serilog;
using ThingsLibrary.Testing.Attributes;

namespace ThingsLibrary.ItemStore.Tests
{
    [TestClassIf, IgnoreIf(nameof(ItemStoreMissing))]
    public class LocalDb : UnitTests
    {
        //NOTE:   FOR SOME REASON MULTIPLE THREADS WRITING TO THE COLLECTION AT THE SAME TIME CAUSES MISSING DATA ISSUES (AKA: Envelope.Data == null)
        //        PARALLISM IS SET TO CLASS LEVEL TO AVOID THIS ISSUE DURING TEST RUNS

        private static readonly string CollectionName = $"tests_integration";

        #region --- Fixtures ---

        [AssemblyInitialize]
        public static void AssemblyInit(TestContext context)
        {
            Log.Logger = new LoggerConfiguration().CreateLogger(); // new LoggerConfiguration()

            UnitTests.Configuration = new ConfigurationBuilder()
               .SetBasePath(AppDomain.CurrentDomain.BaseDirectory)
               .AddJsonFile("appsettings.json")
               .AddUserSecrets(System.Reflection.Assembly.GetExecutingAssembly())       // Source: %APPDATA%\Microsoft\UserSecrets\<user_secrets_id>\secrets.json
               .Build();

            _ = UnitTests.Configuration.InitCanvas();
        }

        [ClassInitialize]
        public static void ClassInit(TestContext context)
        {
            try
            {
                Assert.IsNotNull(App.Service.Canvas);

                var resourceKey = "databases/local";
                if (!App.Service.Canvas.TryGetItem(resourceKey, out var configOptions))
                {
                    throw new ArgumentException($"Missing '{resourceKey}' canvas config section");
                }

                var connectionStringVariable = configOptions["connection_string_variable"] ?? throw new ArgumentException("'connection_string_variable' missing from cache options");
                ConnectionString = UnitTests.Configuration.TryGetConnectionString(connectionStringVariable) ?? throw new ArgumentException($"Unable to find connection string '{connectionStringVariable}'");

                Assert.IsNotEmpty(ConnectionString, $"Connection string '{connectionStringVariable}' is missing or empty");

                ItemStore = new ItemStoreLocal(ConnectionString, CollectionName);
            }
            catch(ArgumentException ex)
            {
                Log.Error(ex, "Local LiteDB ClassInit failed");
            }
        }

        [ClassCleanup]
        public static void ClassCleanup()
        {
            if (ItemStore != null)
            {
                var localEntityStore = (ItemStoreLocal)ItemStore;
                var filePath = localEntityStore.FilePath;

                // clean up the 
                localEntityStore.Client.DropCollection(localEntityStore.CollectionName);
                localEntityStore.Dispose();
                
                GC.Collect();

                //Try to clean up the Test database file if exists
                try
                {
                    if (!string.IsNullOrEmpty(filePath) && System.IO.File.Exists(filePath))
                    {
                        System.IO.File.Delete(filePath);
                    }
                }
                catch
                {
                    //nothing
                }
                
            }
        }

        [TestInitialize]
        public void TestInit()
        {
            // This method is called before each test method.
        }

        [TestCleanup]
        public void TestCleanup()
        {
            // This method is called after each test method.
        }

        #endregion


        [TestMethod()]
        public void Constructor()
        {            
            var store = new ItemStoreLocal(ConnectionString, CollectionName);

            Assert.AreEqual(ItemStoreType.Local, store.StoreType);
            Assert.AreEqual(CollectionName, store.CollectionName);

            Assert.IsNotEmpty(store.FilePath);
            //Assert.IsTrue(store.IsMemoryDb);            
            Assert.IsFalse(store.IsFileSaving);
        }


        [TestMethod]
        [DataRow(null, "TestTable")]   // Null connectionstring
        [DataRow("", "TestTable")]     // Empty connection string
        [DataRow("Something;something;something", null)] // Null table name
        [DataRow("Something;something;something", "")]   // Empty table name        
        public void Constructor_BadParameters(string connectionString, string tableName)
        {
            Assert.Throws<ArgumentException>(() => new ItemStoreLocal(connectionString, tableName));
        }

        [TestMethod]        
        [DataRow("1")]  // non-letter first character
        [DataRow("_")]  // non-letter first character
        [DataRow("$")]  // non-letter first character
        [DataRow("Space Character")]  //non alpha numeric character
        [DataRow("Bad#Character")]    //non alpha numeric character
        [DataRow("Bad%Character")]    //non alpha numeric character
        [DataRow("5BadCharacter")]    //first character must be a letter
        public void Constructor_TestTableName(string tableName)
        {
            Assert.Throws<ArgumentException>(() => new ItemStoreLocal(ConnectionString, tableName));
        }
    }
}

