namespace CloudFileTester.Framework
{
    public class HttpClientTransferWithProgress : IDisposable
    {
        #region --- Events ---

        public delegate void ProgressChangedHandler(long? totalFileSize, long totalBytesTransferred, double? progressPercentage);

        public event ProgressChangedHandler ProgressChanged;

        #endregion

        private HttpClient HttpClient { get; init; }

        public string CloudFileUrl { get; init; }
        public string LocalFilePath { get; init; }

        public CancellationToken CancellationToken { get; init; }

 

        /// <summary>
        /// Transfer library with progress
        /// </summary>
        /// <param name="cloudFileUrl">Location of the remote file resource</param>
        /// <param name="localFilePath">Location of local file resource</param>
        /// <param name="httpClient">Http Client to use (default = null/auto create)</param>
        /// <param name="cancellationToken">Cancellation Token</param>
        public HttpClientTransferWithProgress(string cloudFileUrl, string localFilePath, HttpClient httpClient = null, CancellationToken cancellationToken = default)
        {
            this.CloudFileUrl = cloudFileUrl;
            this.LocalFilePath = localFilePath;
            this.HttpClient = httpClient;
            this.CancellationToken = cancellationToken;
            
            if (httpClient == null)            
            {
                this.HttpClient = new HttpClient
                {
                    Timeout = TimeSpan.FromDays(1)
                };
            }
        }


        #region --- Upload ---

        //public async Task StartUpload()
        //{
        //    using (var response = await this.HttpClient.GetAsync(this.DownloadUrl, HttpCompletionOption.ResponseHeadersRead))
        //    {
        //        await this.UploadFileFromHttpResponseMessage(response);
        //    }
        //}

        //private async Task UploadFileFromHttpResponseMessage(HttpResponseMessage response)
        //{
        //    response.EnsureSuccessStatusCode();

        //    var totalBytesToTransfer = Core.IO.File.GetFileSize(this.LocalFilePath);

        //    using (var contentStream = await response.Content.ReadAsStreamAsync())
        //    {
        //        await this.ProcessUploadContentStream(totalBytesToTransfer, contentStream);
        //    }
        //}

        //private async Task ProcessUploadContentStream(long? totalBytesToTransfer, Stream contentStream)
        //{
        //    const int BUFFER_SIZE = 8192;

        //    var totalBytesTransferred = 0L;
        //    var transferBlockCount = 0L;
        //    var bufferBlockSize = new byte[BUFFER_SIZE];
        //    var isMoreToTransfer = true;

        //    using (var fileStream = new FileStream(this.LocalFilePath, FileMode.Create, FileAccess.Write, FileShare.None, BUFFER_SIZE, true))
        //    {
        //        do
        //        {
        //            var bytesTransferred = await contentStream.ReadAsync(bufferBlockSize, 0, bufferBlockSize.Length, this.CancellationToken);
        //            if (bytesTransferred == 0)
        //            {
        //                isMoreToTransfer = false;
        //                this.TriggerProgressChanged(totalBytesToTransfer, totalBytesTransferred);
        //                continue;
        //            }

        //            await fileStream.WriteAsync(bufferBlockSize, 0, bytesTransferred);

        //            totalBytesTransferred += bytesTransferred;
        //            transferBlockCount += 1;

        //            if (transferBlockCount % 100 == 0)
        //            {
        //                this.TriggerProgressChanged(totalBytesToTransfer, totalBytesTransferred);
        //            }

        //        }
        //        while (isMoreToTransfer);
        //    }
        //    this.TriggerProgressChanged(totalBytesToTransfer, totalBytesTransferred);
        //}


        #endregion

        #region --- Download ---

        /// <summary>
        /// Start the download process
        /// </summary>
        /// <returns>awaitable Task</returns>
        public async Task StartDownload()
        {
            using (var response = await this.HttpClient.GetAsync(this.CloudFileUrl, HttpCompletionOption.ResponseHeadersRead))
            {
                await this.DownloadFileFromHttpResponseMessage(response);
            }
        }

        private async Task DownloadFileFromHttpResponseMessage(HttpResponseMessage response)
        {
            response.EnsureSuccessStatusCode();

            var totalBytesToTransfer = response.Content.Headers.ContentLength;

            using (var contentStream = await response.Content.ReadAsStreamAsync())
            {
                await this.ProcessDownloadContentStream(totalBytesToTransfer, contentStream);
            }
        }

        private async Task ProcessDownloadContentStream(long? totalBytesToTransfer, Stream contentStream)
        {
            const int BUFFER_SIZE = 8192;

            var totalBytesTransferred = 0L;
            var transferBlockCount = 0L;
            var bufferBlockSize = new byte[BUFFER_SIZE];
            var isMoreToTransfer = true;

            using (var fileStream = new FileStream(this.LocalFilePath, FileMode.Create, FileAccess.Write, FileShare.None, BUFFER_SIZE, true))
            {
                do
                {
                    var bytesTransferred = await contentStream.ReadAsync(bufferBlockSize, 0, bufferBlockSize.Length, this.CancellationToken);
                    if (bytesTransferred == 0)
                    {
                        isMoreToTransfer = false;
                        this.TriggerProgressChanged(totalBytesToTransfer, totalBytesTransferred);
                        continue;
                    }

                    await fileStream.WriteAsync(bufferBlockSize, 0, bytesTransferred);

                    totalBytesTransferred += bytesTransferred;
                    transferBlockCount += 1;

                    if (transferBlockCount % 100 == 0)
                    {
                        this.TriggerProgressChanged(totalBytesToTransfer, totalBytesTransferred);
                    }
                        
                }
                while (isMoreToTransfer);
            }
            this.TriggerProgressChanged(totalBytesToTransfer, totalBytesTransferred);
        }

        #endregion

        private void TriggerProgressChanged(long? totalBytesToTransfer, long totalBytesTransferred)
        {
            if (this.ProgressChanged == null) { return; }
                
            double? progressPercentage = null;
            if (totalBytesToTransfer > 0)
            {
                progressPercentage = Math.Round((double)totalBytesTransferred / totalBytesToTransfer.Value * 100, 2);
            }                

            this.ProgressChanged(totalBytesToTransfer, totalBytesTransferred, progressPercentage);
        }

        public void Dispose()
        {
            this.HttpClient?.Dispose();
        }

        // https://stackoverflow.com/questions/20661652/progress-bar-with-httpclient
        //
        //public async void ExampleDownload()
        //{
        //    var sourceFileUrl = "http://example.com/file.zip";
        //    var destinationFilePath = Path.GetFullPath("file.zip");
        //
        //    using (var client = new HttpClientTransferWithProgress(sourceFileUrl, destinationFilePath))
        //    {
        //        client.ProgressChanged += (totalFileSize, totalBytesTransferred, progressPercentage) =>
        //        {
        //            Console.WriteLine($"{progressPercentage}% ({totalBytesTransferred}/{totalFileSize})");
        //        };
        //
        //        await client.StartDownload();
        //    }
        //}
        //
        //public async void ExampleUpload()
        //{
        //    var destinationFileUrl = "http://example.com/file.zip";
        //    var sourceFilePath = Path.GetFullPath("file.zip");
        //
        //    using (var client = new HttpClientTransferWithProgress(destinationFileUrl, sourceFilePath))
        //    {
        //        client.ProgressChanged += (totalFileSize, totalBytesTransferred, progressPercentage) =>
        //        {
        //            Console.WriteLine($"{progressPercentage}% ({totalBytesTransferred}/{totalFileSize})");
        //        };
        //
        //        await client.StartUpload();
        //    }
        //}
    }
}
