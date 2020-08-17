using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Newtonsoft.Json;

namespace GomLib.Models
{
    public enum Faction
    {
        None = 0,
        Empire = 1,
        Republic = 2,
        Neutral = 3,
        EmpireUtilityNpc = 4,
        ScriptedRepOppositionConvNoKill = 5,
        Hostile1 = 6,
        Hostile2 = 7,
        FlashpointHostile3NoDamage = 8,
        ScriptedNeutralOppositionNoKill = 9,
        ScriptedImpSupport = 10,
        Friendly = 11,
        EmpireEscort = 12,
        Anarchy = 13,
        RepublicStaged = 14,
        WildlifePredator = 15,
        ScriptedHostileOppositionNoKill = 16,
        ScriptedRepSupportNoKill = 17,
        EmpireStaged = 18,
        ScriptedHostileOpposition = 19,
        PvpPets = 20,
        RepublicHostage = 21,
        RepublicEscort = 22,
        CombatTestNpc = 23,
        FlashpointHostile1 = 24,
        FlashpointHostile2 = 25,
        FlashpointHostile3 = 26,
        NeutralGuard = 27,
        WildlifePrey = 28,
        ScriptedImpOppositionConvNoKill = 29,
        ScriptedNeutralOpposition = 30,
        ScriptedRepOppositionConv = 31,
        ScriptedImpSupportNoKill = 32,
        RepublicCitizen = 33,
        EmpireHelp = 34,
        FlashpointHostile4NoDamage = 35,
        EmpireHostage = 36,
        RepublicHelp = 37,
        RegressionBots = 38,
        RepublicUtilityNpc = 39,
        EmpireCitizen = 40,
        ScriptedRepSupport = 41,
        ScriptedImpOppositionConv = 42,
        UnknownNegative3364812825255774337 = 43,
        FlashpointHostile6HostileRealDamage = 44,
        FlashpointHostile5NoDamage = 45,
        UnknownNegative9168132803403588183 = 46,
        Unknown5533278335529788882 = 47,
        UnknownNegative755389375346907506 = 48,
        UnknownNegative3250311453106264388 = 49
    }
    //AdvArchaeology = 50
    public static class FactionExtensions
    {
        public static Faction ToFaction(this string str)
        {
            if (String.IsNullOrEmpty(str)) return Faction.None;

            switch (str.ToLower())
            {
                case "republic": return Faction.Republic;
                case "imperial": return Faction.Empire;
                default: throw new InvalidOperationException("Unknown faction: " + str);
            }
        }

        public static Faction ToFaction(this long factionId)
        {
            switch (factionId)
            {
                case -432386136094338236:
                case 0: return Faction.None;
                case 1086966210362573345: return Faction.Republic;
                case -1855280666668608219: return Faction.Empire;
                case -5750514287198089128: return Faction.Neutral;
                case 562573234638329886: return Faction.EmpireUtilityNpc;
                case 1573164028283502856: return Faction.FlashpointHostile5NoDamage;
                case 1814907077791255075: return Faction.ScriptedRepOppositionConvNoKill;
                case 2335203698424887668: return Faction.Hostile1;
                case 2335206996959772301: return Faction.Hostile2;
                case 2879502666945966340: return Faction.FlashpointHostile3NoDamage;
                case 3251225815323856526: return Faction.ScriptedNeutralOppositionNoKill;
                case 3637181978308091762: return Faction.ScriptedImpSupport;
                case 3780939478746129694: return Faction.Friendly;
                case 3842112251581187830: return Faction.EmpireEscort;
                case 4815981259741724869: return Faction.Anarchy;
                case 5541056412744728752: return Faction.RepublicStaged;
                case 6005713525767326303: return Faction.WildlifePredator;
                case 6637183276383572243: return Faction.ScriptedHostileOppositionNoKill;
                case 7474727620554664127: return Faction.ScriptedRepSupportNoKill;
                case 8664490702236993268: return Faction.EmpireStaged;
                case 8758438452262606755: return Faction.ScriptedHostileOpposition;
                case -8567988720127768316: return Faction.PvpPets;
                case -8376179368116850121: return Faction.RepublicHostage;
                case -8183437355124924550: return Faction.RepublicEscort;
                case -7429301532149948034: return Faction.CombatTestNpc;
                case -7110405713848361669: return Faction.FlashpointHostile1;
                case -7110404614336733458: return Faction.FlashpointHostile2;
                case -7110403514825105247: return Faction.FlashpointHostile3;
                case -6120731575019401619: return Faction.NeutralGuard;
                case -4293050603363720816: return Faction.WildlifePrey;
                case -4229476494653560322: return Faction.FlashpointHostile6HostileRealDamage;
                case -3599216580662476716: return Faction.ScriptedImpOppositionConvNoKill;
                case -3023033381226652520: return Faction.ScriptedNeutralOpposition;
                case -2847218559425484173: return Faction.ScriptedRepOppositionConv;
                case -2701948717364661396: return Faction.ScriptedImpSupportNoKill;
                case -2590071116750678966: return Faction.RepublicCitizen;
                case -2341148374483479055: return Faction.EmpireHelp;
                case -1759135445220001501: return Faction.FlashpointHostile4NoDamage;
                case -1710518401651655933: return Faction.EmpireHostage;
                case -1133957216952754979: return Faction.RepublicHelp;
                case -1113141567053934806: return Faction.RegressionBots;
                case -878620586690540766: return Faction.RepublicUtilityNpc;
                case -603835216087812082: return Faction.EmpireCitizen;
                case -567008111076736177: return Faction.ScriptedRepSupport;
                case -161428275004295206: return Faction.ScriptedImpOppositionConv;
                case -3364812825255774337: return Faction.UnknownNegative3364812825255774337;
                case -9168132803403588183: return Faction.UnknownNegative9168132803403588183;
                case 5533278335529788882: return Faction.Unknown5533278335529788882;
                case -755389375346907506: return Faction.UnknownNegative755389375346907506;
                case -3250311453106264388: return Faction.UnknownNegative3250311453106264388;
                // case -5808608550200435537: return Faction.
                default:
                    return Faction.Anarchy;
                    throw new InvalidOperationException("Unknown Faction: " + factionId);
            }
        }
    }

