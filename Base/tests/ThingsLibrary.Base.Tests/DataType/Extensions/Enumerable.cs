// ================================================================================
// <copyright file="Enumerable.cs" company="Starlight Software Co">
//    Copyright (c) 2025 Starlight Software Co. All rights reserved.
//    Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
// </copyright>
// ================================================================================

using System.Collections.Generic;

namespace ThingsLibrary.Tests.DataType.Extensions
{
    [TestClass, ExcludeFromCodeCoverage]
    public class EnumerableTests
    {
        [TestMethod]        
        public void ForEach()
        {
            IEnumerable<int> items = new List<int> { 1, 2, 3, 4, 5, 6, 7, 8, 9, 10 }.ToList();

            int i = 0;
            items.ForEach(item => 
            {
                i++;
                Assert.AreEqual(i, item);
            });            
        }
    }
}