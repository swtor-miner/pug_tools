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
        public uint Id { get; set; }
        public string Name { get; set; }
        public string OggName { get; set; }
        public long Length { get; set; }
        public long Offset { get; set; }
        public byte[] Data { get; set; }
        public WaveStream Vorbis { get; set; }

        public WEM_File(string name = null, Stream inputStream = null)
        {
            if (name != null)
            {
                if (!name.Contains(".wem"))
                    name += ".wem";
                this.Name = name;
                this.OggName = name.Replace(".wem", ".ogg");
            }
            if (inputStream != null)
            {
                MemoryStream memStream = new MemoryStream();
                inputStream.CopyTo(memStream);
                this.Data = memStream.ToArray();
                memStream.Close();
                inputStream.Close();
            }
        }

        public WEM_File(BinaryReader br)
        {
            this.Id = br.ReadUInt32();
            this.Offset = br.ReadUInt32();
            this.Length = br.ReadUInt32();
            this.Name = this.Id.ToString() + ".wem";
            this.OggName = this.Id.ToString() + ".ogg";
        }

#pragma warning disable CS1998, CS4014
        public async void ConvertWEM()
        {
            if (!System.IO.Directory.Exists(@".\Temp"))
            {
                System.IO.Directory.CreateDirectory(@".\Temp");
            }

            FileStream fs = new FileStream(@".\Temp\" + this.Name, FileMode.Create, FileAccess.Write);
            fs.Write(this.Data, 0, this.Data.Count());
            fs.Close();

            ProcessStartInfo convertWEMInfo = new ProcessStartInfo
            {
                CreateNoWindow = true,
                UseShellExecute = false,
                FileName = @".\Tools\ww2ogg.exe",
                WindowStyle = ProcessWindowStyle.Hidden,
                Arguments = @".\Temp\" + this.Name + @" --pcb .\Tools\packed_codebooks_aoTuV_603.bin"
            };

            ProcessStartInfo convertOGGInfo = new ProcessStartInfo
            {
                CreateNoWindow = true,
                UseShellExecute = false,
                FileName = @".\Tools\revorb.exe",
                WindowStyle = ProcessWindowStyle.Hidden,
                Arguments = @".\Temp\" + this.OggName
            };

            try
            {
                using (Process convertWEM = Process.Start(convertWEMInfo))
                {
                    convertWEM.WaitForExit();
                }
                System.IO.File.Delete(@".\Temp\" + this.Name);

                using (Process convertOGG = Process.Start(convertOGGInfo))
                {
                    convertOGG.WaitForExit();
                }
                this.Vorbis = new NVorbis.NAudioSupport.VorbisFileReader(@".\Temp\" + this.OggName);
            }
            catch
            {
                // Log error.
            }
        }
#pragma warning restore CS1998, CS4014
    }
}
