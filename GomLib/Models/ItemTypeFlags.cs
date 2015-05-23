using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GomLib.Models
{
    /// <summary>Represents the bits set in an item's itmTypeBitSet</summary>
    [Flags]
    public enum ItemTypeFlags
    {
        IsArmor             = 0x1,      // Armor / Shields / Generators / Focii
        IsWeapon            = 0x2,      // Weapons / Vibroknives / Shotguns
        HasGTNCategory      = 0x4,      // old CatPutOnGTN
        Unk8                = 0x8,      // set on 61,555 items
        HasConversation     = 0x10,     // 
        IsCrafted           = 0x20,     // Has Field itmCraftedCategory
        CanBeDisassembled   = 0x40,     // Item can be RE'd
        HasDurability       = 0x80,     // Has Durability
        IsModdable          = 0x100,    // Has EnhancementCategory, so is moddable
        IsMod               = 0x200,    // Set on item mods
        CanHaveStats        = 0x400,    // Can have stats?
        Unk800              = 0x800,    // set on 50,255 items, not set on credits, cc
        IsGift              = 0x1000,   // companion gifts and some odds and ends
        IsMissionItem       = 0x2000,   // These seem to be the looted mission items 
        Unk4000             = 0x4000,   // old IsMissionItem - set on all items
        IsShipPart          = 0x8000,   // Set on on-rails ship parts
        Unk10000            = 0x10000,  // old IsSchematic - 4 items - Equip abilities that function from inventory?
        IsCmpCstmztn        = 0x20000,  // old IsShipUpgrade
        HasUniqueLimit      = 0x40000,  // old Unk40000
        HasOnUse            = 0x80000,  // old IsCustomAppearance
        IsEquipable         = 0x100000, // old IsUnique
        IsCurrency          = 0x200000, // old hasOnUse
        IsMtxItem           = 0x400000, // Unk400000
        IsRepItem           = 0x800000  // old IsCurrency
    }
}
