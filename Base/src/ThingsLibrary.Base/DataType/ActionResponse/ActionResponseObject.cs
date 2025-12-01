// ================================================================================
// <copyright file="ActionResponseObject.cs" company="Starlight Software Co">
//    Copyright (c) Starlight Software Co. All rights reserved.
//    Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
// </copyright>
// ================================================================================

using System.Net;
using ThingsLibrary.DataType.Extensions;

namespace ThingsLibrary.DataType
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
    [DisplayName("ActionResponse")]
    [DebuggerDisplay("StatusCode = {StatusCode}, DisplayMessage = {DisplayMessage}")]
    public class ActionResponse<TEntity> : ActionResponse
    {
        [JsonPropertyName("data")]
        public TEntity Data { get; set; } = default;
                
        public ActionResponse() 
        { 
            //nothing
        }

        /// <summary>
        /// Create Json Response
        /// </summary>
        /// <param name="data">Data</param>        
        public ActionResponse(TEntity data, string displayMessage = "Success", string? eventCode = null) : base(HttpStatusCode.OK, displayMessage, eventCode)
        {
            var type = typeof(TEntity);
            this.Type = $"{type.Namespace}.{type.Name}";

            this.Data = data;

            if (!string.IsNullOrEmpty(eventCode))
            {
                this.EventCode = eventCode;
            }            
        }

        /// <summary>
        /// Create Json Response
        /// </summary>
        /// <param name="statusCode"><see cref="HttpStatusCode"/></param>
        /// <param name="title">User Friendly Title</param>
        public ActionResponse(HttpStatusCode statusCode, string displayMessage, string eventCode) : base(statusCode, displayMessage, eventCode)
        {
            //nothing
        }

        /// <summary>
        /// Create a Json Response
        /// </summary>
        /// <param name="statusCode"><see cref="HttpStatusCode"/></param>
        /// <param name="displayMessage">User Friendly Title</param>
        /// <param name="errorFieldName">Field/property that is causing this message</param>
        /// <param name="fieldErrorMessage">Error message</param>
        public ActionResponse(HttpStatusCode statusCode, string displayMessage, string errorFieldName, string fieldErrorMessage) : base(statusCode, displayMessage, errorFieldName, fieldErrorMessage)
        {
            //nothing
        }

        /// <summary>
        /// Create json response based on exception
        /// </summary>
        /// <param name="exception"></param>
        public ActionResponse(Exception exception, string? displayMessage = null) : base(exception, displayMessage)
        {
            //nothing
        }

        /// <summary>
        /// Create Json Response based on validation results
        /// </summary>
        /// <param name="results">Validation Results</param>
        public ActionResponse(ICollection<ValidationResult> results) : base(results)
        {
            //nothing
        }

        /// <summary>
        /// Initialize using json response object
        /// </summary>
        /// <param name="jsonResponse"></param>
        public ActionResponse(ActionResponse jsonResponse)
        {
            jsonResponse.CopyPropertyValues(this);
        }
    }
}