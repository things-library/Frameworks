using ThingsLibrary.Testing.Containers;
using ThingsLibrary.Testing.Extensions;

namespace ThingsLibrary.Testing.Tests.Containers
{
    [TestClass, ExcludeFromCodeCoverage]
    public class ReflectionTests
    {
        private class TestClass
        {
            public string Name { get; set; }            
            public int Age { get; set; }
        }

        [TestMethod]
        public void AssertNonDefaultProperties()
        {            
            new TestClass { Name = "Test Name", Age = 10 }.AssertNonDefaultProperties();
        }

        [TestMethod]
        public void AssertNonDefaultProperties_BadData()
        {
            Assert.ThrowsException<AssertFailedException>(() => new TestClass { Name = "Test Name" }.AssertNonDefaultProperties());
            Assert.ThrowsException<AssertFailedException>(() => new TestClass { Age = 10 }.AssertNonDefaultProperties());
        }
    }
}