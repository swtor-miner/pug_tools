using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace GomLib.Models
{
    public class CompanionGiftInterest : IEquatable<CompanionGiftInterest>
    {
        [JsonIgnore]
        public Companion Companion { get; set; }
        [JsonConverter(typeof(StringEnumConverter))]
        public GiftType GiftType { get; set; }
        [JsonConverter(typeof(StringEnumConverter))]
        public GiftInterest Reaction { get; set; }
        [JsonConverter(typeof(StringEnumConverter))]
        public GiftInterest RomancedReaction { get; set; }

        public override int GetHashCode()
        {
            int hash = GiftType.GetHashCode();
            hash ^= Reaction.GetHashCode();
            hash ^= RomancedReaction.GetHashCode();
            return hash;
        }

        public override bool Equals(object obj)
        {
            if (obj == null) return false;

            if (ReferenceEquals(this, obj)) return true;

            if (!(obj is CompanionGiftInterest cgi)) return false;

            return Equals(cgi);
        }

        public bool Equals(CompanionGiftInterest cgi)
        {
            if (cgi == null) return false;

            if (ReferenceEquals(this, cgi)) return true;

            if (GiftType != cgi.GiftType)
                return false;
            if (Reaction != cgi.Reaction)
                return false;
            if (RomancedReaction != cgi.RomancedReaction)
                return false;
            return true;
        }
    }
}
