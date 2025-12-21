// ================================================================================
// <copyright file="Decimal.cs" company="Starlight Software Co">
//    Copyright (c) 2025 Starlight Software Co. All rights reserved.
//    Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
// </copyright>
// ================================================================================

namespace ThingsLibrary.Tests.DataType.Extensions
{
    [TestClass, ExcludeFromCodeCoverage]
    public class DecimalTests
    {
        [TestMethod]
        [DataRow(1.0f, 1)]
        [DataRow(1.2f, 1)]
        [DataRow(1.4f, 1)]
        [DataRow(1.5f, 2)]
        [DataRow(1.7f, 2)]
        public void RoundToInt(float testValue, int expectedValue)
        {
            var testValueDec = (decimal)testValue; //decimal is not a primitive type and can't be used.. so we cast it to decimal from our float test data
            Assert.AreEqual(expectedValue, testValueDec.RoundToInt());
        }

        [TestMethod]
        [DataRow(1.0f, 1, 1.0f)]
        [DataRow(1.23456f, 1, 1.2f)]
        [DataRow(1.23456f, 2, 1.23f)]
        [DataRow(1.23556f, 3, 1.236f)]
        [DataRow(123.456f, 1, 123.5f)]
        [DataRow(123.456f, 2, 123.46f)]
        public void Round(float testValue, int decimals, float expectedValue)
        {
            var testValueDec = (decimal)testValue; //decimal is not a primitive type and can't be used.. so we cast it to decimal from our float test data
            var expectedDec = (decimal)expectedValue;

            Assert.AreEqual(expectedDec, testValueDec.Round(decimals));
        }
    }
}