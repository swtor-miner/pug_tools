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

namespace tor_tools
{  
    public partial class AssetBrowser : Form
    {   
        private bool autoPreview = true;
        private string extractPath;
        private string assetLocation;
        private bool usePTS;

        private HashDictionaryInstance hashData;
        private Dictionary<string, List<TorLib.FileInfo>> fileDict = new Dictionary<string, List<TorLib.FileInfo>>();
        private Dictionary<string, TreeListItem> assetDict = new Dictionary<string, TreeListItem>();

        private Stream inputStream;
        private MemoryStream memStream;
        private XmlDocument xmlDoc;
        private View_GR2 panelRender;
        public bool _closing = false;
        private Thread render;

        private List<string> searchNodes = new List<string>();
        private int searchIndex = 0;
        private TreeNode[] nodeMatch;
        private int namesFound = 0;
        private int totalNamesFound = 0;
        private int filesSearched = 0;
        private int totalFilesSearched = 0;
        private ArrayList rootList = new ArrayList();
        
        private Hasher hasher = new Hasher(Hasher.HasherType.TOR);
        private HashSet<string> foundFiles = new HashSet<string>();

        private bool audioState = false;
        private bool audioHasPlayed = false;
        private WaveOutEvent waveOut;

        private bool extractByExtensions = false;
        private HashSet<string> extractExtensions = new HashSet<string>();
        private int extractCount = 0;
        private ulong modNewCount = 0;
        private int foundNewFileCount = 0;

        public AssetBrowser(string assetLocation, bool usePTS)
        {
            InitializeComponent();
            FormClosed += AssetBrowser_FormClosed;
            backgroundWorker2.ProgressChanged += backgroundWorker2_ProgressChanged;

            //This should probably done by Designer?
            backgroundWorker3.DoWork += backgroundWorker3_DoWork;
            backgroundWorker3.RunWorkerCompleted += backgroundWorker3_RunWorkerCompleted;

            hashData = HashDictionaryInstance.Instance;
            if(!hashData.Loaded)
            {
                hashData.Load();
            }

            this.assetLocation = assetLocation;
            this.usePTS = usePTS;

            Config.Load();
            txtExtractPath.Text = Config.ExtractAssetsPath;
            this.extractPath = txtExtractPath.Text;

            List<object> args = new List<object>();
            args.Add(assetLocation);
            args.Add(usePTS);
            toolStripStatusLabel1.Text = "Loading Assets...";
            showLoader();            
            treeListView1.CanExpandGetter = delegate(object x) 
            {
                if (x.GetType() == typeof(NodeListItem))
                {
                    return (((NodeListItem)x).children.Count > 0);
                }
                if (x.GetType() == typeof(WemListItem))
                {
                    return (((WemListItem)x).children.Count > 0);
                }
                return false;
            };
            treeListView1.ChildrenGetter = delegate(object x)
            {
                if (x.GetType() == typeof(NodeListItem))                
                    return new ArrayList(((NodeListItem)x).children);                
                if (x.GetType() == typeof(WemListItem))
                    return new ArrayList(((WemListItem)x).children);                
                return null;
            };
            backgroundWorker1.RunWorkerAsync(args);            
        }

    #region Background Wokers Methods

        private void backgroundWorker1_DoWork(object sender, DoWorkEventArgs e)
        {
            if(_closing)
            {
                return;
            }

            //Make sure the DOM is loaded.
            Assets assets = AssetHandler.Instance.getCurrentAssets(assetLocation, usePTS);
            DomHandler.Instance.getCurrentDOM(assets);
            
            //this.currentAssets = this.currentAssets.getCurrentAssets((string)args[0], (bool)args[1]);            
        }

        private void backgroundWorker1_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            if(_closing)
            {
                return;
            }

            toolStripProgressBar1.Style = ProgressBarStyle.Continuous;
            toolStripStatusLabel1.Text = "Loading Files...";
            backgroundWorker2.RunWorkerAsync();
        }

