﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GomLib.Models;

namespace GomLib.ModelLoader
{
    public class CompanionLoader
    {
        //StringTable strTable;
        Dictionary<object, object> crewPositionLookup;
        Dictionary<object, object> crewData;
        Dictionary<object, object> factionLookup;
        Dictionary<object, object> npcNodeIdLookup;
        Dictionary<ulong, List<ulong>> CompanionAcquireMap;
        Dictionary<ulong, ulong> NpcToNcoMap;

        private readonly DataObjectModel _dom;

        public CompanionLoader(DataObjectModel dom)
        {
            _dom = dom;
            Flush();
        }

        public void Flush()
        {
            //strTable = null;
            crewPositionLookup = new Dictionary<object, object>();
            crewData = new Dictionary<object, object>();
            factionLookup = new Dictionary<object, object>();
            npcNodeIdLookup = new Dictionary<object, object>();
            CompanionAcquireMap = new Dictionary<ulong, List<ulong>>();
            NpcToNcoMap = new Dictionary<ulong, ulong>();
        }

        public string ClassName
        {
            get { return "chrCompanionInfoRow"; }
        }

        public Companion Load(ulong npcId)
        {
            var gom = _dom.GetObject("chrCompanionInfo_Prototype");

            var chrCompanionInfoData = gom.Data.ValueOrDefault("chrCompanionInfoData", new Dictionary<object, object>());
            if (!chrCompanionInfoData.ContainsKey(npcId))
                return null;
            return Load(new Companion(), npcId, (GomObjectData)chrCompanionInfoData[npcId]);
        }

