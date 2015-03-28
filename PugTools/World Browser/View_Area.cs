using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.IO;
using System.Diagnostics;
using System.Drawing;
using System.Windows.Forms;
using System.Xml;
using SlimDX;
using SlimDX.Direct3D11;
using SlimDX.D3DCompiler;
using SlimDX.DXGI;
using Buffer = SlimDX.Direct3D11.Buffer;
using ShaderResourceView = SlimDX.Direct3D11.ShaderResourceView;
using SlimDX_Framework;
using SlimDX_Framework.Camera;
using SlimDX_Framework.FX;
using SlimDX_Framework.Vertex;
using FileFormats;
using GomLib;
using GomLib.Models;

namespace tor_tools
{
    class View_AREA : D3DPanelApp
    { 
        String fqn;

        Dictionary<long, GR2> models = new Dictionary<long, GR2>();
        Dictionary<string, Stream> resources = new Dictionary<string, Stream>();
        Dictionary<string, GR2_Material> materials = new Dictionary<string, GR2_Material>();
        List<RoomSpecification> rooms = new List<RoomSpecification>();

        private DirectionalLight[] _dirLights;

        private Matrix _texTransform;
        private Matrix _world;
        private Matrix _view;
        private Matrix _proj;

        private Point _lastMousePos;

        public bool _disposed;
        private GR2_Effect _fx;

        private FpsCamera _cam;
        private LookAtCamera _cam2;
        private bool _useFpsCamera;
        private float _FPScameraSpeed = 1.0f;
        private float _LookAtZoomSpeed = 0.05f;

        private Vector3 globalBoxMin;
        private Vector3 globalBoxMax;
        private Vector3 globalBoxCenter;
        private Vector3 cameraPos;

        private List<PosNormalTexTan> vertices = new List<PosNormalTexTan>();
        private List<FileFormats.GR2_Mesh_Vertex_Index> indexes = new List<FileFormats.GR2_Mesh_Vertex_Index>();
        private List<ushort> indexList = new List<ushort>();

        bool makeScreenshot = false;
        bool rotateModel = false;

        private Matrix blankMatrix;
        private GR2 focus = new GR2();

        public string selectedModel = "";

        public View_AREA(IntPtr hInstance, Form form, string panelName = "")
            : base(hInstance, panelName)
        {
            Window = form;
            RenderPanelName = panelName;
            Enable4XMsaa = true;
            ClientHeight = form.Controls.Find(panelName, true).First().Height;
            ClientWidth = form.Controls.Find(panelName, true).First().Width;

            _useFpsCamera = true;
            _cam = new FpsCamera();
            _cam2 = new LookAtCamera();

            _lastMousePos = new Point();

            _world = Matrix.Identity;
            _texTransform = Matrix.Identity;
            _view = Matrix.Identity;
            _proj = Matrix.Identity;

            _dirLights = new[] {                
                new DirectionalLight {
                Ambient = Color.White,
                Diffuse = Color.White,                
                Specular = new Color4(0.5f, 0.5f, 0.5f),
                Direction = new Vector3(0.57735f, -0.57735f, 0.57735f)
                },              
            };

        }

        public void Clear()
        {   
            if (this.models != null)
            {
                foreach (var model in this.models)
                {
                    foreach (var mesh in model.Value.meshes)
                    {
                        Util.ReleaseCom(ref mesh.meshIdxBuff);
                        Util.ReleaseCom(ref mesh.meshVertBuff);
                    }

                    foreach (var mat in model.Value.materials)
                    {
                        Util.ReleaseCom(ref mat.diffuseSRV);
                        Util.ReleaseCom(ref mat.rotationSRV);
                        Util.ReleaseCom(ref mat.glossSRV);                        
                    }

                    if (model.Value.attachedModels.Count() > 0)
                    {
                        foreach (var attach in model.Value.attachedModels)
                        {
                            if (attach.attachedModels.Count() > 0)
                            {
                                Console.WriteLine("pause");
                            }

                            foreach (var mesh in attach.meshes)
                            {
                                Util.ReleaseCom(ref mesh.meshIdxBuff);
                                Util.ReleaseCom(ref mesh.meshVertBuff);
                            }

                            foreach (var mat in attach.materials)
                            {
                                Util.ReleaseCom(ref mat.diffuseSRV);
                                Util.ReleaseCom(ref mat.rotationSRV);
                                Util.ReleaseCom(ref mat.glossSRV);                              
                            }
                            attach.Dispose();
                        }
                    }
                    model.Value.Dispose();
                }
                this.models.Clear();
            }
            if (this.materials != null)
            {
                foreach (var mat in this.materials)
                {
                    Util.ReleaseCom(ref mat.Value.diffuseSRV);
                    Util.ReleaseCom(ref mat.Value.rotationSRV);
                    Util.ReleaseCom(ref mat.Value.glossSRV);          
                }
                this.materials.Clear();
            }
            this.resources.Clear();
            this.vertices.Clear();
            this.indexes.Clear();
            this.indexList.Clear();
            rotateModel = false;
        }

