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
            double e = 0;
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

        private XElement CompanionDataFromPrototypeAsXElement(Dictionary<object, object> cmpProto, bool addedChangedOnly)
        {
            double i = 0;
            //double e = 0;
            XElement companions = new XElement("Companions");
            foreach (var gomItm in cmpProto)
            {
                GomLib.Models.Companion cmp = new GomLib.Models.Companion();
                currentDom.companionLoader.Load(cmp, (ulong)gomItm.Key, (GomLib.GomObjectData)gomItm.Value);

                addtolist2("Companion Name: " + cmp.Name);

                companions.Add(CompanionToXElement(cmp));

                if (cmp.Name != null)
                {
                    string name = cmp.Name.Replace("'", " ");
                }
                //sqlexec("INSERT INTO `companions` (`companion_idc`, `quest_name`, `quest_nodeid`, `quest_id`, `IsBonus`, `BonusShareable`, `Branches`, `CanAbandon`, `Category`, `CategoryId`, `Classes`, `Difficulty`, `Fqn`, `Icon`, `IsClassQuest`, `IsHidden`, `IsRepeatable`, `Items`, `RequiredLevel`, `XpLevel`) VALUES (NULL, '" + name + "', '" + cmp.NodeId + "', '" + cmp.Id + "', '" + cmp.IsBonus + "', '" + cmp.BonusShareable + "', '" + cmp.Branches + "', '" + cmp.CanAbandon + "', '" + cmp.Category + "', '" + cmp.CategoryId + "', '" + cmp.Classes + "', '" + cmp.Difficulty + "', '" + cmp.Fqn + "', '" + cmp.Icon + "', '" + cmp.IsClassQuest + "', '" + cmp.IsHidden + "', '" + cmp.IsRepeatable + "', '" + cmp.Items + "', '" + cmp.RequiredLevel + "', '" + cmp.XpLevel + "');");
                i++;
            }

            if (addedChangedOnly)
            {
                //addtolist("Comparing the Current Companions to the loaded Patch");

                XElement addedItems = FindChangedEntries(companions, "Companions", "Companion");
                addedItems = SortCompanions(addedItems);
                addtolist("The Companion list has been generated there are " + addedItems.Elements("Companion").Count() + " new/changed Companions");
                companions = null;
                return addedItems;
            }

            companions = SortCompanions(companions);
            addtolist("The Companion list has been generated there are " + i + " Companions");
            return companions;
        }

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

        private XElement CompanionToXElement(GomLib.Models.Companion cmp)
        {
            return CompanionToXElement(cmp, false);
        }

        private XElement CompanionToXElement(GomLib.Models.Companion cmp, bool overrideVerbose)
        {
            XElement companion = new XElement("Companion");
            if (cmp != null)
            {
                companion.Add(new XAttribute("Id", cmp.uId),
                    new XElement("Name", cmp.Name),
                    new XElement("Description", cmp.Description),
                    new XElement("Faction", cmp.Faction));
                if (verbose && !overrideVerbose)
                {
                    companion.Add(new XElement("Fqn", cmp.Npc.Fqn,
                        new XAttribute("Id", cmp.Npc.NodeId)),
                    new XElement("Potrait", cmp.Portrait));

                    int p = 1;
                    foreach( var prof in cmp.ProfessionModifiers)
                    {
                        companion.Add(new XElement("ProfessionModifier", new XAttribute("Id", p), new XElement("Name", prof.Stat), new XElement("Modifier",prof.Modifier)));
                        p++;
                    }

                    companion.Add(new XElement("ConversationMultiplier", cmp.ConversationMultiplier),
                        new XElement("IsRomanceable", cmp.IsRomanceable),
                        new XElement("IsGenderMale", cmp.IsGenderMale));

                    var giftDic = new Dictionary<string, string>();
                    foreach (var gift in cmp.GiftInterest)
                    {
                        if (giftDic.ContainsKey(gift.Reaction.ToString()))
                        {
                            string oldReactionList = giftDic[gift.Reaction.ToString()];
                            giftDic[gift.Reaction.ToString()] = oldReactionList + ", " + gift.GiftType.ToString();
                        }
                        else
                        {
                            giftDic.Add(gift.Reaction.ToString(), gift.GiftType.ToString());
                        }

                        if (giftDic.ContainsKey(gift.RomancedReaction.ToString()))
                        {
                            string oldReactionList = giftDic[gift.RomancedReaction.ToString()];
                            giftDic[gift.RomancedReaction.ToString()] = oldReactionList + ", " + gift.GiftType.ToString();
                        }
                        else
                        {
                            giftDic.Add(gift.RomancedReaction.ToString(), gift.GiftType.ToString());
                        }
                    }
                    int g = 1;
                    foreach(var reaction in giftDic)
                    {
                        companion.Add(new XElement("GiftReactions", new XAttribute("Id", reaction.Key), reaction.Value));
                        g++;
                    }

                    string affRanks = "";
                    if (cmp.AffectionRanks.Count > 0)
                    {
                        foreach (var affRank in cmp.AffectionRanks)
                        {
                            affRanks += affRank.Affection + ", ";
                        }
                        affRanks.Remove(affRanks.Length - 2);
                    }

                    XElement affectionRanks = new XElement("ConversationAffectionRanks", affRanks);
                    companion.Add(affectionRanks);

                    companion.Add(new XElement("SpaceIcon", cmp.SpaceIcon));
                    if (cmp.CrewPositions != null)
                    {
                        companion.Add(new XElement("CrewPositions", String.Join(", ", (List<string>)cmp.CrewPositions)));
                    }
                    else
                    {
                        companion.Add(new XElement("CrewPositions"));
                    }

                    string reqclasses = null;
                    if (cmp.Classes != null)
                    {
                        foreach (var reqclass in cmp.Classes)
                        {
                            reqclasses += reqclass.Name.ToString() + ", ";
                        }
                    }
                    if (reqclasses != null) { reqclasses = reqclasses.Substring(0, reqclasses.Length - 2); }
                    companion.Add(new XElement("Classes", reqclasses));
                    companion.Add(new XElement("SpaceAbility", new XAttribute("Id", 0), AbilityToXElement(cmp.SpaceAbility)));
                    int s = 1;
                    foreach( var crwAbl in cmp.CrewAbilities)
                    {
                        companion.Add(new XElement("SpacePassive", new XAttribute("Id", s), TalentToXElement(crwAbl)));
                        s++;
                    }

                    companion.Add(NpcToXElement(cmp.Npc));
                }
            }
            return companion;
        }

        #endregion
    }
}
