using System.Reflection;
using System.Web;

namespace ThingsLibrary.Tests.DataType.Extensions
{
    [TestClass, ExcludeFromCodeCoverage]
    public class ClassTests
    {
        public class TestClass
        {
            [Key]
            [JsonPropertyName("id")]
            public Guid Id { get; set; }

            [JsonPropertyName("name")]
            [StringLength(20, ErrorMessage = "Name must be between 3 and 20 characters.", MinimumLength = 3), Required(AllowEmptyStrings = false, ErrorMessage = "Name is required")]
            public string Name { get; set; } = string.Empty;

            [JsonPropertyName("hired"), JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
            public DateTimeOffset? HireDate { get; set; }

            [JsonPropertyName("empid"), JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
            [Range(1, 9999)]
            public int? EmployeeNumber { get; set; }
        }

        private class TestClassNoKey
        {
            public Guid Id { get; set; }

            public string Name { get; set; } = string.Empty;
        }

        private class TestClassCompositeKeys
        {
            [Key]
            public int Id { get; set; }

            [Key]
            public int Key{ get; set; }
        }

        [TestMethod]
        public void ToValidationResults()
        {
            var testClass = new TestClass
            {
                Id = Guid.NewGuid(),
                Name = "Test User",
                HireDate = new DateTimeOffset(DateTime.Now),
                EmployeeNumber = 123
            };

            var testResults = testClass.ToValidationResults();

            // ======================================================================
            // no annotation errors
            // ======================================================================
            Assert.IsTrue(testResults.Count == 0);

            // ======================================================================
            // range (max) violation
            // ======================================================================
            testClass.EmployeeNumber = 99999999;
            testResults = testClass.ToValidationResults();
            Assert.IsTrue(testResults.Count == 1);
            Assert.IsTrue(testResults.First().MemberNames.First() == "EmployeeNumber");

            // ======================================================================
            // range (min) violation
            // ======================================================================
            testClass.EmployeeNumber = 0;
            testResults = testClass.ToValidationResults();
            Assert.IsTrue(testResults.Count == 1);
            Assert.IsTrue(testResults.First().MemberNames.First() == "EmployeeNumber");

            testClass.EmployeeNumber = null;
            testResults = testClass.ToValidationResults();
            Assert.IsTrue(testResults.Count == 0);

            testClass.EmployeeNumber = 123; //put it back

            // ======================================================================
            // string min length violation
            // ======================================================================
            testClass.Name = "1";
            testResults = testClass.ToValidationResults();
            Assert.IsTrue(testResults.Count == 1);
            Assert.IsTrue(testResults.First().MemberNames.First() == "Name");

            // ======================================================================
            // string max length violation
            // ======================================================================
            testClass.Name = "1234567890123456789012345";
            testResults = testClass.ToValidationResults();
            Assert.IsTrue(testResults.Count == 1);
            Assert.IsTrue(testResults.First().MemberNames.First() == "Name");

            testClass.Name = "Test User";   //put it back

            // ======================================================================
            // required field validation
            // ======================================================================
            testClass.Name = string.Empty;
            testResults = testClass.ToValidationResults();
            Assert.IsTrue(testResults.Count == 1);
            Assert.IsTrue(testResults.First().MemberNames.First() == "Name");

        }

        [TestMethod]        
        public void ToQueryString()
        {
            var seperator = "&";

            var testClass = new TestClass
            {
                Id = Guid.NewGuid(),
                Name = "Test User",
                HireDate = DateTimeOffset.Now,
                EmployeeNumber = 123
            };

            // serialize the class to querystring
            var testResults = testClass.ToQueryString(seperator);

            // parse it back out as a query string collection
            var query = HttpUtility.ParseQueryString(testResults);

            // test their json serialization names, parsing of different types.
            Assert.IsTrue(query.Count == 4);
            Assert.IsTrue(Guid.Parse(query["id"]) == testClass.Id);
            Assert.IsTrue(string.Compare(query["name"], testClass.Name, false) == 0);
            Assert.IsTrue(DateTimeOffset.Parse(query["hired"]) == testClass.HireDate);
            Assert.IsTrue(int.Parse(query["empid"]) == testClass.EmployeeNumber);
            
            // json ignore field test
            testClass.HireDate = null;
            query = HttpUtility.ParseQueryString(testClass.ToQueryString(seperator));
            Assert.IsTrue(query.Count == 3);            
        }

        [TestMethod]
        public void GetProperty()
        {
            var testClass = new TestClass
            {
                Id = Guid.NewGuid()
            };

            var attrib = testClass.GetType().GetProperties().Single(x => x.GetCustomAttributes(typeof(KeyAttribute), false).Any());

            var keyValue = testClass.GetPropertyValue<Guid>(attrib, Guid.Empty);
            Assert.AreEqual(testClass.Id, keyValue);
        }

        [TestMethod]
        public void GetProperty_ByAttribute()
        {
            var testClass = new TestClass
            {
                Id = Guid.NewGuid()
            };

            var keyValue = testClass.GetPropertyValue<KeyAttribute, Guid>(Guid.Empty);
            Assert.AreEqual(testClass.Id, keyValue);
        }


        [TestMethod]
        public void GetKeys()
        {
            var testClass = new TestClass
            {
                Id = Guid.NewGuid()
            };

            var keys = testClass.GetKeys((Type)null);

            Assert.AreEqual(testClass.Id, keys[0]);
        }

        [TestMethod]
        public void GetKeys_NoKey()
        {
            var testClass = new TestClassNoKey();

            Assert.ThrowsException<ArgumentException>(() => testClass.GetKeys((Type)null));
        }


        [TestMethod]
        public void GetKeys_CompositeKey()
        {
            var testClass = new TestClassCompositeKeys()
            {
                Id = 123,
                Key = 456
            };

            Assert.AreEqual("123|456", testClass.GetCompositeKey());
        }

        [TestMethod]
        public void GetKeys_CompositeKeyFromProperty()
        {
            var testClass = new TestClassCompositeKeys()
            {
                Id = 123,
                Key = 456
            };

            var keys = new List<PropertyInfo>()
            {
                testClass.GetType().GetProperty("Id"),
                testClass.GetType().GetProperty("Key")
            };
                        
            Assert.AreEqual("123|456", testClass.GetCompositeKey(keys));
        }       

    }
}