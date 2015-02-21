using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GomLib.Models
{
    public class ClassSpec : GameObject, IEquatable<ClassSpec>
    {
        //public int Id { get; set; }
        public string Fqn { get; set; }
        //public ulong Id { get; set; }
        public int DataHash { get; set; }
        public bool IsPlayerClass { get; set; }
        public string Name { get; set; }
        public Dictionary<string, string> LocalizedName { get; set; }
        public long NameId { get; set; }
        public string Icon { get; set; }
        public int AlignmentLight { get; set; }
        public int AlignmentDark { get; set; }
        public ulong AbilityPackageId { get; set; }
        public AbilityPackage AbilityPackage { get; set; }

        //public static List<string> ParseClassList(string str)
        //{
        //    List<string> results = new List<string>();
        //    if (String.IsNullOrEmpty(str)) return results;

        //    var classes = str.Split(',');
        //    foreach (var c in classes)
        //    {
        //        if (!c.StartsWith("class."))
        //        {
        //            Console.WriteLine("Parsed Class List contains something that isn't a class! {0}", c);
        //            continue;
        //        }

        //        results.Add(FqnToId(c.Trim()));
        //    }
        //    return results;
        //}

        //public static string FqnToId(string fqn)
        //{
        //    if (fqn.StartsWith("class.pc."))
        //    {
        //        string classId = fqn.Substring(9);
        //        switch (classId)
        //        {
        //            case "sith_warrior": return "sith-warrior";
        //            case "sith_sorcerer": return "sith-inquisitor";
        //            case "bounty_hunter": return "bounty-hunter";
        //            case "spy": return "imperial-agent";
        //            case "jedi_knight": return "jedi-knight";
        //            case "jedi_wizard": return "jedi-consular";
        //            case "trooper": return "trooper";
        //            case "smuggler": return "smuggler";
        //            case "advanced.marauder": return "marauder";
        //            case "advanced.juggernaut": return "juggernaut";
        //            case "advanced.assassin": return "assassin";
        //            case "advanced.sorcerer": return "sorcerer";
        //            case "advanced.sniper": return "sniper";
        //            case "advanced.operative": return "operative";
        //            case "advanced.mercenary": return "mercenary";
        //            case "advanced.powertech": return "powertech";
        //            case "advanced.guardian": return "guardian";
        //            case "advanced.sentinel": return "sentinel";
        //            case "advanced.force_wizard": return "sage";
        //            case "advanced.shadow": return "shadow";
        //            case "advanced.gunslinger": return "gunslinger";
        //            case "advanced.scoundrel": return "scoundrel";
        //            case "advanced.commando": return "commando";
        //            case "advanced.specialist": return "vanguard";
        //            default: return classId;
        //        }
        //    }

        //    if (fqn.StartsWith("class."))
        //        fqn = fqn.Substring(6);
        //    fqn = fqn.Replace('.', '-');
        //    fqn = fqn.Replace('_', '-');
        //    return fqn;
        //}

        //static string PackageFqnToId(string fqn)
        //{
        //    string id = fqn;
        //    if (fqn.StartsWith("pkg.abilities."))
        //        id = id.Substring(14);
        //    id = id.Replace('.', '-');
        //    id = id.Replace('_', '-');
        //    return id;
        //}

        public override int GetHashCode()
        {
            int hash = this.Name.GetHashCode();
            hash ^= this.AlignmentDark.GetHashCode();
            hash ^= this.AlignmentLight.GetHashCode();
            hash ^= this.AbilityPackageId.GetHashCode();
            if (this.Icon != null)
            {
                hash ^= this.Icon.GetHashCode();
            }
            hash ^= this.Fqn.GetHashCode();
            hash ^= this.Id.GetHashCode();
            return hash;
        }

        public override bool Equals(object obj)
        {
            if (obj == null) return false;

            if (ReferenceEquals(this, obj)) return true;

            ClassSpec itm = obj as ClassSpec;
            if (itm == null) return false;

            return Equals(itm);
        }

        public bool EqualsWithoutAbilityPackage(ClassSpec itm)
        {
            if (itm == null) return false;

            if (ReferenceEquals(this, itm)) return true;

            if (this.AbilityPackageId != itm.AbilityPackageId)
                return false;
            if (this.AlignmentDark != itm.AlignmentDark)
                return false;
            if (this.AlignmentLight != itm.AlignmentLight)
                return false;
            if (this.Fqn != itm.Fqn)
                return false;
            if (this.Icon != itm.Icon)
                return false;
            if (this.Id != itm.Id)
                return false;
            if (this.IsPlayerClass != itm.IsPlayerClass)
                return false;
            var dComp = new DictionaryComparer<string, string>();
            if (!dComp.Equals(this.LocalizedName, itm.LocalizedName))
                return false;
            if (this.Name != itm.Name)
                return false;
            if (this.NameId != itm.NameId)
                return false;
            if (this.Id != itm.Id)
                return false;

            return true;
        }

        public bool Equals(ClassSpec itm)
        {
            if (itm == null) return false;

            if (ReferenceEquals(this, itm)) return true;

            if (this.AbilityPackageId != itm.AbilityPackageId)
                return false;
            if (this.AlignmentDark != itm.AlignmentDark)
                return false;
            if (this.AlignmentLight != itm.AlignmentLight)
                return false;
            if (this.Fqn != itm.Fqn)
                return false;
            if (this.Icon != itm.Icon)
                return false;
            if (this.Id != itm.Id)
                return false;
            if (this.IsPlayerClass != itm.IsPlayerClass)
                return false;
            var dComp = new DictionaryComparer<string, string>();
            if (!dComp.Equals(this.LocalizedName, itm.LocalizedName))
                return false;
            if (this.Name != itm.Name)
                return false;
            if (this.NameId != itm.NameId)
                return false;
            if (this.Id != itm.Id)
                return false;
            if (this.AbilityPackage != null)
            {
                if (!this.AbilityPackage.Equals(itm.AbilityPackage))
                    return false;
            }
            else if (itm.AbilityPackage != null)
                return false;

            return EqualsWithoutAbilityPackage(itm);
        }
    }
}
