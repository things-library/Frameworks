namespace Starlight.Cloud.File.Tests.Integration.Aws
{
    [TestClassIf, IgnoreIf(nameof(IgnoreTests)), ExcludeFromCodeCoverage]
    public class AwsTests : IBaseTests
    {
        private const FileStoreType FILE_STORE_TYPE = FileStoreType.AWS_S3;
        public static string BucketName { get; set; } = $"aaatestbucket";
        public static IFileStore FileStore { get; set; } = null;

        #region --- Provider ---

        private static TestcontainersContainer TestContainer { get; set; }

        private static void Init()
        {
            var configuration = Settings.GetConfigurationRoot();

            var connectionString = configuration.GetConnectionString("AWS");
            if (string.IsNullOrEmpty(connectionString)) { return; }

            // get a test container to use for our tests            
            var testContainerSection = configuration.GetSection("TestContainer");
            if (testContainerSection.Exists())
            {
                var containerConfig = testContainerSection.Get<TestContainerConfig>();

                Console.Write("Starting docker container...");
                TestContainer = containerConfig.TryStartContainer();
                if (TestContainer == null) { return; }
                Console.WriteLine("Done");
            }

            // set up the static properties
            FileStore = new Aw.FileStore(connectionString, BucketName);
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
