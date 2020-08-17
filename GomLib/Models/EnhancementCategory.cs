using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GomLib.Models
{
    public enum EnhancementCategory
    {
        None = 0,
        LightSaber = 1,
        Armor = 2,
        Blaster = 3,
        VibroPrimary = 4,
        Misc = 5
    }

    public static class EnhancementCategoryExtensions
    {
        public static EnhancementCategory ToEnhancementCategory(this ScriptEnum val)
        {
            if (val == null) { return ToEnhancementCategory(String.Empty); }
            return ToEnhancementCategory(val.ToString());
        }

        public static EnhancementCategory ToEnhancementCategory(this string str)
        {
            if (String.IsNullOrEmpty(str)) return EnhancementCategory.None;

            switch (str)
            {
                case "itmEnhancementCategoryNone": return EnhancementCategory.None;
                case "itmEnhancementCategoryLightSaber": return EnhancementCategory.LightSaber;
                case "itmEnhancementCategoryArmor": return EnhancementCategory.Armor;
                case "itmEnhancementCategoryBlaster": return EnhancementCategory.Blaster;
                case "itmEnhancementCategoryVibroPrimary": return EnhancementCategory.VibroPrimary;
                case "itmEnhancementCategoryMisc": return EnhancementCategory.Misc;
                default: throw new InvalidOperationException("Invalid EnhancementCategory: " + str);
            }
        }
    }
}
