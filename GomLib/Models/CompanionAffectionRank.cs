using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GomLib.Models
{
    public class CompanionAffectionRank : IEquatable<CompanionAffectionRank>
    {
        public int Affection { get; set; }
        [Newtonsoft.Json.JsonIgnore]
        public Companion Companion { get; set; }
        public int Rank { get; set; }

        public override int GetHashCode()
        {
            int hash = Affection.GetHashCode();
            hash ^= Rank.GetHashCode();
            return hash;
        }

        public override bool Equals(object obj)
        {
            if (obj == null) return false;

            if (ReferenceEquals(this, obj)) return true;

            if (!(obj is CompanionAffectionRank car)) return false;

            return Equals(car);
        }

        public bool Equals(CompanionAffectionRank car)
        {
            if (car == null) return false;

            if (ReferenceEquals(this, car)) return true;

            if (Affection != car.Affection)
                return false;
            if (Rank != car.Rank)
                return false;
            return true;
        }
    }
}
