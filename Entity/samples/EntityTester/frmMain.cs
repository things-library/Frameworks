using Microsoft.Extensions.Logging;

using Gc = Core.Cloud.Data.Entity.GCP;
using Az = Core.Cloud.Data.Entity.Azure;
using Le = Core.Cloud.Data.Entity.Local;

using Core.Cloud.Data.Entity;

namespace CloudEntityTester
{
    public partial class frmMain : Form
    {
        private ILogger<frmMain> _logger;

        private AppSettings AppSettings { get; set; }

        private IEntityStore CurrentStore { get; set; }
        private CloudEntity CurrentEntity { get; set; }

        public frmMain(AppSettings appSettings)
        {
            this.AppSettings = appSettings;

            InitializeComponent();            
        }

        private void frmMain_Load(object sender, EventArgs e)
        {
            this.cboEntityStore.DataSource = this.AppSettings.CloudEntitySettings;
            //this.cboEntityStore.DisplayMember = "Name";

            this.ManualRefresh();
        }

        private void ManualRefresh()
        {
            if(this.CurrentStore == null)
            {
                this.grpEntityStores.Enabled = true;
                this.btnStoreConnect.Visible = true;
                this.btnStoreDisconnect.Visible = false;                
                this.grpEntities.Enabled = false;
                this.grpEntity.Enabled = false;

                this.lsvEntities.Items.Clear();
                this.lsvEntityProperties.Items.Clear();
            }
            else
            {
                this.grpEntityStores.Enabled = false;
                this.btnStoreConnect.Visible = false;
                this.btnStoreDisconnect.Visible = true;                
                this.grpEntities.Enabled = true;
                this.grpEntity.Enabled = true;                
            }            
        }

        private void RefreshEntities()
        {            
            this.lsvEntities.Items.Clear();

            var entities = this.CurrentStore.GetEntities();
            foreach(var entity in entities)
            {
                var item = new ListViewItem(entity.InstanceId);
                item.SubItems.Add(entity.Id);
                item.SubItems.Add($"{entity.UpdatedOn:d} {entity.UpdatedOn:t}");
                item.Tag = entity;

                this.lsvEntities.Items.Add(item);
            }

            this.RefreshEntity();
        }


