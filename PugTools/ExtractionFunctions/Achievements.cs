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
        private XElement SortAchievements(XElement achievements)
        {
            //addtolist("Sorting Achievement Entries");
            achievements.ReplaceNodes(achievements.Elements("Achievement")
                .OrderBy(x => (string)x.Attribute("Status"))
                .ThenBy(x => (string)x.Element("Fqn"))
                .ThenBy(x => (string)x.Element("Name"))
                .ThenBy(x => (string)x.Element("Id")));
            return achievements;
        }

        public XElement AchievementToXElement(GomObject gomItm)
        {
            return AchievementToXElement(gomItm, false);
        }

        public XElement AchievementToXElement(GomObject gomItm, bool overrideVerbose)
        {
            if (gomItm != null)
            {
                GomLib.Models.Achievement itm = new GomLib.Models.Achievement();
                currentDom.achievementLoader.Load(itm, gomItm);
                gomItm.Unload(); // These GomObjects are staying loaded in memory and cause our massive memory issues.
                return AchievementToXElement(itm, overrideVerbose);
            }
            return null;
        }

        private XElement AchievementToXElement(GomLib.Models.Achievement itm) 
        {
            return AchievementToXElement(itm, false);
        }

        private XElement AchievementToXElement(GomLib.Models.Achievement itm, bool overrideVerbose) //split to see if it was necessary to loop through linked achievements. Didn't seem like it.
        {
            XElement achievement = new XElement("Achievement");

            achievement.Add(
                new XElement("Name", itm.Name),
                new XElement("Description", itm.Description),
                new XElement("Fqn", itm.Fqn),
                new XAttribute("Id", itm.Id));

            if (verbose && !overrideVerbose)
            {
                /*achievement.Element("Name").RemoveAll();
                for (int i = 0; i < localizations.Count; i++)
                {
                    if (itm.LocalizedName[localizations[i]] != "")
                    {
                        achievement.Element("Name").Add(new XElement(localizations[i], itm.LocalizedName[localizations[i]]));
                    }
                }

                achievement.Element("Description").RemoveAll();
                for (int i = 0; i < localizations.Count; i++)
                {
                    if (itm.LocalizedDescription[localizations[i]] != "")
                    {
                        achievement.Element("Description").Add(new XElement(localizations[i], itm.LocalizedDescription[localizations[i]]));
                    }
                } */

                achievement.Element("Name").Add(new XAttribute("Id", itm.NameId));
                achievement.Element("Description").Add(new XAttribute("Id", itm.DescriptionId));
                achievement.Element("Fqn").Add(new XAttribute("Id", itm.NodeId));

                //new XAttribute("Hash", itm.GetHashCode()),
                achievement.Add(new XElement("Rewards"),
                new XElement("NonSpoilerDescription", itm.NonSpoilerDesc,
                    new XAttribute("Id", itm.nonSpoilerId)),
                new XElement("Requirements"),
                new XElement("Amounts"));

                
                /*for (int i = 0; i < localizations.Count; i++)
                {
                    if (itm.LocalizedNonSpoilerDesc[localizations[i]] != "")
                    {
                        achievement.Element("NonSpoilerDescription").Add(new XElement(localizations[i], itm.LocalizedNonSpoilerDesc[localizations[i]]));
                    }
                }*/

                //string rewards = "{ " + String.Join(", ", itm.RewardsList) + " }";
                if (itm.RewardsId != 0)
                {
                    string cC = itm.Rewards.CartelCoins.ToString();
                    if (cC == "0") cC = null;
                    string fR = itm.Rewards.Requisition.ToString();
                    if (fR == "0") fR = null;
                    achievement.Element("Rewards").Add(new XAttribute("Id", itm.RewardsId),
                        new XElement("AchievementPoints", itm.Rewards.AchievementPoints),
                        new XElement("CartelCoins", cC),
                        new XElement("FleetRequisition", fR),
                        new XElement("LegacyTitle", itm.Rewards.LegacyTitle));
                    
                    XElement itemRew = new XElement("Items", new XAttribute("Num", itm.Rewards.ItemRewardList.Count));
                    foreach (var kvp in itm.Rewards.ItemRewardList)
                    {
                        var item = ConvertToXElement(kvp.Key, itm._dom, true);
                        if (item != null)
                        {
                            item.Add(new XAttribute("Quantity", kvp.Value));
                            itemRew.Add(item);
                        }
                    }
                    if (itemRew.HasElements)
                        achievement.Element("Rewards").Add(itemRew);
                    /*if (itm.Rewards.LocalizedLegacyTitle.Count > 0)
                    {
                        for (int i = 0; i < localizations.Count; i++)
                        {
                            if (itm.Rewards.LocalizedLegacyTitle[localizations[i]] != "")
                            {
                                achievement.Element("Rewards").Element("LegacyTitle").Add(new XElement(localizations[i], itm.Rewards.LocalizedLegacyTitle[localizations[i]]));
                            }
                        }
                    }*/
                }
                //TODO: Add code to output tasks
                if (itm.References != null)
                    achievement.Add(ReferencesToXElement(itm.References));
            }
            return achievement;
        }

        private string AchievementDataFromFqnListAsPHP(IEnumerable<GomLib.GomObject> itmList)
        {
            double i = 0;
            string n = Environment.NewLine;

            var txtFile = new StringBuilder();
            foreach (var gomItm in itmList)
            {
                GomLib.Models.Achievement itm = new GomLib.Models.Achievement();
                currentDom.achievementLoader.Load(itm, gomItm);

                addtolist2("Achievement Title: " + itm.Name);

                /*txtFile.Append("$newarray=array(");
                txtFile.Append("\"id\"=>\"" + itm.NodeId.ToString() + "\"");
                txtFile.Append(",\"fqn\"=>\"" + itm.Fqn + "\"");
                txtFile.Append(",\"conditions\"=>array(");
                //...
                txtFile.Append(")");
                txtFile.Append(",\"icon\"=>\"" + itm.Icon + "\"");
                txtFile.Append(",\"rewardId\"=>\"" + itm.RewardsId.ToString() + "\"");
                txtFile.Append(",\"visibility\"=>" + itm.Visibility + "");
                //incomplete
                txtFile.Append(");" + n);*/

                string jsonString = ConvertToJson(itm); //added method in Tools.cs
                txtFile.Append(jsonString + Environment.NewLine); //Append it with a newline to the output.
                i++;
            }
            addtolist("The Achievement list has been generated; there are " + i + " Achievement Entries");
            return txtFile.ToString();
        }
    }
}
