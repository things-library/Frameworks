namespace ThingsLibrary.Tests.DataType.Extensions
{
    [TestClass, ExcludeFromCodeCoverage]
    public class GuidTests
    {
        [DataTestMethod]
        [DataRow("005cf14a-0000-0000-0000-000000000000", 6091082)]
        [DataRow("0007ce2e-0000-0000-0000-000000000000", 511534)]
        [DataRow("00002cef-0000-0000-0000-000000000000", 11503)]
        [DataRow("00001dd2-0000-0000-0000-000000000000", 7634)]
        [DataRow("005cf14b-0000-0000-0000-000000000000", 6091083)]
        public void ToInt(string testValue, int expectedValue)
        {            
            Assert.AreEqual(expectedValue, Guid.Parse(testValue).ToInt());
        }
    }
}
