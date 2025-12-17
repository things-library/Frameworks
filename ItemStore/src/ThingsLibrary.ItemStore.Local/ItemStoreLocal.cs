// ================================================================================
// <copyright file="ItemStoreLocal.cs" company="Starlight Software Co">
//    Copyright (c) 2025 Starlight Software Co. All rights reserved.
//    Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
// </copyright>
// ================================================================================

using ThingsLibrary.ItemStore.Entities;

namespace ThingsLibrary.ItemStore.Local
{

    public class ItemStoreLocal : IItemStore
    {
        #region --- General ---
                      
        /// <summary>
        /// Entity Store Type
        /// </summary>
        public ItemStoreType StoreType => ItemStoreType.Local;

        /// <summary>
        /// Table name
        /// </summary>
        public string CollectionName { get; init; }

        #endregion

        #region --- Store Specific ---

        public LiteDatabase Client { get; init; }
        public ILiteCollection<ItemEnvelope> Collection { get; init; }

        public string FilePath { get; init; } = string.Empty;
        public string TableDirPath { get; init; } = string.Empty;

        public bool IsMemoryDb => string.IsNullOrEmpty(this.FilePath);
        public bool IsFileSaving { get; init; } = false;

        /// <summary>
        /// Lock object for thread-safe operations
        /// </summary>
        public static readonly object LockObject = new object();

        #endregion

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="storageConnectionString">Storage Connection String</param>
        /// <param name="collectionName">Collection Name</param>
        /// <remarks>
        /// Connection String: 
        //    "Filename=C:\database.db;Password=1234;Upgrade=true;"
        //    "Filename=C:\database.db;Password=1234;Upgrade=true;Files=true"     //Extra property 'files' creates local json files that represent the entities
        //    "Filename=C:\database.db;Password=1234;Upgrade=true;Files=true;RootTableDir=c:\root"     //Extra property 'files' creates local json files that represent the entities
        //    "Filename=:memory:"
        /// </remarks>
        /// <see cref="http://www.litedb.org/docs/connection-string/"/>
        /// <exception cref="ArgumentNullException">Storage connection string, table name must not be null, 'storage_dir_path' must be in connection string.</exception>
        /// <exception cref="ArgumentException">Table name must strat with a latter, be alphanumeric and be 3-63 characters</exception>
        public ItemStoreLocal(string storageConnectionString, string collectionName)
        {
            // validate the generic entity
            
            // validate the parameters
            ArgumentException.ThrowIfNullOrEmpty(storageConnectionString);
            ArgumentException.ThrowIfNullOrEmpty(collectionName);

            // validate name
            ValidateCollectionName(collectionName);
            CollectionName = collectionName;

            // parse out the ProjectId and InstanceId from connectionstring            
            var builder = new System.Data.Common.DbConnectionStringBuilder
            {
                ConnectionString = storageConnectionString
            };

            // validate file name property
            var filePath = builder.GetValue<string?>("FileName", null);
            if (string.IsNullOrEmpty(filePath)) { throw new ArgumentException("No 'FileName' found in connection string."); }

            // get full path if not memory db
            if (!filePath.StartsWith(':'))
            {
                this.FilePath = Path.GetFullPath(filePath);
            }

            // connect to database
            Log.Debug("Getting LiteDatabase Client...");
            this.Client = new LiteDatabase(storageConnectionString);

            Log.Debug("Getting Collection: {CollectionName}", this.CollectionName);
            this.Collection = this.Client.GetCollection<ItemEnvelope>(this.CollectionName);
            
            // dates will always be UTC
            this.Client.UtcDate = true;

            // create any indexes that we should have already
            Log.Debug("Creating or verifying indexes...");
            this.CreateIndexes();

            // see if we are going to be saving out files
            this.IsFileSaving = builder.GetValue<bool>("Files", false);
            if (this.IsFileSaving)
            {
                // validate storage path  
                if (this.IsMemoryDb) { throw new InvalidDataException("In-Memory or temp databases can not be used with file saving."); }

                var rootTableDir = builder.GetValue<string?>("RootTableDir", null);
                if (!string.IsNullOrEmpty(rootTableDir))
                {
                    rootTableDir = Path.GetFullPath(rootTableDir);
                }
                else
                {
                    // get the pathing to the table
                    rootTableDir = Path.GetDirectoryName(this.FilePath);
                }

                this.TableDirPath = Path.Combine(rootTableDir!, this.CollectionName);
                this.ProcessExistingFiles();    //TODO: async task?
            }

            //TODO: kick off a job to read in any existing files that aren't in the database?
        }

