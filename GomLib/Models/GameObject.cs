using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Newtonsoft.Json;

namespace GomLib.Models
{
    public class GameObject : Dependencies
    {
        public ulong Id { get; set; }
        public string Fqn { get; set; }
        [Newtonsoft.Json.JsonIgnore]
        public DataObjectModel _dom { get; set; }
        public Dictionary<string, List<ulong>> References { get; set; }
        public Dictionary<ulong, string> FullReferences { get; set; }

        public string ToJSON()
        {
            JsonSerializerSettings settings = new JsonSerializerSettings();
            settings.NullValueHandling = NullValueHandling.Ignore;
            return ToJSON(settings);
        }

        public string ToJSON(JsonSerializerSettings settings)
        {
            string json = JsonConvert.SerializeObject(this, settings);
            return json;
        }

        public virtual HashSet<string> GetDependencies()
        {
            return new HashSet<string>();
        }
    }

    public class PseudoGameObject : Dependencies
    {
        public virtual long Id { get; set; }
        public virtual string Name { get; set; }
        public string Prototype { get; set; } //Which prototype this object is from.
        public string ProtoDataTable { get; set; } //which prototype field contains the object
        [Newtonsoft.Json.JsonIgnore]
        public DataObjectModel _dom { get; set; }

        public string ToJSON()
        {
            string json = JsonConvert.SerializeObject(this);
            return json;
        }

        public virtual HashSet<string> GetDependencies()
        {
            return new HashSet<string>();
        }
    }

    interface Dependencies
    {
        HashSet<string> GetDependencies();
    }
}
