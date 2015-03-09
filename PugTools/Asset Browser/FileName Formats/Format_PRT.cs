using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace tor_tools
{
    class Format_PRT
    {
        public string dest = "";
        public HashSet<string> fileNames = new HashSet<string>();
        public List<string> errors = new List<string>();
        public string extension;

        public Format_PRT(string dest, string ext)
        {
            this.dest = dest;
            this.extension = ext;
        }        

        public void parsePRT(Stream fileStream, string fullFileName)
        {
            StreamReader reader = new StreamReader(fileStream);
            string line;
            while ((line = reader.ReadLine()) != null)
            {
                string test = line.Replace("  .", "");
                test = test.Replace("Name=", "");
                test = test.Replace("EmitSpec=", "");
                test = test.Replace("Trail", "");
                test = test.Replace("Texture_Purple", "");
                test = test.Replace("Texture_Blue", "");
                test = test.Replace("Texture_Red", "");
                test = test.Replace("Texture_Green", "");
                test = test.Replace("Texture_White", "");
                test = test.Replace("Texture_Yellow", "");
                test = test.Replace("Texture_Orange", "");
                test = test.Replace("Texture", "/");                
                test = test.Replace("GrannyFile", "");
                test = test.Replace("EmitFXSpec=", "");
                test = test.Replace("EmitAtDeathSpec=", "");
                test = test.Replace("=", "");
                test = test.Replace("\\", "/");
                test = test.Replace("//", "/");
                test = test.ToLower();

                if (test.Contains(".prt"))
                {
                    if (!test.Contains("/art/fx/particles/"))
                        fileNames.Add("/resources/art/fx/particles/" + test);
                    else
                        fileNames.Add("/resources" + test);
                }
                else if (test.Contains(".dds"))
                {
                    fileNames.Add("/resources" + test);
                    fileNames.Add("/resources" + test.Replace(".dds", ".tiny.dds"));
                    fileNames.Add("/resources" + test.Replace(".dds", ".tex"));
                }
                else if (test.Contains(".fxspec"))
                {
                    fileNames.Add("/resources" + test);
                }
                else if (test.Contains(".gr2"))
                {
                    fileNames.Add("/resources" + test);
                }
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
