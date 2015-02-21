using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;

namespace GomLib.Models
{
    public class ItemSet : GameObject
    {
        public string Name { get; set; }
        public Dictionary<string, string> LocalizedName { get; set; }
        public int NameId { get; set; } // String Table = str.itm.setbonuses
        public List<ItemSetAbility> Abilities { get; set; }
        public List<ItemSetItem> SetItems { get; set; }
        public int Count { get; set; }
        //public List<Item> PrimaryItems { get; set; }

        public override int GetHashCode()
        {
            int hash = LocalizedName.GetHashCode();
            hash ^= Count.GetHashCode();
            foreach (var x in Abilities) { hash ^= x.GetHashCode(); }
            foreach (var x in SetItems) { hash ^= x.Item.Id.GetHashCode(); }
            return hash;
        }
    }
}
