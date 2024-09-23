// ================================================================================
// <copyright file="FileStore.cs" company="Starlight Software Co">
//    Copyright (c) Starlight Software Co. All rights reserved.
//    Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
// </copyright>
// ================================================================================

using System.Text.RegularExpressions;

using Amazon.S3;
using Amazon.S3.Model;
using Amazon.S3.Transfer;
using ThingsLibrary.Storage.Aws.Extensions;

namespace ThingsLibrary.Storage.Aws
{
    public class FileStore : IFileStore
    {
        // TRANSFER EVENTS
        public event EventHandler<Events.TransferProgressChangedEventArgs> TransferProgressChanged;
        public event EventHandler<Events.TransferCompleteEventArgs> TransferComplete;

        // ================================================================================
        // INTERFACE
        // ================================================================================
        /// <inheritdoc/>
        public string BucketName { get; init; }
        
        /// <inheritdoc/>
        public FileStoreType StorageType { get; init; } = FileStoreType.AWS_S3;

        /// <inheritdoc/>
        public bool IsVersioning { get; set; } = false;

        public CancellationToken CancellationToken { get; set; } = default;

        // ================================================================================
        // VENDOR SPECIFIC (if you aren't using the interface methods) allow for exposure of vendor specific objects
        // ================================================================================
        public AmazonS3Client AmazonStorageClient { get; set; }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="storageConnectionString"></param>
        /// <param name="bucketName"></param>
        /// <exception cref="ArgumentNullException"></exception>
        public FileStore(string storageConnectionString, string bucketName)
        {
            // quick exits?
            if (string.IsNullOrEmpty(storageConnectionString)) { throw new ArgumentNullException(nameof(storageConnectionString)); }
            if (string.IsNullOrEmpty(bucketName)) { throw new ArgumentNullException(nameof(bucketName)); }
            
            // validate the bucket naming against cloud specs
            FileStore.ValidateBucketName(bucketName);

            // set the core properties            
            this.BucketName = bucketName;

            //var regionName = metadata.GetValueOrDefault("cloud_storage_region_name");
            //var accessKey = metadata.GetValueOrDefault("cloud_storage_key");
            //var secretKey = metadata.GetValueOrDefault("cloud_storage_secret");

            //var config = new AmazonS3Config();
            //config.RegionEndpoint = RegionEndpoint.GetBySystemName(regionName);

            //this.AmazonStorageClient = new AmazonS3Client(accessKey, secretKey, config);            
        }

        //public override void CreateIfNotExists()
        //{
        //    // see if the bucket already exists
        //    if(this.Exists()) { return; }

        //    try
        //    {
        //        var putBucketRequest = new PutBucketRequest
        //        {
        //            BucketName = this.BucketName,
        //            UseClientRegion = true,
        //            CannedACL = S3CannedACL.Private
        //        };

        //        var putBucketResponse = this.AmazonStorageClient.PutBucketAsync(putBucketRequest).Result;

        //        // Retrieve the bucket location.
        //        var bucketLocation = this.GetBucketLocation();
        //    }
        //    catch (AggregateException e)
        //    when(e.InnerException is AmazonS3Exception)
        //    {
        //        var ex = (AmazonS3Exception)e.InnerException;

        //        throw ex;
        //    }
        //}

        //public string GetBucketLocation()
        //{
        //    var request = new GetBucketLocationRequest()
        //    {
        //        BucketName = this.BucketName
        //    };

        //    var response = this.AmazonStorageClient.GetBucketLocationAsync(request).Result;

        //    return response.Location.ToString();
        //}

        #region --- Transfer Events ---

        private void OnTransferProgressChanged(long totalBytesTransferred, long totalBytesToTransfer)
        {
            // Make a temporary copy of the event to avoid possibility of a race condition if the last subscriber unsubscribes immediately after the null check and before the event is raised.
            var raiseEvent = this.TransferProgressChanged;

            // Event will be null if there are no subscribers
            if (raiseEvent != null)
            {
                this.TransferProgressChanged(this, new Events.TransferProgressChangedEventArgs(totalBytesTransferred, totalBytesToTransfer));
            }
        }

