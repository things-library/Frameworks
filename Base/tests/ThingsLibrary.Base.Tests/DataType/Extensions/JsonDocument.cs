// ================================================================================
// <copyright file="JsonDocument.cs" company="Starlight Software Co">
//    Copyright (c) 2025 Starlight Software Co. All rights reserved.
//    Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
// </copyright>
// ================================================================================

namespace ThingsLibrary.Tests.DataType.Extensions
{
    [TestClass, ExcludeFromCodeCoverage]
    public class JsonDocumentTests
    {
        public static JsonDocument TestDocument;

        public static TestClass GetTestClass() => new TestClass
        {
            GuidValue = new Guid("99999999-9999-9999-9999-999999999991"),
            GuidNullibleValue = new Guid("99999999-9999-9999-9999-999999999992"),

            ByteValue = 12,
            ByteNullibleValue = 13,

            ShortValue = 123,
            ShortNullibleValue = 1234,

            IntValue = 99999991,
            IntNullibleValue = 99999992,

            LongValue = 123456789012,
            LongNullibleValue = 123456789013,

            DoubleValue = 2345678890.12345,
            DoubleNullibleValue = 345678890.2345,

            StringValue = "String1234567890",

            BoolValue = true,
            BoolNullibleValue = false,

            DateTimeValue = new DateTime(2000, 1, 2, 3, 4, 5),
            DateTimeNullibleValue = new DateTime(2000, 1, 2, 3, 4, 5),

            DateTimeOffsetValue = new DateTimeOffset(2000, 1, 2, 3, 4, 5, 6, TimeSpan.FromHours(2)),
            DateTimeOffsetNullibleValue = new DateTimeOffset(2000, 1, 2, 3, 4, 5, 6, TimeSpan.FromHours(5))
        };

        [ClassInitialize]
        public static void ClassInitialize(TestContext testContext)
        {
            TestDocument = GetTestDocument();
        }

        [TestMethod]
        [DataRow("user/id", "10000000-0000-0000-0000-000000000001")]
        [DataRow("user/name", "Test")]
        [DataRow("user/contact/address/city", "Test City")]
        [DataRow("user/MISSING_NODE", "[DEFAULT]")]
        [DataRow("user/MISSING_NODE/name", "[DEFAULT]")]
        public void GetPropertyString(string propertyKey, string expectedValue)
        {
            Assert.AreEqual(expectedValue, TestDocument.GetProperty<string>(propertyKey, "[DEFAULT]"));
        }

        [TestMethod]
        [DataRow("CreatedOn", "2000-01-02T03:04:05.0", "System.DateTime")]
        [DataRow("id", "12345", "System.Int32")]
        [DataRow("user/contact/address/zip", "50263", "System.Int32")]
        [DataRow("user/contact/MISSING_NODE/zip", "0", "System.Int32")]
        public void GetPropertyGenerics(string propertyKey, string expectedValue, string typeStr)
        {
            //var returnType = Type.GetType(typeStr);
            //var expectedResult = Convert.ChangeType(expectedValue, returnType);

            //// Get the generic method
            //var genericGetMethod = typeof(JsonDocumentExtensions).GetMethod("GetProperty");

            //// Make the non-generic method via the 'MakeGenericMethod' reflection call.            
            //var typedGetMethod = genericGetMethod.MakeGenericMethod(new[] { returnType });

            ////// Invoke the method just like a normal method.
            ///// INVOKE: public static T GetProperty<T>(this JsonElement rootElement, string propertyPath, T defaultValue = default)
            //var result = typedGetMethod.Invoke(new JsonDocument(), new object[] { TestDocument, propertyKey, null });

            //Assert.AreEqual(expectedResult, result);            
        }

        [TestMethod]
        public void ToObject()
        {
            var testClass = GetTestClass();

            var json = JsonSerializer.Serialize(testClass);

            var doc = JsonDocument.Parse(json);

            var testClass2 = doc.ToObject<TestClass>();

            Assert.AreEqual(testClass.IntValue, testClass2.IntValue);
            Assert.AreEqual(testClass.DateTimeValue, testClass2.DateTimeValue);
            Assert.AreEqual(testClass.StringValue, testClass2.StringValue);
            Assert.AreEqual(testClass.BoolValue, testClass2.BoolValue);            
        }

        public static JsonDocument GetTestDocument()
        {
            //{
            //    "id": 12345,
            //    "user": {            
            //        "id": "10000000-0000-0000-0000-000000000001",
            //        "name": "Test",
            //        "familyId": null,
            //        "contact": {
            //            "type": 2,
            //            "address": {
            //                "city": "Test City",
            //                "zip": 50263
            //            },
            //            "phone": "555-555-5555",
            //            "email": "testuser@test.com"
            //        } 
            //    }
            //    "CreatedOn": "2000-01-02T03:04:05.0"
            //}

            return JsonDocument.Parse(
            @"{
                ""id"": 12345,
                ""user"": {
                    ""id"": ""10000000-0000-0000-0000-000000000001"",
                    ""name"": ""Test"",
                    ""familyId"": null,
                    ""contact"": {
                        ""type"": 2,
                        ""address"": {
                            ""city"": ""Test City"",
                            ""zip"": 50263
                        },
                        ""phone"": ""555-555-5555"",
                        ""email"": ""testuser@test.com""
                    }
                },
                ""CreatedOn"": ""2000-01-02T03:04:05.0""
            }");
        }

        public class TestClass
        {
            // NUMERICS
            [Key]
            public Guid GuidValue { get; set; }
            public Guid? GuidNullibleValue { get; set; }

            // 8bit
            public byte ByteValue { get; set; }
            public byte? ByteNullibleValue { get; set; }

            // 16bit
            public short ShortValue { get; set; }
            public short? ShortNullibleValue { get; set; }

            // 32bit
            public int IntValue { get; set; }
            public int? IntNullibleValue { get; set; }

            // 64bit
            public long LongValue { get; set; }
            public long? LongNullibleValue { get; set; }

            public double DoubleValue { get; set; }
            public double? DoubleNullibleValue { get; set; }


            // ASCII
            public string StringValue { get; set; }

            // BOOL
            public bool BoolValue { get; set; }
            public bool? BoolNullibleValue { get; set; }

            // DATE 
            public DateTime DateTimeValue { get; set; }
            public DateTime? DateTimeNullibleValue { get; set; }

            public DateTimeOffset DateTimeOffsetValue { get; set; }
            public DateTimeOffset? DateTimeOffsetNullibleValue { get; set; }
        }
    }
}
