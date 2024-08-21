namespace ThingsLibrary.Storage.Gcp.Extensions
{
    public static class CloudFileExtensions
    {
        public static FileItem ToCloudFile(this Google.Apis.Storage.v1.Data.Object googleObject)
        {            
            var item = new FileItem(googleObject.Name)
            {
                Metadata = googleObject.Metadata
            };

            item.Add(new Dictionary<string, object>()
            {            
                { "size", Convert.ToInt64(googleObject.Size) },
                { "content_type", googleObject.ContentType },
                { "content_md5", googleObject.Md5Hash },
                { "content_etag", googleObject.ETag },
                    
                { "version", googleObject.Generation.ToString() },

                { "created",  googleObject.TimeCreatedDateTimeOffset },
                { "updated",  googleObject.UpdatedDateTimeOffset }
            });            

            return item;
        }
    }
}