        private void OnTransferComplete(Exception error, bool cancelled)
        {
            // Make a temporary copy of the event to avoid possibility of a race condition if the last subscriber unsubscribes immediately after the null check and before the event is raised.
            var raiseEvent = this.TransferProgressChanged;

            // Event will be null if there are no subscribers
            if (raiseEvent != null)
            {
                this.TransferComplete(this, new Events.TransferCompleteEventArgs(error, cancelled));
            }
        }

        #endregion

        #region --- File ---

        public IFileItem GetFile(string cloudFilePath)
        {
            try
            {
                var request = new GetObjectRequest()
                {
                    BucketName = this.BucketName,
                    Key = cloudFilePath
                };

                var response = this.AmazonStorageClient.GetObjectAsync(request).Result;

                return response.ToCloudFile();
            }
            catch (AggregateException e)
            when (e.InnerException is AmazonS3Exception exception && exception.StatusCode == System.Net.HttpStatusCode.NotFound)
            {
                return null;
            }
        }

        public IEnumerable<IFileItem> GetFileVersions(string cloudFilePath)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<IFileItem> GetFiles(string cloudFolderPath)
        {
            throw new NotImplementedException();
        }

        public void UploadFile(string localFilePath, string cloudFilePath)
        {
            //EXAMPLE:  https://docs.aws.amazon.com/AmazonS3/latest/dev/HLuploadFileDotNet.html

            if (!System.IO.File.Exists(localFilePath))
            {
                throw new FileNotFoundException($"File not found at: '{localFilePath}'");
            }
            
            try
            {
                var transferUtility = new TransferUtility(this.AmazonStorageClient);

                transferUtility.Upload(localFilePath, this.BucketName, cloudFilePath);
            }
            catch (AmazonS3Exception amazonS3Exception)
            {
                if (amazonS3Exception.ErrorCode != null && (amazonS3Exception.ErrorCode.Equals("InvalidAccessKeyId") || amazonS3Exception.ErrorCode.Equals("InvalidSecurity")))
                {
                    throw new Exception("Check the provided AWS Credentials.");
                }
                else
                {
                    throw new Exception("Error occurred: " + amazonS3Exception.Message);
                }
            }
        }

        public void UploadFile(Stream stream, string cloudFilePath)
        {            
            var contentMD5 = IO.File.ComputeMD5Base64(stream);
            var contentType = IO.File.GetMimeType(cloudFilePath);

            var memoryStream = new MemoryStream();
            stream.CopyTo(memoryStream);
            memoryStream.Position = 0;

            try
            {
                var request = new TransferUtilityUploadRequest()
                {
                    BucketName = this.BucketName,
                    Key = cloudFilePath,
                    ContentType = contentType,
                    InputStream = memoryStream                    
                };
                request.Headers.ContentType = contentType;
                request.Headers.ContentMD5 = contentMD5;
                request.Headers.ContentLength = memoryStream.Length;

                var transferUtility = new TransferUtility(this.AmazonStorageClient);
                transferUtility.Upload(request);                
            }
            catch (AmazonS3Exception amazonS3Exception)
            {
                if (amazonS3Exception.ErrorCode != null && (amazonS3Exception.ErrorCode.Equals("InvalidAccessKeyId") || amazonS3Exception.ErrorCode.Equals("InvalidSecurity")))
                {
                    throw new AccessViolationException("Check the provided AWS Credentials.");
                }
                else
                {
                    throw new HttpRequestException($"Error occurred: {amazonS3Exception.Message}");
                }
            }
        }


        public void DownloadFile(string cloudFilePath, string localFilePath)
        {
            try
            {
                var transferUtility = new TransferUtility(this.AmazonStorageClient);

                transferUtility.Download(localFilePath, this.BucketName, cloudFilePath);
            }
            catch (AmazonS3Exception amazonS3Exception)
            {
                if (amazonS3Exception.ErrorCode != null && (amazonS3Exception.ErrorCode.Equals("InvalidAccessKeyId") || amazonS3Exception.ErrorCode.Equals("InvalidSecurity")))
                {
                    throw new AccessViolationException("Check the provided AWS Credentials.");
                }
                else
                {
                    throw new HttpRequestException($"Error occurred: {amazonS3Exception.Message}");
                }
            }
        }

        public void DownloadFile(string cloudFilePath, Stream stream)
        {
            throw new NotImplementedException();
        }

        //private async Task<Stream> InternalCopyTo(Stream source)
        //{
        //    var destination = new MemoryStream();

