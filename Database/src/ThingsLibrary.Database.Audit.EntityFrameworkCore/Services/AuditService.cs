// ================================================================================
// <copyright file="AuditService.cs" company="Starlight Software Co">
//    Copyright (c) Starlight Software Co. All rights reserved.
//    Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
// </copyright>
// ================================================================================

using ThingsLibrary.Database.Audit.Adapters;

namespace ThingsLibrary.Database.Audit.EntityFrameworkCore.Services
{
    /// <summary>
    /// User server for getting the logged in user details
    /// </summary>
    public class AuditService : Interfaces.IAuditService
    {
        private Interfaces.IAuditRepository AuditRepository { get; set; }

        /// <summary>
        /// Constructor
        /// </summary>        
        /// <param name="auditRepository"><see cref="Interfaces.IAuditRepository"/></param>        
        /// <exception cref="ArgumentNullException">When any parameters are null</exception>
        public AuditService(Interfaces.IAuditRepository auditRepository)
        {
            this.AuditRepository = auditRepository ?? throw new ArgumentNullException(nameof(auditRepository));
        }

        /// <summary>
        /// Get all Audit Events from storage
        /// </summary>
        /// <param name="objectId">Object Id of entity we are interested in</param>
        /// <returns><see cref="List{AuditEventDto}"/></returns>
        public async Task<List<Contracts.AuditEventDto>> GetAllAsync(Guid objectId, CancellationToken cancellationToken = default)
        {
            var auditEvents = await this.AuditRepository.GetAllAsync(objectId, cancellationToken);

            return auditEvents.ConvertAll(x => x.ToDto());
        }
    }
}
