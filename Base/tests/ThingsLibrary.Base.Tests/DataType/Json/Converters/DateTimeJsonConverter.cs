// ================================================================================
// <copyright file="DateTimeJsonConverter.cs" company="Starlight Software Co">
//    Copyright (c) 2025 Starlight Software Co. All rights reserved.
//    Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
// </copyright>
// ================================================================================

using ThingsLibrary.DataType.Json.Converters;

namespace ThingsLibrary.Tests.Json.Converters
{
    [TestClass, ExcludeFromCodeCoverage]
    public class DateTimeJsonConverterTests
    {
        [TestMethod]
        public void TestDeserializer()
        {
            var testClass = JsonSerializer.Deserialize<TestClass>(
               @"{                     
                    ""date1"": ""2022-09-04"",
                    ""date2"": ""10/11/2022""
                }");

            // Arrays (maxItems, minItems, uniqueItems, maxContains, minContains)
            Assert.IsNotNull(testClass);
                        
            Assert.AreEqual(new DateTime(2022, 9, 4), testClass.Date1);
            Assert.AreEqual(new DateTime(2022, 10, 11), testClass.Date2);
        }

        [TestMethod]
        public void TestSerializer()
        {
            var newClass = new TestClass
            {
                Date1 = new DateTime(2022, 9, 4, 1, 2, 3),
                Date2 = new DateTime(2022, 10, 11),
            };

            var json = JsonSerializer.Serialize(newClass);

            var testClass = JsonSerializer.Deserialize<TestClass>(json);

            // Arrays (maxItems, minItems, uniqueItems, maxContains, minContains)
            Assert.IsNotNull(testClass);

            Assert.AreEqual(new DateTime(2022, 9, 4), testClass.Date1);
            Assert.AreEqual(new DateTime(2022, 10, 11), testClass.Date2);
        }

        [TestMethod]
        public void TestSerializer_BadClass()
        {
            Assert.Throws<ArgumentException>(() => JsonSerializer.Deserialize<BadClass>(
               @"{  
                    ""date1"": ""10/11/2022""
                }"));
        }

        [ExcludeFromCodeCoverage]
        public class TestClass
        {
            [JsonConverter(typeof(DateTimeJsonConverter))]
            [JsonPropertyName("date1")]
            public DateTime Date1 { get; set; }

            [JsonPropertyName("date2")]
            [DateTimeJson("MM/dd/yyyy")]
            public DateTime Date2 { get; set; }
        }

        public class BadClass
        {
            [DateTimeJson("MM/dd/yyyy")]
            [JsonPropertyName("date1")]
            public Double Date1 { get; set; }
        }
    }
}
