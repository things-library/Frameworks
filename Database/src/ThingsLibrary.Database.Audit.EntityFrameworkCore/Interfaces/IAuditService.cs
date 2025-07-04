// ================================================================================
// <copyright file="IAuditService.cs" company="Starlight Software Co">
//    Copyright (c) Starlight Software Co. All rights reserved.
//    Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
// </copyright>
// ================================================================================

using ThingsLibrary.Database.Audit.Contracts;

namespace ThingsLibrary.Database.Audit.EntityFrameworkCore.Interfaces
{
    public interface IAuditService
    {
        public Task<List<AuditEventDto>> GetAllAsync(Guid objectId, CancellationToken cancellationToken = default);
    }
}