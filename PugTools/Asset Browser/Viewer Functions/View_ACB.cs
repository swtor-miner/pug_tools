﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Threading;
using System.Diagnostics;

namespace tor_tools
{
    public class View_ACB
    {
        public List<WEM_File> wems = new List<WEM_File>();

        public List<WEM_File> parseACB(BinaryReader br)
        {
            long numWEMs = br.ReadInt64();
            for (int intCount = 0; intCount < (int)numWEMs; intCount++)
            {
                WEM_File wem = new WEM_File();
                List<byte> strTemp = new List<byte>();
                byte value;
                while ((value = br.ReadByte()) != 0x00)
                {
                    strTemp.Add(value);
                }
                wem.name = Encoding.ASCII.GetString(strTemp.ToArray());
                wem.length = br.ReadInt64();
                wem.offset = br.ReadInt64();
                long orig_pos = br.BaseStream.Position;
                br.BaseStream.Seek(wem.offset, SeekOrigin.Begin);
                wem.data = br.ReadBytes((int)wem.length);
                br.BaseStream.Seek(orig_pos, SeekOrigin.Begin);
                wem.oggName = wem.name.Replace(".wem", ".ogg");
                wems.Add(wem);
            }
            return wems;
        }
    }
}
