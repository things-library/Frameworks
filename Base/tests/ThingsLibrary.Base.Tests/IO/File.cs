using System.IO;
using System.Reflection;
using System.Runtime.InteropServices;

namespace ThingsLibrary.Tests.IO
{
    [TestClass, ExcludeFromCodeCoverage]
    public class FileTests
    {
        private static string TestDirectoryPath { get; set; }

        [ClassInitialize]
        public static void ClassInitialize(TestContext testContext)
        {
            TestDirectoryPath = testContext.TestRunDirectory;
        }

        [TestMethod]
        public void RootFileDelete()
        {
            // Init
            var testDirPath = Path.Combine(TestDirectoryPath, $"Test1-{Guid.NewGuid()}");
            ThingsLibrary.IO.Directory.VerifyPath(testDirPath);
            Assert.IsTrue(Directory.Exists(testDirPath));

            var rootFilePath = Path.Combine(testDirPath, "TestFile1.json");
            File.WriteAllText(Path.Combine(testDirPath, rootFilePath), "1234567890");
            File.WriteAllText(Path.Combine(testDirPath, "TestFile1.txt"), "1234567890");
            File.WriteAllText(Path.Combine(testDirPath, "TestFile1.dat"), "1234567890");
            File.WriteAllText(Path.Combine(testDirPath, "TestFile1.mtn.jpg"), "1234567890");
            File.WriteAllText(Path.Combine(testDirPath, "TestFile1.dtn.jpg"), "1234567890");

            // delete the root
            ThingsLibrary.IO.File.RootFileDelete(rootFilePath);

            Assert.IsFalse(File.Exists(Path.Combine(testDirPath, rootFilePath)));
            Assert.IsFalse(File.Exists(Path.Combine(testDirPath, "TestFile1.txt")));
            Assert.IsFalse(File.Exists(Path.Combine(testDirPath, "TestFile1.dat")));
            Assert.IsFalse(File.Exists(Path.Combine(testDirPath, "TestFile1.mtn.jpg")));
            Assert.IsFalse(File.Exists(Path.Combine(testDirPath, "TestFile1.dtn.jpg")));

            //BAD Tests
            Assert.ThrowsException<ArgumentNullException>(() => ThingsLibrary.IO.File.RootFileDelete(null));
            Assert.ThrowsException<ArgumentNullException>(() => ThingsLibrary.IO.File.RootFileDelete(""));
            Assert.ThrowsException<ArgumentException>(() => ThingsLibrary.IO.File.RootFileDelete(Path.Combine(testDirPath, "MISSINGFILE.txt")));
        }

