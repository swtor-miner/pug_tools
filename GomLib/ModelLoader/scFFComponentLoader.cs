using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GomLib.Models;

namespace GomLib.ModelLoader
{
    public class SCFFComponentLoader
    {
        Dictionary<object, object> talentTreeLookup;
        Dictionary<object, object> talentCostLookup;
        Dictionary<object, object> componentCostLookup;
        Dictionary<object, object> componentAppearanceLookup;

        DataObjectModel _dom;

        public SCFFComponentLoader(DataObjectModel dom)
        {
            _dom = dom;
            Flush();
        }

        public void Flush()
        {
            talentTreeLookup = new Dictionary<object,object>();
            talentCostLookup = new Dictionary<object, object>();
            componentCostLookup = new Dictionary<object, object>();
            componentAppearanceLookup = new Dictionary<object, object>();
        }

        public Models.scFFComponent Load(ulong nodeId)
        {
            GomObject obj = _dom.GetObject(nodeId);
            Models.scFFComponent cmp = new scFFComponent();
            return Load(cmp, obj);
        }

        public Models.scFFComponent Load(Models.scFFComponent cmp, GomObject obj)
        {
            if (obj == null) { return cmp; }
            if (cmp == null) { return null; }

            if (talentTreeLookup.Count == 0)
            {
                talentTreeLookup = _dom.GetObject("scFFComponentUpgradePackagesPrototype").Data.Get<Dictionary<object, object>>("scFFComponentUpgradePackagesData");
                talentCostLookup = _dom.GetObject("scFFComponentUpgradesCostPrototype").Data.Get<Dictionary<object, object>>("scFFComponentUpgradesCostData");
                componentCostLookup = _dom.GetObject("scFFComponentsCostPrototype").Data.Get<Dictionary<object, object>>("scFFComponentsCostData");
                componentAppearanceLookup = _dom.GetObject("scFFComponentAppearanceDataPrototype").Data.Get<Dictionary<object, object>>("scFFComponentAppearanceData");
            }

            cmp.Fqn = obj.Name;
            cmp.Id = obj.Data.Get<ulong>("conEntitySpec");
            //var container = DataObjectModel.GetObject(cmp.NodeId);
            cmp.ComponentId = obj.Data.ValueOrDefault<long>("scFFComponentId", 0);

            cmp.NumUpgradeTiers = obj.Data.ValueOrDefault<long>("scFFComponentUpgradeTierCount", 0);
            
            ScriptEnum availability = obj.Data.Get<ScriptEnum>("scFFComponentAvailability"); //ex. scFFUnavailable
            CheckAvailability(cmp, availability);

            cmp.ColorOption = obj.Data.ValueOrDefault<string>("scFFComponentDefaultColor", null);

            cmp.Slot = obj.Data.ValueOrDefault<ScriptEnum>("scFFComponentEquipSlot", null).ToString().Replace("conSlotEquipSCFF", "");

            cmp.NameId = obj.Data.ValueOrDefault<long>("scFFComponentName", 0);
            cmp.Name = _dom.stringTable.TryGetString("str.spvp.components", cmp.NameId);

            cmp.ControllerNodeId = obj.Data.ValueOrDefault<ulong>("scFFController", 0);
            cmp.StatsList = new Dictionary<string, float>();
            var controller = _dom.GetObject(cmp.ControllerNodeId);
            if (controller != null)
            {
                var gomStats = controller.Data.ValueOrDefault<Dictionary<object, object>>("modStatBase", null);
                if (gomStats != null)
                {
                    var stringStats = gomStats.ToDictionary(k => k.Key.ToString(), k => (float)k.Value);
                    gomStats = null;
                    /*foreach (var statEntry in gomStats)
                    {
                        cmp.StatsList.Add(StatD.ToStat(statEntry.Key.ToString()).Replace("Space PvP ", "").Replace("Space PVP ", "").Replace(' ', '_'), (float)statEntry.Value);
                    }*/
                    List<string> statNames = new List<string>()
                    {
                        "stat_spc_accuracy",
                        "stat_spc_armor_penetration",
                        "STAT_spc_crit_damage",
                        "stat_spc_damage",
                        "stat_spc_firing_arc",
                        "stat_spc_hull_efficiency",
                        "stat_spc_range_long",
                        "stat_spc_range_mid",
                        "stat_spc_range_point_blank",
                        "stat_spc_shield_efficiency",
                        "stat_spc_shield_piercing",
                        "stat_spc_weapon_ammo_pool_size",
                        "stat_spc_weapon_charge_seconds",
                        "stat_spc_weapon_cooldown_seconds",
                        "stat_spc_weapon_lock_on_seconds",
                        "stat_spc_weapon_power_draw",
                        "stat_spc_weapon_rate_of_fire",
                        "stat_spc_weapon_reload_seconds",
                        "STAT_spc_crit_chance"
                    };

                    foreach (var stat in statNames)
                    {
                        float value = 0;
                        stringStats.TryGetValue(stat, out value);
                        cmp.StatsList.Add(_dom.statData.ToStat(stat).ToString().Replace("Space PvP ", "").Replace("Space PVP ", "").Replace(' ', '_'), (float)value);
                    }

                    var unknownMulti = controller.Data.ValueOrDefault<float>("4611686298398014003", 0); // 70, 85 for Cannons, 1E+07 for Railguns, and 1E+08 for Missiles
                    //var eppId = controller.Data.ValueOrDefault<ulong>("scFFControllerEpp", 0); // 16140946975682401685
                    //var firingMode = controller.Data.ValueOrDefault<ScriptEnum>("scFFControllerRequiredMode", null).ToString(); // scShipFiringModeSniper

                    if (!cmp.StatsList.ContainsKey("Ammo_Pool_Size"))
                    {
                        cmp.StatsList["Ammo_Pool_Size"] = controller.Data.ValueOrDefault<float>("scFFControllerAmmoPoolSize", 0); // Infinity
                    }

                    cmp.StatsList["pbRangeDamMulti"] = controller.Data.ValueOrDefault<float>("scFFControllerPbRangeDamMulti", 0); // 1
                    cmp.StatsList["midRangeDamMulti"] = controller.Data.ValueOrDefault<float>("scFFControllerMidRangeDamMulti", 0); // 0.79
                    cmp.StatsList["longRangeDamMulti"] = controller.Data.ValueOrDefault<float>("scFFControllerLongRangeDamMulti", 0); // 0.64

                    cmp.StatsList["pbRangeAccMulti"] = controller.Data.ValueOrDefault<float>("scFFControllerPbRangeAccMulti", 0); // 0.1
                    cmp.StatsList["midRangeAccMulti"] = controller.Data.ValueOrDefault<float>("scFFControllerMidRangeAccMulti", 0); // -0.1
                    cmp.StatsList["longRangeAccMulti"] = controller.Data.ValueOrDefault<float>("scFFControllerLongRangeAccMulti", 0); // -0.2

                    cmp.StatsList["trackingAccuracyLoss"] = controller.Data.ValueOrDefault<float>("scFFControllerTrackingAccuracyLoss", 0); // 0.1
                    //var damAblId = controller.Data.ValueOrDefault<ulong>("scFFControllerAbility", 0); // 16141065603985844578

                    var unknownBool = controller.Data.ValueOrDefault<bool>("4611686350202417000", false); // true
                }
                else
                {
                    cmp.ControllerNodeId = 0; //hack to disable drone/mines
                }
                controller.Unload();
            }
            
            
            cmp.Icon = obj.Data.ValueOrDefault<string>("scFFComponentIcon", "");
            _dom._assets.icons.Add(cmp.Icon);

            cmp.DescriptionId = obj.Data.ValueOrDefault<long>("scFFComponentDescription", 0);
            cmp.Description = _dom.stringTable.TryGetString("str.spvp.components", cmp.DescriptionId);

            cmp.CostLookupId = obj.Data.ValueOrDefault<ulong>("scFFComponentCostId", 0); //ex. 16140964519275703828
            cmp.Cost = -1;
            if (componentCostLookup.ContainsKey(cmp.CostLookupId))
            {
                cmp.Cost = Convert.ToInt32(((GomObjectData)componentCostLookup[cmp.CostLookupId]).ValueOrDefault<long>("scFFShipRequisitionCost", -1));
            }
            cmp.TalentCostList = new Dictionary<int, int>();
            if (talentCostLookup.ContainsKey(cmp.CostLookupId))
            {
                Dictionary<object, object> talentCostData = (Dictionary<object, object>)talentCostLookup[cmp.CostLookupId];
                foreach (var row in talentCostData)
                {
                    int rowCost = Convert.ToInt32(((GomObjectData)row.Value).ValueOrDefault<long>("scFFShipRequisitionCost", 0));
                    cmp.TalentCostList.Add(Convert.ToInt32(row.Key), rowCost);
                }
            }

            cmp.UnknownId = obj.Data.ValueOrDefault<long>("4611686348912157005", 0); //ex. -1, 4
            cmp.UsedByShipId = obj.Data.ValueOrDefault<long>("scFFComponentShipId", 0);

            cmp.Model = "";
            if (componentAppearanceLookup.ContainsKey(cmp.CostLookupId))
            {
                var controllerAppearanceMap = (Dictionary<object, object>)componentAppearanceLookup[cmp.CostLookupId];
                if (controllerAppearanceMap.ContainsKey(cmp.UsedByShipId))
                {
                    cmp.Model = ((GomObjectData)controllerAppearanceMap[cmp.UsedByShipId]).ValueOrDefault<string>("scFFComponentAppearance", "");
                }
            }

            scFFTalentTree scTree = new scFFTalentTree();
            scTree.Tree = new Dictionary<int, Dictionary<int, List<object>>>();
            if (talentTreeLookup.ContainsKey(cmp.ComponentId))
            {
                Dictionary<object, object> talentData = ((GomObjectData)talentTreeLookup[cmp.ComponentId]).Get<Dictionary<object, object>>("scFFComponentTalentData");
                foreach (var talentSlot in talentData)
                {
                    if ((long)talentSlot.Key == 0)
                    {

                        /*tal = TalentLoader.Load(talentId);
                        cmp.TalentList.Add(tal);*/
                    }
                    switch ((long)talentSlot.Key)
                    {
                        case 0:
                            var abilityTier = (GomObjectData)((Dictionary<object, object>)talentSlot.Value)[(long)1];
                            scTree.Ability = _dom.abilityLoader.Load((ulong)(abilityTier.ValueOrDefault("scFFAbilityId", new ulong())));
                            break;
                        default:
                            scTree.Tree.Add(Convert.ToInt32(talentSlot.Key), ParseTalentTier(talentSlot));
                            break;
                    }

                }
            }
            cmp.Talents = scTree;

            var talentList = new Dictionary<int, GameObject>();
            if (obj.Data.ContainsKey("scFFComponentAblTalList")) //this looks to be a backup list of sorts in case the component prototype doesn't have the data.
            {
                Dictionary<object, object> talentLookup = obj.Data.Get<Dictionary<object, object>>("scFFComponentAblTalList");
                foreach (var talentLookupPair in talentLookup)
                {
                    GomObject talentOrAbility = _dom.GetObject((ulong)talentLookupPair.Value);
                    if (talentOrAbility != null)
                    {
                        if (talentOrAbility.Name.StartsWith("tal"))
                        {
                            Talent tal = new Talent();
                            _dom.talentLoader.Load(tal, talentOrAbility);
                            talentList.Add(Convert.ToInt32(talentLookupPair.Key), tal);
                        }
                        else
                        {
                            Ability abl = new Ability();
                            _dom.abilityLoader.Load(abl, talentOrAbility);
                            talentList.Add(Convert.ToInt32(talentLookupPair.Key), abl);
                        }
                        talentOrAbility.Unload();
                    }
                }
            }
            cmp.TalentList = talentList;

            obj.Unload();
            return cmp;
        }


