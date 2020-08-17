using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;
using System.Xml.XPath;
using System.Xml;
using Newtonsoft.Json;

namespace GomLib.Models
{
    public class Item : GameObject, IEquatable<Item>
    {
        public override string ToString()
        {
            return string.Format("{0:000000} {1}: [{2}] {3}",
                Id, LocalizedName["enMale"] ?? "", ItemLevel, StatModifiers);
        }

        public Item Clone()
        {
            Item clone = this.MemberwiseClone() as Item;
            //
            ItemStatList ar = new ItemStatList();
            foreach (ItemStat istat in this.StatModifiers)
            {
                ar.Add(istat.Clone());
            }
            clone.StatModifiers = ar;
            //
            return clone;
        }

        #region Properties
        public ulong ShortId { get; set; }
        [JsonConverter(typeof(LongConverter))]
        public long NameId { get; set; }
        public string CleanName
        {
            get
            {
                if (Name == null || Name == "")
                    return "Unnamed_Item";
                //return Name.Replace("'", "");
                return System.IO.Path.GetInvalidFileNameChars().Aggregate(Name, (current, c) => current.Replace(c.ToString(), string.Empty)).Replace("'", "").Replace(" ", "_");
            }
        }
        public string Name { get; set; }
        public Dictionary<string, string> LocalizedName { get; set; }
        [JsonConverter(typeof(LongConverter))]
        public long DescriptionId { get; set; }
        [JsonIgnore]
        public string Description_
        {
            get
            {
                return System.Text.RegularExpressions.Regex.Replace(Description, @"\r\n?|\n", "<br />");
            }
        }
        public string Description { get; set; }

        public Dictionary<string, string> LocalizedDescription { get; set; }
        public int Value { get; set; }
        public int Durability { get; set; }
        public int MaxStack { get; set; }
        public int UniqueLimit { get; set; }
        public WeaponSpec WeaponSpec { get; set; }
        public ArmorSpec ArmorSpec { get; set; }
        public ArmorSpec ShieldSpec { get; set; }
        [JsonConverter(typeof(Newtonsoft.Json.Converters.StringEnumConverter))]
        public ItemBindingRule Binding { get; set; }
        public string Icon { get; set; }
        public string ImperialIcon { get; set; }
        public string RepublicIcon { get; set; }
        public string HashedIcon
        {
            get
            {
                var fileId = TorLib.FileId.FromFilePath(String.Format("/resources/gfx/icons/{0}.dds", this.Icon));
                return String.Format("{0}_{1}", fileId.ph, fileId.sh);
            }
        }
        public string HashedImperialIcon
        {
            get
            {
                if (String.IsNullOrEmpty(RepublicIcon)) return "";
                var fileId = TorLib.FileId.FromFilePath(String.Format("/resources/gfx/icons/{0}.dds", this.ImperialIcon));
                return String.Format("{0}_{1}", fileId.ph, fileId.sh);
            }
        }
        public string HashedRepublicIcon
        {
            get
            {
                if (String.IsNullOrEmpty(RepublicIcon)) return "";
                var fileId = TorLib.FileId.FromFilePath(String.Format("/resources/gfx/icons/{0}.dds", this.RepublicIcon));
                return String.Format("{0}_{1}", fileId.ph, fileId.sh);
            }
        }
        [JsonConverter(typeof(Newtonsoft.Json.Converters.StringEnumConverter))]
        public ItemQuality Quality { get; set; }
        public int ItemLevel { get; set; }
        public int Rating { get; set; }
        public int CombinedRating { get; set; }
        public int RequiredLevel { get; set; }
        public int CombinedRequiredLevel { get; set; }
        [JsonConverter(typeof(Newtonsoft.Json.Converters.StringEnumConverter))]
        public ItemDamageType DamageType { get; set; }
        public int VendorStackSize { get; set; }
        public bool RequiresAlignment { get; set; }
        public int RequiredAlignmentTier { get; set; }
        public bool RequiredAlignmentInverted { get; set; }
        public bool RequiresSocial { get; set; }
        public int RequiredSocialTier { get; set; }
        [JsonConverter(typeof(Newtonsoft.Json.Converters.StringEnumConverter))]
        public Profession RequiredProfession { get; set; }
        public int RequiredProfessionLevel { get; set; }
        [JsonConverter(typeof(Newtonsoft.Json.Converters.StringEnumConverter))]
        public ProfessionSubtype DisassembleCategory { get; set; }
        [JsonConverter(typeof(Newtonsoft.Json.Converters.StringEnumConverter))]
        public EnhancementCategory EnhancementCategory { get; set; }
        [JsonConverter(typeof(Newtonsoft.Json.Converters.StringEnumConverter))]
        public EnhancementSubCategory EnhancementSubCategory { get; set; }
        [JsonConverter(typeof(Newtonsoft.Json.Converters.StringEnumConverter))]
        public EnhancementType EnhancementType { get; set; }
        public DetailedEnhancementType DetEnhancementType { get; set; }
        [JsonConverter(typeof(Newtonsoft.Json.Converters.StringEnumConverter))]
        public GiftType GiftType { get; set; }
        [JsonConverter(typeof(Newtonsoft.Json.Converters.StringEnumConverter))]
        public GiftRank GiftRank { get; set; }
        public int GiftRankNum { get; set; }
        public int AuctionCategoryId { get; set; }
        public AuctionCategory AuctionCategory { get; set; }
        public int AuctionSubCategoryId { get; set; }
        public AuctionSubCategory AuctionSubCategory { get; set; }
        public string AppearanceColor { get; set; }
        public int DyeId { get; set; }
        public DetailedAppearanceColor DyeColor { get; set; }
        [JsonConverter(typeof(ULongConverter))]
        public ulong EquipAbilityId { get; set; }
        public string EquipAbilityB62Id { get { return EquipAbilityId.ToMaskedBase62(); } }
        [JsonIgnore]
        public Ability EquipAbility { get; set; }
        [JsonConverter(typeof(ULongConverter))]
        public ulong UseAbilityId { get; set; }
        public string UseAbilityB62Id { get { return UseAbilityId.ToMaskedBase62(); } }
        internal Ability UseAbility_ { get; set; }
        [JsonIgnore]
        public Ability UseAbility { get; set; }
        public Dictionary<string, string> UseAbilityParsedDescription
        {
            get
            {
                if (UseAbility == null) return null;
                return UseAbility.ParsedLocalizedDescription;
            }
        }

        public string ConversationFqn { get; set; }

        [JsonIgnore]
        public Conversation Conversation { get; set; }

        [JsonConverter(typeof(LongConverter))]
        public long ModifierSpec { get; set; }
        [JsonIgnore]
        public Schematic Schematic_ { get; set; }
        [JsonIgnore]
        public Schematic Schematic
        {
            get
            {
                if (Schematic_ == null)
                {
                    Schematic_ = Dom_.schematicLoader.Load(SchematicId);
                }
                return Schematic_;
            }
        }
        [JsonConverter(typeof(ULongConverter))]
        public ulong SchematicId { get; set; }
        public string SchematicB62Id { get { return SchematicId.ToMaskedBase62(); } }
        //public string TreasurePackageSpec { get; set; } //unused
        [JsonConverter(typeof(LongConverter))]
        public long TreasurePackageId { get; set; }
        [JsonConverter(typeof(LongConverter))]
        public long MountSpec { get; set; }
        public Gender RequiredGender { get; set; }
        public int RequiredValorRank { get; set; }
        public int RequiredReputationId { get; set; }
        [JsonIgnore]
        public ReputationGroup RequiredReputation_ { get; set; }
        [JsonIgnore]
        public ReputationGroup RequiredReputation
        {
            get
            {
                if (RequiredReputation_ == null)
                    RequiredReputation_ = Dom_.reputationGroupLoader.Load(RequiredReputationId);
                return RequiredReputation_;
            }
        }
        public string RequiredReputationName
        {
            get
            {
                if (RequiredReputation != null)
                {
                    if (RequiredReputation.GroupEmpireTitle != RequiredReputation.GroupRepublicTitle)
                    {
                        return String.Join(" / ", RequiredReputation.GroupEmpireTitle, RequiredReputation.GroupRepublicTitle);
                    }
                    else
                        return RequiredReputation.GroupEmpireTitle;
                }
                return null;
            }
        }
        public int RequiredReputationLevelId { get; set; }
        public ReputationRank RequiredReputationLevel_ { get; set; }
        [JsonIgnore]
        public ReputationRank RequiredReputationLevel
        {
            get
            {
                if (RequiredReputationLevel_ == null)
                    RequiredReputationLevel_ = Dom_.reputationRankLoader.Load(RequiredReputationLevelId);
                return RequiredReputationLevel_;
            }
        }
        public string RequiredReputationLevelName
        {
            get
            {
                if (RequiredReputationLevel != null)
                {
                    return RequiredReputationLevel.RankTitle;
                }
                return null;
            }
        }
        public Dictionary<string, string> LocalizedRequiredReputationLevelName
        {
            get
            {
                if (RequiredReputationLevel != null)
                {
                    return RequiredReputationLevel.LocalizedRankTitle;
                }
                return null;
            }
        }

        public bool ConsumedOnUse { get; set; }
        public int TypeBitSet { get; set; }

        [JsonConverter(typeof(LongConverter))]
        public long ItmCraftedCategory { get; set; }

        [JsonConverter(typeof(Newtonsoft.Json.Converters.StringEnumConverter))]
        public ItemCategory Category { get; set; }
        [JsonConverter(typeof(Newtonsoft.Json.Converters.StringEnumConverter))]
        public ItemSubCategory SubCategory { get; set; }

        [JsonIgnore]
        public ItemStatList StatModifiers { get; set; }
        internal Dictionary<string, int> SimpleStatModifiers_ { get; set; }
        public Dictionary<string, int> SimpleStatModifiers
        {
            get
            {
                if (SimpleStatModifiers_ == null && StatModifiers != null)
                {
                    SimpleStatModifiers_ = StatModifiers.ToDictionary(x => x.DetailedStat.LocalizedDisplayName["enMale"], x => x.Modifier);
                }
                return SimpleStatModifiers_;
            }
        }
        [JsonIgnore]
        public ItemStatList CombinedStatModifiers { get; set; }
        internal Dictionary<string, int> SimpleCombinedStatModifiers_ { get; set; }
        public Dictionary<string, int> SimpleCombinedStatModifiers
        {
            get
            {
                if (SimpleCombinedStatModifiers_ == null && CombinedStatModifiers != null)
                {
                    SimpleCombinedStatModifiers_ = CombinedStatModifiers.ToDictionary(x => x.DetailedStat.LocalizedDisplayName["enMale"], x => x.Modifier);
                }
                return SimpleCombinedStatModifiers_;
            }
        }

        public ItemEnhancementList EnhancementSlots { get; set; }
        [JsonIgnore]
        public ClassSpecList RequiredClasses { get; set; }
        [JsonProperty(ItemConverterType = typeof(Newtonsoft.Json.Converters.StringEnumConverter))]
        public SlotTypeList Slots { get; set; }
        public string SoundType { get; set; }
        public long StackCount { get; set; }
        public long MaxDurability { get; set; }
        public string WeaponAppSpec { get; set; }
        [JsonIgnore]
        internal WeaponAppearance _WeaponApp;
        public WeaponAppearance WeaponApp
        {
            get
            {
                if (WeaponAppSpec == null || WeaponAppSpec == "")
                    return null;
                if (_WeaponApp == null)
                    _WeaponApp = Dom_.appearanceLoader.LoadWeaponAppearance(WeaponAppSpec);
                return _WeaponApp;
            }
        }

