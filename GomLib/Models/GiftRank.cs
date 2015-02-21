using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GomLib.Models
{
    public enum GiftRank
    {
        None = 0,
        Rank1 = 1,
        Rank2 = 2,
        Rank3 = 3,
        Rank4 = 4,
        Rank5 = 5
    }

    public static class GiftRankExtensions
    {
        public static GiftRank ToGiftRank(this ScriptEnum val)
        {
            if (val == null) { return ToGiftRank(String.Empty); }
            return ToGiftRank(val.ToString());
        }

        public static GiftRank ToGiftRank(this string str)
        {
            if (String.IsNullOrEmpty(str)) return GiftRank.None;
            switch (str)
            {
                case "chrCompanionAffection_Rank1": return GiftRank.Rank1;
                case "chrCompanionAffection_Rank2": return GiftRank.Rank2;
                case "chrCompanionAffection_Rank3": return GiftRank.Rank3;
                case "chrCompanionAffection_Rank4": return GiftRank.Rank4;
                case "chrCompanionAffection_Rank5": return GiftRank.Rank5;
                default: throw new InvalidOperationException("Invalid GiftRank: " + str);
            }
        }
    }
}
