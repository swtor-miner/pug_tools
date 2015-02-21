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
        public void getItems()
        {
            Clearlist2();
            ClearProgress();

            LoadData();
            var itmList = currentDom.GetObjectsStartingWith("itm.").Where(obj => !obj.Name.StartsWith("itm.test.") && !obj.Name.StartsWith("itm.npc."));
            var ttl = itmList.Count();

            var append = false;
            bool addedChanged = false;
            string changed = "";
            if (sql)
            {
                ItemDataFromFqnListToSQL(itmList);
            }
            else
            {
                if(chkBuildCompare.Checked)
                {
                    addedChanged = true;
                    changed = "Changed";
                }
                else
                {
                    sqlTransactionsInitialize("item", "");
                }
                var filename = changed + "Items.xml";

                if(outputTypeName == "Text")
                {
                    filename = changed + "Items.txt";
                    string generatedContent = ItemDataFromFqnList(itmList);
                    WriteFile(generatedContent, filename, append);
                }
                else if(outputTypeName == "JSON")
                {
                    ItemDataFromFqnListAsJSON(itmList);
                }
                else
                {
                    if (addedChanged) ProcessGameObjects("itm.", "Items");
                    else
                    {
                        XDocument xmlContent = new XDocument(ItemDataFromFqnListAsXElement(itmList, addedChanged));
                        WriteFile(xmlContent, filename, append);
                    }
                }
                
                if (MessageBox.Show("Export Icons?", "Select an Option", MessageBoxButtons.YesNo) == System.Windows.Forms.DialogResult.Yes)
                {
                    addtolist("Exporting Icons.");
                    if (!System.IO.Directory.Exists(Config.ExtractPath + prefix + "\\ICONS\\")) { System.IO.Directory.CreateDirectory(Config.ExtractPath + prefix + "\\ICONS\\"); }
                    this.currentAssets.icons.SaveTo(Config.ExtractPath + prefix + "\\ICONS\\", true);
                }
            }

            //MessageBox.Show("The Item list has been generated there are " + ttl + " Items");
            EnableButtons();
        }

        public void getItemApps()
        {
            Clearlist2();
            ClearProgress();

            LoadData();
            var itmList = currentDom.GetObjectsStartingWith("ipp.");
            var ttl = itmList.Count();

            var append = false;
            bool addedChanged = false;
            string changed = "";

            if(chkBuildCompare.Checked)
            {
                addedChanged = true;
                changed = "Changed";
            }
            else
            {
                sqlTransactionsInitialize("item", "");
            }
            var filename = changed + "Items.xml";

            if(outputTypeName == "Text")
            {
                filename = changed + "Items.txt";
                string generatedContent = ItemDataFromFqnList(itmList);
                WriteFile(generatedContent, filename, append);
            }
            else
            {
                if (addedChanged) ProcessGameObjects("ipp.", "ItemAppearances");
                else
                {
                    int i = 0;
                    int count = itmList.Count();
                    ClearProgress();
                    foreach (var itm in itmList)
                    {
                        i++;
                        progressUpdate(i, count);
                        addtolist2(itm.Name);

                        WriteFile(new XDocument(ItemAppearanceToXElement(itm)), itm.Name.Replace(".", "\\") + ".ipp", false);
                    }
                }
            }

            EnableButtons();
        }
        
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
        #region text
        public string ItemDataFromFqnList(IEnumerable<GomLib.GomObject> itmList)
        {
            int i = 0;
            string n = Environment.NewLine;
            var txtFile = new StringBuilder();
            var count = itmList.Count();
            foreach (var gomItm in itmList)
            {
                progressUpdate(i, count);
                GomLib.Models.Item itm = new GomLib.Models.Item();
                currentDom.itemLoader.Load(itm, gomItm);
                gomItm.Unload(); // These GomObjects are staying loaded in memory and cause our massive memory issues.
                addtolist2("ItemName: " + itm.Name);

                if (!verbose)
                {
                    txtFile.Append(itm.Name + ": " + itm.Description.Replace("\u000A", " ") + n);
                    //txtFile.Append(itm.Name + ": " + itm.StatModifiers.ToString() + n);
                }
                else
                {
                    txtFile.Append("------------------------------------------------------------" + n);
                    txtFile.Append("ItemName: " + itm.Name + n);
                    txtFile.Append("ItemNodeID: " + itm.NodeId + n);
                    txtFile.Append("NameId: " + itm.NameId + n);
                    //file.Append("------------------------------------------------------------" + n);
                    //file.Append("  Item INFO" + n);
                    txtFile.Append("  Item.fqn: " + itm.Fqn + n);
                    txtFile.Append("  ItemLevel: " + itm.ItemLevel + n);
                    txtFile.Append("  ItemRequiredLevel: " + itm.RequiredLevel + n);
                    txtFile.Append("  AppearanceColor: " + itm.AppearanceColor + n);
                    txtFile.Append("  Description: " + itm.Description.Replace("\u000A", " ") + n);
                    txtFile.Append("  Icon: " + itm.Icon + n);
                    txtFile.Append("  ArmorSpec: " + itm.ArmorSpec + n);
                    txtFile.Append("  MaxStack: " + itm.MaxStack + n);
                    txtFile.Append("  Bindtype: " + itm.Binding + n);
                    txtFile.Append("  ModifierSpec: " + itm.ModifierSpec + n);
                    txtFile.Append("  Quality: " + itm.Quality + n);
                    txtFile.Append("  Rating: " + itm.Rating + n);
                    txtFile.Append("  RequiredAlignmentInverte: " + itm.RequiredAlignmentInverted + n);
                    txtFile.Append("  RequiredAlignmentTier: " + itm.RequiredAlignmentTier + n);
                    txtFile.Append("  RequiredClasses: " + itm.RequiredClasses + n);
                    txtFile.Append("  RequiredGender: " + itm.RequiredGender + n);
                    txtFile.Append("  RequiredProfession: " + itm.RequiredProfession + n);
                    txtFile.Append("  RequiredProfessionLevel: " + itm.RequiredProfessionLevel + n);
                    txtFile.Append("  RequiredSocialTier: " + itm.RequiredSocialTier + n);
                    txtFile.Append("  RequiredValorRank: " + itm.RequiredValorRank + n);
                    txtFile.Append("  RequiresAlignment: " + itm.RequiresAlignment + n);
                    txtFile.Append("  RequiresSocial: " + itm.RequiresSocial + n);
                    txtFile.Append("  Schematic: " + itm.Schematic + n);
                    txtFile.Append("  SchematicId: " + itm.SchematicId + n);
                    txtFile.Append("  ShieldSpec: " + itm.ShieldSpec + n);
                    txtFile.Append("  Slots: " + itm.Slots + n);
                    txtFile.Append("  StatModifiers: " + itm.StatModifiers + n);
                    txtFile.Append("  SubCategory: " + itm.SubCategory + n);
                    txtFile.Append("  TreasurePackageId: " + itm.TreasurePackageId + n);
                    txtFile.Append("  TreasurePackageSpec: " + itm.TreasurePackageSpec + n);
                    txtFile.Append("  TypeBitSet: " + itm.TypeBitSet + n);
                    txtFile.Append("  UniqueLimit: " + itm.UniqueLimit + n);
                    txtFile.Append("  UseAbility: " + itm.UseAbility + n);
                    txtFile.Append("  UseAbilityId: " + itm.UseAbilityId + n);
                    txtFile.Append("  Value: " + itm.Value + n);
                    txtFile.Append("  VendorStackSize: " + itm.VendorStackSize + n);
                    txtFile.Append("  WeaponSpec: " + itm.weaponAppearanceSpec + n);
                    txtFile.Append("------------------------------------------------------------" + n + n);
                }
                //AddItemToSQL(itm);
                i++;
            }
            addtolist("the item lists has been generated there are " + i + " items");
            ClearProgress();
            return txtFile.ToString();
        }
        #endregion
        #region XML
        public XElement ItemDataFromFqnListAsXElement(IEnumerable<GomLib.GomObject> itmList, bool addedChangedOnly)
        {
            int i = 0;
            int count = itmList.Count();
            XElement items = new XElement("Items");
            foreach (var gomItm in itmList)
            {
                progressUpdate(i, count);
                GomLib.Models.Item itm = new GomLib.Models.Item();
                currentDom.itemLoader.Load(itm, gomItm);
                addtolist2("ItemName: " + itm.Name);
                XElement item = ItemToXElement(itm);
                item.Add(ReferencesToXElement(gomItm.References));
                gomItm.Unload(); // These GomObjects are staying loaded in memory and cause our massive memory issues.
                
                items.Add(item);
                //AddItemToSQL(itm);
                i++;
            }

            /*if (addedChangedOnly)
            {
                //addtolist("Comparing the Current Items to the loaded Patch");
                XElement addedItems = FindChangedEntries(items, "Items", "Item");
                addtolist("The Item list has been generated there are " + addedItems.Elements("Item").Count() + " new/changed Items.");
                items = null;
                addedItems = SortItems(addedItems);
                return addedItems;
            }*/

            items = SortItems(items);
            //var count = items.Elements().Count();
            addtolist("The Item list has been generated there are " + i + " Items.");
            ClearProgress();
            return items;
        }

        private XElement SortItems(XElement items)
        {
            //addtolist("Sorting Items");
            items.ReplaceNodes(items.Elements("Item")
                .OrderBy(x => (string)x.Attribute("Status"))
                .ThenBy(x => (string)x.Element("Fqn")));
                //.ThenBy(x => (string)x.Element("Name")));
            return items;
        }

        public XElement ItemToXElement(GomObject gomItm)
        {
            return ItemToXElement(gomItm, false);
        }

        public XElement ItemToXElement(GomObject gomItm, bool overrideVerbose)
        {
            if (gomItm != null)
            {
                GomLib.Models.Item itm = new GomLib.Models.Item();
                currentDom.itemLoader.Load(itm, gomItm);
                gomItm.Unload(); // These GomObjects are staying loaded in memory and cause our massive memory issues.
                return ItemToXElement(itm, overrideVerbose);
            }
            return null;
        }

        public XElement ItemToXElement(GomLib.Models.Item itm)
        {
            return ItemToXElement(itm, false);
        }

        public XElement ItemToXElement(GomLib.Models.Item itm, bool overrideVerbose)
        {
            //addtolist2("ItemName: " + itm.Name);
            XElement item = new XElement("Item",
                new XElement("Fqn", itm.Fqn),
                new XAttribute("Id", itm.Id),
                //new XAttribute("Hash", itm.GetHashCode()),
                new XElement("Name", itm.Name),
                new XElement("Description", itm.Description));

            if (itm.UseAbilityId == 0) { item.Add(new XElement("UseAbility")); }
            else { item.Add(new XElement("UseAbility", ConvertToXElement(itm.UseAbility, overrideVerbose))); }
            if (itm.EquipAbilityId == 0) { item.Add(new XElement("EquipAbility")); }
            else { item.Add(new XElement("EquipAbility", ConvertToXElement(itm.EquipAbility, overrideVerbose))); }

            if (verbose && !overrideVerbose)
            {
                /*item.Element("Name").RemoveAll();
                for (int i = 0; i < localizations.Count; i++)
                {
                    if (itm.LocalizedName[localizations[i]] != "")
                    {
                        item.Element("Name").Add(new XElement(localizations[i], itm.LocalizedName[localizations[i]]));
                    }
                }

                item.Element("Description").RemoveAll();
                for (int i = 0; i < localizations.Count; i++)
                {
                    if (itm.LocalizedDescription[localizations[i]] != "")
                    {
                        item.Element("Description").Add(new XElement(localizations[i], itm.LocalizedDescription[localizations[i]]));
                    }
                }*/
                item.Element("Name").Add(new XAttribute("Id", itm.NameId));
                item.Element("Description").Add(new XAttribute("Id", itm.DescriptionId));
                item.Element("Fqn").Add(new XAttribute("Id", itm.NodeId));
                //item.Element("EquipAbility").Add(new XAttribute("Id", itm.EquipAbilityId));
                //item.Element("UseAbility").Add(new XAttribute("Id", itm.UseAbilityId));
                item.Add(new XElement("Icon", itm.Icon),
                    new XElement("SoundPackage", itm.SoundType),
                    new XElement("Model", itm.Model),
                    new XElement("ItemLevel", itm.ItemLevel),
                    new XElement("RequiredLevel", itm.RequiredLevel),
                    new XElement("CombinedRequiredLevel", itm.CombinedRequiredLevel),
                    new XElement("ArmorSpec", itm.ArmorSpec),
                    new XElement("Binding", itm.Binding),
                    new XElement("CombinedRating", itm.CombinedRating),
                    new XElement("ConsumedOnUse", itm.ConsumedOnUse),
                    new XElement("AppearanceColor", itm.AppearanceColor));
                if (itm.ConversationFqn != null)
                {
                    item.Add(ConvertToXElement(itm._dom.conversationLoader.Load(itm.ConversationFqn), true));
                }
                else
                {
                    item.Add(new XElement("Conversation"));
                }
                item.Add(new XElement("DamageType", itm.DamageType),
                    new XElement("DisassembleCategory", itm.DisassembleCategory),
                    new XElement("Durability", itm.Durability));
                XElement enhancements = new XElement("Enhancements",
                        new XElement("Category", itm.EnhancementCategory),
                        new XElement("SubCategory", itm.EnhancementSubCategory));
                foreach (var enhancement in itm.EnhancementSlots)
                {
                    enhancements.Add(new XElement(enhancement.Slot.ToString(),
                        new XElement("Modification", enhancement.Modification,
                            new XAttribute("Id", enhancement.ModificationId.ToString()))));
                }
                item.Add(enhancements,
                    new XElement("StatModifiers", itm.StatModifiers.ToString()),
                    new XElement("CombinedStatModifiers", itm.CombinedStatModifiers.ToString()),
                    new XElement("EnhancementType", itm.EnhancementType),
                    new XElement("GiftType", itm.GiftType,
                        new XAttribute("GiftRank", itm.GiftRank)),
                    new XElement("IsModdable", itm.IsModdable),
                    new XElement("MaxStack", itm.MaxStack),
                    new XElement("ModifierSpec", itm.ModifierSpec),
                    new XElement("MountSpec", itm.MountSpec),
                    new XElement("Quality", itm.Quality),
                    new XElement("Rating", itm.Rating),
                    new XElement("RequiredAlignmentInverted", itm.RequiredAlignmentInverted));

                XElement requirements = new XElement("Requirements");
                string reqclasses = null;
                foreach (var reqclass in itm.RequiredClasses)
                {
                    reqclasses += reqclass.Name.ToString() + ", ";
                }
                if (reqclasses != null) { reqclasses = reqclasses.Substring(0, reqclasses.Length - 2); }
                requirements.Add(new XElement("Classes", reqclasses));
                requirements.Add(new XElement("Gender", itm.RequiredGender),
                    new XElement("Profession", itm.RequiredProfession,
                        new XAttribute("Level", itm.RequiredProfessionLevel)));
                if (itm.RequiresSocial)
                {
                    requirements.Add(new XElement("Social", itm.RequiredSocialTier));
                }
                else
                {
                    requirements.Add(new XElement("Social"));
                }
                requirements.Add(new XElement("ValorRank", itm.RequiredValorRank),
                    new XElement("Alignment", itm.RequiresAlignment));
                item.Add(requirements);
                item.Add(ConvertToXElement(itm.Schematic));
                item.Add(new XElement("ShieldSpec", itm.ShieldSpec),
                    new XElement("Slots", itm.Slots),
                    new XElement("SubCategory", itm.SubCategory),
                    new XElement("TreasurePackageSpec", itm.TreasurePackageSpec,
                        new XAttribute("Id", itm.TreasurePackageId)),
                    new XElement("TypeBitSet", itm.TypeBitSet),
                    /*   new XAttribute("Id", itm.TypeBitSet),
                       ((GomLib.Models.ItemTypeFlags)(object)itm.TypeBitSet)
                       .ToString()
                       .Replace(" ", "")
                       .Split(',')
                       .ToList()
                       .Select(x => new XElement("Flag", x))), */
                    new XElement("UniqueLimit", itm.UniqueLimit),
                    new XElement("Value", itm.Value),
                    new XElement("VendorStackSize", itm.VendorStackSize),
                    new XElement("WeaponSpec", itm.weaponAppearanceSpec));
                if (itm.ImperialVOModulation != null)
                {
                    item.Add(new XElement("ImpVoiceModulation", itm.ImperialVOModulation),
                        new XElement("RepublicVoiceModulation", itm.RepublicVOModulation));
                }
                if (itm.ImperialAppearanceTag != null)
                {
                    item.Add(new XElement("ImperialAppearanceTag", itm.ImperialAppearanceTag),
                        new XElement("RepublicAppearanceTag", itm.RepublicAppearanceTag));
                }
                if (itm.classAppearance != null)
                    item.Add(new XElement("ClassAppearances", itm.classAppearance.Select(x => new XElement(x.Key, x.Value))));
            }
            else
            {
                item.Elements().Where(x => x.IsEmpty).Remove();
            }

            if (ExportICONS)
            {
                OutputSchematicIcon(itm.Icon);
            }
            return item;
        }

        public XElement ItemAppearanceToXElement(ulong id, DataObjectModel dom)
        {
            if (id != 0 && id != null)
                return ItemAppearanceToXElement((GomLib.Models.ItemAppearance)dom.appearanceLoader.Load(id));
            return null;
        }

        public XElement ItemAppearanceToXElement(string str, DataObjectModel dom)
        {
            if (str != "" && str != null)
                return ItemAppearanceToXElement((GomLib.Models.ItemAppearance)dom.appearanceLoader.Load(str));
            return null;
        }

        public XElement ItemAppearanceToXElement(GomObject obj)
        {
            if (obj != null)
                return ItemAppearanceToXElement((GomLib.Models.ItemAppearance)obj._dom.appearanceLoader.Load(obj));
            return null;
        }

        public XElement ItemAppearanceToXElement(GomLib.Models.ItemAppearance ipp)
        {
            XElement ItemAppearance = new XElement("ItemAppearance", new XAttribute("Id", ipp.IPP.ModelID));
            XElement appSlot = AppearanceSlotToXElement(ipp.IPP);

            ItemAppearance.Add(appSlot.Elements());
            ItemAppearance.Add(new XElement("ColorScheme", ipp.ColorScheme),
                new XElement("VOSoundTypeOverride", ipp.VOSoundTypeOverride));

            return ItemAppearance;
        }

        #endregion
        #region JSON
        private void ItemDataFromFqnListAsJSON(IEnumerable<GomLib.GomObject> itmList)
        {
            int i = 0;
            short e = 0;
            string n = Environment.NewLine;
            var txtFile = new StringBuilder();
            WriteFile("", "Items.json", false);
            var count = itmList.Count();
            foreach (var gomItm in itmList)
            {
                progressUpdate(i, count);
                if (e%1000 == 1)
                {
                    WriteFile(txtFile.ToString(), "Items.json", true);
                    txtFile.Clear();
                    e = 0;
                }
                GomLib.Models.Item itm = new GomLib.Models.Item();
                currentDom.itemLoader.Load(itm, gomItm);
                gomItm.Unload();

                addtolist2("Item: " + itm.Name);

                string jsonString = itm.ToJSON(); // ConvertToJson(itm); //added method in Tools.cs
                txtFile.Append(jsonString + Environment.NewLine); //Append it with a newline to the output.
                i++;
                e++;
            }
            addtolist("The item json file has been generated; there are " + i + " items");
            WriteFile(txtFile.ToString(), "Items.json", true);
            ClearProgress();
        }
        #endregion
    }
}