// ================================================================================
// <copyright file="AuditEvent.cs" company="Starlight Software Co">
//    Copyright (c) Starlight Software Co. All rights reserved.
//    Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
// </copyright>
// ================================================================================

namespace ThingsLibrary.Database.Audit.Adapters
{
    public static class AuditEventAdapters
    {
        public static Contracts.AuditEventDto ToDto(this Models.AuditEvent auditEvent)
        {
            return new Contracts.AuditEventDto
            {
                SequenceId = auditEvent.SequenceId,
                EventType = auditEvent.EventType.ToString(),

                NewValues = auditEvent.NewValues,
                OldValues = auditEvent.OldValues,

                EventOn = auditEvent.EventOn,

                AuditUser = auditEvent.AuditUser!.ToDto()
            };
        }

        public static IEnumerable<Contracts.AuditEventDto> ToDto(this IEnumerable<Models.AuditEvent> auditEvents)
        {
            return auditEvents.ToList().ConvertAll(auditEvents => ToDto(auditEvents));            
        }
    }
}
