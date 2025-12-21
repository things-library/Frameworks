// ================================================================================
// <copyright file="DbConnectionStringBuilder.cs" company="Starlight Software Co">
//    Copyright (c) 2025 Starlight Software Co. All rights reserved.
//    Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
// </copyright>
// ================================================================================

namespace ThingsLibrary.Tests.DataType.Extensions
{
    [TestClass, ExcludeFromCodeCoverage]
    public class DbConnectionStringBuilderTests
    {
        [TestMethod]
        [DataRow("param1=hello;param2=2000-01-02T03:04:05.0;param3=123", "param1", "hello", "System.String")]
        [DataRow("param1=hello;param2=2000-01-02T03:04:05.0;param3=123", "param2", "2000-01-02T03:04:05.0", "System.DateTime")]
        [DataRow("param1=hello;param2=2000-01-02T03:04:05.0;param3=123", "param3", "123", "System.Int32")]
        [DataRow("param1=hello;param2=2000-01-02T03:04:05.0;param3=123", "MISSING", null, "System.String")]
        [DataRow("param1=hello;param2=2000-01-02T03:04:05.0;param3=123", "MISSING", "001-01-01T00:00:00.0", "System.DateTime")]
        [DataRow("param1=hello;param2=2000-01-02T03:04:05.0;param3=123", "MISSING", "0", "System.Int32")]
        
        public void GetPropertyGenerics(string connectionString, string propertyKey, string expectedValue, string typeStr)
        {
            if(expectedValue == "") { expectedValue = null; }

            var returnType = Type.GetType(typeStr);
            var expectedResult = Convert.ChangeType(expectedValue, returnType);

            var builder = new DbConnectionStringBuilder();
            builder.ConnectionString = connectionString;

            // Get the generic method
            var genericGetMethod = typeof(DbConnectionStringExtensions).GetMethod("GetValue");

            // Make the non-generic method via the 'MakeGenericMethod' reflection call.            
            var typedGetMethod = genericGetMethod.MakeGenericMethod(new[] { returnType });

            //// Invoke the method just like a normal method.
            /// INVOKE: public static T GetProperty<T>(this JsonElement rootElement, string propertyPath, T defaultValue = default)
            var result = typedGetMethod.Invoke(new JsonElement(), new object[] { builder, propertyKey, null });

            Assert.AreEqual(expectedResult, result);
        }

        [TestMethod]        
        public void GetValue_Null()
        {
            var builder = new DbConnectionStringBuilder();

            Assert.Throws<ArgumentException>(() => builder.GetValue<string>("Any", string.Empty));
            
        }
    }
}