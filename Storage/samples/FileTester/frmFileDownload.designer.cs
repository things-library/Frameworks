namespace CloudFileTester
{
    partial class frmFileDownload
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(frmFileDownload));
            this.btnCancel = new System.Windows.Forms.Button();
            this.lblPercent = new System.Windows.Forms.Label();
            this.lblEstTimeLeft = new System.Windows.Forms.Label();
            this.panel1 = new System.Windows.Forms.Panel();
            this.lblSkipped = new System.Windows.Forms.Label();
            this.progressQueue = new System.Windows.Forms.ProgressBar();
            this.lblCurrentStatus = new System.Windows.Forms.Label();
            this.panel1.SuspendLayout();
            this.SuspendLayout();
            // 
            // btnCancel
            // 
            resources.ApplyResources(this.btnCancel, "btnCancel");
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Click += new System.EventHandler(this.btnCancel_Click);
            // 
            // lblPercent
            // 
            resources.ApplyResources(this.lblPercent, "lblPercent");
            this.lblPercent.Name = "lblPercent";
            // 
            // lblEstTimeLeft
            // 
            resources.ApplyResources(this.lblEstTimeLeft, "lblEstTimeLeft");
            this.lblEstTimeLeft.ForeColor = System.Drawing.Color.Gray;
            this.lblEstTimeLeft.Name = "lblEstTimeLeft";
            // 
            // panel1
            // 
            resources.ApplyResources(this.panel1, "panel1");
            this.panel1.BackColor = System.Drawing.Color.White;
            this.panel1.Controls.Add(this.lblSkipped);
            this.panel1.Controls.Add(this.lblEstTimeLeft);
            this.panel1.Controls.Add(this.progressQueue);
            this.panel1.Controls.Add(this.lblCurrentStatus);
            this.panel1.Controls.Add(this.lblPercent);
            this.panel1.Name = "panel1";
            // 
            // lblSkipped
            // 
            resources.ApplyResources(this.lblSkipped, "lblSkipped");
            this.lblSkipped.Name = "lblSkipped";
            // 
            // progressQueue
            // 
            resources.ApplyResources(this.progressQueue, "progressQueue");
            this.progressQueue.MarqueeAnimationSpeed = 0;
            this.progressQueue.Maximum = 10000;
            this.progressQueue.Name = "progressQueue";
            // 
            // lblCurrentStatus
            // 
            resources.ApplyResources(this.lblCurrentStatus, "lblCurrentStatus");
            this.lblCurrentStatus.Name = "lblCurrentStatus";
            // 
            // frmFileDownload
            // 
            resources.ApplyResources(this, "$this");
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.btnCancel);
            this.Controls.Add(this.panel1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "frmFileDownload";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.frmFileDownload_FormClosing);
            this.Shown += new System.EventHandler(this.frmFileDownload_Shown);
            this.panel1.ResumeLayout(false);
            this.panel1.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion
        private System.Windows.Forms.Button btnCancel;
        private System.Windows.Forms.Label lblPercent;
        private System.Windows.Forms.Label lblEstTimeLeft;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.ProgressBar progressQueue;
        private System.Windows.Forms.Label lblCurrentStatus;
        private System.Windows.Forms.Label lblSkipped;
    }
}