namespace ThingsLibrary.DataType.Extensions
{
    public static class IntegerExtensions
    {
        /// <summary>
        /// Convert integer into a guid value as best as we can.
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static Guid ToGuid(this int value)
        {
            byte[] bytes = new byte[16];
            BitConverter.GetBytes(value).CopyTo(bytes, 0);
            return new Guid(bytes);
        }
    }
}
