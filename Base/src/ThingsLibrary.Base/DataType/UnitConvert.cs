// ================================================================================
// <copyright file="UnitConvert.cs" company="Starlight Software Co">
//    Copyright (c) Starlight Software Co. All rights reserved.
//    Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
// </copyright>
// ================================================================================

namespace ThingsLibrary.DataType
{
    public static class UnitConvert
    {           
        /// <summary>
        /// Basic convertions to other units (no nuget Library)
        /// </summary>
        /// <param name="sourceValue">Source Value</param>
        /// <param name="sourceUnits">Source Unit Symbol</param>
        /// <param name="destinationUnits">Destination Unit Symbol</param>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException">When null parameter provided</exception>
        /// <exception cref="ArgumentException">If conversion doesn't exist yet</exception>
        public static double ConvertUnitValue(double sourceValue, string sourceUnits, string destinationUnits)
        {            
            // already converted?
            if (sourceUnits == destinationUnits) { return sourceValue; }

            // parameter checks
            if (destinationUnits != null)
            {
                destinationUnits = destinationUnits.Trim().ToLower();
            }
            ArgumentNullException.ThrowIfNullOrEmpty(destinationUnits, nameof(destinationUnits));

            if (sourceUnits != null)
            {
                sourceUnits = sourceUnits.Trim().ToLower();
            }
            ArgumentNullException.ThrowIfNullOrEmpty(sourceUnits, nameof(sourceUnits));


            switch (sourceUnits)
            {
                // ======================================================================
                // AREA
                // ======================================================================                
                case "sq m":
                case "m2":
                    {
                        switch (destinationUnits)
                        {
                            case "m2":
                            case "sq m":
                                { return sourceValue; }    // unit only change

                            case "ac": { return sourceValue * 0.00024711f; }
                            case "ha": { return sourceValue * 0.0001f; }

                            case "sq ft":
                            case "ft2":
                                { return sourceValue * 10.7639104f; }

                            default: { throw new ArgumentException($"No convertion supported for '{sourceUnits}' to '{destinationUnits}'"); }
                        }
                    }

                case "ha":
                    {
                        switch (destinationUnits)
                        {
                            case "m2":
                            case "sq m":
                                { return sourceValue * 10000.0f; }

                            case "ac": { return sourceValue * 2.47105381f; }                            

                            case "sq ft":
                            case "ft2":
                                { return sourceValue * 107639.104f; }

                            default: { throw new ArgumentException($"No convertion supported for '{sourceUnits}' to '{destinationUnits}'"); }
                        }
                    }

                // ======================================================================
                // DISTANCE
                // ======================================================================
                case "ft":
                    {
                        switch (destinationUnits)
                        {
                            case "cm": { return sourceValue * 30.48f; }
                            case "m": {  return sourceValue * 0.3048f; }
                            case "km": { return sourceValue * 0.0003048f; }

                            case "in": { return sourceValue * 12f; }
                            case "yd": { return sourceValue * 0.33333333f; }
                            case "mi": { return sourceValue * 0.00018939f; }

                            default: { throw new ArgumentException($"No convertion supported for '{sourceUnits}' to '{destinationUnits}'"); }
                        }
                    }

                // ======================================================================
                // WEIGHT
                // ======================================================================
                case "lb":
                case "lbs":
                    {
                        switch (destinationUnits)
                        {
                            case "lb":
                            case "lbs":
                                { return sourceValue; }

                            case "kg": { return sourceValue * 0.45359237f; }

                            default: { throw new ArgumentException($"No convertion supported for '{sourceUnits}' to '{destinationUnits}'"); }
                        }
                    }

                // ======================================================================
                // SPEED
                // ======================================================================
                case "mi/h":
                case "mi/hr":
                case "mph":
                    {
                        switch (destinationUnits)
                        {
                            // unit only change
                            case "mph":
                            case "mi/hr":
                            case "mi/h":
                                { return sourceValue; }

                            case "km1hr-1":
                            case "km/hr":
                            case "km/h":
                                { return sourceValue * 1.609344f; }

                            default: { throw new ArgumentException($"No convertion supported for '{sourceUnits}' to '{destinationUnits}'"); }
                        }
                    }
                case "km1hr-1":
                case "km/hr":
                case "km/h":
                    {
                        switch (destinationUnits)
                        {
                            // unit only change
                            case "km1hr-1":
                            case "km/hr":
                            case "km/h":
                                { return sourceValue; }

                            case "mph":
                            case "mi/hr":
                            case "mi/h":
                                { return sourceValue * 0.62137119f; }

                            default: { throw new ArgumentException($"No convertion supported for '{sourceUnits}' to '{destinationUnits}'"); }
                        }
                    }

                // ======================================================================
                // VOLUME
                // ======================================================================
                case "m3/ha":
                case "[m3]1ha-1":
                    {
                        switch (destinationUnits)
                        {
                            // unit only change                            
                            case "m3/ha":
                            case "[m3]1ha-1":
                                { return sourceValue; }

                            case "bu/ac":        //m3-->bu == 28.3776, ha -> ac == 2.47105381.. so m3/ha -> bu/ac ..  11.484
                                { return sourceValue * 11.484f; }

                            default: { throw new ArgumentException($"No convertion supported for '{sourceUnits}' to '{destinationUnits}'"); }
                        }
                    }

                // ======================================================================
                //MISC
                // ======================================================================
                case "seeds/ha":
                case "seeds1ha-1":
                    {
                        switch (destinationUnits)
                        {
                            // unit only change                            
                            case "seeds/ha":
                            case "seeds1ha-1":
                                { return sourceValue; }

                            case "seeds/ac":        // ha -> ac == 2.47105381
                                { return sourceValue * 2.47105381f; }

                            default: { throw new ArgumentException($"No convertion supported for '{sourceUnits}' to '{destinationUnits}'"); }
                        }
                    }

                case "l1ha-1":
                case "l/ha":
                    {
                        switch (destinationUnits)
                        {
                            // unit only change
                            case "l1ha-1":
                            case "l/ha":
                                { return sourceValue; }

                            case "gal/ac":        // 1 l/ha = 0.106906637 US gal/acre
                                { return sourceValue * 0.106906637f; }

                            default: { throw new ArgumentException($"No convertion supported for '{sourceUnits}' to '{destinationUnits}'"); }
                        }
                    }

                //case "kg1ha-1":
                //case "kg/ha":
                //    { return sourceValue * 0.892179122f; }


                default:
                    { throw new ArgumentException($"No convertion supported for source units '{sourceUnits}' to '{destinationUnits}'"); }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="unixTime"></param>
        /// <returns></returns>
        public static DateTime FromUnixTime(long unixTime)
        {
            var epoch = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);
            return epoch.AddSeconds(unixTime);
        }

        /// <summary>
        /// Converts date time to Unit Time
        /// </summary>
        /// <param name="date"></param>
        /// <returns>Seconds since 1/1/1970 - Epoch / Unix time</returns>
        public static long ToUnixTime(DateTime date)
        {
            var epoch = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);

            var seconds = (date - epoch).TotalSeconds;
            if (seconds < 0) { throw new ArgumentException($"Date is less then epoch (1970-01-01)"); }

            return Convert.ToInt64(seconds);
        }
        

