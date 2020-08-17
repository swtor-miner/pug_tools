using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;
using Newtonsoft.Json;

namespace GomLib.Models
{
    public class Companion : PseudoGameObject, IEquatable<Companion>
    {
        public List<CompanionAffectionRank> AffectionRanks { get; set; }
        public ClassSpecList Classes { get; set; }
        public float ConversationMultiplier { get; set; }
        [JsonIgnore]
        public List<Talent> CrewAbilities { get; set; }
        public List<string> CrewAbilityB62Ids
        {
            get
            {
                if (CrewAbilities != null)
                    return CrewAbilities.Select(x => x.Id.ToMaskedBase62()).ToList();
                return null;
            }
        }
        public List<string> CrewPositions { get; set; }
        public string Description { get; set; }
        public string Faction { get; set; }
        [JsonConverter(typeof(LongConverter))]
        public long FactionId { get; set; }
        public List<CompanionGiftInterest> GiftInterest { get; set; }
        [JsonConverter(typeof(LongConverter))]
        public override long Id
        {
            get
            {
                return (long)UId;
            }
        }
        public bool IsGenderMale { get; set; }
        [JsonIgnore]
        public ulong AppearanceClassId { get; set; }
        public string AppearanceClassB62Id
        {
            get
            {
                if (AppearanceClassId != 0)
                    return AppearanceClassId.ToMaskedBase62();
                return null;
            }
        }
        public bool IsRomanceable { get; set; }

        public Dictionary<string, string> LocalizedDescription { get; set; }
        //public string Name { get; set; }
        public Dictionary<string, string> LocalizedName { get; set; }
        [JsonIgnore]
        public Npc Npc { get; set; }
        public string Portrait { get; set; }
        public List<CompanionProfessionModifier> ProfessionModifiers { get; set; }
        [Newtonsoft.Json.JsonIgnore]
        public Ability SpaceAbility { get; set; }
        [JsonConverter(typeof(ULongConverter))]
        public ulong SpaceAbilityId { get; set; }
        public string SpaceAbilityB62Id { get { return SpaceAbilityId.ToMaskedBase62(); } }
        public string SpaceIcon { get; set; }
        [JsonConverter(typeof(ULongConverter))]
        public ulong UId { get; set; }
        public string UB62Id { get { return UId.ToMaskedBase62(); } }
        public List<ulong> AllowedClasses { get; set; }
        [JsonConverter(typeof(ULongConverter))]
        public ulong NcoId { get; set; }

        [JsonConverter(typeof(ULongConverter))]
        public ulong TankApcId { get; set; }
        public string TankApcB62Id { get { if (TankApcId != 0) return TankApcId.ToMaskedBase62(); return ""; } }
        [JsonConverter(typeof(ULongConverter))]
        public ulong DpsApcId { get; set; }
        public string DpsApcB62Id { get { if (DpsApcId != 0) return DpsApcId.ToMaskedBase62(); return ""; } }
        [JsonConverter(typeof(ULongConverter))]
        public ulong HealApcId { get; set; }
        public string HealApcB62Id { get { if (HealApcId != 0) return HealApcId.ToMaskedBase62(); return ""; } }

