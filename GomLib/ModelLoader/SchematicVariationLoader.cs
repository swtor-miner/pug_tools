using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GomLib.ModelLoader
{
    public class SchematicVariationLoader
    {
        private DataObjectModel _dom;
        private Dictionary<object, object> _modifierDict;

        public SchematicVariationLoader(DataObjectModel dom)
        {
            _dom = dom;
            Flush();
        }

        public void Flush()
        {
            _modifierDict = null;
        }

        public Models.SchematicVariation Load(ulong id)
        {
            GomObject prototypeObj = _dom.GetObject("prfSchematicVariationsPrototype");
            Dictionary<object, object> prototypeTable = new Dictionary<object, object>();
            if (prototypeObj != null) //fix to ensure old game assets don't throw exceptions.
            {
                prototypeTable = prototypeObj.Data.Get<Dictionary<object, object>>("prfSchematicVariationMasterList");
                prototypeObj.Unload();
            }

            object data;
            prototypeTable.TryGetValue(id, out data);

            Models.SchematicVariation variation = new Models.SchematicVariation();
            return Load(variation, id, data as Dictionary<object, object>);
        }

        public Models.SchematicVariation Load(Models.SchematicVariation variation, ulong id, Dictionary<object, object> objDict)
        {
            //Why do we pass the variation object and return it??
            if (objDict == null) { return variation; }
            if (variation == null) { return null; }

            if(_modifierDict == null)
            {
                //Load the modifier list
                GomObject modpackTable = _dom.GetObject("itmModifierPackageTablePrototype");
                _modifierDict = modpackTable.Data.ValueOrDefault<Dictionary<object, object>>("itmModifierNewPackagesList", new Dictionary<object, object>()); //I really should remove .Get()
                modpackTable.Unload();
            }

            GomLib.Models.Schematic baseSchem = _dom.schematicLoader.Load(id);

            string baseItemName;
            long baseItemLevel = 0;
            if (baseSchem.ItemId != 0 && baseSchem.Item != null)
            {
                baseItemName = baseSchem.Item.Name;
                baseItemLevel = baseSchem.Item.CombinedRequiredLevel;
            } else {
                baseItemName = string.Empty;
            }
            StringTable strTable = _dom.stringTable.Find("str.gui.itm.modifiers");

            variation.Name = baseItemName;
            variation.SchemId = id;
            variation.Id = (long)id;

            List<Models.ModPackage> variationList = new List<Models.ModPackage>();
            foreach (long variationID in objDict.Values)
            {
                object modifier;
                if (_modifierDict.TryGetValue(variationID, out modifier))
                {
                    List<object> modifierDataList = modifier as List<object>;
                    for (int i = 0; i < modifierDataList.Count(); i++)
                    {
                        GomObjectData modifierData = modifierDataList[i] as GomObjectData;
                        //Load the modfier name.
                        long strID = modifierData.Get<long>("itmModPkgNameId");
                        string variationName = strTable.GetText(strID, string.Empty);
                        variationName = variationName.Replace("<<1>>", baseItemName);

                        long min = modifierData.ValueOrDefault<long>("itmModPkgMinLevel", 0);
                        long max = modifierData.ValueOrDefault<long>("itmModPkgMaxLevel", 0);
                        Dictionary<object, object> statModDict = modifierData.ValueOrDefault<Dictionary<object, object>>("itmModPkgAttributePercentages", new Dictionary<object, object>());

                        //Key to stat enum, value is the percentage divided by 100.
                        Dictionary<Models.DetailedStat, float> statValuesByStat = statModDict.ToDictionary(x => _dom.statData.ToStat((ScriptEnum)x.Key),
                            x => ((long)x.Value / 100f));
                        Models.ModPackage package = new Models.ModPackage(variationID, strID, variationName, statValuesByStat, min, max);
                        variationList.Add(package);
                    }
                }
            }
            variation.VariationPackages = variationList;

            return variation;
        }
    }
}
