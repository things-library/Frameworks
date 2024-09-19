namespace ThingsLibrary.Storage.Tests.Integration.Azure
{
    [TestClassIf, IgnoreIf(nameof(IgnoreTests)), ExcludeFromCodeCoverage]
    public class AzureTests : IBaseTests
    {
        private const FileStoreType FILE_STORE_TYPE = FileStoreType.Azure_Blob;
        public static string BucketName { get; set; } = $"aaatestbucket";
        public static IFileStore FileStore { get; set; } = null;

        #region --- Provider ---

        private static TestEnvironment TestEnvironment { get; set; }

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

            // set up the static properties
            FileStore = new Az.FileStore(TestEnvironment.ConnectionString, BucketName);
        }

        // ======================================================================
        // Called once AFTER all tests
        // ======================================================================
        [ClassCleanup]
        public static async Task ClassCleanup()
        {
            if(TestEnvironment == null) { return; }
            
            await TestEnvironment.DisposeAsync();

            // if we aren't using a test container, clean up our test bucket
            if (TestEnvironment.TestContainer == null)
            {
                //TODO: FileStore.DeleteStore();
            }
        }

        /// <summary>
        /// Show the tests be ignored due to initialize incompatibility
        /// </summary>
        /// <returns></returns>
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

