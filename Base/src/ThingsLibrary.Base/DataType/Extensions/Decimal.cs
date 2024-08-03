namespace ThingsLibrary.DataType.Extensions
{
    public static class DecimalExtensions
    {
        /// <summary>
        /// Round to the nearest integer
        /// </summary>
        /// <param name="value">Value to rount</param>
        /// <returns>Value rounded to the nearest integer</returns>
        public static int RoundToInt(this decimal value)
        {
            return (int)System.Math.Round(value, 0);
        }

        /// <summary>
        /// Round to the nearest decimal place
        /// </summary>
        /// <param name="value">Value to rount</param>
        /// <param name="decimals">Decimal places to round to</param>
        /// <returns></returns>
        public static decimal Round(this decimal value, int decimals = 0)
        {
            return System.Math.Round(value, decimals);
        }
    }
}
