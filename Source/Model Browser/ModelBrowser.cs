using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using GomLib;
using GomLib.Models;
using TorLib;
using FileFormats;
using SlimDX;
using System.Xml;

namespace tor_tools
{
    public partial class ModelBrowser : Form
    {
        public bool _closing = false;
        private readonly Dictionary<string, NodeAsset> assetDict = new Dictionary<string, NodeAsset>();
        public Assets currentAssets;
        private DataObjectModel currentDom;
        private readonly Dictionary<string, NodeAsset> dataviewDict = new Dictionary<string, NodeAsset>();
        private readonly List<string> helmetClosed = new List<string>()
                {
                    "helmet", "gemask24", "gemask08", "gemask08suv", "gemask10", "swmask02", "swmask04", "gemask07",
                    "gemask28", "gemask33", "jwmask03", "jwmask05", "gehathair01"
                };
        public List<ItemAppearance> items = new List<ItemAppearance>();
        Dictionary<object, object> mntMountInfoTable = new Dictionary<object, object>();
        public Dictionary<string, GR2> models = new Dictionary<string, GR2>();
        private readonly List<string> nodeKeys = new List<string>();
        private View_NPC_GR2 panelRender;
        public Assets previousAssets;
        private DataObjectModel previousDom;
        private Thread render;
        public Dictionary<string, object> resources = new Dictionary<string, object>();
        private readonly List<string> tagExclusion = new List<string>()
                {
                    "abyssin", "anomid", "aqualish", "arcona", "bith", "bothan", "chagrian", "daichyura",
                    "devaronian", "duros", "gammorean", "gand", "gormak", "gran", "houk", "ishitib", "kaleesh",
                    "keldor", "kitonak", "klatooinian", "krex", "krexorn", "kubaz", "moncalamari", "nautolan",
                    "nikto", "ongree", "rakata", "rodian", "selkath", "snivvian", "sullustan", "togruta",
                    "trandoshan", "weequay"
                };
        Dictionary<object, object> weaponAppearance = new Dictionary<object, object>();

        public ModelBrowser(string assetLocation, bool usePTS,
            string previousAssetLocation, bool previousUsePTS,
            bool loadprevious)
        {
            InitializeComponent();

            List<object> args = new List<object>
            {
                assetLocation,
                usePTS,
                previousAssetLocation,
                previousUsePTS,
                loadprevious
            };

            toolStripProgressBar1.Visible = true;
            toolStripStatusLabel1.Text = "Loading Assets";
            DisableUI();
            backgroundWorker1.RunWorkerAsync(args);
        }

        private void BackgroundWorker1_DoWork(object sender, DoWorkEventArgs workEvent)
        {
            List<object> args = workEvent.Argument as List<object>;

            //Load current assets.
            currentAssets = AssetHandler.Instance.GetCurrentAssets((string)args[0], (bool)args[1]);
            currentDom = DomHandler.Instance.GetCurrentDOM(currentAssets);

            if ((bool)args[4])
            {
                //Load previous assets.
                previousAssets = AssetHandler.Instance.GetPreviousAssets((string)args[2], (bool)args[3]);
                previousDom = DomHandler.Instance.GetPreviousDOM(previousAssets);
            }
        }

