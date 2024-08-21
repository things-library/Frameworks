namespace ThingsLibrary.Storage.Tests.Integration.Gcp
{
    [TestClassIf, IgnoreIf(nameof(IgnoreTests)), ExcludeFromCodeCoverage]
    public class GcpTests : Base.IBaseTests
    {
        private const FileStoreType FILE_STORE_TYPE = FileStoreType.GCP_Storage;
        public static string BucketName { get; set; } = $"aaatestbucket";
        public static IFileStore FileStore { get; set; } = null;

        #region --- Provider ---

        private static IContainer TestContainer { get; set; }

        private static async Task Init()
        {
            var configuration = typeof(GcpTests).GetConfigurationRoot();

            var connectionString = configuration.GetConnectionString("GCP_TestStorage");
            if (string.IsNullOrEmpty(connectionString)) { return; }

            // get a test container to use for our tests            
            var testContainerSection = configuration.GetSection("TestContainer");
            if (testContainerSection.Exists())
            {
                var containerConfig = testContainerSection.Get<TestContainerOptions>();
                TestContainer = containerConfig
                    .GetContainerBuilder()
                    .Build();

                Console.Write("Starting docker container...");
                await TestContainer.StartAsync().ConfigureAwait(false);

                Console.WriteLine("Done");
            }

            // set up the static properties
            FileStore = new Gp.FileStore(connectionString, BucketName);
        }

        // ======================================================================
        // Called once before ALL tests
        // ======================================================================
        [ClassInitialize]
        public static async Task ClassInitialize(TestContext testContext)
        {
            await Init();
        }

        // ======================================================================
        // Called once AFTER all tests
        // ======================================================================
        [ClassCleanup]
        public static async Task ClassCleanup()
        {
            // if we aren't using a test container, clean up our test bucket
            if (TestContainer != null)
            {
                await TestContainer.DisposeAsync();
            }
            else if (FileStore != null)
            {
                //TODO: FileStore.DeleteStore();
            }
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
