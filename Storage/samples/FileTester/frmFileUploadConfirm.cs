using CloudFileTester.Framework.Extensions;
using Core.Cloud.Storage.File;

namespace CloudFileTester
{
    public partial class frmFileUploadConfirm : Form
    {
        private bool IsCancelled { get; set; } = false;
        private Task CalculationTask { get; set; } = null;

        private IFileStore FileStore { get; set; }
        private List<ICloudFile> UploadFiles { get; set; } = null;

        private int FilesSkipped { get; set; } = 0;

        private IEnumerable<string> LocalPaths { get; set; }
        private string DestinationCloudFolderPath { get; set; }

        public frmFileUploadConfirm(IFileStore fileStore, IEnumerable<string> localPaths, string destinationCloudFolderPath)
        {
            InitializeComponent();

            // convert to transfer items
            this.FileStore = fileStore;
            this.LocalPaths = localPaths;
            this.DestinationCloudFolderPath = destinationCloudFolderPath;

            this.ManualRefresh();
        }

        private void ManualRefresh()
        {
            // move over to the UI thread if required
            if (this.InvokeRequired)
            {
                this.InvokeEx(() => this.ManualRefresh());
                return;
            }

            this.lblDestination.Text = this.DestinationCloudFolderPath;
            if (this.UploadFiles?.Count > 0)
            {
                long totalFileSize = this.UploadFiles.Sum(x => x.FileSize);

                this.lblSource.Text = $"{this.UploadFiles.Count:n0} file{(this.UploadFiles.Count != 1 ? "s" : "")} ({(double)totalFileSize / (double)1048576:n2} MB)";
            }
            else
            {
                this.lblSource.Text = "0 files";
            }

            if(this.FilesSkipped > 0)
            {
                this.lblSource.Text += $", Skipped: {this.FilesSkipped}";
            }

            this.btnOk.Enabled = (this.UploadFiles?.Count > 0);
            
            Application.DoEvents();
        }

        private void frmUploadConfirm_FormClosing(object sender, FormClosingEventArgs e)
        {
            this.IsCancelled = true;
        }

        private void frmUploadConfirm_Shown(object sender, EventArgs e)
        {
            this.btnOk.Enabled = false;

            this.lblSource.Text = "Calculating...";
            Application.DoEvents();

            this.CalculationTask = Task.Factory.StartNew(() => CalculateUpload()).ContinueWith((prevTask) => TaskCleanup(prevTask));
        }

        private void CalculateUpload()
        {
            var uploadFiles = new List<ICloudFile>();

            foreach (string localPath in this.LocalPaths)
            {
                // if the user hit 'cancel' then stop calculating
                if (this.IsCancelled) { return; }

                if (File.Exists(localPath))
                {
                    string localFileName = System.IO.Path.GetFileName(localPath);
                                        
                    var cloudFilePath = $"{this.DestinationCloudFolderPath}/{Path.GetFileName(localPath)}";

                    // see if this is a new file or a replacement of a file
                    var existingCloudFile = this.FileStore.GetFile(cloudFilePath);
                    if (existingCloudFile != null)
                    {
                        existingCloudFile.LocalFilePath = localPath;

                        if (existingCloudFile.IsAlreadyTransferred())
                        {
                            this.FilesSkipped++;
                            continue;
                        }
                        
                        // file has changed so we need to upload it.
                        uploadFiles.Add(existingCloudFile);
                    }
                    else
                    {
                        // NEW FILE TO ADD
                        uploadFiles.Add(new CloudFile
                        {                            
                            BucketName = this.FileStore.BucketName,
                            FilePath = cloudFilePath,
                            LocalFilePath = localPath   // make sure we connect the file to the local one
                        });
                    }
                }
                else if (Directory.Exists(localPath))
                {
                    throw new Exception("Folder uploads are not currently supported.");

                    //string folderName = System.IO.Path.GetFileName(localPath);

                    //// get the details of the current folder
                    //LockerFolder subFolder;
                    //using (WebClient client = Webservices.GetFreshAuthorizedClient())
                    //{
                    //    subFolder = Webservices.LockerFolder_GetByName(client, this.DestinationFolder.Id, folderName, false, true, false);
                    //}

                    //this.CalculateUpload(uploadFiles, subFolder, localPath);
                }
            }

            this.UploadFiles = uploadFiles;
        }

        private void TaskCleanup(Task task)
        {
            // move over to the UI thread if required
            if (this.InvokeRequired)
            {
                this.InvokeEx(() => this.TaskCleanup(task));
                return;
            }

            this.ManualRefresh();

            // see if we have a error message to show
            if (task.IsFaulted == true)
            {
                if (this.CalculationTask.Exception.InnerException != null)
                {
                    MessageBox.Show(this, this.CalculationTask.Exception.InnerException.Message, "Upload Calculation Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
                else
                {
                    MessageBox.Show(this, this.CalculationTask.Exception.Message, "Upload Calculation Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
                this.DialogResult = DialogResult.Cancel;

                return;
            }

            // see if we have anything left to upload
            if(this.UploadFiles?.Count == 0)
            {
                MessageBox.Show("All files already uploaded.", "Upload Status", MessageBoxButtons.OK, MessageBoxIcon.Information);
                this.DialogResult = DialogResult.Cancel;

                return;
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

            this.DialogResult = DialogResult.OK;
        }

        private void btnOk_Click(object sender, EventArgs e)
        {
            var frmDataUpload = new frmFileUpload(this.FileStore, this.UploadFiles);
            this.DialogResult = frmDataUpload.ShowDialog(this.Owner);
        }
    }
}
