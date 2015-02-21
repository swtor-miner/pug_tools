using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GomLib.Models
{
    public class AbilityPackage: GameObject, IEquatable<AbilityPackage>
    {
        public AbilityPackage()
        {
            this.PackageAbilities = new List<PackageAbility>();
            this.PackageTalents = new List<PackageTalent>();
        }

        //public string Fqn { get; set; }
        //public ulong Id { get; set; }
        public List<PackageAbility> PackageAbilities { get; private set; }
        public List<PackageTalent> PackageTalents { get; private set; }

        public override bool Equals(object obj)
        {
            if (obj == null) return false;

            if (ReferenceEquals(this, obj)) return true;

            AbilityPackage apc = obj as AbilityPackage;
            if (apc == null) return false;

            return Equals(apc);
        }

        public bool Equals(AbilityPackage apc)
        {
            if (apc == null) return false;

            if (ReferenceEquals(this, apc)) return true;

            if (this.Fqn != apc.Fqn)
                return false;
            if (this.Id != apc.Id)
                return false;
            if (this.IsUtilityPackage != apc.IsUtilityPackage)
                return false;
            if (!this.PackageAbilities.SequenceEqual(apc.PackageAbilities))
                return false;
            if (!this.PackageTalents.SequenceEqual(apc.PackageTalents))
                return false;
            return true;
        }

        public bool IsUtilityPackage { get; set; }
    }
}
