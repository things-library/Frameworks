// ================================================================================
// <copyright file="HealthCheckException.cs" company="Starlight Software Co">
//    Copyright (c) Starlight Software Co. All rights reserved.
//    Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
// </copyright>
// ================================================================================

namespace ThingsLibrary.Services.HealthChecks
{
    /// <summary>
    /// Exception occurred during a health check event
    /// </summary>
    public class HealthCheckException : Exception
    {
        /// <summary>
        /// Exception occurred during a health check event
        /// </summary>
        public HealthCheckException()
        {
            //nothing
        }

        /// <summary>
        /// Exception occurred during a health check event
        /// </summary>
        /// <param name="message">Error Message</param>
        public HealthCheckException(string message) : base(message)
        {
            //nothing
        }

        /// <summary>
        /// Exception occurred during a health check event
        /// </summary>
        /// <param name="message">Error Message</param>
        /// <param name="inner">Inner Exception</param>
        public HealthCheckException(string message, Exception inner) : base(message, inner)
        {
            //nothing
        }
    }
}