        private void BackgroundWorker1_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs completedEvent)
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
            if (previousDom != null)
            {
                for (int i = 0; i < itmList.Count; i++)
                {
                    GomObject newObj = itmList[i];

                    GomObject oldObj = previousDom.GetObject(newObj.Name);
                    if (oldObj == null)
                    {
                        //Node is new.
                        newNodeIndexes.Add(i);
                    }
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

            mntMountInfoTable = currentDom.GetObject("mntMountInfoPrototype").Data.Get<Dictionary<object, object>>("mntMountInfoData");

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

                    if (item.Name.StartsWith("itm."))
                    {
                        //Try and get the item name.
                        if (item.Data.ContainsKey("locTextRetrieverMap"))
                        {
                            GomObjectData nameLookupData = (GomObjectData)item.Data.Get<Dictionary<object, object>>("locTextRetrieverMap")[-2761358831308646330];
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

                NodeAsset asset = new NodeAsset(item.Name.ToString(), parent, display, currentDom);
                assetDict.Add(item.Name.ToString(), asset);
                item.Unload(); //make sure these aren't hanging around
            }

            //Build the new list.
            foreach (int i in newNodeIndexes)
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
                            GomObjectData nameLookupData = (GomObjectData)item.Data.Get<Dictionary<object, object>>("locTextRetrieverMap")[-2761358831308646330];
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

                value.Dictionary.TryGetValue("mntDataSpecString", out object spec);

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

                NodeAsset asset = new NodeAsset(dir.ToString(), parentDir, display, null);
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
                        HashFileInfo hashInfo = new HashFileInfo(file.FileInfo.PrimaryHash, file.FileInfo.SecondaryHash, file);

                        if (hashInfo.IsNamed)
                        {
                            if (hashInfo.FileName == "metadata.bin" || hashInfo.FileName == "ft.sig" || hashInfo.Extension.ToUpper() != "GR2")
                            {
                                continue;
                            }

                            NodeAsset assetAll = new NodeAsset(prefixAll + hashInfo.Directory + "/" + hashInfo.FileName, prefixAll + hashInfo.Directory, hashInfo.FileName, hashInfo);
                            if (!assetDict.ContainsKey(prefixAll + hashInfo.Directory + "/" + hashInfo.FileName))
                                assetDict.Add(prefixAll + hashInfo.Directory + "/" + hashInfo.FileName, assetAll);
                            fileDirs.Add(prefixAll + hashInfo.Directory);

                            if (hashInfo.FileState == HashFileInfo.State.New)
                            {
                                string assetFilename = String.Format("{0}{1}/{2}", prefixNew, hashInfo.Directory, hashInfo.FileName);
                                if (!assetDict.ContainsKey(assetFilename))
                                {
                                    NodeAsset assetNew = new NodeAsset(prefixNew + hashInfo.Directory + "/" + hashInfo.FileName, prefixNew + hashInfo.Directory, hashInfo.FileName, hashInfo);
                                    assetDict.Add(prefixNew + hashInfo.Directory + "/" + hashInfo.FileName, assetNew);
                                    fileDirs.Add(prefixNew + hashInfo.Directory);
                                }
                            }
                            if (hashInfo.FileState == HashFileInfo.State.Modified)
                            {
                                string assetFilename = String.Format("{0}{1}/{2}", prefixMod, hashInfo.Directory, hashInfo.FileName);
                                if (!assetDict.ContainsKey(assetFilename))
                                {
                                    NodeAsset assetMod = new NodeAsset(assetFilename, prefixMod + hashInfo.Directory, hashInfo.FileName, hashInfo);
                                    assetDict.Add(assetFilename, assetMod);
                                    fileDirs.Add(prefixMod + hashInfo.Directory);
                                }
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

            assetDict.Add("/", new NodeAsset("/", "", "Root", null));
            assetDict.Add("/assets", new NodeAsset("/assets", "/", "Assets", null));
            assetDict.Add("/nodes", new NodeAsset("/nodes", "/", "Nodes", null));

            foreach (var dir in allDirs)
            {
                string[] temp = dir.ToString().Split('/');
                string parentDir = String.Join("/", temp.Take(temp.Length - 1));
                if (parentDir == "")
                    parentDir = "/assets";
                string display = temp.Last();

                NodeAsset asset = new NodeAsset(dir.ToString(), parentDir, display, null);
                if (!assetDict.ContainsKey(dir.ToString()))
                    assetDict.Add(dir.ToString(), asset);
            }

            mntMountInfoTable.Clear();
            mntMountInfoTable = null;

            toolStripStatusLabel1.Text = "Loading Tree View Items ...";

            Refresh();

            Func<NodeAsset, string> getId = (x => x.Id);
            Func<NodeAsset, string> getParentId = (x => x.parentId);
            Func<NodeAsset, string> getDisplayName = (x => x.displayName);

            treeViewList.SuspendLayout();
            treeViewList.BeginUpdate();
            treeViewList.LoadItems<NodeAsset>(assetDict, getId, getParentId, getDisplayName);
            treeViewList.EndUpdate();
            treeViewList.ResumeLayout();
            treeViewList.Visible = true;

            toolStripStatusLabel1.Text = "Loading Complete";
            toolStripProgressBar1.Visible = false;

            pictureBox1.Visible = false;

            EnableUI();

            panelRender = new View_NPC_GR2(Handle, this, "renderPanel");
            panelRender.Init();

            System.Runtime.GCSettings.LatencyMode = System.Runtime.GCLatencyMode.Interactive;

            if (treeViewList.Nodes.Count > 0)
            {
                treeViewList.Nodes[0].Expand();
                if (treeViewList.Nodes[0].Nodes.Count > 0)
                    foreach (TreeNode node in treeViewList.Nodes[0].Nodes)
                    {
                        node.Expand();
                    }
            }
        }

        private void DisableUI()
        {
            btnHideData.Enabled = false;
            btnModelBrowserHelp.Enabled = false;
            btnStopRender.Enabled = false;
            btnExport.Enabled = false;
            treeViewList.Enabled = false;
            tvfDataViewer.Enabled = false;
            dgvDataViewer.Enabled = false;
        }

        private void EnableUI()
        {
            btnHideData.Enabled = true;
            btnModelBrowserHelp.Enabled = true;
            btnStopRender.Enabled = true;
            btnExport.Enabled = true;
            treeViewList.Enabled = true;
            tvfDataViewer.Enabled = true;
            dgvDataViewer.Enabled = true;
        }

        private async void TreeViewList_AfterSelect(object sender, TreeViewEventArgs treeViewEvent)
        {
            TreeNode selectedNode = treeViewList.SelectedNode;
            NodeAsset tag = (NodeAsset)selectedNode.Tag;
            Text = "Model Browser - " + tag.Id.ToString();
            if (tag.Obj != null || tag.objData != null || tag.dynObject != null)
            {
                if (panelRender != null)
                {
                    if (render != null)
                    {
                        panelRender.StopRender();
                        render.Join();
                        panelRender.Clear();
                    }
                    renderPanel.Visible = false;
                }
                models = new Dictionary<string, GR2>();
                dgvDataViewer.DataSource = null;
                dgvDataViewer.Enabled = false;
                tvfDataViewer.Nodes.Clear();
                tvfDataViewer.Enabled = false;
                if (tag.dynObject != null && tag.dynObject is HashFileInfo info1)
                {
                    toolStripStatusLabel1.Text = "Loading GR2 File...";
                    Refresh();
                    HashFileInfo info = info1;
                    await Task.Run(() => PreviewGR2(info));
                    BuildDataViewer();
                    renderPanel.Visible = true;
                    treeViewList.Enabled = true;
                    toolStripProgressBar1.Visible = false;
                    toolStripStatusLabel1.Text = "GR2 Loaded";

                }
                else if (tag.Obj != null)
                {
                    GomObject obj = tag.Obj;
                    NpcAppearance npcData = null;
                    ItemAppearance itemData = null;

                    object visualList = null;
                    object weaponData = null;

                    try
                    {
                        switch (obj.Name.Substring(0, 3))
                        {
                            case "npp":
                                npcData = (NpcAppearance)currentDom.appearanceLoader.Load(obj.Name);
                                toolStripStatusLabel1.Text = "Loading NPP Data ...";
                                Refresh();
                                await Task.Run(() => PreviewNPC_GR2(npcData));
                                toolStripStatusLabel1.Text = "NPP Loaded";
                                break;
                            case "ipp":
                                itemData = (ItemAppearance)currentDom.appearanceLoader.Load(obj.Name);
                                toolStripStatusLabel1.Text = "Loading IPP Data ...";
                                Refresh();
                                await Task.Run(() => PreviewIPP_GR2(itemData));
                                toolStripStatusLabel1.Text = "IPP Loaded";
                                break;
                            case "itm":
                                string appearSpec = obj.Data.ValueOrDefault<string>("cbtWeaponAppearanceSpec", null);
                                weaponAppearance.TryGetValue(appearSpec.ToLower(), out weaponData);
                                weaponData = (GomObjectData)weaponData;
                                toolStripStatusLabel1.Text = "Loading ITM Data ...";
                                Refresh();
                                if (weaponData != null)
                                    await Task.Run(() => PreviewITM_GR2(obj, (GomObjectData)weaponData));
                                else
                                    MessageBox.Show("ERROR: Cannot load model! \r\nWeapon Apperance Spec Missing", "Missing Weapon Appearance Spec");
                                toolStripStatusLabel1.Text = "ITM Loaded";
                                break;
                            case "dyn":
                                try
                                {
                                    visualList = obj.Data.ValueOrDefault<List<object>>("dynVisualList", null);
                                    toolStripStatusLabel1.Text = "Loading DYN Data ...";
                                    Refresh();
                                    if (visualList != null)
                                        await Task.Run(() => PreviewDYN_GR2(obj, (List<object>)visualList));
                                    else
                                        MessageBox.Show("ERROR: Cannot load model! \r\nVisual List Missing", "Missing Visual List Spec");
                                    toolStripStatusLabel1.Text = "DYN Loaded";
                                }
                                catch (Exception excep)
                                {
                                    MessageBox.Show(excep.Message.ToString() + "\r\n" + excep.InnerException.ToString() + "\r\n" + excep.StackTrace.ToString(), "Error");
                                }
                                break;
                        }
                        BuildDataViewer();
                        renderPanel.Visible = true;
                        treeViewList.Enabled = true;
                        toolStripProgressBar1.Visible = false;
                    }
                    catch (Exception excep)
                    {
                        MessageBox.Show("Could not load NPC \r\n" + excep.ToString());
                        toolStripStatusLabel1.Text = "NPC Load Error";
                        toolStripProgressBar1.Visible = false;
                        treeViewList.Enabled = true;
                    }
                }
                else if (tag.objData != null)
                {
                    GomObjectData obj = tag.objData;
                    if (obj.Dictionary.ContainsKey("mntDataSpecString"))
                    {
                        try
                        {
                            toolStripStatusLabel1.Text = "Loading MNT Data ...";
                            Refresh();
                            await Task.Run(() => PreviewMNT_GR2(obj));
                        }
                        catch (Exception excep)
                        {
                            MessageBox.Show(excep.Message.ToString() + "\r\n" + excep.InnerException.ToString() + "\r\n" + excep.StackTrace.ToString(), "Error");
                        }
                    }

                    BuildDataViewer();
                    renderPanel.Visible = true;
                    treeViewList.Enabled = true;
                    toolStripProgressBar1.Visible = false;
                    toolStripStatusLabel1.Text = "MNT Loaded";
                }
            }
        }

        private void BuildDataViewer()
        {
            tvfDataViewer.Nodes.Clear();
            dgvDataViewer.DataSource = null;
            dataviewDict.Clear();
            dataviewDict.Add("/", new NodeAsset("/", "", "Root", null));

            if (models.Count > 0)
            {
                dataviewDict.Add("/models", new NodeAsset("/models", "/", "Models", null));

                foreach (var model in models)
                {
                    GR2 gr2 = model.Value;
                    NodeAsset asset = new NodeAsset("/models/" + model.Key, "/models", model.Key, gr2);
                    dataviewDict.Add("/models/" + model.Key, asset);

                    if (gr2.attachedModels.Count > 0)
                    {
                        dataviewDict.Add("/models/" + model.Key + "/attached", new NodeAsset("/models/" + model.Key + "/attached", "/models/" + model.Key, "Attached Models", null));

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

                    if (gr2.meshes.Count > 0)
                    {
                        if (!dataviewDict.ContainsKey("/models/" + model.Key + "/meshes"))
                            dataviewDict.Add("/models/" + model.Key + "/meshes", new NodeAsset("/models/" + model.Key + "/meshes", "/models/" + model.Key, "Meshes", null));
                        foreach (var mesh in model.Value.meshes)
                        {
                            GR2_Mesh gr2_mesh = mesh;
                            NodeAsset meshAsset = new NodeAsset("/models/" + model.Key + "/meshes/" + gr2_mesh.meshName, "/models/" + model.Key + "/meshes", gr2_mesh.meshName, gr2_mesh);
                            if (!dataviewDict.ContainsKey("/models/" + model.Key + "/meshes/" + gr2_mesh.meshName))
                                dataviewDict.Add("/models/" + model.Key + "/meshes/" + gr2_mesh.meshName, meshAsset);
                        }
                    }

                    if (gr2.materials.Count > 0)
                    {
                        if (!dataviewDict.ContainsKey("/models/" + model.Key + "/materials"))
                            dataviewDict.Add("/models/" + model.Key + "/materials", new NodeAsset("/models/" + model.Key + "/materials", "/models/" + model.Key, "Materials", null));
                        foreach (var material in model.Value.materials)
                        {
                            GR2_Material gr2_material = material;
                            NodeAsset materialAsset = new NodeAsset("/models/" + model.Key + "/materials/" + gr2_material.materialName, "/models/" + model.Key + "/materials", gr2_material.materialName, gr2_material);
                            if (!dataviewDict.ContainsKey("/models/" + model.Key + "/materials/" + gr2_material.materialName))
                                dataviewDict.Add("/models/" + model.Key + "/materials/" + gr2_material.materialName, materialAsset);
                        }
                    }

                    if (gr2.numBones > 0)
                    {
                        if (!dataviewDict.ContainsKey("/models/" + model.Key + "/bones"))
                            dataviewDict.Add("/models/" + model.Key + "/bones", new NodeAsset("/models/" + model.Key + "/bones", "/models/" + model.Key, "Bones", null));
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
                dataviewDict.Add("/resources", new NodeAsset("/resources", "/", "Resources", null));
                foreach (var resource in resources)
                {
                    NodeAsset asset = new NodeAsset("/resources/" + resource.Key, "/resources", resource.Key, null);
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

#pragma warning disable CS1998, CS4014
        private async Task PreviewGR2(HashFileInfo hashInfo)
        {
            if (InvokeRequired)
            {
                Invoke(new Action(() => PreviewGR2(hashInfo)));
            }
            else
            {
                string model = hashInfo.FileName;
                var file = hashInfo.File;

                if (file != null)
                {
                    string name = model.Split('/').Last();
                    Stream modelStream = file.OpenCopyInMemory();
                    BinaryReader br = new BinaryReader(modelStream);
                    GR2 gr2_model = new GR2(br, name)
                    {
                        transformMatrix = Matrix.Scaling(new Vector3(1.0f, 1.0f, 1.0f))
                    };

                    if (gr2_model.materials.Count == 0)
                    {
                        foreach (GR2_Mesh mesh in gr2_model.meshes)
                        {
                            if (mesh.meshName == "generated_collision")
                                continue;
                            else
                                gr2_model.numMaterials = mesh.numPieces;
                        }

                        if (gr2_model.numMaterials == 1)
                            gr2_model.materials = new List<GR2_Material>
                            {
                                new GR2_Material("all_test_grey_128")
                            };

                        if (gr2_model.numMaterials == 2)
                        {
                            gr2_model.materials = new List<GR2_Material>
                            {
                                new GR2_Material("all_test_grey_128"),
                                new GR2_Material("defaultMirror")
                            };
                        }
                    }

                    if (gr2_model.materials.Count > 0)
                    {
                        if (gr2_model.materials[0].materialName == "default")
                            gr2_model.materials[0] = new GR2_Material("all_test_grey_128");

                        // if (gr2_model.materials.Count > 1)
                        //     if (gr2_model.materials[1].materialName == "defaultMirror")
                        //         gr2_model.materials[1] = new GR2_Material("defaultMirror");
                    }

                    models.Add(model.Substring(model.LastIndexOf('/') + 1), gr2_model);
                    panelRender.LoadModel(models, resources, name, "");

                    render = new Thread(panelRender.StartRender)
                    {
                        IsBackground = true
                    };

                    render.Start();
                }
            }
        }

        private async Task PreviewNPC_GR2(NpcAppearance npcData)
        {
            if (InvokeRequired)
            {
                Invoke(new Action(() => PreviewNPC_GR2(npcData)));
            }
            else
            {
                ParseNpcData(npcData);

                // --------------------------------------------------------------------------------------
                // HACK: Until I can figure out how to parse and use /resources/art/dynamic/testrules.rul
                // --------------------------------------------------------------------------------------
                if (models.ContainsKey("appSlotHead"))
                    if (tagExclusion.Any(x => models["appSlotHead"].filename.Contains(x)))
                    {
                        // Hide Head Gear
                        if (models.ContainsKey("appSlotFace"))
                            models["appSlotFace"].enabled = false;

                        // Hide Hood
                        if (models.ContainsKey("appSlotChest"))
                            if (models["appSlotChest"].attachedModels.Count > 0)
                                foreach (var attach in models["appSlotChest"].attachedModels)
                                {
                                    if (attach.filename.Contains("hoodup"))
                                        attach.enabled = false;
                                }
                    }

                // Hides Hair with full Helmets
                if (models.ContainsKey("appSlotFace"))
                    if (helmetClosed.Any(x => models["appSlotFace"].filename.Contains(x)))
                        if (models.ContainsKey("appSlotHair"))
                            models["appSlotHair"].enabled = false;

                // Hides Hair with Hood Up
                if (models.ContainsKey("appSlotChest"))
                    if (models["appSlotChest"].attachedModels.Count > 0)
                        foreach (var attach in models["appSlotChest"].attachedModels)
                        {
                            if (attach.filename.Contains("hoodup"))
                                if (models.ContainsKey("appSlotHair"))
                                    models["appSlotHair"].enabled = false;
                        }
                // End HACK ------------------------------------------------------------------------------

                panelRender.LoadModel(models, resources, npcData.Fqn, npcData.NppType);
                render = new Thread(panelRender.StartRender)
                {
                    IsBackground = true
                };
                render.Start();
            }
        }
#pragma warning restore CS1998, CS4014

        string Bodytype = "bmn";

        readonly Dictionary<string, string> Bodytypes = new Dictionary<string, string> {
            {"bfa", "Female BT1"},
            {"bfn", "Female BT2"},
            {"bfs", "Female BT3"},
            {"bfb", "Female BT4"},
            {"bma", "Male BT1"},
            {"bmn", "Male BT2"},
            {"bms", "Male BT3"},
            {"bmf", "Male BT4"} };

        private void ChangeBodyType(object sender, EventArgs evnt)
        {
            Bodytype = ((ToolStripMenuItem)sender).Name;
        }

#pragma warning disable CS1998, CS4014
        private async Task PreviewIPP_GR2(ItemAppearance itemData)
        {
            if (InvokeRequired)
            {
                Invoke(new Action(() => PreviewIPP_GR2(itemData)));
            }
            else
            {
                string model = itemData.IPP.Model;

                if (model.Contains(".gr2"))
                {
                    // string bodyType = currentAssets.FindFile("/resources" + model.Replace("[bt]", "bfn")) != null ? "bfn" : "bmn";

                    model = model.Replace("[bt]", Bodytype);

                    var modelFile = currentAssets.FindFile("/resources" + model);

                    if (modelFile != null)
                    {
                        Stream modelStream = modelFile.OpenCopyInMemory();
                        BinaryReader br = new BinaryReader(modelStream);
                        string name = model.Split('/').Last();
                        GR2 gr2_model = new GR2(br, name);

                        string material0 = itemData.IPP.Material0;
                        string materialMirror = itemData.IPP.MaterialMirror;

                        string palette1XML = "";
                        string palette2XML = "";

                        if (material0 != null)
                        {
                            if (itemData.IPP.PrimaryHue != "")
                                palette1XML = "/resources" + itemData.IPP.PrimaryHue.Split(';').First();

                            if (itemData.IPP.SecondaryHue != "")
                                palette2XML = "/resources" + itemData.IPP.SecondaryHue.Split(';').First();

                            material0 = material0.Replace("[gen]", Bodytype.Substring(1, 1)).Replace("[bt]", Bodytype);
                            materialMirror = materialMirror.Replace("[gen]", Bodytype.Substring(1, 1)).Replace("[bt]", Bodytype);

                            if (gr2_model.numMaterials == 0)
                            {
                                gr2_model.numMaterials = 1;
                                gr2_model.materials.Add(new GR2_Material(material0));
                                if (palette1XML != null)
                                    gr2_model.materials[0].palette1XML = palette1XML;
                                if (palette2XML != null)
                                    gr2_model.materials[0].palette2XML = palette2XML;
                            }
                            else if (gr2_model.numMaterials == 1)
                            {
                                gr2_model.materials[0] = new GR2_Material(material0);
                                if (palette1XML != null)
                                    gr2_model.materials[0].palette1XML = palette1XML;
                                if (palette2XML != null)
                                    gr2_model.materials[0].palette2XML = palette2XML;
                            }
                            else if (gr2_model.numMaterials == 2)
                            {
                                gr2_model.materials[0] = new GR2_Material(material0);

                                if (palette1XML != null)
                                    gr2_model.materials[0].palette1XML = palette1XML;

                                if (palette2XML != null)
                                    gr2_model.materials[0].palette2XML = palette2XML;

                                var appSlot = model.Split('/').Last().Split('_').First();

                                if (materialMirror == "")
                                    materialMirror = appSlot + "_naked_caucasian_young_a01c01_" + Bodytype;

                                gr2_model.materials[1] = new GR2_Material(materialMirror);
                            }
                        }

                        if (itemData.IPP.AttachedModels.Count() > 0)
                        {
                            foreach (var attach in itemData.IPP.AttachedModels)
                            {
                                string attachFileName = attach;

                                attachFileName = attachFileName.Replace("[bt]", Bodytype);

                                var attachFile = currentAssets.FindFile("/resources" + attachFileName);

                                if (attachFile != null)
                                {
                                    Stream attachModelStream = attachFile.OpenCopyInMemory();
                                    BinaryReader br2 = new BinaryReader(attachModelStream);
                                    string attachName = attachFileName.Split('/').Last();
                                    GR2 attachModel = new GR2(br2, attachName);

                                    if (attachModel.numMaterials == 0)
                                    {
                                        attachModel.numMaterials = 1;
                                        attachModel.materials.Add(new GR2_Material(material0));
                                        if (palette1XML != null)
                                            attachModel.materials[0].palette1XML = palette1XML;
                                        if (palette2XML != null)
                                            attachModel.materials[0].palette2XML = palette2XML;
                                    }
                                    else if (attachModel.numMaterials == 1)
                                    {
                                        attachModel.materials[0] = new GR2_Material(material0);
                                        if (palette1XML != null)
                                            attachModel.materials[0].palette1XML = palette1XML;
                                        if (palette2XML != null)
                                            attachModel.materials[0].palette2XML = palette2XML;
                                    }
                                    else if (attachModel.numMaterials == 2)
                                    {
                                        attachModel.materials[0] = new GR2_Material(material0);

                                        if (palette1XML != null)
                                            attachModel.materials[0].palette1XML = palette1XML;

                                        if (palette2XML != null)
                                            attachModel.materials[0].palette2XML = palette2XML;

                                        var appSlot = attachFileName.Split('/').Last().Split('_').First();

                                        if (materialMirror == "")
                                            materialMirror = appSlot + "_naked_caucasian_young_a01c01_" + Bodytype;

                                        attachModel.materials[1] = new GR2_Material(materialMirror);
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
                    Stream inputStream = currentAssets.FindFile("/resources" + model).OpenCopyInMemory();
                    if (inputStream != null)
                        resources.Add(model.Substring(model.LastIndexOf('/') + 1), inputStream);
                }

                panelRender.LoadModel(models, resources, itemData.Fqn, "ipp");

                render = new Thread(panelRender.StartRender)
                {
                    IsBackground = true
                };

                render.Start();
            }
        }

        private async Task PreviewMultipleIPPs_GR2(List<ItemAppearance> itemsData)
        {
            if (InvokeRequired)
            {
                Invoke(new Action(() => PreviewMultipleIPPs_GR2(itemsData)));
            }
            else
            {
                foreach (ItemAppearance itemData in itemsData)
                {
                    string model = itemData.IPP.Model;

                    model = model.Replace("[bt]", Bodytype);

                    if (model.Contains(".gr2"))
                    {
                        var modelFile = currentAssets.FindFile("/resources" + model);

                        if (modelFile != null)
                        {
                            Stream modelStream = modelFile.OpenCopyInMemory();
                            BinaryReader br = new BinaryReader(modelStream);
                            string name = model.Split('/').Last();
                            GR2 gr2_model = new GR2(br, name);

                            string material0 = itemData.IPP.Material0;
                            string materialMirror = itemData.IPP.MaterialMirror;

                            string palette1XML = "";
                            string palette2XML = "";

                            if (material0 != null)
                            {
                                if (itemData.IPP.PrimaryHue != "")
                                    palette1XML = "/resources" + itemData.IPP.PrimaryHue.Split(';').First();

                                if (itemData.IPP.SecondaryHue != "")
                                    palette2XML = "/resources" + itemData.IPP.SecondaryHue.Split(';').First();

                                material0 = material0.Replace("[gen]", Bodytype.Substring(1, 1)).Replace("[bt]", Bodytype);
                                materialMirror = materialMirror.Replace("[gen]", Bodytype.Substring(1, 1)).Replace("[bt]", Bodytype);

                                if (gr2_model.numMaterials == 0)
                                {
                                    gr2_model.numMaterials = 1;
                                    gr2_model.materials.Add(new GR2_Material(material0));
                                    if (palette1XML != null)
                                        gr2_model.materials[0].palette1XML = palette1XML;
                                    if (palette2XML != null)
                                        gr2_model.materials[0].palette2XML = palette2XML;
                                }
                                else if (gr2_model.numMaterials == 1)
                                {
                                    gr2_model.materials[0] = new GR2_Material(material0);
                                    if (palette1XML != null)
                                        gr2_model.materials[0].palette1XML = palette1XML;
                                    if (palette2XML != null)
                                        gr2_model.materials[0].palette2XML = palette2XML;
                                }
                                else if (gr2_model.numMaterials == 2)
                                {
                                    gr2_model.materials[0] = new GR2_Material(material0);

                                    if (palette1XML != null)
                                        gr2_model.materials[0].palette1XML = palette1XML;

                                    if (palette2XML != null)
                                        gr2_model.materials[0].palette2XML = palette2XML;

                                    var appSlot = model.Split('/').Last().Split('_').First();

                                    if (materialMirror == "")
                                        materialMirror = appSlot + "_naked_caucasian_young_a01c01_" + Bodytype;

                                    gr2_model.materials[1] = new GR2_Material(materialMirror);
                                }
                            }

                            if (itemData.IPP.AttachedModels.Count() > 0)
                            {
                                foreach (var attach in itemData.IPP.AttachedModels)
                                {
                                    string attachFileName = attach;

                                    attachFileName = attachFileName.Replace("[bt]", Bodytype);

                                    var attachFile = currentAssets.FindFile("/resources" + attachFileName);

                                    if (attachFile != null)
                                    {
                                        Stream attachModelStream = attachFile.OpenCopyInMemory();
                                        BinaryReader br2 = new BinaryReader(attachModelStream);
                                        string attachName = attachFileName.Split('/').Last();
                                        GR2 attachModel = new GR2(br2, attachName);

                                        if (attachModel.numMaterials == 0)
                                        {
                                            attachModel.numMaterials = 1;
                                            attachModel.materials.Add(new GR2_Material(material0));
                                            if (palette1XML != null)
                                                attachModel.materials[0].palette1XML = palette1XML;
                                            if (palette2XML != null)
                                                attachModel.materials[0].palette2XML = palette2XML;
                                        }
                                        else if (attachModel.numMaterials == 1)
                                        {
                                            attachModel.materials[0] = new GR2_Material(material0);
                                            if (palette1XML != null)
                                                attachModel.materials[0].palette1XML = palette1XML;
                                            if (palette2XML != null)
                                                attachModel.materials[0].palette2XML = palette2XML;
                                        }
                                        else if (attachModel.numMaterials == 2)
                                        {
                                            attachModel.materials[0] = new GR2_Material(material0);

                                            if (palette1XML != null)
                                                attachModel.materials[0].palette1XML = palette1XML;

                                            if (palette2XML != null)
                                                attachModel.materials[0].palette2XML = palette2XML;

                                            var appSlot = attachFileName.Split('/').Last().Split('_').First();

                                            if (materialMirror == "")
                                                materialMirror = appSlot + "_naked_caucasian_young_a01c01_" + Bodytype;

                                            attachModel.materials[1] = new GR2_Material(materialMirror);
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
                render = new Thread(panelRender.StartRender)
                {
                    IsBackground = true
                };
                render.Start();
            }
        }

        private async Task PreviewITM_GR2(GomObject obj, GomObjectData itemData)
        {
            if (InvokeRequired)
            {
                Invoke(new Action(() => PreviewITM_GR2(obj, itemData)));
            }
            else
            {
                string model = itemData.ValueOrDefault<string>("itmModel", null).Replace('\\', '/');
                string fxspec = itemData.ValueOrDefault<string>("itmFxSpec", null);

                if (model.Contains(".gr2"))
                {
                    var modelFile = currentAssets.FindFile("/resources" + model);

                    if (modelFile != null)
                    {
                        string name = model.Split('/').Last();

                        Stream modelStream = modelFile.OpenCopyInMemory();
                        BinaryReader br = new BinaryReader(modelStream);
                        GR2 gr2_model = new GR2(br, name)
                        {
                            transformMatrix = Matrix.Scaling(new Vector3(1.0f, 1.0f, 1.0f))
                        };

                        models.Add(model.Substring(model.LastIndexOf('/') + 1), gr2_model);
                    }
                }

                ParseFXSpec(fxspec, "itm");

                panelRender.LoadModel(models, resources, obj.Name, "itm");
                render = new Thread(panelRender.StartRender)
                {
                    IsBackground = true
                };
                render.Start();
            }
        }

        private async Task PreviewDYN_GR2(GomObject obj, List<object> visualList)
        {
            if (InvokeRequired)
            {
                Invoke(new Action(() => PreviewDYN_GR2(obj, visualList)));
            }
            else
            {
                foreach (GomObjectData visualItem in visualList)
                {
                    string model = "";
                    string visualName = "";

                    Vector3 rotationVec = new Vector3();
                    Vector3 scaleVec = new Vector3(1.0f, 1.0f, 1.0f);
                    Vector3 positionVec = new Vector3();

                    foreach (KeyValuePair<string, object> item in visualItem.Dictionary)
                    {
                        if (item.Key == "dynVisualFqn")
                        {
                            if (item.Value.ToString().Contains(".gr2"))
                                model = item.Value.ToString();
                            else
                                continue;
                        }
                        else if (item.Key == "dynVisualName")
                        {
                            visualName = item.Value.ToString();
                        }
                        else if (item.Key == "dynRotation" || item.Key == "dynScale" || item.Key == "dynPosition")
                        {
                            List<Single> value = (List<Single>)item.Value;
                            if (item.Key == "dynRotation")
                            {
                                rotationVec = new Vector3(value[0], value[1], value[2]);
                            }
                            else if (item.Key == "dynScale")
                            {
                                scaleVec = new Vector3(value[0], value[1], value[2]);
                            }
                            else if (item.Key == "dynPosition")
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

                    var file = currentAssets.FindFile("/resources" + model);

                    if (file != null)
                    {
                        Stream modelStream = file.OpenCopyInMemory();
                        BinaryReader br = new BinaryReader(modelStream);

                        string name = model.Split('/').Last();

                        GR2 gr2_model = new GR2(br, name)
                        {
                            transformMatrix = Matrix.Scaling(scaleVec) *
                                                        Matrix.RotationZ((float)((rotationVec.Z * Math.PI) / 180.0)) *
                                                        Matrix.RotationX((float)((rotationVec.X * Math.PI) / 180.0)) *
                                                        Matrix.RotationY((float)((rotationVec.Y * Math.PI) / 180.0)) *
                                                        Matrix.Translation(positionVec)
                        };

                        try
                        {
                            models.Add(visualName, gr2_model);
                        }
                        catch (Exception excep)
                        {
                            Console.WriteLine(excep.StackTrace.ToString());
                        }
                    }
                }

                panelRender.LoadModel(models, resources, obj.Name, "dyn");

                render = new Thread(panelRender.StartRender)
                {
                    IsBackground = true
                };

                render.Start();
            }
        }

        private async Task PreviewMNT_GR2(GomObjectData obj)
        {
            if (InvokeRequired)
            {
                Invoke(new Action(() => PreviewMNT_GR2(obj)));
            }
            else
            {
                obj.Dictionary.TryGetValue("mntDataVFX", out object fxSpec);

                string fqn = (string)obj.Dictionary["mntDataSpecString"];

                if (fxSpec != null)
                    ParseFXSpec(fxSpec.ToString(), "mnt");

                // "4611686299207604004"
                obj.Dictionary.TryGetValue("mntDataNpc", out object npcNodeId);

                if (npcNodeId != null)
                {
                    GomObject npcNode = currentDom.GetObject((ulong)npcNodeId);
                    List<object> npcVisualList = currentDom.GetObject((ulong)npcNodeId).Data.ValueOrDefault<List<object>>("npcVisualDataList", null);

                    if (npcVisualList != null & npcVisualList.Count > 0)
                    {
                        foreach (GomObjectData visualItem in npcVisualList)
                        {
                            if (visualItem.Dictionary.ContainsKey("npcTemplateVisualDataAppearance"))
                            {
                                NpcAppearance npcData = (NpcAppearance)currentDom.appearanceLoader.Load((ulong)visualItem.Dictionary["npcTemplateVisualDataAppearance"]);

                                ParseNpcData(npcData);
                            }
                        }
                    }
                }
                else
                {
                    string skeletonModel = "/resources/art/dynamic/spec/" + Bodytype + "new_skeleton.gr2";

                    TorLib.File file = currentAssets.FindFile(skeletonModel);

                    if (file != null)
                    {
                        Stream skeletonStream = file.OpenCopyInMemory();
                        BinaryReader br = new BinaryReader(skeletonStream);

                        string name = skeletonModel.Split('/').Last();

                        GR2 gr2_model = new GR2(br, name)
                        {
                            transformMatrix = Matrix.Scaling(new Vector3(1.0f, 1.0f, 1.0f))
                        };

                        models.Add(name, gr2_model);
                    }
                }

                if (models.Count() > 0)
                {
                    panelRender.LoadModel(models, resources, fqn, "mnt");

                    render = new Thread(panelRender.StartRender)
                    {
                        IsBackground = true
                    };

                    render.Start();
                }
                else
                {
                    MessageBox.Show("No models were found", "Error Loading Models");
                }
            }
        }
#pragma warning restore CS1998, CS4014

        private void ParseNpcData(NpcAppearance npcData)
        {
            // Load NPC Skeleton
            if (npcData.BodyType != null)
            {
                string skeletonModel;

                if (npcData.BodyType.StartsWith("bf") || npcData.BodyType.StartsWith("bm"))
                    skeletonModel = "/resources/art/dynamic/spec/" + npcData.BodyType + "new_skeleton.gr2";
                else
                    skeletonModel = "/resources/art/dynamic/spec/" + npcData.BodyType + "_skeleton.gr2";

                TorLib.File file = currentAssets.FindFile(skeletonModel);

                if (file != null)
                {
                    Stream skeletonStream = file.OpenCopyInMemory();
                    BinaryReader br = new BinaryReader(skeletonStream);
                    string name = skeletonModel.Split('/').Last();
                    GR2 gr2_model = new GR2(br, name);

                    models.Add(name, gr2_model);
                }
            }

            // Load NPC Slots
            foreach (var appSlot in npcData.AppearanceSlotMap)
            {
                if (appSlot.Value.Count == 1)
                {
                    string Bodytype;
                    Bodytype = appSlot.Value[0].BodyType;

                    string model;
                    model = appSlot.Value[0].Model.Replace("[bt]", Bodytype);

                    // Load Model & Materials for this Slot
                    if (model.Contains(".gr2"))
                    {
                        var modelFile = currentAssets.FindFile("/resources" + model);

                        if (modelFile != null)
                        {
                            Stream modelStream = modelFile.OpenCopyInMemory();
                            BinaryReader br = new BinaryReader(modelStream);
                            string name = model.Split('/').Last();
                            GR2 gr2_model = new GR2(br, name);

                            string material0 = appSlot.Value[0].Material0.Replace("[bt]", Bodytype);
                            string materialMirror = appSlot.Value[0].MaterialMirror.Replace("[bt]", Bodytype);

                            string palette1XML = "";
                            string palette2XML = "";

                            gr2_model.materials = new List<GR2_Material> { };

                            // Naked Skin Material Substitution
                            if (npcData.AppearanceSlotMap.ContainsKey("appSlotHead"))
                            {
                                var appSlotHead = npcData.AppearanceSlotMap["appSlotHead"];

                                if (material0.Contains("_naked_"))
                                {
                                    if (appSlotHead[0].AMI.ChildSkinMaterials != null)
                                        material0 = appSlotHead[0].AMI.ChildSkinMaterials[appSlot.Key].Replace("[bt]", Bodytype);
                                }

                                if (gr2_model.numMaterials > 1 && materialMirror == "")
                                {
                                    if (appSlotHead[0].AMI.ChildSkinMaterials != null)
                                        materialMirror = appSlotHead[0].AMI.ChildSkinMaterials[appSlot.Key].Replace("[bt]", Bodytype);
                                }
                            }

                            // default Material
                            if (material0 != null)
                            {
                                if (appSlot.Value[0].PrimaryHue != "")
                                    palette1XML = "/resources" + appSlot.Value[0].PrimaryHue.Split(';').First();

                                if (appSlot.Value[0].SecondaryHue != "")
                                    palette2XML = "/resources" + appSlot.Value[0].SecondaryHue.Split(';').First();

                                if (Bodytype.Contains("bf"))
                                    material0 = material0.Replace("[gen]", "f");

                                if (Bodytype.Contains("bm"))
                                    material0 = material0.Replace("[gen]", "m");

                                gr2_model.materials.Add(new GR2_Material(material0));

                                if (palette1XML != null)
                                    gr2_model.materials[0].palette1XML = palette1XML;

                                if (palette2XML != null)
                                    gr2_model.materials[0].palette2XML = palette2XML;
                            }

                            // defaultMirror Material
                            if (materialMirror != "")
                            {
                                if (Bodytype.Contains("bf"))
                                    materialMirror = materialMirror.Replace("[gen]", "f");

                                if (Bodytype.Contains("bm"))
                                    materialMirror = materialMirror.Replace("[gen]", "m");

                                gr2_model.materials.Add(new GR2_Material(materialMirror));
                            }

                            // Attachments
                            if (appSlot.Value[0].AttachedModels.Count() > 0)
                            {
                                foreach (var attach in appSlot.Value[0].AttachedModels)
                                {
                                    string attachFile = attach.Replace("[bt]", Bodytype);
                                    var file = this.currentAssets.FindFile("/resources" + attachFile);

                                    if (file != null)
                                    {
                                        Stream attachStream = file.OpenCopyInMemory();
                                        BinaryReader br2 = new BinaryReader(attachStream);

                                        string attachName = attachFile.Split('/').Last();
                                        GR2 attachModel = new GR2(br2, attachName)
                                        {
                                            materials = gr2_model.materials
                                        };

                                        if (attachModel.numMaterials == 0)
                                        {
                                            attachModel.numMaterials = 1;
                                            attachModel.materials.Add(new GR2_Material(material0));

                                            if (palette1XML != null)
                                                attachModel.materials[0].palette1XML = palette1XML;

                                            if (palette2XML != null)
                                                attachModel.materials[0].palette1XML = palette1XML;
                                        }
                                        else if (attachModel.numMaterials == 1)
                                        {
                                            attachModel.materials[0] = new GR2_Material(material0);
                                            if (palette1XML != null)
                                                attachModel.materials[0].palette1XML = palette1XML;
                                            if (palette2XML != null)
                                                attachModel.materials[0].palette2XML = palette2XML;
                                        }
                                        else if (attachModel.numMaterials == 2)
                                        {
                                            attachModel.materials.Add(new GR2_Material(material0));
                                            attachModel.materials.Add(new GR2_Material(materialMirror));

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
                            models.Add(appSlot.Key.ToString(), gr2_model);
                        }
                    }

                    if (model.Contains(".dds"))
                        resources.Add(appSlot.Key.ToString(), appSlot.Value.First().Model);

                    if (model.Contains(".xml"))
                    {
                        string dynFqn = model.Replace("/art/", "").Replace(".xml", "").Replace("/", ".");
                        GomObject dynObj = currentDom.GetObject(dynFqn);
                        if (dynObj != null)
                            resources.Add(appSlot.Key, dynObj);
                    }
                }
                else
                {
                    break;
                }
            }
        }

        private void ParseFXSpec(string fxspec, string type = "")
        {
            if (fxspec != null)
            {
                // Standardise filepath formatting
                fxspec = fxspec.StartsWith("/") ? fxspec.Substring(1) : fxspec;
                fxspec += fxspec.Contains(".fxspec") ? "" : ".fxspec";

                // Define full filepath
                var fxFile = currentAssets.FindFile("/resources/art/fx/fxspec/" + fxspec);

                // If filepath is valid, proceed
                if (fxFile != null)
                {
                    // Load FxSpec as XML document
                    XmlDocument xmlDoc = new XmlDocument();
                    xmlDoc.Load(fxFile.OpenCopyInMemory());

                    // Load the emitter list from the FxSpec
                    XmlNode emitterList = xmlDoc.SelectSingleNode("/nodeWClasses/marshalData/node/f[@name='_fxEmitterList']");

                    // Load the model list from the FxSpec
                    XmlNode modelList = xmlDoc.SelectSingleNode("/nodeWClasses/marshalData/node/f[@name='_fxModelList']");

                    // Relative transforms
                    Vector3 relPosVec = new Vector3();
                    Vector3 relRotVec = new Vector3();

                    // Check the model list isn't empty
                    if (modelList.ChildNodes.Count > 0)
                    {
                        foreach (XmlNode modelNode in modelList.ChildNodes)
                        {
                            // Ignore models that never start
                            if (modelNode.SelectSingleNode("./node()[@name='_fxWhenToStart']").InnerText == "NEVER")
                                continue;

                            // Get the resource name
                            string resourceName = modelNode.SelectSingleNode("./node()[@name='_fxResourceName']").InnerText;
                            XmlNode resourceFxName = modelNode.SelectSingleNode("./node()[@name='_fxName']");

                            // Transform vectors
                            Vector3 positionVec = new Vector3();
                            Vector3 rotationVec = new Vector3();
                            Vector3 scaleVec = new Vector3();

                            // Transform vector nodes
                            XmlNode positionVecNode = modelNode.SelectSingleNode("./node()[@name='_fxAttachPosition']");
                            XmlNode rotationVecNode = modelNode.SelectSingleNode("./node()[@name='_fxAttachRotation']");
                            XmlNode scaleVecNode = modelNode.SelectSingleNode("./node()[@name='_fxScale']");

                            // Attach nodes
                            XmlNode attachToNode = modelNode.SelectSingleNode("./node()[@name='_fxAttachTo']");
                            XmlNode attachRelativeNode = modelNode.SelectSingleNode("./node()[@name='_fxAttachRelative']");
                            XmlNode boneAttachNode = modelNode.SelectSingleNode("./node()[@name='_fxAttachBone']");

                            // Parent nodes
                            XmlNode parentAttachToNode = null;
                            XmlNode parentBoneAttachNode = null;
                            XmlNode parentPositionVecNode = null;
                            XmlNode parentRotationVecNode = null;

                            // Ignore certain models
                            if (resourceName.Contains("vfx") || resourceName.Contains("spawn") ||
                                resourceName.Contains("fx_all_lasersight_flare") || resourceName.Contains("_distortion") ||
                                resourceName.Contains("bh_jetpack"))
                                continue;

                            // Hide creature handles and weapon crystals
                            if (resourceFxName.InnerText.Contains("handle") || resourceFxName.InnerText.Contains("m_crystal"))
                                continue;

                            // Check the resource name is valid
                            if (resourceName.Contains(".gr2"))
                            {
                                // Standardise model filepath
                                string modelPath = "/resources" + resourceName.Replace("\\", "/");

                                // Find the model file in the game assets
                                var attachModel = currentAssets.FindFile(modelPath);

                                // Check the model file was successfully found
                                if (attachModel != null)
                                {
                                    // Open the mode file
                                    string name = modelPath.Split('/').Last();
                                    Stream modelStream = attachModel.OpenCopyInMemory();
                                    BinaryReader br = new BinaryReader(modelStream);
                                    GR2 gr2_model = new GR2(br, name);

                                    // Parse the emitter node chain
                                    XmlNode emitterNode = emitterList.SelectSingleNode(".//node()[@name='_fxName' and text() = '" + attachToNode.InnerText + "']");
                                    if (emitterNode != null)
                                    {
                                        parentAttachToNode = emitterNode.ParentNode.SelectSingleNode("./node()[@name='_fxAttachTo']");
                                        parentBoneAttachNode = emitterNode.ParentNode.SelectSingleNode("./node()[@name='_fxAttachBone']");
                                        parentPositionVecNode = emitterNode.ParentNode.SelectSingleNode("./node()[@name='_fxAttachPosition']");
                                        parentRotationVecNode = emitterNode.ParentNode.SelectSingleNode("./node()[@name='_fxAttachRotation']");

                                        // Position Transform
                                        if (parentPositionVecNode != null)
                                        {
                                            string[] temp = parentPositionVecNode.InnerText.ToString().Replace("(", "").Replace(")", "").Split(',');
                                            positionVec += new Vector3(float.Parse(temp[0]), float.Parse(temp[1]), float.Parse(temp[2]));
                                        }

                                        // Rotation Transform
                                        if (parentRotationVecNode != null)
                                        {
                                            string[] temp = parentRotationVecNode.InnerText.ToString().Replace("(", "").Replace(")", "").Split(',');
                                            rotationVec += new Vector3(float.Parse(temp[0]), float.Parse(temp[1]), float.Parse(temp[2]));
                                        }

                                        ParseFXSpecEmitters(emitterNode, emitterList, ref positionVec, ref rotationVec, type);
                                    }

                                    // Check if attachments should be relative
                                    if (attachRelativeNode.InnerText == "true" && resourceFxName.InnerText == "speeder")
                                    {
                                        string[] pos = modelNode.SelectSingleNode("./node()[@name='_fxStartLocOffset']").InnerText.ToString().Replace("(", "").Replace(")", "").Split(',');
                                        relPosVec = new Vector3(float.Parse(pos[0]), float.Parse(pos[1]), float.Parse(pos[2]));

                                        string[] rot = modelNode.SelectSingleNode("./node()[@name='_fxRotation']").InnerText.ToString().Replace("(", "").Replace(")", "").Split(',');
                                        relRotVec = new Vector3(float.Parse(rot[0]), float.Parse(rot[1]), float.Parse(rot[2]));
                                    }

                                    // Position Transform 
                                    if (positionVecNode != null)
                                    {
                                        string[] temp = positionVecNode.InnerText.ToString().Replace("(", "").Replace(")", "").Split(',');
                                        positionVec += new Vector3(float.Parse(temp[0]), float.Parse(temp[1]), float.Parse(temp[2]));

                                        if (relPosVec != new Vector3())
                                            positionVec += relPosVec;
                                    }

                                    // Rotation Transform
                                    if (rotationVecNode != null)
                                    {
                                        string[] temp = rotationVecNode.InnerText.ToString().Replace("(", "").Replace(")", "").Split(',');
                                        rotationVec += new Vector3(float.Parse(temp[0]), float.Parse(temp[1]), float.Parse(temp[2]));

                                        if (relRotVec != new Vector3())
                                            rotationVec += relRotVec;
                                    }

                                    // Scale Transform
                                    if (scaleVecNode != null)
                                    {
                                        string[] temp = scaleVecNode.InnerText.ToString().Replace("(", "").Replace(")", "").Split(',');
                                        scaleVec = new Vector3(float.Parse(temp[0]), float.Parse(temp[1]), float.Parse(temp[2]));
                                    }

                                    // Check if model is attached to a valid attachment point or bone
                                    if (parentBoneAttachNode != null && parentBoneAttachNode.InnerText != "")
                                    {
                                        foreach (KeyValuePair<string, GR2> model in models)
                                        {
                                            GR2_Attachment attach = model.Value.attachments.SingleOrDefault(x => x.attachName.ToLower() == parentBoneAttachNode.InnerText.ToLower());
                                            if (attach != null)
                                                gr2_model.attachMatrix = attach.attach_matrix;

                                            GR2_Bone_Skeleton boneAttach = model.Value.skeleton_bones.SingleOrDefault(x => x.boneName.ToLower() == parentBoneAttachNode.InnerText.ToLower());
                                            if (boneAttach != null)
                                                gr2_model.attachMatrix = boneAttach.root;
                                        }
                                    }
                                    else if (boneAttachNode != null && boneAttachNode.InnerText != "")
                                    {
                                        foreach (KeyValuePair<string, GR2> model in models)
                                        {
                                            GR2_Attachment attach = model.Value.attachments.SingleOrDefault(x => x.attachName.ToLower() == boneAttachNode.InnerText.ToLower());
                                            if (attach != null)
                                                gr2_model.attachMatrix = attach.attach_matrix;

                                            GR2_Bone_Skeleton boneAttach = model.Value.skeleton_bones.SingleOrDefault(x => x.boneName.ToLower() == boneAttachNode.InnerText.ToLower());
                                            if (boneAttach != null)
                                                gr2_model.attachMatrix = boneAttach.root;
                                        }
                                    }

                                    // Axis adjustments
                                    if (type == "itm")
                                    {
                                        string itemName = fxFile.FilePath.Split('/').Last().Split('.').First();

                                        if (itemName.Contains("assaultcannon_"))
                                        {
                                            positionVec = new Vector3(positionVec.Z, positionVec.Y, positionVec.X);
                                            rotationVec = new Vector3(rotationVec.X, rotationVec.Z, rotationVec.Y);
                                            scaleVec = new Vector3(scaleVec.X, scaleVec.Z, scaleVec.Y);
                                        }
                                        else if (itemName.Contains("blaster_") || itemName.Contains("rifle_"))
                                        {
                                            positionVec = new Vector3(positionVec.X, positionVec.Z, positionVec.Y);
                                            rotationVec = new Vector3(rotationVec.X, rotationVec.Z, rotationVec.Y);
                                            scaleVec = new Vector3(scaleVec.X, scaleVec.Z, scaleVec.Y);
                                        }
                                        else
                                        {
                                            positionVec = new Vector3(positionVec.X, positionVec.Y, positionVec.Z);
                                            rotationVec = new Vector3(rotationVec.X, rotationVec.Z, rotationVec.Y);
                                            scaleVec = new Vector3(scaleVec.X, scaleVec.Z, scaleVec.Y);
                                        }
                                    }

                                    // Scale matrix from vector
                                    if (scaleVec != new Vector3(0.0f, 0.0f, 0.0f))
                                        gr2_model.scaleMatrix = Matrix.Scaling(scaleVec);

                                    // Transform matrix from vectors
                                    gr2_model.transformMatrix = Matrix.Scaling(scaleVec) *
                                                    Matrix.RotationZ((float)(rotationVec.Z * Math.PI / 180.0)) *
                                                    Matrix.RotationX((float)(rotationVec.X * Math.PI / 180.0)) *
                                                    Matrix.RotationY((float)(rotationVec.Y * Math.PI / 180.0)) *
                                                    Matrix.Translation(positionVec);

                                    // Add model to models list
                                    models.Add(resourceFxName.InnerText, gr2_model);
                                }
                            }
                        }
                    }
                }
            }
        }

        private void ParseFXSpecEmitters(XmlNode emitterNode, XmlNode emitterList, ref Vector3 positionVec, ref Vector3 rotationVec, string type = "")
        {
            XmlNode checkMe = emitterNode.ParentNode;
            if (checkMe.SelectSingleNode("./node()[@name='_fxAttachBone']").InnerText == "" &&
                checkMe.SelectSingleNode("./node()[@name='_fxAttachTo']").InnerText != "CASTER" &&
                checkMe.SelectSingleNode("./node()[@name='_fxAttachTo']").InnerText != "TARGET")
            {
                XmlNode attachToNode = emitterNode.ParentNode.SelectSingleNode("./node()[@name='_fxAttachTo']");
                emitterNode = emitterList.SelectSingleNode(".//node()[@name='_fxName' and text() = '" + attachToNode.InnerText + "']");

                XmlNode parentAttachToNode;
                XmlNode parentPositionVecNode;
                XmlNode parentRotationVecNode;

                if (emitterNode != null)
                {
                    parentAttachToNode = emitterNode.ParentNode.SelectSingleNode("./node()[@name='_fxAttachTo']");
                    parentPositionVecNode = emitterNode.ParentNode.SelectSingleNode("./node()[@name='_fxAttachPosition']");
                    parentRotationVecNode = emitterNode.ParentNode.SelectSingleNode("./node()[@name='_fxAttachRotation']");

                    // if (parentAttachToNode.InnerText != "CASTER" && parentAttachToNode.InnerText != "TARGET")
                    if (type == "itm" || parentAttachToNode.InnerText != "CASTER" && parentAttachToNode.InnerText != "TARGET")
                    {
                        // Position Transform
                        if (parentPositionVecNode != null)
                        {
                            string[] temp = parentPositionVecNode.InnerText.ToString().Replace("(", "").Replace(")", "").Split(',');
                            positionVec += new Vector3(float.Parse(temp[0]), float.Parse(temp[1]), float.Parse(temp[2]));
                        }

                        // Rotation Transform
                        if (parentRotationVecNode != null)
                        {
                            string[] temp = parentRotationVecNode.InnerText.ToString().Replace("(", "").Replace(")", "").Split(',');
                            rotationVec += new Vector3(float.Parse(temp[0]), float.Parse(temp[1]), float.Parse(temp[2]));
                        }
                    }

                    // Recursive
                    ParseFXSpecEmitters(emitterNode, emitterList, ref positionVec, ref rotationVec, type);
                }
            }
        }

        private void RenderPanel_MouseHover(object sender, EventArgs evnt)
        {
            if (!_closing)
                renderPanel.Focus();
        }

        private void ModelBrowser_FormClosing(object sender, FormClosingEventArgs evnt)
        {
            if (render != null)
            {
                panelRender.StopRender();
                render.Join();
                panelRender.Clear();
            }
            panelRender.Dispose();
            assetDict.Clear();
            nodeKeys.Clear();
            models.Clear();
            resources.Clear();

            currentAssets = null;
            currentDom = null;

            previousAssets = null;
            previousDom = null;


            panelRender = null;

            treeViewList.Dispose();
            treeViewList = null;

            this.Dispose();
        }

        private void BtnExport_Click(object sender, EventArgs evnt)
        {
            panelRender.ExportGeometry(Bodytype);
        }

        private void TreeViewList_MouseUp(object sender, MouseEventArgs evnt)
        {
            if (evnt.Button == MouseButtons.Right)
            {
                treeViewList.SelectedNode = treeViewList.GetNodeAt(evnt.X, evnt.Y);

                if (treeViewList.SelectedNode != null && treeViewList.SelectedNode.Nodes.Count > 0)
                {
                    if (treeViewList.SelectedNode.Name.Contains("ipp."))
                    {
                        contextMenuStrip1.Show(treeViewList, evnt.Location);
                    }
                    else if (treeViewList.SelectedNode.Name == "ipp")
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
                        BodyTypeStrip.Show(treeViewList, evnt.Location);
                    }
                }
            }
        }

        private async void ToolStripMenuItemViewAll_Click(object sender, EventArgs e)
        {
            items.Clear();
            treeViewList.Enabled = false;
            tvfDataViewer.Nodes.Clear();
            tvfDataViewer.Enabled = false;
            toolStripProgressBar1.Visible = true;
            toolStripStatusLabel1.Text = "Loading IPP Data ...";
            Refresh();
            TreeNode selectedNode = treeViewList.SelectedNode;

            foreach (TreeNode node in selectedNode.Nodes)
            {
                NodeAsset tag = (NodeAsset)node.Tag;
                if (tag.Obj != null)
                {
                    GomObject obj = tag.Obj;
                    ItemAppearance itemData = (ItemAppearance)currentDom.appearanceLoader.Load(obj.Name);
                    items.Add(itemData);
                }
            }

            if (panelRender != null)
            {
                renderPanel.Visible = false;
                if (render != null)
                {
                    panelRender.StopRender();
                    render.Join();
                    panelRender.Clear();
                }
            }

            await Task.Run(() => PreviewMultipleIPPs_GR2(items));
            BuildDataViewer();
            renderPanel.Visible = true;
            treeViewList.Enabled = true;
            toolStripProgressBar1.Visible = false;
            toolStripStatusLabel1.Text = "IPP Set Loaded";
        }

        private void BtnModelBrowserHelp_Click(object sender, EventArgs e)
        {
            ModelBrowserHelp formHelp = new ModelBrowserHelp();
            formHelp.Show();
        }

        public void SetStatusLabel(string message)
        {
            if (statusStrip1.InvokeRequired)
            {
                statusStrip1.Invoke(new Action(() => SetStatusLabel(message)));
            }
            else
            {
                toolStripStatusLabel1.Text = message;
            }
        }

        private void BtnStopRender_Click(object sender, EventArgs e)
        {
            if (panelRender != null)
            {
                renderPanel.Visible = false;
                if (render != null)
                {
                    panelRender.StopRender();
                    render.Join();
                    panelRender.Clear();
                }
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

        private void BtnHideData_Click(object sender, EventArgs e)
        {
            bool current = splitContainerCenter.Panel2Collapsed;
            if (current)
            {
                splitContainerCenter.Panel2Collapsed = false;
                btnHideData.Text = "Hide Data Viewer";
            }
            else
            {
                splitContainerCenter.Panel2Collapsed = true;
                btnHideData.Text = "Show Data Viewer";
            }
        }

        private void TvfDataViewer_AfterSelect(object sender, TreeViewEventArgs e)
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
                if (tag.dynObject is GR2 model)
                {
                    dt.Rows.Add(new string[] { "Render", model.enabled.NullSafeToString() });
                    dt.Rows.Add(new string[] { "# Attachments", model.numAttach.NullSafeToString() });
                    dt.Rows.Add(new string[] { "# Bones", model.numBones.NullSafeToString() });
                    dt.Rows.Add(new string[] { "# Meshes", model.numMeshes.NullSafeToString() });
                    dt.Rows.Add(new string[] { "# Materials", model.numMaterials.NullSafeToString() });
                }
                else if (tag.dynObject is GR2_Material material)
                {
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
                else if (tag.dynObject is GR2_Mesh mesh)
                {
                    dt.Rows.Add(new string[] { "# Bones", mesh.numBones.NullSafeToString() });
                    dt.Rows.Add(new string[] { "# Pieces", mesh.numPieces.NullSafeToString() });
                    dt.Rows.Add(new string[] { "# Vertices", mesh.numVerts.NullSafeToString() });
                    dt.Rows.Add(new string[] { "Bones", string.Join(", ", mesh.meshBones).NullSafeToString() });
                }
                else if (tag.dynObject is GR2_Bone_Skeleton bone)
                {
                    dt.Rows.Add(new string[] { "Bone Name", bone.boneName.NullSafeToString() });
                    dt.Rows.Add(new string[] { "Bone Index", bone.boneIndex.NullSafeToString() });
                    dt.Rows.Add(new string[] { "Bone Parent Index", bone.parentBoneIndex.NullSafeToString() });
                }
                else
                {

                }
            }
        }

        private void TvfDataViewer_MouseUp(object sender, MouseEventArgs evnt)
        {
            if (evnt.Button == MouseButtons.Right)
            {
                tvfDataViewer.SelectedNode = tvfDataViewer.GetNodeAt(evnt.X, evnt.Y);

                if (tvfDataViewer.SelectedNode != null && tvfDataViewer.SelectedNode.Tag is NodeAsset asset1)
                {
                    NodeAsset asset = asset1;
                    if (asset.dynObject != null && asset.dynObject is GR2)
                        contextMenuStrip2.Show(tvfDataViewer, evnt.Location);
                    else if (asset.dynObject is GR2_Material)
                        contextMenuStrip4.Show(tvfDataViewer, evnt.Location);
                }
            }
        }

        private void ToolStripMenuItem1_Click(object sender, EventArgs evnt)
        {
            NodeAsset asset = (NodeAsset)tvfDataViewer.SelectedNode.Tag;
            if (asset.dynObject != null && asset.dynObject is GR2 gR)
            {
                GR2 model = gR;
                model.enabled = !model.enabled;
            }
        }

        private void ToolStripMenuItem2_Click(object sender, EventArgs evnt)
        {
            NodeAsset asset = (NodeAsset)tvfDataViewer.SelectedNode.Tag;
            if (asset.dynObject != null && asset.dynObject is GR2_Material material1)
            {
                GR2_Material material = material1;
                ModelBrowserViewMaterial matView = new ModelBrowserViewMaterial(material);
                matView.Show();
            }
        }
    }
}
