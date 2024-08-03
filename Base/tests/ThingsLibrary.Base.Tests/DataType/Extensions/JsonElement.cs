namespace ThingsLibrary.Tests.DataType.Extensions
{
    [TestClass, ExcludeFromCodeCoverage]
    public class JsonElementTests
    {
        public static JsonElement TestDocument;


        [ClassInitialize]
        public static void ClassInitialize(TestContext testContext)
        {
            TestDocument = JsonDocumentTests.GetTestDocument().RootElement;
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

        [TestMethod]
        public void GetProperty_NullPath()
        {
            Assert.ThrowsException<ArgumentNullException>(() => TestDocument.GetProperty<string>(null, "[DEFAULT]"));
        }

        [DataTestMethod]
        [DataRow("CreatedOn", "2000-01-02T03:04:05.0", "System.DateTime")]
        [DataRow("id", "12345", "System.Int32")]
        [DataRow("user/contact/address/zip", "50263", "System.Int32")]
        [DataRow("user/contact/MISSING_NODE/zip", "0", "System.Int32")]
        public void GetPropertyGenerics(string propertyKey, string expectedValue, string typeStr)
        {
            var returnType = Type.GetType(typeStr);
            var expectedResult = Convert.ChangeType(expectedValue, returnType);

            // Get the generic method
            var genericGetMethod = typeof(JsonElementExtensions).GetMethod("GetProperty");

            // Make the non-generic method via the 'MakeGenericMethod' reflection call.            
            var typedGetMethod = genericGetMethod.MakeGenericMethod(new[] { returnType });

            //// Invoke the method just like a normal method.
            /// INVOKE: public static T GetProperty<T>(this JsonElement rootElement, string propertyPath, T defaultValue = default)
            var result = typedGetMethod.Invoke(new JsonElement(), new object[] { TestDocument, propertyKey, null });

            Assert.AreEqual(expectedResult, result);
        }

        [TestMethod]
        public void ToObject()
        {
            var testClass = JsonDocumentTests.GetTestClass();

            var json = JsonSerializer.Serialize(testClass);

            var doc = JsonDocument.Parse(json);

            var testClass2 = doc.RootElement.ToObject<JsonDocumentTests.TestClass>();

            Assert.AreEqual(testClass.IntValue, testClass2.IntValue);
            Assert.AreEqual(testClass.DateTimeValue, testClass2.DateTimeValue);
            Assert.AreEqual(testClass.StringValue, testClass2.StringValue);
            Assert.AreEqual(testClass.BoolValue, testClass2.BoolValue);
        }
    }
}

