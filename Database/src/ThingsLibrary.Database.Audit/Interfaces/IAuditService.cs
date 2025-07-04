// ================================================================================
// <copyright file="IAuditService.cs" company="Starlight Software Co">
//    Copyright (c) Starlight Software Co. All rights reserved.
//    Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
// </copyright>
// ================================================================================

namespace ThingsLibrary.Database.Audit.Interfaces
{
    /// <summary>
    /// Audit Service
    /// </summary>
    public interface IAuditService
    {
        /// <summary>
        /// Get all audit events for a object id
        /// </summary>
        /// <returns><see cref="Contracts.AuditEventDto"/></returns>
        public Task<List<Contracts.AuditEventDto>> GetAll(string entityId);        
    }
}
