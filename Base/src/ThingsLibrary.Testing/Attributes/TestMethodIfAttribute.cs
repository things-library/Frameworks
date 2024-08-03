namespace ThingsLibrary.Testing.Attributes
{
    // Source: https://matt.kotsenas.com/posts/ignoreif-mstest

    /// <summary>
    /// An extension to the [TestMethod] attribute. It walks the method and class hierarchy looking for an [IgnoreIf] attribute. If one or more are found, 
    /// they are each evaluated, if the attribute returns 'true', evaluation is short-circuited, and the test method is skipped.
    /// </summary>
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = false, Inherited = true)]
    public class TestMethodIfAttribute : TestMethodAttribute
    {
        public TestMethodIfAttribute() : base()
        {                 
            //nothing
            //TODO: set display name to the namespace of the method otherwise test results in devops will be just the method names
        }

        public TestMethodIfAttribute(string displayName) : base(displayName)
        {
            //nothing
        }

        public override TestResult[] Execute(ITestMethod testMethod)
        {
            var ignoreAttributes = this.FindAttributes(testMethod);

            // Evaluate each attribute, and skip if one returns `true`
            foreach (var ignoreAttribute in ignoreAttributes)
            {
                if (ignoreAttribute.ShouldIgnore(testMethod))
                {
                    var message = $"Test not executed. Conditional ignore method '{ignoreAttribute.IgnoreCriteriaMethodName}' evaluated to 'true'.";
                    return new[]
                    {
                        new TestResult
                        {
                            Outcome = UnitTestOutcome.Inconclusive,
                            TestFailureException = new AssertInconclusiveException(message)
                        }
                    };
                }
            }
            return base.Execute(testMethod);
        }

        private IEnumerable<IgnoreIfAttribute> FindAttributes(ITestMethod testMethod)
        {
            // Look for an [IgnoreIf] on the method, including any virtuals this method overrides
            var ignoreAttributes = new List<IgnoreIfAttribute>();
            ignoreAttributes.AddRange(testMethod.GetAttributes<IgnoreIfAttribute>(inherit: true));

            // Walk the class hierarchy looking for an [IgnoreIf] attribute
            var type = testMethod.MethodInfo.DeclaringType;
            while (type != null)
            {
                ignoreAttributes.AddRange(type.GetCustomAttributes<IgnoreIfAttribute>(inherit: true));
                type = type.DeclaringType;
            }
            return ignoreAttributes;
        }
    }
}