        public Companion Load(Companion cmp, ulong npcId, GomObjectData obj)
        {
            if (obj == null) { return cmp; }
            if (cmp == null) { return null; }

            if (crewPositionLookup.Count == 0)
            {
                var crewProto = _dom.GetObject("scffCrewPrototype");
                if (crewProto != null)
                {
                    crewPositionLookup = crewProto.Data.ValueOrDefault("scFFCrewPositionData", crewPositionLookup);
                    crewData = crewProto.Data.Get<Dictionary<object, object>>("scFFShipsCrewAndPatternData");
                    factionLookup = crewProto.Data.Get<Dictionary<object, object>>("scFFCrewFactionData");
                    npcNodeIdLookup = crewProto.Data.Get<Dictionary<object, object>>("scFFCrewNpcNodeData");
                }
                crewProto = null;

                var chrCompanionInfo_Prototype = _dom.GetObject("chrCompanionInfo_Prototype");
                var chrCompanionPermittedLookup = chrCompanionInfo_Prototype.Data.ValueOrDefault("chrCompanionPermittedLookup", new Dictionary<object, object>());

                foreach (var kvp in chrCompanionPermittedLookup)
                {
                    foreach (var cmpid in (List<object>)kvp.Value)
                    {
                        if (!CompanionAcquireMap.ContainsKey((ulong)cmpid))
                        {
                            CompanionAcquireMap.Add((ulong)cmpid, new List<ulong>());
                        }
                        CompanionAcquireMap[(ulong)cmpid].Add((ulong)kvp.Key);
                    }
                }

                var contactsPrototype = _dom.GetObject("contactsPrototype");
                if (contactsPrototype != null)
                {
                    var contactsNpcToNcoMap = contactsPrototype.Data.ValueOrDefault("contactsNpcToNcoMap", new Dictionary<object, object>());
                    NpcToNcoMap = contactsNpcToNcoMap.ToDictionary(x => (ulong)x.Key, x => (ulong)x.Value);
                }
            }

            cmp.Dom_ = _dom;
            cmp.Prototype = "chrCompanionInfo_Prototype";
            cmp.ProtoDataTable = "chrCompanionInfoData";

            IDictionary<string, object> objAsDict = obj.Dictionary;
            cmp.Npc = _dom.npcLoader.Load(npcId);
            cmp.Name = cmp.Npc.Name;
            cmp.LocalizedName = cmp.Npc.LocalizedName;
            cmp.UId = cmp.Npc.Id;

            CompanionAcquireMap.TryGetValue(npcId, out List<ulong> classes);

            if (obj.Dictionary.TryGetValue("chrCompanionInfo_portrait", out object portrait))
            {
                cmp.Portrait = ParsePortrait((string)portrait);
            }
            cmp.ConversationMultiplier = (float)obj.Dictionary["chrCompanionInfo_affectionMultiplier"];
            cmp.IsGenderMale = obj.Dictionary.ContainsKey("chrCompanionInfo_gender_male");
            cmp.AppearanceClassId = obj.ValueOrDefault<ulong>("chrCompanionInfo_appearance_class", 0);
            cmp.Classes = new ClassSpecList();


            var companionTable = _dom.GetObject("chrCompanionTable_Prototype").Data.ValueOrDefault<Dictionary<object, object>>("chrCompanionClassesData", null);
            if (companionTable != null)
            {
                if (companionTable.TryGetValue(npcId, out object listObj))
                {
                    foreach (object clas in (List<object>)listObj)
                    {
                        ClassSpec c = _dom.classSpecLoader.Load((ulong)clas);
                        cmp.Classes.Add(c);
                    }
                }
            }

            Dictionary<object, object> profMods = (Dictionary<object, object>)obj.Dictionary["chrCompanionInfo_profession_modifiers"];
            cmp.ProfessionModifiers = new List<CompanionProfessionModifier>();
            foreach (var profKvp in profMods)
            {
                CompanionProfessionModifier mod = new CompanionProfessionModifier
                {
                    Companion = cmp,
                    Stat = _dom.statData.ToStat((ScriptEnum)profKvp.Key).ToString(),
                    Modifier = (int)(long)profKvp.Value
                };
                cmp.ProfessionModifiers.Add(mod);
            }

            Dictionary<object, object> giftInterestMap = (Dictionary<object, object>)obj.Dictionary["chrCompanionInfo_gift_interest_unromanced_map"];
            cmp.GiftInterest = new List<CompanionGiftInterest>();
            foreach (var giftKvp in giftInterestMap)
            {
                CompanionGiftInterest cgi = new CompanionGiftInterest
                {
                    Companion = cmp,
                    GiftType = GiftTypeExtensions.ToGiftType((ScriptEnum)giftKvp.Key),
                    Reaction = GiftInterestExtensions.ToGiftInterest((ScriptEnum)giftKvp.Value)
                };
                cmp.GiftInterest.Add(cgi);
            }

            giftInterestMap = (Dictionary<object, object>)obj.Dictionary["chrCompanionInfo_gift_interest_romanced_map"];
            foreach (var giftKvp in giftInterestMap)
            {
                GiftType gftType = GiftTypeExtensions.ToGiftType((ScriptEnum)giftKvp.Key);
                var cgi = cmp.GiftInterest.First(x => x.GiftType == gftType);
                cgi.RomancedReaction = GiftInterestExtensions.ToGiftInterest((ScriptEnum)giftKvp.Value);
                cmp.IsRomanceable = true;
            }
            if (!cmp.IsRomanceable)
            {
                // Force Malavai Quinn and Lt. Pierce to be listed as romanceable
                if (cmp.Name.Contains("Quinn") || (cmp.Name.Contains("Pierce"))) { cmp.IsRomanceable = true; }
            }

            cmp.AffectionRanks = new List<CompanionAffectionRank>();
            List<object> affectionRanks = (List<object>)obj.Dictionary["chrCompanionInfo_threshold_list"];
            int rank = 0;
            foreach (long aff in affectionRanks)
            {
                CompanionAffectionRank car = new CompanionAffectionRank
                {
                    Companion = cmp,
                    Rank = rank,
                    Affection = (int)aff
                };
                cmp.AffectionRanks.Add(car);
                rank++;
            }

            object crewLookupId = new object();
            cmp.CrewAbilities = new List<Talent>();
            cmp.SpaceAbility = new Ability();

            if (npcNodeIdLookup.Count != 0)
            {
                npcNodeIdLookup.TryGetValue(cmp.Npc.NodeId, out crewLookupId);

                if (crewLookupId != null)
                {
                    GomObjectData cmpSpaceData = (GomObjectData)(crewData[(long)crewLookupId]);
                    cmp.SpaceAbility = _dom.abilityLoader.Load((ulong)(cmpSpaceData.Get<List<object>>("scFFCrewAbility"))[0]);
                    cmp.SpaceAbilityId = cmp.SpaceAbility.NodeId;
                    cmp.CrewAbilities.Add(_dom.talentLoader.Load((ulong)(cmpSpaceData.Get<List<object>>("scFFCrewTalentList"))[0]));
                    cmp.CrewAbilities.Add(_dom.talentLoader.Load((ulong)(cmpSpaceData.Get<List<object>>("scFFCrewTalentList"))[1]));
                    long textRetrieverId = cmpSpaceData.ValueOrDefault<long>("scFFCrewDescription", 0);
                    cmp.Description = _dom.stringTable.TryGetString("str.spvp.crew", textRetrieverId);
                    if (cmp.Description == "")
                    {
                        cmp.Description = _dom.stringTable.TryGetString("str.spvp.misc", textRetrieverId);
                        if (cmp.Description == "")
                        {
                            cmp.Description = _dom.stringTable.TryGetString("str.cdx", textRetrieverId);
                            cmp.LocalizedDescription = _dom.stringTable.TryGetLocalizedStrings("str.cdx", textRetrieverId);
                        }
                        else
                        {
                            cmp.LocalizedDescription = _dom.stringTable.TryGetLocalizedStrings("str.spvp.misc", textRetrieverId);
                        }
                    }
                    else
                    {
                        cmp.LocalizedDescription = _dom.stringTable.TryGetLocalizedStrings("str.spvp.crew", textRetrieverId);
                    }

                    if (((List<object>)factionLookup[1086966210362573345]).Contains(crewLookupId))
                    {
                        cmp.Faction = "Republic";
                        cmp.FactionId = 1086966210362573345;
                    }
                    else if (((List<object>)factionLookup[-1855280666668608219]).Contains(crewLookupId))
                    {
                        cmp.Faction = "Imperial";
                        cmp.FactionId = -1855280666668608219;
                    }
                    else
                    {
                        cmp.Faction = "Unknown"; //this should never happen
                    }
                    Dictionary<object, object> factionPositions = (Dictionary<object, object>)crewPositionLookup[cmp.FactionId];
                    cmp.CrewPositions = new List<string>();
                    foreach (var position in factionPositions)
                    {
                        string positionName = position.Key.ToString().Replace("scffCrewPosition_", "");
                        if (((List<object>)position.Value).Contains(crewLookupId))
                        {
                            cmp.CrewPositions.Add(positionName);
                        }
                    }
                    cmp.SpaceIcon = cmpSpaceData.ValueOrDefault("scFFCrewIcon", ""); //ex "spvp_Crew_icon_risha"
                    _dom._assets.icons.Add(cmp.SpaceIcon);
                    /* string tokens = "";
                    if (cmp.SpaceAbility.TalentTokens != null) tokens = cmp.SpaceAbility.TalentTokens.Substring(cmp.SpaceAbility.TalentTokens.IndexOf(',') + 1);
                    cmp.SpaceAbility.Description = cmp.SpaceAbility.Description.Replace("<<1>>", (tokens.Replace("\'", "")));*/ //should be unnecessary now
                }
            }

            CompanionAcquireMap.TryGetValue(npcId, out List<ulong> outList);
            cmp.AllowedClasses = outList;

            NpcToNcoMap.TryGetValue(npcId, out ulong ncoId);
            cmp.NcoId = ncoId;

            cmp.TankApcId = obj.ValueOrDefault<ulong>("chrCompanionInfo_tank_apc", 0);
            cmp.DpsApcId = obj.ValueOrDefault<ulong>("chrCompanionInfo_dps_apc", 0);
            cmp.HealApcId = obj.ValueOrDefault<ulong>("chrCompanionInfo_heal_apc", 0);

            return cmp;
        }