        private void ProcessExistingFiles()
        {
            //// create the root table named folder in the same folder as the database file
            //IO.Directory.VerifyPath(this.TableDirPath);
            //if (!Directory.Exists(this.TableDirPath)) { throw new IOException($"Unable to create root entity directory '{this.TableDirPath}'."); }

            //// get all files in the table directory
            //var filePaths = Directory.GetFiles(this.TableDirPath, "*.json", SearchOption.AllDirectories);
            //foreach (var filePath in filePaths)
            //{
            //    try
            //    {
            //        var json = File.ReadAllText(filePath);
                    
            //        var entity = System.Text.Json.JsonSerializer.Deserialize<T>(json);
            //        if (entity != null)
            //        {
            //            // see if it exists
            //            if (this.Collection.Count(x => x.ResourceKey == entity.ResourceKey) > 0) { continue; }
                        
            //            // insert it
            //            this.Collection.Insert(entity.Id, entity);                       
            //        }
            //    }
            //    catch (Exception ex)
            //    {
            //        Log.Error(ex, "Error processing file '{FilePath}': {ErrorMessage}", filePath, ex.Message);
            //    }
            //}
        }

        #region --- Indexes ---

        private void CreateIndexes()
        {
            this.Collection.EnsureIndex(x => x.ResourceKey, false);
            this.Collection.EnsureIndex(x => x.ResourceKey, true);
            this.Collection.EnsureIndex(x => x.partition);
        }        

        #endregion

        /// <summary>
        /// Save out the entity as a file into the file system
        /// </summary>
        /// <param name="entity"></param>
        /// <remarks>
        /// File Pathing: {TableDirPath}\{PartitionKey}\{ResourceKey}.json;         
        //  </remarks>
        private void SaveItemFile(string partitionKey, string resourceKey, ItemEnvelope itemEnvelope)
        {
            //TODO: start with partitionKey (split by /)... then see if resource key starts with PartitionKey

            var filePath = this.GetFilePath(partitionKey, resourceKey);

            var options = new JsonSerializerOptions { WriteIndented = true };

            var json = System.Text.Json.JsonSerializer.Serialize(itemEnvelope, options);

            //save out entity
            File.WriteAllText(filePath, json);
        }

        private string GetFilePath(string partitionKey, string resourceKey)
        {
            if (resourceKey.StartsWith(partitionKey))
            {
                var resourcePath = Path.Combine($"{resourceKey}.json".Split('/'));

                return Path.Combine(this.TableDirPath, resourcePath); 
            }
            else
            {
                var resourcePath = Path.Combine($"{resourceKey}.json".Split('/'));
                var partitionPath = Path.Combine($"{partitionKey}".Split('/'));
                
                return Path.Combine(this.TableDirPath, partitionPath, resourcePath);
            }
        }

        #region --- Entities ---

        /// <inheritdoc />       
        public Task<bool> ExistsAsync(string partitionKey, string resourceKey, CancellationToken cancellationToken)
        {
            ArgumentException.ThrowIfNullOrEmpty(partitionKey);
            ArgumentException.ThrowIfNullOrEmpty(resourceKey);

            return Task.FromResult<bool>(this.Collection.Count(x => x.ResourceKey == resourceKey) > 0);
        }

        /// <inheritdoc />       
        public Task<ItemEnvelope?> GetAsync(string partitionKey, string resourceKey, CancellationToken cancellationToken)
        {
            ArgumentException.ThrowIfNullOrEmpty(partitionKey);
            ArgumentException.ThrowIfNullOrEmpty(resourceKey);

            var result = this.Collection.FindOne(x => x.ResourceKey == resourceKey);

            return Task.FromResult<ItemEnvelope?>(result);            
        }

        /// <inheritdoc />       
        public Task<List<ItemEnvelope>> GetAllAsync(string partitionKey, string resourceKey, CancellationToken cancellationToken)
        {
            ArgumentException.ThrowIfNullOrEmpty(partitionKey);
            ArgumentException.ThrowIfNullOrEmpty(resourceKey);

            var resourceKeyPrefix = $"{resourceKey}/";

            var items = this.Collection.Query().Where(x => x.ResourceKey == resourceKey || x.ResourceKey.StartsWith(resourceKeyPrefix)).ToList();
            
            return Task.FromResult<List<ItemEnvelope>>(items);
        }

