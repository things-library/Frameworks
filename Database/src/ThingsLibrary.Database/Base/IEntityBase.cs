// ================================================================================
// <copyright file="IEntityBase.cs" company="Starlight Software Co">
//    Copyright (c) Starlight Software Co. All rights reserved.
//    Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
// </copyright>
// ================================================================================

using ThingsLibrary.Interfaces;

namespace ThingsLibrary.Database.Base
{
    public interface IEntityBase : IId, IPartitionKey, IRevisionNumber, ILastEditDate
    {        
        /// <summary>
        /// Record Created DateTime (UTC)
        /// </summary>
        public DateTimeOffset CreatedOn { get; }
    }
}
