// ================================================================================
// <copyright file="IDataSeeder.cs" company="Starlight Software Co">
//    Copyright (c) Starlight Software Co. All rights reserved.
//    Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
// </copyright>
// ================================================================================

namespace ThingsLibrary.Database.Audit.EntityFrameworkCore.Interfaces
{
    /// <summary>
    /// Data Seeder Interface
    /// </summary>
    public interface IDataSeeder
    {
        /// <summary>
        /// Seed Data in database
        /// </summary>
        /// <param name="baseContext"><see cref="DataContext"/> or inherited version</param>
        /// <param name="serviceUser">Current User doing the action</param>
        /// <param name="traceId">Trace ID</param>        
        public void Seed(DataContext baseContext, ClaimsPrincipal serviceUser, string traceId);
    }
}
