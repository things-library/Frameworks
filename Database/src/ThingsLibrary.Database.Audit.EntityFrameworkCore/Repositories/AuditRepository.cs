// ================================================================================
// <copyright file="AuditRepository.cs" company="Starlight Software Co">
//    Copyright (c) Starlight Software Co. All rights reserved.
//    Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
// </copyright>
// ================================================================================

using ThingsLibrary.Database.Audit.EntityFrameworkCore.Interfaces;

namespace ThingsLibrary.Database.Audit.EntityFrameworkCore.Repositories
{
    /// <summary>
    /// Audit Data Repository
    /// </summary>
    public class AuditRepository : IAuditRepository    
    {
        private DataContext DataContext { get; init; }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="dataContext"><see cref="DataContext"/></param>        
        public AuditRepository(DataContext dataContext)
        {
            this.DataContext = dataContext;
        }

        /// <inheritdoc />
        public async Task<List<Models.AuditEvent>> GetAllAsync(Guid entityId, CancellationToken cancellationToken = default)
        {
            return await this.DataContext.AuditEvents
                .Include(x => x.AuditUser)
                .Where(x => x.EntityId == entityId)
                .OrderByDescending(x => x.SequenceId)
                .ToListAsync(cancellationToken);
        }
    }
}
