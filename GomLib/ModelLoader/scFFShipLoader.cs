using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GomLib.Models;

namespace GomLib.ModelLoader
{
    public class SCFFShipLoader
    {
        Dictionary<object, object> shipMap;
        Dictionary<object, object> colorMap;
        Dictionary<object, object> ComponentColorUIData;
        Dictionary<object, object> patternMap;
        Dictionary<object, object> patternUIData;
        Dictionary<object, object> defaultLoadoutData;

        DataObjectModel _dom;

        public SCFFShipLoader(DataObjectModel dom)
        {
            _dom = dom;
            Flush();
        }

        public void Flush()
        {
            shipMap = new Dictionary<object, object>();
            colorMap = new Dictionary<object, object>();
            ComponentColorUIData = new Dictionary<object, object>();
            patternMap = new Dictionary<object, object>();
            patternUIData = new Dictionary<object, object>();
            defaultLoadoutData = new Dictionary<object, object>();
        }

        public Models.scFFShip Load(Models.scFFShip shp, long Id, GomObjectData obj)
        {
            if (obj == null) { return shp; }
            if (shp == null) { return null; }

            if (shipMap != null && shipMap.Count == 0)
            {
                GomObject masterMap = _dom.GetObject("MasterComponentMap");
                if (masterMap != null)
                {
                    shipMap = masterMap.Data.ValueOrDefault<Dictionary<object, object>>("scFFShipMasterComponentMap", null);
                    masterMap.Unload();
                }

                GomObject masterColorMap = _dom.GetObject("scFFColorOptionMasterPrototype");
                if (masterColorMap != null)
                {
                    colorMap = masterColorMap.Data.ValueOrDefault<Dictionary<object, object>>("scffFactionColorMap", null);
                    ComponentColorUIData = masterColorMap.Data.ValueOrDefault<Dictionary<object, object>>("scFFComponentColorUIData", null);
                    masterColorMap.Unload();
                }

                GomObject masterPatternData = _dom.GetObject("scFFPatternsDefinitionProtoype"); // Protoype ?!?
                if (masterPatternData != null)
                {
                    patternMap = masterPatternData.Data.ValueOrDefault<Dictionary<object, object>>("scFFShipPatternMap", null);
                    patternUIData = masterPatternData.Data.ValueOrDefault<Dictionary<object, object>>("scFFShipPatternUIData", null);
                    masterPatternData.Unload();
                }

                GomObject masterLoadoutData = _dom.GetObject("scFFShipDefaultLoadoutsPrototype");
                if (masterLoadoutData != null)
                {
                    defaultLoadoutData = masterLoadoutData.Data.ValueOrDefault<Dictionary<object, object>>("scFFDefaultLoadoutData", null);
                    masterLoadoutData.Unload();
                }
            }

            shp._dom = _dom;
            shp.Prototype = "scFFShipsDataPrototype";
            shp.ProtoDataTable = "scFFShipsData";

            long shortId = obj.ValueOrDefault<long>("scFFShortId", 0); //scFFShortIdtoShipIdMap has a lookup list for these in the current prototype ex. 1682
            object id = new object();
            Dictionary<object, object> shipIdLookup = new Dictionary<object, object>();
            _dom.GetObject("scFFShipsDataPrototype").Data.ValueOrDefault<Dictionary<object, object>>("scFFShortIdtoShipIdMap", shipIdLookup).TryGetValue(shortId, out id);
            shp.LookupId = 0;
            if (id != null)
            {
                shp.LookupId = (long)((List<object>)id)[0];
            }
            shp.Id = Id;
            /*if (shp.Id != shp.LookupId)
            {
                return shp;
            }*/ //not sure what this was originally intended to do.

            shp.Model = obj.Get<string>("scFFShipModel"); // ex. "/art/dynamic/space_pvp/ships/imp_striker/imp_striker_c.gr2"

            shp.minorComponentsContainerId = obj.Get<ulong>("scFFMinorComponentsPackage"); //ex. 16140923285529548575 (Node conSpec_scff_equip_min_ACRS) 

            shp.MinorComponentSlots = new Dictionary<string, long>();
            GomObject minorComponentData = _dom.GetObject(shp.minorComponentsContainerId);
            var minorContainerSlots = minorComponentData.Data.Get<Dictionary<object, object>>("conContainerDataSlots");
            string minorEquipType = minorComponentData.Data.Get<string>("conContainerEventType");
            shp.minorEquipType = minorEquipType.Replace("conSpec_scff_equip_min_", "");
            minorComponentData.Unload();

            foreach(var slot in minorContainerSlots)
            {
                shp.MinorComponentSlots.Add(slot.Key.ToString().Replace("conSlotEquipSCFF", "").Replace("AuxSystem", "Systems"), (long)slot.Value);
            }

            shp.majorComponentsContainerId = obj.Get<ulong>("scFFMajorComponentsPackage"); //ex. 16141040006346149027 (Node conSpec_scff_equip_maj_PSYHE)

            shp.MajorComponentSlots = new Dictionary<string, long>();
            GomObject majorEquipData = _dom.GetObject(shp.majorComponentsContainerId);
            var majorContainerSlots = majorEquipData.Data.Get<Dictionary<object, object>>("conContainerDataSlots");

            string majorEquipType = majorEquipData.Data.Get<string>("conContainerEventType");
            shp.majorEquipType = majorEquipType.Replace("conSpec_scff_equip_maj_", "");
            majorEquipData.Unload();

            foreach (var slot in majorContainerSlots)
            {
                shp.MajorComponentSlots.Add(slot.Key.ToString().Replace("conSlotEquipSCFF", "").Replace("AuxSystem", "Systems"), (long)slot.Value);
            }

            object shipComponentMap = new object();
            if (shipMap != null)
            {
                shipMap.TryGetValue(Id, out shipComponentMap);
                shp.ComponentMap = new Dictionary<string, List<scFFComponent>>();
                foreach (var componentSlot in (Dictionary<object, object>)shipComponentMap)
                {
                    List<scFFComponent> compNames = new List<scFFComponent>();
                    string compName = componentSlot.Key.ToString().Replace("conSlotEquipSCFF", "").Replace("AuxSystem", "Systems");

                    int c = 0;
                    foreach (var cNodeLookup in (List<object>)componentSlot.Value)
                    {
                        scFFComponent cmp = _dom.scFFComponentLoader.Load((ulong)cNodeLookup);
                        if (cmp.ComponentId == 0) cmp.ComponentId = c;
                        compNames.Add(cmp);
                        c++;
                    }
                    shp.ComponentMap.Add(compName, compNames);
                }
            }

            shp.damagedPackageNodeId = obj.Get<ulong>("scFFDamagePackage"); //ex. 16141171526721883186 (Node imp_striker_c_damage_package)

            ScriptEnum availability = obj.Get<ScriptEnum>("scFFAvailability"); //ex. scFFUnavailable
            CheckAvailability(shp, availability);

            long lgcAchEvt = obj.ValueOrDefault<long>("lgcAchievementEvents", 0); // lgcAchievementEventsPrototype - Legacy Achievement Events Lookup
            long crewPkg = obj.ValueOrDefault<long>("scFFCrewPackage", 0); // scFFCrewPackagesPrototype - Crew Package Lookup
            long shipColorId = obj.ValueOrDefault<long>("scffFactionId", 0); // scFFColorOptionMasterPrototype - Color Option Master Lookup
            object shipColorOptionsMap = new object();
            shp.ColorOptions = new Dictionary<string, List<scFFColorOption>>();
            if (colorMap != null)
            {
                colorMap.TryGetValue(shipColorId, out shipColorOptionsMap);
                if (shipColorOptionsMap != null)
                {
                    foreach (var colorList in (Dictionary<object, object>)shipColorOptionsMap)
                    {
                        List<scFFColorOption> colorNames = new List<scFFColorOption>();
                        string colorListName = colorList.Key.ToString();
                        foreach (object colorId in (List<object>)colorList.Value)
                        {
                            //Console.WriteLine(colorId.ToString());
                            object colorData = new object();
                            ComponentColorUIData.TryGetValue(colorId, out colorData);
                            scFFColorOption col = new scFFColorOption();
                            _dom.scFFColorOptionLoader.Load(col, (GomLib.GomObjectData)colorData);
                            colorNames.Add(col);                                                        
                        }
                        shp.ColorOptions.Add(colorListName, colorNames);                        
                    }
                }                                              
            }

            shp.EppDynamicCollectionId = obj.ValueOrDefault("scFFEppDynamicCollection", (ulong)0); //ex. 16140982269598876670 (Node imp_striker_c_eppdynamicdata_collection)

            shp.DescriptionId = obj.ValueOrDefault<long>("scFFShipDescription", 0); //str.spvp.ships.stb ex. 3282759468450093
            shp.Description = _dom.stringTable.TryGetString("str.spvp.ships", shp.DescriptionId);

            long patternDefinition = obj.ValueOrDefault<long>("scFFPatternId", 0);  //scFFPatternsDefinitionPrototype - Pattern Definition Lookup
            object shipPatMap = new object();
            shp.PatternOptions = new List<scFFPattern>();
            if (patternMap != null)
            {
                patternMap.TryGetValue(patternDefinition, out shipPatMap);
                if (shipPatMap != null)
                {
                    foreach (var patternId in (List<object>)shipPatMap)
                    {
                        object patData = new object();
                        patternUIData.TryGetValue(patternId, out patData);
                        scFFPattern pat = new scFFPattern();
                        _dom.scFFPatternLoader.Load(pat, (GomObjectData)patData);
                        pat.TextureForCurrentShip = pat.TexturesByShipId[shp.Id];
                        shp.PatternOptions.Add(pat);
                    }
                }
            }

            shp.DefaultLoadout = new Dictionary<string, ulong>();
            if (defaultLoadoutData != null)
            {
                object shipLoadoutMap;
                defaultLoadoutData.TryGetValue((ulong)shp.Id, out shipLoadoutMap);
                if (shipLoadoutMap != null)
                {
                    foreach (var slotList in (Dictionary<object, object>)shipLoadoutMap)
                    {
                        string slotName = slotList.Key.ToString().Replace("conSlotEquipSCFF", "").Replace("AuxSystem", "Systems");
                        //scFFComponent lCmp = scFFComponentLoader.Load((ulong)((List<object>)slotList.Value)[0]); //don't need to load the full component. This was for testing

                        shp.DefaultLoadout.Add(slotName, (ulong)((List<object>)slotList.Value)[0]);
                        if (((List<object>)slotList.Value).Count > 1)
                        {
                            //lCmp = scFFComponentLoader.Load((ulong)((List<object>)slotList.Value)[1]); //for testing not needed.
                            shp.DefaultLoadout.Add(slotName + 2, (ulong)((List<object>)slotList.Value)[1]);
                        }
                    }
                }
            }

            shp.NameId = obj.Get<long>("scFFShipName"); //str.spvp.ships.stb ex. 3282759468450057
            shp.Name = _dom.stringTable.TryGetString("str.spvp.ships", shp.NameId);

            ulong interdictionDriveEppNodeId = obj.ValueOrDefault<ulong>("scFFAfterBurnerEpp", 0); //ex. 16140995633020982770 (Node epp.space_combat.freeflight.imperial.fighter_afterburner)

            shp.unknownStat1 = obj.ValueOrDefault<float>("4611686298643904002", 0); //scale ?? ex. 0.15 - Override Power_Shield_Regen_Rate Modifier?
            shp.unknownStat2 = obj.ValueOrDefault<float>("4611686298656584000", 0); // ?? ex. 0.0025

            shp.unknownStat3 = obj.ValueOrDefault<float>("4611686348394117006", 0); //ex. 2
            shp.unknownStat4 = obj.ValueOrDefault<float>("4611686348594427000", 0); //ex. 2
            shp.unknownStat5 = obj.ValueOrDefault<float>("4611686348594427001", 0); //ex. 1.25

            shp.unknownStat6 = obj.ValueOrDefault<float>("4611686348976567004", 0); //ex. 0.032
            shp.unknownStat7 = obj.ValueOrDefault<float>("4611686348976567005", 0); //ex. 0.3
            shp.unknownStat8 = obj.ValueOrDefault<float>("4611686348976567007", 0); //ex. 0.6

            long unknown9 = obj.ValueOrDefault<long>("4611686349455207001", 0); //ex. 29


            ScriptEnum shipCategory = obj.ValueOrDefault("scFFShipCategory", new ScriptEnum());// only accessible ships have this.
            shp.Category = shipCategory.ToString().Replace("0x00", "Strike Fighter").Replace("scFFShip", "");
            
            /*switch(shipClass)
            {
                case "*/
            ulong shipStatPackageId = obj.Get<ulong>("scFFShipStatsPackage"); //ex. 16141013442936013929 pkg.pvp.striker

            GomObject statPackage = _dom.GetObject(shipStatPackageId);
            shp.Stats = new Dictionary<string, float>();
            if (statPackage != null)
            {
                Dictionary<object, object> statsObject = statPackage.Data.ValueOrDefault<Dictionary<object, object>>("scFFShipStatData", new Dictionary<object, object>());
                statPackage.Unload();
                foreach (var stat in statsObject)
                {
                    shp.Stats.Add(stat.Key.ToString(), (float)stat.Value);
                }
            }

            shp.engStatsNodeId = obj.Get<ulong>("scFFEngineStatsPackage"); //ex. 16140967054983465763 (Node spvp.eng.striker)
            GomObject engStatPackage = _dom.GetObject(shp.engStatsNodeId);
            if (engStatPackage != null)
            {
                foreach (var stat in engStatPackage.Data.Dictionary.Skip(3).ToDictionary(x => x.Key.ToString(), x => (float)x.Value))
                {
                    /*if (statNames.ContainsKey(stat.Key))
                    {
                        shp.Stats.Add(statNames[stat.Key], stat.Value);
                    }
                    else
                    {*/
                        shp.Stats.Add(stat.Key, stat.Value);
                    //}
                }
                engStatPackage.Unload();
            }


            ulong cameraPackageLookupNodeId = obj.Get<ulong>("scFFCameraPackage"); //ex. 16140941624160915343 (Node spvp.camera.package.imp_striker_c)


            shp.ShipIcon = obj.Get<string>("scFFShipHullIcon"); //ex. "shipicon_imp_sniper" //not sure what this does, but it's not the icon.
            shp.Icon = obj.ValueOrDefault("scFFShipIcon", ""); //ex. "spvp_imp_striker_3"
            _dom._assets.icons.Add(shp.Icon);
            _dom._assets.icons.Add(shp.ShipIcon);
            shp.Faction = "";

            long f = obj.Get<long>("scffFactionId");
            if (f == -1855280666668608219) // Imperial
            {
                shp.Faction = "Imperial";
            }
            else if (f == 1086966210362573345) //Republic
            {
                shp.Faction = "Republic";
            }
            else
            {
                shp.Faction = "???";
            }

            string engineSound = obj.ValueOrDefault<string>("scFFEngineSound", null); //ex. "Play_playership_engine_med_imp"
            string engineSoundRemote = obj.ValueOrDefault<string>("scFFEngineSoundRemote", null); //ex. "Play_playership_engine_med_imp_remote"

            ulong deathPkgNear = obj.Get<ulong>("scFFDeathPackageNear"); //FFDeathPackagePrototype - Death Package Near Lookup 
            ulong deathPkgFar = obj.Get<ulong>("scFFDeathPackageFar"); //FFDeathPackagePrototype - Death Package Far Lookup 

            var masterCostData = _dom.GetObject("scFFShipCostPrototype");
            
            if (masterCostData != null)
            {
                var costLookup = masterCostData.Data.Get<Dictionary<object, object>>("scFFShipCostMap");
                if (costLookup.ContainsKey(shp.Id))
                {
                    GomObjectData costData = (GomObjectData)costLookup[shp.Id];
                    shp.Cost = costData.ValueOrDefault<long>("scFFShipCost", 0);
                    shp.IsPurchasedWithCC = costData.ValueOrDefault<bool>("scFFIsPurchasedWithCC", false);
                }
                else
                {
                    shp.Cost = -1;
                }
            }
            ulong apcLookup = obj.ValueOrDefault("scFFShipAblPackage", new ulong());
            if (apcLookup != 0)
            {
                shp.abilityPackage = _dom.abilityPackageLoader.Load(apcLookup); //ex. 16141125605689795242 (Node apc.spvp.gunship.weapon_swap_secondary)
            }
            return shp;
        }

        private void CheckAvailability(Models.scFFShip shp, ScriptEnum availability)
        {
            bool available = false;
            bool deprecated = false;
            bool hidden = false;
            switch (availability.ToString())
            {
                case "scFFDeprecated":
                    deprecated = true;
                    break;
                case "scFFAvailable": //default is scFFUnavailable
                    available = true;
                    break;
                case "scFFHidden":
                    hidden = true;
                    break;
            }
            shp.IsAvailable = available;
            shp.IsDeprecated = deprecated;
            shp.IsHidden = hidden;
            availability = null;
        }
    }
}
