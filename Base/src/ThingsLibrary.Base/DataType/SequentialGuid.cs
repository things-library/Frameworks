// ================================================================================
// <copyright file="SequentialGuid.cs" company="Starlight Software Co">
//    Copyright (c) Starlight Software Co. All rights reserved.
//    Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
// </copyright>
// ================================================================================

using System.Runtime.CompilerServices;

namespace ThingsLibrary.DataType
{
    /// <summary>
    /// Generates globally unique identifiers that are sequential in nature and can be created both in code and from SQL Server.
    /// The NewGuid() method can be used by SQL Server via CLR Integration.
    /// </summary>
    /// <remarks>
    /// The last six bytes of each Guid created contains bits from the current system time.
    /// Because sorting is done on this group of bytes first, this provides an always-increasing collection of guid values.
    /// </remarks>
    public static class SequentialGuid
    {
        [ThreadStatic]
        [CompilerGenerated] // NOTE: this attribute allows SQL Server to reference static values in SAFE permission level.
        private static long s_ThreadStaticLastTicks = 0;

        /// private const long January_1_2009 = 633663648000000000;
        private const long January_1_2020 = 637134336000000000;

        /// <summary>
        /// Returns a new sequential Guid based on the current UTC system time.
        /// This method can be used by SQL Server via CLR Integration.
        /// </summary>
        /// <returns>A new <see cref="Guid"/> object.</returns>
        public static Guid NewGuid()
        {
            // Source Article: http://www.informit.com/articles/article.asp?p=25862&seqNum=7&rl=1
            // UUID RFP Standards: ftp://ftp.rfc-editor.org/in-notes/rfc4122.txt

            // Notes:
            //
            // - No bit shift in the ticks value means the lower six-bytes rollover every 325 days (0.892 years).
            //
            // - Shifting ticks 6 bits means the value rolls over to zero every 57 years (next natural rollover is in year 2055).
            //
            // - We are offsetting from Jan 1, 2009 to avoid rolling over for the full 57 years (rather than just 46 years).
            //
            // - The resolution of DateTime.UtcNow is anywhere from 1ms and 15ms. An old P3 box tested at 15.633 ms,
            //   but my current Core2Duo box (as of 2009) tested at 1.000 ms.
            //   This means, at a minimum, the lower 13 bits (see chart below) are not significant and don't change
            //   frequently enough to justify using them at the expense of higher-order bits.
            //
            // - In the interest of performance and simplicity, it was decided not to try to make this
            //   method perfectly thread-safe.  It's currently possible fot two threads could get two guids
            //   with the same six-byte sequential value.  In which case the guids issued will not be in
            //   perfect sequential order.  A single thread will always get perfectly sequential values since it
            //   uses it's own (thread static) variable to enforce uniqueness.

            // The table below summarizes the trade-offs associated with shifting the ticks value a given nubmer of
            // bits.  The Resolution column is the the minimum resolution of the upper bits (the about of time represented
            // for each increment of the bits).  The Rollover column is the time it takes for the upper 48 bits to rollover
            // back to zero.
            //
            // Bit Shift  Resolution(in ms)  Rollover(in years)
            //     0          0.0001             0.89
            //     1          0.0002             1.78
            //     2          0.0004             3.57
            //     3          0.0008             7.14
            //     4          0.0016            14.27
            //     5          0.0032            28.54
            //     6          0.0064            57.08
            //     7          0.0128           114.17
            //     8          0.0256           228.34
            //     9          0.0512           456.67
            //    10          0.1024           913.34
            //    11          0.2048          1826.68
            //    12          0.4096          3653.36
            //    13          0.8192          7306.73
            //    14          1.6384         14613.46
            //    15          3.2768         29226.91
            //    16          6.5536         58453.82


            // get the current ticks (shifted) and make sure it is greater than the last
            var ticks = (DateTime.UtcNow.Ticks - January_1_2020) >> 6;
            if (ticks <= s_ThreadStaticLastTicks)
            {
                ticks = s_ThreadStaticLastTicks + 1;
            }
            s_ThreadStaticLastTicks = ticks;

            // get byte array for a new Guid and the shifted ticks value
            var guidBytes = Guid.NewGuid().ToByteArray();

            // copy last six bytes from shifted ticks value to last six in guid
            guidBytes[10] = (byte)(ticks >> 40);
            guidBytes[11] = (byte)(ticks >> 32);
            guidBytes[12] = (byte)(ticks >> 24);
            guidBytes[13] = (byte)(ticks >> 16);
            guidBytes[14] = (byte)(ticks >> 8);
            guidBytes[15] = (byte)ticks;

            // return the sequential guid
            return new Guid(guidBytes);
        }
    }
}