        public override int GetHashCode()
        {
            int hash = Id.GetHashCode();
            if (Classes != null) hash ^= Classes.GetHashCode();
            hash ^= ConversationMultiplier.GetHashCode();
            if (Faction != null) hash ^= Faction.GetHashCode();
            hash ^= FactionId.GetHashCode();
            hash ^= IsGenderMale.GetHashCode();
            hash ^= IsRomanceable.GetHashCode();
            hash ^= Npc.GetHashCode();
            if (Portrait != null) hash ^= Portrait.GetHashCode();
            hash ^= SpaceAbilityId.GetHashCode();
            if (SpaceIcon != null) hash ^= SpaceIcon.GetHashCode();
            hash ^= UId.GetHashCode();
            if (AffectionRanks != null) foreach (var x in AffectionRanks) { hash ^= x.GetHashCode(); }
            if (CrewAbilities != null) foreach (var x in CrewAbilities) { hash ^= x.GetHashCode(); }
            if (CrewPositions != null) foreach (var x in CrewPositions) { hash ^= x.GetHashCode(); }
            if (GiftInterest != null) foreach (var x in GiftInterest) { hash ^= x.GetHashCode(); }
            if (LocalizedDescription != null) foreach (var x in LocalizedDescription) { hash ^= x.GetHashCode(); }
            if (LocalizedName != null) foreach (var x in LocalizedName) { hash ^= x.GetHashCode(); }
            if (ProfessionModifiers != null) foreach (var x in ProfessionModifiers) { hash ^= x.GetHashCode(); }
            return hash;
        }

        public override bool Equals(object obj)
        {
            if (obj == null) return false;

            if (ReferenceEquals(this, obj)) return true;

            if (!(obj is Companion cmp)) return false;

            return Equals(cmp);
        }

        public bool Equals(Companion cmp)
        {
            if (cmp == null) return false;

            if (ReferenceEquals(this, cmp)) return true;

            if (this.AffectionRanks != null)
            {
                if (cmp.AffectionRanks != null)
                {
                    if (!this.AffectionRanks.SequenceEqual(cmp.AffectionRanks))
                        return false;
                }
            }
            if (!this.Classes.Equals(cmp.Classes, false))
                return false;
            if (this.ConversationMultiplier != cmp.ConversationMultiplier)
                return false;
            if (this.CrewAbilities != null)
            {
                if (cmp.CrewAbilities != null)
                {
                    if (!this.CrewAbilities.SequenceEqual(cmp.CrewAbilities))
                        return false;
                }
            }
            if (this.CrewPositions != null)
            {
                if (cmp.CrewPositions != null)
                {
                    if (!this.CrewPositions.SequenceEqual(cmp.CrewPositions))
                        return false;
                }
            }
            if (this.Description != cmp.Description)
                return false;
            if (this.Faction != cmp.Faction)
                return false;
            if (this.FactionId != cmp.FactionId)
                return false;
            if (this.GiftInterest != null)
            {
                if (cmp.GiftInterest != null)
                {
                    if (!this.GiftInterest.SequenceEqual(cmp.GiftInterest))
                        return false;
                }
            }
            if (this.Id != cmp.Id)
                return false;
            if (this.IsGenderMale != cmp.IsGenderMale)
                return false;
            if (this.IsRomanceable != cmp.IsRomanceable)
                return false;

            var ssComp = new DictionaryComparer<string, string>();
            if (!ssComp.Equals(this.LocalizedDescription, cmp.LocalizedDescription))
                return false;

            if (this.Name != cmp.Name)
                return false;
            if (!this.Npc.Equals(cmp.Npc))
                return false;
            if (this.Portrait != cmp.Portrait)
                return false;
            if (this.ProfessionModifiers != null)
            {
                if (cmp.ProfessionModifiers != null)
                {
                    if (!this.ProfessionModifiers.SequenceEqual(cmp.ProfessionModifiers))
                        return false;
                }
            }
            if (!this.SpaceAbility.Equals(cmp.SpaceAbility))
                return false;
            if (this.SpaceAbilityId != cmp.SpaceAbilityId)
                return false;
            if (this.SpaceIcon != cmp.SpaceIcon)
                return false;
            if (this.UId != cmp.UId)
                return false;
            if (NcoId != cmp.NcoId)
                return false;
            if (this.AllowedClasses != cmp.AllowedClasses)
                return false;
            return true;
        }