        public string Model { get; set; }
        public string VOModulationImperial { get; set; }
        public string VOModulationRepublic { get; set; }
        public string ImperialAppearanceTag { get; set; }
        public string RepublicAppearanceTag { get; set; }

        [JsonConverter(typeof(ULongConverter))]
        public ulong TeachesRef { get; set; }
        public string TeachesRefB62 { get { return TeachesRef.ToMaskedBase62(); } }
        public string TeachesType { get; set; }
        public bool IsUnknownBool { get; set; }
        public List<long> StrongholdSourceList { get; set; }
        public Dictionary<long, string> StrongholdSourceNameDict { get; set; }
        public Dictionary<long, Dictionary<string, string>> LocalizedStrongholdSourceNameDict { get; set; }

        //public List<ulong> rewardedForQuests { get; set; }
        [JsonIgnore]
        public Dictionary<ulong, ulong> IppRefs { get; set; }
        public Dictionary<string, string> ClassAppearance { get; set; }
        public string AppearanceImperial { get; set; }
        public string AppearanceRepublic { get; set; }
        [JsonConverter(typeof(LongConverter))]
        public long SetBonusId { get; set; }
        public string SetBonusB62Id { get { return SetBonusId.ToMaskedBase62(); } }
        [JsonIgnore]
        internal SetBonusEntry SetBonus_ { get; set; }
        [JsonIgnore]
        public SetBonusEntry SetBonus
        {
            get
            {
                if (SetBonus_ == null)
                {
                    SetBonus_ = Dom_.setBonusLoader.Load(SetBonusId);
                }
                return SetBonus_;
            }
            set
            {
                SetBonus_ = value;
            }
        }
        [JsonConverter(typeof(ULongConverter))]
        public ulong ChildId { get; set; }
        public string ChildBase62Id
        {
            get
            {
                if (ChildId != 0)
                    return ChildId.ToMaskedBase62();
                else
                    return "";
            }
        }
        internal string MtxRarity { get; set; }
        public string MTXRarity
        {
            get
            {
                return MtxRarity;
            }
            set
            {
                if (value != "0x00")
                {
                    MtxRarity = value.Replace("itmMTXRarity", "");
                    switch (value)
                    {
                        case "itmMTXRaritySuperRare": MtxRarity = "Gold"; break;
                        case "itmMTXRarityRare": MtxRarity = "Silver"; break;
                        case "itmMTXRarityNormal": MtxRarity = "Bronze"; break;
                        default: MtxRarity = "Unknown"; break;
                    }
                }
            }
        }

        public bool IsDecoration
        {
            get
            {
                return (TeachesType == "Decoration");
            }
        }
        [JsonIgnore]
        public Decoration Decoration_ { get; set; }
        [JsonIgnore]
        public Decoration Decoration
        {
            get
            {
                if (Decoration_ == null)
                {
                    Decoration_ = Dom_.decorationLoader.Load(TeachesRef);
                }
                return Decoration_;
            }
        }

        public bool BindsToSlot { get; set; }
        public int RepFactionId { get; set; }
        [JsonIgnore]
        internal ReputationGroup Faction { get; set; }
        [JsonIgnore]
        internal string RepFactionName_ { get; set; }
        public string RepFactionName
        {
            get
            {
                if (RepFactionName_ == null)
                {
                    if (Faction == null) Faction = Dom_.reputationGroupLoader.Load(RequiredReputationId);
                    if (Faction != null)
                    {
                        if (Faction.GroupRepublicTitle != Faction.GroupEmpireTitle)
                            RepFactionName_ = String.Format("{0} / {1}", Faction.GroupRepublicTitle, Faction.GroupEmpireTitle);
                        else
                            RepFactionName_ = Faction.GroupEmpireTitle;
                    }
                }
                return RepFactionName_;
            }
        }
        [JsonIgnore]
        internal Dictionary<string, Dictionary<string, string>> LocalizedRepFactionDictionary_ { get; set; }
        public Dictionary<string, Dictionary<string, string>> LocalizedRepFactionDictionary
        {
            get
            {
                if (LocalizedRepFactionDictionary_ == null)
                {
                    LocalizedRepFactionDictionary_ = new Dictionary<string, Dictionary<string, string>>();
                    if (Faction == null) Faction = Dom_.reputationGroupLoader.Load(RepFactionId);
                    if (Faction != null)
                    {
                        LocalizedRepFactionDictionary_.Add("Imperial", Faction.LocalizedGroupEmpireTitle);
                        LocalizedRepFactionDictionary_.Add("Republic", Faction.LocalizedGroupRepublicTitle);
                    }
                    else
                        return new Dictionary<string, Dictionary<string, string>>();
                }
                return LocalizedRepFactionDictionary_;
            }
        }
        internal Dictionary<string, string> LocalizedRepFactionName_ { get; set; }
        public Dictionary<string, string> LocalizedRepFactionName
        {
            get
            {
                if (LocalizedRepFactionName_ == null)
                {
                    if (Faction == null) Faction = Dom_.reputationGroupLoader.Load(RequiredReputationId);
                    if (Faction != null)
                    {
                        LocalizedRepFactionName_ = Faction.LocalizedGroupleTitle;
                    }
                }
                return LocalizedRepFactionName_;
            }

            set
            {
                LocalizedRepFactionName_ = value;
            }
        }

        public long RequiredPermission { get; set; }
        public bool ReqArtEquipAuth { get; set; }
        public long DisintegrateCmdXP { get; set; }

        [JsonIgnore]
        public string Tooltip
        {
            get
            {
                return new Tooltip(this).HTML;
            }
        }
        #endregion
        #region FlatData Properties and Methods
        [JsonIgnore]
        public string ArmoringBId { get; set; }
        [JsonIgnore]
        public string ModificationBId { get; set; }
        [JsonIgnore]
        public string EnhancementBId { get; set; }
        [JsonIgnore]
        public string ColorCrystalBId { get; set; }
        [JsonIgnore]
        public string BarrelBId { get; set; }
        [JsonIgnore]
        public string HiltBId { get; set; }

        //primary
        [JsonIgnore]
        public int Endurance { get; set; }
        [JsonIgnore]
        public int Presence { get; set; }
        [JsonIgnore]
        public int Aim { get; set; }
        [JsonIgnore]
        public int Cunning { get; set; }
        [JsonIgnore]
        public int Strength { get; set; }
        [JsonIgnore]
        public int Willpower { get; set; }
        [JsonIgnore]
        public int Mastery { get; set; }
        //secondary
        [JsonIgnore]
        public int AbsorptionRating { get; set; }
        [JsonIgnore]
        public int CriticalRating { get; set; }
        [JsonIgnore]
        public int DefenseRating { get; set; }
        [JsonIgnore]
        public int Power { get; set; }
        //tertiary
        [JsonIgnore]
        public int AccuracyRating { get; set; }
        [JsonIgnore]
        public int AlacrityRating { get; set; }
        [JsonIgnore]
        public int ShieldRating { get; set; }
        [JsonIgnore]
        public int SurgeRating { get; set; }
        //quarternary
        [JsonIgnore]
        public int ExpertiseRating { get; set; }
        [JsonIgnore]
        public int ForcePower { get; set; }
        [JsonIgnore]
        public int TechPower { get; set; }
        [JsonIgnore]
        public int TechHealingPower { get; set; }
        [JsonIgnore]
        public int ForceHealingPower { get; set; }
        // Derived
        [JsonIgnore]
        public int Armor { get; set; }
        [JsonIgnore]
        public string AdaptiveArmor { get; set; }
        [JsonIgnore]
        public float WeaponMinDamage { get; set; }
        [JsonIgnore]
        public float WeaponMaxDamage { get; set; }
        [JsonIgnore]
        public int ModLevel { get; set; }
        //offhand stats
        [JsonIgnore]
        public float AbsorbChance { get; set; }
        [JsonIgnore]
        public float ShieldChance { get; set; }

        //Slots
        [JsonIgnore]
        public bool Any { get; set; }
        [JsonIgnore]
        public bool EquipHumanEar { get; set; }
        [JsonIgnore]
        public bool EquipHumanMainHand { get; set; }
        [JsonIgnore]
        public bool EquipHumanOffHand { get; set; }
        [JsonIgnore]
        public bool EquipHumanLeg { get; set; }
        [JsonIgnore]
        public bool EquipHumanRangedPrimary { get; set; }
        [JsonIgnore]
        public bool EquipHumanShield { get; set; }
        [JsonIgnore]
        public bool EquipHumanFoot { get; set; }
        [JsonIgnore]
        public bool EquipHumanImplant { get; set; }
        [JsonIgnore]
        public bool EquipHumanChest { get; set; }
        [JsonIgnore]
        public bool EquipHumanGlove { get; set; }
        [JsonIgnore]
        public bool EquipHumanCustomRanged { get; set; }
        [JsonIgnore]
        public bool EquipHumanFace { get; set; }
        [JsonIgnore]
        public bool EquipHumanBelt { get; set; }
        [JsonIgnore]
        public bool EquipHumanRangedSecondary { get; set; }
        [JsonIgnore]
        public bool EquipHumanWrist { get; set; }
        [JsonIgnore]
        public bool EquipHumanCustomMelee { get; set; }
        [JsonIgnore]
        public bool EquipHumanRelic { get; set; }
        [JsonIgnore]
        public bool EquipDroidSensor { get; set; }
        [JsonIgnore]
        public bool EquipHumanOutfit { get; set; }
        [JsonIgnore]
        public bool EquipDroidUpper { get; set; }
        [JsonIgnore]
        public bool EquipDroidUtility { get; set; }
        [JsonIgnore]
        public bool EquipDroidLower { get; set; }
        [JsonIgnore]
        public bool EquipSpaceShipArmor { get; set; }
        [JsonIgnore]
        public bool EquipSpaceShieldRegenerator { get; set; }
        [JsonIgnore]
        public bool EquipSpaceProtonTorpedoes { get; set; }
        [JsonIgnore]
        public bool EffectOther { get; set; }
        [JsonIgnore]
        public bool EquipSpaceBeamGenerator { get; set; }
        [JsonIgnore]
        public bool Upgrade { get; set; }
        [JsonIgnore]
        public bool EquipHumanHeirloom { get; set; }
        [JsonIgnore]
        public bool EquipSpaceBeamCharger { get; set; }
        [JsonIgnore]
        public bool EquipSpaceAbilityDefense { get; set; }
        [JsonIgnore]
        public bool EquipSpaceMissileMagazine { get; set; }
        [JsonIgnore]
        public bool EquipSpaceEnergyShield { get; set; }
        [JsonIgnore]
        public bool EquipSpaceAbilitySystems { get; set; }
        [JsonIgnore]
        public bool EquipDroidShield { get; set; }
        [JsonIgnore]
        public bool EquipDroidSpecial { get; set; }
        [JsonIgnore]
        public bool EquipDroidWeapon1 { get; set; }
        [JsonIgnore]
        public bool EquipDroidWeapon2 { get; set; }
        [JsonIgnore]
        public bool EquipSpaceAbilityOffense { get; set; }
        [JsonIgnore]
        public bool QuestTracker { get; set; }

