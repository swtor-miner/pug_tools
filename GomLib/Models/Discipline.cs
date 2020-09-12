﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;
using Newtonsoft.Json;

namespace GomLib.Models
{
    public class Discipline : PseudoGameObject, IEquatable<Discipline>
    {
        [JsonIgnore]
        public ulong NodeId { get { return (ulong)Id; } }
        public string Icon { get; set; }
        public string HashedIcon
        {
            get
            {
                string icon = "none";
                if (Icon != null)
                {
                    icon = Icon;
                }
                var fileId = TorLib.FileId.FromFilePath(string.Format("/resources/gfx/icons/{0}.dds", icon));
                return string.Format("{0}_{1}", fileId.ph, fileId.sh);
            }
        }
        public long SortIdx { get; set; }
        [JsonIgnore]
        public ulong ClassId { get; set; }
        [JsonIgnore]
        public ulong PathApcId { get; set; }
        //[Newtonsoft.Json.JsonIgnore]
        public AbilityPackage PathAbilities { get; set; }
        [JsonConverter(typeof(LongConverter))]
        public long DescriptionId { get; set; }
        [JsonConverter(typeof(LongConverter))]
        public long NameId { get; set; }
        public Dictionary<string, string> LocalizedName { get; set; }
        [JsonIgnore]
        public string Description { get; set; }
        public Dictionary<string, string> LocalizedDescription { get; set; }
        [JsonConverter(typeof(LongConverter))]
        public long ClassNameId { get; set; }
        public Dictionary<string, string> LocalizedClassName { get; set; }
        [JsonIgnore]
        public string ClassName { get; set; }
        public string Role { get; set; }
        [JsonIgnore]
        public Dictionary<ulong, int> BaseAbilityIds { get; set; }
        public Dictionary<string, int> BaseAbility62Ids
        {
            get
            {
                if (BaseAbilityIds == null) return new Dictionary<string, int>();
                return BaseAbilityIds.ToDictionary(x => x.Key.ToMaskedBase62(), x => x.Value);
            }
        }
        [JsonIgnore]
        internal List<Ability> _BaseAbilities;
        [JsonIgnore]
        public List<Ability> BaseAbilities
        {
            get
            {
                if (_BaseAbilities == null)
                {
                    _BaseAbilities = new List<Ability>();
                    for (int i = 0; i < 4; i++)
                    {
                        var abl = Dom_.abilityLoader.Load(BaseAbilityIds.ElementAt(i).Key);
                        abl.Level = BaseAbilityIds.ElementAt(i).Value;
                        _BaseAbilities.Add(abl);
                    }
                }
                return _BaseAbilities;
            }
        }

        public override HashSet<string> GetDependencies()
        {
            HashSet<string> returnList = new HashSet<string>();
            //Add code here to add things like icons and models to the list.
            return returnList;
        }

        public override int GetHashCode()
        {
            int hash = NodeId.GetHashCode();
            return hash;
        }
        public override bool Equals(object obj)
        {
            if (obj == null) return false;

            if (ReferenceEquals(this, obj)) return true;

            if (!(obj is Discipline obj2)) return false;

            return Equals(obj2);
        }
        public bool Equals(Discipline obj)
        {
            if (obj == null) return false;

            if (ReferenceEquals(this, obj)) return true;

            if (NodeId != obj.NodeId)
                return false;
            if (!BaseAbilities.SequenceEqual(obj.BaseAbilities))
                return false;
            if (!BaseAbilityIds.SequenceEqual(obj.BaseAbilityIds))
                return false;
            if (ClassId != obj.ClassId)
                return false;
            if (ClassName != obj.ClassName)
                return false;
            if (ClassNameId != obj.ClassNameId)
                return false;
            if (Description != obj.Description)
                return false;
            if (DescriptionId != obj.DescriptionId)
                return false;
            if (Icon != obj.Icon)
                return false;
            if (Id != obj.Id)
                return false;

            var dComp = new DictionaryComparer<string, string>();
            if (!dComp.Equals(LocalizedClassName, obj.LocalizedClassName))
                return false;
            if (!dComp.Equals(LocalizedDescription, obj.LocalizedDescription))
                return false;
            if (!dComp.Equals(LocalizedName, obj.LocalizedName))
                return false;

            if (Name != obj.Name)
                return false;
            if (NameId != obj.NameId)
                return false;
            if (!PathAbilities.Equals(obj.PathAbilities))
                return false;
            if (PathApcId != obj.PathApcId)
                return false;
            if (Role != obj.Role)
                return false;
            if (SortIdx != obj.SortIdx)
                return false;

            return true;
        }



        public override XElement ToXElement(bool verbose)
        {
            XElement element = new XElement("Discipline");
            element.Add(new XAttribute("Id", Id),
                new XElement("Name", Name),
                new XElement("Description", Description),
                new XElement("Icon", Icon),
                new XElement("SortIndex", SortIdx),
                new XElement("DisciplinePath", PathAbilities.ToXElement(verbose)));

            return element;
        }
    }
}