        private string ParsePortrait(string portrait)
        {
            // Remove img:/
            portrait = portrait.Substring(5).ToLower();

            _dom._assets.icons.AddPortrait(portrait);

            // Parse filename
            //var filename = System.IO.Path.GetFileNameWithoutExtension(portrait);
            return portrait.Replace("/gfx/portraits/", "");
        }

        public void LoadReferences(GameObject obj, GomObject gom)
        {
            if (obj is null)
            {
                throw new ArgumentNullException(nameof(obj));
            }

            if (gom is null)
            {
                throw new ArgumentNullException(nameof(gom));
            }
            // No references to load
        }

        public static string ClassFromId(ulong id)
        {
            string comp;

            switch (id)
            {
                case 0: comp = "All/Temporary?"; break;
                case 16140902893827567561: comp = "Sith Warrior"; break;
                case 16140912704077491401: comp = "Smuggler"; break;
                case 16140943676484767978: comp = "Imperial Agent"; break;
                case 16140973599688231714: comp = "Trooper"; break;
                case 16141010271067846579: comp = "Sith Inquisitor"; break;
                case 16141119516274073244: comp = "Jedi Knight"; break;
                case 16141170711935532310: comp = "Bounty Hunter"; break;
                case 16141179471541245792: comp = "Jedi Consular"; break;
                default: comp = "Unknown"; break;
            }

            return comp;
        }
    }
    public class NewCompanionLoader : IModelLoader
    {
        const long NameLookupKey = -2761358831308646330;
        const long TitleLookupKey = 4860477835249092778;
        const long DescLookupKey = 1078249248256508798;