        //    int num;
        //    var buffer = new byte[16 * 1024];
        //    while ((num = await source.ReadAsync(buffer, 0, buffer.Length)) != 0)
        //    {
        //        destination.Write(buffer, 0, num);
        //    }

        //    // move back to beginning
        //    destination.Position = 0;

        //    return destination;
        //}


        public void DeleteFile(string cloudFilePath)
        {
            try
            {
                var request = new DeleteObjectRequest()
                {
                    BucketName = this.BucketName,
                    Key = cloudFilePath
                };

                this.AmazonStorageClient.DeleteObjectAsync(request, this.CancellationToken).Wait();
            }
            catch (AggregateException e)
            when (e.InnerException is AmazonS3Exception exception && exception.StatusCode == System.Net.HttpStatusCode.NotFound)
            {
                //nothing
            }
        }

        #endregion

        public string GetDownloadUrl(string cloudFilePath, double ttlMinutes)
        {
            var request = new GetPreSignedUrlRequest()
            {
                BucketName = this.BucketName,
                Key = cloudFilePath,
                Expires = DateTime.UtcNow.AddMinutes(ttlMinutes)
            };

            return this.AmazonStorageClient.GetPreSignedURL(request);
        }

        #region --- Static Methods ---

        /// <summary>
        /// Validates the name of a bucket meets the vendor requirements
        /// </summary>
        /// <remarks>
        /// The following rules apply for naming buckets in Amazon S3:  https://docs.aws.amazon.com/AmazonS3/latest/userguide/bucketnamingrules.html
        /// - Bucket names must be between 3 (min) and 63 (max) characters long.
        /// - Bucket names can consist only of lowercase letters, numbers, dots (.), and hyphens (-).
        /// - Bucket names must begin and end with a letter or number.
        /// - Bucket names must not be formatted as an IP address (for example, 192.168.5.4).
        /// - Bucket names must not start with the prefix xn--.
        /// - Bucket names must not end with the suffix -s3 alias. This suffix is reserved for access point alias names. For more information, see Using a bucket-style alias for your access point.
        /// - Bucket names must be unique across all AWS accounts in all the AWS Regions within a partition. A partition is a grouping of Regions. AWS currently has three partitions: aws (Standard Regions), aws-cn (China Regions), and aws-us-gov (AWS GovCloud (US)).
        /// - A bucket name cannot be used by another AWS account in the same partition until the bucket is deleted.
        /// - Buckets used with Amazon S3 Transfer Acceleration can't have dots (.) in their names. For more information about Transfer Acceleration, see Configuring fast, secure file transfers using Amazon S3 Transfer Acceleration.
        /// </remarks>
        /// <param name="bucketName"></param>
        public static void ValidateBucketName(string bucketName)
        {
            if (bucketName.Length < 3 || bucketName.Length > 63) { throw new ArgumentException($"Table name '{bucketName}' must be between 3 and 63 characters long."); }            
            if (!char.IsLetterOrDigit(bucketName[0])) { throw new ArgumentException($"Bucket name '{bucketName}' must start with a letter or a number."); }
            if (!char.IsLetterOrDigit(bucketName[bucketName.Length - 1])) { throw new ArgumentException($"Bucket name '{bucketName}' must end with a letter or number."); }
            if (bucketName.Any(char.IsUpper)) { throw new ArgumentException($"Bucket name '{bucketName}' must not have any uppercase letters."); }

            if (bucketName.StartsWith("xn--", StringComparison.InvariantCultureIgnoreCase)) { throw new ArgumentException("Bucket names can not start with 'xn--'"); }
            if (bucketName.EndsWith("-s3alias", StringComparison.InvariantCultureIgnoreCase)) { throw new ArgumentException("Bucket names can not end with '-s3alias'"); }
                        
            if(Regex.IsMatch(bucketName, "^(25[0-5]|2[0-4][0-9]|[01]?[0-9][0-9]?)(\\.(25[0-5]|2[0-4][0-9]|[01]?[0-9][0-9]?)){3}$"))
            {
                throw new ArgumentException("Bucket names can not be named like a IP address.");
            }

            if (!Regex.IsMatch(bucketName, "^[a-z0-9.-]*$", RegexOptions.Singleline | RegexOptions.CultureInvariant))
            {
                throw new ArgumentException($"Bucket name '{bucketName}' does not meet AWS S3 naming requirements.");
            }
        }

        #endregion
    }
}
