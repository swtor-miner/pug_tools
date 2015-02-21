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
        IsArmor             = 0x1,  // Armor / Shields / Generators / Focii
        IsWeapon            = 0x2,  // Weapons / Vibroknives / Shotguns
        CanPutOnGTN         = 0x4,  // Can sell on the GTN
        Unk8                = 0x8,  // Consumed on use?
        HasConversation     = 0x10,
        IsCrafted           = 0x20, // Has Field itmCraftedCategory
        CanBeDisassembled   = 0x40,
        Unk80               = 0x80, // Has Durability?
        Unk100              = 0x100,
        IsMod               = 0x200,
        Unk400              = 0x400, // Can have stats?
        Unk800              = 0x800,
        IsGift              = 0x1000,
        IsMount             = 0x2000, // ?
        IsMissionItem       = 0x4000,
        IsShipPart          = 0x8000, // Set on on-rails ship parts
        IsSchematic         = 0x10000,
        IsShipUpgrade       = 0x20000,
        Unk40000            = 0x40000, // Only set on one item
        IsCustomAppearance  = 0x80000,
        IsUnique            = 0x100000,
        HasOnUse            = 0x200000,
        Unk400000           = 0x400000,
        IsCurrency          = 0x800000
    }
}
