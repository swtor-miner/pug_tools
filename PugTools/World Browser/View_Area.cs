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
    internal class View_Area : D3DPanelApp
    {
        NpcAppearance npcData;
        ItemAppearance itemData;
        GomObject itemObject;
        String fqn;

        Dictionary<string, GR2> models = new Dictionary<string, GR2>();
        Dictionary<string, Stream> resources = new Dictionary<string, Stream>();

        private readonly DirectionalLight[] _dirLights;

        private Matrix _texTransform;
        private Matrix _world;
        private Matrix _view;
        private Matrix _proj;

        private Point _lastMousePos;

        public bool _disposed;
        private GR2_Effect _fx;

        private readonly FpsCamera _cam;
        private readonly LookAtCamera _cam2;
        private bool _useFpsCamera;
        private float _FPScameraSpeed = 1.0f;
        private float _LookAtZoomSpeed = 0.05f;

        private Vector3 globalBoxMin;
        private Vector3 globalBoxMax;
        private Vector3 globalBoxCenter;
        private Vector3 cameraPos;

        private List<PosNormalTexTan> vertices = new List<PosNormalTexTan>();
        private readonly List<FileFormats.GR2_Mesh_Vertex_Index> indexes = new List<FileFormats.GR2_Mesh_Vertex_Index>();
        private readonly List<ushort> indexList = new List<ushort>();

        bool makeScreenshot = false;
        bool rotateModel = false;

#pragma warning disable CS0649
        private Matrix blankMatrix;
#pragma warning restore CS0649

        public Matrix View { get => _view; set => _view = value; }

        public View_Area(IntPtr hInstance, Form form, string panelName = "")
            : base(hInstance, panelName)
        {
            Window = form;
            RenderPanelName = panelName;
            Enable4XMsaa = true;
            ClientHeight = form.Controls.Find(panelName, true).First().Height;
            ClientWidth = form.Controls.Find(panelName, true).First().Width;

            _useFpsCamera = false;
            _cam = new FpsCamera();
            _cam2 = new LookAtCamera();

            _lastMousePos = new Point();

            _world = Matrix.Identity;
            _texTransform = Matrix.Identity;
            View = Matrix.Identity;
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
            this.npcData = null;
            this.itemData = null;
            this.itemObject = null;
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
                        Util.ReleaseCom(ref mat.paletteSRV);
                        Util.ReleaseCom(ref mat.paletteMaskSRV);
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
                                Util.ReleaseCom(ref mat.paletteSRV);
                                Util.ReleaseCom(ref mat.paletteMaskSRV);
                            }
                            attach.Dispose();
                        }
                    }
                    model.Value.Dispose();
                }
                this.models.Clear();
            }
            this.resources.Clear();
            this.vertices.Clear();
            this.indexes.Clear();
            this.indexList.Clear();
            rotateModel = false;
        }

        public void LoadModel(Dictionary<string, GR2> models, Dictionary<string, Stream> resources, GameObject Data = null, GomObject Object = null)
        {
            if (Data != null)
                if (Data.GetType() == typeof(NpcAppearance))
                {
                    this.npcData = (NpcAppearance)Data;
                    this.fqn = this.npcData.Fqn;
                }
                else if (Data.GetType() == typeof(ItemAppearance))
                {
                    this.itemData = (ItemAppearance)Data;
                    this.fqn = this.itemData.Fqn;
                }
            if (Object != null)
            {
                this.itemObject = Object;
                this.fqn = Object.Name;
            }
            if (models != null)
                this.models = models;
            if (resources != null)
                this.resources = resources;

            GR2 focus = new GR2();

            if (npcData != null)
            {
                rotateModel = false;
                if (npcData.NppType == "nppTypeHumanoid")
                {
                    focus = models["appSlotLeg"];
                }
                else if (npcData.NppType == "nppTypeCreature")
                {
                    focus = models["appSlotCreature"];
                }
                else
                {
                    //Console.WriteLine("pause");
                }
            }
            else if (itemData != null && models.Count > 0)
            {
                rotateModel = false;
                focus = models.First().Value;
            }
            else if (itemObject != null && models.Count > 0)
            {
                if (itemObject.Name.StartsWith("itm."))
                {
                    //rotateModel = true;
                }
                focus = models.First().Value;
            }
            else if (models.Count > 0)
            {
                focus = models.First().Value;
            }

            this.globalBoxMin = new Vector3(focus.global_box.minX, focus.global_box.minY, focus.global_box.minZ);
            this.globalBoxMax = new Vector3(focus.global_box.maxX, focus.global_box.maxY, focus.global_box.maxZ);
            this.globalBoxCenter = globalBoxMin + (globalBoxMax - globalBoxMin) / 2;
            this.cameraPos = new Vector3(globalBoxCenter.X, globalBoxCenter.Y, (globalBoxCenter.Z + 1.0f));

            this._useFpsCamera = false;
            this._cam.Reset();
            this._cam.Position = cameraPos;

            this._cam2.Reset();
            this._cam2.Position = cameraPos;
            this._cam2.LookAt(cameraPos, globalBoxCenter, Vector3.UnitY);

            foreach (var model in models)
            {
                if (model.Value.numMaterials > 0)
                {
                    foreach (GR2_Material mat in model.Value.materials)
                    {
                        mat.ParseMAT(Device);
                    }
                }

                if (model.Value.attachedModels.Count() > 0)
                {
                    foreach (var attachModel in model.Value.attachedModels)
                    {
                        if (attachModel.numMaterials > 0)
                        {
                            foreach (GR2_Material attachMat in attachModel.materials)
                            {
                                attachMat.ParseMAT(Device, model.Value.materials);
                            }
                        }
                    }
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
                            Util.ReleaseCom(ref mat.paletteSRV);
                            Util.ReleaseCom(ref mat.paletteMaskSRV);
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
                                    Util.ReleaseCom(ref mat.paletteSRV);
                                    Util.ReleaseCom(ref mat.paletteMaskSRV);
                                }
                                attach.Dispose();
                            }
                        }
                        model.Value.Dispose();
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
                    _FPScameraSpeed = 15.0f;
                    _LookAtZoomSpeed = 0.20f;
                }

                if (Util.IsKeyDown(Keys.OemMinus))
                {
                    _FPScameraSpeed = 2.5f;
                    _LookAtZoomSpeed = 0.05f;
                }

                if (Util.IsKeyDown(Keys.W) && _useFpsCamera)
                    if (Util.IsKeyDown(Keys.LShiftKey))
                        _cam.Walk(-0.2f * dt);
                    else if (Util.IsKeyDown(Keys.LControlKey))
                        _cam.Walk(-30.0f * dt);
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
            Matrix viewProj;

            if (_useFpsCamera)
            {
                _cam.UpdateViewMatrix();
                _ = _cam.View;
                _ = _cam.Proj;
                viewProj = _cam.ViewProj;
                _fx.SetEyePosW(_cam.Position);
            }
            else
            {
                _cam2.UpdateViewMatrix();
                _ = _cam2.View;
                _ = _cam2.Proj;
                viewProj = _cam2.ViewProj;
                _fx.SetEyePosW(_cam2.Position);
            }

            _fx.SetDirLights(_dirLights);
            _fx.SetEyePosW(_cam.Position);

            var activeTech = _fx.Light1Garment;

            if (Form.ActiveForm != null)
            {
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
                }

                if (Util.IsKeyDown(Keys.C))
                {
                    ImmediateContext.Rasterizer.State = RenderStates.WireframeNoneRS;
                }

                if (Util.IsKeyDown(Keys.PrintScreen))
                {
                    makeScreenshot = true;
                }
            }

            var world = _world;
            var wit = MathF.InverseTranspose(world);
            var wvp = world * viewProj;

            _fx.SetWorld(world);
            _fx.SetWorldInvTranspose(wit);
            _fx.SetWorldViewProj(wvp);
            _fx.SetTexTransform(_texTransform);

            foreach (var model in models)
            {
                if (model.Value.enabled == false)
                    continue;
                foreach (var mesh in model.Value.meshes)
                {
                    if (mesh.meshName.Contains("collision"))
                        continue;

                    foreach (FileFormats.GR2_Mesh_Piece piece in mesh.meshPieces)
                    {
                        ImmediateContext.InputAssembler.SetVertexBuffers(0, new VertexBufferBinding(mesh.meshVertBuff, PosNormalTexTan.Stride, 0));
                        ImmediateContext.InputAssembler.SetIndexBuffer(mesh.meshIdxBuff, Format.R16_UInt, 0);

                        if (piece.matID != -1)
                        {
                            if (model.Value.materials.ElementAtOrDefault(piece.matID) != null)
                            {

                                if (activeTech == _fx.Light1Garment)
                                {
                                    if (model.Value.materials[piece.matID].derived != "Garment" && model.Value.materials[piece.matID].derived != "GarmentScrolling")
                                    {
                                        activeTech = _fx.Light1Tech;
                                    }
                                }
                                _fx.SetDiffuseMap(model.Value.materials[piece.matID].diffuseSRV);
                                _fx.SetGlossMap(model.Value.materials[piece.matID].glossSRV);
                                _fx.SetRotationMap(model.Value.materials[piece.matID].rotationSRV);

                                _fx.SetPaletteMap(model.Value.materials[piece.matID].paletteSRV);
                                _fx.SetPaletteMaskMap(model.Value.materials[piece.matID].paletteMaskSRV);

                                _fx.SetPalette1(model.Value.materials[piece.matID].palette1);
                                _fx.SetPalette2(model.Value.materials[piece.matID].palette2);

                                _fx.SetPalette1Spec(model.Value.materials[piece.matID].palette1Spec);
                                _fx.SetPalette2Spec(model.Value.materials[piece.matID].palette2Spec);

                                _fx.SetPalette1MetSpec(model.Value.materials[piece.matID].palette1MetSpec);
                                _fx.SetPalette2MetSpec(model.Value.materials[piece.matID].palette2MetSpec);
                            }
                        }
                        else
                        {
                            if (model.Value.materials.Count > 0)
                            {
                                _fx.SetDiffuseMap(model.Value.materials[0].diffuseSRV);
                                _fx.SetGlossMap(model.Value.materials[0].glossSRV);
                                _fx.SetRotationMap(model.Value.materials[0].rotationSRV);

                                _fx.SetPaletteMap(model.Value.materials[0].paletteSRV);
                                _fx.SetPaletteMaskMap(model.Value.materials[0].paletteMaskSRV);

                                _fx.SetPalette1(model.Value.materials[0].palette1);
                                _fx.SetPalette2(model.Value.materials[0].palette2);

                                _fx.SetPalette1Spec(model.Value.materials[0].palette1Spec);
                                _fx.SetPalette2Spec(model.Value.materials[0].palette2Spec);

                                _fx.SetPalette1MetSpec(model.Value.materials[0].palette1MetSpec);
                                _fx.SetPalette2MetSpec(model.Value.materials[0].palette2MetSpec);
                            }
                        }

                        activeTech.GetPassByIndex(0).Apply(ImmediateContext);

                        ImmediateContext.DrawIndexed(((int)piece.numPieceFaces) * 3, ((int)piece.startIndex) * 3, 0);
                    }
                }

                if (model.Value.attachedModels.Count() > 0)
                {
                    foreach (var attachModel in model.Value.attachedModels)
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
                                    _fx.SetDiffuseMap(attachModel.materials[attachPiece.matID].diffuseSRV);
                                    _fx.SetGlossMap(attachModel.materials[attachPiece.matID].glossSRV);
                                    _fx.SetRotationMap(attachModel.materials[attachPiece.matID].rotationSRV);

                                    _fx.SetPaletteMap(attachModel.materials[attachPiece.matID].paletteSRV);
                                    _fx.SetPaletteMaskMap(attachModel.materials[attachPiece.matID].paletteMaskSRV);

                                    _fx.SetPalette1(attachModel.materials[attachPiece.matID].palette1);
                                    _fx.SetPalette2(attachModel.materials[attachPiece.matID].palette2);

                                    _fx.SetPalette1Spec(attachModel.materials[attachPiece.matID].palette1Spec);
                                    _fx.SetPalette2Spec(attachModel.materials[attachPiece.matID].palette2Spec);

                                    _fx.SetPalette1MetSpec(attachModel.materials[attachPiece.matID].palette1MetSpec);
                                    _fx.SetPalette2MetSpec(attachModel.materials[attachPiece.matID].palette2MetSpec);
                                }
                                else
                                {
                                    _fx.SetDiffuseMap(attachModel.materials[0].diffuseSRV);
                                    _fx.SetGlossMap(attachModel.materials[0].glossSRV);
                                    _fx.SetRotationMap(attachModel.materials[0].rotationSRV);

                                    _fx.SetPaletteMap(attachModel.materials[0].paletteSRV);
                                    _fx.SetPaletteMaskMap(attachModel.materials[0].paletteMaskSRV);

                                    _fx.SetPalette1(attachModel.materials[0].palette1);
                                    _fx.SetPalette2(attachModel.materials[0].palette2);

                                    _fx.SetPalette1Spec(attachModel.materials[0].palette1Spec);
                                    _fx.SetPalette2Spec(attachModel.materials[0].palette2Spec);

                                    _fx.SetPalette1MetSpec(attachModel.materials[0].palette1MetSpec);
                                    _fx.SetPalette2MetSpec(attachModel.materials[0].palette2MetSpec);
                                }

                                activeTech.GetPassByIndex(0).Apply(ImmediateContext);

                                ImmediateContext.DrawIndexed(((int)attachPiece.numPieceFaces) * 3, ((int)attachPiece.startIndex) * 3, 0);
                            }
                        }
                    }
                }
            }

            SwapChain.Present(0, PresentFlags.None);

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
                ((tor_tools.ModelBrowser)Window).SetStatusLabel("Screenshot Completed");
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
            foreach (var model in models)
            {
                if (rotateModel)
                {
                    Vector4 minVec = new Vector4(model.Value.global_box.minX, model.Value.global_box.minY, model.Value.global_box.minZ, model.Value.global_box.minW);
                    minVec = Vector3.Transform(new Vector3(minVec.X, minVec.Y, minVec.Z), rotateX);
                    model.Value.global_box.minX = minVec.X;
                    model.Value.global_box.minY = minVec.Y;
                    model.Value.global_box.minZ = minVec.Z;

                    Vector4 maxVec = new Vector4(model.Value.global_box.maxX, model.Value.global_box.maxY, model.Value.global_box.maxZ, model.Value.global_box.maxW);
                    maxVec = Vector3.Transform(new Vector3(maxVec.X, maxVec.Y, maxVec.Z), rotateX);
                    model.Value.global_box.maxX = maxVec.X;
                    model.Value.global_box.maxY = maxVec.Y;
                    model.Value.global_box.maxZ = maxVec.Z;
                }

                if (model.Value.rotationMatrix != blankMatrix)
                {
                    Vector4 minVec = new Vector4(model.Value.global_box.minX, model.Value.global_box.minY, model.Value.global_box.minZ, model.Value.global_box.minW);
                    minVec = Vector3.Transform(new Vector3(minVec.X, minVec.Y, minVec.Z), model.Value.rotationMatrix);
                    model.Value.global_box.minX = minVec.X;
                    model.Value.global_box.minY = minVec.Y;
                    model.Value.global_box.minZ = minVec.Z;

                    Vector4 maxVec = new Vector4(model.Value.global_box.maxX, model.Value.global_box.maxY, model.Value.global_box.maxZ, model.Value.global_box.maxW);
                    maxVec = Vector3.Transform(new Vector3(maxVec.X, maxVec.Y, maxVec.Z), model.Value.rotationMatrix);
                    model.Value.global_box.maxX = maxVec.X;
                    model.Value.global_box.maxY = maxVec.Y;
                    model.Value.global_box.maxZ = maxVec.Z;
                }

                if (model.Value.scaleMatrix != blankMatrix)
                {
                    Vector4 minVec = new Vector4(model.Value.global_box.minX, model.Value.global_box.minY, model.Value.global_box.minZ, model.Value.global_box.minW);
                    minVec = Vector3.Transform(new Vector3(minVec.X, minVec.Y, minVec.Z), model.Value.scaleMatrix);
                    model.Value.global_box.minX = minVec.X;
                    model.Value.global_box.minY = minVec.Y;
                    model.Value.global_box.minZ = minVec.Z;

                    Vector4 maxVec = new Vector4(model.Value.global_box.maxX, model.Value.global_box.maxY, model.Value.global_box.maxZ, model.Value.global_box.maxW);
                    maxVec = Vector3.Transform(new Vector3(maxVec.X, maxVec.Y, maxVec.Z), model.Value.scaleMatrix);
                    model.Value.global_box.maxX = maxVec.X;
                    model.Value.global_box.maxY = maxVec.Y;
                    model.Value.global_box.maxZ = maxVec.Z;
                }

                if (model.Value.positionMatrix != blankMatrix)
                {
                    Vector4 minVec = new Vector4(model.Value.global_box.minX, model.Value.global_box.minY, model.Value.global_box.minZ, model.Value.global_box.minW);
                    minVec = Vector3.Transform(new Vector3(minVec.X, minVec.Y, minVec.Z), model.Value.positionMatrix);
                    model.Value.global_box.minX = minVec.X;
                    model.Value.global_box.minY = minVec.Y;
                    model.Value.global_box.minZ = minVec.Z;

                    Vector4 maxVec = new Vector4(model.Value.global_box.maxX, model.Value.global_box.maxY, model.Value.global_box.maxZ, model.Value.global_box.maxW);
                    maxVec = Vector3.Transform(new Vector3(maxVec.X, maxVec.Y, maxVec.Z), model.Value.positionMatrix);
                    model.Value.global_box.maxX = maxVec.X;
                    model.Value.global_box.maxY = maxVec.Y;
                    model.Value.global_box.maxZ = maxVec.Z;
                }

                foreach (var mesh in model.Value.meshes)
                {
                    vertices = new List<PosNormalTexTan>();
                    if (mesh.meshName.Contains("collision"))
                        continue;


                    foreach (var vertex in mesh.meshVerts)
                    {
                        Vector3 pos = new Vector3(vertex.X, vertex.Y, vertex.Z);
                        if (rotateModel)
                        {
                            Vector4 posVec = Vector3.Transform(pos, rotateX);
                            pos = new Vector3(posVec.X, posVec.Y, posVec.Z);
                        }

                        if (model.Value.rotationMatrix != blankMatrix)
                        {
                            Vector4 posVec = Vector3.Transform(pos, model.Value.rotationMatrix);
                            pos = new Vector3(posVec.X, posVec.Y, posVec.Z);
                        }

                        if (model.Value.scaleMatrix != blankMatrix)
                        {
                            Vector4 posVec = Vector3.Transform(pos, model.Value.scaleMatrix);
                            pos = new Vector3(posVec.X, posVec.Y, posVec.Z);
                        }

                        if (model.Value.positionMatrix != blankMatrix)
                        {
                            Vector4 posVec = Vector3.Transform(pos, model.Value.positionMatrix);
                            pos = new Vector3(posVec.X, posVec.Y, posVec.Z);
                        }
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

                if (model.Value.attachedModels.Count() > 0)
                {
                    foreach (var attachModel in model.Value.attachedModels)
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

                                if (attachModel.rotationMatrix != blankMatrix)
                                {
                                    Vector4 posVec = Vector3.Transform(pos, attachModel.rotationMatrix);
                                    pos = new Vector3(posVec.X, posVec.Y, posVec.Z);
                                }

                                if (attachModel.scaleMatrix != blankMatrix)
                                {
                                    Vector4 posVec = Vector3.Transform(pos, attachModel.scaleMatrix);
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

        #region ExportModel
        public void ExportGeometry(string bt)
        {
            if (!this.fqn.StartsWith("ipp."))
                bt = "";
            string path = String.Format("export\\{0}[{1}]", this.fqn, bt);
            string filename = String.Format("{0}\\{1}[{2}].obj", path, this.fqn, bt);
            string mtlFile = String.Format("{0}\\{1}[{2}].mtl", path, this.fqn, bt);

            tor_tools.Tools.WriteFile("", filename, false);

            StringBuilder output = new StringBuilder();

            output.Append("mtllib ").Append(String.Format("{0}[{1}].mtl\ns 1\n", this.fqn, bt));
            tor_tools.Tools.WriteFile("", mtlFile, false);
            int StartIndex = 0;
            foreach (var model in models)
            {
                foreach (var mat in model.Value.materials)
                {
                    tor_tools.Tools.WriteFile("newmtl " + mat.materialName + "\n" +
                    "Kd 1.000000 1.000000 1.000000\n" +
                    "Ks 1.000000 1.000000 1.000000\n" +
                    "Ns 32.000000\n" +
                    "d 1.000000\n" +
                    "map_Ka " + mat.diffuseDDS.Substring(mat.diffuseDDS.LastIndexOf('/') + 1) + "\n" +
                    "map_Kd " + mat.diffuseDDS.Substring(mat.diffuseDDS.LastIndexOf('/') + 1) + "\n" +
                    "map_bump " + mat.rotationDDS.Substring(mat.diffuseDDS.LastIndexOf('/') + 1) + "\n\n"
                    , mtlFile, true);

                    var file = ((tor_tools.ModelBrowser)Window).currentAssets.FindFile(mat.diffuseDDS);
                    if (file != null)
                    {
                        DevIL.ImageImporter imp = new DevIL.ImageImporter();
                        DevIL.ImageExporter exp = new DevIL.ImageExporter();

                        //Load dds
                        DevIL.Image dds;
                        using (Stream diffuseStream = file.OpenCopyInMemory())
                            dds = imp.LoadImageFromStream(DevIL.ImageType.Dds, diffuseStream);

                        var diffuseData = dds.GetImageData(0);

                        var diffuseBM = new System.Drawing.Bitmap(diffuseData.Width, diffuseData.Height, System.Drawing.Imaging.PixelFormat.Format32bppArgb);

                        if (mat.derived == "Garment" || mat.derived == "GarmentScrolling") //Only do palette mapping to Garment materials.
                        {
                            file = ((tor_tools.ModelBrowser)Window).currentAssets.FindFile(mat.paletteMaskDDS);
                            if (file != null)
                            {
                                //load paletteMask
                                using (Stream pMStream = file.OpenCopyInMemory())
                                    dds = imp.LoadImageFromStream(DevIL.ImageType.Dds, pMStream);
                                dds.Resize(diffuseData.Width, diffuseData.Height, diffuseData.Depth, DevIL.SamplingFilter.Bilinear, false);

                                var pMaskData = dds.GetImageData(0);
                                file = ((tor_tools.ModelBrowser)Window).currentAssets.FindFile(mat.paletteDDS);
                                if (file != null)
                                {
                                    //load paletteMap
                                    using (Stream pMStream = file.OpenCopyInMemory())
                                        dds = imp.LoadImageFromStream(DevIL.ImageType.Dds, pMStream);
                                    dds.Resize(diffuseData.Width, diffuseData.Height, diffuseData.Depth, DevIL.SamplingFilter.Bilinear, false);

                                    var pMapData = dds.GetImageData(0);

                                    //manipulate Bitmap
                                    for (int k = 0; k < diffuseData.Height * diffuseData.Width; k++)
                                    {
                                        Color4 diffusePixel = new Color4(diffuseData.Data[k * 4 + 3],
                                            diffuseData.Data[k * 4 + 0],
                                            diffuseData.Data[k * 4 + 1],
                                            diffuseData.Data[k * 4 + 2]);
                                        Color4 maskPixel = new Color4(pMaskData.Data[k * 4 + 3],
                                            pMaskData.Data[k * 4 + 0],
                                            pMaskData.Data[k * 4 + 1],
                                            pMaskData.Data[k * 4 + 2]);
                                        Color4 mapPixel = new Color4(pMapData.Data[k * 4 + 3],
                                            pMapData.Data[k * 4 + 0],
                                            pMapData.Data[k * 4 + 1],
                                            pMapData.Data[k * 4 + 2]);
                                        float sum = (maskPixel.Red + maskPixel.Green) / 255;
                                        //HuePixel
                                        Vector4 palette = mat.palette1;
                                        if (maskPixel.Green > maskPixel.Red)
                                            palette = mat.palette2;
                                        float ambOcc = mapPixel.Red / 255;
                                        float h = mapPixel.Green / 255;
                                        float s = mapPixel.Blue / 255;
                                        float l = mapPixel.Alpha / 255;
                                        //ManipulateHSL
                                        //ExpandHSL
                                        h = (h * (.706f - .3137f)) + .3137f;
                                        h -= .41176f;
                                        s *= .5882f;
                                        l *= .70588f;
                                        //AdujustLightness
                                        l = (float)Math.Pow((double)l, palette.W) * palette.W;
                                        l = palette.Z + ((1 - palette.Z) * l);

                                        //OffsetHSL
                                        //OffsetHue
                                        h = (h + palette.X) % 1;
                                        //OffsetSaturation
                                        s = (float)Math.Pow((double)s, palette.Y);
                                        s *= (1 - palette.Y);
                                        s = (float)Saturate((double)s);

                                        //ManipulateAO
                                        float brightness = palette.Z + 1;
                                        float ret = ambOcc * (brightness + (1 - brightness) * ambOcc);
                                        ret = (float)Saturate((double)ret);

                                        l = (float)l * ret;
                                        l = (float)Saturate((double)l);

                                        Color semiPixel = HSL2RGB(h, s, l);

                                        Color completedPixel = Color.FromArgb(255,
                                            Lerp((byte)diffusePixel.Red, semiPixel.R, sum),
                                            Lerp((byte)diffusePixel.Green, semiPixel.G, sum),
                                            Lerp((byte)diffusePixel.Blue, semiPixel.B, sum));
                                        diffuseBM.SetPixel(k % diffuseData.Width, (int)k / diffuseData.Width, completedPixel);
                                    }
                                }
                            }
                        }
                        else
                        {
                            DevIL.ImageData rotationData = null;
                            file = ((tor_tools.ModelBrowser)Window).currentAssets.FindFile(mat.rotationDDS);
                            if (file != null)
                            {
                                using (Stream rotationStream = file.OpenCopyInMemory())
                                    dds = imp.LoadImageFromStream(DevIL.ImageType.Dds, rotationStream);
                                dds.Resize(diffuseData.Width, diffuseData.Height, diffuseData.Depth, DevIL.SamplingFilter.Bilinear, false);
                                rotationData = dds.GetImageData(0);
                            }

                            for (int k = 0; k < diffuseData.Height * diffuseData.Width; k++)
                            {
                                int p = 255;
                                if (rotationData != null)
                                    p = rotationData.Data[k * 4 + 0];

                                Color diffusePixel = Color.FromArgb(p,
                                            diffuseData.Data[k * 4 + 0],
                                            diffuseData.Data[k * 4 + 1],
                                            diffuseData.Data[k * 4 + 2]);

                                diffuseBM.SetPixel(k % diffuseData.Width, (int)k / diffuseData.Width, diffusePixel);
                            }
                        }

                        //save
                        using (var bmStream = new MemoryStream())
                        {
                            diffuseBM.Save(bmStream, System.Drawing.Imaging.ImageFormat.Bmp); //Bitmap to Stream
                            DevIL.Image bmp = imp.LoadImageFromStream(new MemoryStream(bmStream.GetBuffer())); //Image from Stream
                            exp.SaveImageToStream(bmp, DevIL.ImageType.Bmp, bmStream); //Image to DDS
                            tor_tools.Tools.WriteFile(bmStream, String.Format("{0}\\{1}", path, mat.diffuseDDS.Substring(mat.diffuseDDS.LastIndexOf('/') + 1))); //Save DDS
                        }
                    }
                }
                foreach (var mesh in model.Value.meshes)
                {
                    if (mesh.meshName.Contains("collision"))
                        continue;
                    output.Append(MeshToString(mesh, StartIndex));
                    StartIndex += mesh.meshVerts.Count();
                }

                if (model.Value.attachedModels.Count() > 0)
                {
                    foreach (var attachModel in model.Value.attachedModels)
                    {
                        foreach (var attachMesh in attachModel.meshes)
                        {
                            output.Append(MeshToString(attachMesh, StartIndex));
                            StartIndex += attachMesh.meshVerts.Count();
                        }
                    }
                }
            }
            tor_tools.Tools.WriteFile(output.ToString(), filename, false);
        }

        private static string MeshToString(GR2_Mesh mesh, int StartIndex)
        {
            StringBuilder sb = new StringBuilder();
            sb.Append("g ").Append(mesh.meshName).Append("\n");

            StringBuilder vertices = new StringBuilder();
            StringBuilder normals = new StringBuilder();
            StringBuilder uvs = new StringBuilder();

            foreach (var vertex in mesh.meshVerts)
            {
                vertices.Append(String.Format("v {0} {1} {2}\n", vertex.X, vertex.Y, vertex.Z));
                normals.Append(String.Format("vn {0} {1} {2}\n", -vertex.normX, -vertex.normY, vertex.normZ));
                uvs.Append(String.Format("vt {0} {1}\n", vertex.texU, -vertex.texV));
            }
            sb.Append(vertices).Append("\n")
                .Append(normals).Append("\n")
                .Append(uvs);

            for (int material = 0; material < mesh.meshPieces.Count; material++)
            {
                sb.Append("\n");
                if (mesh.meshPieces[material].matID > -1)
                {
                    sb.Append("usemtl ").Append(mesh._GR2.materials[mesh.meshPieces[material].matID].materialName).Append("\n");
                }
                else
                {
                    sb.Append("usemtl ").Append(mesh._GR2.materials[0].materialName).Append("\n");
                }

                ushort[] triangles = mesh.meshVertIndex.Select(GR2_Mesh_Vertex_Index => GR2_Mesh_Vertex_Index.index).ToArray();
                for (int i = 0; i < triangles.Length; i += 3)
                {
                    sb.Append(string.Format("f {0}/{0}/{0} {1}/{1}/{1} {2}/{2}/{2}\n",
                        triangles[i] + 1 + StartIndex, triangles[i + 1] + 1 + StartIndex, triangles[i + 2] + 1 + StartIndex));
                }
            }
            return sb.ToString();
        }

        private double Saturate(double i)
        {
            if (i > 1.0)
                return 1.0;
            else if (i < 0.0)
                return 0.0;
            return i;
        }

        private int Lerp(byte _a, byte _b, double _t)
        {
            double val = _a + (_b - _a) * _t;
            return (int)(Saturate(val / 255.0) * 255.0);
        }

        public struct ColorRGB
        {
            public byte R;
            public byte G;
            public byte B;
            public ColorRGB(Color value)
            {
                this.R = value.R;
                this.G = value.G;
                this.B = value.B;
            }
            public static implicit operator Color(ColorRGB rgb)
            {
                Color c = Color.FromArgb(255, rgb.R, rgb.G, rgb.B);
                return c;
            }
            public static explicit operator ColorRGB(Color c)
            {
                return new ColorRGB(c);
            }
        }

        public ColorRGB HSL2RGB(double h, double sl, double l)
        {
            double v;
            double r, g, b;

            r = l;   // default to gray
            g = l;
            b = l;
            v = (l <= 0.5) ? (l * (1.0 + sl)) : (l + sl - l * sl);
            if (v > 0)
            {
                double m;
                double sv;
                int sextant;
                double fract, vsf, mid1, mid2;

                m = l + l - v;
                sv = (v - m) / v;
                h *= 6.0;
                sextant = (int)h;
                fract = h - sextant;
                vsf = v * sv * fract;
                mid1 = m + vsf;
                mid2 = v - vsf;
                switch (sextant)
                {
                    case 0:
                        r = v;
                        g = mid1;
                        b = m;
                        break;
                    case 1:
                        r = mid2;
                        g = v;
                        b = m;
                        break;
                    case 2:
                        r = m;
                        g = v;
                        b = mid1;
                        break;
                    case 3:
                        r = m;
                        g = mid2;
                        b = v;
                        break;
                    case 4:
                        r = mid1;
                        g = m;
                        b = v;
                        break;
                    case 5:
                        r = v;
                        g = m;
                        b = mid2;
                        break;
                }
            }
            ColorRGB rgb;
            rgb.R = Convert.ToByte(r * 255.0f);
            rgb.G = Convert.ToByte(Saturate(g) * 255.0f);
            rgb.B = Convert.ToByte(b * 255.0f);
            return rgb;
        }

        public override bool Equals(object obj)
        {
            return obj is View_Area area &&
                   _proj.Equals(area._proj);
        }

        public override string ToString()
        {
            return base.ToString();
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }

        protected override void EndFrame()
        {
            base.EndFrame();
        }

        public override void StopRender()
        {
            base.StopRender();
        }

        public override void StartRender()
        {
            base.StartRender();
        }
        #endregion
    }
}