        //public List<string> MaterialForSchems { get; set; }
        //public List<string> RewardFromQuests { get; set; }
        internal List<string> RequiredClassesB62_ { get; set; }
        public List<string> RequiredClassesB62
        {
            get
            {
                if (RequiredClassesB62_ == null && RequiredClasses != null)
                {
                    RequiredClassesB62_ = RequiredClasses.Select(x => x.Base62Id).ToList();
                }
                return RequiredClassesB62_;
            }
        }
        //public List<string> SimilarAppearance { get; set; } 
        public static List<long> ArmorSpecIds = new List<long> { -8622546409652942944, 589686270506543030, 5763611092890301551 };
        public static List<ArmorSpec> ArmorSpecs = new List<ArmorSpec> { };

        //typebitflags
        internal TypeBitFlags TypeBitFlags_ { get; set; }
        public TypeBitFlags TypeBitFlags
        {
            get
            {
                if (TypeBitFlags_ == null)
                {
                    var flags = ((ItemTypeFlags)TypeBitSet);
                    TypeBitFlags_ = new TypeBitFlags(flags.HasFlag(ItemTypeFlags.IsArmor), flags.HasFlag(ItemTypeFlags.IsWeapon), flags.HasFlag(ItemTypeFlags.HasGTNCategory), flags.HasFlag(ItemTypeFlags.Unk8), flags.HasFlag(ItemTypeFlags.HasConversation), flags.HasFlag(ItemTypeFlags.IsCrafted), flags.HasFlag(ItemTypeFlags.CanBeDisassembled), flags.HasFlag(ItemTypeFlags.HasDurability), flags.HasFlag(ItemTypeFlags.IsModdable), flags.HasFlag(ItemTypeFlags.IsMod), flags.HasFlag(ItemTypeFlags.CanHaveStats), flags.HasFlag(ItemTypeFlags.Unk800), flags.HasFlag(ItemTypeFlags.IsGift), flags.HasFlag(ItemTypeFlags.IsMissionItem), flags.HasFlag(ItemTypeFlags.Unk4000), flags.HasFlag(ItemTypeFlags.IsShipPart), flags.HasFlag(ItemTypeFlags.Unk10000), flags.HasFlag(ItemTypeFlags.IsCmpCstmztn), flags.HasFlag(ItemTypeFlags.HasUniqueLimit), flags.HasFlag(ItemTypeFlags.HasOnUse), flags.HasFlag(ItemTypeFlags.IsEquipable), flags.HasFlag(ItemTypeFlags.IsCurrency), flags.HasFlag(ItemTypeFlags.IsMtxItem), flags.HasFlag(ItemTypeFlags.IsRepTrophy));
                }
                return TypeBitFlags_;
            }
        }

        public static Item FillFlatData(Item itm)
        {
            foreach (var stat in itm.CombinedStatModifiers)
            {
                switch (stat.Stat)
                {
                    case Stat.GlanceRating: itm.ShieldRating = stat.Modifier; break;
                    case Stat.Strength: itm.Strength = stat.Modifier; break;
                    case Stat.Aim: itm.Aim = stat.Modifier; break;
                    case Stat.Cunning: itm.Cunning = stat.Modifier; break;
                    case Stat.Endurance: itm.Endurance = stat.Modifier; break;
                    case Stat.Presence: itm.Presence = stat.Modifier; break;
                    case Stat.Willpower: itm.Willpower = stat.Modifier; break;
                    case Stat.Mastery: itm.Mastery = stat.Modifier; break;
                    case Stat.ExpertiseRating: itm.ExpertiseRating = stat.Modifier; break;
                    case Stat.AbsorptionRating: itm.AbsorptionRating = stat.Modifier; break;
                    case Stat.AttackPowerRating: itm.Power = stat.Modifier; break;
                    case Stat.ForcePowerRating: itm.ForcePower = stat.Modifier; break;
                    case Stat.TechPowerRating: itm.TechPower = stat.Modifier; break;
                    case Stat.AccuracyRating: itm.AccuracyRating = stat.Modifier; break;
                    case Stat.CriticalChanceRating: itm.CriticalRating = stat.Modifier; break;
                    case Stat.DefenseRating: itm.DefenseRating = stat.Modifier; break;
                    case Stat.TechHealingPower: itm.TechHealingPower = stat.Modifier; break;
                    case Stat.ForceHealingPower: itm.ForceHealingPower = stat.Modifier; break;
                    case Stat.SurgeRating: itm.SurgeRating = stat.Modifier; break;
                    case Stat.AlacrityRating: itm.AlacrityRating = stat.Modifier; break;
                    default:
                        break;
                }
            }
            foreach (var slot in itm.Slots)
            {
                switch (slot)
                {
                    case SlotType.Any: itm.Any = true; break;
                    case SlotType.EquipHumanEar: itm.EquipHumanEar = true; break;
                    case SlotType.EquipHumanMainHand: itm.EquipHumanMainHand = true; break;
                    case SlotType.EquipHumanOffHand: itm.EquipHumanOffHand = true; break;
                    case SlotType.EquipHumanLeg: itm.EquipHumanLeg = true; break;
                    case SlotType.EquipHumanRangedPrimary: itm.EquipHumanRangedPrimary = true; break;
                    case SlotType.EquipHumanShield: itm.EquipHumanShield = true; break;
                    case SlotType.EquipHumanFoot: itm.EquipHumanFoot = true; break;
                    case SlotType.EquipHumanImplant: itm.EquipHumanImplant = true; break;
                    case SlotType.EquipHumanChest: itm.EquipHumanChest = true; break;
                    case SlotType.EquipHumanGlove: itm.EquipHumanGlove = true; break;
                    case SlotType.EquipHumanCustomRanged: itm.EquipHumanCustomRanged = true; break;
                    case SlotType.EquipHumanFace: itm.EquipHumanFace = true; break;
                    case SlotType.EquipHumanBelt: itm.EquipHumanBelt = true; break;
                    case SlotType.EquipHumanRangedSecondary: itm.EquipHumanRangedSecondary = true; break;
                    case SlotType.EquipHumanWrist: itm.EquipHumanWrist = true; break;
                    case SlotType.EquipHumanCustomMelee: itm.EquipHumanCustomMelee = true; break;
                    case SlotType.EquipHumanRelic: itm.EquipHumanRelic = true; break;
                    case SlotType.EquipDroidSensor: itm.EquipDroidSensor = true; break;
                    case SlotType.EquipHumanOutfit: itm.EquipHumanOutfit = true; break;
                    case SlotType.EquipDroidUpper: itm.EquipDroidUpper = true; break;
                    case SlotType.EquipDroidUtility: itm.EquipDroidUtility = true; break;
                    case SlotType.EquipDroidLower: itm.EquipDroidLower = true; break;
                    case SlotType.EquipSpaceShipArmor: itm.EquipSpaceShipArmor = true; break;
                    case SlotType.EquipSpaceShieldRegenerator: itm.EquipSpaceShieldRegenerator = true; break;
                    case SlotType.EquipSpaceProtonTorpedoes: itm.EquipSpaceProtonTorpedoes = true; break;
                    case SlotType.EffectOther: itm.EffectOther = true; break;
                    case SlotType.EquipSpaceBeamGenerator: itm.EquipSpaceBeamGenerator = true; break;
                    case SlotType.Upgrade: itm.Upgrade = true; break;
                    case SlotType.EquipHumanHeirloom: itm.EquipHumanHeirloom = true; break;
                    case SlotType.EquipSpaceBeamCharger: itm.EquipSpaceBeamCharger = true; break;
                    case SlotType.EquipSpaceAbilityDefense: itm.EquipSpaceAbilityDefense = true; break;
                    case SlotType.EquipSpaceMissileMagazine: itm.EquipSpaceMissileMagazine = true; break;
                    case SlotType.EquipSpaceEnergyShield: itm.EquipSpaceEnergyShield = true; break;
                    case SlotType.EquipSpaceAbilitySystems: itm.EquipSpaceAbilitySystems = true; break;
                    case SlotType.EquipDroidShield: itm.EquipDroidShield = true; break;
                    case SlotType.EquipDroidSpecial: itm.EquipDroidSpecial = true; break;
                    case SlotType.EquipDroidWeapon1: itm.EquipDroidWeapon1 = true; break;
                    case SlotType.EquipDroidWeapon2: itm.EquipDroidWeapon2 = true; break;
                    case SlotType.EquipSpaceAbilityOffense: itm.EquipSpaceAbilityOffense = true; break;
                    case SlotType.QuestTracker: itm.QuestTracker = true; break;

                    default:
                        break;
                }
            }
            foreach (var mod in itm.EnhancementSlots)
            {

                switch (mod.Slot)
                {
                    case EnhancementType.Hilt: itm.HiltBId = mod.ModificationId.ToMaskedBase62(); break;
                    case EnhancementType.Harness: itm.ArmoringBId = mod.ModificationId.ToMaskedBase62(); break;
                    case EnhancementType.Overlay: itm.ModificationBId = mod.ModificationId.ToMaskedBase62(); break;
                    case EnhancementType.Support: itm.EnhancementBId = mod.ModificationId.ToMaskedBase62(); break;
                    case EnhancementType.Barrel: itm.BarrelBId = mod.ModificationId.ToMaskedBase62(); break;
                    case EnhancementType.PowerCrystal: itm.HiltBId = mod.ModificationId.ToMaskedBase62(); break;
                    case EnhancementType.ColorCrystal: itm.ColorCrystalBId = mod.ModificationId.ToMaskedBase62(); break;
                    default:
                        break;
                }
            }

            //slot
            bool isEquipable = false;
            if (itm.Slots.Count > 1) //the Any slot was removed from the item by the itemloader
            {
                isEquipable = true;
            }
            //armor, rating etc if gear
            itm.ModLevel = itm.ItemLevel;
            itm.AdaptiveArmor = "";
            if (itm.WeaponSpec != null)
            {
                List<int> mainSlots = new List<int> { 1, 3, 9 };
                ItemEnhancement mainMod = null;
                if (itm.EnhancementSlots != null)
                {
                    var potentials = itm.EnhancementSlots.Where(x => x.Slot.IsBaseMod());
                    if (potentials.Count() != 0)
                        mainMod = itm.EnhancementSlots.Where(x => x.Slot.IsBaseMod()).Single();
                }
                ItemQuality qual = ItemQuality.Premium;
                if (itm.EnhancementSlots.Count() != 0)
                {
                    if (mainMod != null)
                    {
                        if (mainMod.ModificationId != 0)
                        {
                            itm.ModLevel = mainMod.Modification.ItemLevel;
                            qual = mainMod.Modification.Quality;
                        }
                        //else
                        //nothing premium is what we want
                    }
                }
                else
                {
                    itm.ModLevel = itm.ItemLevel;
                    qual = itm.Quality;
                }
                try
                {
                    itm.WeaponMinDamage = itm.Dom_.data.weaponPerLevel.GetStat(itm.WeaponSpec.Id, itm.ModLevel, qual, Stat.MinWeaponDamage);
                    itm.WeaponMaxDamage = itm.Dom_.data.weaponPerLevel.GetStat(itm.WeaponSpec.Id, itm.ModLevel, qual, Stat.MaxWeaponDamage);  //change this so items without barrels use level 1 premium numbers
                    itm.TechPower = (int)itm.Dom_.data.weaponPerLevel.GetStat(itm.WeaponSpec.Id, itm.ModLevel, qual, Stat.TechPowerRating);
                    itm.ForcePower = (int)itm.Dom_.data.weaponPerLevel.GetStat(itm.WeaponSpec.Id, itm.ModLevel, qual, Stat.ForcePowerRating);
                }
                catch (Exception)
                {
                    // string dosomething = ""; //suppress for now, break here to debug
                }
            }
            else if (isEquipable)
            {
                var qual = itm.Quality;
                if (qual == ItemQuality.Moddable) qual = ItemQuality.Prototype;
                ArmorSpec shield = itm.ShieldSpec;
                if (shield != null)
                {
                    List<int> mainSlots = new List<int> { 1, 3, 9 };
                    ItemEnhancement mainMod = null;
                    if (itm.EnhancementSlots != null)
                    {
                        var potentials = itm.EnhancementSlots.Where(x => x.Slot.IsBaseMod());
                        if (potentials.Count() != 0)
                            mainMod = itm.EnhancementSlots.Where(x => x.Slot.IsBaseMod()).Single();
                    }
                    //if (mainMod.Count == 0
                    qual = ItemQuality.Premium;
                    if (itm.EnhancementSlots.Count() != 0)
                    {
                        if (mainMod != null)
                        {
                            if (mainMod.ModificationId != 0)
                            {
                                itm.ModLevel = mainMod.Modification.ItemLevel;
                                qual = mainMod.Modification.Quality;
                            }
                            //else
                            //nothing premium is what we want
                        }
                    }
                    else
                    {
                        itm.ModLevel = itm.ItemLevel;
                        qual = itm.Quality;
                    }
                    try
                    {
                        itm.TechPower = (int)itm.Dom_.data.shieldPerLevel.GetShield(itm.ArmorSpec, qual, itm.ModLevel, Stat.TechPowerRating);
                        itm.ForcePower = (int)itm.Dom_.data.shieldPerLevel.GetShield(itm.ArmorSpec, qual, itm.ModLevel, Stat.ForcePowerRating);
                        itm.AbsorbChance = itm.Dom_.data.shieldPerLevel.GetShield(itm.ArmorSpec, qual, itm.ModLevel, Stat.MeleeShieldAbsorb);
                        itm.ShieldChance = itm.Dom_.data.shieldPerLevel.GetShield(itm.ArmorSpec, qual, itm.ModLevel, Stat.MeleeShieldChance);
                    }
                    catch (Exception)
                    {
                        // string dosomething = ""; //suppress for now, break here to debug
                    }
                }
                ArmorSpec arm = itm.ArmorSpec;
                if (arm != null)
                {
                    var temp = itm.EnhancementSlots.Where(x => x.Slot == EnhancementType.Harness);
                    if (temp.Count() != 0)
                        if (temp.First().ModificationId != 0)
                            itm.ModLevel = temp.First().Modification.ItemLevel;
                    if (arm.DebugSpecName == "adaptive")
                    {
                        List<int> armorValues = new List<int>();
                        if (ArmorSpecs.Count == 0)
                        {
                            foreach (var armorId in ArmorSpecIds)
                            {
                                ArmorSpecs.Add(ArmorSpec.Load(itm.Dom_, armorId));
                            }
                        }
                        foreach (var armorspec in ArmorSpecs)
                        {
                            try
                            {
                                armorValues.Add(itm.Dom_.data.armorPerLevel.GetArmor(armorspec, itm.ModLevel, qual, itm.Slots.Where(x => x != SlotType.Any).First()));
                            }
                            catch (Exception) { } //this is here to catch errors for adaptive implants and such.
                        }
                        itm.AdaptiveArmor = String.Format("[{0}]", String.Join(",", armorValues));
                    }
                    else
                    {
                        itm.Armor = itm.Dom_.data.armorPerLevel.GetArmor(arm, itm.ModLevel, qual, itm.Slots.Where(x => x != SlotType.Any).First());
                    }
                }
            }

            return itm;
        }

