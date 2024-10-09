// ================================================================================
// <copyright file="DateTime.cs" company="Starlight Software Co">
//    Copyright (c) Starlight Software Co. All rights reserved.
//    Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
// </copyright>
// ================================================================================

namespace ThingsLibrary.DataType.Extensions
{
    public static class DateTimeExtensions
    {
        /// <summary>
        /// To Epoch Seconds * 100 (milliseconds)
        /// </summary>
        /// <param name="dateTime"></param>
        /// <returns></returns>
        public static long ToEpoch100(DateTime dateTime) => (long)(dateTime - new DateTime(1970, 1, 1)).TotalSeconds * 100;

        /// <summary>
        /// Nullable version of the ToHHMMSS format
        /// </summary>
        /// <param name="dateTime"></param>
        /// <param name="isUtc"></param>
        /// <returns></returns>
        public static string ToHHMMSS(this DateTime? dateTime, bool isUtc = true)
        {
            if (dateTime == null) { return "--"; }
            
            return ToHHMMSS(dateTime.Value, isUtc);
        }

        /// <summary>
        /// Convert the date time to a TimeSpan from NOW
        /// </summary>
        /// <param name="dateTime">Date Time</param>
        /// <param name="isUtc">Use UTC or local time zone</param>
        /// <returns></returns>
        public static string ToHHMMSS(this DateTime dateTime, bool isUtc = true)
        {
            // DD:HH:MM:SS
            // DD:HH:MM:SS.sss
            // HH:MM:SS
            // HH:MM:SS.sss

            var timeSpan = (isUtc ? DateTime.UtcNow  : DateTime.Now) - dateTime;

            return TimeSpanExtensions.ToHHMMSS(timeSpan);
        }

        /// <summary>
        /// Converts the DateTime to yyyyMMdd format
        /// </summary>
        /// <param name="dateTime"></param>
        /// <returns></returns>
        public static string ToYMD(this DateTime dateTime)
        {
            return dateTime.ToString("yyyyMMdd");
        }

        /// <summary>
        /// Converts the DateTime to yyyyMMdd format
        /// </summary>
        /// <param name="dateTime"></param>
        /// <returns></returns>
        public static string ToYMD(this DateTime? dateTime)
        {
            return dateTime.HasValue ? dateTime.Value.ToYMD() : string.Empty;
        }

        /// <summary>
        /// Converts the DateTime to yyyyMMdd-HHmmss format
        /// </summary>
        /// <param name="dateTime">Source DateTime</param>
        /// <returns></returns>
        public static string ToYMDHMS(this DateTime dateTime)
        {
            return dateTime.ToString("yyyyMMdd-HHmmss");
        }

        /// <summary>
        /// Converts the DateTime to yyyyMMdd-HHmmss format
        /// </summary>
        /// <param name="dateTime">Source DateTime</param>
        /// <returns></returns>
        public static string ToYMDHMS(this DateTime? dateTime)
        {
            return dateTime.HasValue ? dateTime.Value.ToYMDHMS() : string.Empty;
        }
    }
}
