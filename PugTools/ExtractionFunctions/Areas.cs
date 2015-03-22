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
        #region Deprecated
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

        /* code moved to GomLib.Models.Area.cs */

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

        #endregion
        private XElement SortMapAreas(XElement areas)
        {
            //addtolist("Sorting Abilities");
            areas.ReplaceNodes(areas.Elements("Area")
                .OrderBy(x => (string)x.Attribute("Status"))
                .ThenBy(x => (string)x.Element("Name")));            
            return areas;
        }

        /* code moved to GomLib.Models.Area.cs */

    }
}
