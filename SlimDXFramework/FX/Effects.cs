namespace SlimDX_Framework.FX
{
    using System;
    using System.IO;
    using System.Text;

    using SlimDX.Direct3D11;

    public static class Effects
    {
        public static void InitAll(Device device)
        {
            /*
             * if (!Directory.GetCurrentDirectory().EndsWith("bin\\Debug"))
            {
                Directory.SetCurrentDirectory(Directory.GetCurrentDirectory() + "\\bin\\debug");                
            }*/

            // string dir = Directory.GetCurrentDirectory();
            // Console.WriteLine(dir);

            // Console.WriteLine("Loading effects from: " + Directory.GetCurrentDirectory());
            try
            {
                GR2_FX = new GR2_Effect(device, "Asset Browser\\Viewer Functions\\GR2 Classes\\GR2.fx");
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
