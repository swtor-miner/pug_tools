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
    public partial class WorldBrowser : Form
    {
        public TorLib.Assets currentAssets;
        private DataObjectModel currentDom;
                
        private Dictionary<string, NodeAsset> assetDict = new Dictionary<string, NodeAsset>();
        private List<string> nodeKeys = new List<string>();

        private Dictionary<string, NodeAsset> dataviewDict = new Dictionary<string, NodeAsset>();

        private View_Area panelRender;
        public bool _closing = false;
        private Thread render;

        public Dictionary<string, GR2> models = new Dictionary<string, GR2>();
        public Dictionary<string, Stream> resources = new Dictionary<string, Stream>();
        public List<ItemAppearance> items = new List<ItemAppearance>();

        Dictionary<object, object> weaponAppearance = new Dictionary<object, object>();

        Vector3 blankVec = new Vector3();

        public WorldBrowser(string assetLocation, bool usePTS)
        {
            InitializeComponent();

            List<object> args = new List<object>();
            args.Add(assetLocation);
            args.Add(usePTS);
            toolStripProgressBar1.Visible = true;
            toolStripStatusLabel1.Text = "Loading Assets";
            disableUI();
            backgroundWorker1.RunWorkerAsync(args);            
        }

        private void backgroundWorker1_DoWork(object sender, DoWorkEventArgs e)
        {
            List<object> args = e.Argument as List<object>;
            this.currentAssets = AssetHandler.Instance.getCurrentAssets((string)args[0], (bool)args[1]);            
            this.currentDom = DomHandler.Instance.getCurrentDOM(currentAssets);
            //this.currentDom.ami.Load();
        }

        private void backgroundWorker1_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            List<GomObject> itmList = currentDom.GetObjectsStartingWith("npp.")
                    .Union(currentDom.GetObjectsStartingWith("ipp."))
                    .Union(currentDom.GetObjectsStartingWith("itm."))
                    .Union(currentDom.GetObjectsStartingWith("dyn.housing"))
                    .Union(currentDom.GetObjectsStartingWith("dyn.stronghold"))
                    .ToList();
            weaponAppearance = currentDom.GetObject("itmAppearanceDatatable").Data.Get<Dictionary<object, object>>("itmAppearances");
            Dictionary<object, object> tempAppear = new Dictionary<object, object>();
            foreach (var appear in weaponAppearance)
            {
                tempAppear.Add(appear.Key.ToString().ToLower(), appear.Value);
            }
            weaponAppearance = new Dictionary<object, object>(tempAppear);
            tempAppear.Clear();
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
                //nodeKeys.Add(item.Key.ToString());
                //GomLib.GomObject obj = (GomLib.GomObject)currentNode.Value;
                string parent = "";
                //string display = currentNode.Key.ToString();
                string display = item.Name;
                if (item.Name.Contains("."))
                {
                    string[] temp = item.Name.Split('.');
                    parent = String.Join(".", temp.Take(temp.Length - 1));
                    display = display.Replace(parent, "").Replace(".", "");
                    nodeDirs.Add(parent);
                }
                else
                    parent = "/nodes";
                NodeAsset asset = new NodeAsset(item.Name.ToString(), parent, display, currentDom);
                assetDict.Add(item.Name.ToString(), asset);
                item.Unload(); //make sure these aren't hanging around
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

                            NodeAsset assetAll = new NodeAsset(prefixAll + hashInfo.Directory + "/" + hashInfo.Extension + "/" + hashInfo.FileName + "." + hashInfo.Extension, prefixAll + hashInfo.Directory + "/" + hashInfo.Extension, hashInfo.FileName + "." + hashInfo.Extension, hashInfo);
                            assetDict.Add(prefixAll + hashInfo.Directory + "/" + hashInfo.Extension + "/" + hashInfo.FileName + "." + hashInfo.Extension, assetAll);
                            fileDirs.Add(prefixAll + hashInfo.Directory + "/" + hashInfo.Extension);                            

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
            
            toolStripStatusLabel1.Text = "Loading Tree View Items ...";
            this.Refresh();            

            Func<NodeAsset, string> getId = (x => x.Id);
            Func<NodeAsset, string> getParentId = (x => x.parentId);
            Func<NodeAsset, string> getDisplayName = (x => x.displayName);
            treeViewFast1.SuspendLayout();
            treeViewFast1.BeginUpdate();
            treeViewFast1.LoadItems<NodeAsset>(assetDict, getId, getParentId, getDisplayName);            
            //treeViewFast1.Sort();
            treeViewFast1.EndUpdate();
            treeViewFast1.ResumeLayout();
            toolStripStatusLabel1.Text = "Loading Complete";
            toolStripProgressBar1.Visible = false;
            enableUI();
            pictureBox1.Visible = false;
            treeViewFast1.Visible = true;
            panelRender = new View_Area(this.Handle, this, "renderPanel");
            panelRender.Init();
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
            if (tag.obj != null)
            {
                GomObject obj = (GomObject)tag.obj;

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

                NpcAppearance npcData = null;
                ItemAppearance itemData = null;
                object weaponData = null;
                object visualList = null;
                this.models.Clear();

                switch (obj.Name.Substring(0, 3))
                {
                    case "npp":
                        npcData = (NpcAppearance)this.currentDom.appearanceLoader.Load(obj.Name);
                        break;
                    case "ipp":
                        itemData = (ItemAppearance)this.currentDom.appearanceLoader.Load(obj.Name);
                        break;
                    case "itm":
                        string appearSpec = obj.Data.ValueOrDefault<string>("cbtWeaponAppearanceSpec", null);
                        weaponAppearance.TryGetValue(appearSpec.ToLower(), out weaponData);
                        weaponData = (GomObjectData)weaponData;
                        break;
                    case "dyn":                        
                        try
                        {
                            visualList = obj.Data.ValueOrDefault<List<object>>("dynVisualList", null);
                        }
                        catch (Exception ee)
                        {
                            Console.WriteLine(ee.Message.ToString() + "\r\n" + ee.InnerException.ToString() + "\r\n" + ee.StackTrace.ToString());                            
                        }                        
                        break;
                }

                treeViewFast1.Enabled = false;
                toolStripProgressBar1.Visible = true;                
                this.Refresh();
               
                try
                {
                    switch (obj.Name.Substring(0, 3))
                    {
                        case "npp":
                            this.toolStripStatusLabel1.Text = "Loading NPP Data ...";
                            this.Refresh();
                            await Task.Run(() => previewNPC_GR2(npcData));
                            break;
                        case "ipp":
                            this.toolStripStatusLabel1.Text = "Loading IPP Data ...";
                            this.Refresh();
                            await Task.Run(() => previewIPP_GR2(itemData));
                            break;
                        case "itm":
                            this.toolStripStatusLabel1.Text = "Loading ITM Data ...";
                            this.Refresh();
                            if (weaponData != null)
                                await Task.Run(() => previewITM_GR2((GomObject)obj, (GomObjectData)weaponData));
                            else
                                MessageBox.Show("ERROR: Cannot load model! \r\nWeapon Apperance Spec Missing", "Missing Weapon Appearance Spec");
                            break;
                        case "dyn":
                            this.toolStripStatusLabel1.Text = "Loading DYN Data ...";
                            this.Refresh();
                            if (visualList != null)
                                await Task.Run(() => previewDYN_GR2((GomObject)obj, (List<object>)visualList));
                            else
                                MessageBox.Show("ERROR: Cannot load model! \r\nWeapon Apperance Spec Missing", "Missing Weapon Appearance Spec");
                            break;
                    }                    
					buildDataViewer();
                    renderPanel.Visible = true;
                    treeViewFast1.Enabled = true;
                    toolStripProgressBar1.Visible = false;
                    this.toolStripStatusLabel1.Text = "NPC Loaded";
                }catch(Exception excep) {
                    MessageBox.Show("Could not load NPC \r\n" + excep.ToString());
                    this.toolStripStatusLabel1.Text = "NPC Load Error";
                    toolStripProgressBar1.Visible = false;
                    treeViewFast1.Enabled = true;
                }
            }
            else if (tag.dynObject != null)
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
                treeViewFast1.Enabled = false;
                toolStripProgressBar1.Visible = true;
                this.Refresh();

                if (tag.dynObject is HashFileInfo)
                {
                    await Task.Run(() => previewGR2((HashFileInfo)tag.dynObject));
                }
                buildDataViewer();
                renderPanel.Visible = true;
                treeViewFast1.Enabled = true;
                toolStripProgressBar1.Visible = false;
                this.toolStripStatusLabel1.Text = "GR2 Loaded";
            }
        }

		private void buildDataViewer()
        {
            tvfDataViewer.Nodes.Clear();
            dgvDataViewer.DataSource = null;
            if (models.Count > 0)
            {
                dataviewDict.Clear();
                dataviewDict.Add("/", new NodeAsset("/", "", "Root", (GomObject)null));
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
        }


        private async Task previewNPC_GR2(NpcAppearance npcData)
        {
            if (InvokeRequired)
            {
                Invoke(new Action(() => previewNPC_GR2(npcData)));
            }
            else
            {              
                string model;
                foreach (var slotDict in npcData.AppearanceSlotMap)
                {
                    if (slotDict.Value.Count() > 1)
                    {
                        //Console.WriteLine("pause");
                    }
                    else if (slotDict.Value.Count() == 1)
                    {
                        model = slotDict.Value[0].Model.Replace("[bt]", slotDict.Value[0].BodyType);
                        if (model.Contains(".gr2"))                        
                        {
                            //Console.WriteLine(model);
                            Stream modelStream = this.currentAssets.FindFile("/resources" + model).OpenCopyInMemory();                                                        
                            if (modelStream != null)
                            {
                                BinaryReader br = new BinaryReader(modelStream);
                                string name = model.Split('/').Last();
                                FileFormats.GR2 gr2_model = new FileFormats.GR2(br, name);
                                string mat0 = slotDict.Value[0].Material0;
                                string matMir = slotDict.Value[0].MaterialMirror;
                                string palette1XML = "";
                                string palette2XML = "";

                                if (slotDict.Value[0].Material0 != null)
                                {
                                    //Console.WriteLine("into material checking");
                                    if (slotDict.Value[0].PrimaryHue != "")
                                        palette1XML = "/resources" + slotDict.Value[0].PrimaryHue.Split(';').First();
                                    
                                    if(slotDict.Value[0].SecondaryHue != "")
                                        palette2XML = "/resources" + slotDict.Value[0].SecondaryHue.Split(';').First();                                     

                                    //if (slotDict.Value[0].Material0.Contains("[gen]"))
                                    //{
                                        if(slotDict.Value[0].BodyType.Contains("bf")){
                                            mat0 = mat0.Replace("[gen]", "f").Replace("[bt]", slotDict.Value[0].BodyType);
                                            matMir = matMir.Replace("[gen]", "f").Replace("[bt]", slotDict.Value[0].BodyType);
                                        }
                                        else if (slotDict.Value[0].BodyType.Contains("bm"))
                                        {
                                            mat0 = mat0.Replace("[gen]", "m").Replace("[bt]", slotDict.Value[0].BodyType);
                                            matMir = matMir.Replace("[gen]", "m").Replace("[bt]", slotDict.Value[0].BodyType);
                                        }
                                        //else
                                            //Console.WriteLine("pause - unknown body type");
                                    //}

                                    //Console.WriteLine("past gender checking");

                                    /*if (gr2_model.numMaterials == 0)
                                    {
                                        //Console.WriteLine("checking 0 material");
                                        gr2_model.numMaterials = 1;
                                        gr2_model.materials.Add(new GR2_Material(mat0));
                                        if (palette1XML != null)
                                            gr2_model.materials[0].palette1XML = palette1XML;
                                        if (palette2XML != null)
                                            gr2_model.materials[0].palette2XML = palette2XML;
                                    }
                                    else if (gr2_model.numMaterials == 1)
                                    {
                                        //Console.WriteLine("checking 1 material");
                                        gr2_model.materials[0] = new GR2_Material(mat0);  
                                        if(palette1XML != null)
                                            gr2_model.materials[0].palette1XML = palette1XML;
                                        if (palette2XML != null)
                                            gr2_model.materials[0].palette2XML = palette2XML;
                                    }
                                    else if (gr2_model.numMaterials == 2)
                                    {*/
                                        //Console.WriteLine("checking 2 material");
                                        gr2_model.materials = new List<GR2_Material>();
                                        gr2_model.materials.Add(new GR2_Material(mat0));
                                        if(matMir != mat0 && matMir != "")
                                            gr2_model.materials.Add(new GR2_Material(matMir));
                                        gr2_model.numMaterials = (ushort)gr2_model.materials.Count;
                                        if (palette1XML != null)
                                        {
                                            gr2_model.materials[0].palette1XML = palette1XML;
                                            //gr2_model.materials[1].palette1XML = palette1XML;
                                        }
                                        if (palette2XML != null)
                                        {
                                            gr2_model.materials[0].palette2XML = palette2XML;
                                            //gr2_model.materials[1].palette2XML = palette2XML;
                                        }   
                                    /*}
                                    else
                                    {
                                        //Console.WriteLine("more than 2 materials");
                                    }*/
                                    //Console.WriteLine("pause");
                                }
                                //Console.WriteLine("pause");

                                if(slotDict.Value[0].AttachedModels.Count() > 0)
                                {
                                    foreach (var attach in slotDict.Value[0].AttachedModels)
                                    {
                                        string attachFileName = attach.Replace("[bt]", slotDict.Value[0].BodyType);
                                        if (attachFileName == "")
                                            continue;
                                        Stream attachModelStream = this.currentAssets.FindFile("/resources" + attachFileName).OpenCopyInMemory();
                                        if (attachModelStream != null)
                                        {
                                            BinaryReader br2 = new BinaryReader(attachModelStream);
                                            string attachName = attachFileName.Split('/').Last();
                                            FileFormats.GR2 attachModel = new FileFormats.GR2(br2, attachName);

                                            attachModel.materials = gr2_model.materials;
                                            if (attachModel.numMaterials == 0)
                                            {
                                                //Console.WriteLine("checking 0 material");
                                                attachModel.numMaterials = 1;
                                                attachModel.materials.Add(new GR2_Material(mat0));
                                                if (palette1XML != null)
                                                    attachModel.materials[0].palette1XML = palette1XML;
                                                if (palette2XML != null)
                                                    attachModel.materials[0].palette2XML = palette2XML;
                                            }
                                            else if (attachModel.numMaterials == 1)
                                            {
                                                //Console.WriteLine("checking 1 material");
                                                attachModel.materials[0] = new GR2_Material(mat0);
                                                if(palette1XML != null)
                                                    attachModel.materials[0].palette1XML = palette1XML;
                                                if (palette2XML != null)
                                                    attachModel.materials[0].palette2XML = palette2XML;
                                            }
                                            else if (attachModel.numMaterials == 2)
                                            {
                                                //Console.WriteLine("checking 2 material");
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
                                            gr2_model.attachedModels.Add(attachModel);
                                        }
                                    }
                                }

                                models.Add(slotDict.Key.ToString(), gr2_model);
                                //Console.WriteLine("pause");
                            }
                            else
                            {
                                //Console.WriteLine("Error Loading Model");
                            }
                        }
                        if (model.Contains(".dds"))
                        {
                            Stream inputStream = this.currentAssets.FindFile("/resources" + model).OpenCopyInMemory();
                            if(inputStream != null)
                                resources.Add(slotDict.Key.ToString(), inputStream);
                            //Console.WriteLine("DDS PAUSE");
                        }
                    }
                    else
                    {
                        //Console.WriteLine("pause");
                    }
                }
                panelRender.LoadModel(models, resources, npcData);
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
                            //Console.WriteLine("into material checking");
                            if (itemData.IPP.PrimaryHue != "")
                                palette1XML = "/resources" + itemData.IPP.PrimaryHue.Split(';').First();

                            if (itemData.IPP.SecondaryHue != "")
                                palette2XML = "/resources" + itemData.IPP.SecondaryHue.Split(';').First();

                            //if (itemData.IPP.Material0.Contains("[gen]"))
                            //{
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
                                mat0 = mat0.Replace("[gen]", Bodytype.Substring(1,1)).Replace("[bt]", Bodytype);
                                matMir = mat0.Replace("[gen]", Bodytype.Substring(1, 1)).Replace("[bt]", Bodytype);
                                //Console.WriteLine("pause - unknown body type");
                            }

                            //Console.WriteLine("past gender checking");

                            if (gr2_model.numMaterials == 0)
                            {
                                //Console.WriteLine("checking 0 material");
                                gr2_model.numMaterials = 1;
                                gr2_model.materials.Add(new GR2_Material(mat0));
                                if (palette1XML != null)
                                    gr2_model.materials[0].palette1XML = palette1XML;
                                if (palette2XML != null)
                                    gr2_model.materials[0].palette2XML = palette2XML;
                            }
                            else if (gr2_model.numMaterials == 1)
                            {
                                //Console.WriteLine("checking 1 material");
                                gr2_model.materials[0] = new GR2_Material(mat0);
                                if (palette1XML != null)
                                    gr2_model.materials[0].palette1XML = palette1XML;
                                if (palette2XML != null)
                                    gr2_model.materials[0].palette2XML = palette2XML;
                            }
                            else if (gr2_model.numMaterials == 2)
                            {
                                //Console.WriteLine("checking 2 material");
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
                            else
                            {
                                //Console.WriteLine("more than 2 materials");
                            }
                            //Console.WriteLine("pause");
                        }
                        //Console.WriteLine("pause");

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
                                        //Console.WriteLine("checking 0 material");
                                        attachModel.numMaterials = 1;
                                        attachModel.materials.Add(new GR2_Material(mat0));
                                        if (palette1XML != null)
                                            attachModel.materials[0].palette1XML = palette1XML;
                                        if (palette2XML != null)
                                            attachModel.materials[0].palette2XML = palette2XML;
                                    }
                                    else if (attachModel.numMaterials == 1)
                                    {
                                        //Console.WriteLine("checking 1 material");
                                        attachModel.materials[0] = new GR2_Material(mat0);
                                        if (palette1XML != null)
                                            attachModel.materials[0].palette1XML = palette1XML;
                                        if (palette2XML != null)
                                            attachModel.materials[0].palette2XML = palette2XML;
                                    }
                                    else if (attachModel.numMaterials == 2)
                                    {
                                        //Console.WriteLine("checking 2 material");
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
                                    gr2_model.attachedModels.Add(attachModel);
                                }
                            }
                        }

                        models.Add(model.Substring(model.LastIndexOf('/') + 1), gr2_model);
                    }
                }
                if (model.Contains(".dds"))
                {
                    Stream inputStream = this.currentAssets.FindFile("/resources" + model).OpenCopyInMemory();
                    if (inputStream != null)
                        resources.Add(model.Substring(model.LastIndexOf('/') + 1), inputStream);
                    //Console.WriteLine("DDS PAUSE");
                }
                panelRender.LoadModel(models, resources, itemData, null);
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
                                //Console.WriteLine("into material checking");
                                if (itemData.IPP.PrimaryHue != "")
                                    palette1XML = "/resources" + itemData.IPP.PrimaryHue.Split(';').First();

                                if (itemData.IPP.SecondaryHue != "")
                                    palette2XML = "/resources" + itemData.IPP.SecondaryHue.Split(';').First();

                                //if (itemData.IPP.Material0.Contains("[gen]"))
                                //{
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
                                    //Console.WriteLine("pause - unknown body type");
                                }

                                //Console.WriteLine("past gender checking");

                                if (gr2_model.numMaterials == 0)
                                {
                                    //Console.WriteLine("checking 0 material");
                                    gr2_model.numMaterials = 1;
                                    gr2_model.materials.Add(new GR2_Material(mat0));
                                    if (palette1XML != null)
                                        gr2_model.materials[0].palette1XML = palette1XML;
                                    if (palette2XML != null)
                                        gr2_model.materials[0].palette2XML = palette2XML;
                                }
                                else if (gr2_model.numMaterials == 1)
                                {
                                    //Console.WriteLine("checking 1 material");
                                    gr2_model.materials[0] = new GR2_Material(mat0);
                                    if (palette1XML != null)
                                        gr2_model.materials[0].palette1XML = palette1XML;
                                    if (palette2XML != null)
                                        gr2_model.materials[0].palette2XML = palette2XML;
                                }
                                else if (gr2_model.numMaterials == 2)
                                {
                                    //Console.WriteLine("checking 2 material");
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
                                else
                                {
                                    //Console.WriteLine("more than 2 materials");
                                }
                                //Console.WriteLine("pause");
                            }
                            //Console.WriteLine("pause");

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
                                            //Console.WriteLine("checking 0 material");
                                            attachModel.numMaterials = 1;
                                            attachModel.materials.Add(new GR2_Material(mat0));
                                            if (palette1XML != null)
                                                attachModel.materials[0].palette1XML = palette1XML;
                                            if (palette2XML != null)
                                                attachModel.materials[0].palette2XML = palette2XML;
                                        }
                                        else if (attachModel.numMaterials == 1)
                                        {
                                            //Console.WriteLine("checking 1 material");
                                            attachModel.materials[0] = new GR2_Material(mat0);
                                            if (palette1XML != null)
                                                attachModel.materials[0].palette1XML = palette1XML;
                                            if (palette2XML != null)
                                                attachModel.materials[0].palette2XML = palette2XML;
                                        }
                                        else if (attachModel.numMaterials == 2)
                                        {
                                            //Console.WriteLine("checking 2 material");
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
                                        gr2_model.attachedModels.Add(attachModel);
                                    }
                                }
                            }

                            models.Add(model.Substring(model.LastIndexOf('/') + 1), gr2_model);
                        }
                    }
                    if (model.Contains(".dds"))
                    {
                        Stream inputStream = this.currentAssets.FindFile("/resources" + model).OpenCopyInMemory();
                        if (inputStream != null)
                            resources.Add(model.Substring(model.LastIndexOf('/') + 1), inputStream);
                        //Console.WriteLine("DDS PAUSE");
                    }
                }
                panelRender.LoadModel(models, resources, itemsData.First(), null);
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
                    //Console.WriteLine(model);
                    var file = this.currentAssets.FindFile("/resources" + model);
                    if (file != null)
                    {
                        Stream modelStream = file.OpenCopyInMemory();
                        BinaryReader br = new BinaryReader(modelStream);
                        string name = model.Split('/').Last();
                        FileFormats.GR2 gr2_model = new FileFormats.GR2(br, name);                        
                        models.Add(model.Substring(model.LastIndexOf('/') + 1), gr2_model);
                    }
                }

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
                        
                        if (modelList.ChildNodes.Count > 0)
                        {
                            foreach (XmlNode entryNode in modelList.ChildNodes)
                            {
                                if (entryNode.ChildNodes.Count > 0)
                                {
                                    XmlNode resFxName = entryNode.SelectSingleNode("./node()[@name='_fxName']");
                                    XmlNode resourceName = entryNode.SelectSingleNode("./node()[@name='_fxResourceName']");
                                    XmlNode rotationVecNode = entryNode.SelectSingleNode("./node()[@name='_fxAttachRotation']");
                                    //XmlNode scaleVecNode = entryNode.SelectSingleNode("node()[@name='_fxScale']");
                                    XmlNode positionVecNode = entryNode.SelectSingleNode("./node()[@name='_fxAttachPosition']");
                                    XmlNode boneVecNode = entryNode.SelectSingleNode("./node()[@name='_fxAttachBone']");                                        
                                    
                                    if (resourceName.InnerText.Contains(".gr2"))
                                    {
                                        if (resourceName.InnerText.Contains("vfx") || resourceName.InnerText.Contains("fx_"))
                                            continue;                                            
                                        string modelPath = "/resources" + resourceName.InnerText.Replace("\\", "/");
                                        var attachModel = this.currentAssets.FindFile(modelPath);
                                        if (attachModel != null)
                                        {
                                            Stream modelStream = attachModel.OpenCopyInMemory();
                                            BinaryReader br = new BinaryReader(modelStream);
                                            string name = modelPath.Split('/').Last();
                                            FileFormats.GR2 gr2_model = new FileFormats.GR2(br, name);


                                            if (rotationVecNode != null)
                                            {
                                                string[] temp = rotationVecNode.InnerText.ToString().Replace("(", "").Replace(")", "").Split(',');
                                                Vector3 rotationVec = new Vector3(float.Parse(temp[0]), float.Parse(temp[1]), float.Parse(temp[2]));
                                                if (rotationVec != blankVec)
                                                {
                                                    Matrix rotMatrix = Matrix.RotationX((float)(rotationVec.X * Math.PI) / 180) * Matrix.RotationY((float)(rotationVec.Y * Math.PI) / 180) * Matrix.RotationZ((float)(rotationVec.Z * Math.PI) / 180);
                                                    gr2_model.rotationMatrix = rotMatrix;
                                                }
                                            }
                                            /*
                                            if (scaleVecNode != null)
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
                                            if (positionVecNode != null)
                                            {
                                                string[] temp = positionVecNode.InnerText.ToString().Replace("(", "").Replace(")", "").Split(',');
                                                Vector3 positionVec = new Vector3(float.Parse(temp[0]), float.Parse(temp[1]), float.Parse(temp[2]));
                                                if (positionVec != blankVec)
                                                {
                                                    Matrix positionMatrix = Matrix.Translation(positionVec);
                                                    gr2_model.positionMatrix = positionMatrix;
                                                }
                                            }

                                            if (boneVecNode != null)
                                            {
                                                GR2_Attachment attach = (GR2_Attachment)models.First().Value.attachments.SingleOrDefault(x => x.attachName == boneVecNode.InnerText);
                                                if (attach != null)
                                                {
                                                    gr2_model.attachMatrix = attach.attach_matrix;
                                                }
                                            }

                                            models.Add(resFxName.InnerText, gr2_model);
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
                           
                panelRender.LoadModel(models, resources, null, obj);
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
                    Vector3 rotationVec = new Vector3();
                    Vector3 scaleVec = new Vector3();
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
                        else if (itm.Key == "dynRotation" || itm.Key == "dynScale" || itm.Key == "dynPosition")
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

                    //Console.WriteLine(model);
                    if (model.Contains("designblockout"))
                        continue;
                    var file = this.currentAssets.FindFile("/resources" + model);
                    if (file != null)
                    {
                        Stream modelStream = file.OpenCopyInMemory();
                        BinaryReader br = new BinaryReader(modelStream);
                        string name = model.Split('/').Last();
                        FileFormats.GR2 gr2_model = new FileFormats.GR2(br, name);
                        Vector3 blankVec = new Vector3();
                        if (positionVec != blankVec)
                        {
                            Matrix posMatrix = Matrix.Translation(positionVec);
                            gr2_model.positionMatrix = posMatrix;
                        }

                        if (scaleVec != blankVec)
                        {
                            Matrix scaleMatrix = Matrix.Scaling(scaleVec);
                            gr2_model.scaleMatrix = scaleMatrix;
                        }

                        if (rotationVec != blankVec)
                        {
                            Matrix rotMatrix = Matrix.RotationX((float)(rotationVec.X * Math.PI) / 180) * Matrix.RotationY((float)(rotationVec.Y * Math.PI) / 180) * Matrix.RotationZ((float)(rotationVec.Z * Math.PI) / 180);
                            gr2_model.rotationMatrix = rotMatrix;
                        }
                        try
                        {
                            string key = model.Substring(model.LastIndexOf('/') + 1);
                            if(models.ContainsKey(key))
                            {
                                int i = 0;
                                while(models.ContainsKey(key) != false)
                                {
                                    key += i;
                                    i++;
                                }
                            }
                            models.Add(key, gr2_model);
                        }
                        catch (Exception e)
                        {
                            Console.WriteLine(e.StackTrace.ToString());
                            Console.WriteLine("pause here");
                        }
                      
                    }
                    /*
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
                            XmlNodeList resourceList = modelList.SelectNodes("//node()[@name='_fxResourceName']");
                            if (resourceList.Count > 0)
                            {
                                foreach (XmlNode node in resourceList)
                                {
                                    string modelPath = "/resources" + node.InnerText.Replace("\\", "/");
                                    if (modelPath.Contains(".gr2"))
                                    {
                                        var attachModel = this.currentAssets.FindFile(modelPath);
                                        if (attachModel != null)
                                        {
                                            Stream modelStream = attachModel.OpenCopyInMemory();
                                            BinaryReader br = new BinaryReader(modelStream);
                                            string name = modelPath.Split('/').Last();
                                            FileFormats.GR2 gr2_model = new FileFormats.GR2(br, name);
                                            models.First().Value.attachedModels.Add(gr2_model);
                                        }
                                    }
                                }
                            }
                        }
                    }
                     */
                }
                panelRender.LoadModel(models, resources, null, obj);
                render = new Thread(panelRender.startRender);
                render.IsBackground = true;
                render.Start();
            }
        }

        private async Task previewGR2(HashFileInfo info)
        {
            if (InvokeRequired)
            {
                Invoke(new Action(() => previewGR2(info)));
            }
            else
            {   
                if (info != null)
                {
                    Stream modelStream = info.file.OpenCopyInMemory();
                    BinaryReader br = new BinaryReader(modelStream);
                    string name = info.FileName.Split('/').Last();
                    FileFormats.GR2 gr2_model = new FileFormats.GR2(br, name);
                    models.Add(info.FileName.Substring(info.FileName.LastIndexOf('/') + 1), gr2_model);
                }
                panelRender.LoadModel(models, resources, null, null);
                render = new Thread(panelRender.startRender);
                render.IsBackground = true;
                render.Start();
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

                if (treeViewFast1.SelectedNode.Nodes.Count > 0)
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
            WorldBrowserHelp formHelp = new WorldBrowserHelp();
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
                    dt.Rows.Add(new string[] { "Pal 1", material.palette1.NullSafeToString() });
                    dt.Rows.Add(new string[] { "Pal 1 Met Spec", material.palette1MetSpec.NullSafeToString() });
                    dt.Rows.Add(new string[] { "Pal 1 Spec", material.palette1Spec.NullSafeToString() });
                    dt.Rows.Add(new string[] { "Pal 2", material.palette2.NullSafeToString() });
                    dt.Rows.Add(new string[] { "Pal 2 Met Spec", material.palette2MetSpec.NullSafeToString() });
                    dt.Rows.Add(new string[] { "Pal 2 Spec", material.palette2Spec.NullSafeToString() });                    
                }
                else if(tag.dynObject is GR2_Mesh)
                {
                    GR2_Mesh mesh = (GR2_Mesh)tag.dynObject;
                    dt.Rows.Add(new string[] { "# Bones", mesh.numBones.NullSafeToString() });
                    dt.Rows.Add(new string[] { "# Pieces", mesh.numPieces.NullSafeToString() });
                    dt.Rows.Add(new string[] { "# Vertices", mesh.numVerts.NullSafeToString() });
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
                    contextMenuStrip2.Show(tvfDataViewer, e.Location);
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
    }
}
