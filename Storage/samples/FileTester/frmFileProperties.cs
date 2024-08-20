using System.Runtime.InteropServices;

using Humanizer;

using Core.Cloud.Storage.File;
using CloudFileTester.Framework.Extensions;

namespace CloudFileTester
{
    public partial class frmFileProperties : Form
    {
        public IFileStore FileStore { get; set; }
        public string CloudFilePath { get; set; }

        public ICloudFile SelectedVersion
        {
            get
            {
                if (this.lsvFileVersions.SelectedItems.Count == 0) { return null; }

                return this.lsvFileVersions.SelectedItems[0].Tag as ICloudFile;
            }
        }

        public frmFileProperties(IFileStore fileStore, string cloudFilePath)
        {
            InitializeComponent();

            this.FileStore = fileStore;            
            this.CloudFilePath = cloudFilePath;
        }

        private void frmFileProperties_Shown(object sender, EventArgs e)
        {
            this.Enabled = false;

            this.ManualRefresh();

            this.Enabled = true;
        }

        private void ManualRefresh()
        {
            // move over to the UI thread if required
            if (this.InvokeRequired)
            {
                this.InvokeEx(() => this.ManualRefresh());
                return;
            }

            try
            {
                var cloudFile = this.FileStore.GetFile(this.CloudFilePath);

                this.Text = $"{cloudFile.FileName} - File Properties";

                this.lblSize.Text = $"{cloudFile.FileSizeMB:n3} MB ({cloudFile.FileSize:n0} bytes)";
                this.lblEdited.Text = $"{cloudFile.UpdatedOn?.ToLocalTime():d} {cloudFile.UpdatedOn?.ToLocalTime():t} ({cloudFile.UpdatedOn?.Humanize()})";
                this.lblBucketName.Text = cloudFile.BucketName;
                                
                ListViewItem item;

                // ***********************************************************************************
                // VERSIONS
                // ***********************************************************************************
                this.lsvFileVersions.Items.Clear();

                
                var fileVersions = this.FileStore.GetFileVersions(this.CloudFilePath).ToList();
                int verId = fileVersions.Count;

                foreach (var fileVersion in fileVersions)
                {
                    item = new ListViewItem($"{verId--}");
                    item.SubItems.Add($"{fileVersion.CreatedOn?.ToLocalTime():d} {fileVersion.CreatedOn?.ToLocalTime():t} ({fileVersion.CreatedOn?.ToLocalTime().Humanize()})");
                    if(fileVersion.FileSizeMB > 0.0009)
                    {
                        item.SubItems.Add($"{fileVersion.FileSizeMB:n3} MB");
                    }
                    else
                    {
                        item.SubItems.Add($"{fileVersion.FileSize:n0} bytes");
                    }
                    if(fileVersions.First() == fileVersion)
                    {
                        item.SubItems.Add("Current");
                    }
                    
                    item.Tag = fileVersion;

                    this.lsvFileVersions.Items.Add(item);
                }

                this.lsvFileVersions.AutoResizeColumns(ColumnHeaderAutoResizeStyle.ColumnContent);
                this.lsvFileVersions.AutoResizeColumns(ColumnHeaderAutoResizeStyle.HeaderSize);
                

                // ***********************************************************************************
                // METADATA
                // ***********************************************************************************                
                this.lsvMetadata.Items.Clear();

                if (cloudFile.Metadata?.Count > 0)
                {
                    //this.LockerFile.Properties.Sort()
                    foreach (KeyValuePair<string, string> pair in cloudFile.Metadata)
                    {
                        item = new ListViewItem(pair.Key);
                        item.SubItems.Add(pair.Value);
                        this.lsvMetadata.Items.Add(item);
                    }

                    this.lsvMetadata.AutoResizeColumns(ColumnHeaderAutoResizeStyle.ColumnContent);
                    this.lsvMetadata.AutoResizeColumns(ColumnHeaderAutoResizeStyle.HeaderSize);

                    this.lsvMetadata.Sort();
                }               
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error getting file details.\r\n\r\nError Details: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private string GetProperty(string propertyName)
        {
            //if (this.LockerFile.Properties.ContainsKey(propertyName))
            //{
            //    return this.LockerFile.Properties[propertyName];
            //}
            //else
            //{
                return "(N/A)";
            ///}
        }

        private void btnClose_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.Cancel;
        }

        #region --- Popup ---

        private void contextFile_Opening(object sender, System.ComponentModel.CancelEventArgs e)
        {
            if (this.SelectedVersion == null)
            {
                e.Cancel = true;
                return;
            }
        }

        private void contextFileOpen_Click(object sender, EventArgs e)
        {
            try
            {
                var cloudFile = this.lsvFileVersions.SelectedItems[0].Tag as ICloudFile;

                var downloadUrl = this.FileStore.GetDownloadUrl(cloudFile, 60);

                this.OpenUrl(downloadUrl);
            }
            catch (Exception ex)
            {
                MessageBox.Show(this, $"Error Getting Download Url.\r\n\r\nError: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void contextFileDownload_Click(object sender, EventArgs e)
        {
            try
            {
                var downloadUrl = this.FileStore.GetDownloadUrl(this.SelectedVersion, 60);

                this.OpenUrl(downloadUrl);
            }
            catch (Exception ex)
            {
                MessageBox.Show(this, $"Error Getting Download Url.\r\n\r\nError: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

            try
            {
                //BlobEntity sourceBlob = this.SelectedSnapshot;

                //Program.SaveFileDialog.DefaultExt = System.IO.Path.GetExtension(sourceBlob.FileName);
                //Program.SaveFileDialog.FileName = sourceBlob.FileName;

                //DialogResult result = Program.SaveFileDialog.ShowDialog();
                //if (result != DialogResult.OK) { return; }

                //string localFilePath = Program.SaveFileDialog.FileName;

                //// download the file.
                //frmDataTransfer frm = new frmDataTransfer(new TransferItem(sourceBlob, localFilePath), false);
                //result = frm.ShowDialog(this);
                //if (result != DialogResult.OK) { return; }

                //// see if the user wants to open it
                //result = MessageBox.Show("Would you like to open the downloaded file?", "Open File?", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                //if (result == DialogResult.Yes)
                //{
                //    Process.Start(localFilePath);
                //}
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error downloading and opening file.\r\n\r\nError: {ex.Message}", "Error Opening File", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void contextFileDelete_Click(object sender, EventArgs e)
        {
            var response = MessageBox.Show("Are you sure you want to delete this cloud file?", "Delete File?", MessageBoxButtons.OKCancel, MessageBoxIcon.Question);
            if (response != DialogResult.OK) { return; }

            try
            {                
                this.FileStore.DeleteFile(this.SelectedVersion.FilePath);
            }
            catch (Exception ex)
            {
                MessageBox.Show(this, $"Error Getting Download Url.\r\n\r\nError: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void contextFileProperties_Click(object sender, EventArgs e)
        {
            var frmProperties = new frmFileProperties(this.FileStore, this.SelectedVersion.FilePath);
            frmProperties.ShowDialog(this);
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


        #endregion


        private void lnkUrl_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            //string url = $"{Program.Settings.CurrentStorageBaseUrl}{this.Blob.AzureBlob.Uri.AbsolutePath}";

            //System.Windows.Forms.Clipboard.SetText(url);
            //MessageBox.Show(this,"File Url copied to clipboard.", "Path Copied", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private void mnuMetadataCopyToClipboard_Click(object sender, EventArgs e)
        {
            System.Text.StringBuilder metadata = new System.Text.StringBuilder();

            if (this.lsvMetadata.SelectedItems.Count == 0)
            {
                foreach (ListViewItem item in this.lsvMetadata.Items)
                {
                    metadata.AppendLine($"{item.Text}: {item.SubItems[1].Text}");
                }
            }
            else
            {
                foreach (ListViewItem item in this.lsvMetadata.SelectedItems)
                {
                    metadata.AppendLine($"{item.Text}: {item.SubItems[1].Text}");
                }
            }

            Clipboard.SetText(metadata.ToString());

            MessageBox.Show("Data copied to clipboard.", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }
        
    }
}
