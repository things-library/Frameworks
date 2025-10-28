using ThingsLibrary.DataType.Json.Converters;

namespace ThingsLibrary.Tests.Json.Converters
{
    [TestClass, ExcludeFromCodeCoverage]
    public class StringOrEmptyArrayConverterTests
    {
        [TestMethod]
        public void TestDeserializer()
        {
            var testClass = JsonSerializer.Deserialize<TestClass>(
               @"{                     
                    ""name"": ""Test"",
                    ""name2"": []
                }");

            // Arrays (maxItems, minItems, uniqueItems, maxContains, minContains)
            Assert.IsNotNull(testClass);

            Assert.AreEqual("Test", testClass.Name);
            Assert.AreEqual("", testClass.Name2);
        }

        [TestMethod]
        public void TestSerializer()
        {
            var newClass = new TestClass
            {
                Name = "Test",
                Name2 = ""
            };

            var json = JsonSerializer.Serialize(newClass);

            var testClass = JsonSerializer.Deserialize<TestClass>(json);

            // Arrays (maxItems, minItems, uniqueItems, maxContains, minContains)
            Assert.IsNotNull(testClass);

            Assert.AreEqual("Test", testClass.Name);
            Assert.AreEqual("", testClass.Name2);
        }

        [TestMethod]
        public void TestDeserializer_BadData()
        {
            Assert.Throws<ArgumentException>(() => JsonSerializer.Deserialize<TestClass>(
               @"{                     
                    ""name"": ""Test"",
                    ""name2"": [""BadValue""]
                }"));
        }

        [ExcludeFromCodeCoverage]
        public class TestClass
        {
            [JsonConverter(typeof(StringOrEmptyArrayConverter))]
            [JsonPropertyName("name")]
            public string Name { get; set; }

            [JsonPropertyName("name2")]
            [JsonConverter(typeof(StringOrEmptyArrayConverter))]
            public string Name2 { get; set; }
        }
    }    
}
