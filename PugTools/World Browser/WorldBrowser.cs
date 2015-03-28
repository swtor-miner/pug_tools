﻿using System;
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
using File = TorLib.File;
//using TreeViewFast.Controls;

namespace tor_tools
{
    public partial class WorldBrowser : Form
    {
        public TorLib.Assets currentAssets;
        private DataObjectModel currentDom;

        private Dictionary<string, NodeAsset> assetDict = new Dictionary<string, NodeAsset>();
        private List<string> nodeKeys = new List<string>();

        private Dictionary<string, NodeAsset> dataviewDict = new Dictionary<string, NodeAsset>();

        private View_AREA panelRender;
        public bool _closing = false;
        private Thread render;

        public Dictionary<long, GR2> loadedModels = new Dictionary<long, GR2>();
        public Dictionary<string, GR2_Material> loadedMaterials = new Dictionary<string, GR2_Material>();
        public AreaSpecification area;
        

        List<object> mapAreaData = new List<object>();

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
            mapAreaData = currentDom.GetObject("mapareasdata").Data.Get<List<object>>("utlDatatableRows");
            //List<GomObject> itmList = currentDom.GetObjectsStartingWith("world.").ToList();
            HashSet<string> nodeDirs = new HashSet<string>();
            HashSet<string> allDirs = new HashSet<string>();

            Dictionary<ulong, string> mapAreas = new Dictionary<ulong, string>();
            StringTable strTable = this.currentDom.stringTable.Find("str.sys.worldmap");

