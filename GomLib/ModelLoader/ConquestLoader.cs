using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GomLib.Models;
using System.Xml.Linq;

namespace GomLib.ModelLoader
{
    public class ConquestLoader
    {
        readonly DataObjectModel _dom;

        public ConquestLoader(DataObjectModel dom)
        {
            _dom = dom;
            Flush();
        }

        public void Flush()
        {
            PlanetList = null;
            ObjectiveList = null;
            ActiveConquestData = null;
            NewActiveConquestData = null;
        }

        private Dictionary<string, Dictionary<ulong, Planet>> PlanetList;
        private Dictionary<long, ConquestObjectivePackage> ObjectiveList;
        private Dictionary<long, List<ConquestData>> ActiveConquestData;
        private Dictionary<long, List<DateTime>> NewActiveConquestData;

        public PseudoGameObject CreateObject()
        {
            return new Conquest();
        }

        private readonly HashSet<string> fields = new HashSet<string>()
        {
        };

        public HashSet<string> Fields => fields;

        public Conquest Load(PseudoGameObject obj, long id, GomObjectData gom)
        {
            if (gom == null) { return (Conquest)obj; }
            if (gom == null) { return null; }

            var cnq = (Conquest)obj;

            cnq.Id = id;
            cnq.Dom_ = _dom;
            var keys = gom.Dictionary.Keys;
            foreach (var key in keys.Skip(3)) //Skip the first three keys as they are internal ones
            {
                /*if (!fields.Contains(key))
                {
                    Console.WriteLine(String.Join(", ", key, dec.Fqn));
                    //if (key != "Script_Type" && key != "Script_TypeId" && key != "Script_NumFields")
                    //throw new IndexOutOfRangeException();
                }*/
            }

            if (PlanetList == null)
            {
                LoadPlanets();
                LoadObjectives();
                LoadActiveConquests();
            }

            ActiveConquestData.TryGetValue(id, out List<ConquestData> cD);
            cnq.ActiveData = cD;

            NewActiveConquestData.TryGetValue(id, out List<DateTime> dT);
            cnq.NewActiveData = dT;

            var nameTable = _dom.stringTable.Find("str.gui.planetaryconquest");

            cnq.NameId = gom.ValueOrDefault<long>("wevConquestNameId", 0);
            if (cnq.NameId == 0)
                cnq.NameId = gom.ValueOrDefault<long>("cnqConquestNameId", 0);
            cnq.Name = nameTable.GetText(cnq.NameId, "str.gui.planetaryconquest");
            cnq.LocalizedName = nameTable.GetLocalizedText(cnq.NameId, "str.gui.planetaryconquest");

            cnq.DescId = gom.ValueOrDefault<long>("wevConquestDescId", 0);
            if (cnq.DescId == 0)
                cnq.DescId = gom.ValueOrDefault<long>("cnqConquestDescId", 0);
            cnq.Description = nameTable.GetText(cnq.DescId, "str.gui.planetaryconquest");
            cnq.LocalizedDescription = nameTable.GetLocalizedText(cnq.DescId, "str.gui.planetaryconquest");

            cnq.Icon = gom.ValueOrDefault("wevConquestIcon", "");
            if (cnq.Icon == "")
                cnq.Icon = gom.ValueOrDefault("cnqConquestIcon", "");
            cnq.ParticipateGoal = gom.ValueOrDefault<long>("wevConquestParticipateGoal", 0);
            if (cnq.ParticipateGoal == 0)
                cnq.ParticipateGoal = gom.ValueOrDefault<long>("cnqConquestParticipateGoal", 0);
            cnq.RepeatableObjectivesIdList = gom.ValueOrDefault("wevConquestRepeatableObjectivesList", new List<object>()).ConvertAll(x => (long)x).ToList();
            cnq.RepeatableObjectivesList = new List<ConquestObjectivePackage>();
            foreach (var key in cnq.RepeatableObjectivesIdList)
            {
                ObjectiveList.TryGetValue(key, out ConquestObjectivePackage cqo);
                if (cqo != null)
                    cnq.RepeatableObjectivesList.Add(cqo);
                //else
                //throw new IndexOutOfRangeException();
            }
            cnq.OneTimeObjectiveIdList = gom.ValueOrDefault("wevConquestOneTimeObjectiveList", new List<object>()).ConvertAll(x => (long)x).ToList();
            cnq.OneTimeObjectivesList = new List<ConquestObjectivePackage>();
            foreach (var key in cnq.OneTimeObjectiveIdList)
            {
                ObjectiveList.TryGetValue(key, out ConquestObjectivePackage cqo);
                if (cqo != null)
                    cnq.OneTimeObjectivesList.Add(cqo);
                //else
                //throw new IndexOutOfRangeException();
            }
            var newObjectiveIdList = gom.ValueOrDefault("cnqConquestObjectivesList", new Dictionary<object, object>());
            cnq.NewObjectivesList = new Dictionary<string, List<ulong>>();
            foreach (var kvp in newObjectiveIdList)
            {
                var xid = (long)((ulong)kvp.Key);

                ulong achID = (ulong)kvp.Key;
                //Achievement achObj = _dom.achievementLoader.Load(achID);
                //if (achObj != null)
                //{
                string type = kvp.Value.ToString().Replace("cnqConquestAchievementType_", "");
                if (!cnq.NewObjectivesList.ContainsKey(type))
                    cnq.NewObjectivesList.Add(type, new List<ulong>());
                cnq.NewObjectivesList[type].Add(achID);// achObj);
                //}
                //else
                //throw new IndexOutOfRangeException();
            }

            cnq.DesignName = gom.ValueOrDefault("wevConquestDesignName", "");
            if (cnq.DesignName == "")
                cnq.DesignName = gom.ValueOrDefault("cnqConquestDesignName", "");
            cnq.ActivePlanets = gom.ValueOrDefault("wevConquestActivePlanets", new Dictionary<object, object>()).ToDictionary(x => (ulong)x.Key, x => (bool)x.Value);
            //if (cnq.ActivePlanets.Count == 0)
            //{
            //    gom.ValueOrDefault<Dictionary<object, object>>("cnqConquestActivePlanets", null);
            //    cnq.ActivePlanets = gom.ValueOrDefault<Dictionary<object, object>>("cnqConquestActivePlanets", new Dictionary<object, object>()).ToDictionary(x => (ulong)x.Key, x => (bool)x.Value);
            //}
            cnq.RepublicActivePlanets = new List<Planet>();
            cnq.ImperialActivePlanets = new List<Planet>();
            cnq.ActivePlanetObjects = new Dictionary<ulong, Planet>();
            foreach (var kvp in cnq.ActivePlanets)
            {
                PlanetList["Republic"].TryGetValue(kvp.Key, out Planet plt);
                if (plt != null)
                {
                    cnq.RepublicActivePlanets.Add(plt);
                    if (!cnq.ActivePlanetObjects.ContainsKey(plt.Id))
                        cnq.ActivePlanetObjects.Add(plt.Id, plt);
                }
                PlanetList["Imperial"].TryGetValue(kvp.Key, out plt);
                if (plt != null)
                {
                    cnq.ImperialActivePlanets.Add(plt);
                    if (!cnq.ActivePlanetObjects.ContainsKey(plt.Id))
                        cnq.ActivePlanetObjects.Add(plt.Id, plt);
                }
            }
            //new planets
            var newActivePlanets = gom.ValueOrDefault("cnqConquestActivePlanets", new Dictionary<object, object>());
            foreach (var nap in newActivePlanets)
            {
                GomObjectData g = nap.Value as GomObjectData;
                ulong cnqPlanetId = (ulong)nap.Key;
                string cnqPlanetDesignName = g.ValueOrDefault("cnqPlanetDesignName", "");
                //ulong cnqPlanetId = gom.ValueOrDefault<ulong>("cnqPlanetId", 0);
                string cnqPlanetSize = g.ValueOrDefault("cnqPlanetSize", new object()).ToString().Replace("cnqPlanetSize_", "");
                ulong cnqPlanetQuestId = g.ValueOrDefault<ulong>("cnqPlanetQuestId", 0);
                string cnqPlanetIcon = g.ValueOrDefault("cnqPlanetIcon", "");

                PlanetList["Republic"].TryGetValue(cnqPlanetId, out Planet plt);
                if (plt != null)
                {
                    plt.ConquestGuildQuestId = cnqPlanetQuestId;
                    plt.Size = cnqPlanetSize;
                    cnq.RepublicActivePlanets.Add(plt);
                    if (!cnq.ActivePlanetObjects.ContainsKey(plt.Id))
                        cnq.ActivePlanetObjects.Add(plt.Id, plt);
                }
                PlanetList["Imperial"].TryGetValue(cnqPlanetId, out plt);
                if (plt != null)
                {
                    plt.ConquestGuildQuestId = cnqPlanetQuestId;
                    plt.Size = cnqPlanetSize;
                    cnq.ImperialActivePlanets.Add(plt);
                    if (!cnq.ActivePlanetObjects.ContainsKey(plt.Id))
                        cnq.ActivePlanetObjects.Add(plt.Id, plt);
                }
            }
            cnq.QuestId = gom.ValueOrDefault<ulong>("cnqConquestQuestId", 0);

            return cnq;
        }

