using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Newtonsoft.Json;

namespace GomLib.Models
{
    public class PackageAbility : IEquatable<PackageAbility>
    {
        [JsonIgnore]
        public DataObjectModel _dom;
        public PackageAbility()
        {
            Levels = new List<int>();
        }
        [JsonIgnore]
        public ulong PackageId { get; set; }
        public string PackageB62Id
        {
            get
            {
                if (PackageId == 0) return "";
                return PackageId.ToMaskedBase62();
            }
        }

        internal Ability Ability_ { get; set; }
        [JsonIgnore]
        public Ability Ability
        {
            get
            {
                if (Ability_ == null)
                {
                    Ability_ = _dom.abilityLoader.Load(AbilityId);
                }
                return Ability_;
            }
        }

        [JsonIgnore]
        public ulong AbilityId { get; set; }
        public string AbilityB62Id
        {
            get
            {
                if (AbilityId == 0) return "";
                return AbilityId.ToMaskedBase62();
            }
        }
        public List<int> Levels { get; set; }
        public bool Scales { get; set; }
        public int Level { get; set; }
        public bool AutoAcquire { get; set; }
        public string Toughness { get; set; }
        public long AiUsePriority { get; set; }
        public bool IsUtilityPackage { get; set; }
        public long UtilityTier { get; set; }
        public long UtilityPosition { get; set; }

        public override int GetHashCode()
        {
            int hash = PackageId.GetHashCode();
            if (Ability != null) hash ^= Ability.GetHashCode();
            hash ^= AbilityId.GetHashCode();
            if (Levels != null) foreach (var x in Levels) { hash ^= x.GetHashCode(); }
            hash ^= Scales.GetHashCode();
            hash ^= Level.GetHashCode();
            hash ^= AutoAcquire.GetHashCode();
            if (Toughness != null) hash ^= Toughness.GetHashCode();
            hash ^= IsUtilityPackage.GetHashCode();
            hash ^= UtilityTier.GetHashCode();
            hash ^= UtilityPosition.GetHashCode();
            return hash;
        }

        public override bool Equals(object obj)
        {
            if (obj == null) return false;

            if (ReferenceEquals(this, obj)) return true;

            if (!(obj is PackageAbility pkga)) return false;

            return Equals(pkga);
        }

        public bool Equals(PackageAbility pkga)
        {
            if (pkga == null) return false;

            if (ReferenceEquals(this, pkga)) return true;

            if (!Ability.Equals(pkga.Ability))
                return false;
            if (AbilityId != pkga.AbilityId)
                return false;
            if (AutoAcquire != pkga.AutoAcquire)
                return false;
            if (Level != pkga.Level)
                return false;
            if (!Levels.SequenceEqual(pkga.Levels))
                return false;
            if (PackageId != pkga.PackageId)
                return false;
            if (Scales != pkga.Scales)
                return false;
            return true;
        }
    }

    public class PackageTalent : IEquatable<PackageTalent>
    {
        [JsonIgnore]
        public DataObjectModel _dom;
        public PackageTalent() { }

        [JsonIgnore]
        public ulong PackageId { get; set; }

        internal Talent Talent_ { get; set; }
        [JsonIgnore]
        public Talent Talent
        {
            get
            {
                if (Talent_ == null)
                {
                    Talent_ = _dom.talentLoader.Load(PackageId);
                }
                return Talent_;
            }
        }
        public string TalentB62Id
        {
            get
            {
                if (PackageId == 0) return "";
                return PackageId.ToMaskedBase62();
            }
        }

        public long UtilityTier { get; set; }
        public long UtilityPosition { get; set; }

        public override int GetHashCode()
        {
            int hash = PackageId.GetHashCode();
            if (Talent != null) hash ^= Talent.GetHashCode();
            hash ^= UtilityTier.GetHashCode();
            hash ^= UtilityPosition.GetHashCode();
            return hash;
        }

        public override bool Equals(object obj)
        {
            if (obj == null) return false;

            if (ReferenceEquals(this, obj)) return true;

            if (!(obj is PackageTalent pkga)) return false;

            return Equals(pkga);
        }

        public bool Equals(PackageTalent pkga)
        {
            if (pkga == null) return false;

            if (ReferenceEquals(this, pkga)) return true;

            if (Talent != null)
            {
                if (!Talent.Equals(pkga.Talent))
                    return false;
            }
            else if (pkga.Talent != null)
                return false;
            if (UtilityPosition != pkga.UtilityPosition)
                return false;
            if (UtilityTier != pkga.UtilityTier)
                return false;
            if (PackageId != pkga.PackageId)
                return false;
            return true;
        }

        public long Level { get; set; }
    }
}