    public class FactionData
    {
        public Dictionary<long, DetailedFaction> FactionLookup { get; set; }

        [Newtonsoft.Json.JsonIgnore]
        readonly DataObjectModel _dom;

        public FactionData(DataObjectModel dom)
        {
            _dom = dom;
        }

        public DetailedFaction ToFaction(long id)
        {
            if (id == 0) { return new DetailedFaction(); }

            if (FactionLookup == null)
            {
                FactionLookup = new Dictionary<long, DetailedFaction>();
                var chrFactionPackages = _dom.GetObject("chrFactionPackagesPrototype");
                Dictionary<object, object> chrFactionSpecToPackageMap = chrFactionPackages.Data.Get<Dictionary<object, object>>("chrFactionSpecToPackageMap");
                chrFactionPackages.Unload();
                StringTable table = _dom.stringTable.Find("str.sys.factions");
                foreach (var faction in chrFactionSpecToPackageMap)
                {
                    DetailedFaction detFact = new DetailedFaction();
                    GomObjectData gomy = faction.Value as GomObjectData;
                    detFact.Id = gomy.ValueOrDefault<long>("cbtFaction", 0);
                    detFact.RepublicReaction = gomy.ValueOrDefault<ScriptEnum>("chrFactionPackageRepublicPCsAttitude", new ScriptEnum()).ToString();
                    detFact.ImperialReaction = gomy.ValueOrDefault<ScriptEnum>("chrFactionPackageEmpirePCsAttitude", new ScriptEnum()).ToString();
                    detFact.NameId = gomy.ValueOrDefault<long>("chrFactionPackageDisplayName", 0) + 1173582633762816;
                    detFact.LocalizedName = table.GetLocalizedText(detFact.NameId, "str.sys.factions");
                    detFact.DefendedFactionIds = gomy.ValueOrDefault<Dictionary<object, object>>("chrFactionPackageDefendedFactions", new Dictionary<object, object>()).Select(x => (long)x.Key).ToList();
                    detFact.FactionString = gomy.ValueOrDefault<string>("cbtFactionString", "");
                    detFact.OpposingFactionIds = gomy.ValueOrDefault<Dictionary<object, object>>("chrFactionPackageOpposingFactions", new Dictionary<object, object>()).Select(x => (long)x.Key).ToList();
                    FactionLookup.Add(detFact.Id, detFact);
                }
                chrFactionSpecToPackageMap = null;
            }

            if (!FactionLookup.ContainsKey(id)) { return null; }
            else { return FactionLookup[id]; }
        }
    }

    public class DetailedFaction : PseudoGameObject, IEquatable<DetailedFaction>
    {
        public string RepublicReaction { get; set; }
        public string ImperialReaction { get; set; }
        [JsonConverter(typeof(LongConverter))]
        public long NameId { get; set; }
        public Dictionary<string, string> LocalizedName { get; set; }
        public List<long> DefendedFactionIds { get; set; }
        [JsonConverter(typeof(LongConverter))]
        public long FactionId { get { return Id; } }
        public string FactionString { get; set; }
        public List<long> OpposingFactionIds { get; set; }

        public override int GetHashCode()
        {
            int result = NameId.GetHashCode();
            result ^= RepublicReaction.GetHashCode();
            result ^= ImperialReaction.GetHashCode();
            if (Name != null)
                result ^= Name.GetHashCode();
            if (LocalizedName != null) foreach (var x in LocalizedName) { result ^= x.GetHashCode(); }
            if (DefendedFactionIds != null)
            {
                DefendedFactionIds.Sort();
                foreach (var x in DefendedFactionIds) { result ^= x.GetHashCode(); }
            }
            result ^= FactionId.GetHashCode();
            result ^= FactionString.GetHashCode();
            if (OpposingFactionIds != null)
            {
                OpposingFactionIds.Sort();
                foreach (var x in OpposingFactionIds) { result ^= x.GetHashCode(); }
            }

            return result;
        }

        public override bool Equals(object obj)
        {
            if (obj == null) return false;

            if (ReferenceEquals(this, obj)) return true;

            if (!(obj is DetailedFaction itm)) return false;

            return Equals(itm);
        }

        public bool Equals(DetailedFaction itm)
        {
            if (itm == null) return false;

            if (ReferenceEquals(this, itm)) return true;

            if (this.RepublicReaction != itm.RepublicReaction)
                return false;
            if (this.Id != itm.Id)
                return false;

            var dComp = new DictionaryComparer<string, string>();
            if (!dComp.Equals(this.LocalizedName, itm.LocalizedName))
                return false;

            if (this.Name != itm.Name)
                return false;
            if (this.NameId != itm.NameId)
                return false;
            if (this.FactionId != itm.FactionId)
                return false;
            if (this.FactionString != itm.FactionString)
                return false;
            if (this.OpposingFactionIds != itm.OpposingFactionIds)
                return false;
            if (this.DefendedFactionIds != itm.DefendedFactionIds)
                return false;

            return true;
        }
    }
}
