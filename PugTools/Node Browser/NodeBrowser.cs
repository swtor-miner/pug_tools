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
using BrightIdeasSoftware;
using TreeViewFast.Controls;

namespace tor_tools
{  
    public partial class NodeBrowser : Form
    {   
        private TorLib.Assets currentAssets;
        private DataObjectModel currentDom;   
     
        //private bool autoPreview = true;
        private string extractPath;
        //private bool invalidXML = false;

        /*
        private HashDictionaryInstance hashData = HashDictionaryInstance.Instance;
        private Dictionary<string, List<TorLib.FileInfo>> fileDict = new Dictionary<string, List<TorLib.FileInfo>>();
        
        private List<string> assetKeys = new List<string>();
        */

        private Dictionary<string, DomType> nodeDict;
        private Dictionary<string, DomType> fieldDict;
        private Dictionary<string, NodeAsset> assetDict = new Dictionary<string, NodeAsset>();        
        private List<string> nodeKeys = new List<string>();

        private List<NodeOutput> outputList = new List<NodeOutput>();
        private HashSet<string> outputData = new HashSet<string>();
        private string currentNode = "";
        private string currentItem = "";
        private string currentParent = "";
        private string currentTreeNode = "";

        private DataTable dt = new DataTable();            

        private Stream inputStream;
        private MemoryStream memStream;
        //private XmlDocument xmlDoc;

        private List<string> searchNodes = new List<string>();
        private int searchIndex = 0;
        private TreeNode[] nodeMatch;

        private ArrayList rootList = new ArrayList();
        private bool collapseStatus = false;

        private bool customSort = false;
        private HashSet<string> customRoots = new HashSet<string>();
        private Dictionary<string, string> customNodeSort = new Dictionary<string, string>();

        private bool filterEnabled = false;

        private bool BuildCSV = false;
      
        public NodeBrowser(string assetLocation, bool usePTS, string extractLocation)
        {
            InitializeComponent();

            Config.Load();
            txtExtractPath.Text = Config.ExtractAssetsPath;
            this.extractPath = txtExtractPath.Text;
            
            List<object> args = new List<object>();
            args.Add(assetLocation);
            args.Add(usePTS);

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
            showLoader();            
            backgroundWorker1.RunWorkerAsync(args);                                
            treeListView1.CanExpandGetter = delegate(object x) { return (((NodeListItem)x).children.Count > 0); };
            treeListView1.ChildrenGetter = delegate(object x)
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

        private void backgroundWorker1_DoWork(object sender, DoWorkEventArgs e)
        {
            List<object> args = e.Argument as List<object>;
            this.currentAssets = AssetHandler.Instance.getCurrentAssets((string)args[0], (bool)args[1]);            
            //this.currentAssets = this.currentAssets.getCurrentAssets((string)args[0], (bool)args[1]);            
            this.currentDom = DomHandler.Instance.getCurrentDOM(currentAssets);            
        }

        private void backgroundWorker1_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            toolStripStatusLabel1.Text = "Loading Nodes ...";

            HashSet<string> nodeDirs = new HashSet<string>();
            HashSet<string> allDirs = new HashSet<string>();

            assetDict.Add("/", new NodeAsset("/", "", "Root", (GomObject)null));
            if (customSort)
            {
                if (customRoots.Count() > 0)
                {
                    foreach (string customRoot in customRoots)
                    {
                        assetDict.Add(customRoot, new NodeAsset(customRoot, "/", customRoot, (GomObject)null));
                        nodeDirs.Add(customRoot);
                    }
                }
            }
              
            this.currentDom.nodeLookup.TryGetValue(typeof(GomObject), out nodeDict);
            

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
                    GomLib.GomObject obj = (GomLib.GomObject)currentNode.Value;
                    string parent = "";
                    string display = currentNode.Key.ToString();                    

                    if (obj.Name.Contains("."))
                    {
                        string[] temp = obj.Name.Split('.');
                        parent = String.Join(".", temp.Take(temp.Length - 1));
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
            this.Refresh();
            backgroundWorker2.RunWorkerAsync();
        }

        private void backgroundWorker2_DoWork(object sender, DoWorkEventArgs e)
        {
            Func<NodeAsset, string> getId = (x => x.Id);
            Func<NodeAsset, string> getParentId = (x => x.parentId);
            Func<NodeAsset, string> getDisplayName = (x => x.displayName);
            treeViewFast1.BeginUpdate();
            treeViewFast1.LoadItems<NodeAsset>(assetDict, getId, getParentId, getDisplayName);            
        }

        private void backgroundWorker2_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            treeViewFast1.EndUpdate();

            //Expand root node
            if (treeViewFast1.Nodes.Count > 0) treeViewFast1.Nodes[0].Expand();

            toolStripStatusLabel1.Text = "Loading Complete";
            treeViewFast1.Visible = true;            
            hideLoader();
            enableUI();
        }

