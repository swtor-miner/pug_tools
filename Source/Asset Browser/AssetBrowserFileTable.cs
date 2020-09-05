using System;
using System.Collections.Generic;
using System.Collections;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;
using System.Xml;
using System.Windows.Forms.Integration;
using GomLib;
using TorLib;
using nsHashDictionary;
using DevIL;
using Be.HexEditor;
using Be.Windows.Forms;
using nsHasherFunctions;
using ColorCode;
using NAudio;
using NAudio.Wave;
using NVorbis;
using NVorbis.NAudioSupport;
using NVorbis.Ogg;
using System.Diagnostics;
using System.Windows.Forms.Layout;
using System.Runtime.Remoting;

namespace PugTools
{
    public partial class AssetBrowserFileTable : Form
    {
        private readonly HashDictionaryInstance hashData;
        private static readonly Dictionary<string, List<TorLib.FileInfo>> dictionaries = new Dictionary<string, List<TorLib.FileInfo>>();
        private Dictionary<string, List<TorLib.FileInfo>> fileDict = dictionaries;
        private Dictionary<string, ArchTreeListItem> assetDict = new Dictionary<string, ArchTreeListItem>();

        private readonly ArrayList rootList = new ArrayList();
        public bool _closing = false;
        private static readonly Hasher hasher1 = new Hasher(Hasher.HasherType.TOR);
        private readonly Hasher hasher = hasher1;
        private static readonly HashSet<string> hashSets = new HashSet<string>();
        private readonly HashSet<string> foundFiles = hashSets;

        private Archive currentArchive;
        private Dictionary<string, int> currentArchDetails = new Dictionary<string, int>();

        public uint filesTotal = 0;
        public uint filesNamed = 0;
        public uint filesUnnamed = 0;
        public uint filesMissing = 0;

        public override bool AllowDrop { get => base.AllowDrop; set => base.AllowDrop = value; }
        public override AnchorStyles Anchor { get => base.Anchor; set => base.Anchor = value; }
        public override Point AutoScrollOffset { get => base.AutoScrollOffset; set => base.AutoScrollOffset = value; }

        public override LayoutEngine LayoutEngine => base.LayoutEngine;

        public override System.Drawing.Image BackgroundImage { get => base.BackgroundImage; set => base.BackgroundImage = value; }
        public override ImageLayout BackgroundImageLayout { get => base.BackgroundImageLayout; set => base.BackgroundImageLayout = value; }

        protected override bool CanRaiseEvents => base.CanRaiseEvents;

        public override ContextMenu ContextMenu { get => base.ContextMenu; set => base.ContextMenu = value; }
        public override ContextMenuStrip ContextMenuStrip { get => base.ContextMenuStrip; set => base.ContextMenuStrip = value; }
        public override Cursor Cursor { get => base.Cursor; set => base.Cursor = value; }

        protected override Cursor DefaultCursor => base.DefaultCursor;

        protected override Padding DefaultMargin => base.DefaultMargin;

        protected override Size DefaultMaximumSize => base.DefaultMaximumSize;

        protected override Size DefaultMinimumSize => base.DefaultMinimumSize;

        protected override Padding DefaultPadding => base.DefaultPadding;

        public override DockStyle Dock { get => base.Dock; set => base.Dock = value; }
        protected override bool DoubleBuffered { get => base.DoubleBuffered; set => base.DoubleBuffered = value; }

        public override bool Focused => base.Focused;

        public override Font Font { get => base.Font; set => base.Font = value; }
        public override System.Drawing.Color ForeColor { get => base.ForeColor; set => base.ForeColor = value; }
        public override RightToLeft RightToLeft { get => base.RightToLeft; set => base.RightToLeft = value; }

        protected override bool ScaleChildren => base.ScaleChildren;

        public override ISite Site { get => base.Site; set => base.Site = value; }

        protected override bool ShowKeyboardCues => base.ShowKeyboardCues;

        protected override bool ShowFocusCues => base.ShowFocusCues;

        protected override ImeMode ImeModeBase { get => base.ImeModeBase; set => base.ImeModeBase = value; }

        public override Rectangle DisplayRectangle => base.DisplayRectangle;

        public override BindingContext BindingContext { get => base.BindingContext; set => base.BindingContext = value; }

        protected override bool CanEnableIme => base.CanEnableIme;

