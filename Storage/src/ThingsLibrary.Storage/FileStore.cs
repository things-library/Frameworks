namespace ThingsLibrary.Storage
{
    public abstract class FileStore
    {
        /// <inheritdoc />        
        public string BucketPrefix { get; set; }

        /// <inheritdoc />        
        public string BucketName { get; set; }

        /// <inheritdoc />        
        public FileStoreType StorageType { get; set; }

        /// <inheritdoc />        
        public abstract DateTime? CreatedOn { get; }

        /// <inheritdoc />        
        public abstract DateTime? UpdatedOn { get; }

        /// <inheritdoc />        
        public abstract void CreateIfNotExists();

        /// <inheritdoc />        
        public abstract void FetchProperties();

        /// <inheritdoc />        
        public abstract bool Exists();

        /// <inheritdoc />        
        public abstract void Delete();

        #region --- FILE ---

        /// <inheritdoc />        
        public abstract FileItem GetFile(string cloudFilePath);

        /// <inheritdoc />
        public abstract List<FileItem> GetFiles(string cloudFolderPath);

        /// <inheritdoc />
        public abstract void DownloadFile(string cloudFilePath, string localFilePath);

        /// <inheritdoc />
        public abstract void DownloadFile(string cloudFilePath, Stream stream);

        /// <inheritdoc />
        public abstract void UploadFile(string localFilePath, string cloudFilePath, long contentLength, string contentMD5);

        /// <inheritdoc />
        public abstract void UploadFile(Stream stream, string cloudFilePath, long contentLength, string contentMD5);

        /// <inheritdoc />
        public abstract void ReplaceFile(string localFilePath, string cloudFilePath, long contentLength, string contentMD5);

        /// <inheritdoc />
        public abstract void ReplaceFile(Stream stream, string cloudFilePath, long contentLength, string contentMD5);

        /// <inheritdoc />
        public abstract void DeleteFile(string cloudFilePath);

        /// <inheritdoc />
        public abstract string GetDownloadUrl(FileItem cloudFile, double ttlMinutes);

        #endregion
    }
}
