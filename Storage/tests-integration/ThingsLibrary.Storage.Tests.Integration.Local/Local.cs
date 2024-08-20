namespace ThingsLibrary.Storage.Tests.Integration.Local
{
    [TestClassIf, IgnoreIf(nameof(IgnoreTests)), ExcludeFromCodeCoverage]
    public class LocalTests : Base.IBaseTests
    {
        private const FileStoreType FILE_STORE_TYPE = FileStoreType.Local;
        public static string BucketName { get; set; } = $"TestBucket";
        public static Loc.FileStore FileStore { get; set; } = null;

        #region --- Provider ---
                
        private static void Init()
        {
            var configuration = Settings.GetConfigurationRoot();

            var connectionString = configuration.GetConnectionString("Local");
            if (string.IsNullOrEmpty(connectionString)) { return; }
            
            // set up the static properties
            FileStore = new Loc.FileStore(connectionString, BucketName);
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
            // dispose of the File store / closing database connection
            if(FileStore != null)
            {
                var bucketFolderPath = FileStore.BucketDirectoryPath;

                FileStore.DataContext.Dispose();
                FileStore = null;

                if (!string.IsNullOrEmpty(bucketFolderPath))
                {
                    IO.Directory.TryDeleteDirectory(bucketFolderPath);
                }
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