        public override Size AutoScaleBaseSize { get => base.AutoScaleBaseSize; set => base.AutoScaleBaseSize = value; }
        public override bool AutoScroll { get => base.AutoScroll; set => base.AutoScroll = value; }
        public override bool AutoSize { get => base.AutoSize; set => base.AutoSize = value; }
        public override AutoValidate AutoValidate { get => base.AutoValidate; set => base.AutoValidate = value; }
        public override System.Drawing.Color BackColor { get => base.BackColor; set => base.BackColor = value; }

        protected override CreateParams CreateParams => base.CreateParams;

        protected override ImeMode DefaultImeMode => base.DefaultImeMode;

        protected override Size DefaultSize => base.DefaultSize;

        public override Size MaximumSize { get => base.MaximumSize; set => base.MaximumSize = value; }
        public override Size MinimumSize { get => base.MinimumSize; set => base.MinimumSize = value; }
        public override bool RightToLeftLayout { get => base.RightToLeftLayout; set => base.RightToLeftLayout = value; }

        protected override bool ShowWithoutActivation => base.ShowWithoutActivation;

        public override string Text { get => base.Text; set => base.Text = value; }

        public Hasher Hasher => hasher;

        public HashSet<string> FoundFiles => foundFiles;

        public AssetBrowserFileTable()
        {
            InitializeComponent();
            FormClosed += AssetBrowser_FormClosed;

            hashData = HashDictionaryInstance.Instance;
            if (!hashData.Loaded)
            {
                hashData.Load();
            }

            ShowLoader();
            treeListView1.CanExpandGetter = delegate (object x)
            {
                if (x.GetType() == typeof(NodeListItem))
                    return (((NodeListItem)x).children.Count > 0);
                return false;
            };
            treeListView1.ChildrenGetter = delegate (object x)
            {
                if (x.GetType() == typeof(NodeListItem))
                    return new ArrayList(((NodeListItem)x).children);
                return null;
            };
            backgroundWorker1.RunWorkerAsync();
        }

        #region Background Wokers Methods

        private void BackgroundWorker1_DoWork(object sender, DoWorkEventArgs e)
        {
            if (_closing)
            {
                return;
            }

            Assets currentAssets = AssetHandler.Instance.GetCurrentAssets();
            foreach (var lib in currentAssets.libraries)
            {
                string path = lib.Location;
                if (!lib.Loaded)
                    lib.Load();
                if (lib.archives.Count > 0)
                {
                    foreach (var arch in lib.archives)
                    {
                        string name = arch.Value.FileName.Split('\\').Last().Replace(".tor", "");
                        ArchTreeListItem assetArch = new ArchTreeListItem("/root/" + name, "/root", name, arch.Value);
                        assetDict.Add("/root/" + lib.Name, assetArch);
                    }
                }

            }
            assetDict.Add("/root", new ArchTreeListItem("/root", string.Empty, "Root", null));

        }

        private void BackgroundWorker1_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            if (_closing)
            {
                return;
            }

            toolStripStatusLabel1.Text = "Loading Tree View Items ...";
            Func<ArchTreeListItem, string> getId = (x => x.Id);
            Func<ArchTreeListItem, string> getParentId = (x => x.parentId);
            Func<ArchTreeListItem, string> getDisplayName = (x => x.displayName);
            treeViewFast1.BeginUpdate();
            treeViewFast1.LoadItems<ArchTreeListItem>(assetDict, getId, getParentId, getDisplayName);
            treeViewFast1.EndUpdate();
            toolStripStatusLabel1.Text = "Loading Complete";
            treeViewFast1.Visible = true;
            HideLoader();
            EnableUI();
            if (treeViewFast1.Nodes.Count > 0) treeViewFast1.Nodes[0].Expand();
        }
        #endregion