        public static Item SimpleTagDicts(Item itm)
        {
            itm.LocalizedName = itm.LocalizedName.SimpleTagLocalizedDict();
            itm.LocalizedRepFactionName = itm.LocalizedRepFactionName.SimpleTagLocalizedDict();
            itm.LocalizedName = itm.LocalizedName.SimpleTagLocalizedDict();
            itm.LocalizedName = itm.LocalizedName.SimpleTagLocalizedDict();
            itm.LocalizedName = itm.LocalizedName.SimpleTagLocalizedDict();

            return itm;
        }

        public static Item NullEmptyLists(Item itm)
        {
            if (itm.StatModifiers.Count == 0)
                itm.StatModifiers = null;
            if (itm.CombinedStatModifiers.Count == 0)
                itm.CombinedStatModifiers = null;
            if (itm.EnhancementSlots.Count == 0)
                itm.EnhancementSlots = null;
            if (itm.RequiredClasses.Count == 0)
                itm.RequiredClasses = null;
            if (itm.StrongholdSourceList.Count == 0)
                itm.StrongholdSourceList = null;
            if (itm.StrongholdSourceNameDict != null && itm.StrongholdSourceNameDict.Count == 0)
                itm.StrongholdSourceNameDict = null;
            if (itm.LocalizedStrongholdSourceNameDict != null && itm.LocalizedStrongholdSourceNameDict.Count == 0)
                itm.LocalizedStrongholdSourceNameDict = null;
            return itm;
        }

        #endregion

        public void AddStat(ItemStat stat)
        {
            AddStat(stat.Stat, stat.Modifier);
        }

        public void AddStat(Stat stat, int modifier)
        {
            var s = this.CombinedStatModifiers.Where(x => x.Stat == stat).FirstOrDefault();
            if (s != null)
            {
                s.Modifier += modifier;
            }
            else
            {
                this.CombinedStatModifiers.Add(new ItemStat
                {
                    Modifier = modifier,
                    Stat = stat
                });
            }
        }

        public override HashSet<string> GetDependencies()
        {
            HashSet<string> returnList = new HashSet<string>
            {
                String.Format("/resources/gfx/icons/{0}.dds", this.Icon)
            };
            var distinct = this.ClassAppearance.Values.Distinct();
            foreach (var distApp in distinct)
            {
                var appearance = Dom_.appearanceLoader.Load(distApp);
                returnList.UnionWith(appearance.GetDependencies());
            }
            return returnList;
        }

        #region IEquatable<Item>
        public override int GetHashCode() //needs fixed, it's changing for the same data
        {
            int hash = Id.GetHashCode();
            if (LocalizedName != null) foreach (var x in LocalizedName) { hash ^= x.GetHashCode(); } //dictionaries need to hashed like this
            if (LocalizedDescription != null) foreach (var x in LocalizedDescription) { hash ^= x.GetHashCode(); } //dictionaries need to hashed like this
            if (AppearanceColor != null) hash ^= AppearanceColor.GetHashCode();
            if (ArmorSpec != null) hash ^= ArmorSpec.GetHashCode();
            hash ^= Binding.GetHashCode();
            hash ^= Category.GetHashCode();
            hash ^= CombinedRating.GetHashCode();
            hash ^= CombinedRequiredLevel.GetHashCode();
            hash ^= ConsumedOnUse.GetHashCode();
            hash ^= DisassembleCategory.GetHashCode();
            hash ^= Durability.GetHashCode();
            hash ^= EnhancementCategory.GetHashCode();
            hash ^= EnhancementSubCategory.GetHashCode();
            hash ^= EnhancementType.GetHashCode();
            hash ^= EquipAbility.GetHashCode();
            hash ^= GiftRank.GetHashCode();
            hash ^= AuctionCategoryId.GetHashCode();
            hash ^= AuctionSubCategoryId.GetHashCode();
            hash ^= GiftType.GetHashCode();
            hash ^= Icon.GetHashCode();
            hash ^= ItemLevel.GetHashCode();
            hash ^= MaxStack.GetHashCode();
            hash ^= ModifierSpec.GetHashCode();
            hash ^= MountSpec.GetHashCode();
            hash ^= Quality.GetHashCode();
            hash ^= Rating.GetHashCode();
            hash ^= RequiredAlignmentInverted.GetHashCode();
            hash ^= RequiredAlignmentTier.GetHashCode();
            hash ^= RequiredGender.GetHashCode();
            hash ^= RequiredLevel.GetHashCode();
            hash ^= RequiredProfession.GetHashCode();
            hash ^= RequiredProfessionLevel.GetHashCode();
            hash ^= RequiredSocialTier.GetHashCode();
            hash ^= RequiredValorRank.GetHashCode();
            hash ^= RequiresAlignment.GetHashCode();
            hash ^= RequiresSocial.GetHashCode();
            hash ^= SchematicId.GetHashCode();
            if (ShieldSpec != null) hash ^= ShieldSpec.GetHashCode();
            hash ^= SubCategory.GetHashCode();
            hash ^= TypeBitSet.GetHashCode();
            hash ^= UniqueLimit.GetHashCode();
            if (UseAbility != null) hash ^= UseAbility.GetHashCode();
            hash ^= Value.GetHashCode();
            hash ^= VendorStackSize.GetHashCode();
            if (WeaponSpec != null) hash ^= WeaponSpec.GetHashCode();
            foreach (var x in CombinedStatModifiers) { hash ^= x.GetHashCode(); }
            foreach (var x in EnhancementSlots) { hash ^= x.GetHashCode(); }
            foreach (var x in RequiredClasses) { hash ^= x.Id.GetHashCode(); }
            foreach (var x in Slots) { hash ^= x.GetHashCode(); }
            //foreach (var x in StatModifiers) { hash ^= x.GetHashCode(); }
            hash ^= StackCount.GetHashCode();
            hash ^= MaxDurability.GetHashCode();
            if (WeaponAppSpec != null) hash ^= WeaponAppSpec.GetHashCode();
            if (Model != null) hash ^= Model.GetHashCode();
            if (VOModulationImperial != null) hash ^= VOModulationImperial.GetHashCode();
            if (VOModulationRepublic != null) hash ^= VOModulationRepublic.GetHashCode();
            hash ^= BindsToSlot.GetHashCode();
            hash ^= RepFactionId.GetHashCode();
            hash ^= SetBonusId.GetHashCode();
            if (MTXRarity != null) hash ^= MTXRarity.GetHashCode();
            return hash;
        }

        public override bool Equals(object obj)
        {
            if (obj == null) return false;

            if (ReferenceEquals(this, obj)) return true;

            if (!(obj is Item itm)) return false;

            return Equals(itm);
        }

