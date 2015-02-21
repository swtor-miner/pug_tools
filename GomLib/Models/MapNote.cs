using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;
using System.Xml;

namespace GomLib.Models
{
    public class MapNote : GameObject
    {
        public ulong NodeId { get; set; }
        public string Name { get; set; }
        public MapNoteIcon Icon { get; set; }
        public MapNoteCondition Condition { get; set; }
        public HuntingRadius HuntingRadius { get; set; }
        public HuntingRadius BonusHuntingRadius { get; set; }
        public int LinkedMapId { get; set; }
        public long WonkaPackageId { get; set; }
        public long WonkaDestinationId { get; set; }
        // public string MapLink { get; set; }
        // public bool IsHidden { get; set; }
        // public Placeable Placeable { get; set; }

        public override int GetHashCode()
        {
            int result = Name.GetHashCode();
            result ^= Icon.GetHashCode();
            result ^= Condition.GetHashCode();
            result ^= HuntingRadius.GetHashCode();
            result ^= BonusHuntingRadius.GetHashCode();
            result ^= LinkedMapId.GetHashCode();
            result ^= WonkaPackageId.GetHashCode();
            result ^= WonkaDestinationId.GetHashCode();
            return result;
        }
    }
}
