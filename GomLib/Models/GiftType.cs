using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GomLib.Models
{
    public enum GiftType
    {
        None = 0,
        Weapon = 1,
        MilitaryGear = 2,
        Courting = 3,
        Luxury = 4,
        Technology = 5,
        RepublicMemorabilia = 6,
        ImperialMemorabilia = 7,
        CulturalArtifact = 8,
        Trophy = 9,
        UnderworldGood = 10,
        Artwork = 11,
        Kitsch = 12,
        Pet = 13,
        Secret = 14
    }

    public static class GiftTypeExtensions
    {
        public static GiftType ToGiftType(this ScriptEnum val)
        {
            if (val == null) { return ToGiftType(String.Empty); }
            return ToGiftType(val.ToString());
        }

        public static GiftType ToGiftType(this string str)
        {
            if (String.IsNullOrEmpty(str)) return GiftType.None;

            switch (str)
            {
                case "itmGiftNone": return GiftType.None;
                case "itmGiftWeapon": return GiftType.Weapon;
                case "itmGiftMilitary_Gear": return GiftType.MilitaryGear;
                case "itmGiftCourting": return GiftType.Courting;
                case "itmGiftLuxury": return GiftType.Luxury;
                case "itmGiftTechnology": return GiftType.Technology;
                case "itmGiftRepublic_Memorabilia": return GiftType.RepublicMemorabilia;
                case "itmGiftImperial_Memorabilia": return GiftType.ImperialMemorabilia;
                case "itmGiftCultural_Artifact": return GiftType.CulturalArtifact;
                case "itmGiftTrophy": return GiftType.Trophy;
                case "itmGiftUnderworld_Good": return GiftType.UnderworldGood;
                case "itmGiftArtwork": return GiftType.Artwork;
                case "itmGiftKitsch": return GiftType.Kitsch;
                case "itmGiftPet": return GiftType.Pet;
                case "itmGiftSecret": return GiftType.Secret;
                default: throw new InvalidOperationException("Invalid GiftType: " + str);
            }
        }
    }
}
