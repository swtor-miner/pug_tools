using System;
using System.Collections.Generic;
using System.Collections;
using System.ComponentModel;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Xml;
using System.Xml.Linq;
using GomLib;
using TorLib;
using BrightIdeasSoftware;

namespace tor_tools
{
    public partial class NodeBrowser : Form
    {
        private Assets currentAssets;
        private DataObjectModel currentDom;

        // private bool autoPreview = true;
        private string extractPath;
        // private bool invalidXML = false;

        /*
        private HashDictionaryInstance hashData = HashDictionaryInstance.Instance;
        private Dictionary<string, List<TorLib.FileInfo>> fileDict = new Dictionary<string, List<TorLib.FileInfo>>();
        
        private List<string> assetKeys = new List<string>();
        */

        private Dictionary<string, DomType> nodeDict;
        private readonly Dictionary<string, NodeAsset> assetDict = new Dictionary<string, NodeAsset>();
        private readonly List<string> nodeKeys = new List<string>();

        private readonly List<NodeOutput> outputList = new List<NodeOutput>();
        private readonly HashSet<string> outputData = new HashSet<string>();
        private string currentNode = "";
        private string currentItem = "";
        private string currentParent = "";
        private string currentTreeNode = "";

        private DataTable dt = new DataTable();

        private List<string> searchNodes = new List<string>();
        private int searchIndex = 0;
        private TreeNode[] nodeMatch;

        private readonly ArrayList rootList = new ArrayList();
        private bool collapseStatus = false;

        private readonly bool customSort = false;
        private readonly HashSet<string> customRoots = new HashSet<string>();
        private readonly Dictionary<string, string> customNodeSort = new Dictionary<string, string>();

        private bool filterEnabled = false;

        private bool BuildCSV = false;

        public NodeBrowser(string assetLocation, bool usePTS, string extractLocation)
        {
            if (extractLocation is null)
            {
                throw new ArgumentNullException(nameof(extractLocation));
            }

            InitializeComponent();

            Config.Load();
            txtExtractPath.Text = Config.ExtractPath;
            extractPath = txtExtractPath.Text;

            List<object> args = new List<object>
            {
                assetLocation,
                usePTS
            };

            if (System.IO.File.Exists(".\\Node Browser\\custom_node_sorting.xml"))
            {
                XmlDocument xmlDoc = new XmlDocument();
                xmlDoc.Load(".\\Node Browser\\custom_node_sorting.xml");

                customSort = Convert.ToBoolean(xmlDoc.SelectSingleNode("/custom_sort").Attributes["enabled"].Value);

                foreach (XmlNode node in xmlDoc.SelectNodes("/custom_sort/roots/root"))
                {
                    customRoots.Add(node.Attributes["name"].Value);
                }

                foreach (XmlNode node in xmlDoc.SelectNodes("/custom_sort/nodes/node"))
                {
                    customNodeSort.Add(node.Attributes["name"].Value, node.Attributes["parent"].Value);
                }
            }

            toolStripStatusLabel1.Text = "Loading Assets ...";
            ShowLoader();
            backgroundWorker1.RunWorkerAsync(args);
            treeListView1.CanExpandGetter = delegate (object x) { return ((NodeListItem)x).children.Count > 0; };
            treeListView1.ChildrenGetter = delegate (object x)
            {
                NodeListItem obj = (NodeListItem)x;
                ArrayList children = new ArrayList();
                foreach (var child in obj.children)
                {
                    if (child.displayName.Contains("Script_"))
                        continue;
                    children.Add(child);
                }
                return children;
            };
        }

        #region Background Wokers Methods

        private void BackgroundWorker1_DoWork(object sender, DoWorkEventArgs e)
        {
            List<object> args = e.Argument as List<object>;
            currentAssets = AssetHandler.Instance.GetCurrentAssets((string)args[0], (bool)args[1]);
            // currentAssets = currentAssets.getCurrentAssets((string)args[0], (bool)args[1]);            
            currentDom = DomHandler.Instance.GetCurrentDOM(currentAssets);
        }

        private void BackgroundWorker1_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            toolStripStatusLabel1.Text = "Loading Nodes ...";

            HashSet<string> nodeDirs = new HashSet<string>();
            HashSet<string> allDirs = new HashSet<string>();

