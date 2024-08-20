using ThingsLibrary.Storage.Interfaces;

namespace ThingsLibrary.Storage
{
    public class CloudFile : ItemDto//, ICloudFile
    {
                
        /// <inheritdoc />        
        public string FilePath { get; init; }

        /// <inheritdoc />
        public string FileName => System.IO.Path.GetFileName(this.FilePath);

        /// <inheritdoc />
        public DateTimeOffset? CreatedOn => this.Get<DateTimeOffset?>("created", null);

        /// <inheritdoc />
        public DateTimeOffset? UpdatedOn => this.Get<DateTimeOffset?>("updated", null);

        /// <inheritdoc />
        public string ContentType => this["content_type"];

        /// <inheritdoc />
        public string ContentMD5 => this["content_md5"]; //MD5 Hash        

        /// <inheritdoc />
        public long FileSize => this.Get<long>("file_size", 0);
        
        /// <inheritdoc />
        public double FileSizeMB => this.FileSize / (double)1048576;     //(1024x1024) == bytes to megs
                
        #region --- Local / Generic Properties ---

        /// <inheritdoc />
        public string LocalFilePath { get; set; } = null;

        /// <inheritdoc />
        public bool IsAlreadyTransferred()
        {
            if (string.IsNullOrEmpty(this.LocalFilePath)) { return false; }
            if (!System.IO.File.Exists(this.LocalFilePath)) { return false; }

            var contentMD5 = IO.File.ComputeMD5Base64(this.LocalFilePath);

            return (string.Compare(contentMD5, this.ContentMD5) == 0);
        }

        #endregion

    }
}
