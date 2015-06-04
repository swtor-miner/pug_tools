using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GomLib.Models;

namespace GomLib.ModelLoader
{
    public class SchematicLoader
    {
        const long strOffset = 0x35D0200000000;

        StringTable missionStrTable;
        Dictionary<ulong, Schematic> idMap;
        Dictionary<string, Schematic> nameMap;
        //static int maxMats = 0;

        DataObjectModel _dom;

        public SchematicLoader(DataObjectModel dom)
        {
            _dom = dom;
            Flush();
        }
        
        public void Flush()
        {
            missionStrTable = null;
            idMap = new Dictionary<ulong, Schematic>();
            nameMap = new Dictionary<string, Schematic>();
        }

        public string ClassName
        {
            get { return "prfSchematic"; }
        }

        public Models.Schematic Load(ulong nodeId)
        {
            Schematic result;
            if (idMap.TryGetValue(nodeId, out result))
            {
                return result;
            }

            GomObject obj = _dom.GetObject(nodeId);
            Schematic sch = new Schematic();
            return Load(sch, obj);
        }

        public Models.Schematic Load(string fqn)
        {
            Schematic result;
            if (nameMap.TryGetValue(fqn, out result))
            {
                return result;
            }

            GomObject obj = _dom.GetObject(fqn);
            Schematic sch = new Schematic();
            return Load(sch, obj);
        }

        public Models.Schematic Load(GomObject obj)
        {
            Schematic sch = new Schematic();
            return Load(sch, obj);
        }

        public Models.GameObject CreateObject()
        {
            return new Models.Schematic();
        }

