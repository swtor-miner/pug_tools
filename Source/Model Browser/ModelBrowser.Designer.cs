namespace PugTools
{
    partial class ModelBrowser
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(ModelBrowser));
            this.splitContainerWindow = new System.Windows.Forms.SplitContainer();
            this.splitContainerLeft = new System.Windows.Forms.SplitContainer();
            this.treeViewList = new TreeViewFast.Controls.TreeViewFast();
            this.btnStopRender = new System.Windows.Forms.Button();
            this.btnHideData = new System.Windows.Forms.Button();
            this.btnExport = new System.Windows.Forms.Button();
            this.btnModelBrowserHelp = new System.Windows.Forms.Button();
            this.splitContainerCenter = new System.Windows.Forms.SplitContainer();
            this.renderPanel = new System.Windows.Forms.Panel();
            this.pictureBox1 = new System.Windows.Forms.PictureBox();
            this.splitContainerRight = new System.Windows.Forms.SplitContainer();
            this.tvfDataViewer = new TreeViewFast.Controls.TreeViewFast();
            this.dgvDataViewer = new System.Windows.Forms.DataGridView();
            this.statusStrip1 = new System.Windows.Forms.StatusStrip();
            this.toolStripStatusLabel1 = new System.Windows.Forms.ToolStripStatusLabel();
            this.toolStripProgressBar1 = new System.Windows.Forms.ToolStripProgressBar();
            this.backgroundWorker1 = new System.ComponentModel.BackgroundWorker();
            this.contextMenuStrip1 = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.toolStripMenuItemViewAll = new System.Windows.Forms.ToolStripMenuItem();
            this.contextMenuStrip2 = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.toolStripMenuItem1 = new System.Windows.Forms.ToolStripMenuItem();
            this.BodyTypeStrip = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.contextMenuStrip4 = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.toolStripMenuItem2 = new System.Windows.Forms.ToolStripMenuItem();
            this.imageList1 = new System.Windows.Forms.ImageList(this.components);
            ((System.ComponentModel.ISupportInitialize)(this.splitContainerWindow)).BeginInit();
            this.splitContainerWindow.Panel1.SuspendLayout();
            this.splitContainerWindow.Panel2.SuspendLayout();
            this.splitContainerWindow.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainerLeft)).BeginInit();
            this.splitContainerLeft.Panel1.SuspendLayout();
            this.splitContainerLeft.Panel2.SuspendLayout();
            this.splitContainerLeft.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainerCenter)).BeginInit();
            this.splitContainerCenter.Panel1.SuspendLayout();
            this.splitContainerCenter.Panel2.SuspendLayout();
            this.splitContainerCenter.SuspendLayout();
            this.renderPanel.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainerRight)).BeginInit();
            this.splitContainerRight.Panel1.SuspendLayout();
            this.splitContainerRight.Panel2.SuspendLayout();
            this.splitContainerRight.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgvDataViewer)).BeginInit();
            this.statusStrip1.SuspendLayout();
            this.contextMenuStrip1.SuspendLayout();
            this.contextMenuStrip2.SuspendLayout();
            this.contextMenuStrip4.SuspendLayout();
            this.SuspendLayout();
            // 
            // splitContainerWindow
            // 
            this.splitContainerWindow.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.splitContainerWindow.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitContainerWindow.Location = new System.Drawing.Point(0, 0);
            this.splitContainerWindow.Name = "splitContainerWindow";
            this.splitContainerWindow.Size = new System.Drawing.Size(960, 615);
            this.splitContainerWindow.SplitterDistance = 160;
            this.splitContainerWindow.TabStop = false;
            // 
            // splitContainerWindow.Panel1
            // 
            this.splitContainerWindow.Panel1.Controls.Add(this.splitContainerLeft);
            // 
            // splitContainerLeft
            // 
            this.splitContainerLeft.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.splitContainerLeft.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitContainerLeft.FixedPanel = System.Windows.Forms.FixedPanel.Panel2;
            this.splitContainerLeft.Location = new System.Drawing.Point(0, 0);
            this.splitContainerLeft.Name = "splitContainerLeft";
            this.splitContainerLeft.Orientation = System.Windows.Forms.Orientation.Horizontal;
            this.splitContainerLeft.Size = new System.Drawing.Size(160, 561);
            this.splitContainerLeft.SplitterDistance = 491;
            this.splitContainerLeft.TabStop = false;
            // 
            // splitContainerLeft.Panel1
            // 
            this.splitContainerLeft.Panel1.Controls.Add(this.treeViewList);
            // 
            // treeViewList
            // 
            this.treeViewList.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
            | System.Windows.Forms.AnchorStyles.Left)
            | System.Windows.Forms.AnchorStyles.Right)));
            this.treeViewList.Enabled = false;
            this.treeViewList.ImageIndex = 0;
            this.treeViewList.ImageList = this.imageList1;
            this.treeViewList.Location = new System.Drawing.Point(3, 3);
            this.treeViewList.Name = "treeViewList";
            this.treeViewList.SelectedImageIndex = 0;
            this.treeViewList.Size = new System.Drawing.Size(151, 482);
            this.treeViewList.TabIndex = 0;
            this.treeViewList.AfterSelect += new System.Windows.Forms.TreeViewEventHandler(this.TreeViewList_AfterSelect);
            this.treeViewList.MouseUp += new System.Windows.Forms.MouseEventHandler(this.TreeViewList_MouseUp);
            // 
            // splitContainerLeft.Panel2
            // 
            this.splitContainerLeft.Panel2.Controls.Add(this.btnStopRender);
            this.splitContainerLeft.Panel2.Controls.Add(this.btnHideData);
            this.splitContainerLeft.Panel2.Controls.Add(this.btnExport);
            this.splitContainerLeft.Panel2.Controls.Add(this.btnModelBrowserHelp);
            // 
            // btnStopRender
            // 
            this.btnStopRender.Click += new System.EventHandler(this.BtnStopRender_Click);
            this.btnStopRender.Enabled = false;
            this.btnStopRender.Location = new System.Drawing.Point(5, 6);
            this.btnStopRender.Name = "btnStopRender";
            this.btnStopRender.Size = new System.Drawing.Size(108, 23);
            this.btnStopRender.TabIndex = 0;
            this.btnStopRender.Text = "Stop Render";
            this.btnStopRender.UseVisualStyleBackColor = true;
            // 
            // btnHideData
            // 
            this.btnHideData.Click += new System.EventHandler(this.BtnHideData_Click);
            this.btnHideData.Enabled = false;
            this.btnHideData.Location = new System.Drawing.Point(118, 6);
            this.btnHideData.Name = "btnHideData";
            this.btnHideData.Size = new System.Drawing.Size(108, 23);
            this.btnHideData.TabIndex = 1;
            this.btnHideData.Text = "Hide Data Viewer";
            this.btnHideData.UseVisualStyleBackColor = true;
            // 
            // btnExport
            // 
            this.btnExport.Click += new System.EventHandler(this.BtnExport_Click);
            this.btnExport.Enabled = false;
            this.btnExport.Location = new System.Drawing.Point(5, 35);
            this.btnExport.Name = "btnExport";
            this.btnExport.Size = new System.Drawing.Size(108, 23);
            this.btnExport.TabIndex = 2;
            this.btnExport.Text = "Export";
            this.btnExport.UseVisualStyleBackColor = true;
            // 
            // btnModelBrowserHelp
            // 
            this.btnModelBrowserHelp.Click += new System.EventHandler(this.BtnModelBrowserHelp_Click);
            this.btnModelBrowserHelp.Enabled = false;
            this.btnModelBrowserHelp.Location = new System.Drawing.Point(118, 35);
            this.btnModelBrowserHelp.Name = "btnModelBrowserHelp";
            this.btnModelBrowserHelp.Size = new System.Drawing.Size(108, 23);
            this.btnModelBrowserHelp.TabIndex = 3;
            this.btnModelBrowserHelp.Text = "Help";
            this.btnModelBrowserHelp.UseVisualStyleBackColor = true;
            // 
            // splitContainerWindow.Panel2
            // 
            this.splitContainerWindow.Panel2.Controls.Add(this.splitContainerCenter);
            this.splitContainerWindow.Panel2.Controls.Add(this.statusStrip1);
            // 
            // splitContainerCenter
            // 
            this.splitContainerCenter.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
            | System.Windows.Forms.AnchorStyles.Left)
            | System.Windows.Forms.AnchorStyles.Right)));
            this.splitContainerCenter.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.splitContainerCenter.Location = new System.Drawing.Point(3, 3);
            this.splitContainerCenter.Name = "splitContainerCenter";
            this.splitContainerCenter.Size = new System.Drawing.Size(816, 583);
            this.splitContainerCenter.SplitterDistance = 619;
            this.splitContainerCenter.TabStop = false;
            // 
            // splitContainerCenter.Panel1
            // 
            this.splitContainerCenter.Panel1.Controls.Add(this.renderPanel);
            // 
            // renderPanel
            // 
            this.renderPanel.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
            | System.Windows.Forms.AnchorStyles.Left)
            | System.Windows.Forms.AnchorStyles.Right)));
            this.renderPanel.Controls.Add(this.pictureBox1);
            this.renderPanel.Location = new System.Drawing.Point(3, 3);
            this.renderPanel.MouseHover += new System.EventHandler(this.RenderPanel_MouseHover);
            this.renderPanel.Name = "renderPanel";
            this.renderPanel.Resize += new System.EventHandler(this.RenderPanel_Resize);
            this.renderPanel.Size = new System.Drawing.Size(609, 573);
            this.renderPanel.TabStop = false;
            // 
            // pictureBox1
            // 
            this.pictureBox1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
            | System.Windows.Forms.AnchorStyles.Left)
            | System.Windows.Forms.AnchorStyles.Right)));
            this.pictureBox1.BackColor = System.Drawing.Color.White;
            this.pictureBox1.Image = global::PugTools.Properties.Resources.LoadingImage;
            this.pictureBox1.Location = new System.Drawing.Point(3, 3);
            this.pictureBox1.Name = "pictureBox1";
            this.pictureBox1.Size = new System.Drawing.Size(604, 568);
            this.pictureBox1.SizeMode = System.Windows.Forms.PictureBoxSizeMode.CenterImage;
            this.pictureBox1.TabStop = false;
            // 
            // splitContainerCenter.Panel2
            // 
            this.splitContainerCenter.Panel2.Controls.Add(this.splitContainerRight);
            // 
            // splitContainerRight
            // 
            this.splitContainerRight.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.splitContainerRight.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitContainerRight.Location = new System.Drawing.Point(0, 0);
            this.splitContainerRight.Name = "splitContainerRight";
            this.splitContainerRight.Orientation = System.Windows.Forms.Orientation.Horizontal;
            this.splitContainerRight.Size = new System.Drawing.Size(143, 583);
            this.splitContainerRight.SplitterDistance = 343;
            this.splitContainerRight.TabStop = false;
            // 
            // splitContainerRight.Panel1
            // 
            this.splitContainerRight.Panel1.Controls.Add(this.tvfDataViewer);
            // 
            // tvfDataViewer
            // 
            this.tvfDataViewer.AfterSelect += new System.Windows.Forms.TreeViewEventHandler(this.TvfDataViewer_AfterSelect);
            this.tvfDataViewer.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
            | System.Windows.Forms.AnchorStyles.Left)
            | System.Windows.Forms.AnchorStyles.Right)));
            this.tvfDataViewer.Enabled = false;
            this.tvfDataViewer.Location = new System.Drawing.Point(3, 3);
            this.tvfDataViewer.MouseUp += new System.Windows.Forms.MouseEventHandler(this.TvfDataViewer_MouseUp);
            this.tvfDataViewer.Name = "tvfDataViewer";
            this.tvfDataViewer.Size = new System.Drawing.Size(109, 334);
            this.tvfDataViewer.TabIndex = 0;
            // 
            // splitContainerRight.Panel2
            // 
            this.splitContainerRight.Panel2.Controls.Add(this.dgvDataViewer);
            // 
            // dgvDataViewer
            // 
            this.dgvDataViewer.AllowUserToAddRows = false;
            this.dgvDataViewer.AllowUserToDeleteRows = false;
            this.dgvDataViewer.AllowUserToResizeRows = false;
            this.dgvDataViewer.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
            | System.Windows.Forms.AnchorStyles.Left)
            | System.Windows.Forms.AnchorStyles.Right)));
            this.dgvDataViewer.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgvDataViewer.Enabled = false;
            this.dgvDataViewer.Location = new System.Drawing.Point(3, 3);
            this.dgvDataViewer.Name = "dgvDataViewer";
            this.dgvDataViewer.RowHeadersVisible = false;
            this.dgvDataViewer.Size = new System.Drawing.Size(109, 226);
            this.dgvDataViewer.TabIndex = 0;
            // 
            // statusStrip1
            // 
            this.statusStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStripStatusLabel1,
            this.toolStripProgressBar1});
            this.statusStrip1.Location = new System.Drawing.Point(0, 589);
            this.statusStrip1.Name = "statusStrip1";
            this.statusStrip1.Size = new System.Drawing.Size(822, 22);
            this.statusStrip1.TabStop = false;
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
            // 
            // backgroundWorker1
            // 
            this.backgroundWorker1.DoWork += new System.ComponentModel.DoWorkEventHandler(this.BackgroundWorker1_DoWork);
            this.backgroundWorker1.RunWorkerCompleted += new System.ComponentModel.RunWorkerCompletedEventHandler(this.BackgroundWorker1_RunWorkerCompleted);
            // 
            // contextMenuStrip1
            // 
            this.contextMenuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStripMenuItemViewAll});
            this.contextMenuStrip1.Name = "contextMenuStrip1";
            this.contextMenuStrip1.Size = new System.Drawing.Size(149, 26);
            // 
            // toolStripMenuItemViewAll
            // 
            this.toolStripMenuItemViewAll.Name = "toolStripMenuItemViewAll";
            this.toolStripMenuItemViewAll.Size = new System.Drawing.Size(148, 22);
            this.toolStripMenuItemViewAll.Text = "View All Items";
            this.toolStripMenuItemViewAll.Click += new System.EventHandler(this.ToolStripMenuItemViewAll_Click);
            // 
            // contextMenuStrip2
            // 
            this.contextMenuStrip2.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStripMenuItem1});
            this.contextMenuStrip2.Name = "contextMenuStrip2";
            this.contextMenuStrip2.Size = new System.Drawing.Size(152, 26);
            // 
            // toolStripMenuItem1
            // 
            this.toolStripMenuItem1.Name = "toolStripMenuItem1";
            this.toolStripMenuItem1.Size = new System.Drawing.Size(151, 22);
            this.toolStripMenuItem1.Text = "Toggle Render";
            this.toolStripMenuItem1.Click += new System.EventHandler(this.ToolStripMenuItem1_Click);
            // 
            // BodyTypeStrip
            // 
            this.BodyTypeStrip.Name = "contextMenuStrip3";
            this.BodyTypeStrip.Size = new System.Drawing.Size(61, 4);
            // 
            // contextMenuStrip4
            // 
            this.contextMenuStrip4.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStripMenuItem2});
            this.contextMenuStrip4.Name = "contextMenuStrip4";
            this.contextMenuStrip4.Size = new System.Drawing.Size(146, 26);
            // 
            // toolStripMenuItem2
            // 
            this.toolStripMenuItem2.Name = "toolStripMenuItem2";
            this.toolStripMenuItem2.Size = new System.Drawing.Size(145, 22);
            this.toolStripMenuItem2.Text = "View Material";
            this.toolStripMenuItem2.Click += new System.EventHandler(this.ToolStripMenuItem2_Click);
            // 
            // imageList1
            // 
            this.imageList1.ImageStream = ((System.Windows.Forms.ImageListStreamer)(resources.GetObject("imageList1.ImageStream")));
            this.imageList1.TransparentColor = System.Drawing.Color.Transparent;
            this.imageList1.Images.SetKeyName(0, "COMPUTER.ICO");
            this.imageList1.Images.SetKeyName(1, "Folder.ico");
            this.imageList1.Images.SetKeyName(2, "textdoc.ico");
            // 
            // ModelBrowser
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(960, 615);
            this.Controls.Add(this.splitContainerWindow);
            this.Name = "ModelBrowser";
            this.Text = "Model Browser";
            this.WindowState = System.Windows.Forms.FormWindowState.Maximized;
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.ModelBrowser_FormClosing);
            this.splitContainerWindow.Panel1.ResumeLayout(false);
            this.splitContainerWindow.Panel2.ResumeLayout(false);
            this.splitContainerWindow.Panel2.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainerWindow)).EndInit();
            this.splitContainerWindow.ResumeLayout(false);
            this.splitContainerLeft.Panel1.ResumeLayout(false);
            this.splitContainerLeft.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.splitContainerLeft)).EndInit();
            this.splitContainerLeft.ResumeLayout(false);
            this.splitContainerCenter.Panel1.ResumeLayout(false);
            this.splitContainerCenter.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.splitContainerCenter)).EndInit();
            this.splitContainerCenter.ResumeLayout(false);
            this.renderPanel.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).EndInit();
            this.splitContainerRight.Panel1.ResumeLayout(false);
            this.splitContainerRight.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.splitContainerRight)).EndInit();
            this.splitContainerRight.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.dgvDataViewer)).EndInit();
            this.statusStrip1.ResumeLayout(false);
            this.statusStrip1.PerformLayout();
            this.contextMenuStrip1.ResumeLayout(false);
            this.contextMenuStrip2.ResumeLayout(false);
            this.contextMenuStrip4.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.SplitContainer splitContainerWindow;
        private System.Windows.Forms.SplitContainer splitContainerLeft;
        private TreeViewFast.Controls.TreeViewFast treeViewList;
        private System.Windows.Forms.Button btnStopRender;
        private System.Windows.Forms.Button btnHideData;
        private System.Windows.Forms.Button btnExport;
        private System.Windows.Forms.Button btnModelBrowserHelp;
        private System.Windows.Forms.SplitContainer splitContainerCenter;
        private System.Windows.Forms.Panel renderPanel;
        private System.Windows.Forms.PictureBox pictureBox1;
        private System.Windows.Forms.SplitContainer splitContainerRight;
        private TreeViewFast.Controls.TreeViewFast tvfDataViewer;
        private System.Windows.Forms.DataGridView dgvDataViewer;
        private System.Windows.Forms.StatusStrip statusStrip1;
        private System.Windows.Forms.ToolStripStatusLabel toolStripStatusLabel1;
        private System.Windows.Forms.ToolStripProgressBar toolStripProgressBar1;
        private System.ComponentModel.BackgroundWorker backgroundWorker1;
        private System.Windows.Forms.ContextMenuStrip contextMenuStrip1;
        private System.Windows.Forms.ToolStripMenuItem toolStripMenuItemViewAll;
        private System.Windows.Forms.ContextMenuStrip contextMenuStrip2;
        private System.Windows.Forms.ToolStripMenuItem toolStripMenuItem1;
        private System.Windows.Forms.ContextMenuStrip BodyTypeStrip;
        private System.Windows.Forms.ContextMenuStrip contextMenuStrip4;
        private System.Windows.Forms.ToolStripMenuItem toolStripMenuItem2;
        private System.Windows.Forms.ImageList imageList1;
    }
}