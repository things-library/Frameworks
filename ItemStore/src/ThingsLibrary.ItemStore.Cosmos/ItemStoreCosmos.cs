// ================================================================================
// <copyright file="ItemStoreCosmos.cs" company="Starlight Software Co">
//    Copyright (c) 2025 Starlight Software Co. All rights reserved.
//    Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
// </copyright>
// ================================================================================

using System.Linq.Expressions;
using Microsoft.Azure.Cosmos;
using Microsoft.Azure.Cosmos.Linq;

namespace ThingsLibrary.ItemStore.Cosmos
{
    /// <summary>
    /// Item Store
    /// </summary>
    /// <see cref="https://learn.microsoft.com/en-us/azure/cosmos-db/nosql/quickstart-dotnet?tabs=azure-portal%2Cwindows%2Cconnection-string%2Csign-in-azure-cli"/>
    public class ItemStoreCosmos : IItemStore
    {
        /// <summary>
        /// Entity Store Type
        /// </summary>
        public ItemStoreType StoreType => ItemStoreType.Azure_Cosmos;

        /// <summary>
        /// Database Name
        /// </summary>
        public string DatabaseName { get; init; }

        /// <summary>
        /// Table name
        /// </summary>
        public string CollectionName { get; init; }


        /// <summary>
        /// Cosmos Database
        /// </summary>
        private Database Database { get; init; }

        /// <summary>
        /// Cosmos Container of items
        /// </summary>        
        public Container Container { get; init; }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="connectionString">Storage Connection String</param>
        /// <param name="databaseName">Database Name</param>
        /// <param name="collectionName">Collection Name</param>
        /// <exception cref="ArgumentNullException">Storage connection string, table name must not be null.</exception>
        /// <exception cref="ArgumentException">Table name must strat with a latter, be alphanumeric and be 3-63 characters</exception>
        public ItemStoreCosmos(string connectionString, string databaseName, string collectionName)
        {
            Log.Information("Init CosmosDB Database: {DatabaseName}, Collection: {CollectionName}", databaseName, collectionName);

            // validate parameters (name is validated in ValidateTableName)
            if (string.IsNullOrEmpty(connectionString)) { throw new ArgumentNullException(nameof(connectionString)); }
            if (string.IsNullOrEmpty(databaseName)) { throw new ArgumentNullException(nameof(databaseName)); }
            if (string.IsNullOrEmpty(collectionName)) { throw new ArgumentNullException(nameof(collectionName)); }

            // validate name
            ValidateTableName(collectionName);

            // set the core properties            
            this.DatabaseName = databaseName;
            this.CollectionName = collectionName;

            // create the vendor specific object
            Log.Debug("Getting CosmosDB Client...");
            var client = new CosmosClient(connectionString, new CosmosClientOptions
            {
                SerializerOptions = new CosmosSerializationOptions
                {
                    PropertyNamingPolicy = CosmosPropertyNamingPolicy.CamelCase                    
                }
            });

            Log.Debug("Creating Database {DatabaseName} if not exists...", this.DatabaseName);
            DatabaseResponse databaseResponse = client.CreateDatabaseIfNotExistsAsync(this.DatabaseName, cancellationToken: default).Result;
            this.Database = databaseResponse.Database;

            Log.Debug("Creating Container {CollectionName} if not exists...", this.CollectionName);
            ContainerResponse containerResponse = this.Database.CreateContainerIfNotExistsAsync(
                id: this.CollectionName,
                partitionKeyPath: "/partition",
                cancellationToken: default
            ).Result;
            this.Container = containerResponse.Container;

            // create any indexes that we should have already
            Log.Debug("Creating or verifying indexes...");
            this.CreateIndexes();
        }

        #region --- Indexes ---


        private void CreateIndexes()
        {
            // KEY INDEX (if not 'Id' field name, aka: existing index)        
            this.CreateIdIndex();

            // path friendly resource path
            this.CreateResourceKeyIndex();

            // Partition index.. how the data is partitioned
            this.CreatePartitionIndex();
        }

        private void CreateIdIndex()
        {
            try
            {
                //Log.Debug("+ Key Index: {IndexName}", this.Key.Name);

                //IndexKeysDefinition<T> keys = "{ " + this.Key.Name + ": 1 }";
                //var indexModel = new CreateIndexModel<T>(keys, new CreateIndexOptions
                //{
                //    Name = this.Key.Name,
                //    Unique = true
                //});

                //this.Collection.Indexes.CreateOne(indexModel);
            }
            catch (Exception ex)
            {
                Log.Error(ex, ex.Message);
            }
        }

        private void CreateResourceKeyIndex()
        {
            //try
            //{
            //    Log.Debug("+ Key Index: {IndexName}", "ResourceKey");

            //    IndexKeysDefinition<ItemEnvelope> keys = "{ ResourceKey: 1 }";
            //    var indexModel = new CreateIndexModel<ItemEnvelope>(keys, new CreateIndexOptions
            //    {
            //        Name = "ResourceKey",
            //        Unique = true
            //    });

            //    this.Container.Indexes.CreateOne(indexModel);
            //}
            //catch (Exception ex)
            //{
            //    Log.Error(ex, ex.Message);
            //}
        }

