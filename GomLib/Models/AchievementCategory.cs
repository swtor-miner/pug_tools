using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;
using Newtonsoft.Json;

namespace GomLib.Models
{
    public class AchievementCategory : PseudoGameObject, IEquatable<AchievementCategory>
    {
        //public Dictionary<string, string> LocalizedTitle { get; set; }
        //public string Title { get; set; }

        public long CatId { get; set; }
        public string Icon { get; set; }
        public string CodexIcon { get; set; }
        public ulong NameId { get; set; }
        public Dictionary<string, string> LocalizedName { get; set; }
        public long Index { get; set; }
        [JsonIgnore]
        public List<long> SubCategories { get; set; }
        public long ParentCategory { get; set; }
        [JsonIgnore]
        public List<List<AchievementCategoryEntry>> Rows { get; set; }

        //[Newtonsoft.Json.JsonIgnore]

        public override int GetHashCode()
        {
            int hash = CatId.GetHashCode();
            hash ^= Icon.GetHashCode();
            hash ^= CodexIcon.GetHashCode();
            hash ^= NameId.GetHashCode();
            hash ^= Name.GetHashCode();
            foreach (var x in LocalizedName) { hash ^= x.GetHashCode(); } //dictionaries need to hashed like this
            //hash ^= LocalizedName.GetHashCode(); //not like this
            hash ^= Index.GetHashCode();
            //SubCategories
            foreach (var x in SubCategories) { hash ^= x.GetHashCode(); }
            hash ^= ParentCategory.GetHashCode();
            //Achievements
            foreach (var x in Rows) foreach (var y in x) { hash ^= y.GetHashCode(); }
            return hash;
        }

        public override bool Equals(object obj)
        {
            if (obj == null) return false;

            if (ReferenceEquals(this, obj)) return true;

            if (!(obj is AchievementCategory obj2)) return false;

            return Equals(obj2);
        }

        public bool Equals(AchievementCategory obj)
        {
            if (obj == null) return false;

            if (ReferenceEquals(this, obj)) return true;

            if (this.CatId != obj.CatId)
                return false;
            if (this.Icon != obj.Icon)
                return false;
            if (this.CodexIcon != obj.CodexIcon)
                return false;
            if (this.NameId != obj.NameId)
                return false;
            if (this.Name != obj.Name)
                return false;
            if (this.Index != obj.Index)
                return false;
            //TODO: SubCategories
            if (this.ParentCategory != obj.ParentCategory)
                return false;
            //TODO: Achievements

            return true;
        }

        public override List<SQLProperty> SQLProperties
        {
            get
            {
                return new List<SQLProperty>
                    {                //(SQL Column Name, C# Property Name, SQL Column type statement, isUnique/PrimaryKey, Serialize value to json)
                        new SQLProperty("CatId", "CatId", "bigint(20) signed NOT NULL", true),
                        new SQLProperty("Name", "Name", "varchar(255) COLLATE utf8_unicode_ci NOT NULL"),
                        new SQLProperty("NameId", "NameId", "bigint(20) NOT NULL"),
                        new SQLProperty("Index", "Index", "int(11) NOT NULL"),
                        new SQLProperty("ParentCatId", "ParentCategory", "bigint(20) unsigned NOT NULL"),
                        new SQLProperty("CodexIcon", "CodexIcon", "varchar(255) COLLATE utf8_unicode_ci NOT NULL"),
                        new SQLProperty("Icon", "Icon", "varchar(255) COLLATE utf8_unicode_ci NOT NULL"),
                        new SQLProperty("SubCategories", "SubCategories", "varchar(600) COLLATE utf8_unicode_ci NOT NULL", false, true),
                        new SQLProperty("Rows", "Rows", "varchar(3000) COLLATE utf8_unicode_ci NOT NULL", false, true)
                    };
            }
        }

        public override XElement ToXElement(bool verbose)
        {
            XElement item = new XElement("AchievementCategory");

            item.Add(new XElement("UnImplemented", new XAttribute("Hash", GetHashCode())));
            return item;
        }
    }
    public class AchievementCategoryEntry : IEquatable<AchievementCategoryEntry>
    {
        public ulong Id;//Id of the achievement
        public bool DrawArrow;//Whether to draw an arrow to the next achievement on this row, indicating that that achievement is a continuation of the current achievement

        public override int GetHashCode()
        {
            int hash = Id.GetHashCode();
            hash ^= DrawArrow.GetHashCode();
            return hash;
        }

        public override bool Equals(object obj)
        {
            if (obj == null) return false;

            if (ReferenceEquals(this, obj)) return true;

            if (!(obj is AchievementCategoryEntry obj2)) return false;

            return Equals(obj2);
        }

        public bool Equals(AchievementCategoryEntry obj)
        {
            if (obj == null) return false;

            if (ReferenceEquals(this, obj)) return true;

            if (this.Id != obj.Id)
                return false;
            if (this.DrawArrow != obj.DrawArrow)
                return false;

            return true;
        }
    }
}
