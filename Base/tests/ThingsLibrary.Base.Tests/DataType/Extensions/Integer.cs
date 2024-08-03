namespace ThingsLibrary.Tests.DataType.Extensions
{
    [TestClass, ExcludeFromCodeCoverage]
    public class IntegerTests
    {
        [DataTestMethod]
        [DataRow(6091082, "005cf14a-0000-0000-0000-000000000000")]
        [DataRow(511534, "0007ce2e-0000-0000-0000-000000000000")]
        [DataRow(11503, "00002cef-0000-0000-0000-000000000000")]
        [DataRow(7634, "00001dd2-0000-0000-0000-000000000000")]
        [DataRow(6091083, "005cf14b-0000-0000-0000-000000000000")]
        public void ToGuid(int testValue, string expectedValue)
        {            
            Assert.AreEqual(Guid.Parse(expectedValue), testValue.ToGuid());
        }
    }
}
