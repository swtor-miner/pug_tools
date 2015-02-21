using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;

namespace GomLib.Models
{
    public class ItemSetItem
    {
        public ItemSet ItemSet { get; set; }
        public Item Item { get; set; }
        public Item PrimaryItem { get; set; }

        public override int GetHashCode()
        {
            return Item.GetHashCode() ^ PrimaryItem.GetHashCode();
        }

    }
}
