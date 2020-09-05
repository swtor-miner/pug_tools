using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using TorLib;
using GomLib;

namespace PugTools
{
    class Format_PLC
    {
        public string dest = "";
        public HashSet<string> fileNames = new HashSet<string>();
        public List<string> errors = new List<string>();
        public string extension;

        public Format_PLC(string dest, string ext)
        {
            this.dest = dest;
            this.extension = ext;
        }

        public void ParsePLC(List<GomObject> plcNodes)
        {
            foreach (GomObject obj in plcNodes)
            {
                string plcModel = obj.Data.ValueOrDefault<string>("plcModel", null);
                if (plcModel != null)
                    if (plcModel.Contains("dyn."))
                        continue;
                    else
                        fileNames.Add(plcModel.Replace("\\", "/").Replace("//", "/"));
                obj.Unload();
            }
        }

        public void WriteFile(bool _ = false)
        {
            if (!System.IO.Directory.Exists(this.dest + "\\File_Names"))
                System.IO.Directory.CreateDirectory(this.dest + "\\File_Names");
            if (this.fileNames.Count > 0)
            {
                System.IO.StreamWriter outputFileNames = new System.IO.StreamWriter(this.dest + "\\File_Names\\" + this.extension + "_file_names.txt", false);
                foreach (string item in fileNames)
                {
                    if (item != "")
                        outputFileNames.WriteLine(("/resources/" + item).Replace("//", "/"));
                }
                outputFileNames.Close();
                fileNames.Clear();
            }

            if (this.errors.Count > 0)
            {
                System.IO.StreamWriter outputErrors = new System.IO.StreamWriter(this.dest + "\\File_Names\\" + this.extension + "_error_list.txt", false);
                foreach (string error in errors)
                {
                    outputErrors.Write(error + "\r\n");
                }
                outputErrors.Close();
                errors.Clear();
            }
        }
    }
}
