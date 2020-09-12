using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Threading;
using System.Diagnostics;
using NAudio.Wave;

namespace PugTools
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
                Name = name;
                OggName = name.Replace(".wem", ".ogg");
            }
            if (inputStream != null)
            {
                MemoryStream memStream = new MemoryStream();
                inputStream.CopyTo(memStream);
                Data = memStream.ToArray();
                memStream.Close();
                inputStream.Close();
            }
        }

        public WEM_File(BinaryReader br)
        {
            Id = br.ReadUInt32();
            Offset = br.ReadUInt32();
            Length = br.ReadUInt32();
            Name = Id.ToString() + ".wem";
            OggName = Id.ToString() + ".ogg";
        }

#pragma warning disable CS1998, CS4014
        public async void ConvertWEM()
        {
            if (!Directory.Exists(@".\Temp"))
            {
                Directory.CreateDirectory(@".\Temp");
            }

            FileStream fs = new FileStream(@".\Temp\" + Name, FileMode.Create, FileAccess.Write);
            fs.Write(Data, 0, Data.Count());
            fs.Close();

            ProcessStartInfo convertWEMInfo = new ProcessStartInfo
            {
                CreateNoWindow = true,
                UseShellExecute = false,
                FileName = @".\Tools\ww2ogg.exe",
                WindowStyle = ProcessWindowStyle.Hidden,
                Arguments = @".\Temp\" + Name + @" --pcb .\Tools\packed_codebooks_aoTuV_603.bin"
            };

            ProcessStartInfo convertOGGInfo = new ProcessStartInfo
            {
                CreateNoWindow = true,
                UseShellExecute = false,
                FileName = @".\Tools\revorb.exe",
                WindowStyle = ProcessWindowStyle.Hidden,
                Arguments = @".\Temp\" + OggName
            };

            try
            {
                using (Process convertWEM = Process.Start(convertWEMInfo))
                {
                    convertWEM.WaitForExit();
                }
                File.Delete(@".\Temp\" + Name);

                using (Process convertOGG = Process.Start(convertOGGInfo))
                {
                    convertOGG.WaitForExit();
                }
                Vorbis = new NVorbis.NAudioSupport.VorbisFileReader(@".\Temp\" + OggName);
            }
            catch
            {
                // Log error.
            }
        }
#pragma warning restore CS1998, CS4014
    }
}
