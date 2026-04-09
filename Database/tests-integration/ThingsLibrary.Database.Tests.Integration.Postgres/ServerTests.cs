using System.Diagnostics.CodeAnalysis;
using ThingsLibrary.Database.Tests.Integration.Base;
using ThingsLibrary.Testing.Attributes;

namespace ThingsLibrary.Database.Tests.Integration.Postgres
{
    [TestClassIf, IgnoreIf(nameof(DbTestEnvironment.IgnoreTests)), ExcludeFromCodeCoverage]
    public class ServerTests
    {
        public static DatabaseTestingEnvironment DbTestEnvironment { get; set; } = new DatabaseTestingEnvironment();

        public new static DataContext DB { get; set; }

        #region --- Provider ---
            
        // ======================================================================
        // Called once before ALL tests
        // ======================================================================
        [ClassInitialize]
        public new static async Task ClassInitialize(TestContext testContext)
        {
            await DbTestEnvironment.StartAsync();
                        
            ServerTests.DB = DataContext.Create(DbTestEnvironment.ConnectionString);

            DbTestEnvironment.DB = ServerTests.DB;
            //DbTestEnvironment.DB.Database.EnsureDeleted();        //clean up for bad run last time (if exists)
            DbTestEnvironment.DB.Database.EnsureCreated();
        }

        // ======================================================================
        // Called once AFTER all tests
        // ======================================================================
        [ClassCleanup]
        public static async Task ClassCleanup()
        {
            await DbTestEnvironment.DisposeAsync();
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