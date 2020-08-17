using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GomLib.ModelLoader
{
    public class ClassSpecLoader
    {
        StringTable classNames;
        Dictionary<ulong, Models.ClassSpec> idMap;
        Dictionary<string, Models.ClassSpec> nameMap;

        private readonly DataObjectModel _dom;

        public ClassSpecLoader(DataObjectModel dom)
        {
            _dom = dom;
            if (nameMap == null)
            {
                Flush();
            }
        }

        public void Flush()
        {
            classNames = null;
            idMap = new Dictionary<ulong, Models.ClassSpec>();
            nameMap = new Dictionary<string, Models.ClassSpec>();
        }

        public Models.ClassSpec Load(ulong nodeId)
        {
            if (idMap.TryGetValue(nodeId, out Models.ClassSpec result))
            {
                return result;
            }

            var obj = _dom.GetObject(nodeId);
            if (obj == null) { return null; }
            return Load(obj);
        }

        public Models.ClassSpec Load(string fqn)
        {
            if (nameMap.TryGetValue(fqn, out Models.ClassSpec result))
            {
                return result;
            }

            var obj = _dom.GetObject(fqn);
            return Load(obj);
        }

        public Models.ClassSpec Load(GomObject obj)
        {
            if (classNames == null)
            {
                classNames = _dom.stringTable.Find("str.gui.classnames");
            }

            if (nameMap.ContainsKey(obj.Name)) //check if ClassSpec is already loaded
                return nameMap[obj.Name];

            Models.ClassSpec spec = new Models.ClassSpec
            {
                Id = obj.Id,
                Fqn = obj.Name,
                Dom_ = _dom,
                References = obj.References,
                IsPlayerClass = obj.Name.StartsWith("class.pc.")
            };
            if (spec.IsPlayerClass)
            {
                spec.IsPlayerAdvancedClass = obj.Name.StartsWith("class.pc.advanced");
            }
            else
            {
                spec.IsPlayerAdvancedClass = false;
            }

            if (!spec.IsPlayerAdvancedClass)
            {
                spec.AbilityPackageId = obj.Data.ValueOrDefault<ulong>("chrAbilityPackage", 0);
                spec.AlignmentDark = (int)obj.Data.ValueOrDefault<float>("chrAlignmentDark", 0);
                spec.AlignmentLight = (int)obj.Data.ValueOrDefault<float>("chrAlignmentLight", 0);
                spec.Icon = obj.Data.ValueOrDefault<string>("chrClassDataIcon", null);
                spec.NameId = obj.Data.ValueOrDefault<long>("chrClassDataNameId", 0); // Index into str.gui.classnames
            }
            else
            {
                spec.NameId = obj.Data.ValueOrDefault<long>("chrAdvancedClassDataNameId", 0);
                spec.Icon = null;
                spec.AlignmentDark = 0;
                spec.AlignmentLight = 0;

            }
            //spec.Id = (int)spec.NameId;

            spec.Name = classNames.GetText(spec.NameId, obj.Name);
            spec.LocalizedName = classNames.GetLocalizedText(spec.NameId, obj.Name);
            if (String.IsNullOrEmpty(spec.Name))
            {
                spec.Name = obj.Data.ValueOrDefault<string>("chrClassDataName", "");
            }

            idMap[obj.Id] = spec;
            nameMap[obj.Name] = spec;

            spec.AbilityPackage = _dom.abilityPackageLoader.Load(spec.AbilityPackageId);

            obj.Unload();
            return spec;
        }
    }
}
