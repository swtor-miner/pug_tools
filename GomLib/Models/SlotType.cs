using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GomLib.Models
{
    public enum SlotType
    {
        Inventory = 0,
        Bank = 1,
        EquipHumanMainHand = 2,
        EquipHumanOffHand = 3,
        EquipHumanWrist = 4,
        EquipHumanBelt = 5,
        EquipHumanChest = 6,
        EquipHumanEar = 7,
        EquipHumanFace = 8,
        EquipHumanFoot = 9,
        EquipHumanGlove = 10,
        EquipHumanImplant = 11,
        EquipHumanLeg = 12,
        EquipDroidUpper = 13,
        EquipDroidLower = 14,
        EquipDroidShield = 15,
        EquipDroidGyro = 16,
        EquipDroidUtility = 17,
        EquipDroidSensor = 18,
        EquipDroidSpecial = 19,
        EquipDroidWeapon1 = 20,
        EquipDroidWeapon2 = 21,
        Upgrade = 22,
        EffectPositive = 23,
        EffectNegative = 24,
        Quickslot = 25,
        Abilities = 26,
        Timers = 27,
        EquipHumanRanged = 28,
        EffectOther = 29,
        Loot = 30,
        EquipHumanHeirloom = 31,
        Invalid = 32,
        EquipHumanRangedPrimary = 33,
        EquipHumanRangedSecondary = 34,
        EquipHumanRangedTertiary = 35,
        Buyback = 36,
        EquipHumanCustomRanged = 37,
        EquipHumanCustomMelee = 38,
        EquipHumanShield = 39,
        EquipHumanOutfit = 40,
        EquipDroidLeg = 41,
        EquipDroidFeet = 42,
        EquipDroidOutfit = 43,
        EquipDroidChest = 44,
        EquipDroidHand = 45,
        EquipHumanLightSide = 46,
        EquipHumanDarkSide = 47,
        EquipHumanRelic = 48,
        EquipHumanFocus = 49,
        Quest = 50,
        EquipSpaceShipArmor = 51,
        EquipSpaceBeamGenerator = 52,
        EquipSpaceBeamCharger = 53,
        EquipSpaceEnergyShield = 54,
        EquipSpaceShieldRegenerator = 55,
        EquipSpaceMissileMagazine = 56,
        EquipSpaceProtonTorpedoes = 57,
        EquipSpaceAbilityDefense = 58,
        EquipSpaceAbilityOffense = 59,
        EquipSpaceAbilitySystems = 60,
        EquipSpaceShipAbilityDefense = 61,
        EquipSpaceShipAbilityOffense = 62,
        EquipSpaceShipAbilitySystems = 63,
        Any = 64,
        QuestTracker = 65,
        EquipHumanTactical = 66
    }

    public static class SlotTypeExtensions
    {
        public static SlotType ToSlotType(this ScriptEnum val)
        {
            if (val == null) { return ToSlotType(string.Empty); }
            return ToSlotType(val.ToString());
        }

        public static SlotType ToSlotType(this string str)
        {
            if (string.IsNullOrEmpty(str)) return SlotType.Invalid;

            switch (str)
            {
                case "conSlotInventory": return SlotType.Inventory;
                case "conSlotBank": return SlotType.Bank;
                case "conSlotEquipHumanMainHand": return SlotType.EquipHumanMainHand;
                case "conSlotEquipHumanOffHand": return SlotType.EquipHumanOffHand;
                case "conSlotEquipHumanWrist": return SlotType.EquipHumanWrist;
                case "conSlotEquipHumanBelt": return SlotType.EquipHumanBelt;
                case "conSlotEquipHumanChest": return SlotType.EquipHumanChest;
                case "conSlotEquipHumanEar": return SlotType.EquipHumanEar;
                case "conSlotEquipHumanFace": return SlotType.EquipHumanFace;
                case "conSlotEquipHumanFoot": return SlotType.EquipHumanFoot;
                case "conSlotEquipHumanGlove": return SlotType.EquipHumanGlove;
                case "conSlotEquipHumanImplant": return SlotType.EquipHumanImplant;
                case "conSlotEquipHumanLeg": return SlotType.EquipHumanLeg;
                case "conSlotEquipDroidUpper": return SlotType.EquipDroidUpper;
                case "conSlotEquipDroidLower": return SlotType.EquipDroidLower;
                case "conSlotEquipDroidShield": return SlotType.EquipDroidShield;
                case "conSlotEquipDroidGyro": return SlotType.EquipDroidGyro;
                case "conSlotEquipDroidUtility": return SlotType.EquipDroidUtility;
                case "conSlotEquipDroidSensor": return SlotType.EquipDroidSensor;
                case "conSlotEquipDroidSpecial": return SlotType.EquipDroidSpecial;
                case "conSlotEquipDroidWeapon1": return SlotType.EquipDroidWeapon1;
                case "conSlotEquipDroidWeapon2": return SlotType.EquipDroidWeapon2;
                case "conSlotUpgrade": return SlotType.Upgrade;
                case "conSlotEffectPositive": return SlotType.EffectPositive;
                case "conSlotEffectNegative": return SlotType.EffectNegative;
                case "conSlotQuickslot": return SlotType.Quickslot;
                case "conSlotAbilities": return SlotType.Abilities;
                case "conSlotTimers": return SlotType.Timers;
                case "conSlotEquipHumanRanged": return SlotType.EquipHumanRanged;
                case "conSlotEffectOther": return SlotType.EffectOther;
                case "conSlotLoot": return SlotType.Loot;
                case "conSlotEquipHumanHeirloom": return SlotType.EquipHumanHeirloom;
                case "conSlotInvalid": return SlotType.Invalid;
                case "conSlotEquipHumanRangedPrimary": return SlotType.EquipHumanRangedPrimary;
                case "conSlotEquipHumanRangedSecondary": return SlotType.EquipHumanRangedSecondary;
                case "conSlotEquipHumanRangedTertiary": return SlotType.EquipHumanRangedTertiary;
                case "conSlotBuyback": return SlotType.Buyback;
                case "conSlotEquipHumanCustomRanged": return SlotType.EquipHumanCustomRanged;
                case "conSlotEquipHumanCustomMelee": return SlotType.EquipHumanCustomMelee;
                case "conSlotEquipHumanShield": return SlotType.EquipHumanShield;
                case "conSlotEquipHumanOutfit": return SlotType.EquipHumanOutfit;
                case "conSlotEquipDroidLeg": return SlotType.EquipDroidLeg;
                case "conSlotEquipDroidFeet": return SlotType.EquipDroidFeet;
                case "conSlotEquipDroidOutfit": return SlotType.EquipDroidOutfit;
                case "conSlotEquipDroidChest": return SlotType.EquipDroidChest;
                case "conSlotEquipDroidHand": return SlotType.EquipDroidHand;
                case "conSlotEquipHumanLightSide": return SlotType.EquipHumanLightSide;
                case "conSlotEquipHumanDarkSide": return SlotType.EquipHumanDarkSide;
                case "conSlotEquipHumanRelic": return SlotType.EquipHumanRelic;
                case "conSlotEquipHumanFocus": return SlotType.EquipHumanFocus;
                case "conSlotQuest": return SlotType.Quest;
                case "conSlotEquipSpaceShipArmor": return SlotType.EquipSpaceShipArmor;
                case "conSlotEquipSpaceBeamGenerator": return SlotType.EquipSpaceBeamGenerator;
                case "conSlotEquipSpaceBeamCharger": return SlotType.EquipSpaceBeamCharger;
                case "conSlotEquipSpaceEnergyShield": return SlotType.EquipSpaceEnergyShield;
                case "conSlotEquipSpaceShieldRegenerator": return SlotType.EquipSpaceShieldRegenerator;
                case "conSlotEquipSpaceMissileMagazine": return SlotType.EquipSpaceMissileMagazine;
                case "conSlotEquipSpaceProtonTorpedoes": return SlotType.EquipSpaceProtonTorpedoes;
                case "conSlotEquipSpaceAbilityDefense": return SlotType.EquipSpaceAbilityDefense;
                case "conSlotEquipSpaceAbilityOffense": return SlotType.EquipSpaceAbilityOffense;
                case "conSlotEquipSpaceAbilitySystems": return SlotType.EquipSpaceAbilitySystems;
                case "conSlotEquipSpaceShipAbilityDefense": return SlotType.EquipSpaceShipAbilityDefense;
                case "conSlotEquipSpaceShipAbilityOffense": return SlotType.EquipSpaceShipAbilityOffense;
                case "conSlotEquipSpaceShipAbilitySystems": return SlotType.EquipSpaceShipAbilitySystems;
                case "conSlotAny": return SlotType.Any;
                case "conSlotQuestTracker": return SlotType.QuestTracker;
                case "conSlotEquipHumanTactical": return SlotType.EquipHumanTactical; //fix this later
                case "<None>":
                    return SlotType.Invalid;
                default: throw new InvalidOperationException("Invalid SlotType: " + str);
            }
        }

        public static bool IgnoreSlot(SlotType slotType)
        {
            // These damn slots are in every item!
            switch (slotType)
            {
                case SlotType.Loot:
                case SlotType.Buyback:
                case SlotType.Bank:
                case SlotType.Inventory:
                case SlotType.Quest:
                    return true;
                default: return false;
            }
        }
    }
}