        public override XElement ToXElement(bool verbose)
        {
            XElement companion = new XElement("Companion");
            if (this.Id != 0)
            {
                companion.Add(new XAttribute("Id", UId),
                    new XElement("Name", Name),
                    new XElement("Description", Description),
                    new XElement("Faction", Faction));
                if (AllowedClasses != null)
                    companion.Add(new XElement("AvailableFor", String.Join(", ", AllowedClasses.Select(x => GomLib.ModelLoader.CompanionLoader.ClassFromId(x)).ToList())));
                int p = 1;
                foreach (var prof in ProfessionModifiers)
                {
                    companion.Add(new XElement("ProfessionModifier", new XAttribute("Id", p), new XElement("Name", prof.Stat), new XElement("Modifier", prof.Modifier)));
                    p++;
                }
                if (verbose)
                {
                    companion.Add(new XElement("Fqn", Npc.Fqn,
                        new XAttribute("Id", Npc.NodeId)),
                    new XElement("Potrait", Portrait));

                    companion.Add(new XElement("ConversationMultiplier", ConversationMultiplier),
                        new XElement("IsRomanceable", IsRomanceable),
                        new XElement("IsGenderMale", IsGenderMale));

                    var giftDic = new Dictionary<string, string>();
                    foreach (var gift in GiftInterest)
                    {
                        if (giftDic.ContainsKey(gift.Reaction.ToString()))
                        {
                            string oldReactionList = giftDic[gift.Reaction.ToString()];
                            giftDic[gift.Reaction.ToString()] = oldReactionList + ", " + gift.GiftType.ToString();
                        }
                        else
                        {
                            giftDic.Add(gift.Reaction.ToString(), gift.GiftType.ToString());
                        }

                        if (giftDic.ContainsKey(gift.RomancedReaction.ToString()))
                        {
                            string oldReactionList = giftDic[gift.RomancedReaction.ToString()];
                            giftDic[gift.RomancedReaction.ToString()] = oldReactionList + ", " + gift.GiftType.ToString();
                        }
                        else
                        {
                            giftDic.Add(gift.RomancedReaction.ToString(), gift.GiftType.ToString());
                        }
                    }
                    int g = 1;
                    foreach (var reaction in giftDic)
                    {
                        companion.Add(new XElement("GiftReactions", new XAttribute("Id", reaction.Key), reaction.Value));
                        g++;
                    }

                    string affRanks = "";
                    if (AffectionRanks.Count > 0)
                    {
                        foreach (var affRank in AffectionRanks)
                        {
                            affRanks += affRank.Affection + ", ";
                        }
                        affRanks.Remove(affRanks.Length - 2);
                    }

                    XElement affectionRanks = new XElement("ConversationAffectionRanks", affRanks);
                    companion.Add(affectionRanks);

                    companion.Add(new XElement("SpaceIcon", SpaceIcon));
                    if (CrewPositions != null)
                    {
                        companion.Add(new XElement("CrewPositions", String.Join(", ", (List<string>)CrewPositions)));
                    }
                    else
                    {
                        companion.Add(new XElement("CrewPositions"));
                    }

                    string reqclasses = null;
                    if (Classes != null)
                    {
                        foreach (var reqclass in Classes)
                        {
                            reqclasses += reqclass.Name.ToString() + ", ";
                        }
                    }
                    if (reqclasses != null) { reqclasses = reqclasses.Substring(0, reqclasses.Length - 2); }
                    companion.Add(new XElement("Classes", reqclasses));
                    companion.Add(new XElement("SpaceAbility", new XAttribute("Id", 0), SpaceAbility.ToXElement(verbose)));
                    int s = 1;
                    foreach (var crwAbl in CrewAbilities)
                    {
                        companion.Add(new XElement("SpacePassive", new XAttribute("Id", s), crwAbl.ToXElement(verbose)));
                        s++;
                    }

                    companion.Add(Npc.ToXElement(verbose));
                }
            }
            return companion;
        }
    }

