using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GomLib.Models;

namespace GomLib.ModelLoader
{
    public class AchievementCategoryLoader
    {
        Dictionary<object, object> idLookup;

        DataObjectModel _dom;

        public AchievementCategoryLoader(DataObjectModel dom)
        {
            _dom = dom;
            Flush();
        }

        public void Flush()
        {
            idLookup = new Dictionary<object, object>();
        }

        public Models.AchievementCategory Load(Models.AchievementCategory model, GomObjectData obj)
        {
            if (obj == null) { return model; }
            if (model == null) { return null; }

            //model.NodeId = obj.ValueOrDefault<long>("repGroupInfoId", 0);

            model.Icon = obj.ValueOrDefault<string>("achCategoriesIcon", "");
            model.CodexIcon = obj.ValueOrDefault<string>("achCategoriesCodexIcon", "");
            model.NameId = obj.ValueOrDefault<ulong>("4611686297612174002");
            model.Name = _dom.stringTable.TryGetString("str.gui.achievementcategories", (long)(model.NameId), "enMale");
            model.LocalizedName = _dom.stringTable.TryGetLocalizedStrings("str.gui.achievementcategories", (long)(model.NameId));
            model.Index = obj.ValueOrDefault<long>("4611686297612174003");

            //Subcategories
            model.SubCategories=new List<long>();
            foreach (long subcat in obj.Get<List<object>>("4611686297612174007")) {
                model.SubCategories.Add(subcat);
            }
            model.ParentCategory = obj.ValueOrDefault<long>("4611686297698994000");

            //Achievements
            model.Rows = new List<List<AchievementCategoryEntry>>();
            foreach (KeyValuePair<object, object> achRow in obj.Get<Dictionary<object, object>>("4611686297880004003"))
            {
                List<AchievementCategoryEntry> tmpRow = new List<AchievementCategoryEntry>();
                foreach (KeyValuePair<object, object> achievement in (Dictionary<object, object>)achRow.Value) 
                {
                    AchievementCategoryEntry tmpEntry = new AchievementCategoryEntry();
                    tmpEntry.Id=((GomObjectData)achievement.Value).ValueOrDefault<ulong>("4611686297880004000");
                    tmpEntry.DrawArrow = ((GomObjectData)achievement.Value).ValueOrDefault<bool>("4611686297880004001");
                    tmpRow.Add(tmpEntry);
                }
                model.Rows.Add(tmpRow);
            }

            return model;
        }
    }
}
