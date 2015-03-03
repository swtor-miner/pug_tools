namespace TreeViewFast
{
    partial class Demo
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
            this.btnLoad = new System.Windows.Forms.Button();
            this.lblItemCount = new System.Windows.Forms.Label();
            this.txtItemCount = new System.Windows.Forms.TextBox();
            this.tableMainLayout = new System.Windows.Forms.TableLayoutPanel();
            this.pnlLoad = new System.Windows.Forms.Panel();
            this.lblFeedbackFast = new System.Windows.Forms.Label();
            this.lblFeedbackStandard = new System.Windows.Forms.Label();
            this.myTreeView = new System.Windows.Forms.TreeView();
            this.myTreeViewFast = new TreeViewFast.Controls.TreeViewFast();
            this.tableMainLayout.SuspendLayout();
            this.pnlLoad.SuspendLayout();
            this.SuspendLayout();
            // 
            // btnLoad
            // 
            this.btnLoad.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnLoad.Location = new System.Drawing.Point(735, 10);
            this.btnLoad.Name = "btnLoad";
            this.btnLoad.Size = new System.Drawing.Size(75, 23);
            this.btnLoad.TabIndex = 1;
            this.btnLoad.Text = "Load";
            this.btnLoad.UseVisualStyleBackColor = true;
            this.btnLoad.Click += new System.EventHandler(this.btnLoad_Click);
            // 
            // lblItemCount
            // 
            this.lblItemCount.AutoSize = true;
            this.lblItemCount.Location = new System.Drawing.Point(10, 13);
            this.lblItemCount.Name = "lblItemCount";
            this.lblItemCount.Size = new System.Drawing.Size(121, 13);
            this.lblItemCount.TabIndex = 2;
            this.lblItemCount.Text = "Number of items to load:";
            // 
            // txtItemCount
            // 
            this.txtItemCount.Location = new System.Drawing.Point(135, 10);
            this.txtItemCount.Name = "txtItemCount";
            this.txtItemCount.Size = new System.Drawing.Size(100, 20);
            this.txtItemCount.TabIndex = 3;
            this.txtItemCount.Text = "5000";
            // 
            // tableMainLayout
            // 
            this.tableMainLayout.ColumnCount = 2;
            this.tableMainLayout.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tableMainLayout.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tableMainLayout.Controls.Add(this.pnlLoad, 0, 0);
            this.tableMainLayout.Controls.Add(this.lblFeedbackFast, 1, 1);
            this.tableMainLayout.Controls.Add(this.myTreeViewFast, 1, 2);
            this.tableMainLayout.Controls.Add(this.lblFeedbackStandard, 0, 1);
            this.tableMainLayout.Controls.Add(this.myTreeView, 0, 2);
            this.tableMainLayout.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableMainLayout.Location = new System.Drawing.Point(0, 0);
            this.tableMainLayout.Name = "tableMainLayout";
            this.tableMainLayout.RowCount = 3;
            this.tableMainLayout.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 50F));
            this.tableMainLayout.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 40F));
            this.tableMainLayout.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableMainLayout.Size = new System.Drawing.Size(819, 578);
            this.tableMainLayout.TabIndex = 5;
            // 
            // pnlLoad
            // 
            this.tableMainLayout.SetColumnSpan(this.pnlLoad, 2);
            this.pnlLoad.Controls.Add(this.lblItemCount);
            this.pnlLoad.Controls.Add(this.btnLoad);
            this.pnlLoad.Controls.Add(this.txtItemCount);
            this.pnlLoad.Dock = System.Windows.Forms.DockStyle.Fill;
            this.pnlLoad.Location = new System.Drawing.Point(3, 3);
            this.pnlLoad.Name = "pnlLoad";
            this.pnlLoad.Size = new System.Drawing.Size(813, 44);
            this.pnlLoad.TabIndex = 6;
            // 
            // lblFeedbackFast
            // 
            this.lblFeedbackFast.Dock = System.Windows.Forms.DockStyle.Fill;
            this.lblFeedbackFast.Location = new System.Drawing.Point(412, 50);
            this.lblFeedbackFast.Name = "lblFeedbackFast";
            this.lblFeedbackFast.Size = new System.Drawing.Size(404, 40);
            this.lblFeedbackFast.TabIndex = 7;
            this.lblFeedbackFast.Text = "TreeView Fast";
            this.lblFeedbackFast.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // lblFeedbackStandard
            // 
            this.lblFeedbackStandard.Dock = System.Windows.Forms.DockStyle.Fill;
            this.lblFeedbackStandard.Location = new System.Drawing.Point(3, 50);
            this.lblFeedbackStandard.Name = "lblFeedbackStandard";
            this.lblFeedbackStandard.Size = new System.Drawing.Size(403, 40);
            this.lblFeedbackStandard.TabIndex = 8;
            this.lblFeedbackStandard.Text = "TreeView Standard";
            this.lblFeedbackStandard.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // myTreeView
            // 
            this.myTreeView.Dock = System.Windows.Forms.DockStyle.Fill;
            this.myTreeView.Location = new System.Drawing.Point(3, 93);
            this.myTreeView.Name = "myTreeView";
            this.myTreeView.Size = new System.Drawing.Size(403, 482);
            this.myTreeView.TabIndex = 9;
            // 
            // myTreeViewFast
            // 
            this.myTreeViewFast.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.myTreeViewFast.Location = new System.Drawing.Point(412, 93);
            this.myTreeViewFast.Name = "myTreeViewFast";
            this.myTreeViewFast.Size = new System.Drawing.Size(404, 482);
            this.myTreeViewFast.TabIndex = 0;
            // 
            // Demo
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(819, 578);
            this.Controls.Add(this.tableMainLayout);
            this.Name = "Demo";
            this.Text = "Tree load demo";
            this.tableMainLayout.ResumeLayout(false);
            this.pnlLoad.ResumeLayout(false);
            this.pnlLoad.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private Controls.TreeViewFast myTreeViewFast;
        private System.Windows.Forms.Button btnLoad;
        private System.Windows.Forms.Label lblItemCount;
        private System.Windows.Forms.TextBox txtItemCount;
        private System.Windows.Forms.TableLayoutPanel tableMainLayout;
        private System.Windows.Forms.Panel pnlLoad;
        private System.Windows.Forms.Label lblFeedbackFast;
        private System.Windows.Forms.Label lblFeedbackStandard;
        private System.Windows.Forms.TreeView myTreeView;

    }
}

