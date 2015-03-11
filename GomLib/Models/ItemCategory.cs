using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GomLib.Models
{
    public enum ItemCategory
    {
        None = 0,
        Equipment = 1,
        Weapon = 2,
        ItemMod = 3,
        Crafting = 4,
        Consumable = 5,
        Other = 6
    }

    public enum ItemSubCategory
    {
        Undefined = 0,
        OtherQuestItem = 1,          // itm.location.*  (Low Priority)
        OtherLockbox = 2,            // itm.loot.lockbox.*
        OtherCustomization = 3,      // Contains Slot: EquipHumanOutfit
        OtherGift = 4,               // GiftType != None (High Priority)
        OtherCurrency = 5,           // itm.loot.token.*
        OtherMount = 6,              // itm.mount.*
        OtherPet = 7,                // itm.pet.*
        EquipmentLightArmor = 8,     // ArmorSpec == Light
        EquipmentMediumArmor = 9,    // ArmorSpec == Medium
        EquipmentHeavyArmor = 10,    // ArmorSpec == Heavy
        EquipmentDroidPart = 11,     // Slots Contains any droid slots (higher priority than EquipmentHeavyArmor)
        EquipmentEar = 12,           // Slots Contains EquipHumanEar
        EquipmentImplant = 13,       // Slots Contains EquipHumanImplant
        EquipmentRelic = 14,         // Slots Contains EquipHumanRelic
        EquipmentFocus = 15,         // ArmorSpec == Focus
        EquipmentGenerator = 16,     // ArmorSpec == Generator
        EquipmentShield = 17,        // ArmorSpec == Shield OR ArmorSpec == ShieldForce
        EquipmentSpace = 18,         // itm.space.*
        WeaponBlasterPistol = 19,    // WeaponSpec == Pistol
        WeaponBlasterRifle = 20,     // WeaponSpec == Rifle
        WeaponSniperRifle = 21,      // WeaponSpec == SniperRifle
        WeaponAssaultCannon = 22,    // WeaponSpec == AssaultCannon
        WeaponLightsaber = 23,       // WeaponSpec == Lightsaber
        WeaponPolesaber = 24,        // WeaponSpec == Polesaber
        WeaponVibroblade = 25,       // WeaponSpec == Vibroblade
        WeaponVibroknife = 26,       // WeaponSpec == Vibroknife
        WeaponShotgun = 27,          // WeaponSpec == Shotgun
        WeaponElectrostaff = 28,     // WeaponSpec == Electrostaff
        WeaponTrainingSaber = 29,    // WeaponSpec == TrainingSaber
        CraftingMaterial = 30,       // itm.mat.*
        CraftingSchematic = 31,      // itm.schem.*
        ConsumableAdrenal = 32,      // itm.potion.adrenal.*
        ConsumableMedpac = 33,       // itm.potion.medpac.*
        ConsumableStim = 34,         // itm.potion.stimpack.*
        ConsumableFood = 35,         // itm.potion.food.*
        ConsumablePvp = 36,          // itm.potion.pvp.*
        ConsumableGrenade = 37,      // itm.grenade.*
        ItemModColorCrystal = 38,    // EnhancementType == ColorCrystal
        ItemModArmoring = 39,        // EnhancementType == Harness
        ItemModAugment = 40,         // EnhancementType == Modulator
        ItemModMod = 41,             // EnhancementType == Overlay
        ItemModEnhancement = 42,     // EnhancementType == Support
        ItemModBarrel = 43,          // EnhancementType == Barrel
        ItemModHilt = 44,            // EnhancementType == PowerCrystal
        OtherUnknown = 45,           // Default
        WeaponTechstaff = 46,        // WeaponSpec == ElectrostaffTech
        WeaponTechblade = 47         // WeaponSpec == VibrobladeTech
    }

    public static class ItemSubCategoryExtensions
    {
        public static void SetCategory(Item itm)
        {
            itm.SubCategory = ItemSubCategory.OtherUnknown;
            if (itm.Fqn.StartsWith("itm.location.")) { itm.SubCategory = ItemSubCategory.OtherQuestItem; }
            if (itm.Fqn.StartsWith("itm.mat.")) { itm.SubCategory = ItemSubCategory.CraftingMaterial; }
            if (itm.Fqn.StartsWith("itm.schem.")) { itm.SubCategory = ItemSubCategory.CraftingSchematic; }
            if (itm.Fqn.StartsWith("itm.potion.adrenal.")) { itm.SubCategory = ItemSubCategory.ConsumableAdrenal; }
            if (itm.Fqn.StartsWith("itm.potion.medpac.")) { itm.SubCategory = ItemSubCategory.ConsumableMedpac; }
            if (itm.Fqn.StartsWith("itm.potion.stimpack.")) { itm.SubCategory = ItemSubCategory.ConsumableStim; }
            if (itm.Fqn.StartsWith("itm.potion.food.")) { itm.SubCategory = ItemSubCategory.ConsumableFood; }
            if (itm.Fqn.StartsWith("itm.potion.pvp.")) { itm.SubCategory = ItemSubCategory.ConsumablePvp; }
            if (itm.Fqn.StartsWith("itm.grenade.")) { itm.SubCategory = ItemSubCategory.ConsumableGrenade; }
            if (itm.Fqn.StartsWith("itm.space.")) { itm.SubCategory = ItemSubCategory.EquipmentSpace; }
            if (itm.Fqn.StartsWith("itm.mount.")) { itm.SubCategory = ItemSubCategory.OtherMount; }
            if (itm.Fqn.StartsWith("itm.pet.")) { itm.SubCategory = ItemSubCategory.OtherPet; }
            if (itm.Fqn.StartsWith("itm.loot.token.")) { itm.SubCategory = ItemSubCategory.OtherCurrency; }
            if (itm.Fqn.StartsWith("itm.loot.lockbox.")) { itm.SubCategory = ItemSubCategory.OtherLockbox; }

            switch (itm.EnhancementType)
            {
                case EnhancementType.ColorCrystal: itm.SubCategory = ItemSubCategory.ItemModColorCrystal; break;
                case EnhancementType.Harness: itm.SubCategory = ItemSubCategory.ItemModArmoring; break;
                case EnhancementType.Modulator: itm.SubCategory = ItemSubCategory.ItemModAugment; break;
                case EnhancementType.Overlay: itm.SubCategory = ItemSubCategory.ItemModMod; break;
                case EnhancementType.Support: itm.SubCategory = ItemSubCategory.ItemModEnhancement; break;
                case EnhancementType.Barrel: itm.SubCategory = ItemSubCategory.ItemModBarrel; break;
                case EnhancementType.PowerCrystal: itm.SubCategory = ItemSubCategory.ItemModHilt; break;
                default: break;
            }

            switch (itm.WeaponSpec)
            {
                case WeaponSpec.AssaultCannon: itm.SubCategory = ItemSubCategory.WeaponAssaultCannon; break;
                case WeaponSpec.Electrostaff: itm.SubCategory = ItemSubCategory.WeaponElectrostaff; break;
                case WeaponSpec.Lightsaber: itm.SubCategory = ItemSubCategory.WeaponLightsaber; break;
                case WeaponSpec.Pistol: itm.SubCategory = ItemSubCategory.WeaponBlasterPistol; break;
                case WeaponSpec.Polesaber: itm.SubCategory = ItemSubCategory.WeaponPolesaber; break;
                case WeaponSpec.Rifle: itm.SubCategory = ItemSubCategory.WeaponBlasterRifle; break;
                case WeaponSpec.Shotgun: itm.SubCategory = ItemSubCategory.WeaponShotgun; break;
                case WeaponSpec.SniperRifle: itm.SubCategory = ItemSubCategory.WeaponSniperRifle; break;
                case WeaponSpec.TrainingSaber: itm.SubCategory = ItemSubCategory.WeaponTrainingSaber; break;
                case WeaponSpec.Vibroblade: itm.SubCategory = ItemSubCategory.WeaponVibroblade; break;
                case WeaponSpec.Vibroknife: itm.SubCategory = ItemSubCategory.WeaponVibroknife; break;
                case WeaponSpec.VibrobladeTech: itm.SubCategory = ItemSubCategory.WeaponTechblade; break;
                case WeaponSpec.ElectrostaffTech: itm.SubCategory = ItemSubCategory.WeaponTechstaff; break;
                default: break;
            }

            switch (itm.ArmorSpec)
            {
                case ArmorSpec.Light: itm.SubCategory = ItemSubCategory.EquipmentLightArmor; break;
                case ArmorSpec.Medium: itm.SubCategory = ItemSubCategory.EquipmentMediumArmor; break;
                case ArmorSpec.Heavy: itm.SubCategory = ItemSubCategory.EquipmentHeavyArmor; break;
                case ArmorSpec.Focus: itm.SubCategory = ItemSubCategory.EquipmentFocus; break;
                case ArmorSpec.Generator: itm.SubCategory = ItemSubCategory.EquipmentGenerator; break;
                case ArmorSpec.Shield:
                case ArmorSpec.ShieldForce:
                    itm.SubCategory = ItemSubCategory.EquipmentShield; break;
                default: break;
            }

            if (itm.Slots.Contains(SlotType.EquipHumanEar)) { itm.SubCategory = ItemSubCategory.EquipmentEar; }
            if (itm.Slots.Contains(SlotType.EquipHumanImplant)) { itm.SubCategory = ItemSubCategory.EquipmentImplant; }
            if (itm.Slots.Contains(SlotType.EquipHumanRelic)) { itm.SubCategory = ItemSubCategory.EquipmentRelic; }
            if (itm.Slots.Contains(SlotType.EquipHumanOutfit)) { itm.SubCategory = ItemSubCategory.OtherCustomization; }
            if (itm.Slots.Contains(SlotType.EquipDroidUtility) || itm.Slots.Contains(SlotType.EquipDroidUpper) || itm.Slots.Contains(SlotType.EquipDroidSpecial) || itm.Slots.Contains(SlotType.EquipDroidShield) ||
                itm.Slots.Contains(SlotType.EquipDroidSensor) || itm.Slots.Contains(SlotType.EquipDroidLower) || itm.Slots.Contains(SlotType.EquipDroidLeg) ||
                itm.Slots.Contains(SlotType.EquipDroidHand) || itm.Slots.Contains(SlotType.EquipDroidGyro) || itm.Slots.Contains(SlotType.EquipDroidFeet) || itm.Slots.Contains(SlotType.EquipDroidChest))
            {
                itm.SubCategory = ItemSubCategory.EquipmentDroidPart;
            }

            if (itm.GiftType != GiftType.None) { itm.SubCategory = ItemSubCategory.OtherGift; }

            itm.Category = itm.SubCategory.ToItemCategory();
        }

        public static ItemCategory ToItemCategory(this ItemSubCategory sub)
        {
            switch (sub)
            {
                case ItemSubCategory.OtherUnknown:           // Default
                case ItemSubCategory.OtherQuestItem:         // itm.location.*  (Low Priority)
                case ItemSubCategory.OtherLockbox:           // itm.loot.lockbox.*
                case ItemSubCategory.OtherCustomization:     // Contains Slot: EquipHumanOutfit
                case ItemSubCategory.OtherGift:              // GiftType != None (High Priority)
                case ItemSubCategory.OtherCurrency:          // itm.loot.token.*
                case ItemSubCategory.OtherMount:             // itm.mount.*
                case ItemSubCategory.OtherPet:               // itm.pet.*
                    return ItemCategory.Other;
                case ItemSubCategory.EquipmentLightArmor:    // ArmorSpec == Light
                case ItemSubCategory.EquipmentMediumArmor:   // ArmorSpec == Medium
                case ItemSubCategory.EquipmentHeavyArmor:    // ArmorSpec == Heavy
                case ItemSubCategory.EquipmentDroidPart:     // Slots Contains any droid slots (higher priority than EquipmentHeavyArmor)
                case ItemSubCategory.EquipmentEar:           // Slots Contains EquipHumanEar
                case ItemSubCategory.EquipmentImplant:       // Slots Contains EquipHumanImplant
                case ItemSubCategory.EquipmentRelic:         // Slots Contains EquipHumanRelic
                case ItemSubCategory.EquipmentFocus:         // ArmorSpec == Focus
                case ItemSubCategory.EquipmentGenerator:     // ArmorSpec == Generator
                case ItemSubCategory.EquipmentShield:        // ArmorSpec == Shield OR ArmorSpec == ShieldForce
                case ItemSubCategory.EquipmentSpace:         // itm.space.*
                    return ItemCategory.Equipment;
                case ItemSubCategory.WeaponBlasterPistol:    // WeaponSpec == Pistol
                case ItemSubCategory.WeaponBlasterRifle:     // WeaponSpec == Rifle
                case ItemSubCategory.WeaponSniperRifle:      // WeaponSpec == SniperRifle
                case ItemSubCategory.WeaponAssaultCannon:    // WeaponSpec == AssaultCannon
                case ItemSubCategory.WeaponLightsaber:       // WeaponSpec == Lightsaber
                case ItemSubCategory.WeaponPolesaber:        // WeaponSpec == Polesaber
                case ItemSubCategory.WeaponVibroblade:       // WeaponSpec == Vibroblade
                case ItemSubCategory.WeaponVibroknife:       // WeaponSpec == Vibroknife
                case ItemSubCategory.WeaponShotgun:          // WeaponSpec == Shotgun
                case ItemSubCategory.WeaponElectrostaff:     // WeaponSpec == Electrostaff
                case ItemSubCategory.WeaponTrainingSaber:    // WeaponSpec == TrainingSaber
                    return ItemCategory.Weapon;
                case ItemSubCategory.CraftingMaterial:       // itm.mat.*
                case ItemSubCategory.CraftingSchematic:      // itm.schem.*
                    return ItemCategory.Crafting;
                case ItemSubCategory.ConsumableAdrenal:      // itm.potion.adrenal.*
                case ItemSubCategory.ConsumableMedpac:       // itm.potion.medpac.*
                case ItemSubCategory.ConsumableStim:         // itm.potion.stimpack.*
                case ItemSubCategory.ConsumableFood:         // itm.potion.food.*
                case ItemSubCategory.ConsumablePvp:          // itm.potion.pvp.*
                case ItemSubCategory.ConsumableGrenade:      // itm.grenade.*
                    return ItemCategory.Consumable;
                case ItemSubCategory.ItemModColorCrystal:    // EnhancementType == ColorCrystal
                case ItemSubCategory.ItemModArmoring:        // EnhancementType == Harness
                case ItemSubCategory.ItemModAugment:         // EnhancementType == Modulator
                case ItemSubCategory.ItemModMod:             // EnhancementType == Overlay
                case ItemSubCategory.ItemModEnhancement:     // EnhancementType == Support
                case ItemSubCategory.ItemModBarrel:          // EnhancementType == Barrel
                case ItemSubCategory.ItemModHilt:            // EnhancementType == PowerCrystal
                    return ItemCategory.ItemMod;
                default:
                    return ItemCategory.Other;
            }
        }
    }
}
