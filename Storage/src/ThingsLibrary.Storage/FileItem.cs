﻿namespace ThingsLibrary.Storage
{
    public class FileItem : ItemDto, IFileItem
    {
        /// <inheritdoc />        
        public string FilePath => this["resource_key"];

        /// <inheritdoc />
        public string FileName => this.Name;
        
        /// <inheritdoc />
        public long FileSize => this.Get<long>("file_size", 0);

        /// <inheritdoc />
        public double FileSizeMB => this.FileSize / (double)1048576;     //(1024x1024) == bytes to megs

        /// <inheritdoc />
        public string ContentType => this["content_type"];

        /// <inheritdoc />
        public string ContentMD5 => this["content_md5"]; //MD5 Hash        

        /// <inheritdoc />
        public DateTimeOffset? CreatedOn => this.Get<DateTimeOffset?>("created", null);

        /// <inheritdoc />
        public DateTimeOffset? UpdatedOn => this.Get<DateTimeOffset?>("updated", null);


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

        public FileItem(string key, string name)
        {
            this.Key = key;
            this.Name = name;
        }

        public FileItem(string resourcePath)
        {
            this.Key = Path.GetFileName(resourcePath);
            this.Name = Path.GetFileName(resourcePath);

            // keep track of the entire resource path
            this.Attributes["resource_key"] = resourcePath;
        }

    }
}