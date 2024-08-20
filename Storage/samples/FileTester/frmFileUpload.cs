using CloudFileTester.Framework;
using CloudFileTester.Framework.Extensions;
using Core.Cloud.Storage.File;
using System.Net;
using System.Timers;

namespace CloudFileTester
{
    public partial class frmFileUpload : Form
    {
        private object SyncObject = new object();
        private System.Timers.Timer _refreshTimer;

        private IFileStore FileStore { get; set; }
        private List<ICloudFile> UploadFiles { get; set; } = new List<ICloudFile>();
        private int CurrentFileIndex { get; set; } = 0;
        public ICloudFile CurrentFile => (this.CurrentFileIndex < this.UploadFiles.Count ? this.UploadFiles[this.CurrentFileIndex] : null);

        private WebClient WebClient { get; set; }
        private bool IsCancelled { get; set; } = false;

        private DateTime? StartedOn { get; set; } = null;

        // CURRENT FILE UPLOADING
        private long CurrentFileBytesTransferred { get; set; } = 0;
        private long CompletedFileBytesTransferred { get; set; }

        // TOTALS
        private long TotalBytesToTransfer { get; set; } = 0;

        // COUNTS
        private long FilesTransferred { get; set; } = 0;
        private long FilesSkipped { get; set; } = 0;
        private long FilesFailed { get; set; } = 0;

        // SPEED
        private long BytesPerSecond { get; set; } = 0;
        private TimeSpan? EstimatedTimeLeft { get; set; } = null;

        // ERRORS
        public int ErrorCount { get; set; } = 0;
        public Exception LastException { get; set; } = null;


        public frmFileUpload(IFileStore fileStore, ICloudFile uploadFile)
        {
            InitializeComponent();

            this.FileStore = fileStore;
            this.UploadFiles.Add(uploadFile);

            this.Init();
        }

        public frmFileUpload(IFileStore fileStore, List<ICloudFile> uploadFiles)
        {
            InitializeComponent();

            this.FileStore = fileStore;
            this.UploadFiles = uploadFiles;

            this.Init();
        }

        private void frmFileUpload_FormClosing(object sender, FormClosingEventArgs e)
        {
            this.IsCancelled = true;

            if (this.WebClient?.IsBusy == true)
            {
                this.WebClient.CancelAsync();
            }
        }

        private void frmFileUpload_Shown(object sender, EventArgs e)
        {
            // add a refresh timer
            _refreshTimer = new System.Timers.Timer(200);
            _refreshTimer.Elapsed += Timer_Elapsed;
            _refreshTimer.AutoReset = true;
            _refreshTimer.Enabled = true;

            this.CalculateTransfer();

            this.StartNextTransfer();
        }

        private void Timer_Elapsed(object sender, ElapsedEventArgs e)
        {
            this.RefreshStatus();
        }

        public void Init()
        {
            // move over to the UI thread if required
            if (this.InvokeRequired)
            {
                this.InvokeEx(() => this.Init());
                return;
            }

            this.StartedOn = null;
            this.FilesFailed = 0;
            this.FilesSkipped = 0;
            this.FilesTransferred = 0;

            this.CurrentFileBytesTransferred = 0;
            this.CompletedFileBytesTransferred = 0;

            this.lblCurrentStatus.Text = "";
            this.lblEstTimeLeft.Text = "";
            this.lblPercent.Text = "";
            this.lblSkipped.Text = "";

            this.progressQueue.Value = 0;
        }

        private void CalculateTransfer()
        {
            this.Init();

            this.lblCurrentStatus.Text = "Calculating Upload...";

            // =========================================================
            // NOTE: WE ARE DOING THIS IN THE UPLOAD CONFIRM DIALOG
            // =========================================================
            //for (int i = this.UploadFiles.Count - 1; i >= 0; i--)
            //{
            //    double percent = (this.UploadFiles.Count - 1 - i) / (double)this.UploadFiles.Count;

            //    this.progressQueue.Maximum = 10000;
            //    this.progressQueue.Value = Convert.ToInt32(percent * 10000);

            //    this.lblPercent.Text = $"{percent:P1}";
            //    Application.DoEvents();

            //    // see if we should remove it from the listing as we already downloaded it
            //    if (this.UploadFiles[i].IsAlreadyUploaded())
            //    {
            //        this.FilesSkipped++;
            //        this.UploadFiles.RemoveAt(i);
            //    }
            //}

            this.TotalBytesToTransfer = this.UploadFiles.Sum(df => df.FileSize);

            this.progressQueue.Value = 0;
            this.lblCurrentStatus.Text = "";
            this.lblPercent.Text = "";

            if (this.FilesSkipped > 0)
            {
                this.lblSkipped.Text = $"Skipped: {this.FilesSkipped}";
            }
        }

