using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows.Forms;
using nsHashDictionary;

namespace tor_tools
{
    public static class View_DEP
    {
        public static List<DEP_Entry> Read(BinaryReader br, HashDictionary hashdic)
        {
            ulong header = br.ReadUInt32();                        
            if (header.ToString() == "1")
            {
                //Read length (4 bytes)                
                int size = br.ReadInt32();

                //Read number of entries                
                int numEntries = br.ReadInt32();

                //Read length2                
                int length2 = br.ReadInt32();

                //Read number of entries2                
                int numEntries2 = br.ReadInt32();

                //Parse definitions
                List<DEP_Entry> entries = new List<DEP_Entry>();

                for (int i = 0; i < numEntries; i++)                
                {
                    DEP_Entry entry = new DEP_Entry();                    
                    uint sh = br.ReadUInt32();                    
                    uint ph = br.ReadUInt32();

                    HashData data = hashdic.SearchHashList(ph, sh);
                    if (data != null && data.filename != "")
                    {
                        entry.ID = (ulong)data.ph << 32 | data.sh;
                        entry.filename = data.filename;
                    }
                    else
                    {
                        if (data == null)
                        {
                            entry.ID = (ulong)ph << 32 | sh;
                            entry.filename = ((ulong)entry.ID).ToString();
                        }
                        else
                        {
                            entry.ID = (ulong)data.ph << 32 | data.sh;
                            entry.filename = ((ulong)entry.ID).ToString();
                        }

                    }

                    if (entry.filename.Contains('.'))
                    {
                        entry.type = (((string)entry.filename).Split('.').Last()).ToUpper();
                    }
                    else
                    {
                        entry.type = "Unknown";
                    }                    
                    entry.intDepencies = br.ReadInt16();
                    
                    if (entry.intDepencies > 0)
                    {   
                        for (int c = 0; c < entry.intDepencies; c++)
                        {
                            uint dep_sh = br.ReadUInt32();                         
                            uint dep_ph = br.ReadUInt32();

                            HashData dep_data = hashdic.SearchHashList(dep_ph, dep_sh);
                            string dependency = "";
                            if (dep_data != null)
                            {
                                dependency = dep_data.filename;
                                if (dependency == "")
                                    dependency = string.Format("{0}", (ulong)dep_ph << 32 | dep_sh);                                
                            }
                            else
                            {
                                dependency = string.Format("{0}", (ulong)dep_ph << 32 | dep_sh);                                
                            }                            
                            entry.Dependencies.Add(dependency);                            
                        }                        
                    }
                    entries.Add(entry);                    
                }
                return entries;              
            }
            else
            {   
                return null;
            }                        
        }
    }

    public class DEP_Entry
    {
        public ulong ID { get; set; }
        internal int intDepencies { get; set; }
        internal int strLength { get; set; }
        internal int strOffset { get; set; }
        internal string Value { get; set; }
        public string type { get; set; }
        public string filename { get; set; }
        public List<string> Dependencies { get; set; }

        public DEP_Entry()
        {
            Dependencies = new List<string>();
        }


        public override string ToString()
        {
            return string.Format("Entry {0}: {1}", ID, Value);
        }
    }
}