        public bool Equals(Item itm)
        {
            if (itm == null) return false;

            if (ReferenceEquals(this, itm)) return true;

            if (this.AppearanceColor != itm.AppearanceColor)
                return false;
            if (this.ArmorSpec != itm.ArmorSpec)
                return false;
            if (this.Binding != itm.Binding)
                return false;
            if (this.Category != itm.Category)
                return false;
            if (this.CombinedRating != itm.CombinedRating)
                return false;
            if (this.CombinedRequiredLevel != itm.CombinedRequiredLevel)
                return false;
            if (!this.CombinedStatModifiers.Equals(itm.CombinedStatModifiers))
                return false;
            if (this.ConsumedOnUse != itm.ConsumedOnUse)
                return false;
            if (this.Conversation != itm.Conversation)
                return false;
            if (this.ConversationFqn != itm.ConversationFqn)
                return false;
            if (this.DamageType != itm.DamageType)
                return false;
            if (this.Description != itm.Description)
                return false;
            if (this.DescriptionId != itm.DescriptionId)
                return false;
            if (this.DisassembleCategory != itm.DisassembleCategory)
                return false;
            if (this.Durability != itm.Durability)
                return false;
            if (this.EnhancementCategory != itm.EnhancementCategory)
                return false;
            if (!this.EnhancementSlots.Equals(itm.EnhancementSlots))
                return false;
            if (this.EnhancementSubCategory != itm.EnhancementSubCategory)
                return false;
            if (this.EnhancementType != itm.EnhancementType)
                return false;
            if (!this.EquipAbility.Equals(itm.EquipAbility))
                return false;
            if (this.EquipAbilityId != itm.EquipAbilityId)
                return false;
            if (this.Fqn != itm.Fqn)
                return false;
            if (this.GiftRank != itm.GiftRank)
                return false;
            if (this.AuctionCategoryId != itm.AuctionCategoryId)
                return false;
            if (this.AuctionSubCategoryId != itm.AuctionSubCategoryId)
                return false;
            if (this.GiftType != itm.GiftType)
                return false;
            if (this.Icon != itm.Icon)
                return false;
            if (this.Id != itm.Id)
                return false;
            if (this.ImperialAppearanceTag != itm.ImperialAppearanceTag)
                return false;
            if (this.VOModulationImperial != itm.VOModulationImperial)
                return false;
            if (this.IsUnknownBool != itm.IsUnknownBool)
                return false;
            if (this.ItemLevel != itm.ItemLevel)
                return false;
            if (this.ItmCraftedCategory != itm.ItmCraftedCategory)
                return false;

            var dComp = new DictionaryComparer<string, string>();
            if (!dComp.Equals(this.LocalizedDescription, itm.LocalizedDescription))
                return false;
            if (!dComp.Equals(this.LocalizedName, itm.LocalizedName))
                return false;

            if (this.MaxDurability != itm.MaxDurability)
                return false;
            if (this.MaxStack != itm.MaxStack)
                return false;
            if (this.Model != itm.Model)
                return false;
            if (this.ModifierSpec != itm.ModifierSpec)
                return false;
            if (this.MountSpec != itm.MountSpec)
                return false;
            if (this.Name != itm.Name)
                return false;
            if (this.NameId != itm.NameId)
                return false;
            if (this.ShortId != itm.ShortId)
                return false;
            if (this.Quality != itm.Quality)
                return false;
            if (this.Rating != itm.Rating)
                return false;
            if (this.RepublicAppearanceTag != itm.RepublicAppearanceTag)
                return false;
            if (this.VOModulationRepublic != itm.VOModulationRepublic)
                return false;
            if (this.RequiredAlignmentInverted != itm.RequiredAlignmentInverted)
                return false;
            if (this.RequiredAlignmentTier != itm.RequiredAlignmentTier)
                return false;
            if (!this.RequiredClasses.Equals(itm.RequiredClasses, false))
                return false;
            if (this.RequiredGender != itm.RequiredGender)
                return false;
            if (this.RequiredLevel != itm.RequiredLevel)
                return false;
            if (this.RequiredProfession != itm.RequiredProfession)
                return false;
            if (this.RequiredProfessionLevel != itm.RequiredProfessionLevel)
                return false;
            if (this.RequiredSocialTier != itm.RequiredSocialTier)
                return false;
            if (this.RequiredValorRank != itm.RequiredValorRank)
                return false;
            if (this.RequiresAlignment != itm.RequiresAlignment)
                return false;
            if (this.RequiresSocial != itm.RequiresSocial)
                return false;
            if (this.Schematic != null)
            {
                if (!this.Schematic.Equals(itm.Schematic))
                    return false;
            }
            else if (itm.Schematic != null)
                return false;
            if (this.SchematicId != itm.SchematicId)
                return false;
            if (this.ShieldSpec != itm.ShieldSpec)
                return false;
            if (!this.Slots.Equals(itm.Slots))
                return false;
            if (this.SoundType != itm.SoundType)
                return false;
            if (this.StackCount != itm.StackCount)
                return false;
            if (!this.StatModifiers.Equals(itm.StatModifiers))
                return false;
            if (!this.StrongholdSourceList.SequenceEqual(itm.StrongholdSourceList))
                return false;
            if (this.SubCategory != itm.SubCategory)
                return false;
            if (this.TeachesRef != itm.TeachesRef)
                return false;
            if (this.TreasurePackageId != itm.TreasurePackageId)
                return false;
            //if (this.TreasurePackageSpec != itm.TreasurePackageSpec)
            //return false;
            if (this.TypeBitSet != itm.TypeBitSet)
                return false;
            if (this.UniqueLimit != itm.UniqueLimit)
                return false;
            if (this.UseAbility != null)
            {
                if (!this.UseAbility.Equals(itm.UseAbility))
                    return false;
            }
            else if (itm.UseAbility != null)
                return false;
            if (this.UseAbilityId != itm.UseAbilityId)
                return false;
            if (this.Value != itm.Value)
                return false;
            if (this.VendorStackSize != itm.VendorStackSize)
                return false;
            if (this.WeaponAppSpec != itm.WeaponAppSpec)
                return false;
            if (this.WeaponSpec != itm.WeaponSpec)
                return false;
            return true;
        }
        #endregion

