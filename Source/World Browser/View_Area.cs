using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.IO;
using System.Drawing;
using System.Windows.Forms;
using SlimDX;
using SlimDX.Direct3D11;
using SlimDX.DXGI;
using Buffer = SlimDX.Direct3D11.Buffer;
using SlimDXNet;
using SlimDXNet.Camera;
using SlimDXNet.FX;
using SlimDXNet.Vertex;
using FileFormats;

namespace PugTools
{
    class View_AREA : D3DPanelApp
    {
        string fqn;
        Dictionary<ulong, GR2> models = new Dictionary<ulong, GR2>();
        readonly Dictionary<string, Stream> resources = new Dictionary<string, Stream>();
        Dictionary<string, GR2_Material> materials = new Dictionary<string, GR2_Material>();
        List<Room> rooms = new List<Room>();
        private Point lastMousePos;
        public bool _disposed;
        private GR2_Effect _fx;
        private readonly FpsCamera camera;
        private float cameraSpeed = 1.0f;
        private readonly List<string> ignoreList = new List<string>
        {
            "collision", "dbo", "fadeportal", "occluder"
        };
        private List<PosNormalTexTan> vertices = new List<PosNormalTexTan>();
        private readonly List<GR2_Mesh_Vertex_Index> indexes = new List<GR2_Mesh_Vertex_Index>();
        private readonly List<ushort> indexList = new List<ushort>();
        bool makeScreenshot = false;

        public View_AREA(IntPtr hInstance, Form form, string panelName = "")
            : base(hInstance, panelName)
        {
            Window = form;
            RenderPanelName = panelName;
            Enable4XMsaa = true;
            ClientHeight = form.Controls.Find(panelName, true).First().Height;
            ClientWidth = form.Controls.Find(panelName, true).First().Width;

            camera = new FpsCamera();

            lastMousePos = new Point();
        }

        public void Clear()
        {
            if (models != null)
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
                models.Clear();
            }
            if (materials != null)
            {
                foreach (var mat in materials)
                {
                    Util.ReleaseCom(ref mat.Value.diffuseSRV);
                    Util.ReleaseCom(ref mat.Value.rotationSRV);
                    Util.ReleaseCom(ref mat.Value.glossSRV);
                }
                materials.Clear();
            }
            resources.Clear();
            vertices.Clear();
            indexes.Clear();
            indexList.Clear();
        }

