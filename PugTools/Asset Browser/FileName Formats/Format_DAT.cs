using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace tor_tools
{
    class Format_DAT
    {
        
        public string dest = "";
        public HashSet<string> fileNames = new HashSet<string>();
        public List<string> errors = new List<string>();
        public string extension;
        public string filename;        
        private HashSet<string> checkKeys = new HashSet<string>(new string[] { ".NormalMap2", ".NormalMap1", ".SurfaceMap", ".RampMap", ".Falloff", ".IlluminationMap", ".FxSpecName", ".EnvironmentMap", ".Intensity", ".PortalTarget", ".Color", ".gfxMovieName", ".DiffuseColor", ".ProjectionTexture" });
        public HashSet<string> portalTargets = new HashSet<string>();

        public Format_DAT(string dest, string ext)
        {
            this.dest = dest;
            this.extension = ext;
        }        

        public void parseDAT(Stream fileStream, string fullFileName)
        {
            this.filename = fullFileName;
            StreamReader reader = new StreamReader(fileStream);
            string stream_line;
            List<string> stream_lines = new List<string>();
            while ((stream_line = reader.ReadLine()) != null)
            {
                stream_lines.Add(stream_line.TrimStart());                
            }
            reader.Close();
            if (stream_lines.Any(x => x.Contains("! Area Specification")))
                parseAreaDAT(stream_lines);
            else if (stream_lines.Any(x => x.Contains("! Room Specification")))
                parseRoomDAT(stream_lines);
            else if (stream_lines.Any(x => x.Contains("! Character Specification")))
                parseCharacterDAT(stream_lines);
            else
                //throw new Exception("Unknown DAT Specification" + stream_lines[1]);
                Console.WriteLine("Unknown DAT Specification" + stream_lines[1]);
        }

        public void parseAreaDAT(List<string> lines)
        {   
            string areaID = "";
            if (this.filename.Contains("/resources/world/areas"))
            {
                areaID = this.filename.Replace("/resources/world/areas/", "").Replace("/area.dat", "");
                fileNames.Add("/resources/world/areas/" + areaID + "/mapnotes.not");
            }
            lines.RemoveAt(0);
            string area_name = lines[0].Split(new string[] { "for " }, StringSplitOptions.None).Last().Trim();            
            List<string> sectionNames = new List<string>( new string[] { "[ROOMS]", "[ASSETS]", "[PATHS]", "[SCHEMES]", "[TERRAINTEXTURES]", "[DYDTEXTURES]", "[DYDCHANNELPARAMS]", "[SETTINGS]"});
            
            string current = "";

            foreach (string line in lines)
            {
                if (sectionNames.Contains(line))
                    current = line;
                else
                {                    
                    switch (current)
                    {
                        case "[ROOMS]":
                            if (areaID != "")
                            {
                                fileNames.Add("/resources/world/areas/" + areaID + "/" + line.ToLower() + ".dat");
                            }
                            break;
                        case "[ASSETS]":
                            if (line.Contains(':') || line.Contains('#'))
                                continue;
                            fileNames.Add("/resources" + line.Split('=').Last().ToString().ToLower().Replace("\\", "/"));
                            break;
                        case "[PATHS]":                            
                            break;
                        case "[SCHEMES]":                            
                            if(line.Contains("/"))
                            {
                                int idx = 0;
                                while((idx = line.IndexOf('/', idx)) != -1)
                                {   
                                    int end = line.IndexOf('|', idx);
                                    int len = end - idx;
                                    string final = line.Substring(idx, len).ToLower();
                                    fileNames.Add("/resources" + final + ".tex");
                                    fileNames.Add("/resources" + final + ".dds");
                                    fileNames.Add("/resources" + final + ".tiny.dds");
                                    idx = end;
                                }
                            }                            
                            break;
                        case "[TERRAINTEXTURES]":
                            string[] terrainTemp = line.Split(':');
                            if (terrainTemp.Count() >= 3)
                            {
                                fileNames.Add("/resources/art/shaders/materials/" + terrainTemp[3].ToLower() + ".mat");
                                fileNames.Add("/resources/art/shaders/environmentmaterials/" + terrainTemp[3].ToLower() + ".emt");
                            }
                            break;
                        case "[DYDTEXTURES]":
                            string[] dydTemp = line.Split(':');
                            if (dydTemp.Count() >= 2)
                            {
                                fileNames.Add("/resources/art/shaders/materials/" + dydTemp[1].ToLower() + ".mat");
                                fileNames.Add("/resources/art/shaders/environmentmaterials/" + dydTemp[1].ToLower() + ".emt");
                            }
                            break;
                        case "[DYDCHANNELPARAMS]":                            
                            break;
                        case "[SETTINGS]":                            
                            break;
                        default:
                            break;
                    }
                    
                }                
            }            
        }

        public void parseRoomDAT(List<string> lines)
        {   
            List<string> sectionNames = new List<string>(new string[] { "[INSTANCES]", "[VISIBLE]", "[SETTINGS]" });

            Dictionary<string, HashSet<string>> instances = new Dictionary<string, HashSet<string>>();

            string current = "";

            foreach (string line in lines)
            {
                if (sectionNames.Contains(line))
                    current = line;
                else
                {
                    if (line.Contains(':') || line.Contains('#'))
                        continue;
                    switch (current)
                    {
                        case "[INSTANCES]":
                            string[] split = line.Split('=');
                            if (!instances.ContainsKey(split[0]))                                
                                instances.Add(split[0], new HashSet<string>());
                            instances[split[0]].Add(split[1]);
                            break;
                        case "[VISIBLE]":                            
                            break;
                        case "[SETTINGS]":
                            break;                        
                        default:                            
                            break;
                    }

                }
            }
            
            /*** - Diabled - Uncomment to find new keys that have slashes
            foreach (var item in instances)
            {
                var items = item.Value.Where(x => x.Contains('\\')).ToList();
                if (items.Count() > 0)
                {
                    if (!checkKeys.Contains(item.Key))
                    {
                        checkKeys.Add(item.Key);
                        Console.WriteLine(item.Key.ToString());
                    }
                }

                var items2 = item.Value.Where(x => x.Contains('/')).ToList();
                if (items2.Count() > 0)
                {
                    if (!checkKeys.Contains(item.Key))
                    {
                        checkKeys.Add(item.Key);
                        Console.WriteLine(item.Key.ToString());
                    }
                }
            }
            ***/

            foreach (string key in checkKeys)
            {
                HashSet<string> slashItems = new HashSet<string>();
                if (instances.TryGetValue(key, out slashItems))
                {
                    foreach (var item in slashItems)
                    {
                        if (key == ".PortalTarget")
                            continue; //Nothing of interest in the key
                        if (key == ".FxSpecName")
                        {
                            string fxSpec = ("/resources/art/fx/fxspec/" + item.ToLower() + ".fxspec").Replace("\\", "/").Replace("//", "/").Replace(".fxspec.fxspec", ".fxspec");
                            fileNames.Add(fxSpec);
                        }
                        else
                        {
                            string file = ("/resources/" + item.ToLower()).Replace("\\", "/").Replace("//", "/").Replace(".dds", "");
                            fileNames.Add(file + ".dds");
                            fileNames.Add(file + ".tiny.dds");
                            fileNames.Add(file + ".tex");                            
                        }                     
                    }
                }
            }
        }

        public void parseCharacterDAT(List<string> lines)
        {
            List<string> sectionNames = new List<string>(new string[] { "[PARTS]" });

            lines.RemoveAt(0);
            string skeleton_name = lines[0].Split(new string[] { "for " }, StringSplitOptions.None).Last().Trim();
            fileNames.Add("/resources/art/dynamic/spec/" + skeleton_name + ".gr2");

            Dictionary<string, string> parts = new Dictionary<string, string>();

            string current = "";

            foreach (string line in lines)
            {
                if (sectionNames.Contains(line))
                    current = line;
                else
                {
                    if (line.Contains(':') || line.Contains('#'))
                        continue;
                    switch (current)
                    {
                        case "[PARTS]":
                            if (line == "")
                                continue;
                            string[] split = line.Split('=');
                            if (!parts.ContainsKey(split[0]))
                            { 
                               parts.Add(split[0], split[1]);                              
                            }
                            break;                     
                        default:                            
                            break;
                    }

                }
            }

            if (parts.ContainsKey("Model"))
            {
                fileNames.Add("/resources/art/dynamic/spec/" + parts["Model"]);
                fileNames.Add("/resources/art/dynamic/spec/" + parts["Model"].Replace(".dyc", ".dat"));
                fileNames.Add("/resources/art/dynamic/spec/" + parts["Model"].Replace(".dyc", ".mag"));
            }

            if (parts.ContainsKey("AnimMetadataFqn"))
            {
                string[] temp = parts["AnimMetadataFqn"].Split(',');
                foreach (string item in temp)
                {
                    string tempName = "/resources/" + item;
                    fileNames.Add(tempName.Replace('\\', '/').Replace("//", "/"));
                }
            }

            if (parts.ContainsKey("AnimLibraryFqn"))
            {  
                string tempName = "/resources/" + parts["AnimLibraryFqn"];
                fileNames.Add(tempName.Replace('\\', '/').Replace("//", "/"));
            }

            if (parts.ContainsKey("AnimShareMetadataFqn"))
            {
                string tempName = "/resources/" + parts["AnimShareMetadataFqn"];
                fileNames.Add(tempName.Replace('\\', '/').Replace("//", "/"));
            }

            /*** Disabled - Enable to find new keys that have slashes
            HashSet<string> animKeys = new HashSet<string>(new string[] { "AnimShareMetadataFqn", "AnimLibraryFqn", "AnimMetadataFqn", "Model", "AnimNetworkFolder" });
            foreach (var part in parts)
            {
                if (animKeys.Contains(part.Key))
                    continue;
                if (part.Value.Contains('\\'))
                {
                    Console.WriteLine(part.Key.ToString());
                }

                if (part.Value.Contains('/'))
                {
                    Console.WriteLine(part.Key.ToString());
                }              
            }
            ***/
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
