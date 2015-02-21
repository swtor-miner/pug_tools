using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GomLib.Models;

namespace GomLib.ModelLoader
{
    public class AdvancedClassLoader
    {
        const long StringOffset = 0x2F85F00000000;
        private StringTable classNames;
        private StringTable classDescriptions;

        private DataObjectModel _dom;

        public AdvancedClassLoader(DataObjectModel dom)
        {
            _dom = dom;
        }

        public void Flush()
        {
            classNames = null;
        }

        public Models.AdvancedClass Load(GomObject obj)
        {
            if (classNames == null)
            {
                classNames = _dom.stringTable.Find("str.gui.classnames");
                classDescriptions = _dom.stringTable.Find("str.gui.classdescriptions");
            }

            Models.AdvancedClass ac = new Models.AdvancedClass();
            ac._dom = _dom;
            ac.NodeId = obj.Id;
            ac.Fqn = obj.Name;
            ac.NameId = obj.Data.ValueOrDefault<long>("chrAdvancedClassDataNameId", 0);
            ac.Id = (int)ac.NameId;
            ac.Name = classNames.GetText(ac.NameId, obj.Name);
            ac.LocalizedName = classNames.GetLocalizedText(ac.NameId, obj.Name);
            ac.DescriptionId = Convert.ToInt64(obj.Data.ValueOrDefault<string>("chrAdvancedClassDescription"));
            ac.Description = classDescriptions.GetText(ac.DescriptionId, obj.Name);
            ac.LocalizedDescription = classDescriptions.GetLocalizedText(ac.DescriptionId, obj.Description);
            ac.classSpecId = obj.Data.ValueOrDefault<ulong>("chrAdvancedClassDataClassSpec", 0);
            ac.ClassSpec = _dom.classSpecLoader.Load(ac.classSpecId);
            ac.ClassBackground = obj.Data.ValueOrDefault<string>("chrAdvancedClassBackground");


            var ablPackagePrototype = _dom.GetObject("ablPackagePrototype");
            if (ablPackagePrototype == null) return ac;
            var classDisciplinesTable = ablPackagePrototype.Data.ValueOrDefault<Dictionary<object, object>>("classDisciplinesTable");
            var disUtilityTable = ablPackagePrototype.Data.ValueOrDefault<Dictionary<object, object>>("disUtilityTable");
            var classBaseTable = ablPackagePrototype.Data.ValueOrDefault<Dictionary<object, object>>("classBaseTable");
            ablPackagePrototype.Unload();

            if (classDisciplinesTable.ContainsKey((object)ac.NodeId))
            {
                var discData = (List<GomObjectData>)((List<object>)classDisciplinesTable[(object)ac.NodeId]).ConvertAll(x => (GomObjectData)x);

                ac.Disciplines = new List<Discipline>();
                foreach (var disc in discData)
                {
                    Discipline dis = new Discipline();
                    _dom.disciplineLoader.Load(dis, disc);
                    ac.Disciplines.Add(dis);
                }
                ac.Disciplines = ac.Disciplines.OrderBy(x => x.SortIdx).ToList();


                if (disUtilityTable.ContainsKey(ac.NodeId))
                {
                    var entry = (GomObjectData)disUtilityTable[ac.NodeId];
                    ac.UtiltyPkgId = entry.ValueOrDefault<ulong>("disApcId");
                    ac.UtilPkgIsActive = entry.ValueOrDefault<bool>("disUtilPkgActive");

                    var backupNameId = entry.ValueOrDefault<long>("className") + 2031339142381568;
                    var unusedStringId = entry.ValueOrDefault<long>("disName") + 2031339142381568;
                    var nameTable = _dom.stringTable.Find("str.gui.abl.player.skill_trees");
                    string backupName = nameTable.GetText(backupNameId, "str.gui.abl.player.skill_trees");
                    string unusedString = nameTable.GetText(unusedStringId, "str.gui.abl.player.skill_trees");
                }
                if (classBaseTable.ContainsKey((object)ac.NodeId))
                {
                    var entries = (List<GomObjectData>)((List<object>)classBaseTable[ac.NodeId]).ConvertAll(x => (GomObjectData)x);

                    ac.AdvancedClassPkgIds = new List<ulong>();
                    foreach (var entry in entries)
                        ac.AdvancedClassPkgIds.Add(entry.ValueOrDefault<ulong>("disApcId"));

                    entries = (List<GomObjectData>)((List<object>)classBaseTable[ac.classSpecId]).ConvertAll(x => (GomObjectData)x);

                    ac.BaseClassPkgIds = new List<ulong>();
                    foreach (var entry in entries)
                        ac.BaseClassPkgIds.Add(entry.ValueOrDefault<ulong>("disApcId"));
                }
            }

            return ac;
        }
    }
}
