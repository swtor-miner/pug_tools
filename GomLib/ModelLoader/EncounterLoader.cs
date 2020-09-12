using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TorLib;
using GomLib.Models;

namespace GomLib.ModelLoader
{
    public class EncounterLoader
    {
        Dictionary<ulong, Encounter> idMap;
        Dictionary<string, Encounter> nameMap;
        readonly DataObjectModel _dom;

        public EncounterLoader(DataObjectModel dom)
        {
            _dom = dom;
            Flush();
        }

        public void Flush()
        {
            idMap = new Dictionary<ulong, Encounter>();
            nameMap = new Dictionary<string, Encounter>();
        }

        public string ClassName
        {
            get { return "spnEncounterController"; }
        }

        public Encounter Load(ulong nodeId)
        {
            if (idMap.TryGetValue(nodeId, out Encounter result))
            {
                return result;
            }

            GomObject obj = _dom.GetObject(nodeId);
            Encounter enc = new Encounter();
            return Load(enc, obj);
        }

        public Encounter Load(string fqn)
        {
            if (nameMap.TryGetValue(fqn, out Encounter result))
            {
                return result;
            }

            GomObject obj = _dom.GetObject(fqn);
            Encounter enc = new Encounter();
            return Load(enc, obj);
        }

        public Encounter Load(Encounter enc, GomObject obj)
        {
            if (obj == null) { return null; }
            if (enc == null) { return null; }

            enc.Fqn = obj.Name;
            enc.Id = obj.Id;
            enc.References = obj.References;

            enc.Spawners = new Dictionary<string, string>();

            if (obj.Data.ContainsKey("spnEncounterSpawnerIdsToFqns"))
            {
                Dictionary<object, object> spawnerIdToFqn = obj.Data.ValueOrDefault<Dictionary<object, object>>("spnEncounterSpawnerIdsToFqns", null);
                foreach (var kvp in spawnerIdToFqn)
                {
                    enc.Spawners.Add(kvp.Key.ToString().ToLower(), kvp.Value.ToString());
                }
            }

            ulong hydraScriptId = obj.Data.ValueOrDefault<ulong>("plcHydraId", 0);
            if (hydraScriptId > 0)
            {
                //enc.HydraScript = HydraScriptLoader.Load(hydraScriptId);
            }

            return enc;
        }

        public void LoadObject(GameObject loadMe, GomObject obj)
        {
            Encounter loadObj = (Encounter)loadMe;
            Load(loadObj, obj);
        }

        public void LoadReferences(GameObject obj, GomObject gom)
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
