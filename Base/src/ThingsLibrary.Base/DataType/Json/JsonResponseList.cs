// ================================================================================
// <copyright file="JsonResponseList.cs" company="Starlight Software Co">
//    Copyright (c) Starlight Software Co. All rights reserved.
//    Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
// </copyright>
// ================================================================================

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
    public class JsonResponseList<TEntity> : JsonResponse
    {
        #region --- PagedList ---

        /// <summary>
        /// Total Items Found
        /// </summary>
        [JsonPropertyName("count")]
        [DisplayName("Item Count"), Display(Name = "Count"), Range(0, int.MaxValue), ReadOnly(true)]
        public int Count { get; init; }

        /// <summary>
        /// Current Page Number (0 based)
        /// </summary>
        [JsonPropertyName("page")]
        [Display(Name = "Current Page", Description = "Current page of results.", Prompt = "Current Page"), Range(0, int.MaxValue)]
        public int CurrentPage { get; init; }

        /// <summary>
        /// Total Pages
        /// </summary>
        [JsonPropertyName("totalPages")]
        [Display(Name = "Total Pages")]
        public int TotalPages => (this.PageSize > 0 ? (int)System.Math.Ceiling((double)this.Count / this.PageSize) : 0);

        /// <summary>
        /// Size of each page (0 = no item limit)
        /// </summary>
        [JsonPropertyName("pageSize")]
        [Display(Name = "Page Size"), Range(0, int.MaxValue)]
        public int PageSize { get; init; }


        /// <summary>
        /// Links
        /// </summary>
        /// <remarks>Provides a way to keep state and allow for a 'next page', 'previous page' type linking structure</remarks>
        [JsonPropertyName("links")]
        public Dictionary<string, object> Links { get; set; } = [];

        #endregion

        /// <summary>
        /// List of Items
        /// </summary>
        [JsonPropertyName("items")]
        [Display(Name = "Items"), Required]
        public IEnumerable<TEntity> Data { get; init; } = [];

               
        /// <summary>
        /// Json List Response
        /// </summary>
        public JsonResponseList() 
        { 
            //nothing
        }

        /// <summary>
        /// Convert json response to JsonResponseList
        /// </summary>
        /// <param name="jsonResponse"></param>
        public JsonResponseList(JsonResponse jsonResponse)
        {
            this.Type = jsonResponse.Type;
            this.StatusCode = jsonResponse.StatusCode;
            this.Title = jsonResponse.Title;

            this.ErrorDetails = jsonResponse.ErrorDetails;
            this.Errors = jsonResponse.Errors;

            this.TraceId = jsonResponse.TraceId;
        }

        /// <summary>
        /// Takes a partial data collection
        /// </summary>
        /// <param name="page">Current Page</param>
        /// <param name="pageSize">Page Size</param>
        /// <param name="count">Total Items In Set</param>
        /// <param name="data">Collection of items for the page</param>
        public JsonResponseList(IEnumerable<TEntity> data, int page, int pageSize, int count)
        {
            var type = typeof(TEntity);
            this.Type = $"{type.Namespace}.{type.Name}";

            this.CurrentPage = page;
            this.PageSize = pageSize;
            this.Count = count;
            this.Data = data;
        }

        /// <summary>
        /// Takes the entire dataset results
        /// </summary>
        /// <param name="data">Collection of Items</param>
        public JsonResponseList(IEnumerable<TEntity> data)
        {
            var type = typeof(TEntity);
            this.Type = $"{type.Namespace}.{type.Name}";

            this.CurrentPage = 0;
            this.PageSize = data.Count();
            this.Count = data.Count();
            this.Data = data;
        }

        /// <summary>
        /// Create Json Response
        /// </summary>
        /// <param name="statusCode"><see cref="HttpStatusCode"/></param>
        /// <param name="title">User Friendly Title</param>
        public JsonResponseList(HttpStatusCode statusCode, string title, string? errorMessage = null)
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
        public JsonResponseList(HttpStatusCode statusCode, string title, string errorFieldName, string fieldErrorMessage)
        {
            this.StatusCode = statusCode;
            this.Title = title;

            this.Errors.Add(errorFieldName, fieldErrorMessage);
        }

        /// <summary>
        /// Create json response based on exception
        /// </summary>
        /// <param name="exception"></param>
        public JsonResponseList(Exception exception)
        {
            this.StatusCode = HttpStatusCode.InternalServerError;
            this.Title = exception.Message;
            this.ErrorDetails = exception.ToString();            
        }

        /// <summary>
        /// Create Json Response based on validation results
        /// </summary>
        /// <param name="results">Validation Results</param>
        public JsonResponseList(ICollection<ValidationResult> results)
        {
            // nothing to see folks
            if (results == null) { return; }
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
        public JsonResponseList(Dictionary<string, string> validationResults, HttpStatusCode statusCode = HttpStatusCode.BadRequest)
        {
            this.StatusCode = statusCode;

            if (validationResults.Count > 0) 
            {
                this.Title = "Validation errors occurred.";
                this.Errors = validationResults;
            }            
        }        
    }
}