using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Drawing;
using System.Windows.Forms;
using SlimDX;
using SlimDX.Direct3D11;
using SlimDX.DXGI;
using Buffer = SlimDX.Direct3D11.Buffer;
using SlimDX_Framework;
using SlimDX_Framework.Camera;
using SlimDX_Framework.FX;
using SlimDX_Framework.Vertex;
using FileFormats;
using GomLib;

namespace tor_tools
{
    internal class View_NPC_GR2 : D3DPanelApp
    {
        public bool _disposed;
        private readonly FpsCamera _flyingCamera;
        private float _flyingCameraSpeed = 1.0f;
        private GR2_Effect _fx;
        private Point _lastMousePos;
        private readonly LookAtCamera _modelCamera;
        private float _modelCameraSpeed = 0.05f;
        private bool _useFlyingCamera;
        public EffectTechnique activeTech;
        private Vector3 cameraPos;
        public Matrix cMatrix;
        GR2 focus = new GR2();
        string fqn;
        private Vector3 globalBoxMin;
        private Vector3 globalBoxMax;
        private Vector3 globalBoxCenter;
        private readonly List<GR2_Mesh_Vertex_Index> indexes = new List<GR2_Mesh_Vertex_Index>();
        private readonly List<ushort> indexList = new List<ushort>();
        bool makeScreenshot = false;
        Dictionary<string, GR2> models = new Dictionary<string, GR2>();
        public Matrix pMatrix;
        Dictionary<string, object> resources = new Dictionary<string, object>();
        public string selectedModel = "";
        private List<PosNormalTexTan> vertices = new List<PosNormalTexTan>();

