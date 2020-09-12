using Be.HexEditor;
using Be.Windows.Forms;
using ColorCode;
using DevIL;
using GomLib;
using NAudio.Wave;
using nsHashDictionary;
using nsHasherFunctions;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Xml;
using TorLib;

namespace PugTools
{
    public partial class AssetBrowser : Form
    {
        private bool autoPreview = true;
        private string extractPath;
        private readonly string assetLocation;
        private readonly bool usePTS;

        private readonly HashDictionaryInstance hashData;
        private Dictionary<string, TreeListItem> assetDict = new Dictionary<string, TreeListItem>();

        private Stream inputStream;
        private MemoryStream memStream;
        private XmlDocument xmlDoc;
        // private View_GR2 panelRender;
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

        private readonly Hasher hasher = new Hasher(Hasher.HasherType.TOR);
        private readonly HashSet<string> foundFiles = new HashSet<string>();

        private bool audioState = false;
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
            backgroundWorker2.ProgressChanged += BackgroundWorker2_ProgressChanged;

            //This should probably done by Designer?
            backgroundWorker3.DoWork += BackgroundWorker3_DoWork;
            backgroundWorker3.RunWorkerCompleted += BackgroundWorker3_RunWorkerCompleted;

            hashData = HashDictionaryInstance.Instance;
            if (!hashData.Loaded)
            {
                hashData.Load();
            }

            this.assetLocation = assetLocation;
            this.usePTS = usePTS;

            Config.Load();
            txtExtractPath.Text = Config.ExtractAssetsPath;
            extractPath = txtExtractPath.Text;

            List<object> args = new List<object>
            {
                assetLocation,
                usePTS
            };
            toolStripStatusLabelLeft1.Text = "Loading Assets...";
            ShowLoader();
            treeItemView.CanExpandGetter = delegate (object x)
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
            treeItemView.ChildrenGetter = delegate (object x)
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

        private void BackgroundWorker1_DoWork(object sender, DoWorkEventArgs e)
        {
            if (_closing)
            {
                return;
            }

            System.Runtime.GCSettings.LatencyMode = System.Runtime.GCLatencyMode.LowLatency;

            Assets assets = AssetHandler.Instance.GetCurrentAssets(assetLocation, usePTS);
            DomHandler.Instance.GetCurrentDOM(assets);

            //this.currentAssets = this.currentAssets.GetCurrentAssets((string)args[0], (bool)args[1]);            
        }

        private void BackgroundWorker1_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            if (_closing)
            {
                return;
            }

            toolStripProgressBar1.Style = ProgressBarStyle.Continuous;
            toolStripStatusLabelLeft1.Text = "Loading Files...";
            backgroundWorker2.RunWorkerAsync();
        }

        private void BackgroundWorker2_DoWork(object sender, DoWorkEventArgs e)
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


            Assets currentAssets = AssetHandler.Instance.GetCurrentAssets(assetLocation, usePTS);
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
                        HashFileInfo hashInfo = new HashFileInfo(file.FileInfo.PrimaryHash, file.FileInfo.SecondaryHash, file);
                        string prefixDir = prefixNam + hashInfo.Directory;

