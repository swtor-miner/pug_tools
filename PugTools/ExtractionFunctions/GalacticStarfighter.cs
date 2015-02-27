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
using GomLib;
using Newtonsoft.Json;
using MessageBox = System.Windows.Forms.MessageBox;

namespace tor_tools
{
    public partial class Tools
    {
        #region Txt

        private void ShipDataFromPrototype(Dictionary<object, object> shipProto)
        {
            double i = 0;
            double e = 0;
            string n = Environment.NewLine;

            foreach (var shipEntry in shipProto)
            {
                var txtFile = new StringBuilder();
                GomLib.Models.scFFShip ship = new GomLib.Models.scFFShip();

                currentDom.scFFShipLoader.Load(ship, (long)shipEntry.Key, (GomObjectData)shipEntry.Value);
                if (true) //ship.IsAvailable)
                {
                    addtolist2("Ship Name: " + ship.Name);

                    string t = "  ";

                    txtFile.Append(ship.Name + n);
                    txtFile.Append(t + "Description: " + ship.Description + n);
                    txtFile.Append(t + "Category: " + ship.Category.Replace("scFF", "") + n);
                    txtFile.Append(t + "Stats: " + n + t);
                    txtFile.Append(t + string.Join(n + t + t, ship.Stats.Select(x => currentDom.statD.ToStat(x.Key) + ": " + x.Value).ToArray()).Replace("Space PvP ", "").Replace("Space PVP ", ""));
                    txtFile.Append(n + n);

                    foreach (var containerMapSlot in ship.MajorComponentSlots)
                    {
                        txtFile.Append(t + containerMapSlot.Key + " (" + containerMapSlot.Value + ")" + n);
                        if (ship.ComponentMap.ContainsKey(containerMapSlot.Key))
                        {
                            foreach (GomLib.Models.scFFComponent comp in ship.ComponentMap[containerMapSlot.Key])
                            {
                                txtFile.Append(t + t + comp.Name);
                                //txtFile.Append(t + t + t + comp.Description + n);
                                string conMapSlot = containerMapSlot.Key.Replace("AuxSystem", "Systems");
                                bool isDefault = ((ulong)ship.DefaultLoadout[conMapSlot] == comp.Id);
                                if (isDefault)
                                {
                                    txtFile.Append(" (Default)" + n);
                                }
                                else
                                {
                                    txtFile.Append(n);
                                }
                                txtFile.Append(t + t + t + "Available: " + comp.IsAvailable + n);
                                txtFile.Append(t + t + t + "Stats:" + n);
                                foreach (var stat in comp.StatsList)
                                {
                                    txtFile.Append(t + t + t + t + stat.Key + ": " + stat.Value + n);
                                }
                                txtFile.Append(t + t + t + "Base: ");
                                if (comp.TalentList.ContainsKey(0))
                                {
                                    var type = comp.TalentList[0].GetType();
                                    if (type.Name == "Talent")
                                    {
                                        if (comp.Talents.Ability.Id != 0)
                                        {
                                            txtFile.Append(comp.Talents.Ability.Name + ": " + comp.Talents.Ability.Description + n);
                                        }
                                        else
                                        {
                                            txtFile.Append(((GomLib.Models.Talent)comp.TalentList[0]).Name + ": " + ((GomLib.Models.Talent)comp.TalentList[0]).Description + n);
                                        }
                                        foreach (var stat in ((GomLib.Models.Talent)comp.TalentList[0]).RankStats.First().DefensiveStats)
                                        {
                                            txtFile.Append(t + t + t + t + currentDom.statD.ToStat(stat.Stat).Replace("Space PvP ", "").Replace("Space PVP ", "").Replace(' ', '_')
                                                + ": " + stat.Value + " (" + stat.Modifier + ")" + n);
                                        }
                                        foreach (var stat in ((GomLib.Models.Talent)comp.TalentList[0]).RankStats.First().OffensiveStats)
                                        {
                                            txtFile.Append(t + t + t + t + currentDom.statD.ToStat(stat.Stat).Replace("Space PvP ", "").Replace("Space PVP ", "").Replace(' ', '_')
                                                + ": " + stat.Value + " (" + stat.Modifier + ")" + n);
                                        }
                                    }
                                    else
                                    {
                                        txtFile.Append(((GomLib.Models.Ability)comp.TalentList[0]).Description + n);
                                    }
                                }
                                else
                                {
                                    txtFile.Append(n);
                                }
                                txtFile.Append(t + t + t + "TalentTree" + "(" + comp.NumUpgradeTiers + ")" + n);
                                //txtFile.Append(t + t + t + comp.Talents.Ability.Name + ": " + comp.Talents.Ability.Description + n);
                                foreach (var row in comp.Talents.Tree)
                                {
                                    txtFile.Append(t + t + t + t + "Row " + row.Key + n);
                                    foreach (var column in row.Value)
                                    {
                                        if (((List<object>)column.Value)[0].GetType() == typeof(GomLib.Models.Talent))
                                        {
                                            if (((List<object>)column.Value)[1].GetType() == typeof(ScriptEnum))
                                            {
                                                txtFile.Append(t + t + t + t + t + ((GomLib.Models.Talent)((List<object>)column.Value)[0]).Description.Replace("\n", " ")
                                                    + " (" + ((ScriptEnum)((List<object>)column.Value)[1]).ToString().Replace("scFFComponentTalentTarget", "") + ")" + n);
                                                foreach (var stat in ((GomLib.Models.Talent)((List<object>)column.Value)[0]).RankStats.First().DefensiveStats)
                                                {
                                                    txtFile.Append(t + t + t + t + t + t + currentDom.statD.ToStat(stat.Stat).Replace("Space PvP ", "").Replace("Space PVP ", "").Replace(' ', '_')
                                                        + ": " + stat.Value + " (" + stat.Modifier + ")" + n);
                                                }
                                                foreach (var stat in ((GomLib.Models.Talent)((List<object>)column.Value)[0]).RankStats.First().OffensiveStats)
                                                {
                                                    txtFile.Append(t + t + t + t + t + t + currentDom.statD.ToStat(stat.Stat).Replace("Space PvP ", "").Replace("Space PVP ", "").Replace(' ', '_')
                                                        + ": " + stat.Value + " (" + stat.Modifier + ")" + n);
                                                }
                                            }
                                            else
                                            {
                                                txtFile.Append(t + t + t + t + t + ((GomLib.Models.Talent)((List<object>)column.Value)[0]).Description.Replace("\n", " ") + " (Ship)" + n);
                                                foreach (var stat in ((GomLib.Models.Talent)((List<object>)column.Value)[0]).RankStats.First().DefensiveStats)
                                                {
                                                    txtFile.Append(t + t + t + t + t + t + currentDom.statD.ToStat(stat.Stat).Replace("Space PvP ", "").Replace("Space PVP ", "").Replace(' ', '_')
                                                        + ": " + stat.Value + " (" + stat.Modifier + ")" + n);
                                                }
                                                foreach (var stat in ((GomLib.Models.Talent)((List<object>)column.Value)[0]).RankStats.First().OffensiveStats)
                                                {
                                                    txtFile.Append(t + t + t + t + t + t + currentDom.statD.ToStat(stat.Stat).Replace("Space PvP ", "").Replace("Space PVP ", "").Replace(' ', '_')
                                                        + ": " + stat.Value + " (" + stat.Modifier + ")" + n);
                                                }
                                            }

                                        }
                                        else if (((List<object>)column.Value)[0].GetType() == typeof(GomLib.Models.Ability))
                                        {
                                            txtFile.Append(t + t + t + t + t + ((GomLib.Models.Ability)((List<object>)column.Value)[0]).Description + n);
                                        }
                                    }
                                }
                                txtFile.Append(n);
                            }
                        }
                    }

                    foreach (var containerMapSlot in ship.MinorComponentSlots)
                    {
                        txtFile.Append(t + containerMapSlot.Key + " (" + containerMapSlot.Value.ToString() + ")" + n);
                        if (ship.ComponentMap.ContainsKey(containerMapSlot.Key))
                        {
                            foreach (GomLib.Models.scFFComponent comp in ship.ComponentMap[containerMapSlot.Key])
                            {
                                txtFile.Append(t + t + comp.Name);
                                //txtFile.Append(t + t + t + comp.Description + n);
                                string conMapSlot = containerMapSlot.Key.Replace("AuxSystem", "Systems");
                                bool isDefault = ((ulong)ship.DefaultLoadout[conMapSlot] == comp.Id);
                                if (isDefault)
                                {
                                    txtFile.Append(" (Default)" + n);
                                }
                                else
                                {
                                    txtFile.Append(n);
                                }
                                txtFile.Append(t + t + t + "Available: " + comp.IsAvailable + n);
                                txtFile.Append(t + t + t + "Stats:" + n);
                                foreach (var stat in comp.StatsList)
                                {
                                    txtFile.Append(t + t + t + t + stat.Key + ": " + stat.Value + n);
                                }
                                txtFile.Append(t + t + t + "Base: ");
                                if (comp.TalentList.ContainsKey(0))
                                {
                                    var type = comp.TalentList[0].GetType();
                                    if (type.Name == "Talent")
                                    {
                                        if (comp.Talents.Ability.Id != 0)
                                        {
                                            txtFile.Append(comp.Talents.Ability.Name + ": " + comp.Talents.Ability.Description + n);
                                        }
                                        else
                                        {
                                            txtFile.Append(((GomLib.Models.Talent)comp.TalentList[0]).Name + ": " + ((GomLib.Models.Talent)comp.TalentList[0]).Description + n);
                                        }
                                        foreach (var stat in ((GomLib.Models.Talent)comp.TalentList[0]).RankStats.First().DefensiveStats)
                                        {
                                            txtFile.Append(t + t + t + t + currentDom.statD.ToStat(stat.Stat).Replace("Space PvP ", "").Replace("Space PVP ", "").Replace(' ', '_')
                                                + ": " + stat.Value + " (" + stat.Modifier + ")" + n);
                                        }
                                        foreach (var stat in ((GomLib.Models.Talent)comp.TalentList[0]).RankStats.First().OffensiveStats)
                                        {
                                            txtFile.Append(t + t + t + t + currentDom.statD.ToStat(stat.Stat).Replace("Space PvP ", "").Replace("Space PVP ", "").Replace(' ', '_')
                                                + ": " + stat.Value + " (" + stat.Modifier + ")" + n);
                                        }
                                    }
                                    else
                                    {
                                        txtFile.Append(t + ((GomLib.Models.Ability)comp.TalentList[0]).Description + n);
                                    }
                                }
                                txtFile.Append(t + t + t + "TalentTree" + "(" + comp.NumUpgradeTiers + ")" + n);
                                //txtFile.Append(t + t + t + comp.Talents.Ability.Name + ": " + comp.Talents.Ability.Description + n);
                                foreach (var row in comp.Talents.Tree)
                                {
                                    txtFile.Append(t + t + t + t + "Row " + row.Key + n);
                                    foreach (var column in row.Value)
                                    {
                                        if (((List<object>)column.Value)[0].GetType() == typeof(GomLib.Models.Talent))
                                        {
                                            if (((List<object>)column.Value)[1].GetType() == typeof(ScriptEnum))
                                            {
                                                txtFile.Append(t + t + t + t + t + ((GomLib.Models.Talent)((List<object>)column.Value)[0]).Description.Replace("\n", " ")
                                                    + " (" + ((ScriptEnum)((List<object>)column.Value)[1]).ToString().Replace("scFFComponentTalentTarget", "") + ")" + n);
                                                foreach (var stat in ((GomLib.Models.Talent)((List<object>)column.Value)[0]).RankStats.First().DefensiveStats)
                                                {
                                                    txtFile.Append(t + t + t + t + t + t + currentDom.statD.ToStat(stat.Stat).Replace("Space PvP ", "").Replace("Space PVP ", "").Replace(' ', '_')
                                                        + ": " + stat.Value + " (" + stat.Modifier + ")" + n);
                                                }
                                                foreach (var stat in ((GomLib.Models.Talent)((List<object>)column.Value)[0]).RankStats.First().OffensiveStats)
                                                {
                                                    txtFile.Append(t + t + t + t + t + t + currentDom.statD.ToStat(stat.Stat).Replace("Space PvP ", "").Replace("Space PVP ", "").Replace(' ', '_')
                                                        + ": " + stat.Value + " (" + stat.Modifier + ")" + n);
                                                }
                                            }
                                            else
                                            {
                                                txtFile.Append(t + t + t + t + t + ((GomLib.Models.Talent)((List<object>)column.Value)[0]).Description.Replace("\n", " ") + " (Ship)" + n);
                                                foreach (var stat in ((GomLib.Models.Talent)((List<object>)column.Value)[0]).RankStats.First().DefensiveStats)
                                                {
                                                    txtFile.Append(t + t + t + t + t + t + currentDom.statD.ToStat(stat.Stat).Replace("Space PvP ", "").Replace("Space PVP ", "").Replace(' ', '_')
                                                        + ": " + stat.Value + " (" + stat.Modifier + ")" + n);
                                                }
                                                foreach (var stat in ((GomLib.Models.Talent)((List<object>)column.Value)[0]).RankStats.First().OffensiveStats)
                                                {
                                                    txtFile.Append(t + t + t + t + t + t + currentDom.statD.ToStat(stat.Stat).Replace("Space PvP ", "").Replace("Space PVP ", "").Replace(' ', '_')
                                                        + ": " + stat.Value + " (" + stat.Modifier + ")" + n);
                                                }
                                            }

                                        }
                                        else if (((List<object>)column.Value)[0].GetType() == typeof(GomLib.Models.Ability))
                                        {
                                            txtFile.Append(t + t + t + t + t + ((GomLib.Models.Ability)((List<object>)column.Value)[0]).Description + n);
                                        }
                                    }
                                }
                                txtFile.Append(n);
                            }
                        }
                    }
                    i++;
                }
                WriteFile(txtFile.ToString(), ship.Name + ".txt", false);
            }
            addtolist("the Ship lists has been generated there are " + i + " Ships");
        }
        #endregion
        #region Deprecated

