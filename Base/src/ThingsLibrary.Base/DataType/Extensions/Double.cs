// ================================================================================
// <copyright file="Double.cs" company="Starlight Software Co">
//    Copyright (c) Starlight Software Co. All rights reserved.
//    Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
// </copyright>
// ================================================================================

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
