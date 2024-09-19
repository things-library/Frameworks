using System.Text.RegularExpressions;

using Azure.Storage;
using Azure.Storage.Sas;
using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;

using ThingsLibrary.Storage.Azure.Extensions;

namespace ThingsLibrary.Storage.Azure
{
    public class FileStore : IFileStore
    {        
        #region --- Transfer Events ---

        public event EventHandler<Events.TransferProgressChangedEventArgs>? TransferProgressChanged;
        public event EventHandler<Events.TransferCompleteEventArgs>? TransferComplete;

        private void OnTransferProgressChanged(long totalBytesTransferred, long totalBytesToTransfer)
        {
            // Make a temporary copy of the event to avoid possibility of a race condition if the last subscriber unsubscribes immediately after the null check and before the event is raised.
            var raiseEvent = this.TransferProgressChanged;

            // Event will be null if there are no subscribers
            if (raiseEvent != null && this.TransferProgressChanged != null)
            {
                this.TransferProgressChanged(this, new Events.TransferProgressChangedEventArgs(totalBytesTransferred, totalBytesToTransfer));
            }
        }

        private void OnTransferComplete(Exception? error, bool cancelled)
        {
            // Make a temporary copy of the event to avoid possibility of a race condition if the last subscriber unsubscribes immediately after the null check and before the event is raised.
            var raiseEvent = this.TransferProgressChanged;

            // Event will be null if there are no subscribers
            if (raiseEvent != null && this.TransferComplete != null)
            {
                this.TransferComplete(this, new Events.TransferCompleteEventArgs(error, cancelled));
            }
        }

        #endregion

        // ================================================================================
        // INTERFACE
        // ================================================================================
        /// <inheritdoc/>
        public string BucketName { get; private set; } = string.Empty;

        /// <inheritdoc/>
        public FileStoreType StorageType { get; init; } = FileStoreType.Azure_Blob;

        /// <inheritdoc/>
        public bool IsVersioning { get; set; } = false;

        public CancellationToken CancellationToken { get; set; } = default;

        // ================================================================================
        // VENDOR SPECIFIC (if you aren't using the interface methods) allow for exposure of vendor specific objects
        // ================================================================================
        public BlobContainerClient AzureContainerClient { get; set; } = null;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="blobServiceClient">Blob Service Client</param>
        /// <param name="bucketName">Bucket Name</param>
        /// <exception cref="ArgumentNullException"></exception>
        public FileStore(BlobServiceClient blobServiceClient, string bucketName)
        {            
            this.Init(blobServiceClient, bucketName);
        }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="storageConnectionString">Connection String</param>
        /// <param name="bucketName">Bucket Name</param>
        /// <exception cref="ArgumentNullException"></exception>
        public FileStore(string storageConnectionString, string bucketName)
        {
            // quick exits?
            if (string.IsNullOrEmpty(storageConnectionString)) { throw new ArgumentNullException(nameof(storageConnectionString)); }
            if (string.IsNullOrEmpty(bucketName)) { throw new ArgumentNullException(nameof(bucketName)); }

            // validate the bucket naming against cloud specs
            FileStore.ValidateBucketName(bucketName);

            // Create a BlobServiceClient object which will be used to create a container client
            var blobServiceClient = new BlobServiceClient(storageConnectionString);
                        
            this.Init(blobServiceClient, bucketName);
        }

        private void Init(BlobServiceClient blobServiceClient, string bucketName)
        {
            // quick exits?
            if (blobServiceClient == null) { throw new ArgumentNullException(nameof(blobServiceClient)); }
            if (string.IsNullOrEmpty(bucketName)) { throw new ArgumentNullException(nameof(bucketName)); }

            // validate the bucket naming against cloud specs
            FileStore.ValidateBucketName(bucketName);

            // set the core properties            
            this.BucketName = bucketName;
                        
            // Create the container and return a container client object
            this.AzureContainerClient = blobServiceClient.GetBlobContainerClient(bucketName);

            // create the container
            this.AzureContainerClient.CreateIfNotExists(PublicAccessType.None);
        }


        #region --- File ---

        /// <inheritdoc/>
        public IFileItem? GetFile(string cloudFilePath)
        {
            Log.Debug("Getting File: '{CloudFilePath}'...", cloudFilePath);

            var blobClient = this.AzureContainerClient.GetBlobClient(cloudFilePath);

            // google doesn't return empty objects if they don't exist.. so don't return a object if it doesn't exist in blob
            if (!blobClient.Exists()) { return null; }

            return blobClient.ToCloudFile();
        }

