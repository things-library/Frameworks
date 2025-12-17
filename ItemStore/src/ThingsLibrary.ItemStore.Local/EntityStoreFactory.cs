// ================================================================================
// <copyright file="EntityStoreFactory.cs" company="Starlight Software Co">
//    Copyright (c) 2025 Starlight Software Co. All rights reserved.
//    Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
// </copyright>
// ================================================================================

namespace ThingsLibrary.Entity.Local
{

    public class EntityStoreFactory : IEntityStores
    {
        public EntityStoreType StoreType => EntityStoreType.Local;

        private string StorageConnectionString { get; set; }
        private LiteDatabase DataContext { get; set; }

        /// <inheritdoc />
        public EntityStoreFactory(string storageConnectionString, string databaseName)
        {
            this.StorageConnectionString = storageConnectionString;
            this.DataContext = new LiteDatabase(this.StorageConnectionString);
        }

        /// <inheritdoc />
        public IEntityStore<T> GetStore<T>(string name) where T : Entity
        {
            return new EntityStore<T>(this.StorageConnectionString, name);
        }

        /// <inheritdoc />
        public Task<IEnumerable<string>> GetStoreListAsync(CancellationToken cancellationToken = default)
        {
            return Task.FromResult(this.DataContext.GetCollectionNames());
        }

        /// <inheritdoc />
        public Task DeleteStoreAsync(string name, CancellationToken cancellationToken = default)
        {
            this.DataContext.DropCollection(name);

            return Task.CompletedTask;
        }        
    }
}