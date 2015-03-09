using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GomLib.Models;

namespace GomLib.Tables
{
    /// <summary>GomLib.Tables.ArmorPerLevel.TableData[ArmorSpec][quality][ilvl][ArmorSlot]</summary>
    public class ItemModifierPackageTablePrototype
    {
        [Newtonsoft.Json.JsonIgnore]
        private DataObjectModel _dom;

        public ItemModifierPackageTablePrototype(DataObjectModel dom)
        {
            _dom = dom;
            LoadData();
        }

        private Dictionary<long, Dictionary<string, object>> item_modpkgprototype_data;
        string itmModifierPackageTablePrototypePath = "itmModifierPackageTablePrototype";
        private bool disposed = false;

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (disposed)
                return;
            if (disposing)
            {
                foreach (var dict in item_modpkgprototype_data)
                {
                    dict.Value.Clear();
                }
                item_modpkgprototype_data.Clear();
                item_modpkgprototype_data = null;
            }
            disposed = true;
        }

        ~ItemModifierPackageTablePrototype()
        {
            Dispose(false);
        }
        
        public Dictionary<long, Dictionary<string, object>> TableData
        {
            get
            {
                if (item_modpkgprototype_data == null) { LoadData(); }
                return item_modpkgprototype_data;
            }
        }

        public long GetModPkgNameId(long id)
        {
            if (item_modpkgprototype_data == null) { LoadData(); }
            return (long)item_modpkgprototype_data[id]["itmModPkgNameId"];
        }
        public Dictionary<object, object> GetModPkgStatValues(long id)
        {
            if (item_modpkgprototype_data == null) { LoadData(); }
            return (Dictionary<object, object>)item_modpkgprototype_data[id]["itmModPkgAttributePercentages"];
        }

        private void LoadData()
        {
            GomObject table = _dom.GetObject(itmModifierPackageTablePrototypePath);
            Dictionary<object, object> tableData = table.Data.Get<Dictionary<object, object>>("itmModifierPackagesList");
            item_modpkgprototype_data = new Dictionary<long, Dictionary<string, object>>();
            foreach (var kvp in tableData)
            {
                long modId = (long)kvp.Key;
                Dictionary<string, object> map = (Dictionary<string, object>)((GomLib.GomObjectData)kvp.Value).Dictionary;

                item_modpkgprototype_data[modId] = map;

                //List<List<int>> qData = new List<List<int>>();
                /*foreach (List<object> stats in qlist)
                {
                    var lvlData = new List<int>();
                    foreach (long stat in stats)
                    {
                        lvlData.Add((int)stat);
                    }
                    qData.Add(lvlData);
                }*/
                //item_modpkgprototype_data.Add((int)quality, qData);
            }
        }
    }
}
