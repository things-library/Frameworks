using Az = ThingsLibrary.Storage.Azure;

namespace ThingsLibrary.Storage.Tests.Integration.Azure
{
    [TestClassIf, IgnoreIf(nameof(IgnoreTests)), ExcludeFromCodeCoverage]
    public class AzureTests : IBaseTests
    {
        private const FileStoreType FILE_STORE_TYPE = FileStoreType.Azure_Blob;
        public static string BucketName { get; set; } = $"aaatestbucket";
        public static IFileStore FileStore { get; set; } = null;

        #region --- Provider ---

        private static IContainer TestContainer { get; set; }

        private static void Init()
        {
            var configuration = Settings.GetConfigurationRoot();

            var connectionString = configuration.GetConnectionString("Azure");
            if (string.IsNullOrEmpty(connectionString)) { return; }

            // get a test container to use for our tests            
            var testContainerSection = configuration.GetSection("TestContainer");
            if (testContainerSection.Exists())
            {
                var containerConfig = testContainerSection.Get<TestContainerOptions>();

                Console.Write("Starting docker container...");
                //TODO: TestContainer = containerConfig.TryStartContainer();
                if (TestContainer == null) { return; }
                Console.WriteLine("Done");
            }

            // set up the static properties
            FileStore = new Az.FileStore(connectionString, BucketName);
        }

        // ======================================================================
        // Called once before ALL tests
        // ======================================================================
        [ClassInitialize]
        public static void ClassInitialize(TestContext testContext)
        {
            Init();
        }

        // ======================================================================
        // Called once AFTER all tests
        // ======================================================================
        [ClassCleanup]
        public static void ClassCleanup()
        {
            //FileStore?.DeleteStore();

            TestContainer?.DisposeAsync();
        }

        public static bool IgnoreTests()
        {
            return (FileStore == null);
        }

        #endregion

        #region --- Provider Specific Tests


        #endregion

        #region --- BASE TESTS ---

        [TestMethodIf]
        public void StoreType()
        {
            Assert.AreEqual(FILE_STORE_TYPE, FileStore.StorageType);
        }

        [TestMethodIf]
        public void TestFile()
        {
            BaseTests.TestFile(FileStore);
        }

        [TestMethodIf]
        public void TestImageFile()
        {
            BaseTests.TestImageFile(FileStore);
        }

        #endregion
    }
}

