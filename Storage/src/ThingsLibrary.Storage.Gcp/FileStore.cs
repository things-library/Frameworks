using System.Text.RegularExpressions;

using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;

using Google.Apis.Auth.OAuth2;
using Google.Apis.Download;
using Google.Apis.Upload;
using Google.Apis.Storage.v1.Data;
using Google.Cloud.Storage.V1;


using System.ComponentModel.DataAnnotations;
using Serilog;


using ThingsLibrary.Storage.Gcp.Extensions;


namespace ThingsLibrary.Storage.Gcp
{
    public class FileStore : IFileStore
    {
        // ================================================================================
        // EVENTS
        // ================================================================================
        public event EventHandler<Events.TransferProgressChangedEventArgs> TransferProgressChanged;
        public event EventHandler<Events.TransferCompleteEventArgs> TransferComplete;

        // ================================================================================
        // INTERFACE
        // ================================================================================
        /// <inheritdoc/>
        public string BucketName { get; init; }
        
        /// <inheritdoc/>
        public FileStoreType StorageType { get; init; } = FileStoreType.GCP_Storage;

        /// <inheritdoc/>
        public bool IsVersioning { get; set; } = false;

        public CancellationToken CancellationToken { get; set; } = default;

        // ================================================================================
        // VENDOR SPECIFIC (if you aren't using the interface methods) allow for exposure of vendor specific objects
        // ================================================================================
        public GoogleCredential GoogleCreds { get; init; }
        public StorageClient GoogleStorageClient { get; init; }
        public Bucket GoogleBucket { get; init; }


        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="storageConnectionString">Storage Connection String</param>
        /// <param name="bucketName">Bucket Name</param>
        /// <exception cref="ArgumentNullException"></exception>
        /// <exception cref="ArgumentException"></exception>
        public FileStore(string storageConnectionString, string bucketName)
        {
            // quick exits?
            if (string.IsNullOrEmpty(storageConnectionString)) { throw new ArgumentNullException(nameof(storageConnectionString)); }
            if (string.IsNullOrEmpty(bucketName)) { throw new ArgumentNullException(nameof(bucketName)); }

            // parse out the ProjectId and InstanceId from connectionstring
            var builder = new System.Data.Common.DbConnectionStringBuilder
            {
                ConnectionString = storageConnectionString
            };


            string bucketNamePrefix = builder.GetValue<string>("bucket_name_prefix", ""); //TODO: pluck off the prefix from the connection string

            var fullBucketName = $"{bucketNamePrefix}{bucketName}";

            // validate the bucket naming against cloud specs
            var results = FileStore.ValidateBucketName(fullBucketName);
            if (results.Count > 0) { throw new ArgumentException(string.Join(";", results)); }

            // set the core properties            
            this.BucketName = bucketName;

            string projectId = builder.GetValue<string>("project_id", null) ?? throw new ArgumentException("No 'projectId' found in connection string.");

            this.GoogleCreds = GoogleCredential.FromJsonParameters(new JsonCredentialParameters
            {
                Type = builder.GetValue<string>("type", null),
                ProjectId = projectId,
                PrivateKeyId = builder.GetValue<string>("private_key_id", null),
                PrivateKey = builder.GetValue<string>("private_key", null),
                ClientEmail = builder.GetValue<string>("client_email", null),
                ClientId = builder.GetValue<string>("client_id", null)
            });

            this.GoogleStorageClient = StorageClient.Create(this.GoogleCreds);


            // Create the container and return a container client object
            try
            {
                this.GoogleBucket = this.GoogleStorageClient.GetBucket(fullBucketName);
            }
            catch (Google.GoogleApiException e)
            when (e.Error.Code == 404)
            {
                Log.Information("Creating missing bucket: {FullBucketName} ({BucketName})...", fullBucketName, bucketName);

                // use the template bucket to get the location, storage class and access control
                var newBucket = new Bucket()
                {
                    Name = fullBucketName,
                    Location = builder.GetValue<string>("default_storage_location", "US-CENTRAL1"),
                    StorageClass = builder.GetValue<string>("default_storage_class", "STANDARD"),
                    Acl = null,
                    Versioning = new Bucket.VersioningData() { Enabled = true } // support versioning
                };

                // create the bucket
                this.GoogleBucket = this.GoogleStorageClient.CreateBucket(projectId, newBucket);
            }
        }

