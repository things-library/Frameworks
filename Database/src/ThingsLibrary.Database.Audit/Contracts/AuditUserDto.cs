// ================================================================================
// <copyright file="AuditUserDto.cs" company="Starlight Software Co">
//    Copyright (c) Starlight Software Co. All rights reserved.
//    Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
// </copyright>
// ================================================================================

namespace ThingsLibrary.Database.Audit.Contracts
{
    /// <summary>
    /// Audit User
    /// </summary>
    [System.Diagnostics.DebuggerDisplay("{FullName} ({Username})")]
    public class AuditUserDto
    {
        /// <summary>
        /// ID
        /// </summary>
        public Guid Id { get; set; }

        /// <summary>
        /// Username
        /// </summary>
        [JsonPropertyName("username")]
        public string Username { get; set; } = string.Empty;

        /// <summary>
        /// This is the unique username (Claim: 'name') of who caused the event
        /// </summary>
        [JsonPropertyName("name")]        
        public string? FullName { get; set; }

        /// <summary>
        /// Constructor
        /// </summary>
        public AuditUserDto()
        {
            //nothing
        }
    }    
}