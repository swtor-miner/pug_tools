using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Newtonsoft.Json;

namespace GomLib.Models
{
    /*public enum AuctionCategory
    {
        None = 0,
        ArmorLight = 1,
        ArmorMedium = 2,
        ArmorHeavy = 3,
        WeaponMelee = 4,
        WeaponRanged = 5,
        Implant = 6,
        Earpiece = 7,
        Shield = 8,
        Focus = 9,
        Relic = 10,
        Gadget = 11,
        ItemModification = 12,
        Consumables = 13,
        Droid = 14,
        CraftingMaterials = 15,
        CraftingRecipes = 16,
        CompanionTools = 17,
        Misc = 18
    }

    public static class AuctionCategoryExtensions
    {
        public static AuctionCategory ToAuctionCategory(this string str)
        {
            switch (str)
            {
                case "itmAuctionCategoryNone": return AuctionCategory.None;
                case "itmAuctionCategoryArmorLight": return AuctionCategory.ArmorLight;
                case "itmAuctionCategoryArmorMedium": return AuctionCategory.ArmorMedium;
                case "itmAuctionCategoryArmorHeavy": return AuctionCategory.ArmorHeavy;
                case "itmAuctionCategoryWeaponMelee": return AuctionCategory.WeaponMelee;
                case "itmAuctionCategoryWeaponRanged": return AuctionCategory.WeaponRanged;
                case "itmAuctionCategoryImplant": return AuctionCategory.Implant;
                case "itmAuctionCategoryEarpiece": return AuctionCategory.Earpiece;
                case "itmAuctionCategoryShield": return AuctionCategory.Shield;
                case "itmAuctionCategoryFocus": return AuctionCategory.Focus;
                case "itmAuctionCategoryRelic": return AuctionCategory.Relic;
                case "itmAuctionCategoryGadget": return AuctionCategory.Gadget;
                case "itmAuctionCategoryItemModification": return AuctionCategory.ItemModification;
                case "itmAuctionCategoryConsumables": return AuctionCategory.Consumables;
                case "itmAuctionCategoryDroid": return AuctionCategory.Droid;
                case "itmAuctionCategoryCraftingMaterials": return AuctionCategory.CraftingMaterials;
                case "itmAuctionCategoryCraftingRecipes": return AuctionCategory.CraftingRecipes;
                case "itmAuctionCategoryCompanionTools": return AuctionCategory.CompanionTools;
                case "itmAuctionCategoryMisc": return AuctionCategory.Misc;
                default: throw new InvalidOperationException("Invalid AuctionCategory: " + str);
            }
        }
    }*/

    public class AuctionCategory : PseudoGameObject, IEquatable<AuctionCategory>
    {
        public AuctionCategory(DataObjectModel dom, int id)
        {
            Dom_ = dom;
            Id = id;
        }

        //public string Name { get; set; } //str.gui.tooltips 836131348283392
        public long NameId { get; set; }
        public Dictionary<string, string> LocalizedName { get; set; }
        [JsonIgnore]
        public bool SerializeSubCats { get; set; }
        public bool ShouldSerializeSubCategories() { return SerializeSubCats; }
        public Dictionary<int, AuctionSubCategory> SubCategories { get; set; }

        public static Dictionary<long, AuctionCategory> AuctionCategoryList;
        public static void Flush()
        {
            AuctionCategoryList = null;
        }

        public override string ToString()
        {
            return Name;
        }

        public override int GetHashCode()
        {
            int hash = Id.GetHashCode();
            hash ^= NameId.GetHashCode();
            if (Name != null) hash ^= Name.GetHashCode();
            if (LocalizedName != null) foreach (var x in LocalizedName) { hash ^= x.GetHashCode(); }
            if (SubCategories != null) foreach (var x in SubCategories) { hash ^= x.GetHashCode(); }
            return hash;
        }

        public override bool Equals(object obj)
        {
            if (obj == null) return false;
            if (ReferenceEquals(this, obj)) return true;

            if (!(obj is AuctionCategory obj2)) return false;

            return Equals(obj2);
        }

        public bool Equals(AuctionCategory obj)
        {
            if (obj == null) return false;
            if (ReferenceEquals(this, obj)) return true;

            if (GetHashCode() != obj.GetHashCode())
                return false;
            return true;
        }

