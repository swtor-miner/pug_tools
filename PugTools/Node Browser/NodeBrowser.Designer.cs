namespace tor_tools
{
    partial class NodeBrowser
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(NodeBrowser));
            this.splitContainer1 = new System.Windows.Forms.SplitContainer();
            this.splitContainer4 = new System.Windows.Forms.SplitContainer();
            this.btnClearSearch = new System.Windows.Forms.Button();
            this.btnFindNext = new System.Windows.Forms.Button();
            this.btnSearch = new System.Windows.Forms.Button();
            this.txtSearch = new System.Windows.Forms.TextBox();
            this.treeViewFast1 = new TreeViewFast.Controls.TreeViewFast();
            this.imageList1 = new System.Windows.Forms.ImageList(this.components);
            this.splitContainer2 = new System.Windows.Forms.SplitContainer();
            this.treeListView1 = new BrightIdeasSoftware.TreeListView();
            this.olvColumn1 = ((BrightIdeasSoftware.OLVColumn)(new BrightIdeasSoftware.OLVColumn()));
            this.olvColumn2 = ((BrightIdeasSoftware.OLVColumn)(new BrightIdeasSoftware.OLVColumn()));
            this.olvColumn3 = ((BrightIdeasSoftware.OLVColumn)(new BrightIdeasSoftware.OLVColumn()));
            this.pictureBox2 = new System.Windows.Forms.PictureBox();
            this.statusStrip1 = new System.Windows.Forms.StatusStrip();
            this.toolStripStatusLabel1 = new System.Windows.Forms.ToolStripStatusLabel();
            this.toolStripProgressBar1 = new System.Windows.Forms.ToolStripProgressBar();
            this.toolStripStatusLabel2 = new System.Windows.Forms.ToolStripStatusLabel();
            this.splitContainer3 = new System.Windows.Forms.SplitContainer();
            this.dataGridView1 = new System.Windows.Forms.DataGridView();
            this.lblListFilter = new System.Windows.Forms.Label();
            this.btnClearFilter = new System.Windows.Forms.Button();
            this.btnFileFinder = new System.Windows.Forms.Button();
            this.btnFilterList = new System.Windows.Forms.Button();
            this.btnToggleCollapse = new System.Windows.Forms.Button();
            this.txtFilter = new System.Windows.Forms.TextBox();
            this.btnChooseExtract = new System.Windows.Forms.Button();
            this.lblExtractPath = new System.Windows.Forms.Label();
            this.txtExtractPath = new System.Windows.Forms.TextBox();
            this.btnExtract = new System.Windows.Forms.Button();
            this.backgroundWorker1 = new System.ComponentModel.BackgroundWorker();
            this.backgroundWorker2 = new System.ComponentModel.BackgroundWorker();
            this.contextMenuStrip1 = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.extractToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.cmsJumpToNode = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.tsmiJumpToNode = new System.Windows.Forms.ToolStripMenuItem();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).BeginInit();
            this.splitContainer1.Panel1.SuspendLayout();
            this.splitContainer1.Panel2.SuspendLayout();
            this.splitContainer1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer4)).BeginInit();
            this.splitContainer4.Panel1.SuspendLayout();
            this.splitContainer4.Panel2.SuspendLayout();
            this.splitContainer4.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer2)).BeginInit();
            this.splitContainer2.Panel1.SuspendLayout();
            this.splitContainer2.Panel2.SuspendLayout();
            this.splitContainer2.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.treeListView1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox2)).BeginInit();
            this.statusStrip1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer3)).BeginInit();
            this.splitContainer3.Panel1.SuspendLayout();
            this.splitContainer3.Panel2.SuspendLayout();
            this.splitContainer3.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView1)).BeginInit();
            this.contextMenuStrip1.SuspendLayout();
            this.cmsJumpToNode.SuspendLayout();
            this.SuspendLayout();
            // 
            // splitContainer1
            // 
            this.splitContainer1.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.splitContainer1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitContainer1.Location = new System.Drawing.Point(0, 0);
            this.splitContainer1.Name = "splitContainer1";
            // 
            // splitContainer1.Panel1
            // 
            this.splitContainer1.Panel1.Controls.Add(this.splitContainer4);
            // 
            // splitContainer1.Panel2
            // 
            this.splitContainer1.Panel2.Controls.Add(this.splitContainer2);
            this.splitContainer1.Size = new System.Drawing.Size(1187, 633);
            this.splitContainer1.SplitterDistance = 243;
            this.splitContainer1.TabIndex = 0;
            // 
            // splitContainer4
            // 
            this.splitContainer4.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitContainer4.FixedPanel = System.Windows.Forms.FixedPanel.Panel1;
            this.splitContainer4.Location = new System.Drawing.Point(0, 0);
            this.splitContainer4.Name = "splitContainer4";
            this.splitContainer4.Orientation = System.Windows.Forms.Orientation.Horizontal;
            // 
            // splitContainer4.Panel1
            // 
            this.splitContainer4.Panel1.Controls.Add(this.btnClearSearch);
            this.splitContainer4.Panel1.Controls.Add(this.btnFindNext);
            this.splitContainer4.Panel1.Controls.Add(this.btnSearch);
            this.splitContainer4.Panel1.Controls.Add(this.txtSearch);
            // 
            // splitContainer4.Panel2
            // 
            this.splitContainer4.Panel2.Controls.Add(this.treeViewFast1);
            this.splitContainer4.Size = new System.Drawing.Size(239, 629);
            this.splitContainer4.SplitterDistance = 65;
            this.splitContainer4.TabIndex = 0;
            // 
            // btnClearSearch
            // 
            this.btnClearSearch.Enabled = false;
            this.btnClearSearch.Location = new System.Drawing.Point(121, 37);
            this.btnClearSearch.Name = "btnClearSearch";
            this.btnClearSearch.Size = new System.Drawing.Size(112, 23);
            this.btnClearSearch.TabIndex = 3;
            this.btnClearSearch.Text = "Clear";
            this.btnClearSearch.UseVisualStyleBackColor = true;
            this.btnClearSearch.Click += new System.EventHandler(this.btnClearSearch_Click);
            // 
            // btnFindNext
            // 
            this.btnFindNext.Enabled = false;
            this.btnFindNext.Location = new System.Drawing.Point(3, 37);
            this.btnFindNext.Name = "btnFindNext";
            this.btnFindNext.Size = new System.Drawing.Size(112, 23);
            this.btnFindNext.TabIndex = 2;
            this.btnFindNext.Text = "Find Next";
            this.btnFindNext.UseVisualStyleBackColor = true;
            this.btnFindNext.Click += new System.EventHandler(this.btnFindNext_Click);
            // 
            // btnSearch
            // 
            this.btnSearch.Enabled = false;
            this.btnSearch.Location = new System.Drawing.Point(158, 8);
            this.btnSearch.Name = "btnSearch";
            this.btnSearch.Size = new System.Drawing.Size(75, 23);
            this.btnSearch.TabIndex = 1;
            this.btnSearch.Text = "Search";
            this.btnSearch.UseVisualStyleBackColor = true;
            this.btnSearch.Click += new System.EventHandler(this.btnSearch_Click);
            // 
            // txtSearch
            // 
            this.txtSearch.Enabled = false;
            this.txtSearch.Location = new System.Drawing.Point(3, 11);
            this.txtSearch.Name = "txtSearch";
            this.txtSearch.Size = new System.Drawing.Size(149, 20);
            this.txtSearch.TabIndex = 0;
            // 
            // treeViewFast1
            // 
            this.treeViewFast1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.treeViewFast1.ImageIndex = 0;
            this.treeViewFast1.ImageList = this.imageList1;
            this.treeViewFast1.Location = new System.Drawing.Point(0, 3);
            this.treeViewFast1.Name = "treeViewFast1";
            this.treeViewFast1.SelectedImageIndex = 0;
            this.treeViewFast1.Size = new System.Drawing.Size(239, 557);
            this.treeViewFast1.TabIndex = 1;
            this.treeViewFast1.Visible = false;
            this.treeViewFast1.AfterSelect += new System.Windows.Forms.TreeViewEventHandler(this.treeViewFast1_AfterSelect);
            this.treeViewFast1.MouseUp += new System.Windows.Forms.MouseEventHandler(this.treeViewFast1_MouseUp);
            // 
            // imageList1
            // 
            this.imageList1.ImageStream = ((System.Windows.Forms.ImageListStreamer)(resources.GetObject("imageList1.ImageStream")));
            this.imageList1.TransparentColor = System.Drawing.Color.Transparent;
            this.imageList1.Images.SetKeyName(0, "COMPUTER.ICO");
            this.imageList1.Images.SetKeyName(1, "Folder.ico");
            this.imageList1.Images.SetKeyName(2, "textdoc.ico");
            // 
            // splitContainer2
            // 
            this.splitContainer2.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.splitContainer2.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitContainer2.Location = new System.Drawing.Point(0, 0);
            this.splitContainer2.Name = "splitContainer2";
            // 
            // splitContainer2.Panel1
            // 
            this.splitContainer2.Panel1.AutoScroll = true;
            this.splitContainer2.Panel1.Controls.Add(this.treeListView1);
            this.splitContainer2.Panel1.Controls.Add(this.pictureBox2);
            this.splitContainer2.Panel1.Controls.Add(this.statusStrip1);
            // 
            // splitContainer2.Panel2
            // 
            this.splitContainer2.Panel2.Controls.Add(this.splitContainer3);
            this.splitContainer2.Size = new System.Drawing.Size(940, 633);
            this.splitContainer2.SplitterDistance = 711;
            this.splitContainer2.TabIndex = 0;
            // 
            // treeListView1
            // 
            this.treeListView1.AllColumns.Add(this.olvColumn1);
            this.treeListView1.AllColumns.Add(this.olvColumn2);
            this.treeListView1.AllColumns.Add(this.olvColumn3);
            this.treeListView1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.treeListView1.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.olvColumn1,
            this.olvColumn2,
            this.olvColumn3});
            this.treeListView1.GridLines = true;
            this.treeListView1.Location = new System.Drawing.Point(3, 3);
            this.treeListView1.Name = "treeListView1";
            this.treeListView1.OwnerDraw = true;
            this.treeListView1.ShowGroups = false;
            this.treeListView1.Size = new System.Drawing.Size(701, 601);
            this.treeListView1.TabIndex = 4;
            this.treeListView1.UseCompatibleStateImageBehavior = false;
            this.treeListView1.UseFiltering = true;
            this.treeListView1.View = System.Windows.Forms.View.Details;
            this.treeListView1.VirtualMode = true;
            this.treeListView1.Visible = false;
            this.treeListView1.SelectedIndexChanged += new System.EventHandler(this.treeListView1_SelectedIndexChanged);
            this.treeListView1.MouseUp += new System.Windows.Forms.MouseEventHandler(this.treeListView1_MouseUp);
            // 
            // olvColumn1
            // 
            this.olvColumn1.AspectName = "displayName";
            this.olvColumn1.CellPadding = null;
            this.olvColumn1.Text = "Name";
            this.olvColumn1.Width = 144;
            // 
            // olvColumn2
            // 
            this.olvColumn2.AspectName = "type";
            this.olvColumn2.CellPadding = null;
            this.olvColumn2.Text = "Type";
            this.olvColumn2.Width = 134;
            // 
            // olvColumn3
            // 
            this.olvColumn3.AspectName = "displayValue";
            this.olvColumn3.CellPadding = null;
            this.olvColumn3.Text = "Value";
            this.olvColumn3.Width = 371;
            // 
            // pictureBox2
            // 
            this.pictureBox2.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.pictureBox2.BackColor = System.Drawing.Color.White;
            this.pictureBox2.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Center;
            this.pictureBox2.Image = global::PugTools.Properties.Resources.LoadingImage;
            this.pictureBox2.Location = new System.Drawing.Point(3, 3);
            this.pictureBox2.Name = "pictureBox2";
            this.pictureBox2.Size = new System.Drawing.Size(701, 601);
            this.pictureBox2.SizeMode = System.Windows.Forms.PictureBoxSizeMode.CenterImage;
            this.pictureBox2.TabIndex = 3;
            this.pictureBox2.TabStop = false;
            this.pictureBox2.Visible = false;
            // 
            // statusStrip1
            // 
            this.statusStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStripStatusLabel1,
            this.toolStripProgressBar1,
            this.toolStripStatusLabel2});
            this.statusStrip1.Location = new System.Drawing.Point(0, 607);
            this.statusStrip1.Name = "statusStrip1";
            this.statusStrip1.Size = new System.Drawing.Size(707, 22);
            this.statusStrip1.TabIndex = 1;
            this.statusStrip1.Text = "statusStrip1";
            // 
            // toolStripStatusLabel1
            // 
            this.toolStripStatusLabel1.Name = "toolStripStatusLabel1";
            this.toolStripStatusLabel1.Size = new System.Drawing.Size(0, 17);
            // 
            // toolStripProgressBar1
            // 
            this.toolStripProgressBar1.ForeColor = System.Drawing.Color.Lime;
            this.toolStripProgressBar1.Name = "toolStripProgressBar1";
            this.toolStripProgressBar1.Size = new System.Drawing.Size(100, 16);
            this.toolStripProgressBar1.Style = System.Windows.Forms.ProgressBarStyle.Marquee;
            this.toolStripProgressBar1.Visible = false;
            // 
            // toolStripStatusLabel2
            // 
            this.toolStripStatusLabel2.Name = "toolStripStatusLabel2";
            this.toolStripStatusLabel2.Size = new System.Drawing.Size(0, 17);
            // 
            // splitContainer3
            // 
            this.splitContainer3.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitContainer3.FixedPanel = System.Windows.Forms.FixedPanel.Panel2;
            this.splitContainer3.Location = new System.Drawing.Point(0, 0);
            this.splitContainer3.Name = "splitContainer3";
            this.splitContainer3.Orientation = System.Windows.Forms.Orientation.Horizontal;
            // 
            // splitContainer3.Panel1
            // 
            this.splitContainer3.Panel1.Controls.Add(this.dataGridView1);
            // 
            // splitContainer3.Panel2
            // 
            this.splitContainer3.Panel2.Controls.Add(this.lblListFilter);
            this.splitContainer3.Panel2.Controls.Add(this.btnClearFilter);
            this.splitContainer3.Panel2.Controls.Add(this.btnFileFinder);
            this.splitContainer3.Panel2.Controls.Add(this.btnFilterList);
            this.splitContainer3.Panel2.Controls.Add(this.btnToggleCollapse);
            this.splitContainer3.Panel2.Controls.Add(this.txtFilter);
            this.splitContainer3.Panel2.Controls.Add(this.btnChooseExtract);
            this.splitContainer3.Panel2.Controls.Add(this.lblExtractPath);
            this.splitContainer3.Panel2.Controls.Add(this.txtExtractPath);
            this.splitContainer3.Panel2.Controls.Add(this.btnExtract);
            this.splitContainer3.Size = new System.Drawing.Size(221, 629);
            this.splitContainer3.SplitterDistance = 359;
            this.splitContainer3.TabIndex = 0;
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
            this.dataGridView1.Location = new System.Drawing.Point(3, 3);
            this.dataGridView1.Name = "dataGridView1";
            this.dataGridView1.Size = new System.Drawing.Size(215, 353);
            this.dataGridView1.TabIndex = 0;
            this.dataGridView1.VirtualMode = true;
            // 
            // lblListFilter
            // 
            this.lblListFilter.AutoSize = true;
            this.lblListFilter.Enabled = false;
            this.lblListFilter.Location = new System.Drawing.Point(0, 165);
            this.lblListFilter.Name = "lblListFilter";
            this.lblListFilter.Size = new System.Drawing.Size(48, 13);
            this.lblListFilter.TabIndex = 8;
            this.lblListFilter.Text = "List Filter";
            // 
            // btnClearFilter
            // 
            this.btnClearFilter.Enabled = false;
            this.btnClearFilter.Location = new System.Drawing.Point(3, 233);
            this.btnClearFilter.Name = "btnClearFilter";
            this.btnClearFilter.Size = new System.Drawing.Size(137, 23);
            this.btnClearFilter.TabIndex = 7;
            this.btnClearFilter.Text = "Clear Filter";
            this.btnClearFilter.UseVisualStyleBackColor = true;
            this.btnClearFilter.Click += new System.EventHandler(this.btnClearFilter_Click);
            // 
            // btnFileFinder
            // 
            this.btnFileFinder.Enabled = false;
            this.btnFileFinder.Location = new System.Drawing.Point(3, 135);
            this.btnFileFinder.Name = "btnFileFinder";
            this.btnFileFinder.Size = new System.Drawing.Size(137, 23);
            this.btnFileFinder.TabIndex = 7;
            this.btnFileFinder.Text = "Run File Name Finder";
            this.btnFileFinder.UseVisualStyleBackColor = true;
            this.btnFileFinder.Click += new System.EventHandler(this.btnFileFinder_Click);
            // 
            // btnFilterList
            // 
            this.btnFilterList.Enabled = false;
            this.btnFilterList.Location = new System.Drawing.Point(3, 205);
            this.btnFilterList.Name = "btnFilterList";
            this.btnFilterList.Size = new System.Drawing.Size(137, 23);
            this.btnFilterList.TabIndex = 6;
            this.btnFilterList.Text = "Filter List";
            this.btnFilterList.UseVisualStyleBackColor = true;
            this.btnFilterList.Click += new System.EventHandler(this.btnFilterList_Click);
            // 
            // btnToggleCollapse
            // 
            this.btnToggleCollapse.Enabled = false;
            this.btnToggleCollapse.Location = new System.Drawing.Point(3, 105);
            this.btnToggleCollapse.Name = "btnToggleCollapse";
            this.btnToggleCollapse.Size = new System.Drawing.Size(137, 23);
            this.btnToggleCollapse.TabIndex = 6;
            this.btnToggleCollapse.Text = "Collapse Child Nodes";
            this.btnToggleCollapse.UseVisualStyleBackColor = true;
            this.btnToggleCollapse.Click += new System.EventHandler(this.btnToggleCollapse_Click);
            // 
            // txtFilter
            // 
            this.txtFilter.Enabled = false;
            this.txtFilter.Location = new System.Drawing.Point(3, 181);
            this.txtFilter.Name = "txtFilter";
            this.txtFilter.Size = new System.Drawing.Size(206, 20);
            this.txtFilter.TabIndex = 5;
            // 
            // btnChooseExtract
            // 
            this.btnChooseExtract.Enabled = false;
            this.btnChooseExtract.Location = new System.Drawing.Point(3, 46);
            this.btnChooseExtract.Name = "btnChooseExtract";
            this.btnChooseExtract.Size = new System.Drawing.Size(137, 23);
            this.btnChooseExtract.TabIndex = 5;
            this.btnChooseExtract.Text = "Select Extract Folder";
            this.btnChooseExtract.UseVisualStyleBackColor = true;
            this.btnChooseExtract.Click += new System.EventHandler(this.btnChooseExtract_Click);
            // 
            // lblExtractPath
            // 
            this.lblExtractPath.AutoSize = true;
            this.lblExtractPath.Enabled = false;
            this.lblExtractPath.Location = new System.Drawing.Point(0, 4);
            this.lblExtractPath.Name = "lblExtractPath";
            this.lblExtractPath.Size = new System.Drawing.Size(65, 13);
            this.lblExtractPath.TabIndex = 3;
            this.lblExtractPath.Text = "Extract Path";
            // 
            // txtExtractPath
            // 
            this.txtExtractPath.Enabled = false;
            this.txtExtractPath.Location = new System.Drawing.Point(3, 20);
            this.txtExtractPath.Name = "txtExtractPath";
            this.txtExtractPath.Size = new System.Drawing.Size(206, 20);
            this.txtExtractPath.TabIndex = 2;
            // 
            // btnExtract
            // 
            this.btnExtract.Enabled = false;
            this.btnExtract.Location = new System.Drawing.Point(3, 75);
            this.btnExtract.Name = "btnExtract";
            this.btnExtract.Size = new System.Drawing.Size(137, 23);
            this.btnExtract.TabIndex = 1;
            this.btnExtract.Text = "Extract Object";
            this.btnExtract.UseVisualStyleBackColor = true;
            this.btnExtract.Click += new System.EventHandler(this.btnExtract_Click);
            // 
            // backgroundWorker1
            // 
            this.backgroundWorker1.DoWork += new System.ComponentModel.DoWorkEventHandler(this.backgroundWorker1_DoWork);
            this.backgroundWorker1.RunWorkerCompleted += new System.ComponentModel.RunWorkerCompletedEventHandler(this.backgroundWorker1_RunWorkerCompleted);
            // 
            // backgroundWorker2
            // 
            this.backgroundWorker2.DoWork += new System.ComponentModel.DoWorkEventHandler(this.backgroundWorker2_DoWork);
            this.backgroundWorker2.RunWorkerCompleted += new System.ComponentModel.RunWorkerCompletedEventHandler(this.backgroundWorker2_RunWorkerCompleted);
            // 
            // contextMenuStrip1
            // 
            this.contextMenuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.extractToolStripMenuItem});
            this.contextMenuStrip1.Name = "contextMenuStrip1";
            this.contextMenuStrip1.Size = new System.Drawing.Size(110, 26);
            // 
            // extractToolStripMenuItem
            // 
            this.extractToolStripMenuItem.Name = "extractToolStripMenuItem";
            this.extractToolStripMenuItem.Size = new System.Drawing.Size(109, 22);
            this.extractToolStripMenuItem.Text = "Extract";
            this.extractToolStripMenuItem.Click += new System.EventHandler(this.extractToolStripMenuItem_Click);
            // 
            // cmsJumpToNode
            // 
            this.cmsJumpToNode.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.tsmiJumpToNode});
            this.cmsJumpToNode.Name = "cmsJumpToNode";
            this.cmsJumpToNode.Size = new System.Drawing.Size(153, 48);
            // 
            // tsmiJumpToNode
            // 
            this.tsmiJumpToNode.Name = "tsmiJumpToNode";
            this.tsmiJumpToNode.Size = new System.Drawing.Size(152, 22);
            this.tsmiJumpToNode.Text = "Go to Node";
            this.tsmiJumpToNode.Click += new System.EventHandler(this.tsmiJumpToNode_Click);
            // 
            // NodeBrowser
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1187, 633);
            this.Controls.Add(this.splitContainer1);
            this.Name = "NodeBrowser";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Node Browser";
            this.WindowState = System.Windows.Forms.FormWindowState.Maximized;
            this.splitContainer1.Panel1.ResumeLayout(false);
            this.splitContainer1.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).EndInit();
            this.splitContainer1.ResumeLayout(false);
            this.splitContainer4.Panel1.ResumeLayout(false);
            this.splitContainer4.Panel1.PerformLayout();
            this.splitContainer4.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer4)).EndInit();
            this.splitContainer4.ResumeLayout(false);
            this.splitContainer2.Panel1.ResumeLayout(false);
            this.splitContainer2.Panel1.PerformLayout();
            this.splitContainer2.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer2)).EndInit();
            this.splitContainer2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.treeListView1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox2)).EndInit();
            this.statusStrip1.ResumeLayout(false);
            this.statusStrip1.PerformLayout();
            this.splitContainer3.Panel1.ResumeLayout(false);
            this.splitContainer3.Panel2.ResumeLayout(false);
            this.splitContainer3.Panel2.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer3)).EndInit();
            this.splitContainer3.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView1)).EndInit();
            this.contextMenuStrip1.ResumeLayout(false);
            this.cmsJumpToNode.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.SplitContainer splitContainer1;
        private TreeViewFast.Controls.TreeViewFast treeViewFast1;
        private System.Windows.Forms.DataGridView dataGridView1;
        private System.Windows.Forms.ImageList imageList1;
        private System.Windows.Forms.SplitContainer splitContainer2;
        private System.Windows.Forms.StatusStrip statusStrip1;
        private System.Windows.Forms.ToolStripStatusLabel toolStripStatusLabel1;
        private System.Windows.Forms.SplitContainer splitContainer3;
        private System.Windows.Forms.Button btnExtract;
        private System.Windows.Forms.PictureBox pictureBox2;
        private System.Windows.Forms.ToolStripProgressBar toolStripProgressBar1;
        private System.Windows.Forms.Label lblExtractPath;
        private System.Windows.Forms.TextBox txtExtractPath;
        private System.ComponentModel.BackgroundWorker backgroundWorker1;
        private System.ComponentModel.BackgroundWorker backgroundWorker2;
        private System.Windows.Forms.SplitContainer splitContainer4;
        private System.Windows.Forms.Button btnClearSearch;
        private System.Windows.Forms.Button btnFindNext;
        private System.Windows.Forms.Button btnSearch;
        private System.Windows.Forms.TextBox txtSearch;
        private System.Windows.Forms.ToolStripStatusLabel toolStripStatusLabel2;
        private System.Windows.Forms.ContextMenuStrip contextMenuStrip1;
        private System.Windows.Forms.ToolStripMenuItem extractToolStripMenuItem;
        private System.Windows.Forms.Button btnChooseExtract;
        private BrightIdeasSoftware.TreeListView treeListView1;
        private BrightIdeasSoftware.OLVColumn olvColumn1;
        private BrightIdeasSoftware.OLVColumn olvColumn2;
        private BrightIdeasSoftware.OLVColumn olvColumn3;
        private System.Windows.Forms.Button btnToggleCollapse;
        private System.Windows.Forms.Button btnFileFinder;
        private System.Windows.Forms.Button btnClearFilter;
        private System.Windows.Forms.Button btnFilterList;
        private System.Windows.Forms.TextBox txtFilter;
        private System.Windows.Forms.Label lblListFilter;
        private System.Windows.Forms.ContextMenuStrip cmsJumpToNode;
        private System.Windows.Forms.ToolStripMenuItem tsmiJumpToNode;
    }
}