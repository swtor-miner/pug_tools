using System;
using SlimDX.Direct2D;
using SlimDX.Direct3D11;
using FeatureLevel = SlimDX.Direct3D11.FeatureLevel;

namespace SlimDX_Framework
{
    using System.Linq;
    using System.Threading;
    using System.Windows.Forms;

    using SlimDX_Framework.Vertex;

    using SlimDX;
    using SlimDX.DXGI;

    using Buffer = SlimDX.Direct3D11.Buffer;
    using Device = Device;

    public class D3DPanelApp : DisposableClass
    {
        public static D3DPanelApp GD3DPanelApp;
        private bool _disposed;

        public Form Window { get; protected set; }
        public IntPtr AppInst { get; protected set; }
        public float AspectRatio { get { return (float)ClientWidth / ClientHeight; } }
        public string RenderPanelName { get; protected set; }

        protected bool AppPaused;
        protected bool Minimized;
        protected bool Maximized;
        protected bool Resizing;
        protected int Msaa4XQuality;
        protected bool Enable4XMsaa;
        protected GameTimer Timer;

        protected Device Device;
        protected DeviceContext ImmediateContext;
        protected SwapChain SwapChain;
        protected Texture2D DepthStencilBuffer;
        protected RenderTargetView RenderTargetView;
        protected DepthStencilView DepthStencilView;
        protected Viewport Viewport;
        protected DriverType DriverType;
        public int ClientWidth { get; protected set; }
        public int ClientHeight { get; protected set; }
        private volatile bool _resize;
        private volatile bool _running;
        private WindowRenderTarget _dxWRT;
        internal WindowRenderTarget DxWrt
        {
            get { return _dxWRT; }
        }

        protected Buffer _screenQuadVB;
        protected Buffer _screenQuadIB;

        public Buffer ScreenQuadVB
        {
            get { return _screenQuadVB; }
            set { _screenQuadVB = value; }
        }

        public Buffer ScreenQuadIB
        {
            get { return _screenQuadIB; }
            set { _screenQuadIB = value; }
        }

        protected bool InitMainWindow()
        {
            try
            {
                ClientWidth = Window.Controls.Find(RenderPanelName, true).First().Width;
                ClientHeight = Window.Controls.Find(RenderPanelName, true).First().Height;
                Window.Controls.Find(RenderPanelName, true).First().MouseDown += OnMouseDown;
                Window.Controls.Find(RenderPanelName, true).First().MouseUp += OnMouseUp;
                Window.Controls.Find(RenderPanelName, true).First().MouseMove += OnMouseMove;
                Window.Controls.Find(RenderPanelName, true).First().MouseWheel += OnMouseWheel;

                Window.ResizeBegin += (sender, args) =>
                {
                    AppPaused = true;
                    Resizing = true;
                    Timer.Stop();
                };
                Window.ResizeEnd += (sender, args) =>
                {
                    AppPaused = false;
                    Resizing = false;
                    Timer.Start();
                    OnResize();
                };

                Window.Show();
                Window.Update();
                return true;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message + "\n" + ex.StackTrace, "Error");
                return false;
            }
        }

        protected virtual void OnMouseWheel(object sender, MouseEventArgs e) { }
        protected virtual void OnMouseMove(object sender, MouseEventArgs e) { }
        protected virtual void OnMouseUp(object sender, MouseEventArgs e) { }
        protected virtual void OnMouseDown(object sender, MouseEventArgs e) { }

        protected bool InitDirect3D()
        {
            var creationFlags = DeviceCreationFlags.None;
#if DEBUG
            //This is broken on Windows 10.
            //creationFlags |= DeviceCreationFlags.Debug;
#endif
            try
            {
                Device = new Device(DriverType, creationFlags);
            }
            catch (Exception ex)
            {
                MessageBox.Show("D3D11Device creation failed\n" + ex.Message + "\n" + ex.StackTrace, "Error");
                return false;
            }
            ImmediateContext = Device.ImmediateContext;
            if (Device.FeatureLevel != FeatureLevel.Level_11_0)
            {
                Console.WriteLine("Direct3D Feature Level 11 unsupported\nSupported feature level: " + Enum.GetName(Device.FeatureLevel.GetType(), Device.FeatureLevel));
            }
            Msaa4XQuality = Device.CheckMultisampleQualityLevels(Format.R8G8B8A8_UNorm, 4);
            try
            {
                var sd = new SwapChainDescription
                {
                    ModeDescription = new ModeDescription(ClientWidth, ClientHeight, new Rational(60, 1), Format.R8G8B8A8_UNorm)
                    {
                        ScanlineOrdering = DisplayModeScanlineOrdering.Unspecified,
                        Scaling = DisplayModeScaling.Unspecified
                    },
                    // SampleDescription = new SampleDescription(1, 0),
                    SampleDescription = Enable4XMsaa && Device.FeatureLevel >= FeatureLevel.Level_10_1 ? new SampleDescription(4, Msaa4XQuality - 1) : new SampleDescription(1, 0),
                    Usage = Usage.RenderTargetOutput,
                    BufferCount = 1,
                    OutputHandle = Window.Controls.Find(RenderPanelName, true).First().Handle,
                    IsWindowed = true,
                    SwapEffect = SwapEffect.Discard,
                    Flags = SwapChainFlags.None

                };
                SwapChain = new SwapChain(Device.Factory, Device, sd);
            }
            catch (Exception ex)
            {
                MessageBox.Show("SwapChain creation failed\n" + ex.Message + "\n" + ex.StackTrace, "Error");
                return false;
            }
            OnResize();
            return true;
        }

