using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GomLib.Models
{
    public class CompanionGiftInterest : IEquatable<CompanionGiftInterest>
    {
        [Newtonsoft.Json.JsonIgnore]
        public Companion Companion { get; set; }
        public GiftType GiftType { get; set; }
        public GiftInterest Reaction { get; set; }
        public GiftInterest RomancedReaction { get; set; }

        public override bool Equals(object obj)
        {
            if (obj == null) return false;

            if (ReferenceEquals(this, obj)) return true;

            CompanionGiftInterest cgi = obj as CompanionGiftInterest;
            if (cgi == null) return false;

            return Equals(cgi);
        }

        public bool Equals(CompanionGiftInterest cgi)
        {
            if (cgi == null) return false;

            if (ReferenceEquals(this, cgi)) return true;

            if (this.GiftType != cgi.GiftType)
                return false;
            if (this.Reaction != cgi.Reaction)
                return false;
            if (this.RomancedReaction != cgi.RomancedReaction)
                return false;
            return true;
        }
    }
}