        #region UI Methods
        private async void TreeViewFast1_AfterSelect(object sender, TreeViewEventArgs e)
        {
            TreeNode node = treeViewFast1.SelectedNode;
            ArchTreeListItem obj = (ArchTreeListItem)node.Tag;
            this.Text = "Asset File Table Browser  - " + obj.Id.ToString();
            if (obj.arch != null)
            {
                HideViewers();
                ShowLoader();
                DataTable dt = new DataTable();
                this.currentArchive = obj.arch;
                this.currentArchDetails = new Dictionary<string, int>();
                this.filesTotal = 0;
                this.filesNamed = 0;
                this.filesUnnamed = 0;
                this.filesMissing = 0;
                FileListItem.ResetTreeListViewColumns(treeListView1);
                this.treeListView1.TopItemIndex = 0;
                rootList.Clear();
                await Task.Run(() => ParseLibFiles());
                HideLoader();
                treeListView1.Roots = rootList;
                int count = 0;
                foreach (BrightIdeasSoftware.OLVColumn col in treeListView1.Columns)
                {
                    if (count == 0)
                        continue;
                    col.AutoResize(ColumnHeaderAutoResizeStyle.HeaderSize);
                }
                treeListView1.Sort((BrightIdeasSoftware.OLVColumn)treeListView1.Columns[2], SortOrder.Ascending);
                treeListView1.Visible = true;
                double dblCompletion = (double)this.filesNamed / (double)this.filesTotal;
                dt.Columns.Add("Property");
                dt.Columns.Add("Value");
                dt.Rows.Add(new string[] { "Archive", obj.arch.FileName.Split('\\').Last().Replace(".tor", "") });
                dt.Rows.Add(new string[] { "Total Files", String.Format("{0:n0}", filesTotal.ToString()) });
                dt.Rows.Add(new string[] { "Named Files", String.Format("{0:n0}", filesNamed.ToString()) });
                dt.Rows.Add(new string[] { "Unnamed Files", String.Format("{0:n0}", filesUnnamed.ToString()) });
                dt.Rows.Add(new string[] { "Name Completion", String.Format("{0:0.0%}", dblCompletion) });

                if (this.currentArchDetails.Count > 0)
                {
                    List<string> keys = this.currentArchDetails.Keys.ToList();
                    keys.Sort();
                    foreach (var key in keys)
                    {
                        dt.Rows.Add(new string[] { key.ToUpper() + " Files", this.currentArchDetails[key].ToString() });
                    }
                }
                dataGridView1.DataSource = dt;
                dataGridView1.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.AllCells;
            }
        }

        private void ParseLibFiles()
        {
            if (InvokeRequired)
            {
                _ = Invoke(new Action(() => ParseLibFiles()));
            }
            else
            {
                List<TorLib.File> files = this.currentArchive.files;
                files.Sort(delegate (TorLib.File x, TorLib.File y)
                {
                    if (x.FileInfo.Offset > y.FileInfo.Offset)
                        return -1;
                    else
                        return 1;
                });

                foreach (var file in files)
                {
                    HashFileInfo hashInfo = new HashFileInfo(file.FileInfo.PrimaryHash, file.FileInfo.SecondaryHash, file);
                    if (hashInfo.FileName.Contains("metadata.bin") || hashInfo.FileName.Contains("ft.sig"))
                        continue;
                    filesTotal++;
                    if (hashInfo.IsNamed)
                        filesNamed++;
                    else
                        filesUnnamed++;
                    if (!currentArchDetails.Keys.Contains(hashInfo.Extension))
                        currentArchDetails.Add(hashInfo.Extension, 0);
                    else
                        currentArchDetails[hashInfo.Extension]++;
                    rootList.Add(new FileListItem(hashInfo, file.FileInfo));
                }
            }
        }


        public void HideViewers()
        {
            treeListView1.Visible = false;
        }

        public void ShowLoader()
        {
            pictureBox2.Visible = true;
            toolStripProgressBar1.Visible = true;
        }

        public void HideLoader()
        {
            pictureBox2.Visible = false;
            toolStripProgressBar1.Visible = false;
        }

        private void EnableUI()
        {
            this.dataGridView1.Enabled = true;
            this.treeViewFast1.Enabled = true;
            this.treeListView1.Enabled = true;
        }

        private void SetStripProgressBarValue(int prog)
        {
            if (InvokeRequired)
            {
                this.Invoke(new Action<int>(SetStripProgressBarValue), new object[] { prog });
                return;
            }

            toolStripProgressBar1.Value = prog;
        }

        private void SetStripProgressBarMax(int prog)
        {
            if (InvokeRequired)
            {
                this.Invoke(new Action<int>(SetStripProgressBarMax), new object[] { prog });
                return;
            }

            toolStripProgressBar1.Maximum = prog;
        }

        private void SetStripProgressBarStyle(ProgressBarStyle style)
        {
            if (InvokeRequired)
            {
                this.Invoke(new Action<ProgressBarStyle>(SetStripProgressBarStyle), new object[] { style });
                return;
            }

            toolStripProgressBar1.Style = style;
        }

