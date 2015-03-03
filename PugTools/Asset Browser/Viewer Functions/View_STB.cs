using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace tor_tools
{
    class View_STB
    {
        public List<STB_Entry> entries = new List<STB_Entry>();

        public List<STB_Entry> parseSTB(BinaryReader br)
        {
            byte[] header = br.ReadBytes(3);            
            if (header[0] == 1 && header[1] == 0 && header[2] == 0)
            {
                int numStrings = br.ReadInt32();
                for (int intCount = 0; intCount < numStrings; intCount++)
                {
                    STB_Entry entry = new STB_Entry();
                    entry.ID = br.ReadInt64();
                    br.ReadByte();
                    br.ReadByte();
                    br.ReadInt32();
                    entry.length = br.ReadInt32();
                    entry.offset = br.ReadInt32();
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
            br.BaseStream.Seek(entry.offset, SeekOrigin.Begin);            
            byte[] b = br.ReadBytes(entry.length);
            entry.stringValue = System.Text.Encoding.UTF8.GetString(b);
            return;
        }

    }

    class STB_Entry
    {
        public long ID { get; set; }
        public int length { get; set; }
        public int offset { get; set; }
        public string stringValue { get; set; }
    }
}