        private void RefreshStatus()
        {
            if (this.IsCancelled) { return; }
            if (this.TotalBytesToTransfer <= 0) { return; }

            // move over to the UI thread if required
            if (this.InvokeRequired)
            {
                this.InvokeEx(() => this.RefreshStatus());
                return;
            }

            lock (this.SyncObject)
            {
                double percent = ((double)this.CompletedFileBytesTransferred + (double)this.CurrentFileBytesTransferred) / (double)this.TotalBytesToTransfer;

                this.progressQueue.Value = Convert.ToInt32(percent * 10000);
                this.lblPercent.Text = $"{percent:P1}";

                if (this.EstimatedTimeLeft != null)
                {
                    this.lblEstTimeLeft.Text = $"(Estimated Time Left: {this.EstimatedTimeLeft})";
                }
                else
                {
                    this.lblEstTimeLeft.Text = "";
                }
                Application.DoEvents();
            }
        }

        private void StartNextTransfer()
        {
            if (this.IsCancelled) { return; }

            // lets get started
            var currentFile = this.CurrentFile;

            // nothing left to transfer?
            if (currentFile == null) { this.CloseDialog(); return; }

            if (this.StartedOn == null) { this.StartedOn = DateTime.UtcNow; }
            this.lblCurrentStatus.Text = $"Uploading '{System.IO.Path.GetFileName(this.CurrentFile.LocalFilePath)}'...";

            // get a new fresh web client

            //this.WebClient = Webservices.GetFreshAuthorizedClient();
            //this.WebClient.UploadProgressChanged += Client_UploadProgressChanged;
            //this.WebClient.UploadFileCompleted += Client_UploadFileCompleted;

            // add the necessary headers
            //this.WebClient.Headers.Add("Content-Type", Landus.IO.File.GetMimeType(currentFile.LocalFilePath));
            //this.WebClient.Headers.Add("Content-MD5", currentFile.ContentMD5);
            //this.WebClient.Headers.Add("Content-Length", $"{currentFile.FileSize}");

            // is it a new file or a update
            //string uploadUrl = $"{this.WebClient.BaseAddress}lockers/{Program.Settings.CurrentLocker.Id}/folders/{currentFile.LockerFolder.Id}/files/{System.IO.Path.GetFileName(currentFile.LocalFilePath)}";

            //this.WebClient.UploadFileAsync(new Uri(uploadUrl), this.CurrentFile.LocalFilePath);

            this.FileStore.UploadFile(currentFile.LocalFilePath, currentFile.FilePath);
        }

        private void Client_UploadFileCompleted(object sender, UploadFileCompletedEventArgs e)
        {
            var currentFile = this.CurrentFile;

            // add on the bytes to the counter
            this.CurrentFileBytesTransferred = 0;

            if (e.Error != null)
            {
                this.FilesFailed++;
                this.LastException = e.Error;

                // move over to the UI thread if required
                if (this.InvokeRequired)
                {
                    this.InvokeEx(() =>
                    {
                        MessageBox.Show(e.Error.Message, "Upload Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        this.CloseDialog();
                    });
                }
                else
                {
                    MessageBox.Show(e.Error.Message, "Upload Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    this.CloseDialog();
                }
                return;
            }
            else if (e.Cancelled)
            {
                this.FilesSkipped++;
            }
            else
            {
                this.FilesTransferred++;
                this.CompletedFileBytesTransferred += currentFile.FileSize;
            }

            // move to next file to download
            this.CurrentFileIndex++;
            this.StartNextTransfer();
        }


        private void Client_UploadProgressChanged(object sender, UploadProgressChangedEventArgs e)
        {
            lock (this.SyncObject)
            {
                this.CurrentFileBytesTransferred = e.BytesReceived;

                //wait at least 10 seconds to start trying to figure out estimated time left
                TimeSpan elapsedTime = DateTime.UtcNow.Subtract(this.StartedOn.Value);
                if (elapsedTime.TotalSeconds > 10)
                {
                    double totalBytesTransferred = (double)this.CompletedFileBytesTransferred + (double)this.CurrentFileBytesTransferred;

                    this.BytesPerSecond = Convert.ToInt64(totalBytesTransferred / (double)elapsedTime.TotalSeconds);

                    // we are over what we expected
                    if (this.BytesPerSecond > 0 && totalBytesTransferred < this.TotalBytesToTransfer)
                    {
                        // calculate the time left
                        int secondsLeft = Convert.ToInt32((double)(this.TotalBytesToTransfer - totalBytesTransferred) / (double)this.BytesPerSecond);

                        this.EstimatedTimeLeft = new TimeSpan(0, 0, 0, secondsLeft);
                    }
                    else
                    {
                        this.EstimatedTimeLeft = null;
                    }
                }
            }
        }

        private void CloseDialog()
        {
            // move over to the UI thread if required
            if (this.InvokeRequired)
            {
                this.InvokeEx(() => this.CloseDialog());
                return;
            }

            this.IsCancelled = true;
            this.DialogResult = DialogResult.OK;
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.Cancel;
        }
    }
}