        [TestMethod]
        public void RootFileCopy()
        {
            // source
            var testSourcePath = Path.Combine(TestDirectoryPath, $"TestSource-{Guid.NewGuid()}");
            ThingsLibrary.IO.Directory.VerifyPath(testSourcePath);
            Assert.IsTrue(Directory.Exists(testSourcePath));
            
            var rootFilePath = Path.Combine(testSourcePath, "TestFile1.json");
            File.WriteAllText(Path.Combine(testSourcePath, rootFilePath), "1234567890");
            File.WriteAllText(Path.Combine(testSourcePath, "TestFile1.txt"), "1234567890");
            File.WriteAllText(Path.Combine(testSourcePath, "TestFile1.dat"), "1234567890");
            File.WriteAllText(Path.Combine(testSourcePath, "TestFile1.mtn.jpg"), "1234567890");
            File.WriteAllText(Path.Combine(testSourcePath, "TestFile1.dtn.jpg"), "1234567890");

            // destination
            var testDestinationPath = Path.Combine(TestDirectoryPath, $"TestDestination-{Guid.NewGuid()}");
            ThingsLibrary.IO.Directory.VerifyPath(testDestinationPath);
            Assert.IsTrue(Directory.Exists(testDestinationPath));

            // DO COMMAND
            ThingsLibrary.IO.File.RootFileCopy(testSourcePath, "TestFile1", testDestinationPath, false);

            //TEST FOR COPIES
            Assert.IsTrue(File.Exists(Path.Combine(testDestinationPath, rootFilePath)));
            Assert.IsTrue(File.Exists(Path.Combine(testDestinationPath, "TestFile1.txt")));
            Assert.IsTrue(File.Exists(Path.Combine(testDestinationPath, "TestFile1.dat")));
            Assert.IsTrue(File.Exists(Path.Combine(testDestinationPath, "TestFile1.mtn.jpg")));
            Assert.IsTrue(File.Exists(Path.Combine(testDestinationPath, "TestFile1.dtn.jpg")));


            //BAD Tests
            Assert.ThrowsException<ArgumentNullException>(() => ThingsLibrary.IO.File.RootFileCopy("", "Something", testDestinationPath));
            Assert.ThrowsException<ArgumentNullException>(() => ThingsLibrary.IO.File.RootFileCopy(null, "Something", testDestinationPath));

            Assert.ThrowsException<ArgumentNullException>(() => ThingsLibrary.IO.File.RootFileCopy(testSourcePath, "", testDestinationPath));
            Assert.ThrowsException<ArgumentNullException>(() => ThingsLibrary.IO.File.RootFileCopy(testSourcePath, null, testDestinationPath));

            Assert.ThrowsException<ArgumentNullException>(() => ThingsLibrary.IO.File.RootFileCopy(testSourcePath, "Something", ""));
            Assert.ThrowsException<ArgumentNullException>(() => ThingsLibrary.IO.File.RootFileCopy(testSourcePath, "Something", null));

            Assert.ThrowsException<ArgumentException>(() => ThingsLibrary.IO.File.RootFileCopy(testSourcePath, "Something", testSourcePath));
                                    
            Assert.ThrowsException<ArgumentException>(() => ThingsLibrary.IO.File.RootFileCopy(Path.Combine(testSourcePath, "BADFOLDER"), "Something", testDestinationPath));
            Assert.ThrowsException<ArgumentException>(() => ThingsLibrary.IO.File.RootFileCopy(testSourcePath, "Something", Path.Combine(testDestinationPath, "BADFOLDER")));
        }

        #region --- MD5 Hashing ---

        // used online calculator https://md5calc.com/hash/md5 to get

        [DataTestMethod]
        [DataRow("Hello World", "b10a8db164e0754105b7a99be72e3fe5")]
        [DataRow("This is a test of the MD5 hashing algorithm", "1a88d16c6414fc3ebe3eb6932b3f2e4b")]
        [DataRow("SuPe4S!r0#g_P@$$W0#d!", "369db275f46da8c0dd3a98d9670799fb")]
        [DataRow("12345678901234567890123456789012345678901234567890", "fb46ea63c015ee690bd3f2e50a461296")]
        [DataRow("g5MDEyMzQ1Njc4OTAxMjM0NTY3ODkwMTIzNDU2Nzg5MDEyMzQ1Njc4OTA=", "0f5f8b4310a5469879035286f7c13708")]
        public void ComputeMD5(string testValue, string expectedValue)
        {
            var stream = new System.IO.MemoryStream(System.Text.Encoding.UTF8.GetBytes(testValue));
            var md5 = ThingsLibrary.IO.File.ComputeMD5(stream);

            var md5Hex = string.Concat(md5.Select(x => x.ToString("X2")));

            Assert.AreEqual(expectedValue, md5Hex.ToLower());
        }

        [DataTestMethod]
        [DataRow("Hello World", "sQqNsWTgdUEFt6mb5y4/5Q==")]
        [DataRow("This is a test of the MD5 hashing algorithm", "GojRbGQU/D6+PraTKz8uSw==")]
        [DataRow("SuPe4S!r0#g_P@$$W0#d!", "Np2ydfRtqMDdOpjZZweZ+w==")]
        [DataRow("12345678901234567890123456789012345678901234567890", "+0bqY8AV7mkL0/LlCkYSlg==")]
        [DataRow("g5MDEyMzQ1Njc4OTAxMjM0NTY3ODkwMTIzNDU2Nzg5MDEyMzQ1Njc4OTA=", "D1+LQxClRph5A1KG98E3CA==")]
        public void ComputeMD5Base64(string testValue, string expectedValue)
        {
            var byteData = System.Text.Encoding.UTF8.GetBytes(testValue);
            var stream = new System.IO.MemoryStream(byteData);
            var md5FromStream = ThingsLibrary.IO.File.ComputeMD5Base64(stream);

            Assert.AreEqual(expectedValue, md5FromStream);

            var md5FromBytes = ThingsLibrary.IO.File.ComputeMD5Base64(byteData);
            Assert.AreEqual(expectedValue, md5FromBytes);
        }

