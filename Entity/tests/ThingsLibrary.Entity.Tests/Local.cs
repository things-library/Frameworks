using ThingsLibrary.Entity.Local;

namespace ThingsLibrary.Entity.Tests
{
    [TestClass, ExcludeFromCodeCoverage]
    public class LocalUnitTests
    {
        public static string TableName { get; set; }
        public static TestData TestData { get; set; }

        private string ConnectionString => "Filename=:memory:";

        [ClassInitialize]
        public static void ClassInitialize(TestContext testContext)
        {
            TableName = $"Test{System.Math.Abs(Guid.NewGuid().GetHashCode())}";
            TestData = new TestData();
        }

        [TestMethod]
        public void Constructor_TestClass()
        {
            var store = new EntityStore<TestClass>(ConnectionString, TableName);

            Assert.AreEqual(typeof(TestClass), store.Type);

            Assert.AreNotEqual(null, store.Key);
            Assert.AreEqual(22, store.Properties.Count);
        }

        [TestMethod]
        public void Constructor_TestInheritedClass()
        {
            var store = new EntityStore<TestInheritedClass>(ConnectionString, TableName);

            Assert.AreEqual(typeof(TestInheritedClass), store.Type);

            Assert.AreNotEqual(null, store.Key);
            Assert.AreEqual(25, store.Properties.Count);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void Constructor_TestDualKeyClass()
        {
            new EntityStore<TestDualKeyClass>(ConnectionString, TableName);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void Constructor_TestNoKeyClass()
        {
            new EntityStore<TestNoKeyClass>(ConnectionString, TableName);
        }

        [DataTestMethod]
        [DataRow(null, "TestTable")]   // Null connectionstring
        [DataRow("", "TestTable")]     // Empty connection string
        [DataRow("Something;something;something", null)] // Null table name
        [DataRow("Something;something;something", "")]   // Empty table name
        [ExpectedException(typeof(ArgumentNullException))]
        public void Constructor_TestBadParameters(string connectionString, string tableName)
        {
            new EntityStore<TestClass>(connectionString, tableName);
        }

        [DataTestMethod]
        [DataRow("A")]  // Too Short
        [DataRow("A2")] // Too short
        [DataRow("A234567890123456789012345678901234567890123456789012345678901234")]    //Too long >63
        [DataRow("Space Character")]  //non alpha numeric character
        [DataRow("Bad#Character")]    //non alpha numeric character
        [DataRow("Bad%Character")]    //non alpha numeric character
        [DataRow("5BadCharacter")]    //first character must be a letter
        [ExpectedException(typeof(ArgumentException))]
        public void Constructor_TestTableName(string tableName)
        {
            new EntityStore<TestClass>(ConnectionString, tableName);
        }
    }
}

