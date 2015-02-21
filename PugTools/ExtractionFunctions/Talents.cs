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

namespace tor_tools
{
    public partial class Tools
    {
        public void getTalents()
        {
            Clearlist2();

            LoadData();
            var itmList = currentDom.GetObjectsStartingWith("tal.");
            double ttl = itmList.Count();
            bool append = false;
            bool addedChanged = false;
            string changed = "";
            if(chkBuildCompare.Checked)
            {
                addedChanged = true;
                changed = "Changed";
            }
            var filename = changed + "Talents.xml";
            if(outputTypeName == "Text")
            {
                filename = changed + "Talents.txt";
                string generatedContent = TalentDataFromFqnList(itmList);
                WriteFile(generatedContent, filename, append);
            }
            else
            {
                if (addedChanged)
                {
                    ProcessGameObjects("tal.", "Talents");
                }
                else
                {
                    XDocument xmlContent = new XDocument(TalentDataFromFqnListAsXElement(itmList, addedChanged));
                    WriteFile(xmlContent, filename, append);
                }
            }

            //MessageBox.Show("The Talent lists has been generated there are " + ttl + " Talents");
            EnableButtons();
        }

        private string TalentDataFromFqnList(IEnumerable<GomLib.GomObject> itmList)
        {
            double i = 0;
            string n = Environment.NewLine;

            var txtFile = new StringBuilder();
            var talLoad = new GomLib.ModelLoader.TalentLoader(currentDom);
            foreach (var gomItm in itmList)
            {
                GomLib.Models.Talent itm = new GomLib.Models.Talent();
                talLoad.Load(itm, gomItm);

                addtolist2("Talent Title: " + itm.Name);

                txtFile.Append("------------------------" + n);
                txtFile.Append("Talent Title: " + itm.Name + n);
                txtFile.Append("Talent NodeId: " + itm.NodeId + n);
                txtFile.Append("Talent Id: " + itm.Id + n);
                //txtFile.Append("------------------------" + n );
                //txtFile.Append("Talent INFO" + n );
                txtFile.Append("  Fqn: " + itm.Fqn + n);
                txtFile.Append("------------------------" + n + n);
                i++;
            }
            addtolist("The Talent list has been generated there are " + i + " Talents");
            return txtFile.ToString();
        }

        private XElement TalentDataFromFqnListAsXElement(IEnumerable<GomLib.GomObject> itmList, bool addedChangedOnly)
        {
            double i = 0;
            XElement talents = new XElement("Talents");
            foreach (GomLib.GomObject gomItm in itmList)
            {
                GomLib.Models.Talent itm = new GomLib.Models.Talent();
                currentDom.talentLoader.Load(itm, gomItm);

                addtolist2("Talent Title: " + itm.Name);

                XElement talent = TalentToXElement(itm);
                talent.Add(ReferencesToXElement(gomItm.References));
                talents.Add(talent); //add talent to talents
                i++;
            }

            if (addedChangedOnly)
            {
                //addtolist("Comparing the Current Talents to the loaded Patch");
                XElement addedItems = FindChangedEntries(talents, "Talents", "Talent");
                addedItems = SortTalents(addedItems);
                addtolist("The Talent list has been generated there are " + addedItems.Elements("Talent").Count() + " new/changed Talents");
                talents = null;
                return addedItems;
            }

            talents = SortTalents(talents);

            addtolist("The Talent list has been generated there are " + i + " Talents");
            return talents;
        }

        private XElement SortTalents(XElement talents)
        {
            //addtolist("Sorting Talents");
            talents.ReplaceNodes(talents.Elements("Talent")
                .OrderBy(x => (string)x.Attribute("Status"))
                .ThenBy(x => (string)x.Element("Fqn"))
                .ThenBy(x => (string)x.Element("Title"))
                .ThenBy(x => (string)x.Element("ID")));
            return talents;
        }

        public XElement TalentToXElement(GomObject gomItm)
        {
            return TalentToXElement(gomItm, false);
        }