        private void CreatePartitionIndex()
        {
            try
            {
                //Log.Debug("+ Partition Index: {IndexName}", this.PartitionKey.Name);

                //IndexKeysDefinition<T> keys = "{ " + this.PartitionKey.Name + ": 1 }";
                //var indexModel = new CreateIndexModel<T>(keys, new CreateIndexOptions
                //{
                //    Name = this.PartitionKey.Name,
                //    Unique = false
                //});

                //this.Collection.Indexes.CreateOne(indexModel);
            }
            catch (Exception ex)
            {
                Log.Error(ex, ex.Message);
            }
        }
        
        #endregion

        #region --- Entities --- 

        /// <inheritdoc />
        public async Task<bool> ExistsAsync(string partitionKey, string resourceKey, CancellationToken cancellationToken)
        {
            ArgumentException.ThrowIfNullOrEmpty(partitionKey);
            ArgumentException.ThrowIfNullOrEmpty(resourceKey);

            return await this.GetAsync(partitionKey, resourceKey, cancellationToken) != null;
        }
        
        /// <inheritdoc />
        public async Task<ItemEnvelope?> GetAsync(string partitionKey, string resourceKey, CancellationToken cancellationToken)
        {
            ArgumentException.ThrowIfNullOrEmpty(partitionKey);
            ArgumentException.ThrowIfNullOrEmpty(resourceKey);

            try
            {
                var query = this.Container.GetItemLinqQueryable<ItemEnvelope>().Where(x => x.Partition == partitionKey && x.ResourceKey == resourceKey && !x.IsDeleted);

                using var feedIterator = query.ToFeedIterator();
                if (feedIterator.HasMoreResults)
                {
                    var response = await feedIterator.ReadNextAsync(cancellationToken);
                    return response.FirstOrDefault();
                }

                return null;
            }
            catch (CosmosException ex) when (ex.StatusCode == System.Net.HttpStatusCode.NotFound)
            {
                return null;
            }
            catch (Exception ex)
            {
                throw new IOException(ex.Message, ex);
            }
        }

        /// <inheritdoc />
        public async Task<List<ItemEnvelope>> GetFamilyAsync(string partitionKey, string resourceKey, CancellationToken cancellationToken)
        {
            ArgumentException.ThrowIfNullOrEmpty(partitionKey);
            ArgumentException.ThrowIfNullOrEmpty(resourceKey);

            var resourceKeyPrefix = $"{resourceKey}/";

            var query = this.Container.GetItemLinqQueryable<ItemEnvelope>().Where(x => 
                x.Partition == partitionKey 
                && (x.ResourceKey == resourceKey || x.ResourceKey.StartsWith(resourceKeyPrefix))
                && !x.IsDeleted
            );

            var results = new List<ItemEnvelope>();
            using var feed = query.ToFeedIterator();
            while (feed.HasMoreResults)
            {
                var response = await feed.ReadNextAsync(cancellationToken);
                results.AddRange(response);
            }

            return results;
        }

        /// <inheritdoc />
        public async Task<IEnumerable<ItemEnvelope>> GetAllAsync(Expression<Func<ItemEnvelope, bool>> predicate, CancellationToken cancellationToken)
        {
            var query = this.Container.GetItemLinqQueryable<ItemEnvelope>().Where(predicate);

            var results = new List<ItemEnvelope>();
            using var feed = query.ToFeedIterator();
            while (feed.HasMoreResults)
            {
                var response = await feed.ReadNextAsync(cancellationToken);
                results.AddRange(response);
            }

            return results;
        }
        
        /// <inheritdoc />
        public async Task InsertAsync(ItemEnvelope itemEnvelope, CancellationToken cancellationToken)
        {
            ArgumentNullException.ThrowIfNull(itemEnvelope);
            ArgumentException.ThrowIfNullOrEmpty(itemEnvelope.Id);
            ArgumentException.ThrowIfNullOrEmpty(itemEnvelope.Partition);            

            Log.Debug("Inserting Entity {ResourceKey} into partition {PartitionKey}.", itemEnvelope.ResourceKey, itemEnvelope.Partition);
                       
            try
            {                
                await this.Container.CreateItemAsync(                    
                    item: itemEnvelope,                    
                    partitionKey: new PartitionKey(itemEnvelope.Partition),
                    cancellationToken: cancellationToken
                );
            }
            catch (CosmosException ex) when (ex.StatusCode == System.Net.HttpStatusCode.Conflict)
            {
                Log.Error(ex, "Entity with ResourceKey {ResourceKey} already exists.", itemEnvelope.ResourceKey);
                throw new InvalidOperationException($"Entity with ResourceKey '{itemEnvelope.ResourceKey}' already exists.", ex);
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Failed to insert entity {ResourceKey}.", itemEnvelope.ResourceKey);
                throw new IOException(ex.Message, ex);
            }
        }


