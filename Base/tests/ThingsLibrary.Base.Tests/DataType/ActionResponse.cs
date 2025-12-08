// ================================================================================
// <copyright file="ActionResponse.cs" company="Starlight Software Co">
//    Copyright (c) 2025 Starlight Software Co. All rights reserved.
//    Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
// </copyright>
// ================================================================================

using System.Net;

namespace ThingsLibrary.Base.Tests.DataType
{
    [TestClass, ExcludeFromCodeCoverage]
    public class JsonResponseTests
    {
        [TestMethod]
        public void Constructor()
        {
            var response = new ThingsLibrary.DataType.ActionResponse<int>()
            {
                Type = "SomeType",
                DisplayMessage = "Display Message",
                StatusCode = System.Net.HttpStatusCode.Created,
                TraceId = "TraceId",

                EventCode = "Error.Missing.Something",
                Errors = new Dictionary<string, string>
                {
                    { "Bad1", "Bad Something" },
                    { "Bad2", "Bad Something Also" }
                }
            };

            response.Errors.Add("TestError", "Test Error Here");

            Assert.AreEqual("SomeType", response.Type);
            Assert.AreEqual("Display Message", response.DisplayMessage);
            Assert.AreEqual(System.Net.HttpStatusCode.Created, response.StatusCode);
            Assert.AreEqual("TraceId", response.TraceId);
            Assert.AreEqual("Error.Missing.Something", response.EventCode);

            Assert.AreEqual("Bad1", response.Errors.First().Key);
        }

        [TestMethod]
        public void Constructor_Parameters()
        {
            var response = new ThingsLibrary.DataType.ActionResponse<int>(HttpStatusCode.IMUsed, "Test Message", "test_message");

            Assert.AreEqual(HttpStatusCode.IMUsed, response.StatusCode);
            Assert.AreEqual("Test Message", response.DisplayMessage);
            Assert.AreEqual("test_message", response.EventCode);
        }

        [TestMethod]
        public void Constructor_Error()
        {
            var ex = new Exception("Test Exception");

            var response = new ThingsLibrary.DataType.ActionResponse<int>(ex);

            Assert.AreEqual(System.Net.HttpStatusCode.InternalServerError, response.StatusCode);
            Assert.AreEqual("Test Exception", response.DisplayMessage);
            Assert.IsTrue(response.Errors.Count > 0);
        }

        [TestMethod]
        public void Constructor_ValidationErrors()
        {
            var errors = new List<ValidationResult>()
            {
                new ValidationResult("Bad Something", new List<string>{ "Bad1" }),
                new ValidationResult("Bad Something Also", new List<string>{ "Bad2" })
            };

            var response = new ThingsLibrary.DataType.ActionResponse<int>(errors);

            Assert.AreEqual(System.Net.HttpStatusCode.BadRequest, response.StatusCode);
            Assert.AreEqual("One or more validation errors occurred.", response.DisplayMessage);
            Assert.AreEqual(2, response.Errors.Count);
        }
    }
}
