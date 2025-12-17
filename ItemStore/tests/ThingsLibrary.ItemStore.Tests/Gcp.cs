using CloudProvider = Starlight.Cloud.Entity.Gcp;

namespace Starlight.Cloud.Entity.Tests
{
    [TestClass, ExcludeFromCodeCoverage]
    public class GcpUnitTests
    {
        public static string TableName { get; set; }
        public static TestData TestData { get; set; }

        private string ConnectionString => "project_id=;dataSource=;namespace=TestDabase";

        [ClassInitialize]
        public static void ClassInitialize(TestContext testContext)
        {
            TableName = $"Test{System.Math.Abs(Guid.NewGuid().GetHashCode())}";
            TestData = new TestData();
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void Constructor_TestNoKeyClass()
        {
            new CloudProvider.EntityStore<TestNoKeyClass>(ConnectionString, TableName);
        }

        [DataTestMethod]
        [DataRow(null, "TestTable")]   // Null connectionstring
        [DataRow("", "TestTable")]     // Empty connection string
        [DataRow("Something;something;something", null)] // Null table name
        [DataRow("Something;something;something", "")]   // Empty table name
        [ExpectedException(typeof(ArgumentNullException))]
        public void Constructor_TestBadParameters(string connectionString, string tableName)
        {
            new CloudProvider.EntityStore<TestClass>(connectionString, tableName);
        }

        [DataTestMethod]
        [DataRow("Something=234;Source=SomeSource;SomethingElse=1")] // Missing Project ID
        [ExpectedException(typeof(ArgumentException))]
        public void Constructor_TestConnectionString(string connectionString)
        {
            new CloudProvider.EntityStore<TestClass>(connectionString, TableName);
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
            new CloudProvider.EntityStore<TestClass>(ConnectionString, tableName);
        }
    }
}

