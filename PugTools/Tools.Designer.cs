namespace tor_tools
{
    partial class Tools
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
            this.components = new System.ComponentModel.Container();
            this.textBoxAssetsFolder = new System.Windows.Forms.TextBox();
            this.labelAssetsFolder = new System.Windows.Forms.Label();
            this.listBox1 = new System.Windows.Forms.ListBox();
            this.textBox2 = new System.Windows.Forms.TextBox();
            this.label2 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.textBox3 = new System.Windows.Forms.TextBox();
            this.label4 = new System.Windows.Forms.Label();
            this.label5 = new System.Windows.Forms.Label();
            this.textBox4 = new System.Windows.Forms.TextBox();
            this.listBox2 = new System.Windows.Forms.ListBox();
            this.buttonToggleDatabase = new System.Windows.Forms.Button();
            this.textBox5 = new System.Windows.Forms.TextBox();
            this.textBox6 = new System.Windows.Forms.TextBox();
            this.searchButton = new System.Windows.Forms.Button();
            this.gbxFormat = new System.Windows.Forms.GroupBox();
            this.versionTexBox = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.chkBuildCompare = new System.Windows.Forms.CheckBox();
            this.chkRemoveElements = new System.Windows.Forms.CheckBox();
            this.cbxExtractFormat = new System.Windows.Forms.ComboBox();
            this.chkVerbose = new System.Windows.Forms.CheckBox();
            this.gbxDB = new System.Windows.Forms.GroupBox();
            this.extractButton = new System.Windows.Forms.Button();
            this.gbxFQN = new System.Windows.Forms.GroupBox();
            this.label7 = new System.Windows.Forms.Label();
            this.textBoxExtractFolder = new System.Windows.Forms.TextBox();
            this.labelExtractFolder = new System.Windows.Forms.Label();
            this.usePTSAssets = new System.Windows.Forms.CheckBox();
            this.buttonFindAssets = new System.Windows.Forms.Button();
            this.buttonSelectExtractFolder = new System.Windows.Forms.Button();
            this.buttonFindPrevAssets = new System.Windows.Forms.Button();
            this.labelPrevAssetsFolder = new System.Windows.Forms.Label();
            this.textBoxPrevAssetsFolder = new System.Windows.Forms.TextBox();
            this.prevUsePTSAssets = new System.Windows.Forms.CheckBox();
            this.CrossLinkDomCheckBox = new System.Windows.Forms.CheckBox();
            this.progressBar1 = new System.Windows.Forms.ProgressBar();
            this.gbxPath = new System.Windows.Forms.GroupBox();
            this.btnUnloadAllData = new System.Windows.Forms.Button();
            this.gbxLogs = new System.Windows.Forms.GroupBox();
            this.gbxTools = new System.Windows.Forms.GroupBox();
            this.btnNodeBrowser = new System.Windows.Forms.Button();
            this.btnWorldBrowser = new System.Windows.Forms.Button();
            this.btnModelBrowser = new System.Windows.Forms.Button();
            this.btnAssetBrowser = new System.Windows.Forms.Button();
            this.button1 = new System.Windows.Forms.Button();
            this.toolTip1 = new System.Windows.Forms.ToolTip(this.components);
            this.gbxExtract = new System.Windows.Forms.GroupBox();
            this.comboBoxExtractTypes = new System.Windows.Forms.ComboBox();
            this.label6 = new System.Windows.Forms.Label();
            this.gbxFormat.SuspendLayout();
            this.gbxDB.SuspendLayout();
            this.gbxFQN.SuspendLayout();
            this.gbxPath.SuspendLayout();
            this.gbxLogs.SuspendLayout();
            this.gbxTools.SuspendLayout();
            this.gbxExtract.SuspendLayout();
            this.SuspendLayout();
            // 
            // textBoxAssetsFolder
            // 
            this.textBoxAssetsFolder.Location = new System.Drawing.Point(77, 13);
            this.textBoxAssetsFolder.Name = "textBoxAssetsFolder";
            this.textBoxAssetsFolder.Size = new System.Drawing.Size(264, 20);
            this.textBoxAssetsFolder.TabIndex = 0;
            this.textBoxAssetsFolder.TextChanged += new System.EventHandler(this.textBox1_TextChanged);
            // 
            // labelAssetsFolder
            // 
            this.labelAssetsFolder.AutoSize = true;
            this.labelAssetsFolder.Location = new System.Drawing.Point(6, 16);
            this.labelAssetsFolder.Name = "labelAssetsFolder";
            this.labelAssetsFolder.Size = new System.Drawing.Size(65, 13);
            this.labelAssetsFolder.TabIndex = 3;
            this.labelAssetsFolder.Text = "Asset Folder";
            // 
            // listBox1
            // 
            this.listBox1.FormattingEnabled = true;
            this.listBox1.Location = new System.Drawing.Point(10, 19);
            this.listBox1.Name = "listBox1";
            this.listBox1.ScrollAlwaysVisible = true;
            this.listBox1.Size = new System.Drawing.Size(323, 160);
            this.listBox1.TabIndex = 0;
            this.listBox1.TabStop = false;
            // 
            // textBox2
            // 
            this.textBox2.Location = new System.Drawing.Point(122, 35);
            this.textBox2.Name = "textBox2";
            this.textBox2.Size = new System.Drawing.Size(108, 20);
            this.textBox2.TabIndex = 6;
            this.textBox2.Text = "root";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(119, 19);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(75, 13);
            this.label2.TabIndex = 6;
            this.label2.Text = "DB UserName";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(6, 57);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(71, 13);
            this.label3.TabIndex = 7;
            this.label3.Text = "DB Password";
            // 
            // textBox3
            // 
            this.textBox3.Location = new System.Drawing.Point(9, 34);
            this.textBox3.Name = "textBox3";
            this.textBox3.Size = new System.Drawing.Size(108, 20);
            this.textBox3.TabIndex = 5;
            this.textBox3.Text = "127.0.0.1";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(6, 18);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(35, 13);
            this.label4.TabIndex = 10;
            this.label4.Text = "DB IP";
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(119, 58);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(53, 13);
            this.label5.TabIndex = 11;
            this.label5.Text = "DB Name";
            // 
            // textBox4
            // 
            this.textBox4.Location = new System.Drawing.Point(122, 74);
            this.textBox4.Name = "textBox4";
            this.textBox4.Size = new System.Drawing.Size(108, 20);
            this.textBox4.TabIndex = 8;
            this.textBox4.Text = "tor_dump";
            // 
            // listBox2
            // 
            this.listBox2.FormattingEnabled = true;
            this.listBox2.Location = new System.Drawing.Point(10, 182);
            this.listBox2.Name = "listBox2";
            this.listBox2.Size = new System.Drawing.Size(323, 225);
            this.listBox2.TabIndex = 0;
            this.listBox2.TabStop = false;
            // 
            // buttonToggleDatabase
            // 
            this.buttonToggleDatabase.Location = new System.Drawing.Point(121, 95);
            this.buttonToggleDatabase.Name = "buttonToggleDatabase";
            this.buttonToggleDatabase.Size = new System.Drawing.Size(108, 24);
            this.buttonToggleDatabase.TabIndex = 9;
            this.buttonToggleDatabase.Text = "Mysql Off";
            this.buttonToggleDatabase.UseVisualStyleBackColor = true;
            this.buttonToggleDatabase.Click += new System.EventHandler(this.button3_Click);
            // 
            // textBox5
            // 
            this.textBox5.Location = new System.Drawing.Point(9, 73);
            this.textBox5.Name = "textBox5";
            this.textBox5.PasswordChar = '*';
            this.textBox5.Size = new System.Drawing.Size(108, 20);
            this.textBox5.TabIndex = 7;
            // 
            // textBox6
            // 
            this.textBox6.Location = new System.Drawing.Point(9, 17);
            this.textBox6.Name = "textBox6";
            this.textBox6.Size = new System.Drawing.Size(162, 20);
            this.textBox6.TabIndex = 22;
            // 
            // searchButton
            // 
            this.searchButton.Location = new System.Drawing.Point(175, 15);
            this.searchButton.Name = "searchButton";
            this.searchButton.Size = new System.Drawing.Size(53, 23);
            this.searchButton.TabIndex = 23;
            this.searchButton.Text = "Search";
            this.toolTip1.SetToolTip(this.searchButton, "Perform search");
            this.searchButton.UseVisualStyleBackColor = true;
            this.searchButton.Click += new System.EventHandler(this.searchButton_Click);
            // 
            // gbxFormat
            // 
            this.gbxFormat.Controls.Add(this.versionTexBox);
            this.gbxFormat.Controls.Add(this.label1);
            this.gbxFormat.Controls.Add(this.chkBuildCompare);
            this.gbxFormat.Controls.Add(this.chkRemoveElements);
            this.gbxFormat.Controls.Add(this.cbxExtractFormat);
            this.gbxFormat.Controls.Add(this.chkVerbose);
            this.gbxFormat.Location = new System.Drawing.Point(12, 119);
            this.gbxFormat.Name = "gbxFormat";
            this.gbxFormat.Size = new System.Drawing.Size(584, 41);
            this.gbxFormat.TabIndex = 24;
            this.gbxFormat.TabStop = false;
            this.gbxFormat.Text = "Extract Format";
            // 
            // versionTexBox
            // 
            this.versionTexBox.Location = new System.Drawing.Point(512, 15);
            this.versionTexBox.Name = "versionTexBox";
            this.versionTexBox.Size = new System.Drawing.Size(62, 20);
            this.versionTexBox.TabIndex = 12;
            this.versionTexBox.TextChanged += new System.EventHandler(this.textBox1_TextChanged_1);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(462, 18);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(45, 13);
            this.label1.TabIndex = 13;
            this.label1.Text = "Version.";
            // 
            // chkBuildCompare
            // 
            this.chkBuildCompare.AutoSize = true;
            this.chkBuildCompare.Location = new System.Drawing.Point(175, 17);
            this.chkBuildCompare.Name = "chkBuildCompare";
            this.chkBuildCompare.Size = new System.Drawing.Size(94, 17);
            this.chkBuildCompare.TabIndex = 18;
            this.chkBuildCompare.Text = "Compare Build";
            this.toolTip1.SetToolTip(this.chkBuildCompare, "Generate an output of the changes between current and the previous builds.");
            this.chkBuildCompare.UseVisualStyleBackColor = true;
            this.chkBuildCompare.CheckedChanged += new System.EventHandler(this.chkBuildCompare_CheckedChanged);
            // 
            // chkRemoveElements
            // 
            this.chkRemoveElements.AutoSize = true;
            this.chkRemoveElements.Checked = true;
            this.chkRemoveElements.CheckState = System.Windows.Forms.CheckState.Checked;
            this.chkRemoveElements.Cursor = System.Windows.Forms.Cursors.Default;
            this.chkRemoveElements.Location = new System.Drawing.Point(275, 17);
            this.chkRemoveElements.Name = "chkRemoveElements";
            this.chkRemoveElements.Size = new System.Drawing.Size(92, 17);
            this.chkRemoveElements.TabIndex = 17;
            this.chkRemoveElements.Text = "Remove Elem";
            this.toolTip1.SetToolTip(this.chkRemoveElements, "Remove unchanged elements.");
            this.chkRemoveElements.UseVisualStyleBackColor = true;
            this.chkRemoveElements.CheckedChanged += new System.EventHandler(this.removeElementsCheckBox_CheckedChanged);
            // 
            // cbxExtractFormat
            // 
            this.cbxExtractFormat.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cbxExtractFormat.FormattingEnabled = true;
            this.cbxExtractFormat.Items.AddRange(new object[] {
            "Text",
            "JSON",
            "XML",
            "SQL"});
            this.cbxExtractFormat.Location = new System.Drawing.Point(6, 14);
            this.cbxExtractFormat.Name = "cbxExtractFormat";
            this.cbxExtractFormat.Size = new System.Drawing.Size(86, 21);
            this.cbxExtractFormat.TabIndex = 15;
            this.cbxExtractFormat.SelectedIndexChanged += new System.EventHandler(this.cbxExtractFormat_SelectedIndexChanged);
            // 
            // chkVerbose
            // 
            this.chkVerbose.AutoSize = true;
            this.chkVerbose.Checked = true;
            this.chkVerbose.CheckState = System.Windows.Forms.CheckState.Checked;
            this.chkVerbose.Location = new System.Drawing.Point(104, 17);
            this.chkVerbose.Name = "chkVerbose";
            this.chkVerbose.Size = new System.Drawing.Size(65, 17);
            this.chkVerbose.TabIndex = 14;
            this.chkVerbose.Text = "Verbose";
            this.toolTip1.SetToolTip(this.chkVerbose, "Export all data.");
            this.chkVerbose.UseVisualStyleBackColor = true;
            this.chkVerbose.CheckedChanged += new System.EventHandler(this.checkBox1_CheckedChanged);
            // 
            // gbxDB
            // 
            this.gbxDB.Controls.Add(this.label4);
            this.gbxDB.Controls.Add(this.textBox2);
            this.gbxDB.Controls.Add(this.label2);
            this.gbxDB.Controls.Add(this.label3);
            this.gbxDB.Controls.Add(this.textBox3);
            this.gbxDB.Controls.Add(this.label5);
            this.gbxDB.Controls.Add(this.textBox4);
            this.gbxDB.Controls.Add(this.textBox5);
            this.gbxDB.Controls.Add(this.buttonToggleDatabase);
            this.gbxDB.Location = new System.Drawing.Point(358, 160);
            this.gbxDB.Name = "gbxDB";
            this.gbxDB.Size = new System.Drawing.Size(238, 125);
            this.gbxDB.TabIndex = 26;
            this.gbxDB.TabStop = false;
            this.gbxDB.Text = "Database Options";
            // 
            // extractButton
            // 
            this.extractButton.Location = new System.Drawing.Point(160, 45);
            this.extractButton.Name = "extractButton";
            this.extractButton.Size = new System.Drawing.Size(68, 23);
            this.extractButton.TabIndex = 19;
            this.extractButton.Text = "Extract";
            this.toolTip1.SetToolTip(this.extractButton, "Opens the extraction dialog to extract data and compare builds.");
            this.extractButton.UseVisualStyleBackColor = true;
            this.extractButton.Click += new System.EventHandler(this.extractButton_Click);
            // 
            // gbxFQN
            // 
            this.gbxFQN.Controls.Add(this.textBox6);
            this.gbxFQN.Controls.Add(this.searchButton);
            this.gbxFQN.Controls.Add(this.label7);
            this.gbxFQN.Location = new System.Drawing.Point(359, 488);
            this.gbxFQN.Name = "gbxFQN";
            this.gbxFQN.Size = new System.Drawing.Size(233, 79);
            this.gbxFQN.TabIndex = 33;
            this.gbxFQN.TabStop = false;
            this.gbxFQN.Text = "FQN Search";
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Cursor = System.Windows.Forms.Cursors.IBeam;
            this.label7.Location = new System.Drawing.Point(6, 40);
            this.label7.MaximumSize = new System.Drawing.Size(230, 0);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(225, 26);
            this.label7.TabIndex = 26;
            this.label7.Text = "Be careful what you put in here. As it will search and output all occurences in t" +
    "he GOM.";
            this.label7.TextAlign = System.Drawing.ContentAlignment.TopCenter;
            // 
            // textBoxExtractFolder
            // 
            this.textBoxExtractFolder.Location = new System.Drawing.Point(87, 63);
            this.textBoxExtractFolder.Name = "textBoxExtractFolder";
            this.textBoxExtractFolder.Size = new System.Drawing.Size(254, 20);
            this.textBoxExtractFolder.TabIndex = 6;
            this.textBoxExtractFolder.TextChanged += new System.EventHandler(this.textBox7_TextChanged);
            // 
            // labelExtractFolder
            // 
            this.labelExtractFolder.AutoSize = true;
            this.labelExtractFolder.Location = new System.Drawing.Point(6, 66);
            this.labelExtractFolder.Name = "labelExtractFolder";
            this.labelExtractFolder.Size = new System.Drawing.Size(72, 13);
            this.labelExtractFolder.TabIndex = 35;
            this.labelExtractFolder.Text = "Extract Folder";
            // 
            // usePTSAssets
            // 
            this.usePTSAssets.AutoSize = true;
            this.usePTSAssets.Cursor = System.Windows.Forms.Cursors.Default;
            this.usePTSAssets.Location = new System.Drawing.Point(415, 16);
            this.usePTSAssets.Name = "usePTSAssets";
            this.usePTSAssets.Size = new System.Drawing.Size(47, 17);
            this.usePTSAssets.TabIndex = 2;
            this.usePTSAssets.Text = "PTS";
            this.toolTip1.SetToolTip(this.usePTSAssets, "Loads PTS assets if checked.");
            this.usePTSAssets.UseVisualStyleBackColor = true;
            this.usePTSAssets.CheckedChanged += new System.EventHandler(this.usePTSAssets_CheckedChanged);
            // 
            // buttonFindAssets
            // 
            this.buttonFindAssets.Image = global::PugTools.Properties.Resources.cross_shield;
            this.buttonFindAssets.ImageAlign = System.Drawing.ContentAlignment.BottomLeft;
            this.buttonFindAssets.Location = new System.Drawing.Point(346, 11);
            this.buttonFindAssets.Name = "buttonFindAssets";
            this.buttonFindAssets.Size = new System.Drawing.Size(63, 23);
            this.buttonFindAssets.TabIndex = 1;
            this.buttonFindAssets.Text = "Select";
            this.buttonFindAssets.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageBeforeText;
            this.buttonFindAssets.UseVisualStyleBackColor = true;
            this.buttonFindAssets.Click += new System.EventHandler(this.buttonFindAssets_Click);
            // 
            // buttonSelectExtractFolder
            // 
            this.buttonSelectExtractFolder.Location = new System.Drawing.Point(346, 61);
            this.buttonSelectExtractFolder.Name = "buttonSelectExtractFolder";
            this.buttonSelectExtractFolder.Size = new System.Drawing.Size(63, 23);
            this.buttonSelectExtractFolder.TabIndex = 7;
            this.buttonSelectExtractFolder.Text = "Select";
            this.buttonSelectExtractFolder.UseVisualStyleBackColor = true;
            this.buttonSelectExtractFolder.Click += new System.EventHandler(this.buttonSelectExtractFolder_Click);
            // 
            // buttonFindPrevAssets
            // 
            this.buttonFindPrevAssets.Image = global::PugTools.Properties.Resources.tick_shield;
            this.buttonFindPrevAssets.Location = new System.Drawing.Point(346, 36);
            this.buttonFindPrevAssets.Name = "buttonFindPrevAssets";
            this.buttonFindPrevAssets.Size = new System.Drawing.Size(63, 23);
            this.buttonFindPrevAssets.TabIndex = 4;
            this.buttonFindPrevAssets.Text = "Select";
            this.buttonFindPrevAssets.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageBeforeText;
            this.buttonFindPrevAssets.UseVisualStyleBackColor = true;
            this.buttonFindPrevAssets.Click += new System.EventHandler(this.buttonFindPrevAssets_Click);
            // 
            // labelPrevAssetsFolder
            // 
            this.labelPrevAssetsFolder.AutoSize = true;
            this.labelPrevAssetsFolder.Location = new System.Drawing.Point(6, 41);
            this.labelPrevAssetsFolder.Name = "labelPrevAssetsFolder";
            this.labelPrevAssetsFolder.Size = new System.Drawing.Size(90, 13);
            this.labelPrevAssetsFolder.TabIndex = 43;
            this.labelPrevAssetsFolder.Text = "Prev Asset Folder";
            // 
            // textBoxPrevAssetsFolder
            // 
            this.textBoxPrevAssetsFolder.Location = new System.Drawing.Point(102, 38);
            this.textBoxPrevAssetsFolder.Name = "textBoxPrevAssetsFolder";
            this.textBoxPrevAssetsFolder.Size = new System.Drawing.Size(239, 20);
            this.textBoxPrevAssetsFolder.TabIndex = 3;
            this.textBoxPrevAssetsFolder.TextChanged += new System.EventHandler(this.textBoxPrevAssetsFolder_TextChanged);
            // 
            // prevUsePTSAssets
            // 
            this.prevUsePTSAssets.AutoSize = true;
            this.prevUsePTSAssets.Cursor = System.Windows.Forms.Cursors.Default;
            this.prevUsePTSAssets.Location = new System.Drawing.Point(415, 41);
            this.prevUsePTSAssets.Name = "prevUsePTSAssets";
            this.prevUsePTSAssets.Size = new System.Drawing.Size(47, 17);
            this.prevUsePTSAssets.TabIndex = 5;
            this.prevUsePTSAssets.Text = "PTS";
            this.toolTip1.SetToolTip(this.prevUsePTSAssets, "Loads PTS assets if checked.");
            this.prevUsePTSAssets.UseVisualStyleBackColor = true;
            this.prevUsePTSAssets.CheckedChanged += new System.EventHandler(this.prevUsePTSAssets_CheckedChanged);
            // 
            // CrossLinkDomCheckBox
            // 
            this.CrossLinkDomCheckBox.AutoSize = true;
            this.CrossLinkDomCheckBox.Cursor = System.Windows.Forms.Cursors.Default;
            this.CrossLinkDomCheckBox.Location = new System.Drawing.Point(415, 66);
            this.CrossLinkDomCheckBox.Name = "CrossLinkDomCheckBox";
            this.CrossLinkDomCheckBox.Size = new System.Drawing.Size(54, 17);
            this.CrossLinkDomCheckBox.TabIndex = 6;
            this.CrossLinkDomCheckBox.Text = "X-Lnk";
            this.CrossLinkDomCheckBox.UseVisualStyleBackColor = true;
            this.CrossLinkDomCheckBox.CheckedChanged += new System.EventHandler(this.CrossLinkDomCheckBox_CheckedChanged);
            // 
            // progressBar1
            // 
            this.progressBar1.Location = new System.Drawing.Point(12, 579);
            this.progressBar1.MarqueeAnimationSpeed = 1000;
            this.progressBar1.Name = "progressBar1";
            this.progressBar1.Size = new System.Drawing.Size(584, 22);
            this.progressBar1.TabIndex = 45;
            // 
            // gbxPath
            // 
            this.gbxPath.Controls.Add(this.btnUnloadAllData);
            this.gbxPath.Controls.Add(this.CrossLinkDomCheckBox);
            this.gbxPath.Controls.Add(this.labelAssetsFolder);
            this.gbxPath.Controls.Add(this.prevUsePTSAssets);
            this.gbxPath.Controls.Add(this.usePTSAssets);
            this.gbxPath.Controls.Add(this.textBoxAssetsFolder);
            this.gbxPath.Controls.Add(this.textBoxExtractFolder);
            this.gbxPath.Controls.Add(this.buttonFindPrevAssets);
            this.gbxPath.Controls.Add(this.labelExtractFolder);
            this.gbxPath.Controls.Add(this.labelPrevAssetsFolder);
            this.gbxPath.Controls.Add(this.textBoxPrevAssetsFolder);
            this.gbxPath.Controls.Add(this.buttonFindAssets);
            this.gbxPath.Controls.Add(this.buttonSelectExtractFolder);
            this.gbxPath.Location = new System.Drawing.Point(12, 2);
            this.gbxPath.Name = "gbxPath";
            this.gbxPath.Size = new System.Drawing.Size(584, 91);
            this.gbxPath.TabIndex = 46;
            this.gbxPath.TabStop = false;
            this.gbxPath.Text = "Path Information";
            // 
            // btnUnloadAllData
            // 
            this.btnUnloadAllData.Location = new System.Drawing.Point(491, 12);
            this.btnUnloadAllData.Name = "btnUnloadAllData";
            this.btnUnloadAllData.Size = new System.Drawing.Size(89, 23);
            this.btnUnloadAllData.TabIndex = 46;
            this.btnUnloadAllData.Text = "Unload All Data";
            this.toolTip1.SetToolTip(this.btnUnloadAllData, "Unload current DOM and assets.");
            this.btnUnloadAllData.UseVisualStyleBackColor = true;
            this.btnUnloadAllData.Click += new System.EventHandler(this.btnUnloadAllData_Click);
            // 
            // gbxLogs
            // 
            this.gbxLogs.Controls.Add(this.listBox1);
            this.gbxLogs.Controls.Add(this.listBox2);
            this.gbxLogs.Location = new System.Drawing.Point(12, 160);
            this.gbxLogs.Name = "gbxLogs";
            this.gbxLogs.Size = new System.Drawing.Size(341, 413);
            this.gbxLogs.TabIndex = 47;
            this.gbxLogs.TabStop = false;
            this.gbxLogs.Text = "Logs";
            // 
            // gbxTools
            // 
            this.gbxTools.Controls.Add(this.btnNodeBrowser);
            this.gbxTools.Controls.Add(this.btnWorldBrowser);
            this.gbxTools.Controls.Add(this.btnModelBrowser);
            this.gbxTools.Controls.Add(this.btnAssetBrowser);
            this.gbxTools.Controls.Add(this.button1);
            this.gbxTools.Location = new System.Drawing.Point(359, 374);
            this.gbxTools.Name = "gbxTools";
            this.gbxTools.Size = new System.Drawing.Size(233, 108);
            this.gbxTools.TabIndex = 34;
            this.gbxTools.TabStop = false;
            this.gbxTools.Text = "Tools";
            // 
            // btnNodeBrowser
            // 
            this.btnNodeBrowser.Location = new System.Drawing.Point(119, 19);
            this.btnNodeBrowser.Name = "btnNodeBrowser";
            this.btnNodeBrowser.Size = new System.Drawing.Size(108, 23);
            this.btnNodeBrowser.TabIndex = 50;
            this.btnNodeBrowser.Text = "Node Browser";
            this.btnNodeBrowser.UseVisualStyleBackColor = true;
            this.btnNodeBrowser.Click += new System.EventHandler(this.btnNodeBrowser_Click);
            // 
            // btnWorldBrowser
            // 
            this.btnWorldBrowser.Enabled = false;
            this.btnWorldBrowser.Location = new System.Drawing.Point(6, 77);
            this.btnWorldBrowser.Name = "btnWorldBrowser";
            this.btnWorldBrowser.Size = new System.Drawing.Size(108, 23);
            this.btnWorldBrowser.TabIndex = 49;
            this.btnWorldBrowser.Text = "World Browser";
            this.btnWorldBrowser.UseVisualStyleBackColor = true;
            this.btnWorldBrowser.Click += new System.EventHandler(this.btnWorldBrowser_Click);
            // 
            // btnModelBrowser
            // 
            this.btnModelBrowser.Location = new System.Drawing.Point(6, 48);
            this.btnModelBrowser.Name = "btnModelBrowser";
            this.btnModelBrowser.Size = new System.Drawing.Size(108, 23);
            this.btnModelBrowser.TabIndex = 48;
            this.btnModelBrowser.Text = "Model Browser";
            this.btnModelBrowser.UseVisualStyleBackColor = true;
            this.btnModelBrowser.Click += new System.EventHandler(this.btnModelBrowser_Click);
            // 
            // btnAssetBrowser
            // 
            this.btnAssetBrowser.Location = new System.Drawing.Point(6, 19);
            this.btnAssetBrowser.Name = "btnAssetBrowser";
            this.btnAssetBrowser.Size = new System.Drawing.Size(108, 23);
            this.btnAssetBrowser.TabIndex = 24;
            this.btnAssetBrowser.Text = "Asset Browser";
            this.btnAssetBrowser.UseVisualStyleBackColor = true;
            this.btnAssetBrowser.Click += new System.EventHandler(this.btnAssetBrowser_Click);
            // 
            // button1
            // 
            this.button1.Location = new System.Drawing.Point(121, 48);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(106, 23);
            this.button1.TabIndex = 1;
            this.button1.Text = "Create SQL";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.button1_Click);
            // 
            // gbxExtract
            // 
            this.gbxExtract.Controls.Add(this.comboBoxExtractTypes);
            this.gbxExtract.Controls.Add(this.extractButton);
            this.gbxExtract.Location = new System.Drawing.Point(358, 291);
            this.gbxExtract.Name = "gbxExtract";
            this.gbxExtract.Size = new System.Drawing.Size(238, 77);
            this.gbxExtract.TabIndex = 31;
            this.gbxExtract.TabStop = false;
            this.gbxExtract.Text = "Extractors";
            // 
            // comboBoxExtractTypes
            // 
            this.comboBoxExtractTypes.BackColor = System.Drawing.SystemColors.ControlLightLight;
            this.comboBoxExtractTypes.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.comboBoxExtractTypes.FormattingEnabled = true;
            this.comboBoxExtractTypes.Location = new System.Drawing.Point(10, 47);
            this.comboBoxExtractTypes.Name = "comboBoxExtractTypes";
            this.comboBoxExtractTypes.Size = new System.Drawing.Size(144, 21);
            this.comboBoxExtractTypes.Sorted = true;
            this.comboBoxExtractTypes.TabIndex = 20;
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Cursor = System.Windows.Forms.Cursors.IBeam;
            this.label6.Location = new System.Drawing.Point(367, 307);
            this.label6.MaximumSize = new System.Drawing.Size(230, 0);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(229, 26);
            this.label6.TabIndex = 29;
            this.label6.Text = "Click and select what you want from the dialog window. The default is to dump eve" +
    "rything.";
            // 
            // Tools
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(604, 613);
            this.Controls.Add(this.label6);
            this.Controls.Add(this.gbxTools);
            this.Controls.Add(this.gbxLogs);
            this.Controls.Add(this.gbxPath);
            this.Controls.Add(this.progressBar1);
            this.Controls.Add(this.gbxFQN);
            this.Controls.Add(this.gbxExtract);
            this.Controls.Add(this.gbxDB);
            this.Controls.Add(this.gbxFormat);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow;
            this.Name = "Tools";
            this.RightToLeft = System.Windows.Forms.RightToLeft.No;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Swtor Pug Tools";
            this.gbxFormat.ResumeLayout(false);
            this.gbxFormat.PerformLayout();
            this.gbxDB.ResumeLayout(false);
            this.gbxDB.PerformLayout();
            this.gbxFQN.ResumeLayout(false);
            this.gbxFQN.PerformLayout();
            this.gbxPath.ResumeLayout(false);
            this.gbxPath.PerformLayout();
            this.gbxLogs.ResumeLayout(false);
            this.gbxTools.ResumeLayout(false);
            this.gbxExtract.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TextBox textBoxAssetsFolder;
        private System.Windows.Forms.Label labelAssetsFolder;
        private System.Windows.Forms.ListBox listBox1;
        private System.Windows.Forms.TextBox textBox2;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.TextBox textBox3;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.TextBox textBox4;
        private System.Windows.Forms.ListBox listBox2;
        private System.Windows.Forms.Button buttonToggleDatabase;
        private System.Windows.Forms.TextBox textBox5;
        private System.Windows.Forms.TextBox textBox6;
        private System.Windows.Forms.Button searchButton;
        private System.Windows.Forms.GroupBox gbxFormat;
        private System.Windows.Forms.GroupBox gbxDB;
        private System.Windows.Forms.GroupBox gbxFQN;
        private System.Windows.Forms.TextBox textBoxExtractFolder;
        private System.Windows.Forms.Label labelExtractFolder;
        private System.Windows.Forms.CheckBox chkVerbose;
        private System.Windows.Forms.Button buttonFindAssets;
        private System.Windows.Forms.Button buttonSelectExtractFolder;
        private System.Windows.Forms.CheckBox chkRemoveElements;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox versionTexBox;
        private System.Windows.Forms.Button extractButton;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.CheckBox usePTSAssets;
        private System.Windows.Forms.Button buttonFindPrevAssets;
        private System.Windows.Forms.Label labelPrevAssetsFolder;
        private System.Windows.Forms.TextBox textBoxPrevAssetsFolder;
        private System.Windows.Forms.CheckBox prevUsePTSAssets;
        private System.Windows.Forms.ProgressBar progressBar1;
        private System.Windows.Forms.CheckBox CrossLinkDomCheckBox;
        private System.Windows.Forms.ComboBox cbxExtractFormat;
        private System.Windows.Forms.GroupBox gbxPath;
        private System.Windows.Forms.GroupBox gbxLogs;
        private System.Windows.Forms.CheckBox chkBuildCompare;
        private System.Windows.Forms.Button btnUnloadAllData;
        private System.Windows.Forms.GroupBox gbxTools;
        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.ToolTip toolTip1;
        private System.Windows.Forms.Button btnNodeBrowser;
        private System.Windows.Forms.Button btnWorldBrowser;
        private System.Windows.Forms.Button btnModelBrowser;
        private System.Windows.Forms.Button btnAssetBrowser;
        private System.Windows.Forms.GroupBox gbxExtract;
        private System.Windows.Forms.ComboBox comboBoxExtractTypes;
        private System.Windows.Forms.Label label6;
    }
}

