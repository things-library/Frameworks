namespace ThingsLibrary.Base.DataType.Extensions
{
    public static class ChecksumExtensions
    {
        /// <summary>
        /// Append the checksum
        /// </summary>
        /// <param name="sentence"></param>
        public static void AddChecksum(this StringBuilder sentence)
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

            // this isn't a sentence to sign
            if (sentence[i] != '*') { return; }

            // Return the checksum formatted as a two-character hexadecimal
            sentence.Append(checksum.ToString("X2"));
        }

        /// <summary>
        /// Calculate a checksum value based on the NMEA style calculation
        /// </summary>
        /// <param name="sentence">Sentence</param>
        /// <remarks>Sentences must have a $ start character and * end character</remarks>
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

    public static class Crc16
    {
        const ushort polynomial = 0xA001;
        static readonly ushort[] table = new ushort[256];

        public static ushort ComputeChecksum(byte[] bytes)
        {
            ushort crc = 0;
            for (int i = 0; i < bytes.Length; ++i)
            {
                byte index = (byte)(crc ^ bytes[i]);
                crc = (ushort)((crc >> 8) ^ table[index]);
            }
            return crc;
        }

        static Crc16()
        {
            ushort value;
            ushort temp;
            for (ushort i = 0; i < table.Length; ++i)
            {
                value = 0;
                temp = i;
                for (byte j = 0; j < 8; ++j)
                {
                    if (((value ^ temp) & 0x0001) != 0)
                    {
                        value = (ushort)((value >> 1) ^ polynomial);
                    }
                    else
                    {
                        value >>= 1;
                    }
                    temp >>= 1;
                }
                table[i] = value;
            }
        }
    }
}
