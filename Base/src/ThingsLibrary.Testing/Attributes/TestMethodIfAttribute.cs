// ================================================================================
// <copyright file="TestMethodIfAttribute.cs" company="Starlight Software Co">
//    Copyright (c) Starlight Software Co. All rights reserved.
//    Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
// </copyright>
// ================================================================================


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

        public async override Task<TestResult[]> ExecuteAsync(ITestMethod testMethod)
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

            return await base.ExecuteAsync(testMethod);
        }

        private IEnumerable<IgnoreIfAttribute> FindAttributes(ITestMethod testMethod)
        {
            // Look for an [IgnoreIf] on the method, including any virtuals this method overrides
            var ignoreAttributes = new List<IgnoreIfAttribute>();
            ignoreAttributes.AddRange(testMethod.GetAttributes<IgnoreIfAttribute>());

            var type = testMethod.MethodInfo.ReflectedType;
            ignoreAttributes.AddRange(type.GetCustomAttributes<IgnoreIfAttribute>(inherit: false));

            // Walk the class hierarchy looking for an [IgnoreIf] attribute
            // This includes walking up the inheritance chain (base classes)
            if (testMethod.MethodInfo.DeclaringType != type)
            {
                type = testMethod.MethodInfo.DeclaringType;
                ignoreAttributes.AddRange(type.GetCustomAttributes<IgnoreIfAttribute>(inherit: true));
            }

            return ignoreAttributes;
        }
    }
}