        private void LoadPlanets()
        {
            PlanetList = new Dictionary<string, Dictionary<ulong, Planet>>();

            GomObject protoData = _dom.GetObject("gldFlagshipPrototype");
            if (protoData == null)
            {
                PlanetList = null;
                return;
            }
            Dictionary<string, Dictionary<object, object>> destDataDict = new Dictionary<string, Dictionary<object, object>>
            {
                { "Republic", protoData.Data.Get<Dictionary<object, object>>("gldFlagshipRepDestinations") },
                { "Imperial", protoData.Data.Get<Dictionary<object, object>>("gldFlagshipImpDestinations") }
            };

            var areaList = _dom.GetObject("mapAreasDataProto").Data.Get<Dictionary<object, object>>("mapAreasDataObjectList");
            StringTable strTable = _dom.stringTable.Find("str.sys.worldmap");

            var sysAreaInfoTablePrototype = _dom.GetObject("sysAreaInfoTablePrototype");
            var areaFactionRestriction = sysAreaInfoTablePrototype.Data.Get<Dictionary<object, object>>("sysAreaFactionRestriction");
            var invBonusList = sysAreaInfoTablePrototype.Data.Get<Dictionary<object, object>>("sysAreaBonusNameIdList");
            StringTable conqStrTable = _dom.stringTable.Find("str.gui.planetaryconquest");

            foreach (var kvp in destDataDict)
            {
                Dictionary<ulong, Planet> tempDict = new Dictionary<ulong, Planet>();
                foreach (var subKvp in kvp.Value)
                {
                    areaList.TryGetValue(subKvp.Key, out object areaObject);
                    Planet plt = LoadPlanet((ulong)subKvp.Key, (GomObjectData)subKvp.Value, (GomObjectData)areaObject, strTable);
                    invBonusList.TryGetValue(plt.Id, out object ibId);
                    if (ibId != null)
                    {
                        plt.InvasionBonusId = (long)ibId;
                        plt.InvasionBonus = conqStrTable.GetText((long)ibId, "str.gui.planetaryconquest");
                        plt.LocalizedInvasionBonus = conqStrTable.GetLocalizedText((long)ibId, "str.gui.planetaryconquest");
                    }
                    areaFactionRestriction.TryGetValue(plt.Id, out object factId);
                    if (factId != null)
                    {
                        plt.Faction = _dom.factionData.ToFaction((long)factId);
                    }

                    tempDict.Add((ulong)subKvp.Key, plt);
                }
                PlanetList.Add(kvp.Key, tempDict);
            }
            protoData.Unload();
            return;
        }

