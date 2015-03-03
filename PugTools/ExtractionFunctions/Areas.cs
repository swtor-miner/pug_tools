using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Net;
using System.Windows;
using System.Threading;
using Microsoft.Win32;
using MySql.Data.MySqlClient;
using System.Text.RegularExpressions;
using System.Xml;
using System.IO;
using System.Xml.Linq;
using System.Xml.Serialization;

namespace tor_tools
{
    public partial class Tools
    {
        public void getAreas()
        {
            Clearlist2();

            LoadData();

            var areaList = currentDom.GetObject("mapAreasDataProto").Data.Get<Dictionary<object, object>>("mapAreasDataObjectList");
            
            double ttl = areaList.Count();
            bool append = false;
            bool addedChanged = false;
            string changed = "";
            if(chkBuildCompare.Checked)
            {
                addedChanged = true;
                changed = "Changed";
            }
            var filename = changed + "Areas.xml";
            if(outputTypeName == "Text")
            {
                filename = changed + "Areas.txt";
                string generatedContent = AreaDataFromPrototype(areaList);
                WriteFile(generatedContent, filename, append);
            }
            else
            {
                XDocument xmlContent = new XDocument(AreaDataFromPrototypeAsXElement(areaList, addedChanged));
                WriteFile(xmlContent, filename, append);
            }

            //MessageBox.Show("the Ability lists has been generated there are " + ttl + " Abilities.");
            EnableButtons();
        }
                
        private string AreaDataFromPrototype(Dictionary<object, object> areaList)
        {
            double i = 0;
            string n = Environment.NewLine;

            var txtFile = new StringBuilder();
            foreach (var gomItm in areaList)
            {                
                GomLib.Models.Area area = new GomLib.Models.Area();                
                currentDom.areaLoader.Load(area, (GomLib.GomObjectData)gomItm.Value);
                if (area.Id == 0)
                {
                    continue;
                }
               
                addtolist2("Area Name: " + area.Name);

                txtFile.Append("Name: " + area.Name + n);
                txtFile.Append("AreaId: " + area.AreaId + n);
                txtFile.Append("Chat Zone Name: " + area.ZoneName + n);
                if (area.MapPages != null)
                {
                    int ii = 0;
                    foreach (var map_page in area.MapPages)
                    {
                        ii++;
                        txtFile.Append("\tMap Pages " + ii + " Name: " + map_page.Name + n);
                        txtFile.Append("\tMap Pages " + ii + " Map Name: " + map_page.MapName + n);
                        txtFile.Append("\tMap Pages " + ii + " Id: " + map_page.Id + n);
                        if (map_page.HasImage == true)
                        {
                            txtFile.Append("\tMap Pages " + ii + " Image Path: " + String.Format("/resources/world/areas/{0}/{1}_r.dds", area.AreaId, map_page.MapName) + n);
                        }
                    }
                        
                }
                i++;
                //sqlexec("INSERT INTO `areas` (`ability_idc`, `quest_name`, `quest_nodeid`, `quest_id`, `IsBonus`, `BonusShareable`, `Branches`, `CanAbandon`, `Category`, `CategoryId`, `Classes`, `Difficulty`, `Fqn`, `Icon`, `IsClassQuest`, `IsHidden`, `IsRepeatable`, `Items`, `RequiredLevel`, `XpLevel`) VALUES (NULL, '" + name + "', '" + itm.NodeId + "', '" + itm.Id + "', '" + itm.IsBonus + "', '" + itm.BonusShareable + "', '" + itm.Branches + "', '" + itm.CanAbandon + "', '" + itm.Category + "', '" + itm.CategoryId + "', '" + itm.Classes + "', '" + itm.Difficulty + "', '" + itm.Fqn + "', '" + itm.Icon + "', '" + itm.IsClassQuest + "', '" + itm.IsHidden + "', '" + itm.IsRepeatable + "', '" + itm.Items + "', '" + itm.RequiredLevel + "', '" + itm.XpLevel + "');");
            }
            addtolist("the area lists has been generated there are " + i + " Areas");
            return txtFile.ToString();
        }

        
        private XElement AreaDataFromPrototypeAsXElement(Dictionary<object, object> areaList, bool addedChangedOnly)
        {
            double i = 0;
            //double e = 0;
            XElement areas = new XElement("Areas");
            foreach (var gomItm in areaList)
            {
                GomLib.Models.Area area = new GomLib.Models.Area();
                currentDom.areaLoader.Load(area, (GomLib.GomObjectData)gomItm.Value);
                if (area.Id == 0)
                {
                    if (area.AreaId != 0)
                    {
                        area.Id = (int)area.AreaId;
                    }
                    else continue;
                }

                addtolist2("Area Name: " + area.Name);

                areas.Add(AreaAsXElement(area));

                if (area.Name != null)
                {
                    string name = area.Name.Replace("'", " ");
                }
                //sqlexec("INSERT INTO `areas` (`ability_idc`, `quest_name`, `quest_nodeid`, `quest_id`, `IsBonus`, `BonusShareable`, `Branches`, `CanAbandon`, `Category`, `CategoryId`, `Classes`, `Difficulty`, `Fqn`, `Icon`, `IsClassQuest`, `IsHidden`, `IsRepeatable`, `Items`, `RequiredLevel`, `XpLevel`) VALUES (NULL, '" + name + "', '" + itm.NodeId + "', '" + itm.Id + "', '" + itm.IsBonus + "', '" + itm.BonusShareable + "', '" + itm.Branches + "', '" + itm.CanAbandon + "', '" + itm.Category + "', '" + itm.CategoryId + "', '" + itm.Classes + "', '" + itm.Difficulty + "', '" + itm.Fqn + "', '" + itm.Icon + "', '" + itm.IsClassQuest + "', '" + itm.IsHidden + "', '" + itm.IsRepeatable + "', '" + itm.Items + "', '" + itm.RequiredLevel + "', '" + itm.XpLevel + "');");
                i++;
            }

            if (addedChangedOnly)
            {
                //addtolist("Comparing the Current Areas to the loaded Patch");

                XElement addedAreas = FindChangedEntries(areas, "Areas", "Area");
                addedAreas = SortMapAreas(addedAreas);
                addtolist("The Area list has been generated there are " + addedAreas.Elements("Area").Count() + " new/changed Areas");
                areas = null;
                return addedAreas;
            }

            areas = SortMapAreas(areas);
            addtolist("The Area list has been generated there are " + i + " Areas");
            return areas;
        }

