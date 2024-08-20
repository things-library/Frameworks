using Azure.Data.Tables;
using ThingsLibrary.Entity;

namespace CloudEntityTester
{
    public partial class frmEntityPropertyAdd : Form
    {
        private ITableEntity CloudEntity { get; set; }

        public frmEntityPropertyAdd(ITableEntity cloudEntity)
        {
            this.CloudEntity = cloudEntity;

            InitializeComponent();

            // select the first item (force a refresh)
            this.cboType.SelectedIndex = 0;
         }

        private void btnAdd_Click(object sender, EventArgs e)
        {
            try
            {
                var key = this.txtKey.Text.Trim();

                switch (this.cboType.SelectedItem)
                {
                    case "String": { this.CloudEntity[key] = this.txtValue.Text.Trim(); break; }
                    case "Date and Time": 
                        {
                            this.CloudEntity[key] = new DateTimeOffset(this.dateValue.Value);                            
                            break; 
                        }
                    case "Boolean": { this.CloudEntity[key] = this.chkValue.Checked; break; }
                    case "Text": { this.CloudEntity[key] = this.txtValue.Text; break; }

                    case "Integer": { this.CloudEntity[key] = (int)this.numValue.Value; break; }
                    case "Floating Point Number": { this.CloudEntity[key] = (decimal)this.numValue.Value; break; }

                    default:
                        {
                            throw new ArgumentException($"Unknown type '{this.cboType.SelectedText}'");
                        }
                }

                this.DialogResult = DialogResult.OK;
            }
            catch (Exception ex)
            {
                MessageBox.Show(this, $"Error Adding Property.\r\n\r\nError: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }            
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.Cancel;
        }

        private void cboType_SelectedIndexChanged(object sender, EventArgs e)
        {
            // set invisible and only make one of them visible based on selection
            this.txtValue.Visible = false;
            this.numValue.Visible = false;
            this.dateValue.Visible = false;
            this.chkValue.Visible = false;

            switch (this.cboType.SelectedItem)
            {
                case "String": { this.txtValue.Visible = true; break;}
                case "Date and Time": { this.dateValue.Visible = true; break;}
                case "Boolean": { this.chkValue.Visible = true; break; }
                case "Text": { this.txtValue.Visible = true; break; }

                case "Integer": 
                    {
                        this.numValue.Visible = true;
                        this.numValue.Value = 0;

                        this.numValue.DecimalPlaces = 0;
                        this.numValue.Minimum = int.MinValue;
                        this.numValue.Maximum = int.MaxValue;

                        break;
                    }
                case "Floating Point Number": 
                    {
                        this.numValue.Visible = true;
                        this.numValue.Value = 0;

                        this.numValue.DecimalPlaces = 7;    // does it really need to be more than a float precision?
                        this.numValue.Minimum = decimal.MinValue;
                        this.numValue.Maximum = decimal.MaxValue;

                        break;
                    }
                
                
                default:
                    {
                        throw new ArgumentException($"Unknown type '{this.cboType.SelectedText}'");                        
                    }
            }
        }
    }
}
