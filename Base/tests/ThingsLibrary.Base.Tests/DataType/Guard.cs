// ================================================================================
// <copyright file="Guard.cs" company="Starlight Software Co">
//    Copyright (c) 2025 Starlight Software Co. All rights reserved.
//    Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
// </copyright>
// ================================================================================

using ThingsLibrary.DataType;

namespace ThingsLibrary.Tests.DataType
{
    [TestClass, ExcludeFromCodeCoverage]
    public class GuardTests
    {
        [TestMethod]
        public void GuardTest()
        {
            string testString = "something";

            _ = Guard.Argument(testString, nameof(testString))
                .NotNull()
                .NotEmpty()
                .NotUppercase();

            testString = null;
            _ = Guard.Argument(testString, nameof(testString))
                .Null();

            testString = "";
            _ = Guard.Argument(testString, nameof(testString))
                .Empty();

            _ = Guard.Argument(testString, nameof(testString))
                .ThrowIf(argument => argument.Value == null, argument => new ArgumentException("Test Argument is Null", argument.Name));
        }

        [TestMethod]
        public void GuardTest_BadData()
        {
            string testString;

            testString = null;
            Assert.Throws<ArgumentException>(() => Guard.Argument(testString, nameof(testString)).NotNull());

            testString = "";
            Assert.Throws<ArgumentException>(() => Guard.Argument(testString, nameof(testString)).NotEmpty());

            testString = "Hello";
            Assert.Throws<ArgumentException>(() => Guard.Argument(testString, nameof(testString)).NotUppercase());
            Assert.Throws<ArgumentException>(() => Guard.Argument(testString, nameof(testString)).Null());
            Assert.Throws<ArgumentException>(() => Guard.Argument(testString, nameof(testString)).Empty());

            testString = null;
            Assert.Throws<ArgumentException>(() => Guard.Argument(testString, nameof(testString)).ThrowIf(argument => argument.Value == null, argument => new ArgumentException("Test Argument is Null", argument.Name)));
            
        }
    }
}
