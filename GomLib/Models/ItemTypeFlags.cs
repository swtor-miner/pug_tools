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
        HasGTNCategory      = 0x4,      // 
        Unk8                = 0x8,      // Unknown - Set on 61,555 items
        HasConversation     = 0x10,     // 
        IsCrafted           = 0x20,     // Has Field itmCraftedCategory
        CanBeDisassembled   = 0x40,     //
        HasDurability       = 0x80,     //
        IsModdable          = 0x100,    //
        IsMod               = 0x200,    //
        CanHaveStats        = 0x400,    // Can have stats?
        Unk800              = 0x800,    // Unknown - Set on 50,255 items, not set on credits, cc
        IsGift              = 0x1000,   // Companion gifts and some odds and ends
        IsMissionItem       = 0x2000,   // These seem to be the looted mission items 
        Unk4000             = 0x4000,   // Set on all items
        IsShipPart          = 0x8000,   // Set on on-rails ship parts
        Unk10000            = 0x10000,  // Unknown - Set on 4 items - Equip abilities that function from inventory?
        IsCmpCstmztn        = 0x20000,  //
        HasUniqueLimit      = 0x40000,  //
        HasOnUse            = 0x80000,  //
        IsEquipable         = 0x100000, //
        IsCurrency          = 0x200000, //
        IsMtxItem           = 0x400000, //
        IsRepTrophy         = 0x800000  //
    }
}