        Dictionary<ulong, NewCompanion> idMap;
        Dictionary<string, NewCompanion> nameMap;
        private Dictionary<string, NewCompanion> unknownMap;

        private readonly DataObjectModel _dom;
        public NewCompanionLoader(DataObjectModel dom)
        {
            _dom = dom;
            if (nameMap == null)
            {
                Flush();
            }
        }

        public void Flush()
        {
            idMap = new Dictionary<ulong, NewCompanion>();
            nameMap = new Dictionary<string, NewCompanion>();
            UnknownMap = new Dictionary<string, NewCompanion>();
        }

        public string ClassName
        {
            get { return "ncoNewCompanion"; }
        }

        public long SubCategoryId { get; private set; }
        public Dictionary<string, NewCompanion> UnknownMap { get => unknownMap; set => unknownMap = value; }

        public NewCompanion Load(ulong nodeId)
        {
            if (idMap.TryGetValue(nodeId, out NewCompanion result))
            {
                return result;
            }

            GomObject obj = _dom.GetObject(nodeId);
            NewCompanion ach = new NewCompanion();
            return Load(ach, obj);
        }


        public NewCompanion Load(string fqn)
        {
            if (nameMap.TryGetValue(fqn, out NewCompanion result))
            {
                return result;
            }

            GomObject obj = _dom.GetObject(fqn);
            NewCompanion ach = new NewCompanion();
            return Load(ach, obj);
        }

        public NewCompanion Load(GomObject obj)
        {
            NewCompanion ach = new NewCompanion();
            return Load(ach, obj);
        }

        public GameObject CreateObject()
        {
            return new NewCompanion();
        }

