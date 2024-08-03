using ThingsLibrary.DataType.Extensions;
using ThingsLibrary.DataType.Json.Converters;
using System.Net;

namespace ThingsLibrary.DataType.Json
{
    /// <summary>
    /// Create a standarized error response that can be seen by a user as well as processed by backend systems.
    /// </summary>
    /// <example>
    ///{
    ///    "type": "https://tools.ietf.org/html/rfc7231#section-6.5.1",
    ///    "title": "One or more validation errors occurred.",
    ///    "status": 400,
    ///    "traceId": "00-4204121a29a8b9a00530169dfcb3994a-4e55e621efeebd22-00",
    ///    "isError": true,
    ///    "isSuccess": false,
    ///    "errorMessage": "Division by zero error in queue record 1451",
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
    public class JsonResponse
    {
        /// <summary>
        /// Data type of the response
        /// </summary>
        [JsonPropertyName("type"), JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
        public string Type { get; init; } = string.Empty;

        /// <summary>
        /// User friendly message of what happened
        /// </summary>
        [JsonPropertyName("title"), JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
        public string Title { get; set; } = string.Empty;

        /// <summary>
        /// Http status code of the response
        /// </summary>
        [JsonPropertyName("status")]
        public HttpStatusCode StatusCode { get; set; } = HttpStatusCode.OK;

        /// <summary>
        /// The request trace ID
        /// </summary>
        [JsonPropertyName("traceId"), JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
        public string TraceId { get; init; } = Activity.Current?.Id ?? string.Empty;

        /// <summary>
        /// Standardized error key/code system for easy understanding of error message (which could be translated, etc).. 
        /// </summary>
        /// <remarks>Codes might look like:  STATION_NOT_FOUND, STATION_ALREADY_EXISTS</remarks>
        [JsonPropertyName("errorCode"), JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
        public string ErrorCode { get; init; } = string.Empty;

        /// <summary>
        /// Error listing with key that could tie to a field on the provided DTO data model
        /// </summary>
        [JsonPropertyName("errors"), JsonIgnoreEmptyCollection]
        public Dictionary<string, string> Errors { get; init; } = []; // new Dictionary<string, string>();

        /// <summary>
        /// Error message not meant for the user to see (aka: ex.Message)
        /// </summary>
        [JsonPropertyName("errorMessage"), JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
        public string ErrorMessage { get; set; } = string.Empty;

        /// <summary>
        /// Detailed error details such as stack trace (aka: ex.ToString())
        /// </summary>
        [JsonPropertyName("errorDetails"), JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
        public string ErrorDetails { get; init; } = string.Empty;

        /// <summary>
        /// If the response is in a error state
        /// </summary>
        [JsonPropertyName("isError"), JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]        
        public bool IsError => ((int)this.StatusCode < 200 || (int)this.StatusCode >= 300);


        [JsonPropertyName("isSuccess")]
        public bool IsSuccessStatusCode
        {
            get { return ((int)this.StatusCode >= 200) && ((int)this.StatusCode <= 299); }
        }

        /// <summary>
        /// Json Response
        /// </summary>
        public JsonResponse() 
        { 
            //nothing
        }

        /// <summary>
        /// Create Json Response
        /// </summary>        
        /// <param name="title">User Friendly Title</param>
        public JsonResponse(string title)
        {            
            this.Title = title;
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
            this.StatusCode = HttpStatusCode.BadRequest;

            this.Title = "One or more validation errors occurred.";

            if (results.Count > 0)
            {
                // show the first error as the main error.                
                this.Title = results.First().ErrorMessage ?? "Validation Error";
                
                this.Errors = new Dictionary<string, string>(results.Count);
                results.ForEach(result => this.Errors.Add(string.Join(';', result.MemberNames), $"{result.ErrorMessage}"));                
            }            
        }
    }
}