namespace ThingsLibrary.Storage.Interfaces
{
    public interface ICloudFile
    {
        /// <summary>
        /// MD5 of the contents
        /// </summary>
        public string ContentMD5 { get; }

        /// <summary>
        /// Content Type
        /// </summary>
        public string ContentType { get; }

        /// <summary>
        /// File Name
        /// </summary>
        public string FileName { get; }

        /// <summary>
        /// File Path
        /// </summary>
        public string FilePath { get; }

        /// <summary>
        /// File Size (in bytes)
        /// </summary>
        public long FileSize { get; }

        /// <summary>
        /// File Size (in MB)
        /// </summary>
        public double FileSizeMB { get; } // => this.FileSize / (double)1048576;     //(1024x1024) == bytes to megs

        /// <summary>
        /// Actual metadata stamped on the cloud files
        /// </summary>
        public IDictionary<string, string> Metadata { get; }

        /// <summary>
        /// Updated Date
        /// </summary>
        public DateTimeOffset? UpdatedOn { get; }

        /// <summary>
        /// Creation Date
        /// </summary>
        public DateTimeOffset? CreatedOn { get; }

        #region --- Extended Local Properties --- 

        /// <summary>
        /// Locak File Path
        /// </summary>
        public string LocalFilePath { get; set; }

        /// <summary>
        /// Tag
        /// </summary>
        public object Tag { get; set; }

        #endregion

        /// <summary>
        /// Is the local file path the same one in cloud storage based on MD5
        /// </summary>
        /// <returns>true if MD5 matches, fals if local file path is missing or file does not exist, or MD5 does not match</returns>
        public bool IsAlreadyTransferred();
    }
}