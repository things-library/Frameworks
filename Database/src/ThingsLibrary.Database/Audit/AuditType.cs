// ================================================================================
// <copyright file="AuditType.cs" company="Starlight Software Co">
//    Copyright (c) Starlight Software Co. All rights reserved.
//    Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
// </copyright>
// ================================================================================

namespace ThingsLibrary.Database.Audit
{
    /// <summary>
    /// Audit Type
    /// </summary>    
    public static class AuditType
    {
        public const char Unknown = default;

        public const char Create = 'C';
        public const char Delete = 'D';
        public const char Update = 'U';
        public const char UnDelete = 'X';

        public const char Query = 'Q';
        public const char QueryHistory = 'H';
    }
}