            assetDict.Add("/", new NodeAsset("/", "", "Root", null));
            if (customSort)
            {
                if (customRoots.Count() > 0)
                {
                    foreach (string customRoot in customRoots)
                    {
                        assetDict.Add(customRoot, new NodeAsset(customRoot, "/", customRoot, null));
                        nodeDirs.Add(customRoot);
                    }
                }
            }

            currentDom.nodeLookup.TryGetValue(typeof(GomObject), out nodeDict);


            /*
            if (!nodeLookup.TryGetValue(typeType, out nameMap))
            {
                nameMap = new Dictionary<string, DomType>();
                nodeLookup[typeType] = nameMap;
            }
             */

            if (nodeDict != null)
            {
                foreach (var currentNode in nodeDict)
                {
                    nodeKeys.Add(currentNode.Key.ToString());
                    GomObject obj = (GomObject)currentNode.Value;
                    string display = currentNode.Key.ToString();

                    string parent;
                    if (obj.Name.Contains("."))
                    {
                        string[] temp = obj.Name.Split('.');
                        parent = string.Join(".", temp.Take(temp.Length - 1));
                        if (customSort)
                        {
                            if (customNodeSort.Count() > 0)
                            {
                                foreach (var node in customNodeSort)
                                {
                                    if (obj.Name.StartsWith(node.Key))
                                    {
                                        string origParent = parent;
                                        parent = node.Value + "." + parent;
                                        display = display.Replace(origParent, "").Replace(".", "");
                                    }
                                    else
                                    {
                                        display = display.Replace(parent, "").Replace(".", "");
                                    }
                                }
                            }
                        }
                        nodeDirs.Add(parent);
                    }
                    else
                    {
                        parent = "/";
                        if (customSort)
                        {
                            if (customNodeSort.Count() > 0)
                            {
                                foreach (var node in customNodeSort)
                                {
                                    if (obj.Name.StartsWith(node.Key))
                                    {
                                        parent = node.Value;
                                    }
                                }
                            }
                        }
                    }

                    NodeAsset asset = new NodeAsset(currentNode.Key.ToString(), parent, display, obj);
                    assetDict.Add(currentNode.Key.ToString(), asset);
                }
                nodeKeys.Sort();
            }

            foreach (var dir in nodeDirs)
            {
                string[] temp = dir.ToString().Split('.');
                int intLength = temp.Length;
                for (int intCount2 = 0; intCount2 <= intLength; intCount2++)
                {
                    string output = String.Join(".", temp, 0, intCount2);
                    if (output != "")
                        allDirs.Add(output);
                }
            }

            foreach (var dir in allDirs)
            {
                string[] temp = dir.ToString().Split('.');
                string parentDir = String.Join(".", temp.Take(temp.Length - 1));
                if (parentDir == "")
                    parentDir = "/";
                string display = temp.Last();

                NodeAsset asset = new NodeAsset(dir.ToString(), parentDir, display, (GomObject)null);
                if (!assetDict.ContainsKey(dir.ToString()))
                    assetDict.Add(dir.ToString(), asset);
            }

            toolStripStatusLabel1.Text = "Loading Tree View Items ...";
            Refresh();
            backgroundWorker2.RunWorkerAsync();
        }

        private void BackgroundWorker2_DoWork(object sender, DoWorkEventArgs e)
        {
            string getId(NodeAsset x) => x.Id;
            string getParentId(NodeAsset x) => x.parentId;
            string getDisplayName(NodeAsset x) => x.displayName;
            treeViewFast1.BeginUpdate();
            treeViewFast1.LoadItems<NodeAsset>(assetDict, getId, getParentId, getDisplayName);
        }

        private void BackgroundWorker2_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            treeViewFast1.EndUpdate();

            // Expand root node
            if (treeViewFast1.Nodes.Count > 0) treeViewFast1.Nodes[0].Expand();

