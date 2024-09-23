// ================================================================================
// <copyright file="FileStore.cs" company="Starlight Software Co">
//    Copyright (c) Starlight Software Co. All rights reserved.
//    Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
// </copyright>
// ================================================================================

using System.Runtime.CompilerServices;
using System.Text.RegularExpressions;

using LiteDB;
using Serilog;

namespace ThingsLibrary.Storage.Local
{
    public class FileStore : IFileStore
    {
        // TRANSFER EVENTS
        #region --- Observable Events ---

        /// <inheritdoc/>
        public event EventHandler<Events.TransferProgressChangedEventArgs> TransferProgressChanged;

        /// <inheritdoc/>
        public event EventHandler<Events.TransferCompleteEventArgs> TransferComplete;
               
        /// <inheritdoc/>
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

        /// <inheritdoc/>
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

        // INTERFACE

        /// <inheritdoc/>
        public string BucketName { get; init; }

        /// <inheritdoc/>
        public bool IsVersioning { get; set; } = false;

        /// <inheritdoc/>
        public FileStoreType StorageType { get; init; } = FileStoreType.Local;

        /// <inheritdoc/>
        public CancellationToken CancellationToken { get; set; } = default;

        // VENDOR SPECIFIC (if you aren't using the interface methods) allow for exposure of vendor specific objects
        public LiteDatabase DataContext { get; init; }
        public ILiteCollection<FileItem> DataCollection { get; init; }

        // allow for access to vendor specific 
        public string BucketDirectoryPath { get; set; }

        private Task ScanTask { get; set; }

        /// <inheritdoc/>
        public FileStore(string storageConnectionString, string bucketName)
        {
            //Connection String: 
            //  "RootStorePath=./TestDirectory"
            //  "RootStorePath=./TestDirectory;Versioning=true"

            // quick exits?
            if (string.IsNullOrEmpty(storageConnectionString)) { throw new ArgumentNullException(nameof(storageConnectionString)); }
            if (string.IsNullOrEmpty(bucketName)) { throw new ArgumentNullException(nameof(bucketName)); }

            // parse out the ProjectId and InstanceId from connectionstring
            var builder = new System.Data.Common.DbConnectionStringBuilder
            {
                ConnectionString = storageConnectionString
            };

            // validate the bucket naming against cloud specs
            FileStore.ValidateBucketName(bucketName);
            
            // set the core properties            
            this.BucketName = bucketName;

            this.IsVersioning = builder.GetValue<bool>("Versioning", false);

            // ================================================================================
            // Bucket Directory
            // ================================================================================
            var rootStoreFolderPath = builder.GetValue<string>("RootStorePath", null);
            if (string.IsNullOrEmpty(rootStoreFolderPath)) { throw new ArgumentException("'RootStorePath' does not exist in connection string."); }

            this.BucketDirectoryPath = Path.Combine(rootStoreFolderPath, bucketName);

            // make sure the directory exists
            if (!Directory.Exists(this.BucketDirectoryPath))
            {
                Log.Information("Creating Bucket Directory Path: '{CloudFilePath}'...", this.BucketDirectoryPath);
                if (!this.TryCreateDirectory(this.BucketDirectoryPath)) { throw new ArgumentException("Unable to create directory."); }
            }

            // ================================================================================
            // DATABASE  (note: Filename=:memory:   ==> memory database
            // ================================================================================
            var storageDbPath = builder.GetValue<string>("Filename", null);
            if (storageDbPath != ":memory:" && !string.IsNullOrEmpty(storageDbPath))
            {
                storageDbPath = this.GetOrCreateDatabasePath();
            }

            this.DataContext = new LiteDatabase($"Filename={storageDbPath};Upgrade=true;");
            this.DataCollection = this.DataContext.GetCollection<FileItem>(bucketName);

            // make sure we can do quick lookups
            this.DataCollection.EnsureIndex(x => x.FilePath);

            // see if we need to populate database
            if (!this.IsDatabasePopulated())
            {
                Log.Information("Generating Missing DB Data...");

                this.UpdateDatabase();
            }
            else
            {
                this.RefreshDatabase();
            }
        }

        private bool TryCreateDirectory(string path)
        {
            try
            {
                Directory.CreateDirectory(path);

                return true;
            }
            catch(Exception ex)
            {
                Console.WriteLine($"Error creating path: '{path}'");
                Console.WriteLine(ex.ToString());

                return false;
            }
        }

        #region --- File ---

