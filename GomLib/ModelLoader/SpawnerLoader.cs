using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GomLib.Models;

namespace GomLib.ModelLoader
{
    public class SpawnerLoader
    {
        HashSet<string> FieldList { get; set; }

        readonly DataObjectModel _dom;

        public SpawnerLoader(DataObjectModel dom)
        {
            _dom = dom;
            Flush();
        }

        public void Flush()
        {
            FieldList = new HashSet<string>();
        }

        public string ClassName
        {
            get { return "spnDispenserTemplate"; }
        }

        public Spawner Load(ulong nodeId)
        {
            GomObject obj = _dom.GetObject(nodeId);
            Spawner spn = new Spawner();
            return Load(spn, obj);
        }

        public Spawner Load(string fqn)
        {
            GomObject obj = _dom.GetObject(fqn);
            Spawner spn = new Spawner();
            return Load(spn, obj);
        }

        public Spawner Load(GomObject obj)
        {
            Spawner spn = new Spawner();
            return Load(spn, obj);
        }

        public Models.Spawner Load(Models.Spawner itm, GomObject obj)
        {
            if (obj == null) { return null; }
            if (itm == null) { return null; }

            itm.Fqn = obj.Name;
            itm.Id = obj.Id;

            itm.References = obj.References;

            FieldList.UnionWith(obj.Data.Dictionary.Keys);

            //itm.
            return itm;
        }

        public void LoadObject(Models.GameObject loadMe, GomObject obj)
        {
            GomLib.Models.Spawner loadObj = (Models.Spawner)loadMe;
            Load(loadObj, obj);
        }

        public void LoadReferences(Models.GameObject obj, GomObject gom)
        {
            if (obj is null)
            {
                throw new ArgumentNullException(nameof(obj));
            }

            if (gom is null)
            {
                throw new ArgumentNullException(nameof(gom));
            }
            // No references to load
        }
    }
}