                        if (hashInfo.IsNamed)
                        {
                            if (hashInfo.FileName == "metadata.bin" || hashInfo.FileName == "ft.sig" || hashInfo.FileName == "groupmanifest.bin")
                            {
                                continue;
                            }

                            TreeListItem assetAll = new TreeListItem(prefixDir + "/" + hashInfo.FileName, prefixDir, hashInfo.FileName, hashInfo);
                            if (!assetDict.ContainsKey(prefixDir + "/" + hashInfo.FileName))
                                assetDict.Add(prefixDir + "/" + hashInfo.FileName, assetAll);
                            else
                            {
                                // string pausehere = "";
                            }
                            fileDirs.Add(prefixDir);
                            intNamCount++;

                            if (hashInfo.FileState == HashFileInfo.State.New)
                            {
                                TreeListItem assetNew = new TreeListItem(prefixNew + hashInfo.Directory + "/" + hashInfo.FileName, prefixNew + hashInfo.Directory, hashInfo.FileName, hashInfo);
                                string filename = string.Format("{0}{1}/{2}", prefixNew, hashInfo.Directory, hashInfo.FileName);
                                if (!assetDict.ContainsKey(filename))
                                {
                                    assetDict.Add(prefixNew + hashInfo.Directory + "/" + hashInfo.FileName, assetNew);
                                    fileDirs.Add(prefixNew + hashInfo.Directory);
                                    intNewCount++;
                                }
                            }
                            if (hashInfo.FileState == HashFileInfo.State.Modified)
                            {
                                TreeListItem assetMod = new TreeListItem(prefixMod + hashInfo.Directory + "/" + hashInfo.FileName, prefixMod + hashInfo.Directory, hashInfo.FileName, hashInfo);
                                string filename = string.Format("{0}{1}/{2}", prefixMod, hashInfo.Directory, hashInfo.FileName);
                                if (!assetDict.ContainsKey(filename))
                                {
                                    assetDict.Add(prefixMod + hashInfo.Directory + "/" + hashInfo.FileName, assetMod);
                                    fileDirs.Add(prefixMod + hashInfo.Directory);
                                    intModCount++;
                                }
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

            modNewCount = (ulong)(intModCount + intNewCount);

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
                    string output = string.Join("/", temp, 0, intCount2);
                    if (output.Length > 0)
                        allDirs.Add(output);
                }
            }
            foreach (var dir in allDirs)
            {
                string[] temp = dir.Split('/');
                string parentDir = string.Join("/", temp.Take(temp.Length - 1));
                if (parentDir.Length == 0)
                    parentDir = "/root";
                string display = temp.Last();

                TreeListItem asset = new TreeListItem(dir, parentDir, display, empty);
                if (!assetDict.ContainsKey(dir))
                    assetDict.Add(dir.ToString(), asset);
            }
        }

        private void BackgroundWorker2_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            if (_closing)
            {
                return;
            }

            toolStripProgressBar1.Value = 0;
            toolStripProgressBar1.Style = ProgressBarStyle.Marquee;

            toolStripStatusLabelLeft1.Text = "Loading Tree View Items ...";
            backgroundWorker3.RunWorkerAsync();
        }

        private void BackgroundWorker3_DoWork(object sender, DoWorkEventArgs e)
        {
            if (_closing)
            {
                return;
            }

            Func<TreeListItem, string> getId = (x => x.Id);
            Func<TreeListItem, string> getParentId = (x => x.parentId);
            Func<TreeListItem, string> getDisplayName = (x => x.displayName);
            treeViewList.BeginUpdate();
            treeViewList.LoadItems<TreeListItem>(assetDict, getId, getParentId, getDisplayName);
        }

        private void BackgroundWorker3_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            if (_closing)
            {
                return;
            }

            treeViewList.EndUpdate();
            toolStripStatusLabelLeft1.Text = "Loading Complete";
            treeViewList.Visible = true;
            panelRender = new View_GR2(Handle, this, "renderPanel");
            panelRender.Init();

            System.Runtime.GCSettings.LatencyMode = System.Runtime.GCLatencyMode.Interactive;

            HideLoader();
            EnableUI();
            if (treeViewList.Nodes.Count > 0) treeViewList.Nodes[0].Expand();
        }

        #endregion

        #region Input / Output Methods         

        // #pragma warning disable CS1998, CS4014
        // private async Task LoadObject(TorLib.File file)
        private void LoadObject(TorLib.File file)
        {
            inputStream = file.OpenCopyInMemory();
            return;
        }

        // private async void ExtractByNode(TreeNodeCollection nodes)
        private void ExtractByNode(TreeNodeCollection nodes)
        {
            foreach (TreeNode child in nodes)
            {
                TreeListItem asset = (TreeListItem)child.Tag;

                if (asset.hashInfo.File != null)
                {
                    if (extractByExtensions)
                    {
                        if (extractExtensions.Contains(asset.hashInfo.Extension.ToUpper()))
                        {
                            ExtractAsset(asset.hashInfo);
                        }
                        else
                            continue;
                    }
                    else
                        ExtractAsset(asset.hashInfo);
                }

                if (child.Nodes.Count > 0)
                    ExtractByNode(child.Nodes);
            }
        }
        // #pragma warning restore CS1998, CS4014

        private void ExtractAsset(HashFileInfo assetFile)
        {
            if (assetFile.FileName.EndsWith(".bkt"))
                return; // don't need bucket files output all the time
            string fileName;
            string directory;
            if (assetFile.IsNamed)
                fileName = extractPath + string.Join("\\", assetFile.Directory, assetFile.FileName).Replace("/", "\\");
            else
                fileName = extractPath + assetFile.Directory.Replace("/", "\\") + "\\" + assetFile.Extension.ToLower() + "\\" + assetFile.FileName + "." + assetFile.Extension;
            fileName = fileName.Replace("\\\\", "\\");
            directory = Path.GetDirectoryName(fileName);
            if (!Directory.Exists(directory)) { Directory.CreateDirectory(directory); }

            using (Stream file = assetFile.File.Open())
            {
                using (var outputStream = System.IO.File.Create(fileName))
                {
                    byte[] fileBuffer = new byte[assetFile.File.FileInfo.UncompressedSize];
                    file.Read(fileBuffer, 0, fileBuffer.Length);
                    outputStream.Write(fileBuffer, 0, fileBuffer.Length);
                }
            }

            extractCount++;
        }

        // #pragma warning disable CS1998, CS4014
        // private async void SearchTreeNodes()
        private void SearchTreeNodes()
        {
            nodeMatch = treeViewList.Nodes.Find(searchNodes[searchIndex], true);
        }
        // #pragma warning restore CS1998, CS4014

        #endregion

        #region UI Methods
        private void TreeViewList_AfterSelect(object sender, TreeViewEventArgs e)
        {
            TreeNode node = treeViewList.SelectedNode;
            TreeListItem asset = (TreeListItem)node.Tag;
            Text = "Asset Browser - " + asset.Id.ToString();
            if (asset.hashInfo.File != null)
            {
                DataTable dt = new DataTable();
                HashFileInfo info = asset.hashInfo;
                dt.Columns.Add("Property");
                dt.Columns.Add("Value");
                dt.Rows.Add(new string[] { "Archive", info.Source.ToString() });
                dt.Rows.Add(new string[] { "File ID", string.Format("{0:X16}", info.File.FileInfo.FileId) });
                if (info.IsNamed)
                    dt.Rows.Add(new string[] { "File Name", info.FileName });
                else
                    dt.Rows.Add(new string[] { "File Name", info.FileName + "." + info.Extension });
                dt.Rows.Add(new string[] { "File Type", info.Extension });
                dt.Rows.Add(new string[] { "Path", info.Directory });
                dt.Rows.Add(new string[] { "State", info.FileState.ToString() });
                dt.Rows.Add(new string[] { "Compressed Size", info.File.FileInfo.CompressedSize.ToString() });
                dt.Rows.Add(new string[] { "Uncompressed Size", info.File.FileInfo.UncompressedSize.ToString() });
                dt.Rows.Add(new string[] { "Header Size", info.File.FileInfo.HeaderSize.ToString() });
                dt.Rows.Add(new string[] { "Offset", ((long)info.File.FileInfo.Offset).ToString() });
                dt.Rows.Add(new string[] { "Primary Hash", string.Format("{0:X8}", info.File.FileInfo.PrimaryHash) });
                dt.Rows.Add(new string[] { "Secondary Hash", string.Format("{0:X8}", info.File.FileInfo.SecondaryHash) });
                dt.Rows.Add(new string[] { "Checksum", string.Format("{0:X8}", info.File.FileInfo.Checksum) });
                dt.Rows.Add(new string[] { "Is Compressed", info.File.FileInfo.IsCompressed.ToString() });
                dataGridView1.DataSource = dt;
                dataGridView1.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.AllCells;

                if (autoPreview)
                {
                    PreviewAsset(asset);
                }

            }
        }

        private void TreeViewList_MouseUp(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Right)
            {
                treeViewList.SelectedNode = treeViewList.GetNodeAt(e.X, e.Y);

                if (treeViewList.SelectedNode != null)
                {
                    contextMenuStrip1.Show(treeViewList, e.Location);
                }
            }
        }

        private void ExtractToolStripMenuItem_Click(object sender, EventArgs e)
        {
            extractByExtensions = false;
            BtnExtract_Click(this, null);
        }

        public void HideViewers()
        {
            pictureBox1.Visible = false;
            elementHost1.Visible = false;
            txtRawView.Visible = false;
            hexBox1.Visible = false;
            treeItemView.Visible = false;
            renderPanel.Visible = false;
            webBrowser1.Visible = false;
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
            txtSearch.Enabled = true;
            btnSearch.Enabled = true;
            treeViewList.Enabled = true;
            treeItemView.Enabled = true;
            dataGridView1.Enabled = true;
            txtExtractPath.Enabled = true;
            btnChooseExtract.Enabled = true;
            btnPreview.Enabled = true;
            btnExtract.Enabled = true;
            btnSaveTxtHash.Enabled = true;
            btnViewRaw.Enabled = true;
            btnViewHex.Enabled = true;
            btnFindFileNames.Enabled = true;
            btnTestFile.Enabled = true;
            btnFileTable.Enabled = true;
            btnHashStatus.Enabled = true;
            btnHelp.Enabled = true;
        }

        private void DisableUI()
        {
            txtSearch.Enabled = false;
            btnSearch.Enabled = false;
            treeViewList.Enabled = false;
            treeItemView.Enabled = false;
            dataGridView1.Enabled = false;
            txtExtractPath.Enabled = false;
            btnChooseExtract.Enabled = false;
            btnPreview.Enabled = false;
            btnExtract.Enabled = false;
            btnSaveTxtHash.Enabled = false;
            btnViewRaw.Enabled = false;
            btnViewHex.Enabled = false;
            btnFindFileNames.Enabled = false;
            btnTestFile.Enabled = false;
            btnFileTable.Enabled = false;
            btnHashStatus.Enabled = false;
            btnHelp.Enabled = false;
        }

        private void Position_Changed(object sender, EventArgs e)
        {
            toolStripStatusLabelLeft1.Text = string.Format("Ln {0}    Col {1}", hexBox1.CurrentLine, hexBox1.CurrentPositionInLine);

            string bitPresentation = string.Empty;

            byte? currentByte = hexBox1.ByteProvider != null && hexBox1.ByteProvider.Length > hexBox1.SelectionStart
                ? hexBox1.ByteProvider.ReadByte(hexBox1.SelectionStart)
                : (byte?)null;

            BitInfo bitInfo = currentByte != null ? new BitInfo((byte)currentByte, hexBox1.SelectionStart) : null;

            if (bitInfo != null)
            {
                bitPresentation = string.Format("Bits of Byte {0}: {1}"
                    , hexBox1.SelectionStart
                    , bitInfo.ToString()
                    );

                toolStripStatusLabelLeft2.Text = bitInfo.ToString();
            }

            toolStripStatusLabelRight.Text = bitPresentation;
        }

        private void BackgroundWorker2_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            toolStripProgressBar1.Value = e.ProgressPercentage;
        }

        private void SetStripProgressBarValue(int prog)
        {
            if (InvokeRequired)
            {
                Invoke(new Action<int>(SetStripProgressBarValue), new object[] { prog });
                return;
            }

            toolStripProgressBar1.Value = prog;
        }

        private void SetStripProgressBarMax(int prog)
        {
            if (InvokeRequired)
            {
                Invoke(new Action<int>(SetStripProgressBarMax), new object[] { prog });
                return;
            }

            toolStripProgressBar1.Maximum = prog;
        }

        private void SetStripProgressBarStyle(ProgressBarStyle style)
        {
            if (InvokeRequired)
            {
                Invoke(new Action<ProgressBarStyle>(SetStripProgressBarStyle), new object[] { style });
                return;
            }

            toolStripProgressBar1.Style = style;
        }

        #endregion

        #region Preview Methods

        public async void PreviewAsset(TreeListItem asset)
        {
            if (asset.hashInfo.File != null)
            {
                HideViewers();
                ShowLoader();
                toolStripStatusLabelLeft1.Text = "Loading File";
                toolStripStatusLabelRight.Text = "";
                toolStripStatusLabelLeft2.Text = "";
                treeItemView.SelectedIndices.Clear();
                if (render != null)
                {
                    panelRender.StopRender();
                    render.Join();
                    panelRender.Clear();
                }
                await Task.Run(() => LoadObject(asset.hashInfo.File));
                //DynamicFileByteProvider byteProvider = new DynamicFileByteProvider(this.inputStream);
                //hexBox1.ByteProvider = byteProvider;
                //this.inputStream.Position = 0;
                if (asset.hashInfo.Directory == "/resources/systemgenerated/compilednative")
                {
                    await Task.Run(() => PreviewSCPT());
                }
                else
                {
                    switch (asset.hashInfo.Extension.ToUpper())
                    {
                        case "DDS":
                            await Task.Run(() => PreviewDDS());
                            CheckDDSPath(asset.hashInfo.Directory);
                            pictureBox1.Visible = true;
                            break;
                        case "PNG":
                            await Task.Run(() => PreviewPNG());
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
                            await Task.Run(() => PreviewXML());
                            webBrowser1.Visible = true;
                            break;
                        case "NOT":
                            await Task.Run(() => PreviewNOT());
                            webBrowser1.Visible = true;
                            break;
                        case "DAT":
                            //rootList.Clear();
                            //await Task.Run(() => previewDAT());  //disabled as there is a new bianry format for dat files
                            await Task.Run(() => PreviewHEX());
                            //treeItemView.Roots = rootList;
                            //treeItemView.ExpandAll();
                            HideLoader();
                            //txtRawView.Visible = true;
                            //treeItemView.Visible = true;
                            break;
                        case "DYC":
                        case "MAG":
                        case "PRT":
                            txtRawView.Visible = true;
                            await Task.Run(() => PreviewRAW());
                            break;
                        case "GR2":
                            await Task.Run(() => PreviewGR2(asset.hashInfo.FileName));
                            renderPanel.Visible = true;
                            break;
                        case "STB":
                            rootList.Clear();
                            await Task.Run(() => PreviewSTB());
                            treeItemView.Roots = rootList;
                            treeItemView.ExpandAll();
                            HideLoader();
                            treeItemView.Visible = true;
                            break;
                        case "BNK":
                            rootList.Clear();
                            await Task.Run(() => PreviewBNK());
                            treeItemView.Roots = rootList;
                            treeItemView.ExpandAll();
                            HideLoader();
                            treeItemView.Visible = true;
                            break;
                        case "ACB":
                            rootList.Clear();
                            await Task.Run(() => PreviewACB());
                            treeItemView.Roots = rootList;
                            treeItemView.ExpandAll();
                            HideLoader();
                            treeItemView.Visible = true;
                            break;
                        case "WAV":
                        case "WEM":
                            treeViewList.Enabled = false;
                            toolStripStatusLabelLeft1.Text = "Playing Audio...";
                            toolStripProgressBar1.Visible = true;
                            audioState = false;
                            await Task.Run(() => PreviewWEM(asset.hashInfo.FileName));
                            toolStripStatusLabelLeft1.Text = "Audio Stopped";
                            toolStripProgressBar1.Visible = false;
                            treeViewList.Enabled = true;
                            break;
                        case "DEP":
                            toolStripStatusLabelLeft1.Text = "Parsing DEP...";
                            toolStripProgressBar1.Visible = true;
                            await Task.Run(() => PreviewDEP());
                            treeItemView.Roots = rootList;
                            treeItemView.ExpandAll();
                            HideLoader();
                            treeItemView.Visible = true;
                            toolStripProgressBar1.Visible = false;
                            break;
                        case "SCPT":
                            await Task.Run(() => PreviewSCPT());
                            break;
                        case "GFX":
                        case "SWF":
                            await Task.Run(() => PreviewGFX());
                            break;
                        default:
                            await Task.Run(() => PreviewHEX());
                            txtRawView.Visible = true;
                            break;
                    }
                }
                treeItemView.TopItemIndex = 0;
                toolStripStatusLabelLeft1.Text = "File Loaded";
                toolStripStatusLabelRight.Text = "";
                toolStripStatusLabelLeft2.Text = "";
                HideLoader();
            }
        }

