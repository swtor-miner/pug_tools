using FileFormats;
using SlimDX;
using SlimDX.Direct3D11;
using SlimDX.DXGI;
using SlimDXNet;
using SlimDXNet.Camera;
using SlimDXNet.FX;
using SlimDXNet.Vertex;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using Buffer = SlimDX.Direct3D11.Buffer;

namespace PugTools
{
    class View_GR2 : D3DPanelApp
    {
        public bool _disposed;
        private readonly FpsCamera _flyingCamera;
        private float _flyingCameraSpeed = 15.0f;
        private GR2_Effect _fx;
        private Point _lastMousePos;
        private float _modelCameraZoomSpeed = 0.40f;
        private readonly LookAtCamera _modelCamera;
        private bool _useFlyingCamera;

        private Vector3 cameraPos;
        public Matrix cMatrix;
        private Vector3 globalBoxCenter;
        private Vector3 globalBoxMax;
        private Vector3 globalBoxMin;
        private readonly List<ushort> indexList = new List<ushort>();
        private readonly List<GR2_Mesh_Vertex_Index> indices = new List<GR2_Mesh_Vertex_Index>();
        bool makeScreenshot = false;
        GR2 model;
        public Matrix pMatrix;
        private List<PosNormalTexTan> vertices = new List<PosNormalTexTan>();


        public View_GR2(IntPtr hInstance, Form form, string panelName = "") : base(hInstance, panelName)
        {
            Window = form;
            RenderPanelName = panelName;
            Enable4XMsaa = true;
            ClientHeight = form.Controls.Find(panelName, true).First().Height;
            ClientWidth = form.Controls.Find(panelName, true).First().Width;

            _useFlyingCamera = false;

            _flyingCamera = new FpsCamera();
            _modelCamera = new LookAtCamera();

            _lastMousePos = new Point();
        }