        #region Output related
        public override string ToString(bool verbose)
        {

            string n = Environment.NewLine;
            var txtFile = new StringBuilder();

            if (!verbose)
            {
                txtFile.Append(Name + ": " + Description.Replace("\u000A", " ") + n);
                //txtFile.Append(Name + ": " + StatModifiers.ToString() + n);
            }
            else
            {
                txtFile.Append("------------------------------------------------------------" + n);
                txtFile.Append("ItemName: " + Name + n);
                txtFile.Append("ItemNodeID: " + ShortId + n);
                txtFile.Append("NameId: " + NameId + n);
                //file.Append("------------------------------------------------------------" + n);
                //file.Append("  Item INFO" + n);
                txtFile.Append("  Item.fqn: " + Fqn + n);
                txtFile.Append("  ItemLevel: " + ItemLevel + n);
                txtFile.Append("  ItemRequiredLevel: " + RequiredLevel + n);
                txtFile.Append("  AppearanceColor: " + AppearanceColor + n);
                txtFile.Append("  Description: " + Description.Replace("\u000A", " ") + n);
                txtFile.Append("  Icon: " + Icon + n);
                txtFile.Append("  ArmorSpec: " + ArmorSpec + n);
                txtFile.Append("  MaxStack: " + MaxStack + n);
                txtFile.Append("  Bindtype: " + Binding + n);
                txtFile.Append("  ModifierSpec: " + ModifierSpec + n);
                txtFile.Append("  Quality: " + Quality + n);
                txtFile.Append("  Rating: " + Rating + n);
                txtFile.Append("  RequiredAlignmentInverte: " + RequiredAlignmentInverted + n);
                txtFile.Append("  RequiredAlignmentTier: " + RequiredAlignmentTier + n);
                txtFile.Append("  RequiredClasses: " + RequiredClasses + n);
                txtFile.Append("  RequiredGender: " + RequiredGender + n);
                txtFile.Append("  RequiredProfession: " + RequiredProfession + n);
                txtFile.Append("  RequiredProfessionLevel: " + RequiredProfessionLevel + n);
                txtFile.Append("  RequiredSocialTier: " + RequiredSocialTier + n);
                txtFile.Append("  RequiredValorRank: " + RequiredValorRank + n);
                txtFile.Append("  RequiresAlignment: " + RequiresAlignment + n);
                txtFile.Append("  RequiresSocial: " + RequiresSocial + n);
                txtFile.Append("  Schematic: " + Schematic + n);
                txtFile.Append("  SchematicId: " + SchematicId + n);
                txtFile.Append("  ShieldSpec: " + ShieldSpec + n);
                txtFile.Append("  Slots: " + Slots + n);
                txtFile.Append("  StatModifiers: " + StatModifiers + n);
                txtFile.Append("  SubCategory: " + SubCategory + n);
                txtFile.Append("  TreasurePackageId: " + TreasurePackageId + n);
                //txtFile.Append("  TreasurePackageSpec: " + TreasurePackageSpec + n);
                txtFile.Append("  TypeBitSet: " + TypeBitSet + n);
                txtFile.Append("  UniqueLimit: " + UniqueLimit + n);
                txtFile.Append("  UseAbility: " + UseAbility + n);
                txtFile.Append("  UseAbilityId: " + UseAbilityId + n);
                txtFile.Append("  Value: " + Value + n);
                txtFile.Append("  VendorStackSize: " + VendorStackSize + n);
                txtFile.Append("  WeaponSpec: " + WeaponSpec + n);
                txtFile.Append("------------------------------------------------------------" + n + n);
            }
            return txtFile.ToString();
        }
        public override List<SQLProperty> SQLProperties
        {
            get
            {
                if (Id != 0)
                {
                    FillFlatData(this);
                    //SimpleTagDicts(this);
                }
                return new List<SQLProperty>
                    {                //(SQL Column Name, C# Property Name, SQL Column type statement, IsUnique/PrimaryKey, Serialize value to json)
                        new SQLProperty("Name", "Name", "varchar(255) COLLATE utf8_unicode_ci NOT NULL", SQLPropSetting.AddIndex),
                        new SQLProperty("FrName", "LocalizedName[frMale]", "varchar(255) COLLATE utf8_unicode_ci NOT NULL", SQLPropSetting.AddIndex),
                        new SQLProperty("DeName", "LocalizedName[deMale]", "varchar(255) COLLATE utf8_unicode_ci NOT NULL", SQLPropSetting.AddIndex),
                        new SQLProperty("CleanName", "CleanName", "varchar(255) COLLATE utf8_unicode_ci NOT NULL", SQLPropSetting.AddIndex),
                        new SQLProperty("Base62Id", "Base62Id", "varchar(7) COLLATE latin1_general_cs NOT NULL", SQLPropSetting.PrimaryKey),
                        new SQLProperty("Fqn", "Fqn", "varchar(255) COLLATE utf8_unicode_ci NOT NULL"),
                        new SQLProperty("ItemLevel", "ItemLevel", "int(11) NOT NULL", SQLPropSetting.AddIndex),
                        new SQLProperty("AppearanceImperial", "AppearanceImperial", "varchar(7) COLLATE latin1_general_cs NOT NULL"),
                        new SQLProperty("AppearanceRepublic", "AppearanceRepublic", "varchar(7) COLLATE latin1_general_cs NOT NULL"),
                        new SQLProperty("AuctionCategoryId", "AuctionCategoryId", "int(11) NOT NULL"),
                        new SQLProperty("AuctionCategory", "AuctionCategory.LocalizedName[enMale]", "varchar(255) COLLATE utf8_unicode_ci NOT NULL", SQLPropSetting.AddIndex),
                        new SQLProperty("FrAuctionCategory", "AuctionCategory.LocalizedName[frMale]", "varchar(255) COLLATE utf8_unicode_ci NOT NULL", SQLPropSetting.AddIndex),
                        new SQLProperty("DeAuctionCategory", "AuctionCategory.LocalizedName[deMale]", "varchar(255) COLLATE utf8_unicode_ci NOT NULL", SQLPropSetting.AddIndex),
                        new SQLProperty("AuctionSubCategoryId", "AuctionSubCategoryId", "int(11) NOT NULL"),
                        new SQLProperty("AuctionSubCategory", "AuctionSubCategory.LocalizedName[enMale]", "varchar(255) COLLATE utf8_unicode_ci NOT NULL"),
                        new SQLProperty("FrAuctionSubCategory", "AuctionSubCategory.LocalizedName[frMale]", "varchar(255) COLLATE utf8_unicode_ci NOT NULL"),
                        new SQLProperty("DeAuctionSubCategory", "AuctionSubCategory.LocalizedName[deMale]", "varchar(255) COLLATE utf8_unicode_ci NOT NULL"),
                        new SQLProperty("Binding", "Binding", "varchar(255) COLLATE utf8_unicode_ci NOT NULL"),
                        new SQLProperty("CombinedRating", "CombinedRating", "int(11) NOT NULL", SQLPropSetting.AddIndex),
                        new SQLProperty("CombinedRequiredLevel", "CombinedRequiredLevel", "int(11) NOT NULL", SQLPropSetting.AddIndex),
                        new SQLProperty("ConsumedOnUse", "ConsumedOnUse", "tinyint(1) NOT NULL"),
                        new SQLProperty("DisassembleCategory", "DisassembleCategory", "varchar(255) COLLATE utf8_unicode_ci NOT NULL"),
                        new SQLProperty("Durability", "Durability", "int(11) NOT NULL"),
                        new SQLProperty("EnhancementCategory", "EnhancementCategory", "varchar(255) COLLATE utf8_unicode_ci NOT NULL"),
                        new SQLProperty("EnhancementSubCategory", "EnhancementSubCategory", "varchar(255) COLLATE utf8_unicode_ci NOT NULL"),
                        new SQLProperty("EnhancementType", "EnhancementType", "varchar(255) COLLATE utf8_unicode_ci NOT NULL"),
                        new SQLProperty("EquipAbilityId", "EquipAbilityB62Id", "varchar(7) COLLATE latin1_general_cs NOT NULL"),
                        new SQLProperty("GiftRank", "GiftRankNum", "int(11) NOT NULL"),
                        new SQLProperty("GiftType", "GiftType", "varchar(255) COLLATE utf8_unicode_ci NOT NULL"),
                        new SQLProperty("Icon", "HashedIcon", "varchar(255) COLLATE utf8_unicode_ci NOT NULL"),
                        new SQLProperty("IsDecoration", "IsDecoration", "tinyint(1) NOT NULL"),
                        new SQLProperty("BindsToSlot", "BindsToSlot", "tinyint(1) NOT NULL"),
                        new SQLProperty("MaxDurability", "MaxDurability", "int(11) NOT NULL"),
                        new SQLProperty("MaxStack", "MaxStack", "int(11) NOT NULL"),
                        new SQLProperty("MountSpec", "MountSpec", "varchar(255) COLLATE utf8_unicode_ci NOT NULL"),
                        new SQLProperty("MTXRarity","MTXRarity", "varchar(15) COLLATE utf8_unicode_ci NOT NULL", SQLPropSetting.AddIndex),
                        new SQLProperty("Quality", "Quality", "varchar(255) COLLATE utf8_unicode_ci NOT NULL", SQLPropSetting.AddIndex),
                        new SQLProperty("Rating", "Rating", "int(11) NOT NULL", SQLPropSetting.AddIndex),
                        new SQLProperty("RequiredAlignmentInverted", "RequiredAlignmentInverted", "tinyint(1) NOT NULL"),
                        new SQLProperty("RequiredClasses", "RequiredClasses", "text NOT NULL"),
                        new SQLProperty("RequiredGender", "RequiredGender", "varchar(10) COLLATE utf8_unicode_ci NOT NULL", SQLPropSetting.AddIndex),
                        new SQLProperty("RequiredLevel", "RequiredLevel", "int(11) NOT NULL"),
                        new SQLProperty("RequiredProfession", "RequiredProfession", "varchar(35) COLLATE utf8_unicode_ci NOT NULL"),
                        new SQLProperty("RequiredProfessionLevel", "RequiredProfessionLevel", "int(11) NOT NULL"),
                        new SQLProperty("RequiredSocialTier", "RequiredSocialTier", "int(11) NOT NULL", SQLPropSetting.AddIndex),
                        new SQLProperty("RequiredValorRank", "RequiredValorRank", "int(11) NOT NULL", SQLPropSetting.AddIndex),
                        new SQLProperty("RequiresAlignment", "RequiresAlignment", "tinyint(1) NOT NULL"),
                        new SQLProperty("RequiresSocial", "RequiresSocial", "tinyint(1) NOT NULL"),
                        new SQLProperty("RequiredReputationId", "RequiredReputationId", "int(11) NOT NULL", SQLPropSetting.AddIndex),
                        new SQLProperty("RequiredReputationLevelId", "RequiredReputationLevelId", "int(11) NOT NULL"),
                        new SQLProperty("RequiredReputation", "RequiredReputationName", "varchar(1000) NOT NULL"),
                        new SQLProperty("RequiredReputationLevel", "RequiredReputationLevelName", "varchar(255) NOT NULL"),
                        new SQLProperty("SchematicId", "SchematicB62Id", "varchar(7) COLLATE latin1_general_cs NOT NULL", SQLPropSetting.AddIndex),
                        new SQLProperty("ShieldSpec", "ShieldSpec", "varchar(255) COLLATE utf8_unicode_ci NOT NULL"),
                        new SQLProperty("StackCount", "StackCount", "int(11) NOT NULL"),
                        new SQLProperty("TeachesRef", "TeachesRefB62", "varchar(7) COLLATE latin1_general_cs NOT NULL"),
                        new SQLProperty("TeachesType", "TeachesType", "varchar(255) COLLATE utf8_unicode_ci NOT NULL"),
                        new SQLProperty("TreasurePackageId", "TreasurePackageId", "varchar(255) COLLATE utf8_unicode_ci NOT NULL"),
                        new SQLProperty("TypeBitSet", "TypeBitSet", "int(11) NOT NULL"),
                        new SQLProperty("UniqueLimit", "UniqueLimit", "int(11) NOT NULL"),
                        new SQLProperty("UseAbilityId", "UseAbilityB62Id", "varchar(7) COLLATE latin1_general_cs NOT NULL"),
                        new SQLProperty("Value", "Value", "int(11) NOT NULL"),
                        new SQLProperty("VendorStackSize", "VendorStackSize", "int(11) NOT NULL"),
                        new SQLProperty("VOModulationImperial", "VOModulationImperial", "varchar(255) COLLATE utf8_unicode_ci NOT NULL"),
                        new SQLProperty("VOModulationRepublic", "VOModulationRepublic", "varchar(255) COLLATE utf8_unicode_ci NOT NULL"),
                        new SQLProperty("WeaponAppSpec", "WeaponAppSpec", "varchar(255) COLLATE utf8_unicode_ci NOT NULL"),
                        new SQLProperty("WeaponSpec", "WeaponSpec", "varchar(255) COLLATE utf8_unicode_ci NOT NULL"),
                        new SQLProperty("ChildBase62Id", "ChildBase62Id", "varchar(7) COLLATE latin1_general_cs NOT NULL"),
                        /* Flat Data */
                        //new SQLProperty("Tooltip", "Tooltip", "TEXT NOT NULL"),
                        new SQLProperty("ArmoringBId", "ArmoringBId", "varchar(7) COLLATE latin1_general_cs NOT NULL", SQLPropSetting.AddIndex),
                        new SQLProperty("ModificationBId", "ModificationBId", "varchar(7) COLLATE latin1_general_cs NOT NULL", SQLPropSetting.AddIndex),
                        new SQLProperty("EnhancementBId", "EnhancementBId", "varchar(7) COLLATE latin1_general_cs NOT NULL", SQLPropSetting.AddIndex),
                        new SQLProperty("ColorCrystalBId", "ColorCrystalBId", "varchar(7) COLLATE latin1_general_cs NOT NULL", SQLPropSetting.AddIndex),
                        new SQLProperty("BarrelBId", "BarrelBId", "varchar(7) COLLATE latin1_general_cs NOT NULL", SQLPropSetting.AddIndex),
                        new SQLProperty("HiltBId", "HiltBId", "varchar(7) COLLATE latin1_general_cs NOT NULL", SQLPropSetting.AddIndex),
                        new SQLProperty("Endurance","Endurance", "int(11) NOT NULL", SQLPropSetting.AddIndex),
                        new SQLProperty("Presence","Presence", "int(11) NOT NULL", SQLPropSetting.AddIndex),
                        new SQLProperty("Aim","Aim", "int(11) NOT NULL"),
                        new SQLProperty("Cunning","Cunning", "int(11) NOT NULL"),
                        new SQLProperty("Strength","Strength", "int(11) NOT NULL"),
                        new SQLProperty("Willpower","Willpower", "int(11) NOT NULL"),
                        new SQLProperty("Mastery","Mastery", "int(11) NOT NULL", SQLPropSetting.AddIndex),
                        new SQLProperty("AbsorptionRating","AbsorptionRating", "int(11) NOT NULL", SQLPropSetting.AddIndex),
                        new SQLProperty("CriticalRating","CriticalRating", "int(11) NOT NULL", SQLPropSetting.AddIndex),
                        new SQLProperty("DefenseRating","DefenseRating", "int(11) NOT NULL", SQLPropSetting.AddIndex),
                        new SQLProperty("Power","Power", "int(11) NOT NULL", SQLPropSetting.AddIndex),
                        new SQLProperty("AccuracyRating","AccuracyRating", "int(11) NOT NULL", SQLPropSetting.AddIndex),
                        new SQLProperty("AlacrityRating","AlacrityRating", "int(11) NOT NULL", SQLPropSetting.AddIndex),
                        new SQLProperty("ShieldRating","ShieldRating", "int(11) NOT NULL", SQLPropSetting.AddIndex),
                        new SQLProperty("SurgeRating","SurgeRating", "int(11) NOT NULL"),
                        new SQLProperty("ExpertiseRating","ExpertiseRating", "int(11) NOT NULL", SQLPropSetting.AddIndex),
                        new SQLProperty("ForcePower","ForcePower", "int(11) NOT NULL"),
                        new SQLProperty("TechPower","TechPower", "int(11) NOT NULL"),
                        new SQLProperty("TechHealingPower","TechHealingPower", "int(11) NOT NULL"),
                        new SQLProperty("ForceHealingPower","ForceHealingPower", "int(11) NOT NULL"),
                        new SQLProperty("Armor", "Armor", "int(11) NOT NULL"),
                        //new SQLProperty("AdaptiveArmor", "AdaptiveArmor", "varchar(25) COLLATE utf8_unicode_ci NOT NULL"),
                        new SQLProperty("WeaponMinDamage", "WeaponMinDamage", "float(10,1) NOT NULL"),
                        new SQLProperty("WeaponMaxDamage", "WeaponMaxDamage", "float(10,1) NOT NULL"),
                        new SQLProperty("AbsorbChance", "AbsorbChance", "float(10,1) NOT NULL"),
                        new SQLProperty("ShieldChance", "ShieldChance", "float(10,1) NOT NULL"),
                        new SQLProperty("ModLevel", "ModLevel", "int(11) NOT NULL"),
                        //typebitset
                        new SQLProperty("IsArmor", "TypeBitFlags.IsArmor", "tinyint(1) NOT NULL"),
                        new SQLProperty("IsWeapon", "TypeBitFlags.IsWeapon", "tinyint(1) NOT NULL"),
                        new SQLProperty("HasGTNCategory", "TypeBitFlags.HasGTNCategory", "tinyint(1) NOT NULL", SQLPropSetting.AddIndex),
                        new SQLProperty("Unk8", "TypeBitFlags.Unk8", "tinyint(1) NOT NULL"),
                        new SQLProperty("HasConversation", "TypeBitFlags.HasConversation", "tinyint(1) NOT NULL"),
                        new SQLProperty("IsCrafted", "TypeBitFlags.IsCrafted", "tinyint(1) NOT NULL", SQLPropSetting.AddIndex),
                        new SQLProperty("CanBeDisassembled", "TypeBitFlags.CanBeDisassembled", "tinyint(1) NOT NULL", SQLPropSetting.AddIndex),
                        new SQLProperty("HasDurability", "TypeBitFlags.HasDurability", "tinyint(1) NOT NULL", SQLPropSetting.AddIndex),
                        new SQLProperty("IsModdable", "TypeBitFlags.IsModdable", "tinyint(1) NOT NULL", SQLPropSetting.AddIndex),
                        new SQLProperty("IsMod", "TypeBitFlags.IsMod", "tinyint(1) NOT NULL"),
                        new SQLProperty("CanHaveStats", "TypeBitFlags.CanHaveStats", "tinyint(1) NOT NULL", SQLPropSetting.AddIndex),
                        new SQLProperty("Unk800", "TypeBitFlags.Unk800", "tinyint(1) NOT NULL"),
                        new SQLProperty("IsGift", "TypeBitFlags.IsGift", "tinyint(1) NOT NULL"),
                        new SQLProperty("IsMissionItem", "TypeBitFlags.IsMissionItem", "tinyint(1) NOT NULL"),
                        new SQLProperty("Unk4000", "TypeBitFlags.Unk4000", "tinyint(1) NOT NULL"),
                        new SQLProperty("IsShipPart", "TypeBitFlags.IsShipPart", "tinyint(1) NOT NULL"),
                        new SQLProperty("Unk10000", "TypeBitFlags.Unk10000", "tinyint(1) NOT NULL"),
                        new SQLProperty("IsCmpCstmztn", "TypeBitFlags.IsCmpCstmztn", "tinyint(1) NOT NULL"),
                        new SQLProperty("HasUniqueLimit", "TypeBitFlags.HasUniqueLimit", "tinyint(1) NOT NULL", SQLPropSetting.AddIndex),
                        new SQLProperty("HasOnUse", "TypeBitFlags.HasOnUse", "tinyint(1) NOT NULL", SQLPropSetting.AddIndex),
                        new SQLProperty("IsEquipable", "TypeBitFlags.IsEquipable", "tinyint(1) NOT NULL", SQLPropSetting.AddIndex),
                        new SQLProperty("IsCurrency", "TypeBitFlags.IsCurrency", "tinyint(1) NOT NULL"),
                        new SQLProperty("IsMtxItem", "TypeBitFlags.IsMtxItem", "tinyint(1) NOT NULL", SQLPropSetting.AddIndex),
                        new SQLProperty("IsRepTrophy", "TypeBitFlags.IsRepTrophy", "tinyint(1) NOT NULL", SQLPropSetting.AddIndex),
                        //Slots
                        new SQLProperty("Any", "Any", "tinyint(1) NOT NULL"),
                        new SQLProperty("EquipHumanEar", "EquipHumanEar", "tinyint(1) NOT NULL", SQLPropSetting.AddIndex),
                        new SQLProperty("EquipHumanMainHand", "EquipHumanMainHand", "tinyint(1) NOT NULL", SQLPropSetting.AddIndex),
                        new SQLProperty("EquipHumanOffHand", "EquipHumanOffHand", "tinyint(1) NOT NULL", SQLPropSetting.AddIndex),
                        new SQLProperty("EquipHumanLeg", "EquipHumanLeg", "tinyint(1) NOT NULL", SQLPropSetting.AddIndex),
                        new SQLProperty("EquipHumanRangedPrimary", "EquipHumanRangedPrimary", "tinyint(1) NOT NULL", SQLPropSetting.AddIndex),
                        new SQLProperty("EquipHumanShield", "EquipHumanShield", "tinyint(1) NOT NULL", SQLPropSetting.AddIndex),
                        new SQLProperty("EquipHumanFoot", "EquipHumanFoot", "tinyint(1) NOT NULL", SQLPropSetting.AddIndex),
                        new SQLProperty("EquipHumanImplant", "EquipHumanImplant", "tinyint(1) NOT NULL", SQLPropSetting.AddIndex),
                        new SQLProperty("EquipHumanChest", "EquipHumanChest", "tinyint(1) NOT NULL", SQLPropSetting.AddIndex),
                        new SQLProperty("EquipHumanGlove", "EquipHumanGlove", "tinyint(1) NOT NULL", SQLPropSetting.AddIndex),
                        new SQLProperty("EquipHumanCustomRanged", "EquipHumanCustomRanged", "tinyint(1) NOT NULL", SQLPropSetting.AddIndex),
                        new SQLProperty("EquipHumanFace", "EquipHumanFace", "tinyint(1) NOT NULL", SQLPropSetting.AddIndex),
                        new SQLProperty("EquipHumanBelt", "EquipHumanBelt", "tinyint(1) NOT NULL", SQLPropSetting.AddIndex),
                        new SQLProperty("EquipHumanRangedSecondary", "EquipHumanRangedSecondary", "tinyint(1) NOT NULL", SQLPropSetting.AddIndex),
                        new SQLProperty("EquipHumanWrist", "EquipHumanWrist", "tinyint(1) NOT NULL", SQLPropSetting.AddIndex),
                        new SQLProperty("EquipHumanCustomMelee", "EquipHumanCustomMelee", "tinyint(1) NOT NULL", SQLPropSetting.AddIndex),
                        new SQLProperty("EquipHumanRelic", "EquipHumanRelic", "tinyint(1) NOT NULL", SQLPropSetting.AddIndex),
                        new SQLProperty("EquipDroidSensor", "EquipDroidSensor", "tinyint(1) NOT NULL"),
                        new SQLProperty("EquipHumanOutfit", "EquipHumanOutfit", "tinyint(1) NOT NULL"),
                        new SQLProperty("EquipDroidUpper", "EquipDroidUpper", "tinyint(1) NOT NULL"),
                        new SQLProperty("EquipDroidUtility", "EquipDroidUtility", "tinyint(1) NOT NULL"),
                        new SQLProperty("EquipDroidLower", "EquipDroidLower", "tinyint(1) NOT NULL"),
                        new SQLProperty("EquipSpaceShipArmor", "EquipSpaceShipArmor", "tinyint(1) NOT NULL"),
                        new SQLProperty("EquipSpaceShieldRegenerator", "EquipSpaceShieldRegenerator", "tinyint(1) NOT NULL"),
                        new SQLProperty("EquipSpaceProtonTorpedoes", "EquipSpaceProtonTorpedoes", "tinyint(1) NOT NULL"),
                        new SQLProperty("EffectOther", "EffectOther", "tinyint(1) NOT NULL"),
                        new SQLProperty("EquipSpaceBeamGenerator", "EquipSpaceBeamGenerator", "tinyint(1) NOT NULL"),
                        new SQLProperty("Upgrade", "Upgrade", "tinyint(1) NOT NULL"),
                        new SQLProperty("EquipHumanHeirloom", "EquipHumanHeirloom", "tinyint(1) NOT NULL"),
                        new SQLProperty("EquipSpaceBeamCharger", "EquipSpaceBeamCharger", "tinyint(1) NOT NULL"),
                        new SQLProperty("EquipSpaceAbilityDefense", "EquipSpaceAbilityDefense", "tinyint(1) NOT NULL"),
                        new SQLProperty("EquipSpaceMissileMagazine", "EquipSpaceMissileMagazine", "tinyint(1) NOT NULL"),
                        new SQLProperty("EquipSpaceEnergyShield", "EquipSpaceEnergyShield", "tinyint(1) NOT NULL"),
                        new SQLProperty("EquipSpaceAbilitySystems", "EquipSpaceAbilitySystems", "tinyint(1) NOT NULL"),
                        new SQLProperty("EquipDroidShield", "EquipDroidShield", "tinyint(1) NOT NULL"),
                        new SQLProperty("EquipDroidSpecial", "EquipDroidSpecial", "tinyint(1) NOT NULL"),
                        new SQLProperty("EquipDroidWeapon1", "EquipDroidWeapon1", "tinyint(1) NOT NULL"),
                        new SQLProperty("EquipDroidWeapon2", "EquipDroidWeapon2", "tinyint(1) NOT NULL"),
                        new SQLProperty("EquipSpaceAbilityOffense", "EquipSpaceAbilityOffense", "tinyint(1) NOT NULL"),
                        new SQLProperty("QuestTracker", "QuestTracker", "tinyint(1) NOT NULL"),

            };
            }
        }

