namespace EntityTester
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
            this.grpEntities = new GroupBox();
            this.btnEntityNew = new Button();
            this.lsvEntities = new ListView();
            this.colName = new ColumnHeader();
            this.colType = new ColumnHeader();
            this.colSize = new ColumnHeader();
            this.btnFolderRefresh = new Button();
            this.mnuEntityPopup = new ContextMenuStrip(this.components);
            this.mnuEntityPopupAdd = new ToolStripMenuItem();
            this.mnuEntityPopupRemove = new ToolStripMenuItem();
            this.grpEntity = new GroupBox();
            this.label3 = new Label();
            this.lsvEntityProperties = new ListView();
            this.colKey = new ColumnHeader();
            this.colValue = new ColumnHeader();
            this.btnEntityDelete = new Button();
            this.btnEntitySave = new Button();
            this.txtRowKey = new TextBox();
            this.label2 = new Label();
            this.txtPartitionKey = new TextBox();
            this.label1 = new Label();
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
            this.grpEntityStores.Location = new Point(17, 20);
            this.grpEntityStores.Margin = new Padding(4, 5, 4, 5);
            this.grpEntityStores.Name = "grpEntityStores";
            this.grpEntityStores.Padding = new Padding(4, 5, 4, 5);
            this.grpEntityStores.Size = new Size(496, 97);
            this.grpEntityStores.TabIndex = 0;
            this.grpEntityStores.TabStop = false;
            this.grpEntityStores.Text = "File Store";
            // 
            // btnStoreConnect
            // 
            this.btnStoreConnect.Location = new Point(367, 37);
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
            this.cboEntityStore.DropDownStyle = ComboBoxStyle.DropDownList;
            this.cboEntityStore.FormattingEnabled = true;
            this.cboEntityStore.Location = new Point(26, 37);
            this.cboEntityStore.Margin = new Padding(4, 5, 4, 5);
            this.cboEntityStore.Name = "cboEntityStore";
            this.cboEntityStore.Size = new Size(331, 33);
            this.cboEntityStore.TabIndex = 0;
            // 
            // btnStoreDisconnect
            // 
            this.btnStoreDisconnect.Location = new Point(386, 55);
            this.btnStoreDisconnect.Margin = new Padding(4, 5, 4, 5);
            this.btnStoreDisconnect.Name = "btnStoreDisconnect";
            this.btnStoreDisconnect.Size = new Size(107, 40);
            this.btnStoreDisconnect.TabIndex = 6;
            this.btnStoreDisconnect.Text = "Disconnect";
            this.btnStoreDisconnect.UseVisualStyleBackColor = true;
            this.btnStoreDisconnect.Visible = false;
            this.btnStoreDisconnect.Click += this.btnStoreDisconnect_Click;
            // 
            // grpEntities
            // 
            this.grpEntities.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            this.grpEntities.Controls.Add(this.btnEntityNew);
            this.grpEntities.Controls.Add(this.lsvEntities);
            this.grpEntities.Controls.Add(this.btnFolderRefresh);
            this.grpEntities.Enabled = false;
            this.grpEntities.Location = new Point(17, 130);
            this.grpEntities.Margin = new Padding(4, 5, 4, 5);
            this.grpEntities.Name = "grpEntities";
            this.grpEntities.Padding = new Padding(4, 5, 4, 5);
            this.grpEntities.Size = new Size(723, 573);
            this.grpEntities.TabIndex = 13;
            this.grpEntities.TabStop = false;
            this.grpEntities.Text = "Entities";
            // 
            // btnEntityNew
            // 
            this.btnEntityNew.Anchor = AnchorStyles.Bottom | AnchorStyles.Left;
            this.btnEntityNew.Location = new Point(564, 514);
            this.btnEntityNew.Margin = new Padding(4, 5, 4, 5);
            this.btnEntityNew.Name = "btnEntityNew";
            this.btnEntityNew.Size = new Size(149, 38);
            this.btnEntityNew.TabIndex = 10;
            this.btnEntityNew.Text = "New Entity";
            this.btnEntityNew.UseVisualStyleBackColor = true;
            this.btnEntityNew.Click += this.btnEntityNew_Click_1;
            // 
            // lsvEntities
            // 
            this.lsvEntities.Activation = ItemActivation.OneClick;
            this.lsvEntities.AllowDrop = true;
            this.lsvEntities.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            this.lsvEntities.BackColor = SystemColors.Window;
            this.lsvEntities.Columns.AddRange(new ColumnHeader[] { this.colName, this.colType, this.colSize });
            this.lsvEntities.FullRowSelect = true;
            this.lsvEntities.GridLines = true;
            this.lsvEntities.HeaderStyle = ColumnHeaderStyle.Nonclickable;
            this.lsvEntities.LabelWrap = false;
            this.lsvEntities.Location = new Point(10, 37);
            this.lsvEntities.Margin = new Padding(6, 5, 6, 5);
            this.lsvEntities.MultiSelect = false;
            this.lsvEntities.Name = "lsvEntities";
            this.lsvEntities.Size = new Size(701, 465);
            this.lsvEntities.TabIndex = 9;
            this.lsvEntities.UseCompatibleStateImageBehavior = false;
            this.lsvEntities.View = View.Details;
            this.lsvEntities.SelectedIndexChanged += this.lsvEntities_SelectedIndexChanged;
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
            this.btnFolderRefresh.Anchor = AnchorStyles.Bottom | AnchorStyles.Left;
            this.btnFolderRefresh.Location = new Point(10, 514);
            this.btnFolderRefresh.Margin = new Padding(4, 5, 4, 5);
            this.btnFolderRefresh.Name = "btnFolderRefresh";
            this.btnFolderRefresh.Size = new Size(149, 38);
            this.btnFolderRefresh.TabIndex = 4;
            this.btnFolderRefresh.Text = "Refresh";
            this.btnFolderRefresh.UseVisualStyleBackColor = true;
            this.btnFolderRefresh.Click += this.btnEntityGetAll_Click;
            // 
            // mnuEntityPopup
            // 
            this.mnuEntityPopup.ImageScalingSize = new Size(24, 24);
            this.mnuEntityPopup.Items.AddRange(new ToolStripItem[] { this.mnuEntityPopupAdd, this.mnuEntityPopupRemove });
            this.mnuEntityPopup.Name = "mnuEntityPopup";
            this.mnuEntityPopup.Size = new Size(149, 68);
            this.mnuEntityPopup.Opening += this.mnuEntityPopup_Opening;
            // 
            // mnuEntityPopupAdd
            // 
            this.mnuEntityPopupAdd.Name = "mnuEntityPopupAdd";
            this.mnuEntityPopupAdd.Size = new Size(148, 32);
            this.mnuEntityPopupAdd.Text = "&Add...";
            this.mnuEntityPopupAdd.Click += this.mnuEntityPopupAdd_Click;
            // 
            // mnuEntityPopupRemove
            // 
            this.mnuEntityPopupRemove.Name = "mnuEntityPopupRemove";
            this.mnuEntityPopupRemove.Size = new Size(148, 32);
            this.mnuEntityPopupRemove.Text = "&Remove";
            this.mnuEntityPopupRemove.Click += this.mnuEntityPopupRemove_Click;
            // 
            // grpEntity
            // 
            this.grpEntity.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Right;
            this.grpEntity.Controls.Add(this.label3);
            this.grpEntity.Controls.Add(this.lsvEntityProperties);
            this.grpEntity.Controls.Add(this.btnEntityDelete);
            this.grpEntity.Controls.Add(this.btnEntitySave);
            this.grpEntity.Controls.Add(this.txtRowKey);
            this.grpEntity.Controls.Add(this.label2);
            this.grpEntity.Controls.Add(this.txtPartitionKey);
            this.grpEntity.Controls.Add(this.label1);
            this.grpEntity.Enabled = false;
            this.grpEntity.Location = new Point(749, 130);
            this.grpEntity.Margin = new Padding(4, 5, 4, 5);
            this.grpEntity.Name = "grpEntity";
            this.grpEntity.Padding = new Padding(4, 5, 4, 5);
            this.grpEntity.Size = new Size(426, 573);
            this.grpEntity.TabIndex = 14;
            this.grpEntity.TabStop = false;
            this.grpEntity.Text = "Entity";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new Point(14, 208);
            this.label3.Margin = new Padding(4, 0, 4, 0);
            this.label3.Name = "label3";
            this.label3.Size = new Size(92, 25);
            this.label3.TabIndex = 8;
            this.label3.Text = "Properties";
            // 
            // lsvEntityProperties
            // 
            this.lsvEntityProperties.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            this.lsvEntityProperties.Columns.AddRange(new ColumnHeader[] { this.colKey, this.colValue });
            this.lsvEntityProperties.ContextMenuStrip = this.mnuEntityPopup;
            this.lsvEntityProperties.FullRowSelect = true;
            this.lsvEntityProperties.GridLines = true;
            this.lsvEntityProperties.Location = new Point(14, 238);
            this.lsvEntityProperties.Margin = new Padding(4, 5, 4, 5);
            this.lsvEntityProperties.Name = "lsvEntityProperties";
            this.lsvEntityProperties.Size = new Size(401, 263);
            this.lsvEntityProperties.TabIndex = 7;
            this.lsvEntityProperties.UseCompatibleStateImageBehavior = false;
            this.lsvEntityProperties.View = View.Details;
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
            this.btnEntityDelete.Anchor = AnchorStyles.Bottom | AnchorStyles.Right;
            this.btnEntityDelete.Location = new Point(269, 514);
            this.btnEntityDelete.Margin = new Padding(4, 5, 4, 5);
            this.btnEntityDelete.Name = "btnEntityDelete";
            this.btnEntityDelete.Size = new Size(149, 38);
            this.btnEntityDelete.TabIndex = 6;
            this.btnEntityDelete.Text = "Delete";
            this.btnEntityDelete.UseVisualStyleBackColor = true;
            this.btnEntityDelete.Click += this.btnEntityDelete_Click;
            // 
            // btnEntitySave
            // 
            this.btnEntitySave.Anchor = AnchorStyles.Bottom | AnchorStyles.Left;
            this.btnEntitySave.Location = new Point(14, 514);
            this.btnEntitySave.Margin = new Padding(4, 5, 4, 5);
            this.btnEntitySave.Name = "btnEntitySave";
            this.btnEntitySave.Size = new Size(149, 38);
            this.btnEntitySave.TabIndex = 5;
            this.btnEntitySave.Text = "Save";
            this.btnEntitySave.UseVisualStyleBackColor = true;
            this.btnEntitySave.Click += this.btnEntityUpdate_Click;
            // 
            // txtRowKey
            // 
            this.txtRowKey.Location = new Point(14, 143);
            this.txtRowKey.Margin = new Padding(4, 5, 4, 5);
            this.txtRowKey.Name = "txtRowKey";
            this.txtRowKey.Size = new Size(401, 31);
            this.txtRowKey.TabIndex = 3;
            this.txtRowKey.Leave += this.txtEntityId_Leave;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new Point(14, 113);
            this.label2.Margin = new Padding(4, 0, 4, 0);
            this.label2.Name = "label2";
            this.label2.Size = new Size(79, 25);
            this.label2.TabIndex = 2;
            this.label2.Text = "Row Key";
            // 
            // txtPartitionKey
            // 
            this.txtPartitionKey.Location = new Point(14, 70);
            this.txtPartitionKey.Margin = new Padding(4, 5, 4, 5);
            this.txtPartitionKey.Name = "txtPartitionKey";
            this.txtPartitionKey.Size = new Size(401, 31);
            this.txtPartitionKey.TabIndex = 1;
            this.txtPartitionKey.Leave += this.txtInstanceId_Leave;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new Point(14, 40);
            this.label1.Margin = new Padding(4, 0, 4, 0);
            this.label1.Name = "label1";
            this.label1.Size = new Size(110, 25);
            this.label1.TabIndex = 0;
            this.label1.Text = "Partition Key";
            // 
            // frmMain
            // 
            this.AutoScaleDimensions = new SizeF(10F, 25F);
            this.AutoScaleMode = AutoScaleMode.Font;
            this.ClientSize = new Size(1191, 723);
            this.Controls.Add(this.grpEntity);
            this.Controls.Add(this.grpEntities);
            this.Controls.Add(this.grpEntityStores);
            this.Controls.Add(this.btnStoreDisconnect);
            this.Margin = new Padding(4, 5, 4, 5);
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.MinimumSize = new Size(1205, 779);
            this.Name = "frmMain";
            this.Text = "Entity Tester";
            this.Load += this.frmMain_Load;
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
        private TextBox txtRowKey;
        private Label label2;
        private TextBox txtPartitionKey;
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