using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;
using System.Xml.XPath;
using System.Xml;
using Newtonsoft.Json;

namespace GomLib.Models
{
    public class Conquest : PseudoGameObject, IEquatable<Conquest>
    {
        [JsonConverter(typeof(LongConverter))]
        public long NameId { get; set; }
        //public string Name { get; set; }
        public Dictionary<string, string> LocalizedName { get; set; }
        [JsonConverter(typeof(LongConverter))]
        public long DescId { get; set; }
        public string Description { get; set; }
        public Dictionary<string, string> LocalizedDescription { get; set; }
        public string Icon { get; set; }
        public string HashedIcon
        {
            get
            {
                var fileId = TorLib.FileId.FromFilePath(String.Format("/resources/gfx/codex/{0}.dds", this.Icon));
                return String.Format("{0}_{1}", fileId.ph, fileId.sh);
            }
        }
        [JsonConverter(typeof(LongConverter))]
        public long ParticipateGoal { get; set; }
        [JsonIgnore]
        public List<long> RepeatableObjectivesIdList { get; set; }
        public List<ConquestObjectivePackage> RepeatableObjectivesList { get; set; }
        [JsonIgnore]
        public List<long> OneTimeObjectiveIdList { get; set; }
        public List<ConquestObjectivePackage> OneTimeObjectivesList { get; set; }
        public string DesignName { get; set; }
        [JsonIgnore]
        public Dictionary<ulong, bool> ActivePlanets { get; set; }
        internal Dictionary<string, bool> _ActivePlanetsB62 { get; set; }
        public Dictionary<string, bool> ActivePlanetsB62
        {
            get
            {
                if (_ActivePlanetsB62 == null && ActivePlanets != null)
                {
                    _ActivePlanetsB62 = ActivePlanets.ToDictionary(x => (x.Key).ToMaskedBase62(), x => x.Value);
                }
                return _ActivePlanetsB62;
            }
        }
        [JsonIgnore]
        public Dictionary<ulong, Planet> ActivePlanetObjects { get; set; }
        internal Dictionary<string, Planet> _ActivePlanetObjectsB62 { get; set; }
        public Dictionary<string, Planet> ActivePlanetObjectsB62
        {
            get
            {
                if (_ActivePlanetObjectsB62 == null && ActivePlanetObjects != null)
                {
                    _ActivePlanetObjectsB62 = ActivePlanetObjects.ToDictionary(x => (x.Key).ToMaskedBase62(), x => x.Value);
                }
                return _ActivePlanetObjectsB62;
            }
        }
        [JsonIgnore]
        public List<Planet> RepublicActivePlanets { get; set; }
        [JsonIgnore]
        public List<Planet> ImperialActivePlanets { get; set; }
        public List<ConquestData> ActiveData { get; set; }
        public List<DateTime> NewActiveData { get; set; }
        [JsonIgnore]
        public Dictionary<string, List<ulong>> NewObjectivesList { get; internal set; }
        internal Dictionary<string, List<string>> _NewObjectivesB62 { get; set; }
        public Dictionary<string, List<string>> NewObjectivesB62
        {
            get
            {
                if (_NewObjectivesB62 == null && NewObjectivesList != null)
                {
                    _NewObjectivesB62 = NewObjectivesList.ToDictionary(x=> x.Key, x => (x.Value).ToMaskedBase62());
                }
                return _NewObjectivesB62;
            }
        }

        [JsonIgnore]
        public ulong QuestId { get; internal set; }
        public string QuestB62
        {
            get
            {
                return QuestId.ToMaskedBase62();
            }
        }

        public override int GetHashCode()
        {
            int hash = Id.GetHashCode();
            hash ^= NameId.GetHashCode();
            hash ^= DescId.GetHashCode();
            if (Icon != null) hash ^= Icon.GetHashCode();
            hash ^= ParticipateGoal.GetHashCode();
            if (DesignName != null) hash ^= DesignName.GetHashCode();
            if (LocalizedName != null) foreach (var x in LocalizedName) { hash ^= x.GetHashCode(); } //dictionaries need to hashed like this
            if (LocalizedDescription != null) foreach (var x in LocalizedDescription) { hash ^= x.GetHashCode(); }
            if (RepeatableObjectivesList != null) foreach (var x in RepeatableObjectivesList) { hash ^= x.GetHashCode(); }
            if (OneTimeObjectivesList != null) foreach (var x in OneTimeObjectivesList) { hash ^= x.GetHashCode(); }
            if (ActivePlanets != null) foreach (var x in ActivePlanets) { hash ^= x.GetHashCode(); }
            if (ActivePlanetObjects != null) foreach (var x in ActivePlanetObjects) { hash ^= x.GetHashCode(); }
            if (RepublicActivePlanets != null) foreach (var x in RepublicActivePlanets) { hash ^= x.GetHashCode(); }
            if (ImperialActivePlanets != null) foreach (var x in ImperialActivePlanets) { hash ^= x.GetHashCode(); }
            if (ActiveData != null) foreach (var x in ActiveData) { hash ^= x.GetHashCode(); }

            return hash;
        }

