using System.Runtime.InteropServices;

namespace FileTester
{
    public partial class frmMain : Form
    {
        private AppSettings AppSettings { get; set; }

        private IFileStore CurrentBucket { get; set; }
        
        public frmMain(AppSettings appSettings)
        {
            this.AppSettings = appSettings;

            InitializeComponent();
        }

        private void frmMain_Load(object sender, EventArgs e)
        {
            this.cboEntityStore.DataSource = this.AppSettings.FileStoreSettings;
        }

        private void ManualRefresh()
        {
            if (this.CurrentBucket == null)
            {
                this.grpEntityStores.Enabled = true;
                this.btnStoreConnect.Visible = true;
                this.btnStoreDisconnect.Visible = false;                
                this.grpFiles.Enabled = false;
            }
            else
            {
                this.grpEntityStores.Enabled = false;
                this.btnStoreConnect.Visible = false;
                this.btnStoreDisconnect.Visible = true;                
                this.grpFiles.Enabled = true;
            }

            this.lsvFiles.Items.Clear();
        }


        private void btnStoreConnect_Click(object sender, EventArgs e)
        {
            this.Enabled = false;
            Application.DoEvents();

            try
            {
                this.CurrentBucket = this.GetFileStore(this.cboEntityStore.SelectedItem as FileStoreOptions);
                
                this.ManualRefresh();

                this.RefreshFiles();
            }
            catch (Exception ex)
            {
                MessageBox.Show(this, $"Error Creating Store.\r\n\r\nError: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

            this.Enabled = true;
        }

        private void btnStoreDisconnect_Click(object sender, EventArgs e)
        {
            this.CurrentBucket = null;
            this.ManualRefresh();
        }


        private void btnEntityGetAll_Click(object sender, EventArgs e)
        {
            this.grpFiles.Enabled = false;
            Application.DoEvents();

            try
            {
                this.RefreshFiles();
            }
            catch (Exception ex)
            {
                MessageBox.Show(this, $"Error Getting Entities from store.\r\n\r\nError: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

            this.grpFiles.Enabled = true;
        }


        private IFileStore GetFileStore(FileStoreOptions storeSettings)
        {
            if (storeSettings.Type == "Azure_Blob")
            {
                return new Az.FileStore(storeSettings.ConnectionString, storeSettings.BucketName);
            }
            //else if (storeSettings.Type == "GCP_Storage")
            //{
            //    return new Gc.FileStore(storeSettings.ConnectionString, storeSettings.BucketName);
            //}
            else
            {
                MessageBox.Show(this, $"Unknown Store Type '{storeSettings.Type}'", "Invalid Type", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return null;
            }
        }

        private void RefreshFiles()
        {
            this.lsvFiles.Items.Clear();
                        
            var files = this.CurrentBucket.GetFiles(null);
            foreach (var file in files)
            {
                var item = new ListViewItem(file.FilePath);
                item.SubItems.Add(file.ContentType);
                item.SubItems.Add($"{file.UpdatedOn:d} {file.UpdatedOn:t}");                
                item.SubItems.Add($"{file.FileSizeMB:0.000} MB");
                
                item.Tag = file;

                this.lsvFiles.Items.Add(item);
            }

            // make sure the columns fit the contents and headers
            this.lsvFiles.AutoResizeColumns(ColumnHeaderAutoResizeStyle.ColumnContent);
            this.lsvFiles.AutoResizeColumns(ColumnHeaderAutoResizeStyle.HeaderSize);
        }

        private void treeFolders_AfterSelect(object sender, TreeViewEventArgs e)
        {

        }

        #region --- Popup ---

        private void mnuEntityPopup_Opening(object sender, System.ComponentModel.CancelEventArgs e)
        {
            if(this.lsvFiles.SelectedItems.Count != 1) { e.Cancel = true; return; }
            

        }

        private void mnuFilePopupDownload_Click(object sender, EventArgs e)
        {
            //try
            //{
            //    var fileItem = this.lsvFiles.SelectedItems[0].Tag as IFileItem;

            //    var downloadUrl = this.CurrentBucket.GetDownloadUrl(fileItem, 60);
                                
            //    this.OpenUrl(downloadUrl);
            //}
            //catch (Exception ex)
            //{
            //    MessageBox.Show(this, $"Error Getting Download Url.\r\n\r\nError: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            //}            
        }

        private void OpenUrl(string url)
        {
            try
            {
                System.Diagnostics.Process.Start(new System.Diagnostics.ProcessStartInfo(url) { UseShellExecute = true });             
            }
            catch
            {
                // hack because of this: https://github.com/dotnet/corefx/issues/10361
                if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
                {
                    url = url.Replace("&", "^&");
                    System.Diagnostics.Process.Start(new System.Diagnostics.ProcessStartInfo(url)
                    { 
                        UseShellExecute = true,
                        Verb = "open"
                    });
                }
                else if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
                {
                    System.Diagnostics.Process.Start("xdg-open", url);
                }
                else if (RuntimeInformation.IsOSPlatform(OSPlatform.OSX))
                {
                    System.Diagnostics.Process.Start("open", url);
                }
                else
                {
                    throw;
                }
            }
        }

        private void mnuFilePopupDelete_Click(object sender, EventArgs e)
        {
            var response = MessageBox.Show("Are you sure you want to delete this cloud file?", "Delete File?", MessageBoxButtons.OKCancel, MessageBoxIcon.Question);
            if(response != DialogResult.OK) { return; }

            try
            {
                var cloudFile = this.lsvFiles.SelectedItems[0].Tag as IFileItem;

                this.CurrentBucket.DeleteFile(cloudFile.FilePath);                
            }
            catch (Exception ex)
            {
                MessageBox.Show(this, $"Error Getting Download Url.\r\n\r\nError: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

            // call the refresh button like a user did it.
            this.btnEntityGetAll_Click(sender, e);
        }

        private void mnuFilePopupProperties_Click(object sender, EventArgs e)
        {
            //var cloudFile = this.lsvFiles.SelectedItems[0].Tag as IFileItem;

            //var frmProperties = new frmFileProperties(this.CurrentBucket, cloudFile.FilePath);
            //frmProperties.ShowDialog(this);
        }

        #endregion

        private void lsvFiles_DragDrop(object sender, DragEventArgs e)
        {
            try
            {
                if (e.Data.GetDataPresent(DataFormats.FileDrop))
                {
                    var paths = (string[])e.Data.GetData(DataFormats.FileDrop);
                    if(paths.Length == 0) { return; }

                    this.ProcessUploadPaths(paths.ToList());
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(this, ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void lsvFiles_DragEnter(object sender, DragEventArgs e)
        {
            e.Effect = DragDropEffects.Copy;            
            
        }

        private void lsvFiles_ItemDrag(object sender, ItemDragEventArgs e)
        {
            DoDragDrop(e.Item, DragDropEffects.Move);
        }

        private void ProcessUploadPaths(List<string> paths)
        {
            ////TODO:
            //var frm = new frmFileUploadConfirm(this.CurrentBucket, paths, "TestFolderPath");
            //var result = frm.ShowDialog(this);
            //if(result != DialogResult.OK) { return; }

            //this.RefreshFiles();
        }

        private void lsvFiles_KeyDown(object sender, KeyEventArgs e)
        {
            if(e.KeyCode == Keys.Delete)
            {
                // do whatever the menu item would do
                this.mnuFilePopupDelete_Click(sender, e);
            }
        }

        private void btnFileUpload_Click(object sender, EventArgs e)
        {
            //var result = this.openFileDialog1.ShowDialog();
            //if(result != DialogResult.OK) { return; }            
            //if(this.openFileDialog1.FileNames.Length == 0) { return; }

            //var frm = new frmFileUploadConfirm(this.CurrentBucket, this.openFileDialog1.FileNames, $"{DateTime.Now:yyyy-MM-dd}");
            //result = frm.ShowDialog(this);

            //// refresh regardless of being cancelled or something
            //this.RefreshFiles();
        }

        private void btnFileDownload_Click(object sender, EventArgs e)
        {
            //if(this.lsvFiles.SelectedItems.Count == 0) 
            //{
            //    MessageBox.Show(this, "No items selected to download.", "No Selection", MessageBoxButtons.OK, MessageBoxIcon.Error);
            //    return;
            //}

            //var result = this.folderBrowserDialog1.ShowDialog(this);
            //if(result != DialogResult.OK) { return; }

            //var cloudFiles = new List<FileItem>();
            //foreach(var item in this.lsvFiles.SelectedItems)
            //{
            //    var listItem = item as ListViewItem;

            //    var cloudFile = listItem.Tag as FileItem;

            //    cloudFile.LocalFilePath = Path.Combine(this.folderBrowserDialog1.SelectedPath, cloudFile.FileName);
            //    cloudFiles.Add(cloudFile);
            //}            

            //var frm = new frmFileDownload(this.CurrentBucket, cloudFiles);
            //result = frm.ShowDialog(this);
            
            //TODO: show folder path where files downloaded to
        }

        private void btnFileExplorer_Click(object sender, EventArgs e)
        {
            System.Diagnostics.Process.Start(new System.Diagnostics.ProcessStartInfo("explorer.exe")
            {
                UseShellExecute = true
            });
        }

    }
}