        /*public Models.Conquest Load(ulong nodeId)
        {
            GomObject obj = _dom.GetObject(nodeId);
            if (obj == null) { return null; }
            Models.Stronghold itm = new Stronghold();
            return Load(itm, obj);
        }*/

        public Planet LoadPlanet(ulong id)
        {
            var areaList = _dom.GetObject("mapAreasDataProto").Data.Get<Dictionary<object, object>>("mapAreasDataObjectList");
            areaList.TryGetValue(id, out object areaObject);
            StringTable areaTable = _dom.stringTable.Find("str.sys.worldmap");

            return LoadPlanet(id, (GomObjectData)areaObject, areaTable);
        }

        public Planet LoadPlanet(ulong id, GomObjectData areaObject, StringTable areaTable)
        {
            var plt = new Planet
            {
                Dom_ = _dom,
                Id = id
            };

            if (areaObject != null)
                plt.NameId = areaObject.ValueOrDefault<long>("mapAreasDataDisplayNameId", 0);
            else
                plt.NameId = 0;

            plt.Name = areaTable.GetText(plt.NameId, "str.sys.worldmap");
            plt.LocalizedName = areaTable.GetLocalizedText(plt.NameId, "str.sys.worldmap");
            return plt;
        }

        public Planet LoadPlanet(ulong id, GomObjectData gom, GomObjectData areaObject, StringTable areaTable)
        {
            if (gom == null) { return new Planet(); }

            var plt = LoadPlanet(id, areaObject, areaTable);

            var descTable = _dom.stringTable.Find("str.cdx");

            plt.DescId = gom.ValueOrDefault<long>("gldFlgDestDescId", 0);
            plt.Description = descTable.GetText(plt.DescId, "str.cdx");
            plt.LocalizedDescription = descTable.GetLocalizedText(plt.DescId, "str.cdx");

            plt.Icon = gom.ValueOrDefault("gldFlgDestIcon", "");
            plt.ExitList = gom.ValueOrDefault("gldFlgDestExitList", new Dictionary<object, object>()).ToDictionary(x => (ulong)x.Key, x => x.Value); //TODO load subobjects
            plt.PrimaryAreaId = gom.ValueOrDefault<ulong>("gldFlgDestPrimaryAreaId", 0);
            plt.OrbtSupportAblId = gom.ValueOrDefault<ulong>("gldFlgDestOrbtSupportAblId", 0);
            plt.OrbtSupportCost = gom.ValueOrDefault<long>("gldFlgDestOrbtSupportCost", 0);
            plt.TransportCost = gom.ValueOrDefault<long>("gldFlgDestFuelCost", 0);

            return plt;
        }