        public override bool Equals(object obj)
        {
            if (obj == null) return false;

            if (ReferenceEquals(this, obj)) return true;

            Conquest itm = obj as Conquest;
            if (itm == null) return false;

            return Equals(itm);
        }

        public bool Equals(Conquest itm)
        {
            if (itm == null) return false;

            if (ReferenceEquals(this, itm)) return true;

            if (this.ActivePlanets != null)
            {
                if (itm.ActivePlanets == null)
                    return false;
                foreach (var kvp in this.ActivePlanets)
                {
                    var prev = false;
                    itm.ActivePlanets.TryGetValue(kvp.Key, out prev);
                    if (!kvp.Value.Equals(prev))
                        return false;
                }
            }
            if (this.DescId != itm.DescId)
                return false;
            if (this.Description != itm.Description)
                return false;
            if (this.DesignName != itm.DesignName)
                return false;
            if (this.Icon != itm.Icon)
                return false;
            if (this.Id != itm.Id)
                return false;
            if (this.RepeatableObjectivesList != null)
            {
                if (itm.RepeatableObjectivesList == null)
                    return false;
                if (!this.RepeatableObjectivesList.SequenceEqual(itm.RepeatableObjectivesList))
                    return false;
            }
            if (this.OneTimeObjectivesList != null)
            {
                if (itm.OneTimeObjectivesList == null)
                    return false;
                if (!this.OneTimeObjectivesList.SequenceEqual(itm.OneTimeObjectivesList))
                    return false;
            }
            var ssComp = new DictionaryComparer<string, string>();
            if (!ssComp.Equals(this.LocalizedDescription, itm.LocalizedDescription))
                return false; 
            if (!ssComp.Equals(this.LocalizedName, itm.LocalizedName))
                return false; 
            
            if (this.Name != itm.Name)
                return false;
            if (this.NameId != itm.NameId)
                return false;
            if (this.Id != itm.Id)
                return false;
            if (this.RepeatableObjectivesList != null)
            {
                if (itm.RepeatableObjectivesList == null)
                    return false;
                if (!this.RepeatableObjectivesList.SequenceEqual(itm.RepeatableObjectivesList))
                    return false;
            }
            if (this.ParticipateGoal != itm.ParticipateGoal)
                return false;
            if (this.OneTimeObjectivesList != null)
            {
                if (itm.OneTimeObjectivesList == null)
                    return false;
                if (!this.OneTimeObjectivesList.SequenceEqual(itm.OneTimeObjectivesList))
                    return false;
            }
            else if (itm.OneTimeObjectivesList != null)
                return false;
            if (this.ActiveData != null)
            {
                if (itm.ActiveData == null)
                    return false;
                if (!this.ActiveData.SequenceEqual(itm.ActiveData))
                    return false;
            }
            else if (itm.ActiveData != null)
                return false;
            return true;
        }

