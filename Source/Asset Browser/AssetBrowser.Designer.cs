namespace PugTools
{
    partial class AssetBrowser
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(AssetBrowser));
            this.splitContainer1 = new System.Windows.Forms.SplitContainer();
            this.splitContainerLeft = new System.Windows.Forms.SplitContainer();
            this.txtSearch = new System.Windows.Forms.TextBox();
            this.btnSearch = new System.Windows.Forms.Button();
            this.btnFindNext = new System.Windows.Forms.Button();
            this.btnClearSearch = new System.Windows.Forms.Button();
            this.treeViewList = new TreeViewFast.Controls.TreeViewFast();
            this.imageList1 = new System.Windows.Forms.ImageList(this.components);
            this.splitContainerCenter = new System.Windows.Forms.SplitContainer();
            this.elementHost1 = new System.Windows.Forms.Integration.ElementHost();
            this.hexBox1 = new Be.Windows.Forms.HexBox();
            this.pictureBox1 = new System.Windows.Forms.PictureBox();
            this.pictureBox2 = new System.Windows.Forms.PictureBox();
            this.renderPanel = new System.Windows.Forms.Panel();
            this.treeItemView = new BrightIdeasSoftware.TreeListView();
            this.olvColumn1 = ((BrightIdeasSoftware.OLVColumn)(new BrightIdeasSoftware.OLVColumn()));
            this.olvColumn2 = ((BrightIdeasSoftware.OLVColumn)(new BrightIdeasSoftware.OLVColumn()));
            this.txtRawView = new System.Windows.Forms.TextBox();
            this.webBrowser1 = new System.Windows.Forms.WebBrowser();
            this.statusStrip1 = new System.Windows.Forms.StatusStrip();
            this.toolStripStatusLabelLeft1 = new System.Windows.Forms.ToolStripStatusLabel();
            this.toolStripStatusLabelLeft2 = new System.Windows.Forms.ToolStripStatusLabel();
            this.toolStripProgressBar1 = new System.Windows.Forms.ToolStripProgressBar();
            this.toolStripStatusLabelRight = new System.Windows.Forms.ToolStripStatusLabel();
            this.splitContainerRight = new System.Windows.Forms.SplitContainer();
            this.dataGridView1 = new System.Windows.Forms.DataGridView();
            this.lblExtractPath = new System.Windows.Forms.Label();
            this.txtExtractPath = new System.Windows.Forms.TextBox();
            this.btnChooseExtract = new System.Windows.Forms.Button();
            this.btnPreview = new System.Windows.Forms.Button();
            this.btnAudioStop = new System.Windows.Forms.Button();
            this.btnExtract = new System.Windows.Forms.Button();
            this.btnSaveTxtHash = new System.Windows.Forms.Button();
            this.btnViewRaw = new System.Windows.Forms.Button();
            this.btnViewHex = new System.Windows.Forms.Button();
            this.btnFindFileNames = new System.Windows.Forms.Button();
            this.btnTestFile = new System.Windows.Forms.Button();
            this.btnFileTable = new System.Windows.Forms.Button();
            this.btnHashStatus = new System.Windows.Forms.Button();
            this.btnHelp = new System.Windows.Forms.Button();
            this.backgroundWorker1 = new System.ComponentModel.BackgroundWorker();
            this.backgroundWorker2 = new System.ComponentModel.BackgroundWorker();
            this.contextMenuStrip1 = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.extractToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.extractByExtensionToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.backgroundWorker3 = new System.ComponentModel.BackgroundWorker();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).BeginInit();
            this.splitContainer1.Panel1.SuspendLayout();
            this.splitContainer1.Panel2.SuspendLayout();
            this.splitContainer1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainerLeft)).BeginInit();
            this.splitContainerLeft.Panel1.SuspendLayout();
            this.splitContainerLeft.Panel2.SuspendLayout();
            this.splitContainerLeft.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainerCenter)).BeginInit();
            this.splitContainerCenter.Panel1.SuspendLayout();
            this.splitContainerCenter.Panel2.SuspendLayout();
            this.splitContainerCenter.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.treeItemView)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox2)).BeginInit();
            this.statusStrip1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainerRight)).BeginInit();
            this.splitContainerRight.Panel1.SuspendLayout();
            this.splitContainerRight.Panel2.SuspendLayout();
            this.splitContainerRight.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView1)).BeginInit();
            this.contextMenuStrip1.SuspendLayout();
            this.SuspendLayout();
            // 
            // splitContainer1
            // 
            this.splitContainer1.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.splitContainer1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitContainer1.Location = new System.Drawing.Point(0, 0);
            this.splitContainer1.Name = "splitContainer1";
            this.splitContainer1.Size = new System.Drawing.Size(1187, 633);
            this.splitContainer1.SplitterDistance = 243;
            this.splitContainer1.TabStop = false;
            // 
            // splitContainer1.Panel1
            // 
            this.splitContainer1.Panel1.Controls.Add(this.splitContainerLeft);
            // 
            // splitContainerLeft
            // 
            this.splitContainerLeft.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.splitContainerLeft.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitContainerLeft.FixedPanel = System.Windows.Forms.FixedPanel.Panel1;
            this.splitContainerLeft.Location = new System.Drawing.Point(0, 0);
            this.splitContainerLeft.Name = "splitContainerLeft";
            this.splitContainerLeft.Orientation = System.Windows.Forms.Orientation.Horizontal;
            this.splitContainerLeft.Size = new System.Drawing.Size(243, 633);
            this.splitContainerLeft.SplitterDistance = 80;
            this.splitContainerLeft.TabStop = false;
            // 
            // splitContainerLeft.Panel1
            // 
            this.splitContainerLeft.Panel1.Controls.Add(this.btnClearSearch);
            this.splitContainerLeft.Panel1.Controls.Add(this.btnFindNext);
            this.splitContainerLeft.Panel1.Controls.Add(this.btnSearch);
            this.splitContainerLeft.Panel1.Controls.Add(this.txtSearch);
            // 
            // txtSearch
            // 
            this.txtSearch.Enabled = false;
            this.txtSearch.Location = new System.Drawing.Point(5, 7);
            this.txtSearch.Name = "txtSearch";
            this.txtSearch.Size = new System.Drawing.Size(149, 20);
            this.txtSearch.TabIndex = 0;
            // 
            // btnSearch
            // 
            this.btnSearch.Enabled = false;
            this.btnSearch.Location = new System.Drawing.Point(160, 6);
            this.btnSearch.Name = "btnSearch";
            this.btnSearch.Size = new System.Drawing.Size(75, 22);
            this.btnSearch.TabIndex = 1;
            this.btnSearch.Text = "Search";
            this.btnSearch.UseVisualStyleBackColor = true;
            this.btnSearch.Click += new System.EventHandler(this.BtnSearch_Click);
            // 
            // btnFindNext
            // 
            this.btnFindNext.Enabled = false;
            this.btnFindNext.Location = new System.Drawing.Point(5, 34);
            this.btnFindNext.Name = "btnFindNext";
            this.btnFindNext.Size = new System.Drawing.Size(112, 23);
            this.btnFindNext.TabIndex = 2;
            this.btnFindNext.Text = "Find Next";
            this.btnFindNext.UseVisualStyleBackColor = true;
            this.btnFindNext.Click += new System.EventHandler(this.BtnFindNext_Click);
            // 
            // btnClearSearch
            // 
            this.btnClearSearch.Enabled = false;
            this.btnClearSearch.Location = new System.Drawing.Point(123, 34);
            this.btnClearSearch.Name = "btnClearSearch";
            this.btnClearSearch.Size = new System.Drawing.Size(112, 23);
            this.btnClearSearch.TabIndex = 3;
            this.btnClearSearch.Text = "Clear";
            this.btnClearSearch.UseVisualStyleBackColor = true;
            this.btnClearSearch.Click += new System.EventHandler(this.BtnClearSearch_Click);
            // 
            // splitContainerLeft.Panel2
            // 
            this.splitContainerLeft.Panel2.Controls.Add(this.treeViewList);
            // 
            // treeViewList
            // 
            this.treeViewList.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
            | System.Windows.Forms.AnchorStyles.Left)
            | System.Windows.Forms.AnchorStyles.Right)));
            this.treeViewList.ImageIndex = 0;
            this.treeViewList.ImageList = this.imageList1;
            this.treeViewList.Dock = System.Windows.Forms.DockStyle.Fill;
            this.treeViewList.Location = new System.Drawing.Point(3, 3);
            this.treeViewList.Name = "treeViewList";
            this.treeViewList.SelectedImageIndex = 0;
            this.treeViewList.Size = new System.Drawing.Size(234, 555);
            this.treeViewList.Visible = false;
            this.treeViewList.AfterSelect += new System.Windows.Forms.TreeViewEventHandler(this.TreeViewList_AfterSelect);
            this.treeViewList.MouseUp += new System.Windows.Forms.MouseEventHandler(this.TreeViewList_MouseUp);
            // 
            // imageList1
            // 
            this.imageList1.ImageStream = ((System.Windows.Forms.ImageListStreamer)(resources.GetObject("imageList1.ImageStream")));
            this.imageList1.TransparentColor = System.Drawing.Color.Transparent;
            this.imageList1.Images.SetKeyName(0, "COMPUTER.ICO");
            this.imageList1.Images.SetKeyName(1, "Folder.ico");
            this.imageList1.Images.SetKeyName(2, "textdoc.ico");
            // 
            // splitContainer1.Panel2
            // 
            this.splitContainer1.Panel2.Controls.Add(this.splitContainerCenter);
            // 
            // splitContainerCenter
            // 
            this.splitContainerCenter.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.splitContainerCenter.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitContainerCenter.Location = new System.Drawing.Point(0, 0);
            this.splitContainerCenter.Name = "splitContainerCenter";
            this.splitContainerCenter.Size = new System.Drawing.Size(940, 633);
            this.splitContainerCenter.SplitterDistance = 711;
            this.splitContainerCenter.TabStop = false;
            // 
            // splitContainerCenter.Panel1
            // 
            this.splitContainerCenter.Panel1.AutoScroll = true;
            this.splitContainerCenter.Panel1.Controls.Add(this.elementHost1);
            this.splitContainerCenter.Panel1.Controls.Add(this.hexBox1);
            this.splitContainerCenter.Panel1.Controls.Add(this.pictureBox1);
            this.splitContainerCenter.Panel1.Controls.Add(this.pictureBox2);
            this.splitContainerCenter.Panel1.Controls.Add(this.renderPanel);
            this.splitContainerCenter.Panel1.Controls.Add(this.treeItemView);
            this.splitContainerCenter.Panel1.Controls.Add(this.txtRawView);
            this.splitContainerCenter.Panel1.Controls.Add(this.webBrowser1);
            this.splitContainerCenter.Panel1.Controls.Add(this.statusStrip1);
            // 
            // elementHost1
            // 
            this.elementHost1.AutoSize = true;
            this.elementHost1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.elementHost1.Location = new System.Drawing.Point(0, 0);
            this.elementHost1.Name = "elementHost1";
            this.elementHost1.Size = new System.Drawing.Size(707, 607);
            this.elementHost1.Text = "elementHost1";
            this.elementHost1.Visible = false;
            this.elementHost1.Child = null;
            // 
            // hexBox1
            // 
            this.hexBox1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
            | System.Windows.Forms.AnchorStyles.Left)
            | System.Windows.Forms.AnchorStyles.Right)));
            this.hexBox1.ColumnInfoVisible = true;
            this.hexBox1.Font = new System.Drawing.Font("Tahoma", 8.25F);
            this.hexBox1.LineInfoVisible = true;
            this.hexBox1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.hexBox1.Location = new System.Drawing.Point(3, 3);
            this.hexBox1.Name = "hexBox1";
            this.hexBox1.ReadOnly = true;
            this.hexBox1.ShadowSelectionColor = System.Drawing.Color.FromArgb(((int)(((byte)(100)))), ((int)(((byte)(60)))), ((int)(((byte)(188)))), ((int)(((byte)(255)))));
            this.hexBox1.Size = new System.Drawing.Size(701, 601);
            this.hexBox1.StringViewVisible = true;
            this.hexBox1.UseFixedBytesPerLine = true;
            this.hexBox1.Visible = false;
            this.hexBox1.VScrollBarVisible = true;
            this.hexBox1.CurrentLineChanged += new System.EventHandler(this.Position_Changed);
            this.hexBox1.CurrentPositionInLineChanged += new System.EventHandler(this.Position_Changed);
            // 
            // pictureBox1
            // 
            this.pictureBox1.BackColor = System.Drawing.Color.White;
            this.pictureBox1.BackgroundImage = global::PugTools.Properties.Resources.Transparent;
            this.pictureBox1.Dock = System.Windows.Forms.DockStyle.Left;
            this.pictureBox1.Location = new System.Drawing.Point(3, 3);
            this.pictureBox1.Name = "pictureBox1";
            this.pictureBox1.Size = new System.Drawing.Size(701, 601);
            this.pictureBox1.SizeMode = System.Windows.Forms.PictureBoxSizeMode.AutoSize;
            this.pictureBox1.TabStop = false;
            this.pictureBox1.Visible = false;
            // 
            // pictureBox2
            // 
            this.pictureBox2.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
            | System.Windows.Forms.AnchorStyles.Left)
            | System.Windows.Forms.AnchorStyles.Right)));
            this.pictureBox2.BackColor = System.Drawing.Color.White;
            this.pictureBox2.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Center;
            this.pictureBox2.Image = global::PugTools.Properties.Resources.LoadingImage;
            this.pictureBox2.Dock = System.Windows.Forms.DockStyle.Fill;
            this.pictureBox2.Location = new System.Drawing.Point(3, 3);
            this.pictureBox2.Name = "pictureBox2";
            this.pictureBox2.Size = new System.Drawing.Size(701, 601);
            this.pictureBox2.SizeMode = System.Windows.Forms.PictureBoxSizeMode.CenterImage;
            this.pictureBox2.TabStop = false;
            this.pictureBox2.Visible = false;
            // 
            // renderPanel
            // 
            this.renderPanel.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
            | System.Windows.Forms.AnchorStyles.Left)
            | System.Windows.Forms.AnchorStyles.Right)));
            this.renderPanel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.renderPanel.Location = new System.Drawing.Point(3, 3);
            this.renderPanel.Name = "renderPanel";
            this.renderPanel.Size = new System.Drawing.Size(701, 601);
            this.renderPanel.Visible = false;
            this.renderPanel.MouseHover += new System.EventHandler(this.RenderPanel_MouseHover);
            this.renderPanel.Resize += new System.EventHandler(this.RenderPanel_Resize);
            // 
            // treeItemView
            // 
            this.treeItemView.AllColumns.Add(this.olvColumn1);
            this.treeItemView.AllColumns.Add(this.olvColumn2);
            this.treeItemView.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
            | System.Windows.Forms.AnchorStyles.Left)
            | System.Windows.Forms.AnchorStyles.Right)));
            this.treeItemView.BackColor = System.Drawing.SystemColors.Window;
            this.treeItemView.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.olvColumn1,
            this.olvColumn2});
            this.treeItemView.GridLines = true;
            this.treeItemView.Dock = System.Windows.Forms.DockStyle.Fill;
            this.treeItemView.Location = new System.Drawing.Point(3, 3);
            this.treeItemView.Name = "treeItemView";
            this.treeItemView.OwnerDraw = true;
            this.treeItemView.ShowGroups = false;
            this.treeItemView.Size = new System.Drawing.Size(701, 601);
            this.treeItemView.UseCompatibleStateImageBehavior = false;
            this.treeItemView.View = System.Windows.Forms.View.Details;
            this.treeItemView.VirtualMode = true;
            this.treeItemView.Visible = false;
            this.treeItemView.SelectionChanged += new System.EventHandler(this.TreeItemView_ItemSelectionChanged);
            // 
            // olvColumn1
            // 
            this.olvColumn1.AspectName = "name";
            this.olvColumn1.CellPadding = null;
            this.olvColumn1.Text = "Name";
            this.olvColumn1.Width = 167;
            // 
            // olvColumn2
            // 
            this.olvColumn2.AspectName = "displayValue";
            this.olvColumn2.CellPadding = null;
            this.olvColumn2.Text = "Value";
            this.olvColumn2.Width = 230;
            // 
            // txtRawView
            // 
            this.txtRawView.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
            | System.Windows.Forms.AnchorStyles.Left)
            | System.Windows.Forms.AnchorStyles.Right)));
            this.txtRawView.BackColor = System.Drawing.Color.White;
            this.txtRawView.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txtRawView.ForeColor = System.Drawing.Color.Black;
            this.txtRawView.Dock = System.Windows.Forms.DockStyle.Fill;
            this.txtRawView.Location = new System.Drawing.Point(3, 3);
            this.txtRawView.Multiline = true;
            this.txtRawView.Name = "txtRawView";
            this.txtRawView.ScrollBars = System.Windows.Forms.ScrollBars.Both;
            this.txtRawView.Size = new System.Drawing.Size(701, 601);
            this.txtRawView.Visible = false;
            // 
            // webBrowser1
            // 
            this.webBrowser1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.webBrowser1.Location = new System.Drawing.Point(0, 0);
            this.webBrowser1.MinimumSize = new System.Drawing.Size(20, 20);
            this.webBrowser1.Name = "webBrowser1";
            this.webBrowser1.Size = new System.Drawing.Size(707, 629);
            this.webBrowser1.Visible = false;
            // 
            // statusStrip1
            // 
            this.statusStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStripStatusLabelLeft1,
            this.toolStripStatusLabelLeft2,
            this.toolStripProgressBar1,
            this.toolStripStatusLabelRight});
            this.txtRawView.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.statusStrip1.Location = new System.Drawing.Point(0, 607);
            this.statusStrip1.Name = "statusStrip1";
            this.statusStrip1.Size = new System.Drawing.Size(707, 22);
            this.statusStrip1.Text = "statusStrip1";
            // 
            // toolStripStatusLabelLeft1
            // 
            this.toolStripStatusLabelLeft1.Name = "toolStripStatusLabelLeft1";
            this.toolStripStatusLabelLeft1.Size = new System.Drawing.Size(0, 17);
            // 
            // toolStripStatusLabelLeft2
            // 
            this.toolStripStatusLabelLeft2.Name = "toolStripStatusLabelLeft2";
            this.toolStripStatusLabelLeft2.Size = new System.Drawing.Size(0, 17);
            // 
            // toolStripProgressBar1
            // 
            this.toolStripProgressBar1.ForeColor = System.Drawing.Color.Lime;
            this.toolStripProgressBar1.Name = "toolStripProgressBar1";
            this.toolStripProgressBar1.Size = new System.Drawing.Size(100, 16);
            this.toolStripProgressBar1.Style = System.Windows.Forms.ProgressBarStyle.Marquee;
            this.toolStripProgressBar1.Visible = false;
            // 
            // toolStripStatusLabelRight
            // 
            this.toolStripStatusLabelRight.Name = "toolStripStatusLabelRight";
            this.toolStripStatusLabelRight.Size = new System.Drawing.Size(0, 17);
            // 
            // splitContainerCenter.Panel2
            // 
            this.splitContainerCenter.Panel2.Controls.Add(this.splitContainerRight);
            // 
            // splitContainerRight
            // 
            this.splitContainerRight.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.splitContainerRight.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitContainerRight.FixedPanel = System.Windows.Forms.FixedPanel.Panel2;
            this.splitContainerRight.Location = new System.Drawing.Point(0, 0);
            this.splitContainerRight.Name = "splitContainerRight";
            this.splitContainerRight.Orientation = System.Windows.Forms.Orientation.Horizontal;
            this.splitContainerRight.Size = new System.Drawing.Size(225, 633);
            this.splitContainerRight.SplitterDistance = 350;
            this.splitContainerRight.TabStop = false;
            // 
            // splitContainerRight.Panel1
            // 
            this.splitContainerRight.Panel1.Controls.Add(this.dataGridView1);
            // 
            // dataGridView1
            // 
            this.dataGridView1.AllowUserToAddRows = false;
            this.dataGridView1.AllowUserToDeleteRows = false;
            this.dataGridView1.AllowUserToResizeRows = false;
            this.dataGridView1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
            | System.Windows.Forms.AnchorStyles.Left)
            | System.Windows.Forms.AnchorStyles.Right)));
            this.dataGridView1.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dataGridView1.Enabled = false;
            this.dataGridView1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.dataGridView1.Location = new System.Drawing.Point(3, 3);
            this.dataGridView1.Name = "dataGridView1";
            this.dataGridView1.RowHeadersVisible = false;
            this.dataGridView1.Size = new System.Drawing.Size(215, 393);
            this.dataGridView1.VirtualMode = true;
            // 
            // splitContainerRight.Panel2
            // 
            this.splitContainerRight.Panel2.Controls.Add(this.lblExtractPath);
            this.splitContainerRight.Panel2.Controls.Add(this.txtExtractPath);
            this.splitContainerRight.Panel2.Controls.Add(this.btnChooseExtract);
            this.splitContainerRight.Panel2.Controls.Add(this.btnPreview);
            this.splitContainerRight.Panel2.Controls.Add(this.btnAudioStop);
            this.splitContainerRight.Panel2.Controls.Add(this.btnExtract);
            this.splitContainerRight.Panel2.Controls.Add(this.btnSaveTxtHash);
            this.splitContainerRight.Panel2.Controls.Add(this.btnViewRaw);
            this.splitContainerRight.Panel2.Controls.Add(this.btnViewHex);
            this.splitContainerRight.Panel2.Controls.Add(this.btnFindFileNames);
            this.splitContainerRight.Panel2.Controls.Add(this.btnTestFile);
            this.splitContainerRight.Panel2.Controls.Add(this.btnFileTable);
            this.splitContainerRight.Panel2.Controls.Add(this.btnHashStatus);
            this.splitContainerRight.Panel2.Controls.Add(this.btnHelp);
            // 
            // lblExtractPath
            // 
            this.lblExtractPath.AutoSize = true;
            this.lblExtractPath.Location = new System.Drawing.Point(0, 4);
            this.lblExtractPath.Name = "lblExtractPath";
            this.lblExtractPath.Size = new System.Drawing.Size(65, 13);
            // this.lblExtractPath.TabIndex = 3;
            this.lblExtractPath.Text = "Extract Path";
            // 
            // txtExtractPath
            // 
            this.txtExtractPath.Enabled = false;
            this.txtExtractPath.Location = new System.Drawing.Point(3, 20);
            this.txtExtractPath.Name = "txtExtractPath";
            this.txtExtractPath.Size = new System.Drawing.Size(206, 20);
            this.txtExtractPath.TabIndex = 0;
            // 
            // btnChooseExtract
            // 
            this.btnChooseExtract.Enabled = false;
            this.btnChooseExtract.Location = new System.Drawing.Point(212, 19);
            this.btnChooseExtract.Name = "btnChooseExtract";
            this.btnChooseExtract.Size = new System.Drawing.Size(33, 22);
            this.btnChooseExtract.TabIndex = 1;
            this.btnChooseExtract.Text = "...";
            this.btnChooseExtract.UseVisualStyleBackColor = true;
            this.btnChooseExtract.Click += new System.EventHandler(this.BtnChooseExtract_Click);
            // 
            // btnPreview
            // 
            this.btnPreview.Enabled = false;
            this.btnPreview.Location = new System.Drawing.Point(5, 46);
            this.btnPreview.Name = "btnPreview";
            this.btnPreview.Size = new System.Drawing.Size(98, 23);
            this.btnPreview.TabIndex = 2;
            this.btnPreview.Text = "Auto Preview On";
            this.btnPreview.UseVisualStyleBackColor = true;
            this.btnPreview.Click += new System.EventHandler(this.BtnPreview_Click);
            // 
            // btnAudioStop
            // 
            this.btnAudioStop.Enabled = false;
            this.btnAudioStop.Location = new System.Drawing.Point(109, 46);
            this.btnAudioStop.Name = "btnAudioStop";
            this.btnAudioStop.Size = new System.Drawing.Size(98, 23);
            this.btnAudioStop.TabIndex = 3;
            this.btnAudioStop.Text = "Stop Audio";
            this.btnAudioStop.UseVisualStyleBackColor = true;
            this.btnAudioStop.Click += new System.EventHandler(this.BtnAudioStop_Click);
            // 
            // btnExtract
            // 
            this.btnExtract.Enabled = false;
            this.btnExtract.Location = new System.Drawing.Point(5, 75);
            this.btnExtract.Name = "btnExtract";
            this.btnExtract.Size = new System.Drawing.Size(98, 23);
            this.btnExtract.TabIndex = 4;
            this.btnExtract.Text = "Extract Object";
            this.btnExtract.UseVisualStyleBackColor = true;
            this.btnExtract.Click += new System.EventHandler(this.BtnExtract_Click);
            //
            // btnSaveTxtHash
            //
            this.btnSaveTxtHash.Enabled = false;
            this.btnSaveTxtHash.Location = new System.Drawing.Point(109, 75);
            this.btnSaveTxtHash.Name = "SaveTxtHash";
            this.btnSaveTxtHash.Size = new System.Drawing.Size(98, 23);
            this.btnSaveTxtHash.TabIndex = 5;
            this.btnSaveTxtHash.Text = "Save Hash File";
            this.btnSaveTxtHash.UseVisualStyleBackColor = true;
            this.btnSaveTxtHash.Click += new System.EventHandler(this.BtnSaveTxtHash_Click);
            // 
            // btnViewRaw
            // 
            this.btnViewRaw.Enabled = false;
            this.btnViewRaw.Location = new System.Drawing.Point(5, 104);
            this.btnViewRaw.Name = "btnViewRaw";
            this.btnViewRaw.Size = new System.Drawing.Size(98, 23);
            this.btnViewRaw.TabIndex = 6;
            this.btnViewRaw.Text = "View RAW Data";
            this.btnViewRaw.UseVisualStyleBackColor = true;
            this.btnViewRaw.Click += new System.EventHandler(this.BtnViewRaw_Click);
            // 
            // btnViewHex
            // 
            this.btnViewHex.Enabled = false;
            this.btnViewHex.Location = new System.Drawing.Point(109, 104);
            this.btnViewHex.Name = "btnViewHex";
            this.btnViewHex.Size = new System.Drawing.Size(98, 23);
            this.btnViewHex.TabIndex = 7;
            this.btnViewHex.Text = "View HEX";
            this.btnViewHex.UseVisualStyleBackColor = true;
            this.btnViewHex.Click += new System.EventHandler(this.BtnViewHex_Click);
            // 
            // btnFindFileNames
            // 
            this.btnFindFileNames.Enabled = false;
            this.btnFindFileNames.Location = new System.Drawing.Point(5, 133);
            this.btnFindFileNames.Name = "btnFindFileNames";
            this.btnFindFileNames.Size = new System.Drawing.Size(98, 23);
            this.btnFindFileNames.TabIndex = 8;
            this.btnFindFileNames.Text = "Find File Names";
            this.btnFindFileNames.UseVisualStyleBackColor = true;
            this.btnFindFileNames.Click += new System.EventHandler(this.BtnFindFileNames_Click);
            // 
            // btnTestFile
            // 
            this.btnTestFile.Enabled = false;
            this.btnTestFile.Location = new System.Drawing.Point(109, 133);
            this.btnTestFile.Name = "btnTestFile";
            this.btnTestFile.Size = new System.Drawing.Size(98, 23);
            this.btnTestFile.TabIndex = 9;
            this.btnTestFile.Text = "Test Hash File";
            this.btnTestFile.UseVisualStyleBackColor = true;
            this.btnTestFile.Click += new System.EventHandler(this.BtnTestFile_Click);
            // 
            // btnFileTable
            // 
            this.btnFileTable.Enabled = false;
            this.btnFileTable.Location = new System.Drawing.Point(5, 162);
            this.btnFileTable.Name = "btnFileTable";
            this.btnFileTable.Size = new System.Drawing.Size(98, 23);
            this.btnFileTable.TabIndex = 10;
            this.btnFileTable.Text = "File Table";
            this.btnFileTable.UseVisualStyleBackColor = true;
            this.btnFileTable.Click += new System.EventHandler(this.BtnFileTable_Click);
            // 
            // btnHashStatus
            // 
            this.btnHashStatus.Enabled = false;
            this.btnHashStatus.Location = new System.Drawing.Point(109, 162);
            this.btnHashStatus.Name = "btnHashStatus";
            this.btnHashStatus.Size = new System.Drawing.Size(98, 23);
            this.btnHashStatus.TabIndex = 11;
            this.btnHashStatus.Text = "Hash Status";
            this.btnHashStatus.UseVisualStyleBackColor = true;
            this.btnHashStatus.Click += new System.EventHandler(this.BtnHashStatus_Click);
            // 
            // btnHelp
            // 
            this.btnHelp.Enabled = false;
            this.btnHelp.Location = new System.Drawing.Point(5, 189);
            this.btnHelp.Name = "btnHelp";
            this.btnHelp.Size = new System.Drawing.Size(98, 23);
            this.btnHelp.TabIndex = 12;
            this.btnHelp.Text = "Help";
            this.btnHelp.UseVisualStyleBackColor = true;
            this.btnHelp.Click += new System.EventHandler(this.BtnHelp_Click);
            // 
            // backgroundWorker1
            // 
            this.backgroundWorker1.DoWork += new System.ComponentModel.DoWorkEventHandler(this.BackgroundWorker1_DoWork);
            this.backgroundWorker1.RunWorkerCompleted += new System.ComponentModel.RunWorkerCompletedEventHandler(this.BackgroundWorker1_RunWorkerCompleted);
            // 
            // backgroundWorker2
            // 
            this.backgroundWorker2.WorkerReportsProgress = true;
            this.backgroundWorker2.DoWork += new System.ComponentModel.DoWorkEventHandler(this.BackgroundWorker2_DoWork);
            this.backgroundWorker2.RunWorkerCompleted += new System.ComponentModel.RunWorkerCompletedEventHandler(this.BackgroundWorker2_RunWorkerCompleted);
            // 
            // contextMenuStrip1
            // 
            this.contextMenuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.extractToolStripMenuItem,
            this.extractByExtensionToolStripMenuItem});
            this.contextMenuStrip1.Name = "contextMenuStrip1";
            this.contextMenuStrip1.Size = new System.Drawing.Size(179, 48);
            // 
            // extractToolStripMenuItem
            // 
            this.extractToolStripMenuItem.Name = "extractToolStripMenuItem";
            this.extractToolStripMenuItem.Size = new System.Drawing.Size(178, 22);
            this.extractToolStripMenuItem.Text = "Extract";
            this.extractToolStripMenuItem.Click += new System.EventHandler(this.ExtractToolStripMenuItem_Click);
            // 
            // extractByExtensionToolStripMenuItem
            // 
            this.extractByExtensionToolStripMenuItem.Name = "extractByExtensionToolStripMenuItem";
            this.extractByExtensionToolStripMenuItem.Size = new System.Drawing.Size(178, 22);
            this.extractByExtensionToolStripMenuItem.Text = "Extract By Extension";
            this.extractByExtensionToolStripMenuItem.Click += new System.EventHandler(this.ExtractByExtensionToolStripMenuItem_Click);
            // 
            // AssetBrowser
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1187, 633);
            this.Controls.Add(this.splitContainer1);
            this.Name = "AssetBrowser";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Asset Browser";
            this.WindowState = System.Windows.Forms.FormWindowState.Maximized;
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.AssetBrowser_FormClosing);
            this.splitContainer1.Panel1.ResumeLayout(false);
            this.splitContainer1.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).EndInit();
            this.splitContainer1.ResumeLayout(false);
            this.splitContainerLeft.Panel1.ResumeLayout(false);
            this.splitContainerLeft.Panel1.PerformLayout();
            this.splitContainerLeft.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.splitContainerLeft)).EndInit();
            this.splitContainerLeft.ResumeLayout(false);
            this.splitContainerCenter.Panel1.ResumeLayout(false);
            this.splitContainerCenter.Panel1.PerformLayout();
            this.splitContainerCenter.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.splitContainerCenter)).EndInit();
            this.splitContainerCenter.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.treeItemView)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox2)).EndInit();
            this.statusStrip1.ResumeLayout(false);
            this.statusStrip1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).EndInit();
            this.splitContainerRight.Panel1.ResumeLayout(false);
            this.splitContainerRight.Panel2.ResumeLayout(false);
            this.splitContainerRight.Panel2.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainerRight)).EndInit();
            this.splitContainerRight.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView1)).EndInit();
            this.contextMenuStrip1.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.SplitContainer splitContainer1;
        private TreeViewFast.Controls.TreeViewFast treeViewList;
        private System.Windows.Forms.DataGridView dataGridView1;
        private System.Windows.Forms.PictureBox pictureBox1;
        private System.Windows.Forms.ImageList imageList1;
        private System.Windows.Forms.SplitContainer splitContainerCenter;
        private System.Windows.Forms.StatusStrip statusStrip1;
        private System.Windows.Forms.ToolStripStatusLabel toolStripStatusLabelLeft1;
        private System.Windows.Forms.SplitContainer splitContainerRight;
        private System.Windows.Forms.Label lblExtractPath;
        private System.Windows.Forms.TextBox txtExtractPath;
        private System.Windows.Forms.Button btnChooseExtract;
        private System.Windows.Forms.Button btnPreview;
        private System.Windows.Forms.Button btnAudioStop;
        private System.Windows.Forms.Button btnExtract;
        private System.Windows.Forms.Button btnSaveTxtHash;
        private System.Windows.Forms.Button btnViewRaw;
        private System.Windows.Forms.Button btnViewHex;
        private System.Windows.Forms.Button btnFindFileNames;
        private System.Windows.Forms.Button btnTestFile;
        private System.Windows.Forms.Button btnFileTable;
        private System.Windows.Forms.Button btnHashStatus;
        private System.Windows.Forms.Button btnHelp;
        private System.Windows.Forms.Integration.ElementHost elementHost1;
        private System.Windows.Forms.PictureBox pictureBox2;
        private System.Windows.Forms.ToolStripProgressBar toolStripProgressBar1;
        private System.ComponentModel.BackgroundWorker backgroundWorker1;
        private System.ComponentModel.BackgroundWorker backgroundWorker2;
        private System.Windows.Forms.TextBox txtRawView;
        private System.Windows.Forms.SplitContainer splitContainerLeft;
        private System.Windows.Forms.Button btnClearSearch;
        private System.Windows.Forms.Button btnFindNext;
        private System.Windows.Forms.Button btnSearch;
        private System.Windows.Forms.TextBox txtSearch;
        private System.Windows.Forms.ToolStripStatusLabel toolStripStatusLabelRight;
        private System.Windows.Forms.ContextMenuStrip contextMenuStrip1;
        private System.Windows.Forms.ToolStripMenuItem extractToolStripMenuItem;
        private Be.Windows.Forms.HexBox hexBox1;
        private System.Windows.Forms.ToolStripStatusLabel toolStripStatusLabelLeft2;
        private BrightIdeasSoftware.TreeListView treeItemView;
        private BrightIdeasSoftware.OLVColumn olvColumn1;
        private BrightIdeasSoftware.OLVColumn olvColumn2;
        private System.Windows.Forms.Panel renderPanel;
        private System.Windows.Forms.WebBrowser webBrowser1;
        private System.Windows.Forms.ToolStripMenuItem extractByExtensionToolStripMenuItem;
        private System.ComponentModel.BackgroundWorker backgroundWorker3;
    }
}