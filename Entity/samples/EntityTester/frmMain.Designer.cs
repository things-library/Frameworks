namespace CloudEntityTester
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
            this.grpEntities = new System.Windows.Forms.GroupBox();
            this.btnEntityNew = new System.Windows.Forms.Button();
            this.lsvEntities = new System.Windows.Forms.ListView();
            this.colName = new System.Windows.Forms.ColumnHeader();
            this.colType = new System.Windows.Forms.ColumnHeader();
            this.colSize = new System.Windows.Forms.ColumnHeader();
            this.btnFolderRefresh = new System.Windows.Forms.Button();
            this.mnuEntityPopup = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.mnuEntityPopupAdd = new System.Windows.Forms.ToolStripMenuItem();
            this.mnuEntityPopupRemove = new System.Windows.Forms.ToolStripMenuItem();
            this.grpEntity = new System.Windows.Forms.GroupBox();
            this.label3 = new System.Windows.Forms.Label();
            this.lsvEntityProperties = new System.Windows.Forms.ListView();
            this.colKey = new System.Windows.Forms.ColumnHeader();
            this.colValue = new System.Windows.Forms.ColumnHeader();
            this.btnEntityDelete = new System.Windows.Forms.Button();
            this.btnEntitySave = new System.Windows.Forms.Button();
            this.txtEntityId = new System.Windows.Forms.TextBox();
            this.label2 = new System.Windows.Forms.Label();
            this.txtInstanceId = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.grpEntityStores.SuspendLayout();
            this.grpEntities.SuspendLayout();
            this.mnuEntityPopup.SuspendLayout();
            this.grpEntity.SuspendLayout();
            this.SuspendLayout();
            // 
            // grpEntityStores
            // 
            this.grpEntityStores.Controls.Add(this.btnStoreConnect);
            this.grpEntityStores.Controls.Add(this.cboEntityStore);
            this.grpEntityStores.Location = new System.Drawing.Point(12, 12);
            this.grpEntityStores.Name = "grpEntityStores";
            this.grpEntityStores.Size = new System.Drawing.Size(347, 58);
            this.grpEntityStores.TabIndex = 0;
            this.grpEntityStores.TabStop = false;
            this.grpEntityStores.Text = "File Store";
            // 
            // btnStoreConnect
            // 
            this.btnStoreConnect.Location = new System.Drawing.Point(257, 22);
            this.btnStoreConnect.Name = "btnStoreConnect";
            this.btnStoreConnect.Size = new System.Drawing.Size(75, 23);
            this.btnStoreConnect.TabIndex = 1;
            this.btnStoreConnect.Text = "Connect";
            this.btnStoreConnect.UseVisualStyleBackColor = true;
            this.btnStoreConnect.Click += new System.EventHandler(this.btnStoreConnect_Click);
            // 
            // cboEntityStore
            // 
            this.cboEntityStore.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cboEntityStore.FormattingEnabled = true;
            this.cboEntityStore.Location = new System.Drawing.Point(18, 22);
            this.cboEntityStore.Name = "cboEntityStore";
            this.cboEntityStore.Size = new System.Drawing.Size(233, 23);
            this.cboEntityStore.TabIndex = 0;
            // 
            // btnStoreDisconnect
            // 
            this.btnStoreDisconnect.Location = new System.Drawing.Point(270, 33);
            this.btnStoreDisconnect.Name = "btnStoreDisconnect";
            this.btnStoreDisconnect.Size = new System.Drawing.Size(75, 24);
            this.btnStoreDisconnect.TabIndex = 6;
            this.btnStoreDisconnect.Text = "Disconnect";
            this.btnStoreDisconnect.UseVisualStyleBackColor = true;
            this.btnStoreDisconnect.Visible = false;
            this.btnStoreDisconnect.Click += new System.EventHandler(this.btnStoreDisconnect_Click);
            // 
            // grpEntities
            // 
            this.grpEntities.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.grpEntities.Controls.Add(this.btnEntityNew);
            this.grpEntities.Controls.Add(this.lsvEntities);
            this.grpEntities.Controls.Add(this.btnFolderRefresh);
            this.grpEntities.Enabled = false;
            this.grpEntities.Location = new System.Drawing.Point(12, 78);
            this.grpEntities.Name = "grpEntities";
            this.grpEntities.Size = new System.Drawing.Size(506, 574);
            this.grpEntities.TabIndex = 13;
            this.grpEntities.TabStop = false;
            this.grpEntities.Text = "Entities";
            // 
            // btnEntityNew
            // 
            this.btnEntityNew.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.btnEntityNew.Location = new System.Drawing.Point(395, 539);
            this.btnEntityNew.Name = "btnEntityNew";
            this.btnEntityNew.Size = new System.Drawing.Size(104, 23);
            this.btnEntityNew.TabIndex = 10;
            this.btnEntityNew.Text = "New Entity";
            this.btnEntityNew.UseVisualStyleBackColor = true;
            this.btnEntityNew.Click += new System.EventHandler(this.btnEntityNew_Click_1);
            // 
            // lsvEntities
            // 
            this.lsvEntities.Activation = System.Windows.Forms.ItemActivation.OneClick;
            this.lsvEntities.AllowDrop = true;
            this.lsvEntities.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.lsvEntities.BackColor = System.Drawing.SystemColors.Window;
            this.lsvEntities.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.colName,
            this.colType,
            this.colSize});
            this.lsvEntities.FullRowSelect = true;
            this.lsvEntities.GridLines = true;
            this.lsvEntities.HeaderStyle = System.Windows.Forms.ColumnHeaderStyle.Nonclickable;
            this.lsvEntities.LabelWrap = false;
            this.lsvEntities.Location = new System.Drawing.Point(7, 22);
            this.lsvEntities.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.lsvEntities.MultiSelect = false;
            this.lsvEntities.Name = "lsvEntities";
            this.lsvEntities.Size = new System.Drawing.Size(492, 511);
            this.lsvEntities.TabIndex = 9;
            this.lsvEntities.UseCompatibleStateImageBehavior = false;
            this.lsvEntities.View = System.Windows.Forms.View.Details;
            this.lsvEntities.SelectedIndexChanged += new System.EventHandler(this.lsvEntities_SelectedIndexChanged);
            // 
            // colName
            // 
            this.colName.Name = "colName";
            this.colName.Text = "Instance ID";
            this.colName.Width = 150;
            // 
            // colType
            // 
            this.colType.Name = "colType";
            this.colType.Text = "ID";
            this.colType.Width = 120;
            // 
            // colSize
            // 
            this.colSize.Name = "colSize";
            this.colSize.Text = "Updated";
            this.colSize.Width = 200;
            // 
            // btnFolderRefresh
            // 
            this.btnFolderRefresh.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.btnFolderRefresh.Location = new System.Drawing.Point(7, 539);
            this.btnFolderRefresh.Name = "btnFolderRefresh";
            this.btnFolderRefresh.Size = new System.Drawing.Size(104, 23);
            this.btnFolderRefresh.TabIndex = 4;
            this.btnFolderRefresh.Text = "Refresh";
            this.btnFolderRefresh.UseVisualStyleBackColor = true;
            this.btnFolderRefresh.Click += new System.EventHandler(this.btnEntityGetAll_Click);
            // 
            // mnuEntityPopup
            // 
            this.mnuEntityPopup.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.mnuEntityPopupAdd,
            this.mnuEntityPopupRemove});
            this.mnuEntityPopup.Name = "mnuEntityPopup";
            this.mnuEntityPopup.Size = new System.Drawing.Size(118, 48);
            this.mnuEntityPopup.Opening += new System.ComponentModel.CancelEventHandler(this.mnuEntityPopup_Opening);
            // 
            // mnuEntityPopupAdd
            // 
            this.mnuEntityPopupAdd.Name = "mnuEntityPopupAdd";
            this.mnuEntityPopupAdd.Size = new System.Drawing.Size(117, 22);
            this.mnuEntityPopupAdd.Text = "&Add...";
            this.mnuEntityPopupAdd.Click += new System.EventHandler(this.mnuEntityPopupAdd_Click);
            // 
            // mnuEntityPopupRemove
            // 
            this.mnuEntityPopupRemove.Name = "mnuEntityPopupRemove";
            this.mnuEntityPopupRemove.Size = new System.Drawing.Size(117, 22);
            this.mnuEntityPopupRemove.Text = "&Remove";
            this.mnuEntityPopupRemove.Click += new System.EventHandler(this.mnuEntityPopupRemove_Click);
            // 
            // grpEntity
            // 
            this.grpEntity.Controls.Add(this.label3);
            this.grpEntity.Controls.Add(this.lsvEntityProperties);
            this.grpEntity.Controls.Add(this.btnEntityDelete);
            this.grpEntity.Controls.Add(this.btnEntitySave);
            this.grpEntity.Controls.Add(this.txtEntityId);
            this.grpEntity.Controls.Add(this.label2);
            this.grpEntity.Controls.Add(this.txtInstanceId);
            this.grpEntity.Controls.Add(this.label1);
            this.grpEntity.Enabled = false;
            this.grpEntity.Location = new System.Drawing.Point(524, 78);
            this.grpEntity.Name = "grpEntity";
            this.grpEntity.Size = new System.Drawing.Size(298, 574);
            this.grpEntity.TabIndex = 14;
            this.grpEntity.TabStop = false;
            this.grpEntity.Text = "Entity";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(10, 125);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(60, 15);
            this.label3.TabIndex = 8;
            this.label3.Text = "Properties";
            // 
            // lsvEntityProperties
            // 
            this.lsvEntityProperties.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.colKey,
            this.colValue});
            this.lsvEntityProperties.ContextMenuStrip = this.mnuEntityPopup;
            this.lsvEntityProperties.FullRowSelect = true;
            this.lsvEntityProperties.GridLines = true;
            this.lsvEntityProperties.Location = new System.Drawing.Point(10, 143);
            this.lsvEntityProperties.Name = "lsvEntityProperties";
            this.lsvEntityProperties.Size = new System.Drawing.Size(282, 390);
            this.lsvEntityProperties.TabIndex = 7;
            this.lsvEntityProperties.UseCompatibleStateImageBehavior = false;
            this.lsvEntityProperties.View = System.Windows.Forms.View.Details;
            // 
            // colKey
            // 
            this.colKey.Text = "Key";
            this.colKey.Width = 100;
            // 
            // colValue
            // 
            this.colValue.Text = "Value";
            this.colValue.Width = 100;
            // 
            // btnEntityDelete
            // 
            this.btnEntityDelete.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnEntityDelete.Location = new System.Drawing.Point(188, 539);
            this.btnEntityDelete.Name = "btnEntityDelete";
            this.btnEntityDelete.Size = new System.Drawing.Size(104, 23);
            this.btnEntityDelete.TabIndex = 6;
            this.btnEntityDelete.Text = "Delete";
            this.btnEntityDelete.UseVisualStyleBackColor = true;
            this.btnEntityDelete.Click += new System.EventHandler(this.btnEntityDelete_Click);
            // 
            // btnEntitySave
            // 
            this.btnEntitySave.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.btnEntitySave.Location = new System.Drawing.Point(10, 539);
            this.btnEntitySave.Name = "btnEntitySave";
            this.btnEntitySave.Size = new System.Drawing.Size(104, 23);
            this.btnEntitySave.TabIndex = 5;
            this.btnEntitySave.Text = "Save";
            this.btnEntitySave.UseVisualStyleBackColor = true;
            this.btnEntitySave.Click += new System.EventHandler(this.btnEntityUpdate_Click);
            // 
            // txtEntityId
            // 
            this.txtEntityId.Location = new System.Drawing.Point(10, 86);
            this.txtEntityId.Name = "txtEntityId";
            this.txtEntityId.Size = new System.Drawing.Size(282, 23);
            this.txtEntityId.TabIndex = 3;
            this.txtEntityId.Leave += new System.EventHandler(this.txtEntityId_Leave);
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(10, 68);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(44, 15);
            this.label2.TabIndex = 2;
            this.label2.Text = "Row ID";
            // 
            // txtInstanceId
            // 
            this.txtInstanceId.Location = new System.Drawing.Point(10, 42);
            this.txtInstanceId.Name = "txtInstanceId";
            this.txtInstanceId.Size = new System.Drawing.Size(282, 23);
            this.txtInstanceId.TabIndex = 1;
            this.txtInstanceId.Leave += new System.EventHandler(this.txtInstanceId_Leave);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(10, 24);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(65, 15);
            this.label1.TabIndex = 0;
            this.label1.Text = "Instance ID";
            // 
            // frmMain
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(834, 664);
            this.Controls.Add(this.grpEntity);
            this.Controls.Add(this.grpEntities);
            this.Controls.Add(this.btnStoreDisconnect);
            this.Controls.Add(this.grpEntityStores);
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.MinimumSize = new System.Drawing.Size(850, 490);
            this.Name = "frmMain";
            this.Text = "Cloud Entity Tester";
            this.Load += new System.EventHandler(this.frmMain_Load);
            this.grpEntityStores.ResumeLayout(false);
            this.grpEntities.ResumeLayout(false);
            this.mnuEntityPopup.ResumeLayout(false);
            this.grpEntity.ResumeLayout(false);
            this.grpEntity.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private GroupBox grpEntityStores;
        private ComboBox cboEntityStore;
        private Button btnStoreConnect;
        private Button btnStoreDisconnect;
        private GroupBox grpEntities;
        private Button btnFolderRefresh;
        private ContextMenuStrip mnuEntityPopup;
        private ToolStripMenuItem mnuEntityPopupAdd;
        private ToolStripMenuItem mnuEntityPopupRemove;
        private ListView lsvEntities;   
        private GroupBox grpEntity;
        private TextBox txtEntityId;
        private Label label2;
        private TextBox txtInstanceId;
        private Label label1;
        private Label label3;
        private ListView lsvEntityProperties;
        private Button btnEntityDelete;
        private Button btnEntitySave;
        private ColumnHeader colKey;
        private ColumnHeader colValue;
        private ColumnHeader colName;
        private ColumnHeader colType;
        private ColumnHeader colSize;
        private Button btnEntityNew;
    }
}