        public override string ToString(bool verbose)
        {
            string objectives = "";
            //if (RepeatableObjectivesList != null)
              //  objectives = ObjectivesToText(ActivePlanets, RepeatableObjectivesList);
            //else
                objectives = "";
            string secondObjectives = "";
            //if (OneTimeObjectivesList != null)
                //secondObjectives = ObjectivesToText(ActivePlanets, OneTimeObjectivesList);
            //else
                secondObjectives = "";
            string conquestDates = "Event Order/Start Time: None Listed!";
            if (ActiveData != null)
            {
                var data = String.Join(
                    Environment.NewLine,
                    ActiveData.OrderBy(x => x.ActualOrderNum).Select(x => String.Format("Event Order/Start Time: {0} - {1} EST", x.ActualOrderNum, x.StartTime.ToString())));
                conquestDates = data;
            }
            //WriteFile(String.Join(Environment.NewLine, Name, conquestDates, Environment.NewLine), "conquestPost.txt", true);
            return String.Join(Environment.NewLine,
                Name,
                Description,
                conquestDates,
                String.Format("Personal Goal: {0}", ParticipateGoal),
                String.Format("Republic Active Planets: {0}", String.Join(" - ", RepublicActivePlanets.Select(x => x.Name).ToList())),
                String.Format("Imperial Active Planets: {0}", String.Join(" - ", ImperialActivePlanets.Select(x => x.Name).ToList())),
                String.Format("Repeatable Objectives List: {0}  {1}", Environment.NewLine, objectives),
                String.Format("One Time Objectives List: {0}  {1}", Environment.NewLine, secondObjectives),
                Environment.NewLine
                );
        }
        /*private string ObjectivesToText(Dictionary<ulong, bool> activePlanets, List<GomLib.Models.ConquestObjective> objectivesList)
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
        }*/

        public string ConquestToSCSV()
        {
            string objectives = "";
            //if (RepeatableObjectivesList != null)
                //objectives = ObjectivesToSCSV(ActivePlanets, RepeatableObjectivesList);
            //else
                objectives = "";
            string secondObjectives = "";
            //if (OneTimeObjectivesList != null)
                //secondObjectives = ObjectivesToSCSV(ActivePlanets, OneTimeObjectivesList);
            //else
                secondObjectives = "";
            string conquestDates = "Event Order/Start Time: None Listed!";
            if (ActiveData != null)
            {
                var data = String.Join(
                    String.Format("{0};;", Environment.NewLine),
                    ActiveData.OrderBy(x => x.ActualOrderNum).Select(x => String.Format("Event Order/Start Time: {0} - {1} EST", x.ActualOrderNum, x.StartTime.ToString())));
                conquestDates = data;
            }
            //WriteFile(String.Join(Environment.NewLine, Name, conquestDates, Environment.NewLine), "conquestPost.txt", true);
            return String.Join(Environment.NewLine,
                String.Format("{0};;{1}", Name, conquestDates),
                String.Format(";;{0}", Description),
                String.Format("Personal Goal: {0};;;Planetary Multipliers", ParticipateGoal),
                String.Format("Active Planets;Objective Points;{0};Task Description;Required Task Total", String.Join(";", ActivePlanets.Select(y =>
                    (!RepublicActivePlanets.Select(w => w.Id).ToList().Contains(y.Key))
                    ?
                        (!ActivePlanetObjects.ContainsKey(y.Key))
                            ?
                                "Unknown Planet Name"
                            :
                                String.Format("Imperial {0}", ActivePlanetObjects[y.Key].Name)
                    :
                        (!ImperialActivePlanets.Select(w => w.Id).ToList().Contains(y.Key))
                        ?
                            String.Format("Republic {0}", ActivePlanetObjects[y.Key].Name)
                        :
                            (!ActivePlanetObjects.ContainsKey(y.Key))
                            ?
                                "Unknown Planet Name"
                            :
                                ActivePlanetObjects[y.Key].Name
                    ).ToList())),
                //String.Format("Invasion Bonus:;;{0}", String.Join(";", ActivePlanets.Select(y => ActivePlanetObjects[y.Key].InvasionBonus.Replace("Invasion Bonus - ", "")).ToList())),
                String.Format("Repeatable Objectives:;;{0}{1}{2}",
                    String.Join(";", ActivePlanets.Select(y =>
                        (!ActivePlanetObjects.ContainsKey(y.Key))
                            ?
                                ""
                            :
                                ActivePlanetObjects[y.Key].InvasionBonus.Replace("Invasion Bonus - ", "")).ToList()),
                    Environment.NewLine,
                    objectives),
                "",
                String.Format("One Time Objectives: {0}{1}", Environment.NewLine, secondObjectives),
                Environment.NewLine
                );
        }
        /*private string ObjectivesToSCSV(Dictionary<ulong, bool> activePlanets, List<GomLib.Models.ConquestObjective> objectivesList)
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
        }*/

