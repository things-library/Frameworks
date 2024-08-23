using ThingsLibrary.Base.DataType.Extensions;

namespace ThingsLibrary.Tests.DataType.Extensions
{
    [TestClass, ExcludeFromCodeCoverage]
    public class ChecksumTests
    {
        [DataTestMethod]
        [DataRow("$GPGGA,181908.00,3404.7041778,N,07044.3966270,W,4,13,1.00,495.144,M,29.200,M,0.10,0000,*73")]
        [DataRow("$GPGSV,2,1,08,02,74,042,45,04,18,190,36,07,67,279,42,12,29,323,36*77")]
        [DataRow("$GPGSV,2,2,08,15,30,050,47,19,09,158,,26,12,281,40,27,38,173,41*7B")]
        public void SentenceChecksumTests(string testValue)
        {
            // calculate and compare
            Assert.IsTrue(testValue.ValidateChecksum());
        }
        
        [DataTestMethod]
        [DataRow("$GPGSV,2,2,08,15,30,050,47,19,09,158,,26,12,281,40,27,38,173,41*")]
        [DataRow("$GPGSV,2,2,08,15,30,050,47,19,09,158,,26,12,281,40,27,38,173,41")]
        [DataRow("GPGSV,2,2,08,15,30,050,47,19,09,158,,26,12,281,40,27,38,173,41*7B")]
        [DataRow("GPGSV,2,2,08,15,30,050,47,19,09,158,,26,12,281,40,27,38,173,41")]
        [DataRow("08,15,30,050$GPGSV,2,2,08,15,30,050,47,19,09,158,,26,12,281,40,27,38,173,41*7B")]
        public void BadSentenceChecksumTests(string testValue)
        {
            // calculate and compare
            Assert.IsFalse(testValue.ValidateChecksum());
        }
    }
}
