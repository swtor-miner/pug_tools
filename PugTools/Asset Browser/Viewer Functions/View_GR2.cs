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

namespace tor_tools
{
    class View_GR2 : D3DPanelApp
    {
        FileFormats.GR2 model;  

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
        private float _FPScameraSpeed = 15.0f;
        private float _LookAtZoomSpeed = 0.40f;

        private Vector3 globalBoxMin;
        private Vector3 globalBoxMax;
        private Vector3 globalBoxCenter;
        private Vector3 cameraPos;

        private List<PosNormalTexTan> vertices = new List<PosNormalTexTan>();
        private List<FileFormats.GR2_Mesh_Vertex_Index> indexes = new List<FileFormats.GR2_Mesh_Vertex_Index>();
        private List<ushort> indexList = new List<ushort>();

        bool makeScreenshot = false;

        public View_GR2(IntPtr hInstance, Form form, string panelName = "") : base(hInstance, panelName)
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

        protected override void Dispose(bool disposing)
        {
            if (!_disposed)
            {                
                if (disposing)
                {
                    if (model != null)
                    {
                        foreach (var mesh in model.meshes)
                        {
                            Util.ReleaseCom(ref mesh.meshIdxBuff);
                            Util.ReleaseCom(ref mesh.meshVertBuff);                            
                        }

                        foreach (var mat in model.materials)
                        {
                            Util.ReleaseCom(ref mat.diffuseSRV);
                            Util.ReleaseCom(ref mat.rotationSRV);
                            Util.ReleaseCom(ref mat.glossSRV);
                            Util.ReleaseCom(ref mat.paletteMaskSRV);
                            Util.ReleaseCom(ref mat.paletteSRV);
                            Util.ReleaseCom(ref mat.complexionSRV);
                            Util.ReleaseCom(ref mat.facepaintSRV);
                            Util.ReleaseCom(ref mat.ageSRV);
                        }
                        model.Dispose();
                    }
                    Effects.DestroyAll();
                    InputLayouts.DestroyAll();
                    RenderStates.DestroyAll();
                    
                    _fx.Dispose();
                    vertices.Clear();
                    indexes.Clear();
                    indexList.Clear();
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

        public void Clear()
        {
            this.stopRender();
            if (model != null)
            {
                foreach (var mesh in model.meshes)
                {
                    Util.ReleaseCom(ref mesh.meshIdxBuff);
                    Util.ReleaseCom(ref mesh.meshVertBuff);
                }

                foreach (var mat in model.materials)
                {
                    Util.ReleaseCom(ref mat.diffuseSRV);
                    Util.ReleaseCom(ref mat.rotationSRV);
                    Util.ReleaseCom(ref mat.glossSRV);
                    Util.ReleaseCom(ref mat.paletteMaskSRV);
                    Util.ReleaseCom(ref mat.paletteSRV);
                    Util.ReleaseCom(ref mat.complexionSRV);
                    Util.ReleaseCom(ref mat.facepaintSRV);
                    Util.ReleaseCom(ref mat.ageSRV);
                }                
                model.Dispose();
            }            
            vertices.Clear();
            indexes.Clear();
            indexList.Clear();
        }

        public void LoadModel(FileFormats.GR2 model = null)
        {
            if (model != null)
                this.model = model;
            globalBoxMin = new Vector3(model.global_box.minX, model.global_box.minY, model.global_box.minZ);
            globalBoxMax = new Vector3(model.global_box.maxX, model.global_box.maxY, model.global_box.maxZ);
            globalBoxCenter = globalBoxMin + (globalBoxMax - globalBoxMin) / 2;
            cameraPos = new Vector3(globalBoxCenter.X, globalBoxCenter.Y, (globalBoxCenter.Z + 1.0f));

            this._useFpsCamera = false;
            this._cam.Reset();
            this._cam.Position = cameraPos;

            this._cam2.Reset();
            this._cam2.Position = cameraPos;
            this._cam2.LookAt(cameraPos, globalBoxCenter, Vector3.UnitY);

            //this.OnResize();

            if (model.numMaterials > 0)
            {
                foreach (GR2_Material mat in model.materials)
                {
                    mat.ParseMAT(Device);
                }
            }
            BuildGeometry();        
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
                    _LookAtZoomSpeed = 0.40f;
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

            var activeTech = _fx.Light2Tech;

            if (Form.ActiveForm != null)
            {
                if (Util.IsKeyDown(Keys.C))
                {
                    ImmediateContext.Rasterizer.State = RenderStates.WireframeNoneRS;
                }

                if (Util.IsKeyDown(Keys.PrintScreen))
                {
                    makeScreenshot = true;
                }

                if (Util.IsKeyDown(Keys.D1))
                {
                    activeTech = _fx.Light1UberDiffuse;
                }

                if (Util.IsKeyDown(Keys.D2))
                {
                    activeTech = _fx.Light1UberEmissive;
                }

                if (Util.IsKeyDown(Keys.D3))
                {
                    activeTech = _fx.Light1UberAmbient;
                }
                if (Util.IsKeyDown(Keys.D4))
                {
                    activeTech = _fx.Light1UberSpecular;
                }
            }

            _fx.SetDirLights(_dirLights);
            _fx.SetEyePosW(_cam.Position);

            var world = _world;
            var wit = MathF.InverseTranspose(world);
            var wvp = world * viewProj;

            _fx.SetWorld(world);
            _fx.SetWorldInvTranspose(wit);
            _fx.SetWorldViewProj(wvp);
            _fx.SetTexTransform(_texTransform);
            
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
                        _fx.SetDiffuseMap(model.materials[piece.matID].diffuseSRV);
                        _fx.SetGlossMap(model.materials[piece.matID].glossSRV);
                        _fx.SetRotationMap(model.materials[piece.matID].rotationSRV);
                    }
                    else
                    {

                    }
                  
                    activeTech.GetPassByIndex(0).Apply(ImmediateContext);
                        
                    ImmediateContext.DrawIndexed(((int)piece.numPieceFaces)*3, ((int)piece.startIndex)*3, 0);                                                
                }                
            }
            SwapChain.Present(0, PresentFlags.None);

            if (makeScreenshot)
            {
                this.MakeScreenshot(ImageFileFormat.Png);
                makeScreenshot = false;
            }
        }

        public void MakeScreenshot(SlimDX.Direct3D11.ImageFileFormat format)
        {
            try
            {
                string filename = tor_tools.Tools.PrepExtractPath(this.model.filename + '-' + DateTime.Now.ToString("yyyyMMddHHmmss") + '.' + format.ToString().ToLower());
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
                ((tor_tools.AssetBrowser)Window).setStatusLabel("Screenshot Completed");
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
                    if(Util.IsKeyDown(Keys.LShiftKey))
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
                    _cam2.Zoom(1.0f * zoom);
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
            if (model.numMeshes > 0)
            {
                foreach (var mesh in model.meshes)
                {
                    vertices = new List<PosNormalTexTan>();
                    if (mesh.meshName.Contains("collision"))
                        continue;
                    foreach (var vertex in mesh.meshVerts)
                    {
                        Vector3 pos = new Vector3(vertex.X, vertex.Y, vertex.Z);
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
            }
        }      
    }
}
