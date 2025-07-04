// ================================================================================
// <copyright file="AuditEventDto.cs" company="Starlight Software Co">
//    Copyright (c) Starlight Software Co. All rights reserved.
//    Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
// </copyright>
// ================================================================================

namespace ThingsLibrary.Database.Audit.Contracts
{    
    /// <summary>
    /// Audit Event
    /// </summary>
    public class AuditEventDto
    {
        /// <summary>
        /// Sequence Id
        /// </summary>
        [JsonPropertyName("seq")]        
        public int SequenceId { get; init; } = default;

        /// <summary>
        /// Event Type (Deleted, Created, Edited)
        /// </summary>   
        [JsonPropertyName("type")]        
        public string EventType { get; init; } = string.Empty;   //C = Create, U = update, D = delete, X = Undelete

        /// <summary>
        /// Old values that got changed
        /// </summary>
        [JsonPropertyName("newValues")]
        public Dictionary<string, object?> NewValues { get; init; } = new Dictionary<string, object?>();


        /// <summary>
        /// Old values that got changed
        /// </summary>
        [JsonPropertyName("oldValues")]
        public Dictionary<string, object?> OldValues { get; init; } = new Dictionary<string, object?>();

        /// <summary>
        /// Event Occured On (timestamp)
        /// </summary>
        [JsonPropertyName("eventDate")]        
        public DateTimeOffset EventOn { get; init; }

        /// <summary>
        /// Audit User
        /// </summary>
        [JsonPropertyName("auditUser")]        
        public AuditUserDto? AuditUser { get; init; }


        /// <summary>
        /// Constructor
        /// </summary>
        public AuditEventDto()
        {
            //nothing
        }
    }    
}