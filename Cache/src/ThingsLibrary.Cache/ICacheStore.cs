// ================================================================================
// <copyright file="ICacheStore.cs" company="Starlight Software Co">
//    Copyright (c) Starlight Software Co. All rights reserved.
//    Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
// </copyright>
// ================================================================================

namespace ThingsLibrary.Cache
{
    public interface ICacheStore
    {        
        public Task<T?> GetAsync<T>(string key, CancellationToken cancellationToken);

        public Task SetAsync<T>(string key, T item, DateTime expirationDate, CancellationToken cancellationToken);        
        public Task SetAsync<T>(string key, T item, TimeSpan slidingExpiration, CancellationToken cancellationToken);

        public Task RemoveAsync(string key, CancellationToken cancellationToken);
    }
}