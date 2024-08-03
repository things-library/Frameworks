namespace ThingsLibrary.DataType.Extensions
{
    public static class BooleanExtensions
    {
        /// <summary>
        /// Converts boolean variable to 'Yes', 'No'
        /// </summary>
        /// <param name="boolean">Boolean</param>
        /// <returns>'Yes' or 'No' string response</returns>
        public static string ToYesNo(this bool boolean)
        {
            return (boolean ? "Yes" : "No");
        }

        /// <summary>
        /// Converts nullable boolean variable to 'Yes', 'No'
        /// </summary>
        /// <param name="boolean">Boolean</param>
        /// <returns>'Yes' or 'No' string response</returns>
        public static string ToYesNo(this bool? boolean)
        {
            return (boolean.HasValue && boolean.Value ? "Yes" : "No");
        }
    }
}
