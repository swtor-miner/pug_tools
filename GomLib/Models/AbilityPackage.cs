using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;

namespace GomLib.Models
{
    public class AbilityPackage: GameObject, IEquatable<AbilityPackage>
    {
        public AbilityPackage()
        {
            this.PackageAbilities = new List<PackageAbility>();
            this.PackageTalents = new List<PackageTalent>();
        }

        public List<PackageAbility> PackageAbilities { get; private set; }
        public List<PackageTalent> PackageTalents { get; private set; }

        public override int GetHashCode()
        {
            int hash = Id.GetHashCode();
            if (Fqn != null) hash ^= Fqn.GetHashCode();
            if (PackageAbilities != null) foreach (var x in PackageAbilities) { hash ^= x.GetHashCode(); }
            if (PackageTalents != null) foreach (var x in PackageTalents) { hash ^= x.GetHashCode(); }

            return hash;
        }

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
        
        public override XElement ToXElement(bool verbose)
        {
            XElement ablPackage = new XElement("AbilityPackage");
            if (PackageAbilities.Count > 0 || PackageTalents.Count > 0)
            {
                int i = 1;
                ablPackage.Add(new XAttribute("Id", Fqn));
                ablPackage.Add(new XElement("NodeId", Id)); //, new XAttribute("Hash", GetHashCode()));
                foreach (var ablPack in PackageAbilities)
                {
                    var levels = ablPack.Levels[0] + "-" + ablPack.Levels[ablPack.Levels.Count() - 1];
                    XElement package = ablPack.Ability.ToXElement(verbose);
                    //new XElement("Package", new XAttribute("Index", i));
                    //new XElement("CategoryName", ablPack.CategoryName,
                    //new XAttribute("Id", ablPack.CategoryNameId)),

                    if (verbose)
                    {
                        package.Add(new XElement("PackageAutoAcquire", ablPack.AutoAcquire),
                        new XElement("PackageLevel", ablPack.Level),
                        new XElement("PackageLevels", levels), //String.Join(",", ablPack.Levels)),
                        new XElement("PackageScales", ablPack.Scales));
                        if (IsUtilityPackage)
                        {
                            package.Add(new XElement("UtilityTier", ablPack.UtilityTier),
                               new XElement("UtilityPosition", ablPack.UtilityPosition));
                        }
                    }
                    //package.Add(ablPack.Ability.ToXML());
                    ablPackage.Add(package); //add ability to AbilityPackage
                    i++;
                }
                if (IsUtilityPackage)
                {
                    foreach (var talPack in PackageTalents)
                    {
                        XElement package = talPack.Talent.ToXElement(verbose);

                        if (verbose)
                        {
                            package.Add(new XElement("UtilityTier", talPack.UtilityTier),
                            new XElement("UtilityPosition", talPack.UtilityPosition));
                        }
                        ablPackage.Add(package); //add Talent to AbilityPackage
                        i++;
                    }
                }
            }
            return ablPackage;
        }

    }
}
