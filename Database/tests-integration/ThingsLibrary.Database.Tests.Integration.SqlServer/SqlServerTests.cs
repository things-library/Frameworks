using System.Diagnostics.CodeAnalysis;
using ThingsLibrary.Database.Tests.Integration.Base;
using ThingsLibrary.Testing.Attributes;
using ThingsLibrary.Testing.Environment;

namespace ThingsLibrary.Database.Tests.Integration.SqlServer
{
    [TestClassIf, IgnoreIf(nameof(IgnoreTests)), ExcludeFromCodeCoverage]
    public class SqlServerTests
    {        
        #region --- Provider ---

        private static TestEnvironment TestEnvironment { get; set; }
                
        private static DataContext DB { get; set; }
        

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

            //DB = DataContext.Create(TestEnvironment.ConnectionString);
            //DB.Database.EnsureDeleted();        //clean up for bad run last time (if exists)
            //DB.Database.EnsureCreated();
        }

        // ======================================================================
        // Called once AFTER all tests
        // ======================================================================
        [ClassCleanup]
        public static async Task ClassCleanup()
        {
            // nothing to clean up
            if(TestEnvironment == null) { return; }

            await TestEnvironment.DisposeAsync();

            // if we aren't using a test container, clean up our test bucket
            if (TestEnvironment.TestContainer == null && DB != null)
            {
                await DB.Database.EnsureDeletedAsync();    // deletes the test database
            }
        }
        
        public static bool IgnoreTests()
        {
            return (DB == null);
        }

        #endregion

        [TestMethod]
        public void Inherited_AddUpdateDelete()
        {
            var entityTester = new EntityTester<TestData.TestInheritedClass>(DB, DB.TestInheritedClasses);

            var expectedData = TestData.TestInheritedClass.GetInherited();

            Assert.IsTrue(entityTester.AddUpdateDelete(expectedData));
        }
    }
}