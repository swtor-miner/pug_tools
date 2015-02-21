﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GomLib.Models
{
    public class AdvancedClass : GameObject, IEquatable<AdvancedClass>
    {
        public int Id { get; set; }
        public ulong NodeId { get; set; }
        public string Fqn { get; set; }
        public ClassSpec ClassSpec { get; set; }
        public string Name { get; set; }
        public Dictionary<string, string> LocalizedName { get; set; }
        public long NameId { get; set; }

        public override int GetHashCode()
        {
            int hash = this.Name.GetHashCode();
            hash ^= this.ClassSpec.Id.GetHashCode();
            return hash;
        }

        public long DescriptionId { get; set; }

        public string Description { get; set; }

        public Dictionary<string, string> LocalizedDescription { get; set; }

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

            if (this.NodeId != obj.NodeId)
                return false;
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

        public ulong UtiltyPkgId { get; set; }
        [Newtonsoft.Json.JsonIgnore]
        internal AbilityPackage _UtilityPkg;
        [Newtonsoft.Json.JsonIgnore]
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

        public ulong classSpecId { get; set; }

        public List<ulong> AdvancedClassPkgIds { get; set; }
        [Newtonsoft.Json.JsonIgnore]
        internal List<AbilityPackage> _AdvancedClassPkgs;
        [Newtonsoft.Json.JsonIgnore]
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

        public List<ulong> BaseClassPkgIds { get; set; }
        [Newtonsoft.Json.JsonIgnore]
        internal List<AbilityPackage> _BaseClassPkgs;
        [Newtonsoft.Json.JsonIgnore]
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
    }
}