        public override XElement ToXElement(bool verbose)
        {
            XElement stronghold = new XElement("Conquest");

            stronghold.Add(new XElement("Name", Name, new XAttribute("Id", NameId)),
                new XAttribute("Id", Id),
                new XElement("Description", Description, new XAttribute("Id", DescId)),
                new XElement("Icon", Icon),
                new XElement("DesignName", DesignName)
                );
            if (ActiveData != null)
            {

                XElement activeData = new XElement("Schedule", new XAttribute("Num", ActiveData.Count));
                for (int i = 0; i < ActiveData.Count; i++)
                {
                    var actDat = ActiveData[i].ToXElement(verbose);
                    actDat.SetAttributeValue("Id", i);
                    activeData.Add(actDat);
                }
                stronghold.Add(activeData);
            }
            if (NewActiveData != null)
            {

                XElement activeData = new XElement("Schedule", new XAttribute("Num", NewActiveData.Count));
                for (int i = 0; i < NewActiveData.Count; i++)
                {
                    var actDat = new XElement("Date", String.Format("{0} EST", NewActiveData[i].ToString()));
                    actDat.SetAttributeValue("Id", i);
                    activeData.Add(actDat);
                }
                stronghold.Add(activeData);
            }

            XElement repPlanets = new XElement("RepublicActivePlanets", new XAttribute("Num", RepublicActivePlanets.Count));
            foreach (var planet in RepublicActivePlanets)
            {
                repPlanets.Add(planet.ToXElement(verbose));
            }
            stronghold.Add(repPlanets);
            XElement impPlanets = new XElement("ImperialActivePlanets", new XAttribute("Num", ImperialActivePlanets.Count));
            foreach (var planet in ImperialActivePlanets)
            {
                impPlanets.Add(planet.ToXElement(verbose));
            }
            stronghold.Add(impPlanets);

            if (RepeatableObjectivesList.Count > 0)
            {
                XElement rptObjectives = new XElement("RepeatableObjectivesList", new XAttribute("Num", RepeatableObjectivesList.Count));
                foreach (var conquestObj in RepeatableObjectivesList)
                {
                    rptObjectives.Add(conquestObj.ToXElement(verbose));
                }
                stronghold.Add(rptObjectives);
            }

            if (OneTimeObjectivesList.Count > 0)
            {
                XElement oneTObjectives = new XElement("OneTimeObjectivesList", new XAttribute("Num", OneTimeObjectivesList.Count));
                foreach (var conquestObj in OneTimeObjectivesList)
                {
                    oneTObjectives.Add(conquestObj.ToXElement(verbose));
                }
                stronghold.Add(oneTObjectives);
            }
            if (NewObjectivesList.Count > 0)
            {
                XElement Objectives = new XElement("ObjectivesList", new XAttribute("Num", OneTimeObjectivesList.Count));
                foreach (var conquestObj in NewObjectivesList)
                {
                    XElement oneTObjectives = new XElement(conquestObj.Key, new XAttribute("Num", conquestObj.Value.Count));
                    foreach (var achObj in conquestObj.Value)
                    {
                        //oneTObjectives.Add(achObj.ToXElement(false));
                    }
                    Objectives.Add(oneTObjectives);
                }
                stronghold.Add(Objectives);
            }

            stronghold.Add(new XElement("ParticipateGoal", ParticipateGoal));

            return stronghold;
        }
    }

    public class ConquestObjective
    {
        [JsonConverter(typeof(ULongConverter))]
        public ulong AchievementID;
        public string AchievementB62Id
        {
            get
            {
                return ((ulong)AchievementID).ToMaskedBase62();
            }
        }
        [Newtonsoft.Json.JsonIgnore]
        public Achievement AchievementObj;

        [Newtonsoft.Json.JsonIgnore]
        public Dictionary<long, float> PlanetIDMultiplyerList = new Dictionary<long, float>();
        [Newtonsoft.Json.JsonIgnore]
        internal Dictionary<string, float> _PlanetIDMultiplyerB62List { get; set; }
        public Dictionary<string, float> PlanetIDMultiplyerB62List
        {
            get
            {
                if (_PlanetIDMultiplyerB62List == null && PlanetIDMultiplyerList != null)
                {
                    _PlanetIDMultiplyerB62List = PlanetIDMultiplyerList.ToDictionary(x => ((ulong)x.Key).ToMaskedBase62(), x => x.Value);
                }
                return _PlanetIDMultiplyerB62List;
            }
        }

        //public Dictionary<Achievement, Dictionary<Planet, float>> ObjectiveList { get; set; }

        public override int GetHashCode()
        {
            int hash = AchievementID.GetHashCode();
            foreach (var x in PlanetIDMultiplyerList)
            {
                hash ^= x.Key.GetHashCode();
                hash ^= x.Value.GetHashCode();
            }
            
            return hash;
        }

