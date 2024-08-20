namespace CloudFileTester
{
    partial class frmMain
    {
        /// <summary>
        ///  Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        ///  Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        ///  Required method for Designer support - do not modify
        ///  the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            this.grpEntityStores = new System.Windows.Forms.GroupBox();
            this.btnStoreConnect = new System.Windows.Forms.Button();
            this.cboEntityStore = new System.Windows.Forms.ComboBox();
            this.btnStoreDisconnect = new System.Windows.Forms.Button();
            this.grpFiles = new System.Windows.Forms.GroupBox();
            this.btnFileDownload = new System.Windows.Forms.Button();
            this.btnFileUpload = new System.Windows.Forms.Button();
            this.lsvFiles = new System.Windows.Forms.ListView();
            this.colName = new System.Windows.Forms.ColumnHeader();
            this.colType = new System.Windows.Forms.ColumnHeader();
            this.colModified = new System.Windows.Forms.ColumnHeader();
            this.colSize = new System.Windows.Forms.ColumnHeader();
            this.columnHeader1 = new System.Windows.Forms.ColumnHeader();
            this.mnuEntityPopup = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.mnuFilePopupOpen = new System.Windows.Forms.ToolStripMenuItem();
            this.mnuFilePopupDownload = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator2 = new System.Windows.Forms.ToolStripSeparator();
            this.mnuFilePopupDelete = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator1 = new System.Windows.Forms.ToolStripSeparator();
            this.mnuFilePopupProperties = new System.Windows.Forms.ToolStripMenuItem();
            this.btnFolderRefresh = new System.Windows.Forms.Button();
            this.openFileDialog1 = new System.Windows.Forms.OpenFileDialog();
            this.folderBrowserDialog1 = new System.Windows.Forms.FolderBrowserDialog();
            this.grpEntityStores.SuspendLayout();
            this.grpFiles.SuspendLayout();
            this.mnuEntityPopup.SuspendLayout();
            this.SuspendLayout();
            // 
            // grpEntityStores
            // 
            this.grpEntityStores.Controls.Add(this.btnStoreConnect);
            this.grpEntityStores.Controls.Add(this.cboEntityStore);
            this.grpEntityStores.Location = new System.Drawing.Point(12, 12);
            this.grpEntityStores.Name = "grpEntityStores";
            this.grpEntityStores.Size = new System.Drawing.Size(451, 58);
            this.grpEntityStores.TabIndex = 0;
            this.grpEntityStores.TabStop = false;
            this.grpEntityStores.Text = "File Store";
            // 
            // btnStoreConnect
            // 
            this.btnStoreConnect.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnStoreConnect.Location = new System.Drawing.Point(361, 22);
            this.btnStoreConnect.Name = "btnStoreConnect";
            this.btnStoreConnect.Size = new System.Drawing.Size(75, 23);
            this.btnStoreConnect.TabIndex = 1;
            this.btnStoreConnect.Text = "Connect";
            this.btnStoreConnect.UseVisualStyleBackColor = true;
            this.btnStoreConnect.Click += new System.EventHandler(this.btnStoreConnect_Click);
            // 
            // cboEntityStore
            // 
            this.cboEntityStore.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.cboEntityStore.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cboEntityStore.FormattingEnabled = true;
            this.cboEntityStore.Location = new System.Drawing.Point(18, 22);
            this.cboEntityStore.Name = "cboEntityStore";
            this.cboEntityStore.Size = new System.Drawing.Size(337, 23);
            this.cboEntityStore.TabIndex = 0;
            // 
            // btnStoreDisconnect
            // 
            this.btnStoreDisconnect.Location = new System.Drawing.Point(372, 33);
            this.btnStoreDisconnect.Name = "btnStoreDisconnect";
            this.btnStoreDisconnect.Size = new System.Drawing.Size(75, 24);
            this.btnStoreDisconnect.TabIndex = 6;
            this.btnStoreDisconnect.Text = "Disconnect";
            this.btnStoreDisconnect.UseVisualStyleBackColor = true;
            this.btnStoreDisconnect.Visible = false;
            this.btnStoreDisconnect.Click += new System.EventHandler(this.btnStoreDisconnect_Click);
            // 
            // grpFiles
            // 
            this.grpFiles.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.grpFiles.Controls.Add(this.btnFileDownload);
            this.grpFiles.Controls.Add(this.btnFileUpload);
            this.grpFiles.Controls.Add(this.lsvFiles);
            this.grpFiles.Controls.Add(this.btnFolderRefresh);
            this.grpFiles.Enabled = false;
            this.grpFiles.Location = new System.Drawing.Point(12, 78);
            this.grpFiles.Name = "grpFiles";
            this.grpFiles.Size = new System.Drawing.Size(810, 574);
            this.grpFiles.TabIndex = 13;
            this.grpFiles.TabStop = false;
            this.grpFiles.Text = "Files";
            // 
            // btnFileDownload
            // 
            this.btnFileDownload.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.btnFileDownload.Location = new System.Drawing.Point(117, 539);
            this.btnFileDownload.Name = "btnFileDownload";
            this.btnFileDownload.Size = new System.Drawing.Size(104, 23);
            this.btnFileDownload.TabIndex = 16;
            this.btnFileDownload.Text = "Download...";
            this.btnFileDownload.UseVisualStyleBackColor = true;
            this.btnFileDownload.Click += new System.EventHandler(this.btnFileDownload_Click);
            // 
            // btnFileUpload
            // 
            this.btnFileUpload.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.btnFileUpload.Location = new System.Drawing.Point(7, 539);
            this.btnFileUpload.Name = "btnFileUpload";
            this.btnFileUpload.Size = new System.Drawing.Size(104, 23);
            this.btnFileUpload.TabIndex = 15;
            this.btnFileUpload.Text = "Upload...";
            this.btnFileUpload.UseVisualStyleBackColor = true;
            this.btnFileUpload.Click += new System.EventHandler(this.btnFileUpload_Click);
            // 
            // lsvFiles
            // 
            this.lsvFiles.Activation = System.Windows.Forms.ItemActivation.OneClick;
            this.lsvFiles.AllowDrop = true;
            this.lsvFiles.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.lsvFiles.BackColor = System.Drawing.SystemColors.Window;
            this.lsvFiles.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.colName,
            this.colType,
            this.colModified,
            this.colSize,
            this.columnHeader1});
            this.lsvFiles.ContextMenuStrip = this.mnuEntityPopup;
            this.lsvFiles.FullRowSelect = true;
            this.lsvFiles.GridLines = true;
            this.lsvFiles.HeaderStyle = System.Windows.Forms.ColumnHeaderStyle.Nonclickable;
            this.lsvFiles.LabelWrap = false;
            this.lsvFiles.Location = new System.Drawing.Point(7, 22);
            this.lsvFiles.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.lsvFiles.Name = "lsvFiles";
            this.lsvFiles.Size = new System.Drawing.Size(796, 511);
            this.lsvFiles.TabIndex = 9;
            this.lsvFiles.UseCompatibleStateImageBehavior = false;
            this.lsvFiles.View = System.Windows.Forms.View.Details;
            this.lsvFiles.ItemDrag += new System.Windows.Forms.ItemDragEventHandler(this.lsvFiles_ItemDrag);
            this.lsvFiles.DragDrop += new System.Windows.Forms.DragEventHandler(this.lsvFiles_DragDrop);
            this.lsvFiles.DragEnter += new System.Windows.Forms.DragEventHandler(this.lsvFiles_DragEnter);
            this.lsvFiles.KeyDown += new System.Windows.Forms.KeyEventHandler(this.lsvFiles_KeyDown);
            // 
            // colName
            // 
            this.colName.Name = "colName";
            this.colName.Text = "Name";
            this.colName.Width = 250;
            // 
            // colType
            // 
            this.colType.Name = "colType";
            this.colType.Text = "Type";
            this.colType.Width = 80;
            // 
            // colModified
            // 
            this.colModified.Name = "colModified";
            this.colModified.Text = "Modified";
            this.colModified.Width = 220;
            // 
            // colSize
            // 
            this.colSize.Name = "colSize";
            this.colSize.Text = "Size";
            this.colSize.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            this.colSize.Width = 100;
            // 
            // columnHeader1
            // 
            this.columnHeader1.Name = "columnHeader1";
            this.columnHeader1.Text = "";
            this.columnHeader1.Width = 30;
            // 
            // mnuEntityPopup
            // 
            this.mnuEntityPopup.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.mnuFilePopupOpen,
            this.mnuFilePopupDownload,
            this.toolStripSeparator2,
            this.mnuFilePopupDelete,
            this.toolStripSeparator1,
            this.mnuFilePopupProperties});
            this.mnuEntityPopup.Name = "mnuEntityPopup";
            this.mnuEntityPopup.Size = new System.Drawing.Size(137, 104);
            this.mnuEntityPopup.Opening += new System.ComponentModel.CancelEventHandler(this.mnuEntityPopup_Opening);
            // 
            // mnuFilePopupOpen
            // 
            this.mnuFilePopupOpen.Name = "mnuFilePopupOpen";
            this.mnuFilePopupOpen.Size = new System.Drawing.Size(136, 22);
            this.mnuFilePopupOpen.Text = "&Open...";
            // 
            // mnuFilePopupDownload
            // 
            this.mnuFilePopupDownload.Name = "mnuFilePopupDownload";
            this.mnuFilePopupDownload.Size = new System.Drawing.Size(136, 22);
            this.mnuFilePopupDownload.Text = "&Download";
            this.mnuFilePopupDownload.Click += new System.EventHandler(this.mnuFilePopupDownload_Click);
            // 
            // toolStripSeparator2
            // 
            this.toolStripSeparator2.Name = "toolStripSeparator2";
            this.toolStripSeparator2.Size = new System.Drawing.Size(133, 6);
            // 
            // mnuFilePopupDelete
            // 
            this.mnuFilePopupDelete.Name = "mnuFilePopupDelete";
            this.mnuFilePopupDelete.Size = new System.Drawing.Size(136, 22);
            this.mnuFilePopupDelete.Text = "&Delete";
            this.mnuFilePopupDelete.Click += new System.EventHandler(this.mnuFilePopupDelete_Click);
            // 
            // toolStripSeparator1
            // 
            this.toolStripSeparator1.Name = "toolStripSeparator1";
            this.toolStripSeparator1.Size = new System.Drawing.Size(133, 6);
            // 
            // mnuFilePopupProperties
            // 
            this.mnuFilePopupProperties.Name = "mnuFilePopupProperties";
            this.mnuFilePopupProperties.Size = new System.Drawing.Size(136, 22);
            this.mnuFilePopupProperties.Text = "&Properties...";
            this.mnuFilePopupProperties.Click += new System.EventHandler(this.mnuFilePopupProperties_Click);
            // 
            // btnFolderRefresh
            // 
            this.btnFolderRefresh.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.btnFolderRefresh.Location = new System.Drawing.Point(699, 539);
            this.btnFolderRefresh.Name = "btnFolderRefresh";
            this.btnFolderRefresh.Size = new System.Drawing.Size(104, 23);
            this.btnFolderRefresh.TabIndex = 4;
            this.btnFolderRefresh.Text = "Refresh";
            this.btnFolderRefresh.UseVisualStyleBackColor = true;
            this.btnFolderRefresh.Click += new System.EventHandler(this.btnEntityGetAll_Click);
            // 
            // openFileDialog1
            // 
            this.openFileDialog1.FileName = "openFileDialog1";
            this.openFileDialog1.Multiselect = true;
            // 
            // frmMain
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(834, 664);
            this.Controls.Add(this.grpFiles);
            this.Controls.Add(this.btnStoreDisconnect);
            this.Controls.Add(this.grpEntityStores);
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.MinimumSize = new System.Drawing.Size(850, 490);
            this.Name = "frmMain";
            this.Text = "Cloud File Tester";
            this.Load += new System.EventHandler(this.frmMain_Load);
            this.grpEntityStores.ResumeLayout(false);
            this.grpFiles.ResumeLayout(false);
            this.mnuEntityPopup.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private GroupBox grpEntityStores;
        private ComboBox cboEntityStore;
        private Button btnStoreConnect;
        private Button btnStoreDisconnect;
        private GroupBox grpFiles;
        private Button btnFolderRefresh;
        private ContextMenuStrip mnuEntityPopup;
        private ToolStripMenuItem mnuFilePopupDownload;
        private ToolStripMenuItem mnuFilePopupDelete;
        private ListView lsvFiles;
        private ColumnHeader colName;
        private ColumnHeader colType;
        private ColumnHeader colModified;
        private ColumnHeader colSize;
        private ColumnHeader columnHeader1;
        private ToolStripSeparator toolStripSeparator2;
        private ToolStripSeparator toolStripSeparator1;
        private ToolStripMenuItem mnuFilePopupProperties;
        private ToolStripMenuItem mnuFilePopupOpen;
        private Button btnFileDownload;
        private Button btnFileUpload;
        private OpenFileDialog openFileDialog1;
        private FolderBrowserDialog folderBrowserDialog1;
    }
}