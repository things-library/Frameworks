namespace ThingsLibrary.DataType.Extensions
{
    public static class GuidExtensions
    {
        /// <summary>
        /// Convert value into a integer usng bit converter
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static int ToInt(this Guid value)
        {
            byte[] b = value.ToByteArray();
            int bint = BitConverter.ToInt32(b, 0);

            return bint;
        }
    }
}