        #endregion

        private void AssetBrowser_FormClosing(object sender, FormClosingEventArgs e)
        {
            _closing = true;
        }

        public void AssetBrowser_FormClosed(object sender, FormClosedEventArgs e)
        {
            HashDictionaryInstance.Instance.Unload();


            if (treeViewFast1 != null)
            {
                treeViewFast1.Dispose();
                treeViewFast1 = null;
            }

            assetDict = null;
            fileDict = null;

        }

        public void SetStatusLabel(string message)
        {
            if (this.statusStrip1.InvokeRequired)
            {
                this.statusStrip1.Invoke(new Action(() => SetStatusLabel(message)));
            }
            else
            {
                this.toolStripStatusLabel1.Text = message;
            }
        }

        public override bool Equals(object obj)
        {
            return obj is AssetBrowserFileTable table &&
                   EqualityComparer<Dictionary<string, List<TorLib.FileInfo>>>.Default.Equals(fileDict, table.fileDict);
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }

        public override object InitializeLifetimeService()
        {
            return base.InitializeLifetimeService();
        }

        public override ObjRef CreateObjRef(Type requestedType)
        {
            return base.CreateObjRef(requestedType);
        }

        protected override object GetService(Type service)
        {
            return base.GetService(service);
        }

        protected override AccessibleObject GetAccessibilityObjectById(int objectId)
        {
            return base.GetAccessibilityObjectById(objectId);
        }

        public override Size GetPreferredSize(Size proposedSize)
        {
            return base.GetPreferredSize(proposedSize);
        }

        protected override AccessibleObject CreateAccessibilityInstance()
        {
            return base.CreateAccessibilityInstance();
        }

        protected override void DestroyHandle()
        {
            base.DestroyHandle();
        }

        protected override void InitLayout()
        {
            base.InitLayout();
        }

        protected override bool IsInputChar(char charCode)
        {
            return base.IsInputChar(charCode);
        }

        protected override bool IsInputKey(Keys keyData)
        {
            return base.IsInputKey(keyData);
        }

        protected override void NotifyInvalidate(Rectangle invalidatedArea)
        {
            base.NotifyInvalidate(invalidatedArea);
        }

        protected override void OnAutoSizeChanged(EventArgs e)
        {
            base.OnAutoSizeChanged(e);
        }

        protected override void OnBackColorChanged(EventArgs e)
        {
            base.OnBackColorChanged(e);
        }

        protected override void OnBindingContextChanged(EventArgs e)
        {
            base.OnBindingContextChanged(e);
        }

        protected override void OnCausesValidationChanged(EventArgs e)
        {
            base.OnCausesValidationChanged(e);
        }

        protected override void OnContextMenuChanged(EventArgs e)
        {
            base.OnContextMenuChanged(e);
        }

        protected override void OnContextMenuStripChanged(EventArgs e)
        {
            base.OnContextMenuStripChanged(e);
        }

        protected override void OnCursorChanged(EventArgs e)
        {
            base.OnCursorChanged(e);
        }

        protected override void OnDockChanged(EventArgs e)
        {
            base.OnDockChanged(e);
        }

        protected override void OnForeColorChanged(EventArgs e)
        {
            base.OnForeColorChanged(e);
        }

        protected override void OnNotifyMessage(Message m)
        {
            base.OnNotifyMessage(m);
        }

        protected override void OnParentBackColorChanged(EventArgs e)
        {
            base.OnParentBackColorChanged(e);
        }

        protected override void OnParentBackgroundImageChanged(EventArgs e)
        {
            base.OnParentBackgroundImageChanged(e);
        }

        protected override void OnParentBindingContextChanged(EventArgs e)
        {
            base.OnParentBindingContextChanged(e);
        }

        protected override void OnParentCursorChanged(EventArgs e)
        {
            base.OnParentCursorChanged(e);
        }

        protected override void OnParentEnabledChanged(EventArgs e)
        {
            base.OnParentEnabledChanged(e);
        }

        protected override void OnParentFontChanged(EventArgs e)
        {
            base.OnParentFontChanged(e);
        }

        protected override void OnParentForeColorChanged(EventArgs e)
        {
            base.OnParentForeColorChanged(e);
        }

        protected override void OnParentRightToLeftChanged(EventArgs e)
        {
            base.OnParentRightToLeftChanged(e);
        }

