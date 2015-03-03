using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Threading;
using System.Diagnostics;
using NAudio.Wave;

namespace tor_tools
{
    public class WEM_File
    {
        public uint id { get; set; }
        public string name { get; set; }
        public string oggName { get; set; }
        public long length { get; set; }
        public long offset { get; set; }
        public byte[] data { get; set; }
        public WaveStream vorbis { get; set; }

        public WEM_File(string name = null, Stream inputStream = null)
        {
            if (name != null)
            {
                if (!name.Contains(".wem"))
                    name += ".wem";
                this.name = name;
                this.oggName = name.Replace(".wem", ".ogg");
            }
            if (inputStream != null)
            {
                MemoryStream memStream = new MemoryStream();
                inputStream.CopyTo(memStream);
                this.data = memStream.ToArray();
                memStream.Close();
                inputStream.Close();
            }
        }

        public WEM_File(BinaryReader br)
        {
            this.id = br.ReadUInt32();            
            this.offset = br.ReadUInt32();
            this.length = br.ReadUInt32();
            this.name = this.id.ToString() + ".wem";
            this.oggName = this.id.ToString() + ".ogg";
        }

        public async void convertWEM()
        {
            if (!System.IO.Directory.Exists(@".\Temp"))
            {
                System.IO.Directory.CreateDirectory(@".\Temp");
            }

            FileStream fs = new FileStream(@".\Temp\" + this.name, FileMode.Create, FileAccess.Write);
            fs.Write(this.data, 0, this.data.Count());
            fs.Close();

            ProcessStartInfo convertWEMInfo = new ProcessStartInfo();
            convertWEMInfo.CreateNoWindow = true;
            convertWEMInfo.UseShellExecute = false;
            convertWEMInfo.FileName = @".\Tools\ww2ogg.exe";
            convertWEMInfo.WindowStyle = ProcessWindowStyle.Hidden;
            convertWEMInfo.Arguments = @".\Temp\" + this.name + @" --pcb .\Tools\packed_codebooks_aoTuV_603.bin";

            ProcessStartInfo convertOGGInfo = new ProcessStartInfo();
            convertOGGInfo.CreateNoWindow = true;
            convertOGGInfo.UseShellExecute = false;
            convertOGGInfo.FileName = @".\Tools\revorb.exe";
            convertOGGInfo.WindowStyle = ProcessWindowStyle.Hidden;
            convertOGGInfo.Arguments = @".\Temp\" + this.oggName;

            try
            {
                using (Process convertWEM = Process.Start(convertWEMInfo))
                {
                    convertWEM.WaitForExit();
                }
                System.IO.File.Delete(@".\Temp\" + this.name);

                using (Process convertOGG = Process.Start(convertOGGInfo))
                {
                    convertOGG.WaitForExit();
                }
                this.vorbis = new NVorbis.NAudioSupport.VorbisFileReader(@".\Temp\" + this.oggName);
            }
            catch
            {
                // Log error.
            }
        }
    }
}
