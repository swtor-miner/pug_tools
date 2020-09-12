namespace SlimDXNet.FX
{
    using System;

    using SlimDX.Direct3D11;

    public static class Effects
    {
        public static void InitAll(Device device)
        {
            try
            {
                GR2_FX = new GR2_Effect(device, "Shaders\\GR2.fx");
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }

        }
        public static void DestroyAll()
        {
            Util.ReleaseCom(ref GR2_FX);
        }

        public static GR2_Effect GR2_FX;
    }
}
