using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GomLib.Models
{
    public enum EnhancementSubCategory
    {
        None = 0,
        Lightsaber = 1,
        DoubleBladedLightsaber = 2,
        VibroSword = 3,
        VibroKnife = 4,
        VibroStaff = 5,
        BlasterRifle = 6,
        BlasterPistol = 7,
        SniperRifle = 8,
        AssaultCannon = 9,
        HeadHelmet = 10,
        HeadMask = 11,
        Chest = 12,
        Legs = 13,
        Hands = 14,
        Wrists = 15,
        Waist = 16,
        Feet = 17,
        Gadget = 18,
        ScatterGun = 19,
        ShieldGenerator = 20,
        Focus = 21,
        Gauntlet = 22,
        Relic = 23,
        Earpiece = 24,
        Implant = 25,
        Tool = 26
    }

    public static class EnhancementSubCategoryExtensions
    {
        public static EnhancementSubCategory ToEnhancementSubCategory(this ScriptEnum val)
        {
            if (val == null) { return ToEnhancementSubCategory(String.Empty); }
            return ToEnhancementSubCategory(val.ToString());
        }

        public static EnhancementSubCategory ToEnhancementSubCategory(this string str)
        {
            if (String.IsNullOrEmpty(str)) return EnhancementSubCategory.None;

            switch (str)
            {
                case "itmEnhancementSubCategoryNone": return EnhancementSubCategory.None;
                case "itmEnhancementSubCategoryLightsaber": return EnhancementSubCategory.Lightsaber;
                case "itmEnhancementSubCategoryDoubleBladedLightsaber": return EnhancementSubCategory.DoubleBladedLightsaber;
                case "itmEnhancementSubCategoryVibroSword": return EnhancementSubCategory.VibroSword;
                case "itmEnhancementSubCategoryVibroKnife": return EnhancementSubCategory.VibroKnife;
                case "itmEnhancementSubCategoryVibroStaff": return EnhancementSubCategory.VibroStaff;
                case "itmEnhancementSubCategoryBlasterRifle": return EnhancementSubCategory.BlasterRifle;
                case "itmEnhancementSubCategoryBlasterPistol": return EnhancementSubCategory.BlasterPistol;
                case "itmEnhancementSubCategorySniperRifle": return EnhancementSubCategory.SniperRifle;
                case "itmEnhancementSubCategoryAssaultCannon": return EnhancementSubCategory.AssaultCannon;
                case "itmEnhancementSubCategoryHeadHelmet": return EnhancementSubCategory.HeadHelmet;
                case "itmEnhancementSubCategoryHeadMask": return EnhancementSubCategory.HeadMask;
                case "itmEnhancementSubCategoryChest": return EnhancementSubCategory.Chest;
                case "itmEnhancementSubCategoryLegs": return EnhancementSubCategory.Legs;
                case "itmEnhancementSubCategoryHands": return EnhancementSubCategory.Hands;
                case "itmEnhancementSubCategoryWrists": return EnhancementSubCategory.Wrists;
                case "itmEnhancementSubCategoryWaist": return EnhancementSubCategory.Waist;
                case "itmEnhancementSubCategoryFeet": return EnhancementSubCategory.Feet;
                case "itmEnhancementSubCategoryGadget": return EnhancementSubCategory.Gadget;
                case "itmEnhancementSubCategoryScatterGun": return EnhancementSubCategory.ScatterGun;
                case "itmEnhancementSubCategoryShieldGenerator": return EnhancementSubCategory.ShieldGenerator;
                case "itmEnhancementSubCategoryFocus": return EnhancementSubCategory.Focus;
                case "itmEnhancementSubCategoryGauntlet": return EnhancementSubCategory.Gauntlet;
                case "itmEnhancementSubCategoryRelic": return EnhancementSubCategory.Relic;
                case "itmEnhancementSubCategoryEarpiece": return EnhancementSubCategory.Earpiece;
                case "itmEnhancementSubCategoryImplant": return EnhancementSubCategory.Implant;
                case "itmEnhancementSubCategoryTool": return EnhancementSubCategory.Tool;
                default: throw new InvalidOperationException("Unknown EnhancementSubCategory: " + str);
            }
        }
    }
}
