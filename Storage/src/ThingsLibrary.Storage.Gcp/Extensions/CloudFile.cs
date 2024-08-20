using Data = Google.Apis.Storage.v1.Data;

namespace ThingsLibrary.Storage.Gcp.Extensions
{
    public static class CloudFileExtensions
    {
        public static CloudFile ToCloudFile(this Data.Object googleObject)
        {
            return new CloudFile
            {
                FilePath = googleObject.Name,

                FileSize = Convert.ToInt64(googleObject.Size),
                Version = googleObject.Generation.ToString(),

                CreatedOn = googleObject.TimeCreated,
                UpdatedOn = googleObject.Updated,

                ContentType = googleObject.ContentType,
                ContentMD5 = googleObject.Md5Hash,

                Metadata = googleObject.Metadata,
                Tag = googleObject.ETag
            };
        }
    }
}