    public class NewCompanion : GameObject
    {
        public List<ulong> AcquireConditionalIds { get; set; }
        public List<string> AcquireConditionalB62Ids
        {
            get
            {
                return AcquireConditionalIds.Select(x => x.ToMaskedBase62()).ToList();
            }
        }
        public long MaxInfluenceTier { get; set; }
        public List<AllianceAlert> AllianceAlerts { get; set; }
        [JsonIgnore]
        public string Category { get; set; }
        [JsonConverter(typeof(LongConverter))]
        public long CategoryId { get; set; }
        public long InfluenceCap { get; set; }
        public Dictionary<string, string> LocalizedName { get; internal set; }
        public Dictionary<string, string> LocalizedDescription { get; internal set; }
        public Dictionary<string, string> LocalizedTitle { get; internal set; }
        public Dictionary<string, string> LocalizedCategory { get; internal set; }
        public Dictionary<string, string> LocalizedSubCategory { get; internal set; }
        public string Name { get; internal set; }
        [JsonConverter(typeof(LongConverter))]
        public long NameId { get; internal set; }
        public string Description { get; internal set; }
        [JsonConverter(typeof(LongConverter))]
        public long DescriptionId { get; internal set; }
        [JsonConverter(typeof(ULongConverter))]
        public ulong NpcId { get; set; }
        public string Icon { get; set; }
        public string HashedIcon
        {
            get
            {
                var fileId = TorLib.FileId.FromFilePath(String.Format("/resources/gfx/portraits/{0}.dds", this.Icon));
                return String.Format("{0}_{1}", fileId.ph, fileId.sh);
            }
        }
        public string SubCategory { get; set; }
        public long SubCategoryId { get; set; }
        public string Title { get; internal set; }
        [JsonConverter(typeof(LongConverter))]
        public long TitleId { get; internal set; }
        internal Companion Comp_ { get; set; }
        public Companion Companion
        {
            get
            {
                if (Comp_ == null && NpcId != 0)
                {
                    Comp_ = Dom_.companionLoader.Load(NpcId);
                }
                return Comp_;
            }
        }

        public override int GetHashCode()
        {
            int hash = Id.GetHashCode();
            if (AcquireConditionalIds != null) foreach (var x in AcquireConditionalIds) { hash ^= x.GetHashCode(); }
            hash ^= MaxInfluenceTier.GetHashCode();
            if (AllianceAlerts != null) foreach (var x in AllianceAlerts) { hash ^= x.GetHashCode(); }
            hash ^= CategoryId.GetHashCode();
            hash ^= InfluenceCap.GetHashCode();
            if (LocalizedName != null) foreach (var x in LocalizedName) { hash ^= x.GetHashCode(); }
            if (LocalizedTitle != null) foreach (var x in LocalizedTitle) { hash ^= x.GetHashCode(); }
            if (LocalizedName != null) foreach (var x in LocalizedName) { hash ^= x.GetHashCode(); }
            if (LocalizedTitle != null) foreach (var x in LocalizedTitle) { hash ^= x.GetHashCode(); }
            hash ^= NameId.GetHashCode();
            hash ^= NpcId.GetHashCode();
            hash ^= Icon.GetHashCode();
            hash ^= SubCategory.GetHashCode();
            hash ^= SubCategoryId.GetHashCode();
            hash ^= TitleId.GetHashCode();
            hash ^= SubCategory.GetHashCode();
            if (Companion != null) hash ^= Companion.GetHashCode();
            return hash;
        }

        public override bool Equals(object obj)
        {
            if (obj == null) return false;

            if (ReferenceEquals(this, obj)) return true;

            if (!(obj is Companion cmp)) return false;

            return Equals(cmp);
        }

        public bool Equals(Companion cmp)
        {
            if (cmp == null) return false;

            if (ReferenceEquals(this, cmp)) return true;

            if (this.GetHashCode() != cmp.GetHashCode())
                return false;
            return true;
        }

