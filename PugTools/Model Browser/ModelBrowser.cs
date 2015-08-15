using System;
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
using GomLib;
using GomLib.Models;
using TorLib;
using nsHashDictionary;
using DevIL;
using FileFormats;
using ShaderResourceView = SlimDX.Direct3D11.ShaderResourceView;
using SlimDX;
using System.Xml;

namespace tor_tools
{
    public partial class ModelBrowser : Form
    {
        public TorLib.Assets currentAssets;
        private DataObjectModel currentDom;

        public TorLib.Assets previousAssets;
        private DataObjectModel previousDom;
                
        private Dictionary<string, NodeAsset> assetDict = new Dictionary<string, NodeAsset>();
        private List<string> nodeKeys = new List<string>();

        private Dictionary<string, NodeAsset> dataviewDict = new Dictionary<string, NodeAsset>();

        private View_NPC_GR2 panelRender;
        public bool _closing = false;
        private Thread render;

        public Dictionary<string, GR2> models = new Dictionary<string, GR2>();
        public Dictionary<string, object> resources = new Dictionary<string, object>();
        public List<ItemAppearance> items = new List<ItemAppearance>();

        Dictionary<object, object> weaponAppearance = new Dictionary<object, object>();
        Dictionary<object, object> mntMountInfoTable = new Dictionary<object, object>();

        public ModelBrowser(string assetLocation, bool usePTS,
            string previousAssetLocation, bool previousUsePTS)
        {
            InitializeComponent();

            List<object> args = new List<object>();
            args.Add(assetLocation);
            args.Add(usePTS);
            args.Add(previousAssetLocation);
            args.Add(previousUsePTS);

            toolStripProgressBar1.Visible = true;
            toolStripStatusLabel1.Text = "Loading Assets";
            disableUI();
            backgroundWorker1.RunWorkerAsync(args);            
        }

        private void backgroundWorker1_DoWork(object sender, DoWorkEventArgs e)
        {
            List<object> args = e.Argument as List<object>;

            //Load current assets.
            this.currentAssets = AssetHandler.Instance.getCurrentAssets((string)args[0], (bool)args[1]);            
            this.currentDom = DomHandler.Instance.getCurrentDOM(currentAssets);

            //Load previous assets.
            this.previousAssets = AssetHandler.Instance.getPreviousAssets((string)args[2], (bool)args[3]);
            this.previousDom = DomHandler.Instance.getPreviousDOM(previousAssets);
        }

        private void backgroundWorker1_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            //Get the relevant nodes from the new dom.
            List<GomObject> itmList = currentDom.GetObjectsStartingWith("npp.")
                    .Union(currentDom.GetObjectsStartingWith("ipp."))
                    .Union(currentDom.GetObjectsStartingWith("itm."))
                    .Union(currentDom.GetObjectsStartingWith("dyn.housing"))
                    .Union(currentDom.GetObjectsStartingWith("dyn.stronghold"))
                    .ToList();

            //Determine which nodes are new.
            List<int> newNodeIndexes = new List<int>();
            for (int i = 0; i < itmList.Count; i++ )
            {
                GomObject newObj = itmList[i];

                GomObject oldObj = previousDom.GetObject(newObj.Name);
                if (oldObj == null)
                {
                    //Node is new.
                    newNodeIndexes.Add(i);
                }
            }

            weaponAppearance = currentDom.GetObject("itmAppearanceDatatable").Data.Get<Dictionary<object, object>>("itmAppearances");
            Dictionary<object, object> tempAppear = new Dictionary<object, object>();
            foreach (var appear in weaponAppearance)
            {
                tempAppear.Add(appear.Key.ToString().ToLower(), appear.Value);
            }
            weaponAppearance = new Dictionary<object, object>(tempAppear);
            tempAppear.Clear();

            mntMountInfoTable = currentDom.GetObject("mntMountInfoPrototype").Data.Get<Dictionary<object, object>>("4611686298607484000");          
            
            HashSet<string> nodeDirs = new HashSet<string>();
            HashSet<string> allDirs = new HashSet<string>();

            foreach (GomObject item in itmList)
            {
                if (item.Name.StartsWith("itm."))
                {
                    string appearSpec = item.Data.ValueOrDefault<string>("cbtWeaponAppearanceSpec", null);
                    if (appearSpec == null)
                        continue;                    
                }
                string parent = string.Empty;
                string display = item.Name;
                if (item.Name.Contains("."))
                {
                    string[] temp = item.Name.Split('.');
                    parent = String.Join(".", temp.Take(temp.Length - 1));
                    display = display.Replace(parent, string.Empty).Replace(".", string.Empty);

                    if(item.Name.StartsWith("itm."))
                    {
                        //Try and get the item name.
                        if (item.Data.ContainsKey("locTextRetrieverMap"))
                        {
                            GomObjectData nameLookupData = (GomObjectData)(item.Data.Get<Dictionary<object, object>>("locTextRetrieverMap")[-2761358831308646330]);
                            string itmName = currentDom.stringTable.TryGetString(item.Name, nameLookupData);
                            if(itmName.Length > 0)
                            {
                                //Found the item name, put it in brackets.
                                display = display + " (" + itmName + ")";
                            }

                        }
                    }

                    nodeDirs.Add(parent);
                }

                NodeAsset asset = new NodeAsset(item.Name.ToString(), parent, display, currentDom);
                assetDict.Add(item.Name.ToString(), asset);
                item.Unload(); //make sure these aren't hanging around
            }

            //Build the new list.
            foreach(int i in newNodeIndexes)
            {
                GomObject item = itmList[i];
                if (item.Name.StartsWith("itm."))
                {
                    string appearSpec = item.Data.ValueOrDefault<string>("cbtWeaponAppearanceSpec", null);
                    if (appearSpec == null)
                        continue;
                }
                string parent = string.Empty;
                string display = item.Name;
                if (item.Name.Contains("."))
                {
                    string[] temp = item.Name.Split('.');
                    parent = String.Join(".", temp.Take(temp.Length - 1));
                    display = display.Replace(parent, string.Empty).Replace(".", string.Empty);
                    parent = "new." + parent;

                    if (item.Name.StartsWith("itm."))
                    {
                        //Try and get the item name.
                        if (item.Data.ContainsKey("locTextRetrieverMap"))
                        {
                            GomObjectData nameLookupData = (GomObjectData)(item.Data.Get<Dictionary<object, object>>("locTextRetrieverMap")[-2761358831308646330]);
                            string itmName = currentDom.stringTable.TryGetString(item.Name, nameLookupData);
                            if (itmName.Length > 0)
                            {
                                //Found the item name, put it in brackets.
                                display = display + " (" + itmName + ")";
                            }
                        }
                    }

                    nodeDirs.Add(parent);
                }
                newNodeIndexes = null;

                NodeAsset asset = new NodeAsset("new." + item.Name.ToString(), parent, display, item);
                assetDict.Add("new." + item.Name.ToString(), asset);
                item.Unload(); //make sure these aren't hanging around
            }

            foreach (KeyValuePair<object, object> item in mntMountInfoTable)
            {                 
                GomObjectData value = (GomObjectData)item.Value;
                object spec;
                
                value.Dictionary.TryGetValue("mntDataSpecString",out spec);
                
                string parent = "";                
                string display = spec.ToString().Split('.').Last();
                if (spec.ToString().Contains("."))
                {
                    string[] temp = spec.ToString().Split('.');
                    parent = String.Join(".", temp.Take(temp.Length - 1));
                    display = display.Replace(parent, "").Replace(".", "");
                    nodeDirs.Add(parent);
                }
                else
                    parent = "/nodes";
                NodeAsset asset = new NodeAsset(spec.ToString(), parent, display, value);
                assetDict.Add(spec.ToString(), asset);
            }
            nodeKeys.Sort();

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
                    parentDir = "/nodes";
                string display = temp.Last();

                NodeAsset asset = new NodeAsset(dir.ToString(), parentDir, display, (GomObject)null);
                if (!assetDict.ContainsKey(dir.ToString()))
                    assetDict.Add(dir.ToString(), asset);
            }
            allDirs.Clear();

            HashSet<string> fileDirs = new HashSet<string>();
            string prefixAll = "/assets/all";
            string prefixNew = "/assets/new";
            string prefixMod = "/assets/modified";
            string prefixUnn = "/assets/unnamed";


            //Make sure the hash data is loaded.
            HashDictionaryInstance hashInstance = HashDictionaryInstance.Instance;
            if (!hashInstance.Loaded)
            {
                hashInstance.Load();
            }
            hashInstance.dictionary.CreateHelpers();