        private void btnStoreConnect_Click(object sender, EventArgs e)
        {
            this.Enabled = false;
            Application.DoEvents();

            try
            {
                this.CurrentStore = this.GetEntityStore(this.cboEntityStore.SelectedItem as CloudEntitySettings);                                
                this.CurrentEntity = new CloudEntity();
                
                this.ManualRefresh();

                this.RefreshEntities();                 
            }
            catch (Exception ex)
            {
                MessageBox.Show(this, $"Error Creating Store.\r\n\r\nError: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

            this.Enabled = true;
        }

        private void btnStoreDisconnect_Click(object sender, EventArgs e)
        {
            this.CurrentStore = null;
            this.CurrentEntity = new CloudEntity();
            GC.Collect();
                        
            this.ManualRefresh();            
        }
                
        private void btnEntityInsert_Click(object sender, EventArgs e)
        {
            this.grpEntities.Enabled = false;
            this.grpEntity.Enabled = false;
            Application.DoEvents();

            try
            {
                this.CurrentStore.InsertEntity(this.CurrentEntity);

                this.RefreshEntities();
            }
            catch (Exception ex)
            {
                MessageBox.Show(this, $"Error Inserting Entity.\r\n\r\nError: {ex.Message}", "Insert Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

            this.grpEntities.Enabled = true;
            this.grpEntity.Enabled = true;
        }

        private void btnEntityUpdate_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(this.CurrentEntity.Id))
            {
                MessageBox.Show(this, "Entity must have a id.", "ID Missing", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            this.grpEntity.Enabled = false;
            this.grpEntities.Enabled = false;
            Application.DoEvents();

            try
            {
                this.CurrentStore.UpsertEntity(this.CurrentEntity);

                // start the next possible one?
                this.CurrentEntity = new CloudEntity();

                this.RefreshEntities();
            }
            catch (Exception ex)
            {
                MessageBox.Show(this, $"Error Deleting Entity.\r\n\r\nError: {ex.Message}", "Upsert Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

            this.grpEntity.Enabled = true;
            this.grpEntities.Enabled = true;
        }

        private void btnEntityDelete_Click(object sender, EventArgs e)
        {
            this.grpEntity.Enabled = false;
            this.grpEntities.Enabled = false;
            Application.DoEvents();

            try
            {                
                this.CurrentStore.DeleteEntity(this.CurrentEntity.InstanceId, this.CurrentEntity.Id);

                this.RefreshEntities();
            }
            catch (Exception ex)
            {
                MessageBox.Show(this, $"Error Deleting Entity.\r\n\r\nError: {ex.Message}", "Delete Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

            this.grpEntity.Enabled = true;
            this.grpEntities.Enabled = true;
        }

        private void btnEntityGetAll_Click(object sender, EventArgs e)
        {
            this.grpEntities.Enabled = false;
            this.grpEntity.Enabled = false;
            Application.DoEvents();

            try
            {
                this.RefreshEntities();
            }
            catch (Exception ex)
            {
                MessageBox.Show(this, $"Error Getting Entities from store.\r\n\r\nError: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

            this.grpEntities.Enabled = true;
            this.grpEntity.Enabled = true;            
        }

        private IEntityStore GetEntityStore(CloudEntitySettings storeSettings)
        {
            if (storeSettings.Type == EntityStoreType.Azure_Table)
            {
                return new Az.EntityStore(storeSettings.Connection, storeSettings.TableName);
            }
            else if (storeSettings.Type == EntityStoreType.GCP_DataStore)
            {
                return new Gc.EntityStore(storeSettings.Connection, storeSettings.TableName);
            }
            else if (storeSettings.Type == EntityStoreType.Local)
            {
                return new Le.EntityStore(storeSettings.Connection, storeSettings.TableName);
            }
            else
            {
                MessageBox.Show(this, $"Unknown Store Type '{storeSettings.Type}'", "Invalid Type", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return null;
            }
        }

        private void btnEntityNew_Click(object sender, EventArgs e)
        {
            this.CurrentEntity = new CloudEntity();
            this.RefreshEntity();
        }

        private void lsvEntities_ItemSelectionChanged(object sender, ListViewItemSelectionChangedEventArgs e)
        {
            this.CurrentEntity = e.Item.Tag as CloudEntity;

            this.RefreshEntity();
        }

        private void RefreshEntity()
        {
            this.lsvEntityProperties.Items.Clear();

            if (this.CurrentEntity.Id == default)
            {
                this.txtInstanceId.Text = "";
                this.txtInstanceId.ReadOnly = false;

                this.txtEntityId.Text = "";
                this.txtEntityId.ReadOnly = false;

                this.btnEntityDelete.Enabled = false;
            }
            else
            {
                this.txtInstanceId.Text = this.CurrentEntity.InstanceId;
                this.txtInstanceId.ReadOnly = true;

                this.txtEntityId.Text = this.CurrentEntity.Id;
                this.txtEntityId.ReadOnly = true;

                this.btnEntityDelete.Enabled = true;
            }

            foreach (var pair in this.CurrentEntity.OrderBy(x => x.Key))
            {
                var item = new ListViewItem(pair.Key);                
                item.SubItems.Add($"{pair.Value}");                                
                item.Tag = pair;

                this.lsvEntityProperties.Items.Add(item);
            }

            // make sure the columns fit the contents and headers
            this.lsvEntityProperties.AutoResizeColumns(ColumnHeaderAutoResizeStyle.ColumnContent);
            this.lsvEntityProperties.AutoResizeColumns(ColumnHeaderAutoResizeStyle.HeaderSize);
        }

        private void lsvEntities_SelectedIndexChanged(object sender, EventArgs e)
        {
            this.grpEntity.Enabled = false;
            Application.DoEvents();

            try
            {
                if (this.lsvEntities.SelectedItems.Count > 0)
                {
                    this.CurrentEntity = this.lsvEntities.SelectedItems[0].Tag as CloudEntity;
                }
                else
                {
                    this.CurrentEntity = new CloudEntity();
                }

                this.RefreshEntity();
            }
            catch (Exception ex)
            {
                MessageBox.Show(this, $"Error Showing Entity.\r\n\r\nError: {ex.Message}", "Entity Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

            this.grpEntity.Enabled = true;
        }

        private void btnEntityNew_Click_1(object sender, EventArgs e)
        {
            this.grpEntity.Enabled = false;
            Application.DoEvents();

            try
            {
                this.CurrentEntity = new CloudEntity();

                this.RefreshEntity();
            }
            catch (Exception ex)
            {
                MessageBox.Show(this, $"Error Showing Entity.\r\n\r\nError: {ex.Message}", "Entity Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

            this.grpEntity.Enabled = true;            
        }

        private void mnuEntityPopup_Opening(object sender, System.ComponentModel.CancelEventArgs e)
        {
            if (this.lsvEntityProperties.SelectedItems.Count > 0)
            {
                this.mnuEntityPopupRemove.Enabled = true;
            }
            else
            {
                this.mnuEntityPopupRemove.Enabled = false;
            }
        }

        private void mnuEntityPopupAdd_Click(object sender, EventArgs e)
        {
            var frmPopup = new frmEntityPropertyAdd(this.CurrentEntity);
            
            var result = frmPopup.ShowDialog(this);
            if(result != DialogResult.OK) { return; }

            this.RefreshEntity();
        }

        private void mnuEntityPopupRemove_Click(object sender, EventArgs e)
        {
            // nothing to do?
            if(this.lsvEntityProperties.SelectedItems.Count == 0) { return; }

            var result = MessageBox.Show(this, $"Are you sure you want to remove the {this.lsvEntityProperties.SelectedItems.Count} selected item(s)?", "Remove Items", MessageBoxButtons.OKCancel, MessageBoxIcon.Question);
            if(result != DialogResult.OK) { return; }

            foreach(var selectedItem in this.lsvEntityProperties.SelectedItems)
            {
                var item = (ListViewItem)selectedItem;

                this.CurrentEntity.Remove(item.Text);
            }

            this.RefreshEntity();
        }

        private void txtEntityId_Leave(object sender, EventArgs e)
        {
            if (this.txtEntityId.ReadOnly) { return; }

            this.CurrentEntity.Id = this.txtEntityId.Text.Trim();
        }

        private void txtInstanceId_Leave(object sender, EventArgs e)
        {
            if (this.txtInstanceId.ReadOnly) { return; }

            this.CurrentEntity.InstanceId = this.txtInstanceId.Text.Trim();
        }
    }
}