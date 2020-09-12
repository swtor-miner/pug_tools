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
        [JsonIgnore]
        public string Name { get; set; }
        public Dictionary<string, string> LocalizedDescription { get; set; }
        [JsonIgnore]
        public string Description { get; set; }

        public int Level { get; set; }
        public string Image { get; set; }
        public string Icon
        {
            get
            {
                var fileId = TorLib.FileId.FromFilePath(string.Format("/resources/gfx/codex/{0}.dds", Image));
                return string.Format("{0}_{1}", fileId.ph, fileId.sh);
            }
        }
        public bool IsHidden { get; set; }

        [JsonIgnore]
        public string CategoryName { get; set; }
        public Dictionary<string, string> LocalizedCategoryName { get; set; }
        [JsonConverter(typeof(LongConverter))]
        public long CategoryId { get; set; }
        [JsonConverter(typeof(Newtonsoft.Json.Converters.StringEnumConverter))]
        public Faction Faction { get; set; }

        [JsonIgnore]
        public List<ClassSpec> Classes { get; set; }
        internal List<string> ClassesB62_ { get; set; }
        public List<string> ClassesB62
        {
            get
            {
                if (ClassesB62_ == null)
                {
                    if (Classes == null) return new List<string>();
                    ClassesB62_ = Classes.Select(x => x.Base62Id).ToList();
                }
                return ClassesB62_;
            }
        }
        public bool ClassRestricted { get; set; }

        public bool IsPlanet { get; set; }

        public bool HasPlanets { get; set; }

        [JsonIgnore]
        public List<Codex> Planets { get; set; }
        [JsonIgnore]
        public List<ulong> PlanetsIds { get; set; }
        internal List<string> PlanetsB62_ { get; set; }
        public List<string> PlanetsB62
        {
            get
            {
                if (PlanetsB62_ == null)
                {
                    if (Planets == null) return new List<string>();
                    PlanetsB62_ = PlanetsIds.Select(x => x.ToMaskedBase62()).ToList();
                }
                return PlanetsB62_;
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

            if (!(obj is Codex cdx)) return false;

            return Equals(cdx);
        }

        public bool Equals(Codex cdx)
        {
            if (cdx == null) return false;

            if (ReferenceEquals(this, cdx)) return true;

            if (CategoryId != cdx.CategoryId)
                return false;
            if (CategoryName != cdx.CategoryName)
                return false;
            if (Classes != null)
            {
                if (cdx.Classes != null)
                {
                    if (Classes.Count != cdx.Classes.Count
                        && (!Classes.Select(x => x.Name).ToList().SequenceEqual(cdx.Classes.Select(x => x.Name).ToList())))
                        return false;
                }
            }
            else if (cdx.Classes != null)
                return false;
            if (ClassRestricted != cdx.ClassRestricted)
                return false;
            if (Faction != cdx.Faction)
                return false;
            if (Fqn != cdx.Fqn)
                return false;
            if (HasPlanets != cdx.HasPlanets)
                return false;
            if (Id != cdx.Id)
                return false;
            if (Image != cdx.Image)
                return false;
            if (IsHidden != cdx.IsHidden)
                return false;
            if (IsPlanet != cdx.IsPlanet)
                return false;
            if (Level != cdx.Level)
                return false;

            var ssComp = new DictionaryComparer<string, string>();
            if (!ssComp.Equals(LocalizedDescription, cdx.LocalizedDescription))
                return false;
            if (!ssComp.Equals(LocalizedName, cdx.LocalizedName))
                return false;

            if (Planets != null)
            {
                if (!Planets.SequenceEqual(cdx.Planets))
                    return false;
            }
            else if (cdx.Planets != null)
                return false;
            if (PlanetsIds != null)
            {
                if (!PlanetsIds.SequenceEqual(cdx.PlanetsIds))
                    return false;
            }
            else if (cdx.PlanetsIds != null)
                return false;

            if (Description != cdx.Description)
                return false;
            if (Name != cdx.Name)
                return false;
            return true;
        }

        public override List<SQLProperty> SQLProperties
        {
            get
            {
                return new List<SQLProperty>
                    {                //(SQL Column Name, C# Property Name, SQL Column type statement, isUnique/PrimaryKey, Serialize value to json)
                        new SQLProperty("Name", "Name", "varchar(255) COLLATE utf8_unicode_ci NOT NULL", SQLPropSetting.AddIndex),
                        new SQLProperty("FrName", "LocalizedName[frMale]", "varchar(255) COLLATE utf8_unicode_ci NOT NULL", SQLPropSetting.AddIndex),
                        new SQLProperty("DeName", "LocalizedName[deMale]", "varchar(255) COLLATE utf8_unicode_ci NOT NULL", SQLPropSetting.AddIndex),
                        new SQLProperty("Base62Id", "Base62Id", "varchar(7) COLLATE utf8_unicode_ci NOT NULL", SQLPropSetting.PrimaryKey),
                        new SQLProperty("Category","LocalizedCategoryName[enMale]", "varchar(255) COLLATE utf8_unicode_ci NOT NULL", SQLPropSetting.AddIndex),
                        new SQLProperty("FrCategory", "LocalizedCategoryName[frMale]", "varchar(255) COLLATE utf8_unicode_ci NOT NULL", SQLPropSetting.AddIndex),
                        new SQLProperty("DeCategory", "LocalizedCategoryName[deMale]", "varchar(255) COLLATE utf8_unicode_ci NOT NULL", SQLPropSetting.AddIndex),
                        new SQLProperty("Faction","Faction", "varchar(25) COLLATE utf8_unicode_ci NOT NULL", SQLPropSetting.AddIndex),
                        new SQLProperty("HasPlanets","HasPlanets", "tinyint(1) NOT NULL", SQLPropSetting.AddIndex),
                        new SQLProperty("IsPlanet","IsPlanet", "tinyint(1) NOT NULL", SQLPropSetting.AddIndex),
                        new SQLProperty("IsHidden","IsHidden", "tinyint(1) NOT NULL", SQLPropSetting.AddIndex),
                        new SQLProperty("Level", "Level", "int(11) NOT NULL", SQLPropSetting.AddIndex),
                    };
            }
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
                    new XElement("Text", (LocalizedDescription != null ? LocalizedDescription["enMale"] : "")));
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
