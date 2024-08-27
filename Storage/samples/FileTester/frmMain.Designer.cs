namespace FileTester
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
            this.grpEntityStores = new GroupBox();
            this.btnStoreConnect = new Button();
            this.cboEntityStore = new ComboBox();
            this.btnStoreDisconnect = new Button();
            this.grpFiles = new GroupBox();
            this.btnFileDownload = new Button();
            this.btnFileUpload = new Button();
            this.lsvFiles = new ListView();
            this.colName = new ColumnHeader();
            this.colType = new ColumnHeader();
            this.colModified = new ColumnHeader();
            this.colSize = new ColumnHeader();
            this.columnHeader1 = new ColumnHeader();
            this.mnuEntityPopup = new ContextMenuStrip(this.components);
            this.mnuFilePopupOpen = new ToolStripMenuItem();
            this.mnuFilePopupDownload = new ToolStripMenuItem();
            this.toolStripSeparator2 = new ToolStripSeparator();
            this.mnuFilePopupDelete = new ToolStripMenuItem();
            this.toolStripSeparator1 = new ToolStripSeparator();
            this.mnuFilePopupProperties = new ToolStripMenuItem();
            this.btnFolderRefresh = new Button();
            this.openFileDialog1 = new OpenFileDialog();
            this.folderBrowserDialog1 = new FolderBrowserDialog();
            this.grpEntityStores.SuspendLayout();
            this.grpFiles.SuspendLayout();
            this.mnuEntityPopup.SuspendLayout();
            this.SuspendLayout();
            // 
            // grpEntityStores
            // 
            this.grpEntityStores.Controls.Add(this.btnStoreConnect);
            this.grpEntityStores.Controls.Add(this.cboEntityStore);
            this.grpEntityStores.Location = new Point(17, 20);
            this.grpEntityStores.Margin = new Padding(4, 5, 4, 5);
            this.grpEntityStores.Name = "grpEntityStores";
            this.grpEntityStores.Padding = new Padding(4, 5, 4, 5);
            this.grpEntityStores.Size = new Size(644, 97);
            this.grpEntityStores.TabIndex = 0;
            this.grpEntityStores.TabStop = false;
            this.grpEntityStores.Text = "File Store";
            // 
            // btnStoreConnect
            // 
            this.btnStoreConnect.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            this.btnStoreConnect.Location = new Point(516, 37);
            this.btnStoreConnect.Margin = new Padding(4, 5, 4, 5);
            this.btnStoreConnect.Name = "btnStoreConnect";
            this.btnStoreConnect.Size = new Size(107, 38);
            this.btnStoreConnect.TabIndex = 1;
            this.btnStoreConnect.Text = "Connect";
            this.btnStoreConnect.UseVisualStyleBackColor = true;
            this.btnStoreConnect.Click += this.btnStoreConnect_Click;
            // 
            // cboEntityStore
            // 
            this.cboEntityStore.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            this.cboEntityStore.DropDownStyle = ComboBoxStyle.DropDownList;
            this.cboEntityStore.FormattingEnabled = true;
            this.cboEntityStore.Location = new Point(26, 37);
            this.cboEntityStore.Margin = new Padding(4, 5, 4, 5);
            this.cboEntityStore.Name = "cboEntityStore";
            this.cboEntityStore.Size = new Size(480, 33);
            this.cboEntityStore.TabIndex = 0;
            // 
            // btnStoreDisconnect
            // 
            this.btnStoreDisconnect.Location = new Point(531, 55);
            this.btnStoreDisconnect.Margin = new Padding(4, 5, 4, 5);
            this.btnStoreDisconnect.Name = "btnStoreDisconnect";
            this.btnStoreDisconnect.Size = new Size(107, 40);
            this.btnStoreDisconnect.TabIndex = 6;
            this.btnStoreDisconnect.Text = "Disconnect";
            this.btnStoreDisconnect.UseVisualStyleBackColor = true;
            this.btnStoreDisconnect.Visible = false;
            this.btnStoreDisconnect.Click += this.btnStoreDisconnect_Click;
            // 
            // grpFiles
            // 
            this.grpFiles.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            this.grpFiles.Controls.Add(this.btnFileDownload);
            this.grpFiles.Controls.Add(this.btnFileUpload);
            this.grpFiles.Controls.Add(this.lsvFiles);
            this.grpFiles.Controls.Add(this.btnFolderRefresh);
            this.grpFiles.Enabled = false;
            this.grpFiles.Location = new Point(17, 130);
            this.grpFiles.Margin = new Padding(4, 5, 4, 5);
            this.grpFiles.Name = "grpFiles";
            this.grpFiles.Padding = new Padding(4, 5, 4, 5);
            this.grpFiles.Size = new Size(1157, 573);
            this.grpFiles.TabIndex = 13;
            this.grpFiles.TabStop = false;
            this.grpFiles.Text = "Files";
            // 
            // btnFileDownload
            // 
            this.btnFileDownload.Anchor = AnchorStyles.Bottom | AnchorStyles.Left;
            this.btnFileDownload.Location = new Point(167, 514);
            this.btnFileDownload.Margin = new Padding(4, 5, 4, 5);
            this.btnFileDownload.Name = "btnFileDownload";
            this.btnFileDownload.Size = new Size(149, 38);
            this.btnFileDownload.TabIndex = 16;
            this.btnFileDownload.Text = "Download...";
            this.btnFileDownload.UseVisualStyleBackColor = true;
            this.btnFileDownload.Click += this.btnFileDownload_Click;
            // 
            // btnFileUpload
            // 
            this.btnFileUpload.Anchor = AnchorStyles.Bottom | AnchorStyles.Left;
            this.btnFileUpload.Location = new Point(10, 514);
            this.btnFileUpload.Margin = new Padding(4, 5, 4, 5);
            this.btnFileUpload.Name = "btnFileUpload";
            this.btnFileUpload.Size = new Size(149, 38);
            this.btnFileUpload.TabIndex = 15;
            this.btnFileUpload.Text = "Upload...";
            this.btnFileUpload.UseVisualStyleBackColor = true;
            this.btnFileUpload.Click += this.btnFileUpload_Click;
            // 
            // lsvFiles
            // 
            this.lsvFiles.Activation = ItemActivation.OneClick;
            this.lsvFiles.AllowDrop = true;
            this.lsvFiles.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            this.lsvFiles.BackColor = SystemColors.Window;
            this.lsvFiles.Columns.AddRange(new ColumnHeader[] { this.colName, this.colType, this.colModified, this.colSize, this.columnHeader1 });
            this.lsvFiles.ContextMenuStrip = this.mnuEntityPopup;
            this.lsvFiles.FullRowSelect = true;
            this.lsvFiles.GridLines = true;
            this.lsvFiles.HeaderStyle = ColumnHeaderStyle.Nonclickable;
            this.lsvFiles.LabelWrap = false;
            this.lsvFiles.Location = new Point(10, 37);
            this.lsvFiles.Margin = new Padding(6, 5, 6, 5);
            this.lsvFiles.Name = "lsvFiles";
            this.lsvFiles.Size = new Size(1135, 465);
            this.lsvFiles.TabIndex = 9;
            this.lsvFiles.UseCompatibleStateImageBehavior = false;
            this.lsvFiles.View = View.Details;
            this.lsvFiles.ItemDrag += this.lsvFiles_ItemDrag;
            this.lsvFiles.DragDrop += this.lsvFiles_DragDrop;
            this.lsvFiles.DragEnter += this.lsvFiles_DragEnter;
            this.lsvFiles.KeyDown += this.lsvFiles_KeyDown;
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
            this.colSize.TextAlign = HorizontalAlignment.Right;
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
            this.mnuEntityPopup.ImageScalingSize = new Size(24, 24);
            this.mnuEntityPopup.Items.AddRange(new ToolStripItem[] { this.mnuFilePopupOpen, this.mnuFilePopupDownload, this.toolStripSeparator2, this.mnuFilePopupDelete, this.toolStripSeparator1, this.mnuFilePopupProperties });
            this.mnuEntityPopup.Name = "mnuEntityPopup";
            this.mnuEntityPopup.Size = new Size(177, 144);
            this.mnuEntityPopup.Opening += this.mnuEntityPopup_Opening;
            // 
            // mnuFilePopupOpen
            // 
            this.mnuFilePopupOpen.Name = "mnuFilePopupOpen";
            this.mnuFilePopupOpen.Size = new Size(176, 32);
            this.mnuFilePopupOpen.Text = "&Open...";
            // 
            // mnuFilePopupDownload
            // 
            this.mnuFilePopupDownload.Name = "mnuFilePopupDownload";
            this.mnuFilePopupDownload.Size = new Size(176, 32);
            this.mnuFilePopupDownload.Text = "&Download";
            this.mnuFilePopupDownload.Click += this.mnuFilePopupDownload_Click;
            // 
            // toolStripSeparator2
            // 
            this.toolStripSeparator2.Name = "toolStripSeparator2";
            this.toolStripSeparator2.Size = new Size(173, 6);
            // 
            // mnuFilePopupDelete
            // 
            this.mnuFilePopupDelete.Name = "mnuFilePopupDelete";
            this.mnuFilePopupDelete.Size = new Size(176, 32);
            this.mnuFilePopupDelete.Text = "&Delete";
            this.mnuFilePopupDelete.Click += this.mnuFilePopupDelete_Click;
            // 
            // toolStripSeparator1
            // 
            this.toolStripSeparator1.Name = "toolStripSeparator1";
            this.toolStripSeparator1.Size = new Size(173, 6);
            // 
            // mnuFilePopupProperties
            // 
            this.mnuFilePopupProperties.Name = "mnuFilePopupProperties";
            this.mnuFilePopupProperties.Size = new Size(176, 32);
            this.mnuFilePopupProperties.Text = "&Properties...";
            this.mnuFilePopupProperties.Click += this.mnuFilePopupProperties_Click;
            // 
            // btnFolderRefresh
            // 
            this.btnFolderRefresh.Anchor = AnchorStyles.Bottom | AnchorStyles.Left;
            this.btnFolderRefresh.Location = new Point(999, 514);
            this.btnFolderRefresh.Margin = new Padding(4, 5, 4, 5);
            this.btnFolderRefresh.Name = "btnFolderRefresh";
            this.btnFolderRefresh.Size = new Size(149, 38);
            this.btnFolderRefresh.TabIndex = 4;
            this.btnFolderRefresh.Text = "Refresh";
            this.btnFolderRefresh.UseVisualStyleBackColor = true;
            this.btnFolderRefresh.Click += this.btnEntityGetAll_Click;
            // 
            // openFileDialog1
            // 
            this.openFileDialog1.FileName = "openFileDialog1";
            this.openFileDialog1.Multiselect = true;
            // 
            // frmMain
            // 
            this.AutoScaleDimensions = new SizeF(10F, 25F);
            this.AutoScaleMode = AutoScaleMode.Font;
            this.ClientSize = new Size(1191, 723);
            this.Controls.Add(this.grpFiles);
            this.Controls.Add(this.btnStoreDisconnect);
            this.Controls.Add(this.grpEntityStores);
            this.Margin = new Padding(4, 5, 4, 5);
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.MinimumSize = new Size(1205, 779);
            this.Name = "frmMain";
            this.Text = "File Tester";
            this.Load += this.frmMain_Load;
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