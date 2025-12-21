// ================================================================================
// <copyright file="Double.cs" company="Starlight Software Co">
//    Copyright (c) 2025 Starlight Software Co. All rights reserved.
//    Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
// </copyright>
// ================================================================================

namespace ThingsLibrary.Tests.DataType.Extensions
{
    [TestClass, ExcludeFromCodeCoverage]
    public class DoubleTests
    {
        [TestMethod]
        [DataRow(1.0d, 1)]
        [DataRow(1.2d, 1)]
        [DataRow(1.4d, 1)]
        [DataRow(1.5d, 2)]
        [DataRow(1.7d, 2)]
        public void RoundToInt(double testValue, int expectedValue)
        {            
            Assert.AreEqual(expectedValue, testValue.RoundToInt());
        }

        [TestMethod]
        [DataRow(1.0d, 1, 1.0d)]
        [DataRow(1.23456d, 1, 1.2d)]
        [DataRow(1.23456d, 2, 1.23d)]
        [DataRow(1.23556d, 3, 1.236d)]
        [DataRow(123.456d, 1, 123.5d)]
        [DataRow(123.456d, 2, 123.46d)]
        public void Round(double testValue, int decimals, double expectedValue)
        {
            Assert.AreEqual(expectedValue, testValue.Round(decimals));
        }

        [TestMethod]
        [DataRow(1d, "1")]
        [DataRow(1.0d, "1")]
        [DataRow(1.00000d, "1")]
        [DataRow(1.10000000d, "1.1")]
        [DataRow(1.23456d, "1.23456")]
        public void ToStandardNotationString(double testValue, string expectedValue)
        {
            Assert.AreEqual(expectedValue, testValue.ToStandardNotationString());
        }
    }
}