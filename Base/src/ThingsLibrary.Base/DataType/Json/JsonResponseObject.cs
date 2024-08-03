using System.Net;

namespace ThingsLibrary.DataType.Json
{
    /// <summary>
    /// Create a standardized error response that can be seen by a user as well as processed by backend systems.
    /// </summary>
    /// <example>
    ///{
    ///    "type": "https://tools.ietf.org/html/rfc7231#section-6.5.1",
    ///    "title": "One or more validation errors occurred.",
    ///    "status": 400,
    ///    "traceId": "00-4204121a29a8b9a00530169dfcb3994a-4e55e621efeebd22-00",
    ///    "isError": true,
    ///    "errors": {
    ///        "Key": [
    ///            "The Key field is required."
    ///        ],
    ///        "Title": [
    //            "The Title field is required."
    ///        ]
    ///    }
    ///}
    /// </example>
    [DisplayName("JsonResponse")]
    [DebuggerDisplay("StatusCode = {StatusCode}, Title = {Title}")]
    public class JsonResponse<TEntity> : JsonResponse
    {
        [JsonPropertyName("data")]
        public TEntity? Data { get; set; }
                
        public JsonResponse() 
        { 
            //nothing
        }

        /// <summary>
        /// Create Json Response
        /// </summary>
        /// <param name="data">Data</param>        
        public JsonResponse(TEntity data, string title = "Success")
        {
            var type = typeof(TEntity);
            this.Type = $"{type.Namespace}.{type.Name}";
            
            this.StatusCode = HttpStatusCode.OK;
            this.Title = title;

            this.Data = data;
        }

        /// <summary>
        /// Create Json Response
        /// </summary>
        /// <param name="statusCode"><see cref="HttpStatusCode"/></param>
        /// <param name="title">User Friendly Title</param>
        public JsonResponse(HttpStatusCode statusCode, string title, string? errorMessage = null)
        {
            this.StatusCode = statusCode;
            this.Title = title;

            this.ErrorMessage = errorMessage ?? string.Empty;
        }

        /// <summary>
        /// Create a Json Response
        /// </summary>
        /// <param name="statusCode"><see cref="HttpStatusCode"/></param>
        /// <param name="title">User Friendly Title</param>
        /// <param name="errorFieldName">Field/property that is causing this message</param>
        /// <param name="fieldErrorMessage">Error message</param>
        public JsonResponse(HttpStatusCode statusCode, string title, string errorFieldName, string fieldErrorMessage)
        {
            this.StatusCode = statusCode;
            this.Title = title;

            this.Errors.Add(errorFieldName, fieldErrorMessage);
        }

        /// <summary>
        /// Create json response based on exception
        /// </summary>
        /// <param name="exception"></param>
        public JsonResponse(Exception exception)
        {
            this.StatusCode = HttpStatusCode.InternalServerError;
            this.Title = exception.Message;
            this.ErrorDetails = exception.ToString();            
        }

        /// <summary>
        /// Create Json Response based on validation results
        /// </summary>
        /// <param name="results">Validation Results</param>
        public JsonResponse(ICollection<ValidationResult> results)
        {
            // nothing to see here folks
            if (results.Count == 0) { return; }

            this.StatusCode = HttpStatusCode.BadRequest;
            this.Title = "Validation errors occurred.";

            this.Errors = [];
            foreach (var result in results)
            {
                this.Errors.Add(string.Join(';', result.MemberNames), result.ErrorMessage ?? "Validation Error");
            }
        }

        /// <summary>
        /// Create Json Response based on validation results
        /// </summary>
        /// <param name="validationResults">Validation Results</param>
        /// <param name="statusCode">Status Code to use if validation errors have occured</param>
        public JsonResponse(Dictionary<string, string> validationResults, HttpStatusCode statusCode = HttpStatusCode.BadRequest)
        {
            this.StatusCode = statusCode;

            if (validationResults.Count > 0) 
            {
                this.Title = "Validation errors occurred.";
                this.Errors = validationResults;
            }            
        }

        /// <summary>
        /// Initialize using json response object
        /// </summary>
        /// <param name="jsonResponse"></param>
        public JsonResponse(JsonResponse jsonResponse)
        {
            this.Type = jsonResponse.Type;
            this.StatusCode = jsonResponse.StatusCode;
            this.Title = jsonResponse.Title;

            this.ErrorDetails = jsonResponse.ErrorDetails;
            this.Errors = jsonResponse.Errors;

            this.TraceId = jsonResponse.TraceId;
        }
    }
}