        protected override void Dispose(bool disposing)
        {
            if (!_disposed)
            {
                _running = false;
                if (disposing)
                {
                    Util.ReleaseCom(ref RenderTargetView);
                    Util.ReleaseCom(ref DepthStencilView);
                    Util.ReleaseCom(ref _screenQuadIB);
                    Util.ReleaseCom(ref _screenQuadVB);

                    Util.ReleaseCom(ref DepthStencilBuffer);
                    if (ImmediateContext != null)
                    {
                        ImmediateContext.ClearState();
                    }
                    Util.ReleaseCom(ref SwapChain);
                    Util.ReleaseCom(ref ImmediateContext);
                    Util.ReleaseCom(ref Device);

                    Util.ReleaseCom(ref _dxWRT);
                }
                _disposed = true;
            }
            base.Dispose(disposing);
        }

        protected D3DPanelApp(IntPtr hInstance, string panelName)
        {
            AppInst = hInstance;
            DriverType = DriverType.Hardware;
            ClientWidth = 800;
            ClientHeight = 600;
            Window = null;
            RenderPanelName = panelName;
            Msaa4XQuality = 0;
            Enable4XMsaa = true;
            Device = null;
            ImmediateContext = null;
            SwapChain = null;
            DepthStencilBuffer = null;
            RenderTargetView = null;
            DepthStencilView = null;
            Viewport = new Viewport();
            Timer = new GameTimer();

            GD3DPanelApp = this;
        }
        public virtual bool Init()
        {
            if (!InitMainWindow())
            {
                return false;
            }
            if (!InitDirect3D())
            {
                return false;
            }

            BuildScreenQuadGeometryBuffers();

            _running = true;
            return true;
        }
        private void BuildScreenQuadGeometryBuffers()
        {
            var quad = GeometryGenerator.CreateFullScreenQuad();

            var verts = quad.Vertices.Select(v => new Basic32(v.Position, v.Normal, v.TexC)).ToList();
            var vbd = new BufferDescription(Basic32.Stride * verts.Count, ResourceUsage.Immutable, BindFlags.VertexBuffer, CpuAccessFlags.None, ResourceOptionFlags.None, 0);
            ScreenQuadVB = new Buffer(Device, new DataStream(verts.ToArray(), false, false), vbd);

            var ibd = new BufferDescription(sizeof(int) * quad.Indices.Count, ResourceUsage.Immutable, BindFlags.IndexBuffer, CpuAccessFlags.None, ResourceOptionFlags.None, 0);
            ScreenQuadIB = new Buffer(Device, new DataStream(quad.Indices.ToArray(), false, false), ibd);
        }

        public virtual void OnResize()
        {
            Util.ReleaseCom(ref RenderTargetView);
            Util.ReleaseCom(ref DepthStencilView);
            Util.ReleaseCom(ref DepthStencilBuffer);

            SwapChain.ResizeBuffers(1, ClientWidth, ClientHeight, Format.R8G8B8A8_UNorm, SwapChainFlags.None);
            using (var resource = SlimDX.Direct3D11.Resource.FromSwapChain<Texture2D>(SwapChain, 0))
            {
                RenderTargetView = new RenderTargetView(Device, resource);
                RenderTargetView.Resource.DebugName = "main render target";
            }

            var depthStencilDesc = new Texture2DDescription
            {
                Width = ClientWidth,
                Height = ClientHeight,
                MipLevels = 1,
                ArraySize = 1,
                Format = Format.D24_UNorm_S8_UInt,
                // SampleDescription = new SampleDescription(1, 0),
                SampleDescription = Enable4XMsaa && Device.FeatureLevel >= FeatureLevel.Level_10_1 ? new SampleDescription(4, Msaa4XQuality - 1) : new SampleDescription(1, 0),
                Usage = ResourceUsage.Default,
                BindFlags = BindFlags.DepthStencil,
                CpuAccessFlags = CpuAccessFlags.None,
                OptionFlags = ResourceOptionFlags.None
            };
            DepthStencilBuffer = new Texture2D(Device, depthStencilDesc) { DebugName = "DepthStencilBuffer" };
            DepthStencilView = new DepthStencilView(Device, DepthStencilBuffer);

            ImmediateContext.OutputMerger.SetTargets(DepthStencilView, RenderTargetView);

            Viewport = new Viewport(0, 0, ClientWidth, ClientHeight, 0.0f, 1.0f);

            ImmediateContext.Rasterizer.SetViewports(Viewport);
            _resize = false;
        }
        public virtual void UpdateScene(float dt) { if (!_running) return; }
        public virtual void DrawScene()
        {
            if (_resize)
                OnResize();
        }

        protected virtual void EndFrame()
        {
            SwapChain.Present(0, PresentFlags.None);
        }

        public virtual void StopRender()
        {
            _running = false;
        }

        public virtual void StartRender()
        {
            _running = true;
            Run();
        }

        public void Run()
        {
            Timer.Reset();
            Timer.FrameTime = 1.0f / 60.0f;
            while (_running)
            {
                Timer.Tick();
                if (!AppPaused && _running)
                {
                    UpdateScene(Timer.DeltaTime);
                    DrawScene();
                }
                else
                {
                    Thread.Sleep(100);
                }
            }
            // Dispose();
        }

        public void SetSize(int height, int width)
        {
            if (Device != null)
            {
                ClientHeight = height;
                ClientWidth = width;
                _resize = true;
            }
        }
    }
}