        private string GetCloudFilePath(string storageFilePath)
        {
            return Path.GetRelativePath(this.BucketDirectoryPath, storageFilePath).Replace("\\", "/");
        } 
        
        private string GetStorageFilePath(string cloudFilePath)
        {
            return Path.Combine(this.BucketDirectoryPath, cloudFilePath);
        }

        /// <inheritdoc/>
        public IFileItem GetFile(string cloudFilePath)
        {
            Log.Debug("{CloudFilePath} - Getting Cloud File", cloudFilePath);
                        
            return this.GetCloudFile(cloudFilePath);
        }

        /// <inheritdoc/>
        public IEnumerable<IFileItem> GetFileVersions(string cloudFilePath)
        {
            Log.Debug("{CloudFilePath} - Geting Versions...", cloudFilePath);

            if (!this.IsVersioning) 
            {
                Log.Debug("{CloudFilePath} - Revisioning Turned Off!", cloudFilePath);
                return Enumerable.Empty<IFileItem>(); 
            }

            var prefix = $"{Path.GetDirectoryName(cloudFilePath)}/~revisions/{Path.GetFileNameWithoutExtension(cloudFilePath)}~Rev";

            return this.DataCollection.Find(x => x.FilePath.StartsWith(prefix));
        }

        /// <inheritdoc/>
        public IEnumerable<IFileItem> GetFiles(string cloudFolderPath)
        {            
            Log.Debug("Getting Cloud Files For Prefix: '{CloudFolderPath}'...", cloudFolderPath);

            throw new NotImplementedException();
        }

        /// <inheritdoc/>
        public void UploadFile(string localFilePath, string cloudFilePath)
        {
            var storageFilePath = this.GetStorageFilePath(cloudFilePath);

            // do we already have a file
            var cloudFile = this.GetCloudFile(cloudFilePath);
            if (cloudFile != null)
            {
                var oldMd5 = IO.File.ComputeMD5Base64(storageFilePath);

                //EXACT SAME FILE
                if (string.Compare(oldMd5, cloudFile.ContentMD5) == 0) { return; }

                // IS REVISIONING TURNED ON?
                if (this.IsVersioning)
                {
                    this.CreateRevisionFile(cloudFile);
                }                
            }

            // make sure the directory exists first
            Directory.CreateDirectory(System.IO.Path.GetDirectoryName(storageFilePath));

            // copy the file over
            System.IO.File.Copy(localFilePath, storageFilePath, true);

            // update/create CloudFile record
            cloudFile = this.GenerateCloudFile(storageFilePath);

            //add / update cloud file to database
            this.DataCollection.Upsert(new BsonValue(cloudFile.FilePath.ToLower()), cloudFile);
        }

        /// <inheritdoc/>
        public void UploadFile(Stream stream, string cloudFilePath)
        {
            throw new NotImplementedException();
        }

        /// <inheritdoc/>
        public void DownloadFile(string cloudFilePath, string localFilePath)
        {
            var cloudFile = this.GetCloudFile(cloudFilePath);
            if (cloudFile == null) { throw new FileNotFoundException(); }

            var storageFilePath = this.GetStorageFilePath(cloudFile.FilePath);

            System.IO.File.Copy(storageFilePath, localFilePath, true);
        }

        /// <inheritdoc/>
        public void DownloadFile(string cloudFilePath, Stream stream)
        {
            var cloudFile = this.GetCloudFile(cloudFilePath);
            if (cloudFile == null) { throw new FileNotFoundException(); }

            var storageFilePath = this.GetStorageFilePath(cloudFile.FilePath);
            if (!System.IO.File.Exists(storageFilePath)) { throw new FileNotFoundException($"File not found at: {storageFilePath}"); }

            var buffer = new byte[4096];
            int length;

            using (var fileStream = new FileStream(storageFilePath, FileMode.Open, FileAccess.Read))
            {
                while ((length = fileStream.Read(buffer, 0, buffer.Length)) > 0)
                {
                    stream.Write(buffer, 0, length);                    
                }
            }
        }

        /// <inheritdoc/>
        public void DeleteFile(string cloudFilePath)
        {
            var cloudFile = this.GetCloudFile(cloudFilePath);
            if (cloudFile == null) { return; }   //nothing to delete
                        
            // make a copy of the file as a 'revision' before deleting
            if (this.IsVersioning)
            {
                this.CreateRevisionFile(cloudFile);
            }

            // delete the file
            var storageFilePath = this.GetStorageFilePath(cloudFile.FilePath);

            if (System.IO.File.Exists(storageFilePath))
            {
                System.IO.File.Delete(storageFilePath);
            }
        
            this.DataCollection.Delete(new BsonValue(cloudFilePath.ToLower()));
        }

