using ThingsLibrary.Testing.Environment;

namespace ThingsLibrary.Storage.Tests.Integration.Aws
{
    [TestClassIf, IgnoreIf(nameof(IgnoreTests)), ExcludeFromCodeCoverage]
    public class AwsTests : IBaseTests
    {
        private const FileStoreType FILE_STORE_TYPE = FileStoreType.AWS_S3;
        public static string BucketName { get; set; } = $"aaatestbucket";
        public static IFileStore FileStore { get; set; } = null;

        #region --- Provider ---

        private static TestEnvironment TestEnvironment { get; set; } = new TestEnvironment();

        // ======================================================================
        // Called once before ALL tests
        // ======================================================================
        [ClassInitialize]
        public static async Task ClassInitialize(TestContext testContext)
        {
            // start test environment
            await TestEnvironment.StartAsync();

            // see if we have any reason to just exit and ignore tests
            if (TestEnvironment.IgnoreTests()) { return; }
                        
            // set up the static properties
            FileStore = new Aw.FileStore(TestEnvironment.ConnectionString, BucketName);
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
