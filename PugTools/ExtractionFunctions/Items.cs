using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Net;
using System.Windows;
using System.Threading;
using Microsoft.Win32;
using MySql.Data.MySqlClient;
using System.Text.RegularExpressions;
using System.Xml;
using System.IO;
using System.Xml.Linq;
using System.Xml.Serialization;
using GomLib;
using MessageBox = System.Windows.Forms.MessageBox;

namespace tor_tools
{
    public partial class Tools
    {
        #region SQL

        public string ItemDataFromFqnListToSQL(IEnumerable<GomLib.GomObject> itmList)
        {
            int i = 0;
            string n = Environment.NewLine;
            var txtFile = new StringBuilder();

            sqlTransactionsInitialize("INSERT INTO item (current_version, previous_version, Name, NodeId, NameId, Fqn, ItemLevel, RequiredLevel, AppearanceColor, ArmorSpec, Binding, CombinedRating, CombinedRequiredLevel, CombinedStatModifiers, ConsumedOnUse, Conversation, ConversationFqn, DamageType, Description, DescriptionId, DisassembleCategory, Durability, EnhancementCategory, EnhancementSlots, EnhancementSubCategory, EnhancementType, EquipAbilityId, GiftRank, GiftType, Icon, IsModdable, MaxStack, ModifierSpec, MountSpec, Quality, Rating, RequiredAlignmentInverted, RequiredClasses, RequiredGender, RequiredProfession, RequiredProfessionLevel, RequiredSocialTier, RequiredValorRank, RequiresAlignment, RequiresSocial, Schematic, SchematicId, ShieldSpec, Slots, StatModifiers, SubCategory, TreasurePackageId, TreasurePackageSpec, UniqueLimit, UseAbilityId, Value, VendorStackSize, WeaponSpec, TypeBitSet, Hash, StackCount, MaxDurability, WeaponAppSpec, Model, ImperialVOModulation, RepublicVOModulation) VALUES ",

@"ON DUPLICATE KEY UPDATE 
previous_version = IF((@update_record := (Hash <> VALUES(Hash))), current_version, previous_version),
current_version = IF(@update_record, VALUES(current_version), current_version),
Name = IF(@update_record, VALUES(Name), Name),
NodeId = IF(@update_record, VALUES(NodeId), NodeId),
NameId = IF(@update_record, VALUES(NameId), NameId),
Fqn = IF(@update_record, VALUES(Fqn), Fqn),
ItemLevel = IF(@update_record, VALUES(ItemLevel), ItemLevel),
RequiredLevel = IF(@update_record, VALUES(RequiredLevel), RequiredLevel),
AppearanceColor = IF(@update_record, VALUES(AppearanceColor), AppearanceColor),
ArmorSpec = IF(@update_record, VALUES(ArmorSpec), ArmorSpec),
Binding = IF(@update_record, VALUES(Binding), Binding),
CombinedRating = IF(@update_record, VALUES(CombinedRating), CombinedRating),
CombinedRequiredLevel = IF(@update_record, VALUES(CombinedRequiredLevel), CombinedRequiredLevel),
CombinedStatModifiers = IF(@update_record, VALUES(CombinedStatModifiers), CombinedStatModifiers),
ConsumedOnUse = IF(@update_record, VALUES(ConsumedOnUse), ConsumedOnUse),
Conversation = IF(@update_record, VALUES(Conversation), Conversation),
ConversationFqn = IF(@update_record, VALUES(ConversationFqn), ConversationFqn),
DamageType = IF(@update_record, VALUES(DamageType), DamageType),
Description = IF(@update_record, VALUES(Description), Description),
DescriptionId = IF(@update_record, VALUES(DescriptionId), DescriptionId),
DisassembleCategory = IF(@update_record, VALUES(DisassembleCategory), DisassembleCategory),
Durability = IF(@update_record, VALUES(Durability), Durability),
EnhancementCategory = IF(@update_record, VALUES(EnhancementCategory), EnhancementCategory),
EnhancementSlots = IF(@update_record, VALUES(EnhancementSlots), EnhancementSlots),
EnhancementSubCategory = IF(@update_record, VALUES(EnhancementSubCategory), EnhancementSubCategory),
EnhancementType = IF(@update_record, VALUES(EnhancementType), EnhancementType),
EquipAbilityId = IF(@update_record, VALUES(EquipAbilityId), EquipAbilityId),
GiftRank = IF(@update_record, VALUES(GiftRank), GiftRank),
GiftType = IF(@update_record, VALUES(GiftType), GiftType),
Icon = IF(@update_record, VALUES(Icon), Icon),
IsModdable = IF(@update_record, VALUES(IsModdable), IsModdable),
MaxStack = IF(@update_record, VALUES(MaxStack), MaxStack),
ModifierSpec = IF(@update_record, VALUES(ModifierSpec), ModifierSpec),
MountSpec = IF(@update_record, VALUES(MountSpec), MountSpec),
Quality = IF(@update_record, VALUES(Quality), Quality),
Rating = IF(@update_record, VALUES(Rating), Rating),
RequiredAlignmentInverted = IF(@update_record, VALUES(RequiredAlignmentInverted), RequiredAlignmentInverted),
RequiredClasses = IF(@update_record, VALUES(RequiredClasses), RequiredClasses),
RequiredGender = IF(@update_record, VALUES(RequiredGender), RequiredGender),
RequiredProfession = IF(@update_record, VALUES(RequiredProfession), RequiredProfession),
RequiredProfessionLevel = IF(@update_record, VALUES(RequiredProfessionLevel), RequiredProfessionLevel),
RequiredSocialTier = IF(@update_record, VALUES(RequiredSocialTier), RequiredSocialTier),
RequiredValorRank = IF(@update_record, VALUES(RequiredValorRank), RequiredValorRank),
RequiresAlignment = IF(@update_record, VALUES(RequiresAlignment), RequiresAlignment),
RequiresSocial = IF(@update_record, VALUES(RequiresSocial), RequiresSocial),
Schematic = IF(@update_record, VALUES(Schematic), Schematic),
SchematicId = IF(@update_record, VALUES(SchematicId), SchematicId),
ShieldSpec = IF(@update_record, VALUES(ShieldSpec), ShieldSpec),
Slots = IF(@update_record, VALUES(Slots), Slots),
StatModifiers = IF(@update_record, VALUES(StatModifiers), StatModifiers),
SubCategory = IF(@update_record, VALUES(SubCategory), SubCategory),
TreasurePackageId = IF(@update_record, VALUES(TreasurePackageId), TreasurePackageId),
TreasurePackageSpec = IF(@update_record, VALUES(TreasurePackageSpec), TreasurePackageSpec),
UniqueLimit = IF(@update_record, VALUES(UniqueLimit), UniqueLimit),
UseAbilityId = IF(@update_record, VALUES(UseAbilityId), UseAbilityId),
Value = IF(@update_record, VALUES(Value), Value),
VendorStackSize = IF(@update_record, VALUES(VendorStackSize), VendorStackSize),
WeaponSpec = IF(@update_record, VALUES(WeaponSpec), WeaponSpec),
TypeBitSet = IF(@update_record, VALUES(TypeBitSet), TypeBitSet),
Hash = IF(@update_record, VALUES(Hash), Hash),
StackCount = IF(@update_record, VALUES(StackCount), StackCount),
MaxDurability = IF(@update_record, VALUES(MaxDurability), MaxDurability),
WeaponAppSpec = IF(@update_record, VALUES(WeaponAppSpec), WeaponAppSpec),
Model = IF(@update_record, VALUES(Model), Model),
ImperialVOModulation = IF(@update_record, VALUES(ImperialVOModulation), ImperialVOModulation),
RepublicVOModulation = IF(@update_record, VALUES(RepublicVOModulation), RepublicVOModulation);");

            var count = itmList.Count();
            foreach (var gomItm in itmList)
            {
                progressUpdate(i, count);
                GomLib.Models.Item itm = new GomLib.Models.Item();
                currentDom.itemLoader.Load(itm, gomItm);
                gomItm.Unload(); // These GomObjects are staying loaded in memory and cause our massive memory issues.
                addtolist2("ItemName: " + itm.Name);

                AddItemToSQL(itm);
                i++;
            }
            sqlTransactionsFlush();

            addtolist("the item mysql table has been generated there were " + i + " items parsed.");
            ClearProgress();
            return txtFile.ToString();
        }

        private void AddItemToSQL(GomLib.Models.Item itm)
        {
            string s = "', '";
            string value = "('" + sqlSani(patchVersion) + s + s + sqlSani(itm.Name) + s + itm.NodeId + s + itm.NameId + s + sqlSani(itm.Fqn) + s + itm.ItemLevel + s + itm.RequiredLevel + s + itm.AppearanceColor + s + itm.ArmorSpec + s + itm.Binding + s + itm.CombinedRating + s + itm.CombinedRequiredLevel + s + itm.CombinedStatModifiers + s + itm.ConsumedOnUse + s + itm.Conversation + s + itm.ConversationFqn + s + itm.DamageType + s + sqlSani(itm.Description) + s + itm.DescriptionId + s + itm.DisassembleCategory + s + itm.Durability + s + itm.EnhancementCategory + s + sqlSani(itm.EnhancementSlots.ToString()) + s + itm.EnhancementSubCategory + s + itm.EnhancementType + s + itm.EquipAbilityId + s + itm.GiftRank + s + itm.GiftType + s + sqlSani(itm.Icon) + s + itm.IsModdable + s + itm.MaxStack + s + itm.ModifierSpec + s + itm.MountSpec + s + itm.Quality + s + itm.Rating + s + itm.RequiredAlignmentInverted + s + sqlSani(itm.RequiredClasses.ToString()) + s + itm.RequiredGender + s + itm.RequiredProfession + s + itm.RequiredProfessionLevel + s + itm.RequiredSocialTier + s + itm.RequiredValorRank + s + itm.RequiresAlignment + s + itm.RequiresSocial + s + itm.Schematic + s + itm.SchematicId + s + itm.ShieldSpec + s + itm.Slots + s + itm.StatModifiers + s + itm.SubCategory + s + itm.TreasurePackageId + s + itm.TreasurePackageSpec + s + itm.UniqueLimit + s + itm.UseAbilityId + s + itm.Value + s + itm.VendorStackSize + s + itm.weaponAppearanceSpec + s + itm.TypeBitSet + s + itm.GetHashCode() + s + itm.StackCount + s + itm.MaxDurability + s + itm.WeaponAppSpec + s + itm.Model + s + itm.ImperialVOModulation + s + itm.RepublicVOModulation + "')";
            sqlAddTransactionValue(value);
        }
        #endregion
        #region XML

        /* code moved to GomLib.Models.Item.cs */

        private XElement SortItems(XElement items)
        {
            //addtolist("Sorting Items");
            items.ReplaceNodes(items.Elements("Item")
                .OrderBy(x => (string)x.Attribute("Status"))
                .ThenBy(x => (string)x.Element("Fqn")));
                //.ThenBy(x => (string)x.Element("Name")));
            return items;
        }

        
        #endregion
    }
}