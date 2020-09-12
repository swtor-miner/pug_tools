using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Xml;
using System.Xml.Linq;
using GomLib;

namespace PugTools
{
    class Format_EPP
    {
        public string dest = "";
        public HashSet<string> fileNames = new HashSet<string>();
        public HashSet<string> animNames = new HashSet<string>();
        public List<string> errors = new List<string>();
        public string extension;

        public Format_EPP(string dest, string ext)
        {
            this.dest = dest;
            extension = ext;
        }

        public void ParseEPP(Stream fileStream, string fullFileName)
        {
            _ = fullFileName.Substring(fullFileName.LastIndexOf('\\') + 1);
            _ = fullFileName.Substring(0, fullFileName.LastIndexOf('/'));

            try
            {
                XmlDocument doc = new XmlDocument();
                doc.Load(fileStream);

                XmlNode anode = doc.SelectSingleNode("Appearance");
                string file = "/resources/gamedata/" + anode.Attributes.GetNamedItem("fqn").InnerText.Replace('.', '/') + ".epp";
                fileNames.Add(file);

                XmlNodeList elemList = doc.GetElementsByTagName("fxSpecString");

                foreach (XmlNode node in elemList)
                {
                    string fxspec = node.InnerText;
                    fxspec = "/resources/art/fx/fxspec/" + fxspec + ".fxspec";
                    fileNames.Add(fxspec);
                }

                elemList = doc.GetElementsByTagName("projectileFXString");

                foreach (XmlNode node in elemList)
                {
                    string fxspec = node.InnerText;
                    fxspec = "/resources/art/fx/fxspec/" + fxspec + ".fxspec";
                    fileNames.Add(fxspec);
                }

                elemList = doc.GetElementsByTagName("casterAnim");

                foreach (XmlNode node in elemList)
                {
                    string anim = node.InnerText;
                    animNames.Add(anim);
                }

                elemList = doc.GetElementsByTagName("targetAnim");

                foreach (XmlNode node in elemList)
                {
                    string anim = node.InnerText;
                    animNames.Add(anim);
                }

            }
            catch (Exception ex)
            {
                errors.Add("File: " + fullFileName);
                errors.Add(ex.Message + ":");
                errors.Add(ex.StackTrace);
                errors.Add("");
            }
        }

        public void ParseEPPNodes(List<GomObject> eppNodes)
        {
            foreach (GomObject obj in eppNodes)
            {
                string slash = obj.Name.ToLower().ToString().Replace('.', '/');
                string epp = "/resources/gamedata/" + slash + ".epp";
                fileNames.Add(epp);
            }
        }

        public void WriteFile()
        {
            if (!Directory.Exists(dest + "\\File_Names"))
                Directory.CreateDirectory(dest + "\\File_Names");
            if (fileNames.Count > 0)
            {
                StreamWriter outputNames = new StreamWriter(dest + "\\File_Names\\" + extension + "_file_names.txt", false);
                foreach (string file in fileNames)
                {
                    outputNames.Write(file.Replace("\\", "/") + "\r\n");
                }
                outputNames.Close();
                fileNames.Clear();
            }

            if (animNames.Count > 0)
            {
                StreamWriter outputNames = new StreamWriter(dest + "\\File_Names\\" + extension + "_anim_file_names.txt", false);
                foreach (string file in animNames)
                {
                    outputNames.Write(file.Replace("\\", "/") + "\r\n");
                }
                outputNames.Close();
                animNames.Clear();
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