        public void LoadModel(Dictionary<long, GR2> loadedModels, Dictionary<string, GR2_Material> materials, List<RoomSpecification> rooms, string fqn, string type = "")
        {
            this.fqn = fqn;
            rotateModel = false;
            focus = loadedModels.First().Value;

            if (models != null)
                this.models = loadedModels;            
            if (materials != null)
                this.materials = materials;
            if (rooms != null)
                this.rooms = rooms;

            this.globalBoxMin = new Vector3(focus.global_box.minX, focus.global_box.minY, focus.global_box.minZ);
            this.globalBoxMax = new Vector3(focus.global_box.maxX, focus.global_box.maxY, focus.global_box.maxZ);
            this.globalBoxCenter = globalBoxMin + (globalBoxMax - globalBoxMin) / 2;
            this.cameraPos = new Vector3(globalBoxCenter.X * 2.5f, globalBoxCenter.Y * 2.5f, (Math.Max(Math.Max(globalBoxMax.X, globalBoxMax.Y), globalBoxMax.Z) * 2.5f /*+ 1.0f*/));

            this._useFpsCamera = true;
            this._cam.Reset();
            this._cam.Position = cameraPos;

            this._cam2.Reset();
            this._cam2.Position = cameraPos;
            this._cam2.LookAt(cameraPos, globalBoxCenter, Vector3.UnitY);

            if (this.materials.Count > 0)
            {
                foreach (var mat in this.materials)
                {
                    mat.Value.ParseMAT(Device);
                }
            }          
            BuildGeometry();
        }

        protected override void Dispose(bool disposing)
        {
            if (!_disposed)
            {
                if (disposing)
                {
                    foreach (var model in models)
                    {
                        foreach (var mesh in model.Value.meshes)
                        {
                            Util.ReleaseCom(ref mesh.meshIdxBuff);
                            Util.ReleaseCom(ref mesh.meshVertBuff);
                        }

                        foreach (var mat in model.Value.materials)
                        {
                            Util.ReleaseCom(ref mat.diffuseSRV);
                            Util.ReleaseCom(ref mat.rotationSRV);
                            Util.ReleaseCom(ref mat.glossSRV);
                        }                      

                        if (model.Value.attachedModels.Count() > 0)
                        {
                            foreach (var attach in model.Value.attachedModels)
                            {
                                foreach (var mesh in attach.meshes)
                                {
                                    Util.ReleaseCom(ref mesh.meshIdxBuff);
                                    Util.ReleaseCom(ref mesh.meshVertBuff);
                                }

                                foreach (var mat in attach.materials)
                                {
                                    Util.ReleaseCom(ref mat.diffuseSRV);
                                    Util.ReleaseCom(ref mat.rotationSRV);
                                    Util.ReleaseCom(ref mat.glossSRV);                               
                                }
                                attach.Dispose();
                            }
                        }
                        model.Value.Dispose();
                    }


                    if (this.materials.Count > 0)
                    {
                        foreach (var mat in this.materials)
                        {   
                            Util.ReleaseCom(ref mat.Value.diffuseSRV);
                            Util.ReleaseCom(ref mat.Value.rotationSRV);
                            Util.ReleaseCom(ref mat.Value.glossSRV);
                        }
                    }
                    Effects.DestroyAll();
                    InputLayouts.DestroyAll();
                    RenderStates.DestroyAll();
                }
                _disposed = true;
            }
            base.Dispose(disposing);
        }

