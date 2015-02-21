using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GomLib.Models;
using System.Xml.Linq;
using System.Linq;

namespace GomLib.ModelLoader
{
    public class ConquestLoader
    {
        DataObjectModel _dom;

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
        }

        private Dictionary<string, Dictionary<ulong, Planet>> PlanetList;
        private Dictionary<long, ConquestObjective> ObjectiveList;
        private Dictionary<long, List<ConquestData>> ActiveConquestData;

        public Models.PseudoGameObject CreateObject()
        {
            return new Models.Conquest();
        }

        private HashSet<string> fields = new HashSet<string>() {
            "decPrevObjRotationX",
            "decPrevObjRotationY",
            "decNameId",
            "decUseItemName",
            "decUnlockingItemId",
            "decDecorationId",
            "decPlcState",
            "decDefaultAnimation",
            "decMaxUnlockLimit",
            "decUnknownUnlockLimit",
            "decUniquePerLegacy",
            "decFactionPlacementRestriction",
            "decCategoryNameId",
            "decSubCategoryNameId",
            "decHookList",
            "decPrevCamHeightOff",
            "decPrevCamDisOff",
            "decRequiredAbilityType",
            "decRequiresAbilityUnlocked",
            "decGuildPurchaseCost"
            };

        public Conquest Load(Models.PseudoGameObject obj, long id, GomObjectData gom)
        {
            if (gom == null) { return (Conquest)obj; }
            if (gom == null) { return null; }

            var cnq = (Models.Conquest)obj;

            cnq.Id = id;
            cnq._dom = _dom;
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

            List<ConquestData> cD;
            ActiveConquestData.TryGetValue(id, out cD);
            cnq.ActiveData = cD;

            var nameTable = _dom.stringTable.Find("str.gui.planetaryconquest");

            cnq.NameId = gom.ValueOrDefault<long>("wevConquestNameId", 0);
            cnq.Name = nameTable.GetText(cnq.NameId, "str.gui.planetaryconquest");
            cnq.LocalizedName = nameTable.GetLocalizedText(cnq.NameId, "str.gui.planetaryconquest");

            cnq.DescId = gom.ValueOrDefault<long>("wevConquestDescId", 0);
            cnq.Description = nameTable.GetText(cnq.DescId, "str.gui.planetaryconquest");
            cnq.LocalizedDescription = nameTable.GetLocalizedText(cnq.DescId, "str.gui.planetaryconquest");

            cnq.Icon = gom.ValueOrDefault<string>("wevConquestIcon", "");
            cnq.ParticipateGoal = gom.ValueOrDefault<long>("wevConquestParticipateGoal", 0);
            cnq.RepeatableObjectivesIdList = gom.ValueOrDefault<List<object>>("wevConquestRepeatableObjectivesList", new List<object>()).ConvertAll(x => (long)x).ToList();
            cnq.RepeatableObjectivesList = new List<ConquestObjective>();
            foreach (var key in cnq.RepeatableObjectivesIdList)
            {
                ConquestObjective cqo;
                ObjectiveList.TryGetValue(key, out cqo);
                if (cqo != null)
                    cnq.RepeatableObjectivesList.Add(cqo);
                //else
                    //throw new IndexOutOfRangeException();
            }
            cnq.OneTimeObjectiveIdList = gom.ValueOrDefault<List<object>>("wevConquestOneTimeObjectiveList", new List<object>()).ConvertAll(x => (long)x).ToList();
            cnq.OneTimeObjectivesList = new List<ConquestObjective>();
            foreach (var key in cnq.OneTimeObjectiveIdList)
            {
                ConquestObjective cqo;
                ObjectiveList.TryGetValue(key, out cqo);
                if (cqo != null)
                    cnq.OneTimeObjectivesList.Add(cqo);
                //else
                    //throw new IndexOutOfRangeException();
            }
            cnq.DesignName = gom.ValueOrDefault<string>("wevConquestDesignName", "");
            cnq.ActivePlanets = gom.ValueOrDefault<Dictionary<object, object>>("wevConquestActivePlanets", new Dictionary<object, object>()).ToDictionary(x => (ulong)x.Key, x => (bool)x.Value);

            cnq.RepublicActivePlanets = new List<Planet>();
            cnq.ImperialActivePlanets = new List<Planet>();
            cnq.ActivePlanetObjects = new Dictionary<ulong, Planet>();
            foreach (var kvp in cnq.ActivePlanets)
            {
                Planet plt;
                PlanetList["Republic"].TryGetValue(kvp.Key, out plt);
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

            return cnq;
        }

        private void LoadPlanets()
        {
            PlanetList = new Dictionary<string, Dictionary<ulong, Planet>>();
            Dictionary<ulong, Planet> repPlanets = new Dictionary<ulong, Planet>();
            Dictionary<ulong, Planet> impPlanets = new Dictionary<ulong, Planet>();

            GomObject protoData = _dom.GetObject("gldFlagshipPrototype");
            if (protoData == null)
            {
                PlanetList = null;
                return;
            }
            Dictionary<string, Dictionary<object, object>> destDataDict = new Dictionary<string,Dictionary<object,object>>();
            destDataDict.Add("Republic", protoData.Data.Get<Dictionary<object, object>>("gldFlagshipRepDestinations"));
            destDataDict.Add("Imperial", protoData.Data.Get<Dictionary<object, object>>("gldFlagshipImpDestinations"));

            var areaList = _dom.GetObject("mapAreasDataProto").Data.Get<Dictionary<object, object>>("mapAreasDataObjectList");
            StringTable strTable = _dom.stringTable.Find("str.sys.worldmap");

            var invBonusList = _dom.GetObject("sysAreaInfoTablePrototype").Data.Get<Dictionary<object, object>>("sysAreaBonusNameIdList");
            StringTable conqStrTable = _dom.stringTable.Find("str.gui.planetaryconquest");

            foreach(var kvp in destDataDict)
            {
                Dictionary<ulong, Planet> tempDict = new Dictionary<ulong, Planet>();
                foreach(var subKvp in kvp.Value)
                {
                    object areaObject;
                    areaList.TryGetValue(subKvp.Key, out areaObject);
                    Planet plt = LoadPlanet((ulong)subKvp.Key, (GomObjectData)subKvp.Value, (GomObjectData)areaObject, strTable);
                    object ibId;
                    invBonusList.TryGetValue(plt.Id, out ibId);
                    if (ibId != null)
                    {
                        plt.InvasionBonusId = (long)ibId;
                        plt.InvasionBonus = conqStrTable.GetText((long)ibId, "str.gui.planetaryconquest");
                        plt.LocalizedInvasionBonus = conqStrTable.GetLocalizedText((long)ibId, "str.gui.planetaryconquest");
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
            object areaObject;
            areaList.TryGetValue(id, out areaObject);
            StringTable areaTable = _dom.stringTable.Find("str.sys.worldmap");

            return LoadPlanet(id, (GomObjectData)areaObject, areaTable);
        }

        public Planet LoadPlanet(ulong id, GomObjectData areaObject, StringTable areaTable)
        {
            var plt = new Planet();

            plt._dom = _dom;
            plt.Id = id;

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

            plt.Icon = gom.ValueOrDefault<string>("gldFlgDestIcon", "");
            plt.ExitList = gom.ValueOrDefault<Dictionary<object, object>>("gldFlgDestExitList", new Dictionary<object, object>()).ToDictionary(x => (ulong)x.Key, x => x.Value); //TODO load subobjects
            plt.PrimaryAreaId = gom.ValueOrDefault<ulong>("gldFlgDestPrimaryAreaId", 0);
            plt.OrbtSupportAblId = gom.ValueOrDefault<ulong>("gldFlgDestOrbtSupportAblId", 0);
            plt.OrbtSupportCost = gom.ValueOrDefault<long>("gldFlgDestOrbtSupportCost", 0);
            plt.TransportCost = gom.ValueOrDefault<long>("gldFlgDestFuelCost", 0);

            return plt;
        }

        public void LoadObjectives()
        {
            GomObject achProto = _dom.GetObject("wevConquestAchListPrototype");
            if (achProto == null)
            {
                ObjectiveList = new Dictionary<long, ConquestObjective>();
                return;
            }

            var achDict = achProto.Data.Get<Dictionary<object, object>>("wevConquestAchListTable");
            ObjectiveList = new Dictionary<long, ConquestObjective>();
            foreach (var kvp in achDict)
            {
                var tempDict = ((Dictionary<object, object>)kvp.Value).ToDictionary(x => (ulong)x.Key,
                    x => ((Dictionary<object, object>)x.Value).ToDictionary(y => Convert.ToUInt64(y.Key), y => Convert.ToSingle(y.Value)));
                if (tempDict.Count > 0)
                {
                    ObjectiveList.Add((long)kvp.Key, LoadObjective((long)kvp.Key, tempDict));
                }
                else
                {
                    string pausehere = "";
                }
            }
            achProto.Unload();
        }

        public KeyValuePair<Dictionary<string, Dictionary<ulong, Planet>>, Dictionary<long, ConquestObjective>> ReturnAllObjectives()
        {
            if (ObjectiveList == null)
                LoadObjectives();
            if (PlanetList == null)
                LoadPlanets();

            var returnObj = new KeyValuePair<Dictionary<string, Dictionary<ulong, Planet>>, Dictionary<long, ConquestObjective>>(PlanetList, ObjectiveList);

            return returnObj;
        }

        public ConquestObjective LoadObjective(long id, Dictionary<ulong, Dictionary<ulong, float>> objDict)
        {
            if (id == 0) { return new ConquestObjective(); }
            if (objDict == null) { return new ConquestObjective(); }

            var cqo = new ConquestObjective();

            cqo._dom = _dom;
            cqo.Id = id;

            cqo.ObjectiveList = new Dictionary<Achievement, Dictionary<Planet, float>>();
            foreach (var kvp in objDict)
            {
                var tempDict = new Dictionary<Planet, float>();
                var ach = _dom.achievementLoader.Load(kvp.Key);
                foreach(var subKvp in kvp.Value)
                {
                    Planet p;
                    if (!PlanetList.ContainsKey("Republic"))
                        return cqo;
                    PlanetList["Republic"].TryGetValue(subKvp.Key, out p);
                    if (p == null)
                        PlanetList["Imperial"].TryGetValue(subKvp.Key, out p);
                    if (p == null)
                        p = cqo._dom.conquestLoader.LoadPlanet(subKvp.Key);
                    tempDict.Add(p, subKvp.Value);
                }
                cqo.ObjectiveList.Add(ach, tempDict);
            }

            return cqo;
        }

        public void LoadActiveConquests()
        {
            GomObject achProto = _dom.GetObject("wevConquestsPrototype");
            ActiveConquestData = new Dictionary<long, List<ConquestData>>();

            if (achProto == null)
                return;

            var cnqList = achProto.Data.Get<List<object>>("wevConquestsInfoTable");
            var cnqDict = achProto.Data.Get<Dictionary<object, object>>("wevConquestOrderLookupTable");

            foreach (var entry in cnqList)
            {
                ConquestData entryData = new ConquestData();
                entryData._dom = _dom;

                GomObjectData gomEnt = entry as GomObjectData;
                entryData.Id = gomEnt.ValueOrDefault<long>("wevConquestsId", 0);
                entryData.GuildQstId = gomEnt.ValueOrDefault<ulong>("wevConquestsGuildQstId", 0);
                entryData.PersonalQstId = gomEnt.ValueOrDefault<ulong>("wevConquestsPersonalQstId", 0);
                entryData.OrderId = (int)gomEnt.ValueOrDefault<long>("wevConquestsStartOrderId", 0);
                object actualOrdNum;
                cnqDict.TryGetValue((long)entryData.OrderId, out actualOrdNum);
                entryData.ActualOrderNum = Convert.ToInt32(actualOrdNum);
                entryData.RawStartTime = Convert.ToInt64(gomEnt.ValueOrDefault<object>("wevConquestsStartTime", 0));

                DateTime time = new DateTime(1601, 1, 1, 0, 0, 0, 0, System.DateTimeKind.Utc);
                time = time.AddSeconds(entryData.RawStartTime/ 1000).ToLocalTime();
                entryData.StartTime = time;
                if (!ActiveConquestData.ContainsKey(entryData.Id))
                    ActiveConquestData.Add(entryData.Id, new List<ConquestData>());
                ActiveConquestData[entryData.Id].Add(entryData);
            }
            achProto.Unload();
        }
    }

}