            foreach (var lib in currentAssets.libraries)
            {
                string path = lib.Location;
                if (!lib.Loaded)
                {
                    lib.Load();
                }

                foreach (var arch in lib.archives)
                {
                    foreach (var file in arch.Value.files)
                    {
                        HashFileInfo hashInfo = new HashFileInfo(file.FileInfo.ph, file.FileInfo.sh, file);

                        if (hashInfo.IsNamed)
                        {
                            if (hashInfo.FileName == "metadata.bin" || hashInfo.FileName == "ft.sig" || hashInfo.Extension.ToUpper() != "GR2")
                            {
                                continue;
                            }

                            NodeAsset assetAll = new NodeAsset(prefixAll + hashInfo.Directory + "/" + hashInfo.FileName, prefixAll + hashInfo.Directory, hashInfo.FileName, hashInfo);
                            assetDict.Add(prefixAll + hashInfo.Directory + "/" + hashInfo.FileName, assetAll);
                            fileDirs.Add(prefixAll + hashInfo.Directory);                            

                            if (hashInfo.FileState == HashFileInfo.State.New)
                            {
                                NodeAsset assetNew = new NodeAsset(prefixNew + hashInfo.Directory + "/" + hashInfo.FileName, prefixNew + hashInfo.Directory, hashInfo.FileName, hashInfo);
                                assetDict.Add(prefixNew + hashInfo.Directory + "/" + hashInfo.FileName, assetNew);
                                fileDirs.Add(prefixNew + hashInfo.Directory);                                
                            }
                            if (hashInfo.FileState == HashFileInfo.State.Modified)
                            {
                                NodeAsset assetMod = new NodeAsset(prefixMod + hashInfo.Directory + "/" + hashInfo.FileName, prefixMod + hashInfo.Directory, hashInfo.FileName, hashInfo);
                                assetDict.Add(prefixMod + hashInfo.Directory + "/" + hashInfo.FileName, assetMod);
                                fileDirs.Add(prefixMod + hashInfo.Directory);                                
                            }
                        }
                        else
                        {
                            if (hashInfo.Extension.ToUpper() != "GR2")
                                continue;
                            hashInfo.Directory = "/unknown/" + hashInfo.Source.Replace(".tor", "");
                            
                            NodeAsset assetUnn = new NodeAsset(prefixUnn + hashInfo.Directory + "/" + hashInfo.Extension + "/" + hashInfo.FileName + "." + hashInfo.Extension, prefixUnn + hashInfo.Directory + "/" + hashInfo.Extension, hashInfo.FileName + "." + hashInfo.Extension, hashInfo);
                            assetDict.Add(prefixUnn + hashInfo.Directory + "/" + hashInfo.Extension + "/" + hashInfo.FileName + "." + hashInfo.Extension, assetUnn);
                            fileDirs.Add(prefixUnn + hashInfo.Directory + "/" + hashInfo.Extension);                            

                            if (hashInfo.FileState == HashFileInfo.State.New)
                            {
                                NodeAsset assetNew = new NodeAsset(prefixNew + hashInfo.Directory + "/" + hashInfo.Extension + "/" + hashInfo.FileName + "." + hashInfo.Extension, prefixNew + hashInfo.Directory + "/" + hashInfo.Extension, hashInfo.FileName + "." + hashInfo.Extension, hashInfo);
                                assetDict.Add(prefixNew + hashInfo.Directory + "/" + hashInfo.Extension + "/" + hashInfo.FileName + "." + hashInfo.Extension, assetNew);
                                fileDirs.Add(prefixNew + hashInfo.Directory + "/" + hashInfo.Extension);                                
                            }
                            if (hashInfo.FileState == HashFileInfo.State.Modified)
                            {
                                NodeAsset assetMod = new NodeAsset(prefixMod + hashInfo.Directory + "/" + hashInfo.Extension + "/" + hashInfo.FileName + "." + hashInfo.Extension, prefixMod + hashInfo.Directory + "/" + hashInfo.Extension, hashInfo.FileName + "." + hashInfo.Extension, hashInfo);
                                assetDict.Add(prefixMod + hashInfo.Directory + "/" + hashInfo.Extension + "/" + hashInfo.FileName + "." + hashInfo.Extension, assetMod);
                                fileDirs.Add(prefixMod + hashInfo.Directory + "/" + hashInfo.Extension);                                
                            }
                        }
                    }
                }
            }

            foreach (var dir in fileDirs)
            {
                string[] temp = dir.ToString().Split('/');
                int intLength = temp.Length;
                for (int intCount2 = 0; intCount2 <= intLength; intCount2++)
                {
                    string output = String.Join("/", temp, 0, intCount2);
                    if (output != "")
                        allDirs.Add(output);
                }
            }

            assetDict.Add("/", new NodeAsset("/", "", "Root", (GomObject)null));
            assetDict.Add("/assets", new NodeAsset("/assets", "/", "Assets", (GomObject)null));
            assetDict.Add("/nodes", new NodeAsset("/nodes", "/", "Nodes", (GomObject)null));

            foreach (var dir in allDirs)
            {
                string[] temp = dir.ToString().Split('/');
                string parentDir = String.Join("/", temp.Take(temp.Length - 1));
                if (parentDir == "")
                    parentDir = "/assets";
                string display = temp.Last();

                NodeAsset asset = new NodeAsset(dir.ToString(), parentDir, display, (GomObject)null);
                if (!assetDict.ContainsKey(dir.ToString()))
                    assetDict.Add(dir.ToString(), asset);
            }
            
            mntMountInfoTable.Clear();
            mntMountInfoTable = null;


            toolStripStatusLabel1.Text = "Loading Tree View Items ...";
            this.Refresh();            

