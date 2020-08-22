namespace tor_tools
{
    partial class AssetBrowserHashStatus
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
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle1 = new System.Windows.Forms.DataGridViewCellStyle();
            this.dgvHashStatus = new System.Windows.Forms.DataGridView();
            this.lblTotalFiles = new System.Windows.Forms.Label();
            this.lblTotalNamed = new System.Windows.Forms.Label();
            this.lblCompletion = new System.Windows.Forms.Label();
            this.lblTotalMissing = new System.Windows.Forms.Label();
            this.lblTotalFilesVal = new System.Windows.Forms.Label();
            this.lblTotalNamedVal = new System.Windows.Forms.Label();
            this.lblTotalMissingVal = new System.Windows.Forms.Label();
            this.lblCompletionVal = new System.Windows.Forms.Label();
            this.backgroundWorker1 = new System.ComponentModel.BackgroundWorker();
            this.statusStrip1 = new System.Windows.Forms.StatusStrip();
            this.toolStripStatusLabel1 = new System.Windows.Forms.ToolStripStatusLabel();
            this.toolStripProgressBar1 = new System.Windows.Forms.ToolStripProgressBar();
            this.pictureBox1 = new System.Windows.Forms.PictureBox();
            ((System.ComponentModel.ISupportInitialize)(this.dgvHashStatus)).BeginInit();
            this.statusStrip1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).BeginInit();
            this.SuspendLayout();
            // 
            // dgvHashStatus
            // 
            this.dgvHashStatus.AllowUserToAddRows = false;
            this.dgvHashStatus.AllowUserToDeleteRows = false;
            this.dgvHashStatus.AllowUserToOrderColumns = true;
            this.dgvHashStatus.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
            | System.Windows.Forms.AnchorStyles.Left)
            | System.Windows.Forms.AnchorStyles.Right)));
            this.dgvHashStatus.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgvHashStatus.Enabled = false;
            this.dgvHashStatus.Location = new System.Drawing.Point(12, 34);
            this.dgvHashStatus.Name = "dgvHashStatus";
            this.dgvHashStatus.ReadOnly = true;
            dataGridViewCellStyle1.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle1.BackColor = System.Drawing.SystemColors.Control;
            dataGridViewCellStyle1.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            dataGridViewCellStyle1.ForeColor = System.Drawing.SystemColors.WindowText;
            dataGridViewCellStyle1.SelectionBackColor = System.Drawing.SystemColors.Highlight;
            dataGridViewCellStyle1.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
            dataGridViewCellStyle1.WrapMode = System.Windows.Forms.DataGridViewTriState.True;
            this.dgvHashStatus.RowHeadersDefaultCellStyle = dataGridViewCellStyle1;
            this.dgvHashStatus.ShowCellErrors = false;
            this.dgvHashStatus.ShowCellToolTips = false;
            this.dgvHashStatus.ShowEditingIcon = false;
            this.dgvHashStatus.ShowRowErrors = false;
            this.dgvHashStatus.Size = new System.Drawing.Size(713, 563);
            this.dgvHashStatus.TabIndex = 0;
            // 
            // lblTotalFiles
            // 
            this.lblTotalFiles.AutoSize = true;
            this.lblTotalFiles.Font = new System.Drawing.Font("Microsoft Sans Serif", 11.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblTotalFiles.Location = new System.Drawing.Point(14, 9);
            this.lblTotalFiles.Name = "lblTotalFiles";
            this.lblTotalFiles.Size = new System.Drawing.Size(92, 18);
            this.lblTotalFiles.TabIndex = 1;
            this.lblTotalFiles.Text = "Total Files:";
            // 
            // lblTotalNamed
            // 
            this.lblTotalNamed.AutoSize = true;
            this.lblTotalNamed.Font = new System.Drawing.Font("Microsoft Sans Serif", 11.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblTotalNamed.Location = new System.Drawing.Point(187, 9);
            this.lblTotalNamed.Name = "lblTotalNamed";
            this.lblTotalNamed.Size = new System.Drawing.Size(109, 18);
            this.lblTotalNamed.TabIndex = 2;
            this.lblTotalNamed.Text = "Total Named:";
            // 
            // lblCompletion
            // 
            this.lblCompletion.AutoSize = true;
            this.lblCompletion.Font = new System.Drawing.Font("Microsoft Sans Serif", 11.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblCompletion.Location = new System.Drawing.Point(559, 9);
            this.lblCompletion.Name = "lblCompletion";
            this.lblCompletion.Size = new System.Drawing.Size(118, 18);
            this.lblCompletion.TabIndex = 3;
            this.lblCompletion.Text = "Completion %:";
            // 
            // lblTotalMissing
            // 
            this.lblTotalMissing.AutoSize = true;
            this.lblTotalMissing.Font = new System.Drawing.Font("Microsoft Sans Serif", 11.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblTotalMissing.Location = new System.Drawing.Point(373, 9);
            this.lblTotalMissing.Name = "lblTotalMissing";
            this.lblTotalMissing.Size = new System.Drawing.Size(114, 18);
            this.lblTotalMissing.TabIndex = 4;
            this.lblTotalMissing.Text = "Total Missing:";
            // 
            // lblTotalFilesVal
            // 
            this.lblTotalFilesVal.AutoSize = true;
            this.lblTotalFilesVal.Font = new System.Drawing.Font("Microsoft Sans Serif", 11.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblTotalFilesVal.Location = new System.Drawing.Point(112, 9);
            this.lblTotalFilesVal.Name = "lblTotalFilesVal";
            this.lblTotalFilesVal.Size = new System.Drawing.Size(16, 18);
            this.lblTotalFilesVal.TabIndex = 5;
            this.lblTotalFilesVal.Text = "0";
            this.lblTotalFilesVal.Visible = false;
            // 
            // lblTotalNamedVal
            // 
            this.lblTotalNamedVal.AutoSize = true;
            this.lblTotalNamedVal.Font = new System.Drawing.Font("Microsoft Sans Serif", 11.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblTotalNamedVal.Location = new System.Drawing.Point(302, 9);
            this.lblTotalNamedVal.Name = "lblTotalNamedVal";
            this.lblTotalNamedVal.Size = new System.Drawing.Size(16, 18);
            this.lblTotalNamedVal.TabIndex = 6;
            this.lblTotalNamedVal.Text = "0";
            this.lblTotalNamedVal.Visible = false;
            // 
            // lblTotalMissingVal
            // 
            this.lblTotalMissingVal.AutoSize = true;
            this.lblTotalMissingVal.Font = new System.Drawing.Font("Microsoft Sans Serif", 11.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblTotalMissingVal.Location = new System.Drawing.Point(493, 9);
            this.lblTotalMissingVal.Name = "lblTotalMissingVal";
            this.lblTotalMissingVal.Size = new System.Drawing.Size(16, 18);
            this.lblTotalMissingVal.TabIndex = 7;
            this.lblTotalMissingVal.Text = "0";
            this.lblTotalMissingVal.Visible = false;
            // 
            // lblCompletionVal
            // 
            this.lblCompletionVal.AutoSize = true;
            this.lblCompletionVal.Font = new System.Drawing.Font("Microsoft Sans Serif", 11.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblCompletionVal.Location = new System.Drawing.Point(683, 9);
            this.lblCompletionVal.Name = "lblCompletionVal";
            this.lblCompletionVal.Size = new System.Drawing.Size(29, 18);
            this.lblCompletionVal.TabIndex = 8;
            this.lblCompletionVal.Text = "0%";
            this.lblCompletionVal.Visible = false;
            // 
            // backgroundWorker1
            // 
            this.backgroundWorker1.DoWork += new System.ComponentModel.DoWorkEventHandler(this.BackgroundWorker1_DoWork);
            this.backgroundWorker1.RunWorkerCompleted += new System.ComponentModel.RunWorkerCompletedEventHandler(this.BackgroundWorker1_RunWorkerCompleted);
            // 
            // statusStrip1
            // 
            this.statusStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStripStatusLabel1,
            this.toolStripProgressBar1});
            this.statusStrip1.Location = new System.Drawing.Point(0, 600);
            this.statusStrip1.Name = "statusStrip1";
            this.statusStrip1.Size = new System.Drawing.Size(737, 22);
            this.statusStrip1.TabIndex = 9;
            this.statusStrip1.Text = "statusStrip1";
            // 
            // toolStripStatusLabel1
            // 
            this.toolStripStatusLabel1.Name = "toolStripStatusLabel1";
            this.toolStripStatusLabel1.Size = new System.Drawing.Size(0, 17);
            // 
            // toolStripProgressBar1
            // 
            this.toolStripProgressBar1.Name = "toolStripProgressBar1";
            this.toolStripProgressBar1.Size = new System.Drawing.Size(100, 16);
            this.toolStripProgressBar1.Style = System.Windows.Forms.ProgressBarStyle.Marquee;
            this.toolStripProgressBar1.Visible = false;
            // 
            // pictureBox1
            // 
            this.pictureBox1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
            | System.Windows.Forms.AnchorStyles.Left)
            | System.Windows.Forms.AnchorStyles.Right)));
            this.pictureBox1.BackColor = System.Drawing.Color.White;
            this.pictureBox1.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.pictureBox1.Image = global::PugTools.Properties.Resources.LoadingImage;
            this.pictureBox1.Location = new System.Drawing.Point(12, 34);
            this.pictureBox1.Name = "pictureBox1";
            this.pictureBox1.Size = new System.Drawing.Size(713, 563);
            this.pictureBox1.SizeMode = System.Windows.Forms.PictureBoxSizeMode.CenterImage;
            this.pictureBox1.TabIndex = 10;
            this.pictureBox1.TabStop = false;
            // 
            // AssetBrowserHashStatus
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(737, 622);
            this.Controls.Add(this.pictureBox1);
            this.Controls.Add(this.statusStrip1);
            this.Controls.Add(this.lblCompletionVal);
            this.Controls.Add(this.lblTotalMissingVal);
            this.Controls.Add(this.lblTotalNamedVal);
            this.Controls.Add(this.lblTotalFilesVal);
            this.Controls.Add(this.lblTotalMissing);
            this.Controls.Add(this.lblCompletion);
            this.Controls.Add(this.lblTotalNamed);
            this.Controls.Add(this.lblTotalFiles);
            this.Controls.Add(this.dgvHashStatus);
            this.Name = "AssetBrowserHashStatus";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Hash List Status";
            this.Load += new System.EventHandler(this.AssetBrowserHashStatus_Load);
            ((System.ComponentModel.ISupportInitialize)(this.dgvHashStatus)).EndInit();
            this.statusStrip1.ResumeLayout(false);
            this.statusStrip1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.DataGridView dgvHashStatus;
        private System.Windows.Forms.Label lblTotalFiles;
        private System.Windows.Forms.Label lblTotalNamed;
        private System.Windows.Forms.Label lblCompletion;
        private System.Windows.Forms.Label lblTotalMissing;
        private System.Windows.Forms.Label lblTotalFilesVal;
        private System.Windows.Forms.Label lblTotalNamedVal;
        private System.Windows.Forms.Label lblTotalMissingVal;
        private System.Windows.Forms.Label lblCompletionVal;
        private System.ComponentModel.BackgroundWorker backgroundWorker1;
        private System.Windows.Forms.StatusStrip statusStrip1;
        private System.Windows.Forms.ToolStripStatusLabel toolStripStatusLabel1;
        private System.Windows.Forms.ToolStripProgressBar toolStripProgressBar1;
        private System.Windows.Forms.PictureBox pictureBox1;
    }
}