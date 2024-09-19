namespace ThingsLibrary.Storage.Interfaces
{
    public interface IFileStore
    {
        #region --- Observable Events ---

        // TRANSFER EVENTS
        /// <summary>
        /// Provides a progress report as files are transferred (upload or download)
        /// </summary>
        public event EventHandler<Events.TransferProgressChangedEventArgs> TransferProgressChanged;

        /// <summary>
        /// Occurs when a transfer completes
        /// </summary>
        public event EventHandler<Events.TransferCompleteEventArgs> TransferComplete;

        #endregion

        /// <summary>
        /// Bucket Name
        /// </summary>
        public string BucketName { get; }

        /// <summary>
        /// File Store Type
        /// </summary>
        public FileStoreType StorageType { get; }

        /// <summary>
        /// If edits to files should be versioned / snapshots
        /// </summary>
        public bool IsVersioning { get; }

        public CancellationToken CancellationToken { get; }

        #region --- Files --- 

        /// <summary>
        /// Delete file
        /// </summary>
        /// <param name="cloudFilePath"></param>
        public void DeleteFile(string cloudFilePath);

        /// <summary>
        /// Download file to stream
        /// </summary>
        /// <param name="cloudFilePath">Cloud File Path</param>
        /// <param name="stream">Stream to use</param>
        public void DownloadFile(string cloudFilePath, Stream stream);

        /// <summary>
        /// Download cloud file to local file
        /// </summary>
        /// <param name="cloudFilePath">Cloud File Path</param>
        /// <param name="localFilePath">Local File Path</param>
        public void DownloadFile(string cloudFilePath, string localFilePath);

        /// <summary>
        /// Get Download url
        /// </summary>
        /// <param name="cloudFilePath">Cloud File Path</param>
        /// <param name="ttlMinutes">Shared token time length in minutes</param>
        /// <returns></returns>
        public string GetDownloadUrl(string cloudFilePath, double ttlMinutes);

        /// <summary>
        /// Get cloud file details for cloud file path
        /// </summary>
        /// <param name="cloudFilePath"></param>
        /// <returns><see cref="IFileItem"/></returns>
        public IFileItem GetFile(string cloudFilePath);

        /// <summary>
        /// Get the version history for thie specified file
        /// </summary>
        /// <param name="cloudFilePath">Cloud File Path</param>
        /// <returns></returns>
        public IEnumerable<IFileItem> GetFileVersions(string cloudFilePath);

        /// <summary>
        /// Get Files based on cloud file Path
        /// </summary>
        /// <param name="pathPrefix">File Path Prefix</param>
        /// <returns></returns>
        public IEnumerable<IFileItem> GetFiles(string pathPrefix);

        /// <summary>
        /// Upload file to path
        /// </summary>
        /// <param name="stream"><see cref="Stream"/></param>
        /// <param name="cloudFilePath">Cloud File Path</param>
        public void UploadFile(Stream stream, string cloudFilePath);

        /// <summary>
        /// Upload local file to path
        /// </summary>
        /// <param name="localFilePath">Local File Path</param>
        /// <param name="cloudFilePath">Cloud File Path</param>
        public void UploadFile(string localFilePath, string cloudFilePath);

        #endregion        
    }
}