        public override bool Equals(object obj)
        {
            if (obj == null) return false;

            if (ReferenceEquals(this, obj)) return true;

            ConquestObjective itm = obj as ConquestObjective;
            if (itm == null) return false;

            return Equals(itm);
        }

        public bool Equals(ConquestObjective itm)
        {
            if (itm == null) return false;

            if (ReferenceEquals(this, itm)) return true;

            if (this.AchievementID != itm.AchievementID)
                return false;

            if (this.PlanetIDMultiplyerList.Count != itm.PlanetIDMultiplyerList.Count)
                return false;

            for(int i=0; i<this.PlanetIDMultiplyerList.Count; i++)
            {
                KeyValuePair<long, float> thisPlanetMulti = this.PlanetIDMultiplyerList.ElementAt(i);
                KeyValuePair<long, float> itmPlanetMulti = itm.PlanetIDMultiplyerList.ElementAt(i);
                if (thisPlanetMulti.Key != itmPlanetMulti.Key || thisPlanetMulti.Value != itmPlanetMulti.Value)
                    return false;
            }

            return true;
        }

        public XElement ToXElement(bool verbose)
        {
            XElement cObj = new XElement("ConquestObjective",
                new XAttribute("Id", AchievementObj.Id));
            cObj.Add(AchievementObj.ToXElement(verbose));

            //Output the modifier list for the planets.
            if (PlanetIDMultiplyerList.Count > 0)
            {
                XElement ModifierElem = new XElement("Modifiers");
                foreach (KeyValuePair<long, float> kvp in PlanetIDMultiplyerList)
                {
                    ModifierElem.Add(new XElement("Planet",
                        new XAttribute("Id", kvp.Key),
                        kvp.Value));
                }

                cObj.Add(ModifierElem);
            }

            return cObj;
        }
    }

    public class ConquestObjectivePackage : PseudoGameObject, IEquatable<ConquestObjectivePackage>
    {
        public List<ConquestObjective> Objectives = new List<ConquestObjective>();

        public override int GetHashCode()
        {
            int hash = Id.GetHashCode();
            foreach(ConquestObjective objective in Objectives)
            {
                hash ^= objective.GetHashCode();
            }

            return hash;
        }

        public override bool Equals(object obj)
        {
            if (obj == null) return false;

            if (ReferenceEquals(this, obj)) return true;

            ConquestObjectivePackage itm = obj as ConquestObjectivePackage;
            if (itm == null) return false;

            return Equals(itm);
        }

        public bool Equals(ConquestObjectivePackage obj)
        {
            if (obj == null) return false;

            if (ReferenceEquals(this, obj)) return true;

            if (this.Id != obj.Id)
                return false;

            if (this.Objectives.Count != obj.Objectives.Count)
                return false;
            if (!this.Objectives.SequenceEqual(obj.Objectives))
                return false;

            return true;
        }

        public override XElement ToXElement(bool verbose)
        {
            XElement packageElem = new XElement("ObjectivePackage", new XAttribute("Id", Id));
            foreach(ConquestObjective objective in Objectives)
            {
                packageElem.Add(objective.ToXElement(verbose));
            }

            return packageElem;
        }
    }

    public class Planet : GameObject, IEquatable<Planet>
    {
        [Newtonsoft.Json.JsonIgnore]
        public long NameId { get; set; }
        public string Name { get; set; }
        public Dictionary<string, string> LocalizedName { get; set; }
        public long DescId { get; set; }
        public string Description { get; set; }
        public Dictionary<string, string> LocalizedDescription { get; set; }
        public string Icon { get; set; }
        public string HashedIcon
        {
            get
            {
                var fileId = TorLib.FileId.FromFilePath(String.Format("/resources/gfx/codex/{0}.dds", this.Icon));
                return String.Format("{0}_{1}", fileId.ph, fileId.sh);
            }
        }
        [JsonIgnore]
        public Dictionary<ulong, object> ExitList { get; set; }
        public ulong PrimaryAreaId { get; set; }
        public long TransportCost { get; set; }
        public long OrbtSupportCost { get; set; }
        [Newtonsoft.Json.JsonIgnore]
        public ulong OrbtSupportAblId { get; set; }
        public string OrbtSupportAblB62Id
        {
            get
            {
                return ((ulong)OrbtSupportAblId).ToMaskedBase62();
            }
        }
        [Newtonsoft.Json.JsonIgnore]
        internal Ability _OrbtSupportAbility { get; set; }
        [Newtonsoft.Json.JsonIgnore]
        public Ability OrbtSupportAbility
        {
            get
            {
                if (_OrbtSupportAbility == null)
                {
                    _OrbtSupportAbility = _dom.abilityLoader.Load(OrbtSupportAblId);
                }
                return _OrbtSupportAbility;
            }
        }
        public Dictionary<string, string> LocalizedInvasionBonus { get; set; }
        public string InvasionBonus { get; set; }
        public long InvasionBonusId { get; set; }
        public DetailedFaction Faction { get; set; }

