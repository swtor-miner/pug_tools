using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Xml;
using System.Xml.Linq;

namespace tor_tools
{
    class Format_XML_MAT
    {
        public string dest = "";
        public HashSet<string> fileNames = new HashSet<string>();
        public List<string> errors = new List<string>();
        public string extension;

        public Format_XML_MAT(string dest, string ext)
        {
            this.dest = dest;
            this.extension = ext;
        }        

        public void ParseXML(Stream fileStream, string fullFileName)
        {
            string fileName = fullFileName.Substring(fullFileName.LastIndexOf('\\') + 1);
            string directory = fullFileName.Substring(0, fullFileName.LastIndexOf('/'));

            try
            {   
                if (fileName.Contains("am_"))
                {
                    var doc = XDocument.Load(fileStream);

                    string temp = fileName.Split('/').Last();
                    string fileNameNoExtension = temp.Substring(3, (temp.IndexOf('.') - 3));
                    string fullDirectory = directory + '/' + fileNameNoExtension + '/';
                    XElement aamElement = doc.Element("aam");
                    if(aamElement == null)
                    {
                        return;
                    }
                    var actionList = aamElement.Element("actions").Elements("action");

                    foreach (var action in actionList)
                    {
                        var actionName = action.Attribute("name").Value;
                        if (action.Attribute("actionProvider") != null)
                        {
                            var actionProvider = action.Attribute("actionProvider").Value + ".mph";
                            if (fullDirectory.Contains("/humanoid/humanoid/"))
                            {
                                fileNames.Add(fullDirectory.Replace("/humanoid/humanoid/", "/humanoid/bfanew/") + actionProvider);
                                fileNames.Add(fullDirectory.Replace("/humanoid/humanoid/", "/humanoid/bfanew/") + actionProvider + ".amx");
                                fileNames.Add(fullDirectory.Replace("/humanoid/humanoid/", "/humanoid/bfbnew/") + actionProvider);
                                fileNames.Add(fullDirectory.Replace("/humanoid/humanoid/", "/humanoid/bfbnew/") + actionProvider + ".amx");
                                fileNames.Add(fullDirectory.Replace("/humanoid/humanoid/", "/humanoid/bfnnew/") + actionProvider);
                                fileNames.Add(fullDirectory.Replace("/humanoid/humanoid/", "/humanoid/bfnnew/") + actionProvider + ".amx");
                                fileNames.Add(fullDirectory.Replace("/humanoid/humanoid/", "/humanoid/bfsnew/") + actionProvider);
                                fileNames.Add(fullDirectory.Replace("/humanoid/humanoid/", "/humanoid/bfsnew/") + actionProvider + ".amx");

                                fileNames.Add(fullDirectory.Replace("/humanoid/humanoid/", "/humanoid/bmanew/") + actionProvider);
                                fileNames.Add(fullDirectory.Replace("/humanoid/humanoid/", "/humanoid/bmanew/") + actionProvider + ".amx");
                                fileNames.Add(fullDirectory.Replace("/humanoid/humanoid/", "/humanoid/bmfnew/") + actionProvider);
                                fileNames.Add(fullDirectory.Replace("/humanoid/humanoid/", "/humanoid/bmfnew/") + actionProvider + ".amx");
                                fileNames.Add(fullDirectory.Replace("/humanoid/humanoid/", "/humanoid/bmnnew/") + actionProvider);
                                fileNames.Add(fullDirectory.Replace("/humanoid/humanoid/", "/humanoid/bmnnew/") + actionProvider + ".amx");
                                fileNames.Add(fullDirectory.Replace("/humanoid/humanoid/", "/humanoid/bmsnew/") + actionProvider);
                                fileNames.Add(fullDirectory.Replace("/humanoid/humanoid/", "/humanoid/bmsnew/") + actionProvider + ".amx");
                            }
                            else
                            {
                                fileNames.Add(fullDirectory + actionProvider);
                                fileNames.Add(fullDirectory + actionProvider + ".amx");
                            }
                        }
                        if (action.Attribute("animName") != null)
                        {
                            var animationName = action.Attribute("animName").Value;
                            if (actionName != animationName)
                            {
                                animationName += ".jba";
                                if (fullDirectory.Contains("/humanoid/humanoid/"))
                                {
                                    fileNames.Add(fullDirectory.Replace("/humanoid/humanoid/", "/humanoid/bfanew/") + animationName);
                                    fileNames.Add(fullDirectory.Replace("/humanoid/humanoid/", "/humanoid/bfbnew/") + animationName);
                                    fileNames.Add(fullDirectory.Replace("/humanoid/humanoid/", "/humanoid/bfnnew/") + animationName);
                                    fileNames.Add(fullDirectory.Replace("/humanoid/humanoid/", "/humanoid/bfsnew/") + animationName);

                                    fileNames.Add(fullDirectory.Replace("/humanoid/humanoid/", "/humanoid/bmanew/") + animationName);
                                    fileNames.Add(fullDirectory.Replace("/humanoid/humanoid/", "/humanoid/bmfnew/") + animationName);
                                    fileNames.Add(fullDirectory.Replace("/humanoid/humanoid/", "/humanoid/bmnnew/") + animationName);
                                    fileNames.Add(fullDirectory.Replace("/humanoid/humanoid/", "/humanoid/bmsnew/") + animationName);
                                }
                                else
                                {
                                    fileNames.Add(fullDirectory + animationName);
                                }
                            }
                        }
                        actionName += ".jba";
                        if (fullDirectory.Contains("/humanoid/humanoid/"))
                        {                            
                            fileNames.Add(fullDirectory.Replace("/humanoid/humanoid/", "/humanoid/bfanew/") + actionName);
                            fileNames.Add(fullDirectory.Replace("/humanoid/humanoid/", "/humanoid/bfbnew/") + actionName);
                            fileNames.Add(fullDirectory.Replace("/humanoid/humanoid/", "/humanoid/bfnnew/") + actionName);
                            fileNames.Add(fullDirectory.Replace("/humanoid/humanoid/", "/humanoid/bfsnew/") + actionName);

                            fileNames.Add(fullDirectory.Replace("/humanoid/humanoid/", "/humanoid/bmanew/") + actionName);
                            fileNames.Add(fullDirectory.Replace("/humanoid/humanoid/", "/humanoid/bmfnew/") + actionName);
                            fileNames.Add(fullDirectory.Replace("/humanoid/humanoid/", "/humanoid/bmnnew/") + actionName);
                            fileNames.Add(fullDirectory.Replace("/humanoid/humanoid/", "/humanoid/bmsnew/") + actionName);
                        }
                        else
                            fileNames.Add(fullDirectory + actionName);
                    }
                    XElement networkElem = aamElement.Element("networks");
                    if(networkElem == null)
                    {
                        return;
                    }
                    var networkList = networkElem.Descendants("literal");
                    foreach (var network in networkList)
                    {
                        var fqnName = network.Attribute("fqn").Value;
                        if (fqnName != null)
                        {
                            if (fullDirectory.Contains("/humanoid/humanoid/"))
                            {
                                fileNames.Add(fullDirectory.Replace("/humanoid/humanoid/", "/humanoid/bfanew/") + fqnName);
                                fileNames.Add(fullDirectory.Replace("/humanoid/humanoid/", "/humanoid/bfanew/") + fqnName + ".amx");
                                fileNames.Add(fullDirectory.Replace("/humanoid/humanoid/", "/humanoid/bfbnew/") + fqnName);
                                fileNames.Add(fullDirectory.Replace("/humanoid/humanoid/", "/humanoid/bfbnew/") + fqnName + ".amx");
                                fileNames.Add(fullDirectory.Replace("/humanoid/humanoid/", "/humanoid/bfnnew/") + fqnName);
                                fileNames.Add(fullDirectory.Replace("/humanoid/humanoid/", "/humanoid/bfnnew/") + fqnName + ".amx");
                                fileNames.Add(fullDirectory.Replace("/humanoid/humanoid/", "/humanoid/bfsnew/") + fqnName);
                                fileNames.Add(fullDirectory.Replace("/humanoid/humanoid/", "/humanoid/bfsnew/") + fqnName + ".amx");

                                fileNames.Add(fullDirectory.Replace("/humanoid/humanoid/", "/humanoid/bmanew/") + fqnName);
                                fileNames.Add(fullDirectory.Replace("/humanoid/humanoid/", "/humanoid/bmanew/") + fqnName + ".amx");
                                fileNames.Add(fullDirectory.Replace("/humanoid/humanoid/", "/humanoid/bmfnew/") + fqnName);
                                fileNames.Add(fullDirectory.Replace("/humanoid/humanoid/", "/humanoid/bmfnew/") + fqnName + ".amx");
                                fileNames.Add(fullDirectory.Replace("/humanoid/humanoid/", "/humanoid/bmnnew/") + fqnName);
                                fileNames.Add(fullDirectory.Replace("/humanoid/humanoid/", "/humanoid/bmnnew/") + fqnName + ".amx");
                                fileNames.Add(fullDirectory.Replace("/humanoid/humanoid/", "/humanoid/bmsnew/") + fqnName);
                                fileNames.Add(fullDirectory.Replace("/humanoid/humanoid/", "/humanoid/bmsnew/") + fqnName + ".amx");
                            }
                            else
                            {
                                fileNames.Add(fullDirectory + fqnName);
                                fileNames.Add(fullDirectory + fqnName + ".amx");
                            }
                        }
                    }
                }
                else
                {
                    var doc = XDocument.Load(fileStream);
                    foreach (var node in doc.Elements())
                    {
                        NodeChecker(node);
                    }
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

        private void NodeChecker(XElement node)
        {
            if (node.HasElements)
            {
                foreach (var childnode in node.Elements())
                {
                    if (childnode.Name == "input" && childnode.Element("type") != null)
                    {
                        var type = childnode.Element("type").Value; //new way of searching for texture file names
                        if (type == "texture")
                        {
                            var textureName = childnode.Element("value").Value;
                            if (textureName != null && textureName != "")
                            {
                                string scrubbedName = textureName.Replace("////", "//").Replace(" #", "").Replace("#", "").Replace("+", "/").Replace(" ", "_");
                                this.fileNames.Add("\\resources\\" + scrubbedName + ".dds");
                                this.fileNames.Add("\\resources\\" + scrubbedName + ".tex");
                                this.fileNames.Add("\\resources\\" + scrubbedName + ".tiny.dds");
                                string[] fileName = scrubbedName.Split('\\');
                                int startPosition = 0;
                                if (scrubbedName.Contains('\\')) { startPosition = scrubbedName.LastIndexOf('\\') + 1; }
                                int length = scrubbedName.Length - startPosition;
                                List<object> tagsToRemove = new List<object> { "_d", "_n", "_s" };
                                if (tagsToRemove.Any(name => scrubbedName.EndsWith(name.ToString()))) { length -= 2; }
                                string primaryName = scrubbedName.Substring(startPosition, length);
                                this.fileNames.Add("\\resources\\art\\shaders\\materials\\" + primaryName + ".mat");                                
                            }
                        }
                        /*else //catch types for analysis. Caught the following types: bool, uvscale, float, rgba, vector4
                        {
                            System.IO.StreamWriter file3 = new System.IO.StreamWriter("c:\\swtor\\types.txt", true);
                            file3.WriteLine(type);
                            file3.Close();
                        }*/
                    }
                    var fxSpecList = childnode.Elements("fxSpecString");
                    if (childnode.Name == "AppearanceAction" && fxSpecList.Count() > 0) //
                    {
                        foreach (var fxSpec in fxSpecList)
                        {
                            var fxSpecName = "\\resources\\art\\fx\\fxspec\\" + fxSpec.Value;
                            if (!fxSpec.Value.ToLower().EndsWith(".fxspec")) { fxSpecName += ".fxspec"; }
                            fileNames.Add(fxSpecName);
                        }
                    }
                    if (childnode.Name == "Asset")
                    {                        
                        var assetFilenames = AssetReader(childnode);
                        foreach (var name in assetFilenames)
                        {
                            string scrubbedName = name.Replace("////", "//").Replace(" #", "").Replace("#", "").Replace("+", "/").Replace(" ", "_");
                            fileNames.Add(scrubbedName);
                        }
                    }
                    else
                    {
                        NodeChecker(childnode);
                    }
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

        private static List<string> AssetReader(XElement childnode)
        {
            List<string> fileList = new List<string>();
            if (childnode.Element("BaseFile").Value != null)
            {
                var basefile = childnode.Element("BaseFile").Value;
                bool hasBodyTypes = false;
                bool bodyTypeT = (childnode.Element("BodyTypes") != null);
                bool bodyTypet = (childnode.Element("Bodytypes") != null);
                if (bodyTypeT) { hasBodyTypes = childnode.Element("BodyTypes").HasElements; }
                if (bodyTypet) { hasBodyTypes = childnode.Element("Bodytypes").HasElements; }
                if (hasBodyTypes)
                {
                    IEnumerable<string> bodyTypeList;
                    if (bodyTypeT)
                    {
                        bodyTypeList = from c in childnode.Element("BodyTypes").Elements()
                                       select c.Value;
                    }
                    else
                    {
                        bodyTypeList = from c in childnode.Element("Bodytypes").Elements()
                                       select c.Value;
                    }
                    if (childnode.Element("BaseFile").Value != "")
                    {
                        if (basefile.Contains("[bt]") && hasBodyTypes) { fileList.AddRange(BodyType(bodyTypeList, basefile)); } //Checking if we need to create file names for each bodytype.
                        else
                        {
                            if (basefile.Contains("[gen]")) { fileList.AddRange(Genderize(basefile)); } // Checking for gender specific file names
                            else { fileList.Add("/resources" + basefile); }
                        }
                    }

                    var materials = childnode.Element("Materials").Elements();
                    if (materials != null) //check for material file names.
                    {
                        foreach (var material in materials)
                        {
                            string filename = material.Attribute("filename").Value;
                            if (filename.Contains("[bt]") && hasBodyTypes) { fileList.AddRange(BodyType(bodyTypeList, filename)); } //Checking if we need to create file names for each bodytype.
                            else
                            {
                                if (filename.Contains("[gen]")) { fileList.AddRange(Genderize(filename)); } // Checking for gender specific file names
                                else { fileList.Add("/resources" + filename); }
                            }
                            var matoverrides = material.Element("MaterialOverrides").Elements();
                            if (matoverrides != null)
                            {
                                foreach(var over in matoverrides)
                                {
                                    string override_filename = over.Attribute("filename").Value;
                                    if (override_filename.Contains("[bt]") && hasBodyTypes) { fileList.AddRange(BodyType(bodyTypeList, override_filename)); } //Checking if we need to create file names for each bodytype.
                                    else
                                    {
                                        if (override_filename.Contains("[gen]")) { fileList.AddRange(Genderize(override_filename)); } // Checking for gender specific file names
                                        else { fileList.Add("/resources" + override_filename); }
                                    }
                                }
                            }                            
                        }
                    }

                    var attachments = childnode.Element("Attachments").Elements();
                    if (attachments != null) //check for attachment model file names.
                    {
                        foreach (var attachment in attachments)
                        {
                            string filename = attachment.Attribute("filename").Value;
                            if (filename.Contains("[bt]")) { fileList.AddRange(BodyType(bodyTypeList, filename)); } //Checking if we need to create file names for each bodytype.
                            else
                            {
                                if (filename.Contains("[gen]")) { fileList.AddRange(Genderize(filename)); } // Checking for gender specific file names
                                else { fileList.Add("/resources" + filename); }
                            }
                        }
                    }
                }
                else
                {
                    if (childnode.Element("BaseFile").Value != "")
                    {
                        if (basefile.Contains("[gen]")) { fileList.AddRange(Genderize(basefile)); } // Checking for gender specific file names
                        else { fileList.Add("/resources" + basefile); }

                    }

                    var materials = childnode.Element("Materials").Elements();
                    if (materials != null) //check for material file names.
                    {
                        foreach (var material in materials)
                        {
                            string filename = material.Attribute("filename").Value;
                            if (filename.Contains("[gen]")) { fileList.AddRange(Genderize(filename)); } // Checking for gender specific file names
                            else { fileList.Add("/resources" + filename); }
                        }
                    }

                    var attachments = childnode.Element("Attachments").Elements();
                    if (attachments != null) //check for attachment model file names.
                    {
                        foreach (var attachment in attachments)
                        {
                            string filename = attachment.Attribute("filename").Value;
                            if (filename.Contains("[gen]")) { fileList.AddRange(Genderize(filename)); } // Checking for gender specific file names
                            else { fileList.Add("/resources" + filename); }
                        }
                    }
                }
            }
            return fileList;
        }

        private static List<string> Genderize(string filename)
        {
            var fileList = new List<string>();
            List<string> genders = new List<string> { "m", "f", "u" }; //disable "u" to reduce noise in output for analysis, should be turned back on for file name searching

            foreach (var gender in genders)
            {
                string genderFileName = filename.Replace("[gen]", gender);
                fileList.Add("/resources" + genderFileName);
            }

            return fileList;
        }

        private static List<string> BodyType(IEnumerable<string> bodyTypeList, string filename)
        {
            var fileList = new List<string>();

            foreach (var bodytype in bodyTypeList)
            {
                string bodyTypeFileName = filename.Replace("[bt]", bodytype);
                fileList.Add("/resources" + bodyTypeFileName);
            }

            return fileList;
        }
    }
}