        public XElement TalentToXElement(GomObject gomItm, bool overrideVerbose)
        {
            if (gomItm != null)
            {
                GomLib.Models.Talent itm = new GomLib.Models.Talent();
                currentDom.talentLoader.Load(itm, gomItm);
                gomItm.Unload(); // These GomObjects are staying loaded in memory and cause our massive memory issues.
                return TalentToXElement(itm, overrideVerbose);
            }
            return null;
        }

        public XElement TalentToXElement(GomLib.Models.Talent itm) 
        {
            return TalentToXElement(itm, false);
        }

        public XElement TalentToXElement(GomLib.Models.Talent tal, bool overrideVerbose) //split to see if it was necessary to loop through linked talents. Didn't seem like it.
        {
            XElement talent = new XElement("Talent");
            if (tal == null) return talent;
            /*if (!File.Exists(String.Format("{0}{1}/icons/{2}.dds", Config.ExtractPath, prefix, tal.Icon)))
            {
                var stream = tal._dom._assets.FindFile(String.Format("/resources/gfx/icons/{0}.dds", tal.Icon));
                if(stream != null)
                    WriteFile((MemoryStream)stream.OpenCopyInMemory(), String.Format("/icons/{0}.dds", tal.Icon));
            }*/
            if (verbose)
            {
                talent.Add(new XElement("Fqn", tal.Fqn,
                            new XAttribute("NodeId", tal.NodeId)),
                    new XElement("Name", tal.Name, new XAttribute("Id", tal.NameId)),
                    new XAttribute("Id", tal.Id),
                    new XElement("Ranks", tal.Ranks),
                    new XElement("Description", tal.Description,
                        new XAttribute("Id", tal.DescriptionId)),
                    new XElement("DescriptionRank2", tal.DescriptionRank2),
                    new XElement("DescriptionRank3", tal.DescriptionRank3),
                    new XElement("Icon", tal.Icon),
                    new XElement("Tokens", tal.TokenList.Select(x => new XElement("Token", new XAttribute("Id", tal.TokenList.IndexOf(x)), x))));
            }
            else
            {
                talent.Add(new XElement("Name", tal.Name),
                    new XElement("Ranks", tal.Ranks),
                    new XElement("Description", tal.Description));

            }
            if (verbose)
            {
                int r = 1;
                foreach (var blah in tal.RankStats)
                {
                    XElement rank = new XElement("Rank", new XAttribute("Id", r));
                    string firstStatList = "{ ";
                    foreach (var stat in blah.DefensiveStats)
                    {
                        string affectedAbility = "";
                        if (stat.AffectedNodeId != 0)
                        {
                            if (stat.AffectedAbility != null)
                            {
                                if (stat.AffectedAbility.Fqn != null)
                                {
                                    affectedAbility = stat.AffectedAbility.Fqn;
                                }
                                else if (stat.AffectedNodeId != 0)
                                {
                                    affectedAbility = stat.AffectedNodeId.ToString();
                                }
                            }
                        }
                        firstStatList += String.Format("{0}, {1}, {2}, {3}, {4}; ", currentDom.statD.ToStat(stat.Stat), stat.Value, stat.Modifier, stat.Enabled, affectedAbility);
                    }
                    rank.Add(new XElement("FirstStatList", firstStatList + "}"));
                    string secondStatList = "{ ";
                    foreach (var stat in blah.OffensiveStats)
                    {
                        string affectedAbility = "";
                        if (stat.AffectedNodeId != 0)
                        {
                            if (stat.AffectedAbility != null)
                            {
                                if (stat.AffectedAbility.Fqn != null)
                                {
                                    affectedAbility = stat.AffectedAbility.Fqn;
                                }
                                else if (stat.AffectedNodeId != 0)
                                {
                                    affectedAbility = stat.AffectedNodeId.ToString();
                                }
                            }
                        }
                        secondStatList += String.Format("{0}, {1}, {2}, {3}, {4}; ", currentDom.statD.ToStat(stat.Stat), stat.Value, stat.Modifier, stat.Enabled, affectedAbility);
                    }

                    rank.Add(new XElement("SecondStatList", secondStatList + "}"));
                    //new XAttribute("Hash", itm.GetHashCode()),
                    talent.Add(rank);
                    r++;
                }
            }

            return talent;
        }

        //-----------------------------------------------------------------------------------//

