using System.IO;
using System.Runtime.InteropServices;

namespace ThingsLibrary.Tests.IO
{
    [TestClass, ExcludeFromCodeCoverage]
    public class DirectoryTests
    {
        private static string TestDirectoryPath { get; set; }

        [ClassInitialize]
        public static void ClassInitialize(TestContext testContext)
        {
            TestDirectoryPath = testContext.TestRunDirectory;
        }

        [TestMethod]
        public void VerifyPath()
        {
            // Typical Tests
            var test1Path = Path.Combine(TestDirectoryPath, $"Test1-{Guid.NewGuid()}");
            Assert.IsTrue(!Directory.Exists(test1Path));

            ThingsLibrary.IO.Directory.VerifyPath(test1Path);
            Assert.IsTrue(Directory.Exists(test1Path));

            ThingsLibrary.IO.Directory.VerifyPath(test1Path);    //this wont do anything since the folder should already exist
            Assert.IsTrue(Directory.Exists(test1Path));

            // BAD TESTS
            Assert.Throws<ArgumentNullException>(() => ThingsLibrary.IO.Directory.VerifyPath(null));
            Assert.Throws<ArgumentException>(() => ThingsLibrary.IO.Directory.VerifyPath(""));

            // this is only a windows issue.. those characters are valid in linux
            if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            {
                Assert.Throws<DirectoryNotFoundException>(() => ThingsLibrary.IO.Directory.VerifyPath(Path.Combine(TestDirectoryPath, $"\\//*<>")));
            }            
        }

        [TestMethod]
        public void TryDeleteDirectory()
        {
            // Init 
            var test1Path = Path.Combine(TestDirectoryPath, $"Test1-{Guid.NewGuid()}");
            ThingsLibrary.IO.Directory.VerifyPath(test1Path);
            Assert.IsTrue(Directory.Exists(test1Path));

            // Typical Tests
            Assert.IsTrue(ThingsLibrary.IO.Directory.TryDeleteDirectory(test1Path));
            Assert.IsTrue(!Directory.Exists(test1Path));

            // try to delete again but it shouldn't be there to delete again
            Assert.IsFalse(ThingsLibrary.IO.Directory.TryDeleteDirectory(test1Path));

            //BAD TESTS
            Assert.Throws<ArgumentNullException>(() => ThingsLibrary.IO.Directory.TryDeleteDirectory(null));
            Assert.Throws<ArgumentNullException>(() => ThingsLibrary.IO.Directory.TryDeleteDirectory(""));            
        }

        [TestMethod]
        public void RecursiveCopy()
        {
            var fileData = ThingsLibrary.IO.Embedded.LoadAsString("TestData.TestFile.json");

            // INIT the test
            var sourcePath = Path.Combine(TestDirectoryPath, $"TestSource-{Guid.NewGuid()}");
            var destinationPath = Path.Combine(TestDirectoryPath, $"TestDestination-{Guid.NewGuid()}");

            ThingsLibrary.IO.Directory.VerifyPath(sourcePath);
            Assert.IsFalse(ThingsLibrary.IO.Directory.RecursiveCompare(sourcePath, destinationPath));
            ThingsLibrary.IO.Directory.VerifyPath(destinationPath);
            Assert.IsTrue(ThingsLibrary.IO.Directory.RecursiveCompare(sourcePath, destinationPath));

            // put some files in the source
            File.WriteAllText(Path.Combine(sourcePath, "TestFile1-0.json"), fileData);
            File.WriteAllText(Path.Combine(sourcePath, "TestFile1-2.json"), fileData);

            ThingsLibrary.IO.Directory.VerifyPath(Path.Combine(sourcePath, "Test2"));
            File.WriteAllText(Path.Combine(sourcePath, "Test2", "TestFile2-0.json"), fileData);
            File.WriteAllText(Path.Combine(sourcePath, "Test2", "TestFile2-1.json"), fileData);

            ThingsLibrary.IO.Directory.VerifyPath(Path.Combine(sourcePath, "Test2", "Test22"));
            File.WriteAllText(Path.Combine(sourcePath, "Test2", "Test22", "TestFile22-0.json"), fileData);
            File.WriteAllText(Path.Combine(sourcePath, "Test2", "Test22", "TestFile22-1.json"), fileData);

            Assert.IsFalse(ThingsLibrary.IO.Directory.RecursiveCompare(sourcePath, destinationPath));

            // Typical test
            ThingsLibrary.IO.Directory.RecursiveCopy(sourcePath, destinationPath, false);

            //Recursive compare
            Assert.IsTrue(ThingsLibrary.IO.Directory.RecursiveCompare(sourcePath, destinationPath));

            // deleting a source file should still return a TRUE on a compare
            System.IO.File.Delete(Path.Combine(sourcePath, "Test2", "Test22", "TestFile22-0.json"));
            Assert.IsTrue(ThingsLibrary.IO.Directory.RecursiveCompare(sourcePath, destinationPath));

            // change one of the files and recompare
            File.WriteAllText(Path.Combine(sourcePath, "Test2", "Test22", "TestFile22-1.json"), "SOMETHING ELSE");
            Assert.IsFalse(ThingsLibrary.IO.Directory.RecursiveCompare(sourcePath, destinationPath));
            
            // delete a deep DESTINATION file and recompare.. should not be considered the same
            System.IO.File.Delete(Path.Combine(destinationPath, "Test2", "Test22", "TestFile22-1.json"));
            Assert.IsFalse(ThingsLibrary.IO.Directory.RecursiveCompare(sourcePath, destinationPath));

            // BAD TESTS            
            Assert.Throws<ArgumentException>(() => ThingsLibrary.IO.Directory.RecursiveCopy(sourcePath, sourcePath, false));    // source and destination are the same
            Assert.Throws<ArgumentNullException>(() => ThingsLibrary.IO.Directory.RecursiveCopy(null, destinationPath, false)); // bad parameter
            Assert.Throws<ArgumentNullException>(() => ThingsLibrary.IO.Directory.RecursiveCopy("", destinationPath, false));   // bad parameter
            Assert.Throws<ArgumentNullException>(() => ThingsLibrary.IO.Directory.RecursiveCopy(sourcePath, null, false));      // bad parameter
            Assert.Throws<ArgumentNullException>(() => ThingsLibrary.IO.Directory.RecursiveCopy(sourcePath, "", false));        // bad parameter
            Assert.Throws<ArgumentException>(() => ThingsLibrary.IO.Directory.RecursiveCopy(Path.Combine(TestDirectoryPath, "BADPATH"), destinationPath, false));

            Assert.Throws<ArgumentException>(() => ThingsLibrary.IO.Directory.RecursiveCompare(sourcePath, sourcePath));        // source and destination are the same
            Assert.Throws<ArgumentNullException>(() => ThingsLibrary.IO.Directory.RecursiveCompare(null, destinationPath));     // bad parameter
            Assert.Throws<ArgumentNullException>(() => ThingsLibrary.IO.Directory.RecursiveCompare("", destinationPath));       // bad parameter
            Assert.Throws<ArgumentNullException>(() => ThingsLibrary.IO.Directory.RecursiveCompare(sourcePath, null));          // bad parameter
            Assert.Throws<ArgumentNullException>(() => ThingsLibrary.IO.Directory.RecursiveCompare(sourcePath, ""));            // bad parameter
            Assert.Throws<ArgumentException>(() => ThingsLibrary.IO.Directory.RecursiveCompare(Path.Combine(TestDirectoryPath, "BADPATH"), destinationPath));
        }
    }
}
