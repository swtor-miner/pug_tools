using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Xml;
using System.Xml.Linq;

namespace PugTools
{
    class Format_XML_MAT
    {
        public string dest = "";
        public HashSet<string> fileNames = new HashSet<string>();
        public HashSet<string> animFileNames = new HashSet<string>();
        public List<string> errors = new List<string>();
        public string extension;

        public Format_XML_MAT(string dest, string ext)
        {
            this.dest = dest;
            extension = ext;
        }

        public void ParseXML(Stream fileStream, string fullFileName, string baseFolder = null)
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
                    string fullDirectory = "";
                    if (baseFolder != null)
                        fullDirectory = string.Format("/resources/{0}", baseFolder);
                    else
                        fullDirectory = directory + '/' + fileNameNoExtension + '/';
                    XElement aamElement = doc.Element("aam");
                    if (aamElement == null)
                    {
                        return;
                    }
                    var actionElement = aamElement.Element("actions");
                    if (actionElement != null)
                    {
                        var actionList = actionElement.Elements("action");

                        foreach (var action in actionList)
                        {
                            var actionName = action.Attribute("name").Value;
                            if (action.Attribute("actionProvider") != null)
                            {
                                var actionProvider = action.Attribute("actionProvider").Value + ".mph";
                                if (fullDirectory.Contains("/humanoid/humanoid/"))
                                {
                                    animFileNames.Add(fullDirectory.Replace("/humanoid/humanoid/", "/humanoid/bfanew/") + actionProvider);
                                    animFileNames.Add(fullDirectory.Replace("/humanoid/humanoid/", "/humanoid/bfanew/") + actionProvider + ".amx");
                                    animFileNames.Add(fullDirectory.Replace("/humanoid/humanoid/", "/humanoid/bfbnew/") + actionProvider);
                                    animFileNames.Add(fullDirectory.Replace("/humanoid/humanoid/", "/humanoid/bfnnew/") + actionProvider);
                                    animFileNames.Add(fullDirectory.Replace("/humanoid/humanoid/", "/humanoid/bfnnew/") + actionProvider + ".amx");
                                    animFileNames.Add(fullDirectory.Replace("/humanoid/humanoid/", "/humanoid/bfsnew/") + actionProvider);
                                    animFileNames.Add(fullDirectory.Replace("/humanoid/humanoid/", "/humanoid/bfsnew/") + actionProvider + ".amx");

                                    animFileNames.Add(fullDirectory.Replace("/humanoid/humanoid/", "/humanoid/bmanew/") + actionProvider);
                                    animFileNames.Add(fullDirectory.Replace("/humanoid/humanoid/", "/humanoid/bmanew/") + actionProvider + ".amx");
                                    animFileNames.Add(fullDirectory.Replace("/humanoid/humanoid/", "/humanoid/bmfnew/") + actionProvider);
                                    animFileNames.Add(fullDirectory.Replace("/humanoid/humanoid/", "/humanoid/bmfnew/") + actionProvider + ".amx");
                                    animFileNames.Add(fullDirectory.Replace("/humanoid/humanoid/", "/humanoid/bmnnew/") + actionProvider);
                                    animFileNames.Add(fullDirectory.Replace("/humanoid/humanoid/", "/humanoid/bmnnew/") + actionProvider + ".amx");
                                    animFileNames.Add(fullDirectory.Replace("/humanoid/humanoid/", "/humanoid/bmsnew/") + actionProvider);
                                    animFileNames.Add(fullDirectory.Replace("/humanoid/humanoid/", "/humanoid/bmsnew/") + actionProvider + ".amx");
                                }
                                else
                                {
                                    animFileNames.Add(fullDirectory + actionProvider);
                                    animFileNames.Add(fullDirectory + actionProvider + ".amx");
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
                                        animFileNames.Add(fullDirectory.Replace("/humanoid/humanoid/", "/humanoid/bfanew/") + animationName);
                                        animFileNames.Add(fullDirectory.Replace("/humanoid/humanoid/", "/humanoid/bfbnew/") + animationName);
                                        animFileNames.Add(fullDirectory.Replace("/humanoid/humanoid/", "/humanoid/bfnnew/") + animationName);
                                        animFileNames.Add(fullDirectory.Replace("/humanoid/humanoid/", "/humanoid/bfsnew/") + animationName);

                                        animFileNames.Add(fullDirectory.Replace("/humanoid/humanoid/", "/humanoid/bmanew/") + animationName);
                                        animFileNames.Add(fullDirectory.Replace("/humanoid/humanoid/", "/humanoid/bmfnew/") + animationName);
                                        animFileNames.Add(fullDirectory.Replace("/humanoid/humanoid/", "/humanoid/bmnnew/") + animationName);
                                        animFileNames.Add(fullDirectory.Replace("/humanoid/humanoid/", "/humanoid/bmsnew/") + animationName);
                                    }
                                    else
                                    {
                                        animFileNames.Add(fullDirectory + animationName);
                                    }
                                }
                            }
                            actionName += ".jba";
                            if (fullDirectory.Contains("/humanoid/humanoid/"))
                            {
                                animFileNames.Add(fullDirectory.Replace("/humanoid/humanoid/", "/humanoid/bfanew/") + actionName);
                                animFileNames.Add(fullDirectory.Replace("/humanoid/humanoid/", "/humanoid/bfbnew/") + actionName);
                                animFileNames.Add(fullDirectory.Replace("/humanoid/humanoid/", "/humanoid/bfnnew/") + actionName);
                                animFileNames.Add(fullDirectory.Replace("/humanoid/humanoid/", "/humanoid/bfsnew/") + actionName);

                                animFileNames.Add(fullDirectory.Replace("/humanoid/humanoid/", "/humanoid/bmanew/") + actionName);
                                animFileNames.Add(fullDirectory.Replace("/humanoid/humanoid/", "/humanoid/bmfnew/") + actionName);
                                animFileNames.Add(fullDirectory.Replace("/humanoid/humanoid/", "/humanoid/bmnnew/") + actionName);
                                animFileNames.Add(fullDirectory.Replace("/humanoid/humanoid/", "/humanoid/bmsnew/") + actionName);
                            }
                            else
                                animFileNames.Add(fullDirectory + actionName);
                        }
                    }

