using System.Collections.Generic;

namespace ThingsLibrary.Tests.DataType.Extensions
{
    [TestClass, ExcludeFromCodeCoverage]
    public class DictionaryTests
    {
        public static Dictionary<string, int> TestDictionary { get; set; } = new Dictionary<string, int>
        {
            { "One", 1 },
            { "Two", 2 },
            { "Three", 3 },
            { "Four", 4 },
            { "Five", 5 },
            { "Seven", 7 },
            { "Ten", 10 }
        };

        public static Dictionary<string, string> TestStringDictionary { get; set; } = new Dictionary<string, string>
        {
            { "One", "1" },
            { "Two", "2" },
            { "Three", "3" },
            { "Four", "4" },
            { "Five", "5" },
            { "Seven", "7" },
            { "Ten", "10" }
        };

        public static Dictionary<string, object> TestObjectDictionary { get; set; } = new Dictionary<string, object>
        {
            { "One", 1 },
            { "Two", 2 },
            { "Three", 3 },
            { "Four", 4 },
            { "Five", 5 },
            { "Seven", 7 },
            { "Ten", 10 }
        };

        [ClassInitialize]
        public static void ClassInitialize(TestContext testContext)
        {
            //nothing
        }

        [TestMethod]
        public void AddRange()
        {
            var addDictionary = new Dictionary<string, int>
            {
                { "Five", 71 },
                { "Seven", 11 },
                { "Eight", 8 }
            };

            // only the "Eight" should have changed
            DictionaryTests.TestDictionary.AddRange(addDictionary, false);

            Assert.AreEqual(5, DictionaryTests.TestDictionary["Five"]);
            Assert.AreEqual(7, DictionaryTests.TestDictionary["Seven"]);
            Assert.AreEqual(8, DictionaryTests.TestDictionary["Eight"]);

            // all of them should now match
            DictionaryTests.TestDictionary.AddRange(addDictionary, true);

            Assert.AreEqual(71, DictionaryTests.TestDictionary["Five"]);
            Assert.AreEqual(11, DictionaryTests.TestDictionary["Seven"]);
            Assert.AreEqual(8, DictionaryTests.TestDictionary["Eight"]);
        }

        [DataTestMethod]
        [DataRow("One", 1)]
        [DataRow("Two", 2)]
        [DataRow("Three", 3)]
        [DataRow("Ten", 10)]
        [DataRow("MISSING", 0)]
        public void GetValue(string key, int expectedValue)
        {
            Assert.AreEqual(expectedValue, DictionaryTests.TestStringDictionary.GetValue<int>(key, default));
        }

        [DataTestMethod]
        [DataRow("One", 1)]
        [DataRow("Two", 2)]
        [DataRow("Three", 3)]
        [DataRow("Ten", 10)]
        [DataRow("MISSING", 0)]
        public void GetValueObj(string key, int expectedValue)
        {
            Assert.AreEqual(expectedValue, DictionaryTests.TestObjectDictionary.GetValue<int>(key, default));
        }


        [TestMethod]        
        public void GetValue_T()
        {
            var dictionary = new Dictionary<string, string>()
            {
                { "Key123", "2022-09-03T12:01:02" },
                { "Key234", "BAD DATE" }
            };

            Assert.AreEqual(new DateTime(2022, 09, 03, 12, 1, 2), dictionary.GetValue<DateTime>("Key123", DateTime.MinValue));
            Assert.AreEqual(DateTime.MinValue, dictionary.GetValue<DateTime>("Key234", DateTime.MinValue));
            Assert.AreEqual(DateTime.MinValue, dictionary.GetValue<DateTime>("BADKEY", DateTime.MinValue));            
        }
    }
}