using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GomLib.Models
{
    public class ItemSetAbility
    {
        //public int Id { get; set; }
        public ItemSet ItemSet { get; set; }
        public Ability Ability { get; set; }
        public int ItemCount { get; set; }

        public override int GetHashCode()
        {
            return Ability.GetHashCode() ^ ItemCount.GetHashCode();
        }
    }
}
