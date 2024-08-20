using Azure.Storage.Blobs.Models;
using Azure.Storage.Blobs;

namespace ThingsLibrary.Storage.Azure.Extensions
{
    public static class CloudFileExtensions
    {
        public static CloudFile ToCloudFile(this BlobItem blob)
        {
            if (blob.Properties != null)
            {
                return new CloudFile
                {
                    // blob items                    
                    FilePath = blob.Name,
                    Version = blob.VersionId,
                    Metadata = blob.Metadata,

                    // properties
                    FileSize = (long)blob.Properties.ContentLength,
                    CreatedOn = blob.Properties.CreatedOn,
                    UpdatedOn = blob.Properties.LastModified,

                    ContentType = blob.Properties.ContentType,
                    ContentMD5 = (blob.Properties.ContentHash != null ? Convert.ToBase64String(blob.Properties.ContentHash) : null),

                    Tag = blob.Properties.ETag.ToString()
                };
            }
            else
            {
                return new CloudFile
                {
                    // blob items                    
                    FilePath = blob.Name,
                    Version = blob.VersionId,
                    Metadata = blob.Metadata
                };
            }

        }

        public static CloudFile ToCloudFile(this BlobClient fromEntity)
        {
            var response = fromEntity.GetProperties();

            if (response != null)
            {
                var properties = response.Value;

                return new CloudFile
                {
                    FilePath = fromEntity.Name,

                    FileSize = properties.ContentLength,

                    CreatedOn = properties.CreatedOn,
                    UpdatedOn = properties.LastModified,

                    ContentType = properties.ContentType,
                    ContentMD5 = (properties.ContentHash != null ? Convert.ToBase64String(properties.ContentHash) : null),

                    Metadata = properties.Metadata,

                    Tag = properties.ETag.ToString()
                };
            }
            else
            {
                return new CloudFile
                {
                    FilePath = fromEntity.Name
                };
            }
        }
    }
}
