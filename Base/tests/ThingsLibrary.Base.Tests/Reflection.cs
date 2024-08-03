namespace ThingsLibrary.Tests
{
    [TestClass, ExcludeFromCodeCoverage]
    public class ReflectionTests
    {
        [TestMethod]
        public void GetDefault_Primitives()
        {
            Assert.AreEqual(false, Reflection.GetDefault<bool>());
            Assert.AreEqual(null, Reflection.GetDefault<bool?>());

            Assert.AreEqual(new DateTime(), Reflection.GetDefault<DateTime>());
            Assert.AreEqual(null, Reflection.GetDefault<DateTime?>());

            Assert.AreEqual(null, Reflection.GetDefault(null));
        }

        [TestMethod]
        public void GetDefault_Objects()
        {
            Assert.AreEqual(default(TestStruct), Reflection.GetDefault(typeof(TestStruct)));

            Assert.AreEqual(null, Reflection.GetDefault(typeof(TestClass2<>)));

            Assert.AreEqual(null, Reflection.GetDefault(typeof(ITestInterface)));            
        }

        [TestMethod]
        public void GetDefaultProperties()
        {
            var testClass = new TestClass();

            // check how many properties of the class are default(T)
            var defaultProperties = Reflection.GetDefaultProperties(testClass);

            Assert.AreEqual(5, defaultProperties.Count);
            Assert.IsTrue(defaultProperties.Any(x => x.Name == "Name"));
            Assert.IsTrue(defaultProperties.Any(x => x.Name == "OrcFamilyName"));
            Assert.IsTrue(defaultProperties.Any(x => x.Name == "BirthDate"));
            Assert.IsTrue(defaultProperties.Any(x => x.Name == "DeathDate"));
            Assert.IsTrue(defaultProperties.Any(x => x.Name == "Age"));

            // set one of them and see how many properties are still set to 'default(T)' value
            testClass.Name = "Test Name";

            defaultProperties = Reflection.GetDefaultProperties(testClass);
            Assert.AreEqual(4, defaultProperties.Count);
            Assert.IsTrue(!defaultProperties.Any(x => x.Name == "Name"));
        }

        [TestMethod]
        public void GetDefaultProperties_OnePropSet()
        {
            var testClass = new TestClass()
            {
                Name = "Test Name"
            };

            // check how many properties of the class are default(T)
            var defaultProperties = Reflection.GetDefaultProperties(testClass);

            Assert.AreEqual(4, defaultProperties.Count);
            Assert.IsTrue(!defaultProperties.Any(x => x.Name == "Name"));

            Assert.IsTrue(defaultProperties.Any(x => x.Name == "OrcFamilyName"));
            Assert.IsTrue(defaultProperties.Any(x => x.Name == "BirthDate"));
            Assert.IsTrue(defaultProperties.Any(x => x.Name == "DeathDate"));
            Assert.IsTrue(defaultProperties.Any(x => x.Name == "Age"));
        }

        [TestMethod]
        public void GetDefaultProperties_Generics()
        {
            //Arrange
            var testClass = new TestClass2<bool>();

            //Act
            var defaultProperties = Reflection.GetDefaultProperties(testClass);

            //Assert
            Assert.AreEqual(6, defaultProperties.Count);
        }

        public struct TestStruct
        {
            public string Name { get; set; }
            public DateTime CreatedOn { get; set; }
        }

        public interface ITestInterface
        {
            public string Name { get; set; }
            public DateTime CreatedOn { get; set; }
        }

        private class TestClass
        {
            public string Name { get; set; }
            public string OrcFamilyName { get; set; }

            public DateTime BirthDate { get; set; }
            public DateTime? DeathDate { get; set; }

            public int Age { get; set; }

            // internal variables            
            private int PartyDays { get; set; }
            private int? Arrests { get; set; }

            public TestClass()
            {
                //nothing
            }
        }

        private class TestClass2<T> : TestClass
        {
            public T Value { get; set; }

            private DateTime? GraduationDate { get; set; }
        }

        private class TestClass3
        {
            public TestStruct Value { get; set; }

            private DateTime? GraduationDate { get; set; }
        }
    }
}
