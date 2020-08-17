namespace GomLib.Tables
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using GomLib.Models;

    /// <summary>GomLib.Tables.ArmorPerLevel.TableData[ArmorSpec][quality][ilvl][ArmorSlot]</summary>
    public class SchematicVariationsPrototype
    {
        private readonly DataObjectModel _dom;

        public SchematicVariationsPrototype(DataObjectModel dom)
        {
            _dom = dom;
            LoadData();
        }

        private Dictionary<ulong, Dictionary<int, int>> prf_schemvarprototype_data;
        readonly string prfSchematicVariationsPrototypePath = "prfSchematicVariationsPrototype";
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
                prf_schemvarprototype_data.Clear();
                prf_schemvarprototype_data = null;
            }
            disposed = true;
        }

        ~SchematicVariationsPrototype()
        {
            Dispose(false);
        }

        public Dictionary<ulong, Dictionary<int, int>> TableData
        {
            get
            {
                if (prf_schemvarprototype_data == null) { LoadData(); }
                return prf_schemvarprototype_data;
            }
        }

        public int GetModPkgTblPrototype(ulong id, int variant)
        {
            //if (level <= 0) { return new List<ulong>(); }

            if (prf_schemvarprototype_data == null) { LoadData(); }

            return prf_schemvarprototype_data[id][variant];
        }

        private void LoadData()
        {
            GomObject table = _dom.GetObject(prfSchematicVariationsPrototypePath);
            Dictionary<object, object> tableData = table.Data.Get<Dictionary<object, object>>("prfSchematicVariationMasterList");
            prf_schemvarprototype_data = new Dictionary<ulong, Dictionary<int, int>>();
            foreach (var kvp in tableData)
            {
                ulong itemid = (ulong)kvp.Key;
                Dictionary<object, object> qlist = (Dictionary<object, object>)kvp.Value;
                _ = new Dictionary<int, Dictionary<int, Dictionary<int, int>>>();

                Dictionary<int, int> qData = new Dictionary<int, int>();
                foreach (var kvp2 in qlist)
                {
                    qData[(int)(long)kvp2.Key] = (int)(long)kvp2.Value;
                }
                prf_schemvarprototype_data.Add(itemid, qData);
            }
        }
    }
}