        public bool HasRocketFX { get; set; }
        public bool ImperialRocketFX { get; set; }
        [JsonIgnore]
        public HashSet<string> ImperialAttachedFX_ { get; set; }
        public HashSet<string> ImperialAttachedFX
        {
            get
            {
                return ImperialAttachedFX_;
            }
            set
            {
                ImperialAttachedFX_ = value;
                foreach (var str in value)
                {
                    if (str.StartsWith("vfx_jetpackthrust"))
                    {
                        ImperialRocketFX = true;
                        HasRocketFX = true;
                        break;
                    }
                }
            }
        }
        public bool RepublicRocketFX { get; set; }
        [JsonIgnore]
        public HashSet<string> RepublicAttachedFX_ { get; set; }
        public HashSet<string> RepublicAttachedFX
        {
            get
            {
                return RepublicAttachedFX_;
            }
            set
            {
                RepublicAttachedFX_ = value;
                foreach (var str in value)
                {
                    if (str.StartsWith("vfx_jetpackthrust"))
                    {
                        RepublicRocketFX = true;
                        HasRocketFX = true;
                        break;
                    }
                }
            }
        }

        public override XElement ToXElement(bool verbose)
        {
            XElement item = new XElement("Item",
                new XElement("Fqn", Fqn),
                new XAttribute("Id", Id),
                //new XAttribute("Hash", GetHashCode()),
                new XElement("Name", Name),
                new XElement("Description", Description));
            item.Add(new XElement("Base62Id", Base62Id));
            if (UseAbilityId == 0) { item.Add(new XElement("UseAbility")); }
            else { item.Add(new XElement("UseAbility", UseAbility.ToXElement(verbose))); }
            if (EquipAbilityId == 0) { item.Add(new XElement("EquipAbility")); }
            else { item.Add(new XElement("EquipAbility", EquipAbility.ToXElement(verbose))); }

            if (verbose)
            {
                /*item.Element("Name").RemoveAll();
                for (int i = 0; i < localizations.Count; i++)
                {
                    if (LocalizedName[localizations[i]] != "")
                    {
                        item.Element("Name").Add(new XElement(localizations[i], LocalizedName[localizations[i]]));
                    }
                }

                item.Element("Description").RemoveAll();
                for (int i = 0; i < localizations.Count; i++)
                {
                    if (LocalizedDescription[localizations[i]] != "")
                    {
                        item.Element("Description").Add(new XElement(localizations[i], LocalizedDescription[localizations[i]]));
                    }
                }*/
                item.Element("Name").Add(new XAttribute("Id", NameId));
                item.Element("Description").Add(new XAttribute("Id", DescriptionId));
                item.Element("Fqn").Add(new XAttribute("Id", ShortId));
                //item.Element("EquipAbility").Add(new XAttribute("Id", EquipAbilityId));
                //item.Element("UseAbility").Add(new XAttribute("Id", UseAbilityId));
                item.Add(new XElement("Icon", Icon),
                    new XElement("SoundPackage", SoundType),
                    new XElement("Model", Model),
                    new XElement("ItemLevel", ItemLevel),
                    new XElement("RequiredLevel", RequiredLevel),
                    new XElement("CombinedRequiredLevel", CombinedRequiredLevel),
                    new XElement("ArmorSpec", ArmorSpec),
                    new XElement("Binding", Binding),
                    new XElement("CombinedRating", CombinedRating),
                    new XElement("ConsumedOnUse", ConsumedOnUse),
                    new XElement("AppearanceColor", AppearanceColor));
                if (ConversationFqn != null)
                {
                    item.Add(new GameObject().ToXElement(ConversationFqn, Dom_, false));
                }
                else
                {
                    item.Add(new XElement("Conversation"));
                }
                item.Add(new XElement("DamageType", DamageType),
                    new XElement("DisassembleCategory", DisassembleCategory),
                    new XElement("Durability", Durability));
                XElement enhancements = new XElement("Enhancements",
                        new XElement("Category", EnhancementCategory),
                        new XElement("SubCategory", EnhancementSubCategory));
                foreach (var enhancement in EnhancementSlots)
                {
                    enhancements.Add(new XElement(enhancement.Slot.ToString(),
                        new XElement("Modification", enhancement.Modification,
                            new XAttribute("Id", enhancement.ModificationId.ToString()))));
                }
                item.Add(enhancements,
                    new XElement("StatModifiers", StatModifiers.ToString()),
                    new XElement("CombinedStatModifiers", CombinedStatModifiers.ToString()),
                    new XElement("EnhancementType", EnhancementType),
                    new XElement("GiftType", GiftType,
                        new XAttribute("GiftRank", GiftRank)),
                    new XElement("IsModdable", TypeBitFlags.IsModdable),
                    new XElement("MaxStack", MaxStack),
                    new XElement("ModifierSpec", ModifierSpec),
                    new XElement("MountSpec", MountSpec),
                    new XElement("Quality", Quality),
                    new XElement("Rating", Rating),
                    new XElement("RequiredAlignmentInverted", RequiredAlignmentInverted));

                XElement requirements = new XElement("Requirements");
                string reqclasses = null;
                foreach (var reqclass in RequiredClasses)
                {
                    reqclasses += reqclass.Name.ToString() + ", ";
                }
                if (reqclasses != null) { reqclasses = reqclasses.Substring(0, reqclasses.Length - 2); }
                requirements.Add(new XElement("Classes", reqclasses));
                requirements.Add(new XElement("Gender", RequiredGender),
                    new XElement("Profession", RequiredProfession,
                        new XAttribute("Level", RequiredProfessionLevel)));
                if (RequiresSocial)
                {
                    requirements.Add(new XElement("Social", RequiredSocialTier));
                }
                else
                {
                    requirements.Add(new XElement("Social"));
                }
                requirements.Add(new XElement("ValorRank", RequiredValorRank),
                    new XElement("Alignment", RequiresAlignment));
                item.Add(requirements);
                item.Add((Schematic ?? new Schematic()).ToXElement(false));
                item.Add(new XElement("ShieldSpec", ShieldSpec),
                    new XElement("Slots", Slots),
                    new XElement("SubCategory", SubCategory),
                    (this.AuctionCategory != null) ? new XElement("AuctionCat", AuctionCategory.Name) : new XElement("AuctionCat"),
                    (this.AuctionSubCategory != null) ? new XElement("AuctionSubCat", AuctionSubCategory.Name) : new XElement("AuctionSubCat"),
                    //new XElement("TreasurePackageSpec", TreasurePackageSpec,
                    //new XAttribute("Id", TreasurePackageId)),
                    new XElement("TypeBitSet", TypeBitSet),
                    /*   new XAttribute("Id", TypeBitSet),
                       ((GomLib.Models.ItemTypeFlags)(object)TypeBitSet)
                       .ToString()
                       .Replace(" ", "")
                       .Split(',')
                       .ToList()
                       .Select(x => new XElement("Flag", x))), */
                    new XElement("UniqueLimit", UniqueLimit),
                    new XElement("Value", Value),
                    new XElement("VendorStackSize", VendorStackSize),
                    new XElement("WeaponSpec", WeaponSpec));
                if (VOModulationImperial != null)
                {
                    item.Add(new XElement("ImpVoiceModulation", VOModulationImperial),
                        new XElement("RepublicVoiceModulation", VOModulationRepublic));
                }
                if (ImperialAppearanceTag != null)
                {
                    item.Add(new XElement("ImperialAppearanceTag", ImperialAppearanceTag),
                        new XElement("RepublicAppearanceTag", RepublicAppearanceTag));
                }
                if (ClassAppearance != null)
                    item.Add(new XElement("ClassAppearances", ClassAppearance.Select(x => new XElement(x.Key, x.Value))));
            }
            else
            {
                item.Elements().Where(x => x.IsEmpty).Remove();
            }

            /*if (ExportICONS)
            {
                OutputSchematicIcon(Icon);
            }*/
            return item;
        }