    #endregion

    #region Input / Output Methods

        private async void extractByNode(TreeNodeCollection nodes)
        {
            foreach (TreeNode child in nodes)
            {
                TreeListItem asset = (TreeListItem)child.Tag;

                if (asset.hashInfo.file != null)
                {
                    //extractAsset(asset.file);
                }

                if (child.Nodes.Count > 0)
                    extractByNode(child.Nodes);
            }
        }
        
        private async void searchTreeNodes()
        {
            this.nodeMatch = treeViewFast1.Nodes.Find(this.searchNodes[this.searchIndex], true);
        }
        

    #endregion

    #region UI Methods

        private async void getNodeData(NodeAsset asset)
        {
            if (asset.obj.Data != null)
            {   
                foreach (KeyValuePair<string, object> item in asset.obj.Data.Dictionary)
                {
                    if (item.Key.Contains("Script_"))
                        continue;
                    
                    DomClass classLookup = (DomClass)asset.obj.Data.Dictionary["Script_Type"];
                    DomField fieldLookup = classLookup.Fields.Find(x => x.Name == item.Key.ToString());                    
                    
                    if (fieldLookup == null)
                    {
                        try
                        {
                            ulong id = ulong.Parse(item.Key);
                            fieldLookup = classLookup.Fields.Find(x => x.Id == id);
                            if (fieldLookup == null)
                            {
                                DomType fieldLookup2 = null;
                                this.currentDom.DomTypeMap.TryGetValue(id, out fieldLookup2);
                                if (fieldLookup2 != null)
                                {
                                    /*if (this.currentDom.UnNamedMap.ContainsKey(id))
                                    {
                                        Console.WriteLine("found value");
                                    }else{
                                    
                                        Console.WriteLine("not found");
                                    } */                                   
                                }
                            }
                        }
                        catch (Exception e) { }                    
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
                    catch (Exception e)
                    {
                        Console.WriteLine("exception - " + e.ToString());
                    }                    
                }                
            }
        }

        private async void treeViewFast1_AfterSelect(object sender, TreeViewEventArgs e)
        {
            treeListView1.ModelFilter = null;
            treeListView1.Visible = false;
            TreeNode selected = treeViewFast1.SelectedNode;
            NodeAsset asset = (NodeAsset)selected.Tag;
            this.collapseStatus = false;
            this.btnToggleCollapse.Enabled = true;  
            this.btnToggleCollapse.Text = "Collapse Child Nodes";

            if (selected.Tag != null)
            {
                toolStripStatusLabel1.Text = "Loading Node Data ...";
                rootList.Clear();                
                
                if(asset.obj != null)
                {
                    treeListView1.Visible = false;
                    showLoader();
                    await Task.Run(() => getNodeData(asset));
                    treeListView1.Roots = rootList;
                    treeListView1.ExpandAll();
                    hideLoader();                   
                    treeListView1.Visible = true;
                    if (this.filterEnabled)
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
            this.currentTreeNode = asset.Id.ToString();
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
            btnExtract_Click(this, null);
        }

        public void hideViewers()
        {
            treeListView1.Visible = false;
            txtFilter.Visible = false;
            btnFilterList.Visible = false;
            btnClearFilter.Visible = false;
        }

        public void showLoader()
        {
            treeListView1.Visible = false;
            pictureBox2.Visible = true;
            toolStripProgressBar1.Visible = true;
        }

        public void hideLoader()
        {
            treeListView1.Visible = true;
            pictureBox2.Visible = false;
            toolStripProgressBar1.Visible = false;
        }

        private void enableUI()
        {
            this.dataGridView1.Enabled = true;
            //this.txtExtractPath.Enabled = true;            
            //this.btnExtract.Enabled = true;            
            this.txtSearch.Enabled = true;
            this.btnSearch.Enabled = true;
            this.txtFilter.Enabled = true;
            this.btnFilterList.Enabled = true;
            this.lblListFilter.Enabled = true;
            this.btnFileFinder.Enabled = true;
        }

    #endregion  
   
    #region Buttton Methods

        private async void btnExtract_Click(object sender, EventArgs e)
        {
            MessageBox.Show("Function disabled for now");
            return;

            this.extractPath = txtExtractPath.Text;

            TreeNode node = treeViewFast1.SelectedNode;
            TreeListItem asset = (TreeListItem)node.Tag;

            if (asset.hashInfo.file != null)
            {
                showLoader();
                //extractAsset(asset.file);
                hideLoader();
            }
            else
            {
                if (node.Nodes.Count > 0)
                {
                    if (MessageBox.Show("Extract all objects from " + node.Name + "?", "Extract Confirm", MessageBoxButtons.YesNo) == DialogResult.Yes)
                    {
                        showLoader();
                        await Task.Run(() => extractByNode(node.Nodes));
                        hideLoader();
                        MessageBox.Show("Extraction Completed");
                    }
                }
            }
        }           
        
        private async void btnSearch_Click(object sender, EventArgs e)
        {            
            this.toolStripStatusLabel1.Text = "Performing Search ...";
            this.searchNodes = this.nodeKeys.Where(d => d.Contains(txtSearch.Text)).ToList();
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

        private void btnToggleCollapse_Click(object sender, EventArgs e)
        {
            if (this.collapseStatus)
            {
                this.collapseStatus = false;
                treeListView1.ExpandAll();
                this.btnToggleCollapse.Text = "Collapse Child Nodes";
            }
            else
            {
                this.collapseStatus = true;
                treeListView1.CollapseAll();
                this.btnToggleCollapse.Text = "Expand Child Nodes";
            }
        }

        private void btnFilterList_Click(object sender, EventArgs e)
        {
            this.txtFilter.Enabled = false;
            this.btnFilterList.Enabled = false;
            this.btnClearFilter.Enabled = true;
            this.treeListView1.ModelFilter = TextMatchFilter.Contains(this.treeListView1, txtFilter.Text);
            this.filterEnabled = true;
        }

        private void btnClearFilter_Click(object sender, EventArgs e)
        {
            this.btnClearFilter.Enabled = false;
            this.btnFilterList.Enabled = true;
            this.txtFilter.Enabled = true;
            this.treeListView1.ModelFilter = null;
            this.filterEnabled = false;
        }
    #endregion        

        private async void btnFileFinder_Click(object sender, EventArgs e)
        {
            DialogResult result = MessageBox.Show("Run File Name Finder?", "Confirm File Name Finder", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
            if(result == DialogResult.Yes)
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

                showLoader();
                this.toolStripStatusLabel1.Text = "Running File Name Finder ...";
                await Task.Run(() => runFileNameFinder());        
                this.toolStripStatusLabel1.Text = "File Name Finder Complete";
                MessageBox.Show("File Name Files Generated", "Files Generated", MessageBoxButtons.OK, MessageBoxIcon.Information);
                hideLoader();
            }
        }

        private async void runFileNameFinder()
        
        {
            int nodeCount = 0;
            bool firstRun = true;
                       
            NodeFileSource nodeSource = new NodeFileSource();

            foreach (var source in nodeSource.sources)
            {
                this.searchNodes = this.nodeKeys.Where(d => d.Contains(source.Key)).ToList();
                foreach (var nodeKey in this.searchNodes)
                {                    
                    this.currentNode = nodeKey;
                    NodeAsset node = assetDict[nodeKey];
                    {
                        if (node.obj.Data != null)
                        {
                            this.currentItem = node.Id.ToString();
                            foreach (KeyValuePair<string, object> item in node.obj.Data.Dictionary)
                            {
                                NodeListItem dataItem = new NodeListItem(item.Key.ToString(), item.Value);
                                if (dataItem.children.Count > 0)
                                {
                                    this.currentParent = dataItem.name.ToString();
                                    foreach (var child in dataItem.children)
                                    {
                                        handleChildData(child, source.Value);
                                    }
                                    dataItem.children.Clear();
                                }
                                dataItem = null;
                            }
                            node.obj.Unload();
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
            this.currentParent = null;
            this.currentItem = null;
            this.currentNode = null;
            this.searchNodes.Clear();
            this.searchNodes = null;
            nodeSource.sources.Clear();
            nodeSource = null;
            GC.Collect();
            writeData(firstRun);
            
        }

        private void handleChildData(NodeListItem item, List<NodeFileSourceItem> fields)
        {
            if (item != null)
            {
                if (item.children.Count > 0)
                {
                    this.currentParent = item.name.ToString();
                    foreach (var child in item.children)
                    {
                        handleChildData(child, fields);
                    }                    
                }
                else
                {
                    if (item.value != null)
                    {
                        foreach (var field in fields)
                        {
                            if (item.name == field.field)
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
                                    NodeOutput output = new NodeOutput(this.currentNode, this.currentItem, this.currentParent, item.name.ToString(), item.value.ToString());
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
                            string value = (string)item.value;
                            if (value.Contains("/") || value.Contains("\\"))
                            {
                                if(value.Contains("</text>") || value.Contains("<locComment />"))
                                    return;
                                NodeOutput output = new NodeOutput(this.currentNode, this.currentItem, this.currentParent, item.name.ToString(), value);
                                outputList.Add(output);
                                outputData.Add(value);
                            }
                        }
                         */
                    }
                }
            }
        }

        public void writeData(bool firstRun)
        {
            if (!System.IO.Directory.Exists(this.extractPath + "\\File_Names"))
                System.IO.Directory.CreateDirectory(this.extractPath + "\\File_Names");
            if (BuildCSV)
            {
                if (this.outputList.Count > 0)
                {
                    System.IO.StreamWriter outputCSV = new System.IO.StreamWriter(this.extractPath + "\\File_Names\\node_string_data.csv", !firstRun);
                    foreach (NodeOutput output in outputList)
                    {
                        outputCSV.Write(output.node + ", " + output.item + ", " + output.parent + ", " + output.name + ", " + output.value + "\r\n");
                    }
                    outputCSV.Close();
                    this.outputList.Clear();
                }
            }
            if (this.outputData.Count > 0)
            {
                System.IO.StreamWriter output = new System.IO.StreamWriter(this.extractPath + "\\File_Names\\node_string_list.txt", !firstRun);
                foreach (string data in outputData)
                {
                    output.Write(data + "\r\n");
                }
                output.Close();
                this.outputData.Clear();
            }
            GC.Collect();
        }

        private void treeListView1_SelectedIndexChanged(object sender, EventArgs e)
        {
            BrightIdeasSoftware.TreeListView tlv = sender as BrightIdeasSoftware.TreeListView;
            NodeListItem child = tlv.SelectedObject as NodeListItem;
            dt = new DataTable();
            dt.Columns.Add("Property");
            dt.Columns.Add("Value");
            dt.Rows.Add(new string[] { "Current Node", this.currentTreeNode });
            if (child != null)
            {
                if (child.name != null)
                    dt.Rows.Add(new string[] { "Current Item", child.name.ToString() });
                else
                    dt.Rows.Add(new string[] { "Current Item", "" });
                if(child.value != null)
                    dt.Rows.Add(new string[] { "Current Value", child.value.ToString() });
                else
                    dt.Rows.Add(new string[] { "Current Value", "" });
            }
            dataGridView1.DataSource = dt;
            dataGridView1.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.AllCells;
        }

        private void treeListView1_MouseUp(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Right)
            {
                BrightIdeasSoftware.TreeListView tlv = sender as BrightIdeasSoftware.TreeListView;
                NodeListItem item = tlv.SelectedObject as NodeListItem;

                if (item != null)
                {
                    //Prefer the value. Do we ever have a case of both the key and value representing nodes?
                    if (item.type == "ulong" && item.displayValue.Contains("(") && item.displayValue.Contains(")"))
                    {
                        cmsJumpToNode.Show(treeListView1, e.Location);
                    } else if (item.displayName.Contains(" (") && item.displayName.EndsWith(")"))
                    {
                        cmsJumpToNode.Show(treeListView1, e.Location);
                    }
                }
            }
        }

        private void tsmiJumpToNode_Click(object sender, EventArgs e)
        {
            //BrightIdeasSoftware.TreeListView tlv = sender as BrightIdeasSoftware.TreeListView;
            NodeListItem item = treeListView1.SelectedObject as NodeListItem;
            //NodeListItem item = tr .SelectedObject as NodeListItem;
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