        /// <inheritdoc/>
        public IEnumerable<IFileItem> GetFileVersions(string cloudFilePath)
        {
            Log.Debug("Getting File Versions: '{CloudFilePath}'...", cloudFilePath);

            var blobs = this.AzureContainerClient.GetBlobs(BlobTraits.All, BlobStates.Version, prefix: cloudFilePath, this.CancellationToken)
                .OrderByDescending(version => version.VersionId).Where(blob => blob.Name == cloudFilePath);

            return blobs.Select(x => x.ToCloudFile());            
        }

        /// <inheritdoc/>
        public IEnumerable<IFileItem> GetFiles(string pathPrefix)
        {
            // tack it on if it is missing
            if (!string.IsNullOrEmpty(pathPrefix) && !pathPrefix.EndsWith("/"))
            {
                pathPrefix += "/";
            }

            Log.Debug("Getting Cloud Files For Prefix: '{CloudFolderPath}'...", pathPrefix);

            var blobs = this.AzureContainerClient.GetBlobs(BlobTraits.None, BlobStates.None, pathPrefix, this.CancellationToken);

            return blobs.Select(x => x.ToCloudFile());
        }

        /// <inheritdoc/>
        public void UploadFile(string localFilePath, string cloudFilePath)
        {
            if (!System.IO.File.Exists(localFilePath))
            {
                Log.Error("Local File Not Found: '{LocalFilePath}'...", localFilePath);
                throw new FileNotFoundException($"File not found at: '{localFilePath}'");
            }

            // Open the file and upload its data
            using var fileStream = System.IO.File.OpenRead(localFilePath);

            Log.Debug("Uploading Local File: '{LocalFilePath}' To '{CloudFolderPath}'...", localFilePath, cloudFilePath);
            this.UploadFile(fileStream, cloudFilePath);
        }

        /// <inheritdoc/>
        public void UploadFile(Stream stream, string cloudFilePath)
        {
            var contentLength = stream.Length;
            var contentMD5 = IO.File.ComputeMD5Base64(stream);
            var contentType = IO.File.GetMimeType(cloudFilePath);

            var contentHashBytes = (byte[])null;
            if (!string.IsNullOrEmpty(contentMD5))
            {
                contentHashBytes = Convert.FromBase64String(contentMD5);
            }

            // Get a reference to a blob
            var headers = new BlobHttpHeaders
            {
                ContentType = contentType,
                ContentHash = contentHashBytes                
            };

            //Initialize a progress handler. When the file is being uploaded, the current uploaded bytes will be published back to us using this progress handler by the Blob Storage Service
            var progressHandler = new Progress<long>(progress => this.OnTransferProgressChanged(contentLength, progress));
                        
            // get the client handler to use            
            var blobClient = this.AzureContainerClient.GetBlobClient(cloudFilePath);

            // fire off at least the first event
            this.OnTransferProgressChanged(0, contentLength);

            Log.Debug("Uploading Stream To '{CloudFolderPath}', Length: {TransferSize}...", cloudFilePath, contentLength);

            // Provide the client configuration options for connecting to Azure Blob Storage
            var blobOptions = new BlobUploadOptions()
            {
                HttpHeaders = headers,
                ProgressHandler = progressHandler,

                TransferOptions = new StorageTransferOptions
                {
                    MaximumConcurrency = 16,

                    // Set the initial transfer length to 8 MiB
                    InitialTransferSize = 4 * 1024 * 1024,

                    // Set the maximum length of a transfer to 4 MiB (per request)
                    MaximumTransferSize = 4 * 1024 * 1024
                }
            };
                        
            blobClient.Upload(stream, options: blobOptions, this.CancellationToken);

            //// do the transfer
            //blobClient.Upload(
            //    content: stream,
            //    httpHeaders: headers,                
            //    metadata: null,
            //    conditions: null,
            //    progressHandler: progressHandler,
            //    accessTier: null, 
            //    transferOptions: new StorageTransferOptions(), 
            //    cancellationToken: this.CancellationToken);

            // fire off transfer complete events
            this.OnTransferComplete(null, false);
        }

        /// <inheritdoc/>
        public void DownloadFile(string cloudFilePath, string localFilePath)
        {
            // Get a reference to a blob
            var blobClient = this.AzureContainerClient.GetBlobClient(cloudFilePath);
            if (!blobClient.Exists()) { throw new FileNotFoundException("Cloud File Not Found."); }

            Log.Debug("Downloading Cloud File '{CloudFolderPath} to '{LocalFilePath}'...", cloudFilePath, localFilePath);

            // Download the blob's contents and save it to a file
            blobClient.DownloadTo(localFilePath, this.CancellationToken);
        }

