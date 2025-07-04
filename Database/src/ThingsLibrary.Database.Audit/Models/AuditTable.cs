// ================================================================================
// <copyright file="AuditTable.cs" company="Starlight Software Co">
//    Copyright (c) Starlight Software Co. All rights reserved.
//    Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
// </copyright>
// ================================================================================

namespace ThingsLibrary.Database.Audit.Models
{
    /// <summary>
    /// Audit Table
    /// </summary>    
    public abstract class AuditTable : ThingsLibrary.Interfaces.IId
    {
        /// <summary>
        /// ID
        /// </summary>
        [Key, Required, Display(Name = "ID")]
        public Guid Id { get; set; }

        /// <summary>
        /// Version Number
        /// </summary>
        /// <remarks>(0 = not saved yet, 1 = first version... )</remarks>
        [Display(Name = "Version Number")]
        [RevisionNumber, Required]
        public int Version { get; set; } = 0;

        /// <summary>
        /// Is Deleted
        /// </summary>
        [Required, Display(Name = "Deleted")]
        public bool IsDeleted { get; set; } = false;

        // =======================================================
        // Event Attributes
        // =======================================================                

        /// <summary>
        /// Last Updated Date
        /// </summary>
        [LastEditDate, Required]
        public DateTime LastUpdatedOn { get; set; } = DateTime.UtcNow;

        /// <summary>
        /// Record Creation Date
        /// </summary>
        [Required]
        public DateTime CreatedOn { get; set; } = DateTime.UtcNow;

        // =======================================================        

        /// <summary>
        /// Event Id that created this record
        /// </summary>
        [Required, Display(Name = "Create Event ID")]
        public Guid CreateEventId { get; set; }

        /// <summary>
        /// Last event id that edited this record
        /// </summary>
        [Required, Display(Name = "Last Update Event ID")]
        public Guid LastUpdateEventId { get; set; }

        /// <summary>
        /// Event that deleted this record
        /// </summary>
        [Display(Name = "Delete Event ID")]
        public Guid? DeleteEventId { get; set; }
        // =======================================================

        #region --- Foreign ---

        /// <summary>
        /// Create Event
        /// </summary>
        [ForeignKey("CreateEventId")]
        public virtual AuditEvent? CreateEvent { get; set; }

        /// <summary>
        /// Last Edit Event
        /// </summary>
        [ForeignKey("LastUpdateEventId")]
        public virtual AuditEvent? LastUpdateEvent { get; set; }

        /// <summary>
        /// Delete Event
        /// </summary>
        [ForeignKey("DeleteEventId")]
        public virtual AuditEvent? DeleteEvent { get; set; }

        #endregion

        /// <summary>
        /// Constructor
        /// </summary>
        protected AuditTable()
        {
            //nothing
        }
    }
}