        public View_NPC_GR2(IntPtr hInstance, Form form, string panelName = "")
            : base(hInstance, panelName)
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
                        Util.ReleaseCom(ref mat.paletteSRV);
                        Util.ReleaseCom(ref mat.paletteMaskSRV);
                        Util.ReleaseCom(ref mat.complexionSRV);
                        Util.ReleaseCom(ref mat.facepaintSRV);
                        Util.ReleaseCom(ref mat.ageSRV);
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
                                Util.ReleaseCom(ref mat.complexionSRV);
                                Util.ReleaseCom(ref mat.facepaintSRV);
                                Util.ReleaseCom(ref mat.ageSRV);
                            }
                            attach.Dispose();
                        }
                    }
                    model.Value.Dispose();
                }
                models.Clear();
            }
            resources.Clear();
            vertices.Clear();
            indexes.Clear();
            indexList.Clear();
        }

        public void LoadModel(Dictionary<string, GR2> models, Dictionary<string, object> resources, string fqn, string type = "")
        {
            this.fqn = fqn;

            float fac;

            switch (type)
            {
                case "dyn":
                    fac = 2.25f;
                    break;
                case "itm":
                    fac = 1.75f;
                    break;
                case "mnt":
                    fac = 2.5f;
                    break;
                case "nppTypeHumanoid":
                    fac = 1.4f;
                    break;
                case "nppTypeCreature":
                    fac = 1.4f;
                    break;
                default:
                    fac = 2.0f;
                    break;
            }

            globalBoxCenter = new Vector3();
            globalBoxMax = new Vector3();
            globalBoxMin = new Vector3();

            if (type == "ipp")
            {
                focus = models.First().Value;

                globalBoxMin = new Vector3(focus.global_box.minX, focus.global_box.minY, focus.global_box.minZ);
                Vector4 tempMin = Vector3.Transform(globalBoxMin, focus.GetTransform());
                globalBoxMin = new Vector3(tempMin.X, tempMin.Y, tempMin.Z);

                globalBoxMax = new Vector3(focus.global_box.maxX, focus.global_box.maxY, focus.global_box.maxZ);
                Vector4 tempMax = Vector3.Transform(globalBoxMax, focus.GetTransform());
                globalBoxMax = new Vector3(tempMax.X, tempMax.Y, tempMax.Z);

                globalBoxCenter = globalBoxMin + (globalBoxMax - globalBoxMin) / 2;
                cameraPos = new Vector3(globalBoxCenter.X * 1.85f, globalBoxCenter.Y * 1.85f, Math.Max(Math.Max(globalBoxMax.X, globalBoxMax.Y), globalBoxMax.Z) * 1.85f);
            }
            else
            {
                foreach (KeyValuePair<string, GR2> model in models)
                {
                    if (model.Key.Contains("skeleton"))
                        continue;

                    focus = model.Value;
                    var min = Vector3.Transform(new Vector3(focus.global_box.minX, focus.global_box.minY, focus.global_box.minZ), focus.GetTransform());
                    var max = Vector3.Transform(new Vector3(focus.global_box.maxX, focus.global_box.maxY, focus.global_box.maxZ), focus.GetTransform());

                    globalBoxMin.X = min.X < globalBoxMin.X ? min.X : globalBoxMin.X;
                    globalBoxMin.Y = min.Y < globalBoxMin.Y ? min.Y : globalBoxMin.Y;
                    globalBoxMin.Z = min.Z < globalBoxMin.Z ? min.Z : globalBoxMin.Z;

                    globalBoxMax.X = max.X > globalBoxMax.X ? max.X : globalBoxMax.X;
                    globalBoxMax.Y = max.Y > globalBoxMax.Y ? max.Y : globalBoxMax.Y;
                    globalBoxMax.Z = max.Z > globalBoxMax.Z ? max.Z : globalBoxMax.Z;
                }

                globalBoxCenter = globalBoxMin + (globalBoxMax - globalBoxMin) / 2;
                cameraPos = new Vector3(globalBoxCenter.X * fac, globalBoxCenter.Y * fac, Math.Max(Math.Max(globalBoxMax.X, globalBoxMax.Y), globalBoxMax.Z) * fac);
            }

            if (models != null)
                this.models = models;
            if (resources != null)
                this.resources = resources;

            _modelCamera.Reset();
            _modelCamera.Position = cameraPos;
            _modelCamera.LookAt(cameraPos, globalBoxCenter, Vector3.UnitY);

            _flyingCamera.Reset();
            _flyingCamera.Position = cameraPos;

            foreach (var model in models)
            {
                if (model.Value.materials.Count > 0)
                {
                    foreach (GR2_Material material in model.Value.materials)
                    {
                        material.ParseMAT(Device);
                    }
                }

                if (model.Value.attachedModels.Count() > 0)
                {
                    foreach (var attachModel in model.Value.attachedModels)
                    {
                        if (attachModel.materials.Count > 0)
                        {
                            foreach (GR2_Material attachMaterial in attachModel.materials)
                            {
                                attachMaterial.ParseMAT(Device, model.Value.materials);
                            }
                        }
                    }
                }
            }

            if (resources.Count > 0)
            {
                foreach (var model in models)
                {
                    if (model.Value.materials.Count > 0)
                    {
                        foreach (GR2_Material material in model.Value.materials)
                        {
                            if (material.derived == "HairC")
                            {
                                if (resources.Keys.Contains("appSlotHairColor"))
                                    material.SetDynamicColor((GomObject)resources["appSlotHairColor"]);
                            }

                            if (material.derived == "SkinB")
                            {
                                if (model.Value.filename.Contains("head_"))
                                {
                                    if (resources.Keys.Contains("appSlotFacePaint"))
                                        material.SetFacepaintMap(Device, (string)resources["appSlotFacePaint"]);

                                    if (resources.Keys.Contains("appSlotComplexion"))
                                        material.SetComplexionMap(Device, (string)resources["appSlotComplexion"]);
                                }

                                if (resources.Keys.Contains("appSlotSkinColor"))
                                    material.SetDynamicColor((GomObject)resources["appSlotSkinColor"], 1);
                            }

                            if (material.derived == "Eye")
                            {
                                if (resources.Keys.Contains("appSlotEyeColor"))
                                    material.SetDynamicColor((GomObject)resources["appSlotEyeColor"]);
                            }
                        }
                    }

                    if (model.Value.attachedModels.Count() > 0)
                    {
                        foreach (var attachModel in model.Value.attachedModels)
                        {
                            if (attachModel.materials.Count > 0)
                            {
                                foreach (GR2_Material attachMaterial in attachModel.materials)
                                {
                                    if (attachMaterial.derived == "HairC")
                                    {
                                        if (resources.Keys.Contains("appSlotHairColor"))
                                            attachMaterial.SetDynamicColor((GomObject)resources["appSlotHairColor"]);
                                    }
                                    if (attachMaterial.derived == "SkinB")
                                    {
                                        if (model.Value.filename.Contains("head_"))
                                        {
                                            if (resources.Keys.Contains("appSlotFacePaint"))
                                                attachMaterial.SetFacepaintMap(Device, (string)resources["appSlotFacePaint"]);

                                            if (resources.Keys.Contains("appSlotComplexion"))
                                                attachMaterial.SetComplexionMap(Device, (string)resources["appSlotComplexion"]);
                                        }

                                        if (resources.Keys.Contains("appSlotSkinColor"))
                                            attachMaterial.SetDynamicColor((GomObject)resources["appSlotSkinColor"], 1);
                                    }

                                    if (attachMaterial.derived == "Eye")
                                    {
                                        if (resources.Keys.Contains("appSlotEyeColor"))
                                            attachMaterial.SetDynamicColor((GomObject)resources["appSlotEyeColor"]);
                                    }
                                }
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
                    Window = null;
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
                            Util.ReleaseCom(ref mat.complexionSRV);
                            Util.ReleaseCom(ref mat.facepaintSRV);
                            Util.ReleaseCom(ref mat.ageSRV);
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
                                    Util.ReleaseCom(ref mat.complexionSRV);
                                    Util.ReleaseCom(ref mat.facepaintSRV);
                                    Util.ReleaseCom(ref mat.ageSRV);
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
            _modelCamera.SetLens(0.25f * MathF.PI, AspectRatio, 0.001f, 1000.0f);
            _flyingCamera.SetLens(0.25f * MathF.PI, AspectRatio, 0.001f, 1000.0f);
        }

        public override void UpdateScene(float dt)
        {
            base.UpdateScene(dt);

            if (Form.ActiveForm != null)
            {
                if (Util.IsKeyDown(Keys.R))
                {
                    if (!_useFlyingCamera)
                    {
                        _modelCamera.Reset();
                        _modelCamera.Position = cameraPos;
                        _modelCamera.LookAt(cameraPos, globalBoxCenter, Vector3.UnitY);
                    }
                    else
                    {
                        _flyingCamera.Reset();
                        _flyingCamera.Position = cameraPos;
                    }
                }

                if (Util.IsKeyDown(Keys.Oemplus))
                {
                    _flyingCameraSpeed = 15.0f;
                    _modelCameraSpeed = 0.20f;
                }

                if (Util.IsKeyDown(Keys.OemMinus))
                {
                    _flyingCameraSpeed = 2.5f;
                    _modelCameraSpeed = 0.05f;
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

            if (!_useFlyingCamera)
            {
                _modelCamera.UpdateViewMatrix();
                cMatrix = _modelCamera.View;
                pMatrix = _modelCamera.Proj;
            }
            else
            {
                _flyingCamera.UpdateViewMatrix();
                cMatrix = _flyingCamera.View;
                pMatrix = _flyingCamera.Proj;
            }

            activeTech = _fx.Generic;

            if (Form.ActiveForm != null)
            {
                if (Util.IsKeyDown(Keys.Q))
                {
                    activeTech = _fx.filterPaletteMask;
                }

                if (Util.IsKeyDown(Keys.E))
                {
                    activeTech = _fx.filterDiffuseMap;
                }

                if (Util.IsKeyDown(Keys.D1))
                {
                    activeTech = _fx.filterPalette1;
                }

                if (Util.IsKeyDown(Keys.D2))
                {
                    activeTech = _fx.filterPalette2;
                }

                if (Util.IsKeyDown(Keys.D3))
                {
                    activeTech = _fx.filterPaletteMap;
                }

                if (Util.IsKeyDown(Keys.D4))
                {
                    activeTech = _fx.filterComplexionMap;
                }

                if (Util.IsKeyDown(Keys.D5))
                {
                    activeTech = _fx.filterFacepaintMap;
                }

                if (Util.IsKeyDown(Keys.D6))
                {
                    activeTech = _fx.filterAgeMap;
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

            foreach (var model in models)
            {
                if (model.Value.enabled == false)
                    continue;

                var mvMatrix = new Matrix();
                mvMatrix = model.Value.GetTransform();
                Matrix.Multiply(ref mvMatrix, ref cMatrix, out mvMatrix);

                var wvp = new Matrix();
                Matrix.Multiply(ref mvMatrix, ref pMatrix, out wvp);
                Matrix.Invert(ref mvMatrix, out mvMatrix);
                Matrix.Transpose(ref mvMatrix, out mvMatrix);

                _fx.SetWorld(mvMatrix);
                _fx.SetWorldViewProj(wvp);

                foreach (var mesh in model.Value.meshes)
                {
                    if (mesh.meshName.Contains("collision"))
                        continue;

                    int pieceCount = 0;
                    foreach (GR2_Mesh_Piece piece in mesh.meshPieces)
                    {
                        ImmediateContext.InputAssembler.SetVertexBuffers(0, new VertexBufferBinding(mesh.meshVertBuff, PosNormalTexTan.Stride, 0));
                        ImmediateContext.InputAssembler.SetIndexBuffer(mesh.meshIdxBuff, Format.R16_UInt, 0);

                        if (model.Value.filename.Contains("head_"))
                        {
                            if (model.Value.materials.Count > 0)
                            {
                                if (model.Value.materials.Count == 2 && pieceCount > 0)
                                    SetMaterial(model.Value.materials[1]);
                                else
                                    SetMaterial(model.Value.materials[0]);
                            }
                        }
                        else
                        {
                            if (piece.matID != -1)
                            {
                                if (model.Value.materials.ElementAtOrDefault(piece.matID) != null)
                                    SetMaterial(model.Value.materials[piece.matID]);
                            }
                            else
                            {
                                if (model.Value.materials.Count > 0)
                                {
                                    GR2_Material selectedMaterial;
                                    if (model.Value.materials.Count == 2 && pieceCount > 0)
                                        selectedMaterial = model.Value.materials[1];
                                    else
                                        selectedMaterial = model.Value.materials[0];
                                    SetMaterial(selectedMaterial);
                                }
                            }
                        }

                        activeTech.GetPassByIndex(0).Apply(ImmediateContext);

                        ImmediateContext.DrawIndexed(((int)piece.numPieceFaces) * 3, ((int)piece.startIndex) * 3, 0);
                        pieceCount++;
                    }
                }

                if (model.Value.attachedModels.Count() > 0)
                {
                    foreach (var attachModel in model.Value.attachedModels)
                    {
                        if (!attachModel.enabled)
                            continue;

                        foreach (var attachMesh in attachModel.meshes)
                        {
                            if (attachMesh.meshName.Contains("collision"))
                                continue;

                            foreach (GR2_Mesh_Piece attachPiece in attachMesh.meshPieces)
                            {
                                ImmediateContext.InputAssembler.SetVertexBuffers(0, new VertexBufferBinding(attachMesh.meshVertBuff, PosNormalTexTan.Stride, 0));
                                ImmediateContext.InputAssembler.SetIndexBuffer(attachMesh.meshIdxBuff, Format.R16_UInt, 0);

                                if (attachPiece.matID != -1)
                                    SetMaterial(attachModel.materials[attachPiece.matID]);
                                else
                                    SetMaterial(attachModel.materials[0]);
                                activeTech.GetPassByIndex(0).Apply(ImmediateContext);
                                ImmediateContext.DrawIndexed(((int)attachPiece.numPieceFaces) * 3, ((int)attachPiece.startIndex) * 3, 0);
                            }
                        }
                    }
                }
            }

            SwapChain.Present(1, PresentFlags.None);

            if (makeScreenshot)
            {
                MakeScreenshot(ImageFileFormat.Png);
                makeScreenshot = false;
            }

        }

        public void SetMaterial(GR2_Material selectedMaterial)
        {
            List<EffectTechnique> derivedList = new List<EffectTechnique>() {
        _fx.Generic,
        _fx.Eye,
        _fx.Garment,
        _fx.HairC,
        _fx.SkinB
    };

            if (derivedList.Any(x => activeTech == x))
            {
                switch (selectedMaterial.derived)
                {
                    case "AnimatedUV":
                        activeTech = _fx.Generic;
                        break;
                    case "AnimatedUVAlphaBlend":
                        activeTech = _fx.Generic;
                        break;
                    case "Creature":
                        activeTech = _fx.Generic;
                        break;
                    case "DiffuseFlat":
                        activeTech = _fx.Generic;
                        break;
                    case "EmissiveOnly":
                        activeTech = _fx.Generic;
                        break;
                    case "Eye":
                        activeTech = _fx.Eye;
                        break;
                    case "Garment":
                        activeTech = _fx.Garment;
                        break;
                    case "Glass":
                        activeTech = _fx.Generic;
                        break;
                    case "HairC":
                        activeTech = _fx.HairC;
                        break;
                    case "HighQualityCharacter":
                        activeTech = _fx.Generic;
                        break;
                    case "Ice":
                        activeTech = _fx.Generic;
                        break;
                    case "NoShadeTexFogged":
                        activeTech = _fx.Generic;
                        break;
                    case "OpacityFade":
                        activeTech = _fx.Generic;
                        break;
                    case "SkinB":
                        activeTech = _fx.SkinB;
                        break;
                    case "Skydome":
                        activeTech = _fx.Generic;
                        break;
                    case "Uber":
                        activeTech = _fx.Generic;
                        break;
                    case "UberEnvBlend":
                        activeTech = _fx.Generic;
                        break;
                    case "Vegetation":
                        activeTech = _fx.Generic;
                        break;
                    default:
                        activeTech = _fx.Generic;
                        break;
                }
            }

            // switch (selectedMaterial.polytype)
            // {
            //     case "Ignore":
            //         _fx.SetPolyType(new Vector2(1));
            //         break;
            //     default:
            //         _fx.SetPolyType(new Vector2(0));
            //         break;
            // }

            switch (selectedMaterial.alphaMode)
            {
                case "Test":
                    _fx.SetAlphaMode(0);
                    break;
                case "Add":
                    _fx.SetAlphaMode(4);
                    break;
                case "Multiply":
                    _fx.SetAlphaMode(2);
                    break;
                case "Full":
                    _fx.SetAlphaMode(3);
                    break;
                case "MultiPassFull":
                    _fx.SetAlphaMode(3);
                    break;
                default:
                    _fx.SetAlphaMode(0);
                    break;
            }

            _fx.SetAlphaTestValue(selectedMaterial.alphaTestValue);

            if (selectedMaterial.isTwoSided)
                ImmediateContext.Rasterizer.State = RenderStates.TwoSidedRS;

            _fx.SetDiffuseMap(selectedMaterial.diffuseSRV);
            _fx.SetRotationMap(selectedMaterial.rotationSRV);
            _fx.SetGlossMap(selectedMaterial.glossSRV);
            _fx.SetPaletteMap(selectedMaterial.paletteSRV);
            _fx.SetPaletteMaskMap(selectedMaterial.paletteMaskSRV);

            _fx.SetComplexionMap(selectedMaterial.complexionSRV);
            _fx.SetFacepaintMap(selectedMaterial.facepaintSRV);
            _fx.SetAgeMap(selectedMaterial.ageSRV);

            _fx.SetPalette1(selectedMaterial.palette1);
            _fx.SetPalette2(selectedMaterial.palette2);

            _fx.SetPalette1Spec(selectedMaterial.palette1Spec);
            _fx.SetPalette2Spec(selectedMaterial.palette2Spec);

            _fx.SetPalette1MetSpec(selectedMaterial.palette1MetSpec);
            _fx.SetPalette2MetSpec(selectedMaterial.palette2MetSpec);

            _fx.SetFlushTone(selectedMaterial.flushTone);
            _fx.SetFleshBrightness(selectedMaterial.fleshBrightness);
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
                ((ModelBrowser)Window).SetStatusLabel("Screenshot Completed");
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
                    _modelCamera.Zoom(_modelCameraSpeed * zoom);
                }
            }
            else
                _flyingCamera.Zoom(0.10f * zoom);

        }

        private void BuildGeometry()
        {
            foreach (var model in models)
            {
                foreach (var mesh in model.Value.meshes)
                {
                    vertices = new List<PosNormalTexTan>();
                    if (mesh.meshName.Contains("collision"))
                        continue;

                    foreach (var vertex in mesh.meshVerts)
                    {
                        Vector3 pos = new Vector3(vertex.X, vertex.Y, vertex.Z);
                        Vector3 nor = new Vector3(vertex.normX, vertex.normY, vertex.normZ);
                        Vector2 tex = new Vector2(vertex.texU, vertex.texV);
                        Vector3 tan = new Vector3(vertex.tanX, vertex.tanY, vertex.tanZ);
                        vertices.Add(new PosNormalTexTan(pos, nor, tex, tan));
                    }

                    var vbd = new BufferDescription(PosNormalTexTan.Stride * vertices.Count, ResourceUsage.Immutable, BindFlags.VertexBuffer, CpuAccessFlags.None, ResourceOptionFlags.None, 0);
                    mesh.meshVertBuff = new Buffer(Device, new DataStream(vertices.ToArray(), false, false), vbd);

                    ushort[] indexArray = mesh.meshVertIndex.Select(GR2_Mesh_Vertex_Index => GR2_Mesh_Vertex_Index.index).ToArray();
                    var ibd = new BufferDescription(sizeof(ushort) * indexArray.Count(), ResourceUsage.Immutable, BindFlags.IndexBuffer, CpuAccessFlags.None, ResourceOptionFlags.None, 0);
                    mesh.meshIdxBuff = new Buffer(Device, new DataStream(indexArray, false, false), ibd);
                }

                if (model.Value.attachedModels.Count > 0)
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
            string parsedName;
            if (fqn == null)
            {
                parsedName = models.First().Key;
                bt = "";
            }
            else
            {
                parsedName = fqn;
                if (!fqn.StartsWith("ipp."))
                    bt = "";
            }
            string path = string.Format("export\\{0}[{1}]", parsedName, bt);
            string filename = string.Format("{0}\\{1}[{2}].obj", path, parsedName, bt);
            string mtlFile = string.Format("{0}\\{1}[{2}].mtl", path, parsedName, bt);

            Tools.WriteFile("", filename, false);

            StringBuilder output = new StringBuilder();

            output.Append("mtllib ").Append(string.Format("{0}[{1}].mtl\ns 1\n", parsedName, bt));
            Tools.WriteFile("", mtlFile, false);
            int StartIndex = 0;
            foreach (var model in models)
            {
                foreach (var mat in model.Value.materials)
                {
                    Tools.WriteFile("newmtl " + mat.materialName + "\n" +
                    "Kd 1.000000 1.000000 1.000000\n" +
                    "Ks " + mat.palette1Spec.Y.ToString() + " " + mat.palette1Spec.Z.ToString() + " " + mat.palette1Spec.W.ToString() + "\n" +
                    "Ns 32.000000\n" +
                    "d 1.000000\n" +
                    "illum 2\n" +
                    "map_Ka " + mat.diffuseDDS.Substring(mat.diffuseDDS.LastIndexOf('/') + 1).Replace(".dds", ".png") + "\n" +
                    "map_Kd " + mat.diffuseDDS.Substring(mat.diffuseDDS.LastIndexOf('/') + 1).Replace(".dds", ".png") + "\n" +
                    ((mat.glossDDS != null) ? ("map_Ks " + mat.glossDDS.Substring(mat.glossDDS.LastIndexOf('/') + 1).Replace(".dds", ".png") + "\n" +
                    "map_Ns " + (mat.glossDDS ?? "gloss_s.dds").Substring(mat.glossDDS.LastIndexOf('/') + 1).Replace("_s.", "_si.").Replace(".dds", ".png") + "\n") : "") +
                    ((mat.useEmissive && mat.glossDDS != null) ? "map_Ke " + mat.glossDDS.Substring(mat.glossDDS.LastIndexOf('/') + 1).Replace("_s.", "_emis.").Replace(".dds", ".png") + "\n" : "") +
                    ((mat.rotationDDS != null) ? ("map_Normal " + mat.rotationDDS.Substring(mat.rotationDDS.LastIndexOf('/') + 1)) : "") + "\n\n"
                    , mtlFile, true);

                    var file = ((ModelBrowser)Window).currentAssets.FindFile(mat.diffuseDDS);
                    if (file != null)
                    {
                        DevIL.ImageImporter imp = new DevIL.ImageImporter();
                        DevIL.ImageExporter exp = new DevIL.ImageExporter();

                        //Load dds
                        DevIL.Image dds;
                        using (Stream diffuseStream = file.OpenCopyInMemory())
                            dds = imp.LoadImageFromStream(DevIL.ImageType.Dds, diffuseStream);

                        var diffuseData = dds.GetImageData(0);

                        var diffuseBM = new Bitmap(diffuseData.Width, diffuseData.Height, System.Drawing.Imaging.PixelFormat.Format32bppArgb);
                        var bumpBM = new Bitmap(diffuseData.Width, diffuseData.Height, System.Drawing.Imaging.PixelFormat.Format24bppRgb);
                        var emisBM = new Bitmap(diffuseData.Width, diffuseData.Height, System.Drawing.Imaging.PixelFormat.Format24bppRgb);

                        bool garment = false;
                        if (mat.derived == "Garment" || mat.derived == "GarmentScrolling") //Only do palette mapping to Garment materials.
                        {
                            garment = true;
                            file = ((ModelBrowser)Window).currentAssets.FindFile(mat.paletteMaskDDS);
                            if (file != null)
                            {
                                //load paletteMask
                                using (Stream pMStream = file.OpenCopyInMemory())
                                    dds = imp.LoadImageFromStream(DevIL.ImageType.Dds, pMStream);
                                dds.Resize(diffuseData.Width, diffuseData.Height, diffuseData.Depth, DevIL.SamplingFilter.Bilinear, false);

                                var pMaskData = dds.GetImageData(0);
                                file = ((ModelBrowser)Window).currentAssets.FindFile(mat.paletteDDS);
                                if (file != null)
                                {
                                    // Load paletteMap
                                    using (Stream pMStream = file.OpenCopyInMemory())
                                        dds = imp.LoadImageFromStream(DevIL.ImageType.Dds, pMStream);
                                    dds.Resize(diffuseData.Width, diffuseData.Height, diffuseData.Depth, DevIL.SamplingFilter.Bilinear, false);

                                    var pMapData = dds.GetImageData(0);

                                    // Manipulate Bitmap
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
                                        l = (float)Math.Pow(l, palette.W) * palette.W;
                                        l = palette.Z + ((1 - palette.Z) * l);

                                        //OffsetHSL
                                        //OffsetHue
                                        h = (h + palette.X) % 1;
                                        //OffsetSaturation
                                        s = (float)Math.Pow(s, palette.Y);
                                        s *= 1 - palette.Y;
                                        s = (float)Saturate(s);

                                        //ManipulateAO
                                        float brightness = palette.Z + 1;
                                        float ret = ambOcc * (brightness + (1 - brightness) * ambOcc);
                                        ret = (float)Saturate(ret);

                                        l = (float)l * ret;
                                        l = (float)Saturate(l);

                                        Color semiPixel = HSL2RGB(h, s, l);

                                        Color completedPixel = Color.FromArgb(255,
                                            Lerp((byte)diffusePixel.Red, semiPixel.R, sum),
                                            Lerp((byte)diffusePixel.Green, semiPixel.G, sum),
                                            Lerp((byte)diffusePixel.Blue, semiPixel.B, sum));
                                        diffuseBM.SetPixel(k % diffuseData.Width, k / diffuseData.Width, completedPixel);
                                    }
                                }
                            }
                        }

                        DevIL.ImageData rotationData = null;
                        file = ((ModelBrowser)Window).currentAssets.FindFile(mat.rotationDDS);
                        if (file != null)
                        {
                            using (Stream rotationStream = file.OpenCopyInMemory())
                                dds = imp.LoadImageFromStream(DevIL.ImageType.Dds, rotationStream);
                            dds.Resize(diffuseData.Width, diffuseData.Height, diffuseData.Depth, DevIL.SamplingFilter.Bilinear, false);
                            rotationData = dds.GetImageData(0);
                        }

                        for (int k = 0; k < diffuseData.Height * diffuseData.Width; k++)
                        {
                            if (!garment)
                            {
                                int p = 255;
                                if (rotationData != null && mat.alphaClip)
                                {
                                    if ((rotationData.Data[k * 4 + 0] / 255.0f) > mat.alphaTestValue)
                                        p = rotationData.Data[k * 4 + 0];
                                }

                                Color diffusePixel = Color.FromArgb(p,
                                            diffuseData.Data[k * 4 + 0],
                                            diffuseData.Data[k * 4 + 1],
                                            diffuseData.Data[k * 4 + 2]);

                                diffuseBM.SetPixel(k % diffuseData.Width, k / diffuseData.Width, diffusePixel);
                            }
                            if (rotationData != null)
                            {
                                float x = rotationData.Data[k * 4 + 1] / 255.0f;
                                float y = rotationData.Data[k * 4 + 3] / 255.0f;
                                float sign = (x < 0) ? -1.0f : 1.0f;
                                x = Math.Abs(x) * 2.0f - 1.0f;
                                float z = (float)Math.Sqrt(1.0f - x * x - y * y) * sign;
                                z = (z < 0) ? 0 : (z > 1) ? 1 : float.IsNaN(z) ? 0 : z;
                                Color bumpPixel = Color.FromArgb(255,
                                            rotationData.Data[k * 4 + 1],
                                            rotationData.Data[k * 4 + 3],
                                            Convert.ToByte(z * 255.0f));
                                bumpBM.SetPixel(k % diffuseData.Width, k / diffuseData.Width, bumpPixel);

                                if (mat.useEmissive)
                                {
                                    Color emisPixel = Color.FromArgb(255,
                                                rotationData.Data[k * 4 + 2],
                                                rotationData.Data[k * 4 + 2],
                                                rotationData.Data[k * 4 + 2]);
                                    emisBM.SetPixel(k % diffuseData.Width, k / diffuseData.Width, emisPixel);
                                }
                            }
                        }

                        if (rotationData != null)
                        {
                            using (var bmStream = new MemoryStream())
                            {
                                bumpBM.Save(bmStream, System.Drawing.Imaging.ImageFormat.Png); //Bitmap to Stream
                                DevIL.Image bmp = imp.LoadImageFromStream(new MemoryStream(bmStream.GetBuffer())); //Image from Stream
                                exp.SaveImageToStream(bmp, DevIL.ImageType.Png, bmStream); //Image to DDS
                                Tools.WriteFile(bmStream, string.Format("{0}\\{1}", path, mat.rotationDDS.Substring(mat.rotationDDS.LastIndexOf('/') + 1).Replace(".dds", ".png"))); //Save DDS
                            }
                            if (mat.useEmissive)
                                using (var bmStream = new MemoryStream())
                                {
                                    emisBM.Save(bmStream, System.Drawing.Imaging.ImageFormat.Png); //Bitmap to Stream
                                    DevIL.Image bmp = imp.LoadImageFromStream(new MemoryStream(bmStream.GetBuffer())); //Image from Stream
                                    exp.SaveImageToStream(bmp, DevIL.ImageType.Png, bmStream); //Image to DDS
                                    Tools.WriteFile(bmStream, string.Format("{0}\\{1}", path, mat.glossDDS.Substring(mat.glossDDS.LastIndexOf('/') + 1).Replace("_s.", "_emis.").Replace(".dds", ".png"))); //Save DDS
                                }
                        }

                        //save
                        using (var bmStream = new MemoryStream())
                        {
                            diffuseBM.Save(bmStream, System.Drawing.Imaging.ImageFormat.Png); //Bitmap to Stream
                            DevIL.Image bmp = imp.LoadImageFromStream(new MemoryStream(bmStream.GetBuffer())); //Image from Stream
                            exp.SaveImageToStream(bmp, DevIL.ImageType.Png, bmStream); //Image to DDS
                            Tools.WriteFile(bmStream, string.Format("{0}\\{1}", path, mat.diffuseDDS.Substring(mat.diffuseDDS.LastIndexOf('/') + 1).Replace(".dds", ".png"))); //Save DDS
                        }
                    }

                    //gloss map
                    file = ((ModelBrowser)Window).currentAssets.FindFile(mat.glossDDS);
                    if (file != null)
                    {
                        DevIL.ImageImporter imp = new DevIL.ImageImporter();
                        DevIL.ImageExporter exp = new DevIL.ImageExporter();

                        //Load dds
                        DevIL.Image dds;
                        using (Stream glossStream = file.OpenCopyInMemory())
                        {
                            dds = imp.LoadImageFromStream(DevIL.ImageType.Dds, glossStream);
                            //tor_tools.Tools.WriteFile((MemoryStream)glossStream, String.Format("{0}\\{1}", path, mat.glossDDS.Substring(mat.glossDDS.LastIndexOf('/') + 1))); //Save DDS
                        }

                        var glossData = dds.GetImageData(0);

                        var glossBM = new Bitmap(glossData.Width, glossData.Height, System.Drawing.Imaging.PixelFormat.Format24bppRgb);
                        var specBM = new Bitmap(glossData.Width, glossData.Height, System.Drawing.Imaging.PixelFormat.Format32bppArgb);

                        for (int k = 0; k < glossData.Height * glossData.Width; k++)
                        {
                            Color glossPixel = Color.FromArgb(
                                        glossData.Data[k * 4 + 3],
                                        glossData.Data[k * 4 + 3],
                                        glossData.Data[k * 4 + 3]);

                            glossBM.SetPixel(k % glossData.Width, (int)k / glossData.Width, glossPixel);

                            Color specPixel = Color.FromArgb(255,
                                        glossData.Data[k * 4 + 0],
                                        glossData.Data[k * 4 + 1],
                                        glossData.Data[k * 4 + 2]);

                            specBM.SetPixel(k % glossData.Width, (int)k / glossData.Width, specPixel);
                        }

                        //save
                        using (var bmStream = new MemoryStream())
                        {
                            glossBM.Save(bmStream, System.Drawing.Imaging.ImageFormat.Png); //Bitmap to Stream
                            DevIL.Image bmp = imp.LoadImageFromStream(new MemoryStream(bmStream.GetBuffer())); //Image from Stream
                            exp.SaveImageToStream(bmp, DevIL.ImageType.Png, bmStream); //Image to DDS
                            tor_tools.Tools.WriteFile(bmStream, string.Format("{0}\\{1}", path, mat.glossDDS.Substring(mat.glossDDS.LastIndexOf('/') + 1).Replace("_s.", "_si.").Replace(".dds", ".png"))); //Save DDS
                        }

                        using (var bmStream = new MemoryStream())
                        {
                            specBM.Save(bmStream, System.Drawing.Imaging.ImageFormat.Png); //Bitmap to Stream
                            DevIL.Image bmp = imp.LoadImageFromStream(new MemoryStream(bmStream.GetBuffer())); //Image from Stream
                            exp.SaveImageToStream(bmp, DevIL.ImageType.Png, bmStream); //Image to DDS
                            tor_tools.Tools.WriteFile(bmStream, string.Format("{0}\\{1}", path, mat.glossDDS.Substring(mat.glossDDS.LastIndexOf('/') + 1).Replace(".dds", ".png"))); //Save DDS
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
            Tools.WriteFile(output.ToString(), filename, false);
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
                vertices.Append(string.Format("v {0} {1} {2}\n", vertex.X, vertex.Y, vertex.Z));
                normals.Append(string.Format("vn {0} {1} {2} {3}\n", vertex.normX, vertex.normY, vertex.normZ, vertex.normW));
                uvs.Append(string.Format("vt {0} {1}\n", vertex.texU, -vertex.texV));
            }
            sb.Append(vertices).Append("\n")
                .Append(uvs).Append("\n")
                .Append(normals);

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
                R = value.R;
                G = value.G;
                B = value.B;
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
            return obj is View_NPC_GR2 gR &&
                    pMatrix.Equals(gR.pMatrix);
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
