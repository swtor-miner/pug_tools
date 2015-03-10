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
        public string Name { get; set; }
        public Dictionary<string, string> LocalizedName { get; set; }
        public long Index { get; set; }
        public List<long> SubCategories { get; set; }
        public long ParentCategory { get; set; }
        public List<List<AchievementCategoryEntry>> Rows { get; set; }

        //[Newtonsoft.Json.JsonIgnore]

        public override int GetHashCode()
        {
            int hash = CatId.GetHashCode();
            hash ^= Icon.GetHashCode();
            hash ^= CodexIcon.GetHashCode();
            hash ^= NameId.GetHashCode();
            hash ^= Name.GetHashCode();
            hash ^= LocalizedName.GetHashCode();
            hash ^= Index.GetHashCode();
            //TODO: SubCategories
            hash ^= ParentCategory.GetHashCode();
            //TODO: Achievements
            return hash;
        }

        public override bool Equals(object obj)
        {
            if (obj == null) return false;

            if (ReferenceEquals(this, obj)) return true;

            AchievementCategory obj2 = obj as AchievementCategory;
            if (obj2 == null) return false;

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

        public override string ToSQL(string patchVersion)
        {
            JsonSerializerSettings settings = new JsonSerializerSettings();
            settings.NullValueHandling = NullValueHandling.Ignore;
            string subcat = JsonConvert.SerializeObject(SubCategories, settings);
            string rows = JsonConvert.SerializeObject(Rows, settings);

            string s = "', '";
            string value = "('" + sqlSani(patchVersion) + s + s + CatId + s + sqlSani(Name) + s + NameId + s + Index + s + ParentCategory + s + sqlSani(CodexIcon) + s + sqlSani(Icon) + s + sqlSani(subcat) + s + sqlSani(rows) + s + GetHashCode() + "')";
            return value;
        }

        public override XElement ToXElement(bool verbose)
        {
            XElement item = new XElement("AchievementCategory");

            item.Add(new XElement("UnImplemented"));
            return item;
        }
    }
    public class AchievementCategoryEntry
    {
        public ulong Id;//Id of the achievement
        public bool DrawArrow;//Whether to draw an arrow to the next achievement on this row, indicating that that achievement is a continuation of the current achievement
    }
}
