namespace ThingsLibrary.Tests.IO
{
    [TestClass, ExcludeFromCodeCoverage]
    public class ZipTests
    {
        [TestMethod]
        public void CompressThenUncompress()
        {
            var orgText = "Hello World, how are you?";

            var compressed = ThingsLibrary.IO.Compression.ZipString.Compress(orgText);

            var testText = ThingsLibrary.IO.Compression.ZipString.Uncompress(compressed);

            Assert.AreEqual(orgText, testText);                    
        }
    }
}
