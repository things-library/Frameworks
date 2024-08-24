using ThingsLibrary.DataType;

namespace ThingsLibrary.Tests.DataType
{
    [TestClass, ExcludeFromCodeCoverage]
    public class TelemetryTests
    {
        [TestMethod]        
        public void Basic()
        {
            var telem = new TelemetryEntry("sens", DateTime.UtcNow);

            telem.Attributes.Add("gn", "Mark");
            telem.Attributes.Add("cp", "Starlight");
            telem.Attributes.Add("r", "1");

            var sentence = telem.ToString();

            var expectedPrefix = $"${telem.Timestamp.ToUnixTimeMilliseconds()}|{telem.Type}|";

            Assert.IsTrue(sentence.StartsWith(expectedPrefix));
            Assert.IsTrue(sentence.Contains('*'));
        }

    }
}
