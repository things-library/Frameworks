﻿// ================================================================================
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
                // Search for the method specified by name in this class or any parent classes.
                var searchFlags = BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.FlattenHierarchy | BindingFlags.Static;
                var method = testMethod.MethodInfo.DeclaringType?.GetMethod(this.IgnoreCriteriaMethodName, searchFlags);
                
                if(method == null) { return false; }

                return (bool)(method.Invoke(null, null) ?? false);
            }
            catch (Exception e)
            {
                var message = $"Conditional ignore method {this.IgnoreCriteriaMethodName} not found. Ensure the method is in the same class as the test method, marked as 'static', returns a 'bool', and doesn't accept any parameters.";
                throw new ArgumentException(message, e);
            }
        }
    }
}