        private Dictionary<int, List<object>> ParseTalentTier(KeyValuePair<object, object> talentSlot)
        {
            Dictionary<int, List<object>> tier = new Dictionary<int, List<object>>();
            foreach (var tierLookup in (Dictionary<object, object>)talentSlot.Value)
            {
                var ablId = (((GomObjectData)tierLookup.Value).ValueOrDefault<ulong>("scFFAbilityId", 0)); 
                var talId = (((GomObjectData)tierLookup.Value).ValueOrDefault<ulong>("scFFTalentId", 0));
                var target = (((GomObjectData)tierLookup.Value).ValueOrDefault<object>("scFFTalentTarget", new ScriptEnum()));
                if (ablId != 0)
                {
                    Ability abl = _dom.abilityLoader.Load(ablId);
                    tier.Add(Convert.ToInt32(tierLookup.Key), new List<object>() { abl, target });
                }
                else if (talId != 0)
                {
                    Talent tal = _dom.talentLoader.Load(talId);
                    tier.Add(Convert.ToInt32(tierLookup.Key), new List<object>() { tal, target });
                }
            }
            return tier;
        }

        private void CheckAvailability(Models.scFFComponent cmp, ScriptEnum availability)
        {
            bool available = false;
            bool deprecated = false;
            switch (availability.ToString())
            {
                case "scFFDeprecated":
                    deprecated = true;
                    break;
                case "scFFAvailable": //default is scFFUnavailable
                    available = true;
                    break;
            }
            cmp.IsAvailable = available;
            cmp.IsDeprecated = deprecated;
            availability = null;
        }
    }
}
