using Amazon.S3.Model;

using CoreFile = Core.Cloud.Storage.File;

namespace Core.Cloud.Storage.File.AWS
{
    public class CloudFile : CoreFile.CloudFile
    {
        public GetObjectResponse AmazonCloudFile { get; set; }
        public GetObjectMetadataResponse AmazonCloudFileMetadata { get; set; }

        #region --- Properties ---

        public override DateTime? CreatedOn => null;
        public override DateTime? UpdatedOn => this.AmazonCloudFile.LastModified;

        public override string ContentType => (this.AmazonCloudFile.Headers.ContentType);
        public override string ContentMD5 => this.AmazonCloudFile.Headers.ContentMD5;
        
        public override long FileSize => (this.AmazonCloudFile?.ContentLength != null ? this.AmazonCloudFile.ContentLength : 0);

        #endregion
        

        public CloudFile(string cloudFilePath)
        {
            this.FilePath = cloudFilePath;            
        }

        public CloudFile(FileStore storageBucket, GetObjectResponse amazonCloudFile)
        {
            this.StorageBucket = storageBucket;

            this.AmazonCloudFile = amazonCloudFile;
            this.FilePath = amazonCloudFile.Key;
        }

        public override void FetchProperties()
        {
            GetObjectMetadataRequest request = new GetObjectMetadataRequest()
            {
                BucketName = this.StorageBucket.BucketName,
                Key = this.FilePath                
            };

            FileStore amazonStoragebucket = (FileStore)this.StorageBucket;

            this.AmazonCloudFileMetadata = amazonStoragebucket.AmazonStorageClient.GetObjectMetadataAsync(request).Result;
        }
    }
}
