using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GomLib.ModelLoader
{
    public class AbilityPackageLoader
    {
        private Dictionary<string, Models.AbilityPackage> nameMap;
        private Dictionary<ulong, Models.AbilityPackage> idMap;

        private DataObjectModel _dom;
        public AbilityPackageLoader(DataObjectModel dom)
        {
            _dom = dom;
            if (nameMap == null)
            {
                Flush();
            }
        }

        public void Flush()
        {
            nameMap = new Dictionary<string, Models.AbilityPackage>();
            idMap = new Dictionary<ulong, Models.AbilityPackage>();
        }

        public Models.AbilityPackage Load(ulong nodeId)
        {
            Models.AbilityPackage pkg = null;
            if (idMap.TryGetValue(nodeId, out pkg))
            {
                return pkg;
            }

            var obj = _dom.GetObject(nodeId);
            return Load(obj);
        }

        public Models.AbilityPackage Load(string fqn)
        {
            Models.AbilityPackage pkg = null;
            if (nameMap.TryGetValue(fqn, out pkg))
            {
                return pkg;
            }

            var obj = _dom.GetObject(fqn);
            return Load(obj);
        }

        public Models.AbilityPackage Load(GomObject obj)
        {
            if (obj == null) { return null; }

            if (nameMap.ContainsKey(obj.Name))
            {
                return nameMap[obj.Name];
            }

            Models.AbilityPackage pkg = new Models.AbilityPackage();
            pkg._dom = _dom;
            pkg.References = obj.References;
            pkg.Fqn = obj.Name;
            pkg.Id = obj.Id;

            IDictionary<object, object> ablList = obj.Data.ValueOrDefault<IDictionary<object, object>>("ablPackageAbilitiesList", null);
            if (ablList != null)
            {
                foreach (var kvp in ablList)
                {
                    // Load PackageAbility from kvp.Value
                    Models.PackageAbility pkgAbl = _dom.packageAbilityLoader.Load((GomObjectData)kvp.Value);

                    // Add PackageAbility to pkg
                    pkg.PackageAbilities.Add(pkgAbl);
                }
            }

            pkg.IsUtilityPackage = false;
            IDictionary<object, object> talList = obj.Data.ValueOrDefault<IDictionary<object, object>>("ablPackageTalentsList", null);
            if (talList != null)
            {
                pkg.IsUtilityPackage = true;
                foreach (var kvp in talList)
                {
                    // Load PackageAbility from kvp.Value
                    var pkgAbl = _dom.packageAbilityLoader.LoadTalent((GomObjectData)kvp.Value);

                    // Add PackageTalent to pkg
                    pkg.PackageTalents.Add(pkgAbl);
                }
            }

            nameMap[obj.Name] = pkg;
            idMap[obj.Id] = pkg;
            return pkg;
        }
    }
}
