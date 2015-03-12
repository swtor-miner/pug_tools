using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;
using System.Xml.XPath;
using System.Xml;

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

        public ulong NodeId { get; set; }
        public long NameId { get; set; }
        public string Name { get; set; }
        public Dictionary<string, string> LocalizedName { get; set; }
        public long DescriptionId { get; set; }
        public string Description { get; set; }
        public Dictionary<string, string> LocalizedDescription { get; set; }
        public int Value { get; set; }
        public int Durability { get; set; }
        public int MaxStack { get; set; }
        public int UniqueLimit { get; set; }
        public WeaponSpec WeaponSpec { get; set; }
        public ArmorSpec ArmorSpec { get; set; }
        public ArmorSpec ShieldSpec { get; set; }
        public ItemBindingRule Binding { get; set; }
        public string Icon { get; set; }
        public ItemQuality Quality { get; set; }
        public int ItemLevel { get; set; }
        public int Rating { get; set; }
        public int CombinedRating { get; set; }
        public int RequiredLevel { get; set; }
        public int CombinedRequiredLevel { get; set; }
        public ItemDamageType DamageType { get; set; }
        public int VendorStackSize { get; set; }
        public bool RequiresAlignment { get; set; }
        public int RequiredAlignmentTier { get; set; }
        public bool RequiredAlignmentInverted { get; set; }
        public bool RequiresSocial { get; set; }
        public int RequiredSocialTier { get; set; }
        public Profession RequiredProfession { get; set; }
        public int RequiredProfessionLevel { get; set; }
        public ProfessionSubtype DisassembleCategory { get; set; }
        public EnhancementCategory EnhancementCategory { get; set; }
        public EnhancementSubCategory EnhancementSubCategory { get; set; }
        public EnhancementType EnhancementType { get; set; }
        public GiftType GiftType { get; set; }
        public GiftRank GiftRank { get; set; }
        // public AuctionCategory AuctionCategory { get; set; }
        public AppearanceColor AppearanceColor { get; set; }
        public ulong EquipAbilityId { get; set; }

        [Newtonsoft.Json.JsonIgnore]
        public Ability EquipAbility { get; set; }

        public ulong UseAbilityId { get; set; }

        [Newtonsoft.Json.JsonIgnore]
        public Ability UseAbility { get; set; }
        
        public string ConversationFqn { get; set; }

        [Newtonsoft.Json.JsonIgnore]
        public Conversation Conversation { get; set; }

        public long ModifierSpec { get; set; }
        public Schematic Schematic { get; set; }
        public ulong SchematicId { get; set; }
        public string TreasurePackageSpec { get; set; }
        public long TreasurePackageId { get; set; }
        public long MountSpec { get; set; }
        public Gender RequiredGender { get; set; }
        public int RequiredValorRank { get; set; }
        public bool ConsumedOnUse { get; set; }
        public int TypeBitSet { get; set; }
        public bool IsModdable { get; set; }

        public long itmCraftedCategory { get; set; }

        public ItemCategory Category { get; set; }
        public ItemSubCategory SubCategory { get; set; }

        public ItemStatList StatModifiers { get; set; }
        public ItemStatList CombinedStatModifiers { get; set; }
        public ItemEnhancementList EnhancementSlots { get; set; }
        public ClassSpecList RequiredClasses { get; set; }
        public SlotTypeList Slots { get; set; }
        public string SoundType { get; set; }
        public long StackCount { get; set; }
        public long MaxDurability { get; set; }
        public string WeaponAppSpec { get; set; }
        internal WeaponAppearance _WeaponApp;
        public WeaponAppearance WeaponApp
        {
            get
            {
                if (WeaponAppSpec == null || WeaponAppSpec == "")
                    return null;
                if (_WeaponApp == null)
                    _WeaponApp = _dom.appearanceLoader.LoadWeaponAppearance(WeaponAppSpec);
                return _WeaponApp;
            }
        }

        public string Model { get; set; }
        public string ImperialVOModulation { get; set; }
        public string RepublicVOModulation { get; set; }
        public string ImperialAppearanceTag { get; set; }
        public string RepublicAppearanceTag { get; set; }

        public ulong TeachesRef { get; set; }
        public bool IsStrongholdDecoration { get; set; }
        public List<long> StrongholdSourceList { get; set; }
        public Dictionary<long, string> StrongholdSourceNameDict { get; set; }

        public List<ulong> rewardedForQuests { get; set; }
        public Dictionary<ulong, ulong> ippRefs { get; set; }
        public Dictionary<string, string> classAppearance { get; set; }

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
            HashSet<string> returnList = new HashSet<string>();

            returnList.Add(String.Format("/resources/gfx/icons/{0}.dds", this.Icon));
            var distinct = this.classAppearance.Values.Distinct();
            foreach (var distApp in distinct)
            {
                var appearance = _dom.appearanceLoader.Load(distApp);
                returnList.UnionWith(appearance.GetDependencies());
            }
            return returnList;
        }

        public override int GetHashCode() //needs fixed, it's changing for the same data
        {
            int hash = Id.GetHashCode();
            if (LocalizedName != null) foreach (var x in LocalizedName) { hash ^= x.GetHashCode(); } //dictionaries need to hashed like this
            if (LocalizedDescription != null) foreach (var x in LocalizedDescription) { hash ^= x.GetHashCode(); } //dictionaries need to hashed like this
            hash ^= AppearanceColor.GetHashCode();
            hash ^= ArmorSpec.GetHashCode();
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
            hash ^= GiftType.GetHashCode();
            hash ^= Icon.GetHashCode();
            hash ^= IsModdable.GetHashCode();
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
            hash ^= ShieldSpec.GetHashCode();
            hash ^= SubCategory.GetHashCode();
            hash ^= TypeBitSet.GetHashCode();
            hash ^= UniqueLimit.GetHashCode();
            hash ^= UseAbility.GetHashCode();
            hash ^= Value.GetHashCode();
            hash ^= VendorStackSize.GetHashCode();
            hash ^= WeaponSpec.GetHashCode();
            foreach (var x in CombinedStatModifiers) { hash ^= x.GetHashCode(); }
            foreach (var x in EnhancementSlots) { hash ^= x.GetHashCode(); }
            foreach (var x in RequiredClasses) { hash ^= x.Id.GetHashCode(); }
            foreach (var x in Slots) { hash ^= x.GetHashCode(); }
            foreach (var x in StatModifiers) { hash ^= x.GetHashCode(); }
            hash ^= StackCount.GetHashCode();
            hash ^= MaxDurability.GetHashCode();
            if (WeaponAppSpec != null) hash ^= WeaponAppSpec.GetHashCode();
            if (Model != null) hash ^= Model.GetHashCode();
            if (ImperialVOModulation != null) hash ^= ImperialVOModulation.GetHashCode();
            if (RepublicVOModulation != null) hash ^= RepublicVOModulation.GetHashCode();
            return hash;
        }

        public override bool Equals(object obj)
        {
            if (obj == null) return false;

            if (ReferenceEquals(this, obj)) return true;

            Item itm = obj as Item;
            if (itm == null) return false;

            return Equals(itm);
        }

        public bool Equals(Item itm)
        {
            if (itm == null) return false;

            if (ReferenceEquals(this, itm)) return true;

            if (this.AppearanceColor != itm.AppearanceColor)
                return false;
            if(this.ArmorSpec != itm.ArmorSpec)
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
            if (this.GiftType != itm.GiftType)
                return false;
            if (this.Icon != itm.Icon)
                return false;
            if (this.Id != itm.Id)
                return false;
            if (this.ImperialAppearanceTag != itm.ImperialAppearanceTag)
                return false;
            if (this.ImperialVOModulation != itm.ImperialVOModulation)
                return false;
            if (this.IsModdable != itm.IsModdable)
                return false;
            if (this.IsStrongholdDecoration != itm.IsStrongholdDecoration)
                return false;
            if (this.ItemLevel != itm.ItemLevel)
                return false;
            if (this.itmCraftedCategory != itm.itmCraftedCategory)
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
            if (this.NodeId != itm.NodeId)
                return false;
            if (this.Quality != itm.Quality)
                return false;
            if (this.Rating != itm.Rating)
                return false;
            if (this.RepublicAppearanceTag != itm.RepublicAppearanceTag)
                return false;
            if (this.RepublicVOModulation != itm.RepublicVOModulation)
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
            if (this.TreasurePackageSpec != itm.TreasurePackageSpec)
                return false;
            if (this.TypeBitSet != itm.TypeBitSet)
                return false;
            if (this.UniqueLimit != itm.UniqueLimit)
                return false;
            if (!this.UseAbility.Equals(itm.UseAbility))
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
                txtFile.Append("ItemNodeID: " + NodeId + n);
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
                txtFile.Append("  TreasurePackageSpec: " + TreasurePackageSpec + n);
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
                return new List<SQLProperty>
                    {                //(SQL Column Name, C# Property Name, SQL Column type statement, isUnique/PrimaryKey, Serialize value to json)
                        new SQLProperty("Name", "Name", "varchar(255) COLLATE utf8_unicode_ci NOT NULL"),
                        new SQLProperty("NodeId", "NodeId", "bigint(20) unsigned NOT NULL", true),
                        new SQLProperty("NameId", "NameId", "bigint(20) NOT NULL"),
                        new SQLProperty("Fqn", "Fqn", "varchar(255) COLLATE utf8_unicode_ci NOT NULL"),
                        new SQLProperty("ItemLevel", "ItemLevel", "int(11) NOT NULL"),
                        new SQLProperty("RequiredLevel", "RequiredLevel", "int(11) NOT NULL"),
                        new SQLProperty("AppearanceColor", "AppearanceColor", "varchar(255) COLLATE utf8_unicode_ci NOT NULL"),
                        new SQLProperty("ArmorSpec", "ArmorSpec", "varchar(255) COLLATE utf8_unicode_ci NOT NULL"),
                        new SQLProperty("Binding", "Binding", "varchar(255) COLLATE utf8_unicode_ci NOT NULL"),
                        new SQLProperty("CombinedRating", "CombinedRating", "int(11) NOT NULL"),
                        new SQLProperty("CombinedRequiredLevel", "CombinedRequiredLevel", "int(11) NOT NULL"),
                        new SQLProperty("CombinedStatModifiers", "CombinedStatModifiers", "varchar(255) COLLATE utf8_unicode_ci NOT NULL"),
                        new SQLProperty("ConsumedOnUse", "ConsumedOnUse", "varchar(255) COLLATE utf8_unicode_ci NOT NULL"),
                        new SQLProperty("ConversationFqn", "ConversationFqn", "varchar(255) COLLATE utf8_unicode_ci NOT NULL"),
                        new SQLProperty("DamageType", "DamageType", "varchar(255) COLLATE utf8_unicode_ci NOT NULL"),
                        new SQLProperty("Description", "Description", "varchar(255) COLLATE utf8_unicode_ci NOT NULL"),
                        new SQLProperty("DescriptionId", "DescriptionId", "bigint(20) NOT NULL"),
                        new SQLProperty("DisassembleCategory", "DisassembleCategory", "varchar(255) COLLATE utf8_unicode_ci NOT NULL"),
                        new SQLProperty("Durability", "Durability", "int(11) NOT NULL"),
                        new SQLProperty("EnhancementCategory", "EnhancementCategory", "varchar(255) COLLATE utf8_unicode_ci NOT NULL"),
                        new SQLProperty("EnhancementSlots", "EnhancementSlots", "text COLLATE utf8_unicode_ci NOT NULL"),
                        new SQLProperty("EnhancementSubCategory", "EnhancementSubCategory", "varchar(255) COLLATE utf8_unicode_ci NOT NULL"),
                        new SQLProperty("EnhancementType", "EnhancementType", "varchar(255) COLLATE utf8_unicode_ci NOT NULL"),
                        new SQLProperty("EquipAbilityId", "EquipAbilityId", "varchar(255) COLLATE utf8_unicode_ci NOT NULL"),
                        new SQLProperty("GiftRank", "GiftRank", "varchar(255) COLLATE utf8_unicode_ci NOT NULL"),
                        new SQLProperty("GiftType", "GiftType", "varchar(255) COLLATE utf8_unicode_ci NOT NULL"),
                        new SQLProperty("Icon", "Icon", "varchar(255) COLLATE utf8_unicode_ci NOT NULL"),
                        new SQLProperty("IsModdable", "IsModdable", "varchar(255) COLLATE utf8_unicode_ci NOT NULL"),
                        new SQLProperty("MaxStack", "MaxStack", "varchar(255) COLLATE utf8_unicode_ci NOT NULL"),
                        new SQLProperty("ModifierSpec", "ModifierSpec", "varchar(255) COLLATE utf8_unicode_ci NOT NULL"),
                        new SQLProperty("MountSpec", "MountSpec", "varchar(255) COLLATE utf8_unicode_ci NOT NULL"),
                        new SQLProperty("Quality", "Quality", "varchar(255) COLLATE utf8_unicode_ci NOT NULL"),
                        new SQLProperty("Rating", "Rating", "int(11) NOT NULL"),
                        new SQLProperty("RequiredAlignmentInverted", "RequiredAlignmentInverted", "varchar(255) COLLATE utf8_unicode_ci NOT NULL"),
                        new SQLProperty("RequiredClasses", "RequiredClasses", "varchar(255) COLLATE utf8_unicode_ci NOT NULL"),
                        new SQLProperty("RequiredGender", "RequiredGender", "varchar(255) COLLATE utf8_unicode_ci NOT NULL"),
                        new SQLProperty("RequiredProfession", "RequiredProfession", "varchar(255) COLLATE utf8_unicode_ci NOT NULL"),
                        new SQLProperty("RequiredProfessionLevel", "RequiredProfessionLevel", "int(11) NOT NULL"),
                        new SQLProperty("RequiredSocialTier", "RequiredSocialTier", "int(11) NOT NULL"),
                        new SQLProperty("RequiredValorRank", "RequiredValorRank", "int(11) NOT NULL"),
                        new SQLProperty("RequiresAlignment", "RequiresAlignment", "varchar(255) COLLATE utf8_unicode_ci NOT NULL"),
                        new SQLProperty("RequiresSocial", "RequiresSocial", "varchar(255) COLLATE utf8_unicode_ci NOT NULL"),
                        new SQLProperty("SchematicId", "SchematicId", "varchar(255) COLLATE utf8_unicode_ci NOT NULL"),
                        new SQLProperty("ShieldSpec", "ShieldSpec", "varchar(255) COLLATE utf8_unicode_ci NOT NULL"),
                        new SQLProperty("Slots", "Slots", "varchar(255) COLLATE utf8_unicode_ci NOT NULL"),
                        new SQLProperty("StatModifiers", "StatModifiers", "varchar(255) COLLATE utf8_unicode_ci NOT NULL"),
                        new SQLProperty("SubCategory", "SubCategory", "varchar(255) COLLATE utf8_unicode_ci NOT NULL"),
                        new SQLProperty("TreasurePackageId", "TreasurePackageId", "varchar(255) COLLATE utf8_unicode_ci NOT NULL"),
                        new SQLProperty("TreasurePackageSpec", "TreasurePackageSpec", "varchar(255) COLLATE utf8_unicode_ci NOT NULL"),
                        new SQLProperty("UniqueLimit", "UniqueLimit", "varchar(255) COLLATE utf8_unicode_ci NOT NULL"),
                        new SQLProperty("UseAbilityId", "UseAbilityId", "varchar(255) COLLATE utf8_unicode_ci NOT NULL"),
                        new SQLProperty("Value", "Value", "int(11) NOT NULL"),
                        new SQLProperty("VendorStackSize", "VendorStackSize", "bigint(20) NOT NULL"),
                        new SQLProperty("WeaponSpec", "WeaponSpec", "varchar(255) COLLATE utf8_unicode_ci NOT NULL"),
                        new SQLProperty("TypeBitSet", "TypeBitSet", "varchar(255) COLLATE utf8_unicode_ci NOT NULL"),
                        new SQLProperty("StackCount", "StackCount", "int(11) NOT NULL"),
                        new SQLProperty("MaxDurability", "MaxDurability", "int(11) NOT NULL"),
                        new SQLProperty("WeaponAppSpec", "WeaponAppSpec", "varchar(255) COLLATE utf8_unicode_ci NOT NULL"),
                        new SQLProperty("Model", "Model", "varchar(255) COLLATE utf8_unicode_ci NOT NULL"),
                        new SQLProperty("ImperialVOModulation", "ImperialVOModulation", "varchar(255) COLLATE utf8_unicode_ci NOT NULL"),
                        new SQLProperty("RepublicVOModulation", "RepublicVOModulation", "varchar(255) COLLATE utf8_unicode_ci NOT NULL")
                    };
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
                item.Element("Fqn").Add(new XAttribute("Id", NodeId));
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
                    item.Add(new GameObject().ToXElement(ConversationFqn, _dom, false));
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
                    new XElement("IsModdable", IsModdable),
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
                    new XElement("TreasurePackageSpec", TreasurePackageSpec,
                        new XAttribute("Id", TreasurePackageId)),
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
                if (ImperialVOModulation != null)
                {
                    item.Add(new XElement("ImpVoiceModulation", ImperialVOModulation),
                        new XElement("RepublicVoiceModulation", RepublicVOModulation));
                }
                if (ImperialAppearanceTag != null)
                {
                    item.Add(new XElement("ImperialAppearanceTag", ImperialAppearanceTag),
                        new XElement("RepublicAppearanceTag", RepublicAppearanceTag));
                }
                if (classAppearance != null)
                    item.Add(new XElement("ClassAppearances", classAppearance.Select(x => new XElement(x.Key, x.Value))));
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
            positions = positions & 0x1F;

            // Save the existing bit pattern, but interpret it as an unsigned integer. 
            uint number = BitConverter.ToUInt32(BitConverter.GetBytes(value), 0);
            // Preserve the bits to be discarded. 
            uint wrapped = number >> (32 - positions);
            // Shift and wrap the discarded bits. 
            return BitConverter.ToInt32(BitConverter.GetBytes((number << positions) | wrapped), 0);
        }
    }
}
