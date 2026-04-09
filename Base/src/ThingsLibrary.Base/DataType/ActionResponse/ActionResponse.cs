// ================================================================================
// <copyright file="ActionResponse.cs" company="Starlight Software Co">
//    Copyright (c) Starlight Software Co. All rights reserved.
//    Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
// </copyright>
// ================================================================================

using System.Net;

using ThingsLibrary.DataType.Extensions;
using ThingsLibrary.DataType.Json.Converters;

namespace ThingsLibrary.DataType
{
    /// <summary>
    /// Create a standarized error response that can be seen by a user as well as processed by backend systems.
    /// </summary>
    /// <example>
    ///{
    ///    "type": "https://tools.ietf.org/html/rfc7231#section-6.5.1",
    ///    "message": "One or more validation errors occurred.",
    ///    "status": 400,
    ///    "traceId": "00-4204121a29a8b9a00530169dfcb3994a-4e55e621efeebd22-00",
    ///    "isError": true,
    ///    "isSuccess": false,    
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
    [DisplayName("ActionResponse")]
    [DebuggerDisplay("StatusCode = {StatusCode}, DisplayMessage = {DisplayMessage}")]
    public class ActionResponse
    {
        /// <summary>
        /// Data type of the 'data' field if present
        /// </summary>
        [JsonPropertyName("type"), JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
        public string Type { get; init; } = "System.String"; //unless otherwise specified

        /// <summary>
        /// Http status code of the response
        /// </summary>
        [JsonPropertyName("status")]
        public HttpStatusCode StatusCode { get; set; } = HttpStatusCode.OK;

        /// <summary>
        /// User friendly message of what happened
        /// </summary>
        [JsonPropertyName("message"), JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
        public string DisplayMessage { get; set; } = string.Empty;

        /// <summary>
        /// The request trace ID
        /// </summary>
        [JsonPropertyName("traceId"), JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
        public string TraceId { get; init; } = Activity.Current?.Id ?? string.Empty;

        /// <summary>
        /// Standardized key/code system for easy understanding of message (which could be translated, etc).. 
        /// </summary>
        /// <remarks>Codes might look like:  STATION_NOT_FOUND, STATION_ALREADY_EXISTS</remarks>
        [JsonPropertyName("eventCode"), JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
        public string EventCode { get; init; } = string.Empty;

        /// <summary>
        /// Error listing with key that could tie to a field on the provided DTO data model (customer/user readible)
        /// </summary>
        [JsonPropertyName("errors"), JsonIgnoreEmptyCollection]
        public Dictionary<string, string> Errors { get; init; } = [];
                
        /// <summary>
        /// Machine exception details
        /// </summary>
        [JsonPropertyName("exception")]
        public ActionException? Exception { get; set; }

        /// <summary>
        /// If the response is in a error state
        /// </summary>
        [JsonPropertyName("isError"), JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
        public bool IsError => !this.IsSuccessStatusCode;

        /// <summary>
        /// Simple check on the response status
        /// </summary>
        [JsonPropertyName("isSuccess")]
        public bool IsSuccessStatusCode
        {
            get { return ((int)this.StatusCode >= 200) && ((int)this.StatusCode <= 299); }
        }

        /// <summary>
        /// Json Response
        /// </summary>
        public ActionResponse() 
        { 
            //nothing
        }

        /// <summary>
        /// Create Json Response
        /// </summary>        
        /// <param name="title">User Friendly Title</param>
        public ActionResponse(string title)
        {            
            this.DisplayMessage = title;
        }

        /// <summary>
        /// Create Json Response
        /// </summary>
        /// <param name="statusCode"><see cref="System.Net.HttpStatusCode"/></param>
        /// <param name="displayMessage">User Friendly Title</param>
        public ActionResponse(HttpStatusCode statusCode, string displayMessage, string? eventCode = null)
        {
            this.StatusCode = statusCode;
            this.DisplayMessage = displayMessage;
            
            if(eventCode != null)
            {
                this.EventCode = eventCode;
            }
            else if (this.IsSuccessStatusCode)
            {
                this.EventCode = "success";
            }
            //if (!string.IsNullOrEmpty(errorMessage))
            //{
            //    this.Exception = new ActionException(errorMessage);
            //}
        }

        /// <summary>
        /// Create a Json Response
        /// </summary>
        /// <param name="statusCode"><see cref="System.Net.HttpStatusCode"/></param>
        /// <param name="displayMessage">User Friendly Title</param>
        /// <param name="errorFieldName">Field/property that is causing this message</param>
        /// <param name="fieldErrorMessage">Error message</param>
        public ActionResponse(HttpStatusCode statusCode, string displayMessage, string errorFieldName, string fieldErrorMessage)
        {
            this.StatusCode = statusCode;
            this.DisplayMessage = displayMessage;

            this.Errors.Add(errorFieldName, fieldErrorMessage);
        }

        /// <summary>
        /// Create json response based on exception
        /// </summary>
        /// <param name="exception"></param>
        public ActionResponse(Exception exception, string? displayMessage = null, string? eventCode = null)
        {
            this.StatusCode = HttpStatusCode.InternalServerError;
            this.DisplayMessage = (string.IsNullOrEmpty(displayMessage) ? exception.Message : displayMessage);

            this.Exception = new ActionException(exception);
            this.Errors.Add("exception", exception.Message);

            if (!string.IsNullOrEmpty(eventCode))
            {
                this.EventCode = eventCode;
            }
        }

        /// <summary>
        /// Create json response based on exception
        /// </summary>
        /// <param name="exception"></param>
        public ActionResponse(ActionException exception, string? displayMessage = null, string? eventCode = null)
        {
            this.StatusCode = HttpStatusCode.InternalServerError;
            this.DisplayMessage = (string.IsNullOrEmpty(displayMessage) ? exception.Message : displayMessage);

            this.Exception = exception;
            this.Errors.Add("exception", exception.Message);

            if (!string.IsNullOrEmpty(eventCode))
            {
                this.EventCode = eventCode;
            }
        }

        /// <summary>
        /// Create Json Response based on validation results
        /// </summary>
        /// <param name="results">Validation Results</param>
        public ActionResponse(ICollection<ValidationResult> results, string displayMessage = "One or more validation errors occurred.")
        {
            if (results == null) { return; }
            if (results.Count == 0) { return; }

            this.StatusCode = HttpStatusCode.BadRequest;
            this.DisplayMessage = displayMessage;

            // show the first error as the main error.   
            this.Errors = [];
            results.ForEach(result => this.Errors.Add(string.Join(';', result.MemberNames), $"{result.ErrorMessage}"));
        }        
    }
}