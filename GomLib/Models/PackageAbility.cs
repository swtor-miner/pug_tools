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

        internal Ability _Ability { get; set; }
        [Newtonsoft.Json.JsonIgnore]
        public Ability Ability
        {
            get
            {
                if (_Ability == null)
                {
                    _Ability = _dom.abilityLoader.Load(AbilityId);
                }
                return _Ability;
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

            PackageAbility pkga = obj as PackageAbility;
            if (pkga == null) return false;

            return Equals(pkga);
        }

        public bool Equals(PackageAbility pkga)
        {
            if (pkga == null) return false;

            if (ReferenceEquals(this, pkga)) return true;

            if (!this.Ability.Equals(pkga.Ability))
                return false;
            if (this.AbilityId != pkga.AbilityId)
                return false;
            if (this.AutoAcquire != pkga.AutoAcquire)
                return false;
            if (this.Level != pkga.Level)
                return false;
            if (!this.Levels.SequenceEqual(pkga.Levels))
                return false;
            if (this.PackageId != pkga.PackageId)
                return false;
            if (this.Scales != pkga.Scales)
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

        internal Talent _Talent { get; set; }
        [Newtonsoft.Json.JsonIgnore]
        public Talent Talent
        {
            get
            {
                if (_Talent == null)
                {
                    _Talent = _dom.talentLoader.Load(PackageId);
                }
                return _Talent;
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
            if(Talent != null) hash ^= Talent.GetHashCode();
            hash ^= UtilityTier.GetHashCode();
            hash ^= UtilityPosition.GetHashCode();
            return hash;
        }

        public override bool Equals(object obj)
        {
            if (obj == null) return false;

            if (ReferenceEquals(this, obj)) return true;

            PackageTalent pkga = obj as PackageTalent;
            if (pkga == null) return false;

            return Equals(pkga);
        }

        public bool Equals(PackageTalent pkga)
        {
            if (pkga == null) return false;

            if (ReferenceEquals(this, pkga)) return true;

            if (!this.Talent.Equals(pkga.Talent))
                return false;
            if (this.UtilityPosition != pkga.UtilityPosition)
                return false;
            if (this.UtilityTier != pkga.UtilityTier)
                return false;
            if (this.PackageId != pkga.PackageId)
                return false;
            return true;
        }

        public long Level { get; set; }
    }
}