        public void getAreaFilenames()
        {
            Clearlist2();
            
            LoadData();

            var areaList = currentDom.GetObject("mapAreasDataProto").Data.Get<Dictionary<object, object>>("mapAreasDataObjectList");

            double ttl = areaList.Count();
            bool append = false;
            bool addedChanged = false;
            string changed = "";
            if(chkBuildCompare.Checked)
            {
                addedChanged = true;
                changed = "Changed";
            }
            var filename = changed + "Areas.xml";
            if(outputTypeName == "Text")
            {
                filename = changed + "Areas.txt";
                List<string> generatedContent = AreaFileNamesFromPrototype(areaList);
                foreach (string line in generatedContent)
                {
                    WriteFile(line, filename, true);
                }
            }
            else
            {
                XDocument xmlContent = new XDocument(AreaDataFromPrototypeAsXElement(areaList, addedChanged));
                WriteFile(xmlContent, filename, append);
            }

            //MessageBox.Show("the Ability lists has been generated there are " + ttl + " Abilities.");
            EnableButtons();
        }

        private List<string> AreaFileNamesFromPrototype(Dictionary<object, object> areaList)
        {
            double i = 0;
            string n = Environment.NewLine;

            List<string> areas = new List<string>();
            if (!System.IO.Directory.Exists(Config.ExtractPath + prefix)) { System.IO.Directory.CreateDirectory(Config.ExtractPath + prefix); }
            int file_count = 1;
            System.IO.StreamWriter file2 = new System.IO.StreamWriter(Config.ExtractPath + prefix + "Area_Filenames_" + file_count + ".txt", false);
            int line_count = 0;
            int unknown_area = 0;
            foreach (var gomItm in areaList)
            {
                if (line_count >= 750000)
                {
                    line_count = 0;
                    file2.Close();
                    file_count++;
                    file2 = new System.IO.StreamWriter(Config.ExtractPath + prefix + "Area_Filenames_" + file_count + ".txt", false);
                }                
                GomLib.Models.Area area = new GomLib.Models.Area();
                currentDom.areaLoader.Load(area, (GomLib.GomObjectData)gomItm.Value);
                if (area.Id == 0)
                {
                    if (area.AreaId == 0)
                    {
                        Console.WriteLine("Skipping Area");
                        continue;
                    }
                    else
                    {
                        unknown_area++;
                        area.Id = unknown_area;
                    }
                }

                addtolist2("Area Name: " + area.Name);
                if (area.MapPages != null)
                {
                    int ii = 0;
                    foreach (var map_page in area.MapPages)
                    {
                        ii++;                        
                        if (map_page.HasImage == true)
                        {
                            file2.Write(String.Format("/resources/world/areas/{0}/{1}_r.dds", area.AreaId, map_page.MapName) + n);
                            line_count++;
                            for (int m = 0; m <= 120; m++)
                            {
                                for (int mm = 0; mm <= 60; mm++)
                                {
                                    file2.Write(String.Format("/resources/world/areas/{0}/minimaps/{1}_{2:00}_{3:00}_r.dds", area.AreaId, map_page.MapName, m, mm) + n);
                                    line_count++;
                                }
                            }
                        }
                    }                    
                }
                i++;                
            }
            file2.Close();
            addtolist("the area lists has been generated there are " + i + " Areas");
            return areas; //.ToString();
        }

