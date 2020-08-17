using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GomLib.Models
{
    public enum ItemDamageType
    {
        /// <summary>ItemDamageType.None = 0</summary>
        None = 0,
        /// <summary>ItemDamageType.Kinetic = 1</summary>
        Kinetic = 1,
        /// <summary>ItemDamageType.Energy = 2</summary>
        Energy = 2,
        /// <summary>ItemDamageType.Internal = 3</summary>
        Internal = 3,
        /// <summary>ItemDamageType.Elemental = 4</summary>
        Elemental = 4,
    }

    public static class ItemDamageTypeExtensions
    {
        public static ItemDamageType ToItemDamageType(this string str)
        {
            if (String.IsNullOrEmpty(str)) return ItemDamageType.None;

            switch (str)
            {
                case "None": return ItemDamageType.None;
                case "Kinetic": return ItemDamageType.Kinetic;
                case "Energy": return ItemDamageType.Energy;
                case "Internal": return ItemDamageType.Internal;
                case "Elemental": return ItemDamageType.Elemental;
                default: throw new InvalidOperationException("Invalid ItemDamageType: " + str);
            }
        }
        public static ItemDamageType ToItemDamageType(this ulong nodeId)
        {
            if (nodeId <= 0) return ItemDamageType.None;

            switch (nodeId)
            {
                case 0: return ItemDamageType.None;
                case 1: return ItemDamageType.Kinetic;
                case 2: return ItemDamageType.Energy;
                case 3: return ItemDamageType.Internal;
                case 4: return ItemDamageType.Elemental;
                default:
                    throw new InvalidOperationException("Invalid ItemDamageType: " + nodeId);
            }
        }
    }
}