        public override bool Init()
        {
            if (!base.Init()) return false;

            Effects.InitAll(Device);
            _fx = Effects.GR2_FX;
            InputLayouts.InitAll(Device);
            RenderStates.InitAll(Device);
            return true;
        }

        public override void OnResize()
        {
            base.OnResize();
            _cam.SetLens(0.25f * MathF.PI, AspectRatio, 0.001f, 1000.0f);
            _cam2.SetLens(0.25f * MathF.PI, AspectRatio, 0.001f, 1000.0f);
            _proj = Matrix.PerspectiveFovRH(0.25f * MathF.PI, AspectRatio, 1.0f, 1000.0f);
        }

        public override void UpdateScene(float dt)
        {
            base.UpdateScene(dt);

            if (Form.ActiveForm != null)
            {
                if (Util.IsKeyDown(Keys.R))
                {
                    if (_useFpsCamera)
                    {
                        this._cam.Reset();
                        this._cam.Position = cameraPos;
                    }
                    else
                    {
                        this._cam2.Reset();
                        this._cam2.Position = cameraPos;
                        this._cam2.LookAt(cameraPos, globalBoxCenter, Vector3.UnitY);
                    }
                }

                if (Util.IsKeyDown(Keys.Oemplus))
                {
                    _FPScameraSpeed = 60.0f;
                    _LookAtZoomSpeed = 0.20f;
                }

                if (Util.IsKeyDown(Keys.OemMinus))
                {
                    _FPScameraSpeed = 30.0f;
                    _LookAtZoomSpeed = 0.05f;
                }

                if (Util.IsKeyDown(Keys.W) && _useFpsCamera)
                    if (Util.IsKeyDown(Keys.LShiftKey))
                        _cam.Walk(-30f * dt);
                    else if (Util.IsKeyDown(Keys.LControlKey))
                        _cam.Walk(-120.0f * dt);
                    else
                        _cam.Walk(-_FPScameraSpeed * dt);

                if (Util.IsKeyDown(Keys.S) && _useFpsCamera)
                    if (Util.IsKeyDown(Keys.LShiftKey))
                        _cam.Walk(0.2f * dt);
                    else if (Util.IsKeyDown(Keys.LControlKey))
                        _cam.Walk(30.0f * dt);
                    else
                        _cam.Walk(_FPScameraSpeed * dt);

                if (Util.IsKeyDown(Keys.A) && _useFpsCamera)
                    if (Util.IsKeyDown(Keys.LShiftKey))
                        _cam.Strafe(-0.2f * dt);
                    else if (Util.IsKeyDown(Keys.LControlKey))
                        _cam.Strafe(-30.0f * dt);
                    else
                        _cam.Strafe(-_FPScameraSpeed * dt);

                if (Util.IsKeyDown(Keys.D) && _useFpsCamera)
                    if (Util.IsKeyDown(Keys.LShiftKey))
                        _cam.Strafe(0.2f * dt);
                    else if (Util.IsKeyDown(Keys.LControlKey))
                        _cam.Strafe(30.0f * dt);
                    else
                        _cam.Strafe(_FPScameraSpeed * dt);

                if (Util.IsKeyDown(Keys.L))
                    _useFpsCamera = false;

                if (Util.IsKeyDown(Keys.F))
                    _useFpsCamera = true;

                if (Util.IsKeyDown(Keys.PageUp))
                    if (!_useFpsCamera)
                        _cam2.Zoom(-_FPScameraSpeed * dt);
                    else
                        _cam.Zoom(-dt);

                if (Util.IsKeyDown(Keys.PageDown))
                    if (!_useFpsCamera)
                        _cam2.Zoom(_FPScameraSpeed * dt);
                    else
                        _cam.Zoom(+dt);

                if (Util.IsKeyDown(Keys.L))
                    _useFpsCamera = false;

                if (Util.IsKeyDown(Keys.F))
                    _useFpsCamera = true;

                if (Util.IsKeyDown(Keys.PageUp))
                    if (!_useFpsCamera)
                        _cam2.Zoom(-_FPScameraSpeed * dt);
                    else
                        _cam.Zoom(-dt);

                if (Util.IsKeyDown(Keys.PageDown))
                    if (!_useFpsCamera)
                        _cam2.Zoom(_FPScameraSpeed * dt);
                    else
                        _cam.Zoom(+dt);
            }
            System.Threading.Thread.Sleep(1); //Fix for UI lag. Sleeps the thread for 1 millisecond...
        }

