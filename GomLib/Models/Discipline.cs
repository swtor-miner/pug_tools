using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GomLib.Models
{
    public class Discipline : PseudoGameObject, IEquatable<Discipline>
    {
        public ulong NodeId { get; set; }
        //public Dictionary<string, string> LocalizedTitle { get; set; }
        //public string Title { get; set; }

        //[Newtonsoft.Json.JsonIgnore]

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

            Discipline obj2 = obj as Discipline;
            if (obj2 == null) return false;

            return Equals(obj2);
        }

        public bool Equals(Discipline obj)
        {
            if (obj == null) return false;

            if (ReferenceEquals(this, obj)) return true;

            if (this.NodeId != obj.NodeId)
                return false;
            if (!this.BaseAbilities.SequenceEqual(obj.BaseAbilities))
                return false;
            if (!this.BaseAbilityIds.SequenceEqual(obj.BaseAbilityIds))
                return false;
            if (this.ClassId != obj.ClassId)
                return false;
            if (this.ClassName != obj.ClassName)
                return false;
            if (this.ClassNameId != obj.ClassNameId)
                return false;
            if (this.Description != obj.Description)
                return false;
            if (this.DescriptionId != obj.DescriptionId)
                return false;
            if (this.Icon != obj.Icon)
                return false;
            if (this.Id != obj.Id)
                return false;

            var dComp = new DictionaryComparer<string, string>();
            if (!dComp.Equals(this.LocalizedClassName, obj.LocalizedClassName))
                return false; 
            if (!dComp.Equals(this.LocalizedDescription, obj.LocalizedDescription))
                return false;
            if (!dComp.Equals(this.LocalizedName, obj.LocalizedName))
                return false;

            if (this.Name != obj.Name)
                return false;
            if (this.NameId != obj.NameId)
                return false;
            if (!this.PathAbilities.Equals(obj.PathAbilities))
                return false;
            if (this.PathApcId != obj.PathApcId)
                return false;
            if (this.Role != obj.Role)
                return false;
            if (this.SortIdx != obj.SortIdx)
                return false;

            return true;
        }

        public string Icon { get; set; }

        public long SortIdx { get; set; }

        public ulong ClassId { get; set; }

        public ulong PathApcId { get; set; }
        [Newtonsoft.Json.JsonIgnore]
        public AbilityPackage PathAbilities { get; set; }

        public long DescriptionId { get; set; }

        public long NameId { get; set; }

        public Dictionary<string, string> LocalizedName { get; set; }

        public string Description { get; set; }

        public Dictionary<string, string> LocalizedDescription { get; set; }

        public long ClassNameId { get; set; }

        public Dictionary<string, string> LocalizedClassName { get; set; }

        public string ClassName { get; set; }

        public string Role { get; set; }

        public Dictionary<ulong, int> BaseAbilityIds { get; set; }
        [Newtonsoft.Json.JsonIgnore]
        internal List<Ability> _BaseAbilities;
        [Newtonsoft.Json.JsonIgnore]
        public List<Ability> BaseAbilities
        {
            get
            {
                if (_BaseAbilities == null)
                {
                    _BaseAbilities = new List<Ability>();
                    for (int i = 0; i < 4; i++)
                    {
                        var abl = _dom.abilityLoader.Load(BaseAbilityIds.ElementAt(i).Key);
                        abl.Level = BaseAbilityIds.ElementAt(i).Value;
                        _BaseAbilities.Add(abl);
                    }
                }
                return _BaseAbilities;
            }
        }
    }
}
