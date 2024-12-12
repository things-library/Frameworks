// ================================================================================
// <copyright file="Checksum.cs" company="Starlight Software Co">
//    Copyright (c) Starlight Software Co. All rights reserved.
//    Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
// </copyright>
// ================================================================================

namespace ThingsLibrary.DataType.Extensions
{
    public static class ChecksumExtensions
    {
        /// <summary>
        /// Append the checksum
        /// </summary>
        /// <param name="sentence"></param>
        /// <remarks>Checksum based on the NMEA0183 checksum which is just a simple byte-by-byte XOR of all the bytes between $ and * signs exclusively.</remarks>
        public static void AppendChecksum(this StringBuilder sentence)
        {
            // not the beginning of a sentence
            if (sentence.Length == 0) { return; }
            if (sentence[0] != '$') { return; }

            //Start with first Item
            int checksum = Convert.ToByte(sentence[1]);

            // Loop through all chars to get a checksum
            int i;
            for (i = 2; i < sentence.Length; i++)
            {
                if (sentence[i] == '*') { break; }

                // No. XOR the checksum with this character's value
                checksum ^= Convert.ToByte(sentence[i]);
            }

            // no astrisk to mark the CRC check
            if (i == sentence.Length)
            {
                sentence.Append("*");
            }

            // Return the checksum formatted as a two-character hexadecimal
            sentence.Append(checksum.ToString("X2"));
        }

        /// <summary>
        /// Calculate a checksum value based on the NMEA0183 style calculation
        /// </summary>
        /// <param name="sentence">Sentence</param>
        /// <remarks>Checksum based on the NMEA0183 checksum which is just a simple byte-by-byte XOR of all the bytes between $ and * signs exclusively.</remarks>
        /// <returns>Two character hexadecimal checksum value</returns>
        public static string ToChecksum(this string sentence)
        {
            if (string.IsNullOrEmpty(sentence)) { return string.Empty; }

            // not the beginning of a sentence
            if (sentence[0] != '$') { return string.Empty; }

            //Start with first Item
            int checksum = Convert.ToByte(sentence[1]);

            // Loop through all chars to get a checksum
            for (int i = 2; i < sentence.Length; i++)
            {
                if (sentence[i] == '*') { break; }

                // No. XOR the checksum with this character's value
                checksum ^= Convert.ToByte(sentence[i]);
            }

            // Return the checksum formatted as a two-character hexadecimal
            return checksum.ToString("X2");
        }

        /// <summary>
        /// Validate that the checksum on the sentence is correct
        /// </summary>
        /// <param name="sentence"></param>
        /// <returns></returns>
        public static bool ValidateChecksum(this string sentence)
        {
            var checksum = sentence.ToChecksum();
            if (string.IsNullOrEmpty(checksum)) { return false; }

            return sentence.EndsWith(checksum);
        }        
    }
}
