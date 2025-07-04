// ================================================================================
// <copyright file="AuditUser.cs" company="Starlight Software Co">
//    Copyright (c) Starlight Software Co. All rights reserved.
//    Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
// </copyright>
// ================================================================================

namespace ThingsLibrary.Database.Audit.Adapters
{
    public static class AuditUserAdapters
    {
        public static Contracts.AuditUserDto ToDto(this Models.AuditUser auditUser)
        {
            return new Contracts.AuditUserDto
            {   
                Id = auditUser.Id,
                Username = auditUser.Username,                               
                FullName = auditUser.FullName                
            };
        }
    }
}
