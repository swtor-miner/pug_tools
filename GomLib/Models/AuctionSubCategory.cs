using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GomLib.Models
{
    public enum AuctionSubCategory
    {
        None = 0,
        ArmorHead = 1,
        ArmorHands = 2,
        ArmorChest = 3,
        ArmorWaist = 4,
        ArmorWrist = 5,
        ArmorLegs = 6,
        ArmorFeet = 7,
        WeaponDoubleBladedLightsaber = 8,
        WeaponElectrostaff = 9,
        WeaponGauntlet = 10,
        WeaponLightsaber = 11,
        WeaponVibroblade = 12,
        WeaponVibroknife = 13,
        WeaponAssaultCannon = 14,
        WeaponBlasterPistol = 15,
        WeaponBlasterRifle = 16,
        WeaponShotgun = 17,
        WeaponSniperRifle = 18,
        EnhancementArmor = 19,
        EnhancementWeaponLightsaber = 20,
        EnhancementWeaponVibro = 21,
        EnhancementWeaponRanged = 22,
        EnhancementOverlay = 23,
        EnhancementUnderlay = 24,
        EnhancementCircuitry = 25,
        EnhancementColorCrystal = 26,
        EnhancementPowerCrystal = 27,
        EnhancementFocusLens = 28,
        EnhancementEmitterMatrix = 29,
        EnhancementTrigger = 30,
        EnhancementScope = 31,
        EnhancementBarrel = 32,
        EnhancementModulator = 33,
        EnhancementPowerCell = 34,
        EnhancementHilt = 35
    }

    public static class AuctionSubCategoryExtensions
    {
        public static AuctionSubCategory ToAuctionSubCategory(this string str)
        {
            switch (str)
            {
                case "itmAuctionSubcategoryNone": return AuctionSubCategory.None;
                case "itmAuctionSubcategoryArmorHead": return AuctionSubCategory.ArmorHead;
                case "itmAuctionSubcategoryArmorHands": return AuctionSubCategory.ArmorHands;
                case "itmAuctionSubcategoryArmorChest": return AuctionSubCategory.ArmorChest;
                case "itmAuctionSubcategoryArmorWaist": return AuctionSubCategory.ArmorWaist;
                case "itmAuctionSubcategoryArmorWrist": return AuctionSubCategory.ArmorWrist;
                case "itmAuctionSubcategoryArmorLegs": return AuctionSubCategory.ArmorLegs;
                case "itmAuctionSubcategoryArmorFeet": return AuctionSubCategory.ArmorFeet;
                case "itmAuctionSubcategoryWeaponDoubleBladedLightsaber": return AuctionSubCategory.WeaponDoubleBladedLightsaber;
                case "itmAuctionSubcategoryWeaponElectrostaff": return AuctionSubCategory.WeaponElectrostaff;
                case "itmAuctionSubcategoryWeaponGauntlet": return AuctionSubCategory.WeaponGauntlet;
                case "itmAuctionSubcategoryWeaponLightsaber": return AuctionSubCategory.WeaponLightsaber;
                case "itmAuctionSubcategoryWeaponVibroblade": return AuctionSubCategory.WeaponVibroblade;
                case "itmAuctionSubcategoryWeaponVibroknife": return AuctionSubCategory.WeaponVibroknife;
                case "itmAuctionSubcategoryWeaponAssaultCannon": return AuctionSubCategory.WeaponAssaultCannon;
                case "itmAuctionSubcategoryWeaponBlasterPistol": return AuctionSubCategory.WeaponBlasterPistol;
                case "itmAuctionSubcategoryWeaponBlasterRifle": return AuctionSubCategory.WeaponBlasterRifle;
                case "itmAuctionSubcategoryWeaponShotgun": return AuctionSubCategory.WeaponShotgun;
                case "itmAuctionSubcategoryWeaponSniperRifle": return AuctionSubCategory.WeaponSniperRifle;
                case "itmAuctionSubcategoryEnhancementArmor": return AuctionSubCategory.EnhancementArmor;
                case "itmAuctionSubcategoryEnhancementWeaponLightsaber": return AuctionSubCategory.EnhancementWeaponLightsaber;
                case "itmAuctionSubcategoryEnhancementWeaponVibro": return AuctionSubCategory.EnhancementWeaponVibro;
                case "itmAuctionSubcategoryEnhancementWeaponRanged": return AuctionSubCategory.EnhancementWeaponRanged;
                case "itmAuctionSubcategoryEnhancementOverlay": return AuctionSubCategory.EnhancementOverlay;
                case "itmAuctionSubcategoryEnhancementUnderlay": return AuctionSubCategory.EnhancementUnderlay;
                case "itmAuctionSubcategoryEnhancementCircuitry": return AuctionSubCategory.EnhancementCircuitry;
                case "itmAuctionSubcategoryEnhancementColorCrystal": return AuctionSubCategory.EnhancementColorCrystal;
                case "itmAuctionSubcategoryEnhancementPowerCrystal": return AuctionSubCategory.EnhancementPowerCrystal;
                case "itmAuctionSubcategoryEnhancementFocusLens": return AuctionSubCategory.EnhancementFocusLens;
                case "itmAuctionSubcategoryEnhancementEmitterMatrix": return AuctionSubCategory.EnhancementEmitterMatrix;
                case "itmAuctionSubcategoryEnhancementTrigger": return AuctionSubCategory.EnhancementTrigger;
                case "itmAuctionSubcategoryEnhancementScope": return AuctionSubCategory.EnhancementScope;
                case "itmAuctionSubcategoryEnhancementBarrel": return AuctionSubCategory.EnhancementBarrel;
                case "itmAuctionSubcategoryEnhancementModulator": return AuctionSubCategory.EnhancementModulator;
                case "itmAuctionSubcategoryEnhancementPowerCell": return AuctionSubCategory.EnhancementPowerCell;
                case "itmAuctionSubcategoryEnhancementHilt": return AuctionSubCategory.EnhancementHilt;
                default: throw new InvalidOperationException("Invalid AuctionSubCategory: " + str);
            }
        }
    }
}
