namespace ThingsLibrary.Tests.DataType.Extensions
{
    [TestClass, ExcludeFromCodeCoverage]
    public class VersionTests
    {
        [DataTestMethod]
        [DataRow("1.0.0.0", 1000000000L)]
        [DataRow("1.2.0.0", 1002000000L)]
        [DataRow("1.2.3.0", 1002003000L)]
        [DataRow("1.2.3.4", 1002003004L)]
        public void ToLong(string versionStr, long expectedValue)
        {
            var version = new Version(versionStr);
            Assert.AreEqual(expectedValue, version.ToLong());
        }

        [DataTestMethod]
        [DataRow("5.4.1.0", "5.4.3.2")]
        [DataRow("5.4.4.1", "5.4.3.2")]
        [DataRow("5.4.3.0", "5.4.3.2")]
        [DataRow("5.4.3.2", "5.4.3.2")]
        public void IsBackCompatible(string versionStr, string compareVersionStr)
        {
            var version = new Version(versionStr);
            var versionCompare = new Version(compareVersionStr);

            Assert.IsTrue(versionCompare.IsBackCompatible(version));
        }

        [DataTestMethod]
        [DataRow("4.0.0.0", "5.4.3.2")]
        [DataRow("5.3.0.0", "5.4.3.2")]
        [DataRow("6.4.3.0", "5.4.3.2")]
        [DataRow("4.6.3.2", "5.4.3.2")]
        public void IsNotBackCompatible(string versionStr, string compareVersionStr)
        {
            var version = new Version(versionStr);
            var versionCompare = new Version(compareVersionStr);

            Assert.IsFalse(versionCompare.IsBackCompatible(version));
        }

        [TestMethod]
        public void ToDotString()
        {
            var version = new Version(1, 2, 3, 4);
            Assert.AreEqual("1.2.3.4", version.ToDotString());
        }

        [TestMethod]        
        public void IsBackCompatibleNull()
        {
            var version = new Version("1.0.0.0");

            Assert.Throws<ArgumentNullException>(() => version.IsBackCompatible(null));
        }
    }
}