        public void LoadObjectives()
        {
            bool newFormat = false;
            GomObject achProto = _dom.GetObject("wevConquestAchListPrototype");
            if (achProto == null)
            {
                achProto = _dom.GetObject("cnqAchGroupPrototype");
                newFormat = true;
            }
            if (achProto == null)
            {
                ObjectiveList = new Dictionary<long, ConquestObjectivePackage>();
                return;
            }

            if (newFormat)
            {
                var achList = achProto.Data.Get<Dictionary<object, object>>("cnqAchGroupTable");
                ObjectiveList = new Dictionary<long, ConquestObjectivePackage>();

                foreach (var kvp in achList)
                {
                    ConquestObjectivePackage package = new ConquestObjectivePackage
                    {
                        Id = (long)kvp.Key
                    };

                    //Key is ulong, achievement node ID.
                    //Value is dictionary, ulong planet id key, float key.
                    List<object> objectiveDict = kvp.Value as List<object>;
                    foreach (object objectiveKvp in objectiveDict)
                    {
                        ulong achID = (ulong)objectiveKvp;
                        Achievement achObj = _dom.achievementLoader.Load(achID);
                        if (achObj != null)
                        {
                            ConquestObjective objective = new ConquestObjective
                            {
                                AchievementID = achID,
                                AchievementObj = achObj
                            };

                            package.Objectives.Add(objective);
                        }
                    }

                    ObjectiveList.Add(package.Id, package);
                }
                achProto.Unload();
                return;
            }

            var achDict = achProto.Data.Get<Dictionary<object, object>>("wevConquestAchListTable");
            ObjectiveList = new Dictionary<long, ConquestObjectivePackage>();
            foreach (var kvp in achDict)
            {
                ConquestObjectivePackage package = new ConquestObjectivePackage
                {
                    Id = (long)kvp.Key
                };

                //Key is ulong, achievement node ID.
                //Value is dictionary, ulong planet id key, float key.
                Dictionary<object, object> objectiveDict = kvp.Value as Dictionary<object, object>;
                foreach (KeyValuePair<object, object> objectiveKvp in objectiveDict)
                {
                    ulong achID = (ulong)objectiveKvp.Key;
                    Achievement achObj = _dom.achievementLoader.Load(achID);
                    if (achObj != null)
                    {
                        Dictionary<object, object> objMultiplierDict = objectiveKvp.Value as Dictionary<object, object>;
                        ConquestObjective objective = new ConquestObjective
                        {
                            AchievementID = achID,
                            AchievementObj = achObj
                        };

                        foreach (KeyValuePair<object, object> multiplierKvp in objMultiplierDict)
                        {
                            objective.PlanetIDMultiplyerList.Add((long)multiplierKvp.Key, (float)multiplierKvp.Value);
                        }

                        package.Objectives.Add(objective);
                    }
                }

                ObjectiveList.Add(package.Id, package);
            }
            achProto.Unload();
        }

