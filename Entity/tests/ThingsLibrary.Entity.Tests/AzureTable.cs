using Starlight.Entity.AzureTable;

namespace Starlight.Entity.Tests
{
    [TestClass, ExcludeFromCodeCoverage]
    public class AzureTableUnitTests
    {
        public static string TableName { get; set; }
        public static TestData TestData { get; set; }

        private string ConnectionString => "DataSource=;Name=TestDabase";

        [ClassInitialize]
        public static void ClassInitialize(TestContext testContext)
        {
            TableName = $"a000Test{System.Math.Abs(Guid.NewGuid().GetHashCode())}";  // so it is at the top
            TestData = new TestData();
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

        /// <summary>
        /// Tests the invalid table naming
        /// </summary>
        /// <param name="tableName">Table Name</param>
        /// <remarks>
        /// Naming Policies:
        /// 1. Not Empty
        /// 2. Must be between 3 and 63 characters
        /// 3. Must be alphanumeric
        /// 4. Cannot begin with a number
        /// </remarks>
        [DataTestMethod]
        [DataRow("A")]                  // 2. Must be between 3 and 63 characters
        [DataRow("A2")]                 // 2. Must be between 3 and 63 characters
        [DataRow("A234567890123456789012345678901234567890123456789012345678901234")]    // 2. Must be between 3 and 63 characters
        [DataRow("Space Character")]    // 3. Must be alphanumeric
        [DataRow("Bad#Character")]      // 3. Must be alphanumeric
        [DataRow("Bad%Character")]      // 3. Must be alphanumeric
        [DataRow("5BadCharacter")]      // 4. Cannot begin with a number
        [ExpectedException(typeof(ArgumentException))]
        public void Constructor_TestTableName(string tableName)
        {
            new EntityStore<TestClass>(ConnectionString, tableName);
        }
    }
}