        public override List<SQLProperty> SQLProperties
        {
            get
            {
                return new List<SQLProperty>
                    {                //(SQL Column Name, C# Property Name, SQL Column type statement, IsUnique/PrimaryKey, Serialize value to json)
                        new SQLProperty("Name", "LocalizedName[enMale]", "varchar(255) COLLATE utf8_unicode_ci NOT NULL", SQLPropSetting.AddIndex),
                        new SQLProperty("FrName", "LocalizedName[frMale]", "varchar(255) COLLATE utf8_unicode_ci NOT NULL", SQLPropSetting.AddIndex),
                        new SQLProperty("DeName", "LocalizedName[deMale]", "varchar(255) COLLATE utf8_unicode_ci NOT NULL", SQLPropSetting.AddIndex),
                        new SQLProperty("Title", "LocalizedTitle[enMale]", "varchar(255) COLLATE utf8_unicode_ci NOT NULL", SQLPropSetting.AddIndex),
                        new SQLProperty("Base62Id", "Base62Id", "varchar(7) COLLATE latin1_general_cs NOT NULL", SQLPropSetting.PrimaryKey),
                        new SQLProperty("MaxInfluenceTier", "MaxInfluenceTier", "int(11) NOT NULL", SQLPropSetting.AddIndex),
                        new SQLProperty("InfluenceCap", "InfluenceCap", "int(11) NOT NULL", SQLPropSetting.AddIndex),
                        new SQLProperty("Category", "LocalizedCategory[enMale]", "varchar(255) COLLATE utf8_unicode_ci NOT NULL", SQLPropSetting.AddIndex),
                        new SQLProperty("FrCategory", "LocalizedCategory[frMale]", "varchar(255) COLLATE utf8_unicode_ci NOT NULL", SQLPropSetting.AddIndex),
                        new SQLProperty("DeCategory", "LocalizedCategory[deMale]", "varchar(255) COLLATE utf8_unicode_ci NOT NULL", SQLPropSetting.AddIndex),
                        new SQLProperty("SubCategory", "LocalizedSubCategory[enMale]", "varchar(255) COLLATE utf8_unicode_ci NOT NULL", SQLPropSetting.AddIndex),
                        new SQLProperty("FrSubCategory", "LocalizedSubCategory[frMale]", "varchar(255) COLLATE utf8_unicode_ci NOT NULL", SQLPropSetting.AddIndex),
                        new SQLProperty("DeSubCategory", "LocalizedSubCategory[deMale]", "varchar(255) COLLATE utf8_unicode_ci NOT NULL", SQLPropSetting.AddIndex),
                    };
            }
        }

        public Dictionary<string, Dictionary<string, string>> InteractionList { get; internal set; }

        public override XElement ToXElement(bool verbose)
        {
            XElement companion = new XElement("NewCompanion");
            if (this.Id != 0)
            {
                companion.Add(new XAttribute("Id", Id),
                    new XElement("Name", Name),
                    new XElement("Title", Title));
                if (verbose)
                {
                    companion.Add(new XElement("Fqn", Fqn,
                        new XAttribute("Id", Id)));


                    companion.Add(new XElement("MaxInfluenceTier", MaxInfluenceTier),
                        new XElement("ContactCategory", Category),
                        new XElement("FollowerCategory", SubCategory),
                        new XElement("InfluenceCap", InfluenceCap));


                    if (Companion != null)
                        companion.Add(Companion.ToXElement(false));
                }
            }
            return companion;
        }
    }

    public class AllianceAlert
    {
        [JsonConverter(typeof(ULongConverter))]
        public ulong ConditionId { get; set; }
        public string ConditionB62Id { get { if (ConditionId != 0) return ConditionId.ToMaskedBase62(); return ""; } }
        [JsonConverter(typeof(ULongConverter))]
        public ulong MissionId { get; set; }
        public string MissionB62Id { get { if (MissionId != 0) return MissionId.ToMaskedBase62(); return ""; } }
        [JsonConverter(typeof(ULongConverter))]
        public ulong NcoId { get; set; }
        public string NcoB62Id { get { if (NcoId != 0) return NcoId.ToMaskedBase62(); return ""; } }
    }
}
