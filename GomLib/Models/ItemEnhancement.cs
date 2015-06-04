using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Newtonsoft.Json;

namespace GomLib.Models
{
    /// <summary>Contains information about default modifications for items - including the slot for the modification, and the item ID of the modification</summary>
    public class ItemEnhancement : IEquatable<ItemEnhancement>
    {
        public EnhancementType Slot { get; set; }
        public DetailedEnhancementType DetailedSlot { get; set; }
        public ulong ModificationId { get; set; }

        [JsonIgnore]
        public Item Modification { get; set; }

        public override string ToString()
        {
            return string.Format("{0}: [{1}] {2}", Slot, ModificationId, Modification);
        }

        public override int GetHashCode()
        {
            return Slot.GetHashCode() ^ ModificationId.GetHashCode();
        }

        public override bool Equals(object obj)
        {
            if (obj == null) return false;

            if (ReferenceEquals(this, obj)) return true;

            ItemEnhancement itmEnh = obj as ItemEnhancement;
            return Equals(itmEnh);
        }

        public bool Equals(ItemEnhancement itmEnh)
        {
            if (itmEnh == null) return false;

            if (this.ModificationId != itmEnh.ModificationId)
                return false;
            if (!this.Slot.Equals(itmEnh.Slot))
                return false;
            if (this.ModificationId != 0)
            {
                if (!this.Modification.Equals(itmEnh.Modification))
                    return false;
            }
            else if (itmEnh.ModificationId != 0)
            {
                return false;
            }
            else
            {
                //both zero, so equal
            }
            return true;
        }
    }
}
