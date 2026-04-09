// ================================================================================
// <copyright file="IFileStores.cs" company="Starlight Software Co">
//    Copyright (c) Starlight Software Co. All rights reserved.
//    Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
// </copyright>
// ================================================================================

namespace ThingsLibrary.Storage.Interfaces
{
    public interface IFileStores
    {
        /// <summary>
        /// Type of entity store (Azure_Table, GCP_DataStore)
        /// </summary>
        public FileStoreType StoreType { get; }

        /// <summary>
        /// Get/Create Cloud File Store
        /// </summary>        
        /// <returns><see cref="FileStore"/></returns>
        public IFileStore GetStore(string bucketName);
    }
}