// ================================================================================
// <copyright file="IFileItem.cs" company="Starlight Software Co">
//    Copyright (c) Starlight Software Co. All rights reserved.
//    Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
// </copyright>
// ================================================================================

using System.Text.Json.Serialization;

namespace ThingsLibrary.Storage.Interfaces
{
    public interface IFileItem
    {
        /// <summary>
        /// File Name
        /// </summary>
        [JsonPropertyName("name")]
        public string Name { get; }

        /// <summary>
        /// File Path
        /// </summary>
        [JsonPropertyName("path")]
        public string Path { get; }

        /// <summary>
        /// MD5 of the contents
        /// </summary>
        [JsonPropertyName("md5")]
        public string ContentMD5 { get; }

        /// <summary>
        /// Content Type
        /// </summary>
        [JsonPropertyName("type")]
        public string ContentType { get; }
                
        /// <summary>
        /// File Size (in bytes)
        /// </summary>
        [JsonPropertyName("size")]
        public long Size { get; }

        /// <summary>
        /// File Size (in MB)
        /// </summary>
        [JsonIgnore]
        public double SizeMB { get; } // => this.FileSize / (double)1048576;     //(1024x1024) == bytes to megs

        /// <summary>
        /// Actual metadata stamped on the cloud files
        /// </summary>
        [JsonPropertyName("meta")]
        public IDictionary<string, string> Metadata { get; }

        /// <summary>
        /// Updated Date
        /// </summary>
        [JsonPropertyName("updated")]
        public DateTimeOffset? UpdatedOn { get; }

        /// <summary>
        /// Creation Date
        /// </summary>
        [JsonPropertyName("created")]
        public DateTimeOffset? CreatedOn { get; }

        #region --- Extended Local Properties --- 

        /// <summary>
        /// Locak File Path
        /// </summary>
        [JsonIgnore]
        public string LocalFilePath { get; set; }

        /// <summary>
        /// Tag
        /// </summary>
        [JsonIgnore]
        public object Tag { get; set; }

        #endregion

        /// <summary>
        /// Is the local file path the same one in cloud storage based on MD5
        /// </summary>
        /// <returns>true if MD5 matches, fals if local file path is missing or file does not exist, or MD5 does not match</returns>
        public bool IsAlreadyTransferred();
    }
}