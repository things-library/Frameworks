using System.Timers;

using CloudFileTester.Framework.Extensions;

namespace CloudFileTester
{
    public partial class frmFileDownload : Form
    {
        private System.Timers.Timer _refreshTimer;

        private IFileStore FileStore { get; set; }
        private List<CloudFile> DownloadFiles { get; set; } = new List<CloudFile>();
        private int CurrentFileIndex { get; set; } = 0;
        public CloudFile CurrentFile => (this.CurrentFileIndex < this.DownloadFiles.Count ? this.DownloadFiles[this.CurrentFileIndex] : null);

        private HttpClient HttpClient { get; set; }
        private bool IsCancelled { get; set; } = false;

        private DateTime? StartedOn { get; set; } = null;

        // CURRENT FILE Downloading
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



        public frmFileDownload(IFileStore fileStore, CloudFile downloadFile)
        {
            InitializeComponent();

            this.FileStore = fileStore;
            this.DownloadFiles.Add(downloadFile);

            this.Init();
        }

        public frmFileDownload(IFileStore fileStore, List<CloudFile> downloadFiles)
        {
            InitializeComponent();

            this.FileStore = fileStore;
            this.DownloadFiles = downloadFiles;

            this.Init();
        }

        private void frmFileDownload_Shown(object sender, EventArgs e)
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

        private void frmFileDownload_FormClosing(object sender, FormClosingEventArgs e)
        {
            this.IsCancelled = true;

            //if (this.HttpClient?.IsBusy == true)
            //{
            //    this.HttpClient.CancelAsync();
            //}
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
            this.TotalBytesToTransfer = 0;

            this.lblCurrentStatus.Text = "";
            this.lblEstTimeLeft.Text = "";
            this.lblPercent.Text = "";
            this.lblSkipped.Text = "";

            this.progressQueue.Value = 0;
        }


        private void RefreshStatus()
        {
            if(this.IsCancelled) { return; }
            if (this.TotalBytesToTransfer <= 0) { return; }

            // move over to the UI thread if required
            if (this.InvokeRequired)
            {
                this.InvokeEx(() => this.RefreshStatus());
                return;
            }

            double percent = ((double)this.CompletedFileBytesTransferred + (double)this.CurrentFileBytesTransferred) / (double)this.TotalBytesToTransfer;
            if(percent > 1) { percent = 1; }

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

        private void CalculateTransfer()
        {
            this.Init();
            this.lblCurrentStatus.Text = "Calculating Download...";

            
            for (int i = this.DownloadFiles.Count - 1; i >= 0; i--)
            {
                double percent = (this.DownloadFiles.Count - 1 - i) / (double)this.DownloadFiles.Count;

                this.progressQueue.Maximum = 10000;
                this.progressQueue.Value = Convert.ToInt32(percent * 10000);

                this.lblPercent.Text = $"{percent:P1}";
                Application.DoEvents();

                // see if we should remove it from the listing as we already downloaded it
                if (this.DownloadFiles[i].IsAlreadyTransferred())
                {
                    this.FilesSkipped++;
                    this.DownloadFiles.RemoveAt(i);
                }
            }

            this.TotalBytesToTransfer = this.DownloadFiles.Sum(df => df.FileSize);

            this.progressQueue.Value = 0;
            this.lblCurrentStatus.Text = "";
            this.lblPercent.Text = "";

            if(this.FilesSkipped > 0)
            {
                this.lblSkipped.Text = $"Skipped: {this.FilesSkipped}";
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
            this.lblCurrentStatus.Text = $"Downloading '{this.CurrentFile.FileName}'...";

            this.FileStore.TransferProgressChanged += FileStore_TransferProgressChanged;

            this.FileStore.DownloadFile(currentFile.FilePath, currentFile.LocalFilePath);

            this.FileStore.TransferProgressChanged -= FileStore_TransferProgressChanged;           
        }

        private void FileStore_DownloadFileCompleted(object sender, TransferCompleteEventArgs e)
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
                        MessageBox.Show(e.Error.Message, "Download Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        this.CloseDialog();
                    });
                }
                else
                {
                    MessageBox.Show(e.Error.Message, "Download Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
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

        private void FileStore_TransferProgressChanged(object sender, TransferProgressChangedEventArgs e)
        {
            this.CurrentFileBytesTransferred = e.BytesTransferred;

            //wait at least 10 seconds to start trying to figure out estimated time left
            var elapsedTime = DateTime.UtcNow.Subtract(this.StartedOn.Value);
            if (elapsedTime.TotalSeconds > 10)
            {
                double totalBytesTransferred = (double)this.CompletedFileBytesTransferred + (double)this.CurrentFileBytesTransferred;

                // we are over what we expected
                if (totalBytesTransferred < this.TotalBytesToTransfer)
                {
                    this.BytesPerSecond = Convert.ToInt64(totalBytesTransferred / (double)elapsedTime.TotalSeconds);

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