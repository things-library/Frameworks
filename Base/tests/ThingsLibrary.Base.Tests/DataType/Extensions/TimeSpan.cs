namespace ThingsLibrary.Tests.DataType.Extensions
{
    [TestClass, ExcludeFromCodeCoverage]
    public class TimeSpanTests
    {
        [DataTestMethod]
        [DataRow(0, 0, 0, 0, "00:00:00")]
        [DataRow(1, 2, 3, 4, "1:02:03:04")]
        [DataRow(0, 2, 3, 4, "02:03:04")]
        public void ToHHMMSS(int days, int hours, int minutes, int seconds, string expected)
        {
            Assert.AreEqual(expected, new TimeSpan(days, hours, minutes, seconds).ToHHMMSS());            
        } 
    }
}