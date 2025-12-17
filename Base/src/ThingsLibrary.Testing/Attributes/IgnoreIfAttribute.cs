// ================================================================================
// <copyright file="IgnoreIfAttribute.cs" company="Starlight Software Co">
//    Copyright (c) Starlight Software Co. All rights reserved.
//    Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
// </copyright>
// ================================================================================

namespace ThingsLibrary.Testing.Attributes
{
    // Source: https://matt.kotsenas.com/posts/ignoreif-mstest

    /// <summary>
    /// An extension to the [Ignore] attribute. Instead of using test lists / test categories to conditionally skip tests, allow a [TestClass] or [TestMethod] 
    /// to specify a method to run. If the method returns 'true' the test method will be skipped. The "ignore criteria" method must be 'static', return a single
    /// 'bool' value, and not accept any parameters. By default, it is named "IgnoreIf".
    /// </summary>
    /// <remarks>
    /// Source: https://matt.kotsenas.com/posts/ignoreif-mstest
    /// </remarks>
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, AllowMultiple = false, Inherited = true)]
    public class IgnoreIfAttribute : Attribute
    {
        public string IgnoreCriteriaMethodName { get; set; }

        public IgnoreIfAttribute(string ignoreCriteriaMethodName = "IgnoreIf")
        {
            this.IgnoreCriteriaMethodName = ignoreCriteriaMethodName;
        }

        internal bool ShouldIgnore(ITestMethod testMethod)
        {
            try
            {
                var declaringType = testMethod.MethodInfo.DeclaringType;
                if (declaringType == null) { return false; }

                // Search for the method specified by name in this class or any parent classes.
                // Note: Using NonPublic binding flags to allow access to private/internal methods
                // This is intentional for test infrastructure purposes
                var searchFlags = BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.FlattenHierarchy | BindingFlags.Static;
                var method = declaringType.GetMethod(this.IgnoreCriteriaMethodName, searchFlags);

                // If no method found, try to find a property with the same name
                if (method != null)
                {
                    // Validate the method signature
                    if (method.ReturnType != typeof(bool))
                    {
                        throw new ArgumentException($"Conditional ignore method '{this.IgnoreCriteriaMethodName}' must return a bool.");
                    }

                    if (method.GetParameters().Length > 0)
                    {
                        throw new ArgumentException($"Conditional ignore method '{this.IgnoreCriteriaMethodName}' must not have parameters.");
                    }

                    return (bool)(method.Invoke(null, null) ?? false);
                }
                else // see if it is a property instead of a bool method
                { 
                    var property = declaringType.GetProperty(this.IgnoreCriteriaMethodName, searchFlags);
                    
                    if (property != null)
                    {
                        // Validate the property signature
                        if (property.PropertyType != typeof(bool))
                        {
                            throw new ArgumentException($"Conditional ignore property '{this.IgnoreCriteriaMethodName}' must return a bool.");
                        }

                        if (!property.CanRead)
                        {
                            throw new ArgumentException($"Conditional ignore property '{this.IgnoreCriteriaMethodName}' must have a getter.");
                        }

                        return (bool)(property.GetValue(null) ?? false);
                    }
                    
                    return false;
                }                
            }
            catch (Exception e)
            {
                var message = $"Conditional ignore method or property '{this.IgnoreCriteriaMethodName}' not found or invalid. Ensure the method/property is in the same class as the test method, marked as 'static', returns a 'bool', and (for methods) doesn't accept any parameters.";
                throw new ArgumentException(message, e);
            }
        }
    }
}