        /*public void getTalentsFromPrototype()
        {
            Clearlist();
            TorLib.Assets.assetPath = textBoxAssetsFolder.Text;

            Load();
            var talentDataProto = currentDom.GetObject("sklSkillTreePrototype").Data.Get<Dictionary<object, object>>("sklSkillTreePackageList"); 
            
            bool append = false;
            bool addedChanged = false;
            string changed = "";
            if(chkBuildCompare.Checked)
            {
                addedChanged = true;
                changed = "Changed";
            }
            var filename = changed + "Talents.xml";
            if (radioButton1.Checked)
            {
                filename = changed + "Talents.txt";
                string generatedContent = TalentDataFromPrototype(talentDataProto);
                WriteFile(generatedContent, filename, append);
            }
            else
            {
                XDocument xmlContent = new XDocument(TalentDataFromPrototypeAsXElement(talentDataProto, addedChanged));
                WriteFile(xmlContent, filename, append);
            }

            //MessageBox.Show("The Talent lists has been generated there are " + ttl + " Talents");
            EnableButtons();
        }
        private string TalentDataFromPrototype(Dictionary<object, object> talentProto)
        {
            double i = 0;
            double e = 0;
            string n = Environment.NewLine;

            var txtFile = new StringBuilder();
            foreach (var talentEntry in talentProto)
            {
                GomLib.Models.Talent talent = new GomLib.Models.Talent();

                //GomLib.ModelLoader.TalentLoader.Load(talent, (long)talentEntry.Key, (GomObjectData)talentEntry.Value);

                //var uObj = DataObjectModel.GetObject(unknownProtoLookup1);
                txtFile.Append(talent.Name + ": " + talent.Description + n);
                i++;
            }
            addtolist("the Talent lists has been generated there are " + i + " Talents");
            return txtFile.ToString();
        }

        private XElement TalentDataFromPrototypeAsXElement(Dictionary<object, object> talentProto, bool addedChangedOnly)
        {
            double i = 0;
            //double e = 0;
            XElement talents = new XElement("Talents");
            foreach (var talentTreeEntry in talentProto)
            {
                Dictionary<string, GomLib.Models.Talent> talent = new Dictionary<string, GomLib.Models.Talent>();
                GomLib.ModelLoader.TalentTreeLoader.Load(talent, (long)talentEntry.Key, (GomLib.GomObjectData)talentTreeEntry.Value);
                addtolist2("Talent Name: " + talent.Name);

                talents.Add(TalentAsXElement(talent));

                if (talent.Name != null)
                {
                    string name = talent.Name.Replace("'", " ");
                }
                //sqlexec("INSERT INTO `talents` (`talent_idc`, `quest_name`, `quest_nodeid`, `quest_id`, `IsBonus`, `BonusShareable`, `Branches`, `CanAbandon`, `Category`, `CategoryId`, `Classes`, `Difficulty`, `Fqn`, `Icon`, `IsClassQuest`, `IsHidden`, `IsRepeatable`, `Items`, `RequiredLevel`, `XpLevel`) VALUES (NULL, '" + name + "', '" + talent.NodeId + "', '" + talent.Id + "', '" + talent.IsBonus + "', '" + talent.BonusShareable + "', '" + talent.Branches + "', '" + talent.CanAbandon + "', '" + talent.Category + "', '" + talent.CategoryId + "', '" + talent.Classes + "', '" + talent.Difficulty + "', '" + talent.Fqn + "', '" + talent.Icon + "', '" + talent.IsClassQuest + "', '" + talent.IsHidden + "', '" + talent.IsRepeatable + "', '" + talent.Items + "', '" + talent.RequiredLevel + "', '" + talent.XpLevel + "');");
                i++;
            }

            if (addedChangedOnly)
            {
                //addtolist("Comparing the Current Talents to the loaded Patch");

                XElement addedItems = FindAddedEntries(talents, "Talents", "Talent");
                addedItems = SortTalents(addedItems);
                addtolist("The Talent list has been generated there are " + addedItems.Elements("Talent").Count() + " new/changed Talents");
                talents = null;
                return addedItems;
            }

            talents = SortTalents(talents);
            addtolist("The Talent list has been generated there are " + i + " Talents");
            return talents;
        }
          */
    }
}
