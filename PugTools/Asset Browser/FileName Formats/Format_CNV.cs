using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Xml;
using System.Xml.Linq;
using GomLib;

namespace tor_tools
{
    class Format_CNV
    {
        public string dest = "";
        public HashSet<string> fileNames = new HashSet<string>();
        public HashSet<string> animNames = new HashSet<string>();
        public List<string> errors = new List<string>();
        public string extension;

        public Format_CNV(string dest, string ext)
        {
            this.dest = dest;
            this.extension = ext;
        }        

        public void ParseCNVNodes(List<GomObject> cnvNodes)
        {
            foreach (GomObject obj in cnvNodes)
            {
                string under = obj.Name.ToLower().ToString().Replace('.', '_');
                string slash = obj.Name.ToLower().ToString().Replace('.', '/');
                string stb = "/resources/en-us/str/" + slash + ".stb";
                string acb = "/resources/en-us/bnk2/" + under + ".acb";
                string fxe = "/resources/en-us/fxe/" + slash + ".fxe";
                fileNames.Add(stb);
                fileNames.Add(acb);
                fileNames.Add(fxe);
            }
        }

        public void WriteFile()
        {
            if (!System.IO.Directory.Exists(this.dest + "\\File_Names"))
                System.IO.Directory.CreateDirectory(this.dest + "\\File_Names");
            if (this.fileNames.Count > 0)
            {
                System.IO.StreamWriter outputNames = new System.IO.StreamWriter(this.dest + "\\File_Names\\" + this.extension + "_file_names.txt", false);
                foreach (string file in fileNames)
                {
                    outputNames.Write(file.Replace("\\", "/") + "\r\n");
                }
                outputNames.Close();
            }
           
            if(this.errors.Count > 0)
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
