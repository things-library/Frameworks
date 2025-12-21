// ================================================================================
// <copyright file="XmlElement.cs" company="Starlight Software Co">
//    Copyright (c) 2025 Starlight Software Co. All rights reserved.
//    Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
// </copyright>
// ================================================================================

using System.Xml;

namespace ThingsLibrary.Tests.DataType.Extensions
{

    [TestClass, ExcludeFromCodeCoverage]
    public class XmlElementTests
    {
        public static XmlElement TestElement;

        [ClassInitialize]
        public static void ClassInitialize(TestContext testContext)
        {
            TestElement = XmlDocumentTests.GetTestDocument().DocumentElement;
        }

        [TestMethod]
        [DataRow("user/id", "12345")]
        [DataRow("user/name", "Test")]
        [DataRow("user/lastname", "Last")]
        [DataRow("user/contact/address/city", "Test City")]
        [DataRow("user/contact/phone", "555-555-5555")]
        [DataRow("user/MISSING_NODE", "[DEFAULT]")]
        [DataRow("user/MISSING_NODE/name", "[DEFAULT]")]
        public void GetPropertyString(string propertyKey, string expectedValue)
        {
            Assert.AreEqual(expectedValue, TestElement.GetProperty<string>(propertyKey, "[DEFAULT]"));
        }

        [TestMethod]
        [DataRow("user/createdOn", "2000-01-02T03:04:05.0", "System.DateTime")]
        [DataRow("id", "2", "System.Int32")]
        [DataRow("user/contact/address/zip", "50263", "System.Int32")]
        [DataRow("user/contact/MISSING_NODE/zip", "0", "System.Int32")]
        public void GetPropertyGenerics(string propertyKey, string expectedValue, string typeStr)
        {
            var returnType = Type.GetType(typeStr);
            var expectedResult = Convert.ChangeType(expectedValue, returnType);

            // Get the generic method
            var genericGetMethod = typeof(XmlElementExtensions).GetMethod("GetProperty");

            // Make the non-generic method via the 'MakeGenericMethod' reflection call.            
            var typedGetMethod = genericGetMethod.MakeGenericMethod(new[] { returnType });

            // Invoke the method just like a normal method.
            var result = typedGetMethod.Invoke(new XmlDocument().DocumentElement, new object[] { TestElement, propertyKey, null });

            Assert.AreEqual(expectedResult, result);
        }

        [TestMethod]
        public void GetProperty_NullPath()
        {
            Assert.Throws<ArgumentNullException>(() => TestElement.GetProperty<string>(null, "[DEFAULT]"));

            Assert.Throws<ArgumentNullException>(() => XmlElementExtensions.GetProperty<string>((XmlElement)null, "[DEFAULT]", string.Empty));
        }
    }
}

