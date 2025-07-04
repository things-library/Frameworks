// ================================================================================
// <copyright file="Long.cs" company="Starlight Software Co">
//    Copyright (c) Starlight Software Co. All rights reserved.
//    Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
// </copyright>
// ================================================================================

namespace ThingsLibrary.DataType.Extensions
{
    public static class LongExtensions
    {        
        /// <summary>
        /// Generate a short version of the long number based on the provided key space
        /// </summary>
        /// <param name="number">Base Number</param>
        /// <param name="keyspace">Characters to use in encoding</param>
        /// <returns></returns>
        public static string ToShortCode(this long number, string keyspace = "abcdefghijklmnopqrstuvwxyz0123456789")
        {
            if (number == 0) { return string.Empty; }

            var keyspaceLength = keyspace.Length;            
            var numberToEncode = number;
            StringBuilder result = new StringBuilder();

            var i = 0;
            do
            {
                i++;
                var characterValue = (numberToEncode % keyspaceLength == 0 ? keyspaceLength : numberToEncode % keyspaceLength);
                var indexer = (int)characterValue - 1;
                
                // add to beginning
                result.Insert(0, keyspace[indexer]);
                //result = keyspace[indexer] + shortcodeResult;

                numberToEncode = ((numberToEncode - characterValue) / keyspaceLength);
            }
            while (numberToEncode != 0);

            return result.ToString();
        }

        /// <summary>
        /// Convert a short code back to a long data type
        /// </summary>
        /// <param name="shortcode">Short Code value</param>
        /// <param name="keyspace">Characters to use in encoding</param>
        /// <returns></returns>
        public static long FromShortCode(string shortcode, string keyspace = "abcdefghijklmnopqrstuvwxyz0123456789")
        {
            long result = 0;

            var keyspaceLength = keyspace.Length;            
            var shortcodeLength = shortcode.Length;
            var codeToDecode = shortcode;

            foreach (var character in codeToDecode)
            {
                shortcodeLength--;
                var codeChar = character;
                var codeCharIndex = keyspace.IndexOf(codeChar);
                if (codeCharIndex < 0)
                {
                    // The character is not part of the keyspace and so entire shortcode is invalid.
                    return 0;
                }
                try
                {
                    checked
                    {
                        result += (codeCharIndex + 1) * (long)(System.Math.Pow(keyspaceLength, shortcodeLength));
                    }
                }
                catch (OverflowException)
                {
                    // We've overflowed the maximum size for a long (possibly the shortcode is invalid or too long).
                    return 0;
                }
            }
            return result;
        }
    }
}
