// ================================================================================
// <copyright file="DateTime.cs" company="Starlight Software Co">
//    Copyright (c) 2025 Starlight Software Co. All rights reserved.
//    Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
// </copyright>
// ================================================================================

namespace ThingsLibrary.Tests.DataType.Extensions
{
    [TestClass, ExcludeFromCodeCoverage]
    public class DateTimeTests
    {
        [DataTestMethod]
        [DataRow(0, 0, 0, 0, false, "00:00:00")]
        [DataRow(1, 2, 3, 4, false, "1:02:03:04")]
        [DataRow(0, 2, 3, 4, false, "02:03:04")]
        [DataRow(0, 0, 0, 0, true, "00:00:00")]
        [DataRow(1, 2, 3, 4, true, "1:02:03:04")]
        [DataRow(0, 2, 3, 4, true, "02:03:04")]
        public void ToHHMMSS(int days, int hours, int minutes, int seconds, bool isUtc, string expected)
        {
            var timeSpan = new TimeSpan(days, hours, minutes, seconds);

            DateTime testValue = (isUtc ? DateTime.UtcNow : DateTime.Now).Subtract(timeSpan);

            Assert.AreEqual(expected, testValue.ToHHMMSS(isUtc));
        }

        [DataTestMethod]
        [DataRow(0, 0, 0, 0, false, "--")]
        [DataRow(1, 2, 3, 4, false, "1:02:03:04")]
        [DataRow(0, 2, 3, 4, false, "02:03:04")]
        [DataRow(1, 2, 3, 4, true, "1:02:03:04")]
        [DataRow(0, 2, 3, 4, true, "02:03:04")]
        public void ToHHMMSS_Nullable(int days, int hours, int minutes, int seconds, bool isUtc, string expected)
        {
            var timeSpan = new TimeSpan(days, hours, minutes, seconds);

            DateTime? testValue = (isUtc ? DateTime.UtcNow : DateTime.Now).Subtract(timeSpan);
            if(days == 0 && hours == 0 && minutes == 0 && seconds == 0)
            {
                testValue = null;
            }

            Assert.AreEqual(expected, testValue.ToHHMMSS(isUtc));
        }
    }
}