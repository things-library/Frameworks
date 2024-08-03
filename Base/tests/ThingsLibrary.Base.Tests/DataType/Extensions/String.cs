namespace ThingsLibrary.Tests.DataType.Extensions
{
    [TestClass, ExcludeFromCodeCoverage]
    public class StringTests
    {
        [DataTestMethod]
        [DataRow("Hello World", "hElLo wOrLd")]
        [DataRow("This is SpongeBob", "tHiS Is sPoNgEbOb")]
        public void ToSpongebobText(string testValue, string expectedValue)
        {
            Assert.AreEqual(expectedValue, testValue.ToSpongebobText());
        }
    
        [DataTestMethod]
        [DataRow("HelloWorld==", true)]
        [DataRow("Thi", false)]
        [DataRow("Thi0412=", true)]
        [DataRow("IEhlbGxv", true)]
        [DataRow("IEhlbGxvIFdvcmxk", true)]
        [DataRow("aGVsbG8gd29ybGQ=", true)]
        [DataRow("aGVsG8gd29ybGQ=", false)]
        [DataRow("aGVsb#8gd29ybGQ=", false)]
        public void IsBase64(string testValue, bool expectedValue)
        {            
            Assert.AreEqual(expectedValue, testValue.IsBase64());
        }
    }
}
