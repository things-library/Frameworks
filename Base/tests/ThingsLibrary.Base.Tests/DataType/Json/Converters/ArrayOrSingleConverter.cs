// ================================================================================
// <copyright file="ArrayOrSingleConverter.cs" company="Starlight Software Co">
//    Copyright (c) 2025 Starlight Software Co. All rights reserved.
//    Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
// </copyright>
// ================================================================================

using ThingsLibrary.DataType.Json.Converters;

namespace ThingsLibrary.Tests.Json.Converters
{
    [TestClass, ExcludeFromCodeCoverage]
    public class ArrayOrSingleConverterTests
    {
        [TestMethod]
        public void TestDeserializer()
        {
            var testClass = JsonSerializer.Deserialize<TestClass>(
               @"{                     
                    ""items"": ""Test"",
                    ""moreItems"": [ ""item1"", ""item2"", ""item3"" ]
                }");

            // Arrays (maxItems, minItems, uniqueItems, maxContains, minContains)
            Assert.IsNotNull(testClass);

            Assert.AreEqual("Test", testClass.Items[0]);
            Assert.AreEqual("item3", testClass.MoreItems[2]);
        }

        [TestMethod]
        public void TestSerializer()
        {
            var newClass = new TestClass
            {
                Items = new List<string>
                {
                    "Test"
                },
                MoreItems = new List<string>
                {
                    "item1", "item2", "item3"
                }
            };

            var json = JsonSerializer.Serialize(newClass);

            var testClass = JsonSerializer.Deserialize<TestClass>(json);

            // Arrays (maxItems, minItems, uniqueItems, maxContains, minContains)
            Assert.IsNotNull(testClass);

            Assert.AreEqual("Test", testClass.Items[0]);
            Assert.AreEqual("item3", testClass.MoreItems[2]);
        }

        [ExcludeFromCodeCoverage]
        public class TestClass
        {
            [JsonConverter(typeof(ArrayOrSingleConverter<string>))]
            [JsonPropertyName("items")]
            public List<string> Items { get; set; }

            [JsonPropertyName("moreItems")]
            [JsonConverter(typeof(ArrayOrSingleConverter<string>))]
            public List<string> MoreItems { get; set; }
        }
    }

    
}