        public void LoadModel(Dictionary<ulong, GR2> models,
                              Dictionary<string, GR2_Material> materials,
                              List<Room> rooms,
                              string fqn)
        {
            this.fqn = fqn;

            if (models != null)
                this.models = models;
            if (materials != null)
                this.materials = materials;
            if (rooms != null)
                this.rooms = rooms;

            camera.Reset();
            camera.Position = new Vector3(0.0f, 0.18f, 0.0f);

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

                    if (materials.Count > 0)
                    {
                        foreach (var mat in materials)
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
            camera.SetLens(0.25f * MathF.PI, AspectRatio, 0.001f, 1000.0f);
        }

        public override void UpdateScene(float dt)
        {
            base.UpdateScene(dt);

            if (Form.ActiveForm != null)
            {
                if (Util.IsKeyDown(Keys.R))
                {
                    camera.Reset();
                    camera.Position = new Vector3(0.0f, 0.18f, 0.0f);
                }

                if (Util.IsKeyDown(Keys.Oemplus))
                    cameraSpeed = 60.0f;


                if (Util.IsKeyDown(Keys.OemMinus))
                    cameraSpeed = 30.0f;


                if (Util.IsKeyDown(Keys.W)) // && _useFpsCamera)
                    if (Util.IsKeyDown(Keys.LShiftKey))
                        camera.Walk(-30f * dt);
                    else if (Util.IsKeyDown(Keys.LControlKey))
                        camera.Walk(-120.0f * dt);
                    else
                        camera.Walk(-cameraSpeed * dt);

                if (Util.IsKeyDown(Keys.S)) // && _useFpsCamera)
                    if (Util.IsKeyDown(Keys.LShiftKey))
                        camera.Walk(0.2f * dt);
                    else if (Util.IsKeyDown(Keys.LControlKey))
                        camera.Walk(30.0f * dt);
                    else
                        camera.Walk(cameraSpeed * dt);

                if (Util.IsKeyDown(Keys.A)) // && _useFpsCamera)
                    if (Util.IsKeyDown(Keys.LShiftKey))
                        camera.Strafe(-0.2f * dt);
                    else if (Util.IsKeyDown(Keys.LControlKey))
                        camera.Strafe(-30.0f * dt);
                    else
                        camera.Strafe(-cameraSpeed * dt);

                if (Util.IsKeyDown(Keys.D)) // && _useFpsCamera)
                    if (Util.IsKeyDown(Keys.LShiftKey))
                        camera.Strafe(0.2f * dt);
                    else if (Util.IsKeyDown(Keys.LControlKey))
                        camera.Strafe(30.0f * dt);
                    else
                        camera.Strafe(cameraSpeed * dt);

                if (Util.IsKeyDown(Keys.PageUp))
                    camera.Zoom(-dt);

                if (Util.IsKeyDown(Keys.PageDown))
                    camera.Zoom(+dt);

            }
            Thread.Sleep(1); //Fix for UI lag. Sleeps the thread for 1 millisecond...
        }

        public override void DrawScene()
        {
            base.DrawScene();

            ImmediateContext.ClearRenderTargetView(RenderTargetView, Color.LightSteelBlue);
            ImmediateContext.ClearDepthStencilView(DepthStencilView,
                                                   DepthStencilClearFlags.Depth | DepthStencilClearFlags.Stencil,
                                                   1.0f,
                                                   0);

            ImmediateContext.InputAssembler.InputLayout = InputLayouts.PosNormalTexTan;
            ImmediateContext.InputAssembler.PrimitiveTopology = PrimitiveTopology.TriangleList;

            ImmediateContext.OutputMerger.BlendState = RenderStates.TransparentBS;
            // ImmediateContext.OutputMerger.BlendFactor = new Color4(0, 0, 0, 0);
            // ImmediateContext.OutputMerger.BlendSampleMask = ~0;

            ImmediateContext.Rasterizer.State = RenderStates.TwoSidedRS; // OneSidedRS;

            Matrix cMatrix;
            Matrix pMatrix;

            camera.UpdateViewMatrix();
            cMatrix = camera.View;
            pMatrix = camera.Proj;

            var activeTech = _fx.Generic;

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

            foreach (var room in rooms)
            {
                foreach (KeyValuePair<ulong, List<AssetInstance>> kvp in room.InstancesByAssetId)
                {
                    foreach (AssetInstance instance in kvp.Value)
                    {
                        if (instance.hidden) continue;

                        var mvMatrix = new Matrix();
                        mvMatrix = instance.GetAbsoluteTransform(room);
                        Matrix.Multiply(ref mvMatrix, ref cMatrix, out mvMatrix);

                        var wvp = new Matrix();
                        Matrix.Multiply(ref mvMatrix, ref pMatrix, out wvp);
                        Matrix.Invert(ref mvMatrix, out mvMatrix);
                        Matrix.Transpose(ref mvMatrix, out mvMatrix);

                        _fx.SetWorldMatrix(mvMatrix);
                        _fx.SetMvMatrix(wvp);

                        /*
                        if (instance.hasHeightMap)
                        {
                            ImmediateContext.InputAssembler.SetVertexBuffers(0, new VertexBufferBinding(instance.VBO, VertexPT.Stride, 0));
                            ImmediateContext.InputAssembler.SetIndexBuffer(instance.IBO, Format.R16_UInt, 0);

                            activeTech.GetPassByIndex(0).Apply(ImmediateContext);

                            ImmediateContext.DrawIndexed(instance.numFaces / 3, 0, 0);
                        }
                        */


                        if (models.Keys.Contains(instance.assetID))
                        {
                            GR2 model = models[instance.assetID];

                            if (!model.enabled) continue;

                            foreach (var mesh in model.meshes)
                            {
                                // if (mesh.meshName.Contains("collision")) continue;

                                foreach (GR2_Mesh_Piece piece in mesh.meshPieces)
                                {
                                    ImmediateContext.InputAssembler.SetVertexBuffers(0, new VertexBufferBinding(mesh.meshVertBuff, PosNormalTexTan.Stride, 0));
                                    ImmediateContext.InputAssembler.SetIndexBuffer(mesh.meshIdxBuff, Format.R16_UInt, 0);

                                    if (piece.matID != -1)
                                    {
                                        if (model.materials.ElementAtOrDefault(piece.matID) != null)
                                        {
                                            string matName = model.materials[piece.matID].materialName;
                                            if (materials.Keys.Contains(matName))
                                            {
                                                GR2_Material material = materials[matName];
                                                if (material != null)
                                                {
                                                    SetMaterial(material);
                                                }
                                            }
                                        }
                                    }
                                    else
                                    {
                                        if (model.materials.Count > 0)
                                        {
                                            string matName = model.materials[0].materialName;
                                            if (materials.Keys.Contains(matName))
                                            {
                                                GR2_Material material = materials[matName];
                                                if (material != null)
                                                {
                                                    SetMaterial(material);
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
                                    if (!attachModel.enabled) continue;

                                    foreach (var attachMesh in attachModel.meshes)
                                    {
                                        // if (attachMesh.meshName.Contains("collision")) continue;

                                        foreach (GR2_Mesh_Piece attachPiece in attachMesh.meshPieces)
                                        {
                                            ImmediateContext.InputAssembler.SetVertexBuffers(0, new VertexBufferBinding(attachMesh.meshVertBuff, PosNormalTexTan.Stride, 0));
                                            ImmediateContext.InputAssembler.SetIndexBuffer(attachMesh.meshIdxBuff, Format.R16_UInt, 0);

                                            if (attachPiece.matID != -1)
                                            {
                                                string matName = attachModel.materials[attachPiece.matID].materialName;
                                                if (materials.Keys.Contains(matName))
                                                {
                                                    GR2_Material material = materials[matName];
                                                    if (material != null)
                                                    {
                                                        SetMaterial(material);
                                                    }
                                                }
                                            }
                                            else
                                            {
                                                string matName = attachModel.materials[0].materialName;
                                                if (materials.Keys.Contains(matName))
                                                {
                                                    GR2_Material material = materials[matName];
                                                    if (material != null)
                                                    {
                                                        SetMaterial(material);
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
                MakeScreenshot(ImageFileFormat.Jpg);
                makeScreenshot = false;
            }

        }

        public void SetMaterial(GR2_Material material)
        {
            switch (material.alphaMode)
            {
                case "Test":
                    _fx.SetAlphaMode(1);
                    break;
                case "Add":
                    _fx.SetAlphaMode(2);
                    break;
                case "Multiply":
                    _fx.SetAlphaMode(3);
                    break;
                case "Full":
                    _fx.SetAlphaMode(4);
                    break;
                case "MultiPassFull":
                    _fx.SetAlphaMode(4);
                    break;
                default:
                    _fx.SetAlphaMode(0);
                    break;
            }

            _fx.SetAlphaTestValue(material.alphaTestValue);

            // if (material.isTwoSided && !Util.IsKeyDown(Keys.C))
            //     ImmediateContext.Rasterizer.State = RenderStates.TwoSidedRS;

            _fx.SetDiffuseMap(material.diffuseSRV);
            _fx.SetRotationMap(material.rotationSRV);
            _fx.SetGlossMap(material.glossSRV);
        }

        public void MakeScreenshot(ImageFileFormat format)
        {
            try
            {
                string filename = Tools.PrepExtractPath(fqn + '-' + DateTime.Now.ToString("yyyyMMddHHmmss") + '.' + format.ToString().ToLower());
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
                ((WorldBrowser)Window).SetStatusLabel("Screenshot Completed");
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }
        }

        protected override void OnMouseDown(object sender, MouseEventArgs mouseEventArgs)
        {
            lastMousePos = mouseEventArgs.Location;
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
                var dy = MathF.ToRadians(0.4f * (e.Y - lastMousePos.Y));
                var dx = -MathF.ToRadians(0.4f * (e.X - lastMousePos.X));
                camera.Pitch(-dy);
                camera.Yaw(dx);
            }
            lastMousePos = e.Location;
        }

        protected override void OnMouseWheel(object sender, MouseEventArgs e)
        {
            int zoom = -e.Delta * SystemInformation.MouseWheelScrollLines / 120;
            camera.Zoom(0.10f * zoom);
        }

        private void BuildGeometry()
        {
            Matrix rotateX = Matrix.RotationX((float)Math.PI * 0.5f);
            foreach (var room in rooms)
            {
                foreach (KeyValuePair<ulong, List<AssetInstance>> kvp in room.InstancesByAssetId)
                {
                    foreach (AssetInstance instance in kvp.Value)
                    {
                        if (instance.hidden) continue;

                        if (instance.hasHeightMap)
                        {
                            instance.VBO = new Buffer(Device, instance.VDS, instance.VBD);
                            instance.IBO = new Buffer(Device, instance.IDS, instance.IBD);
                        }

                        if (models.Keys.Contains(instance.assetID))
                        {
                            GR2 model = models[instance.assetID];

                            foreach (var mesh in model.meshes)
                            {
                                if (ignoreList.Any(x => mesh.meshName.Contains(x))) continue;

                                vertices = new List<PosNormalTexTan>();

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
                                        if (ignoreList.Any(x => attachMesh.meshName.Contains(x))) continue;

                                        vertices = new List<PosNormalTexTan>();

                                        foreach (var vertex in attachMesh.meshVerts)
                                        {
                                            Vector3 pos = new Vector3(vertex.X, vertex.Y, vertex.Z);
                                            if (attachModel.attachMatrix != new Matrix())
                                            {
                                                Vector4 posVec = Vector3.Transform(pos, attachModel.attachMatrix);
                                                pos = new Vector3(posVec.X, posVec.Y, posVec.Z);
                                            }

                                            if (attachModel.parentPosMatrix != new Matrix())
                                            {
                                                Vector4 posVec = Vector3.Transform(pos, attachModel.parentPosMatrix);
                                                pos = new Vector3(posVec.X, posVec.Y, posVec.Z);
                                            }

                                            if (attachModel.scaleMatrix != new Matrix())
                                            {
                                                Vector4 posVec = Vector3.Transform(pos, attachModel.scaleMatrix);
                                                pos = new Vector3(posVec.X, posVec.Y, posVec.Z);
                                            }

                                            if (attachModel.parentRotMatrix != new Matrix())
                                            {
                                                Vector4 posVec = Vector3.Transform(pos, attachModel.parentRotMatrix);
                                                pos = new Vector3(posVec.X, posVec.Y, posVec.Z);
                                            }

                                            if (attachModel.rotationMatrix != new Matrix())
                                            {
                                                Vector4 posVec = Vector3.Transform(pos, attachModel.rotationMatrix);
                                                pos = new Vector3(posVec.X, posVec.Y, posVec.Z);
                                            }

                                            if (attachModel.positionMatrix != new Matrix())
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
