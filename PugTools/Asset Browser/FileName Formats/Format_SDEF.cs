using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace tor_tools
{
    class Format_SDEF
    {
        public string dest = "";
        public HashSet<string> fileNames = new HashSet<string>();
        public List<string> errors = new List<string>();
        public string extension;
        public int found;

        public Format_SDEF(string dest, string ext)
        {
            this.dest = dest;
            this.extension = ext;
        }

        public void ParseSDEF(Stream fileStream)
        {
            BinaryReader br = new BinaryReader(fileStream);

            uint header = FileFormats.File_Helpers.ReverseBytes(br.ReadUInt32());

            if (header.ToString("X") != "53444546")
            {
                return;
            }
            else
            {
                //read unknown 1 version info??
                br.ReadBytes(4);

                //C9 indicates 2 byte integer
                br.ReadByte();

                //Read 2 byte integer                
                UInt16 count = FileFormats.File_Helpers.ReverseBytes(br.ReadUInt16());

                for (int c = 0; c < count; c++)
                {
                    //CF Idenitifes 8 byte integer
                    br.ReadByte();

                    //Read the 8 byte integer                    
                    ulong id = FileFormats.File_Helpers.ReverseBytes(br.ReadUInt64());

                    //null seperator
                    br.ReadByte();

                    //CB identifies a 4 byte integer -- CA identifies a 3 byte integer                    
                    byte cb = br.ReadByte();

                    if (cb == 203)
                    {
                        //Read the 4 byte integer
                        br.ReadBytes(4);
                    }
                    else if (cb == 202)
                    {
                        //Read the 3 byte integer
                        br.ReadBytes(3);
                    }

                    //null seperator
                    br.ReadByte();

                    fileNames.Add("/resources/systemgenerated/compilednative/" + id);
                }
            }
            return;
        }

        public void WriteFile()
        {
            if (!System.IO.Directory.Exists(this.dest + "\\File_Names"))
                System.IO.Directory.CreateDirectory(this.dest + "\\File_Names");

            this.found = this.fileNames.Count();

            if (this.fileNames.Count > 0)
            {
                System.IO.StreamWriter outputNames = new System.IO.StreamWriter(this.dest + "\\File_Names\\" + this.extension + "_file_names.txt", false);
                foreach (string file in fileNames)
                {
                    outputNames.WriteLine(file.Replace("\\", "/"));
                }
                outputNames.Close();
                fileNames.Clear();
            }

            if (this.errors.Count > 0)
            {
                System.IO.StreamWriter outputErrors = new System.IO.StreamWriter(this.dest + "\\File_Names\\" + this.extension + "_error_list.txt", false);
                foreach (string error in errors)
                {
                    outputErrors.WriteLine(error);
                }
                outputErrors.Close();
                errors.Clear();
            }
        }
    }
}
