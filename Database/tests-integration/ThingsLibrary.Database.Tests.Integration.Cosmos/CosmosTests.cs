using Microsoft.EntityFrameworkCore;
using System.Diagnostics.CodeAnalysis;
using ThingsLibrary.Database.Cosmos;
using ThingsLibrary.Database.Tests.Integration.Base;
using ThingsLibrary.Testing.Attributes;
using ThingsLibrary.Testing.Environment;

namespace ThingsLibrary.Database.Tests.Integration.Cosmos
{
    [TestClassIf, IgnoreIf(nameof(IgnoreTests)), ExcludeFromCodeCoverage]
    public class CosmosTests
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

            var contextBuilder = DataContext.Parse<DataContext>(TestEnvironment.ConnectionString, "testdatabase");

            DB = new DataContext(contextBuilder.Options);
            
            await DB.Database.EnsureCreatedAsync();
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

        //[TestMethod]
        //public void InsertFetch()
        //{
        //    var entityTester = new EntityTester<TestData.TestInheritedClass>(DB, DB.TestInheritedClasses);

        //    var expectedData = TestData.TestInheritedClass.GetInherited();

        //    // Insert
        //    DB!.TestInheritedClasses.Add(expectedData);
        //    DB.SaveChanges(true);

        //    // clear all tracked entities
        //    DB.ChangeTracker.Clear();

        //    var compareData = DB.TestInheritedClasses.SingleOrDefault(x => x.Id == expectedData.Id);
        //    Assert.IsNotNull(compareData);

        //    compareData.UpdatedOn = DateTime.UtcNow;

        //    // Insert
        //    DB!.TestInheritedClasses.Update(compareData);
        //    DB.SaveChanges();

        //    // Fetch
        //    compareData = DB.TestInheritedClasses.SingleOrDefault(x => x.Id == expectedData.Id);
        //    Assert.IsNotNull(compareData);

        //    entityTester.CompareEntities(compareData, expectedData);
        //}

        [TestMethod]
        public void Inherited_AddUpdateDelete()
        {
            var entityTester = new EntityTester<TestData.TestInheritedClass>(DB, DB.TestInheritedClasses);

            var expectedData = TestData.TestInheritedClass.GetInherited();

            Assert.IsTrue(entityTester.AddUpdateDelete(expectedData));
        }

    }
}