        /* private XElement ShipDataFromPrototypeAsXElement(Dictionary<object, object> shipProto, bool addedChangedOnly)
        {
            double i = 0;
            double e = 0;
            int h = 0;
            XElement ships = new XElement("Ships");
            foreach (var shipEntry in shipProto)
            {
                GomLib.Models.scFFShip ship = new GomLib.Models.scFFShip();
                currentDom.scFFShipLoader.Load(ship, (long)shipEntry.Key, (GomLib.GomObjectData)shipEntry.Value);
                addtolist2("Ship Name: " + ship.Name);

                XElement shipElement = ShipToXElement(ship);
                XDocument xmlContent = new XDocument(shipElement);
                string path = "\\Ships\\";
                if (!System.IO.Directory.Exists(Config.ExtractPath + prefix + path)) { System.IO.Directory.CreateDirectory(Config.ExtractPath + prefix + path); }
                if (ship.Faction != "") path += ship.Faction + "_";
                else path += "Not_Flagged\\";
                WriteFile(xmlContent, path + ship.Name.Replace(" ", "_") + ".xml", false);
                ships.Add(shipElement);

                if (ship.IsAvailable) e++;
                if (ship.IsHidden) h++;
                if (ship.Name != null)
                {
                    string name = ship.Name.Replace("'", " ");
                }
                //sqlexec("INSERT INTO `ships` (`ship_idc`, `quest_name`, `quest_nodeid`, `quest_id`, `IsBonus`, `BonusShareable`, `Branches`, `CanAbandon`, `Category`, `CategoryId`, `Classes`, `Difficulty`, `Fqn`, `Icon`, `IsClassQuest`, `IsHidden`, `IsRepeatable`, `Items`, `RequiredLevel`, `XpLevel`) VALUES (NULL, '" + name + "', '" + ship.NodeId + "', '" + ship.Id + "', '" + ship.IsBonus + "', '" + ship.BonusShareable + "', '" + ship.Branches + "', '" + ship.CanAbandon + "', '" + ship.Category + "', '" + ship.CategoryId + "', '" + ship.Classes + "', '" + ship.Difficulty + "', '" + ship.Fqn + "', '" + ship.Icon + "', '" + ship.IsClassQuest + "', '" + ship.IsHidden + "', '" + ship.IsRepeatable + "', '" + ship.Items + "', '" + ship.RequiredLevel + "', '" + ship.XpLevel + "');");
                i++;
            }

            if (addedChangedOnly)
            {
                //addtolist("Comparing the Current Ships to the loaded Patch");

                XElement addedItems = FindChangedEntries(ships, "Ships", "Ship");
                addedItems = SortShips(addedItems);
                addtolist("The Ship list has been generated there are " + addedItems.Elements("Ship").Count() + " new/changed Ships");
                ships = null;
                return addedItems;
            }

            ships = SortShips(ships);
            addtolist("The Ship list has been generated there are " + i + " Ships");
            addtolist(e + " Ships are available.");
            addtolist(h + " Ships are hidden.");
            return ships;
        }*/
        #endregion
        #region XML

        private XElement SortShips(XElement ships)
        {
            //addtolist("Sorting Ships");
            ships.ReplaceNodes(ships.Elements("Ship")
                .OrderBy(x => (string)x.Attribute("Status"))
                .ThenBy(x => (string)x.Attribute("Id"))
                .ThenBy(x => (string)x.Element("Name"))
                .ThenBy(x => (string)x.Element("Category")));
            return ships;
        }

        /* code moved to GomLib.Models.scFFShip.cs */

        #endregion
        # region Misc
        private void UnloadShipData()
        {
            currentDom.scFFShipLoader.Flush();
            currentDom.scFFComponentLoader.Flush();
            currentDom.scFFPatternLoader.Flush();
        }
        #endregion
    }
}