        public static void Load(DataObjectModel dom)
        {
            if (AuctionCategoryList == null)
            {
                GomObject gom = dom.GetObject("ahItemCategoriesPrototype");
                Dictionary<object, object> ahItemCategoriesIndexToCategoryMap = gom.Data.Get<Dictionary<object, object>>("ahItemCategoriesIndexToCategoryMap");
                _ = gom.Data.Get<Dictionary<object, object>>("ahItemCategoriesSIDToIndexMap");
                gom.Unload();
                AuctionCategoryList = new Dictionary<long, AuctionCategory>();
                foreach (var kvp in ahItemCategoriesIndexToCategoryMap)
                {
                    AuctionCategory itm = new AuctionCategory(dom, Convert.ToInt32(kvp.Key));
                    Load(itm, (GomObjectData)kvp.Value);
                    AuctionCategoryList.Add(Convert.ToInt32(kvp.Key), itm);
                }
            }
        }
        public static AuctionCategory Load(DataObjectModel dom, int id)
        {
            if (dom == null || id == 0) return null;
            Load(dom);
            AuctionCategoryList.TryGetValue(id, out AuctionCategory ret);

            return ret;
        }
        internal static AuctionCategory Load(AuctionCategory itm, GomObjectData gom)
        {
            itm.NameId = gom.ValueOrDefault<long>("ahItemCategoryLocId", 0);
            itm.LocalizedName = itm.Dom_.stringTable.TryGetLocalizedStrings("str.gui.auctionhouse", itm.NameId);
            if (itm.NameId == 0)
                itm.Name = "Root";
            else
                itm.Name = itm.Dom_.stringTable.TryGetString("str.gui.auctionhouse", itm.NameId);

            List<object> subCats = gom.ValueOrDefault("ahItemCategorySubCategories", new List<object>());

            itm.SubCategories = new Dictionary<int, AuctionSubCategory>();
            foreach (var obj in subCats)
            {
                AuctionSubCategory.Load(itm.Dom_);
                if (AuctionSubCategory.AuctionSubCategorySIdList.TryGetValue(Convert.ToInt64(obj), out AuctionSubCategory sub))
                    itm.SubCategories.Add(sub.IntId, sub);
            }
            return itm;
        }
    }

    public class AuctionSubCategory : PseudoGameObject, IEquatable<AuctionSubCategory>
    {
        public AuctionSubCategory(DataObjectModel dom, int id, long sId)
        {
            Dom_ = dom;
            Id = id;
            IntId = id;
            SId = sId;
            Prototype = "ahItemSubCategoriesPrototype";
            ProtoDataTable = "ahItemSubCategoriesIndexToLocIdMap";
        }

        public int IntId { get; set; }
        public long SId { get; set; }
        //public string Name { get; set; } //str.gui.tooltips 836131348283392
        public long NameId { get; set; }
        public Dictionary<string, string> LocalizedName { get; set; }
        public List<AuctionItemSlot> SlotCategories { get; set; }

        public static Dictionary<int, AuctionSubCategory> AuctionSubCategoryList;
        public static Dictionary<long, AuctionSubCategory> AuctionSubCategorySIdList;
        public static void Flush()
        {
            AuctionSubCategoryList = null;
            AuctionSubCategorySIdList = null;
        }

        public override string ToString()
        {
            return System.Text.RegularExpressions.Regex.Replace(Name, @"\r\n?|\n", "<br />");
        }

        public override int GetHashCode()
        {
            int hash = Id.GetHashCode();
            hash ^= NameId.GetHashCode();
            if (Name != null) hash ^= Name.GetHashCode();
            if (LocalizedName != null) foreach (var x in LocalizedName) { hash ^= x.GetHashCode(); }
            if (SlotCategories != null) foreach (var x in SlotCategories) { hash ^= x.GetHashCode(); }
            return hash;
        }

        public override bool Equals(object obj)
        {
            if (obj == null) return false;
            if (ReferenceEquals(this, obj)) return true;

            if (!(obj is AuctionSubCategory obj2)) return false;

            return Equals(obj2);
        }

        public bool Equals(AuctionSubCategory obj)
        {
            if (obj == null) return false;
            if (ReferenceEquals(this, obj)) return true;

            if (GetHashCode() != obj.GetHashCode())
                return false;
            return true;
        }

