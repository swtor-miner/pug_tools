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
        public void getStrong()
        {
            Clearlist2();

            LoadData();
            var itmList = currentDom.GetObjectsStartingWith("dec.");
            //double ttl = collectionDataProto.Count();
            bool append = false;
            bool addedChanged = false;
            string changed = "";
            if(chkBuildCompare.Checked)
            {
                addedChanged = true;
                changed = "Changed";
            }
            var filename = changed + "Decorations.xml";
            if(outputTypeName == "Text")
            {
                filename = changed + "Decorations.txt";
                string generatedContent = AbilityDataFromFqnList(itmList);
                WriteFile(generatedContent, filename, append);
            }
            else
            {
                ProcessGameObjects("dec.", "Decorations");
                /*if (addedChanged) 
                else
                {
                    XDocument xmlContent = new XDocument(DecorationDataFromFqnListAsXElement(itmList, addedChanged));
                    WriteFile(xmlContent, filename, append);
                }*/
            }

            EnableButtons();
        }

        /*private string StrongholdDataFromFqnList(Dictionary<object, object> collectionDataProto)
        {
            double i = 0;
            string n = Environment.NewLine;

            var txtFile = new StringBuilder();
            foreach (var collection in collectionDataProto)
            {
                GomLib.Models.Collection col = new GomLib.Models.Collection();
                currentDom.collectionLoader.Load(col, (long)collection.Key, (GomLib.GomObjectData)collection.Value);

                addtolist2("Name: " + col.Name);

                txtFile.Append("------------------------" + n);
                txtFile.Append("Id: " + col.Id + n); 
                txtFile.Append("Title: " + col.Name + n);
                txtFile.Append("Rarity: " + col.RarityDesc + n);
                txtFile.Append("Unknown: " + col.unknowntext + n);
                txtFile.Append("Icon: " + col.Icon + n); 
                txtFile.Append("Info: " + n);
                foreach (var bullet in col.BulletPoints)
                {
                    txtFile.Append("  " + bullet + n);
                }
                //txtFile.Append("------------------------" + n );
                //txtFile.Append("Collection INFO" + n );
                txtFile.Append("------------------------" + n + n);
                i++;
            }
            addtolist("The Collection list has been generated there are " + i + " Collection Entries");
            return txtFile.ToString();
        }*/

        #region Decorations
        private static string DecorationToText(GomLib.Models.Decoration itm)
        {
            var hookNameList = itm._dom.decorationLoader.HookList.Select(x => x.Value.Name).ToList();
            hookNameList.Sort();
            string hookString = String.Join(";", hookNameList.Select(x => (itm.AvailableHooks.Contains(x)) ? "x" : "").ToList());

            return String.Join(";",
                itm.Name,
                String.Join(" - ", itm.SourceDict.Values),
                itm.UnlockingItem.Binding.ToString().Replace("None", ""),
                //String.Join(" - ", itm.AvailableHooks)
                itm.CategoryName.Replace("\r\n", "").Replace("\n", "").Replace("\r", ""),
                itm.SubCategoryName,
                itm.GuildPurchaseCost,
                itm.StubType,
                hookString).Replace("decStubType", "");
        }

        private XElement DecorationDataFromFqnListAsXElement(IEnumerable<GomLib.GomObject> itmList, bool addedChangedOnly)
        {
            double i = 0;
            //double e = 0;
            XElement abilities = new XElement("Decorations");
            foreach (var gomItm in itmList)
            {
                GomLib.Models.Decoration itm = new GomLib.Models.Decoration();
                currentDom.decorationLoader.Load(itm, gomItm);

                addtolist2("Decoration Name: " + itm.Name);
                var ability = DecorationToXElement(itm);
                ability.Add(ReferencesToXElement(gomItm.References));
                abilities.Add(ability);

                i++;
            }

            if (addedChangedOnly)
            {
                //addtolist("Comparing the Current Abilities to the loaded Patch");

                XElement addedItems = FindChangedEntries(abilities, "Abilities", "Ability");
                addedItems = SortAbilities(addedItems);
                addtolist("The Ability list has been generated there are " + addedItems.Elements("Ability").Count() + " new/changed Abilities");
                abilities = null;
                return addedItems;
            }

            //abilities = SortAbilities(abilities);
            addtolist("The Ability list has been generated there are " + i + " Abilities");
            return abilities;
        }

        private XElement SortDecorations(XElement decorations)
        {
            //addtolist("Sorting Decoration Entries");
            decorations.ReplaceNodes(decorations.Elements("Decoration")
                .OrderBy(x => (string)x.Attribute("Status"))
                .ThenBy(x => (string)x.Attribute("Id"))
                .ThenBy(x => (string)x.Element("Name")));
            return decorations;
        }


        #region XML
        public XElement DecorationToXElement(GomObject gomItm)
        {
            return DecorationToXElement(gomItm, false);
        }
        public XElement DecorationToXElement(GomObject gomItm, bool overrideVerbose)
        {
            if (gomItm != null)
            {
                GomLib.Models.Decoration itm = new GomLib.Models.Decoration();
                currentDom.decorationLoader.Load(itm, gomItm);
                gomItm.Unload(); // These GomObjects are staying loaded in memory and cause our massive memory issues.
                return DecorationToXElement(itm, overrideVerbose);
            }
            return null;
        }
        public XElement DecorationToXElement(GomLib.Models.Decoration itm)
        {
            return DecorationToXElement(itm, false);
        }
        private XElement DecorationToXElement(GomLib.Models.Decoration dec, bool overrideVerbose)
        {
            XElement decoration = new XElement("Decoration");

            decoration.Add(new XElement("Name", dec.Name, new XAttribute("Id", dec.NameId)),
                new XAttribute("Id", dec.Id),
                new XElement("Fqn", dec.Fqn, new XAttribute("Id", dec.NodeId)),
                new XElement("PreviewWindowValues", String.Join(", ", dec.decPrevObjRotationX, dec.decPrevObjRotationY, dec.PrevCamDisOff, dec.PrevCamHeightOff))
                );
            if(dec.State != null) decoration.Add(new XElement("DynamicState", dec.State));

            decoration.Add(new XElement("DecorationFqn", dec.DecorationFqn, new XAttribute("Id", dec.DecorationId)),
                new XElement("IsAvailable", dec.UseItemName));

            if (dec.DefaultAnimation != null) decoration.Add(new XElement("DefaultAnimation", dec.DefaultAnimation));

            decoration.Add(new XElement("FactionPlacementRestriction", dec.FactionPlacementRestriction),
                new XElement("Category", dec.CategoryName, new XAttribute("Id", dec.CategoryId)),
                new XElement("SubCategory", dec.SubCategoryName, new XAttribute("Id", dec.SubCategoryId)));

            decoration.Add(new XElement("AvailableHooks", String.Join(", ", dec.AvailableHooks)));
            decoration.Add(new XElement("GuildPurchaseCost", dec.GuildPurchaseCost));
            if (dec.StubType != null) decoration.Add(new XElement("RequiredAbilityType", dec.StubType));
            decoration.Add(new XElement("RequiresAbilityUnlocked", dec.RequiresAbilityUnlocked));

            XElement sources = new XElement("Sources", new XAttribute("Num", dec.SourceDict.Count));
            foreach(var kvp in dec.SourceDict)
            {
                sources.Add(new XElement("Source", kvp.Value, new XAttribute("Id", kvp.Key)));
            }
            decoration.Add(sources);
            decoration.Add(new XElement("UnlockingItem", ConvertToXElement(dec.UnlockingItem, true)),
                new XElement("UnlockLimits", String.Join(", ", dec.F2PLimit, dec.MaxUnlockLimit)));
            return decoration;
        }
        #endregion
        #endregion

        #region Apartments
        /*public string TorDecorations(IEnumerable<GomLib.GomObject> itmList) //To output item list for Tor-Fashion.com
        {
            int i = 0;
            int e = 0;
            string n = Environment.NewLine;
            //var txtFile = new StringBuilder();
            itmList.OrderBy(o => o.Name);
            var count = itmList.Count();
            List<string> decorationList = new List<string>();
            List<string> conquestList = new List<string>();
            foreach (var gomItm in itmList)
            {
                progressUpdate(i, count);
                GomLib.Models.Decoration itm = new GomLib.Models.Decoration();
                currentDom.decorationLoader.Load(itm, gomItm);
                gomItm.Unload(); // These GomObjects are staying loaded in memory and cause our massive memory issues.
                if(chkBuildCompare.Checked)
                {
                    GomLib.Models.Decoration prevItm = previousDom.decorationLoader.Load(gomItm.Name);
                    if (itm.Equals(prevItm))
                    {
                        i++;
                        continue;
                    }
                }
                if (itm.Name != "")
                {
                    addtolist2("DecorationName: " + itm.Name);
                    decorationList.Add(String.Join("", ConvertToText(itm), n));
                    if (itm.SourceDict.Values.Contains("Planetary Conquest Events "))
                        conquestList.Add(itm.Name);
                    e++;
                }
                i++;
            }
            List<string> outputList = new List<string>() { String.Format("Name; Sources; Hooks; Category; SubCategory; Purchase for Guild Cost{0}", n) };
            decorationList.Sort();
            outputList.AddRange(decorationList);
            conquestList.Sort();
            WriteFile(String.Join(n, conquestList), "conquestDecorations.txt", false);
            addtolist("the TorDecorations file has been generated there were " + e + " decorations");
            return String.Join("", outputList);
        }*/

        private static string StrongholdToText(GomLib.Models.Stronghold itm)
        {
            return String.Join("; ",
                itm.Name,
                String.Join(" - ", itm.RoomTable.Values.Select(x => x.Name).ToList()),
                itm.GuildShCost,
                itm.PlayerShCost);
        }

        private XElement SortStrongholds(XElement strongholds)
        {
            //addtolist("Sorting Decoration Entries");
            strongholds.ReplaceNodes(strongholds.Elements("Stronghold")
                .OrderBy(x => (string)x.Attribute("Status"))
                .ThenBy(x => (string)x.Attribute("Id"))
                .ThenBy(x => (string)x.Element("Name")));
            return strongholds;
        }


        #region XML
        public XElement StrongholdToXElement(GomObject gomItm)
        {
            return StrongholdToXElement(gomItm, false);
        }
        public XElement StrongholdToXElement(GomObject gomItm, bool overrideVerbose)
        {
            if (gomItm != null)
            {
                GomLib.Models.Stronghold itm = new GomLib.Models.Stronghold();
                currentDom.decorationLoader.Load(itm, gomItm);
                gomItm.Unload(); // These GomObjects are staying loaded in memory and cause our massive memory issues.
                return StrongholdToXElement(itm, overrideVerbose);
            }
            return null;
        }
        public XElement StrongholdToXElement(GomLib.Models.Stronghold itm)
        {
            return StrongholdToXElement(itm, false);
        }
        private XElement StrongholdToXElement(GomLib.Models.Stronghold apt, bool overrideVerbose)
        {
            XElement stronghold = new XElement("Stronghold");

            stronghold.Add(new XElement("Name", apt.Name, new XAttribute("Id", apt.NameId)),
                new XAttribute("Id", apt.Id),
                new XElement("Description", apt.Description, new XAttribute("Id", apt.DescId)),
                new XElement("Fqn", apt.Fqn, new XAttribute("Id", apt.NodeId)),
                new XElement("DefaultOccupancy", apt.DefaultOccupancy),
                new XElement("DefaultGuildSHOccupancy", apt.DefGuildShOcc),
                new XElement("GuildSHCost", apt.GuildShCost),
                new XElement("PlayerShCost", apt.PlayerShCost),
                new XElement("CartelMarketEntries",
                    new XElement("DataminingNote", "I haven't confirmed what the second entry is for yet. They could be Subscriber/F2P costs, or a Player Stronghold cost (lower cost one) and a disabled Guild Stronghold cost."),
                    ConvertToXElement(apt.DiscountMtxSF),
                    ConvertToXElement(apt.MtxStoreFront)),
                new XElement("FactionPurchaseRestriction", apt.FactionPurchaseRestriction),
                new XElement("Icon", apt.Icon),
                new XElement("PublicIcon", apt.PublicIcon),
                new XElement("MaxHooks", apt.MaxHooks),
                new XElement("DefaultHooks", apt.DefaultHooks),
                new XAttribute("PhaseId", apt.PhsId),
                new XElement("Type", apt.Type)
                );
            XElement rooms = new XElement("Rooms", new XAttribute("Num", apt.RoomTable.Count));
            foreach (var kvp in apt.RoomTable)
            {
                rooms.Add(ConvertToXElement(kvp.Value));
            }
            stronghold.Add(rooms);

            return stronghold;
        }

        public XElement RoomToXElement(GomLib.Models.Room itm)
        {
            return RoomToXElement(itm, false);
        }
        private XElement RoomToXElement(GomLib.Models.Room itm, bool overrideVerbose)
        {
            XElement room = new XElement("Room");

            room.Add(new XElement("Name", itm.Name, new XAttribute("Id", itm.NameId)),
                new XAttribute("Id", itm.Idx),
                new XElement("Description", itm.Description, new XAttribute("Id", itm.DescId)),
                new XElement("PlayerShCost", itm.PlayerShCost),
                new XElement("IncPlayerSHOccupancy", itm.PlyrShIncOcc),
                new XElement("IncPlayerSHDecorations", itm.PlyrShIncDecs),
                new XElement("GuildSHCost", itm.GldShCost),
                new XElement("IncGuildSHOccupancy", itm.GldShIncOcc),
                new XElement("IncGuildSHDecorations", itm.GldShIncDecs),
                new XElement(ConvertToXElement(itm.DiscountMtxSF)),
                new XElement(ConvertToXElement(itm.MtxStoreFront)),
                new XElement("RequiredItemToUnlock", ConvertToXElement(itm.ReqItmToUnlockId, itm._dom, true),
                    new XAttribute("Qty", itm.ReqQty))
                );

            return room;
        }
        #endregion
        #endregion

        #region Conquests
        /*public string TorDecorations(IEnumerable<GomLib.GomObject> itmList) //To output item list for Tor-Fashion.com
        {
            int i = 0;
            int e = 0;
            string n = Environment.NewLine;
            //var txtFile = new StringBuilder();
            itmList.OrderBy(o => o.Name);
            var count = itmList.Count();
            List<string> decorationList = new List<string>();
            List<string> conquestList = new List<string>();
            foreach (var gomItm in itmList)
            {
                progressUpdate(i, count);
                GomLib.Models.Decoration itm = new GomLib.Models.Decoration();
                currentDom.decorationLoader.Load(itm, gomItm);
                gomItm.Unload(); // These GomObjects are staying loaded in memory and cause our massive memory issues.
                if(chkBuildCompare.Checked)
                {
                    GomLib.Models.Decoration prevItm = previousDom.decorationLoader.Load(gomItm.Name);
                    if (itm.Equals(prevItm))
                    {
                        i++;
                        continue;
                    }
                }
                if (itm.Name != "")
                {
                    addtolist2("DecorationName: " + itm.Name);
                    decorationList.Add(String.Join("", ConvertToText(itm), n));
                    if (itm.SourceDict.Values.Contains("Planetary Conquest Events "))
                        conquestList.Add(itm.Name);
                    e++;
                }
                i++;
            }
            List<string> outputList = new List<string>() { String.Format("Name; Sources; Hooks; Category; SubCategory; Purchase for Guild Cost{0}", n) };
            decorationList.Sort();
            outputList.AddRange(decorationList);
            conquestList.Sort();
            WriteFile(String.Join(n, conquestList), "conquestDecorations.txt", false);
            addtolist("the TorDecorations file has been generated there were " + e + " decorations");
            return String.Join("", outputList);
        }*/

        private static string ConquestToText(GomLib.Models.Conquest itm)
        {
            string objectives = "";
            if (itm.RepeatableObjectivesList != null)
                objectives = ObjectivesToText(itm.ActivePlanets, itm.RepeatableObjectivesList);
            else
                objectives = "";
            string secondObjectives = "";
            if (itm.OneTimeObjectivesList != null)
                secondObjectives = ObjectivesToText(itm.ActivePlanets, itm.OneTimeObjectivesList);
            else
                secondObjectives = "";
            string conquestDates = "Event Order/Start Time: None Listed!";
            if (itm.ActiveData != null)
            {
                var data = String.Join(
                    Environment.NewLine,
                    itm.ActiveData.OrderBy(x => x.ActualOrderNum).Select(x => String.Format("Event Order/Start Time: {0} - {1} EST", x.ActualOrderNum, x.StartTime.ToString())));
                conquestDates = data;
            }
            //WriteFile(String.Join(Environment.NewLine, itm.Name, conquestDates, Environment.NewLine), "conquestPost.txt", true);
            return String.Join(Environment.NewLine,
                itm.Name,
                itm.Description,
                conquestDates,
                String.Format("Personal Goal: {0}", itm.ParticipateGoal),
                String.Format("Republic Active Planets: {0}", String.Join(" - ", itm.RepublicActivePlanets.Select(x => x.Name).ToList())),
                String.Format("Imperial Active Planets: {0}", String.Join(" - ", itm.ImperialActivePlanets.Select(x => x.Name).ToList())),
                String.Format("Repeatable Objectives List: {0}  {1}", Environment.NewLine, objectives),
                String.Format("One Time Objectives List: {0}  {1}", Environment.NewLine, secondObjectives),
                Environment.NewLine
                );
            
        }

        private static string ObjectivesToText(Dictionary<ulong, bool> activePlanets, List<GomLib.Models.ConquestObjective> objectivesList)
        {
            string objectives = String.Join(
                Environment.NewLine + "  ",
                objectivesList
                    .Select(x =>                                            // Behold the abortion of spawned by Linq. Bow before the absurdity!
                        String.Join(
                            Environment.NewLine + "  ",                     // Mash em all together!
                            x.ObjectiveList
                            .Select(y =>                                    // Turn the ConquestObjective List into a string List
                                String.Join(                                // Mash em all together!
                                    Environment.NewLine + "    ",
                                    String.Join(
                                        ": ",
                                        (y.Key.Name == "") ?                // This is a handy inline conditional for fixing missing objective entries
                                            "Missing Objective!"
                                                :
                                            y.Key.Name,                     // Grab the Achievement/Objective Name/Rewards/Description
                                        (y.Key.Rewards != null) ?
                                            y.Key.Rewards.AchievementPoints.ToString()
                                                :
                                            "???"),
                                    (y.Key.Description == "")
                                    ? "Missing Objective Description!" : y.Key.Description.Replace("\r", " ").Replace("\n", " "),
                                    "  " +
                                    String.Join(                            // Mash em all together!
                                        Environment.NewLine + "      ",     // Now to tack on the Planetary bonus list!
                                        y.Value
                                            .Where(q => (activePlanets != null) ?
                                                activePlanets.Keys.Contains(q.Key.Id)
                                                    :
                                                true)  // Choose only those planets listed as active planets in the base conquest or all planets if no list provided
                                            .Select(z =>                    // Turn the Planet/bonus dictionary into a List of strings
                                            String.Format(
                                                "{0} x{1}",
                                                z.Key.Name,
                                                z.Value
                                                )
                                            ).ToList()                      // This enumerates the IEnumberable<string> Linq Statement and puts it into a List<string>
                                    )
                                )
                            ).ToList()                                      // This enumerates the IEnumberable<string> Linq Statement and puts it into a List<string>
                        )
                    ).ToList()                                              // This enumerates the IEnumberable<string> Linq Statement and puts it into a List<string>
            );
            return objectives;
        }

        private static string ConquestToSCSV(GomLib.Models.Conquest itm)
        {
            string objectives = "";
            if (itm.RepeatableObjectivesList != null)
                objectives = ObjectivesToSCSV(itm.ActivePlanets, itm.RepeatableObjectivesList);
            else
                objectives = "";
            string secondObjectives = "";
            if (itm.OneTimeObjectivesList != null)
                secondObjectives = ObjectivesToSCSV(itm.ActivePlanets, itm.OneTimeObjectivesList);
            else
                secondObjectives = "";
            string conquestDates = "Event Order/Start Time: None Listed!";
            if (itm.ActiveData != null)
            {
                var data = String.Join(
                    String.Format("{0};;", Environment.NewLine),
                    itm.ActiveData.OrderBy(x => x.ActualOrderNum).Select(x => String.Format("Event Order/Start Time: {0} - {1} EST", x.ActualOrderNum, x.StartTime.ToString())));
                conquestDates = data;
            }
            //WriteFile(String.Join(Environment.NewLine, itm.Name, conquestDates, Environment.NewLine), "conquestPost.txt", true);
            return String.Join(Environment.NewLine,
                String.Format("{0};;{1}", itm.Name, conquestDates),
                String.Format(";;{0}", itm.Description),
                String.Format("Personal Goal: {0};;;Planetary Multipliers", itm.ParticipateGoal),
                String.Format("Active Planets;Objective Points;{0};Task Description;Required Task Total", String.Join(";", itm.ActivePlanets.Select(y =>
                    (!itm.RepublicActivePlanets.Select(w => w.Id).ToList().Contains(y.Key))
                    ?
                        (!itm.ActivePlanetObjects.ContainsKey(y.Key))
                            ?
                                "Unknown Planet Name"
                            :
                                String.Format("Imperial {0}", itm.ActivePlanetObjects[y.Key].Name)
                    :
                        (!itm.ImperialActivePlanets.Select(w => w.Id).ToList().Contains(y.Key))
                        ?
                            String.Format("Republic {0}", itm.ActivePlanetObjects[y.Key].Name)
                        :
                            (!itm.ActivePlanetObjects.ContainsKey(y.Key))
                            ?
                                "Unknown Planet Name"
                            :
                                itm.ActivePlanetObjects[y.Key].Name
                    ).ToList())),
                //String.Format("Invasion Bonus:;;{0}", String.Join(";", itm.ActivePlanets.Select(y => itm.ActivePlanetObjects[y.Key].InvasionBonus.Replace("Invasion Bonus - ", "")).ToList())),
                String.Format("Repeatable Objectives:;;{0}{1}{2}",
                    String.Join(";", itm.ActivePlanets.Select(y =>
                        (!itm.ActivePlanetObjects.ContainsKey(y.Key))
                            ?
                                ""
                            :
                                itm.ActivePlanetObjects[y.Key].InvasionBonus.Replace("Invasion Bonus - ", "")).ToList()),
                    Environment.NewLine,
                    objectives),
                "",
                String.Format("One Time Objectives: {0}{1}", Environment.NewLine, secondObjectives),
                Environment.NewLine
                );
        }

        /*private static string ConquestObjectivesToSCSV(KeyValuePair<Dictionary<string, Dictionary<ulong, GomLib.Models.Planet>>, Dictionary<long, GomLib.Models.ConquestObjective>> itm)
        {
            string objectives = "";
            if (itm.RepeatableObjectivesList != null)
                objectives = ObjectivesToSCSV(itm.ActivePlanets, itm.RepeatableObjectivesList);
            else
                objectives = "";
            string secondObjectives = "";
            if (itm.OneTimeObjectivesList != null)
                secondObjectives = ObjectivesToSCSV(itm.ActivePlanets, itm.OneTimeObjectivesList);
            else
                secondObjectives = "";
            string conquestDates = "Event Order/Start Time: None Listed!";
            if (itm.ActiveData != null)
            {
                var data = String.Join(
                    String.Format("{0};;", Environment.NewLine),
                    itm.ActiveData.OrderBy(x => x.ActualOrderNum).Select(x => String.Format("Event Order/Start Time: {0} - {1} EST", x.ActualOrderNum, x.StartTime.ToString())));
                conquestDates = data;
            }
            //WriteFile(String.Join(Environment.NewLine, itm.Name, conquestDates, Environment.NewLine), "conquestPost.txt", true);
            return String.Join(Environment.NewLine,
                String.Format("Active Planets;Objective Points;{0};Task Description;Required Task Total", String.Join(";", itm.Keys.ToList())),
                //String.Format("Invasion Bonus:;;{0}", String.Join(";", itm.ActivePlanets.Select(y => itm.ActivePlanetObjects[y.Key].InvasionBonus.Replace("Invasion Bonus - ", "")).ToList())),
                String.Format("Repeatable Objectives:;;{0}{1}{2}",
                    String.Join(";", itm.ActivePlanets.Select(y => itm.ActivePlanetObjects[y.Key].InvasionBonus.Replace("Invasion Bonus - ", "")).ToList()),
                    Environment.NewLine,
                    objectives),
                "",
                String.Format("One Time Objectives: {0}{1}", Environment.NewLine, secondObjectives),
                Environment.NewLine
                );
        }*/

        private static string ObjectivesToSCSV(Dictionary<ulong, bool> activePlanets, List<GomLib.Models.ConquestObjective> objectivesList)
        {
            string objectives = String.Join(
                    Environment.NewLine,
                    objectivesList
                        .Select(x =>                                            // Behold the abortion of spawned by Linq. Bow before the absurdity!
                            String.Join(
                                Environment.NewLine,                     // Mash em all together!
                                x.ObjectiveList
                                .Select(y =>                                    // Turn the ConquestObjective List into a string List
                                    String.Join(                                // Mash em all together!
                                        ";",
                                        String.Join(
                                            ";",
                                            (y.Key.Name == "") ?                // This is a handy inline conditional for fixing missing objective entries
                                                "Missing Objective!"
                                                    :
                                                y.Key.Name,                     // Grab the Achievement/Objective Name/Rewards/Description
                                            (y.Key.Rewards != null) ?
                                                y.Key.Rewards.AchievementPoints.ToString()
                                                    :
                                                "???"),
                                        String.Join(                            // Mash em all together!
                                            ";",     // Now to tack on the Planetary bonus list!
                                            activePlanets
                                                .Select(z =>                    // Turn the Planet/bonus dictionary into a List of strings
                                                    (y.Value.Where(t => t.Key.Id == z.Key).Count() != 0)
                                                    ? y.Value.Where(t => t.Key.Id == z.Key).First().Value : 1.0
                                                ).ToList()                      // This enumerates the IEnumberable<string> Linq Statement and puts it into a List<string>
                                        ),
                                        (y.Key.Description == "")
                                        ? "Missing Objective Description!" : y.Key.Description.Replace("\r", " ").Replace("\n", " "),
                                        (y.Key.Tasks != null)
                                        ? y.Key.Tasks[0].Count.ToString() : "???"
                                    )
                                ).ToList()                                      // This enumerates the IEnumberable<string> Linq Statement and puts it into a List<string>
                            )
                        ).ToList()                                              // This enumerates the IEnumberable<string> Linq Statement and puts it into a List<string>
                ); 
            return objectives;
        }

        private XElement SortConquests(XElement strongholds)
        {
            //addtolist("Sorting Decoration Entries");
            strongholds.ReplaceNodes(strongholds.Elements("Stronghold")
                .OrderBy(x => (string)x.Attribute("Status"))
                .ThenBy(x => (string)x.Attribute("Id"))
                .ThenBy(x => (string)x.Element("Name")));
            return strongholds;
        }


        #region XML
        public XElement ConquestToXElement(GomLib.Models.Conquest itm)
        {
            return ConquestToXElement(itm, false);
        }
        private XElement ConquestToXElement(GomLib.Models.Conquest dec, bool overrideVerbose)
        {
            XElement stronghold = new XElement("Conquest");

            stronghold.Add(new XElement("Name", dec.Name, new XAttribute("Id", dec.NameId)),
                new XAttribute("Id", dec.Id),
                new XElement("Description", dec.Description, new XAttribute("Id", dec.DescId)),
                new XElement("Icon", dec.Icon),
                new XElement("DesignName", dec.DesignName)
                );
            if (dec.ActiveData != null)
            {

                XElement activeData = new XElement("Schedule", new XAttribute("Num", dec.ActiveData.Count));
                for (int i = 0; i < dec.ActiveData.Count; i++)
                {
                    var actDat = ConvertToXElement(dec.ActiveData[i]);
                    actDat.SetAttributeValue("Id", i);
                    activeData.Add(actDat);
                }
                stronghold.Add(activeData);
            }

            XElement repPlanets = new XElement("RepublicActivePlanets", new XAttribute("Num", dec.RepublicActivePlanets.Count));
            foreach (var planet in dec.RepublicActivePlanets)
            {
                repPlanets.Add(ConvertToXElement(planet));
            }
            stronghold.Add(repPlanets);
            XElement impPlanets = new XElement("ImperialActivePlanets", new XAttribute("Num", dec.ImperialActivePlanets.Count));
            foreach (var planet in dec.ImperialActivePlanets)
            {
                impPlanets.Add(ConvertToXElement(planet));
            }
            stronghold.Add(impPlanets);
            XElement rptObjectives = new XElement("RepeatableObjectivesList", new XAttribute("Num", dec.RepeatableObjectivesList.Count));
            foreach (var conquestObj in dec.RepeatableObjectivesList)
            {
                rptObjectives.Add(ConvertToXElement(conquestObj));
            }
            stronghold.Add(rptObjectives);

            XElement oneTObjectives = new XElement("OneTimeObjectivesList", new XAttribute("Num", dec.OneTimeObjectivesList.Count));
            foreach (var conquestObj in dec.OneTimeObjectivesList)
            {
                oneTObjectives.Add(ConvertToXElement(conquestObj));
            }
            stronghold.Add(oneTObjectives);

            return stronghold;
        }

        public XElement PlanetToXElement(GomLib.Models.Planet itm)
        {
            return PlanetToXElement(itm, false);
        }
        private XElement PlanetToXElement(GomLib.Models.Planet itm, bool overrideVerbose)
        {
            XElement room = new XElement("Planet");

            room.Add(new XElement("Name", itm.Name),
                new XAttribute("Id", itm.Id));
            if (!overrideVerbose)
            {
                room.Element("Name").Add(new XAttribute("Id", itm.NameId));
                room.Add(new XElement("Description", itm.Description, new XAttribute("Id", itm.DescId)),
                    new XElement("DataminingNote", "Fuel Cost to transport your Flagship includes the selected exit's fuel cost as well."),
                    new XElement("OrbitalSupportCost", itm.OrbtSupportCost),
                    new XElement("OrbitalSupportAbility", ConvertToXElement(itm.OrbtSupportAbility, true)),
                    new XElement("FuelCost", itm.TransportCost),
                    new XElement("Icon", itm.Icon),
                    new XElement("PrimaryAreaId", itm.PrimaryAreaId)
                    //new XElement("PrimaryAreaFuelCost", itm.ExitList[itm.PrimaryAreaId]
                    );
            }
            //TODO load sub-areas


            return room;
        }

        public XElement ConquestObjectiveToXElement(GomLib.Models.ConquestObjective itm)
        {
            return ConquestObjectiveToXElement(itm, false);
        }
        private XElement ConquestObjectiveToXElement(GomLib.Models.ConquestObjective itm, bool overrideVerbose)
        {
            XElement cObj = new XElement("ConquestObjective");

            cObj.Add(new XAttribute("Id", itm.Id));
            foreach (var kvp in itm.ObjectiveList)
            {
                var ach = ConvertToXElement(kvp.Key, true);
                if (kvp.Key.Name != null && kvp.Key.Name != "")
                    ach.Add(new XElement("ObjectivePoints", kvp.Key.Rewards.AchievementPoints));
                cObj.Add(ach);
                XElement planetMods = new XElement("PlanetModifiers");
                foreach (var subKvp in kvp.Value)
                {
                    var plt = ConvertToXElement(subKvp.Key, true);
                    plt.Add(new XElement("Modifier", String.Format("x{0}", subKvp.Value)));
                    planetMods.Add(plt);
                }
                cObj.Add(planetMods);
            }
            return cObj;
        }

        public XElement ConquestDataToXElement(GomLib.Models.ConquestData itm)
        {
            return ConquestDataToXElement(itm, false);
        }
        private XElement ConquestDataToXElement(GomLib.Models.ConquestData itm, bool overrideVerbose)
        {
            XElement room = new XElement("EventData", new XAttribute("Id", itm.Id));

            room.Add(new XElement("OrderNum", itm.ActualOrderNum),
                new XElement("StartTime", String.Format("{0} EST", itm.StartTime.ToString())));
            if (!overrideVerbose)
            {
                room.Element("OrderNum").Add(new XAttribute("Id", itm.OrderId));
                room.Add(new XElement("PersonalRewardQuest", ConvertToXElement(itm.PersonalQst, true)),
                    new XElement("GuildRewardQuest", ConvertToXElement(itm.GuildQst, true))
                    );
            }
            return room;
        }
        #endregion
        #endregion

    }
}