        private void backgroundWorker2_DoWork(object sender, DoWorkEventArgs e)
        {
            if (_closing)
            {
                return;
            }

            //hashData.dictionary.CreateHelpers();
            HashSet<string> fileDirs = new HashSet<string>();
            HashSet<string> allDirs = new HashSet<string>();

            int intNamCount = 0;
            int intModCount = 0;
            int intNewCount = 0;
            int intUnnCount = 0;

            const string prefixNam = "/root/named";
            const string prefixNew = "/root/new";
            const string prefixMod = "/root/modified";
            const string prefixUnn = "/root/unnamed";


            Assets currentAssets = AssetHandler.Instance.getCurrentAssets(assetLocation, usePTS);
            int libsDone = 0;
            int maxLibs = currentAssets.libraries.Count;

            foreach (var lib in currentAssets.libraries)
            {
                string path = lib.Location;
                if (!lib.Loaded)
                    lib.Load();
                foreach (var arch in lib.archives)
                {
                    foreach (var file in arch.Value.files)
                    {
                        HashFileInfo hashInfo = new HashFileInfo(file.FileInfo.ph, file.FileInfo.sh, file);
                        string prefixDir = prefixNam + hashInfo.Directory;

                        if (hashInfo.IsNamed)
                        {
                            if (hashInfo.FileName == "metadata.bin" || hashInfo.FileName == "ft.sig")
                            {
                                continue;
                            }

                            TreeListItem assetAll = new TreeListItem(prefixDir + "/" + hashInfo.FileName, prefixDir, hashInfo.FileName, hashInfo);
                            assetDict.Add(prefixDir + "/" + hashInfo.FileName, assetAll);
                            fileDirs.Add(prefixDir);
                            intNamCount++;

                            if (hashInfo.FileState == HashFileInfo.State.New)
                            {
                                TreeListItem assetNew = new TreeListItem(prefixNew + hashInfo.Directory + "/" + hashInfo.FileName, prefixNew + hashInfo.Directory, hashInfo.FileName, hashInfo);
                                assetDict.Add(prefixNew + hashInfo.Directory + "/" + hashInfo.FileName, assetNew);
                                fileDirs.Add(prefixNew + hashInfo.Directory);
                                intNewCount++;
                            }
                            if (hashInfo.FileState == HashFileInfo.State.Modified)
                            {
                                TreeListItem assetMod = new TreeListItem(prefixMod + hashInfo.Directory + "/" + hashInfo.FileName, prefixMod + hashInfo.Directory, hashInfo.FileName, hashInfo);
                                assetDict.Add(prefixMod + hashInfo.Directory + "/" + hashInfo.FileName, assetMod);
                                fileDirs.Add(prefixMod + hashInfo.Directory);
                                intModCount++;
                            }
                        }
                        else
                        {
                            hashInfo.Directory = "/" + hashInfo.Source.Replace(".tor", string.Empty);
                           
                            TreeListItem assetUnn = new TreeListItem(prefixUnn + hashInfo.Directory + "/" + hashInfo.Extension + "/" + hashInfo.FileName + "." + hashInfo.Extension, prefixUnn + hashInfo.Directory + "/" + hashInfo.Extension, hashInfo.FileName + "." + hashInfo.Extension, hashInfo);
                            assetDict.Add(prefixUnn + hashInfo.Directory + "/" + hashInfo.Extension + "/" + hashInfo.FileName + "." + hashInfo.Extension, assetUnn);
                            fileDirs.Add(prefixUnn + hashInfo.Directory + "/" + hashInfo.Extension);
                            intUnnCount++;

                            if (hashInfo.FileState == HashFileInfo.State.New)
                            {
                                TreeListItem assetNew = new TreeListItem(prefixNew + hashInfo.Directory + "/" + hashInfo.Extension + "/" + hashInfo.FileName + "." + hashInfo.Extension, prefixNew + hashInfo.Directory + "/" + hashInfo.Extension, hashInfo.FileName + "." + hashInfo.Extension, hashInfo);
                                assetDict.Add(prefixNew + hashInfo.Directory + "/" + hashInfo.Extension + "/" + hashInfo.FileName + "." + hashInfo.Extension, assetNew);
                                fileDirs.Add(prefixNew + hashInfo.Directory + "/" + hashInfo.Extension);
                                intNewCount++;
                            }
                            if (hashInfo.FileState == HashFileInfo.State.Modified)
                            {
                                TreeListItem assetMod = new TreeListItem(prefixMod + hashInfo.Directory + "/" + hashInfo.Extension + "/" + hashInfo.FileName + "." + hashInfo.Extension, prefixMod + hashInfo.Directory + "/" + hashInfo.Extension, hashInfo.FileName + "." + hashInfo.Extension, hashInfo);
                                assetDict.Add(prefixMod + hashInfo.Directory + "/" + hashInfo.Extension + "/" + hashInfo.FileName + "." + hashInfo.Extension, assetMod);
                                fileDirs.Add(prefixMod + hashInfo.Directory + "/" + hashInfo.Extension);
                                intModCount++;
                            }
                        }
                    }
                }
                libsDone++;
                backgroundWorker2.ReportProgress(libsDone * 100 / maxLibs);
            }

            this.modNewCount = (ulong)(intModCount + intNewCount);

            HashFileInfo empty = new HashFileInfo(0, 0, null);
            assetDict.Add("/root", new TreeListItem("/root", string.Empty, "Root", empty));
            assetDict.Add("/root/named", new TreeListItem("/root/named", "/root", "Named Files (" + intNamCount + ")", empty));
            assetDict.Add("/root/modified", new TreeListItem("/root/modified", "/root", "Modified Files (" + intModCount + ")", empty));
            assetDict.Add("/root/new", new TreeListItem("/root/new", "/root", "New Files (" + intNewCount + ")", empty));
            assetDict.Add("/root/unnamed", new TreeListItem("/root/unnamed", "/root", "Unnamed Files (" + intUnnCount + ")", empty));

            foreach (var dir in fileDirs)
            {
                string[] temp = dir.Split('/');
                int intLength = temp.Length;
                for (int intCount2 = 0; intCount2 <= intLength; intCount2++)
                {
                    string output = String.Join("/", temp, 0, intCount2);
                    if (output.Length > 0)
                        allDirs.Add(output);
                }
            }
            foreach (var dir in allDirs)
            {
                string[] temp = dir.Split('/');
                string parentDir = String.Join("/", temp.Take(temp.Length - 1));
                if (parentDir.Length == 0)
                    parentDir = "/root";
                string display = temp.Last();

                TreeListItem asset = new TreeListItem(dir, parentDir, display, empty);
                if (!assetDict.ContainsKey(dir))
                    assetDict.Add(dir.ToString(), asset);
            }
        }

        private void backgroundWorker2_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            if (_closing)
            {
                return;
            }

            toolStripProgressBar1.Value = 0;
            toolStripProgressBar1.Style = ProgressBarStyle.Marquee;