        private XElement SortMapAreas(XElement areas)
        {
            //addtolist("Sorting Abilities");
            areas.ReplaceNodes(areas.Elements("Area")
                .OrderBy(x => (string)x.Attribute("Status"))
                .ThenBy(x => (string)x.Element("Name")));            
            return areas;
        }
      
        public static XElement AreaAsXElement(GomLib.Models.Area area)
        {
            XElement Area = new XElement("Area");
            if (area != null)
            {
                if (verbose)
                {
                    //XElement MapPages = new XElement("MapPages");
                    Area.Add(new XAttribute("Id", area.Id),
                        new XElement("Name", area.Name),
                        new XElement("ChatZone", area.ZoneName),
                        //new XElement("CommentableId",   area.CommentableId), //this is randomly generated and has no meaning.
                        new XElement("DisplayNameId", area.DisplayNameId));
                    string ImagePath = null;
                    if (area.MapPages != null)
                    {
                        foreach (var map_page in area.MapPages)
                        {
                            if(map_page.HasImage){
                                ImagePath = String.Format("/resources/world/areas/{0}/{1}_r.dds", area.AreaId, map_page.MapName);
                            }                            
                            //MapPages.Add(
                            Area.Add(
                                    new XElement("MapPage", 
                                        new XAttribute("Id", map_page.Id),
                                        new XElement("Name", map_page.Name),
                                        new XElement("MapName", map_page.MapName),
                                        new XElement("Guid", map_page.Guid),
                                        new XElement("HasImage", map_page.HasImage),
                                        new XElement("ImagePath", ImagePath),
                                        new XElement("IsHeroic", map_page.IsHeroic),
                                        new XElement("MaxX", map_page.MaxX),
                                        new XElement("MaxY", map_page.MaxY),
                                        new XElement("MaxZ", map_page.MaxZ),
                                        new XElement("MiniMapMaxX", map_page.MiniMapMaxX),
                                        new XElement("MiniMapMaxZ", map_page.MiniMapMaxZ),                                        
                                        new XElement("MiniMapMinX", map_page.MiniMapMinX),
                                        new XElement("MiniMapMinZ", map_page.MiniMapMinZ),
                                        new XElement("MiniMapVolume", map_page.MiniMapVolume),
                                        new XElement("MinX", map_page.MinX),
                                        new XElement("MinY", map_page.MinY),
                                        new XElement("MinZ", map_page.MinZ),
                                        new XElement("MountAllowed", map_page.MountAllowed),
                                        new XElement("Tag", map_page.Tag),
                                        new XElement("Volume", map_page.Volume)
                            ));
                        }
                        
                    }
                    if (area.Assets.Count > 0)
                    {
                        XElement assets = new XElement("Assets");
                        foreach (var kvp in area.Assets)
                        {
                            assets.Add(new XElement("Asset",
                                kvp.Value,
                                new XAttribute("Id", kvp.Key)));
                        }
                        Area.Add(assets);
                    }
                }
                else
                {
                    Area.Add(new XAttribute("Id", area.Id),
                        new XElement("Name",   area.Name),
                        new XElement("ChatZone",   area.ZoneName)
                    );                    
                }
            }
            return Area;
        }
    }
}
