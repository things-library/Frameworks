// ================================================================================
// <copyright file="FileStores.cs" company="Starlight Software Co">
//    Copyright (c) Starlight Software Co. All rights reserved.
//    Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
// </copyright>
// ================================================================================

using Azure.Storage.Blobs;

namespace ThingsLibrary.Storage.Azure
{
    public class FileStores : IFileStores
    {
        /// <inheritdoc />
        public FileStoreType StoreType => FileStoreType.Azure_Blob;

        private string StorageConnectionString { get; set; }
        private BlobServiceClient BlobServiceClient { get; set; }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="storageConnectionString"></param>
        public FileStores(string storageConnectionString)
        {
            this.StorageConnectionString = storageConnectionString;
            this.BlobServiceClient = new BlobServiceClient(storageConnectionString);
        }

        /// <inheritdoc />
        public IFileStore GetStore(string bucketName)
        {
            return new FileStore(this.BlobServiceClient, bucketName);            
        }


        /// <inheritdoc />
        public IEnumerable<string> GetStoreList(CancellationToken cancellationToken = default)
        {
            var results = this.BlobServiceClient.GetBlobContainers();

            return results.Select(x => x.Name).ToList();
        }

        /// <inheritdoc />
        public void DeleteStore(string name, CancellationToken cancellationToken = default)
        {
            this.BlobServiceClient.DeleteBlobContainer(name, null, cancellationToken);
        }
    }
}
