// ================================================================================
// <copyright file="IAuditRepository.cs" company="Starlight Software Co">
//    Copyright (c) Starlight Software Co. All rights reserved.
//    Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
// </copyright>
// ================================================================================

namespace ThingsLibrary.Database.Audit.Interfaces
{
    /// <summary>
    /// Audit Repository
    /// </summary>
    public interface IAuditRepository
    {
        /// <summary>
        /// Get all audit events for a entity id
        /// </summary>
        /// <param name="entityId">Entity Id being tracked</param>
        /// <returns></returns>
        public Task<List<Models.AuditEvent>> GetAll(string entityId);        
    }
}
