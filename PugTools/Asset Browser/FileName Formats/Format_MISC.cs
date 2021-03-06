﻿using System;
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
    class Format_MISC
    {
        public string dest = "";
        public HashSet<string> fileNames = new HashSet<string>();
        public HashSet<string> worldFileNames = new HashSet<string>();        
        public Dictionary<string, HashSet<string>> mapNames = new Dictionary<string, HashSet<string>>();
        public HashSet<string> animNames = new HashSet<string>();
        public List<string> errors = new List<string>();
        public string extension;
        public int found;
        public int searched;

        public Format_MISC(string dest, string ext)
        {
            this.dest = dest;
            this.extension = ext;
        }        

        public void ParseMISC_LdnScn(GomObject ldnScreenNode)
        {
            Dictionary<object,object> ldgLookup = ldnScreenNode.Data.Get<Dictionary<object, object>>("ldgAreaNameToLoadScreen");
            foreach(KeyValuePair<object, object> kvpLdgClass in ldgLookup)
            {
                searched++;
                GomObjectData areaLdgInfo = (GomObjectData)kvpLdgClass.Value;
                string loadingScreen = areaLdgInfo.ValueOrDefault<string>("ldgScreenName", string.Empty);
                if(loadingScreen.Length > 0)
                {
                    fileNames.Add("/resources/gfx/loadingscreen/" + loadingScreen + ".dds");
                }

                string loadingOverlay = areaLdgInfo.ValueOrDefault<string>("ldgOverlayName", string.Empty);
                if (loadingOverlay.Length > 0)
                {
                    fileNames.Add("/resources/gfx/gfx_productions/" + loadingOverlay + ".gfx");
                }
            }
        }

        public void ParseMISC_IPP(List<GomObject> ippNodes)
        {
            foreach (GomObject obj in ippNodes)
            {
                searched++;
                string full = obj.Name.ToLower().ToString();
                string partial = obj.Name.ToLower().ToString().Replace("ipp.", "");

                fileNames.Add("/resources/gfx/icons/" + full + ".dds");
                fileNames.Add("/resources/gfx/icons/" + partial + ".dds");

                fileNames.Add("/resources/gfx/mtxstore/" + full + "_120x120.dds");
                fileNames.Add("/resources/gfx/mtxstore/" + full + "_260x260.dds");
                fileNames.Add("/resources/gfx/mtxstore/" + full + "_260x400.dds");
                fileNames.Add("/resources/gfx/mtxstore/" + full + "_328x160.dds");
                fileNames.Add("/resources/gfx/mtxstore/" + full + "_400x400.dds");

                fileNames.Add("/resources/gfx/mtxstore/" + partial + "_120x120.dds");
                fileNames.Add("/resources/gfx/mtxstore/" + partial + "_260x260.dds");
                fileNames.Add("/resources/gfx/mtxstore/" + partial + "_260x400.dds");
                fileNames.Add("/resources/gfx/mtxstore/" + partial + "_328x160.dds");
                fileNames.Add("/resources/gfx/mtxstore/" + partial + "_400x400.dds");
                obj.Unload();
            }
        }

        public void ParseMISC_CDX(List<GomObject> cdxNodes)
        {
            foreach (GomObject obj in cdxNodes)
            {
                searched++;
                string full = obj.Name.ToLower().ToString();
                fileNames.Add("/resources/gfx/codex/" + full + ".dds");
                obj.Unload();
            }
        }

        public void ParseMISC_ITEM(Dictionary<object, object> itemApperances)
        {
            foreach (KeyValuePair<object,object> kvp in itemApperances)
            {
                searched++;
                var itemAppearance = (GomObjectData)kvp.Value;
                string itmModel = itemAppearance.ValueOrDefault<string>("itmModel", null);
                if (itmModel != null)               
                    fileNames.Add(("/resources/" + (itmModel.Replace("\\", "/"))).Replace("//", "/"));
                string itmFxSpec = itemAppearance.ValueOrDefault<string>("itmFxSpec", null);
                if(itmFxSpec != null)
                    fileNames.Add(("/resources/art/fx/fxspec/" + itmFxSpec + ".fxspec").Replace("//", "/").Replace(".fxspec.fxspec", ".fxspec"));
            }
        }

        public void ParseMISC_WORLD(List<GomObject> worldAreas, Dictionary<object, object> worldAreasProto, DataObjectModel currentDom)
        {
            foreach (GomObject obj in worldAreas)
            {
                searched++;
                UInt64 areaId = obj.Data.ValueOrDefault<ulong>("mapDataContainerAreaID", 0);
                if (areaId > 0)
                {
                    worldFileNames.Add(String.Format("/resources/world/areas/{0}/area.dat", areaId.ToString()));
                    worldFileNames.Add(String.Format("/resources/world/areas/{0}/mapnotes.not", areaId.ToString()));

                    List<object> mapPages = obj.Data.ValueOrDefault<List<object>>("mapDataContainerMapDataList", null);

                    if (mapPages != null)
                    {
                        foreach (GomLib.GomObjectData mapPage in mapPages)
                        {
                            string mapName = mapPage.ValueOrDefault<string>("mapName", null);
                            if (!mapNames.ContainsKey(areaId.ToString()))
                                mapNames.Add(areaId.ToString(), new HashSet<string>());
                            mapNames[areaId.ToString()].Add(mapName.ToString());                            
                        }
                        mapPages.Clear();
                    }
                }
                obj.Unload();
            }
            worldAreas.Clear();
                        
            foreach(var gomItm in worldAreasProto)
            {
                GomLib.Models.Area area = new GomLib.Models.Area();
                currentDom.areaLoader.Load(area, (GomLib.GomObjectData)gomItm.Value);
                if (area.Id == 0)
                {
                    if (area.AreaId == 0)
                    {                     
                        continue;
                    }                 
                }
                searched++;
                if (area.MapPages != null)
                {
                    int ii = 0;
                    foreach (var map_page in area.MapPages)
                    {
                        ii++;
                        if (map_page.HasImage == true)
                        {
                            if (!mapNames.ContainsKey(area.AreaId.ToString()))
                                mapNames.Add(area.AreaId.ToString(), new HashSet<string>());
                            mapNames[area.AreaId.ToString()].Add(map_page.MapName);                               
                        }
                    }
                    area.MapPages.Clear();
                }
                if(area.Assets != null)
                    area.Assets.Clear();
            }
            worldAreasProto.Clear();
        }

        public void ParseMISC_TUTORIAL(DataObjectModel currentDom)
        {
            var tutorialTable = currentDom.stringTable.Find("str.gui.tutorials");
            if(tutorialTable != null && tutorialTable.data != null)
            {                
                foreach(KeyValuePair<long, StringTableEntry> item in tutorialTable.data)
                {
                    if(item.Value.localizedText.ContainsKey("enMale"))
                    {
                        string text = item.Value.localizedText["enMale"];
                        if(text.Contains(".dds"))
                        {                               
                            int start = 0;
                            int end = 0;
                            while ((start = text.IndexOf("img://", start)) != -1)
                            {
                                end = text.IndexOf(".dds", start);
                                if(end != -1)
                                {
                                    string temp = text.Substring(start, ((end - start) + 4)).ToLower();
                                    temp = temp.Replace("img://", "/resources/").Replace("//", "/").Replace("<<grammar::locpath>>", "en-us");
                                    fileNames.Add(temp);
                                    start++;
                                }
                            }                            
                            
                        }
                        else if(text.Contains("img://"))
                        {
                            int start = 0;
                            int end = 0;
                            while ((start = text.IndexOf("img://", start)) != -1)
                            {
                                end = text.IndexOf("'", start);
                                if (end != -1)
                                {
                                    string temp = text.Substring(start, ((end - start) + 1)).ToLower();
                                    temp = temp.Replace("img://", "/resources/").Replace("//", "/").Replace("<<grammar::locpath>>", "en-us");
                                    fileNames.Add(temp + ".dds");                                        
                                    start++;                                        
                                }
                            }                                
                        }
                    }
                }                
            }
        }

        public void ParseMISC_NODE(Dictionary<string, DomType> nodeDict)
        {               
            foreach (var obj in nodeDict)
            {
                searched++;
                GomObject node = (GomObject)obj.Value;
                fileNames.Add("/resources/systemgenerated/prototypes/" + node.Id.ToString() + ".node");
                node.Unload();
            }
        }

        public void WriteFile()
        {
            if (!System.IO.Directory.Exists(this.dest + "\\File_Names"))
                System.IO.Directory.CreateDirectory(this.dest + "\\File_Names");

            this.found = this.fileNames.Count();
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

            this.found += this.worldFileNames.Count();
            if (this.worldFileNames.Count > 0)
            {
                System.IO.StreamWriter outputNames = new System.IO.StreamWriter(this.dest + "\\File_Names\\" + this.extension + "_world_file_names_1.txt", false);
                int fileCount = 1;
                int lineCount = 1;
                foreach (string file in worldFileNames)
                {
                    if (lineCount >= 750000)
                    {
                        outputNames.Close();
                        fileCount++;
                        outputNames = new System.IO.StreamWriter(this.dest + "\\File_Names\\" + this.extension + "_world_file_names_" + fileCount + ".txt", false);
                        lineCount = 0;
                    }
                    outputNames.WriteLine(file.Replace("\\", "/"));
                    lineCount++;
                }
                outputNames.Close();
                worldFileNames.Clear();
            }

            this.found += this.mapNames.Count();
            if (this.mapNames.Count > 0)
            {
                System.IO.StreamWriter outputMapNames = new System.IO.StreamWriter(this.dest + "\\File_Names\\" + this.extension + "_world_map_file_names_1.txt", false);
                int fileCount = 1;
                int lineCount = 1;
                foreach (KeyValuePair<string, HashSet<string>> kvp in mapNames)
                {
                    foreach (string line in kvp.Value)
                    {
                        if (lineCount >= 500000)
                        {
                            outputMapNames.Close();
                            fileCount++;
                            outputMapNames = new System.IO.StreamWriter(this.dest + "\\File_Names\\" + this.extension + "_world_map_file_names_" + fileCount + ".txt", false);
                            lineCount = 0;
                        }
                        outputMapNames.WriteLine(String.Format("/resources/world/areas/{0}/{1}_r.dds", kvp.Key, line).Replace("\\", "/").Replace("//", "/"));
                        lineCount++;
                        for (int m = 0; m <= 50; m++)
                        {
                            for (int mm = 0; mm <= 50; mm++)
                            {
                                outputMapNames.WriteLine(String.Format("/resources/world/areas/{0}/minimaps/{1}_{2:00}_{3:00}_r.dds", kvp.Key, line, m, mm).Replace("\\", "/").Replace("//", "/"));
                                lineCount++;
                            }
                        }                        
                    }
                    kvp.Value.Clear();
                }
                outputMapNames.Close();
                mapNames.Clear();
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