        protected override void OnParentVisibleChanged(EventArgs e)
        {
            base.OnParentVisibleChanged(e);
        }

        protected override void OnPrint(PaintEventArgs e)
        {
            base.OnPrint(e);
        }

        protected override void OnTabIndexChanged(EventArgs e)
        {
            base.OnTabIndexChanged(e);
        }

        protected override void OnTabStopChanged(EventArgs e)
        {
            base.OnTabStopChanged(e);
        }

        protected override void OnClick(EventArgs e)
        {
            base.OnClick(e);
        }

        protected override void OnClientSizeChanged(EventArgs e)
        {
            base.OnClientSizeChanged(e);
        }

        protected override void OnControlAdded(ControlEventArgs e)
        {
            base.OnControlAdded(e);
        }

        protected override void OnControlRemoved(ControlEventArgs e)
        {
            base.OnControlRemoved(e);
        }

        protected override void OnLocationChanged(EventArgs e)
        {
            base.OnLocationChanged(e);
        }

        protected override void OnDoubleClick(EventArgs e)
        {
            base.OnDoubleClick(e);
        }

        protected override void OnDragEnter(DragEventArgs drgevent)
        {
            base.OnDragEnter(drgevent);
        }

        protected override void OnDragOver(DragEventArgs drgevent)
        {
            base.OnDragOver(drgevent);
        }

        protected override void OnDragLeave(EventArgs e)
        {
            base.OnDragLeave(e);
        }

        protected override void OnDragDrop(DragEventArgs drgevent)
        {
            base.OnDragDrop(drgevent);
        }

        protected override void OnGiveFeedback(GiveFeedbackEventArgs gfbevent)
        {
            base.OnGiveFeedback(gfbevent);
        }

        protected override void OnGotFocus(EventArgs e)
        {
            base.OnGotFocus(e);
        }

        protected override void OnHelpRequested(HelpEventArgs hevent)
        {
            base.OnHelpRequested(hevent);
        }

        protected override void OnInvalidated(InvalidateEventArgs e)
        {
            base.OnInvalidated(e);
        }

        protected override void OnKeyDown(KeyEventArgs e)
        {
            base.OnKeyDown(e);
        }

        protected override void OnKeyPress(KeyPressEventArgs e)
        {
            base.OnKeyPress(e);
        }

        protected override void OnKeyUp(KeyEventArgs e)
        {
            base.OnKeyUp(e);
        }

        protected override void OnLeave(EventArgs e)
        {
            base.OnLeave(e);
        }

        protected override void OnLostFocus(EventArgs e)
        {
            base.OnLostFocus(e);
        }

        protected override void OnMarginChanged(EventArgs e)
        {
            base.OnMarginChanged(e);
        }

        protected override void OnMouseDoubleClick(MouseEventArgs e)
        {
            base.OnMouseDoubleClick(e);
        }

        protected override void OnMouseClick(MouseEventArgs e)
        {
            base.OnMouseClick(e);
        }

        protected override void OnMouseCaptureChanged(EventArgs e)
        {
            base.OnMouseCaptureChanged(e);
        }

        protected override void OnMouseDown(MouseEventArgs e)
        {
            base.OnMouseDown(e);
        }

        protected override void OnMouseEnter(EventArgs e)
        {
            base.OnMouseEnter(e);
        }

        protected override void OnMouseLeave(EventArgs e)
        {
            base.OnMouseLeave(e);
        }

        protected override void OnMouseHover(EventArgs e)
        {
            base.OnMouseHover(e);
        }

        protected override void OnMouseMove(MouseEventArgs e)
        {
            base.OnMouseMove(e);
        }

        protected override void OnMouseUp(MouseEventArgs e)
        {
            base.OnMouseUp(e);
        }

        protected override void OnMove(EventArgs e)
        {
            base.OnMove(e);
        }

        protected override void OnQueryContinueDrag(QueryContinueDragEventArgs qcdevent)
        {
            base.OnQueryContinueDrag(qcdevent);
        }

        protected override void OnRegionChanged(EventArgs e)
        {
            base.OnRegionChanged(e);
        }

        protected override void OnPreviewKeyDown(PreviewKeyDownEventArgs e)
        {
            base.OnPreviewKeyDown(e);
        }

        protected override void OnSizeChanged(EventArgs e)
        {
            base.OnSizeChanged(e);
        }