        [Newtonsoft.Json.JsonIgnore]
        public ulong ConquestGuildQuestId { get; set; }
        public string ConquestGuildQuestB62Id
        {
            get
            {
                return ((ulong)ConquestGuildQuestId).ToMaskedBase62();
            }
        }
        [Newtonsoft.Json.JsonIgnore]
        internal Quest _ConquestGuildQuest { get; set; }
        [Newtonsoft.Json.JsonIgnore]
        public Quest ConquestGuildQuest
        {
            get
            {
                if (_ConquestGuildQuest == null)
                {
                    _ConquestGuildQuest = _dom.questLoader.Load(ConquestGuildQuestId);
                }
                return _ConquestGuildQuest;
            }
        }

        public string Size { get; internal set; }

        public override int GetHashCode()
        {
            int hash = Id.GetHashCode();
            hash ^= NameId.GetHashCode();
            hash ^= DescId.GetHashCode();
            if (Icon != null) hash ^= Icon.GetHashCode();
            hash ^= PrimaryAreaId.GetHashCode();
            hash ^= TransportCost.GetHashCode();
            hash ^= OrbtSupportCost.GetHashCode();
            hash ^= OrbtSupportAblId.GetHashCode();
            hash ^= InvasionBonusId.GetHashCode();
            if (LocalizedName != null) foreach (var x in LocalizedName) { hash ^= x.GetHashCode(); } //dictionaries need to hashed like this
            if (LocalizedDescription != null) foreach (var x in LocalizedDescription) { hash ^= x.GetHashCode(); }
            if (ExitList != null) foreach (var x in ExitList) { hash ^= x.GetHashCode(); }
            if (LocalizedInvasionBonus != null) foreach (var x in LocalizedInvasionBonus) { hash ^= x.GetHashCode(); }

            return hash;
        }

        public override bool Equals(object obj)
        {
            if (obj == null) return false;

            if (ReferenceEquals(this, obj)) return true;

            Planet itm = obj as Planet;
            if (itm == null) return false;

            return Equals(itm);
        }

        public bool Equals(Planet itm)
        {
            if (itm == null) return false;

            if (ReferenceEquals(this, itm)) return true;

            if (this.DescId != itm.DescId)
                return false;
            if (this.Description != itm.Description)
                return false;
            if (this.Icon != itm.Icon)
                return false;
            if (this.Id != itm.Id)
                return false;
            if (this.InvasionBonus != itm.InvasionBonus)
                return false;
            if (this.InvasionBonusId != itm.InvasionBonusId)
                return false;

            var ssComp = new DictionaryComparer<string, string>();
            if (!ssComp.Equals(this.LocalizedDescription, itm.LocalizedDescription))
                return false;
            if (!ssComp.Equals(this.LocalizedName, itm.LocalizedName))
                return false;
            if (!ssComp.Equals(this.LocalizedInvasionBonus, itm.LocalizedInvasionBonus))
                return false;

            if (this.Name != itm.Name)
                return false;
            if (this.NameId != itm.NameId)
                return false;
            if (this.OrbtSupportAblId != itm.OrbtSupportAblId)
                return false;
            if (this.OrbtSupportCost != itm.OrbtSupportCost)
                return false;
            if (this.PrimaryAreaId != itm.PrimaryAreaId)
                return false;
            if (this.ExitList != null)
            {
                if (itm.ExitList == null)
                    return false;
                if (this.ExitList.Count != itm.ExitList.Count)
                    return false;
                foreach(var kvp in this.ExitList)
                {
                    object prev;
                    itm.ExitList.TryGetValue(kvp.Key, out prev);
                    if (kvp.Value != prev)
                        return false;
                }
            }
            if (this.TransportCost != itm.TransportCost)
                return false;
            return true;
        }

