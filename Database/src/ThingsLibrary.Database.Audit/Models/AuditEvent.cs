// ================================================================================
// <copyright file="AuditEvent.cs" company="Starlight Software Co">
//    Copyright (c) Starlight Software Co. All rights reserved.
//    Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
// </copyright>
// ================================================================================

namespace ThingsLibrary.Database.Audit.Models
{
    /// <summary>
    /// Audit Type
    /// </summary>
    /// <remarks>Value is in order of security needed to perform operation</remarks>
    public enum AuditType : short
    {
        /// <summary>
        /// Unknown
        /// </summary>
        [Obsolete("Not intended to be used.")]
        Unknown = 0,

        /// <summary>
        /// Query
        /// </summary>
        Query = 100,

        /// <summary>
        /// Query History
        /// </summary>
        QueryHistory = 101,

        /// <summary>
        /// Created
        /// </summary>
        Create = 200,

        /// <summary>
        /// Updated
        /// </summary>
        Update = 300,

        /// <summary>
        /// Deleted
        /// </summary>
        Delete = 500,

        /// <summary>
        /// UnDeleted
        /// </summary>
        UnDelete = 510,  // rise zombie and continue!!!                
    }

    /// <summary>
    /// Audit Event
    /// </summary>
    [System.Diagnostics.DebuggerDisplay("{EntityName} ({EventType})")]
    [Attributes.Index(nameof(EntityId), nameof(Version), IsUnique = true)]
    [Attributes.Index(nameof(AuditUserId))]
    public class AuditEvent
    {
        /// <summary>
        /// Sequence Id
        /// </summary>    
        [Display(Name = "Sequence Id"), Required]
        public int SequenceId { get; init; } = default;

        /// <summary>
        /// Audit Event Id
        /// </summary>
        [Display(Name = "ID"), Key, Required]
        public Guid Id { get; set; }

        /// <summary>
        /// Entity Id
        /// </summary>        
        [Display(Name = "Entity ID"), Required]
        public Guid EntityId { get; set; }

        /// <summary>
        /// Entity Name
        /// </summary>
        [Display(Name = "Entity Name"), Required]
        public string EntityName { get; set; } = string.Empty;

        /// <summary>
        /// Version Number
        /// </summary>    
        [Display(Name = "Version Number"), Required]
        public int Version { get; set; } = 0;

        /// <summary>
        /// Trace Identifier (HttpContext.TraceIdentifier) for the specific request and response
        /// </summary>
        [Display(Name = "Trace Identifier"), StringLength(50)]
        public string? TraceId { get; set; }

        /// <summary>
        /// Event Type (Deleted, Created, Edited)
        /// </summary>
        [Display(Name = "Event Type"), Required]
        public AuditType EventType { get; set; }       //C = Create, U = update, D = delete

        /// <summary>
        /// This is the unique user id (claim: OID) of who caused the event
        /// </summary>
        [Display(Name = "Audit User ID"), Required]
        public Guid AuditUserId { get; set; }

        /// <summary>
        /// New values
        /// </summary>
        [Display(Name = "New Values"), Required]
        public Dictionary<string, object?> NewValues { get; set; } = new Dictionary<string, object?>();

        /// <summary>
        /// Old values
        /// </summary>
        [Display(Name = "Old Values"), Required]
        public Dictionary<string, object?> OldValues { get; set; } = new Dictionary<string, object?>();

        /// <summary>
        /// Event Occured On (timestamp)
        /// </summary>
        [Display(Name = "Event Date"), Required]
        public DateTime EventOn { get; set; } = DateTime.UtcNow;

        #region --- Foreign ---

        /// <summary>
        /// Audit User
        /// </summary>
        [ForeignKey("AuditUserId")]
        public virtual AuditUser? AuditUser { get; set; }

        #endregion


        /// <summary>
        /// Constructor
        /// </summary>
        public AuditEvent()
        {
            //nothing
        }
    }
}