        #region --- Transfer Events ---

        private void OnTransferProgressChanged(long totalBytesToTransfer, IDownloadProgress progress)
        {
            switch (progress.Status)
            {
                case DownloadStatus.Downloading:
                    {
                        // prevent runaway condition if event disconnect happens between null check and event firing
                        var raiseEvent = this.TransferProgressChanged;

                        // Event will be null if there are no subscribers
                        if (raiseEvent != null)
                        {
                            this.TransferProgressChanged(this, new Events.TransferProgressChangedEventArgs(progress.BytesDownloaded, totalBytesToTransfer));
                        }

                        break;
                    }

                case DownloadStatus.Completed:
                    {
                        var raiseEvent = this.TransferComplete;

                        // Event will be null if there are no subscribers
                        if (raiseEvent != null)
                        {
                            this.TransferComplete(this, new Events.TransferCompleteEventArgs(null, false));
                        }

                        break;
                    }

                case DownloadStatus.Failed:
                    {
                        Console.WriteLine($"DOWNLOAD ERROR: {progress.Exception.Message}\r\n\r\nERROR: {progress.Exception}");

                        var raiseEvent = this.TransferComplete;

                        // Event will be null if there are no subscribers
                        if (raiseEvent != null)
                        {
                            this.TransferComplete(this, new Events.TransferCompleteEventArgs(progress.Exception, false));
                        }

                        break;
                    }
            }
        }

        private void OnTransferProgressChanged(long totalBytesToTransfer, IUploadProgress progress)
        {
            switch (progress.Status)
            {
                case UploadStatus.Uploading:
                    {
                        // prevent runaway condition if event disconnect happens between null check and event firing
                        var raiseEvent = this.TransferProgressChanged;

                        // Event will be null if there are no subscribers
                        if (raiseEvent != null)
                        {
                            this.TransferProgressChanged(this, new Events.TransferProgressChangedEventArgs(progress.BytesSent, totalBytesToTransfer));
                        }

                        break;
                    }

                case UploadStatus.Completed:
                    {
                        var raiseEvent = this.TransferComplete;

                        // Event will be null if there are no subscribers
                        if (raiseEvent != null)
                        {
                            this.TransferComplete(this, new Events.TransferCompleteEventArgs(null, false));
                        }

                        break;
                    }

                case UploadStatus.Failed:
                    {
                        Console.WriteLine($"UPLOAD ERROR: {progress.Exception.Message}\r\n\r\nERROR: {progress.Exception}");

                        var raiseEvent = this.TransferComplete;

                        // Event will be null if there are no subscribers
                        if (raiseEvent != null)
                        {
                            this.TransferComplete(this, new Events.TransferCompleteEventArgs(progress.Exception, false));
                        }

                        break;
                    }
            }
        }

        #endregion

        #region --- File ---

        public ICloudFile GetFile(string cloudFilePath)
        {
            try
            {
                
                var googleObject = this.GoogleStorageClient.GetObject(this.BucketName, cloudFilePath);
                if (googleObject == null) { return null; }

                return googleObject.ToCloudFile();
            }
            catch (Google.GoogleApiException e)
            when (e.Error.Code == 404)
            {
                return null;
            }
        }
                
        public IEnumerable<ICloudFile> GetFiles(string cloudFolderPath)
        {
            var list = this.GoogleStorageClient.ListObjects(this.BucketName, cloudFolderPath);

            return list.Select(x => x.ToCloudFile());
        }

        public IEnumerable<ICloudFile> GetFileVersions(string cloudFilePath)
        {
            var list = this.GoogleStorageClient.ListObjects(this.BucketName, cloudFilePath, new ListObjectsOptions
            {
                Versions = true
            })
            .OrderByDescending(x => x.TimeCreatedDateTimeOffset);

            return list.Select(x =>  x.ToCloudFile());            
        }

        public void UploadFile(string localFilePath, string cloudFilePath)
        {
            if (!System.IO.File.Exists(localFilePath))
            {
                throw new FileNotFoundException($"File not found at: '{localFilePath}'");
            }

            using var fileStream = System.IO.File.OpenRead(localFilePath);

            this.UploadFile(fileStream, cloudFilePath);
        }

