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
    public partial class AssetBrowserFileTable : Form
    {   
        private HashDictionaryInstance hashData;
        private Dictionary<string, List<TorLib.FileInfo>> fileDict = new Dictionary<string, List<TorLib.FileInfo>>();
        private Dictionary<string, ArchTreeListItem> assetDict = new Dictionary<string, ArchTreeListItem>();
        
        private ArrayList rootList = new ArrayList();
        public bool _closing = false;
        
        private Hasher hasher = new Hasher(Hasher.HasherType.TOR);
        private HashSet<string> foundFiles = new HashSet<string>();

        private Archive currentArchive;
        private Dictionary<string, int> currentArchDetails = new Dictionary<string, int>();

        public uint filesTotal = 0;
        public uint filesNamed = 0;
        public uint filesUnnamed = 0;
        public uint filesMissing = 0;

        public AssetBrowserFileTable()
        {
            InitializeComponent();
            FormClosed += AssetBrowser_FormClosed;            

            hashData = HashDictionaryInstance.Instance;
            if(!hashData.Loaded)
            {
                hashData.Load();
            }
            
            showLoader();            
            treeListView1.CanExpandGetter = delegate(object x) 
            {
                if (x.GetType() == typeof(NodeListItem))
                    return (((NodeListItem)x).children.Count > 0);
                return false;
            };
            treeListView1.ChildrenGetter = delegate(object x)
            {
                if (x.GetType() == typeof(NodeListItem))                
                    return new ArrayList(((NodeListItem)x).children);                
                return null;
            };
            backgroundWorker1.RunWorkerAsync();            
        }

    #region Background Wokers Methods

        private void backgroundWorker1_DoWork(object sender, DoWorkEventArgs e)
        {
            if (_closing)
            {
                return;
            }

            Assets currentAssets = AssetHandler.Instance.getCurrentAssets();
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

        private void backgroundWorker1_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
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
            hideLoader();
            enableUI();
            if (treeViewFast1.Nodes.Count > 0) treeViewFast1.Nodes[0].Expand();            
        }
    #endregion

    #region UI Methods
        private async void treeViewFast1_AfterSelect(object sender, TreeViewEventArgs e)
        {
            TreeNode node = treeViewFast1.SelectedNode;
            ArchTreeListItem obj = (ArchTreeListItem)node.Tag;
            this.Text = "Asset File Table Browser  - " + obj.Id.ToString();
            if (obj.arch != null)
            {
                hideViewers();
                showLoader();
                DataTable dt = new DataTable();
                this.currentArchive = obj.arch;                
                this.currentArchDetails = new Dictionary<string, int>();
                this.filesTotal = 0;
                this.filesNamed = 0;
                this.filesUnnamed = 0;
                this.filesMissing = 0;                
                FileListItem.resetTreeListViewColumns(treeListView1);
                this.treeListView1.TopItemIndex = 0;
                rootList.Clear();                
                await Task.Run(() => parseLibFiles());
                hideLoader();
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

        private async Task parseLibFiles()
        {
            if (InvokeRequired)
            {
                Invoke(new Action(() => parseLibFiles()));
            }
            else
            {   
                List<TorLib.File> files = this.currentArchive.files;
                files.Sort(delegate(TorLib.File x, TorLib.File y)
                {
                    if (x.FileInfo.Offset > y.FileInfo.Offset)
                        return -1;
                    else
                        return 1;
                });

                foreach (var file in files)
                {
                    HashFileInfo hashInfo = new HashFileInfo(file.FileInfo.ph, file.FileInfo.sh, file);
                    if(hashInfo.FileName.Contains("metadata.bin") || hashInfo.FileName.Contains("ft.sig"))
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

     
        public void hideViewers()
        {                  
            treeListView1.Visible = false;   
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
            this.treeViewFast1.Enabled = true;           
            this.treeListView1.Enabled = true;          
        }

        private void disableUI()
        {
            this.dataGridView1.Enabled = false;          
            this.treeViewFast1.Enabled = false;           
            this.treeListView1.Enabled = false;            
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
    }    
}
