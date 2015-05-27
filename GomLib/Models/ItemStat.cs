using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GomLib.Models
{
    /// <summary>Contains a Stat modification for an item</summary>
    public class ItemStat : IEquatable<ItemStat>
    {
        public ItemStat Clone()
        {
            ItemStat clone = this.MemberwiseClone() as ItemStat;
            //
            //
            return clone;
        }

        public Stat Stat { get; set; }
        public DetailedStat DetailedStat { get; set; }
        public int Modifier { get; set; }

        public override string ToString()
        {
            return string.Format("+{0:#,##0.##} {1}", Modifier, Stat);
        }

        public override int GetHashCode()
        {
            return Stat.GetHashCode() ^ Modifier.GetHashCode();
        }

        public override bool Equals(object obj)
        {
            if (obj == null) return false;

            if (ReferenceEquals(this, obj)) return true;

            ItemStat itmStat = obj as ItemStat;
            return Equals(itmStat);
        }

        public bool Equals(ItemStat itmStat)
        {
            if (itmStat == null) return false;

            if (this.Modifier != itmStat.Modifier)
                return false;
            if (!this.Stat.Equals(itmStat.Stat))
                return false;
            return true;
        }
    }
}
