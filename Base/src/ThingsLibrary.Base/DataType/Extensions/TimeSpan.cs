// ================================================================================
// <copyright file="TimeSpan.cs" company="Starlight Software Co">
//    Copyright (c) Starlight Software Co. All rights reserved.
//    Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
// </copyright>
// ================================================================================

namespace ThingsLibrary.DataType.Extensions
{
    public static class TimeSpanExtensions
    {
        /// <summary>
        /// Outputs a ':' delimited version like a timer/clock
        /// </summary>
        /// <example>3:12:16:07 or 02:06:16 or 04:17</example>
        /// <param name="timeSpan"><see cref="TimeSpan"/></param>
        /// <returns>Variable width clock notation(##:##:##)</returns>
        public static string ToClock(this TimeSpan timeSpan)
        {
            // Outputs:
            //      mm:ss
            //   hh:mm:ss
            // d:hh:mm:ss  
     
            if (timeSpan.TotalHours < 1d)
            {
                return $"{timeSpan:mm\\:ss}";
            }
            else if (timeSpan.TotalDays < 1d)
            {
                return $"{timeSpan:hh\\:mm\\:ss}";

            }
            else
            {
                return $"{timeSpan:d\\:hh\\:mm\\:ss}";
            }
        }

        /// <summary>
        /// Outputs hh:mm:ss if under 1 day, d:hh:mm:ss if more than a day duration
        /// </summary>
        /// <param name="timeSpan"></param>
        /// <returns></returns>
        public static string ToHHMMSS(this TimeSpan timeSpan)
        {
            // Outputs:
            //   hh:mm:ss
            // d:hh:mm:ss            

            if (timeSpan.Days < 1d)
            {
                return $"{timeSpan:hh\\:mm\\:ss}";
                
            }
            else
            {
                return $"{timeSpan:d\\:hh\\:mm\\:ss}";
            }
        }

        /// <summary>
        /// Outputs a static width clock notation 'd:hh:mm:ss'
        /// </summary>
        /// <example>01:12:16:07 or 02:06:16 or 04:17</example>
        /// <param name="timeSpan"><see cref="TimeSpan"/></param>
        /// <returns>Static width clock notation (d:hh:mm:ss)</returns>
        public static string ToDDHHMMSS(this TimeSpan timeSpan)
        {
            // Outputs: d:hh:mm:ss            

            return $"{timeSpan:d\\:hh\\:mm\\:ss}";            
        }
    }
}