            foreach (List<object> area in mapAreaData)
            {
                ulong id = ulong.Parse(area[0].ToString());
                string name;
                if ((string)area[1] != "")
                {
                    long nameId = long.Parse(area[1].ToString());
                    name = strTable.GetText(nameId, null);
                    if(name == "")
                        name = id.ToString();
                }
                else
                {
                    name = id.ToString();
                }
                mapAreas.Add(id, name + " - " + area[3].NullSafeToString());
                //Console.WriteLine(name);
            }

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
                            if (hashInfo.FileName == "metadata.bin" || hashInfo.FileName == "ft.sig" || hashInfo.FileName.ToUpper() != "AREA.DAT")
                            {
                                continue;
                            }
                            string name;
                            ulong id = ulong.Parse(hashInfo.Directory.Replace("/resources/world/areas/", "").Replace("/resources/world/livecontent/systemgenerated/", "").Replace("/area.dat", ""));
                            if(mapAreas.ContainsKey(id))
                                name = mapAreas[id];
                            else
                                name = id.ToString();
                            NodeAsset assetAll = new NodeAsset("/" + id, "/", name, hashInfo);
                            assetDict.Add("/" + id, assetAll);
                        }                      
                    }
                }
            }
            assetDict.Add("/", new NodeAsset("/", "", "Root", (GomObject)null));            
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
            panelRender = new View_AREA(this.Handle, this, "renderPanel");
            panelRender.Init();
            if (treeViewFast1.Nodes.Count > 0) treeViewFast1.Nodes[0].Expand();
        }

        private void disableUI()
        {
            btnDataCollapse.Enabled = false;
            btnWorldBrowserHelp.Enabled = false;
            btnStopRender.Enabled = false;
            exportButton.Enabled = false;
            treeViewFast1.Enabled = false;
            tvfDataViewer.Enabled = false;
            dgvDataViewer.Enabled = false;
        }

        private void enableUI()
        {
            btnDataCollapse.Enabled = true;
            btnWorldBrowserHelp.Enabled = true;
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
                
                this.loadedMaterials = new Dictionary<string, GR2_Material>();
                this.loadedModels = new Dictionary<long, GR2>();

                dgvDataViewer.DataSource = null;
                dgvDataViewer.Enabled = false;
                tvfDataViewer.Nodes.Clear();
                tvfDataViewer.Enabled = false;
                if (tag.dynObject != null && tag.dynObject is HashFileInfo)
                {
                    this.Text = "World Browser - " + tag.displayName;
                    this.toolStripStatusLabel1.Text = "Loading Area.dat File...";
                    this.Refresh();
                    HashFileInfo info = (HashFileInfo)tag.dynObject;                    
                    await Task.Run(() => previewAREA(info));
                    renderPanel.Visible = true;
                    treeViewFast1.Enabled = true;
                    toolStripProgressBar1.Visible = false;
                    this.toolStripStatusLabel1.Text = "GR2 Loaded";

                }
                else if (tag.obj != null)
                {
                    GomObject obj = (GomObject)tag.obj;
                    NpcAppearance npcData = null;                    
                    try
                    {
                        switch (obj.Name.Substring(0, 5))
                        {
                            case "world":
                                npcData = (NpcAppearance)this.currentDom.appearanceLoader.Load(obj.Name);
                                this.toolStripStatusLabel1.Text = "Loading WORLD Data ...";
                                this.Refresh();
                                //await Task.Run(() => previewNPC_GR2(npcData));
                                this.toolStripStatusLabel1.Text = "NPP Loaded";
                                break;
                        }
                        //buildDataViewer();
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
            }
        }

        /*
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
                        if (!dataviewDict.ContainsKey("/models/" + model.Key + "/materials"))
                            dataviewDict.Add("/models/" + model.Key + "/materials", new NodeAsset("/models/" + model.Key + "/materials", "/models/" + model.Key, "Materials", (GomObject)null));
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
        */
        

        private async Task previewAREA(HashFileInfo info)
        {
            if (InvokeRequired)
            {
                Invoke(new Action(() => previewAREA(info)));
            }
            else
            {
                area = new AreaSpecification(info, this.currentAssets);
                area.Read();
                List<AreaAsset> assetModels = area.AssetsByExtension["gr2"];

                foreach(RoomSpecification room in area.Rooms)
                {
                    foreach(KeyValuePair<long, List<AssetInstance>> kvp in room.InstancesByAssetId)
                    {
                        AreaAsset asset = assetModels.Find(x => x.Id == kvp.Key);
                        if(asset != null)
                        {                            
                            foreach(AssetInstance instance in kvp.Value)
                            {
                                if (instance.Hidden)
                                    continue;
                                if(loadedModels.Keys.Contains(instance.AssetId))
                                {
                                    continue;
                                }else{
                                    string modelPath = "/resources/" + asset.Path.Replace("\\", "/") + ".gr2";
                                    File modelFile = this.currentAssets.FindFile(modelPath);
                                    Stream modelStream = modelFile.OpenCopyInMemory();
                                    BinaryReader br = new BinaryReader(modelStream);
                                    GR2 gr2_model = new FileFormats.GR2(br, instance.Id.ToString(), loadedMaterials);                                 
                                    loadedModels.Add(asset.Id, gr2_model);                                    
                                }
                            }
                        }
                    }    
                }
                panelRender.LoadModel(loadedModels, loadedMaterials, area.Rooms, area.Id.ToString(), "");
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

        private void WorldBrowser_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (render != null)
            {
                panelRender.stopRender();
                render.Join();
                panelRender.Clear();
            }
            loadedMaterials.Clear();
            loadedModels.Clear();
            panelRender.Dispose();
            assetDict.Clear();
            nodeKeys.Clear();            
            this.Dispose();
        }    

        private void btnWorldBrowserHelp_Click(object sender, EventArgs e)
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
                    dt.Rows.Add(new string[] { "PaletteMask", material.paletteMaskDDS.NullSafeToString() });
                    dt.Rows.Add(new string[] { "PaletteMaskMap", material.paletteMaskDDS.NullSafeToString() });
                    dt.Rows.Add(new string[] { "UsesEmissive", material.useEmissive.NullSafeToString() });
                    dt.Rows.Add(new string[] { "Pal 1", material.palette1.NullSafeToString() });
                    dt.Rows.Add(new string[] { "Pal 1 Met Spec", material.palette1MetSpec.NullSafeToString() });
                    dt.Rows.Add(new string[] { "Pal 1 Spec", material.palette1Spec.NullSafeToString() });
                    dt.Rows.Add(new string[] { "Pal 2", material.palette2.NullSafeToString() });
                    dt.Rows.Add(new string[] { "Pal 2 Met Spec", material.palette2MetSpec.NullSafeToString() });
                    dt.Rows.Add(new string[] { "Pal 2 Spec", material.palette2Spec.NullSafeToString() });
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
            /*
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
            }*/
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
            /*
            NodeAsset asset = (NodeAsset)tvfDataViewer.SelectedNode.Tag;
            if (asset.dynObject != null && asset.dynObject is GR2_Material)
            {
                GR2_Material material = (GR2_Material)asset.dynObject;
                WorldBrowserViewMaterial matView = new WorldBrowserViewMaterial(material);
                matView.Show();
            }*/
        }
    }
}
