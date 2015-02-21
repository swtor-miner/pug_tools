using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GomLib.Models
{
    public class CompanionProfessionModifier : IEquatable<CompanionProfessionModifier>
    {
        [Newtonsoft.Json.JsonIgnore]
        public Companion Companion { get; set; }
        public string Stat { get; set; }
        public int Modifier { get; set; }

        public override bool Equals(object obj)
        {
            if (obj == null) return false;

            if (ReferenceEquals(this, obj)) return true;

            CompanionProfessionModifier cpm = obj as CompanionProfessionModifier;
            if (cpm == null) return false;

            return Equals(cpm);
        }

        public bool Equals(CompanionProfessionModifier cpm)
        {
            if (cpm == null) return false;

            if (ReferenceEquals(this, cpm)) return true;

            if (this.Modifier != cpm.Modifier)
                return false;
            if (this.Stat != cpm.Stat)
                return false;
            return true;
        }
    }
}
