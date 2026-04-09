namespace FileTester
{
    partial class frmFileProperties
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
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
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(frmFileProperties));
            this.btnClose = new System.Windows.Forms.Button();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.lblBucketName = new System.Windows.Forms.Label();
            this.labelEditedBy = new System.Windows.Forms.Label();
            this.lblEdited = new System.Windows.Forms.Label();
            this.labelEdited = new System.Windows.Forms.Label();
            this.lblSize = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.contextSnapshots = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.contextFileOpen = new System.Windows.Forms.ToolStripMenuItem();
            this.contextFileDownload = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator1 = new System.Windows.Forms.ToolStripSeparator();
            this.contextFileDelete = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator4 = new System.Windows.Forms.ToolStripSeparator();
            this.contextFileProperties = new System.Windows.Forms.ToolStripMenuItem();
            this.contextProperties = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.mnuMetadataCopyToClipboard = new System.Windows.Forms.ToolStripMenuItem();
            this.columnHeader1 = new System.Windows.Forms.ColumnHeader();
            this.columnHeader2 = new System.Windows.Forms.ColumnHeader();
            this.tabProperties = new System.Windows.Forms.TabPage();
            this.lsvMetadata = new System.Windows.Forms.ListView();
            this.tabSnapshots = new System.Windows.Forms.TabPage();
            this.lsvFileVersions = new System.Windows.Forms.ListView();
            this.colID = new System.Windows.Forms.ColumnHeader();
            this.colName = new System.Windows.Forms.ColumnHeader();
            this.colSize = new System.Windows.Forms.ColumnHeader();
            this.columnHeader3 = new System.Windows.Forms.ColumnHeader();
            this.tabControl1 = new System.Windows.Forms.TabControl();
            this.groupBox2.SuspendLayout();
            this.contextSnapshots.SuspendLayout();
            this.contextProperties.SuspendLayout();
            this.tabProperties.SuspendLayout();
            this.tabSnapshots.SuspendLayout();
            this.tabControl1.SuspendLayout();
            this.SuspendLayout();
            // 
            // btnClose
            // 
            resources.ApplyResources(this.btnClose, "btnClose");
            this.btnClose.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.btnClose.Name = "btnClose";
            this.btnClose.Click += new System.EventHandler(this.btnClose_Click);
            // 
            // groupBox2
            // 
            resources.ApplyResources(this.groupBox2, "groupBox2");
            this.groupBox2.Controls.Add(this.lblBucketName);
            this.groupBox2.Controls.Add(this.labelEditedBy);
            this.groupBox2.Controls.Add(this.lblEdited);
            this.groupBox2.Controls.Add(this.labelEdited);
            this.groupBox2.Controls.Add(this.lblSize);
            this.groupBox2.Controls.Add(this.label1);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.TabStop = false;
            // 
            // lblBucketName
            // 
            resources.ApplyResources(this.lblBucketName, "lblBucketName");
            this.lblBucketName.Name = "lblBucketName";
            // 
            // labelEditedBy
            // 
            resources.ApplyResources(this.labelEditedBy, "labelEditedBy");
            this.labelEditedBy.Name = "labelEditedBy";
            // 
            // lblEdited
            // 
            resources.ApplyResources(this.lblEdited, "lblEdited");
            this.lblEdited.Name = "lblEdited";
            // 
            // labelEdited
            // 
            resources.ApplyResources(this.labelEdited, "labelEdited");
            this.labelEdited.Name = "labelEdited";
            // 
            // lblSize
            // 
            resources.ApplyResources(this.lblSize, "lblSize");
            this.lblSize.Name = "lblSize";
            // 
            // label1
            // 
            resources.ApplyResources(this.label1, "label1");
            this.label1.Name = "label1";
            // 
            // contextSnapshots
            // 
            this.contextSnapshots.ImageScalingSize = new System.Drawing.Size(32, 32);
            this.contextSnapshots.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.contextFileOpen,
            this.contextFileDownload,
            this.toolStripSeparator1,
            this.contextFileDelete,
            this.toolStripSeparator4,
            this.contextFileProperties});
            this.contextSnapshots.Name = "contextFile";
            resources.ApplyResources(this.contextSnapshots, "contextSnapshots");
            this.contextSnapshots.Opening += new System.ComponentModel.CancelEventHandler(this.contextFile_Opening);
            // 
            // contextFileOpen
            // 
            this.contextFileOpen.Name = "contextFileOpen";
            resources.ApplyResources(this.contextFileOpen, "contextFileOpen");
            this.contextFileOpen.Click += new System.EventHandler(this.contextFileOpen_Click);
            // 
            // contextFileDownload
            // 
            this.contextFileDownload.Name = "contextFileDownload";
            resources.ApplyResources(this.contextFileDownload, "contextFileDownload");
            this.contextFileDownload.Click += new System.EventHandler(this.contextFileDownload_Click);
            // 
            // toolStripSeparator1
            // 
            this.toolStripSeparator1.Name = "toolStripSeparator1";
            resources.ApplyResources(this.toolStripSeparator1, "toolStripSeparator1");
            // 
            // contextFileDelete
            // 
            this.contextFileDelete.Name = "contextFileDelete";
            resources.ApplyResources(this.contextFileDelete, "contextFileDelete");
            this.contextFileDelete.Click += new System.EventHandler(this.contextFileDelete_Click);
            // 
            // toolStripSeparator4
            // 
            this.toolStripSeparator4.Name = "toolStripSeparator4";
            resources.ApplyResources(this.toolStripSeparator4, "toolStripSeparator4");
            // 
            // contextFileProperties
            // 
            this.contextFileProperties.Name = "contextFileProperties";
            resources.ApplyResources(this.contextFileProperties, "contextFileProperties");
            this.contextFileProperties.Click += new System.EventHandler(this.contextFileProperties_Click);
            // 
            // contextProperties
            // 
            this.contextProperties.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.mnuMetadataCopyToClipboard});
            this.contextProperties.Name = "contextProperties";
            resources.ApplyResources(this.contextProperties, "contextProperties");
            // 
            // mnuMetadataCopyToClipboard
            // 
            this.mnuMetadataCopyToClipboard.Name = "mnuMetadataCopyToClipboard";
            resources.ApplyResources(this.mnuMetadataCopyToClipboard, "mnuMetadataCopyToClipboard");
            this.mnuMetadataCopyToClipboard.Click += new System.EventHandler(this.mnuMetadataCopyToClipboard_Click);
            // 
            // columnHeader1
            // 
            this.columnHeader1.Name = "columnHeader1";
            resources.ApplyResources(this.columnHeader1, "columnHeader1");
            // 
            // columnHeader2
            // 
            this.columnHeader2.Name = "columnHeader2";
            resources.ApplyResources(this.columnHeader2, "columnHeader2");
            // 
            // tabProperties
            // 
            this.tabProperties.Controls.Add(this.lsvMetadata);
            resources.ApplyResources(this.tabProperties, "tabProperties");
            this.tabProperties.Name = "tabProperties";
            this.tabProperties.UseVisualStyleBackColor = true;
            // 
            // lsvMetadata
            // 
            this.lsvMetadata.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.columnHeader1,
            this.columnHeader2});
            this.lsvMetadata.ContextMenuStrip = this.contextProperties;
            resources.ApplyResources(this.lsvMetadata, "lsvMetadata");
            this.lsvMetadata.FullRowSelect = true;
            this.lsvMetadata.GridLines = true;
            this.lsvMetadata.Name = "lsvMetadata";
            this.lsvMetadata.Sorting = System.Windows.Forms.SortOrder.Ascending;
            this.lsvMetadata.UseCompatibleStateImageBehavior = false;
            this.lsvMetadata.View = System.Windows.Forms.View.Details;
            // 
            // tabSnapshots
            // 
            this.tabSnapshots.Controls.Add(this.lsvFileVersions);
            resources.ApplyResources(this.tabSnapshots, "tabSnapshots");
            this.tabSnapshots.Name = "tabSnapshots";
            this.tabSnapshots.UseVisualStyleBackColor = true;
            // 
            // lsvFileVersions
            // 
            resources.ApplyResources(this.lsvFileVersions, "lsvFileVersions");
            this.lsvFileVersions.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.colID,
            this.colName,
            this.colSize,
            this.columnHeader3});
            this.lsvFileVersions.ContextMenuStrip = this.contextSnapshots;
            this.lsvFileVersions.FullRowSelect = true;
            this.lsvFileVersions.GridLines = true;
            this.lsvFileVersions.MultiSelect = false;
            this.lsvFileVersions.Name = "lsvFileVersions";
            this.lsvFileVersions.UseCompatibleStateImageBehavior = false;
            this.lsvFileVersions.View = System.Windows.Forms.View.Details;
            // 
            // colID
            // 
            resources.ApplyResources(this.colID, "colID");
            // 
            // colName
            // 
            this.colName.Name = "colName";
            resources.ApplyResources(this.colName, "colName");
            // 
            // colSize
            // 
            this.colSize.Name = "colSize";
            resources.ApplyResources(this.colSize, "colSize");
            // 
            // columnHeader3
            // 
            this.columnHeader3.Name = "columnHeader3";
            resources.ApplyResources(this.columnHeader3, "columnHeader3");
            // 
            // tabControl1
            // 
            resources.ApplyResources(this.tabControl1, "tabControl1");
            this.tabControl1.Controls.Add(this.tabSnapshots);
            this.tabControl1.Controls.Add(this.tabProperties);
            this.tabControl1.Name = "tabControl1";
            this.tabControl1.SelectedIndex = 0;
            // 
            // frmFileProperties
            // 
            resources.ApplyResources(this, "$this");
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.CancelButton = this.btnClose;
            this.Controls.Add(this.tabControl1);
            this.Controls.Add(this.groupBox2);
            this.Controls.Add(this.btnClose);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "frmFileProperties";
            this.Shown += new System.EventHandler(this.frmFileProperties_Shown);
            this.groupBox2.ResumeLayout(false);
            this.groupBox2.PerformLayout();
            this.contextSnapshots.ResumeLayout(false);
            this.contextProperties.ResumeLayout(false);
            this.tabProperties.ResumeLayout(false);
            this.tabSnapshots.ResumeLayout(false);
            this.tabControl1.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Button btnClose;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.Label lblSize;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.ContextMenuStrip contextSnapshots;
        private System.Windows.Forms.ToolStripMenuItem contextFileOpen;
        private System.Windows.Forms.ToolStripMenuItem contextFileDownload;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator4;
        private System.Windows.Forms.ToolStripMenuItem contextFileDelete;
        private System.Windows.Forms.ToolStripMenuItem contextFileProperties;
        private System.Windows.Forms.Label lblBucketName;
        private System.Windows.Forms.Label labelEditedBy;
        private System.Windows.Forms.Label lblEdited;
        private System.Windows.Forms.Label labelEdited;
        private System.Windows.Forms.ContextMenuStrip contextProperties;
        private System.Windows.Forms.ToolStripMenuItem mnuMetadataCopyToClipboard;
        private ColumnHeader columnHeader1;
        private ColumnHeader columnHeader2;
        private TabPage tabProperties;
        private ListView lsvMetadata;
        private TabPage tabSnapshots;
        private ListView lsvFileVersions;
        private ColumnHeader colName;
        private ColumnHeader colSize;
        private ColumnHeader columnHeader3;
        private TabControl tabControl1;
        private ToolStripSeparator toolStripSeparator1;
        private ColumnHeader colID;
    }
}