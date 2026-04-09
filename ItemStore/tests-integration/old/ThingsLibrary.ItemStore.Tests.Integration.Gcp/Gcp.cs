using System.Diagnostics;

namespace Starlight.Cloud.Entity.Tests.Integration.Gcp
{
    [TestClassIf, IgnoreIf(nameof(IgnoreTests)), ExcludeFromCodeCoverage]
    public class GcpTests : Base.IBaseTests
    {
        private const EntityStoreType ENTITY_STORE_TYPE = EntityStoreType.GCP_DataStore;
        public static string TableName { get; set; } = $"aaaTestTable";
        public static TestData TestData { get; set; } = new TestData();
        public static IEntityStore<Base.TestClass> EntityStore { get; set; } = null;

        #region --- Provider ---
        private static TestcontainersContainer TestContainer { get; set; }

        private static void Init()
        {
            var configuration = Settings.GetConfigurationRoot();

            var connectionString = configuration.GetConnectionString("GCP");
            if (string.IsNullOrEmpty(connectionString)) { return; }
                        
            // get a test container to use for our tests
            var testContainerSection = configuration.GetSection("TestContainer");
            if (testContainerSection.Exists())
            {
                var containerConfig = testContainerSection.Get<TestContainerConfig>();

                var host = containerConfig.Environment["DATASTORE_LISTEN_ADDRESS"];
                if(string.IsNullOrEmpty(host)) { throw new ArgumentException("Environment variable 'DATASTORE_LISTEN_ADDRESS' is Missing"); }
                Environment.SetEnvironmentVariable("DATASTORE_EMULATOR_HOST", host);

                var projectId = containerConfig.Environment["DATASTORE_PROJECT_ID"];
                if(string.IsNullOrEmpty(projectId)) { throw new ArgumentException("Environment variable 'DATASTORE_PROJECT_ID' is Missing");  }
                Environment.SetEnvironmentVariable("DATASTORE_PROJECT_ID", projectId);

                var appPath = System.Reflection.Assembly.GetExecutingAssembly().Location;
                var basePath = Path.GetDirectoryName(appPath);
                Environment.SetEnvironmentVariable("GOOGLE_APPLICATION_CREDENTIALS", Path.Combine(basePath, "client_secret.json"));

                Console.Write("Starting docker container...");
                TestContainer = containerConfig.TryStartContainer();
                if (TestContainer == null) { return; }
                Console.WriteLine("Done");
            }

            // set up the static properties
            EntityStore = new Gp.EntityStore<TestClass>(connectionString, TableName);
        }

        // ======================================================================
        // Called once before ALL tests
        // ======================================================================
        [ClassInitialize]
        public static void ClassInitialize(TestContext testContext)
        {
            // load it up so we don't run into a theading issue
            Init();
        }

        // ======================================================================
        // Called once AFTER all tests
        // ======================================================================
        [ClassCleanup]
        public static void ClassCleanup()
        {
            EntityStore?.DeleteStore();

            GcpTests.TestContainer?.DisposeAsync();
        }

        public static bool IgnoreTests()
        {
            return (EntityStore == null);
        }

        #endregion

        #region --- Provider Specific Tests

        [TestMethodIf]
        public void StoreType()
        {
            // just so there is a basic test in here where all the others are inherited
            Assert.AreEqual(ENTITY_STORE_TYPE, EntityStore.StoreType);
        }

        #endregion

        #region --- BASE TESTS ---

        [TestMethodIf]
        public void InsertUpdateDelete()
        {
            BaseTests.InsertUpdateDelete(EntityStore);
        }

        [TestMethodIf]
        public void InsertDelete_InheritedClass()
        {
            BaseTests.InsertDelete_InheritedClass(EntityStore);
        }

        [TestMethodIf]
        public void InsertDeleteNullible()
        {
            BaseTests.InsertDeleteNullible(EntityStore);
        }

        [TestMethodIf]
        public void UpsertUpsertAndDelete()
        {
            BaseTests.UpsertUpsertAndDelete(EntityStore);
        }

        [TestMethodIf]
        public void InsertTwice()
        {
            BaseTests.InsertTwice(EntityStore);
        }

        [TestMethodIf]
        public void GetEntities()
        {
            BaseTests.GetEntities(EntityStore);
        }

        #endregion
    }
}

