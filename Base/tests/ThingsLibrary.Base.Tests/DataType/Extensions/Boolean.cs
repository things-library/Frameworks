// ================================================================================
// <copyright file="Boolean.cs" company="Starlight Software Co">
//    Copyright (c) 2025 Starlight Software Co. All rights reserved.
//    Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
// </copyright>
// ================================================================================

namespace ThingsLibrary.Tests.DataType.Extensions
{
    [TestClass, ExcludeFromCodeCoverage]
    public class BooleanTests
    {
        [TestMethod]
        [DataRow(true, "Yes")]
        [DataRow(false, "No")]
        public void ToYesNoTests(bool testValue, string expectedValue)
        {
            Assert.AreEqual(expectedValue, testValue.ToYesNo());
        }

        [TestMethod]
        [DataRow(null, "No")]
        [DataRow(true, "Yes")]
        [DataRow(false, "No")]
        public void ToYesNoNullTests(bool? testValue, string expectedValue)
        {
            Assert.AreEqual(expectedValue, testValue.ToYesNo());
        }
    }
}