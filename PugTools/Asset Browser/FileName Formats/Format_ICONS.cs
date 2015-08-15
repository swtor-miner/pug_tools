using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using TorLib;
using GomLib;

namespace tor_tools
{
    class Format_ICONS
    {
        public string dest = "";
        public HashSet<string> fileNames = new HashSet<string>();        
        public List<string> errors = new List<string>();        
        public string extension;
        public int searched;

        public Format_ICONS(string dest, string ext)
        {
            this.dest = dest;
            this.extension = ext;
        }        

        public void parseICONS(DataObjectModel currentDom)
        {   
            var itmList = currentDom.GetObjectsStartingWith("itm.");
            foreach (var gomItm in itmList)
            {
                searched++;
                string icon = gomItm.Data.ValueOrDefault<string>("itmIcon", null);                
                if (icon != null)
                {
                    fileNames.Add("/resources/gfx/icons/" + icon + ".dds");
                    fileNames.Add("/resources/gfx/mtxstore/" + icon + "_120x120.dds");
                    fileNames.Add("/resources/gfx/mtxstore/" + icon + "_260x400.dds");
                    fileNames.Add("/resources/gfx/mtxstore/" + icon + "_260x260.dds");
                    fileNames.Add("/resources/gfx/mtxstore/" + icon + "_328x160.dds");
                    fileNames.Add("/resources/gfx/mtxstore/" + icon + "_400x400.dds");                    
                }
                gomItm.Unload();
            }
            itmList.Clear();            
            
            var ablList = currentDom.GetObjectsStartingWith("abl.");
            foreach (var gomItm in ablList)
            {
                searched++;
                string icon = gomItm.Data.ValueOrDefault<string>("ablIconSpec", null);
                if (icon != null)                
                    fileNames.Add("/resources/gfx/icons/" + icon + ".dds");

                icon = gomItm.Data.ValueOrDefault<string>("effIcon", null);                
                if (icon != null)                
                    fileNames.Add("/resources/gfx/icons/" + icon + ".dds");
                gomItm.Unload();
            }
            ablList.Clear();            
            
            var qstList = currentDom.GetObjectsStartingWith("qst.");
            foreach (var gomItm in qstList)
            {
                searched++;
                string icon = gomItm.Data.ValueOrDefault<string>("qstMissionIcon", null);                
                if (icon != null)                
                    fileNames.Add("/resources/gfx/codex/" + icon + ".dds");
                gomItm.Unload();
            }
            qstList.Clear();            
                        
            var ippList = currentDom.GetObjectsStartingWith("ipp.");
            foreach (var gomItm in ippList)
            {
                searched++;
                string icon = gomItm.Name.ToString();                
                if (icon != null)
                {
                    fileNames.Add("/resources/gfx/icons/" + icon + ".dds");
                    fileNames.Add("/resources/gfx/icons/" + icon.Replace("ipp.", "") + ".dds");

                    fileNames.Add("/resources/gfx/mtxstore/" + icon + "_120x120.dds");
                    fileNames.Add("/resources/gfx/mtxstore/" + icon + "_260x260.dds");
                    fileNames.Add("/resources/gfx/mtxstore/" + icon + "_260x400.dds");
                    fileNames.Add("/resources/gfx/mtxstore/" + icon + "_328x160.dds");
                    fileNames.Add("/resources/gfx/mtxstore/" + icon + "_400x400.dds");

                    fileNames.Add("/resources/gfx/mtxstore/" + icon.Replace("ipp.", "") + "_120x120.dds");
                    fileNames.Add("/resources/gfx/mtxstore/" + icon.Replace("ipp.", "") + "_260x260.dds");
                    fileNames.Add("/resources/gfx/mtxstore/" + icon.Replace("ipp.", "") + "_260x400.dds");
                    fileNames.Add("/resources/gfx/mtxstore/" + icon.Replace("ipp.", "") + "_328x160.dds");
                    fileNames.Add("/resources/gfx/mtxstore/" + icon.Replace("ipp.", "") + "_400x400.dds");
                }
                gomItm.Unload();
            }
            ippList.Clear();
                        
            var cdxList = currentDom.GetObjectsStartingWith("cdx.");
            foreach (var gomItm in cdxList)
            {
                searched++;
                string icon = gomItm.Data.ValueOrDefault<string>("cdxImage", null);                
                if (icon != null)            
                    fileNames.Add("/resources/gfx/codex/" + icon + ".dds");
                gomItm.Unload();
            }
            cdxList.Clear();
                        
            var achList = currentDom.GetObjectsStartingWith("ach.");
            foreach (var gomItm in achList)
            {
                searched++;
                string icon = gomItm.Data.ValueOrDefault<string>("achIcon", null);                
                if (icon != null)                
                    fileNames.Add("/resources/gfx/icons/" + icon + ".dds");
                gomItm.Unload();
            }
            achList.Clear();
                        
            var talList = currentDom.GetObjectsStartingWith("tal.");
            foreach (var gomItm in talList)
            {
                searched++;
                string icon = gomItm.Data.ValueOrDefault<string>("talTalentIcon", null);                
                if (icon != null)                
                    fileNames.Add("/resources/gfx/icons/" + icon + ".dds");
                gomItm.Unload();
            }
            talList.Clear();
                        
            List<string> spvpIcons = new List<string>();
            spvpIcons.Add("armor_");
            spvpIcons.Add("capacitor_");
            spvpIcons.Add("eng_");
            spvpIcons.Add("magazine_");
            spvpIcons.Add("pweap_");
            spvpIcons.Add("reactor_");
            spvpIcons.Add("sensor_");
            spvpIcons.Add("shield_");
            spvpIcons.Add("sweap_");
            spvpIcons.Add("sys_");
            spvpIcons.Add("thruster_");

            foreach (string cmp in spvpIcons)
            {
                var spvpList1 = currentDom.GetObjectsStartingWith(cmp);
                foreach (var gomItm in spvpList1)
                {
                    searched++;
                    string icon = gomItm.Data.ValueOrDefault<string>("scFFComponentIcon", null);                    
                    if (icon != null)
                        fileNames.Add("/resources/gfx/icons/" + icon + ".dds");
                    gomItm.Unload();
                }
                spvpList1.Clear();
            }
            spvpIcons.Clear();

            GomLib.GomObject shipDataProto = currentDom.GetObject("scFFShipsDataPrototype");
            Dictionary<object, object> shipData = shipDataProto.Data.ValueOrDefault<Dictionary<object, object>>("scFFShipsData", null);
            if (shipData != null)
            {
                foreach (var item in (Dictionary<object, object>)shipData)
                {
                    searched++;
                    var item2 = (GomLib.GomObjectData)item.Value;
                    object icon1_string;
                    object icon2_string;
                    item2.Dictionary.TryGetValue("scFFShipHullIcon", out icon1_string);
                    item2.Dictionary.TryGetValue("scFFShipIcon", out icon2_string); 
                    if (icon1_string != null)
                    {
                        fileNames.Add("/resources/gfx/icons/" + icon1_string + ".dds");
                        fileNames.Add("/resources/gfx/textures/" + icon1_string + ".dds");                        
                    }
                    if (icon2_string != null)
                    {
                        fileNames.Add("/resources/gfx/icons/" + icon2_string + ".dds");
                        fileNames.Add("/resources/gfx/textures/" + icon2_string + ".dds");                        
                    }                    
                }
                shipData.Clear();
            }
            shipDataProto.Unload();

            GomLib.GomObject shipColorOptionProto = currentDom.GetObject("scFFColorOptionMasterPrototype");
            Dictionary<object, object> shipColors = shipColorOptionProto.Data.ValueOrDefault<Dictionary<object, object>>("scFFComponentColorUIData", null);
            if (shipColors != null)
            {
                foreach (var item in (Dictionary<object, object>)shipColors)
                {
                    searched++;
                    var item2 = (GomLib.GomObjectData)item.Value;
                    object icon_string;
                    item2.Dictionary.TryGetValue("scFFComponentColorIcon", out icon_string);
                    if (icon_string != null)
                    {
                        fileNames.Add("/resources/gfx/icons/" + icon_string + ".dds");
                        fileNames.Add("/resources/gfx/textures/" + icon_string + ".dds");                        
                    }
                }
                shipColors.Clear();
            }
            shipColorOptionProto.Unload();            

            GomLib.GomObject scffCrewProto = currentDom.GetObject("scffCrewPrototype");
            Dictionary<object, object> shipCrew = scffCrewProto.Data.ValueOrDefault<Dictionary<object, object>>("scFFShipsCrewAndPatternData", null);
            if (shipCrew != null)
            {
                foreach (var item in (Dictionary<object, object>)shipCrew)
                {
                    searched++;
                    var item2 = (GomLib.GomObjectData)item.Value;
                    object icon_string;
                    item2.Dictionary.TryGetValue("scFFCrewIcon", out icon_string);
                    if (icon_string != null)
                    {
                        fileNames.Add("/resources/gfx/icons/" + icon_string + ".dds");
                        fileNames.Add("/resources/gfx/textures/" + icon_string + ".dds");                        
                    }
                }
                shipCrew.Clear();
            }
            scffCrewProto.Unload();
            
            GomLib.GomObject mtxStore = currentDom.GetObject("mtxStorefrontInfoPrototype");
            Dictionary<object, object> mtxItems = mtxStore.Data.ValueOrDefault<Dictionary<object, object>>("mtxStorefrontData", null);
            if (mtxItems != null)
            {
                foreach (var item in (Dictionary<object, object>)mtxItems)
                {
                    searched++;
                    var item2 = (GomLib.GomObjectData)item.Value;
                    object icon_string;
                    item2.Dictionary.TryGetValue("mtxStorefrontIcon", out icon_string);
                    if (icon_string != null)
                    {
                        string icon = icon_string.ToString().ToLower();
                        fileNames.Add("/resources/gfx/icons/" + icon + ".dds");
                        fileNames.Add("/resources/gfx/mtxstore/" + icon + "_120x120.dds");
                        fileNames.Add("/resources/gfx/mtxstore/" + icon + "_260x400.dds");
                        fileNames.Add("/resources/gfx/mtxstore/" + icon + "_260x260.dds");
                        fileNames.Add("/resources/gfx/mtxstore/" + icon + "_328x160.dds");
                        fileNames.Add("/resources/gfx/mtxstore/" + icon + "_400x400.dds");                        
                    }
                }
                mtxItems.Clear();
            }
            mtxStore.Unload();

            GomLib.GomObject colCategoriesProto = currentDom.GetObject("colCollectionCategoriesPrototype");
            Dictionary<object, object> colCats = colCategoriesProto.Data.ValueOrDefault<Dictionary<object, object>>("colCollectionCategoryData", null);
            if (colCats != null)
            {
                foreach (var item in (Dictionary<object, object>)colCats)
                {
                    searched++;
                    var item2 = (GomLib.GomObjectData)item.Value;
                    object icon_string;
                    item2.Dictionary.TryGetValue("colCollectionCategoryIcon", out icon_string);
                    if (icon_string != null)
                    {
                        string icon = icon_string.ToString().ToLower();
                        fileNames.Add("/resources/gfx/icons/" + icon + ".dds");
                        fileNames.Add("/resources/gfx/mtxstore/" + icon + "_120x120.dds");
                        fileNames.Add("/resources/gfx/mtxstore/" + icon + "_260x400.dds");
                        fileNames.Add("/resources/gfx/mtxstore/" + icon + "_260x260.dds");
                        fileNames.Add("/resources/gfx/mtxstore/" + icon + "_328x160.dds");
                        fileNames.Add("/resources/gfx/mtxstore/" + icon + "_400x400.dds");                        
                    }
                }
                colCats.Clear();
            }
            colCategoriesProto.Unload();

            GomLib.GomObject colCollectionItemsProto = currentDom.GetObject("colCollectionItemsPrototype");
            Dictionary<object, object> colItems = colCollectionItemsProto.Data.ValueOrDefault<Dictionary<object, object>>("colCollectionItemsData", null);
            if (colItems != null)
            {
                foreach (var item in (Dictionary<object, object>)colItems)
                {
                    searched++;
                    var item2 = (GomLib.GomObjectData)item.Value;
                    object icon_string;
                    item2.Dictionary.TryGetValue("colCollectionIcon", out icon_string);
                    if (icon_string != null)
                    {
                        string icon = icon_string.ToString().ToLower();
                        fileNames.Add("/resources/gfx/icons/" + icon + ".dds");
                        fileNames.Add("/resources/gfx/mtxstore/" + icon + "_120x120.dds");
                        fileNames.Add("/resources/gfx/mtxstore/" + icon + "_260x400.dds");
                        fileNames.Add("/resources/gfx/mtxstore/" + icon + "_260x260.dds");
                        fileNames.Add("/resources/gfx/mtxstore/" + icon + "_328x160.dds");
                        fileNames.Add("/resources/gfx/mtxstore/" + icon + "_400x400.dds");                        
                    }
                }
                colItems.Clear();
            }
            colCollectionItemsProto.Unload();

            GomLib.GomObject achCategoriesTable_Proto = currentDom.GetObject("achCategoriesTable_Prototype");
            Dictionary<object, object> achCategories = achCategoriesTable_Proto.Data.ValueOrDefault<Dictionary<object, object>>("achCategoriesData", null);
            if (achCategories != null)
            {
                foreach (var item in (Dictionary<object, object>)achCategories)
                {
                    searched++;
                    var item2 = (GomLib.GomObjectData)item.Value;
                    object icon_string1;
                    object icon_string2;
                    item2.Dictionary.TryGetValue("achCategoriesIcon", out icon_string1);
                    if (icon_string1 != null)
                    {
                        string icon = icon_string1.ToString().ToLower();
                        fileNames.Add("/resources/gfx/icons/" + icon + ".dds");
                    }
                    item2.Dictionary.TryGetValue("achCategoriesCodexIcon", out icon_string2);
                    if (icon_string2 != null)
                    {
                        string icon = icon_string2.ToString().ToLower();
                        fileNames.Add("/resources/gfx/codex/" + icon + ".dds");
                    }
                }
                achCategories.Clear();
            }
            achCategoriesTable_Proto.Unload();
        } 

        public void WriteFile(bool outputAllDirs = false)
        {
            if (!System.IO.Directory.Exists(this.dest + "\\File_Names"))
                System.IO.Directory.CreateDirectory(this.dest + "\\File_Names");
            if (this.fileNames.Count > 0)
            {
                System.IO.StreamWriter outputAnimFileNames = new System.IO.StreamWriter(this.dest + "\\File_Names\\" + this.extension + "_file_names.txt", false);
                foreach (string item in fileNames)
                {
                    if (item != "")
                        outputAnimFileNames.WriteLine(item);  
                }
                outputAnimFileNames.Close();
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
