// ================================================================================
// <copyright file="MongoCache.cs" company="Starlight Software Co">
//    Copyright (c) Starlight Software Co. All rights reserved.
//    Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
// </copyright>
// ================================================================================

using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Options;

using MongoDB.Driver;

namespace ThingsLibrary.Cache.MongoDB
{
    internal class MongoCache : IDistributedCache, IDisposable
    {
        //https://github.com/Azure/Microsoft.Extensions.Caching.Cosmos/blob/master/src/CosmosCache.cs

        private MongoCacheOptions _options;
        private MongoClient _mongoClient;
        public IMongoCollection _collection;

        private bool _isInitialized = false;
        private bool _isDisposed = false;

        /// <summary>
        /// Initializes a new instance of the <see cref="CosmosCache"/> class.
        /// </summary>
        /// <param name="optionsAccessor">Options accessor.</param>
        public MongoCache(IOptions<MongoCacheOptions> optionsAccessor)
        {
            
            this.Initialize(optionsAccessor);
        }

        /// <inheritdoc/>
        public void Dispose()
        {
            if (this._isDisposed)
            {
                return;
            }

            if (this._isInitialized && this._mongoClient != null)
            {
                this._mongoClient.Dispose();
            }

            //this.monitorListener?.Dispose();

            this._isDisposed = true;
        }

        /// <inheritdoc/>
        public byte[] Get(string key)
        {
#pragma warning disable VSTHRD002 // Avoid problematic synchronous waits
            return this.GetAsync(key).GetAwaiter().GetResult();
#pragma warning restore VSTHRD002 // Avoid problematic synchronous waits
        }

        public async Task<byte[]> GetAsync(string key, CancellationToken token = default(CancellationToken))
        {
            token.ThrowIfCancellationRequested();

            ArgumentNullException.ThrowIfNullOrEmpty(key);


        }

        public void Refresh(string key)
        {
#pragma warning disable VSTHRD002 // Avoid problematic synchronous waits
            this.RefreshAsync(key).GetAwaiter().GetResult();
#pragma warning restore VSTHRD002 // Avoid problematic synchronous waits
        }

        public async Task RefreshAsync(string key, CancellationToken token = default(CancellationToken))
        {
            token.ThrowIfCancellationRequested();

            ArgumentNullException.ThrowIfNullOrEmpty(key);

        }

        /// <inheritdoc/>
        public void Remove(string key)
        {
#pragma warning disable VSTHRD002 // Avoid problematic synchronous waits
            this.RemoveAsync(key).GetAwaiter().GetResult();
#pragma warning restore VSTHRD002 // Avoid problematic synchronous waits
        }

        /// <inheritdoc/>
        public async Task RemoveAsync(string key, CancellationToken token = default(CancellationToken))
        {
            token.ThrowIfCancellationRequested();

            ArgumentNullException.ThrowIfNullOrEmpty(key);
        }

        /// <inheritdoc/>
        public void Set(string key, byte[] value, DistributedCacheEntryOptions options)
        {
#pragma warning disable VSTHRD002 // Avoid problematic synchronous waits
            this.SetAsync(key, value, options).GetAwaiter().GetResult();
#pragma warning restore VSTHRD002 // Avoid problematic synchronous waits
        }

        /// <inheritdoc/>
        public async Task SetAsync(string key, byte[] value, DistributedCacheEntryOptions options, CancellationToken token = default(CancellationToken))
        {
            token.ThrowIfCancellationRequested();

            ArgumentNullException.ThrowIfNullOrEmpty(key);
            ArgumentNullException.ThrowIfNull(value);
        }

        private static long? GetExpirationInSeconds(DateTimeOffset creationTime, DateTimeOffset? absoluteExpiration, DistributedCacheEntryOptions options)
        {
            if (absoluteExpiration.HasValue && options.SlidingExpiration.HasValue)
            {
                return (long)System.Math.Min((absoluteExpiration.Value - creationTime).TotalSeconds, options.SlidingExpiration.Value.TotalSeconds);
            }
            else if (absoluteExpiration.HasValue)
            {
                return (long)(absoluteExpiration.Value - creationTime).TotalSeconds;
            }
            else if (options.SlidingExpiration.HasValue)
            {
                return (long)options.SlidingExpiration.Value.TotalSeconds;
            }

            return null;
        }

        private static DateTimeOffset? GetAbsoluteExpiration(DateTimeOffset creationTime, DistributedCacheEntryOptions options)
        {
            if (options.AbsoluteExpiration.HasValue && options.AbsoluteExpiration <= creationTime)
            {
                throw new ArgumentOutOfRangeException(nameof(DistributedCacheEntryOptions.AbsoluteExpiration), options.AbsoluteExpiration.Value, "The absolute expiration value must be in the future.");
            }

            DateTimeOffset? absoluteExpiration = options.AbsoluteExpiration;
            if (options.AbsoluteExpirationRelativeToNow.HasValue)
            {
                absoluteExpiration = creationTime + options.AbsoluteExpirationRelativeToNow;
            }

            return absoluteExpiration;
        }


        private void Initialize(IOptions<MongoCacheOptions> optionsAccessor)
        {
            if (optionsAccessor == null)
            {
                throw new ArgumentNullException(nameof(optionsAccessor));
            }

            if (string.IsNullOrEmpty(optionsAccessor.Value.DatabaseName))
            {
                throw new ArgumentNullException(nameof(optionsAccessor.Value.DatabaseName));
            }

            if (string.IsNullOrEmpty(optionsAccessor.Value.ContainerName))
            {
                throw new ArgumentNullException(nameof(optionsAccessor.Value.ContainerName));
            }

            //if (optionsAccessor.Value.ClientBuilder == null && optionsAccessor.Value.CosmosClient == null)
            //{
            //    throw new ArgumentNullException("You need to specify either a CosmosConfiguration or an existing CosmosClient in the CosmosCacheOptions.");
            //}

            this._options = optionsAccessor.Value;
        }

        private void OnOptionsChange(MongoCacheOptions options)
        {
            // Did we create our own internal client? If so, we need to dispose it.
            if (this._isInitialized && this._mongoClient != null)
            {
                // In case this becomes an issue with concurrent access to the client, we can see if ReaderWriterLockSlim can be leveraged.
                this._mongoClient.Dispose();
            }

            this._options = options;

            // Force re-initialization on the next Connect
            this._collection = null;
        }

        //private MongoClient GetClientInstance()
        //{
        //    if (this._options.MongoClient != null)
        //    {
        //        return this._options.MongoClient;
        //    }

        //    if (this._options.ClientBuilder == null)
        //    {
        //        throw new ArgumentNullException(nameof(this.options.ClientBuilder));
        //    }

        //    return new MongoClient(this._options.ConnectionString this._options.ClientBuilder
        //            .WithApplicationName(CosmosCache.UseUserAgentSuffix)
        //            .Build();
        //}
    }
}
