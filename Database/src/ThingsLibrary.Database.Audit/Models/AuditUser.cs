// ================================================================================
// <copyright file="AuditUser.cs" company="Starlight Software Co">
//    Copyright (c) Starlight Software Co. All rights reserved.
//    Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
// </copyright>
// ================================================================================

namespace ThingsLibrary.Database.Audit.Models
{
    /// <summary>
    /// Audit User
    /// </summary>
    [System.Diagnostics.DebuggerDisplay("{GivenName} {FamilyName} ({Username}) {Id}")]
    [Attributes.Index(nameof(Username))]
    [Attributes.Index(nameof(ObjectId))]
    public class AuditUser
    {
        /// <summary>
        /// ID
        /// </summary>
        [Display(Name = "ID"), Key, Required]
        public Guid Id { get; set; }

        /// <summary>
        /// Object ID
        /// </summary>
        /// <remarks>JWT Claim: sub or oid</remarks>
        [Display(Name = "Object ID"), Required]
        public string ObjectId { get; set; } = string.Empty;

        /// <summary>
        /// Username (Claim in order: preferred_username, upn or email)
        /// </summary>
        [Display(Name = "Username"), Required]
        public string Username { get; set; } = string.Empty;

        #region --- Extended ---

        /// <summary>
        /// Email Address
        /// </summary>
        /// <remarks>JWT Claim: email</remarks>
        [Display(Name = "Email Address"), Required]
        public string EmailAddress { get; set; } = string.Empty;

        /// <summary>
        /// This is the unique username (Claim: 'name') of who caused the event
        /// </summary>
        /// <remarks>JWT Claim: given_name</remarks>
        [Display(Name = "Given Name"), Required]
        public string GivenName { get; set; } = string.Empty;

        /// <summary>
        /// This is the unique username (Claim: 'name') of who caused the event
        /// </summary>
        /// <remarks>JWT Claim: family_name</remarks>
        [Display(Name = "Family Name"), Required]
        public string FamilyName { get; set; } = string.Empty;


        public string FullName => $"{this.GivenName} {this.FamilyName}".Trim();

        #endregion

        /// <summary>
        /// Last Updated On
        /// </summary>
        [Display(Name = "Updated On"), LastEditDate, Required]
        public DateTimeOffset LastUpdatedOn { get; set; } = DateTimeOffset.UtcNow;

        /// <summary>
        /// Created On (timestamp)
        /// </summary>
        [Display(Name = "Created Date"), Required]
        public DateTimeOffset CreatedOn { get; set; } = DateTimeOffset.UtcNow;



        /// <summary>
        /// Account is disabled
        /// </summary>        
        [Display(Name = "Disabled"), Required]
        public bool IsDisabled { get; set; } = false;

        /// <summary>
        /// Account is locked
        /// </summary>        
        [Display(Name = "Locked"), Required]
        public bool IsLocked { get; set; } = false;
    }
}
