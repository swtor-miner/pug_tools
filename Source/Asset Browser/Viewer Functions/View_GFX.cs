using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows.Forms;
using ICSharpCode.SharpZipLib.Zip.Compression;

namespace tor_tools
{
    public static class View_GFX
    {

        public static MemoryStream DecompressGFX(BinaryReader br)
        {
            byte[] header = br.ReadBytes(3);

            if ((header[0] == 67 && header[1] == 70 && header[2] == 88) || (header[0] == 67 && header[1] == 87 && header[2] == 83))
            {
                //SmallVersion
                _ = br.ReadByte();

                //Data Length
                uint dataLength = br.ReadUInt32();

                byte[] inflatedData = new Byte[dataLength];

                Inflater inf = new Inflater();
                inf.SetInput(br.ReadBytes((int)br.BaseStream.Length - 8));
                inf.Inflate(inflatedData);

                MemoryStream stream = new MemoryStream();
                stream.Write(inflatedData, 0, inflatedData.Length);
                return stream;

            }
            else
            {
                return null;
            }
        }
    }
}