#pragma warning disable CS1998, CS4014
        private async Task PreviewDDS()
        {
            if (InvokeRequired)
            {
                Invoke(new Action(() => PreviewDDS()));
            }
            else
            {
                memStream = new MemoryStream();
                ImageImporter imp = new ImageImporter();
                ImageExporter exp = new ImageExporter();
                DevIL.Image dds = imp.LoadImageFromStream(ImageType.Dds, inputStream);
                exp.SaveImageToStream(dds, ImageType.Png, memStream);
                Bitmap bm = new Bitmap(memStream);
                pictureBox1.Image = bm;
            }
        }
#pragma warning restore CS1998, CS4014

        private void CheckDDSPath(string directory)
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
                pictureBox1.BackgroundImage = Properties.Resources.Transparent;
            }
        }

#pragma warning disable CS1998, CS4014
        private async Task PreviewDEP()
        {
            if (InvokeRequired)
            {
                Invoke(new Action(() => PreviewDEP()));
            }
            else
            {
                BinaryReader br = new BinaryReader(inputStream);
                List<DEP_Entry> entires = View_DEP.Read(br, hashData.dictionary);
                NodeListItem.ResetTreeListViewColumns(treeItemView);
                foreach (DEP_Entry entry in entires)
                {
                    rootList.Add(new NodeListItem(entry.Filename, entry));
                }
            }
        }

        private async Task PreviewPNG()
        {
            if (InvokeRequired)
            {
                Invoke(new Action(() => PreviewPNG()));
            }
            else
            {
                memStream = new MemoryStream();
                ImageImporter imp = new ImageImporter();
                ImageExporter exp = new ImageExporter();
                DevIL.Image png = imp.LoadImageFromStream(ImageType.Png, inputStream);
                exp.SaveImageToStream(png, ImageType.Bmp, memStream);
                Bitmap bm = new Bitmap(memStream);
                pictureBox1.Image = bm;
            }
        }

        private async Task PreviewXML()
        {
            if (InvokeRequired)
            {
                Invoke(new Action(() => PreviewXML()));
            }
            else
            {
                xmlDoc = new XmlDocument();
                StreamReader reader = new StreamReader(inputStream);
                string output = reader.ReadToEnd();
                output = output.Replace("&lt;", "<").Replace("&gt;", ">").Replace("&amp;lt;", "<").Replace("&amp;gt;", ">").Replace("&amp;apos;", "'").Replace("\0", "");
                xmlDoc.LoadXml(output);
                txtRawView.ReadOnly = false;
                txtRawView.Text = output;
                webBrowser1.DocumentText = new CodeColorizer().Colorize(Beautify(xmlDoc), Languages.Xml);
                txtRawView.ReadOnly = true;
            }
        }

        private async Task PreviewNOT()
        {
            if (InvokeRequired)
            {
                Invoke(new Action(() => PreviewNOT()));
            }
            else
            {
                xmlDoc = new XmlDocument();
                StreamReader reader = new StreamReader(inputStream);
                string output = reader.ReadToEnd();
                output = output.Replace("&lt;", "<").Replace("&gt;", ">").Replace("&amp;lt;", "<").Replace("&amp;gt;", ">").Replace("&amp;apos;", "'").Replace("\0", "");
                xmlDoc.LoadXml(output);
                txtRawView.ReadOnly = false;
                webBrowser1.DocumentText = new CodeColorizer().Colorize(Beautify(xmlDoc), Languages.Xml);
                txtRawView.Text = output;
                txtRawView.ReadOnly = true;
            }
        }

        private async Task PreviewRAW()
        {
            if (InvokeRequired)
            {
                Invoke(new Action(() => PreviewRAW()));
            }
            else
            {
                var sr = new StreamReader(inputStream);
                var myStr = sr.ReadToEnd();
                txtRawView.ReadOnly = false;
                txtRawView.Text = myStr;
                txtRawView.ReadOnly = true;
            }
        }

        private async Task PreviewSCPT()
        {
            if (InvokeRequired)
            {
                Invoke(new Action(() => PreviewSCPT()));
            }
            else
            {
                BinaryReader br = new BinaryReader(inputStream);
                MemoryStream ms = View_SCPT.DecryptSCPT(br);
                DynamicFileByteProvider byteProvider = new DynamicFileByteProvider(ms);
                hexBox1.ByteProvider = byteProvider;
                hexBox1.Visible = true;
            }
        }

        private async Task PreviewGFX()
        {
            if (InvokeRequired)
            {
                Invoke(new Action(() => PreviewGFX()));
            }
            else
            {
                BinaryReader br = new BinaryReader(inputStream);
                MemoryStream ms = View_GFX.DecompressGFX(br);
                DynamicFileByteProvider byteProvider = new DynamicFileByteProvider(ms);
                hexBox1.ByteProvider = byteProvider;
                hexBox1.Visible = true;
            }
        }
