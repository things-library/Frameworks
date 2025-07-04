// ================================================================================
// <copyright file="TestClass.cs" company="Starlight Software Co">
//    Copyright (c) Starlight Software Co. All rights reserved.
//    Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
// </copyright>
// ================================================================================

namespace ThingsLibrary.Cache.Tests.TestData
{
    public class TestClass
    {
        public string Name { get; set; } = string.Empty;
        public DateTimeOffset? Date { get; set; }
        public string Type { get; set; } = string.Empty;
                
        public IDictionary<string, TestClass> Items { get; set; } = new Dictionary<string, TestClass>();
    }
}
