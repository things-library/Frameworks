// ================================================================================
// <copyright file="Class1.cs" company="Starlight Software Co">
//    Copyright (c) Starlight Software Co. All rights reserved.
//    Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
// </copyright>
// ================================================================================

using Microsoft.Extensions.Options;
using MongoDB.Driver;

namespace ThingsLibrary.Cache.MongoDB
{
    public class MongoCacheOptions : IOptions<MongoCacheOptions>
    {
        //https://github.com/Azure/Microsoft.Extensions.Caching.Cosmos/blob/master/src/CosmosCacheOptions.cs
        //https://github.com/tinglesoftware/dotnet-extensions/blob/4feb8d403abca28179f3f1ecc062c6ecfa322ea1/src/Tingle.Extensions.Caching.MongoDB/MongoCacheOptions.cs

        public MongoCacheOptions() { }

        ///// <summary>
        ///// Gets or sets an instance of <see cref="CosmosClientBuilder"/> to build a Cosmos Client with. Either use this or provide an existing <see cref="CosmosClient"/>.
        ///// </summary>
        //public CosmosClientBuilder ClientBuilder { get; set; }

        /// <summary>
        /// Gets or sets an existing CosmosClient to use for the storage operations. Either use this or provide a <see cref="ClientBuilder"/> to provision a client.
        /// </summary>
        public MongoClient MongoClient { get; set; }

        /// <summary>
        /// Gets or sets the database name to store the cache.
        /// </summary>
        public string DatabaseName { get; set; }

        /// <summary>
        /// Gets or sets the container name to store the cache.
        /// </summary>
        public string ContainerName { get; set; } = "cache";

        /// <summary>
        /// Gets or sets a value indicating whether initialization it will check for the Container existence and create it if it doesn't exist using <see cref="ContainerThroughput"/> as provisioned throughput and <see cref="DefaultTimeToLiveInMs"/>.
        /// </summary>
        /// <value>Default value is false.</value>
        public bool CreateIfNotExists { get; set; } = false;

        /// <summary>
        /// Gets or sets a value indicating the name of the property used as Partition Key on the Container.
        /// </summary>
        /// <remarks>
        /// If <see cref="CreateIfNotExists"/> is true, it will be used to define the Partition Key of the created Container.
        /// </remarks>
        /// <value>Default value is "id".</value>
        public string ContainerPartitionKeyAttribute { get; set; }


        /// <summary>
        /// Gets or sets the default Time to Live for the Container in case <see cref="CreateIfNotExists"/> is true and the Container does not exist.
        /// </summary>
        public int? DefaultTimeToLiveInMs { get; set; }

        /// <summary>
        /// Gets the current options values.
        /// </summary>
        MongoCacheOptions IOptions<MongoCacheOptions>.Value
        {
            get { return this; }
        }
    }
}
