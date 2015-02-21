using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GomLib.Models
{
    public class Codex : GameObject, IEquatable<Codex>
    {
        public ulong NodeId { get; set; }

        public Dictionary<string, string> LocalizedName { get; set; }
        [Newtonsoft.Json.JsonIgnore]
        public string Name { get; set; }
        public Dictionary<string, string> LocalizedDescription { get; set; }
        [Newtonsoft.Json.JsonIgnore]
        public string Description { get; set; }

        public int Level { get; set; }
        public string Image { get; set; }
        public bool IsHidden { get; set; }

        public long CategoryId { get; set; }
        public Faction Faction { get; set; }
        public List<ClassSpec> Classes { get; set; }
        public bool ClassRestricted { get; set; }

        public bool IsPlanet { get; set; }

        public bool HasPlanets { get; set; }

        [Newtonsoft.Json.JsonIgnore]
        public List<Codex> Planets { get; set; }
        public List<ulong> PlanetsIds { get; set; }

        public override int GetHashCode()
        {
            int hash = LocalizedName.GetHashCode();
            hash ^= LocalizedDescription.GetHashCode();
            hash ^= Level.GetHashCode();
            if (Image != null) { hash ^= Image.GetHashCode(); }
            hash ^= CategoryId.GetHashCode();
            hash ^= Faction.GetHashCode();
            hash ^= IsHidden.GetHashCode();
            if (ClassRestricted) { foreach (var x in Classes) { hash ^= x.Fqn.GetHashCode(); } }
            if (HasPlanets) { foreach (var x in Planets) { hash ^= x.Id.GetHashCode(); } }
            return hash;
        }

        public override bool Equals(object obj)
        {
            if (obj == null) return false;

            if (ReferenceEquals(this, obj)) return true;

            Codex cdx = obj as Codex;
            if (cdx == null) return false;

            return Equals(cdx);
        }

        public bool Equals(Codex cdx)
        {
            if (cdx == null) return false;

            if (ReferenceEquals(this, cdx)) return true;

            if (this.CategoryId != cdx.CategoryId)
                return false;
            if (this.Classes != null)
            {
                if (cdx.Classes != null)
                {
                    if (this.Classes.Count != cdx.Classes.Count
                        && (!this.Classes.Select(x => x.Name).ToList().SequenceEqual(cdx.Classes.Select(x => x.Name).ToList())))
                        return false;
                }
            }
            else if (cdx.Classes != null)
                return false;
            if (this.ClassRestricted != cdx.ClassRestricted)
                return false;
            if (this.Faction != cdx.Faction)
                return false;
            if (this.Fqn != cdx.Fqn)
                return false;
            if (this.HasPlanets != cdx.HasPlanets)
                return false;
            if (this.Id != cdx.Id)
                return false;
            if (this.Image != cdx.Image)
                return false;
            if (this.IsHidden != cdx.IsHidden)
                return false;
            if (this.IsPlanet != cdx.IsPlanet)
                return false;
            if (this.Level != cdx.Level)
                return false;

            var ssComp = new DictionaryComparer<string, string>();
            if (!ssComp.Equals(this.LocalizedDescription, cdx.LocalizedDescription))
                return false;
            if (!ssComp.Equals(this.LocalizedName, cdx.LocalizedName))
                return false;

            if (this.NodeId != cdx.NodeId)
                return false;
            if (this.Planets != null)
            {
                if (!this.Planets.SequenceEqual(cdx.Planets))
                    return false;
            }
            else if (cdx.Planets != null)
                return false;
            if (this.PlanetsIds != null)
            {
                if (!this.PlanetsIds.SequenceEqual(cdx.PlanetsIds))
                    return false;
            }
            else if (cdx.PlanetsIds != null)
                return false;
            
            if (this.Description != cdx.Description)
                return false;
            if (this.Name != cdx.Name)
                return false;
            return true;
        }
    }
}
