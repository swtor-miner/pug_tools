using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using TorLib;

namespace tor_tools
{
    class Format_AMX
    {
        public string dest = "";
        public HashSet<string> fileNames = new HashSet<string>();
        public List<string> errors = new List<string>();        
        public string extension;

        public Format_AMX(string dest, string ext)
        {
            this.dest = dest;
            this.extension = ext;
        }        

        public void parseAMX(Stream fileStream, string fullFileName)
        {   
            BinaryReader br = new BinaryReader(fileStream);

            ulong header = br.ReadUInt32();            

            if (header.ToString("X") != "20584D41")
            {
                errors.Add("File: " + fullFileName);
                errors.Add("Invalid header" + header.ToString());
                return;
            }
            else
            {
                br.ReadUInt16(); //unknown
                bool stop = false;
                do
                {
                    byte fileLen = br.ReadByte();
                    if (fileLen == 0)
                    {
                        stop = true;
                    }
                    else
                    {
                        byte[] fileNameBytes = br.ReadBytes(fileLen);
                        string fileName = Encoding.ASCII.GetString(fileNameBytes);

                        byte dirLen = br.ReadByte();
                        byte[] dirNameBytes = br.ReadBytes(dirLen);
                        string dirName = Encoding.ASCII.GetString(dirNameBytes);
                        string fullName = "/resources/anim/" + dirName.Replace('\\', '/') + "/" + fileName;
                        fullName = fullName.Replace("//", "/");

                        //humanoid\bfanew
                        //em_wookiee_10
                        fileNames.Add(fullName + ".jba");
                        fileNames.Add(fullName + ".mph");
                        fileNames.Add(fullName + ".mph.amx");

                        br.ReadUInt32();
                        byte check = br.ReadByte();
                        if (check != 2 && check != 3)
                            stop = true;
                    }
                } while (!stop);
            }
        } 

        public void WriteFile(bool outputAllDirs = false)
        {
            if (!System.IO.Directory.Exists(this.dest + "\\File_Names"))
                System.IO.Directory.CreateDirectory(this.dest + "\\File_Names");
            if (this.fileNames.Count > 0)
            {
                System.IO.StreamWriter outputFileNames = new System.IO.StreamWriter(this.dest + "\\File_Names\\" + this.extension + "_file_names.txt", false);
                foreach (var file in fileNames)
                {
                    outputFileNames.WriteLine(file);
                }
                outputFileNames.Close();
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
