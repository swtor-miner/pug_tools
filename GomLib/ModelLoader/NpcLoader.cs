using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GomLib.Models;

namespace GomLib.ModelLoader
{
    public class NpcLoader
    {
        const long NameLookupKey = -2761358831308646330;
        const long TitleLookupKey = -8863348193830878519;

        Dictionary<ulong, Npc> idMap;
        Dictionary<string, Npc> nameMap;
        Dictionary<int, Npc> objIdMap;
        Dictionary<Toughness, Dictionary<string, string>> ToughnessLocalizationMap { get; set; }

        DataObjectModel _dom;

        public NpcLoader(DataObjectModel dom)
        {
            _dom = dom;
            Flush();
        }

        public void Flush()
        {
            idMap = new Dictionary<ulong, Npc>();
            nameMap = new Dictionary<string, Npc>();
            objIdMap = new Dictionary<int, Npc>();
            ToughnessLocalizationMap = new Dictionary<Toughness, Dictionary<string, string>>();
        }

        public string ClassName
        {
            get { return "chrNonPlayerCharacter"; }
        }

        public Models.Npc Load(ulong nodeId)
        {
            Npc result;
            if (idMap.TryGetValue(nodeId, out result))
            {
                return result;
            }

            GomObject obj = _dom.GetObject(nodeId);
            Npc npc = new Npc();
            return Load(npc, obj);
        }

        public Models.Npc Load(string fqn)
        {
            Npc result;
            if (nameMap.TryGetValue(fqn, out result))
            {
                return result;
            }

            GomObject obj = _dom.GetObject(fqn);
            Npc npc = new Npc();
            return Load(npc, obj);
        }

        public Models.Npc Load(GomObject obj)
        {
            Npc npc = new Npc();
            return Load(npc, obj);
        }

        public Models.GameObject CreateObject()
        {
            return new Models.Npc();
        }

        public Models.Npc Load(Models.GameObject obj, GomObject gom)
        {
            if (gom == null) { return (Npc)obj; }

            return Load(obj as Npc, gom);
        }