        /// <inheritdoc />
        public async Task UpdateAsync(ItemEnvelope itemEnvelope, CancellationToken cancellationToken)
        {
            ArgumentNullException.ThrowIfNull(itemEnvelope);
            ArgumentException.ThrowIfNullOrEmpty(itemEnvelope.Id);
            ArgumentException.ThrowIfNullOrEmpty(itemEnvelope.Partition);            

            Log.Information("Updating Entity {ResourceKey} in partition {PartitionKey}.", itemEnvelope.ResourceKey, itemEnvelope.Partition);

            try
            {
                // Increment revision and update timestamp
                itemEnvelope.IncrementRevision();

                await this.Container.ReplaceItemAsync<ItemEnvelope>(
                    item: itemEnvelope,
                    id: itemEnvelope.Id,
                    partitionKey: new PartitionKey(itemEnvelope.Partition),
                    cancellationToken: cancellationToken
                );
            }
            catch (CosmosException ex) when (ex.StatusCode == System.Net.HttpStatusCode.NotFound)
            {
                Log.Error(ex, "Entity with ResourceKey {ResourceKey} not found for update.", itemEnvelope.ResourceKey);
                throw new ArgumentException($"Entity with ResourceKey '{itemEnvelope.ResourceKey}' not found.", ex);
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Failed to update entity {ResourceKey}.", itemEnvelope.ResourceKey);
                throw new IOException(ex.Message, ex);
            }
        }

        /// <inheritdoc />
        public async Task DeleteAsync(string partitionKey, string id, CancellationToken cancellationToken)
        {
            ArgumentException.ThrowIfNullOrEmpty(partitionKey);
            ArgumentException.ThrowIfNullOrEmpty(id);

            Log.Information("Deleting Entity {Id} from partition {PartitionKey}.", id, partitionKey);

            try
            {
                await this.Container.DeleteItemAsync<ItemEnvelope>(
                    id: id,
                    partitionKey: new PartitionKey(partitionKey),
                    cancellationToken: cancellationToken
                );
            }
            catch (CosmosException ex) when (ex.StatusCode == System.Net.HttpStatusCode.NotFound)
            {
                Log.Information(ex, "No entity found to delete for Id: {Id}, Error: {ErrorMessage}", id, ex.Message);
                throw new ArgumentException($"No entity found to delete for Id: {id}");
            }
            catch (Exception ex)
            {
                throw new IOException(ex.Message, ex);
            }
        }

        #endregion
       

        #region --- IDisposable ---

        private bool _disposed = false;

        public void Dispose()
        {
            this.Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            // Cleanup
            if (_disposed) { return; }

            _disposed = true;   // we are handling the disposing in this thread so don't let others do the same thing

            if (disposing)
            {
                // free up objects
            }
        }

        ~ItemStoreCosmos()
        {
            this.Dispose(false);                        
        }

        #endregion



        /// <summary>
        /// Validate the database name
        /// </summary>
        /// <param name="name">Database Name</param>
        /// <exception cref="ArgumentException"></exception>
        public static void ValidateDatabaseName(string name)
        {
            // Invalid Characters: /."*< >:|?$

            // make sure the database name doesn't violate Mongo's naming
            if (name.IndexOfAny("/\\. \"$*<>:|?".ToCharArray()) != -1)
            {
                throw new ArgumentException("Database name can not include any of these characters: /\\. \"$*<>:|?");
            }

            //// make sure the database name doesn't violate Mongo's naming
            //if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux) && name.IndexOfAny("/\\. \"$*<>:|?".ToCharArray()) != -1)
            //{
            //    throw new ArgumentException("Database name can not include any of these characters: /\\. \"$*<>:|?");
            //}
            //else if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows) && name.IndexOfAny("/\\. \"$".ToCharArray()) != -1)
            //{
            //    throw new ArgumentException("Database name can not include any of these characters: /\\. \"$");
            //}
        }

        /// <summary>
        /// Validates the name to be used 
        /// </summary>
        /// <param name="name"></param>
        /// <exception cref="ArgumentException"></exception>
        /// <remarks>
        /// Naming Policies:
        /// 1. Not Empty
        /// 2. Must be between 3 and 63 characters
        /// </remarks>
        public static void ValidateTableName(string name)
        {
            if (string.IsNullOrEmpty(name)) { throw new ArgumentNullException(nameof(name)); }

            if (name.StartsWith("system.")) { throw new ArgumentException("Name can not begin with 'system.'"); }

            // validate name        
            //if (!name.All(c => char.IsLetterOrDigit(c))) { throw new ArgumentException($"Table name '{name}' can only be alphanumeric characters.", nameof(name)); }
            //if (name.Length < 3 || name.Length > 63) { throw new ArgumentException($"Table name '{name}' must be between 3 and 63 characters long.", nameof(name)); }
            //if (!char.IsLetter(name[0])) { throw new ArgumentException($"Table name '{name}' must start with a letter", nameof(name)); }
        }
    }
}