#pragma warning restore CS1998, CS4014

        static public string Beautify(XmlDocument doc)
        {
            StringBuilder sb = new StringBuilder();
            XmlWriterSettings settings = new XmlWriterSettings
            {
                Indent = true,
                IndentChars = "  ",
                NewLineChars = "\r\n",
                NewLineHandling = NewLineHandling.Replace
            };
            using (XmlWriter writer = XmlWriter.Create(sb, settings))
            {
                doc.Save(writer);
            }
            return sb.ToString();
        }

#pragma warning disable CS1998, CS4014
        private async Task PreviewHEX()
        {
            if (InvokeRequired)
            {
                Invoke(new Action(() => PreviewHEX()));
            }
            else
            {
                DynamicFileByteProvider byteProvider = new DynamicFileByteProvider(inputStream);
                hexBox1.ByteProvider = byteProvider;
                hexBox1.Visible = true;
                var sr = new StreamReader(inputStream);
                var myStr = sr.ReadToEnd();
                txtRawView.ReadOnly = false;
                txtRawView.Text = myStr;
                txtRawView.ReadOnly = true;
            }
        }

        private async Task PreviewGR2(string fileName)
        {
            if (InvokeRequired)
            {
                Invoke(new Action(() => PreviewGR2(fileName)));
            }
            else
            {
                BinaryReader br = new BinaryReader(inputStream);
                FileFormats.GR2 gr2_model = new FileFormats.GR2(br, fileName);
                //panelRender.Init();

                if (gr2_model.materials.Count == 0)
                {
                    foreach (FileFormats.GR2_Mesh mesh in gr2_model.meshes)
                    {
                        if (mesh.meshName.Contains("collision"))
                            continue;
                        else
                            gr2_model.numMaterials = mesh.numPieces;
                    }

                    if (gr2_model.numMaterials == 1)
                        gr2_model.materials = new List<FileFormats.GR2_Material>
                            {
                                new FileFormats.GR2_Material("all_test_grey_128")
                            };

                    if (gr2_model.numMaterials == 2)
                    {
                        gr2_model.materials = new List<FileFormats.GR2_Material>
                            {
                                new FileFormats.GR2_Material("all_test_grey_128"),
                                new FileFormats.GR2_Material("defaultMirror")
                            };
                    }
                }

                if (gr2_model.materials.Count > 0)
                {
                    if (gr2_model.materials[0].materialName == "default")
                        gr2_model.materials[0] = new FileFormats.GR2_Material("all_test_grey_128");

                    // if (gr2_model.materials.Count > 1)
                    //     if (gr2_model.materials[1].materialName == "defaultMirror")
                    //         gr2_model.materials[1] = new GR2_Material("defaultMirror");
                }

                Dictionary<string, FileFormats.GR2> models = new Dictionary<string, FileFormats.GR2>
                {
                    { fileName, gr2_model }
                };

                Dictionary<string, object> resources = new Dictionary<string, object>();

                panelRender.LoadModel(gr2_model);
                render = new Thread(panelRender.StartRender)
                {
                    IsBackground = true
                };
                render.Start();
            }
        }

        private async Task PreviewSTB()
        {
            if (InvokeRequired)
            {
                Invoke(new Action(() => PreviewSTB()));
            }
            else
            {
                BinaryReader br = new BinaryReader(inputStream);
                View_STB stb = new View_STB();
                List<STB_Entry> entries = stb.ParseSTB(br);
                NodeListItem.ResetTreeListViewColumns(treeItemView);
                foreach (STB_Entry entry in entries)
                {
                    rootList.Add(new NodeListItem(entry.ID.ToString(), entry.StringValue));
                }
            }
        }

        private async Task PreviewDAT()
        {
            if (InvokeRequired)
            {
                Invoke(new Action(() => PreviewDAT()));
            }
            else
            {
                StreamReader sr = new StreamReader(inputStream);
                var myStr = sr.ReadToEnd();
                txtRawView.ReadOnly = false;
                txtRawView.Text = myStr;
                txtRawView.ReadOnly = true;
                sr.BaseStream.Seek(0, SeekOrigin.Begin);
                View_DAT dat = new View_DAT();
                rootList = dat.ParseDAT(sr);
            }
        }

        private async Task PreviewACB()
        {
            if (InvokeRequired)
            {
                Invoke(new Action(() => PreviewACB()));
            }
            else
            {
                BinaryReader br = new BinaryReader(inputStream);
                View_ACB acb = new View_ACB();
                List<WEM_File> wems = acb.ParseACB(br);
                WemListItem.ResetTreeListViewColumns(treeItemView);
                foreach (WEM_File wem in wems)
                {
                    rootList.Add(new WemListItem(wem.Name.ToString(), wem));
                }

                //Resize to content.
                olvColumn1.AutoResize(ColumnHeaderAutoResizeStyle.ColumnContent);
            }
        }

        private async Task PreviewBNK()
        {
            if (InvokeRequired)
            {
                Invoke(new Action(() => PreviewBNK()));
            }
            else
            {
                BinaryReader br = new BinaryReader(inputStream);
                FileFormat_BNK bnk = new FileFormat_BNK(br, true);
                List<WEM_File> wems = new List<WEM_File>();
                if (bnk.didx != null && bnk.didx.wems.Count() > 0)
                    wems = bnk.didx.wems;
                WemListItem.ResetTreeListViewColumns(treeItemView);
                if (bnk.hirc != null)
                {
                    WemListItem hirc = new WemListItem("HIRC", bnk.hirc);
                    rootList.Add(hirc);
                }
                if (bnk.didx != null)
                {
                    WemListItem didx = new WemListItem("DIDX", bnk.didx);
                    rootList.Add(didx);
                }
                if (bnk.stid != null)
                {
                    WemListItem stid = new WemListItem("STID", bnk.stid);
                    rootList.Add(stid);
                }
            }
        }