        public override XElement ToXElement(bool verbose)
        {
            XElement room = new XElement("Planet");

            room.Add(new XElement("Name", Name),
                new XAttribute("Id", Id));
            if (verbose)
            {
                room.Element("Name").Add(new XAttribute("Id", NameId));
                room.Add(//new XElement("Description", Description, new XAttribute("Id", DescId)),
                    //new XElement("DataminingNote", "Fuel Cost to transport your Flagship includes the selected exit's fuel cost as well."),
                    new XElement("OrbitalSupportCost", OrbtSupportCost),
                    //new XElement("OrbitalSupportAbility", OrbtSupportAbility.ToXElement(false)),
                    new XElement("FuelCost", TransportCost)
                    //new XElement("Icon", Icon),
                    //new XElement("PrimaryAreaId", PrimaryAreaId)
                    //new XElement("PrimaryAreaFuelCost", ExitList[PrimaryAreaId]
                    );
            }
            //TODO load sub-areas


            return room;
        }
    }

    public class ConquestData : PseudoGameObject, IEquatable<ConquestData>
    {
        public ConquestData()
        {
            Prototype = "wevConquestsPrototype";
            ProtoDataTable = "wevConquestsInfoTable";
        }

        [Newtonsoft.Json.JsonIgnore]
        public ulong GuildQstId { get; set; }
        public string GuildQstB62Id
        {
            get
            {
                return ((ulong)GuildQstId).ToMaskedBase62();
            }
        }
        [Newtonsoft.Json.JsonIgnore]
        internal Quest _GuildQst { get; set; }
        [Newtonsoft.Json.JsonIgnore]
        public Quest GuildQst
        {
            get
            {
                if (_GuildQst == null)
                {
                    _GuildQst = _dom.questLoader.Load(GuildQstId);
                }
                return _GuildQst;
            }
        }
        [Newtonsoft.Json.JsonIgnore]
        public ulong PersonalQstId { get; set; }
        public string PersonalQstB62Id
        {
            get
            {
                return ((ulong)PersonalQstId).ToMaskedBase62();
            }
        }
        [Newtonsoft.Json.JsonIgnore]
        internal Quest _PersonalQst { get; set; }
        [Newtonsoft.Json.JsonIgnore]
        public Quest PersonalQst
        {
            get
            {
                if (_PersonalQst == null)
                {
                    _PersonalQst = _dom.questLoader.Load(PersonalQstId);
                }
                return _PersonalQst;
            }
        }
        public int OrderId { get; set; }
        public long RawStartTime { get; set; }
        public DateTime StartTime { get; set; }
        public int ActualOrderNum { get; set; }

        public override int GetHashCode()
        {
            int hash = Id.GetHashCode();
            hash ^= GuildQstId.GetHashCode();
            hash ^= PersonalQstId.GetHashCode();
            hash ^= OrderId.GetHashCode();
            hash ^= RawStartTime.GetHashCode();
            hash ^= ActualOrderNum.GetHashCode();
            return hash;
        }

        public override bool Equals(object obj)
        {
            if (obj == null) return false;

            if (ReferenceEquals(this, obj)) return true;

            ConquestData itm = obj as ConquestData;
            if (itm == null) return false;

            return Equals(itm);
        }

        public bool Equals(ConquestData itm)
        {
            if (itm == null) return false;

            if (ReferenceEquals(this, itm)) return true;

            if (this.GuildQstId != itm.GuildQstId)
                return false;
            if (this.PersonalQstId != itm.PersonalQstId)
                return false;
            if (this.OrderId != itm.OrderId)
                return false;
            if (this.Id != itm.Id)
                return false;
            if (this.RawStartTime != itm.RawStartTime)
                return false;
            if (this.ActualOrderNum != itm.ActualOrderNum)
                return false;

            return true;
        }

        public override XElement ToXElement(bool verbose)
        {
            XElement room = new XElement("EventData", new XAttribute("Id", Id));

            room.Add(new XElement("OrderNum", ActualOrderNum),
                new XElement("StartTime", String.Format("{0} EST", StartTime.ToString())));
            if (verbose)
            {
                room.Element("OrderNum").Add(new XAttribute("Id", OrderId));
                room.Add(new XElement("PersonalRewardQuest", PersonalQst.ToXElement(false)),
                    new XElement("GuildRewardQuest", GuildQst.ToXElement(false))
                    );
            }
            return room;
        }
    }
}
