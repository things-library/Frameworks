﻿using Azure.Storage.Blobs.Models;
using Azure.Storage.Blobs;

namespace ThingsLibrary.Storage.Azure.Extensions
{
    public static class CloudFileExtensions
    {
        public static CloudFile ToCloudFile(this BlobItem blob)
        {
            var item = new CloudFile(blob.Name)
            {
                Attributes =
                {
                    { "version", blob.VersionId }
                },
                Metadata = blob.Metadata
            };

            if (blob.Properties != null)
            {
                item.Add(new Dictionary<string, object>()
                {
                    // blob items
                    { "size", blob.Properties.ContentLength },
                    { "content_type", blob.Properties.ContentType },
                    { "content_md5", (blob.Properties.ContentHash != null ? Convert.ToBase64String(blob.Properties.ContentHash) : null) },
                    { "content_etag", blob.Properties.ETag.ToString() },
                    
                    { "created",  blob.Properties.CreatedOn },
                    { "updated",  blob.Properties.LastModified }
                });                
            }

            return item;
        }

        public static CloudFile ToCloudFile(this BlobClient fromEntity)
        {
            var item = new CloudFile(fromEntity.Name);
            
            var response = fromEntity.GetProperties();
            if (response != null)
            {
                var properties = response.Value;
                                
                item.Metadata = properties.Metadata;
                
                item.Add(new Dictionary<string, object>
                {
                    { "version", properties.VersionId },

                    { "size", properties.ContentLength },
                    { "content_type", properties.ContentType },
                    { "content_md5", (properties.ContentHash != null ? Convert.ToBase64String(properties.ContentHash) : null) },
                    { "content_etag", properties.ETag.ToString() },

                    { "created",  properties.CreatedOn },
                    { "updated",  properties.LastModified }                    
                });     
            }
            
            return item;
        }
    }
}
