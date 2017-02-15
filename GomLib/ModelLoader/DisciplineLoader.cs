using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GomLib.Models;

namespace GomLib.ModelLoader
{
    public class DisciplineLoader
    {
        DataObjectModel _dom;

        public DisciplineLoader(DataObjectModel dom)
        {
            _dom = dom;
            Flush();
        }

        public void Flush()
        {

        }

        public Models.Discipline Load(Models.Discipline model, GomObjectData gom)
        {
            if (gom == null) { return model; }
            if (model == null) { return null; }

            model._dom = _dom;
            model.Icon = gom.ValueOrDefault<string>("classIcon");
            model.SortIdx = gom.ValueOrDefault<long>("disSortIdx");
            model.ClassId = gom.ValueOrDefault<ulong>("disClassId");
            model.PathApcId = gom.ValueOrDefault<ulong>("disApcId");
            model.Id = (long)model.PathApcId;
            model.PathAbilities = _dom.abilityPackageLoader.Load(model.PathApcId);
            model.NameId = gom.ValueOrDefault<long>("disName") + 2031339142381568;
            model.ClassNameId = gom.ValueOrDefault<long>("className") + 2031339142381568;

            var nameTable = _dom.stringTable.Find("str.gui.abl.player.skill_trees");

            model.Name = nameTable.GetText(model.NameId, "str.gui.abl.player.skill_trees");
            model.LocalizedName = nameTable.GetLocalizedText(model.NameId, "str.gui.abl.player.skill_trees");

            model.ClassName = nameTable.GetText(model.ClassNameId, "str.gui.abl.player.skill_trees");
            model.LocalizedClassName = nameTable.GetLocalizedText(model.ClassNameId, "str.gui.abl.player.skill_trees");

            model.Role = gom.ValueOrDefault<ScriptEnum>("disRole", new ScriptEnum()).ToString().Replace("chrRole", "");

            var disDisciplnePreviews = _dom.GetObject("disDisicplinePreviews");
            var disDescBaseTable = disDisciplnePreviews.Data.ValueOrDefault<Dictionary<object, object>>("disDescBaseTable");
            disDisciplnePreviews.Unload();

            object disPrevObj;
            disDescBaseTable.TryGetValue(model.PathApcId, out disPrevObj);

            if (disPrevObj == null) return model;

            model.DescriptionId = ((GomObjectData)disPrevObj).ValueOrDefault<long>("disPreviewDesc");
            var descTable = _dom.stringTable.Find("str.gui.disciplines");
            model.Description = descTable.GetText(model.DescriptionId, "str.gui.disciplines");
            model.LocalizedDescription = descTable.GetLocalizedText(model.DescriptionId, "str.gui.disciplines");

            model.BaseAbilityIds = new Dictionary<ulong, int>();
            for (int i = 1; i < 5; i++)
            {
                ulong baseId = ((GomObjectData)disPrevObj).ValueOrDefault<ulong>(String.Format("disBaseAbl{0}", i));
                int lvl = 0;
                try
                {
                    lvl = model.PathAbilities.PackageAbilities.Where(x => x.AbilityId == baseId).Select(y => y.Level).First();
                }catch (Exception ex)
                {
                    continue;
                }
                model.BaseAbilityIds.Add(baseId, lvl);
            }

            return model;
        }
    }
}
