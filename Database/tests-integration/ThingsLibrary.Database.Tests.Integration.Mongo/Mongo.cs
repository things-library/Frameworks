using MongoDB.Driver.Core.Configuration;
using MongoDB.Driver;
using System.Diagnostics.CodeAnalysis;
using ThingsLibrary.Testing.Attributes;
using ThingsLibrary.Testing.Environment;

namespace ThingsLibrary.Database.Tests.Integration.Mongo
{
    [TestClassIf, IgnoreIf(nameof(IgnoreTests)), ExcludeFromCodeCoverage]
    public class MongoTests
    {        
        #region --- Provider ---

        private static TestEnvironment TestEnvironment { get; set; }

        private static MongoClient MongoClient { get; set; }
        private static DataContext? DB { get; set; }

        // ======================================================================
        // Called once before ALL tests
        // ======================================================================
        [ClassInitialize]
        public static async Task ClassInitialize(TestContext testContext)
        {
            TestEnvironment = new TestEnvironment();

            // if we have no connection string we have nothing to test
            if (string.IsNullOrWhiteSpace(TestEnvironment.ConnectionString))
            {
                Console.WriteLine("NO CONNECTION STRING TO USE FOR TESTING.");
                return;
            }

            // start test environment
            await TestEnvironment.StartAsync();

            MongoTests.MongoClient = new MongoClient(TestEnvironment.ConnectionString);

            DB = DataContext.Create(MongoTests.MongoClient.GetDatabase("testdatabase"));
            
            DB.Database.EnsureCreated();            
        }

        // ======================================================================
        // Called once AFTER all tests
        // ======================================================================
        [ClassCleanup]
        public static async Task ClassCleanup()
        {
            await TestEnvironment.DisposeAsync();

            // if we aren't using a test container, clean up our test bucket
            if (TestEnvironment.TestContainer == null)
            {
                
            }
        }
        
        public static bool IgnoreTests()
        {
            return (DB == null);
        }

        #endregion

        [TestMethod]
        public void InsertFetch()
        {
            var testData = TestData.TestClass.Get();

            DB!.TestClasses.Add(testData);

            DB.SaveChanges();

            var testData2 = DB.TestClasses.SingleOrDefault(x => x.RowKey == testData.RowKey);
            Assert.IsNotNull(testData2);

        }
        
    }
}