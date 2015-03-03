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

namespace tor_tools
{
    public partial class Tools
    {        
        #region Text
        private string CompanionDataFromPrototype(Dictionary<object, object> cmpProto)
        {
            double i = 0;
            string n = Environment.NewLine;

            var txtFile = new StringBuilder();
            foreach (var gomItm in cmpProto)
            {
                GomLib.Models.Companion cmp = new GomLib.Models.Companion();
                currentDom.companionLoader.Load(cmp, (ulong)gomItm.Key, (GomLib.GomObjectData)gomItm.Value);

                addtolist2("Companion Name: " + cmp.Name);

                txtFile.Append("Name: " + cmp.Name + n);
                //txtFile.Append("  Description: " + cmp.Description + n);

                if (verbose)
                {
                    txtFile.Append("  Faction: " + cmp.Faction + n);
                    //txtFile.Append("  Portrait: " + cmp.Portrait + n);
                    txtFile.Append("  Co-Pilot Ability: " + cmp.SpaceAbility.Name + " - " + cmp.SpaceAbility.Description + n);
                    foreach (var crwAbl in cmp.CrewAbilities)
                    {
                        txtFile.Append("  Crew Bonus: " + crwAbl.Name + " - " + crwAbl.Description + n);
                        foreach (var rankStat in crwAbl.RankStats)
                        {
                            StatListsToTxt(txtFile, rankStat);
                        }
                    }
                    if (cmp.CrewPositions != null)
                    {
                        foreach (var crwPos in cmp.CrewPositions)
                        {
                            txtFile.Append("  Crew Position: " + crwPos + n);
                        }
                    }
                }
                txtFile.Append(n);
                //sqlexec("INSERT INTO `companions` (`companion_idc`, `quest_name`, `quest_nodeid`, `quest_id`, `IsBonus`, `BonusShareable`, `Branches`, `CanAbandon`, `Category`, `CategoryId`, `Classes`, `Difficulty`, `Fqn`, `Icon`, `IsClassQuest`, `IsHidden`, `IsRepeatable`, `Items`, `RequiredLevel`, `XpLevel`) VALUES (NULL, '" + name + "', '" + cmp.NodeId + "', '" + cmp.Id + "', '" + cmp.IsBonus + "', '" + cmp.BonusShareable + "', '" + cmp.Branches + "', '" + cmp.CanAbandon + "', '" + cmp.Category + "', '" + cmp.CategoryId + "', '" + cmp.Classes + "', '" + cmp.Difficulty + "', '" + cmp.Fqn + "', '" + cmp.Icon + "', '" + cmp.IsClassQuest + "', '" + cmp.IsHidden + "', '" + cmp.IsRepeatable + "', '" + cmp.Items + "', '" + cmp.RequiredLevel + "', '" + cmp.XpLevel + "');");
                i++;
            }
            addtolist("the Companion lists has been generated there are " + i + " Companions");
            return txtFile.ToString();
        }

        private void StatListsToTxt(StringBuilder txtFile, GomLib.Models.Talent.RankStatData rankStat)
        {
            foreach (var stat in rankStat.DefensiveStats)
            {
                txtFile.Append("    " + currentDom.statD.ToStat(stat.Stat).Replace("Space PvP ", "").Replace("Space PVP ", "").Replace(' ', '_') + ": " + stat.Value + " (" + stat.Modifier + ")" + Environment.NewLine);
            }
            foreach (var stat in rankStat.OffensiveStats)
            {
                txtFile.Append("    " + currentDom.statD.ToStat(stat.Stat).Replace("Space PvP ", "").Replace("Space PVP ", "").Replace(' ', '_') + ": " + stat.Value + " (" + stat.Modifier + ")" + Environment.NewLine);
            }
        }
        #endregion
        #region XML
            private XElement SortCompanions(XElement companions)
            {
                //addtolist("Sorting Companions");
                companions.ReplaceNodes(companions.Elements("Companion")
                    .OrderBy(x => (string)x.Attribute("Status"))
                    .ThenBy(x => (string)x.Element("Fqn"))
                    .ThenBy(x => (string)x.Element("Name"))
                    .ThenBy(x => (string)x.Attribute("Id")));
                return companions;
            }

            /* code moved to GomLib.Models.Companion.cs */

        #endregion
    }
}
