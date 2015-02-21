using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GomLib.Models
{
    public enum ItemQuality
    {
        Cheap = 0,
        Standard = 1,
        Premium = 2,
        Prototype = 3,
        Artifact = 4,
        Legendary = 5,
        Legacy = 6,
        Quest = 7,
        Currency = 8,
        Moddable = 9
    }

    public static class ItemQualityExtensions
    {
        public static ItemQuality ToItemQuality(this ScriptEnum val)
        {
            if (val == null) { return ToItemQuality(String.Empty); }
            return ToItemQuality(val.ToString());
        }

        public static ItemQuality ToItemQuality(this string str)
        {
            if (String.IsNullOrEmpty(str)) return ItemQuality.Cheap;

            switch (str)
            {
                case "itmQualityCheap": return ItemQuality.Cheap;
                case "itmQualityStandard": return ItemQuality.Standard;
                case "itmQualityPremium": return ItemQuality.Premium;
                case "itmQualityPrototype": return ItemQuality.Prototype;
                case "itmQualityArtifact": return ItemQuality.Artifact;
                case "itmQualityLegendary": return ItemQuality.Legendary;
                case "itmQualityLegacy": return ItemQuality.Legacy;
                case "itmQualityQuest": return ItemQuality.Quest;
                case "itmQualityCurrency": return ItemQuality.Currency;
                case "itmQualityModdable": return ItemQuality.Moddable;
                default: throw new InvalidOperationException("Invalid Quality: " + str);
            }
        }
    }
}