        public override void DrawScene()
        {
            base.DrawScene();
            ImmediateContext.ClearRenderTargetView(RenderTargetView, Color.LightSteelBlue);
            ImmediateContext.ClearDepthStencilView(DepthStencilView, DepthStencilClearFlags.Depth | DepthStencilClearFlags.Stencil, 1.0f, 0);

            ImmediateContext.InputAssembler.InputLayout = InputLayouts.PosNormalTexTan;
            ImmediateContext.InputAssembler.PrimitiveTopology = PrimitiveTopology.TriangleList;

            ImmediateContext.OutputMerger.BlendState = RenderStates.TransparentBS;
            ImmediateContext.OutputMerger.BlendFactor = new Color4(0, 0, 0, 0);
            ImmediateContext.OutputMerger.BlendSampleMask = ~0;

            ImmediateContext.Rasterizer.State = RenderStates.CullClockwiseNoneRS;

            Matrix view;
            Matrix proj;
            Matrix viewProj;

            if (_useFpsCamera)
            {
                _cam.UpdateViewMatrix();
                view = _cam.View;
                proj = _cam.Proj;
                viewProj = _cam.ViewProj;
                _fx.SetEyePosW(_cam.Position);
            }
            else
            {
                _cam2.UpdateViewMatrix();
                view = _cam2.View;
                proj = _cam2.Proj;
                viewProj = _cam2.ViewProj;
                _fx.SetEyePosW(_cam2.Position);
            }

            _fx.SetDirLights(_dirLights);
            _fx.SetEyePosW(_cam.Position);

            var activeTech = _fx.Light1Tech;

            if (Form.ActiveForm != null)
            {
                /*
                if (Util.IsKeyDown(Keys.Q))
                {
                    activeTech = _fx.Light1GarmentMask;
                }

                if (Util.IsKeyDown(Keys.E))
                {
                    activeTech = _fx.Light1GarmentDiffuse;
                }

                if (Util.IsKeyDown(Keys.D1))
                {
                    activeTech = _fx.Light1GarmentPalette1;
                }

                if (Util.IsKeyDown(Keys.D2))
                {
                    activeTech = _fx.Light1GarmentPalette2;
                }

                if (Util.IsKeyDown(Keys.D3))
                {
                    activeTech = _fx.Light1GarmentPaletteMap;
                }*/

                if (Util.IsKeyDown(Keys.C))
                {
                    ImmediateContext.Rasterizer.State = RenderStates.WireframeNoneRS;
                }

                if (Util.IsKeyDown(Keys.PrintScreen))
                {
                    makeScreenshot = true;
                }
            }

          

            foreach (var room in this.rooms)
            {
                foreach (KeyValuePair<long, List<AssetInstance>> kvp in room.InstancesByAssetId)
                {
                    foreach (AssetInstance instance in kvp.Value)
                    {
                        if (instance.Hidden)
                            continue;
                        if (this.models.Keys.Contains(instance.AssetId))
                        {
                            var world = _world * instance.GetAbsoluteTransform();
                            var wit = MathF.InverseTranspose(world);
                            var wvp = world * viewProj;

                            _fx.SetWorld(world);
                            _fx.SetWorldInvTranspose(wit);
                            _fx.SetWorldViewProj(wvp);
                            _fx.SetTexTransform(_texTransform);
                            
                            GR2 model = this.models[instance.AssetId];
                            foreach (var mesh in model.meshes)
                            {
                                if (mesh.meshName.Contains("collision"))
                                    continue;

                                foreach (FileFormats.GR2_Mesh_Piece piece in mesh.meshPieces)
                                {
                                    ImmediateContext.InputAssembler.SetVertexBuffers(0, new VertexBufferBinding(mesh.meshVertBuff, PosNormalTexTan.Stride, 0));
                                    ImmediateContext.InputAssembler.SetIndexBuffer(mesh.meshIdxBuff, Format.R16_UInt, 0);

                                    if (piece.matID != -1)
                                    {
                                        if (model.materials.ElementAtOrDefault(piece.matID) != null)
                                        {
                                            string matName = model.materials[piece.matID].materialName;
                                            if (this.materials.Keys.Contains(matName))
                                            {
                                                GR2_Material material = this.materials[matName];
                                                if (material != null)
                                                {
                                                    _fx.SetDiffuseMap(material.diffuseSRV);
                                                    _fx.SetGlossMap(material.glossSRV);
                                                    _fx.SetRotationMap(material.rotationSRV);
                                                    _fx.SetAlphaClipValue(new Vector2(material.alphaClipValue));
                                                }
                                            }
                                        }
                                    }
                                    else
                                    {
                                        if (model.materials.Count > 0)
                                        {
                                            string matName = model.materials[0].materialName;
                                            if (this.materials.Keys.Contains(matName))
                                            {
                                                GR2_Material material = this.materials[matName];
                                                if (material != null)
                                                {
                                                    _fx.SetDiffuseMap(material.diffuseSRV);
                                                    _fx.SetGlossMap(material.glossSRV);
                                                    _fx.SetRotationMap(material.rotationSRV);
                                                    _fx.SetAlphaClipValue(new Vector2(material.alphaClipValue));
                                                }
                                            }
                                        }
                                    }

                                    activeTech.GetPassByIndex(0).Apply(ImmediateContext);

                                    ImmediateContext.DrawIndexed(((int)piece.numPieceFaces) * 3, ((int)piece.startIndex) * 3, 0);
                                }
                            }

                            if (model.attachedModels.Count() > 0)
                            {
                                foreach (var attachModel in model.attachedModels)
                                {
                                    foreach (var attachMesh in attachModel.meshes)
                                    {
                                        if (attachMesh.meshName.Contains("collision"))
                                            continue;

                                        foreach (FileFormats.GR2_Mesh_Piece attachPiece in attachMesh.meshPieces)
                                        {
                                            ImmediateContext.InputAssembler.SetVertexBuffers(0, new VertexBufferBinding(attachMesh.meshVertBuff, PosNormalTexTan.Stride, 0));
                                            ImmediateContext.InputAssembler.SetIndexBuffer(attachMesh.meshIdxBuff, Format.R16_UInt, 0);

                                            if (attachPiece.matID != -1)
                                            {
                                                string matName = attachModel.materials[attachPiece.matID].materialName;
                                                if (this.materials.Keys.Contains(matName))
                                                {
                                                    GR2_Material material = this.materials[matName];
                                                    if (material != null)
                                                    {
                                                        _fx.SetDiffuseMap(material.diffuseSRV);
                                                        _fx.SetGlossMap(material.glossSRV);
                                                        _fx.SetRotationMap(material.rotationSRV);
                                                        _fx.SetAlphaClipValue(new Vector2(material.alphaClipValue));
                                                    }
                                                }
                                            }
                                            else
                                            {
                                                string matName = attachModel.materials[0].materialName;
                                                if (this.materials.Keys.Contains(matName))
                                                {
                                                    GR2_Material material = this.materials[matName];
                                                    if (material != null)
                                                    {
                                                        _fx.SetDiffuseMap(material.diffuseSRV);
                                                        _fx.SetGlossMap(material.glossSRV);
                                                        _fx.SetRotationMap(material.rotationSRV);
                                                        _fx.SetAlphaClipValue(new Vector2(material.alphaClipValue));
                                                    }
                                                }
                                            }

                                            activeTech.GetPassByIndex(0).Apply(ImmediateContext);

                                            ImmediateContext.DrawIndexed(((int)attachPiece.numPieceFaces) * 3, ((int)attachPiece.startIndex) * 3, 0);
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            }                   

            SwapChain.Present(1, PresentFlags.None);

            if (makeScreenshot)
            {
                this.MakeScreenshot(ImageFileFormat.Jpg);
                makeScreenshot = false;
            }

        }

        public void MakeScreenshot(SlimDX.Direct3D11.ImageFileFormat format)
        {
            try
            {
                string filename = tor_tools.Tools.PrepExtractPath(this.fqn + '-' + DateTime.Now.ToString("yyyyMMddHHmmss") + '.' + format.ToString().ToLower());
                var outputDesc = new Texture2DDescription
                {
                    Width = ClientWidth,
                    Height = ClientHeight,
                    MipLevels = 1,
                    ArraySize = 1,
                    Format = Format.R8G8B8A8_UNorm,
                    SampleDescription = new SampleDescription(1, 0),
                    Usage = ResourceUsage.Default,
                    BindFlags = BindFlags.None,
                    CpuAccessFlags = CpuAccessFlags.None,
                };
                Texture2D outputFile = new Texture2D(Device, outputDesc);
                var BackBuffer = SlimDX.Direct3D11.Resource.FromSwapChain<Texture2D>(SwapChain, 0);

                ImmediateContext.ResolveSubresource(BackBuffer, 0, outputFile, 0, Format.R8G8B8A8_UNorm);
                Texture2D.ToFile(ImmediateContext, outputFile, format, filename);
                Util.ReleaseCom(ref outputFile);
                ((tor_tools.ModelBrowser)Window).setStatusLabel("Screenshot Completed");
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }
        }

        protected override void OnMouseDown(object sender, MouseEventArgs mouseEventArgs)
        {
            _lastMousePos = mouseEventArgs.Location;
            Window.Controls.Find(RenderPanelName, true).First().Capture = true;
        }

        protected override void OnMouseUp(object sender, MouseEventArgs e)
        {
            Window.Controls.Find(RenderPanelName, true).First().Capture = true;
        }

        protected override void OnMouseMove(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                var dy = MathF.ToRadians(0.4f * (e.Y - _lastMousePos.Y));
                var dx = -(MathF.ToRadians(0.4f * (e.X - _lastMousePos.X)));
                if (_useFpsCamera)
                {
                    _cam.Pitch(-dy);
                    _cam.Yaw(dx);
                }
                else
                {
                    if (Util.IsKeyDown(Keys.LShiftKey))
                    {
                        dx = MathF.ToRadians(0.05f * (e.X - _lastMousePos.X));
                        dy = MathF.ToRadians(0.05f * (e.Y - _lastMousePos.Y));
                        _cam2.Strafe(-dx * _cam2.Radius);
                        _cam2.Fly(dy * _cam2.Radius);
                    }
                    else
                    {
                        _cam2.Pitch(dy);
                        _cam2.Yaw(-dx);
                    }
                }
            }
            _lastMousePos = e.Location;
        }

        protected override void OnMouseWheel(object sender, MouseEventArgs e)
        {
            int zoom = -(e.Delta) * SystemInformation.MouseWheelScrollLines / 120;

            if (!_useFpsCamera)
            {
                if (Util.IsKeyDown(Keys.LShiftKey))
                {
                    _cam2.Zoom(0.005f * zoom);
                }
                else if (Util.IsKeyDown(Keys.LControlKey))
                {
                    _cam2.Zoom(0.5f * zoom);
                }
                else
                {
                    _cam2.Zoom(_LookAtZoomSpeed * zoom);
                }
            }
            else
                _cam.Zoom(0.10f * zoom);

        }

        private void BuildGeometry()
        {
            Matrix rotateX = Matrix.RotationX((float)Math.PI * 0.5f);
            foreach (var room in this.rooms)
            {
                foreach (KeyValuePair<long, List<AssetInstance>> kvp in room.InstancesByAssetId)
                {
                    foreach (AssetInstance instance in kvp.Value)
                    {
                        if (instance.Hidden)
                            continue;
                        if (this.models.Keys.Contains(instance.AssetId))
                        {                            
                            GR2 model = this.models[instance.AssetId];
                            foreach (var mesh in model.meshes)
                            {
                                vertices = new List<PosNormalTexTan>();
                                if (mesh.meshName.Contains("collision"))
                                    continue;

                                foreach (var vertex in mesh.meshVerts)
                                {
                                    Vector3 pos = new Vector3(vertex.X, vertex.Y, vertex.Z);

                                    //Vector4 posVec = Vector3.Transform(pos, instance.GetAbsoluteTransform());
                                    //pos = new Vector3(posVec.X, posVec.Y, posVec.Z);

                                    Vector3 norm = new Vector3(vertex.normX, vertex.normY, vertex.normZ);
                                    Vector2 texC = new Vector2(vertex.texU, vertex.texV);
                                    Vector3 tan = new Vector3(vertex.tanX, vertex.tanY, vertex.tanZ);
                                    vertices.Add(new PosNormalTexTan(pos, norm, texC, tan));
                                }
                                var vbd = new BufferDescription(PosNormalTexTan.Stride * vertices.Count, ResourceUsage.Immutable, BindFlags.VertexBuffer, CpuAccessFlags.None, ResourceOptionFlags.None, 0);
                                mesh.meshVertBuff = new Buffer(Device, new DataStream(vertices.ToArray(), false, false), vbd);


                                ushort[] indexArray = mesh.meshVertIndex.Select(GR2_Mesh_Vertex_Index => GR2_Mesh_Vertex_Index.index).ToArray();
                                var ibd = new BufferDescription(sizeof(ushort) * indexArray.Count(), ResourceUsage.Immutable, BindFlags.IndexBuffer, CpuAccessFlags.None, ResourceOptionFlags.None, 0);
                                mesh.meshIdxBuff = new Buffer(Device, new DataStream(indexArray, false, false), ibd);
                            }

                            if (model.attachedModels.Count() > 0)
                            {
                                foreach (var attachModel in model.attachedModels)
                                {
                                    foreach (var attachMesh in attachModel.meshes)
                                    {
                                        vertices = new List<PosNormalTexTan>();
                                        if (attachMesh.meshName.Contains("collision"))
                                            continue;
                                        foreach (var vertex in attachMesh.meshVerts)
                                        {
                                            Vector3 pos = new Vector3(vertex.X, vertex.Y, vertex.Z);
                                            if (attachModel.attachMatrix != blankMatrix)
                                            {
                                                Vector4 posVec = Vector3.Transform(pos, attachModel.attachMatrix);
                                                pos = new Vector3(posVec.X, posVec.Y, posVec.Z);
                                            }

                                            if (attachModel.parentPosMatrix != blankMatrix)
                                            {
                                                Vector4 posVec = Vector3.Transform(pos, attachModel.parentPosMatrix);
                                                pos = new Vector3(posVec.X, posVec.Y, posVec.Z);
                                            }

                                            if (attachModel.scaleMatrix != blankMatrix)
                                            {
                                                Vector4 posVec = Vector3.Transform(pos, attachModel.scaleMatrix);
                                                pos = new Vector3(posVec.X, posVec.Y, posVec.Z);
                                            }

                                            if (attachModel.parentRotMatrix != blankMatrix)
                                            {
                                                Vector4 posVec = Vector3.Transform(pos, attachModel.parentRotMatrix);
                                                pos = new Vector3(posVec.X, posVec.Y, posVec.Z);
                                            }

                                            if (attachModel.rotationMatrix != blankMatrix)
                                            {
                                                Vector4 posVec = Vector3.Transform(pos, attachModel.rotationMatrix);
                                                pos = new Vector3(posVec.X, posVec.Y, posVec.Z);
                                            }

                                            if (attachModel.positionMatrix != blankMatrix)
                                            {
                                                Vector4 posVec = Vector3.Transform(pos, attachModel.positionMatrix);
                                                pos = new Vector3(posVec.X, posVec.Y, posVec.Z);
                                            }

                                            Vector3 norm = new Vector3(vertex.normX, vertex.normY, vertex.normZ);
                                            Vector2 texC = new Vector2(vertex.texU, vertex.texV);
                                            Vector3 tan = new Vector3(vertex.tanX, vertex.tanY, vertex.tanZ);
                                            vertices.Add(new PosNormalTexTan(pos, norm, texC, tan));
                                        }
                                        var vbd = new BufferDescription(PosNormalTexTan.Stride * vertices.Count, ResourceUsage.Immutable, BindFlags.VertexBuffer, CpuAccessFlags.None, ResourceOptionFlags.None, 0);
                                        attachMesh.meshVertBuff = new Buffer(Device, new DataStream(vertices.ToArray(), false, false), vbd);


                                        ushort[] indexArray = attachMesh.meshVertIndex.Select(GR2_Mesh_Vertex_Index => GR2_Mesh_Vertex_Index.index).ToArray();
                                        var ibd = new BufferDescription(sizeof(ushort) * indexArray.Count(), ResourceUsage.Immutable, BindFlags.IndexBuffer, CpuAccessFlags.None, ResourceOptionFlags.None, 0);
                                        attachMesh.meshIdxBuff = new Buffer(Device, new DataStream(indexArray, false, false), ibd);
                                    }
                                }
                            }
                        }
                    }
                }
            }
        }
    }
}
