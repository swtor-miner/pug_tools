using System;
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

        private DataObjectModel _dom;

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

        public Models.Companion Load(Models.Companion cmp, ulong npcId, GomObjectData obj)
        {
            if (obj == null) { return cmp; }
            if (cmp == null) { return null; }

            if (crewPositionLookup.Count == 0)
            {
                var crewProto = _dom.GetObject("scffCrewPrototype");
                if (crewProto != null)
                {
                    crewPositionLookup = crewProto.Data.ValueOrDefault<Dictionary<object, object>>("scFFCrewPositionData", crewPositionLookup);
                    crewData = crewProto.Data.Get<Dictionary<object, object>>("scFFShipsCrewAndPatternData");
                    factionLookup = crewProto.Data.Get<Dictionary<object, object>>("scFFCrewFactionData");
                    npcNodeIdLookup = crewProto.Data.Get<Dictionary<object, object>>("scFFCrewNpcNodeData"); 
                }
                crewProto = null;

                var chrCompanionInfo_Prototype = _dom.GetObject("chrCompanionInfo_Prototype");
                var chrCompanionPermittedLookup = chrCompanionInfo_Prototype.Data.ValueOrDefault<Dictionary<object, object>>("chrCompanionPermittedLookup", new Dictionary<object, object>());

                foreach (var kvp in chrCompanionPermittedLookup)
                {
                    foreach (var cmpid in (List<object>)kvp.Value) {
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
                    var contactsNpcToNcoMap = contactsPrototype.Data.ValueOrDefault<Dictionary<object, object>>("contactsNpcToNcoMap", new Dictionary<object, object>());
                    NpcToNcoMap = contactsNpcToNcoMap.ToDictionary(x => (ulong)x.Key, x => (ulong)x.Value);
                }
            }

            cmp._dom = _dom;
            cmp.Prototype = "chrCompanionInfo_Prototype";
            cmp.ProtoDataTable = "chrCompanionInfoData";

            IDictionary<string, object> objAsDict = obj.Dictionary;
            cmp.Npc = _dom.npcLoader.Load(npcId);
            cmp.Name = cmp.Npc.Name;
            cmp.LocalizedName = cmp.Npc.LocalizedName;
            cmp.uId = cmp.Npc.Id;

            List<ulong> classes;
            CompanionAcquireMap.TryGetValue(npcId, out classes);

            object portrait;
            if(obj.Dictionary.TryGetValue("chrCompanionInfo_portrait", out portrait))
            {
                cmp.Portrait = ParsePortrait((string)portrait);
            }
            cmp.ConversationMultiplier = (float)obj.Dictionary["chrCompanionInfo_affectionMultiplier"];
            cmp.IsGenderMale = (bool)obj.Dictionary.ContainsKey("chrCompanionInfo_gender_male");
            cmp.Classes = new ClassSpecList();

            
            var companionTable = _dom.GetObject("chrCompanionTable_Prototype").Data.Get<Dictionary<object, object>>("chrCompanionClassesData");
            object listObj;
            if(companionTable.TryGetValue(npcId, out listObj))
            {
                foreach (object clas in (List<object>)listObj)
                {
                    ClassSpec c = _dom.classSpecLoader.Load((ulong)clas);
                    cmp.Classes.Add(c);
                }
            }

            Dictionary<object, object> profMods = (Dictionary<object,object>)obj.Dictionary["chrCompanionInfo_profession_modifiers"];
            cmp.ProfessionModifiers = new List<CompanionProfessionModifier>();
            foreach (var profKvp in profMods)
            {
                CompanionProfessionModifier mod = new CompanionProfessionModifier();
                mod.Companion = cmp;
                mod.Stat = _dom.statData.ToStat((ScriptEnum)profKvp.Key).ToString();
                mod.Modifier = (int)(long)profKvp.Value;
                cmp.ProfessionModifiers.Add(mod);
            }

            Dictionary<object, object> giftInterestMap = (Dictionary<object,object>)obj.Dictionary["chrCompanionInfo_gift_interest_unromanced_map"];
            cmp.GiftInterest = new List<CompanionGiftInterest>();
            foreach (var giftKvp in giftInterestMap)
            {
                CompanionGiftInterest cgi = new CompanionGiftInterest();
                cgi.Companion = cmp;
                cgi.GiftType = GiftTypeExtensions.ToGiftType((ScriptEnum)giftKvp.Key);
                cgi.Reaction = GiftInterestExtensions.ToGiftInterest((ScriptEnum)giftKvp.Value);
                cmp.GiftInterest.Add(cgi);
            }

            giftInterestMap = (Dictionary<object,object>)obj.Dictionary["chrCompanionInfo_gift_interest_romanced_map"];
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
                CompanionAffectionRank car = new CompanionAffectionRank();
                car.Companion = cmp;
                car.Rank = rank;
                car.Affection = (int)aff;
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
                    cmp.SpaceIcon = cmpSpaceData.ValueOrDefault<string>("scFFCrewIcon", ""); //ex "spvp_Crew_icon_risha"
                    _dom._assets.icons.Add(cmp.SpaceIcon);
                    /* string tokens = "";
                    if (cmp.SpaceAbility.TalentTokens != null) tokens = cmp.SpaceAbility.TalentTokens.Substring(cmp.SpaceAbility.TalentTokens.IndexOf(',') + 1);
                    cmp.SpaceAbility.Description = cmp.SpaceAbility.Description.Replace("<<1>>", (tokens.Replace("\'", "")));*/ //should be unnecessary now
                }
            }

            List<ulong> outList;
            CompanionAcquireMap.TryGetValue(cmp.Npc.Id, out outList);
            cmp.AllowedClasses = outList;

            ulong ncoId;
            NpcToNcoMap.TryGetValue(npcId, out ncoId);
            if(ncoId != 0)
            {
                GomObject ncoObj = _dom.GetObject(ncoId);
                if (ncoObj != null) {
                    StringTable conCats = _dom.stringTable.Find("str.sys.contactcategories");

                    var acquireConditionals = ncoObj.Data.ValueOrDefault("ncoAcquireConditionalList", new List<object>()); //loop through and load these
                    var allianceAlerts = ncoObj.Data.ValueOrDefault("ncoAllianceAlertList", new List<object>()); //loop through and load these
                    long ncoInfluenceCap = ncoObj.Data.ValueOrDefault<long>("ncoInfluenceCap", 0);
                    long ncoCategoryId = ncoObj.Data.ValueOrDefault<long>("ncoCategory", 0);

                    string ncoCategory = "";
                    if (ncoCategoryId != 0)
                        ncoCategory = conCats.GetText(ncoCategoryId, "str.sys.contactcategories");

                    long ncoSubCategoryId = ncoObj.Data.ValueOrDefault<long>("ncoSubCategory", 0);
                    string ncoSubCategory = "";
                    if (ncoSubCategoryId != 0)
                        ncoSubCategory = conCats.GetText(ncoSubCategoryId, "str.sys.contactcategories");

                    long ncoAcquireMinLevel = ncoObj.Data.ValueOrDefault<long>("ncoAcquireMinLevel", 0);

                    ulong ncoNpcId = ncoObj.Data.ValueOrDefault<ulong>("ncoNpcId", 0);

                    string ncoPreviewIcon = ncoObj.Data.ValueOrDefault("ncoPreviewIcon", "");

                    Dictionary<object, object> ncoInteractionList = ncoObj.Data.ValueOrDefault("ncoInteractionList", new Dictionary<object, object>());

                    foreach(var kvp in ncoInteractionList)
                    {
                        var key = (ulong)kvp.Key;
                        var val = (long)kvp.Value;
                        


                    }
                }
            }

            return cmp;
        }

        private string ParsePortrait(string portrait)
        {
            // Remove img:/
            portrait = portrait.Substring(5).ToLower();

            _dom._assets.icons.AddPortrait(portrait);

            // Parse filename
            var filename = System.IO.Path.GetFileNameWithoutExtension(portrait);
            return filename;
        }

        public void LoadReferences(Models.GameObject obj, GomObject gom)
        {
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
}
