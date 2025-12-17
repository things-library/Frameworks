// ================================================================================
// <copyright file="IItemStore.cs" company="Starlight Software Co">
//    Copyright (c) 2025 Starlight Software Co. All rights reserved.
//    Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
// </copyright>
// ================================================================================

namespace ThingsLibrary.ItemStore.Interfaces
{
    public interface IItemStore : IDisposable
    {
        /// <summary>
        /// Type of entity store (Azure_Table, GCP_DataStore)
        /// </summary>
        public ItemStoreType StoreType { get; }

        /// <summary>
        /// Root name of the collection
        /// </summary>
        public string CollectionName { get; }

        #region --- Entities ---

        /// <summary>
        /// Checks for the existance of a record in the store
        /// </summary>
        /// <param name="partitionKey">Partition Key</param>        
        /// <param name="resourceKey">Resource Key</param>
        /// <param name="cancellationToken">Cancellation Token</param>
        /// <returns>Success if exists</returns>
        public Task<bool> ExistsAsync(string partitionKey, string resourceKey, CancellationToken cancellationToken);

        /// <summary>
        /// Get a specific entity from the table
        /// </summary>        
        /// <param name="partitionKey">Partition Key</param>        
        /// <param name="resourceKey">Resource Key</param>
        /// <param name="cancellationToken">Cancellation Token</param>
        /// <returns></returns>
        public Task<ItemEnvelope?> GetAsync(string partitionKey, string resourceKey, CancellationToken cancellationToken);

        /// <summary>
        /// Get a item and all children entities from the store
        /// </summary>        
        /// <param name="partitionKey">Partition Key</param>        
        /// <param name="resourceKey">Resource Key</param>
        /// <param name="cancellationToken">Cancellation Token</param>
        /// <returns></returns>
        public Task<List<ItemEnvelope>> GetAllAsync(string partitionKey, string resourceKey, CancellationToken cancellationToken);

        /// <summary>
        /// Insert Entity into store
        /// </summary>
        /// <param name="partitionKey">Partition Key</param>        
        /// <param name="resourceKey">Resource Key</param>
        /// <param name="cancellationToken">Cancellation Token</param>
        public Task InsertAsync(ItemEnvelope itemEnvelope, CancellationToken cancellationToken);

        /// <summary>
        /// Update Entity in store
        /// </summary>
        /// <param name="partitionKey">Partition Key</param>        
        /// <param name="resourceKey">Resource Key</param>
        /// <param name="entity">Entity</param>
        /// <param name="cancellationToken">Cancellation Token</param>
        public Task UpdateAsync(ItemEnvelope itemEnvelope, CancellationToken cancellationToken);

        ///// <summary>
        ///// Upsert Entity into store
        ///// </summary>
        ///// <param name="partitionKey">Partition Key</param>        
        ///// <param name="resourceKey">Resource Key</param>
        ///// <param name="entity">Entity</param>
        ///// <param name="cancellationToken">Cancellation Token</param>
        //public Task UpsertAsync(string partitionKey, string resourceKey, ItemEnvelope item, CancellationToken cancellationToken);

        ///// <summary>
        ///// Upsert a bulk of records at once
        ///// </summary>
        ///// <param name="records">Records to insert (if not exist) or update.</param>
        ///// <returns>Success status</returns>
        //public Task BulkUpsertAsync(string partitionKey, string resourceKey, IEnumerable<ItemEnvelope> items, CancellationToken cancellationToken);

        /// <summary>
        /// Delete entity from store
        /// </summary>        
        /// <param name="partitionKey">Partition Key</param>        
        /// <param name="resourceKey">Resource Key</param>
        /// <param name="cancellationToken">Cancellation Token</param>
        public Task DeleteAsync(string partitionKey, string resourceKey, CancellationToken cancellationToken);

        #endregion
    }
}