        [TestMethod]
        public void ComputeMD5Base64_BadData()
        {
            // file path parameter
            Assert.ThrowsException<ArgumentNullException>(() => ThingsLibrary.IO.File.ComputeMD5Base64(""));
            Assert.ThrowsException<ArgumentNullException>(() => ThingsLibrary.IO.File.ComputeMD5Base64((string)null));
            Assert.ThrowsException<ArgumentException>(() => ThingsLibrary.IO.File.ComputeMD5Base64(Path.Combine(TestDirectoryPath, "BADFOLDER")));

            // byte parameter
            Assert.ThrowsException<ArgumentNullException>(() => ThingsLibrary.IO.File.ComputeMD5Base64(new byte[0]));
            Assert.ThrowsException<ArgumentNullException>(() => ThingsLibrary.IO.File.ComputeMD5(new byte[0]));
        }

        [TestMethod]
        public void FileSize()
        {
            var appPath = System.IO.Path.GetDirectoryName(Assembly.GetEntryAssembly().Location);

            Assert.AreEqual(187, ThingsLibrary.IO.File.GetFileSize(Path.Combine(appPath, "TestData", "TestFile.json")));
            Assert.AreEqual(143600, ThingsLibrary.IO.File.GetFileSize(Path.Combine(appPath, "TestData", "TestImage.jpg")));

            //BAD DATA
            Assert.ThrowsException<ArgumentNullException>(() => ThingsLibrary.IO.File.GetFileSize(""));
            Assert.ThrowsException<ArgumentNullException>(() => ThingsLibrary.IO.File.GetFileSize(null));
            Assert.ThrowsException<ArgumentException>(() => ThingsLibrary.IO.File.GetFileSize(Path.Combine(appPath, "BADFOLDER", "TestFile.json")));
        }

        #endregion

        #region --- File Naming ---

        [TestMethod]
        public void IsFileNameValid()
        {
            Assert.IsTrue(ThingsLibrary.IO.File.IsFileNameValid("Test1234567890"));
            Assert.IsTrue(ThingsLibrary.IO.File.IsFileNameValid("Test1234567890.txt"));

            if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            {
                Assert.IsFalse(ThingsLibrary.IO.File.IsFileNameValid("Test123\\4567890.txt"));
                Assert.IsFalse(ThingsLibrary.IO.File.IsFileNameValid("Test123/4567890.txt"));
                Assert.IsFalse(ThingsLibrary.IO.File.IsFileNameValid("Test123:4567890.txt"));
                Assert.IsFalse(ThingsLibrary.IO.File.IsFileNameValid("Test123*4567890.txt"));
                Assert.IsFalse(ThingsLibrary.IO.File.IsFileNameValid("Test123?4567890.txt"));
                Assert.IsFalse(ThingsLibrary.IO.File.IsFileNameValid("Test123\"4567890.txt"));
                Assert.IsFalse(ThingsLibrary.IO.File.IsFileNameValid("Test123<4567890.txt"));
                Assert.IsFalse(ThingsLibrary.IO.File.IsFileNameValid("Test123>4567890.txt"));
                Assert.IsFalse(ThingsLibrary.IO.File.IsFileNameValid("Test123|4567890.txt"));                
            }
            else
            {
                Assert.IsFalse(ThingsLibrary.IO.File.IsFileNameValid("Test123/4567890.txt"));
            }                
        }

        [TestMethod]
        public void GetValidFileName()
        {
            Assert.AreEqual("Test1234567890", ThingsLibrary.IO.File.GetValidFileName("Test1234567890"));
            Assert.AreEqual("Test1234567890.txt", ThingsLibrary.IO.File.GetValidFileName("Test1234567890.txt"));

            if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            {
                Assert.AreEqual("Test1234567890.txt", ThingsLibrary.IO.File.GetValidFileName("Test123\\4567890.txt"));
                Assert.AreEqual("Test1234567890.txt", ThingsLibrary.IO.File.GetValidFileName("Test123/4567890.txt"));
                Assert.AreEqual("Test1234567890.txt", ThingsLibrary.IO.File.GetValidFileName("Test123:4567890.txt"));
                Assert.AreEqual("Test1234567890.txt", ThingsLibrary.IO.File.GetValidFileName("Test123*4567890.txt"));
                Assert.AreEqual("Test1234567890.txt", ThingsLibrary.IO.File.GetValidFileName("Test123?4567890.txt"));
                Assert.AreEqual("Test1234567890.txt", ThingsLibrary.IO.File.GetValidFileName("Test123\"4567890.txt"));
                Assert.AreEqual("Test1234567890.txt", ThingsLibrary.IO.File.GetValidFileName("Test123<4567890.txt"));
                Assert.AreEqual("Test1234567890.txt", ThingsLibrary.IO.File.GetValidFileName("Test123>4567890.txt"));
                Assert.AreEqual("Test1234567890.txt", ThingsLibrary.IO.File.GetValidFileName("Test123|4567890.txt"));               
            }
            else
            {
                Assert.AreEqual("Test1234567890.txt", ThingsLibrary.IO.File.GetValidFileName("Test123/4567890.txt"));
            }            
        }

