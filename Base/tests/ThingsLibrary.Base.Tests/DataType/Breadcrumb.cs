// ================================================================================
// <copyright file="Breadcrumb.cs" company="Starlight Software Co">
//    Copyright (c) 2025 Starlight Software Co. All rights reserved.
//    Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
// </copyright>
// ================================================================================

using ThingsLibrary.DataType;
using System.Diagnostics.CodeAnalysis;

namespace ThingsLibrary.Tests.DataType
{
    [TestClass, ExcludeFromCodeCoverage]
    public class BreadcrumbTests
    {
        [TestMethod]
        public void Constructor()
        {
            var title = "Test Message";
            var url = "/something/something2";
            var isActive = true;


            var breadCrumb = new Breadcrumb(title, url, isActive);

            Assert.AreEqual(title, breadCrumb.Title);
            Assert.AreEqual(url, breadCrumb.Url);
            Assert.AreEqual(isActive, breadCrumb.Active);
        }
    }
}
