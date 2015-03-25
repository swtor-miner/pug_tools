using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;
using System.Xml.Linq;

namespace GomLib
{
    public class AMI : Models.GameObject, IEquatable<AMI>
    {
        [Newtonsoft.Json.JsonIgnore]
        private Dictionary<string, AMI> fqnMap;
        public Dictionary<long, AMIEntry> data;
        bool loaded = false;

        public AMI(DataObjectModel dom)
        {
            _dom = dom;
            Flush();
        }

        public void Flush()
        {
            fqnMap = new Dictionary<string, AMI>();
            data = new Dictionary<long, AMIEntry>();
            loaded = false;
        }

        public AMIEntry Find(string fqn, long id)
        {
            if (String.IsNullOrEmpty(fqn)) { return null; }

            if (!loaded)
                Load();

            AMI table;

            if (fqnMap.Count == 0)
                this.Load();

            if (fqnMap.TryGetValue("ami." + fqn, out table))
            {
                AMIEntry result = table.GetEntry(id);
                return result;
            }

            return null;
        }

        public AMIEntry GetEntry(long id)
        {
            if (!loaded)
                Load();

            AMIEntry result;
            if (data.TryGetValue(id, out result))
            {
                return result;
            }

            return null;
        }

        public AMI GetAMI(string fqn)
        {
            if (!loaded)
                Load();

            AMI result;
            if (fqnMap.TryGetValue(fqn, out result))
            {
                return result;
            }

            return null;
        }

        public void Load()
        {
            var amis = _dom.GetObjectsStartingWith("ami.");

            foreach (var ami in amis)
            {
                AMI table = new AMI(_dom);

                table.Fqn = ami.Name;
                table.Id = ami.Id;
                var entries = ami.Data.Get<Dictionary<object, object>>("appModelDetails");
                table.data = new Dictionary<long, AMIEntry>();
                //Console.WriteLine(table.Fqn);
                foreach (var entry in entries)
                {
                    AMIEntry ame = new AMIEntry();
                    ame.Load((GomObjectData)(entry.Value));
                    table.data.Add((long)entry.Key, ame);
                }
                this.fqnMap.Add(table.Fqn, table);
            }

            loaded = true;
        }

        public void UnLoad()
        {
            fqnMap = null;
            data = null;
            Fqn = null;
            loaded = false;
        }

        public override int GetHashCode()
        {
            int hash = Id.GetHashCode();
            if (Fqn != null) hash ^= Fqn.GetHashCode();
            if (data != null) foreach (var x in data) { hash ^= x.GetHashCode(); }

            return hash;
        }

        public override bool Equals(object obj)
        {
            if (obj == null) return false;

            if (ReferenceEquals(this, obj)) return true;

            AMI stb = obj as AMI;
            if (stb == null) return false;

            return Equals(stb);
        }

        public bool Equals(AMI stb)
        {
            if (stb == null) return false;

            if (ReferenceEquals(this, stb)) return true;

            if (this.Fqn != stb.Fqn)
                return false;

            var ssComp = new Models.DictionaryComparer<long, AMIEntry>();
            if (!ssComp.Equals(this.data, stb.data))
                return false;
            return true;
        }
    }
}