        public override string ToJSON()
        {
            //SimpleTagDicts(this);
            NullEmptyLists(this);
            JsonSerializerSettings settings = new JsonSerializerSettings
            {
                NullValueHandling = NullValueHandling.Ignore
            };
            return ToJSON(settings);
        }
        #endregion
    }

    public class ItemStatList : List<ItemStat>, IEquatable<ItemStatList>
    {
        public ItemStatList() : base() { }
        public ItemStatList(IEnumerable<ItemStat> collection) : base(collection) { }
        public override string ToString()
        {
            if (this == null) { return "null"; }
            if (this.Count <= 0) { return "Empty List"; }
            string retVal = "";
            foreach (ItemStat i in this) { retVal += string.Format("{0},", i); }
            return retVal;
        }

        public override int GetHashCode()
        {
            int hash = 0;
            foreach (var x in this) { hash ^= x.GetHashCode(); }
            return hash;
        }

        public override bool Equals(object obj)
        {
            if (obj == null) return false;

            if (ReferenceEquals(this, obj)) return true;

            ItemStatList itmSList = obj as ItemStatList;
            return Equals(itmSList);
        }

        public bool Equals(ItemStatList itmSList)
        {
            if (itmSList == null) return false;

            if (!Enumerable.SequenceEqual<ItemStat>(this, itmSList)) return false;
            return true;
        }
    }
    public class ItemEnhancementList : List<ItemEnhancement>, IEquatable<ItemEnhancementList>
    {
        public ItemEnhancementList() : base() { }
        public ItemEnhancementList(IEnumerable<ItemEnhancement> collection) : base(collection) { }
        public override string ToString()
        {
            if (this == null) { return "null"; }
            if (this.Count <= 0) { return "Empty List"; }
            string retVal = "";
            foreach (ItemEnhancement i in this) { retVal += string.Format("{0}, ", i); }
            return retVal;
        }

        public override int GetHashCode()
        {
            int hash = 0;
            foreach (var x in this) { hash ^= x.GetHashCode(); }
            return hash;
        }

        public override bool Equals(object obj)
        {
            if (obj == null) return false;

            if (ReferenceEquals(this, obj)) return true;

            ItemEnhancementList itmEList = obj as ItemEnhancementList;
            return Equals(itmEList);
        }

        public bool Equals(ItemEnhancementList itmEList)
        {
            if (itmEList == null) return false;

            if (!Enumerable.SequenceEqual<ItemEnhancement>(this, itmEList))
                return false;
            return true;
        }
    }
    public class ClassSpecList : List<ClassSpec>, IEquatable<ClassSpecList>
    {
        public ClassSpecList() : base() { }
        public ClassSpecList(IEnumerable<ClassSpec> collection) : base(collection) { }
        public override string ToString()
        {
            if (this == null) { return "null"; }
            if (this.Count <= 0) { return "Empty List"; }
            string retVal = "";
            foreach (ClassSpec i in this) { retVal += string.Format("{0}, ", i); }
            return retVal;
        }

        public override int GetHashCode()
        {
            int hash = 0.GetHashCode();
            if (this != null) foreach (var x in this) { hash ^= x.GetHashCode(); }
            return hash;
        }

        public override bool Equals(object obj)
        {
            if (obj == null) return false;

            if (ReferenceEquals(this, obj)) return true;

            ClassSpecList itmCSList = obj as ClassSpecList;
            return Equals(itmCSList);
        }

        public bool Equals(ClassSpecList itmCSList)
        {
            return Equals(itmCSList, true);
        }

        public bool Equals(ClassSpecList itmCSList, bool compareAbilityPackage)
        {
            if (itmCSList == null) return false;
            if (compareAbilityPackage)
            {
                if (!Enumerable.SequenceEqual<ClassSpec>(this, itmCSList)) return false;
            }
            else
            {
                foreach (var classpec in this)
                {
                    var cmp = itmCSList.SingleOrDefault(x => x.Fqn == classpec.Fqn);
                    if (cmp != null)
                    {
                        if (!classpec.EqualsWithoutAbilityPackage(cmp))
                            return false;
                    }
                }
            }
            return true;
        }
    }
    public class SlotTypeList : List<SlotType>
    {
        public SlotTypeList() : base() { }
        public SlotTypeList(IEnumerable<SlotType> collection) : base(collection) { }
        public override string ToString()
        {
            if (this == null) { return "null"; }
            if (this.Count <= 0) { return "Empty List"; }
            string retVal = "";
            foreach (SlotType i in this) { retVal += string.Format("{0}, ", i); }
            return retVal;
        }

        public override bool Equals(object obj)
        {
            if (obj == null) return false;

            if (ReferenceEquals(this, obj)) return true;

            SlotTypeList itmSTList = obj as SlotTypeList;
            return Equals(itmSTList);
        }

        public bool Equals(SlotTypeList itmSTList)
        {
            if (itmSTList == null) return false;

            if (!Enumerable.SequenceEqual<SlotType>(this, itmSTList)) return false;
            return true;
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }
    }

    class DictionaryComparer<TKey, TValue> : IEqualityComparer<IDictionary<TKey, TValue>>
    {
        public DictionaryComparer()
        {
        }

        public bool Equals(IDictionary<TKey, TValue> x, IDictionary<TKey, TValue> y)
        {
            if (x == null)
            {
                if (y == null) return true;
                else return false;
            }
            else if (y == null) return false;

            if (x.Count != y.Count)
                return false;

            HashSet<KeyValuePair<TKey, TValue>> set = new HashSet<KeyValuePair<TKey, TValue>>(x);
            set.SymmetricExceptWith(y);
            return set.Count == 0;
        }

        public int GetHashCode(IDictionary<TKey, TValue> obj)
        {
            int hash = 0;

            foreach (KeyValuePair<TKey, TValue> pair in obj)
            {
                int key = pair.Key.GetHashCode(); // key cannot be null
                int value = pair.Value != null ? pair.Value.GetHashCode() : 0;
                hash ^= ShiftAndWrap(key, 2) ^ value;
            }

            return hash;
        }

        private int ShiftAndWrap(int value, int positions)
        {
            positions &= 0x1F;

            // Save the existing bit pattern, but interpret it as an unsigned integer. 
            uint number = BitConverter.ToUInt32(BitConverter.GetBytes(value), 0);
            // Preserve the bits to be discarded. 
            uint wrapped = number >> (32 - positions);
            // Shift and wrap the discarded bits. 
            return BitConverter.ToInt32(BitConverter.GetBytes((number << positions) | wrapped), 0);
        }
    }
}
