namespace ThingsLibrary.Tests.DataType.Extensions
{
    [TestClass, ExcludeFromCodeCoverage]
    public class BooleanTests
    {
        [DataTestMethod]
        [DataRow(true, "Yes")]
        [DataRow(false, "No")]
        public void ToYesNoTests(bool testValue, string expectedValue)
        {
            Assert.AreEqual(expectedValue, testValue.ToYesNo());
        }

        [DataTestMethod]
        [DataRow(null, "No")]
        [DataRow(true, "Yes")]
        [DataRow(false, "No")]
        public void ToYesNoNullTests(bool? testValue, string expectedValue)
        {
            Assert.AreEqual(expectedValue, testValue.ToYesNo());
        }
    }
}