        protected override void Dispose(bool disposing)
        {
            if (!_disposed)
            {
                if (disposing)
                {
                    Window = null;
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
                    indices.Clear();
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
            StopRender();
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
            indices.Clear();
            indexList.Clear();
        }

        public void LoadModel(GR2 model = null)
        {
            if (model != null)
                this.model = model;

            globalBoxMin = new Vector3(model.global_box.minX, model.global_box.minY, model.global_box.minZ);
            globalBoxMax = new Vector3(model.global_box.maxX, model.global_box.maxY, model.global_box.maxZ);

            globalBoxCenter = globalBoxMin + (globalBoxMax - globalBoxMin) / 2;
            cameraPos = new Vector3(globalBoxCenter.X * 2.5f, globalBoxCenter.Y * 2.5f, Math.Max(Math.Max(globalBoxMax.X, globalBoxMax.Y), globalBoxMax.Z) * 2.5f);

            _flyingCamera.Reset();
            _flyingCamera.Position = cameraPos;

            _modelCamera.Reset();
            _modelCamera.Position = cameraPos;
            _modelCamera.LookAt(cameraPos, globalBoxCenter, Vector3.UnitY);

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
            _flyingCamera.SetLens(0.25f * MathF.PI, AspectRatio, 0.001f, 1000.0f);
            _modelCamera.SetLens(0.25f * MathF.PI, AspectRatio, 0.001f, 1000.0f);
        }

        public override void UpdateScene(float dt)
        {
            base.UpdateScene(dt);

            if (Form.ActiveForm != null)
            {
                if (Util.IsKeyDown(Keys.R))
                {
                    if (_useFlyingCamera)
                    {
                        _flyingCamera.Reset();
                        _flyingCamera.Position = cameraPos;
                    }
                    else
                    {
                        _modelCamera.Reset();
                        _modelCamera.Position = cameraPos;
                        _modelCamera.LookAt(cameraPos, globalBoxCenter, Vector3.UnitY);
                    }
                }

                if (Util.IsKeyDown(Keys.Oemplus))
                {
                    _flyingCameraSpeed = 15.0f;
                    _modelCameraZoomSpeed = 0.20f;
                }

                if (Util.IsKeyDown(Keys.OemMinus))
                {
                    _flyingCameraSpeed = 2.5f;
                    _modelCameraZoomSpeed = 0.05f;
                }

                if (Util.IsKeyDown(Keys.W) && _useFlyingCamera)
                    if (Util.IsKeyDown(Keys.LShiftKey))
                        _flyingCamera.Walk(-0.2f * dt);
                    else if (Util.IsKeyDown(Keys.LControlKey))
                        _flyingCamera.Walk(-30.0f * dt);
                    else
                        _flyingCamera.Walk(-_flyingCameraSpeed * dt);

                if (Util.IsKeyDown(Keys.S) && _useFlyingCamera)
                    if (Util.IsKeyDown(Keys.LShiftKey))
                        _flyingCamera.Walk(0.2f * dt);
                    else if (Util.IsKeyDown(Keys.LControlKey))
                        _flyingCamera.Walk(30.0f * dt);
                    else
                        _flyingCamera.Walk(_flyingCameraSpeed * dt);

                if (Util.IsKeyDown(Keys.A) && _useFlyingCamera)
                    if (Util.IsKeyDown(Keys.LShiftKey))
                        _flyingCamera.Strafe(-0.2f * dt);
                    else if (Util.IsKeyDown(Keys.LControlKey))
                        _flyingCamera.Strafe(-30.0f * dt);
                    else
                        _flyingCamera.Strafe(-_flyingCameraSpeed * dt);

                if (Util.IsKeyDown(Keys.D) && _useFlyingCamera)
                    if (Util.IsKeyDown(Keys.LShiftKey))
                        _flyingCamera.Strafe(0.2f * dt);
                    else if (Util.IsKeyDown(Keys.LControlKey))
                        _flyingCamera.Strafe(30.0f * dt);
                    else
                        _flyingCamera.Strafe(_flyingCameraSpeed * dt);

                if (Util.IsKeyDown(Keys.L))
                    _useFlyingCamera = false;

                if (Util.IsKeyDown(Keys.F))
                    _useFlyingCamera = true;

                if (Util.IsKeyDown(Keys.PageUp))
                    if (!_useFlyingCamera)
                        _modelCamera.Zoom(-_flyingCameraSpeed * dt);
                    else
                        _flyingCamera.Zoom(-dt);

                if (Util.IsKeyDown(Keys.PageDown))
                    if (!_useFlyingCamera)
                        _modelCamera.Zoom(_flyingCameraSpeed * dt);
                    else
                        _flyingCamera.Zoom(+dt);
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

            ImmediateContext.OutputMerger.BlendState = RenderStates.AlphaToCoverageBS;

            ImmediateContext.Rasterizer.State = RenderStates.OneSidedRS;

            if (_useFlyingCamera)
            {
                _flyingCamera.UpdateViewMatrix();
                cMatrix = _flyingCamera.View;
                pMatrix = _flyingCamera.Proj;
            }
            else
            {
                _modelCamera.UpdateViewMatrix();
                cMatrix = _modelCamera.View;
                pMatrix = _modelCamera.Proj;
            }

            var activeTech = _fx.Generic;

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
                    activeTech = _fx.filterDiffuseMap;
                }

                if (Util.IsKeyDown(Keys.D2))
                {
                    activeTech = _fx.filterSpecular;
                }

                if (Util.IsKeyDown(Keys.D3))
                {
                    activeTech = _fx.filterEmissive;
                }

                // if (Util.IsKeyDown(Keys.D4))
                // {
                //     activeTech = _fx.Light1UberAmbient;
                // }
            }

            var mvMatrix = Matrix.Identity;
            Matrix.Multiply(ref mvMatrix, ref cMatrix, out mvMatrix);

            Matrix.Multiply(ref mvMatrix, ref pMatrix, out var wvp);
            Matrix.Invert(ref mvMatrix, out mvMatrix);
            Matrix.Transpose(ref mvMatrix, out mvMatrix);

            _fx.SetWorldMatrix(mvMatrix);
            _fx.SetMvMatrix(wvp);

            foreach (var mesh in model.meshes)
            {
                if (mesh.meshName.Contains("collision"))
                    continue;

                foreach (GR2_Mesh_Piece piece in mesh.meshPieces)
                {
                    ImmediateContext.InputAssembler.SetVertexBuffers(0, new VertexBufferBinding(mesh.meshVertBuff, PosNormalTexTan.Stride, 0));
                    ImmediateContext.InputAssembler.SetIndexBuffer(mesh.meshIdxBuff, Format.R16_UInt, 0);

                    if (piece.matID != -1)
                    {
                        _fx.SetDiffuseMap(model.materials[piece.matID].diffuseSRV);
                        _fx.SetGlossMap(model.materials[piece.matID].glossSRV);
                        _fx.SetRotationMap(model.materials[piece.matID].rotationSRV);
                    }

                    activeTech.GetPassByIndex(0).Apply(ImmediateContext);

                    ImmediateContext.DrawIndexed(((int)piece.numPieceFaces) * 3, ((int)piece.startIndex) * 3, 0);
                }
            }
            SwapChain.Present(1, PresentFlags.None);

            if (makeScreenshot)
            {
                MakeScreenshot(ImageFileFormat.Png);
                makeScreenshot = false;
            }
        }

        public void MakeScreenshot(ImageFileFormat format)
        {
            try
            {
                string filename = Tools.PrepExtractPath(model.filename + '-' + DateTime.Now.ToString("yyyyMMddHHmmss") + '.' + format.ToString().ToLower());
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
                ((AssetBrowser)Window).SetStatusLabel("Screenshot Completed");
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }
        }

        protected override void OnMouseDown(object sender, MouseEventArgs mEvnt)
        {
            _lastMousePos = mEvnt.Location;
            Window.Controls.Find(RenderPanelName, true).First().Capture = true;
        }

        protected override void OnMouseUp(object sender, MouseEventArgs mEvnt)
        {
            Window.Controls.Find(RenderPanelName, true).First().Capture = true;
        }

        protected override void OnMouseMove(object sender, MouseEventArgs mEvnt)
        {
            if (mEvnt.Button == MouseButtons.Left)
            {
                var yDelta = MathF.ToRadians(0.4f * (mEvnt.Y - _lastMousePos.Y));
                var xDelta = -MathF.ToRadians(0.4f * (mEvnt.X - _lastMousePos.X));

                if (_useFlyingCamera)
                {
                    _flyingCamera.Pitch(-yDelta);
                    _flyingCamera.Yaw(xDelta);
                }
                else
                {
                    if (Util.IsKeyDown(Keys.LShiftKey))
                    {
                        xDelta = MathF.ToRadians(0.05f * (mEvnt.X - _lastMousePos.X));
                        yDelta = MathF.ToRadians(0.05f * (mEvnt.Y - _lastMousePos.Y));
                        _modelCamera.Strafe(-xDelta * _modelCamera.Radius);
                        _modelCamera.Fly(yDelta * _modelCamera.Radius);
                    }
                    else
                    {
                        _modelCamera.Pitch(yDelta);
                        _modelCamera.Yaw(-xDelta);
                    }
                }
            }
            else if (mEvnt.Button == MouseButtons.Right)
            {
                var xDelta = MathF.ToRadians(0.05f * (mEvnt.X - _lastMousePos.X));
                var yDelta = MathF.ToRadians(0.05f * (mEvnt.Y - _lastMousePos.Y));
                if (!_useFlyingCamera)
                {
                    _modelCamera.Strafe(-xDelta * _modelCamera.Radius);
                    _modelCamera.Fly(yDelta * _modelCamera.Radius);
                }
            }

            _lastMousePos = mEvnt.Location;
        }

        protected override void OnMouseWheel(object sender, MouseEventArgs mEvnt)
        {
            int zoom = -mEvnt.Delta * SystemInformation.MouseWheelScrollLines / 120;

            if (!_useFlyingCamera)
            {
                if (Util.IsKeyDown(Keys.LShiftKey))
                {
                    _modelCamera.Zoom(0.005f * zoom);
                }
                else if (Util.IsKeyDown(Keys.LControlKey))
                {
                    _modelCamera.Zoom(0.5f * zoom);
                }
                else
                {
                    _modelCamera.Zoom(_modelCameraZoomSpeed * zoom);
                }
            }
            else
                _flyingCamera.Zoom(0.10f * zoom);

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
