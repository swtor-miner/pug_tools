namespace SlimDX_Framework.FX
{
    using System;
    using System.IO;
    using System.Windows.Forms;

    using SlimDX;
    using SlimDX.D3DCompiler;
    using SlimDX.Direct3D11;

    public abstract class Effect : DisposableClass
    {
        protected SlimDX.Direct3D11.Effect FX;
        private bool _disposed;
        protected Effect(Device device, string filename)
        {
            if (!File.Exists(filename))
            {
                throw new FileNotFoundException(string.Format("Effect file {0} not present", filename));
            }
            var shaderFlags = ShaderFlags.None;
#if DEBUG
            shaderFlags |= ShaderFlags.Debug;
            shaderFlags |= ShaderFlags.SkipOptimization;
#endif
            ShaderBytecode compiledShader = null;
            string errors = null;
            try
            {
                compiledShader = ShaderBytecode.CompileFromFile(
                    filename,
                    null,
                    "fx_5_0",
                    shaderFlags,
                    EffectFlags.None,
                    null,
                    null,
                    out errors);
                FX = new SlimDX.Direct3D11.Effect(device, compiledShader);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
            finally
            {
                Util.ReleaseCom(ref compiledShader);
            }
        }
        protected override void Dispose(bool disposing)
        {
            if (!_disposed)
            {
                if (disposing)
                {
                    Util.ReleaseCom(ref FX);
                }
                _disposed = true;
            }
            base.Dispose(disposing);
        }
    }
}