#pragma warning restore CS1998, CS4014

        private async Task PreviewWEM(string fileName)
        {
            if (InvokeRequired)
            {
                _ = Invoke(new Action(async () => await PreviewWEM(fileName)));
            }
            else
            {
                WEM_File wem = new WEM_File(fileName, inputStream);
                await Task.Run(() => wem.ConvertWEM());
                btnAudioStop.Enabled = true;
                toolStripStatusLabelLeft1.Text = "Playing Audio...";
                toolStripProgressBar1.Visible = true;
                await Task.Run(() => PlayOgg(wem));
                audioState = false;
                toolStripStatusLabelLeft1.Text = "Audio Stopped.";
                toolStripProgressBar1.Visible = false;
            }

        }
        #endregion

        #region Buttton Methods
        private void BtnPreview_Click(object sender, EventArgs e)
        {
            if (autoPreview)
            {
                autoPreview = false;
                btnPreview.Text = "Auto Preview Off";
            }
            else
            {
                autoPreview = true;
                btnPreview.Text = "Auto Preview On";
            }

        }

        private async void BtnExtract_Click(object sender, EventArgs e)
        {
            extractCount = 0;
            extractPath = txtExtractPath.Text;

            TreeNode node = treeViewList.SelectedNode;
            if (node == null)
            {
                MessageBox.Show("Please select a node before trying to extract any objects.", "ERROR: No Node Selected", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
            else
            {
                TreeListItem asset = (TreeListItem)node.Tag;

                if (asset.hashInfo.File != null)
                {
                    ShowLoader();
                    ExtractAsset(asset.hashInfo);
                    HideLoader();
                }
                else
                {
                    if (node.Nodes.Count > 0)
                    {
                        string messageText = "";
                        if (extractByExtensions)
                        {
                            string temp = string.Join(", ", extractExtensions);
                            messageText = "Extract (" + temp + ") objects from " + node.Name + "?";
                        }
                        else
                        {
                            messageText = "Extract all objects from " + node.Name + "?";
                        }
                        if (MessageBox.Show(messageText, "Extract Confirm", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                        {
                            ShowLoader();
                            await Task.Run(() => ExtractByNode(node.Nodes));
                            HideLoader();
                            MessageBox.Show("Extracted " + string.Format("{0:n0}", extractCount) + " objects", "Extraction Completed", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        }
                    }
                }
            }
        }

        private void BtnSaveTxtHash_Click(object sender, EventArgs e)
        {
            hashData.dictionary.SaveTxtHashList();
            MessageBox.Show("Saved hashes_filenames.txt");
        }

        private void BtnViewRaw_Click(object sender, EventArgs e)
        {
            HideViewers();
            txtRawView.Visible = true;
        }

        private async void BtnSearch_Click(object sender, EventArgs e)
        {
            toolStripStatusLabelLeft1.Text = "Performing Search ...";
            searchNodes = assetDict.Keys.Where(d => d.Contains(txtSearch.Text)).ToList();
            if (searchNodes.Count() > 0)
            {
                btnClearSearch.Enabled = true;
                btnSearch.Enabled = false;
                txtSearch.Enabled = false;
                btnFindNext.Enabled = true;
                toolStripStatusLabelLeft1.Text = "Found " + (searchNodes.Count() + 1) + " Matches";
                ShowLoader();
                await Task.Run(() => SearchTreeNodes());
                HideLoader();
                treeViewList.SelectedNode = nodeMatch[0];
                treeViewList.Focus();
                toolStripStatusLabelRight.Text = "Item " + (searchIndex + 1) + " of " + searchNodes.Count();
                searchIndex++;
            }
            else
            {
                toolStripStatusLabelLeft1.Text = "Search complete";
                MessageBox.Show("Search term not found.");
            }
        }

        private async void BtnFindNext_Click(object sender, EventArgs e)
        {
            if (searchNodes.ElementAtOrDefault(searchIndex) != null)
            {
                await Task.Run(() => SearchTreeNodes());
                treeViewList.SelectedNode = nodeMatch[0];
                treeViewList.Focus();
                toolStripStatusLabelRight.Text = "Item " + (searchIndex + 1) + " of " + searchNodes.Count();
                searchIndex++;
            }
            else
            {
                toolStripStatusLabelLeft1.Text = "Search complete";
                MessageBox.Show("No more search terms found");
            }
        }

        private void BtnClearSearch_Click(object sender, EventArgs e)
        {
            searchNodes = new List<string>();
            searchIndex = 0;
            txtSearch.Enabled = true;
            txtSearch.Text = "";
            btnFindNext.Enabled = false;
            btnSearch.Enabled = true;
            btnClearSearch.Enabled = false;
            toolStripStatusLabelRight.Text = "";
        }

        private void BtnChooseExtract_Click(object sender, EventArgs e)
        {
            FolderBrowserDialog fbd = new FolderBrowserDialog
            {
                SelectedPath = txtExtractPath.Text
            };
            _ = fbd.ShowDialog();
            txtExtractPath.Text = fbd.SelectedPath + "\\";
        }

        private async void BtnViewHex_Click(object sender, EventArgs e)
        {
            HideViewers();
            await Task.Run(() => PreviewHEX());
            hexBox1.Visible = true;
            Position_Changed(null, null);
        }
        #endregion

        // #pragma warning disable CS1998, CS4014
        // private async void ParseFiles(string extension)
        private void ParseFiles(string extension)
        {
            List<string> assetDictKeys = assetDict.Keys.Where(d => d.Contains("." + extension.ToLower())).ToList();
            List<TreeListItem> matches = new List<TreeListItem>();
            DataObjectModel dom = DomHandler.Instance.GetCurrentDOM();

            filesSearched = 0;

            foreach (string assetKey in assetDictKeys)
            {
                if (assetKey.Split('.').Last().ToUpper() != extension)
                    continue;
                if (assetDict.TryGetValue(assetKey, out TreeListItem asset))
                {
                    matches.Add(asset);
                }
            }

            switch (extension)
            {
                case "XML":
                case "MAT":
                    Format_XML_MAT xml_mat_reader = new Format_XML_MAT(extractPath, extension);
                    foreach (TreeListItem asset in matches)
                    {
                        filesSearched++;
                        Stream assetStream = asset.hashInfo.File.OpenCopyInMemory();
                        xml_mat_reader.ParseXML(assetStream, asset.hashInfo.Directory + "/" + asset.hashInfo.FileName);
                    }
                    namesFound = xml_mat_reader.fileNames.Count + xml_mat_reader.animFileNames.Count;
                    xml_mat_reader.WriteFile();
                    break;
                case "EPP":
                    Format_EPP epp_reader = new Format_EPP(extractPath, extension);
                    List<GomObject> eppNodes = dom.GetObjectsStartingWith("epp.");
                    foreach (TreeListItem asset in matches)
                    {
                        filesSearched++;
                        Stream assetStream = asset.hashInfo.File.OpenCopyInMemory();
                        epp_reader.ParseEPP(assetStream, asset.hashInfo.Directory + "/" + asset.hashInfo.FileName);
                    }
                    epp_reader.ParseEPPNodes(eppNodes);
                    namesFound = epp_reader.fileNames.Count;
                    epp_reader.WriteFile();
                    break;
                case "PRT":
                    Format_PRT prt_reader = new Format_PRT(extractPath, extension);
                    foreach (TreeListItem asset in matches)
                    {
                        filesSearched++;
                        Stream assetStream = asset.hashInfo.File.OpenCopyInMemory();
                        prt_reader.ParsePRT(assetStream, asset.hashInfo.Directory + "/" + asset.hashInfo.FileName);
                    }
                    namesFound = prt_reader.fileNames.Count;
                    prt_reader.WriteFile();
                    break;
                case "GR2":
                    Format_GR2 gr2_reader = new Format_GR2(extractPath, extension);
                    foreach (TreeListItem asset in matches)
                    {
                        if (asset.hashInfo.IsNamed)
                            continue;
                        filesSearched++;
                        Stream assetStream = asset.hashInfo.File.OpenCopyInMemory();
                        gr2_reader.ParseGR2(assetStream, asset.hashInfo.Directory + "/" + asset.hashInfo.FileName, asset.hashInfo.File.Archive);
                    }
                    namesFound = gr2_reader.matNames.Count + gr2_reader.meshNames.Count;
                    gr2_reader.WriteFile(true);
                    break;
                case "BNK":
                    Format_BNK bnk_reader = new Format_BNK(extractPath, extension);
                    foreach (TreeListItem asset in matches)
                    {
                        filesSearched++;
                        Stream assetStream = asset.hashInfo.File.OpenCopyInMemory();
                        bnk_reader.ParseBNK(assetStream, asset.hashInfo.Directory + "/" + asset.hashInfo.FileName);
                    }
                    namesFound = bnk_reader.found;
                    bnk_reader.WriteFile();
                    break;
                case "DAT":
                    Format_DAT dat_reader = new Format_DAT(extractPath, extension);
                    foreach (TreeListItem asset in matches)
                    {
                        filesSearched++;
                        Stream assetStream = asset.hashInfo.File.OpenCopyInMemory();
                        dat_reader.ParseDAT(assetStream, asset.hashInfo.Directory + "/" + asset.hashInfo.FileName, this);
                    }
                    namesFound = dat_reader.fileNames.Count;
                    dat_reader.WriteFile();
                    break;
                case "CNV":
                    List<GomObject> cnvNodes = dom.GetObjectsStartingWith("cnv.");
                    Format_CNV cnv_node_parser = new Format_CNV(extractPath, extension);
                    cnv_node_parser.ParseCNVNodes(cnvNodes);
                    namesFound = cnv_node_parser.fileNames.Count + cnv_node_parser.animNames.Count + cnv_node_parser.fxSpecNames.Count;
                    filesSearched += cnvNodes.Count();
                    cnv_node_parser.WriteFile();
                    cnvNodes.Clear();
                    break;
                case "MISC":
                    Format_MISC misc_parser = new Format_MISC(extractPath, extension);
                    List<GomObject> ippNodes = dom.GetObjectsStartingWith("ipp.");
                    misc_parser.ParseMISC_IPP(ippNodes);
                    List<GomObject> cdxNodes = dom.GetObjectsStartingWith("cdx.");
                    misc_parser.ParseMISC_CDX(cdxNodes);
                    Dictionary<string, DomType> nodeDict;
                    dom.nodeLookup.TryGetValue(typeof(GomObject), out nodeDict);
                    misc_parser.ParseMISC_NODE(nodeDict);
                    GomObject ldgNode = dom.Get<GomObject>("loadingAreaLoadScreenPrototype");
                    //nodeDict.Clear(); //this was destroying dom.nodeLookup causing an annoyingly hard to locate exception.
                    Dictionary<object, object> itemApperances = dom.GetObject("itmAppearanceDatatable").Data.Get<Dictionary<object, object>>("itmAppearances");
                    misc_parser.ParseMISC_LdnScn(ldgNode);
                    misc_parser.ParseMISC_ITEM(itemApperances);
                    misc_parser.ParseMISC_TUTORIAL(dom);
                    misc_parser.WriteFile();
                    namesFound = misc_parser.found;
                    filesSearched += misc_parser.searched;
                    break;
                case "MISC_WORLD":
                    Format_MISC misc_world_parser = new Format_MISC(extractPath, extension);
                    Dictionary<object, object> areaList = dom.GetObject("mapAreasDataProto").Data.Get<Dictionary<object, object>>("mapAreasDataObjectList");
                    List<GomObject> areaList2 = dom.GetObjectsStartingWith("world.areas.");
                    misc_world_parser.ParseMISC_WORLD(areaList2, areaList, dom);
                    areaList.Clear();
                    areaList2.Clear();
                    misc_world_parser.WriteFile();
                    namesFound = misc_world_parser.found;
                    break;
                case "FXSPEC":
                    Format_FXSPEC fxspec_parser = new Format_FXSPEC(extractPath, extension);
                    foreach (TreeListItem asset in matches)
                    {
                        filesSearched++;
                        Stream assetStream = asset.hashInfo.File.OpenCopyInMemory();
                        fxspec_parser.ParseFXSPEC(assetStream, asset.hashInfo.Directory + "/" + asset.hashInfo.FileName);
                    }
                    namesFound = fxspec_parser.fileNames.Count();
                    fxspec_parser.WriteFile();
                    break;
                case "AMX":
                    Format_AMX amx_parser = new Format_AMX(extractPath, extension);
                    foreach (TreeListItem asset in matches)
                    {
                        filesSearched++;
                        Stream assetStream = asset.hashInfo.File.OpenCopyInMemory();
                        amx_parser.ParseAMX(assetStream, asset.hashInfo.Directory + "/" + asset.hashInfo.FileName);
                    }
                    namesFound = amx_parser.fileNames.Count();
                    amx_parser.WriteFile();
                    break;
                case "SDEF":
                    Format_SDEF sdef_parser = new Format_SDEF(extractPath, extension);
                    TorLib.File sdef = AssetHandler.Instance.GetCurrentAssets().FindFile("/resources/systemgenerated/scriptdef.list");
                    sdef_parser.ParseSDEF(sdef.OpenCopyInMemory());
                    sdef_parser.WriteFile();
                    namesFound = sdef_parser.found;
                    filesSearched = 1;
                    break;
                case "HYD":
                    List<GomObject> hydNodes = dom.GetObjectsStartingWith("hyd.");
                    Format_HYD hyd_parser = new Format_HYD(extractPath, extension);
                    hyd_parser.ParseHYD(hydNodes);
                    namesFound = hyd_parser.animFileNames.Count + hyd_parser.vfxFileNames.Count;
                    filesSearched += hydNodes.Count();
                    hyd_parser.WriteFile();
                    hydNodes.Clear();
                    break;
                case "DYN":
                    List<GomObject> dynNodes = dom.GetObjectsStartingWith("dyn.");
                    Format_DYN dyn_parser = new Format_DYN(extractPath, extension);
                    dyn_parser.ParseDYN(dynNodes);
                    namesFound = dyn_parser.fileNames.Count + dyn_parser.unknownFileNames.Count;
                    filesSearched += dynNodes.Count();
                    dyn_parser.WriteFile();
                    break;
                case "ICONS":
                    Format_ICONS icon_parser = new Format_ICONS(extractPath, extension);
                    icon_parser.ParseICONS(dom);
                    namesFound = icon_parser.fileNames.Count;
                    filesSearched += icon_parser.searched;
                    icon_parser.WriteFile();
                    break;
                case "PLC":
                    List<GomObject> plcNodes = dom.GetObjectsStartingWith("plc.");
                    Format_PLC plc_parser = new Format_PLC(extractPath, extension);
                    plc_parser.ParsePLC(plcNodes);
                    namesFound = plc_parser.fileNames.Count;
                    filesSearched += plcNodes.Count();
                    plc_parser.WriteFile();
                    break;
                case "STB":
                    Format_STB stb_parser = new Format_STB(extractPath, extension);
                    TorLib.File manifest = AssetHandler.Instance.GetCurrentAssets().FindFile("/resources/gamedata/str/stb.manifest");
                    stb_parser.ParseSTBManifest(manifest.OpenCopyInMemory());
                    namesFound = stb_parser.fileNames.Count;
                    filesSearched += 1;
                    stb_parser.WriteFile();
                    break;
                default:
                    break;

            }
            totalFilesSearched += filesSearched;
            totalNamesFound += namesFound;
            return;
        }
        // #pragma warning restore CS1998, CS4014

        private async void BtnFindFileNames_Click(object sender, EventArgs e)
        {
            AssetBrowserTestFile testFile = new AssetBrowserTestFile();
            DialogResult result = testFile.ShowDialog(this);
            if (result == DialogResult.OK)
            {
                DisableUI();
                totalFilesSearched = 0;
                totalNamesFound = 0;
                dataGridView1.Enabled = true;
                List<string> extensions = new List<string>();
                extensions = testFile.GetTypes();
                ShowLoader();
                toolStripStatusLabelLeft1.Text = "Running File Name Finders ...";
                DataTable dt = new DataTable();
                dt.Columns.Add("File Type");
                dt.Columns.Add("# Searched");
                dt.Columns.Add("# Parsed");
                dataGridView1.DataSource = dt;
                dataGridView1.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.AllCells;
                foreach (string ext in extensions)
                {
                    toolStripStatusLabelRight.Text = "Looking for " + ext + " Files";
                    await Task.Run(() => ParseFiles(ext));
                    dt.Rows.Add(new string[] { ext, filesSearched.ToString("n0"), namesFound.ToString("n0") });
                    toolStripStatusLabelRight.Text = "Found " + namesFound.ToString("n0") + " File Names From " + ext + " Files";
                }
                dt.Rows.Add(new string[] { "Total Parsed", totalFilesSearched.ToString("n0"), totalNamesFound.ToString("n0") });
                toolStripStatusLabelRight.Text = "";
                toolStripStatusLabelLeft1.Text = "Finding Files Complete";
                Refresh();
                toolStripStatusLabelLeft1.Text = "Testing Parsed Files...";
                await Task.Run(() => TestHashFiles());
                HideViewers();
                EnableUI();
                if (foundFiles.Count() > 0)
                {
                    txtRawView.Text = "Found Files\r\n\r\n";
                    txtRawView.Text += string.Join("\r\n", foundFiles);
                    txtRawView.Visible = true;
                }
                dt.Rows.Add(new string[] { "Total Files Found", foundFiles.Count().ToString("n0") });
                HideLoader();
                string finished = "Parsed " + totalNamesFound.ToString("n0") + " Potential File Names\r\n\r\nFound " + foundFiles.Count().ToString("n0") + " New Files";
                foundNewFileCount += foundFiles.Count();
                toolStripStatusLabelLeft1.Text = finished;
                MessageBox.Show(finished, "File Finder Complete", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }

        private void AssetBrowser_FormClosing(object sender, FormClosingEventArgs e)
        {
            System.Runtime.GCSettings.LatencyMode = System.Runtime.GCLatencyMode.LowLatency;
            _closing = true;

            if (hashData.dictionary.needsSave && (modNewCount > 2 || foundNewFileCount > 0))
            {
                DialogResult save = MessageBox.Show("The hash dictionary needs to be saved. \nThere were " + foundNewFileCount.ToString() + " new files found this session.\n\nSave the dictionary changes?", "Save Dictionary?", MessageBoxButtons.YesNo);
                if (save == DialogResult.Yes)
                {
                    hashData.dictionary.SaveBinHashList();
                }
            }
        }

        public void AssetBrowser_FormClosed(object sender, FormClosedEventArgs e)
        {
            //Reload hash list to ensure other code is unaffected.
            HashDictionaryInstance.Instance.Unload();
            //HashDictionaryInstance.Instance.Load();

            if (panelRender != null)
            {
                panelRender.StopRender();
                if (render != null)
                {
                    render.Join();
                }
                panelRender.Dispose();
            }
            if (treeViewList != null)
            {
                treeViewList.Dispose();
                treeViewList = null;
            }

            if (audioState)
            {
                waveOut.Stop();
            }
            assetDict = null;

            if (Directory.Exists(@".\Temp\"))
            {
                var list = Directory.GetFiles(@".\Temp\", "*.ogg");
                foreach (var item in list)
                {
                    try
                    {
                        System.IO.File.Delete(item);
                    }
                    catch (IOException) { }
                }
                list = Directory.GetFiles(@".\Temp\", "*.wem");
                foreach (var item in list)
                {
                    try
                    {
                        System.IO.File.Delete(item);
                    }
                    catch (IOException) { }
                }
            }

            System.Runtime.GCSettings.LatencyMode = System.Runtime.GCLatencyMode.Interactive;
        }

        private void RenderPanel_MouseHover(object sender, EventArgs e)
        {
            if (!_closing)
                renderPanel.Focus();
        }

        // #pragma warning disable CS1998, CS4014
        // private async void TestHashFiles(string singleFile = null)
        private void TestHashFiles(string singleFile = null)
        {
            hashData.dictionary.SaveBinHashList();
            foundFiles.Clear();
            string[] testFiles;
            if (singleFile != null)
                testFiles = new[] { singleFile };
            else
                testFiles = Directory.GetFiles(extractPath + "\\File_Names\\");
            if (testFiles.Count() > 0)
            {
                foreach (string file in testFiles)
                {
                    HashSet<string> testLines = new HashSet<string>();
                    if (file.EndsWith(".bin")) //import jedipedia hashes.bin format
                    {
                        using (FileStream fs = new FileStream(file, FileMode.Open))
                        {
                            using (BinaryReader br = new BinaryReader(fs))
                            {
                                while (br.BaseStream.Position != br.BaseStream.Length)
                                {
                                    br.ReadUInt32(); //ph
                                    br.ReadUInt32(); //sh
                                    var len = br.ReadByte(); //filename length
                                    var nul = br.ReadByte();
                                    if (nul != 0x00)
                                    {
                                        // string second_len = "????";
                                    }
                                    var filename = Encoding.Default.GetString(br.ReadBytes(len));
                                    testLines.Add(filename.ToLower());
                                }
                            }
                        }
                    }
                    else
                    {

                        string[] lines = System.IO.File.ReadAllLines(file);
                        foreach (var line in lines)
                        {
                            if (line.Contains("#")) //old hash dict format
                            {
                                string[] temp = line.Split('#');
                                if (temp.Length < 3 || temp[2].Length == 0)
                                    continue;
                                else
                                    testLines.Add(temp[2].ToLower());
                            }
                            else if (line.Contains("?")) //new hash dict format
                            {
                                string[] temp = line.Split('?');
                                if (temp.Length < 4 || temp[3].Length == 0)
                                    continue;
                                else
                                    testLines.Add(temp[3].ToLower());
                            }
                            else
                            {
                                testLines.Add(line.ToLower());
                            }
                        }
                    }
                    hashData.dictionary.CreateArchiveHashMasterList();
                    foreach (string line in testLines)
                    {
                        hasher.Hash(line, 0xdeadbeef);
                        IEnumerable<UpdateResults> results = hashData.dictionary.UpdateHash(hasher.ph, hasher.sh, line, 0, true);
                        if (results.Count() > 0)
                            foundFiles.Add(line);
                        //results.Clear();
                    }
                    testLines.Clear();
                }
            }
        }
        // #pragma warning restore CS1998, CS4014

        private async void BtnTestFile_Click(object sender, EventArgs e)
        {
            OpenFileDialog ofd = new OpenFileDialog
            {
                Filter = "Text Files (.txt)|*.txt|Bin Files (.bin)|*.bin|All Files (*.*)|*.*",
                FilterIndex = 1
            };
            DialogResult result = ofd.ShowDialog();
            if (result == DialogResult.OK)
            {
                ShowLoader();
                toolStripStatusLabelLeft1.Text = "Testing Hash File...";
                await Task.Run(() => TestHashFiles(ofd.FileName));
                HideViewers();
                if (foundFiles.Count() > 0)
                {
                    txtRawView.Text = "Found Files\r\n\r\n";
                    StringBuilder sb = new StringBuilder(txtRawView.Text);
                    foreach (string file in foundFiles)
                    {
                        sb.Append(file);
                        sb.Append("\r\n");
                    }
                    txtRawView.Text = sb.ToString();
                    txtRawView.Visible = true;
                }
                HideLoader();
                string finished = "Found " + foundFiles.Count().ToString("n0") + " New Files";
                foundNewFileCount += foundFiles.Count();
                toolStripStatusLabelLeft1.Text = finished;
                MessageBox.Show(finished, "Test Hash File Complete", MessageBoxButtons.OK, MessageBoxIcon.Information);
                EnableUI();
            }
        }



        private async void TreeItemView_ItemSelectionChanged(object sender, EventArgs e)
        {
            audioState = false;
            if (treeItemView.SelectedObjects != null && treeItemView.SelectedObjects.Count > 1)
            {
                //Don't preview audio if we selected multiple files.
                audioState = false;
                btnAudioStop.Enabled = false;
                return;
            }

            BrightIdeasSoftware.OLVListItem selectedItem = treeItemView.SelectedItem;
            if (selectedItem == null)
            {
                return;
            }
            object selectedRow = selectedItem.RowObject;

            if (!audioState && selectedRow.GetType() == typeof(WemListItem))
            {
                WemListItem row = (WemListItem)selectedRow;
                WEM_File wem = row.obj;
                if (wem != null && wem.Data.Count() > 0)
                {
                    await Task.Run(() => wem.ConvertWEM());
                    toolStripStatusLabelLeft1.Text = "Playing Audio...";
                    toolStripProgressBar1.Visible = true;
                    treeItemView.Enabled = false;
                    btnAudioStop.Enabled = true;
                    await Task.Run(() => PlayOgg(wem));
                    btnAudioStop.Enabled = false;
                    treeItemView.Enabled = true;
                    toolStripProgressBar1.Visible = false;
                    toolStripStatusLabelLeft1.Text = "Audio Stopped";
                }
            }
        }

        private void PlayOgg(WEM_File wem)
        {
            if (waveOut == null)
                waveOut = new WaveOutEvent();
            waveOut.Init(wem.Vorbis);
            if (audioState == false)
            {
                audioState = true;
                SetStripProgressBarStyle(ProgressBarStyle.Continuous);
                SetStripProgressBarMax((int)wem.Vorbis.TotalTime.TotalMilliseconds);
                waveOut.Play();
                while (waveOut.PlaybackState != PlaybackState.Stopped)
                {
                    if (audioState == false)
                    {
                        waveOut.Stop();
                        break;
                    }
                    else
                    {
                        SetStripProgressBarValue((int)wem.Vorbis.CurrentTime.TotalMilliseconds);
                        Thread.Sleep(100);
                    }
                }

                SetStripProgressBarMax(100);
                SetStripProgressBarValue(0);
                SetStripProgressBarStyle(ProgressBarStyle.Marquee);
            }
        }

        private void BtnAudioStop_Click(object sender, EventArgs e)
        {
            audioState = false;
            btnAudioStop.Enabled = false;
        }

        private void BtnHelp_Click(object sender, EventArgs e)
        {
            AssetBrowserHelp helpForm = new AssetBrowserHelp();
            helpForm.Show();
        }

        public void SetStatusLabel(string message)
        {
            if (statusStrip1.InvokeRequired)
            {
                statusStrip1.Invoke(new Action(() => SetStatusLabel(message)));
            }
            else
            {
                toolStripStatusLabelLeft1.Text = message;
            }
        }

        private void RenderPanel_Resize(object sender, EventArgs e)
        {
            if (panelRender != null)
            {
                if (renderPanel.Width != panelRender.ClientWidth || renderPanel.Height != panelRender.ClientHeight)
                    panelRender.SetSize(renderPanel.Height, renderPanel.Width);
            }
        }

        private void BtnHashStatus_Click(object sender, EventArgs e)
        {
            AssetBrowserHashStatus hashStatus = new AssetBrowserHashStatus();
            hashStatus.Show();
        }

        private void ExtractByExtensionToolStripMenuItem_Click(object sender, EventArgs e)
        {
            AssetBrowserExtractExt frmExt = new AssetBrowserExtractExt();
            DialogResult result = frmExt.ShowDialog(this);
            if (result == DialogResult.OK)
            {
                extractByExtensions = true;
                extractExtensions = frmExt.GetExtensions();
                BtnExtract_Click(this, null);
            }
        }

        private void BtnFileTable_Click(object sender, EventArgs e)
        {
            AssetBrowserFileTable frmFileTable = new AssetBrowserFileTable();
            frmFileTable.Show();
        }

    }
}