        /// <inheritdoc/>
        public string GetDownloadUrl(string cloudFilePath, double ttlMinutes)
        {
            throw new NotImplementedException();
        }

        #endregion

        #region --- Populate Database ---

        private bool IsDatabasePopulated()
        {
            return (this.DataCollection.FindById(new BsonValue("<Populated>")) != null);
        }

        private void RefreshDatabase()
        {
            // since this is a local file system stuff may have changed since last run/start.. re-index but do it as a side thread
            this.ScanTask = Task.Factory.StartNew(() => this.UpdateDatabase()).ContinueWith((prevTask) => ScanCleanup());
        }

        private void ScanCleanup()
        {
            this.ScanTask = null;            
        }
        
        private void UpdateDatabase()
        {
            this.UpdateDatabase(this.BucketDirectoryPath);

            // add the population complete record (use a invalid bucket name)
            this.DataCollection.Upsert(new BsonValue("<Populated>"), new FileItem("populated"));   //TODO?

            Log.Information("================================================================================");
            Log.Information("File Count: {FileCount}", this.DataCollection.Count());
        }

        private void UpdateDatabase(string parentPath)
        {
            Log.Information("{ParentPath}", parentPath);

            // ================================================================================
            // Files
            // ================================================================================
            var filePaths = Directory.GetFiles(parentPath);

            int total = filePaths.Count();
            int count = 0;

            foreach (var filePath in filePaths)
            {
                count++;
                Log.Information("({Count}/{Total} - {Percent:P1}): {FilePath}", count, total, (double)count / (double)total, filePath);

                this.UpdateDatabaseFile(filePath);

                if (this.CancellationToken.IsCancellationRequested) { return; }
            }

            // ================================================================================
            // Versioning
            // ================================================================================
            if (this.IsVersioning)
            {
                var revisionsFolderPath = this.GetRevisionsPath(parentPath);
                if (Directory.Exists(revisionsFolderPath))
                {
                    Log.Information("{RevisionsPath}", revisionsFolderPath);

                    var revisionFilePaths = Directory.GetFiles(revisionsFolderPath);

                    count = 0;
                    total = revisionFilePaths.Count();

                    foreach (var revisionFilePath in revisionFilePaths)
                    {
                        count++;
                        Log.Information("({Count}/{Total} - {Percent:P1}): {FilePath}", count, total, (double)count / (double)total, revisionFilePath);

                        this.UpdateDatabaseFile(revisionFilePath);

                        if (this.CancellationToken.IsCancellationRequested) { return; }
                    }
                }
            }

            // ================================================================================
            // Sub Folders
            // ================================================================================
            var folderPaths = Directory.GetDirectories(parentPath);
            foreach (var subFolderPath in folderPaths)
            {
                var subFolderName = Path.GetFileName(subFolderPath);
                if (subFolderName.StartsWith("~")) { continue; }    //consider ~ folders are temp/hidden/system and don't index

                this.UpdateDatabase(subFolderPath);

                if (this.CancellationToken.IsCancellationRequested) { return; }
            }            
        }

        private void UpdateDatabaseFile(string storageFilePath)
        {
            var cloudFilePath = this.GetCloudFilePath(storageFilePath);

            var cloudFile = this.GetCloudFile(cloudFilePath);
            if (cloudFile != null)
            {
                DateTimeOffset lastWriteTime = System.IO.File.GetLastWriteTime(storageFilePath).ToUniversalTime();

                if(lastWriteTime.Subtract(cloudFile.UpdatedOn.Value).TotalSeconds < 1) { return; }                
            }

            cloudFile = this.GenerateCloudFile(storageFilePath);

            //add cloud file to database
            this.DataCollection.Upsert(new BsonValue(cloudFile.FilePath.ToLower()), cloudFile);            
        }

        private string GetOrCreateDatabasePath()
        {
            var dirPath = Path.Combine(this.BucketDirectoryPath, "~meta");
            if (!Directory.Exists(dirPath))
            {
                var dir = new DirectoryInfo(dirPath);
                dir.Create();

                // hide it
                dir.Attributes |= FileAttributes.Hidden;
            }

            return Path.Combine(dirPath, "storage.db");
        }

