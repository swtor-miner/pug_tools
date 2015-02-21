using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GomLib.Models
{
    public class PackageAbility : IEquatable<PackageAbility>
    {
        public PackageAbility()
        {
            Levels = new List<int>();
        }

        public ulong PackageId { get; set; }

        [Newtonsoft.Json.JsonIgnore]
        public Ability Ability { get; set; }

        public ulong AbilityId { get; set; }
        // public int Rank { get; set; }
        public List<int> Levels { get; set; }
        public bool Scales { get; set; }
        public int Level { get; set; }
        public bool AutoAcquire { get; set; }
        // public int Toughness { get; set; }
        public int CategoryNameId { get; set; }
        public string CategoryName { get; set; }
        // public bool IsTalent { get; set; }

        //public PackageAbility Clone()
        //{
        //    return new PackageAbility()
        //    {
        //        PackageId = this.PackageId,
        //        Ability = this.Ability,
        //        AbilityId = this.AbilityId,
        //        Rank = this.Rank,
        //        Level = this.Level,
        //        AutoAcquire = this.AutoAcquire,
        //        Toughness = this.Toughness,
        //        CategoryName = this.CategoryName,
        //        CategoryNameId = this.CategoryNameId,
        //        IsTalent = this.IsTalent
        //    };
        //}
        public bool IsUtilityPackage { get; set; }
        public long UtilityTier { get; set; }
        public long UtilityPosition { get; set; }

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
            if (this.CategoryName != pkga.CategoryName)
                return false;
            if (this.CategoryNameId != pkga.CategoryNameId)
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
        public PackageTalent() { }

        public ulong PackageId { get; set; }

        [Newtonsoft.Json.JsonIgnore]
        public Talent Talent { get; set; }

        public long UtilityTier { get; set; }
        public long UtilityPosition { get; set; }

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
