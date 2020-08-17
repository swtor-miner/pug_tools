using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GomLib.Models
{
    public enum GiftInterest
    {
        None = 0,
        Indifferent = 1,
        Like = 2,
        Favorite = 3,
        Love = 4
    }

    public static class GiftInterestExtensions
    {
        public static GiftInterest ToGiftInterest(this ScriptEnum val)
        {
            if (val == null) { return ToGiftInterest(String.Empty); }
            return ToGiftInterest(val.ToString());
        }

        public static GiftInterest ToGiftInterest(this string str)
        {
            if (String.IsNullOrEmpty(str)) { return GiftInterest.None; }
            str = str.Split(' ')[0]; // If there are multiple types, separated by spaces only take the first one

            switch (str)
            {
                case "chrCompanionGiftNone":
                case "none": return GiftInterest.None;
                case "chrCompanionGiftIndifferent":
                case "indifferent": return GiftInterest.Indifferent;
                case "chrCompanionGiftLike":
                case "like": return GiftInterest.Like;
                case "chrCompanionGiftFavorite":
                case "favorite": return GiftInterest.Favorite;
                case "chrCompanionGiftLove":
                case "love": return GiftInterest.Love;
                default: throw new InvalidOperationException("Unknown GiftInterest: " + str);
            }
        }
    }
}
