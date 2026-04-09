// ================================================================================
// <copyright file="ActionException.cs" company="Starlight Software Co">
//    Copyright (c) Starlight Software Co. All rights reserved.
//    Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
// </copyright>
// ================================================================================

namespace ThingsLibrary.DataType
{
    /// <summary>
    /// Create a standarized error response that can be seen by a user as well as processed by backend systems.
    /// </summary>        
    [DebuggerDisplay("Message: {Message}")]
    public class ActionException
    {
        /// <summary>
        /// Error message not intended for user/customers
        /// </summary>
        [JsonPropertyName("message"), JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
        public string Message { get; init; } = string.Empty;

        /// <summary>
        /// Stack trace of the exception
        /// </summary>
        [JsonPropertyName("trace"), JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
        public string StackTrace { get; set; } = string.Empty;

        [JsonPropertyName("inner_exception"), JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public ActionException? InnerException { get; set; }


        /// <summary>
        /// Constructor
        /// </summary>
        public ActionException()
        {
            //nothing
        }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="message">Error Message not intended for a user/customer</param>
        public ActionException(string message)
        {
            this.Message = message;
        }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="ex">Exception</param>
        public ActionException(Exception ex)
        {
            this.Message = ex.Message;
            this.StackTrace = ex.StackTrace ?? string.Empty;
            if(ex.InnerException != null)
            {
                this.InnerException = new ActionException(ex.InnerException);
            }
        }
    }
}