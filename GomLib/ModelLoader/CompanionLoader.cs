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
            }

            cmp._dom = _dom;
            cmp.Prototype = "chrCompanionInfo_Prototype";
            cmp.ProtoDataTable = "chrCompanionInfoData";

            IDictionary<string, object> objAsDict = obj.Dictionary;
            cmp.Npc = _dom.npcLoader.Load(npcId);
            cmp.Name = cmp.Npc.Name;
            cmp.uId = cmp.Npc.Id;
            try
            {
                cmp.Portrait = ParsePortrait((string)obj.Dictionary["chrCompanionInfo_portrait"]);
            }
            catch
            {
                cmp.Portrait = "";
            }
            cmp.ConversationMultiplier = (float)obj.Dictionary["chrCompanionInfo_affectionMultiplier"];
            cmp.IsGenderMale = (bool)obj.Dictionary.ContainsKey("chrCompanionInfo_gender_male");
            cmp.Classes = new ClassSpecList();

            try
            {
                var companionTable = _dom.GetObject("chrCompanionTable_Prototype").Data.Get<Dictionary<object, object>>("chrCompanionClassesData");
                foreach (var clas in (List<object>)companionTable[npcId])
                {
                    ClassSpec c = _dom.classSpecLoader.Load((ulong)clas);
                    cmp.Classes.Add(c);
                }
            }
            catch
            {
                //This companion isn't used by any classes, or this table is missing.
            }
            Dictionary<object, object> profMods = (Dictionary<object,object>)obj.Dictionary["chrCompanionInfo_profession_modifiers"];
            cmp.ProfessionModifiers = new List<CompanionProfessionModifier>();
            foreach (var profKvp in profMods)
            {
                CompanionProfessionModifier mod = new CompanionProfessionModifier();
                mod.Companion = cmp;
                mod.Stat = _dom.statD.ToStat((ScriptEnum)profKvp.Key);
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
    }
}
