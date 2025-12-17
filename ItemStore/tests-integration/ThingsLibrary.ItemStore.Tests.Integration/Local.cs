// ================================================================================
// <copyright file="Local.cs" company="Starlight Software Co">
//    Copyright (c) 2025 Starlight Software Co. All rights reserved.
//    Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
// </copyright>
// ================================================================================

namespace ThingsLibrary.Entity.Tests
{
    
    [TestClass, ExcludeFromCodeCoverage]
    public class LocalDb : UnitTests
    {   
        #region --- Fixtures ---

        [ClassInitialize]
        public static void ClassInit(TestContext context)
        {            
            Assert.IsNotNull(App.Service.Canvas);

            var resourceKey = "databases/local";
            if (!App.Service.Canvas.TryGetItem(resourceKey, out var configOptions))
            {
                throw new ArgumentException($"Missing '{resourceKey}' canvas config section");
            }

            var connectionStringVariable = configOptions["connection_string_variable"] ?? throw new ArgumentException("'connection_string_variable' missing from cache options");
            ConnectionString = Base.Configuration.TryGetConnectionString(connectionStringVariable) ?? throw new ArgumentException($"Unable to find connection string '{connectionStringVariable}'");
            
            Assert.IsNotEmpty(ConnectionString, $"Connection string '{connectionStringVariable}' is missing or empty");

            var tableName = $"Test{System.Math.Abs(Guid.NewGuid().GetHashCode())}";
            EntityStore = new ItemStoreLocal(ConnectionString, tableName);            
        }

        [ClassCleanup]
        public static void ClassCleanup()
        {
            // This method is called once for the test class, after all tests of the class are run.
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
            var tableName = $"Test{System.Math.Abs(Guid.NewGuid().GetHashCode())}";
            var store = new ItemStoreLocal(ConnectionString, tableName);

            Assert.AreEqual(ItemStoreType.Local, store.StoreType);
            Assert.AreEqual(tableName, store.Name);

            Assert.IsEmpty(store.FilePath);
            Assert.IsTrue(store.IsMemoryDb);
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
        [DataRow("A")]  // Too Short
        [DataRow("A2")] // Too short
        [DataRow("A234567890123456789012345678901234567890123456789012345678901234")]    //Too long >63
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

