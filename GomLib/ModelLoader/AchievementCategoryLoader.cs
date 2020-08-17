using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GomLib.Models;

namespace GomLib.ModelLoader
{
    public class AchievementCategoryLoader
    {
        public Dictionary<object, object> achCategoriesData;
        readonly DataObjectModel _dom;

        public AchievementCategoryLoader(DataObjectModel dom)
        {
            _dom = dom;
            Flush();
        }

        public void Flush()
        {
            achCategoriesData = new Dictionary<object, object>();
        }

        public Models.AchievementCategory Load(long id)
        {
            if (achCategoriesData.Count == 0)
            {
                achCategoriesData = _dom.GetObject("achCategoriesTable_Prototype").Data.Get<Dictionary<object, object>>("achCategoriesData");
            }

            _ = new object();
            achCategoriesData.TryGetValue(id, out object achData);

            Models.AchievementCategory ach = new AchievementCategory();
            return Load(ach, id, (GomObjectData)achData);
        }

        public Models.AchievementCategory Load(Models.AchievementCategory model, long Id, GomObjectData obj)
        {
            if (obj == null) { return model; }
            if (model == null) { return null; }

            //model.NodeId = obj.ValueOrDefault<long>("repGroupInfoId", 0);

            model.CatId = Id;
            model.Icon = obj.ValueOrDefault<string>("achCategoriesIcon", "");
            model.CodexIcon = obj.ValueOrDefault<string>("achCategoriesCodexIcon", "");
            model.NameId = obj.ValueOrDefault<ulong>("achCategoriesStrRetrID");
            model.Name = _dom.stringTable.TryGetString("str.gui.achievementcategories", (long)(model.NameId), "enMale");
            model.LocalizedName = _dom.stringTable.TryGetLocalizedStrings("str.gui.achievementcategories", (long)(model.NameId));
            model.Index = obj.ValueOrDefault<long>("achCategoriesIndexPos");

            //Subcategories
            model.SubCategories = new List<long>();
            foreach (long subcat in obj.Get<List<object>>("achCategoriesChildCatList"))
            {
                model.SubCategories.Add(subcat);
            }
            model.ParentCategory = obj.ValueOrDefault<long>("achCategoriesParentCat");

            //Achievements
            model.Rows = new List<List<AchievementCategoryEntry>>();
            foreach (KeyValuePair<object, object> achRow in obj.Get<Dictionary<object, object>>("achCategoriesAchTable"))
            {
                List<AchievementCategoryEntry> tmpRow = new List<AchievementCategoryEntry>();
                foreach (KeyValuePair<object, object> achievement in (Dictionary<object, object>)achRow.Value)
                {
                    AchievementCategoryEntry tmpEntry = new AchievementCategoryEntry
                    {
                        Id = ((GomObjectData)achievement.Value).ValueOrDefault<ulong>("achCategoriesAchID"),
                        DrawArrow = ((GomObjectData)achievement.Value).ValueOrDefault<bool>("achCategoriesLinkedAch")
                    };
                    tmpRow.Add(tmpEntry);
                }
                model.Rows.Add(tmpRow);
            }

            return model;
        }
    }
}
