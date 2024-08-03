namespace ThingsLibrary.DataType.Extensions
{
    public static class DoubleExtensions
    {
        /// <summary>
        /// Round to int
        /// </summary>
        /// <param name="dbl"></param>
        /// <returns></returns>
        public static int RoundToInt(this double value)
        {
            return (int)System.Math.Round(value, 0);
        }

        /// <summary>
        /// Round decimal to fixed decimals
        /// </summary>
        /// <param name="dbl"></param>
        /// <param name="decimals"></param>
        /// <returns></returns>
        public static double Round(this double value, int decimals = 0)
        {
            return System.Math.Round(value, decimals);
        }

        public static String ToStandardNotationString(this double d)
        {
            //Keeps precision of double up to is maximum
            return d.ToString(".#####################################################################################################################################################################################################################################################################################################################################");
        }
    }
}
