using ThingsLibrary.IO;
using ThingsLibrary.DataType;

namespace ThingsLibrary.Tests.IO
{
    [TestClass, ExcludeFromCodeCoverage]
    public class EmbeddedTests
    {
        public const string TestFile2Path = "TestData.TestFile.json";
        public const string TestImagePath = "TestData.TestImage.jpg";


        [TestMethod]
        public void Exists()
        {
            Assert.AreEqual(true, Embedded.Exists(TestFile2Path));

            Assert.AreEqual(true, Embedded.Exists(TestImagePath));

            Assert.AreEqual(false, Embedded.Exists("BADPATH.SOMETHING.txt"));

            //BAD Tests
            Assert.ThrowsException<ArgumentNullException>(() => Embedded.Exists(null));
            Assert.ThrowsException<ArgumentNullException>(() => Embedded.Exists(""));
        }

        /// <summary>
        /// Get Stream Test File
        /// </summary>
        [TestMethod]
        public void GetStream_TestFile()
        {
            // make sure it is the same file we put there

            // ======================================================================
            // TestFilePath
            // ======================================================================
            var stream = Embedded.GetStream(TestFile2Path);

            Assert.AreEqual(187, stream.Length);
                        
            var md5 = ThingsLibrary.IO.File.ComputeMD5Base64(stream);
            Assert.AreEqual("3hCjqg6QntQssOGbuu2p0Q==", md5);

            //BAD Tests
            Assert.ThrowsException<ArgumentNullException>(() => Embedded.GetStream(null));
            Assert.ThrowsException<ArgumentNullException>(() => Embedded.GetStream(""));
        }

        /// <summary>
        /// Get Stream Test Image
        /// </summary>
        [TestMethod]
        public void GetStream_TestImage()
        {
            // make sure it is the same file we put there

            // ======================================================================
            // TestImagePath
            // ======================================================================
            var stream = Embedded.GetStream(TestImagePath);

            Assert.AreEqual(143600, stream.Length);

            var md5 = ThingsLibrary.IO.File.ComputeMD5Base64(stream);
            Assert.AreEqual("L5kVRwElSTmzcyxohH8Jlw==", md5);
        }

        [TestMethod]
        public void LoadAsString()
        {
            var fileData = Embedded.LoadAsString(TestFile2Path);
            var json = JsonDocument.Parse(fileData);

            Assert.AreEqual(12345, json.GetProperty<int>("id", 0));

            //BAD Tests
            Assert.ThrowsException<ArgumentNullException>(() => Embedded.LoadAsString(null));
            Assert.ThrowsException<ArgumentNullException>(() => Embedded.LoadAsString(""));

            Assert.ThrowsException<ArgumentException>(() => Embedded.LoadAsString("BadResourceName"));
        }

        [TestMethod]
        public void LoadByteFile()
        {
            var fileBytes = Embedded.LoadByteFile(TestImagePath);

            var md5 = ThingsLibrary.IO.File.ComputeMD5Base64(fileBytes);
            Assert.AreEqual("L5kVRwElSTmzcyxohH8Jlw==", md5);
            Assert.AreEqual(143600, fileBytes.Length);

            //BAD Tests
            Assert.ThrowsException<ArgumentNullException>(() => Embedded.LoadByteFile(null));
            Assert.ThrowsException<ArgumentNullException>(() => Embedded.LoadByteFile(""));

            Assert.ThrowsException<ArgumentException>(() => Embedded.LoadByteFile("BadResourceName"));
        }
    }
}