            toolStripStatusLabel1.Text = "Loading Tree View Items ...";
            backgroundWorker3.RunWorkerAsync();
        }

        private void backgroundWorker3_DoWork(object sender, DoWorkEventArgs e)
        {
            if (_closing)
            {
                return;
            }

            Func<TreeListItem, string> getId = (x => x.Id);
            Func<TreeListItem, string> getParentId = (x => x.parentId);
            Func<TreeListItem, string> getDisplayName = (x => x.displayName);
            treeViewFast1.BeginUpdate();
            treeViewFast1.LoadItems<TreeListItem>(assetDict, getId, getParentId, getDisplayName);            
        }

        private void backgroundWorker3_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            if (_closing)
            {
                return;
            }

            treeViewFast1.EndUpdate();
            toolStripStatusLabel1.Text = "Loading Complete";
            treeViewFast1.Visible = true;
            panelRender = new View_GR2(this.Handle, this, "renderPanel");
            panelRender.Init();
            hideLoader();
            enableUI();
            if (treeViewFast1.Nodes.Count > 0) treeViewFast1.Nodes[0].Expand();
        }

    #endregion

    #region Input / Output Methods         

        private async Task loadObject(TorLib.File file)
        {
            this.inputStream = file.OpenCopyInMemory();
            return;
        }

        private async void extractByNode(TreeNodeCollection nodes)
        {
            foreach (TreeNode child in nodes)
            {
                TreeListItem asset = (TreeListItem)child.Tag;

                if (asset.hashInfo.file != null)
                {
                    if (this.extractByExtensions)
                    {   
                        if (extractExtensions.Contains(asset.hashInfo.Extension.ToUpper()))
                        {
                            extractAsset(asset.hashInfo);
                        }
                        else
                            continue;
                    }else
                        extractAsset(asset.hashInfo);
                }

                if (child.Nodes.Count > 0)
                    extractByNode(child.Nodes);
            }
        }

        private void extractAsset(HashFileInfo assetFile)
        {
            if (assetFile.FileName.EndsWith(".bkt"))
                return; // don't need bucket files output all the time
            string fileName;
            string directory;
            if (assetFile.IsNamed)
                fileName = this.extractPath + String.Join("\\", assetFile.Directory, assetFile.FileName).Replace("/", "\\");
            else
                fileName = this.extractPath + assetFile.Directory.Replace("/", "\\") + "\\" + assetFile.Extension.ToLower() + "\\" + assetFile.FileName + "." + assetFile.Extension;
            fileName = fileName.Replace("\\\\", "\\");
            directory = Path.GetDirectoryName(fileName);
            if (!System.IO.Directory.Exists(directory)) { System.IO.Directory.CreateDirectory(directory); }            
            Stream file = assetFile.file.Open();
            var outputStream = System.IO.File.Create(fileName);
            file.CopyTo(outputStream);
            outputStream.Close();
            extractCount++;
        }

        private async void searchTreeNodes()
        {
            this.nodeMatch = treeViewFast1.Nodes.Find(this.searchNodes[this.searchIndex], true);
        }

    #endregion

    #region UI Methods
        private void treeViewFast1_AfterSelect(object sender, TreeViewEventArgs e)
        {
            TreeNode node = treeViewFast1.SelectedNode;
            TreeListItem asset = (TreeListItem)node.Tag;
            this.Text = "Asset Browser - " + asset.Id.ToString();
            if (asset.hashInfo.file != null)
            {
                DataTable dt = new DataTable();
                HashFileInfo info = (HashFileInfo)asset.hashInfo;
                dt.Columns.Add("Property");
                dt.Columns.Add("Value");
                dt.Rows.Add(new string[] { "Archive", info.Source.ToString() });
                dt.Rows.Add(new string[] { "File ID", String.Format("{0:X16}", info.file.FileInfo.FileId) });
                if (info.IsNamed)
                    dt.Rows.Add(new string[] { "File Name", info.FileName });
                else
                    dt.Rows.Add(new string[] { "File Name", info.FileName + "." + info.Extension });
                dt.Rows.Add(new string[] { "File Type", info.Extension });
                dt.Rows.Add(new string[] { "Path", info.Directory });
                dt.Rows.Add(new string[] { "State", info.FileState.ToString() });
                dt.Rows.Add(new string[] { "Compressed Size", info.file.FileInfo.CompressedSize.ToString() });
                dt.Rows.Add(new string[] { "Uncompressed Size", info.file.FileInfo.UncompressedSize.ToString() });
                dt.Rows.Add(new string[] { "Header Size", info.file.FileInfo.HeaderSize.ToString() });
                dt.Rows.Add(new string[] { "Offset", ((long)info.file.FileInfo.Offset).ToString() });
                dt.Rows.Add(new string[] { "Primary Hash", String.Format("{0:X8}", info.file.FileInfo.ph) });
                dt.Rows.Add(new string[] { "Secondary Hash", String.Format("{0:X8}", info.file.FileInfo.sh) });
                dt.Rows.Add(new string[] { "Checksum", String.Format("{0:X8}", info.file.FileInfo.Checksum) });
                dt.Rows.Add(new string[] { "Is Compressed", info.file.FileInfo.IsCompressed.ToString() });
                dataGridView1.DataSource = dt;
                dataGridView1.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.AllCells;

                if (this.autoPreview)
                {
                    previewAsset(asset);
                }

            }
        }

        private void treeViewFast1_MouseUp(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Right)
            {
                treeViewFast1.SelectedNode = treeViewFast1.GetNodeAt(e.X, e.Y);

                if (treeViewFast1.SelectedNode != null)
                {
                    contextMenuStrip1.Show(treeViewFast1, e.Location);
                }
            }
        }

        private void extractToolStripMenuItem_Click(object sender, EventArgs e)
        {            
            this.extractByExtensions = false;
            btnExtract_Click(this, null);
        }

        public void hideViewers()
        {
            pictureBox1.Visible = false;
            elementHost1.Visible = false;
            txtRawView.Visible = false;
            hexBox1.Visible = false;
            treeListView1.Visible = false;
            renderPanel.Visible = false;
            webBrowser1.Visible = false;
        }

        public void showLoader()
        {
            pictureBox2.Visible = true;
            toolStripProgressBar1.Visible = true;
        }

        public void hideLoader()
        {
            pictureBox2.Visible = false;
            toolStripProgressBar1.Visible = false;
        }

        private void enableUI()
        {
            this.dataGridView1.Enabled = true;
            this.txtExtractPath.Enabled = true;
            this.btnPreview.Enabled = true;
            this.btnExtract.Enabled = true;
            this.btnViewRaw.Enabled = true;
            this.txtSearch.Enabled = true;
            this.btnSearch.Enabled = true;
            this.btnFindFileNames.Enabled = true;
            this.btnChooseExtract.Enabled = true;
            this.treeViewFast1.Enabled = true;
            this.btnViewHex.Enabled = true;
            this.treeListView1.Enabled = true;
            this.btnTestFile.Enabled = true;
            this.btnHelp.Enabled = true;
            this.btnHashStatus.Enabled = true;
        }

        private void disableUI()
        {
            this.dataGridView1.Enabled = false;
            this.txtExtractPath.Enabled = false;
            this.btnPreview.Enabled = false;
            this.btnExtract.Enabled = false;
            this.btnViewRaw.Enabled = false;
            this.txtSearch.Enabled = false;
            this.btnSearch.Enabled = false;
            this.btnFindFileNames.Enabled = false;
            this.btnChooseExtract.Enabled = false;
            this.treeViewFast1.Enabled = false;
            this.btnViewHex.Enabled = false;
            this.treeListView1.Enabled = false;
            this.btnTestFile.Enabled = false;
            this.btnHelp.Enabled = false;
            this.btnHashStatus.Enabled = false;
        }

        private void Position_Changed(object sender, EventArgs e)
        {
            this.toolStripStatusLabel1.Text = string.Format("Ln {0}    Col {1}", hexBox1.CurrentLine, hexBox1.CurrentPositionInLine);

            string bitPresentation = string.Empty;

            byte? currentByte = hexBox1.ByteProvider != null && hexBox1.ByteProvider.Length > hexBox1.SelectionStart
                ? hexBox1.ByteProvider.ReadByte(hexBox1.SelectionStart)
                : (byte?)null;

            BitInfo bitInfo = currentByte != null ? new BitInfo((byte)currentByte, hexBox1.SelectionStart) : null;

            if (bitInfo != null)
            {
                byte currentByteNotNull = (byte)currentByte;
                bitPresentation = string.Format("Bits of Byte {0}: {1}"
                    , hexBox1.SelectionStart
                    , bitInfo.ToString()
                    );
            }

            this.toolStripStatusLabel2.Text = bitPresentation;

            this.toolStripStatusLabel3.Text = bitInfo.ToString();
        }

       private void backgroundWorker2_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            toolStripProgressBar1.Value = e.ProgressPercentage;
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

    #region Preview Methods

        public async void previewAsset(TreeListItem asset)
        {
            if(asset.hashInfo.file != null)
            {
                hideViewers();                                
                showLoader();
                this.toolStripStatusLabel1.Text = "Loading File";
                this.toolStripStatusLabel2.Text = "";
                this.toolStripStatusLabel3.Text = "";                
                this.treeListView1.SelectedIndices.Clear();
                if (render != null)
                {
                    panelRender.stopRender();
                    render.Join();
                    panelRender.Clear();
                }  
                await Task.Run(() => loadObject(asset.hashInfo.file));                
                //DynamicFileByteProvider byteProvider = new DynamicFileByteProvider(this.inputStream);
                //hexBox1.ByteProvider = byteProvider;
                //this.inputStream.Position = 0;
                switch (asset.hashInfo.Extension.ToUpper())
                {
                    case "DDS":                        
                        await Task.Run(() => previewDDS());
                        checkDDSPath(asset.hashInfo.Directory);
                        pictureBox1.Visible = true;
                        break;
                    case "PNG":
                        await Task.Run(() => previewPNG());                        
                        pictureBox1.Visible = true;
                        break;
                    case "XML":
                    case "MAT":
                    case "TEX":
                    case "EMT":
                    case "EPP":
                    case "FXSPEC":
                    case "RUL":
                    case "MANIFEST":
                    case "SVY":
                    case "TBL":
                    case "LOD":
                        await Task.Run(() => previewXML());
                        webBrowser1.Visible = true;        
                        break;
                    case "NOT":                        
                        await Task.Run(() => previewNOT());
                        webBrowser1.Visible = true;                              
                        break;
                    case "DAT":
                        rootList.Clear();
                        await Task.Run(() => previewDAT());
                        treeListView1.Roots = rootList;
                        treeListView1.ExpandAll();                        
                        hideLoader();
                        //txtRawView.Visible = true;
                        treeListView1.Visible = true;
                        break;
                    case "DYC":
                    case "MAG":
                    case "PRT":
                        txtRawView.Visible = true;    
                        await Task.Run(() => previewRAW());
                        break;
                    case "GR2":                       
                        await Task.Run(() => previewGR2(asset.hashInfo.FileName));                        
                        renderPanel.Visible = true;                                           
                        break;
                    case "STB":
                        rootList.Clear();
                        await Task.Run(() => previewSTB());
                        treeListView1.Roots = rootList;
                        treeListView1.ExpandAll();                        
                        hideLoader();                        
                        treeListView1.Visible = true;
                        break;
                    case "BNK":
                        rootList.Clear();
                        await Task.Run(() => previewBNK());
                        treeListView1.Roots = rootList;
                        treeListView1.ExpandAll();                        
                        hideLoader();                        
                        treeListView1.Visible = true;
                        break;
                    case "ACB":
                        rootList.Clear();
                        await Task.Run(() => previewACB());
                        treeListView1.Roots = rootList;
                        treeListView1.ExpandAll();                        
                        hideLoader();                        
                        treeListView1.Visible = true;
                        break;
                    case "WAV":
                    case "WEM":
                        this.treeViewFast1.Enabled = false;
                        this.toolStripStatusLabel1.Text = "Playing Audio...";
                        this.toolStripProgressBar1.Visible = true;                        
                        this.audioState = false;
                        await Task.Run(() => previewWEM(asset.hashInfo.FileName));                        
                        this.toolStripStatusLabel1.Text = "Audio Stopped";
                        this.toolStripProgressBar1.Visible = false;
                        this.treeViewFast1.Enabled = true;
                        break;
                    case "DEP":
                        this.toolStripStatusLabel1.Text = "Parsing DEP...";
                        this.toolStripProgressBar1.Visible = true;                        
                        await Task.Run(() => previewDEP());
                        treeListView1.Roots = rootList;
                        treeListView1.ExpandAll();                        
                        hideLoader();                        
                        treeListView1.Visible = true;
                        this.toolStripProgressBar1.Visible = false;
                        break;
                    default:                        
                        await Task.Run(() => previewHEX());                        
                        txtRawView.Visible = true;
                        break;                        
                }
                this.treeListView1.TopItemIndex = 0;
                this.toolStripStatusLabel1.Text = "File Loaded";
                this.toolStripStatusLabel2.Text = "";
                this.toolStripStatusLabel3.Text = "";                
                hideLoader();
            }
        }      

        private async Task previewDDS()
        {
            if (InvokeRequired)
            {
                Invoke(new Action(() => previewDDS()));
            }
            else
            {
                this.memStream = new MemoryStream();
                DevIL.ImageImporter imp = new ImageImporter();
                DevIL.ImageExporter exp = new ImageExporter();
                DevIL.Image dds = imp.LoadImageFromStream(DevIL.ImageType.Dds, this.inputStream);
                exp.SaveImageToStream(dds, ImageType.Png, this.memStream);
                Bitmap bm = new Bitmap(this.memStream);
                pictureBox1.Image = bm;
            }
        }

        private void checkDDSPath(string directory)
        {
            if (directory.Contains("codex") ||                
                directory.Contains("reputation") ||
                directory.Contains("tutorials"))
            {
                pictureBox1.BackgroundImageLayout = ImageLayout.None;
                pictureBox1.BackgroundImage = null;
                pictureBox1.BackColor = System.Drawing.Color.Black;
            }
            else
            {
                pictureBox1.BackgroundImageLayout = ImageLayout.Tile;
                pictureBox1.BackColor = System.Drawing.Color.White;
                pictureBox1.BackgroundImage = global::PugTools.Properties.Resources.Transparent;
            }
        }

        private async Task previewDEP()
        {
            if (InvokeRequired)
            {
                Invoke(new Action(() => previewDEP()));
            }
            else
            {
                BinaryReader br = new BinaryReader(this.inputStream);
                List<DEP_Entry> entires = View_DEP.Read(br, hashData.dictionary);
                NodeListItem.resetTreeListViewColumns(treeListView1);
                foreach (DEP_Entry entry in entires)
                {
                    rootList.Add(new NodeListItem(entry.filename, entry));
                }
            }
        }

        private async Task previewPNG()
        {
            if (InvokeRequired)
            {
                Invoke(new Action(() => previewPNG()));
            }
            else
            {
                this.memStream = new MemoryStream();
                DevIL.ImageImporter imp = new ImageImporter();
                DevIL.ImageExporter exp = new ImageExporter();
                DevIL.Image png = imp.LoadImageFromStream(DevIL.ImageType.Png, this.inputStream);
                exp.SaveImageToStream(png, ImageType.Bmp, this.memStream);
                Bitmap bm = new Bitmap(this.memStream);                
                pictureBox1.Image = bm;
            }
        }

        private async Task previewXML()
        {
            if (InvokeRequired)
            {
                Invoke(new Action(() => previewXML()));
            }
            else
            {
                this.xmlDoc = new XmlDocument();
                StreamReader reader = new StreamReader(this.inputStream);
                string output = reader.ReadToEnd();
                output = output.Replace("&lt;", "<").Replace("&gt;", ">").Replace("&amp;lt;", "<").Replace("&amp;gt;", ">").Replace("&amp;apos;", "'").Replace("\0", "");                
                this.xmlDoc.LoadXml(output);    
                txtRawView.ReadOnly = false;                
                txtRawView.Text = output;
                webBrowser1.DocumentText = new CodeColorizer().Colorize(Beautify(this.xmlDoc), Languages.Xml);
                txtRawView.ReadOnly = true;                
            }
        }

        private async Task previewNOT()
        {
            if (InvokeRequired)
            {
                Invoke(new Action(() => previewNOT()));
            }
            else
            {  
                this.xmlDoc = new XmlDocument();
                StreamReader reader = new StreamReader(this.inputStream);
                string output = reader.ReadToEnd();
                output = output.Replace("&lt;", "<").Replace("&gt;", ">").Replace("&amp;lt;", "<").Replace("&amp;gt;", ">").Replace("&amp;apos;", "'").Replace("\0", "");
                this.xmlDoc.LoadXml(output);                    
                txtRawView.ReadOnly = false;
                webBrowser1.DocumentText = new CodeColorizer().Colorize(Beautify(this.xmlDoc), Languages.Xml);
                txtRawView.Text = output;                    
                txtRawView.ReadOnly = true;              
            }
        }

        private async Task previewRAW()
        {
            if (InvokeRequired)
            {
                Invoke(new Action(() => previewRAW()));
            }
            else
            {                
                var sr = new StreamReader(this.inputStream);
                var myStr = sr.ReadToEnd();
                txtRawView.ReadOnly = false;
                txtRawView.Text = myStr;
                txtRawView.ReadOnly = true;
            }
        }

        static public string Beautify(XmlDocument doc)
        {
            StringBuilder sb = new StringBuilder();
            XmlWriterSettings settings = new XmlWriterSettings();
            settings.Indent = true;
            settings.IndentChars = "  ";
            settings.NewLineChars = "\r\n";
            settings.NewLineHandling = NewLineHandling.Replace;
            using (XmlWriter writer = XmlWriter.Create(sb, settings))
            {
                doc.Save(writer);
            }
            return sb.ToString();
        }

        private async Task previewHEX()
        {
             if (InvokeRequired)
            {
                Invoke(new Action(() => previewHEX()));
            }
            else
            {
                DynamicFileByteProvider byteProvider = new DynamicFileByteProvider(this.inputStream);
                hexBox1.ByteProvider = byteProvider;
                hexBox1.Visible = true;
                var sr = new StreamReader(this.inputStream);
                var myStr = sr.ReadToEnd();
                txtRawView.ReadOnly = false;
                txtRawView.Text = myStr;
                txtRawView.ReadOnly = true;
            }
        }

        private async Task previewGR2(string fileName)
        {
            if (InvokeRequired)
            {
                Invoke(new Action(() => previewGR2(fileName)));
            }
            else
            {   
                BinaryReader br = new BinaryReader(this.inputStream);
                FileFormats.GR2 gr2_model = new FileFormats.GR2(br, fileName);                                           
                //panelRender.Init();
                panelRender.LoadModel(gr2_model);                
                render = new Thread(panelRender.startRender);
                render.IsBackground = true;
                render.Start();                
            }
        }

        private async Task previewSTB()
        {
            if (InvokeRequired)
            {
                Invoke(new Action(() => previewSTB()));
            }
            else
            {
                BinaryReader br = new BinaryReader(this.inputStream);
                View_STB stb = new View_STB();
                List<STB_Entry> entries = stb.parseSTB(br);
                NodeListItem.resetTreeListViewColumns(treeListView1);
                foreach (STB_Entry entry in entries)
                {
                    rootList.Add(new NodeListItem(entry.ID.ToString(), entry.stringValue));
                }
            }
        }

        private async Task previewDAT()
        {
            if (InvokeRequired)
            {
                Invoke(new Action(() => previewDAT()));
            }
            else
            {
                StreamReader sr = new StreamReader(this.inputStream);                
                var myStr = sr.ReadToEnd();
                txtRawView.ReadOnly = false;
                txtRawView.Text = myStr;
                txtRawView.ReadOnly = true;
                sr.BaseStream.Seek(0, SeekOrigin.Begin);                
                View_DAT dat = new View_DAT();
                rootList = dat.parseDAT(sr);
            }
        }
		
		 private async Task previewACB()
        {
            if (InvokeRequired)
            {
                Invoke(new Action(() => previewACB()));
            }
            else
            {
                BinaryReader br = new BinaryReader(this.inputStream);
                View_ACB acb = new View_ACB();
                List<WEM_File> wems = acb.parseACB(br);
                WemListItem.resetTreeListViewColumns(treeListView1);
                foreach (WEM_File wem in wems)
                {
                    rootList.Add(new WemListItem(wem.name.ToString(), wem));
                }
            }
        }

        private async Task previewBNK()
        {
            if (InvokeRequired)
            {
                Invoke(new Action(() => previewBNK()));
            }
            else
            {
                BinaryReader br = new BinaryReader(this.inputStream);
                View_BNK bnk = new View_BNK();
                List<WEM_File> wems = bnk.parseBNK(br);
                WemListItem.resetTreeListViewColumns(treeListView1);
                foreach (WEM_File wem in wems)
                {
                    rootList.Add(new WemListItem(wem.name.ToString(), wem));
                }
            }
        }

        private async Task previewWEM(string fileName)
        {
            if (InvokeRequired)
            {
                Invoke(new Action(() => previewWEM(fileName)));
            }
            else
            {
                WEM_File wem = new WEM_File(fileName, this.inputStream);
                await Task.Run(() => wem.convertWEM());
                this.btnAudioStop.Enabled = true;
                this.toolStripStatusLabel1.Text = "Playing Audio...";
                this.toolStripProgressBar1.Visible = true;
                await Task.Run(() => playOgg(wem));                
                this.audioState = false;
                this.toolStripStatusLabel1.Text = "Audio Stopped.";
                this.toolStripProgressBar1.Visible = false;
            }

        }
    #endregion

    #region Buttton Methods
        private void btnPreview_Click(object sender, EventArgs e)
        {
            if (this.autoPreview)
            {
                this.autoPreview = false;
                this.btnPreview.Text = "Auto Preview Off";
            }
            else
            {
                this.autoPreview = true;
                this.btnPreview.Text = "Auto Preview On";
            }

        }

        private async void btnExtract_Click(object sender, EventArgs e)
        {
            this.extractCount = 0;
            this.extractPath = txtExtractPath.Text;

            TreeNode node = treeViewFast1.SelectedNode;
            if (node == null)
            {
                MessageBox.Show("Please select a node before trying to extract any objects.", "ERROR: No Node Selected", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
            else
            {
                TreeListItem asset = (TreeListItem)node.Tag;

                if (asset.hashInfo.file != null)
                {
                    showLoader();
                    extractAsset(asset.hashInfo);
                    hideLoader();
                }
                else
                {
                    if (node.Nodes.Count > 0)
                    {
                        string messageText = "";
                        if (this.extractByExtensions)
                        {
                            string temp = String.Join(", ", this.extractExtensions);
                            messageText = "Extract (" + temp + ") objects from " + node.Name + "?";
                        }
                        else
                        {
                            messageText = "Extract all objects from " + node.Name + "?";
                        }
                        if (MessageBox.Show(messageText, "Extract Confirm", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                        {
                            showLoader();
                            await Task.Run(() => extractByNode(node.Nodes));
                            hideLoader();
                            MessageBox.Show("Extracted " + String.Format("{0:n0}", this.extractCount) + " objects", "Extraction Completed", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        }
                    }
                }
            }
        }     

        private void btnViewRaw_Click(object sender, EventArgs e)
        {
            hideViewers();
            txtRawView.Visible = true;
        }

        private async void btnSearch_Click(object sender, EventArgs e)
        {
            this.toolStripStatusLabel1.Text = "Performing Search ...";
            this.searchNodes = this.assetDict.Keys.Where(d => d.Contains(txtSearch.Text)).ToList();
            if (this.searchNodes.Count() > 0)
            {
                this.btnClearSearch.Enabled = true;
                this.btnSearch.Enabled = false;
                this.txtSearch.Enabled = false;
                this.btnFindNext.Enabled = true;
                this.toolStripStatusLabel1.Text = "Found " + (this.searchNodes.Count() + 1) + " Matches";
                showLoader();
                await Task.Run(() => searchTreeNodes());
                hideLoader();
                treeViewFast1.SelectedNode = this.nodeMatch[0];
                treeViewFast1.Focus();
                this.toolStripStatusLabel2.Text = "Item " + (this.searchIndex + 1) + " of " + this.searchNodes.Count();
                this.searchIndex++;
            }
            else
            {
                this.toolStripStatusLabel1.Text = "Search complete";
                MessageBox.Show("Search term not found.");
            }
        }

        private async void btnFindNext_Click(object sender, EventArgs e)
        {
            if (this.searchNodes.ElementAtOrDefault(this.searchIndex) != null)
            {
                await Task.Run(() => searchTreeNodes());
                treeViewFast1.SelectedNode = this.nodeMatch[0];
                treeViewFast1.Focus();
                this.toolStripStatusLabel2.Text = "Item " + (this.searchIndex + 1) + " of " + this.searchNodes.Count();
                this.searchIndex++;
            }
            else
            {
                this.toolStripStatusLabel1.Text = "Search complete";
                MessageBox.Show("No more search terms found");
            }
        }

        private void btnClearSearch_Click(object sender, EventArgs e)
        {
            this.searchNodes = new List<string>();
            this.searchIndex = 0;
            this.txtSearch.Enabled = true;
            this.txtSearch.Text = "";
            this.btnFindNext.Enabled = false;
            this.btnSearch.Enabled = true;
            this.btnClearSearch.Enabled = false;
            this.toolStripStatusLabel2.Text = "";
        }

        private void btnChooseExtract_Click(object sender, EventArgs e)
        {
            FolderBrowserDialog fbd = new FolderBrowserDialog();
            fbd.SelectedPath = txtExtractPath.Text;
            DialogResult result = fbd.ShowDialog();
            txtExtractPath.Text = fbd.SelectedPath + "\\";
        }

        private async void btnViewHex_Click(object sender, EventArgs e)
        {
            hideViewers();
            await Task.Run(() => previewHEX());
            hexBox1.Visible = true;
            Position_Changed(null, null);
        }   
    #endregion

        private async void parseFiles(string extension)
        {
            List<string> assetDictKeys = this.assetDict.Keys.Where(d => d.Contains("." + extension.ToLower())).ToList();            
            List<TreeListItem> matches = new List<TreeListItem>();
            DataObjectModel dom = DomHandler.Instance.getCurrentDOM();

            this.filesSearched = 0;
            
            foreach (string assetKey in assetDictKeys)
            {
                if (assetKey.Split('.').Last().ToUpper() != extension)
                    continue;
                TreeListItem asset;
                if (this.assetDict.TryGetValue(assetKey, out asset))
                {
                    matches.Add(asset);
                }                
            }

            switch (extension)
            {
                case "XML":
                case "MAT":
                    Format_XML_MAT xml_mat_reader = new Format_XML_MAT(this.extractPath, extension);
                    foreach (TreeListItem asset in matches)
                    {
                        this.filesSearched++;
                        Stream assetStream = asset.hashInfo.file.OpenCopyInMemory();
                        xml_mat_reader.ParseXML(assetStream, asset.hashInfo.Directory + "/" + asset.hashInfo.FileName);                
                    }
                    xml_mat_reader.WriteFile();
                    this.namesFound = xml_mat_reader.fileNames.Count();
                    break;
                case "EPP":
                    Format_EPP epp_reader = new Format_EPP(this.extractPath, extension);
                    List<GomObject> eppNodes = dom.GetObjectsStartingWith("epp.");
                    foreach (TreeListItem asset in matches)
                    {
                        this.filesSearched++;
                        Stream assetStream = asset.hashInfo.file.OpenCopyInMemory();
                        epp_reader.ParseEPP(assetStream, asset.hashInfo.Directory + "/" + asset.hashInfo.FileName);                
                    }
                    epp_reader.ParseEPPNodes(eppNodes);
                    epp_reader.WriteFile();
                    this.namesFound = epp_reader.fileNames.Count();
                    break;
                case "PRT":
                    Format_PRT prt_reader = new Format_PRT(this.extractPath, extension);
                    foreach (TreeListItem asset in matches)
                    {                        
                        this.filesSearched++;
                        Stream assetStream = asset.hashInfo.file.OpenCopyInMemory();
                        prt_reader.parsePRT(assetStream, asset.hashInfo.Directory + "/" + asset.hashInfo.FileName);                
                    }
                    prt_reader.WriteFile();
                    this.namesFound = prt_reader.fileNames.Count();
                    break;
                case "GR2":
                    Format_GR2 gr2_reader = new Format_GR2(this.extractPath, extension);
                    foreach (TreeListItem asset in matches)
                    {
                        if (asset.hashInfo.IsNamed)
                            continue;
                        this.filesSearched++;
                        Stream assetStream = asset.hashInfo.file.OpenCopyInMemory();
                        gr2_reader.parseGR2(assetStream, asset.hashInfo.Directory + "/" + asset.hashInfo.FileName, asset.hashInfo.file.Archive);
                    }
                    gr2_reader.WriteFile(true);
                    this.namesFound = gr2_reader.matNames.Count();
                    this.namesFound += gr2_reader.meshNames.Count();
                    break;
                case "BNK":
                    Format_BNK bnk_reader = new Format_BNK(this.extractPath, extension);
                    foreach (TreeListItem asset in matches)
                    {                        
                        this.filesSearched++;
                        Stream assetStream = asset.hashInfo.file.OpenCopyInMemory();
                        bnk_reader.parseBNK(assetStream, asset.hashInfo.Directory + "/" + asset.hashInfo.FileName);
                    }
                    bnk_reader.WriteFile();
                    this.namesFound = bnk_reader.found;
                    break;
                case "DAT":
                    Format_DAT dat_reader = new Format_DAT(this.extractPath, extension);
                    foreach (TreeListItem asset in matches)
                    {
                        this.filesSearched++;
                        Stream assetStream = asset.hashInfo.file.OpenCopyInMemory();
                        dat_reader.parseDAT(assetStream, asset.hashInfo.Directory + "/" + asset.hashInfo.FileName);
                    }
                    dat_reader.WriteFile();
                    this.namesFound = dat_reader.fileNames.Count();
                    break;
                case "CNV":
                    List<GomObject> cnvNodes = dom.GetObjectsStartingWith("cnv.");
                    Format_CNV cnv_node_parser = new Format_CNV(this.extractPath, extension);
                    cnv_node_parser.ParseCNVNodes(cnvNodes);
                    cnv_node_parser.WriteFile();
                    this.namesFound = cnv_node_parser.fileNames.Count();                                    
                    break;
                case "MISC":
                    Format_MISC misc_parser = new Format_MISC(this.extractPath, extension);
                    List<GomObject> ippNodes = dom.GetObjectsStartingWith("ipp.");                    
                    misc_parser.ParseMISC_IPP(ippNodes);
                    List<GomObject> cdxNodes = dom.GetObjectsStartingWith("cdx.");
                    misc_parser.ParseMISC_CDX(cdxNodes);                    
                    misc_parser.WriteFile();
                    this.namesFound = misc_parser.found;
                    break;
                case "MISC_WORLD":
                    Format_MISC misc_world_parser = new Format_MISC(this.extractPath, extension);                    
                    Dictionary<object, object> areaList = dom.GetObject("mapAreasDataProto").Data.Get<Dictionary<object, object>>("mapAreasDataObjectList");
                    List<GomObject> areaList2 = dom.GetObjectsStartingWith("world.areas.");
                    misc_world_parser.ParseMISC_WORLD(areaList2, areaList, dom);
                    areaList.Clear();
                    areaList2.Clear();
                    //GC.Collect();
                    misc_world_parser.WriteFile();
                    this.namesFound = misc_world_parser.found;
                    break;
                case "FXSPEC":
                    Format_FXSPEC fxspec_parser = new Format_FXSPEC(this.extractPath, extension);
                    foreach (TreeListItem asset in matches)
                    {
                        this.filesSearched++;
                        Stream assetStream = asset.hashInfo.file.OpenCopyInMemory();
                        fxspec_parser.ParseFXSPEC(assetStream, asset.hashInfo.Directory + "/" + asset.hashInfo.FileName);
                    }
                    fxspec_parser.WriteFile();
                    this.namesFound = fxspec_parser.fileNames.Count();
                    break;
                case "AMX":
                    Format_AMX amx_parser = new Format_AMX(this.extractPath, extension);
                    foreach (TreeListItem asset in matches)
                    {
                        this.filesSearched++;
                        Stream assetStream = asset.hashInfo.file.OpenCopyInMemory();
                        amx_parser.parseAMX(assetStream, asset.hashInfo.Directory + "/" + asset.hashInfo.FileName);
                    }
                    amx_parser.WriteFile();
                    this.namesFound = amx_parser.fileNames.Count();
                    break;
                default:
                    break;

            }
            this.totalFilesSearched += this.filesSearched;
            this.totalNamesFound += this.namesFound;
            return;
        }      

        private async void btnFindFileNames_Click(object sender, EventArgs e)
        {   
            AssetBrowserTestFile testFile = new AssetBrowserTestFile();
            DialogResult result = testFile.ShowDialog(this);
            if (result == System.Windows.Forms.DialogResult.OK)
            {
                disableUI();
                this.totalFilesSearched = 0;
                this.totalNamesFound = 0;
                this.dataGridView1.Enabled = true;
                List<string> extensions = new List<string>();                
                extensions = testFile.getTypes();                
                showLoader();
                this.toolStripStatusLabel1.Text = "Running File Name Finders ...";
                DataTable dt = new DataTable();
                dt.Columns.Add("File Type");
                dt.Columns.Add("# Searched");
                dt.Columns.Add("# Parsed");
                dataGridView1.DataSource = dt;
                dataGridView1.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.AllCells;             
                foreach (string ext in extensions)
                {
                    this.toolStripStatusLabel2.Text = "Looking for " + ext + " Files";
                    await Task.Run(() => parseFiles(ext));
                    dt.Rows.Add(new string[] { ext,  this.filesSearched.ToString("n0"), this.namesFound.ToString("n0") });
                    this.toolStripStatusLabel2.Text = "Found " + this.namesFound.ToString("n0") + " File Names From " + ext + " Files";
                }
                dt.Rows.Add(new string[] { "Total Parsed", this.totalFilesSearched.ToString("n0"), this.totalNamesFound.ToString("n0") });
                this.toolStripStatusLabel2.Text = "";
                this.toolStripStatusLabel1.Text = "Finding Files Complete";
                this.Refresh();
                this.toolStripStatusLabel1.Text = "Testing Parsed Files...";
                await Task.Run(() => testHashFiles());
                hideViewers();                
                enableUI();
                if (foundFiles.Count() > 0)
                {   
                    txtRawView.Text = "Found Files\r\n\r\n";
                    foreach (string file in foundFiles)
                    {
                        txtRawView.Text += file + "\r\n";
                    }
                    txtRawView.Visible = true;                    
                }
                dt.Rows.Add(new string[] { "Total Files Found", this.foundFiles.Count().ToString("n0") });
                hideLoader();
                string finished = "Parsed " + this.totalNamesFound.ToString("n0") + " Potential File Names\r\n\r\nFound " + this.foundFiles.Count().ToString("n0") + " New Files";
                this.foundNewFileCount += this.foundFiles.Count();
                this.toolStripStatusLabel1.Text = finished;
                MessageBox.Show(finished, "File Finder Complete", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }

        private void AssetBrowser_FormClosing(object sender, FormClosingEventArgs e)
        {
            _closing = true;

            if (hashData.dictionary.needsSave && (this.modNewCount > 2 || this.foundNewFileCount > 0))
            {   
                DialogResult save = MessageBox.Show("The hash dictionary needs to be saved. \nThere were " + this.foundNewFileCount.ToString() + " new files found this session.\n\nSave the dictionary changes?", "Save Dictionary?", MessageBoxButtons.YesNo);
                if (save == DialogResult.Yes)
                {
                    hashData.dictionary.SaveHashList();
                }
            }
        }

        public void AssetBrowser_FormClosed(object sender, FormClosedEventArgs e)
        {
            HashDictionaryInstance.Instance.Unload();

            if (panelRender != null)
            {
                panelRender.stopRender();
                if (render != null)
                {
                    render.Join();
                }
                panelRender.Dispose();
            }
            if (treeViewFast1 != null)
            {
                treeViewFast1.Dispose();
                treeViewFast1 = null;
            }

            if (audioState)
            {
                waveOut.Stop();
            }
            assetDict = null;
            fileDict = null;

            if (System.IO.Directory.Exists(@".\Temp\"))
            {
                var list = System.IO.Directory.GetFiles(@".\Temp\", "*.ogg");
                foreach (var item in list)
                {
                    try
                    {
                        System.IO.File.Delete(item);
                    }
                    catch (IOException oggExcep) { }
                }
                list = System.IO.Directory.GetFiles(@".\Temp\", "*.wem");
                foreach (var item in list)
                {
                    try
                    {
                        System.IO.File.Delete(item);
                    }
                    catch (IOException wemExcep) { }
                }
            }
        }

        private void renderPanel_MouseHover(object sender, EventArgs e)
        {
            if (!_closing)
                renderPanel.Focus();     
        }
                
        private async void testHashFiles(string singleFile = null)
        {
            foundFiles.Clear();
            string[] testFiles;
            if (singleFile != null)
                testFiles = new []{ singleFile };
            else
                testFiles = Directory.GetFiles(this.extractPath + "\\File_Names\\");
            if (testFiles.Count() > 0)
            {
                foreach (string file in testFiles)
                {
                    HashSet<string> testLines = new HashSet<string>();
                    string[] lines = System.IO.File.ReadAllLines(file);
                    foreach (var line in lines)
                    {
                        if (line.Contains("#"))
                        {
                            string[] temp = line.Split('#');
                            if (temp.Length < 3 || temp[2].Length == 0)
                                continue;
                            else
                                testLines.Add(temp[2].ToLower());
                        }
                        else
                        {
                            testLines.Add(line.ToLower());
                        }
                    }

                    foreach (string line in testLines)
                    {                        
                        hasher.Hash(line, 0xdeadbeef);
                        UpdateResults results = this.hashData.dictionary.UpdateHash(hasher.ph, hasher.sh, line, 0);                    
                        if (results == UpdateResults.UPTODATE)
                        {                            
                            continue;
                        }
                        
                        if (results != UpdateResults.NOT_FOUND)
                        {                            
                            this.foundFiles.Add(line);
                        }
                    }
                    testLines.Clear();
                    lines = null;
                    //GC.Collect();
                }
            }
        }

        private async void btnTestFile_Click(object sender, EventArgs e)
        {
            OpenFileDialog ofd = new OpenFileDialog();
            ofd.Filter = "Text Files (.txt)|*.txt|All Files (*.*)|*.*";
            ofd.FilterIndex = 1;
            DialogResult result = ofd.ShowDialog();
            if (result == DialogResult.OK)
            {
                showLoader();
                this.toolStripStatusLabel1.Text = "Testing Hash File...";
                await Task.Run(() => testHashFiles(ofd.FileName));
                hideViewers();
                if (foundFiles.Count() > 0)
                {
                    txtRawView.Text = "Found Files\r\n\r\n";
                    StringBuilder sb = new StringBuilder(Text);
                    foreach (string file in foundFiles)
                    {
                        sb.Append(file);
                        sb.Append("\r\n");
                    }
                    txtRawView.Text = sb.ToString();
                    txtRawView.Visible = true;
                }
                hideLoader();
                string finished = "Found " + this.foundFiles.Count().ToString("n0") + " New Files";
                this.foundNewFileCount += this.foundFiles.Count();
                this.toolStripStatusLabel1.Text = finished;
                MessageBox.Show(finished, "Test Hash File Complete", MessageBoxButtons.OK, MessageBoxIcon.Information);
                enableUI();
            }
        }

        

        private async void treeListView1_ItemSelectionChanged(object sender, ListViewItemSelectionChangedEventArgs e)
        {
            this.audioState = false;
            object testRow = treeListView1.SelectedItem.RowObject;
            //Console.WriteLine(testRow.GetType());
            if (testRow.GetType() == typeof(WemListItem))
            {
                WemListItem row = (WemListItem)testRow;
                WEM_File wem = (WEM_File)row.obj;
                if (wem.data.Count() > 0)
                {
                    this.toolStripStatusLabel1.Text = "Playing Audio...";
                    this.toolStripProgressBar1.Visible = true;
                    this.treeListView1.Enabled = false;
                    await Task.Run(() => wem.convertWEM());
                    this.btnAudioStop.Enabled = true;
                    await Task.Run(() => playOgg(wem));
                    this.btnAudioStop.Enabled = false;
                    this.treeListView1.Enabled = true;
                    this.toolStripProgressBar1.Visible = false;
                    this.toolStripStatusLabel1.Text = "Audio Stopped";
                }
            }
        }

        private void playOgg(WEM_File wem)
        {
            if(this.waveOut == null)
                 this.waveOut = new NAudio.Wave.WaveOutEvent();
            this.audioHasPlayed = true;
            this.waveOut.Init(wem.vorbis);            
            if (this.audioState == false)            
            {
                this.audioState = true;
                SetStripProgressBarStyle(ProgressBarStyle.Continuous);
                SetStripProgressBarMax((int)wem.vorbis.TotalTime.TotalMilliseconds);
                waveOut.Play();
                while (waveOut.PlaybackState != NAudio.Wave.PlaybackState.Stopped)
                {
                    if (this.audioState == false)
                    {
                        waveOut.Stop();
                        break;
                    }
                    else
                    {
                            SetStripProgressBarValue((int)wem.vorbis.CurrentTime.TotalMilliseconds);
                        Thread.Sleep(100);
                    }
                }

                SetStripProgressBarMax(100);
                SetStripProgressBarValue(0);
                SetStripProgressBarStyle(ProgressBarStyle.Marquee);
            }
        }

        private void btnAudioStop_Click(object sender, EventArgs e)
        {
            this.audioState = false;
            this.btnAudioStop.Enabled = false;
        }

        private void btnHelp_Click(object sender, EventArgs e)
        {
            AssetBrowserHelp helpForm = new AssetBrowserHelp();
            helpForm.Show();
        }

        public void setStatusLabel(string message)
        {
            if (this.statusStrip1.InvokeRequired)
            {
                this.statusStrip1.Invoke(new Action(() => setStatusLabel(message))); 
            }
            else
            {
                this.toolStripStatusLabel1.Text = message;
            }
        }

        private void renderPanel_Resize(object sender, EventArgs e)
        {
            if (panelRender != null)
            {
                if (renderPanel.Width != panelRender.ClientWidth || renderPanel.Height != panelRender.ClientHeight)
                    panelRender.SetSize(renderPanel.Height, renderPanel.Width);
            }
        }      

        private void btnHashStatus_Click(object sender, EventArgs e)
        {
            AssetBrowserHashStatus hashStatus = new AssetBrowserHashStatus();
            hashStatus.Show();
        }

        private void extractByExtensionToolStripMenuItem_Click(object sender, EventArgs e)
        {
            AssetBrowserExtractExt frmExt = new AssetBrowserExtractExt();
            DialogResult result = frmExt.ShowDialog(this);
            if (result == DialogResult.OK)
            {
                this.extractByExtensions = true;
                this.extractExtensions = frmExt.getExtensions();
                btnExtract_Click(this, null);
            }
        }

    }    
}
