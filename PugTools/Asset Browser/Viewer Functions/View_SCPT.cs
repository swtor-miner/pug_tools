using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows.Forms;
using nsHashDictionary;

namespace tor_tools
{
    public static class View_SCPT
    { 

        public static MemoryStream decryptSCPT(BinaryReader br)
        {
            byte[] data;

            ulong header = br.ReadUInt32();                        

            if (header.ToString() == "1414546259")
            {
                //SmallVersion
                ushort smallVer = br.ReadUInt16();

                //BigVersion
                ushort bigVer = br.ReadUInt16();

                //Unknown
                br.ReadUInt64();              

                if(bigVer == 5 && smallVer == 5)
                {
                    //Read ID
                    ulong id = br.ReadUInt64();

                    //Read IsEncrypted 
                    bool encrypted = br.ReadBoolean();

                    //Unknown
                    br.ReadUInt64();
                    
                    //Data Length
                    uint dataLength = br.ReadUInt32();
                    data = br.ReadBytes((int)dataLength);

                    if(encrypted)
                    {
                        byte[] decryptedData = new Byte[data.Length];
                        uint unk = 0x35;
                        for(int i = 0; i < data.Length; i++)
                        {
                            decryptedData[i] = (byte)(data[i] ^ unk);
                            unk += 0x36;
                        }
                        data = decryptedData;
                    }
                    MemoryStream stream = new MemoryStream();
                    stream.Write(data, 0, data.Length);
                    return stream;
                }
                return null;
            }
            else
            {   
                return null;
            }                        
        }       
    }
}
