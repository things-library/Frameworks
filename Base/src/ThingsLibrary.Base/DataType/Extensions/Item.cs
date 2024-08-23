using System.Text;
using ThingsLibrary.Schema.Library;

namespace ThingsLibrary.Base.Tests.DataType.Extensions
{
    public static class ItemExtensions
    {
        public static string ToSentence(this ItemDto item)
        {
            var sentence = new StringBuilder();
            sentence.Append("$");



            sentence.Append("*");

            //Add checksum
            var checksum = "";

            return sentence.ToString();
        }
    }
}
