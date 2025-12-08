// ================================================================================
// <copyright file="Object.cs" company="Starlight Software Co">
//    Copyright (c) 2025 Starlight Software Co. All rights reserved.
//    Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
// </copyright>
// ================================================================================

namespace ThingsLibrary.Tests.DataType.Extensions
{
    [TestClass, ExcludeFromCodeCoverage]
    public class ObjectTests
    {
        [TestMethod]
        public void ConvertTo()
        {            
            Assert.AreEqual(true, "true".ConvertTo<bool>());

            Assert.AreEqual(new DateTime(2000, 1, 2), "2000-01-02".ConvertTo<DateTime>());

            Assert.AreEqual(100, "100".ConvertTo<int>());
            Assert.AreEqual(100, 100f.ConvertTo<int>());
            Assert.AreEqual(100d, 100f.ConvertTo<double>());

            int? value = 100;
            Assert.AreEqual(100d, value.ConvertTo<double>());            
        }

        [TestMethod]        
        public void ConvertTo_Bad()
        {
            int? value = null;
            Assert.Throws<InvalidCastException>(() => value.ConvertTo<double>());

            //Assert.AreEqual(1d, 100d.ConvertTo<DateTime>());
        }
    }
}