        public Models.Schematic Load(Models.Schematic schem, GomObject obj)
        {
            if (obj == null) { return schem; }
            if (schem == null) { return null; }

            if (missionStrTable == null)
            {
                missionStrTable = _dom.stringTable.Find("str.prf.missions");
            }
            schem.Id = obj.Id;
            schem.Fqn = obj.Name;
            schem._dom = _dom;
            schem.References = obj.References;
            schem.Deprecated = obj.Data.ValueOrDefault<bool>("prfSchematicDeprecated", false);
            schem.DisableCritical = obj.Data.ValueOrDefault<bool>("prfDisableCritical", false);
            schem.DisableDisassemble = obj.Data.ValueOrDefault<bool>("prfDisableDisassemble", false);

            schem.ItemId = (ulong)obj.Data.ValueOrDefault<ulong>("prfSchematicItemSpec", 0);
            /*if (itemId > 0)
            {
                schem.Item = _dom.itemLoader.Load(itemId);
                if (schem.Item != null)
                {
                    schem.Id = schem.Item.Id;
                }
                else
                {
                    Console.WriteLine("Schematic references non-existant item: " + schem.Fqn);
                }
            }*/

            schem.MissionCost = (int)obj.Data.ValueOrDefault<long>("prfMissionCost", 0);
            schem.MissionFaction = FactionExtensions.ToFaction((long)obj.Data.ValueOrDefault<long>("prfMissionFaction", 0));
            schem.MissionUnlockable = obj.Data.ValueOrDefault<bool>("prfMissionUnlockable", false);
            schem.MissionLight = (int)obj.Data.ValueOrDefault<long>("prfMissionRewardLight", 0);
            schem.MissionLightCrit = (int)obj.Data.ValueOrDefault<long>("prfMissionRewardLightCritical", 0);
            schem.MissionDark = (int)obj.Data.ValueOrDefault<long>("prfMissionRewardDark", 0);
            schem.MissionDarkCrit = (int)obj.Data.ValueOrDefault<long>("prfMissionRewardDarkCritical", 0);

            schem.NameId = (ulong)obj.Data.ValueOrDefault<long>("prfSchematicNameId", 0);
            if (schem.NameId > 0)
            {
                schem.Name = missionStrTable.GetText((int)schem.NameId + strOffset, schem.Fqn);
                schem.LocalizedName = missionStrTable.GetLocalizedText((int)schem.NameId + strOffset, schem.Fqn);
                //schem.Id = schem.NameId;
            }

            if (String.IsNullOrEmpty(schem.Name) && (schem.Item != null))
            {
                schem.Name = schem.Item.LocalizedName["enMale"];
            }

            schem.MissionYieldDescriptionId = (int)obj.Data.ValueOrDefault<long>("prfMissionYieldDescriptionId", 0);
            if (schem.MissionYieldDescriptionId > 0)
            {
                schem.MissionYieldDescription = missionStrTable.GetText(schem.MissionYieldDescriptionId + strOffset, schem.Fqn);
                schem.LocalizedMissionYieldDescription = missionStrTable.GetLocalizedText(schem.MissionYieldDescriptionId + strOffset, schem.Fqn);
            }

            schem.MissionDescriptionId = (int)obj.Data.ValueOrDefault<long>("prfMissionDescriptionId", 0);
            if (schem.MissionDescriptionId > 0) schem.MissionDescription = missionStrTable.GetText(schem.MissionDescriptionId + strOffset, schem.Fqn);

            schem.CrewSkillId = ProfessionExtensions.ToProfession((ScriptEnum)obj.Data.ValueOrDefault<ScriptEnum>("prfProfessionRequired", null));
            schem.Subtype = ProfessionSubtypeExtensions.ToProfessionSubtype((ScriptEnum)obj.Data.ValueOrDefault<ScriptEnum>("prfProfessionSubtype", null));

            schem.SkillGrey = (int)obj.Data.ValueOrDefault<long>("prfSchematicGrey", 0);
            schem.SkillGreen = (int)obj.Data.ValueOrDefault<long>("prfSchematicGreen", 0);
            schem.SkillYellow = (int)obj.Data.ValueOrDefault<long>("prfSchematicYellow", 0);
            schem.SkillOrange = (int)obj.Data.ValueOrDefault<long>("prfProfessionLevelRequired", 0);

            schem.TrainingCost = (int)obj.Data.ValueOrDefault<long>("prfTrainingCost", 0);
            schem.Workstation = WorkstationExtensions.ToWorkstation((ScriptEnum)obj.Data.ValueOrDefault<ScriptEnum>("prfWorkstationRequired", null));

            var materials = (Dictionary<object, object>)obj.Data.ValueOrDefault<Dictionary<object, object>>("prfSchematicMaterials", null);
            if (materials != null)
            {
                int matIdx = 1;
                schem.Materials = new Dictionary<ulong, int>();
                foreach (var mat_quantity in materials)
                {
                    schem.Materials.Add((ulong)mat_quantity.Key, (int)(long)mat_quantity.Value);
                    matIdx++;
                }
                //if (materials.Count > maxMats) maxMats = materials.Count;
            }

            var researchChance = (Dictionary<object, object>)obj.Data.ValueOrDefault<Dictionary<object, object>>("prfSchematicResearchChances", null);
            if (researchChance != null)
            {
                int rLvl = 1;
                foreach (var r_chance in researchChance)
                {
                    switch (rLvl)
                    {
                        case 1: schem.ResearchChance1 = SchematicResearchChanceExtensions.ToSchematicResearchChance((ScriptEnum)r_chance.Value); break;
                        case 2: schem.ResearchChance2 = SchematicResearchChanceExtensions.ToSchematicResearchChance((ScriptEnum)r_chance.Value); break;
                        case 3: schem.ResearchChance3 = SchematicResearchChanceExtensions.ToSchematicResearchChance((ScriptEnum)r_chance.Value); break;
                        default: throw new InvalidOperationException("This schematic has 4 tiers of research!?!");
                    }

                    rLvl++;
                }
            }

            var craftTime = (List<object>)obj.Data.ValueOrDefault<List<object>>("prfSchematicCraftingTime", null);
            if (craftTime != null)
            {
                int timeIdx = 0;
                foreach (var time in craftTime)
                {
                    switch (timeIdx)
                    {
                        case 0: schem.CraftingTime = (int)(ulong)time / 1000; break;
                        case 1: schem.CraftingTimeT1 = (int)(ulong)time / 1000; break;
                        case 2: schem.CraftingTimeT2 = (int)(ulong)time / 1000; break;
                        case 3: schem.CraftingTimeT3 = (int)(ulong)time / 1000; break;
                        default: throw new InvalidOperationException("This schematic has 4 tiers of research!?!");
                    }

                    timeIdx++;
                }
            }

            var researchMaterials = (Dictionary<object, object>)obj.Data.ValueOrDefault<Dictionary<object, object>>("prfSchematicResearchMaterials", null);
            if (researchMaterials != null)
            {
                int idx = 1;
                foreach (var r_mats in researchMaterials)
                {
                    Dictionary<object, object> matLookup = (Dictionary<object, object>)r_mats.Value;
                    if (matLookup.Count > 1)
                    {
                        throw new InvalidOperationException("Research Tier adds more than one material");
                    }
                    ulong matId = (ulong)matLookup.First().Key;
                    int matQuantity = (int)(long)matLookup.First().Value;

                    if (matId > 0)
                    {
                        switch (idx)
                        {
                            case 1:
                                schem.Research1 = _dom.itemLoader.Load(matId);
                                schem.ResearchQuantity1 = matQuantity;
                                break;
                            case 2:
                                schem.Research2 = _dom.itemLoader.Load(matId);
                                schem.ResearchQuantity2 = matQuantity;
                                break;
                            case 3:
                                schem.Research3 = _dom.itemLoader.Load(matId);
                                schem.ResearchQuantity3 = matQuantity;
                                break;
                            default: throw new InvalidOperationException("This schematic has 4 tiers of research!?!");
                        }
                    }
                    idx++;
                }
                //if(materials!= null)
                //    if (materials.Count + researchMaterials.Count > maxMats) maxMats = materials.Count + researchMaterials.Count;
                //if (researchMaterials.Count > maxMats) maxMats = researchMaterials.Count;
            }
            if (schem.MissionDescriptionId != 0)
            {
                schem.Category = "Mission";
                schem.Quality = "mission";
            }
            else
            {
                if (schem.ItemId != 0)
                {
                    if (schem.Item != null)
                    {
                        if (schem.Item.AuctionCategory != null) schem.Category = schem.Item.AuctionCategory.ToString();
                        if (schem.Item.AuctionSubCategory != null) schem.SubCategory = schem.Item.AuctionSubCategory.ToString();
                        schem.Quality = ((schem.Item.IsModdable && (schem.Item.Quality == ItemQuality.Prototype)) ? "moddable" : schem.Item.Quality.ToString().ToLower());
                    }
                    else
                    {
                        schem.Category = "Unknown";
                        schem.Quality = "cheap";
                    }
                }
                else
                {
                    schem.Category = "Unknown";
                    schem.Quality = "mission";
                }
            }

            _dom.itemLoader.LoadChildMap();
            List<ulong> pIds;
            _dom.itemLoader.schematicLookupMap.TryGetValue(schem.Id, out pIds);
            schem.LearnedIds = pIds ?? new List<ulong>(); //null coalesce so we don't have to account for it later.

            if (idMap.Values.Where(s => s.Id == schem.Id).Count() > 0)
            {
                throw new InvalidOperationException("Attempting to set Id of a schematic to one that's already taken");
            }
            if (obj.References != null)
            {
                if (obj.References.ContainsKey("trainerTaught"))
                {
                    schem.TrainerTaught = true;
                }
            }
            obj.Unload();
            return schem;
        }

        public void LoadObject(Models.GameObject loadMe, GomObject obj)
        {
            GomLib.Models.Schematic sch = (Models.Schematic)loadMe;
            Load(sch, obj);
        }

        public void LoadReferences(Models.GameObject obj, GomObject gom)
        {
            // No references to load
        }
    }
}
