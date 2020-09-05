using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace PugTools
{
    class View_STB
    {
        public List<STB_Entry> entries = new List<STB_Entry>();

        public List<STB_Entry> ParseSTB(BinaryReader br)
        {
            byte[] header = br.ReadBytes(3);
            if (header[0] == 1 && header[1] == 0 && header[2] == 0)
            {
                int numStrings = br.ReadInt32();
                for (int intCount = 0; intCount < numStrings; intCount++)
                {
                    STB_Entry entry = new STB_Entry
                    {
                        ID = br.ReadInt64()
                    };
                    br.ReadByte();
                    br.ReadByte();
                    br.ReadInt32();
                    entry.Length = br.ReadInt32();
                    entry.Offset = br.ReadInt32();
                    br.ReadInt32();
                    entries.Add(entry);
                }

                foreach (STB_Entry entry in entries)
                {
                    ReadStringByLength(br, entry);
                }
            }
            return entries;
        }

        public void ReadStringByLength(BinaryReader br, STB_Entry entry)
        {
            br.BaseStream.Seek(entry.Offset, SeekOrigin.Begin);
            byte[] b = br.ReadBytes(entry.Length);
            entry.StringValue = System.Text.Encoding.UTF8.GetString(b);
            return;
        }

    }

    class STB_Entry
    {
        public long ID { get; set; }
        public int Length { get; set; }
        public int Offset { get; set; }
        public string StringValue { get; set; }
    }
}