                    XElement networkElem = aamElement.Element("networks");
                    if (networkElem != null)
                    {
                        var networkList = networkElem.Descendants("literal");
                        foreach (var network in networkList)
                        {
                            var fqnName = network.Attribute("fqn").Value;
                            if (fqnName != null)
                            {
                                if (fullDirectory.Contains("/humanoid/humanoid/"))
                                {
                                    animFileNames.Add(fullDirectory.Replace("/humanoid/humanoid/", "/humanoid/bfanew/") + fqnName);
                                    animFileNames.Add(fullDirectory.Replace("/humanoid/humanoid/", "/humanoid/bfanew/") + fqnName + ".amx");
                                    animFileNames.Add(fullDirectory.Replace("/humanoid/humanoid/", "/humanoid/bfbnew/") + fqnName);
                                    animFileNames.Add(fullDirectory.Replace("/humanoid/humanoid/", "/humanoid/bfbnew/") + fqnName + ".amx");
                                    animFileNames.Add(fullDirectory.Replace("/humanoid/humanoid/", "/humanoid/bfnnew/") + fqnName);
                                    animFileNames.Add(fullDirectory.Replace("/humanoid/humanoid/", "/humanoid/bfnnew/") + fqnName + ".amx");
                                    animFileNames.Add(fullDirectory.Replace("/humanoid/humanoid/", "/humanoid/bfsnew/") + fqnName);
                                    animFileNames.Add(fullDirectory.Replace("/humanoid/humanoid/", "/humanoid/bfsnew/") + fqnName + ".amx");

                                    animFileNames.Add(fullDirectory.Replace("/humanoid/humanoid/", "/humanoid/bmanew/") + fqnName);
                                    animFileNames.Add(fullDirectory.Replace("/humanoid/humanoid/", "/humanoid/bmanew/") + fqnName + ".amx");
                                    animFileNames.Add(fullDirectory.Replace("/humanoid/humanoid/", "/humanoid/bmfnew/") + fqnName);
                                    animFileNames.Add(fullDirectory.Replace("/humanoid/humanoid/", "/humanoid/bmfnew/") + fqnName + ".amx");
                                    animFileNames.Add(fullDirectory.Replace("/humanoid/humanoid/", "/humanoid/bmnnew/") + fqnName);
                                    animFileNames.Add(fullDirectory.Replace("/humanoid/humanoid/", "/humanoid/bmnnew/") + fqnName + ".amx");
                                    animFileNames.Add(fullDirectory.Replace("/humanoid/humanoid/", "/humanoid/bmsnew/") + fqnName);
                                    animFileNames.Add(fullDirectory.Replace("/humanoid/humanoid/", "/humanoid/bmsnew/") + fqnName + ".amx");
                                }
                                else
                                {
                                    animFileNames.Add(fullDirectory + fqnName);
                                    animFileNames.Add(fullDirectory + fqnName + ".amx");
                                }
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
                                string scrubbedName = textureName.Replace("////", "//").Replace("\\art", "art").Replace(" #", "").Replace("#", "").Replace("+", "/").Replace(" ", "_");
                                fileNames.Add("\\resources\\" + scrubbedName + ".dds");
                                fileNames.Add("\\resources\\" + scrubbedName + ".tex");
                                fileNames.Add("\\resources\\" + scrubbedName + ".tiny.dds");
                                string[] fileName = scrubbedName.Split('\\');
                                int startPosition = 0;
                                if (scrubbedName.Contains('\\')) { startPosition = scrubbedName.LastIndexOf('\\') + 1; }
                                int length = scrubbedName.Length - startPosition;
                                List<object> tagsToRemove = new List<object> { "_d", "_n", "_s" };
                                if (tagsToRemove.Any(name => scrubbedName.EndsWith(name.ToString()))) { length -= 2; }
                                string primaryName = scrubbedName.Substring(startPosition, length);
                                fileNames.Add("\\resources\\art\\shaders\\materials\\" + primaryName + ".mat");
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

            if (animFileNames.Count > 0)
            {
                StreamWriter outputAnimNames = new StreamWriter(dest + "\\File_Names\\" + extension + "_anim_file_names.txt", false);
                foreach (string file in animFileNames)
                {
                    outputAnimNames.Write(file.Replace("\\", "/") + "\r\n");
                }
                outputAnimNames.Close();
                animFileNames.Clear();
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
                                foreach (var over in matoverrides)
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