        #endregion

        #region --- Mime ---

        [DataTestMethod]
        [DataRow("somethingNotInList", "bin")]  //default extension = bin        
        [DataRow("application/vnd.google-earth.kmz", "kmz")]
        [DataRow("text/plain", "txt")]
        [DataRow("video/mpeg", "mpeg")]
        [DataRow("video/x-msvideo", "avi")]
        [DataRow("video/x-m4v", "m4v")]
        [DataRow("video/quicktime", "qt")]
        [DataRow("video/h264", "h264")]
        [DataRow("video/mpeg", "mpeg")]
        public void GetExtension(string mimeType, string expectedValue)
        {
            Assert.AreEqual(expectedValue, ThingsLibrary.IO.File.GetMimeExtension(mimeType));
        }

        [DataTestMethod]
        [DataRow("somethingNotInList.qzqz", "application/octet-stream")]  //default extension = bin        
        [DataRow("something.kmz", "application/vnd.google-earth.kmz")]
        [DataRow("something.txt", "text/plain")]
        [DataRow("something.avi", "video/x-msvideo")]
        [DataRow("something.m4v", "video/x-m4v")]
        [DataRow("something.mov", "video/quicktime")]
        [DataRow("something.qt", "video/quicktime")]
        [DataRow("something.h264", "video/h264")]
        [DataRow("something.mpeg", "video/mpeg")]
        public void GetMimeType(string filename, string expectedValue)
        {
            Assert.AreEqual(expectedValue, ThingsLibrary.IO.File.GetMimeType(filename));
        }

        [TestMethod]
        public void GetMimeType_BadData()
        {            
            //BAD DATA
            Assert.ThrowsException<ArgumentNullException>(() => ThingsLibrary.IO.File.GetMimeType(""));
            Assert.ThrowsException<ArgumentNullException>(() => ThingsLibrary.IO.File.GetMimeType(null));                        
        }

        #endregion

        #region --- Compression ---

        [TestMethod]
        public void CompressToZip()
        {
            var testFile2Path = "TestData.TestFile.json";
                        
            // load the embedded data and save to a file
            var fileData = ThingsLibrary.IO.Embedded.LoadAsString(testFile2Path);

            // SAVE FILE OUT
            var testFilePath = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString(), $"TestFile.json");
            Directory.CreateDirectory(Path.GetDirectoryName(testFilePath));

            File.WriteAllText(testFilePath, fileData);
            Assert.IsTrue(File.Exists(testFilePath));

            // ZIP FILE USING METHOD
            var testZipFilePath = Path.Combine(Path.GetDirectoryName(testFilePath), Path.GetFileNameWithoutExtension(testFilePath) + ".zip");
            ThingsLibrary.IO.File.CompressToZip(testFilePath, testZipFilePath, false);

            Assert.IsTrue(File.Exists(testZipFilePath));

            // already exists should throw a exception if not overwritten
            Assert.ThrowsException<IOException>(() => ThingsLibrary.IO.File.CompressToZip(testFilePath, testZipFilePath, false));

            // COMPRESS AGAIN AND MAKE SURE IT DOESNT ERROR
            ThingsLibrary.IO.File.CompressToZip(testFilePath, testZipFilePath, true);
            Assert.IsTrue(File.Exists(testZipFilePath));

            // test clean up
            ThingsLibrary.IO.Directory.TryDeleteDirectory(Path.GetDirectoryName(testFilePath));
        }

        #endregion
    }
}
