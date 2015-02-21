using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GomLib.Models
{
    public enum AuctionCategory
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
    }
}
