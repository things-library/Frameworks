using System.Globalization;
using System.Text.RegularExpressions;

namespace ThingsLibrary.DataType.Extensions
{
    public static class StringExtensions
    {
        public static TextInfo TextInfo { get; } = new CultureInfo("en-US", false).TextInfo;

        /// <summary>
        /// Convert text to spongebobText
        /// </summary>
        /// <param name="value">Value to convert</param>
        /// <returns>Spongebob text version</returns>
        public static string ToSpongebobText(this string value)
        {
            var newValue = new StringBuilder(value.Length);

            for(int i = 0; i < value.Length; i++)
            {
                newValue.Append((i % 2 == 0 ? Char.ToLower(value[i]) : Char.ToUpper(value[i])));
            }

            return newValue.ToString();
        }

        public static string ToTitleCase(this string value)
        {   
            return StringExtensions.TextInfo.ToTitleCase(value);
        }


        /// <summary>
        /// Tests if the string is base64
        /// </summary>
        /// <param name="data">Data to test</param>
        /// <returns>True if data is base64</returns>
        public static bool IsBase64(this string data)
        {
            return (data.Length % 4 == 0) && Regex.IsMatch(data, @"^[a-zA-Z0-9\+/]*={0,3}$", RegexOptions.None);
        }


        /// <summary>
        /// Appends a item to a delimited list, if already exists it it won't be duplicated
        /// </summary>
        /// <param name="delimitedString">Delimited string</param>
        /// <param name="item">Item to append</param>
        /// <param name="delimiter">Delimiter (default = ';')</param>
        /// <returns></returns>
        public static string AppendDelimitedItem(this string delimitedString, string item, char delimiter = ';')
        {
            if (string.IsNullOrEmpty(delimitedString)) { return item; }

            var list = delimitedString.Split(delimiter).ToList();

            // already in the listing?  then just return the original id
            if (list.Contains(item)) { return delimitedString; }

            list.Add(item);

            return string.Join(delimiter, list);
        }

        /// <summary>
        /// Convert a string representation to a enum
        /// </summary>
        /// <typeparam name="T">Enum type</typeparam>
        /// <param name="value">Value to parse</param>
        /// <returns>Enum representation of the string</returns>
        public static T ToEnum<T>(this string value)// where T : System.Enum
        {
            return (T)Enum.Parse(typeof(T), value, true);
        }
    }
}
