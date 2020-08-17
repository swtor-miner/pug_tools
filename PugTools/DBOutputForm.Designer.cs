namespace tor_tools
{
    partial class DBOutputForm
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
            this.btnOK = new System.Windows.Forms.Button();
            this.btnCancel = new System.Windows.Forms.Button();
            this.groupFileTypes = new System.Windows.Forms.GroupBox();
            this.versionTexBox = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.cbxExtractType = new System.Windows.Forms.ComboBox();
            this.label2 = new System.Windows.Forms.Label();
            this.groupFileTypes.SuspendLayout();
            this.SuspendLayout();
            // 
            // btnOK
            // 
            this.btnOK.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.btnOK.Enabled = false;
            this.btnOK.Location = new System.Drawing.Point(4, 64);
            this.btnOK.Name = "btnOK";
            this.btnOK.Size = new System.Drawing.Size(75, 23);
            this.btnOK.TabIndex = 0;
            this.btnOK.Text = "Ok";
            this.btnOK.UseVisualStyleBackColor = true;
            // 
            // btnCancel
            // 
            this.btnCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.btnCancel.Location = new System.Drawing.Point(85, 64);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(75, 23);
            this.btnCancel.TabIndex = 1;
            this.btnCancel.Text = "Cancel";
            this.btnCancel.UseVisualStyleBackColor = true;
            // 
            // groupFileTypes
            // 
            this.groupFileTypes.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
            | System.Windows.Forms.AnchorStyles.Left)
            | System.Windows.Forms.AnchorStyles.Right)));
            this.groupFileTypes.Controls.Add(this.label2);
            this.groupFileTypes.Controls.Add(this.cbxExtractType);
            this.groupFileTypes.Controls.Add(this.versionTexBox);
            this.groupFileTypes.Controls.Add(this.label1);
            this.groupFileTypes.Controls.Add(this.btnCancel);
            this.groupFileTypes.Controls.Add(this.btnOK);
            this.groupFileTypes.Location = new System.Drawing.Point(12, 12);
            this.groupFileTypes.Name = "groupFileTypes";
            this.groupFileTypes.Size = new System.Drawing.Size(166, 93);
            this.groupFileTypes.TabIndex = 2;
            this.groupFileTypes.TabStop = false;
            // 
            // versionTexBox
            // 
            this.versionTexBox.Location = new System.Drawing.Point(56, 13);
            this.versionTexBox.Name = "versionTexBox";
            this.versionTexBox.Size = new System.Drawing.Size(62, 20);
            this.versionTexBox.TabIndex = 14;
            this.versionTexBox.TextChanged += new System.EventHandler(this.VersionTexBox_TextChanged);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(6, 16);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(45, 13);
            this.label1.TabIndex = 15;
            this.label1.Text = "Version.";
            // 
            // cbxExtractType
            // 
            this.cbxExtractType.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cbxExtractType.FormattingEnabled = true;
            this.cbxExtractType.Items.AddRange(new object[] {
            "Full",
            "Partial"});
            this.cbxExtractType.Location = new System.Drawing.Point(56, 37);
            this.cbxExtractType.Name = "cbxExtractType";
            this.cbxExtractType.Size = new System.Drawing.Size(86, 21);
            this.cbxExtractType.TabIndex = 16;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(6, 40);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(31, 13);
            this.label2.TabIndex = 17;
            this.label2.Text = "Type";
            // 
            // DBOutputForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(190, 117);
            this.Controls.Add(this.groupFileTypes);
            this.Name = "DBOutputForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "DB Output";
            this.groupFileTypes.ResumeLayout(false);
            this.groupFileTypes.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Button btnOK;
        private System.Windows.Forms.Button btnCancel;
        private System.Windows.Forms.GroupBox groupFileTypes;
        private System.Windows.Forms.TextBox versionTexBox;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.ComboBox cbxExtractType;
    }
}