            Func<NodeAsset, string> getId = (x => x.Id);
            Func<NodeAsset, string> getParentId = (x => x.parentId);
            Func<NodeAsset, string> getDisplayName = (x => x.displayName);
            treeViewFast1.SuspendLayout();
            treeViewFast1.BeginUpdate();
            treeViewFast1.LoadItems<NodeAsset>(assetDict, getId, getParentId, getDisplayName);                        
            treeViewFast1.EndUpdate();
            treeViewFast1.ResumeLayout();
            toolStripStatusLabel1.Text = "Loading Complete";
            toolStripProgressBar1.Visible = false;
            enableUI();
            pictureBox1.Visible = false;
            treeViewFast1.Visible = true;
            panelRender = new View_NPC_GR2(this.Handle, this, "renderPanel");
            panelRender.Init();
            System.Runtime.GCSettings.LatencyMode = System.Runtime.GCLatencyMode.Interactive;
            if (treeViewFast1.Nodes.Count > 0) treeViewFast1.Nodes[0].Expand();
        }

        private void disableUI()
        {
            btnDataCollapse.Enabled = false;
            btnModelBrowserHelp.Enabled = false;
            btnStopRender.Enabled = false;
            exportButton.Enabled = false;
            treeViewFast1.Enabled = false;
            tvfDataViewer.Enabled = false;
            dgvDataViewer.Enabled = false;
        }

        private void enableUI()
        {
            btnDataCollapse.Enabled = true;
            btnModelBrowserHelp.Enabled = true;
            btnStopRender.Enabled = true;
            exportButton.Enabled = true;
            treeViewFast1.Enabled = true;
            tvfDataViewer.Enabled = true;
            dgvDataViewer.Enabled = true;
        }


        private async void treeViewFast1_AfterSelect(object sender, TreeViewEventArgs e)
        {
            TreeNode selectedNode = treeViewFast1.SelectedNode;
            NodeAsset tag = (NodeAsset)selectedNode.Tag;
            if (tag.obj != null || tag.objData != null || tag.dynObject != null)
            {
                if (panelRender != null)
                {
                    if (render != null)
                    {
                        panelRender.stopRender();
                        render.Join();
                        panelRender.Clear();
                    }                    
                    renderPanel.Visible = false;
                }
                this.models = new Dictionary<string, GR2>();
                dgvDataViewer.DataSource = null;
                dgvDataViewer.Enabled = false;
                tvfDataViewer.Nodes.Clear();
                tvfDataViewer.Enabled = false;
                if(tag.dynObject != null && tag.dynObject is HashFileInfo)
                {
                    this.toolStripStatusLabel1.Text = "Loading GR2 File...";
                    this.Refresh();
                    HashFileInfo info = (HashFileInfo)tag.dynObject;
                    await Task.Run(() => previewGR2(info));
                    buildDataViewer();
                    renderPanel.Visible = true;
                    treeViewFast1.Enabled = true;
                    toolStripProgressBar1.Visible = false;
                    this.toolStripStatusLabel1.Text = "GR2 Loaded";

                }else  if (tag.obj != null)
                {   
                    GomObject obj = (GomObject)tag.obj;
                    NpcAppearance npcData = null;
                    ItemAppearance itemData = null;
                    object weaponData = null;
                    object visualList = null;
                        try
                    {
                        switch (obj.Name.Substring(0, 3))
                        {
                            case "npp":
                                npcData = (NpcAppearance)this.currentDom.appearanceLoader.Load(obj.Name);
                                this.toolStripStatusLabel1.Text = "Loading NPP Data ...";
                                this.Refresh();
                                await Task.Run(() => previewNPC_GR2(npcData));
                                this.toolStripStatusLabel1.Text = "NPP Loaded";
                                break;
                            case "ipp":
                                itemData = (ItemAppearance)this.currentDom.appearanceLoader.Load(obj.Name);
                                this.toolStripStatusLabel1.Text = "Loading IPP Data ...";
                                this.Refresh();
                                await Task.Run(() => previewIPP_GR2(itemData));
                                this.toolStripStatusLabel1.Text = "IPP Loaded";
                                break;
                            case "itm":
                                string appearSpec = obj.Data.ValueOrDefault<string>("cbtWeaponAppearanceSpec", null);
                                weaponAppearance.TryGetValue(appearSpec.ToLower(), out weaponData);
                                weaponData = (GomObjectData)weaponData;
                                this.toolStripStatusLabel1.Text = "Loading ITM Data ...";
                                this.Refresh();
                                if (weaponData != null)
                                    await Task.Run(() => previewITM_GR2((GomObject)obj, (GomObjectData)weaponData));
                                else
                                    MessageBox.Show("ERROR: Cannot load model! \r\nWeapon Apperance Spec Missing", "Missing Weapon Appearance Spec");
                                this.toolStripStatusLabel1.Text = "ITM Loaded";
                                break;
                            case "dyn":
                                try
                                {
                                    visualList = obj.Data.ValueOrDefault<List<object>>("dynVisualList", null);
                                    this.toolStripStatusLabel1.Text = "Loading DYN Data ...";
                                    this.Refresh();
                                    if (visualList != null)
                                        await Task.Run(() => previewDYN_GR2((GomObject)obj, (List<object>)visualList));
                                    else
                                        MessageBox.Show("ERROR: Cannot load model! \r\nVisual List Missing", "Missing Visual List Spec");
                                    this.toolStripStatusLabel1.Text = "DYN Loaded";
                                }
                                catch (Exception ee)
                                {
                                    MessageBox.Show(ee.Message.ToString() + "\r\n" + ee.InnerException.ToString() + "\r\n" + ee.StackTrace.ToString(), "Error");
                                }
                                break;
                    
                        }
                        buildDataViewer();
                        renderPanel.Visible = true;
                        treeViewFast1.Enabled = true;
                        toolStripProgressBar1.Visible = false;
                    }
                    catch (Exception excep)
                    {
                        MessageBox.Show("Could not load NPC \r\n" + excep.ToString());
                        this.toolStripStatusLabel1.Text = "NPC Load Error";
                        toolStripProgressBar1.Visible = false;
                        treeViewFast1.Enabled = true;
                    }
                }
                else if (tag.objData != null)
                {
                    GomObjectData obj = (GomObjectData)tag.objData;                    
                    if (obj.Dictionary.ContainsKey("mntDataSpecString"))
                    {  
                        try
                        {
                            this.toolStripStatusLabel1.Text = "Loading MNT Data ...";
                            this.Refresh();
                            await Task.Run(() => previewMNT_GR2((GomObjectData)obj));
                        }
                        catch (Exception ee)
                        {
                            MessageBox.Show(ee.Message.ToString() + "\r\n" + ee.InnerException.ToString() + "\r\n" + ee.StackTrace.ToString(), "Error");
                        }
                    }
                    buildDataViewer();
                    renderPanel.Visible = true;
                    treeViewFast1.Enabled = true;
                    toolStripProgressBar1.Visible = false;
                    this.toolStripStatusLabel1.Text = "NPC Loaded"; 
                }            
            }
        }

        private void buildDataViewer()
        {
            tvfDataViewer.Nodes.Clear();
            dgvDataViewer.DataSource = null;
            dataviewDict.Clear();
            dataviewDict.Add("/", new NodeAsset("/", "", "Root", (GomObject)null));
            if (models.Count > 0)
            {
                dataviewDict.Add("/models", new NodeAsset("/models", "/", "Models", (GomObject)null));
                foreach (var model in models)
                {
                    GR2 gr2 = model.Value;
                    NodeAsset asset = new NodeAsset("/models/" + model.Key, "/models", model.Key, gr2);
                    dataviewDict.Add("/models/" + model.Key, asset);
                    if (gr2.attachedModels.Count > 0)
                    {
                        dataviewDict.Add("/models/" + model.Key + "/attached", new NodeAsset("/models/" + model.Key + "/attached", "/models/" + model.Key, "Attached Models", (GomObject)null));
                        foreach (var attachItem in model.Value.attachedModels)
                        {
                            GR2 gr2_attach = attachItem;
                            NodeAsset attachAsset = new NodeAsset("/models/" + model.Key + "/attached/" + gr2_attach.filename, "/models/" + model.Key + "/attached", gr2_attach.filename, gr2_attach);
                            dataviewDict.Add("/models/" + model.Key + "/attached/" + gr2_attach.filename, attachAsset);

                            if (gr2_attach.numMaterials > 0)
                            {
                                if (!dataviewDict.ContainsKey("/models/" + model.Key + "/attached/" + gr2_attach.filename + "/materials"))
                                    dataviewDict.Add("/models/" + model.Key + "/attached/" + gr2_attach.filename + "/materials", new NodeAsset("/models/" + model.Key + "/attached/" + gr2_attach.filename + "/materials", "/models/" + model.Key + "/attached/" + gr2_attach.filename, "Materials", (GomObject)null));
                                foreach (var material in gr2_attach.materials)
                                {
                                    GR2_Material gr2_material = material;
                                    NodeAsset attachMaterial = new NodeAsset("/models/" + model.Key + "/attached/" + gr2_attach.filename + "/materials/" + gr2_material.materialName, "/models/" + model.Key + "/attached/" + gr2_attach.filename + "/materials", gr2_material.materialName, gr2_material);
                                    if (!dataviewDict.ContainsKey("/models/" + model.Key + "/attached/" + gr2_attach.filename + "/materials/" + gr2_material.materialName))
                                        dataviewDict.Add("/models/" + model.Key + "/attached/" + gr2_attach.filename + "/materials/" + gr2_material.materialName, attachMaterial);
                                }
                            }

                            if (gr2_attach.numMeshes > 0)
                            {
                                if (!dataviewDict.ContainsKey("/models/" + model.Key + "/attached/" + gr2_attach.filename + "/meshes"))
                                    dataviewDict.Add("/models/" + model.Key + "/attached/" + gr2_attach.filename + "/meshes", new NodeAsset("/models/" + model.Key + "/attached/" + gr2_attach.filename + "/meshes", "/models/" + model.Key + "/attached/" + gr2_attach.filename, "Meshes", (GomObject)null));
                                foreach (var mesh in gr2_attach.meshes)
                                {
                                    GR2_Mesh gr2_mesh = mesh;
                                    NodeAsset meshAsset = new NodeAsset("/models/" + model.Key + "/meshes/" + gr2_mesh.meshName, "/models/" + model.Key + "/attached/" + gr2_attach.filename + "/meshes", gr2_mesh.meshName, gr2_mesh);
                                    if (!dataviewDict.ContainsKey("/models/" + model.Key + "/meshes/" + gr2_mesh.meshName))
                                        dataviewDict.Add("/models/" + model.Key + "/meshes/" + gr2_mesh.meshName, meshAsset);
                                }
                            }
                        }
                    }

                    if (gr2.numMeshes > 0)
                    {
                        if (!dataviewDict.ContainsKey("/models/" + model.Key + "/meshes"))
                            dataviewDict.Add("/models/" + model.Key + "/meshes", new NodeAsset("/models/" + model.Key + "/meshes", "/models/" + model.Key, "Meshes", (GomObject)null));
                        foreach (var mesh in model.Value.meshes)
                        {
                            GR2_Mesh gr2_mesh = mesh;
                            NodeAsset meshAsset = new NodeAsset("/models/" + model.Key + "/meshes/" + gr2_mesh.meshName, "/models/" + model.Key + "/meshes", gr2_mesh.meshName, gr2_mesh);
                            if (!dataviewDict.ContainsKey("/models/" + model.Key + "/meshes/" + gr2_mesh.meshName))
                                dataviewDict.Add("/models/" + model.Key + "/meshes/" + gr2_mesh.meshName, meshAsset);
                        }
                    }

                    if (gr2.numMaterials > 0)
                    {
                        if(!dataviewDict.ContainsKey("/models/" + model.Key + "/materials"))
                            dataviewDict.Add("/models/" + model.Key + "/materials", new NodeAsset("/models/" + model.Key + "/materials", "/models/" + model.Key, "Materials", (GomObject)null));
                        foreach (var material in model.Value.materials)
                        {
                            GR2_Material gr2_material = material;
                            NodeAsset materialAsset = new NodeAsset("/models/" + model.Key + "/materials/" + gr2_material.materialName, "/models/" + model.Key + "/materials", gr2_material.materialName, gr2_material);
                            if(!dataviewDict.ContainsKey("/models/" + model.Key + "/materials/" + gr2_material.materialName))
                                dataviewDict.Add("/models/" + model.Key + "/materials/" + gr2_material.materialName, materialAsset);
                        }
                    }

                    if (gr2.materialOverride != null)
                    {
                        if (!dataviewDict.ContainsKey("/models/" + model.Key + "/materials"))
                            dataviewDict.Add("/models/" + model.Key + "/materials", new NodeAsset("/models/" + model.Key + "/materials", "/models/" + model.Key, "Materials", (GomObject)null));
                        GR2_Material gr2_material = gr2.materialOverride;
                        NodeAsset materialAsset = new NodeAsset("/models/" + model.Key + "/materials/" + gr2_material.materialName, "/models/" + model.Key + "/materials", gr2_material.materialName, gr2_material);
                        if (!dataviewDict.ContainsKey("/models/" + model.Key + "/materials/" + gr2_material.materialName))
                            dataviewDict.Add("/models/" + model.Key + "/materials/" + gr2_material.materialName, materialAsset);
                    }

                    if (gr2.numBones > 0)
                    {
                        if (!dataviewDict.ContainsKey("/models/" + model.Key + "/bones"))
                            dataviewDict.Add("/models/" + model.Key + "/bones", new NodeAsset("/models/" + model.Key + "/bones", "/models/" + model.Key, "Bones", (GomObject)null));
                        foreach (var bone in model.Value.skeleton_bones)
                        {
                            GR2_Bone_Skeleton gr2_bone = bone;
                            NodeAsset materialAsset = new NodeAsset("/models/" + model.Key + "/bones/" + gr2_bone.boneName, "/models/" + model.Key + "/bones", gr2_bone.boneIndex.ToString() + " - " + gr2_bone.boneName, gr2_bone);
                            if (!dataviewDict.ContainsKey("/models/" + model.Key + "/bones/" + gr2_bone.boneName))
                                dataviewDict.Add("/models/" + model.Key + "/bones/" + gr2_bone.boneName, materialAsset);
                        }
                    }
                }
            }

            if (resources.Count > 0)
            {
                dataviewDict.Add("/resources", new NodeAsset("/resources", "/", "Resources", (GomObject)null));
                foreach (var resource in resources)
                {
                    NodeAsset asset = new NodeAsset("/resources/" + resource.Key, "/models", resource.Key, (GomObject)null);
                    dataviewDict.Add("/resources/" + resource.Key, asset);
                }
            }
            Func<NodeAsset, string> getId = (x => x.Id);
            Func<NodeAsset, string> getParentId = (x => x.parentId);
            Func<NodeAsset, string> getDisplayName = (x => x.displayName);
            tvfDataViewer.SuspendLayout();
            tvfDataViewer.BeginUpdate();
            tvfDataViewer.LoadItems<NodeAsset>(dataviewDict, getId, getParentId, getDisplayName);
            tvfDataViewer.Sort();
            tvfDataViewer.EndUpdate();
            tvfDataViewer.ResumeLayout();
            tvfDataViewer.Enabled = true;
            tvfDataViewer.Nodes[0].Expand();
            dgvDataViewer.Enabled = true;
        }

        private async Task previewGR2(HashFileInfo hashInfo)
        {
            if (InvokeRequired)
            {
                Invoke(new Action(() => previewGR2(hashInfo)));
            }
            else
            {
                string model = hashInfo.FileName;
                var file = hashInfo.file;
                if (file != null)
                {
                    Stream modelStream = file.OpenCopyInMemory();
                    BinaryReader br = new BinaryReader(modelStream);
                    string name = model.Split('/').Last();
                    FileFormats.GR2 gr2_model = new FileFormats.GR2(br, name);
                    gr2_model.transformMatrix = Matrix.Scaling(new Vector3(1.0f, 1.0f, 1.0f));
                    models.Add(model.Substring(model.LastIndexOf('/') + 1), gr2_model);
                    panelRender.LoadModel(models, resources, name, "");
                    render = new Thread(panelRender.startRender);
                    render.IsBackground = true;
                    render.Start();  
                }
            }
        }

        private async Task previewNPC_GR2(NpcAppearance npcData)
        {
            if (InvokeRequired)
            {
                Invoke(new Action(() => previewNPC_GR2(npcData)));
            }
            else
            {
                parseNpcData(npcData);
                panelRender.LoadModel(models, resources, npcData.Fqn, npcData.NppType);
                render = new Thread(panelRender.startRender);
                render.IsBackground = true;
                render.Start();
            }
        }

        string Bodytype = "bmn";
        Dictionary<string, string> Bodytypes = new Dictionary<string, string> {
            {"bfa", "Female BT1"},
            {"bfn", "Female BT2"},
            {"bfs", "Female BT3"},
            {"bfb", "Female BT4"},
            {"bma", "Male BT1"},
            {"bmn", "Male BT2"},
            {"bms", "Male BT3"},
            {"bmf", "Male BT4"} };

        private void ChangeBodyType(object sender, EventArgs e)
        {
            Bodytype = ((ToolStripMenuItem)sender).Name;
        }

        private async Task previewIPP_GR2(ItemAppearance itemData)
        {
            if (InvokeRequired)
            {
                Invoke(new Action(() => previewIPP_GR2(itemData)));
            }
            else
            {
                string model;
                model = itemData.IPP.Model;
                if (itemData.IPP.BodyType != "")
                    model = model.Replace("[bt]", itemData.IPP.BodyType);
                else
                    model = model.Replace("[bt]", Bodytype);
                if (model.Contains(".gr2"))
                {
                    //Console.WriteLine(model);
                    var file = this.currentAssets.FindFile("/resources" + model);
                    if (file != null)
                    {
                        Stream modelStream = file.OpenCopyInMemory();
                        BinaryReader br = new BinaryReader(modelStream);
                        string name = model.Split('/').Last();
                        FileFormats.GR2 gr2_model = new FileFormats.GR2(br, name);
                        string mat0 = itemData.IPP.Material0;
                        string matMir = itemData.IPP.MaterialMirror;
                        string palette1XML = "";
                        string palette2XML = "";

                        if (itemData.IPP.Material0 != null)
                        {                            
                            if (itemData.IPP.PrimaryHue != "")
                                palette1XML = "/resources" + itemData.IPP.PrimaryHue.Split(';').First();

                            if (itemData.IPP.SecondaryHue != "")
                                palette2XML = "/resources" + itemData.IPP.SecondaryHue.Split(';').First();
                         
                            if (itemData.IPP.BodyType.Contains("bf"))
                            {
                                mat0 = mat0.Replace("[gen]", "f").Replace("[bt]", itemData.IPP.BodyType);
                                matMir = mat0.Replace("[gen]", "f").Replace("[bt]", itemData.IPP.BodyType);
                            }
                            else if (itemData.IPP.BodyType.Contains("bm"))
                            {
                                mat0 = mat0.Replace("[gen]", "m").Replace("[bt]", itemData.IPP.BodyType);
                                matMir = mat0.Replace("[gen]", "m").Replace("[bt]", itemData.IPP.BodyType);
                            }
                            else
                            {
                                mat0 = mat0.Replace("[gen]", Bodytype.Substring(1, 1)).Replace("[bt]", Bodytype);
                                matMir = mat0.Replace("[gen]", Bodytype.Substring(1, 1)).Replace("[bt]", Bodytype);                                
                            }

                            if (gr2_model.numMaterials == 0)
                            {                                
                                gr2_model.numMaterials = 1;
                                gr2_model.materials.Add(new GR2_Material(mat0));
                                if (palette1XML != null)
                                    gr2_model.materials[0].palette1XML = palette1XML;
                                if (palette2XML != null)
                                    gr2_model.materials[0].palette2XML = palette2XML;
                            }
                            else if (gr2_model.numMaterials == 1)
                            {   
                                gr2_model.materials[0] = new GR2_Material(mat0);
                                if (palette1XML != null)
                                    gr2_model.materials[0].palette1XML = palette1XML;
                                if (palette2XML != null)
                                    gr2_model.materials[0].palette2XML = palette2XML;
                            }
                            else if (gr2_model.numMaterials == 2)
                            {   
                                gr2_model.materials[0] = new GR2_Material(mat0);
                                gr2_model.materials[1] = new GR2_Material(matMir);
                                if (palette1XML != null)
                                {
                                    gr2_model.materials[0].palette1XML = palette1XML;
                                    gr2_model.materials[1].palette1XML = palette1XML;
                                }
                                if (palette2XML != null)
                                {
                                    gr2_model.materials[0].palette2XML = palette2XML;
                                    gr2_model.materials[1].palette2XML = palette2XML;
                                }
                            }
                        }

                        if (itemData.IPP.AttachedModels.Count() > 0)
                        {
                            foreach (var attach in itemData.IPP.AttachedModels)
                            {
                                string attachFileName = attach;
                                if (itemData.IPP.BodyType != "")
                                    attachFileName = attachFileName.Replace("[bt]", itemData.IPP.BodyType);
                                else
                                    attachFileName = attachFileName.Replace("[bt]", Bodytype);
                                var attachFile = this.currentAssets.FindFile("/resources" + attachFileName);
                                if (attachFile != null)
                                {
                                    Stream attachModelStream = attachFile.OpenCopyInMemory();
                                    BinaryReader br2 = new BinaryReader(attachModelStream);
                                    string attachName = attachFileName.Split('/').Last();
                                    FileFormats.GR2 attachModel = new FileFormats.GR2(br2, attachName);

                                    if (attachModel.numMaterials == 0)
                                    {
                                        attachModel.numMaterials = 1;
                                        attachModel.materials.Add(new GR2_Material(mat0));
                                        if (palette1XML != null)
                                            attachModel.materials[0].palette1XML = palette1XML;
                                        if (palette2XML != null)
                                            attachModel.materials[0].palette2XML = palette2XML;
                                    }
                                    else if (attachModel.numMaterials == 1)
                                    {   
                                        attachModel.materials[0] = new GR2_Material(mat0);
                                        if (palette1XML != null)
                                            attachModel.materials[0].palette1XML = palette1XML;
                                        if (palette2XML != null)
                                            attachModel.materials[0].palette2XML = palette2XML;
                                    }
                                    else if (attachModel.numMaterials == 2)
                                    {   
                                        attachModel.materials[0] = new GR2_Material(mat0);
                                        attachModel.materials[1] = new GR2_Material(matMir);
                                        if (palette1XML != null)
                                        {
                                            attachModel.materials[0].palette1XML = palette1XML;
                                            attachModel.materials[1].palette1XML = palette1XML;
                                        }
                                        if (palette2XML != null)
                                        {
                                            attachModel.materials[0].palette2XML = palette2XML;
                                            attachModel.materials[1].palette2XML = palette2XML;
                                        }
                                    }
                                    attachModel.transformMatrix = Matrix.Scaling(new Vector3(1.0f, 1.0f, 1.0f));
                                    gr2_model.attachedModels.Add(attachModel);
                                }
                            }
                        }
                        gr2_model.transformMatrix = Matrix.Scaling(new Vector3(1.0f, 1.0f, 1.0f));
                        models.Add(model.Substring(model.LastIndexOf('/') + 1), gr2_model);
                    }
                }
                if (model.Contains(".dds"))
                {
                    Stream inputStream = this.currentAssets.FindFile("/resources" + model).OpenCopyInMemory();
                    if (inputStream != null)
                        resources.Add(model.Substring(model.LastIndexOf('/') + 1), inputStream);
                }
                panelRender.LoadModel(models, resources, itemData.Fqn, "ipp");
                render = new Thread(panelRender.startRender);
                render.IsBackground = true;
                render.Start();
            }
        }

        private async Task previewMultipleIPPs_GR2(List<ItemAppearance> itemsData)
        {
            if (InvokeRequired)
            {
                Invoke(new Action(() => previewMultipleIPPs_GR2(itemsData)));
            }
            else
            {                
                foreach(ItemAppearance itemData in itemsData)
                {
                    string model;
                    model = itemData.IPP.Model;
                    if (itemData.IPP.BodyType != "")
                        model = model.Replace("[bt]", itemData.IPP.BodyType);
                    else
                        model = model.Replace("[bt]", Bodytype);
                    if (model.Contains(".gr2"))
                    {   
                        var file = this.currentAssets.FindFile("/resources" + model);
                        if (file != null)
                        {
                            Stream modelStream = file.OpenCopyInMemory();
                            BinaryReader br = new BinaryReader(modelStream);
                            string name = model.Split('/').Last();
                            FileFormats.GR2 gr2_model = new FileFormats.GR2(br, name);
                            string mat0 = itemData.IPP.Material0;
                            string matMir = itemData.IPP.MaterialMirror;
                            string palette1XML = "";
                            string palette2XML = "";

                            if (itemData.IPP.Material0 != null)
                            {   
                                if (itemData.IPP.PrimaryHue != "")
                                    palette1XML = "/resources" + itemData.IPP.PrimaryHue.Split(';').First();

                                if (itemData.IPP.SecondaryHue != "")
                                    palette2XML = "/resources" + itemData.IPP.SecondaryHue.Split(';').First();
                             
                                if (itemData.IPP.BodyType.Contains("bf"))
                                {
                                    mat0 = mat0.Replace("[gen]", "f").Replace("[bt]", itemData.IPP.BodyType);
                                    matMir = mat0.Replace("[gen]", "f").Replace("[bt]", itemData.IPP.BodyType);
                                }
                                else if (itemData.IPP.BodyType.Contains("bm"))
                                {
                                    mat0 = mat0.Replace("[gen]", "m").Replace("[bt]", itemData.IPP.BodyType);
                                    matMir = mat0.Replace("[gen]", "m").Replace("[bt]", itemData.IPP.BodyType);
                                }
                                else
                                {
                                    mat0 = mat0.Replace("[gen]", Bodytype.Substring(1, 1)).Replace("[bt]", Bodytype);
                                    matMir = mat0.Replace("[gen]", Bodytype.Substring(1, 1)).Replace("[bt]", Bodytype);                                    
                                }
                                                                
                                if (gr2_model.numMaterials == 0)
                                {   
                                    gr2_model.numMaterials = 1;
                                    gr2_model.materials.Add(new GR2_Material(mat0));
                                    if (palette1XML != null)
                                        gr2_model.materials[0].palette1XML = palette1XML;
                                    if (palette2XML != null)
                                        gr2_model.materials[0].palette2XML = palette2XML;
                                }
                                else if (gr2_model.numMaterials == 1)
                                {
                                    gr2_model.materials[0] = new GR2_Material(mat0);
                                    if (palette1XML != null)
                                        gr2_model.materials[0].palette1XML = palette1XML;
                                    if (palette2XML != null)
                                        gr2_model.materials[0].palette2XML = palette2XML;
                                }
                                else if (gr2_model.numMaterials == 2)
                                {   
                                    gr2_model.materials[0] = new GR2_Material(mat0);
                                    gr2_model.materials[1] = new GR2_Material(matMir);
                                    if (palette1XML != null)
                                    {
                                        gr2_model.materials[0].palette1XML = palette1XML;
                                        gr2_model.materials[1].palette1XML = palette1XML;
                                    }
                                    if (palette2XML != null)
                                    {
                                        gr2_model.materials[0].palette2XML = palette2XML;
                                        gr2_model.materials[1].palette2XML = palette2XML;
                                    }
                                }
                            }                            

                            if (itemData.IPP.AttachedModels.Count() > 0)
                            {
                                foreach (var attach in itemData.IPP.AttachedModels)
                                {
                                    string attachFileName = attach;
                                    if (itemData.IPP.BodyType != "")
                                        attachFileName = attachFileName.Replace("[bt]", itemData.IPP.BodyType);
                                    else
                                        attachFileName = attachFileName.Replace("[bt]", Bodytype);
                                    var attachFile = this.currentAssets.FindFile("/resources" + attachFileName);
                                    if (attachFile != null)
                                    {
                                        Stream attachModelStream = attachFile.OpenCopyInMemory();
                                        BinaryReader br2 = new BinaryReader(attachModelStream);
                                        string attachName = attachFileName.Split('/').Last();
                                        FileFormats.GR2 attachModel = new FileFormats.GR2(br2, attachName);

                                        if (attachModel.numMaterials == 0)
                                        {   
                                            attachModel.numMaterials = 1;
                                            attachModel.materials.Add(new GR2_Material(mat0));
                                            if (palette1XML != null)
                                                attachModel.materials[0].palette1XML = palette1XML;
                                            if (palette2XML != null)
                                                attachModel.materials[0].palette2XML = palette2XML;
                                        }
                                        else if (attachModel.numMaterials == 1)
                                        {   
                                            attachModel.materials[0] = new GR2_Material(mat0);
                                            if (palette1XML != null)
                                                attachModel.materials[0].palette1XML = palette1XML;
                                            if (palette2XML != null)
                                                attachModel.materials[0].palette2XML = palette2XML;
                                        }
                                        else if (attachModel.numMaterials == 2)
                                        {
                                            attachModel.materials[0] = new GR2_Material(mat0);
                                            attachModel.materials[1] = new GR2_Material(matMir);
                                            if (palette1XML != null)
                                            {
                                                attachModel.materials[0].palette1XML = palette1XML;
                                                attachModel.materials[1].palette1XML = palette1XML;
                                            }
                                            if (palette2XML != null)
                                            {
                                                attachModel.materials[0].palette2XML = palette2XML;
                                                attachModel.materials[1].palette2XML = palette2XML;
                                            }
                                        }
                                        attachModel.transformMatrix = Matrix.Scaling(new Vector3(1.0f, 1.0f, 1.0f));
                                        gr2_model.attachedModels.Add(attachModel);
                                    }
                                }
                            }
                            gr2_model.transformMatrix = Matrix.Scaling(new Vector3(1.0f, 1.0f, 1.0f));
                            models.Add(model.Substring(model.LastIndexOf('/') + 1), gr2_model);
                        }
                    }
                    if (model.Contains(".dds"))
                    {
                        Stream inputStream = this.currentAssets.FindFile("/resources" + model).OpenCopyInMemory();
                        if (inputStream != null)
                            resources.Add(model.Substring(model.LastIndexOf('/') + 1), inputStream);
                    }
                }
                panelRender.LoadModel(models, resources, itemsData.First().Fqn, "ipp");
                render = new Thread(panelRender.startRender);
                render.IsBackground = true;
                render.Start();
            }
        }

        private async Task previewITM_GR2(GomObject obj, GomObjectData itemData)
        {
            if (InvokeRequired)
            {
                Invoke(new Action(() => previewITM_GR2(obj, itemData)));
            }
            else
            {
                string model = itemData.ValueOrDefault<string>("itmModel", null).Replace('\\', '/');
                string fxspec = itemData.ValueOrDefault<string>("itmFxSpec", null);
                if (model.Contains(".gr2"))
                {   
                    var file = this.currentAssets.FindFile("/resources" + model);
                    if (file != null)
                    {
                        Stream modelStream = file.OpenCopyInMemory();
                        BinaryReader br = new BinaryReader(modelStream);
                        string name = model.Split('/').Last();
                        FileFormats.GR2 gr2_model = new FileFormats.GR2(br, name);
                        gr2_model.transformMatrix = Matrix.Scaling(new Vector3(1.0f, 1.0f, 1.0f));
                        models.Add(model.Substring(model.LastIndexOf('/') + 1), gr2_model);
                    }
                }
                parseFXSpec(fxspec);
                panelRender.LoadModel(models, resources, obj.Name, "itm");
                render = new Thread(panelRender.startRender);
                render.IsBackground = true;
                render.Start();
            }
        }

        private async Task previewDYN_GR2(GomObject obj, List<object> visualList)
        {
            if (InvokeRequired)
            {
                Invoke(new Action(() => previewDYN_GR2(obj, visualList)));
            }
            else
            {
                foreach (GomObjectData visItem in visualList)
                {
                    string model = "";
                    string visualName = "";
                    Vector3 rotationVec = new Vector3();
                    Vector3 scaleVec = new Vector3(1.0f, 1.0f, 1.0f);
                    Vector3 positionVec = new Vector3();
                    foreach (KeyValuePair<string, object> itm in visItem.Dictionary)
                    {
                        if (itm.Key == "dynVisualFqn")
                        {
                            if (itm.Value.ToString().Contains(".gr2"))
                                model = itm.Value.ToString();
                            else
                                continue;
                        }
                        else if (itm.Key == "dynVisualName")
                        {
                            visualName = itm.Value.ToString();
                        }
                        else if (itm.Key == "dynRotation" || itm.Key == "dynScale" || itm.Key == "dynPosition" )
                        { 
                            List<Single> value = (List<Single>)itm.Value;
                            if (itm.Key == "dynRotation")
                            {
                                rotationVec = new Vector3(value[0], value[1], value[2]);
                            }
                            else if (itm.Key == "dynScale")
                            {
                                scaleVec = new Vector3(value[0], value[1], value[2]);
                            }
                            else if (itm.Key == "dynPosition")
                            {
                                positionVec = new Vector3(value[0], value[1], value[2]);
                            }
                        }
                        else
                        {
                            continue;
                        }
                    }
                    
                    if (model.Contains("designblockout"))
                        continue;
                    var file = this.currentAssets.FindFile("/resources" + model);
                    if (file != null)
                    {
                        Stream modelStream = file.OpenCopyInMemory();
                        BinaryReader br = new BinaryReader(modelStream);
                        string name = model.Split('/').Last();
                        FileFormats.GR2 gr2_model = new FileFormats.GR2(br, name);
                        gr2_model.transformMatrix = ((( Matrix.Scaling(scaleVec) * 
                                                        Matrix.RotationZ((float)((rotationVec.Z * Math.PI) / 180.0))) * 
                                                        Matrix.RotationX((float)((rotationVec.X * Math.PI) / 180.0))) * 
                                                        Matrix.RotationY((float)((rotationVec.Y * Math.PI) / 180.0))) * 
                                                        Matrix.Translation(positionVec);
                        try
                        {
                            models.Add(visualName, gr2_model);
                        }
                        catch (Exception e)
                        {
                            Console.WriteLine(e.StackTrace.ToString());
                            Console.WriteLine("pause here");
                        }
                    }
                }
                panelRender.LoadModel(models, resources, obj.Name, "dyn");
                render = new Thread(panelRender.startRender);
                render.IsBackground = true;
                render.Start();
            }
        }

        private async Task previewMNT_GR2(GomObjectData obj)
        {
            if (InvokeRequired)
            {
                Invoke(new Action(() => previewMNT_GR2(obj)));
            }
            else
            {
                object fxSpec;
                obj.Dictionary.TryGetValue("mntDataVFX", out fxSpec);
                string fqn = (string)obj.Dictionary["mntDataSpecString"];
                if (fxSpec != null)
                    parseFXSpec(fxSpec.ToString());
                object npcNodeId;
                obj.Dictionary.TryGetValue("4611686299207604004", out npcNodeId);
                if (npcNodeId != null)
                {
                    GomObject npcNode = this.currentDom.GetObject((ulong)npcNodeId);
                    List<object> npcVisualList = this.currentDom.GetObject((ulong)npcNodeId).Data.ValueOrDefault<List<object>>("npcVisualDataList", null);

                    if (npcVisualList != null)
                    {
                        if (npcVisualList.Count > 0)
                        {
                            foreach (GomObjectData visItem in npcVisualList)
                            {
                                if (visItem.Dictionary.ContainsKey("npcTemplateVisualDataAppearance"))
                                {
                                    NpcAppearance npcData = (NpcAppearance)this.currentDom.appearanceLoader.Load((ulong)visItem.Dictionary["npcTemplateVisualDataAppearance"]);
                                    parseNpcData(npcData);
                                }
                            }
                        }
                    }
                }
                else
                {
                    string skeletonModel = "/resources/art/dynamic/spec/bmanew_skeleton.gr2";
                    TorLib.File file = this.currentAssets.FindFile(skeletonModel);
                    if (file != null)
                    {
                        Stream skeletonStream = file.OpenCopyInMemory();
                        BinaryReader br = new BinaryReader(skeletonStream);
                        string name = skeletonModel.Split('/').Last();
                        FileFormats.GR2 gr2_model = new FileFormats.GR2(br, name);
                        gr2_model.transformMatrix = Matrix.Scaling(new Vector3(1.0f, 1.0f, 1.0f));
                        this.models.Add(name, gr2_model);
                    }
                }
           
                if (models.Count() > 0)
                {
                    panelRender.LoadModel(this.models, resources, fqn, "mnt");
                    render = new Thread(panelRender.startRender);
                    render.IsBackground = true;
                    render.Start();
                }
                else
                {
                    MessageBox.Show("No models were found", "Error Loading Models");
                }
            }
        }


        private void parseNpcData(NpcAppearance npcData)
        {
            if (npcData.BodyType != null)
            {
                string skeletonModel;
                if(npcData.BodyType.StartsWith("bf") || npcData.BodyType.StartsWith("bm"))
                    skeletonModel = "/resources/art/dynamic/spec/" + npcData.BodyType + "new_skeleton.gr2";
                else
                    skeletonModel = "/resources/art/dynamic/spec/" + npcData.BodyType + "_skeleton.gr2";
                TorLib.File file = this.currentAssets.FindFile(skeletonModel);
                if (file != null)
                {
                    Stream skeletonStream = file.OpenCopyInMemory();
                    BinaryReader br = new BinaryReader(skeletonStream);
                    string name = skeletonModel.Split('/').Last();
                    FileFormats.GR2 gr2_model = new FileFormats.GR2(br, name);
                    this.models.Add(name, gr2_model);
                }
            }
            foreach (var slotDict in npcData.AppearanceSlotMap)
            {
                if (slotDict.Value.Count() > 1)
                {
                    //Console.WriteLine("pause");
                }
                else if (slotDict.Value.Count() == 1)
                {
                    string model;
                    model = slotDict.Value[0].Model.Replace("[bt]", slotDict.Value[0].BodyType);
                    if (model.Contains(".gr2"))
                    {
                        var filemodel = this.currentAssets.FindFile("/resources" + model);
                        if (filemodel != null)
                        {
                            Stream modelStream = filemodel.OpenCopyInMemory();
                            BinaryReader br = new BinaryReader(modelStream);
                            string name = model.Split('/').Last();
                            FileFormats.GR2 gr2_model = new FileFormats.GR2(br, name);
                            string mat0 = slotDict.Value[0].Material0;
                            string matMir = slotDict.Value[0].MaterialMirror;
                            string palette1XML = "";
                            string palette2XML = "";

                            if (slotDict.Value[0].Material0 != null)
                            {   
                                if (slotDict.Value[0].PrimaryHue != "")
                                    palette1XML = "/resources" + slotDict.Value[0].PrimaryHue.Split(';').First();

                                if (slotDict.Value[0].SecondaryHue != "")
                                    palette2XML = "/resources" + slotDict.Value[0].SecondaryHue.Split(';').First();
                                if (slotDict.Value[0].BodyType.Contains("bf"))
                                {
                                    mat0 = mat0.Replace("[gen]", "f").Replace("[bt]", slotDict.Value[0].BodyType);
                                    matMir = matMir.Replace("[gen]", "f").Replace("[bt]", slotDict.Value[0].BodyType);
                                }
                                else if (slotDict.Value[0].BodyType.Contains("bm"))
                                {
                                    mat0 = mat0.Replace("[gen]", "m").Replace("[bt]", slotDict.Value[0].BodyType);
                                    matMir = matMir.Replace("[gen]", "m").Replace("[bt]", slotDict.Value[0].BodyType);
                                }
                                
                                gr2_model.materials = new List<GR2_Material>();
                                gr2_model.materials.Add(new GR2_Material(mat0));
                                if (matMir != mat0 && matMir != "")
                                    gr2_model.materialOverride = new GR2_Material(matMir);                                                                    
                                gr2_model.numMaterials = (ushort)gr2_model.materials.Count;
                                if (palette1XML != null)                                
                                    gr2_model.materials[0].palette1XML = palette1XML;
                                
                                if (palette2XML != null)                                
                                    gr2_model.materials[0].palette2XML = palette2XML;                                    
                            }                            

                            if (slotDict.Value[0].AttachedModels.Count() > 0)
                            {
                                foreach (var attach in slotDict.Value[0].AttachedModels)
                                {
                                    string attachFileName = attach.Replace("[bt]", slotDict.Value[0].BodyType);
                                    var file = this.currentAssets.FindFile("/resources" + attachFileName);
                                    
                                    if (file != null)
                                    {
                                        Stream attachModelStream = file.OpenCopyInMemory();
                                        BinaryReader br2 = new BinaryReader(attachModelStream);
                                        string attachName = attachFileName.Split('/').Last();
                                        FileFormats.GR2 attachModel = new FileFormats.GR2(br2, attachName);

                                        attachModel.materials = gr2_model.materials;
                                        if (attachModel.numMaterials == 0)
                                        {   
                                            attachModel.numMaterials = 1;
                                            attachModel.materials.Add(new GR2_Material(mat0));
                                            if (palette1XML != null)
                                                attachModel.materials[0].palette1XML = palette1XML;
                                            if (palette2XML != null)
                                                attachModel.materials[0].palette2XML = palette2XML;
                                        }
                                        else if (attachModel.numMaterials == 1)
                                        {   
                                            attachModel.materials[0] = new GR2_Material(mat0);
                                            if (palette1XML != null)
                                                attachModel.materials[0].palette1XML = palette1XML;
                                            if (palette2XML != null)
                                                attachModel.materials[0].palette2XML = palette2XML;
                                        }
                                        else if (attachModel.numMaterials == 2)
                                        {   
                                            attachModel.materials.Add(new GR2_Material(mat0));
                                            attachModel.materials.Add(new GR2_Material(matMir));
                                            if (palette1XML != null)
                                            {
                                                attachModel.materials[0].palette1XML = palette1XML;
                                                attachModel.materials[1].palette1XML = palette1XML;
                                            }
                                            if (palette2XML != null)
                                            {
                                                attachModel.materials[0].palette2XML = palette2XML;
                                                attachModel.materials[1].palette2XML = palette2XML;
                                            }
                                        }
                                        attachModel.transformMatrix = Matrix.Scaling(new Vector3(1.0f, 1.0f, 1.0f));
                                        gr2_model.attachedModels.Add(attachModel);
                                    }
                                }
                            }
                            gr2_model.transformMatrix = Matrix.Scaling(new Vector3(1.0f, 1.0f, 1.0f));
                            models.Add(slotDict.Key.ToString(), gr2_model);                            
                        }
                    }
                    if (model.Contains(".dds"))
                    {   
                        resources.Add(slotDict.Key.ToString(), slotDict.Value.First().Model);                        
                    }

                    if (model.Contains(".xml"))
                    {
                        string dynFqn = model.Replace("/art/", "").Replace(".xml", "").Replace("/", ".");
                        GomObject dynObj = currentDom.GetObject(dynFqn);                     
                        if (dynObj != null)
                        {   
                            resources.Add(slotDict.Key, dynObj);
                        }
                    }
                }                
            }
        }

        private void parseFXSpec(string fxspec)
        {
            if (fxspec != null)
            {
                string fxName = "/resources/art/fx/fxspec/" + fxspec + ".fxspec";
                fxName = fxName.Replace(".fxspec.fxspec", ".fxspec");
                var file = this.currentAssets.FindFile(fxName);
                if (file != null)
                {
                    XmlDocument doc = new XmlDocument();
                    doc.Load(file.OpenCopyInMemory());
                    XmlNode modelList = doc.SelectSingleNode("//node()[@name='_fxModelList']");
                    XmlNodeList resourceList = modelList.SelectNodes(".//node()[@name='_fxResourceName']");

                    XmlNode emitterList = doc.SelectSingleNode("//node()[@name='_fxEmitterList']");
                    XmlNode modelFadeList = doc.SelectSingleNode("//node()[@name='_fxModelFaderList']");
                    Dictionary<string, bool> modelEnabled = new Dictionary<string, bool>();                    
                    foreach (XmlNode node in modelFadeList)
                    {
                        XmlNode assetName = node.SelectSingleNode("./node()[@name='_fxAssetName']");
                        XmlNode enabled = node.SelectSingleNode("./node()[@name='_mfStart']");
                        if (assetName != null && assetName.InnerText != "")
                        {
                            if (!modelEnabled.ContainsKey(assetName.InnerText))
                            {
                                if (enabled != null && enabled.InnerText == "1")
                                    modelEnabled.Add(assetName.InnerText, false);
                                else
                                    modelEnabled.Add(assetName.InnerText, true);
                            }
                        }
                    }

                    if (modelList.ChildNodes.Count > 0)
                    {
                        foreach (XmlNode entryNode in modelList.ChildNodes)
                        {
                            if (entryNode.ChildNodes.Count > 0)
                            {
                                //XmlNode parentRotVecNode = null;
                                //XmlNode parentPosVecNode = null;
                                //XmlNode parentBoneNode = null;
                                XmlNode resourceFxName = entryNode.SelectSingleNode("./node()[@name='_fxName']");
                                XmlNode resourceName = entryNode.SelectSingleNode("./node()[@name='_fxResourceName']");
                                XmlNode rotationVecNode = entryNode.SelectSingleNode("./node()[@name='_fxAttachRotation']");                                
                                XmlNode positionVecNode = entryNode.SelectSingleNode("./node()[@name='_fxAttachPosition']");
                                XmlNode boneAttachNode = entryNode.SelectSingleNode("./node()[@name='_fxAttachBone']");
                                XmlNode attachRelativeNode = entryNode.SelectSingleNode("./node()[@name='_fxAttachRelative']");
                                XmlNode attachTo = entryNode.SelectSingleNode("./node()[@name='_fxAttachTo']");

                                Vector3 rotationVec = new Vector3();
                                Vector3 positionVec = new Vector3();
                                Vector3 scaleVec = new Vector3(1.0f, 1.0f, 1.0f);

                                if (resourceName.InnerText.Contains(".gr2"))
                                {   
                                    if (resourceName.InnerText.Contains("vfx") || resourceName.InnerText.Contains("spawn") || resourceName.InnerText.Contains("fx_all_lasersight_flare"))
                                        continue;
                                    string modelPath = "/resources" + resourceName.InnerText.Replace("\\", "/");
                                    var attachModel = this.currentAssets.FindFile(modelPath);
                                    if (attachModel != null)
                                    {
                                        Stream modelStream = attachModel.OpenCopyInMemory();
                                        BinaryReader br = new BinaryReader(modelStream);
                                        string name = modelPath.Split('/').Last();
                                        FileFormats.GR2 gr2_model = new FileFormats.GR2(br, name);

                                        //if (attachTo != null && attachTo.InnerText != "")
                                        //{
                                            //XmlNode testList3 = doc.SelectSingleNode("//node()[@name='_fxName' and text() = '" + attachTo.InnerText.Replace("\"", "") + "']");
                                         ///   Console.WriteLine("./node()[text() = '" + attachTo.InnerText.Replace("\"", "") + "']");
                                            //XmlNode testList = modelList.SelectSingleNode("./node()[@name='_fxName' and text() = '" + attachTo.InnerText.Replace("\"", "") + "']");
                                            //XmlNode testList2 = emitterList.SelectSingleNode("./node()[@name='_fxName' and text() = '" + attachTo.InnerText.Replace("\"", "") + "']");
                                            
                                            //XmlNode testList = modelList.SelectSingleNode("./node()[@name='_fxName' and text() = '" + attachTo.InnerText.Replace("\"", "") + "']");
                                            //XmlNode testList2 = emitterList.SelectSingleNode("./node()[@name='_fxName' and text() = '" + attachTo.InnerText.Replace("\"", "") + "']");                                           
                                            
                                            /*
                                            if (testList3 != null)
                                            {
                                                //Console.WriteLine(testList3.InnerText);                                                
                                                parentRotVecNode = testList3.ParentNode.SelectSingleNode("./node()[@name='_fxAttachRotation']");
                                                parentPosVecNode = testList3.ParentNode.SelectSingleNode("./node()[@name='_fxAttachPosition']");
                                                parentBoneNode = testList3.ParentNode.SelectSingleNode("./node()[@name='_fxAttachBone']");
                                                //attachRelativeNode = testList3.ParentNode.SelectSingleNode("./node()[@name='_fxAttachRelative']");                                                
                                            }*/
                                        //}
                                        /*
                                        if (parentRotVecNode != null && parentRotVecNode.InnerText != "")
                                        {
                                            string[] temp = parentRotVecNode.InnerText.ToString().Replace("(", "").Replace(")", "").Split(',');
                                            rotationVec = new Vector3(float.Parse(temp[0]), float.Parse(temp[1]), float.Parse(temp[2]));                                          
                                        } */                                    

                                        if (rotationVecNode != null && rotationVecNode.InnerText != "")
                                        {
                                            string[] temp = rotationVecNode.InnerText.ToString().Replace("(", "").Replace(")", "").Split(',');
                                            rotationVec = new Vector3(float.Parse(temp[0]), float.Parse(temp[1]), float.Parse(temp[2]));                                           
                                        }
                                        
                                        /*
                                        if (scaleVecNode != null && scaleVecNode.InnerText != "")
                                        {   
                                            string[] temp = scaleVecNode.InnerText.ToString().Replace("(", "").Replace(")", "").Split(',');
                                            Vector3 scaleVec = new Vector3(float.Parse(temp[0]), float.Parse(temp[1]), float.Parse(temp[2]));
                                            if (scaleVec != blankVec)
                                            {
                                                Matrix scaleMatrix = Matrix.Scaling(scaleVec);
                                                gr2_model.scaleMatrix = scaleMatrix;
                                            }
                                        }
                                        */
                                        /*
                                        if (parentPosVecNode != null && parentPosVecNode.InnerText != "")
                                        {
                                            string[] temp = parentPosVecNode.InnerText.ToString().Replace("(", "").Replace(")", "").Split(',');
                                            positionVec = new Vector3(float.Parse(temp[0]), float.Parse(temp[1]), float.Parse(temp[2]));                                         
                                        }
                                        else
                                        {*/

                                            if (positionVecNode != null && positionVecNode.InnerText != "")
                                            {
                                                string[] temp = positionVecNode.InnerText.ToString().Replace("(", "").Replace(")", "").Split(',');
                                                positionVec = new Vector3(float.Parse(temp[0]), float.Parse(temp[1]), float.Parse(temp[2]));                                               
                                            }
                                        //}
                                        /*
                                        if (parentBoneNode != null && parentBoneNode.InnerText != "")
                                        {
                                            foreach (KeyValuePair<string, GR2> model in models)
                                            {
                                                GR2_Attachment attach = (GR2_Attachment)model.Value.attachments.SingleOrDefault(x => x.attachName.ToLower() == parentBoneNode.InnerText.ToLower());
                                                if (attach != null)
                                                {
                                                    gr2_model.attachMatrix = attach.attach_matrix;
                                                }

                                                GR2_Bone_Skeleton boneAttach = (GR2_Bone_Skeleton)model.Value.skeleton_bones.SingleOrDefault(x => x.boneName.ToLower() == parentBoneNode.InnerText.ToLower());
                                                if (boneAttach != null)
                                                {
                                                    gr2_model.attachMatrix = boneAttach.root;
                                                }
                                            }
                                        }
                                        else
                                        {
                                        */
                                            if (boneAttachNode != null && boneAttachNode.InnerText != "")
                                            {
                                                foreach (KeyValuePair<string, GR2> model in models)
                                                {
                                                    GR2_Attachment attach = (GR2_Attachment)model.Value.attachments.SingleOrDefault(x => x.attachName.ToLower() == boneAttachNode.InnerText.ToLower());
                                                    if (attach != null)
                                                    {
                                                        gr2_model.attachMatrix = attach.attach_matrix;
                                                    }

                                                    GR2_Bone_Skeleton boneAttach = (GR2_Bone_Skeleton)model.Value.skeleton_bones.SingleOrDefault(x => x.boneName.ToLower() == boneAttachNode.InnerText.ToLower());
                                                    if (boneAttach != null)
                                                    {
                                                        gr2_model.attachMatrix = boneAttach.root;
                                                    }
                                                }
                                            }

                                                /*
                                                GR2_Attachment attach = (GR2_Attachment)models.First().Value.attachments.SingleOrDefault(x => x.attachName == boneAttachNode.InnerText);
                                                if (attach != null)
                                                {
                                                    gr2_model.attachMatrix = attach.attach_matrix;
                                                }
                                                 */
                                            //}
                                        //}
                                        /*
                                        if (attachTo != null && attachTo.InnerText != "")
                                        {   
                                            if (boneAttachNode != null && boneAttachNode.InnerText != "")
                                            {       
                                                Console.WriteLine("pause - attachTo & boneAttachNode");
                                            }
                                            foreach (XmlNode emitterNode in emitterList.ChildNodes)
                                            {   
                                                XmlNode emitterNameNode = emitterNode.SelectSingleNode("./node()[@name='_fxName']");

                                                if (emitterNameNode != null)
                                                {
                                                    if (emitterNameNode.InnerText == attachTo.InnerText)
                                                    {
                                                        XmlNode emitterRotationVecNode = emitterNode.SelectSingleNode("./node()[@name='_fxAttachRotation']");
                                                        //XmlNode emitterScaleVecNode = emitterNode.SelectSingleNode("node()[@name='_fxScale']");
                                                        XmlNode emitterPositionVecNode = emitterNode.SelectSingleNode("./node()[@name='_fxAttachPosition']");
                                                        XmlNode emitterBoneAttachNode = emitterNode.SelectSingleNode("./node()[@name='_fxAttachBone']");

                                                        if (emitterBoneAttachNode != null)
                                                        {
                                                            foreach (KeyValuePair<string, GR2> model in models)
                                                            {
                                                                GR2_Attachment attach = (GR2_Attachment)model.Value.attachments.SingleOrDefault(x => x.attachName.ToLower() == emitterBoneAttachNode.InnerText.ToLower());
                                                                if (attach != null)
                                                                {
                                                                    gr2_model.attachMatrix = attach.attach_matrix;
                                                                }

                                                                GR2_Bone_Skeleton boneAttach = (GR2_Bone_Skeleton)model.Value.skeleton_bones.SingleOrDefault(x => x.boneName.ToLower() == emitterBoneAttachNode.InnerText.ToLower());
                                                                if (boneAttach != null)
                                                                {
                                                                    gr2_model.attachMatrix = boneAttach.root;
                                                                }
                                                            }
                                                        }
                                                        Console.WriteLine("pause");
                                                    }
                                                }
                                            }
                                            
                                        }    */                                    
                                        if (modelEnabled.ContainsKey(resourceFxName.InnerText))
                                        {
                                            bool enabled = false;
                                            modelEnabled.TryGetValue(resourceFxName.InnerText, out enabled);
                                            gr2_model.enabled = enabled;                                            
                                        }
                                        gr2_model.transformMatrix = (((Matrix.Scaling(scaleVec) *
                                                        Matrix.RotationZ((float)((rotationVec.Z * Math.PI) / 180.0))) *
                                                        Matrix.RotationX((float)((rotationVec.X * Math.PI) / 180.0))) *
                                                        Matrix.RotationY((float)((rotationVec.Y * Math.PI) / 180.0))) *
                                                        Matrix.Translation(positionVec);
                                        models.Add(resourceFxName.InnerText, gr2_model);
                                    }
                                }
                            }
                        }
                    }
                }
            }
        }
       
        private void renderPanel_MouseHover(object sender, EventArgs e)
        {
            if (!_closing)
                renderPanel.Focus();
        }

        private void ModelBrowser_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (render != null)
            {
                panelRender.stopRender();
                render.Join();
                panelRender.Clear();
            }
            panelRender.Dispose();
            assetDict.Clear();
            nodeKeys.Clear();
            models.Clear();
            resources.Clear();
            this.Dispose();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            panelRender.ExportGeometry(Bodytype);
        }

        private void treeViewFast1_MouseUp(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Right)
            {
                treeViewFast1.SelectedNode = treeViewFast1.GetNodeAt(e.X, e.Y);

                if (treeViewFast1.SelectedNode != null && treeViewFast1.SelectedNode.Nodes.Count > 0)
                {
                    if (treeViewFast1.SelectedNode.Name.Contains("ipp."))
                    {
                        contextMenuStrip1.Show(treeViewFast1, e.Location);
                    }
                    else if (treeViewFast1.SelectedNode.Name == "ipp")
                    {
                        if (BodyTypeStrip.Items.Count == 0)
                        {
                            foreach (var bt in Bodytypes)
                            {
                                var btStripItem = new ToolStripMenuItem(
                                    bt.Value, //text
                                    null, //image
                                    new EventHandler(ChangeBodyType), //event handler
                                    bt.Key);
                                BodyTypeStrip.Items.Add(btStripItem);
                            }
                        }
                        BodyTypeStrip.Show(treeViewFast1, e.Location);
                    }
                }
            }
        }

        private async void toolStripMenuItemViewAll_Click(object sender, EventArgs e)
        {
            items.Clear();
            treeViewFast1.Enabled = false;
            tvfDataViewer.Nodes.Clear();
            tvfDataViewer.Enabled = false;
            toolStripProgressBar1.Visible = true;
            this.toolStripStatusLabel1.Text = "Loading IPP Data ...";
            this.Refresh();
            TreeNode selectedNode = treeViewFast1.SelectedNode;

            foreach (TreeNode node in selectedNode.Nodes)
            {
                NodeAsset tag = (NodeAsset)node.Tag;
                if (tag.obj != null)
                {
                    GomObject obj = (GomObject)tag.obj;
                    ItemAppearance itemData = (ItemAppearance)this.currentDom.appearanceLoader.Load(obj.Name);
                    items.Add(itemData);
                }
            }

            if (panelRender != null)
            {
                renderPanel.Visible = false;
                if (render != null)
                {
                    panelRender.stopRender();
                    render.Join();
                    panelRender.Clear();
                }
            }

            await Task.Run(() => previewMultipleIPPs_GR2(items));
            buildDataViewer();
            renderPanel.Visible = true;
            treeViewFast1.Enabled = true;
            toolStripProgressBar1.Visible = false;
            this.toolStripStatusLabel1.Text = "NPC Loaded";
        }

        private void btnModelBrowserHelp_Click(object sender, EventArgs e)
        {
            ModelBrowserHelp formHelp = new ModelBrowserHelp();
            formHelp.Show();
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

        private void btnStopRender_Click(object sender, EventArgs e)
        {
            if (panelRender != null)
            {
                renderPanel.Visible = false;
                if (render != null)
                {
                    panelRender.stopRender();
                    render.Join();
                    panelRender.Clear();
                }
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

        private void btnDataCollapse_Click(object sender, EventArgs e)
        {
            bool current = splitContainer3.Panel2Collapsed;
            if (current)
            {
                splitContainer3.Panel2Collapsed = false;
                btnDataCollapse.Text = "Hide Data Viewer";
            }
            else
            {
                splitContainer3.Panel2Collapsed = true;
                btnDataCollapse.Text = "Show Data Viewer";
            }
        }

        private void tvfDataViewer_AfterSelect(object sender, TreeViewEventArgs e)
        {
            TreeNode selectedNode = tvfDataViewer.SelectedNode;
            NodeAsset tag = (NodeAsset)selectedNode.Tag;
            if (tag.dynObject != null)
            {
                DataTable dt = new DataTable();
                dt.Columns.Add("Property");
                dt.Columns.Add("Value");
                dgvDataViewer.DataSource = dt;
                dgvDataViewer.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.AllCells;
                if (tag.dynObject is GR2)
                {
                    GR2 model = (GR2)tag.dynObject;

                    dt.Rows.Add(new string[] { "Render", model.enabled.NullSafeToString() });
                    dt.Rows.Add(new string[] { "# Attachments", model.numAttach.NullSafeToString() });
                    dt.Rows.Add(new string[] { "# Bones", model.numBones.NullSafeToString() });
                    dt.Rows.Add(new string[] { "# Meshes", model.numMeshes.NullSafeToString() });
                    dt.Rows.Add(new string[] { "# Materials", model.numMaterials.NullSafeToString() });                    
                }
                else if (tag.dynObject is GR2_Material)
                {
                    GR2_Material material = (GR2_Material)tag.dynObject;
                    dt.Rows.Add(new string[] { "Type", material.derived.NullSafeToString() });
                    dt.Rows.Add(new string[] { "DiffuseMap", material.diffuseDDS.NullSafeToString() });
                    dt.Rows.Add(new string[] { "RotationMap1", material.rotationDDS.NullSafeToString() });
                    dt.Rows.Add(new string[] { "GlossMap", material.glossDDS.NullSafeToString() });
                    dt.Rows.Add(new string[] { "PaletteMask", material.paletteDDS.NullSafeToString() });
                    dt.Rows.Add(new string[] { "PaletteMaskMap", material.paletteMaskDDS.NullSafeToString() });
                    dt.Rows.Add(new string[] { "UsesEmissive", material.useEmissive.NullSafeToString() });
                    dt.Rows.Add(new string[] { "Pal 1", material.palette1.NullSafeToString() });
                    dt.Rows.Add(new string[] { "Pal 1 Met Spec", material.palette1MetSpec.NullSafeToString() });
                    dt.Rows.Add(new string[] { "Pal 1 Spec", material.palette1Spec.NullSafeToString() });
                    dt.Rows.Add(new string[] { "Pal 2", material.palette2.NullSafeToString() });
                    dt.Rows.Add(new string[] { "Pal 2 Met Spec", material.palette2MetSpec.NullSafeToString() });
                    dt.Rows.Add(new string[] { "Pal 2 Spec", material.palette2Spec.NullSafeToString() });
                    
                    dt.Rows.Add(new string[] { "FacePaint Map", material.facepaintDDS.NullSafeToString() });
                    dt.Rows.Add(new string[] { "Complexion Map", material.complexionDDS.NullSafeToString() });
                    dt.Rows.Add(new string[] { "Age Map", material.ageDDS.NullSafeToString() });

                }
                else if (tag.dynObject is GR2_Mesh)
                {
                    GR2_Mesh mesh = (GR2_Mesh)tag.dynObject;
                    dt.Rows.Add(new string[] { "# Bones", mesh.numBones.NullSafeToString() });
                    dt.Rows.Add(new string[] { "# Pieces", mesh.numPieces.NullSafeToString() });
                    dt.Rows.Add(new string[] { "# Vertices", mesh.numVerts.NullSafeToString() });
                }
                else if (tag.dynObject is GR2_Bone_Skeleton)
                {
                    GR2_Bone_Skeleton bone = (GR2_Bone_Skeleton)tag.dynObject;
                    dt.Rows.Add(new string[] { "Bone Name", bone.boneName.NullSafeToString() });
                    dt.Rows.Add(new string[] { "Bone Index", bone.boneIndex.NullSafeToString() });
                    dt.Rows.Add(new string[] { "Bone Parent Index", bone.parentBoneIndex.NullSafeToString() });
                }
                else
                {

                }
            }
        }

        private void tvfDataViewer_MouseUp(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Right)
            {
                tvfDataViewer.SelectedNode = tvfDataViewer.GetNodeAt(e.X, e.Y);

                if (tvfDataViewer.SelectedNode != null && tvfDataViewer.SelectedNode.Tag is NodeAsset)
                {
                    NodeAsset asset = (NodeAsset)tvfDataViewer.SelectedNode.Tag;
                    if (asset.dynObject != null && asset.dynObject is GR2)
                        contextMenuStrip2.Show(tvfDataViewer, e.Location);
                    else if (asset.dynObject is GR2_Material)
                        contextMenuStrip4.Show(tvfDataViewer, e.Location);
                }
            }
        }

        private void toolStripMenuItem1_Click(object sender, EventArgs e)
        {
            NodeAsset asset = (NodeAsset)tvfDataViewer.SelectedNode.Tag;
            if (asset.dynObject != null && asset.dynObject is GR2)
            {
                GR2 model = (GR2)asset.dynObject;
                model.enabled = !model.enabled;
            }
        }

        private void toolStripMenuItem2_Click(object sender, EventArgs e)
        {
            NodeAsset asset = (NodeAsset)tvfDataViewer.SelectedNode.Tag;
            if (asset.dynObject != null && asset.dynObject is GR2_Material)
            {
                GR2_Material material = (GR2_Material)asset.dynObject;
                ModelBrowserViewMaterial matView = new ModelBrowserViewMaterial(material);
                matView.Show();
            }
        }
    }
}