        /// <summary>
        /// Similar to seconds since 1970, but with days since 01-01-1970
        /// </summary>
        /// <param name="unixDay"></param>
        /// <returns></returns>
        public static DateTime FromUnixDay(short unixDay)
        {
            var epoch = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);
            return epoch.AddDays(unixDay);
        }

        /// <summary>
        /// Converts date time to Unit Time
        /// </summary>
        /// <param name="dateTime">Date Time</param>
        /// <returns>Seconds since 1/1/1970 - Epoch / Unix time</returns>
        public static short ToUnixDay(DateTime dateTime)
        {
            var epoch = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);
            
            var days = (dateTime.Date - epoch.Date).TotalDays;
            if (days > short.MaxValue) { throw new ArgumentException($"Day range '{days}' is more then short MaxValue ({short.MaxValue})"); }
            if (days < 0) { throw new ArgumentException($"Day range '{days}' is less then epoch (1970-01-01)"); }

            return Convert.ToInt16(days);
        }

        /// <summary>
        /// Converts date time to Unit Time
        /// </summary>
        /// <param name="date"></param>
        /// <returns>Seconds since 1/1/1970 - Epoch / Unix time</returns>
        public static short ToUnixDay(DateOnly date)
        {
            return ToUnixDay(date.ToDateTime(new TimeOnly()));            
        }


        /// <summary>
        /// Convert a hex string to byte array
        /// </summary>
        /// <param name="hexString">Hex String</param>
        /// <returns>Byte Array</returns>
        /// <exception cref="ArgumentException"></exception>
        public static byte[] ConvertHexStringToByteArray(string hexString)
        {
            if (hexString.Length % 2 != 0)
            {
                throw new ArgumentException($"The hex string cannot have an odd number of digits");
            }

            return Enumerable.Range(0, hexString.Length)
                             .Where(x => x % 2 == 0)
                             .Select(x => Convert.ToByte(hexString.Substring(x, 2), 16))
                             .ToArray();
        }
    }
}