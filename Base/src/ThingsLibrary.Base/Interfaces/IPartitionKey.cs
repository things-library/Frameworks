// ================================================================================
// <copyright file="IPartitionKey.cs" company="Starlight Software Co">
//    Copyright (c) Starlight Software Co. All rights reserved.
//    Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
// </copyright>
// ================================================================================

namespace ThingsLibrary.Interfaces
{
    /// <summary>
    /// Basic Interface making sure the class has a standard 'PartitionKey' field
    /// </summary>
    public interface IPartitionKey
    {
        /// <summary>
        /// Data Partition Key
        /// </summary>
        public string PartitionKey { get; }
    }
}
