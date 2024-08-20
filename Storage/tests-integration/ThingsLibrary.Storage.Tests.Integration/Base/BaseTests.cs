using ThingsLibrary.Storage.Interfaces;

namespace ThingsLibrary.Storage.Tests.Integration.Base
{
    [ExcludeFromCodeCoverage]
    public static class BaseTests
    {
        //public static IFileStore GetFileStore(FileStoreType storeType)
        //{
        //    var bucketName = "testbucket";

        //    var configuration = Settings.GetConfigurationRoot();

        //    switch (storeType)
        //    {
        //        case FileStoreType.AWS_S3:
        //            {
        //                var connectionString = configuration.GetConnectionString("Local");
        //                if (string.IsNullOrEmpty(connectionString)) { return null; }

        //                var fileStore = new AWS.FileStore(connectionString, "", bucketName);

        //                return fileStore;
        //            }

        //        case FileStoreType.Azure_Blob:
        //            {
        //                var connectionString = configuration.GetConnectionString("Azure");
        //                if (string.IsNullOrEmpty(connectionString)) { return null; }

        //                var entityStore = new Azure.FileStore(connectionString, "", bucketName);

        //                return entityStore;
        //            }

        //        case FileStoreType.GCP_Storage:
        //            {
        //                var connectionString = configuration.GetConnectionString("GCP");
        //                if (string.IsNullOrEmpty(connectionString)) { return null; }

        //                var entityStore = new GCP.FileStore(connectionString, "", bucketName);

        //                return entityStore;
        //            }               

        //        case FileStoreType.Wasabi:
        //            {
        //                var connectionString = configuration.GetConnectionString("Wasabi");
        //                if (string.IsNullOrEmpty(connectionString)) { return null; }

        //                var entityStore = new AWS.FileStore(connectionString, "", bucketName);

        //                return entityStore;
        //            }

        //        default:
        //            {
        //                throw new ArgumentException($"Invalid store type ''{storeType}", nameof(storeType));
        //            }
        //    }

        //}

        public static string TestFolderPath { get; set; } = Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), "TestData");

        #region --- Tests ---

        public static void TestFile(IFileStore fileStore)
        {
            var fileName = "TestFile.json";

            var filePath = Path.Combine(TestFolderPath, fileName);
            var cloudFilePath = $"TestFiles/{fileName}";

            if (!System.IO.File.Exists(filePath)) { throw new ArgumentException($"File not found at: '{filePath}'"); }

            var md5 = IO.File.ComputeMD5Base64(filePath);

            // ======================================================================
            // UPLOAD FILE
            // ======================================================================
            fileStore.UploadFile(filePath, cloudFilePath);

            var cloudFile = fileStore.GetFile(cloudFilePath);

            Assert.AreEqual(md5, cloudFile.ContentMD5);
            Assert.AreEqual(Path.GetFileName(filePath), cloudFile.FileName);

            // ======================================================================
            // DOWNLOAD FILE
            // ======================================================================
            using (var stream = new MemoryStream())
            {
                fileStore.DownloadFile(cloudFilePath, stream);
                stream.Position = 0;

                Assert.AreEqual(md5, IO.File.ComputeMD5Base64(stream));
            }

            // ======================================================================
            // DELETE FILE
            // ======================================================================
            fileStore.DeleteFile(cloudFilePath);
            cloudFile = fileStore.GetFile(cloudFilePath);
            Assert.AreEqual(null, cloudFile);

            // ======================================================================
            // REVISION FILE
            // ======================================================================
            // if we have versioning on then it should have created a version when we deleted it
            if (fileStore.IsVersioning)
            {
                var versions = fileStore.GetFileVersions(cloudFilePath);
                Assert.AreEqual(1, versions.Count());
            }
        }

        public static void TestImageFile(IFileStore fileStore)
        {
            var fileName = "TestImage.jpg";
            
            var filePath = Path.Combine(TestFolderPath, fileName);
            var cloudFilePath = $"TestFiles/{fileName}";

            if (!System.IO.File.Exists(filePath)) { throw new ArgumentException($"File not found at: '{filePath}'"); }

            var md5 = IO.File.ComputeMD5Base64(filePath);

            // ======================================================================
            // UPLOAD FILE
            // ======================================================================
            fileStore.UploadFile(filePath, cloudFilePath);

            var cloudFile = fileStore.GetFile(cloudFilePath);

            Assert.AreEqual(md5, cloudFile.ContentMD5);
            Assert.AreEqual(Path.GetFileName(filePath), cloudFile.FileName);

            // ======================================================================
            // DOWNLOAD FILE
            // ======================================================================
            using (var stream = new MemoryStream())
            {
                fileStore.DownloadFile(cloudFilePath, stream);
                stream.Position = 0;

                Assert.AreEqual(md5, IO.File.ComputeMD5Base64(stream));
            }

            // ======================================================================
            // DELETE FILE
            // ======================================================================
            fileStore.DeleteFile(cloudFilePath);
            cloudFile = fileStore.GetFile(cloudFilePath);

            Assert.AreEqual(null, cloudFile);

            // ======================================================================
            // REVISION FILE
            // ======================================================================
            // if we have versioning on then it should have created a version when we deleted it
            if (fileStore.IsVersioning)
            {                
                var versions = fileStore.GetFileVersions(cloudFilePath);
                Assert.AreEqual(1, versions.Count());
            }
        }

        #endregion
    }
}