        /// <inheritdoc/>
        public void DownloadFile(string cloudFilePath, Stream stream)
        {
            // Get a reference to a blob
            var blobClient = this.AzureContainerClient.GetBlobClient(cloudFilePath);
            if (!blobClient.Exists()) { throw new FileNotFoundException("Cloud File Not Found."); }

            var totalBytesToTransfer = 0;

            // Download the blob's contents and save it to a file
            var blobToDownload = blobClient.Download().Value;


            var downloadBuffer = new byte[81920];
            int bytesRead;
            int totalBytesTransferred = 0;

            // fire off at least the first event
            this.OnTransferProgressChanged(0, totalBytesToTransfer);

            Log.Debug("Downloading Cloud File '{cloudFilePath}' to stream...", cloudFilePath);

            while ((bytesRead = blobToDownload.Content.Read(downloadBuffer, 0, downloadBuffer.Length)) != 0)
            {
                if (this.CancellationToken.IsCancellationRequested) { break; }
                stream.Write(downloadBuffer, 0, bytesRead);
                totalBytesTransferred += bytesRead;

                this.OnTransferProgressChanged(totalBytesTransferred, totalBytesToTransfer);
            }
            
            // fire off transfer complete events
            this.OnTransferComplete(null, (this.CancellationToken.IsCancellationRequested));           
        }

        /// <inheritdoc/>
        public void DeleteFile(string cloudFilePath)
        {
            Log.Debug("Deleting Cloud File '{cloudFilePath}'...", cloudFilePath);

            this.AzureContainerClient.DeleteBlobIfExists(
                blobName: cloudFilePath,
                DeleteSnapshotsOption.IncludeSnapshots, 
                cancellationToken: this.CancellationToken);
        }

        /// <inheritdoc/>
        public string GetDownloadUrl(string cloudFilePath, double ttlMinutes)
        {
            Log.Debug("Getting cloud file path for '{cloudFilePath}'...", cloudFilePath);

            var blobClient = this.AzureContainerClient.GetBlobClient(cloudFilePath);

            if (!blobClient.CanGenerateSasUri) { throw new ArgumentException("Not authorized to generage download url."); }
            
            // Create a SAS token that's valid for one hour.
            var sasBuilder = new BlobSasBuilder()
            {
                BlobContainerName = blobClient.BlobContainerName,
                BlobName = blobClient.Name,
                Resource = "b",
                StartsOn = DateTimeOffset.UtcNow.AddMinutes(-1),
                ExpiresOn = DateTimeOffset.UtcNow.AddMinutes(ttlMinutes)
            };

            // Specify read permissions for the SAS.
            sasBuilder.SetPermissions(BlobSasPermissions.Read);

            // Use the key to get the SAS token.
            return blobClient.GenerateSasUri(sasBuilder).ToString();           
        }

        #endregion

        #region --- Static Methods ---

        /// <summary>
        /// Validates bucket name against cloud provider requirements, throws Argument exception on failure
        /// </summary>
        /// <param name="bucketName">Bucket Name</param>
        /// <exception cref="ArgumentException">Throws argument exception on validation error</exception>
        /// <remarks>
        /// A container name must be a valid DNS name, conforming to the following naming rules:
        /// - Container names must start or end with a letter or number, and can contain only letters, numbers, and the dash(-) character.
        /// - Every dash(-) character must be immediately preceded and followed by a letter or number; consecutive dashes are not permitted in container names.
        /// - All letters in a container name must be lowercase.
        /// - Container names must be from 3 through 63 characters long.
        /// </remarks>
        public static void ValidateBucketName(string bucketName)
        {
            if (bucketName.Length < 3 || bucketName.Length > 63) { throw new ArgumentException("Bucket names must be between 3 and 63 characters long."); }

            if (bucketName.Any(char.IsUpper)) { throw new ArgumentException($"Bucket name '{bucketName}' must not have any uppercase letters."); }

            if (!Regex.IsMatch(bucketName, "^[a-z0-9](?!.*--)[a-z0-9-]{1,61}[a-z0-9]$", RegexOptions.Singleline | RegexOptions.CultureInvariant))
            {
                throw new ArgumentException($"Bucket name '{bucketName}' does not meet Azure naming requirements.");
            }
        }

        #endregion
    }
}
