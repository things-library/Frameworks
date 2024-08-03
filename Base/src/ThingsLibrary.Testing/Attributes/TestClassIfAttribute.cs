namespace ThingsLibrary.Testing.Attributes
{
    // Source: https://matt.kotsenas.com/posts/ignoreif-mstest

    /// <summary>
    /// An extension of the [TestClass] attribute. If applied to a class, any [TestMethod] attributes are automatically upgraded to [TestMethodIfIfSupport].
    /// </summary>
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = true)]
    public class TestClassIfAttribute : TestClassAttribute
    {
        public override TestMethodAttribute GetTestMethodAttribute(TestMethodAttribute testMethodAttribute)
        {
            if (testMethodAttribute is TestMethodIfAttribute)
            {
                return testMethodAttribute;
            }

            return new TestMethodIfAttribute();
        }
    }
}
