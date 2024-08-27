using Microsoft.Extensions.Logging;

//using Gc = ThingsLibrary.Entity.GCP;
using Az = ThingsLibrary.Entity.AzureTable;
//using Le = ThingsLibrary.Entity.Local;

using Azure.Data.Tables;
using ThingsLibrary.Entity.Interfaces;
using ThingsLibrary.Schema.Library;

namespace EntityTester
{
    public partial class frmMain : Form
    {
        private ILogger<frmMain> _logger;

        private AppSettings AppSettings { get; set; }

        private IEntityStore<ItemDto> CurrentStore { get; set; }
        private ITableEntity CurrentEntity { get; set; }

        public frmMain(AppSettings appSettings)
        {
            this.AppSettings = appSettings;

            InitializeComponent();            
        }

        private void frmMain_Load(object sender, EventArgs e)
        {
            this.cboEntityStore.DataSource = this.AppSettings.EntityStoreOptions;
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
                this.CurrentStore = this.GetEntityStore(this.cboEntityStore.SelectedItem as EntityStoreOptions);                                
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

        private IEntityStore GetEntityStore(EntityStoreOptions storeSettings)
        {
            if (storeSettings.Type == "Azure_Table")
            {
                return new Az.EntityStore(storeSettings.ConnectionString, storeSettings.TableName);
            }
            //else if (storeSettings.Type == EntityStoreType.GCP_DataStore)
            //{
            //    return new Gc.EntityStore(storeSettings.ConnectionString, storeSettings.TableName);
            //}
            //else if (storeSettings.Type == "Local")
            //{
            //    return new Le.EntityStore(storeSettings.ConnectionString, storeSettings.TableName);
            //}
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
                this.txtPartitionKey.Text = "";
                this.txtPartitionKey.ReadOnly = false;

                this.txtRowKey.Text = "";
                this.txtRowKey.ReadOnly = false;

                this.btnEntityDelete.Enabled = false;
            }
            else
            {
                this.txtPartitionKey.Text = this.CurrentEntity.InstanceId;
                this.txtPartitionKey.ReadOnly = true;

                this.txtRowKey.Text = this.CurrentEntity.Id;
                this.txtRowKey.ReadOnly = true;

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
            if (this.txtRowKey.ReadOnly) { return; }

            this.CurrentEntity.Id = this.txtRowKey.Text.Trim();
        }

        private void txtInstanceId_Leave(object sender, EventArgs e)
        {
            if (this.txtPartitionKey.ReadOnly) { return; }

            this.CurrentEntity.InstanceId = this.txtPartitionKey.Text.Trim();
        }
    }
}