            toolStripStatusLabel1.Text = "Loading Complete";
            treeViewFast1.Visible = true;
            HideLoader();
            EnableUI();
            System.Runtime.GCSettings.LatencyMode = System.Runtime.GCLatencyMode.Interactive;
        }

        #endregion

        #region Input / Output Methods

        private void ExtractByNode(TreeNodeCollection nodes)
        {
            foreach (TreeNode child in nodes)
            {
                TreeListItem asset = (TreeListItem)child.Tag;

                if (asset.hashInfo.File != null)
                {
                    // extractAsset(asset.file);
                }

                if (child.Nodes.Count > 0)
                    ExtractByNode(child.Nodes);
            }
        }

        private void SearchTreeNodes()
        {
            nodeMatch = treeViewFast1.Nodes.Find(searchNodes[searchIndex], true);
        }


        #endregion

        #region UI Methods

        private void GetNodeData(NodeAsset asset)
        {
            if (asset.Obj.Data != null)
            {
                foreach (KeyValuePair<string, object> item in asset.Obj.Data.Dictionary)
                {
                    if (item.Key.Contains("Script_"))
                        continue;

                    DomClass classLookup = (DomClass)asset.Obj.Data.Dictionary["Script_Type"];
                    DomField fieldLookup = classLookup.Fields.Find(x => x.Name == item.Key.ToString());

                    if (fieldLookup == null)
                    {
                        try
                        {
                            ulong id = ulong.Parse(item.Key);
                            fieldLookup = classLookup.Fields.Find(x => x.Id == id);
                            if (fieldLookup == null)
                            {
                                this.currentDom.DomTypeMap.TryGetValue(id, out DomType fieldLookup2);
                            }
                        }
                        catch (Exception) { }
                    }
                    try
                    {
                        if (fieldLookup == null)
                        {
                            NodeListItem item3 = new NodeListItem(item.Key.ToString(), item.Value, null);
                            rootList.Add(item3);
                        }
                        else
                        {
                            NodeListItem item3 = new NodeListItem(item.Key.ToString(), item.Value, fieldLookup.GomType);
                            rootList.Add(item3);
                        }

                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine("exception - " + ex.ToString());
                    }
                }
            }
        }

        private async void TreeViewFast1_AfterSelect(object sender, TreeViewEventArgs e)
        {
            treeListView1.ModelFilter = null;
            treeListView1.Visible = false;
            TreeNode selected = treeViewFast1.SelectedNode;
            NodeAsset asset = (NodeAsset)selected.Tag;
            Text = "Node Browser - " + asset.Id.ToString();
            collapseStatus = false;
            btnToggleCollapse.Enabled = true;
            btnToggleCollapse.Text = "Collapse Child Nodes";

            if (selected.Tag != null)
            {
                toolStripStatusLabel1.Text = "Loading Node Data ...";
                rootList.Clear();

                if (asset.Obj != null)
                {
                    treeListView1.Visible = false;
                    ShowLoader();
                    await Task.Run(() => GetNodeData(asset));
                    treeListView1.Roots = rootList;
                    treeListView1.ExpandAll();
                    HideLoader();
                    treeListView1.Visible = true;
                    if (filterEnabled)
                    {
                        treeListView1.ModelFilter = TextMatchFilter.Contains(treeListView1, txtFilter.Text);
                    }
                }
                treeListView1.TopItemIndex = 0;
                toolStripStatusLabel1.Text = "Loading Complete";
            }
            toolStripStatusLabel1.Text = asset.Id.ToString();

            dt = new DataTable();
            dt.Columns.Add("Property");
            dt.Columns.Add("Value");
            dt.Rows.Add(new string[] { "Current Node", asset.Id.ToString() });
            currentTreeNode = asset.Id.ToString();
            dataGridView1.DataSource = dt;
            dataGridView1.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.AllCells;

            return;

            /*
           
            if (asset.file != null)
            {
                DataTable dt = new DataTable();
                TorLib.File file = (TorLib.File)asset.file;
                dt.Columns.Add("Property");
                dt.Columns.Add("Value");
                dt.Rows.Add(new string[] { "Archive", file.Source.ToString() });
                dt.Rows.Add(new string[] { "File ID", String.Format("{0:X16}", file.FileInfo.FileId) });
                if (file.IsNamed)
                    dt.Rows.Add(new string[] { "File Name", file.FileName });
                else
                    dt.Rows.Add(new string[] { "File Name", file.FileName + "." + file.Extension });
                dt.Rows.Add(new string[] { "File Type", file.Extension });
                dt.Rows.Add(new string[] { "Path", file.Directory });
                dt.Rows.Add(new string[] { "State", file.FileState.ToString() });
                dt.Rows.Add(new string[] { "Compressed Size", file.FileInfo.CompressedSize.ToString() });
                dt.Rows.Add(new string[] { "Uncompressed Size", file.FileInfo.UncompressedSize.ToString() });
                dt.Rows.Add(new string[] { "Header Size", file.FileInfo.HeaderSize.ToString() });
                dt.Rows.Add(new string[] { "Offset", ((long)file.FileInfo.Offset).ToString() });
                dt.Rows.Add(new string[] { "Primary Hash", String.Format("{0:X8}", file.FileInfo.ph) });
                dt.Rows.Add(new string[] { "Secondary Hash", String.Format("{0:X8}", file.FileInfo.sh) });
                dt.Rows.Add(new string[] { "Checksum", String.Format("{0:X8}", file.FileInfo.Checksum) });
                dt.Rows.Add(new string[] { "Is Compressed", file.FileInfo.IsCompressed.ToString() });
                dataGridView1.DataSource = dt;
                dataGridView1.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.AllCells;
                */
            //}
        }

        private void TreeViewFast1_MouseUp(object sender, MouseEventArgs e)
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

        private void ExtractToolStripMenuItem_Click(object sender, EventArgs e)
        {
            BtnExtract_Click(this, null);
        }

        public void HideViewers()
        {
            treeListView1.Visible = false;
            txtFilter.Visible = false;
            btnFilterList.Visible = false;
            btnClearFilter.Visible = false;
        }

        public void ShowLoader()
        {
            treeListView1.Visible = false;
            pictureBox2.Visible = true;
            toolStripProgressBar1.Visible = true;
        }

        public void HideLoader()
        {
            treeListView1.Visible = true;
            pictureBox2.Visible = false;
            toolStripProgressBar1.Visible = false;
        }

        private void EnableUI()
        {
            dataGridView1.Enabled = true;
            txtExtractPath.Enabled = true;
            btnChooseExtract.Enabled = true;
            btnExtract.Enabled = true;
            txtSearch.Enabled = true;
            btnSearch.Enabled = true;
            txtFilter.Enabled = true;
            btnFilterList.Enabled = true;
            lblListFilter.Enabled = true;
            btnFileFinder.Enabled = true;
        }

        #endregion

        #region Buttton Methods

        private void BtnExtract_Click(object sender, EventArgs e)
        {
            TreeNode node = treeViewFast1.SelectedNode;
            NodeAsset asset = (NodeAsset)node.Tag;
            WriteFile(new XDocument(new XElement(asset.Obj.Print())), asset.Obj.Name + ".xml", false, false);
            byte[] buffer = asset.Obj.GetRawUncompressedNode();
            WriteFile(buffer, asset.Obj.Name + ".node");
            MessageBox.Show("Extracted " + asset.Obj.Name + " to " + extractPath);
        }

        public void WriteFile(XDocument content, string filename, bool append, bool trimEmpty)
        {
            if (trimEmpty)
            {
                content.Descendants()
                    .Where(e => e.IsEmpty || string.IsNullOrWhiteSpace(e.Value))
                    .Remove();
            }
            if (content.Root.IsEmpty) return;
            filename = filename.Replace('/', '.');
            if (!System.IO.Directory.Exists(extractPath)) { System.IO.Directory.CreateDirectory(extractPath); }
            using (System.IO.StreamWriter file2 = new System.IO.StreamWriter(extractPath + filename, append))
            {
                content.Save(file2, SaveOptions.None);
            }
        }

        public void WriteFile(byte[] content, string filename)
        {
            if (content == null || content.Count() == 0) return;
            filename = filename.Replace('/', '.');
            if (!System.IO.Directory.Exists(extractPath)) { System.IO.Directory.CreateDirectory(extractPath); }
            System.IO.File.WriteAllBytes(extractPath + filename, content);
        }

        private async void BtnSearch_Click(object sender, EventArgs e)
        {
            toolStripStatusLabel1.Text = "Performing Search ...";
            searchNodes = nodeKeys.Where(d => d.Contains(txtSearch.Text)).ToList();
            if (searchNodes.Count() > 0)
            {
                await OnSuccesfulSearch();
            }
            else
            {
                // Check if search value is ulong.
                if (ulong.TryParse(txtSearch.Text, out ulong nodeID))
                {
                    GomObject node = currentDom.GetObject(nodeID);
                    if (node != null)
                    {
                        txtSearch.Text = node.Name;
                        searchNodes = nodeKeys.Where(d => d.Contains(txtSearch.Text)).ToList();
                        if (searchNodes.Count() > 0)
                        {
                            await OnSuccesfulSearch();
                            return;
                        }
                    }
                }

                toolStripStatusLabel1.Text = "Search complete";
                MessageBox.Show("Search term not found.");
            }
        }

        private async Task OnSuccesfulSearch()
        {
            btnClearSearch.Enabled = true;
            btnSearch.Enabled = false;
            txtSearch.Enabled = false;
            btnFindNext.Enabled = true;
            toolStripStatusLabel1.Text = "Found " + (searchNodes.Count() + 1) + " Matches";
            ShowLoader();
            await Task.Run(() => SearchTreeNodes());
            HideLoader();
            treeViewFast1.SelectedNode = nodeMatch[0];
            treeViewFast1.Focus();
            toolStripStatusLabel2.Text = "Item " + (searchIndex + 1) + " of " + searchNodes.Count();
            searchIndex++;
        }

        private async void BtnFindNext_Click(object sender, EventArgs e)
        {
            if (searchNodes.ElementAtOrDefault(searchIndex) != null)
            {
                await Task.Run(() => SearchTreeNodes());
                treeViewFast1.SelectedNode = nodeMatch[0];
                treeViewFast1.Focus();
                toolStripStatusLabel2.Text = "Item " + (searchIndex + 1) + " of " + searchNodes.Count();
                searchIndex++;
            }
            else
            {
                toolStripStatusLabel1.Text = "Search complete";
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
            toolStripStatusLabel2.Text = "";
        }

        private void BtnChooseExtract_Click(object sender, EventArgs e)
        {
            FolderBrowserDialog fbd = new FolderBrowserDialog
            {
                SelectedPath = txtExtractPath.Text
            };
            _ = fbd.ShowDialog();
            txtExtractPath.Text = fbd.SelectedPath + "\\";
            extractPath = txtExtractPath.Text;
        }

        private void BtnToggleCollapse_Click(object sender, EventArgs e)
        {
            // Check if we can collapse or expand the selected node.
            if (treeListView1.SelectedObject != null)
            {
                TreeListView.Branch br = treeListView1.TreeModel.GetBranch(treeListView1.SelectedObject);
                if (br != null && br.CanExpand && !br.IsExpanded)
                {
                    // Expand node and all child nodes.
                    treeListView1.Visible = false;
                    ShowLoader();
                    foreach (object nodeObj in treeListView1.GetChildren(treeListView1.SelectedObject))
                    {
                        treeListView1.Expand(nodeObj);
                    }

                    treeListView1.Expand(treeListView1.SelectedObject);
                    btnToggleCollapse.Text = "Collapse Child Nodes";
                    treeListView1.Visible = true;
                    HideLoader();
                    return;
                }
                else if (br != null && br.CanExpand && br.IsExpanded)
                {
                    // Collapse node and all child nodes.
                    treeListView1.Visible = false;
                    ShowLoader();
                    foreach (object nodeObj in treeListView1.GetChildren(treeListView1.SelectedObject))
                    {
                        treeListView1.Collapse(nodeObj);
                    }

                    treeListView1.Collapse(treeListView1.SelectedObject);
                    btnToggleCollapse.Text = "Expand Child Nodes";
                    treeListView1.Visible = true;
                    HideLoader();
                    return;
                }
            }

            // Couldn't collapse or expand child node. Do whole page.
            if (collapseStatus)
            {
                collapseStatus = false;
                treeListView1.ExpandAll();
                btnToggleCollapse.Text = "Collapse Child Nodes";
            }
            else
            {
                collapseStatus = true;
                treeListView1.CollapseAll();
                btnToggleCollapse.Text = "Expand Child Nodes";
            }
        }

        private void BtnFilterList_Click(object sender, EventArgs e)
        {
            txtFilter.Enabled = false;
            btnFilterList.Enabled = false;
            btnClearFilter.Enabled = true;
            treeListView1.ModelFilter = TextMatchFilter.Contains(treeListView1, txtFilter.Text);
            filterEnabled = true;
        }

        private void BtnClearFilter_Click(object sender, EventArgs e)
        {
            btnClearFilter.Enabled = false;
            btnFilterList.Enabled = true;
            txtFilter.Enabled = true;
            treeListView1.ModelFilter = null;
            filterEnabled = false;
        }
        #endregion

        private async void BtnFileFinder_Click(object sender, EventArgs e)
        {
            DialogResult result = MessageBox.Show("Run File Name Finder?", "Confirm File Name Finder", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
            if (result == DialogResult.Yes)
            {
                DialogResult resultBuild = MessageBox.Show("Build CSV File?", "Build CSV", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                if (resultBuild == DialogResult.Yes)
                {
                    BuildCSV = true;
                }
                else
                {
                    BuildCSV = false;
                }

                ShowLoader();
                toolStripStatusLabel1.Text = "Running File Name Finder ...";
                await Task.Run(() => RunFileNameFinder());
                toolStripStatusLabel1.Text = "File Name Finder Complete";
                MessageBox.Show("File Name Files Generated", "Files Generated", MessageBoxButtons.OK, MessageBoxIcon.Information);
                HideLoader();
            }
        }

        private void RunFileNameFinder()

        {
            int nodeCount = 0;
            bool firstRun = true;

            NodeFileSource nodeSource = new NodeFileSource();

            foreach (var source in nodeSource.sources)
            {
                searchNodes = nodeKeys.Where(d => d.Contains(source.Key)).ToList();
                foreach (var nodeKey in searchNodes)
                {
                    currentNode = nodeKey;
                    NodeAsset node = assetDict[nodeKey];
                    {
                        if (node.Obj.Data != null)
                        {
                            currentItem = node.Id.ToString();
                            foreach (KeyValuePair<string, object> item in node.Obj.Data.Dictionary)
                            {
                                NodeListItem dataItem = new NodeListItem(item.Key.ToString(), item.Value);
                                if (dataItem.children.Count > 0)
                                {
                                    currentParent = dataItem.name.ToString();
                                    foreach (var child in dataItem.children)
                                    {
                                        HandleChildData(child, source.Value);
                                    }
                                    dataItem.children.Clear();
                                }
                                dataItem = null;
                            }
                            node.Obj.Unload();
                            nodeCount++;
                        }
                        node = null;
                        /*
                        if (nodeCount == 10000)
                        {
                            if (outputData.Count > 0 || outputList.Count > 0)
                            {
                                writeData(firstRun);
                                outputData.Clear();
                                outputList.Clear();
                                firstRun = false;
                            }
                            GC.Collect();
                            nodeCount = 0;                            
                        }
                         */
                    }
                }
                if (source.Value.Count > 0)
                    source.Value.Clear();
            }
            currentParent = null;
            currentItem = null;
            currentNode = null;
            searchNodes.Clear();
            searchNodes = null;
            nodeSource.sources.Clear();
            nodeSource = null;
            GC.Collect();
            WriteData(firstRun);

        }

        private void HandleChildData(NodeListItem item, List<NodeFileSourceItem> fields)
        {
            if (item != null)
            {
                if (item.children.Count > 0)
                {
                    currentParent = item.name.ToString();
                    foreach (var child in item.children)
                    {
                        HandleChildData(child, fields);
                    }
                }
                else
                {
                    if (item.value != null)
                    {
                        foreach (var field in fields)
                        {
                            if (item.displayName == field.field)
                            {
                                switch (field.type)
                                {
                                    case "fx":
                                        break;
                                    case "fxgr2":
                                        break;
                                    case "icon":
                                        break;
                                    case "/":
                                        break;
                                    case "cnv":
                                        break;
                                    case "gfximg":
                                        break;
                                    case "spec":
                                        break;
                                    case "anim":
                                        break;
                                    case "gr2":
                                        break;
                                    case "string":
                                        break;
                                    case "bnk":
                                        break;
                                    case "dds":
                                        break;
                                    case "load":
                                        break;
                                    case "tip":
                                        break;
                                    case "codex":
                                        break;
                                    default:
                                        throw new ArgumentException("Unhandled field type: " + field.type);
                                }
                                if (item.value.ToString().StartsWith("stg."))
                                    continue;
                                if (BuildCSV)
                                {
                                    NodeOutput output = new NodeOutput(currentNode, currentItem, currentParent, item.name.ToString(), item.value.ToString());
                                    outputList.Add(output);
                                }
                                outputData.Add(item.value.ToString());
                            }
                        }
                        /*
                        if (item.value.GetType() != typeof(string))
                            return;
                        else
                        {
                            if (item.displayName == "String Value")
                                return;
                            string value = (string)item.value;
                            if (value.Contains("/") || value.Contains("\\"))
                            {
                                if(value.Contains("</text>") || value.Contains("<locComment />") || value.Contains("/%") || value.Contains("/$"))
                                    return;
                                if (BuildCSV)
                                {
                                    NodeOutput output = new NodeOutput(this.currentNode, this.currentItem, this.currentParent, item.name.ToString(), value);
                                    outputList.Add(output);
                                }                                
                                outputData.Add(value);
                            }
                        }
                         */
                    }
                }
            }
        }

        public void WriteData(bool firstRun)
        {
            if (!System.IO.Directory.Exists(extractPath + "\\File_Names"))
                System.IO.Directory.CreateDirectory(extractPath + "\\File_Names");
            if (BuildCSV)
            {
                if (outputList.Count > 0)
                {
                    System.IO.StreamWriter outputCSV = new System.IO.StreamWriter(extractPath + "\\File_Names\\node_string_data.csv", !firstRun);
                    foreach (NodeOutput output in outputList)
                    {
                        outputCSV.Write(output.node + ", " + output.item + ", " + output.parent + ", " + output.name + ", " + output.value + "\r\n");
                    }
                    outputCSV.Close();
                    outputList.Clear();
                }
            }
            if (outputData.Count > 0)
            {
                System.IO.StreamWriter output = new System.IO.StreamWriter(extractPath + "\\File_Names\\node_string_list.txt", !firstRun);
                foreach (string data in outputData)
                {
                    output.Write(data + "\r\n");
                }
                output.Close();
                outputData.Clear();
            }
            GC.Collect();
        }

        private void TreeListView1_SelectedIndexChanged(object sender, EventArgs e)
        {
            TreeListView tlv = sender as TreeListView;
            dt = new DataTable();
            dt.Columns.Add("Property");
            dt.Columns.Add("Value");
            dt.Rows.Add(new string[] { "Current Node", currentTreeNode });
            if (tlv.SelectedObject is NodeListItem child)
            {
                if (child.name != null)
                    dt.Rows.Add(new string[] { "Current Item", child.name.ToString() });
                else
                    dt.Rows.Add(new string[] { "Current Item", "" });
                if (child.value != null)
                    dt.Rows.Add(new string[] { "Current Value", child.value.ToString() });
                else
                    dt.Rows.Add(new string[] { "Current Value", "" });
            }
            dataGridView1.DataSource = dt;
            dataGridView1.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.AllCells;

            if (tlv.SelectedObject != null)
            {
                // Node selected. Lets see if it can collapse or not.
                TreeListView.Branch br = tlv.TreeModel.GetBranch(tlv.SelectedObject);
                if (br != null && br.CanExpand && br.IsExpanded)
                {
                    btnToggleCollapse.Text = "Collapse Child Nodes";
                }
                else if (br != null && br.CanExpand && !br.IsExpanded)
                {
                    btnToggleCollapse.Text = "Expand Child Nodes";
                }
            }
            else
            {
                // Page selected.
                if (collapseStatus)
                {
                    btnToggleCollapse.Text = "Expand Child Nodes";
                }
                else
                {
                    btnToggleCollapse.Text = "Collapse Child Nodes";
                }
            }
        }

        private void TreeListView1_MouseUp(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Right)
            {
                TreeListView tlv = sender as TreeListView;

                if (tlv.SelectedObject is NodeListItem item)
                {
                    // Prefer the value. Do we ever have a case of both the key and value representing nodes?
                    if (item.type == "ulong" && item.displayValue.Contains("(") && item.displayValue.Contains(")"))
                    {
                        cmsJumpToNode.Show(treeListView1, e.Location);
                    }
                    else if (item.displayName.Contains(" (") && item.displayName.EndsWith(")"))
                    {
                        cmsJumpToNode.Show(treeListView1, e.Location);
                    }
                }
            }
        }

        private void TsmiJumpToNode_Click(object sender, EventArgs e)
        {
            // BrightIdeasSoftware.TreeListView tlv = sender as BrightIdeasSoftware.TreeListView;
            NodeListItem item = treeListView1.SelectedObject as NodeListItem;
            // NodeListItem item = tr .SelectedObject as NodeListItem;
            string nodeString;
            if (item.displayValue.Contains(" (") && item.displayValue.EndsWith(")"))
            {
                nodeString = item.displayValue.Split(' ').Last().Replace("(", "").Replace(")", "");
            }
            else
            {
                nodeString = item.displayName.Split(' ').Last().Replace("(", "").Replace(")", "");
            }

            TreeNode[] node = treeViewFast1.Nodes.Find(nodeString, true);
            treeViewFast1.SelectedNode = node.First();
        }
    }
}
