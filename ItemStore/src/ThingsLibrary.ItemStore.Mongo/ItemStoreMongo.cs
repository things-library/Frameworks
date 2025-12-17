// ================================================================================
// <copyright file="ItemStoreMongo.cs" company="Starlight Software Co">
//    Copyright (c) 2025 Starlight Software Co. All rights reserved.
//    Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
// </copyright>
// ================================================================================

using MongoDB.Bson;
using MongoDB.Bson.Serialization;
using MongoDB.Bson.Serialization.Serializers;
using MongoDB.Driver;

using ThingsLibrary.ItemStore.Entities;

namespace ThingsLibrary.ItemStore.Mongo
{
    /// <summary>
    /// Mongo Collection Wrapper
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class ItemStoreMongo : IItemStore
    {
        /// <summary>
        /// Entity Store Type
        /// </summary>
        public ItemStoreType StoreType => ItemStoreType.MongoDb;

        /// <summary>
        /// Database Name
        /// </summary>
        public string DatabaseName { get; init; }

        /// <summary>
        /// Table name
        /// </summary>
        public string CollectionName { get; init; }


        /// <summary>
        /// Mongo Client
        /// </summary>
        public IMongoClient Client { get; init; }

        /// <summary>
        /// Mongo Database
        /// </summary>
        public IMongoDatabase Database { get; init; }

        /// <summary>
        /// Mongo Collection of items
        /// </summary>
        public IMongoCollection<ItemEnvelope> Collection { get; init; }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="connectionString">Storage Connection String</param>
        /// <param name="databaseName">Database Name</param>
        /// <param name="collectionName">Table Name</param>
        /// <exception cref="ArgumentNullException">Storage connection string, table name must not be null.</exception>
        /// <exception cref="ArgumentException">Table name must strat with a latter, be alphanumeric and be 3-63 characters</exception>
        public ItemStoreMongo(string connectionString, string databaseName, string collectionName)
        {
            Log.Information("Init MongoDB Database: {DatabaseName}, Collection: {CollectionName}", databaseName, collectionName);

            // validate parameters (name is validated in ValidateTableName)
            if (string.IsNullOrEmpty(connectionString)) { throw new ArgumentNullException(nameof(connectionString)); }
            if (string.IsNullOrEmpty(databaseName)) { throw new ArgumentNullException(nameof(databaseName)); }
            if (string.IsNullOrEmpty(collectionName)) { throw new ArgumentNullException(nameof(collectionName)); }

            // validate name
            ValidateCollectionName(collectionName);
                        
            // set the core properties            
            this.DatabaseName = databaseName;
            this.CollectionName = collectionName;

            BsonSerializer.TryRegisterSerializer(new GuidSerializer(GuidRepresentation.Standard));

            // create the vendor specific object
            Log.Debug("Getting MongoDB Client...");
            this.Client = new MongoClient(connectionString);

            Log.Debug("Getting database {DatabaseName}", this.DatabaseName);
            this.Database = this.Client.GetDatabase(this.DatabaseName);

            Log.Debug("Getting Collection: {CollectionName}", this.CollectionName);
            this.Collection = this.Database.GetCollection<ItemEnvelope>(this.CollectionName);

            // create any indexes that we should have already
            Log.Debug("Creating or verifying indexes...");
            this.CreateIndexes();

            this.LogIndexes();
        }

        #region --- Indexes ---

        private void LogIndexes()
        {
            Log.Debug("Indexes:");
            var indexes = this.Collection.Indexes.List().ToList();
            foreach (var index in indexes)
            {
                Log.Debug("- {IndexName} ({IndexKey})", index.GetElement("name").Value, index.GetElement("key").Value);
            }
        }

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
                Log.Debug("+ Key Index: {IndexName}", "Id");

                IndexKeysDefinition<ItemEnvelope> keys = "{ Id: 1 }";
                var indexModel = new CreateIndexModel<ItemEnvelope>(keys, new CreateIndexOptions
                {
                    Name = "Id",
                    //Unique = true
                });

                this.Collection.Indexes.CreateOne(indexModel);
            }
            catch (Exception ex)
            {
                Log.Error(ex, ex.Message);
            }
        }

        private void CreateResourceKeyIndex()
        {
            try
            {
                Log.Debug("+ Key Index: {IndexName}", "ResourceKey");

                IndexKeysDefinition<ItemEnvelope> keys = "{ ResourceKey: 1 }";
                var indexModel = new CreateIndexModel<ItemEnvelope>(keys, new CreateIndexOptions
                {
                    Name = "ResourceKey",
                    Unique = true
                });

                this.Collection.Indexes.CreateOne(indexModel);
            }
            catch (Exception ex)
            {
                Log.Error(ex, ex.Message);
            }
        }

        private void CreatePartitionIndex()
        {
            // easy exit?
            try
            {
                // Create PartitionKey index
                // ======================================================================
                Log.Debug("+ Partition Index: {IndexName}", "PartitionKey");

                IndexKeysDefinition<ItemEnvelope> keys = "{ PartitionKey: 1 }";
                var indexModel = new CreateIndexModel<ItemEnvelope>(keys, new CreateIndexOptions
                {
                    Name = "PartitionKey",
                    Unique = false
                });

                this.Collection.Indexes.CreateOne(indexModel);                
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

            Log.Debug("EXISTS Item: {EntityKey} ({EntityPartitionKey})", resourceKey, partitionKey);

            return await this.Collection.CountDocumentsAsync(x => x.partition == partitionKey && x.ResourceKey == resourceKey && !x.IsDeleted, cancellationToken: cancellationToken) > 0;
        }

        /// <inheritdoc />
        public async Task<ItemEnvelope?> GetAsync(string partitionKey, string resourceKey, CancellationToken cancellationToken)
        {
            ArgumentException.ThrowIfNullOrEmpty(partitionKey);
            ArgumentException.ThrowIfNullOrEmpty(resourceKey);

            Log.Debug("GET Item: {EntityKey} ({EntityPartitionKey})", resourceKey, partitionKey);

            return await this.Collection.Find(x => 
                x.partition == partitionKey && 
                x.ResourceKey == resourceKey && 
                !x.IsDeleted
            ).FirstOrDefaultAsync(cancellationToken);
        }

        /// <inheritdoc />
        public async Task<List<ItemEnvelope>> GetAllAsync(string partitionKey, string resourceKey, CancellationToken cancellationToken)
        {
            ArgumentException.ThrowIfNullOrEmpty(partitionKey);
            ArgumentException.ThrowIfNullOrEmpty(resourceKey);

            var resourceKeyPrefix = $"{resourceKey}/";

            Log.Debug("GET Item: {EntityKey} ({EntityPartitionKey})", resourceKey, partitionKey);

            var items = await this.Collection.FindAsync(x =>
               x.partition == partitionKey &&
               (x.ResourceKey == resourceKey || x.ResourceKey.StartsWith(resourceKeyPrefix) && 
               !x.IsDeleted),
               cancellationToken: cancellationToken);

            return await items.ToListAsync(cancellationToken);
        }

        /// <inheritdoc />
        public async Task InsertAsync(ItemEnvelope itemEnvelope, CancellationToken cancellationToken)
        {
            ArgumentNullException.ThrowIfNull(itemEnvelope);
            ArgumentException.ThrowIfNullOrEmpty(itemEnvelope.partition);
            ArgumentException.ThrowIfNullOrEmpty(itemEnvelope.ResourceKey);

            Log.Debug("+ Entity {EntityKey} ({EntityPartitionKey})", itemEnvelope.ResourceKey, itemEnvelope.partition);

            try
            {
                await this.Collection.InsertOneAsync(itemEnvelope, new InsertOneOptions(), cancellationToken);
            }
            catch (MongoWriteException ex)
            {
                throw new IOException(ex.Message, ex);
            }
        }

        /// <inheritdoc />
        public async Task UpdateAsync(ItemEnvelope itemEnvelope, CancellationToken cancellationToken)
        {
            ArgumentNullException.ThrowIfNull(itemEnvelope);
            ArgumentException.ThrowIfNullOrEmpty(itemEnvelope.partition);
            ArgumentException.ThrowIfNullOrEmpty(itemEnvelope.ResourceKey);

            Log.Debug("= Entity {EntityKey} ({EntityPartitionKey}).", itemEnvelope.ResourceKey, itemEnvelope.partition);
                        
            try
            {                
                var response = await this.Collection.ReplaceOneAsync(x => 
                    x.partition == itemEnvelope.partition &&
                    x.ResourceKey == itemEnvelope.ResourceKey && 
                    !x.IsDeleted, 
                    itemEnvelope, new ReplaceOptions() { IsUpsert = false }
                , cancellationToken);

                if (response.MatchedCount != 1)
                {
                    throw new IOException("No matching record to update/replace.");
                }
            }
            catch (MongoWriteException ex)
            {
                throw new IOException(ex.Message, ex);
            }
        }

        /// <inheritdoc />
        public async Task DeleteAsync(string partitionKey, string resourceKey, CancellationToken cancellationToken)
        {
            try
            {
                Log.Debug("- DELETE Entity {ResourceKey} ({PartitionKey})", resourceKey, partitionKey);

                var response = await this.Collection.DeleteOneAsync(x => 
                    x.partition == partitionKey && 
                    x.ResourceKey == resourceKey && 
                    !x.IsDeleted
                , cancellationToken);

                if (response.DeletedCount == 0)
                {
                    Log.Information("No entity found to delete for: {ResourceKey}", resourceKey);
                }
            }
            catch (MongoWriteException ex)
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

        ~ItemStoreMongo()
        {
            this.Dispose(false);
        }

        #endregion


        /// <summary>
        /// Validate the database name
        /// </summary>
        /// <param name="name">Database Name</param>
        /// <exception cref="ArgumentException">When name is not valid</exception>
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
        /// <param name="name">Collection Name</param>
        /// <exception cref="ArgumentException"></exception>
        /// <remarks>
        /// Naming Policies:   
        /// </remarks>
        public static void ValidateCollectionName(string name)
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