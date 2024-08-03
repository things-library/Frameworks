namespace ThingsLibrary.Tests.Json
{
    [TestClass, ExcludeFromCodeCoverage]
    public class JsonResponseTests
    {
        [TestMethod]
        public void Constructor()
        {
            var response = new ThingsLibrary.DataType.Json.JsonResponse<int>()
            {
                Type = "SomeType",
                Title = "Title",
                StatusCode = System.Net.HttpStatusCode.Created,
                TraceId = "TraceId",
                ErrorDetails = "ErrorDetails",
                Errors = new Dictionary<string, string>
                {
                    { "Bad1", "Bad Something" },
                    { "Bad2", "Bad Something Also" }
                }
            };

            response.Errors.Add("TestError", "Test Error Here");

            Assert.AreEqual("SomeType", response.Type);
            Assert.AreEqual("Title", response.Title);
            Assert.AreEqual(System.Net.HttpStatusCode.Created, response.StatusCode);
            Assert.AreEqual("TraceId", response.TraceId);
            Assert.AreEqual("ErrorDetails", response.ErrorDetails);

            Assert.AreEqual("Bad1", response.Errors.First().Key);
        }

        [TestMethod]
        public void Constructor_Parameters()
        {
            var response = new ThingsLibrary.DataType.Json.JsonResponse<int>(System.Net.HttpStatusCode.IMUsed, "Test Title");

            Assert.AreEqual(System.Net.HttpStatusCode.IMUsed, response.StatusCode);
            Assert.AreEqual("Test Title", response.Title);
        }

        [TestMethod]
        public void Constructor_Error()
        {
            var ex = new Exception("Test Exception");

            var response = new ThingsLibrary.DataType.Json.JsonResponse<int>(ex);

            Assert.AreEqual(System.Net.HttpStatusCode.InternalServerError, response.StatusCode);
            Assert.AreEqual("Test Exception", response.Title);
            Assert.IsTrue(!string.IsNullOrEmpty(response.ErrorDetails));
        }

        [TestMethod]
        public void Constructor_ValidationErrors()
        {
            var errors = new List<ValidationResult>()
            {
                new ValidationResult("Bad Something", new List<string>{ "Bad1" }),
                new ValidationResult("Bad Something Also", new List<string>{ "Bad2" })
            };

            var response = new ThingsLibrary.DataType.Json.JsonResponse<int>(errors);

            Assert.AreEqual(System.Net.HttpStatusCode.BadRequest, response.StatusCode);
            Assert.AreEqual("Validation errors occurred.", response.Title);
            Assert.AreEqual(2, response.Errors.Count);
        }
    }
}
