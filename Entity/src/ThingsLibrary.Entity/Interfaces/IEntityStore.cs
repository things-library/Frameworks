namespace ThingsLibrary.Entity.Interfaces
{
    public interface IEntityStore<T> : IDisposable where T : class
    {
        /// <summary>
        /// Type of entity store (Azure_Table, GCP_DataStore)
        /// </summary>
        public EntityStoreType StoreType { get; }

        /// <summary>
        /// Entity's Type
        /// </summary>
        public Type Type { get; }

        /// <summary>
        /// Entity Keys
        /// </summary>
        public PropertyInfo Key { get; }

        /// <summary>
        /// Partition Key (if exists)
        /// </summary>
        public PropertyInfo? PartitionKey { get; init; }

        /// <summary>
        /// Timestamp Field
        /// </summary>
        public PropertyInfo? Timestamp { get; }

        /// <summary>
        /// Table Name / Kind Name
        /// </summary>
        public string Name { get; }

        #region --- Entities ---

        /// <summary>
        /// Checks for the existance of a record in the store
        /// </summary>
        /// <param name="key">Key</param>
        /// <param name="cancellationToken">Cancellation Token</param>
        /// <returns>Success if exists</returns>
        public Task<bool> ExistsAsync(object key, object partitionKey, CancellationToken cancellationToken = default);

        /// <summary>
        /// Get a specific entity from the table
        /// </summary>        
        /// <param name="key">Key</param>        
        /// <param name="cancellationToken">Cancellation Token</param>
        /// <returns></returns>
        Task<T> GetEntityAsync(object key, object partitionKey, CancellationToken cancellationToken = default);
        
        /// <summary>
        /// Get all entities from store
        /// </summary>        
        /// <param name="sinceDate">If provided will be all records edited after</param>
        /// <param name="cancellationToken">Cancellation Token</param>
        /// <returns>List of matching entities</returns>
        public Task<IEnumerable<T>> GetEntitiesAsync(DateTime? sinceDate = null, CancellationToken cancellationToken = default);

        /// <summary>
        /// Insert Entity into store
        /// </summary>
        /// <param name="entity">Entity</param>
        /// <param name="cancellationToken">Cancellation Token</param>
        public Task InsertEntityAsync(T entity, CancellationToken cancellationToken = default);

        /// <summary>
        /// Update Entity in store
        /// </summary>
        /// <param name="entity">Entity</param>
        /// <param name="cancellationToken">Cancellation Token</param>
        public Task UpdateEntityAsync(T entity, CancellationToken cancellationToken = default);

        /// <summary>
        /// Upsert Entity into store
        /// </summary>
        /// <param name="entity">Entity</param>
        /// <param name="cancellationToken">Cancellation Token</param>
        public Task UpsertEntityAsync(T entity, CancellationToken cancellationToken = default);

        /// <summary>
        /// Upsert a bulk of records at once
        /// </summary>
        /// <param name="records">Records to insert (if not exist) or update.</param>
        /// <returns>Success status</returns>
        public Task BulkUpsertAsync(IEnumerable<T> records, CancellationToken cancellationToken = default);

        /// <summary>
        /// Delete entity from store
        /// </summary>        
        /// <param name="key">Key</param>
        /// <param name="cancellationToken">Cancellation Token</param>
        public Task DeleteEntityAsync(object key, object partitionKey, CancellationToken cancellationToken = default);

        #endregion

        /// <summary>
        /// Delete the entire data store of records
        /// </summary>
        /// <param name="cancellationToken">Cancellation Token</param>
        /// <returns></returns>
        public Task DeleteStoreAsync(CancellationToken cancellationToken = default);

    }
}