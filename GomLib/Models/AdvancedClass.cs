using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;
using Newtonsoft.Json;

namespace GomLib.Models
{
    public class AdvancedClass : GameObject, IEquatable<AdvancedClass>
    {
        public ClassSpec ClassSpec { get; set; }
        [JsonIgnore]
        public string Name { get; set; }
        public Dictionary<string, string> LocalizedName { get; set; }
        [JsonIgnore]
        public long NameId { get; set; }

        public override int GetHashCode()
        {
            int hash = this.Name.GetHashCode();
            hash ^= this.ClassSpec.Id.GetHashCode();
            return hash;
        }

        [JsonIgnore]
        public long DescriptionId { get; set; }

        public string Description { get; set; }

        public Dictionary<string, string> LocalizedDescription { get; set; }

        //[JsonIgnore]
        public List<Discipline> Disciplines { get; set; }

        public override bool Equals(object obj)
        {
            if (obj == null) return false;

            if (ReferenceEquals(this, obj)) return true;

            AdvancedClass obj2 = obj as AdvancedClass;
            if (obj2 == null) return false;

            return Equals(obj2);
        }

        public bool Equals(AdvancedClass obj)
        {
            if (obj == null) return false;

            if (ReferenceEquals(this, obj)) return true;

            if (!this.AdvancedClassPkgIds.SequenceEqual(obj.AdvancedClassPkgIds))
                return false;
            if (!this.AdvancedClassPkgs.SequenceEqual(obj.AdvancedClassPkgs))
                return false;
            if (!this.BaseClassPkgIds.SequenceEqual(obj.BaseClassPkgIds))
                return false;
            if (!this.BaseClassPkgs.SequenceEqual(obj.BaseClassPkgs))
                return false;
            if (this.ClassBackground != obj.ClassBackground)
                return false;
            if (this.classSpecId != obj.classSpecId)
                return false;
            if (!this.ClassSpec.Equals(obj.ClassSpec))
                return false;
            if (this.Description != obj.Description)
                return false;
            if (this.DescriptionId != obj.DescriptionId)
                return false;
            if (!this.Disciplines.SequenceEqual(obj.Disciplines))
                return false;
            if (this.Fqn != obj.Fqn)
                return false;
            if (this.Id != obj.Id)
                return false;
            
            var dComp = new DictionaryComparer<string, string>();
            if (!dComp.Equals(this.LocalizedDescription, obj.LocalizedDescription))
                return false;
            if (!dComp.Equals(this.LocalizedName, obj.LocalizedName))
                return false;

            if (this.Name != obj.Name)
                return false;
            if (this.NameId != obj.NameId)
                return false;
            if (!this.UtilityPkg.Equals(obj.Name))
                return false;
            if (this.UtilPkgIsActive != obj.UtilPkgIsActive)
                return false;
            if (this.UtiltyPkgId != obj.UtiltyPkgId)
                return false;

            return true;
        }

        [JsonIgnore]
        public ulong UtiltyPkgId { get; set; }
        public string UtiltyPkgB62Id { get { if (UtiltyPkgId != 0) return UtiltyPkgId.ToMaskedBase62(); else return ""; } }
        [Newtonsoft.Json.JsonIgnore]
        internal AbilityPackage _UtilityPkg;
        public AbilityPackage UtilityPkg
        {
            get
            {
                if (_UtilityPkg == null)
                    _UtilityPkg = _dom.abilityPackageLoader.Load(UtiltyPkgId);
                return _UtilityPkg;
            }
        }
        public bool UtilPkgIsActive { get; set; }

        [JsonIgnore]
        public ulong classSpecId { get; set; }

        [JsonIgnore]
        public List<ulong> AdvancedClassPkgIds { get; set; }
        [Newtonsoft.Json.JsonIgnore]
        internal List<AbilityPackage> _AdvancedClassPkgs;
        public List<AbilityPackage> AdvancedClassPkgs
        {
            get
            {
                if (_AdvancedClassPkgs == null)
                {
                    _AdvancedClassPkgs = new List<AbilityPackage>();
                    foreach (var id in AdvancedClassPkgIds)
                        _AdvancedClassPkgs.Add(_dom.abilityPackageLoader.Load(id));
                }
                return _AdvancedClassPkgs;
            }
        }

        [JsonIgnore]
        public List<ulong> BaseClassPkgIds { get; set; }
        [Newtonsoft.Json.JsonIgnore]
        internal List<AbilityPackage> _BaseClassPkgs;
        public List<AbilityPackage> BaseClassPkgs
        {
            get
            {
                if (_BaseClassPkgs == null)
                {
                    _BaseClassPkgs = new List<AbilityPackage>();
                    foreach (var id in BaseClassPkgIds)
                        _BaseClassPkgs.Add(_dom.abilityPackageLoader.Load(id));
                }
                return _BaseClassPkgs;
            }
        }

        public string ClassBackground { get; set; }

        public override XElement ToXElement(bool verbose)
        {
            XElement playerClass = new XElement("Class");
            playerClass.Add(new XElement("Name", Name, new XAttribute("Id", NameId)),
                new XElement("Description", Description, new XAttribute("Id", DescriptionId)));

            XElement disciplines = new XElement("Disciplines");
            foreach (var disc in Disciplines)
            {
                disciplines.Add(disc.ToXElement(verbose));
            }
            playerClass.Add(disciplines);
            XElement classPkgs = new XElement("AdvancedClassBasePackages");
            foreach (var classPkg in AdvancedClassPkgs)
            {
                classPkgs.Add(classPkg.ToXElement(verbose));
            }
            playerClass.Add(classPkgs,
                new XElement("Utilities", UtilityPkg.ToXElement(verbose)));

            playerClass.Add(new XElement("BaseClasePackages", ClassSpec.AbilityPackage.ToXElement(verbose)));


            return playerClass;
        }
    }
}
