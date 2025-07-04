// ================================================================================
// <copyright file="CacheStore.cs" company="Starlight Software Co">
//    Copyright (c) Starlight Software Co. All rights reserved.
//    Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
// </copyright>
// ================================================================================

using System.Text.Json;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Logging;

namespace ThingsLibrary.Cache
{
    public class CacheStore : ICacheStore
    {
        private readonly IDistributedCache _cache;
        private readonly ILogger<CacheStore> _logger;

        private readonly JsonSerializerOptions _serializerOptions;

        public CacheStore(IDistributedCache cache, JsonSerializerOptions? jsonSerializerOptions, ILogger<CacheStore> logger)
        {
            _cache = cache;
            _logger = logger;
            _serializerOptions = jsonSerializerOptions ?? new JsonSerializerOptions();
        }
        
        public async Task<T?> GetAsync<T>(string key, CancellationToken cancellationToken)
        {
            ArgumentException.ThrowIfNullOrEmpty(key);

            var json = await _cache.GetStringAsync(key, cancellationToken);

            // not found
            if (json == null || json.Length == 0)
            {
                _logger.LogInformation("= Cache: {Key} - NOT FOUND", key);

                return default;
            }

            try
            {
                var item = JsonSerializer.Deserialize<T>(json, _serializerOptions);

                return item;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unable to deserialze data for key: {Key}", key);

                return default;
            }
        }

        public async Task SetAsync<T>(string key, T item, DateTime expirationDate, CancellationToken cancellationToken)
        {
            var ttl = this.GetTTL(expirationDate);

            await this.SetAsync(key, item, ttl, cancellationToken);
        }
                
        public async Task SetAsync<T>(string key, T item, TimeSpan slidingExpiration, CancellationToken cancellationToken)
        {
            var ttl = this.GetTTL(slidingExpiration);

            await this.SetAsync(key, item, ttl, cancellationToken);
        }

        private async Task SetAsync<T>(string key, T item, DistributedCacheEntryOptions ttlOptions, CancellationToken cancellationToken)
        {
            // remove in case it already exists
            await this.RemoveAsync(key, cancellationToken);

            try
            {
                var json = JsonSerializer.Serialize<T>(item, _serializerOptions);

                await _cache.SetStringAsync(key, json, ttlOptions, cancellationToken);

                _logger.LogInformation("+ Cache: {Key}", key);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unable to remove key: {Key}", key);
            }
        }

        public async Task RemoveAsync(string key, CancellationToken cancellationToken)
        {
            ArgumentException.ThrowIfNullOrEmpty(key);

            try
            {
                await _cache.RemoveAsync(key, cancellationToken);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error removing cache key: {Key}", key);
            }
        }

        private DistributedCacheEntryOptions GetTTL(TimeSpan timeSpan)
        {
            return new DistributedCacheEntryOptions()
            {
                SlidingExpiration = timeSpan
            };
        }

        private DistributedCacheEntryOptions GetTTL(DateTime expirationDate)
        {
            return new DistributedCacheEntryOptions()
            {
                AbsoluteExpiration = expirationDate
            };
        }

    }
}
