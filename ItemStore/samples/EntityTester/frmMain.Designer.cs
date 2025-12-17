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
            components = new System.ComponentModel.Container();
            grpEntityStores = new GroupBox();
            btnStoreConnect = new Button();
            cboEntityStore = new ComboBox();
            btnStoreDisconnect = new Button();
            grpEntities = new GroupBox();
            btnEntityNew = new Button();
            lsvEntities = new ListView();
            colName = new ColumnHeader();
            colType = new ColumnHeader();
            colSize = new ColumnHeader();
            btnFolderRefresh = new Button();
            mnuEntityPopup = new ContextMenuStrip(components);
            mnuEntityPopupAdd = new ToolStripMenuItem();
            mnuEntityPopupRemove = new ToolStripMenuItem();
            grpEntity = new GroupBox();
            label3 = new Label();
            lsvEntityProperties = new ListView();
            colKey = new ColumnHeader();
            colValue = new ColumnHeader();
            btnEntityDelete = new Button();
            btnEntitySave = new Button();
            txtEntityId = new TextBox();
            label2 = new Label();
            txtInstanceId = new TextBox();
            label1 = new Label();
            grpEntityStores.SuspendLayout();
            grpEntities.SuspendLayout();
            mnuEntityPopup.SuspendLayout();
            grpEntity.SuspendLayout();
            SuspendLayout();
            // 
            // grpEntityStores
            // 
            grpEntityStores.Controls.Add(btnStoreConnect);
            grpEntityStores.Controls.Add(cboEntityStore);
            grpEntityStores.Location = new Point(22, 26);
            grpEntityStores.Margin = new Padding(6);
            grpEntityStores.Name = "grpEntityStores";
            grpEntityStores.Padding = new Padding(6);
            grpEntityStores.Size = new Size(644, 124);
            grpEntityStores.TabIndex = 0;
            grpEntityStores.TabStop = false;
            grpEntityStores.Text = "Entity Store";
            // 
            // btnStoreConnect
            // 
            btnStoreConnect.Location = new Point(477, 47);
            btnStoreConnect.Margin = new Padding(6);
            btnStoreConnect.Name = "btnStoreConnect";
            btnStoreConnect.Size = new Size(139, 49);
            btnStoreConnect.TabIndex = 1;
            btnStoreConnect.Text = "Connect";
            btnStoreConnect.UseVisualStyleBackColor = true;
            btnStoreConnect.Click += BtnStoreConnect_Click;
            // 
            // cboEntityStore
            // 
            cboEntityStore.DropDownStyle = ComboBoxStyle.DropDownList;
            cboEntityStore.FormattingEnabled = true;
            cboEntityStore.Location = new Point(33, 47);
            cboEntityStore.Margin = new Padding(6);
            cboEntityStore.Name = "cboEntityStore";
            cboEntityStore.Size = new Size(429, 40);
            cboEntityStore.TabIndex = 0;
            // 
            // btnStoreDisconnect
            // 
            btnStoreDisconnect.Location = new Point(501, 70);
            btnStoreDisconnect.Margin = new Padding(6);
            btnStoreDisconnect.Name = "btnStoreDisconnect";
            btnStoreDisconnect.Size = new Size(139, 51);
            btnStoreDisconnect.TabIndex = 6;
            btnStoreDisconnect.Text = "Disconnect";
            btnStoreDisconnect.UseVisualStyleBackColor = true;
            btnStoreDisconnect.Visible = false;
            btnStoreDisconnect.Click += BtnStoreDisconnect_Click;
            // 
            // grpEntities
            // 
            grpEntities.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            grpEntities.Controls.Add(btnEntityNew);
            grpEntities.Controls.Add(lsvEntities);
            grpEntities.Controls.Add(btnFolderRefresh);
            grpEntities.Enabled = false;
            grpEntities.Location = new Point(22, 166);
            grpEntities.Margin = new Padding(6);
            grpEntities.Name = "grpEntities";
            grpEntities.Padding = new Padding(6);
            grpEntities.Size = new Size(940, 1225);
            grpEntities.TabIndex = 13;
            grpEntities.TabStop = false;
            grpEntities.Text = "Entities";
            // 
            // btnEntityNew
            // 
            btnEntityNew.Anchor = AnchorStyles.Bottom | AnchorStyles.Left;
            btnEntityNew.Location = new Point(734, 1150);
            btnEntityNew.Margin = new Padding(6);
            btnEntityNew.Name = "btnEntityNew";
            btnEntityNew.Size = new Size(193, 49);
            btnEntityNew.TabIndex = 10;
            btnEntityNew.Text = "New Entity";
            btnEntityNew.UseVisualStyleBackColor = true;
            btnEntityNew.Click += BtnEntityNew_Click_1;
            // 
            // lsvEntities
            // 
            lsvEntities.Activation = ItemActivation.OneClick;
            lsvEntities.AllowDrop = true;
            lsvEntities.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            lsvEntities.BackColor = SystemColors.Window;
            lsvEntities.Columns.AddRange(new ColumnHeader[] { colName, colType, colSize });
            lsvEntities.FullRowSelect = true;
            lsvEntities.GridLines = true;
            lsvEntities.HeaderStyle = ColumnHeaderStyle.Nonclickable;
            lsvEntities.LabelWrap = false;
            lsvEntities.Location = new Point(13, 47);
            lsvEntities.Margin = new Padding(7, 6, 7, 6);
            lsvEntities.MultiSelect = false;
            lsvEntities.Name = "lsvEntities";
            lsvEntities.Size = new Size(910, 1086);
            lsvEntities.TabIndex = 9;
            lsvEntities.UseCompatibleStateImageBehavior = false;
            lsvEntities.View = View.Details;
            lsvEntities.SelectedIndexChanged += LsvEntities_SelectedIndexChanged;
            // 
            // colName
            // 
            colName.Name = "colName";
            colName.Text = "Instance ID";
            colName.Width = 150;
            // 
            // colType
            // 
            colType.Name = "colType";
            colType.Text = "ID";
            colType.Width = 120;
            // 
            // colSize
            // 
            colSize.Name = "colSize";
            colSize.Text = "Updated";
            colSize.Width = 200;
            // 
            // btnFolderRefresh
            // 
            btnFolderRefresh.Anchor = AnchorStyles.Bottom | AnchorStyles.Left;
            btnFolderRefresh.Location = new Point(13, 1150);
            btnFolderRefresh.Margin = new Padding(6);
            btnFolderRefresh.Name = "btnFolderRefresh";
            btnFolderRefresh.Size = new Size(193, 49);
            btnFolderRefresh.TabIndex = 4;
            btnFolderRefresh.Text = "Refresh";
            btnFolderRefresh.UseVisualStyleBackColor = true;
            btnFolderRefresh.Click += BtnEntityGetAll_Click;
            // 
            // mnuEntityPopup
            // 
            mnuEntityPopup.ImageScalingSize = new Size(32, 32);
            mnuEntityPopup.Items.AddRange(new ToolStripItem[] { mnuEntityPopupAdd, mnuEntityPopupRemove });
            mnuEntityPopup.Name = "mnuEntityPopup";
            mnuEntityPopup.Size = new Size(175, 80);
            mnuEntityPopup.Opening += MnuEntityPopup_Opening;
            // 
            // mnuEntityPopupAdd
            // 
            mnuEntityPopupAdd.Name = "mnuEntityPopupAdd";
            mnuEntityPopupAdd.Size = new Size(174, 38);
            mnuEntityPopupAdd.Text = "&Add...";
            mnuEntityPopupAdd.Click += MnuEntityPopupAdd_Click;
            // 
            // mnuEntityPopupRemove
            // 
            mnuEntityPopupRemove.Name = "mnuEntityPopupRemove";
            mnuEntityPopupRemove.Size = new Size(174, 38);
            mnuEntityPopupRemove.Text = "&Remove";
            mnuEntityPopupRemove.Click += MnuEntityPopupRemove_Click;
            // 
            // grpEntity
            // 
            grpEntity.Controls.Add(label3);
            grpEntity.Controls.Add(lsvEntityProperties);
            grpEntity.Controls.Add(btnEntityDelete);
            grpEntity.Controls.Add(btnEntitySave);
            grpEntity.Controls.Add(txtEntityId);
            grpEntity.Controls.Add(label2);
            grpEntity.Controls.Add(txtInstanceId);
            grpEntity.Controls.Add(label1);
            grpEntity.Enabled = false;
            grpEntity.Location = new Point(973, 166);
            grpEntity.Margin = new Padding(6);
            grpEntity.Name = "grpEntity";
            grpEntity.Padding = new Padding(6);
            grpEntity.Size = new Size(553, 1225);
            grpEntity.TabIndex = 14;
            grpEntity.TabStop = false;
            grpEntity.Text = "Entity";
            // 
            // label3
            // 
            label3.AutoSize = true;
            label3.Location = new Point(19, 267);
            label3.Margin = new Padding(6, 0, 6, 0);
            label3.Name = "label3";
            label3.Size = new Size(121, 32);
            label3.TabIndex = 8;
            label3.Text = "Properties";
            // 
            // lsvEntityProperties
            // 
            lsvEntityProperties.Columns.AddRange(new ColumnHeader[] { colKey, colValue });
            lsvEntityProperties.ContextMenuStrip = mnuEntityPopup;
            lsvEntityProperties.FullRowSelect = true;
            lsvEntityProperties.GridLines = true;
            lsvEntityProperties.Location = new Point(19, 305);
            lsvEntityProperties.Margin = new Padding(6);
            lsvEntityProperties.Name = "lsvEntityProperties";
            lsvEntityProperties.Size = new Size(520, 827);
            lsvEntityProperties.TabIndex = 7;
            lsvEntityProperties.UseCompatibleStateImageBehavior = false;
            lsvEntityProperties.View = View.Details;
            // 
            // colKey
            // 
            colKey.Text = "Key";
            colKey.Width = 100;
            // 
            // colValue
            // 
            colValue.Text = "Value";
            colValue.Width = 100;
            // 
            // btnEntityDelete
            // 
            btnEntityDelete.Anchor = AnchorStyles.Bottom | AnchorStyles.Right;
            btnEntityDelete.Location = new Point(349, 1150);
            btnEntityDelete.Margin = new Padding(6);
            btnEntityDelete.Name = "btnEntityDelete";
            btnEntityDelete.Size = new Size(193, 49);
            btnEntityDelete.TabIndex = 6;
            btnEntityDelete.Text = "Delete";
            btnEntityDelete.UseVisualStyleBackColor = true;
            btnEntityDelete.Click += BtnEntityDelete_Click;
            // 
            // btnEntitySave
            // 
            btnEntitySave.Anchor = AnchorStyles.Bottom | AnchorStyles.Left;
            btnEntitySave.Location = new Point(19, 1150);
            btnEntitySave.Margin = new Padding(6);
            btnEntitySave.Name = "btnEntitySave";
            btnEntitySave.Size = new Size(193, 49);
            btnEntitySave.TabIndex = 5;
            btnEntitySave.Text = "Save";
            btnEntitySave.UseVisualStyleBackColor = true;
            btnEntitySave.Click += btnEntityUpdate_Click;
            // 
            // txtEntityId
            // 
            txtEntityId.Location = new Point(19, 183);
            txtEntityId.Margin = new Padding(6);
            txtEntityId.Name = "txtEntityId";
            txtEntityId.Size = new Size(520, 39);
            txtEntityId.TabIndex = 3;
            txtEntityId.Leave += TxtEntityId_Leave;
            // 
            // label2
            // 
            label2.AutoSize = true;
            label2.Location = new Point(19, 145);
            label2.Margin = new Padding(6, 0, 6, 0);
            label2.Name = "label2";
            label2.Size = new Size(88, 32);
            label2.TabIndex = 2;
            label2.Text = "Row ID";
            // 
            // txtInstanceId
            // 
            txtInstanceId.Location = new Point(19, 90);
            txtInstanceId.Margin = new Padding(6);
            txtInstanceId.Name = "txtInstanceId";
            txtInstanceId.Size = new Size(520, 39);
            txtInstanceId.TabIndex = 1;
            txtInstanceId.Leave += TxtInstanceId_Leave;
            // 
            // label1
            // 
            label1.AutoSize = true;
            label1.Location = new Point(19, 51);
            label1.Margin = new Padding(6, 0, 6, 0);
            label1.Name = "label1";
            label1.Size = new Size(132, 32);
            label1.TabIndex = 0;
            label1.Text = "Instance ID";
            // 
            // frmMain
            // 
            AutoScaleDimensions = new SizeF(13F, 32F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(1549, 1417);
            Controls.Add(grpEntity);
            Controls.Add(grpEntities);
            Controls.Add(btnStoreDisconnect);
            Controls.Add(grpEntityStores);
            Margin = new Padding(6);
            MaximizeBox = false;
            MinimizeBox = false;
            MinimumSize = new Size(1556, 965);
            Name = "frmMain";
            Text = "Cloud Entity Tester";
            Shown += FrmMain_Shown;
            grpEntityStores.ResumeLayout(false);
            grpEntities.ResumeLayout(false);
            mnuEntityPopup.ResumeLayout(false);
            grpEntity.ResumeLayout(false);
            grpEntity.PerformLayout();
            ResumeLayout(false);

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