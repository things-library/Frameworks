// ================================================================================
// <copyright file="Mongo.cs" company="Starlight Software Co">
//    Copyright (c) 2025 Starlight Software Co. All rights reserved.
//    Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
// </copyright>
// ================================================================================

using Serilog;
using Microsoft.Extensions.Configuration;
using ThingsLibrary.Testing.Attributes;

namespace ThingsLibrary.ItemStore.Tests.Mongo
{
    [TestClassIf, IgnoreIf(nameof(ItemStoreMissing))]
    public class MongoDB : UnitTests
    {
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


        [AssemblyCleanup]
        public static void AssemblyCleanup()
        {
            // This method is called once for the test assembly, after all tests are run.
        }

        [ClassInitialize]
        public static void ClassInit(TestContext context)
        {
            try
            {
                Assert.IsNotNull(App.Service.Canvas);

                var resourceKey = "databases/mongo";
                if (!App.Service.Canvas.TryGetItem(resourceKey, out var configOptions))
                {
                    throw new ArgumentException($"Missing '{resourceKey}' canvas config section");
                }

                DatabaseName = configOptions["database_name"] ?? throw new ArgumentException("'database_name' missing from cache options");

                var connectionStringVariable = configOptions["connection_string_variable"] ?? throw new ArgumentException("'connection_string_variable' missing from cache options");
                ConnectionString = UnitTests.Configuration.TryGetConnectionString(connectionStringVariable) ?? throw new ArgumentException($"Unable to find connection string '{connectionStringVariable}'");

                Assert.IsNotEmpty(ConnectionString, $"Connection string '{connectionStringVariable}' is missing or empty");

                ItemStore = new ItemStoreMongo(ConnectionString, DatabaseName, CollectionName);
            }
            catch(ArgumentException ex)
            {
                Log.Error(ex, "MongoDB ClassInit failed");
            }
        }

        [ClassCleanup]
        public static void ClassCleanup()
        {
            if(ItemStore != null)
            {
                var mongoEntityStore = (ItemStoreMongo)ItemStore;

                mongoEntityStore.Collection.Database.DropCollection(CollectionName);
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

        [TestMethod]
        public void Constructor()
        {   
            var store = new ItemStoreMongo(ConnectionString, DatabaseName, CollectionName);

            Assert.AreEqual(ItemStoreType.MongoDb, store.StoreType);
            Assert.AreEqual(CollectionName, store.CollectionName);

        }


        //[TestMethod]
        //[DataRow(null, "TestTable")]   // Null connectionstring
        //[DataRow("", "TestTable")]     // Empty connection string
        //[DataRow("Something;something;something", null)] // Null table name
        //[DataRow("Something;something;something", "")]   // Empty table name        
        //public void Constructor_BadParameters(string connectionString, string tableName)
        //{
        //    Assert.Throws<ArgumentException>(() => new ItemStoreMongo(connectionString, DatabaseName, tableName));
        //}

        //[TestMethod]
        //[DataRow("A")]  // Too Short
        //[DataRow("A2")] // Too short
        //[DataRow("A234567890123456789012345678901234567890123456789012345678901234")]    //Too long >63
        //[DataRow("Space Character")]  //non alpha numeric character
        //[DataRow("Bad#Character")]    //non alpha numeric character
        //[DataRow("Bad%Character")]    //non alpha numeric character
        //[DataRow("5BadCharacter")]    //first character must be a letter
        //public void Constructor_TestTableName(string tableName)
        //{
        //    Assert.Throws<ArgumentException>(() => new ItemStoreMongo(ConnectionString, DatabaseName, tableName));
        //}
    }
}

