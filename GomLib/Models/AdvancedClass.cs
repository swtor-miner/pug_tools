﻿using System;
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
        [JsonConverter(typeof(LongConverter))]
        public long NameId { get; set; }

        public override int GetHashCode()
        {
            int hash = Name.GetHashCode();
            hash ^= ClassSpec.Id.GetHashCode();
            return hash;
        }

        [JsonConverter(typeof(LongConverter))]
        public long DescriptionId { get; set; }

        public string Description { get; set; }

        public Dictionary<string, string> LocalizedDescription { get; set; }

        //[JsonIgnore]
        public List<Discipline> Disciplines { get; set; }

        public override bool Equals(object obj)
        {
            if (obj == null) return false;

            if (ReferenceEquals(this, obj)) return true;

            if (!(obj is AdvancedClass obj2)) return false;

            return Equals(obj2);
        }

        public bool Equals(AdvancedClass obj)
        {
            if (obj == null) return false;

            if (ReferenceEquals(this, obj)) return true;

            if (!AdvancedClassPkgIds.SequenceEqual(obj.AdvancedClassPkgIds))
                return false;
            if (!AdvancedClassPkgs.SequenceEqual(obj.AdvancedClassPkgs))
                return false;
            if (!BaseClassPkgIds.SequenceEqual(obj.BaseClassPkgIds))
                return false;
            if (!BaseClassPkgs.SequenceEqual(obj.BaseClassPkgs))
                return false;
            if (ClassBackground != obj.ClassBackground)
                return false;
            if (ClassSpecId != obj.ClassSpecId)
                return false;
            if (!ClassSpec.Equals(obj.ClassSpec))
                return false;
            if (Description != obj.Description)
                return false;
            if (DescriptionId != obj.DescriptionId)
                return false;
            if (!Disciplines.SequenceEqual(obj.Disciplines))
                return false;
            if (Fqn != obj.Fqn)
                return false;
            if (Id != obj.Id)
                return false;

            var dComp = new DictionaryComparer<string, string>();
            if (!dComp.Equals(LocalizedDescription, obj.LocalizedDescription))
                return false;
            if (!dComp.Equals(LocalizedName, obj.LocalizedName))
                return false;

            if (Name != obj.Name)
                return false;
            if (NameId != obj.NameId)
                return false;
            if (!UtilityPkg.Equals(obj.Name))
                return false;
            if (UtilPkgIsActive != obj.UtilPkgIsActive)
                return false;
            if (UtiltyPkgId != obj.UtiltyPkgId)
                return false;

            return true;
        }

        [JsonConverter(typeof(ULongConverter))]
        public ulong UtiltyPkgId { get; set; }
        public string UtiltyPkgB62Id { get { if (UtiltyPkgId != 0) return UtiltyPkgId.ToMaskedBase62(); else return ""; } }
        [JsonIgnore]
        internal AbilityPackage _UtilityPkg;
        public AbilityPackage UtilityPkg
        {
            get
            {
                if (_UtilityPkg == null)
                    _UtilityPkg = Dom_.abilityPackageLoader.Load(UtiltyPkgId);
                return _UtilityPkg;
            }
        }
        public bool UtilPkgIsActive { get; set; }

        [JsonConverter(typeof(ULongConverter))]
        public ulong ClassSpecId { get; set; }
        public string ClassSpecB62Id { get { if (ClassSpecId != 0) return ClassSpecId.ToMaskedBase62(); else return ""; } }

        [JsonIgnore]
        public List<ulong> AdvancedClassPkgIds { get; set; }
        public List<string> AdvancedClassPkgB62Ids
        {
            get
            {
                if (AdvancedClassPkgIds != null)
                {
                    return AdvancedClassPkgIds.Select(x => x.ToMaskedBase62()).ToList();
                }
                return null;
            }
        }
        [JsonIgnore]
        internal List<AbilityPackage> _AdvancedClassPkgs;
        [JsonIgnore]
        public List<AbilityPackage> AdvancedClassPkgs
        {
            get
            {
                if (_AdvancedClassPkgs == null)
                {
                    _AdvancedClassPkgs = new List<AbilityPackage>();
                    foreach (var id in AdvancedClassPkgIds)
                        _AdvancedClassPkgs.Add(Dom_.abilityPackageLoader.Load(id));
                }
                return _AdvancedClassPkgs;
            }
        }

        [JsonIgnore]
        public List<ulong> BaseClassPkgIds { get; set; }
        public List<string> BaseClassPkgB62Ids
        {
            get
            {
                if (BaseClassPkgIds != null)
                {
                    return BaseClassPkgIds.Select(x => x.ToMaskedBase62()).ToList();
                }
                return null;
            }
        }
        [JsonIgnore]
        internal List<AbilityPackage> _BaseClassPkgs;
        public List<AbilityPackage> BaseClassPkgs
        {
            get
            {
                if (_BaseClassPkgs == null && BaseClassPkgIds != null)
                {
                    _BaseClassPkgs = new List<AbilityPackage>();
                    foreach (var id in BaseClassPkgIds)
                        _BaseClassPkgs.Add(Dom_.abilityPackageLoader.Load(id));
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