        public NewCompanion Load(GameObject obj, GomObject gom)
        {
            if (gom == null) { return (NewCompanion)obj; }
            if (obj == null) { return null; }

            var nco = obj as NewCompanion;

            nco.Fqn = gom.Name;
            nco.Id = gom.Id;
            nco.Dom_ = _dom;
            nco.References = gom.References;

            // New Companion Info

            StringTable conCats = _dom.stringTable.Find("str.sys.contactcategories");

            var ncoAcquireConditionalList = gom.Data.ValueOrDefault("ncoAcquireConditionalList", new List<object>()); //loop through and load these
            nco.AcquireConditionalIds = ncoAcquireConditionalList.Select(x => (x as GomObjectData).ValueOrDefault<ulong>("contactsQstCondCndId", 0)).ToList();
            var ncoAllianceAlertList = gom.Data.ValueOrDefault("ncoAllianceAlertList", new List<object>()); //loop through and load these
            nco.AllianceAlerts = new List<AllianceAlert>();
            foreach (var alert in ncoAllianceAlertList)
            {
                var goma = alert as GomObjectData;
                AllianceAlert ally = new AllianceAlert
                {
                    ConditionId = goma.ValueOrDefault<ulong>("contactsQstCondCndId", 0),
                    MissionId = goma.ValueOrDefault<ulong>("contactsQstCondQstId", 0),
                    NcoId = goma.ValueOrDefault<ulong>("contactsQstCondNcoId", 0)
                };
                nco.AllianceAlerts.Add(ally);
            }

            nco.InfluenceCap = gom.Data.ValueOrDefault<long>("ncoInfluenceCap", 0);
            nco.CategoryId = gom.Data.ValueOrDefault<long>("ncoCategory", 0);

            nco.Category = "";
            if (nco.CategoryId != 0)
            {
                nco.Category = conCats.GetText(nco.CategoryId, "str.sys.contactcategories");
                nco.LocalizedCategory = conCats.GetLocalizedText(nco.CategoryId, "str.sys.contactcategories");
            }

            nco.SubCategoryId = gom.Data.ValueOrDefault<long>("ncoSubCategory", 0);
            nco.SubCategory = "";
            if (nco.SubCategoryId != 0)
            {
                nco.SubCategory = conCats.GetText(nco.SubCategoryId, "str.sys.contactcategories");
                nco.LocalizedSubCategory = conCats.GetLocalizedText(nco.SubCategoryId, "str.sys.contactcategories");
            }

            nco.MaxInfluenceTier = gom.Data.ValueOrDefault<long>("ncoMaxInfluenceTier", 0);

            nco.NpcId = gom.Data.ValueOrDefault<ulong>("ncoNpcId", 0);

            nco.Icon = gom.Data.ValueOrDefault("ncoPreviewIcon", "");
            if (string.IsNullOrEmpty(nco.Icon) && nco.Companion != null)
                nco.Icon = (nco.Companion.Portrait ?? "").Replace(".dds", "");

            Dictionary<object, object> ncoInteractionList = gom.Data.ValueOrDefault("ncoInteractionList", new Dictionary<object, object>());

            var ncoTable = _dom.stringTable.Find("str.nco");
            nco.InteractionList = new Dictionary<string, Dictionary<string, string>>();
            foreach (var kvp in ncoInteractionList)
            {
                var key = (ulong)kvp.Key;
                var val = (long)kvp.Value;

                var cndObj = _dom.GetObject(key);


                var locals = ncoTable.GetLocalizedText(val, "str.nco");
                if (cndObj != null)
                    nco.InteractionList.Add(cndObj.Name.Substring(cndObj.Name.LastIndexOf('.') + 1), locals);

            }

            var textLookup = gom.Data.Get<Dictionary<object, object>>("locTextRetrieverMap");

            // Load Name
            var nameLookupData = (GomObjectData)textLookup[NameLookupKey];
            nco.NameId = nameLookupData.Get<long>("strLocalizedTextRetrieverStringID");
            nco.LocalizedName = _dom.stringTable.TryGetLocalizedStrings("str.nco", nameLookupData);
            Normalize.Dictionary(nco.LocalizedName, nco.Fqn);
            //nco.Name = _dom.stringTable.TryGetString("str.nco", nameLookupData);
            nco.Name = nco.LocalizedName["enMale"];

            // Load Title
            var titleLookupData = (GomObjectData)textLookup[TitleLookupKey];
            nco.TitleId = titleLookupData.Get<long>("strLocalizedTextRetrieverStringID");
            nco.LocalizedTitle = _dom.stringTable.TryGetLocalizedStrings("str.nco", titleLookupData);
            nco.Title = _dom.stringTable.TryGetString("str.nco", titleLookupData);

            // Load Description
            var descLookupData = (GomObjectData)textLookup[DescLookupKey];
            nco.DescriptionId = descLookupData.Get<long>("strLocalizedTextRetrieverStringID");
            nco.LocalizedDescription = _dom.stringTable.TryGetLocalizedStrings("str.nco", descLookupData);
            nco.Description = _dom.stringTable.TryGetString("str.nco", descLookupData);

            gom.Unload();
            return nco;
        }

        public void LoadObject(GameObject loadMe, GomObject obj)
        {
            NewCompanion ach = (NewCompanion)loadMe;
            Load(ach, obj);
        }

        public void LoadReferences(GameObject obj, GomObject gom)
        {
            // No references to load
        }
    }
}