        public void UploadFile(Stream stream, string cloudFilePath)
        {
            var contentLength = stream.Length;            
            var contentType = IO.File.GetMimeType(cloudFilePath);

            var progress = new Progress<IUploadProgress>(progress => this.OnTransferProgressChanged(contentLength, progress));

            this.GoogleStorageClient.UploadObject(this.BucketName, cloudFilePath, contentType, stream, null, progress);            
        }

        public string GetDownloadUrl(string cloudFilePath, double ttlMinutes)
        {
            var urlSigner = UrlSigner.FromServiceAccountCredential(this.GoogleCreds.UnderlyingCredential as ServiceAccountCredential);

            return urlSigner.Sign(this.BucketName, cloudFilePath, TimeSpan.FromMinutes(ttlMinutes), HttpMethod.Get);
        }


        public void DownloadFile(string cloudFilePath, string localFilePath)
        {
            using var fileStream = System.IO.File.Open(localFilePath, FileMode.Create);

            this.DownloadFile(cloudFilePath, fileStream);
        }

        public void DownloadFile(string cloudFilePath, Stream stream)
        {
            var totalBytesToTransfer = stream.Length;

            var progress = new Progress<IDownloadProgress>(progress => this.OnTransferProgressChanged(totalBytesToTransfer, progress));

            this.GoogleStorageClient.DownloadObject(this.BucketName, cloudFilePath, stream, null, progress);
        }

        public void DeleteFile(string cloudFilePath)
        {
            try
            {
                this.GoogleStorageClient.DeleteObject(this.BucketName, cloudFilePath);
            }
            catch (Google.GoogleApiException e)
            when (e.Error.Code == 404)
            {
                //nothing.. catch
            }
        }

        #endregion

        #region --- Static Methods ---

        /// <summary>
        /// Validates bucket name against cloud provider requirements, throws Argument exception on failure
        /// </summary>
        /// <param name="bucketName">Bucket Name</param>        
        /// <remarks>
        /// Bucket Naming Requirements: https://cloud.google.com/storage/docs/naming-buckets        
        /// - Bucket names can only contain lowercase letters, numeric characters, dashes (-), underscores (_), and dots(.).Spaces are not allowed.Names containing dots require verification.
        /// - Bucket names must start and end with a number or letter.
        /// - Bucket names must contain 3 - 63 characters.Names containing dots can contain up to 222 characters, but each dot - separated component can be no longer than 63 characters.
        /// - Bucket names cannot be represented as an IP address in dotted - decimal notation(for example, 192.168.5.4).
        /// - Bucket names cannot begin with the "goog" prefix.
        /// - Bucket names cannot contain "google" or close misspellings, such as "g00gle".
        /// </remarks>
        public static ICollection<ValidationResult> ValidateBucketName(string bucketName)
        {
            var results = new List<ValidationResult>();

            if (bucketName.Length < 3) { results.Add(new ValidationResult($"Bucket name '{bucketName}' must be more than 2 characters in length.")); }
            if (!char.IsLetterOrDigit(bucketName[0])) { results.Add(new ValidationResult($"Bucket name '{bucketName}' must start with a letter or a number.")); }
            if (!char.IsLetterOrDigit(bucketName[bucketName.Length - 1])) { results.Add(new ValidationResult($"Bucket name '{bucketName}' must end with a letter or number.")); }

            if (bucketName.StartsWith("goog", StringComparison.InvariantCultureIgnoreCase)) { results.Add(new ValidationResult("Bucket names can not start with 'goog'")); }
            if (bucketName.Contains("google", StringComparison.InvariantCultureIgnoreCase)) { results.Add(new ValidationResult("Bucket names can not contain 'google'")); }
            if (bucketName.Contains("g00gle", StringComparison.InvariantCultureIgnoreCase)) { results.Add(new ValidationResult("Bucket names can not contain 'g00gle' or close misspellings")); }

            if (Regex.IsMatch(bucketName, "^(25[0-5]|2[0-4][0-9]|[01]?[0-9][0-9]?)(\\.(25[0-5]|2[0-4][0-9]|[01]?[0-9][0-9]?)){3}$"))
            {
                throw new ArgumentException("Bucket names can not be named like a IP address.");
            }

            if (!Regex.IsMatch(bucketName, "^[a-z0-9_.-]*$", RegexOptions.Singleline | RegexOptions.CultureInvariant))
            {
                results.Add(new ValidationResult($"Bucket name '{bucketName}' does not meet Google Cloud Storage naming requirements."));
            }

            return results;
        }

        #endregion
    }
}
