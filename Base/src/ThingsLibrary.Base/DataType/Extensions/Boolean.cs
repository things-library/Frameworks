// ================================================================================
// <copyright file="Boolean.cs" company="Starlight Software Co">
//    Copyright (c) Starlight Software Co. All rights reserved.
//    Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
// </copyright>
// ================================================================================

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
