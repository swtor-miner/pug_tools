using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;
using Newtonsoft.Json;

namespace GomLib.Models
{
    public class Codex : GameObject, IEquatable<Codex>
    {
        //public ulong NodeId { get; set; }

        public Dictionary<string, string> LocalizedName { get; set; }
        [Newtonsoft.Json.JsonIgnore]
        public string Name { get; set; }
        public Dictionary<string, string> LocalizedDescription { get; set; }
        [Newtonsoft.Json.JsonIgnore]
        public string Description { get; set; }

        public int Level { get; set; }
        public string Image { get; set; }
        public bool IsHidden { get; set; }

        [Newtonsoft.Json.JsonIgnore]
        public string CategoryName { get; set; }
        public Dictionary<string, string> LocalizedCategoryName { get; set; }
        public long CategoryId { get; set; }
        [JsonConverter(typeof(Newtonsoft.Json.Converters.StringEnumConverter))]
        public Faction Faction { get; set; }

        [JsonIgnore]
        public List<ClassSpec> Classes { get; set; }
        [Newtonsoft.Json.JsonIgnore]
        internal List<string> _ClassesB62 { get; set; }
        public List<string> ClassesB62
        {
            get
            {
                if (_ClassesB62 == null)
                {
                    if (Classes == null) return new List<string>();
                    _ClassesB62 = Classes.Select(x => x.Base62Id).ToList();
                }
                return _ClassesB62;
            }
        }
        public bool ClassRestricted { get; set; }

        public bool IsPlanet { get; set; }

        public bool HasPlanets { get; set; }

        [Newtonsoft.Json.JsonIgnore]
        public List<Codex> Planets { get; set; }
        [JsonIgnore]
        public List<ulong> PlanetsIds { get; set; }
        [Newtonsoft.Json.JsonIgnore]
        internal List<string> _PlanetsB62 { get; set; }
        public List<string> PlanetsB62
        {
            get
            {
                if (_PlanetsB62 == null)
                {
                    if (Planets == null) return new List<string>();
                    _PlanetsB62 = PlanetsIds.Select(x => x.ToMaskedBase62()).ToList();
                }
                return _PlanetsB62;
            }
        }

        public override int GetHashCode()
        {
            int hash = Level.GetHashCode();
            if (LocalizedName != null) foreach (var x in LocalizedName) { hash ^= x.GetHashCode(); } //dictionaries need to hashed like this
            if (LocalizedDescription != null) foreach (var x in LocalizedDescription) { hash ^= x.GetHashCode(); } //dictionaries need to hashed like this
            if (Image != null) { hash ^= Image.GetHashCode(); }
            hash ^= CategoryId.GetHashCode();
            if (CategoryName != null)
            {
                hash ^= CategoryName.GetHashCode();
            }
            hash ^= Faction.GetHashCode();
            hash ^= IsHidden.GetHashCode();
            hash ^= ClassRestricted.GetHashCode();
            hash ^= IsPlanet.GetHashCode();
            hash ^= HasPlanets.GetHashCode();
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
            if (this.CategoryName != cdx.CategoryName)
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

        public override XElement ToXElement(bool verbose) //split to see if it was necessary to loop through linked codices. Didn't seem like it.
        {
            XElement codex = new XElement("Codex");

            codex.Add(new XElement("Title", Name),
                new XElement("NodeId", Id),
                new XAttribute("Id", Id),
                //new XAttribute("Hash", GetHashCode()),
                new XElement("Category", new XAttribute("Id", CategoryId), CategoryName));
            if (verbose)
            {
                string reqclasses = null;
                if (Classes != null)
                {
                    foreach (var reqclass in Classes)
                    {
                        reqclasses += reqclass.Name.ToString() + ", ";
                    }
                }
                if (reqclasses != null) { reqclasses = reqclasses.Substring(0, reqclasses.Length - 2); }
                codex.Add(new XElement("Classes", reqclasses));
                codex.Add(new XElement("ClassRestricted", ClassRestricted),
                    new XElement("Faction", Faction),
                    new XElement("Fqn", Fqn),
                    new XElement("HasPlanets", HasPlanets),
                    new XElement("Image", Image),
                    new XElement("IsHidden", IsHidden),
                    new XElement("IsPlanet", IsPlanet),
                    new XElement("Level", Level),
                    new XElement("Text", LocalizedDescription["enMale"]));
                XElement subCodices = new XElement("LinkedCodexEntries");
                if (HasPlanets && Planets != null)
                {
                    foreach (var planet in Planets)
                    {
                        if (planet != null) subCodices.Add(new XElement("Codex", planet.Fqn, new XAttribute("Id", planet.Id))); //change this to call the parent method to iterate through linked codices
                    }
                    codex.Add(subCodices);
                }
                //codex.Add(subCodices);
            }
            return codex;
        }
    }
}