        #endregion

        /// <inheritdoc/>
        private FileItem GenerateCloudFile(string storageFilePath)
        {
            if (!System.IO.File.Exists(storageFilePath)) { throw new ArgumentException("Local file does not exit."); }

            var attributes = new Dictionary<string, object>()
            {
                {   "path", this.GetCloudFilePath(storageFilePath)     },
                {   "Size", IO.File.GetFileSize(storageFilePath)    },
                {   "created", System.IO.File.GetCreationTime(storageFilePath).ToUniversalTime()      },
                {   "updated", System.IO.File.GetLastWriteTime(storageFilePath).ToUniversalTime()     },
                {   "content_type", IO.File.GetMimeType(storageFilePath)     },
                {   "content_md5", IO.File.ComputeMD5Base64(storageFilePath)     }
            };

            var cloudFile = new FileItem(this.GetCloudFilePath(storageFilePath));

            cloudFile.Add(attributes);

            return cloudFile;
        }

        private FileItem GetCloudFile(string cloudFilePath)
        {
            return this.DataCollection.FindById(new BsonValue(cloudFilePath.ToLower()));
        }


        #region --- Revisioning --- 

        private string GetRevisionsPath(string storageFolderPath)
        {
            return Path.Combine(storageFolderPath, "~revisions");
        }

        /// <summary>
        /// Get or Create Revisions hidden folder for the file we want to keep revisions of
        /// </summary>
        /// <param name="storageFilePath">File we are editing</param>
        /// <returns></returns>
        private string GetOrCreateRevisionsPath(string storageFilePath)
        {
            var revisionsDirPath = this.GetRevisionsPath(Path.GetDirectoryName(storageFilePath));
            if (!Directory.Exists(revisionsDirPath))
            {
                var dir = new DirectoryInfo(revisionsDirPath);
                dir.Create();

                // hide it
                dir.Attributes |= FileAttributes.Hidden;
            }

            return revisionsDirPath;
        }

        /// <summary>
        /// This function takes a file and creates a 'revision' of that file
        /// </summary>
        /// <param name="cloudFile">Cloud file we want to create a reivison file of</param>        
        private void CreateRevisionFile(FileItem cloudFile)
        {
            var storageFilePath = Path.Combine(this.BucketDirectoryPath, cloudFile.FilePath);

            var revisionsFolderPath = this.GetOrCreateRevisionsPath(storageFilePath);
            var revisionFilePath = Path.Combine(revisionsFolderPath, $"{Path.GetFileNameWithoutExtension(storageFilePath)}~Rev{DateTimeOffset.Now.ToUnixTimeSeconds()}{Path.GetExtension(storageFilePath)}");

            //TODO: check to see if the last revision is this same file

            // make the copy
            System.IO.File.Copy(storageFilePath, revisionFilePath);
                        
            this.UpdateDatabaseFile(revisionFilePath);
        }

        #endregion

        #region --- Static Methods ---

        /// <summary>
        /// Validates bucket name against provider requirements, throws Argument exception on failure
        /// </summary>
        /// <param name="bucketName">Bucket Name</param>
        /// <exception cref="ArgumentException">Throws argument exception on validation error</exception>
        /// <remarks>
        /// NOTE: (The local storage validation matches Azure for the sake of simplicity and compatibility)
        /// A container name must be a valid local folder name, conforming to the following naming rules:
        /// - Container names must start or end with a letter or number, and can contain only letters, numbers, and the dash(-) character.
        /// - Every dash(-) character must be immediately preceded and followed by a letter or number; consecutive dashes are not permitted in container names.        
        /// - Container names must be from 3 through 63 characters long.
        /// </remarks>
        public static void ValidateBucketName(string bucketName)
        {
            // LOCAL Folder name can't contain any of the following:  \ / : * ? " < > |

            if (string.IsNullOrEmpty(bucketName)) { throw new ArgumentException("Bucket name must be specified."); }

            if (bucketName.Length < 3 || bucketName.Length > 63) { throw new ArgumentException("Bucket names must be between 3 and 63 characters long."); }

            if (!Regex.IsMatch(bucketName, "^[a-zA-Z0-9](?!.*--)[a-zA-Z0-9-]{1,61}[a-zA-Z0-9]$", RegexOptions.Singleline | RegexOptions.CultureInvariant))
            {
                throw new ArgumentException($"Bucket name '{bucketName}' does not meet Azure naming requirements.");
            }
        }

        #endregion
    }
}