        protected override void OnChangeUICues(UICuesEventArgs e)
        {
            base.OnChangeUICues(e);
        }

        protected override void OnSystemColorsChanged(EventArgs e)
        {
            base.OnSystemColorsChanged(e);
        }

        protected override void OnValidating(CancelEventArgs e)
        {
            base.OnValidating(e);
        }

        protected override void OnValidated(EventArgs e)
        {
            base.OnValidated(e);
        }

        public override bool PreProcessMessage(ref Message msg)
        {
            return base.PreProcessMessage(ref msg);
        }

        protected override bool ProcessKeyEventArgs(ref Message m)
        {
            return base.ProcessKeyEventArgs(ref m);
        }

        protected override bool ProcessKeyMessage(ref Message m)
        {
            return base.ProcessKeyMessage(ref m);
        }

        public override void ResetBackColor()
        {
            base.ResetBackColor();
        }

        public override void ResetCursor()
        {
            base.ResetCursor();
        }

        public override void ResetFont()
        {
            base.ResetFont();
        }

        public override void ResetForeColor()
        {
            base.ResetForeColor();
        }

        public override void ResetRightToLeft()
        {
            base.ResetRightToLeft();
        }

        public override void Refresh()
        {
            base.Refresh();
        }

        public override void ResetText()
        {
            base.ResetText();
        }

        protected override Size SizeFromClientSize(Size clientSize)
        {
            return base.SizeFromClientSize(clientSize);
        }

        protected override void OnImeModeChanged(EventArgs e)
        {
            base.OnImeModeChanged(e);
        }

        protected override void OnMouseWheel(MouseEventArgs e)
        {
            base.OnMouseWheel(e);
        }

        protected override void OnRightToLeftChanged(EventArgs e)
        {
            base.OnRightToLeftChanged(e);
        }

        protected override void OnPaintBackground(PaintEventArgs e)
        {
            base.OnPaintBackground(e);
        }

        protected override void OnPaddingChanged(EventArgs e)
        {
            base.OnPaddingChanged(e);
        }

        protected override Point ScrollToControl(Control activeControl)
        {
            return base.ScrollToControl(activeControl);
        }

        protected override void OnScroll(ScrollEventArgs se)
        {
            base.OnScroll(se);
        }

        protected override void OnAutoValidateChanged(EventArgs e)
        {
            base.OnAutoValidateChanged(e);
        }

        protected override void OnParentChanged(EventArgs e)
        {
            base.OnParentChanged(e);
        }

        protected override void SetVisibleCore(bool value)
        {
            base.SetVisibleCore(value);
        }

        protected override void AdjustFormScrollbars(bool displayScrollbars)
        {
            base.AdjustFormScrollbars(displayScrollbars);
        }

        protected override Control.ControlCollection CreateControlsInstance()
        {
            return base.CreateControlsInstance();
        }

        protected override void CreateHandle()
        {
            base.CreateHandle();
        }

        protected override void DefWndProc(ref Message m)
        {
            base.DefWndProc(ref m);
        }

        protected override bool ProcessMnemonic(char charCode)
        {
            return base.ProcessMnemonic(charCode);
        }

        protected override void OnActivated(EventArgs e)
        {
            base.OnActivated(e);
        }

        protected override void OnBackgroundImageChanged(EventArgs e)
        {
            base.OnBackgroundImageChanged(e);
        }

        protected override void OnBackgroundImageLayoutChanged(EventArgs e)
        {
            base.OnBackgroundImageLayoutChanged(e);
        }

        protected override void OnClosing(CancelEventArgs e)
        {
            base.OnClosing(e);
        }

        protected override void OnClosed(EventArgs e)
        {
            base.OnClosed(e);
        }

        protected override void OnFormClosing(FormClosingEventArgs e)
        {
            base.OnFormClosing(e);
        }

        protected override void OnFormClosed(FormClosedEventArgs e)
        {
            base.OnFormClosed(e);
        }

        protected override void OnCreateControl()
        {
            base.OnCreateControl();
        }

        protected override void OnDeactivate(EventArgs e)
        {
            base.OnDeactivate(e);
        }

        protected override void OnEnabledChanged(EventArgs e)
        {
            base.OnEnabledChanged(e);
        }

        protected override void OnEnter(EventArgs e)
        {
            base.OnEnter(e);
        }

