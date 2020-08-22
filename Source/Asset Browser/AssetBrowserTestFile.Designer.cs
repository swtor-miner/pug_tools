namespace tor_tools
{
    partial class AssetBrowserTestFile
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
            this.chkAMX = new System.Windows.Forms.CheckBox();
            this.chkFXSPEC = new System.Windows.Forms.CheckBox();
            this.chkALL = new System.Windows.Forms.CheckBox();
            this.chkEPP = new System.Windows.Forms.CheckBox();
            this.chkXML = new System.Windows.Forms.CheckBox();
            this.chkPRT = new System.Windows.Forms.CheckBox();
            this.chkMAT = new System.Windows.Forms.CheckBox();
            this.chkGR2 = new System.Windows.Forms.CheckBox();
            this.chkDAT = new System.Windows.Forms.CheckBox();
            this.chkBNK = new System.Windows.Forms.CheckBox();
            this.chkDYN = new System.Windows.Forms.CheckBox();
            this.chkHYD = new System.Windows.Forms.CheckBox();
            this.chkSDEF = new System.Windows.Forms.CheckBox();
            this.chkMISC_WORLD = new System.Windows.Forms.CheckBox();
            this.chkMISC = new System.Windows.Forms.CheckBox();
            this.chkCNV = new System.Windows.Forms.CheckBox();
            this.grbNodes = new System.Windows.Forms.GroupBox();
            this.chkSTB = new System.Windows.Forms.CheckBox();
            this.chkPLC = new System.Windows.Forms.CheckBox();
            this.chkICONS = new System.Windows.Forms.CheckBox();
            this.lblWarning = new System.Windows.Forms.Label();
            this.groupFileTypes.SuspendLayout();
            this.grbNodes.SuspendLayout();
            this.SuspendLayout();
            // 
            // btnOK
            // 
            this.btnOK.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.btnOK.Location = new System.Drawing.Point(170, 271);
            this.btnOK.Name = "btnOK";
            this.btnOK.Size = new System.Drawing.Size(75, 23);
            this.btnOK.TabIndex = 0;
            this.btnOK.Text = "Ok";
            this.btnOK.UseVisualStyleBackColor = true;
            // 
            // btnCancel
            // 
            this.btnCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.btnCancel.Location = new System.Drawing.Point(252, 271);
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
            this.groupFileTypes.Controls.Add(this.chkAMX);
            this.groupFileTypes.Controls.Add(this.chkFXSPEC);
            this.groupFileTypes.Controls.Add(this.chkALL);
            this.groupFileTypes.Controls.Add(this.chkEPP);
            this.groupFileTypes.Controls.Add(this.chkXML);
            this.groupFileTypes.Controls.Add(this.chkPRT);
            this.groupFileTypes.Controls.Add(this.chkMAT);
            this.groupFileTypes.Controls.Add(this.chkGR2);
            this.groupFileTypes.Controls.Add(this.chkDAT);
            this.groupFileTypes.Controls.Add(this.chkBNK);
            this.groupFileTypes.Location = new System.Drawing.Point(12, 12);
            this.groupFileTypes.Name = "groupFileTypes";
            this.groupFileTypes.Size = new System.Drawing.Size(233, 254);
            this.groupFileTypes.TabIndex = 2;
            this.groupFileTypes.TabStop = false;
            this.groupFileTypes.Text = "File Types";
            // 
            // chkAMX
            // 
            this.chkAMX.AutoSize = true;
            this.chkAMX.Checked = true;
            this.chkAMX.CheckState = System.Windows.Forms.CheckState.Checked;
            this.chkAMX.Location = new System.Drawing.Point(6, 43);
            this.chkAMX.Name = "chkAMX";
            this.chkAMX.Size = new System.Drawing.Size(109, 17);
            this.chkAMX.TabIndex = 12;
            this.chkAMX.Text = "AMX - Animations";
            this.chkAMX.UseVisualStyleBackColor = true;
            // 
            // chkFXSPEC
            // 
            this.chkFXSPEC.AutoSize = true;
            this.chkFXSPEC.Checked = true;
            this.chkFXSPEC.CheckState = System.Windows.Forms.CheckState.Checked;
            this.chkFXSPEC.Location = new System.Drawing.Point(6, 136);
            this.chkFXSPEC.Name = "chkFXSPEC";
            this.chkFXSPEC.Size = new System.Drawing.Size(117, 17);
            this.chkFXSPEC.TabIndex = 11;
            this.chkFXSPEC.Text = "FXSPEC - FX Spec";
            this.chkFXSPEC.UseVisualStyleBackColor = true;
            // 
            // chkALL
            // 
            this.chkALL.AutoSize = true;
            this.chkALL.Checked = true;
            this.chkALL.CheckState = System.Windows.Forms.CheckState.Checked;
            this.chkALL.Location = new System.Drawing.Point(6, 19);
            this.chkALL.Name = "chkALL";
            this.chkALL.Size = new System.Drawing.Size(90, 17);
            this.chkALL.TabIndex = 7;
            this.chkALL.Text = "Un/Check All";
            this.chkALL.UseVisualStyleBackColor = true;
            this.chkALL.CheckedChanged += new System.EventHandler(this.ChkALL_CheckedChanged);
            // 
            // chkEPP
            // 
            this.chkEPP.AutoSize = true;
            this.chkEPP.Checked = true;
            this.chkEPP.CheckState = System.Windows.Forms.CheckState.Checked;
            this.chkEPP.Location = new System.Drawing.Point(6, 112);
            this.chkEPP.Name = "chkEPP";
            this.chkEPP.Size = new System.Drawing.Size(89, 17);
            this.chkEPP.TabIndex = 6;
            this.chkEPP.Text = "EPP - Effects";
            this.chkEPP.UseVisualStyleBackColor = true;
            // 
            // chkXML
            // 
            this.chkXML.AutoSize = true;
            this.chkXML.Checked = true;
            this.chkXML.CheckState = System.Windows.Forms.CheckState.Checked;
            this.chkXML.Location = new System.Drawing.Point(6, 228);
            this.chkXML.Name = "chkXML";
            this.chkXML.Size = new System.Drawing.Size(103, 17);
            this.chkXML.TabIndex = 5;
            this.chkXML.Text = "XML - XML Files";
            this.chkXML.UseVisualStyleBackColor = true;
            // 
            // chkPRT
            // 
            this.chkPRT.AutoSize = true;
            this.chkPRT.Checked = true;
            this.chkPRT.CheckState = System.Windows.Forms.CheckState.Checked;
            this.chkPRT.Location = new System.Drawing.Point(6, 205);
            this.chkPRT.Name = "chkPRT";
            this.chkPRT.Size = new System.Drawing.Size(125, 17);
            this.chkPRT.TabIndex = 4;
            this.chkPRT.Text = "PRT - Particle Specs";
            this.chkPRT.UseVisualStyleBackColor = true;
            // 
            // chkMAT
            // 
            this.chkMAT.AutoSize = true;
            this.chkMAT.Checked = true;
            this.chkMAT.CheckState = System.Windows.Forms.CheckState.Checked;
            this.chkMAT.Location = new System.Drawing.Point(6, 182);
            this.chkMAT.Name = "chkMAT";
            this.chkMAT.Size = new System.Drawing.Size(100, 17);
            this.chkMAT.TabIndex = 3;
            this.chkMAT.Text = "MAT - Materials";
            this.chkMAT.UseVisualStyleBackColor = true;
            // 
            // chkGR2
            // 
            this.chkGR2.AutoSize = true;
            this.chkGR2.Checked = true;
            this.chkGR2.CheckState = System.Windows.Forms.CheckState.Checked;
            this.chkGR2.Location = new System.Drawing.Point(6, 159);
            this.chkGR2.Name = "chkGR2";
            this.chkGR2.Size = new System.Drawing.Size(110, 17);
            this.chkGR2.TabIndex = 2;
            this.chkGR2.Text = "GR2 - Model Files";
            this.chkGR2.UseVisualStyleBackColor = true;
            // 
            // chkDAT
            // 
            this.chkDAT.AutoSize = true;
            this.chkDAT.Enabled = true;
            this.chkDAT.CheckState = System.Windows.Forms.CheckState.Checked;
            this.chkDAT.Location = new System.Drawing.Point(6, 89);
            this.chkDAT.Name = "chkDAT";
            this.chkDAT.Size = new System.Drawing.Size(206, 17);
            this.chkDAT.TabIndex = 1;
            this.chkDAT.Text = "DAT - Area, Room, Character Specs**";
            this.chkDAT.UseVisualStyleBackColor = true;
            // 
            // chkBNK
            // 
            this.chkBNK.AutoSize = true;
            this.chkBNK.Checked = true;
            this.chkBNK.CheckState = System.Windows.Forms.CheckState.Checked;
            this.chkBNK.Location = new System.Drawing.Point(6, 66);
            this.chkBNK.Name = "chkBNK";
            this.chkBNK.Size = new System.Drawing.Size(117, 17);
            this.chkBNK.TabIndex = 0;
            this.chkBNK.Text = "BNK - Audio Banks";
            this.chkBNK.UseVisualStyleBackColor = true;
            // 
            // chkDYN
            // 
            this.chkDYN.AutoSize = true;
            this.chkDYN.Checked = true;
            this.chkDYN.CheckState = System.Windows.Forms.CheckState.Checked;
            this.chkDYN.Location = new System.Drawing.Point(6, 42);
            this.chkDYN.Name = "chkDYN";
            this.chkDYN.Size = new System.Drawing.Size(111, 17);
            this.chkDYN.TabIndex = 15;
            this.chkDYN.Text = "DYN - Dyn Nodes";
            this.chkDYN.UseVisualStyleBackColor = true;
            // 
            // chkHYD
            // 
            this.chkHYD.AutoSize = true;
            this.chkHYD.Checked = true;
            this.chkHYD.CheckState = System.Windows.Forms.CheckState.Checked;
            this.chkHYD.Location = new System.Drawing.Point(6, 66);
            this.chkHYD.Name = "chkHYD";
            this.chkHYD.Size = new System.Drawing.Size(120, 17);
            this.chkHYD.TabIndex = 14;
            this.chkHYD.Text = "HYD - Hydra Nodes";
            this.chkHYD.UseVisualStyleBackColor = true;
            // 
            // chkSDEF
            // 
            this.chkSDEF.AutoSize = true;
            this.chkSDEF.Checked = true;
            this.chkSDEF.CheckState = System.Windows.Forms.CheckState.Checked;
            this.chkSDEF.Location = new System.Drawing.Point(6, 182);
            this.chkSDEF.Name = "chkSDEF";
            this.chkSDEF.Size = new System.Drawing.Size(123, 17);
            this.chkSDEF.TabIndex = 13;
            this.chkSDEF.Text = "SDEF - Script ID List";
            this.chkSDEF.UseVisualStyleBackColor = true;
            // 
            // chkMISC_WORLD
            // 
            this.chkMISC_WORLD.AutoSize = true;
            this.chkMISC_WORLD.Checked = true;
            this.chkMISC_WORLD.CheckState = System.Windows.Forms.CheckState.Checked;
            this.chkMISC_WORLD.Location = new System.Drawing.Point(6, 135);
            this.chkMISC_WORLD.Name = "chkMISC_WORLD";
            this.chkMISC_WORLD.Size = new System.Drawing.Size(205, 17);
            this.chkMISC_WORLD.TabIndex = 10;
            this.chkMISC_WORLD.Text = "MISC_WORLD - World Area Names**";
            this.chkMISC_WORLD.UseVisualStyleBackColor = true;
            // 
            // chkMISC
            // 
            this.chkMISC.AutoSize = true;
            this.chkMISC.Checked = true;
            this.chkMISC.CheckState = System.Windows.Forms.CheckState.Checked;
            this.chkMISC.Location = new System.Drawing.Point(6, 112);
            this.chkMISC.Name = "chkMISC";
            this.chkMISC.Size = new System.Drawing.Size(123, 17);
            this.chkMISC.TabIndex = 9;
            this.chkMISC.Text = "MISC - Misc Formats";
            this.chkMISC.UseVisualStyleBackColor = true;
            // 
            // chkCNV
            // 
            this.chkCNV.AutoSize = true;
            this.chkCNV.Checked = true;
            this.chkCNV.CheckState = System.Windows.Forms.CheckState.Checked;
            this.chkCNV.Location = new System.Drawing.Point(6, 19);
            this.chkCNV.Name = "chkCNV";
            this.chkCNV.Size = new System.Drawing.Size(113, 17);
            this.chkCNV.TabIndex = 8;
            this.chkCNV.Text = "CNV - CNV Nodes";
            this.chkCNV.UseVisualStyleBackColor = true;
            // 
            // grbNodes
            // 
            this.grbNodes.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
            | System.Windows.Forms.AnchorStyles.Left)
            | System.Windows.Forms.AnchorStyles.Right)));
            this.grbNodes.Controls.Add(this.chkSTB);
            this.grbNodes.Controls.Add(this.chkPLC);
            this.grbNodes.Controls.Add(this.chkICONS);
            this.grbNodes.Controls.Add(this.chkSDEF);
            this.grbNodes.Controls.Add(this.chkDYN);
            this.grbNodes.Controls.Add(this.chkCNV);
            this.grbNodes.Controls.Add(this.chkHYD);
            this.grbNodes.Controls.Add(this.chkMISC_WORLD);
            this.grbNodes.Controls.Add(this.chkMISC);
            this.grbNodes.Location = new System.Drawing.Point(251, 12);
            this.grbNodes.Name = "grbNodes";
            this.grbNodes.Size = new System.Drawing.Size(225, 254);
            this.grbNodes.TabIndex = 3;
            this.grbNodes.TabStop = false;
            this.grbNodes.Text = "Nodes";
            // 
            // chkSTB
            // 
            this.chkSTB.AutoSize = true;
            this.chkSTB.Checked = true;
            this.chkSTB.CheckState = System.Windows.Forms.CheckState.Checked;
            this.chkSTB.Location = new System.Drawing.Point(6, 205);
            this.chkSTB.Name = "chkSTB";
            this.chkSTB.Size = new System.Drawing.Size(120, 17);
            this.chkSTB.TabIndex = 18;
            this.chkSTB.Text = "STB - STB Manifest";
            this.chkSTB.UseVisualStyleBackColor = true;
            // 
            // chkPLC
            // 
            this.chkPLC.AutoSize = true;
            this.chkPLC.Checked = true;
            this.chkPLC.CheckState = System.Windows.Forms.CheckState.Checked;
            this.chkPLC.Location = new System.Drawing.Point(6, 158);
            this.chkPLC.Name = "chkPLC";
            this.chkPLC.Size = new System.Drawing.Size(107, 17);
            this.chkPLC.TabIndex = 17;
            this.chkPLC.Text = "PLC - Placeables";
            this.chkPLC.UseVisualStyleBackColor = true;
            // 
            // chkICONS
            // 
            this.chkICONS.AutoSize = true;
            this.chkICONS.Checked = true;
            this.chkICONS.CheckState = System.Windows.Forms.CheckState.Checked;
            this.chkICONS.Location = new System.Drawing.Point(6, 89);
            this.chkICONS.Name = "chkICONS";
            this.chkICONS.Size = new System.Drawing.Size(94, 17);
            this.chkICONS.TabIndex = 16;
            this.chkICONS.Text = "ICONS - Icons";
            this.chkICONS.UseVisualStyleBackColor = true;
            // 
            // lblWarning
            // 
            this.lblWarning.AutoSize = true;
            this.lblWarning.Location = new System.Drawing.Point(374, 276);
            this.lblWarning.Name = "lblWarning";
            this.lblWarning.Size = new System.Drawing.Size(102, 13);
            this.lblWarning.TabIndex = 4;
            this.lblWarning.Text = "** - Time Consuming";
            // 
            // AssetBrowserTestFile
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(488, 306);
            this.Controls.Add(this.lblWarning);
            this.Controls.Add(this.grbNodes);
            this.Controls.Add(this.groupFileTypes);
            this.Controls.Add(this.btnOK);
            this.Controls.Add(this.btnCancel);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow;
            this.Name = "AssetBrowserTestFile";
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Asset Browser Test Files";
            this.TopMost = true;
            this.groupFileTypes.ResumeLayout(false);
            this.groupFileTypes.PerformLayout();
            this.grbNodes.ResumeLayout(false);
            this.grbNodes.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button btnOK;
        private System.Windows.Forms.Button btnCancel;
        private System.Windows.Forms.GroupBox groupFileTypes;
        private System.Windows.Forms.CheckBox chkMAT;
        private System.Windows.Forms.CheckBox chkGR2;
        private System.Windows.Forms.CheckBox chkDAT;
        private System.Windows.Forms.CheckBox chkBNK;
        private System.Windows.Forms.CheckBox chkXML;
        private System.Windows.Forms.CheckBox chkPRT;
        private System.Windows.Forms.CheckBox chkEPP;
        private System.Windows.Forms.CheckBox chkALL;
        private System.Windows.Forms.CheckBox chkCNV;
        private System.Windows.Forms.CheckBox chkMISC;
        private System.Windows.Forms.CheckBox chkMISC_WORLD;
        private System.Windows.Forms.CheckBox chkFXSPEC;
        private System.Windows.Forms.CheckBox chkAMX;
        private System.Windows.Forms.CheckBox chkSDEF;
        private System.Windows.Forms.CheckBox chkHYD;
        private System.Windows.Forms.CheckBox chkDYN;
        private System.Windows.Forms.GroupBox grbNodes;
        private System.Windows.Forms.Label lblWarning;
        private System.Windows.Forms.CheckBox chkICONS;
        private System.Windows.Forms.CheckBox chkPLC;
        private System.Windows.Forms.CheckBox chkSTB;
    }
}