        public Models.Npc Load(Models.Npc npc, GomObject obj)
        {
            if (obj == null) { return npc; }
            if (npc == null) { return null; }

            if(ToughnessLocalizationMap.Count == 0)
            {
                var strGuiMiscPrototype = _dom.GetObject("strGuiMiscPrototype");
                var strGuiMiscEnumToStringId = strGuiMiscPrototype.Data.ValueOrDefault<Dictionary<object, object>>("strGuiMiscEnumToStringId");
                StringTable portTable = _dom.stringTable.Find("str.gui.portraitbar");
                StringTable miscTable = _dom.stringTable.Find("str.gui.auctionhouse");

                foreach (var toughness in Enum.GetValues(typeof(Toughness)).Cast<Toughness>())
                {
                    switch (toughness)
                    {
                        case Toughness.None:
                            ToughnessLocalizationMap.Add(toughness, null);
                            break;
                        case Toughness.Standard:
                            ToughnessLocalizationMap.Add(toughness, miscTable.GetLocalizedText(1002634345447535, "str.gui.auctionhouse"));
                            break;
                        case Toughness.Weak:
                            ToughnessLocalizationMap.Add(toughness, portTable.GetLocalizedText(946292964458570, "str.gui.portraitbar"));
                            break;
                        case Toughness.Strong:
                            ToughnessLocalizationMap.Add(toughness, portTable.GetLocalizedText(946292964458546, "str.gui.portraitbar"));
                            break;
                        case Toughness.Boss1:
                            ToughnessLocalizationMap.Add(toughness, portTable.GetLocalizedText(946292964458545, "str.gui.portraitbar"));
                            break;
                        case Toughness.BossRaid:
                        case Toughness.Boss2:
                        case Toughness.Boss3:
                        case Toughness.Boss4:
                            ToughnessLocalizationMap.Add(toughness, portTable.GetLocalizedText(946292964458567, "str.gui.portraitbar"));
                            break;
                    }
                }
            }
            npc.parentSpecId = obj.Data.ValueOrDefault<ulong>("npcParentSpecId", 0);
            npc.charRef = obj.Data.ValueOrDefault<string>("npcCharRef", "");
            Npc baseNpc;
            if (npc.parentSpecId > 0)
            {
                baseNpc = Load(npc.parentSpecId);
            }
            else
            {
                baseNpc = new Npc();
            }

            npc.Fqn = obj.Name;
            npc.NodeId = obj.Id;
            npc._dom = _dom;
            npc.References = obj.References;
            var textLookup = obj.Data.ValueOrDefault<Dictionary<object,object>>("locTextRetrieverMap", null);
            GomObjectData nameLookupData = (GomObjectData)textLookup[NameLookupKey];
            var nameId = nameLookupData.ValueOrDefault<long>("strLocalizedTextRetrieverStringID", 0);
            npc.NameId = nameId;
            npc.Name = _dom.stringTable.TryGetString(npc.Fqn, nameLookupData);
            npc.LocalizedName = _dom.stringTable.TryGetLocalizedStrings(npc.Fqn, nameLookupData);

            if (textLookup.ContainsKey(TitleLookupKey))
            {
                var titleLookupData = (GomObjectData)textLookup[TitleLookupKey];
                npc.Title = _dom.stringTable.TryGetString(npc.Fqn, titleLookupData);
                npc.LocalizedTitle = _dom.stringTable.TryGetLocalizedStrings(npc.Fqn, titleLookupData);
            }

            npc.Id = obj.Id; // (ulong)(nameId >> 32);

            //if (objIdMap.ContainsKey(npc.Id))
            //{
            //    Npc otherNpc = objIdMap[npc.Id];
            //    if (!String.Equals(otherNpc.Fqn, npc.Fqn))
            //    {
            //        throw new InvalidOperationException(String.Format("Duplicate NPC Ids: {0} and {1}", otherNpc.Fqn, npc.Fqn));
            //    }
            //}
            //else
            //{
            //    objIdMap[npc.Id] = npc;
            //}

            npc.MinLevel = (int)obj.Data.ValueOrDefault<long>("npcMinLevel", (long)baseNpc.MinLevel);
            npc.MaxLevel = (int)obj.Data.ValueOrDefault<long>("npcMaxLevel", (long)baseNpc.MaxLevel);

            // Load Toughness
            var toughnessEnum = (ScriptEnum)obj.Data.ValueOrDefault<ScriptEnum>("npcToughness", null);
            if (toughnessEnum == null)
            {
                npc.Toughness = baseNpc.Toughness;
            }
            else
            {
                npc.Toughness = ToughnessExtensions.ToToughness(toughnessEnum);
            }

            npc.LocalizedToughness = ToughnessLocalizationMap[npc.Toughness];

            // Load Packages

            npc.movementPackage = obj.Data.ValueOrDefault<string>("npcMovementPackage", "");
            npc.coverPackage = obj.Data.ValueOrDefault<string>("npcCoverPackage", "");
            npc.WanderPackage = obj.Data.ValueOrDefault<string>("npcWanderPackage", "");
            npc.AggroPackage = obj.Data.ValueOrDefault<string>("npcAggroPackage", "");

            // Load Visual Data

            var npcVisualDataList = obj.Data.ValueOrDefault<List<object>>("npcVisualDataList", null);
            if (npcVisualDataList != null)
            {
                npc.VisualDataList = new List<NpcVisualData>();
                
                foreach (var visualDataObject in npcVisualDataList)
                {
                    var visualData = visualDataObject as GomObjectData;

                    NpcVisualData nvd = new NpcVisualData(_dom);

                    nvd.CharSpec = visualData.ValueOrDefault<string>("npcTemplateVisualDataCharSpec", "");
                    nvd.ScaleAdjustment = visualData.ValueOrDefault<float>("npcTemplateVisualDataScaleAdjustment", 0);
                    nvd.MeleeWepId = visualData.ValueOrDefault<ulong>("npcTemplateVisualDataMeleeWeapon", 0);
                    //if (nvd.MeleeWepId != 0) nvd.MeleeWep = _dom.itemLoader.Load(nvd.MeleeWepId);
                    nvd.MeleeOffWepId = visualData.ValueOrDefault<ulong>("npcTemplateVisualDataMeleeOffWeapon", 0);
                    //if (nvd.MeleeOffWepId != 0) nvd.MeleeOffWep = _dom.itemLoader.Load(nvd.MeleeOffWepId);
                    nvd.RangedWepId = visualData.ValueOrDefault<ulong>("npcTemplateVisualDataRangedWeapon", 0);
                    //if (nvd.RangedWepId != 0) nvd.RangedWep = _dom.itemLoader.Load(nvd.RangedWepId);
                    nvd.RangedOffWepId = visualData.ValueOrDefault<ulong>("npcTemplateVisualDataRangedOffWeapon", 0);
                    //if (nvd.RangedOffWepId != 0) nvd.RangedOffWep = _dom.itemLoader.Load(nvd.RangedOffWepId);
                    nvd.AppearanceId = visualData.ValueOrDefault<ulong>("npcTemplateVisualDataAppearance", 0);
                    var appearance = _dom.GetObject(nvd.AppearanceId);
                    if (appearance != null)
                    {
                        nvd.AppearanceFqn = appearance.Name;
                        appearance.Unload();
                    }
                    else nvd.AppearanceFqn = "";
                    //nvd.Appearance = (NpcAppearance)_dom.appearanceLoader.Load(appearance);
                    nvd.SpeciesScale = visualData.ValueOrDefault<long>("npcTemplateVisualDataSpecieScale", 0);

                    npc.VisualDataList.Add(nvd);
                }
            }

            // Load Faction
            long factionId = obj.Data.ValueOrDefault<long>("npcFaction", 0);
            if (factionId == 0)
            {
                var detFaction = baseNpc.DetFaction;
                npc.Faction = baseNpc.Faction;
            }
            else
            {
                npc.DetFaction = _dom.factionData.ToFaction(factionId);
                npc.Faction = FactionExtensions.ToFaction(factionId);
            }

            npc.DifficultyFlags = (int)obj.Data.ValueOrDefault<long>("spnDifficultyLevelFlags", baseNpc.DifficultyFlags);

            npc.LootTableId = obj.Data.ValueOrDefault<long>("npclootPackage", baseNpc.LootTableId);

            npc.ClassId = obj.Data.ValueOrDefault<ulong>("npcClassPackage", 0);
            if (npc.ClassId == 0UL)
            {
                npc.ClassSpec = baseNpc.ClassSpec;
                npc.ClassId = baseNpc.ClassId;
            }
            else
            {
                npc.ClassSpec = _dom.classSpecLoader.Load(npc.ClassId);
            }
            if(npc.ClassSpec == null)
            {
                string sionsodn = "";
            }

            npc.CodexId = obj.Data.ValueOrDefault<ulong>("npcCodexSpec", 0);
            /*if (cdxNodeId > 0)
            {
                npc.Codex = _dom.codexLoader.Load(cdxNodeId);
            }
            else
            {
                npc.Codex = baseNpc.Codex;
            }*/

            var profTrained = (ScriptEnum)obj.Data.ValueOrDefault<ScriptEnum>("prfTrainerProfession", null);
            if (profTrained == null)
            {
                npc.ProfessionTrained = baseNpc.ProfessionTrained;
            }
            else
            {
                npc.ProfessionTrained = ProfessionExtensions.ToProfession(profTrained);
            }

            List<object> trainedPackages = obj.Data.ValueOrDefault<List<object>>("npcTrainerPackages", null);
            if (trainedPackages == null)
            {
                npc.IsClassTrainer = baseNpc.IsClassTrainer;
            }
            else
            {
                npc.IsClassTrainer = trainedPackages.Count > 0;
            }

            npc.cnvConversationName = obj.Data.ValueOrDefault<string>("cnvConversationName", baseNpc.cnvConversationName);

            List<object> vendorPackages = obj.Data.ValueOrDefault<List<object>>("npcVendorPackages", null);
            if (vendorPackages != null)
            {
                foreach (string pkg in vendorPackages) { npc.VendorPackages.Add(pkg.ToLower()); }
            }
            else
            {
                npc.VendorPackages = baseNpc.VendorPackages;
            }

            obj.Unload();
            return npc;
        }

        public void LoadObject(Models.GameObject loadMe, GomObject obj)
        {
            GomLib.Models.Npc npc = (Models.Npc)loadMe;
            Load(npc, obj);
        }

        public void LoadReferences(Models.GameObject obj, GomObject gom)
        {
            var npc = (Npc)obj;

            ulong npcCharacterCompanionOverride = gom.Data.ValueOrDefault<ulong>("npcCharacterCompanionOverride", 0);
            if (npcCharacterCompanionOverride > 0)
            {
                npc.CompanionOverrideId = npcCharacterCompanionOverride;
                //npc.CompanionOverride = Load(npcCharacterCompanionOverride);
            }
            // No references to load
        }
    }
}
