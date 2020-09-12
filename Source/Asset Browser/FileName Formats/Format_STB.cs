using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using TorLib;
using GomLib;
using System.Xml;

namespace PugTools
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
            extension = ext;
        }

        public void ParseSTBManifest(Stream fileStream)
        {
            XmlDocument doc = new XmlDocument();
            doc.Load(fileStream);
            XmlNodeList fileList = doc.GetElementsByTagName("file");
            if (fileList.Count > 0)
            {
                foreach (XmlNode node in fileList)
                {
                    var attr = node.Attributes["val"];
                    if (attr != null)
                        fileNames.Add(attr.Value);
                }
            }
        }

        public void WriteFile(bool _ = false)
        {
            if (!Directory.Exists(dest + "\\File_Names"))
                Directory.CreateDirectory(dest + "\\File_Names");
            if (fileNames.Count > 0)
            {
                StreamWriter outputFileNames = new StreamWriter(dest + "\\File_Names\\" + extension + "_file_names.txt", false);
                foreach (string item in fileNames)
                {
                    if (item != "")
                        outputFileNames.WriteLine(("/resources/en-us/" + item.Replace(".", "/") + ".stb").Replace("//", "/"));
                }
                outputFileNames.Close();
                fileNames.Clear();
            }

            if (errors.Count > 0)
            {
                StreamWriter outputErrors = new StreamWriter(dest + "\\File_Names\\" + extension + "_error_list.txt", false);
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