        public KeyValuePair<Dictionary<string, Dictionary<ulong, Planet>>, Dictionary<long, ConquestObjectivePackage>> ReturnAllObjectives()
        {
            if (ObjectiveList == null)
                LoadObjectives();
            if (PlanetList == null)
                LoadPlanets();

            var returnObj = new KeyValuePair<Dictionary<string, Dictionary<ulong, Planet>>, Dictionary<long, ConquestObjectivePackage>>(PlanetList, ObjectiveList);

            return returnObj;
        }

        public void LoadActiveConquests()
        {
            GomObject achProto = _dom.GetObject("wevConquestsPrototype");
            ActiveConquestData = new Dictionary<long, List<ConquestData>>();
            NewActiveConquestData = new Dictionary<long, List<DateTime>>();

            if (achProto == null)
            {
                achProto = _dom.GetObject("cnqSchedulePrototype");
                if (achProto == null)
                    return; //no conquest at all
                var cnqLst = achProto.Data.ValueOrDefault("cnqScheduleTable", new List<object>());
                foreach (var entry in cnqLst)
                {
                    //ConquestData entryData = new ConquestData();
                    //entryData._dom = _dom;

                    GomObjectData gomEnt = entry as GomObjectData;
                    var entryDataId = gomEnt.ValueOrDefault<long>("cnqScheduleConquestId", 0);
                    //entryData.GuildQstId = gomEnt.ValueOrDefault<ulong>("wevConquestsGuildQstId", 0);
                    //entryData.PersonalQstId = gomEnt.ValueOrDefault<ulong>("wevConquestsPersonalQstId", 0);
                    var entryDataOrderId = (int)gomEnt.ValueOrDefault<long>("cnqScheduleStartOrderId", 0);
                    //entryData.RawStartTime = Convert.ToInt64(gomEnt.ValueOrDefault<object>("wevConquestsStartTime", 0));

                    DateTime time = new DateTime(2017, 10, 10, 18, 0, 0, 0, DateTimeKind.Utc);
                    time = time.AddDays((entryDataOrderId - 1001) * 7).ToLocalTime();
                    var startTime = time;
                    if (!NewActiveConquestData.ContainsKey(entryDataId))
                        NewActiveConquestData.Add(entryDataId, new List<DateTime>());
                    NewActiveConquestData[entryDataId].Add(startTime);
                }
                achProto.Unload();

                return;
            }

            var cnqList = achProto.Data.Get<List<object>>("wevConquestsInfoTable");
            var cnqDict = achProto.Data.Get<Dictionary<object, object>>("wevConquestOrderLookupTable");

            foreach (var entry in cnqList)
            {
                ConquestData entryData = new ConquestData
                {
                    Dom_ = _dom
                };

                GomObjectData gomEnt = entry as GomObjectData;
                entryData.Id = gomEnt.ValueOrDefault<long>("wevConquestsId", 0);
                entryData.GuildQstId = gomEnt.ValueOrDefault<ulong>("wevConquestsGuildQstId", 0);
                entryData.PersonalQstId = gomEnt.ValueOrDefault<ulong>("wevConquestsPersonalQstId", 0);
                entryData.OrderId = (int)gomEnt.ValueOrDefault<long>("wevConquestsStartOrderId", 0);
                cnqDict.TryGetValue((long)entryData.OrderId, out object actualOrdNum);
                entryData.ActualOrderNum = Convert.ToInt32(actualOrdNum);
                entryData.RawStartTime = Convert.ToInt64(gomEnt.ValueOrDefault<object>("wevConquestsStartTime", 0));

                DateTime time = new DateTime(1601, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc);
                time = time.AddSeconds(entryData.RawStartTime / 1000).ToLocalTime();
                entryData.StartTime = time;
                if (!ActiveConquestData.ContainsKey(entryData.Id))
                    ActiveConquestData.Add(entryData.Id, new List<ConquestData>());
                ActiveConquestData[entryData.Id].Add(entryData);
            }
            achProto.Unload();
        }
    }

}
