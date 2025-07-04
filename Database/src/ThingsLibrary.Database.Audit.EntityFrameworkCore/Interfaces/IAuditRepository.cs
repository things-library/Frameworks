// ================================================================================
// <copyright file="IAuditRepository.cs" company="Starlight Software Co">
//    Copyright (c) Starlight Software Co. All rights reserved.
//    Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
// </copyright>
// ================================================================================

namespace ThingsLibrary.Database.Audit.EntityFrameworkCore.Interfaces
{
    public interface IAuditRepository
    {
        public Task<List<AuditEvent>> GetAllAsync(Guid entityId, CancellationToken cancellationToken = default);
    }
}