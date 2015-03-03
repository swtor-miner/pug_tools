using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using TorLib;

namespace tor_tools
{
    class Format_GR2
    {
        public string dest = "";
        public HashSet<string> fileNames = new HashSet<string>();
        public List<string> errors = new List<string>();
        //public HashSet<string> meshNames = new HashSet<string>();
        public Dictionary<string, Archive> meshNames = new Dictionary<string, Archive>();
        public HashSet<string> matNames = new HashSet<string>();
        
        public string extension;

        public Format_GR2(string dest, string ext)
        {
            this.dest = dest;
            this.extension = ext;
        }        

        public void parseGR2(Stream fileStream, string fullFileName, Archive arch)
        {
            List<uint> offsetMeshNames = new List<uint>();
            List<uint> offsetMaterialNames = new List<uint>();
            BinaryReader br = new BinaryReader(fileStream);

            ulong header = br.ReadUInt32();            

            if (header.ToString("X") != "42574147")
            {
                errors.Add("File: " + fullFileName);
                errors.Add("Invalid header" + header.ToString());
                return;
            }
            else
            {                
                br.BaseStream.Seek(0x10, SeekOrigin.Begin);
            
                uint num50Offsets = br.ReadUInt32();                
                uint gr2_type = br.ReadUInt32();                 
                ushort numMeshes = br.ReadUInt16();
                ushort numMaterials = br.ReadUInt16();
                
                br.BaseStream.Seek(0x50, SeekOrigin.Begin);
                                
                uint offset50offset = br.ReadUInt32();
                uint offsetMeshHeader = br.ReadUInt32();
                uint offsetMaterialNameOffsets = br.ReadUInt32();

                if (numMeshes != 0)
                {
                    br.BaseStream.Seek(offsetMeshHeader, SeekOrigin.Begin);
                    
                    for (int i = 0; i < numMeshes; i++)
                    {                        
                        uint offset = br.ReadUInt32();
                        br.ReadSingle();
                        br.ReadUInt16();
                        br.ReadUInt16();
                        br.ReadUInt16();
                        br.ReadUInt16();
                        br.ReadUInt32();
                        br.ReadUInt32();
                        br.ReadUInt32();
                        br.ReadUInt32();
                        br.ReadUInt32();
                        br.ReadUInt32();
                        offsetMeshNames.Add(offset);
                    }

                    if (offsetMeshNames.Count() > 0)
                    {
                        foreach (uint i in offsetMeshNames)
                        {
                            string meshName = ReadString(fileStream, br, i);
                            if(!meshNames.ContainsKey(meshName))
                                meshNames.Add(meshName, arch);
                            matNames.Add(meshName);
                        }
                    }
                }

                if (numMaterials != 0)
                {
                    br.BaseStream.Seek(offsetMaterialNameOffsets, SeekOrigin.Begin);
                    for (int i = 0; i < numMaterials; i++)
                    {                        
                        uint offset = br.ReadUInt32();
                        offsetMaterialNames.Add(offset);
                    }

                    if (offsetMaterialNames.Count() > 0)
                    {                        
                        foreach (uint i in offsetMaterialNames)
                        {
                            matNames.Add(ReadString(fileStream, br, i));
                        }
                    }
                }                
            }
        }        

        public static string ReadString(Stream fileStream, BinaryReader br, uint offset)
        {
            long original_position = fileStream.Position;
            fileStream.Position = offset;
            List<byte> strBytes = new List<byte>();
            int b;
            while ((b = br.ReadByte()) != 0x00)
                strBytes.Add((byte)b);
            fileStream.Position = original_position;
            return Encoding.ASCII.GetString(strBytes.ToArray());
        }

        public void WriteFile(bool outputAllDirs = false)
        {
            if (!System.IO.Directory.Exists(this.dest + "\\File_Names"))
                System.IO.Directory.CreateDirectory(this.dest + "\\File_Names");
            if (this.meshNames.Count > 0)
            {
                System.IO.StreamWriter outputMeshNames = new System.IO.StreamWriter(this.dest + "\\File_Names\\" + this.extension + "_mesh_file_names.txt", false);
                foreach (var file in meshNames)
                {
                    string output = "";
                    if (file.Value.FileName.Contains("_dynamic_"))
                    {
                        if (file.Key.Contains("_"))
                        {
                            string type = file.Key.Split('_').First();
                            output += "/resources/art/dynamic/" + type + "/model/" + file.Key + ".gr2\r\n";
                            output += "/resources/art/dynamic/" + type + "/model/" + file.Key + ".lod.gr2\r\n";
                            output += "/resources/art/dynamic/" + type + "/model/" + file.Key + ".clo\r\n";
                        }
                    }
                    else
                    {
                       /* if (outputAllDirs)
                        {
                            foreach (string dir in file.Value.directories)
                            {
                                output += dir + "/" + file.Key + ".gr2\r\n";                              
                            }
                        }
                        else
                        {*/
                            output += file.Key + ".gr2\r\n";
                        //}
                    }
                    output = output.Replace("//", "/");
                    outputMeshNames.Write(output);
                }
                outputMeshNames.Close();
            }

            if (this.matNames.Count > 0)
            {
                System.IO.StreamWriter outputMatNames = new System.IO.StreamWriter(this.dest + "\\File_Names\\" + this.extension + "_material_file_names.txt", false);
                foreach (string file in matNames)
                {
                    outputMatNames.Write("/resources/art/shaders/materials/" + file + ".mat" + "\r\n");
                }
                outputMatNames.Close();
            }

            if (this.errors.Count > 0)
            {
                System.IO.StreamWriter outputErrors = new System.IO.StreamWriter(this.dest + "\\File_Names\\" + this.extension + "_error_list.txt", false);
                foreach (string error in errors)
                {
                    outputErrors.Write(error + "\r\n");
                }
                outputErrors.Close();
            }
        }
    }
}
