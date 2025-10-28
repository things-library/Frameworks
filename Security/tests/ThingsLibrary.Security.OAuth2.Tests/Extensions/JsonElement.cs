using ThingsLibrary.DataType.Extensions;

namespace ThingsLibrary.Security.OAuth2.Tests.Extensions
{
    [TestClass, ExcludeFromCodeCoverage]
    public class JsonElementTests
    {
        public static JsonElement TestDocument;


        [ClassInitialize]
        public static void ClassInitialize(TestContext testContext)
        {
            TestDocument = GetTestDocument().RootElement;
        }

        [DataTestMethod]
        [DataRow("user/id", "10000000-0000-0000-0000-000000000001")]
        [DataRow("user/name", "Test")]
        [DataRow("user/contact/address/city", "Test City")]
        [DataRow("user/MISSING_NODE", "[DEFAULT]")]
        [DataRow("user/MISSING_NODE/name", "[DEFAULT]")]
        public void GetPropertyString(string propertyKey, string expectedValue)
        {
            Assert.AreEqual(expectedValue, TestDocument.GetProperty<string>(propertyKey, "[DEFAULT]"));
        }

        [DataTestMethod]
        [DataRow("", null, "System.String")]
        [DataRow(null, null, "System.String")]
        public void GetPropertyString_BadData(string propertyKey, string expectedValue, string typeStr)
        {
            Assert.Throws<ArgumentNullException>(() => TestDocument.GetProperty<string>("", ""));
            Assert.Throws<ArgumentNullException>(() => TestDocument.GetProperty<string>(null, ""));
        }

        [DataTestMethod]
        [DataRow("CreatedOn", "2000-01-02T03:04:05.0", "System.DateTime")]
        [DataRow("id", "12345", "System.Int32")]
        [DataRow("user/contact/address/zip", "50263", "System.Int32")]
        [DataRow("user/contact/MISSING_NODE/zip", "0", "System.Int32")]
        public void GetPropertyGenerics(string propertyKey, string expectedValue, string typeStr)
        {
            var returnType = Type.GetType(typeStr) ?? throw new ArgumentNullException(nameof(typeStr));
            var expectedResult = Convert.ChangeType(expectedValue, returnType);

            // Get the generic method
            var genericGetMethod = typeof(JsonElementExtensions).GetMethod("GetProperty") ?? throw new ArgumentException("Unable to find generic method");

            // Make the non-generic method via the 'MakeGenericMethod' reflection call.            
            var typedGetMethod = genericGetMethod.MakeGenericMethod(new[] { returnType });

            //// Invoke the method just like a normal method.
            /// INVOKE: public static T GetProperty<T>(this JsonElement rootElement, string propertyPath, T defaultValue = default)
            var result = typedGetMethod.Invoke(new JsonElement(), new object[] { TestDocument, propertyKey, null });

            Assert.AreEqual(expectedResult, result);
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
    }
}
