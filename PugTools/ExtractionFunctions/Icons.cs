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
using MessageBox = System.Windows.Forms.MessageBox;

namespace tor_tools
{
    public partial class Tools
    {
       public void getIcons() //needs updating
        {
            Clearlist2();

            LoadData();

            double i = 0;           

            addtolist2("Icon Extract Started");

            System.IO.StreamWriter file2 = new System.IO.StreamWriter(Config.ExtractPath + "icons.txt", true);
           
            addtolist2("Starting Item Icon Extraction");
            var itmList = currentDom.GetObjectsStartingWith("itm.");
            foreach (var gomItm in itmList)
            {
                string icon = gomItm.Data.ValueOrDefault<string>("itmIcon", null);

                if (icon != null)
                {                    
                    file2.WriteLine("/resources/gfx/icons/" + icon + ".dds");
                    file2.WriteLine("/resources/gfx/mtxstore/" + icon + "_120x120.dds");
                    file2.WriteLine("/resources/gfx/mtxstore/" + icon + "_260x400.dds");
                    file2.WriteLine("/resources/gfx/mtxstore/" + icon + "_260x260.dds");
                    file2.WriteLine("/resources/gfx/mtxstore/" + icon + "_328x160.dds");
                    file2.WriteLine("/resources/gfx/mtxstore/" + icon + "_400x400.dds");
                    i++;
                }                
            }
            itmList = null;
            GC.Collect();

            addtolist2("Starting Ability Icon Extraction");
            var ablList = currentDom.GetObjectsStartingWith("abl.");
            foreach (var gomItm in ablList)
            {
                string icon = gomItm.Data.ValueOrDefault<string>("ablIconSpec", null);

                if (icon != null)
                {                    
                    file2.WriteLine("/resources/gfx/icons/" + icon + ".dds");
                    i++;
                }

                icon = gomItm.Data.ValueOrDefault<string>("effIcon", null);

                if (icon != null)
                {
                    file2.WriteLine("/resources/gfx/icons/" + icon + ".dds");
                    i++;
                }                
            }
            ablList = null;
            GC.Collect();

            addtolist2("Starting Quest Icon Extraction");
            var qstList = currentDom.GetObjectsStartingWith("qst.");
            foreach (var gomItm in qstList)
            {
                string icon = gomItm.Data.ValueOrDefault<string>("qstMissionIcon", null);
                if (icon != null)
                {                    
                    file2.WriteLine("/resources/gfx/codex/" + icon + ".dds");
                    i++;
                }                
            }
            qstList = currentDom.GetObjectsStartingWith("invalid");
            GC.Collect();
            
            addtolist2("Starting Item Apperance Icon Extraction");
            var ippList = currentDom.GetObjectsStartingWith("ipp.");
            foreach (var gomItm in ippList)
            {
                string icon = gomItm.Name.ToString();

                if (icon != null)
                {                    
                    file2.WriteLine("/resources/gfx/icons/" + icon + ".dds");
                    file2.WriteLine("/resources/gfx/icons/" + icon.Replace("ipp.", "") + ".dds");

                    file2.WriteLine("/resources/gfx/mtxstore/" + icon + "_120x120.dds");
                    file2.WriteLine("/resources/gfx/mtxstore/" + icon + "_260x260.dds");
                    file2.WriteLine("/resources/gfx/mtxstore/" + icon + "_260x400.dds");
                    file2.WriteLine("/resources/gfx/mtxstore/" + icon + "_328x160.dds");
                    file2.WriteLine("/resources/gfx/mtxstore/" + icon + "_400x400.dds");

                    file2.WriteLine("/resources/gfx/mtxstore/" + icon.Replace("ipp.", "") + "_120x120.dds");
                    file2.WriteLine("/resources/gfx/mtxstore/" + icon.Replace("ipp.", "") + "_260x260.dds");
                    file2.WriteLine("/resources/gfx/mtxstore/" + icon.Replace("ipp.", "") + "_260x400.dds");
                    file2.WriteLine("/resources/gfx/mtxstore/" + icon.Replace("ipp.", "") + "_328x160.dds");
                    file2.WriteLine("/resources/gfx/mtxstore/" + icon.Replace("ipp.", "") + "_400x400.dds");
                    
                    i++;
                }
            }
            GC.Collect();

            addtolist2("Starting Codex Icon/Image Extraction");
            var cdxList = currentDom.GetObjectsStartingWith("cdx.");
            foreach (var gomItm in cdxList)
            {
                string icon = gomItm.Data.ValueOrDefault<string>("cdxImage", null);

                if (icon != null)
                {
                    file2.WriteLine("/resources/gfx/codex/" + icon + ".dds");
                    i++;
                }
            }
            GC.Collect();


            addtolist2("Starting Achievement Icon Extraction");
            var achList = currentDom.GetObjectsStartingWith("ach.");
            foreach (var gomItm in achList)
            {
                string icon = gomItm.Data.ValueOrDefault<string>("achIcon", null);
                if (icon != null)
                {                                        
                    file2.WriteLine("/resources/gfx/icons/" + icon + ".dds");
                    i++;
                }                
            }
            GC.Collect();

            addtolist2("Starting Talent Icon Extraction");
            var talList = currentDom.GetObjectsStartingWith("tal.");
            foreach (var gomItm in talList)
            {
                string icon = gomItm.Data.ValueOrDefault<string>("talTalentIcon", null);
                if (icon != null)
                {
                    file2.WriteLine("/resources/gfx/icons/" + icon + ".dds");
                    i++;
                }
            }
            GC.Collect();


            addtolist2("Starting Space PVP Icon Extraction");
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

           foreach(string cmp in spvpIcons)
           {
               var spvpList1 = currentDom.GetObjectsStartingWith(cmp);
                foreach (var gomItm in spvpList1)
                {
                    string icon = gomItm.Data.ValueOrDefault<string>("scFFComponentIcon", null);
                    if (icon != null)
                    {
                        file2.WriteLine("/resources/gfx/icons/" + icon + ".dds");
                        i++;
                    }
                }
                GC.Collect();
           }

           GomLib.GomObject shipDataProto = currentDom.GetObject("scFFShipsDataPrototype");
           Dictionary<object, object> shipData = shipDataProto.Data.ValueOrDefault<Dictionary<object, object>>("scFFShipsData", null);
           if (shipData != null)
           {
               foreach (var item in (Dictionary<object, object>)shipData)
               {
                   var item2 = (GomLib.GomObjectData)item.Value;
                   object icon1_string;
                   object icon2_string;
                   item2.Dictionary.TryGetValue("scFFShipHullIcon", out icon1_string);
                   item2.Dictionary.TryGetValue("scFFShipIcon", out icon2_string); // This doesn't appear anywhere in this prototype. What was this from?
                   if (icon1_string != null)
                   {
                       file2.WriteLine("/resources/gfx/icons/" + icon1_string + ".dds");
                       file2.WriteLine("/resources/gfx/textures/" + icon1_string + ".dds");
                       i++;
                   }

                   if (icon2_string != null)
                   {
                       file2.WriteLine("/resources/gfx/icons/" + icon2_string + ".dds");
                       file2.WriteLine("/resources/gfx/textures/" + icon2_string + ".dds");
                       i++;
                   }
               }
           }
           GC.Collect();

           GomLib.GomObject shipColorOptionProto = currentDom.GetObject("scFFColorOptionMasterPrototype");
           Dictionary<object, object> shipColors = shipColorOptionProto.Data.ValueOrDefault<Dictionary<object, object>>("scFFComponentColorUIData", null);
           if (shipColors != null)
           {
               foreach (var item in (Dictionary<object, object>)shipColors)
               {
                   var item2 = (GomLib.GomObjectData)item.Value;
                   object icon_string;
                   item2.Dictionary.TryGetValue("scFFComponentColorIcon", out icon_string);

                   if (icon_string != null)
                   {
                       file2.WriteLine("/resources/gfx/icons/" + icon_string + ".dds");
                       file2.WriteLine("/resources/gfx/textures/" + icon_string + ".dds");
                       i++;
                   }
               }
           }
           GC.Collect();
           
           GomLib.GomObject scffCrewProto = currentDom.GetObject("scffCrewPrototype");
           Dictionary<object, object> shipCrew = scffCrewProto.Data.ValueOrDefault<Dictionary<object, object>>("scFFShipsCrewAndPatternData", null);
           if (shipCrew != null)
           {
               foreach (var item in (Dictionary<object, object>)shipCrew)
               {
                   var item2 = (GomLib.GomObjectData)item.Value;
                   object icon_string;
                   item2.Dictionary.TryGetValue("scFFCrewIcon", out icon_string);

                   if (icon_string != null)
                   {
                       file2.WriteLine("/resources/gfx/icons/" + icon_string + ".dds");
                       file2.WriteLine("/resources/gfx/textures/" + icon_string + ".dds");
                       i++;
                   }
               }
           }
           GC.Collect();


           addtolist2("Starting MTX Store Icon Extraction");
           GomLib.GomObject mtxStore = currentDom.GetObject("mtxStorefrontInfoPrototype");
           Dictionary<object, object> mtxItems = mtxStore.Data.ValueOrDefault<Dictionary<object, object>>("mtxStorefrontData", null);
           if (mtxItems != null)
           {
               foreach (var item in (Dictionary<object, object>)mtxItems)
               {
                   var item2 = (GomLib.GomObjectData)item.Value;
                   object icon_string;
                   item2.Dictionary.TryGetValue("mtxStorefrontIcon", out icon_string);
                   if (icon_string != null)
                   {
                       string icon = icon_string.ToString().ToLower();
                       file2.WriteLine("/resources/gfx/icons/" + icon + ".dds");
                       file2.WriteLine("/resources/gfx/mtxstore/" + icon + "_120x120.dds");
                       file2.WriteLine("/resources/gfx/mtxstore/" + icon + "_260x400.dds");
                       file2.WriteLine("/resources/gfx/mtxstore/" + icon + "_260x260.dds");
                       file2.WriteLine("/resources/gfx/mtxstore/" + icon + "_328x160.dds");
                       file2.WriteLine("/resources/gfx/mtxstore/" + icon + "_400x400.dds");
                       i++;
                   }                   
               }
           }
           GC.Collect();

           GomLib.GomObject colCategoriesProto = currentDom.GetObject("colCollectionCategoriesPrototype");
           Dictionary<object, object> colCats = colCategoriesProto.Data.ValueOrDefault<Dictionary<object, object>>("colCollectionCategoryData", null);
           if (colCats != null)
           {
               foreach (var item in (Dictionary<object, object>)colCats)
               {
                   var item2 = (GomLib.GomObjectData)item.Value;
                   object icon_string;
                   item2.Dictionary.TryGetValue("colCollectionCategoryIcon", out icon_string);
                   if (icon_string != null)
                   {
                       string icon = icon_string.ToString().ToLower();
                       file2.WriteLine("/resources/gfx/icons/" + icon + ".dds");
                       file2.WriteLine("/resources/gfx/mtxstore/" + icon + "_120x120.dds");
                       file2.WriteLine("/resources/gfx/mtxstore/" + icon + "_260x400.dds");
                       file2.WriteLine("/resources/gfx/mtxstore/" + icon + "_260x260.dds");
                       file2.WriteLine("/resources/gfx/mtxstore/" + icon + "_328x160.dds");
                       file2.WriteLine("/resources/gfx/mtxstore/" + icon + "_400x400.dds");
                       i++;
                   }
               }
           }
           GC.Collect();

           GomLib.GomObject colCollectionItemsProto = currentDom.GetObject("colCollectionItemsPrototype");
           Dictionary<object, object> colItems = colCollectionItemsProto.Data.ValueOrDefault<Dictionary<object, object>>("colCollectionItemsData", null);
           if (colItems != null)
           {
               foreach (var item in (Dictionary<object, object>)colItems)
               {
                   var item2 = (GomLib.GomObjectData)item.Value;
                   object icon_string;
                   item2.Dictionary.TryGetValue("colCollectionIcon", out icon_string);
                   if (icon_string != null)
                   {
                       string icon = icon_string.ToString().ToLower();
                       file2.WriteLine("/resources/gfx/icons/" + icon + ".dds");
                       file2.WriteLine("/resources/gfx/mtxstore/" + icon + "_120x120.dds");
                       file2.WriteLine("/resources/gfx/mtxstore/" + icon + "_260x400.dds");
                       file2.WriteLine("/resources/gfx/mtxstore/" + icon + "_260x260.dds");
                       file2.WriteLine("/resources/gfx/mtxstore/" + icon + "_328x160.dds");
                       file2.WriteLine("/resources/gfx/mtxstore/" + icon + "_400x400.dds");
                       i++;
                   }
               }
           }
           GC.Collect();

           GomLib.GomObject achCategoriesTable_Proto = currentDom.GetObject("achCategoriesTable_Prototype");
           Dictionary<object, object> achCategories = achCategoriesTable_Proto.Data.ValueOrDefault<Dictionary<object, object>>("achCategoriesData", null);
           if (achCategories != null)
           {
               foreach (var item in (Dictionary<object, object>)achCategories)
               {
                   var item2 = (GomLib.GomObjectData)item.Value;
                   object icon_string1;
                   object icon_string2;
                   item2.Dictionary.TryGetValue("achCategoriesIcon", out icon_string1);
                   if (icon_string1 != null)
                   {
                       string icon = icon_string1.ToString().ToLower();
                       file2.WriteLine("/resources/gfx/icons/" + icon + ".dds");
                   }
                   item2.Dictionary.TryGetValue("achCategoriesCodexIcon", out icon_string2);
                   if (icon_string2 != null)
                   {
                       string icon = icon_string2.ToString().ToLower();
                       file2.WriteLine("/resources/gfx/codex/" + icon + ".dds");
                   }
               }
           }
           GC.Collect();

         

            addtolist2("Icon Extract Completed");
            file2.Close();

            addtolist("the icon lists has been generated there are " + i + " icons");
            MessageBox.Show("the icon lists has been generated there are " + i + " icons");
            EnableButtons();
        }
    }
}