        protected override void OnFontChanged(EventArgs e)
        {
            base.OnFontChanged(e);
        }

        protected override void OnHandleCreated(EventArgs e)
        {
            base.OnHandleCreated(e);
        }

        protected override void OnHandleDestroyed(EventArgs e)
        {
            base.OnHandleDestroyed(e);
        }

        protected override void OnHelpButtonClicked(CancelEventArgs e)
        {
            base.OnHelpButtonClicked(e);
        }

        protected override void OnLayout(LayoutEventArgs levent)
        {
            base.OnLayout(levent);
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);
        }

        protected override void OnMaximizedBoundsChanged(EventArgs e)
        {
            base.OnMaximizedBoundsChanged(e);
        }

        protected override void OnMaximumSizeChanged(EventArgs e)
        {
            base.OnMaximumSizeChanged(e);
        }

        protected override void OnMinimumSizeChanged(EventArgs e)
        {
            base.OnMinimumSizeChanged(e);
        }

        protected override void OnInputLanguageChanged(InputLanguageChangedEventArgs e)
        {
            base.OnInputLanguageChanged(e);
        }

        protected override void OnInputLanguageChanging(InputLanguageChangingEventArgs e)
        {
            base.OnInputLanguageChanging(e);
        }

        protected override void OnVisibleChanged(EventArgs e)
        {
            base.OnVisibleChanged(e);
        }

        protected override void OnMdiChildActivate(EventArgs e)
        {
            base.OnMdiChildActivate(e);
        }

        protected override void OnMenuStart(EventArgs e)
        {
            base.OnMenuStart(e);
        }

        protected override void OnMenuComplete(EventArgs e)
        {
            base.OnMenuComplete(e);
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);
        }

        protected override void OnResize(EventArgs e)
        {
            base.OnResize(e);
        }

        protected override void OnRightToLeftLayoutChanged(EventArgs e)
        {
            base.OnRightToLeftLayoutChanged(e);
        }

        protected override void OnShown(EventArgs e)
        {
            base.OnShown(e);
        }

        protected override void OnTextChanged(EventArgs e)
        {
            base.OnTextChanged(e);
        }

        protected override bool ProcessCmdKey(ref Message msg, Keys keyData)
        {
            return base.ProcessCmdKey(ref msg, keyData);
        }

        protected override bool ProcessDialogKey(Keys keyData)
        {
            return base.ProcessDialogKey(keyData);
        }

        protected override bool ProcessDialogChar(char charCode)
        {
            return base.ProcessDialogChar(charCode);
        }

        protected override bool ProcessKeyPreview(ref Message m)
        {
            return base.ProcessKeyPreview(ref m);
        }

        protected override bool ProcessTabKey(bool forward)
        {
            return base.ProcessTabKey(forward);
        }

        protected override void Select(bool directed, bool forward)
        {
            base.Select(directed, forward);
        }

        protected override void ScaleCore(float x, float y)
        {
            base.ScaleCore(x, y);
        }

        protected override Rectangle GetScaledBounds(Rectangle bounds, SizeF factor, BoundsSpecified specified)
        {
            return base.GetScaledBounds(bounds, factor, specified);
        }

        protected override void ScaleControl(SizeF factor, BoundsSpecified specified)
        {
            base.ScaleControl(factor, specified);
        }

        protected override void SetBoundsCore(int x, int y, int width, int height, BoundsSpecified specified)
        {
            base.SetBoundsCore(x, y, width, height, specified);
        }

        protected override void SetClientSizeCore(int x, int y)
        {
            base.SetClientSizeCore(x, y);
        }

        public override string ToString()
        {
            return base.ToString();
        }

        protected override void UpdateDefaultButton()
        {
            base.UpdateDefaultButton();
        }

        protected override void OnResizeBegin(EventArgs e)
        {
            base.OnResizeBegin(e);
        }

        protected override void OnResizeEnd(EventArgs e)
        {
            base.OnResizeEnd(e);
        }

        protected override void OnStyleChanged(EventArgs e)
        {
            base.OnStyleChanged(e);
        }

        public override bool ValidateChildren()
        {
            return base.ValidateChildren();
        }

        public override bool ValidateChildren(ValidationConstraints validationConstraints)
        {
            return base.ValidateChildren(validationConstraints);
        }

        protected override void WndProc(ref Message m)
        {
            base.WndProc(ref m);
        }
    }
}