        /// <inheritdoc />       
        public Task InsertAsync(ItemEnvelope itemEnvelope, CancellationToken cancellationToken)
        {
            ArgumentException.ThrowIfNullOrEmpty(itemEnvelope.partition);
            ArgumentException.ThrowIfNullOrEmpty(itemEnvelope.ResourceKey);
            ArgumentNullException.ThrowIfNull(itemEnvelope.Data);

            lock (LockObject)
            {
                // creates
                this.Collection.Insert(itemEnvelope);
            }

            //save out entity?
            if (this.IsFileSaving)
            {
                this.SaveItemFile(itemEnvelope.partition, itemEnvelope.ResourceKey, itemEnvelope);
            }


            return Task.CompletedTask;
        }

        public Task UpdateAsync(ItemEnvelope itemEnvelope, CancellationToken cancellationToken)
        {
            ArgumentException.ThrowIfNullOrEmpty(itemEnvelope.partition);
            ArgumentException.ThrowIfNullOrEmpty(itemEnvelope.ResourceKey);
            ArgumentNullException.ThrowIfNull(itemEnvelope.Data);

            lock (LockObject)
            {
                // creates (if missing) or replaces entity
                this.Collection.Upsert(itemEnvelope.ResourceKey, itemEnvelope);
            }

            //save out entity?
            if (this.IsFileSaving)
            {
                this.SaveItemFile(itemEnvelope.partition, itemEnvelope.ResourceKey, itemEnvelope);
            }

            return Task.CompletedTask;
        }

        /// <inheritdoc />       
        public Task DeleteAsync(string partitionKey, string resourceKey, CancellationToken cancellationToken)
        {
            ArgumentException.ThrowIfNullOrEmpty(partitionKey);
            ArgumentException.ThrowIfNullOrEmpty(resourceKey);

            lock (LockObject)
            {
                var count = this.Collection.DeleteMany(x => x.ResourceKey == resourceKey || x.ResourceKey.StartsWith($"{resourceKey}/"));
                if (count == 0)
                {
                    // if we expected records to exist then not deleting anything is a warning something is amiss.
                    Log.Warning("No records found to delete for ResourceKey '{ResourceKey}'", resourceKey);
                }
            }

            //save out entity?
            if (this.IsFileSaving)
            {
                //this.DeleteEntityFile(entity);
            }


            return Task.CompletedTask;
        }

        #endregion

        ///// <summary>
        ///// Removes the entity store
        ///// </summary>        
        ///// <param name="includeTableDir">If the working table directory</param>
        //public void DeleteStore(bool includeTableDir)
        //{
        //    this.Client.DropCollection(this.Name);

        //    if (this.IsFileSaving && !string.IsNullOrEmpty(this.TableDirPath))
        //    {
        //        // remove the directory itself
        //        IO.Directory.TryDeleteDirectory(this.TableDirPath);
        //        IO.Directory.VerifyPath(this.TableDirPath);    //restore the base folder
        //    }
        //}

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
                
                // set up the GCP objects
                this.Client.Dispose();
            }
        }

        ~ItemStoreLocal()
        {
            this.Dispose(false);
        }

        #endregion

        /// <summary>
        /// Validates the name to be used 
        /// </summary>
        /// <param name="name">Collection Name</param>
        /// <exception cref="ArgumentException"></exception>
        public static void ValidateCollectionName(string name)
        {
            // https://www.litedb.org/docs/collections/

            //Contains only letters, numbers and _
            //Collection names are case insensitive
            //Collection names starting with _ are reserved for internal storage use
            //Collection names starting with $ are reserved for internal system/virtual collections

            // The total size of all the collections names in a database is limited to 8000 bytes.If you plan to have many collections in your database, make sure to use short names for your collections. For example, if collection names are about 10 bytes in length, you can have ~800 collection in the database.

            // validate name            
            if (string.IsNullOrEmpty(name)) { throw new ArgumentException("Collection name cannot be null or empty."); }
            
            if (!char.IsLetter(name[0])) { throw new ArgumentException("Table names must start with a letter", nameof(name)); }            
            if (!name.All(c => char.IsLetterOrDigit(c) || c == '_')) { throw new ArgumentException("Table names can only be alphanumeric characters or underscores.", nameof(name)); }            
            
            if (name.Length > 8000) { throw new ArgumentException("Table names must be between 1 and 8000 characters long.", nameof(name)); }
        }

    }
}