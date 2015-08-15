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
    class Format_FXSPEC
    {
        public string dest = "";
        public HashSet<string> fileNames = new HashSet<string>();
        public HashSet<string> resourceFileNames = new HashSet<string>();        
        public List<string> errors = new List<string>();
        public string extension;

        public Format_FXSPEC(string dest, string ext)
        {
            this.dest = dest;
            this.extension = ext;
        }        

        public void ParseFXSPEC(Stream fileStream, string fullFileName)
        {
            string fileName = fullFileName.Substring(fullFileName.LastIndexOf('\\') + 1);
            string directory = fullFileName.Substring(0, fullFileName.LastIndexOf('/'));

            try
            {   
                XmlDocument doc = new XmlDocument();
                doc.Load(fileStream);

                
                XmlNodeList fileElemList = doc.SelectNodes("//node()[@name='displayName']");
                foreach (XmlNode node in fileElemList)
                {
                    string resource = node.InnerText;                   
                    fileNames.Add(resource + ".fxspec");
                }

                XmlNodeList resourceElemList = doc.SelectNodes("//node()[@name='_fxResourceName']");
                foreach (XmlNode node in resourceElemList)
                {
                    string resource = node.InnerText;
                    if (resource.Contains(".prt"))
                    {
                        string output = "/resources/art/fx/particles/" + resource.Replace('\\', '/').ToLower();
                        output = output.Replace("//", "/");
                        output = output.Replace("/resources/art/fx/particles/art/fx/particles/", "/resources/art/fx/particles/");
                        resourceFileNames.Add(output);
                    }
                    else if (resource.Contains(".gr2"))
                    {
                        string output = "/resources/" + resource.Replace('\\', '/').ToLower();
                        output = output.Replace("//", "/");
                        resourceFileNames.Add(output);
                    }
                    else if (resource.Contains(".lit") || resource.Contains(".ext") || resource.Contains(".zzp"))
                    {
                        string output = "/resources/" + resource.Replace('\\', '/').ToLower();
                        output = output.Replace("//", "/");
                        resourceFileNames.Add(output);

                    }
                    else if (resource.Contains("Play_") || resource.Contains("play_") || resource.Contains("Stop_") || resource.Contains("stop_") || resource == "" || resource.Contains(".sgt") || resource.Contains(".wav"))
                    {
                        continue;
                    }                 
                }

                XmlNodeList projTexElemList = doc.SelectNodes("//node()[@name='_fxProjectionTexture']");
                foreach (XmlNode node in projTexElemList)
                {
                    string resource = node.InnerText.Replace(".tiny.dds", "").Replace(".dds", "").Replace(".tex", "");
                    string output = "/resources" + resource.Replace('\\', '/').ToLower();
                    resourceFileNames.Add(output + ".dds");
                    resourceFileNames.Add(output + ".tiny.dds");
                    resourceFileNames.Add(output + ".tex");
                }

                XmlNodeList projTex1ElemList = doc.SelectNodes("//node()[@name='_fxProjectionTexture_layer1']");
                foreach (XmlNode node in projTex1ElemList)
                {
                    string resource = node.InnerText.Replace(".tiny.dds", "").Replace(".dds", "").Replace(".tex", "");
                    string output = "/resources" + resource.Replace('\\', '/').ToLower();
                    resourceFileNames.Add(output + ".dds");
                    resourceFileNames.Add(output + ".tiny.dds");
                    resourceFileNames.Add(output + ".tex");
                }

                XmlNodeList texNameElemList = doc.SelectNodes("//node()[@name='_fxTextureName']");
                foreach (XmlNode node in texNameElemList)
                {
                    string resource = node.InnerText.Replace(".tiny.dds", "").Replace(".dds", "").Replace(".tex", "");
                    string output = "/resources" + resource.Replace('\\', '/').ToLower();
                    resourceFileNames.Add(output + ".dds");
                    resourceFileNames.Add(output + ".tiny.dds");
                    resourceFileNames.Add(output + ".tex");
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
                fileNames.Clear();
            }

            if (this.resourceFileNames.Count > 0)
            {
                System.IO.StreamWriter outputNames = new System.IO.StreamWriter(this.dest + "\\File_Names\\" + this.extension + "_resource_file_names.txt", false);
                foreach (string file in resourceFileNames)
                {
                    outputNames.Write(file.Replace("\\", "/") + "\r\n");
                }
                outputNames.Close();
                resourceFileNames.Clear();
            }
    
            if(this.errors.Count > 0)
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
