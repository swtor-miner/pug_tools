using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using TorLib;
using GomLib;
using System.Xml;

namespace tor_tools
{
    class Format_STB
    {
        public string dest = "";
        public HashSet<string> fileNames = new HashSet<string>();        
        public List<string> errors = new List<string>();        
        public string extension;

        public Format_STB(string dest, string ext)
        {
            this.dest = dest;
            this.extension = ext;
        }        

        public void parseSTBManifest(Stream fileStream)
        {
            XmlDocument doc = new XmlDocument();
            doc.Load(fileStream);                        
            XmlNodeList fileList = doc.GetElementsByTagName("file");
            if(fileList.Count > 0)
            {
                foreach(XmlNode node in fileList)
                {
                    var attr = node.Attributes["val"];
                    if (attr != null)
                        fileNames.Add(attr.Value);                    
                }
            }
        }

        public void WriteFile(bool outputAllDirs = false)
        {
            if (!System.IO.Directory.Exists(this.dest + "\\File_Names"))
                System.IO.Directory.CreateDirectory(this.dest + "\\File_Names");
            if (this.fileNames.Count > 0)
            {
                System.IO.StreamWriter outputFileNames = new System.IO.StreamWriter(this.dest + "\\File_Names\\" + this.extension + "_file_names.txt", false);
                foreach (string item in fileNames)
                {
                    if (item != "")                    
                        outputFileNames.WriteLine(("/resources/en-us/" + item.Replace(".", "/") + ".stb").Replace("//", "/"));                    
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