        public static void Load(DataObjectModel dom)
        {
            if (AuctionSubCategoryList == null)
            {
                GomObject gom = dom.GetObject("ahItemSubCategoriesPrototype");
                Dictionary<object, object> ahItemSubCategoriesIndexToLocIdMap = gom.Data.Get<Dictionary<object, object>>("ahItemSubCategoriesIndexToLocIdMap");
                Dictionary<object, object> ahItemSubCategoriesSIDToIndexMap = gom.Data.Get<Dictionary<object, object>>("ahItemSubCategoriesSIDToIndexMap");
                gom.Unload();
                AuctionSubCategoryList = new Dictionary<int, AuctionSubCategory>();
                AuctionSubCategorySIdList = new Dictionary<long, AuctionSubCategory>();
                foreach (var kvp in ahItemSubCategoriesSIDToIndexMap)
                {
                    AuctionSubCategory itm = new AuctionSubCategory(dom, Convert.ToInt32(kvp.Value), Convert.ToInt64(kvp.Key));
                    Load(itm, (long)(ahItemSubCategoriesIndexToLocIdMap[kvp.Value]));
                    AuctionSubCategorySIdList.Add(Convert.ToInt64(kvp.Key), itm);
                    AuctionSubCategoryList.Add(itm.IntId, itm);
                }
            }
        }
        public static AuctionSubCategory Load(DataObjectModel dom, int id)
        {
            if (dom == null || id == 0) return null;
            Load(dom);
            AuctionSubCategoryList.TryGetValue(id, out AuctionSubCategory ret);

            return ret;
        }
        internal static AuctionSubCategory Load(AuctionSubCategory itm, long nameId)
        {
            itm.NameId = nameId;
            itm.LocalizedName = itm.Dom_.stringTable.TryGetLocalizedStrings("str.gui.auctionhouse", itm.NameId);
            if (itm.NameId == 0)
                itm.Name = "Any";
            else
                itm.Name = itm.Dom_.stringTable.TryGetString("str.gui.auctionhouse", itm.NameId);

            GomObject gom = itm.Dom_.GetObject("ahItemSlotCategoriesPrototype");
            if (gom != null)
            {
                Dictionary<object, object> ahItemSlotCategories = gom.Data.Get<Dictionary<object, object>>("ahItemSlotCategories");
                Dictionary<object, object> ahItemSlotConSlotNameIds = gom.Data.Get<Dictionary<object, object>>("ahItemSlotConSlotNameIds");
                itm.SlotCategories = new List<AuctionItemSlot>();
                if (ahItemSlotCategories.ContainsKey(itm.Id))
                {
                    foreach (ScriptEnum obj in (List<ScriptEnum>)ahItemSlotCategories[itm.SId])
                    {
                        if (!AuctionItemSlot.AuctionItemSlotList.TryGetValue(obj.ToString(), out AuctionItemSlot sub))
                            sub = new AuctionItemSlot(itm.Dom_, obj.ToString(), (long)(ahItemSlotConSlotNameIds[obj.ToString()]));
                        itm.SlotCategories.Add(sub);
                    }
                }
            }
            return itm;
        }
    }

    public class AuctionItemSlot : IEquatable<AuctionItemSlot>
    {
        public AuctionItemSlot(DataObjectModel dom, string id, long nameId)
        {
            Dom_ = dom;
            Id = id;
            NameId = nameId;
            LocalizedName = Dom_.stringTable.TryGetLocalizedStrings("str.gui.auctionhouse", NameId);
            Name = Dom_.stringTable.TryGetString("str.gui.auctionhouse", NameId);
            if (!AuctionItemSlotList.ContainsKey(id))
                AuctionItemSlotList.Add(id, this);
        }

        [JsonIgnore]
        public DataObjectModel Dom_ { get; set; }

        public string Id { get; set; }
        public string Name { get; set; } //str.gui.tooltips 836131348283392
        public long NameId { get; set; }
        public Dictionary<string, string> LocalizedName { get; set; }

        public static Dictionary<string, AuctionItemSlot> AuctionItemSlotList = new Dictionary<string, AuctionItemSlot>();

        public override string ToString()
        {
            return Name;
        }

        public override int GetHashCode()
        {
            int hash = Id.GetHashCode();
            hash ^= NameId.GetHashCode();
            if (Name != null) hash ^= Name.GetHashCode();
            if (LocalizedName != null) foreach (var x in LocalizedName) { hash ^= x.GetHashCode(); }
            return hash;
        }

        public override bool Equals(object obj)
        {
            if (obj == null) return false;
            if (ReferenceEquals(this, obj)) return true;

            if (!(obj is AuctionItemSlot obj2)) return false;

            return Equals(obj2);
        }

        public bool Equals(AuctionItemSlot obj)
        {
            if (obj == null) return false;
            if (ReferenceEquals(this, obj)) return true;

            if (GetHashCode() != obj.GetHashCode())
                return false;
            return true;
        }
    }
}
