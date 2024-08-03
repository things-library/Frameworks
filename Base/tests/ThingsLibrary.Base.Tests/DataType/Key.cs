using ThingsLibrary.DataType;


namespace ThingsLibrary.Tests.DataType
{
    [TestClass, ExcludeFromCodeCoverage]
    public class KeyTests
    {
        [DataTestMethod]
        [DataRow("123", true)]
        [DataRow("a123", true)]
        [DataRow("1a23", true)]
        [DataRow("12a4", true)]
        [DataRow("123a", true)]
        [DataRow("A123", false)]
        [DataRow("1A23", false)]
        [DataRow("a#1234", false)]
        [DataRow("b1@234", false)]
        [DataRow("a1!34", false)]
        [DataRow("a1&34", false)]
        public void IsValidJsonKey(string testValue, bool expectedValue)
        {
            Assert.AreEqual(expectedValue, Key.IsValidJsonKey(testValue));
        }

        [TestMethod]
        public void IsValidJsonKey_BadData()
        {
            Assert.AreEqual(false, Key.IsValidJsonKey(null));
            Assert.AreEqual(false, Key.IsValidJsonKey(""));            
        }

        [DataTestMethod]
        [DataRow("123", "123")]
        [DataRow("a123", "a123")]
        [DataRow("1a23", "1a23")]
        [DataRow("12a4", "12a4")]
        [DataRow("123a", "123a")]
        [DataRow("A123", "a123")]
        [DataRow("1A23", "1a23")]
        [DataRow("a#1234", "a_1234")]
        [DataRow("b1@234", "b1_234")]
        [DataRow("a1!34", "a1_34")]
        [DataRow("a1&34", "a1_34")]
        public void GetJsonKey(string testValue, string expectedValue)
        {
            Assert.AreEqual(expectedValue, Key.GetJsonKey(testValue));
        }

        [TestMethod]
        public void GetJsonKey_BadData()
        {
            Assert.ThrowsException<ArgumentNullException>(() => Key.GetJsonKey(null));
            Assert.ThrowsException<ArgumentNullException>(() => Key.GetJsonKey(""));
        }
    }
}
