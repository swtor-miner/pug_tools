using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Newtonsoft.Json;
using System.Xml.Linq;

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

        public virtual string ToString() { return ToString(true); }
        public virtual string ToString(bool verbose) { return ""; }

        public virtual XElement ToXElement(GomObject gomItm) { return ToXElement(gomItm, true); }
        public virtual XElement ToXElement(GomObject gomItm, bool verbose)
        {
            GameObject obj = Load(gomItm);
            return obj.ToXElement(verbose);
        }
        public virtual XElement ToXElement(ulong nodeId, DataObjectModel dom) { return ToXElement(nodeId, dom, false); }
        public virtual XElement ToXElement(ulong nodeId, DataObjectModel dom, bool verbose)
        {
            GameObject obj = Load(nodeId, dom);
            if (obj != null)
                return obj.ToXElement(verbose);
            else
                return new XElement("NotFound", nodeId);
        }
        public virtual XElement ToXElement(string fqn, DataObjectModel dom) { return ToXElement(fqn, dom, false); }
        public virtual XElement ToXElement(string fqn, DataObjectModel dom, bool verbose)
        {
            GameObject obj = Load(fqn, dom);
            if (obj != null)
                return obj.ToXElement(verbose);
            else
                return new XElement("NotFound", fqn);
        }
        public virtual XElement ToXElement() { return ToXElement(false); }
        public virtual XElement ToXElement(bool verbose) { return new XElement("NotImplemented", this.GetType().ToString()); }

        public virtual HashSet<string> GetDependencies()
        {
            return new HashSet<string>();
        }

        public GameObject Load(ulong nodeId, DataObjectModel dom)
        {
            var gomItm = dom.GetObject(nodeId);
            if (gomItm != null) return Load(gomItm);
            return null;
        }
        public GameObject Load(string fqn, DataObjectModel dom)
        {
            var gomItm = dom.GetObject(fqn);
            if (gomItm != null) return Load(gomItm);
            return null;
        }
        public GameObject Load(GomObject gomItm)
        {
            switch (gomItm.Name.Substring(0, 4))
            {
                case "ach.": return gomItm._dom.achievementLoader.Load(gomItm);
                case "abl.":
                    if (!gomItm.Name.Contains("/"))
                        return gomItm._dom.abilityLoader.Load(gomItm);
                    return null;
                case "apn.": case "apc.": return gomItm._dom.abilityPackageLoader.Load(gomItm);
                case "cdx.": return gomItm._dom.codexLoader.Load(gomItm);
                case "cnv.": return gomItm._dom.conversationLoader.Load(gomItm);
                case "npc.": return gomItm._dom.npcLoader.Load(gomItm);
                case "qst.": return gomItm._dom.questLoader.Load(gomItm);
                case "tal.": return gomItm._dom.talentLoader.Load(gomItm);
                case "sche": return gomItm._dom.schematicLoader.Load(gomItm);
                case "dec.": return gomItm._dom.decorationLoader.Load(gomItm);
                case "itm.": return gomItm._dom.itemLoader.Load(gomItm);
                default:
                    return null;
            }
        }

        public XElement ReferencesToXElement()
        {
            XElement references = new XElement("References");
            if (References != null)
            {
                foreach (KeyValuePair<string, List<ulong>> entry in References)
                {
                    XElement tmpEle = new XElement(entry.Key);
                    foreach (ulong ele in entry.Value)
                    {
                        tmpEle.Add(new XElement("Ref", ele));
                    }
                    references.Add(tmpEle);
                }
            }
            return references;
        }

        public XElement FullReferencesToXElement()
        {
            XElement references = new XElement("References");
            if (FullReferences != null)
            {
                foreach (var entry in FullReferences)
                {
                    XElement tmpEle = new XElement("Ref", entry.Value, new XAttribute("Id", entry.Key));
                    references.Add(tmpEle);
                }
            }
            return references;
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
            JsonSerializerSettings settings = new JsonSerializerSettings();
            settings.NullValueHandling = NullValueHandling.Ignore;
            return ToJSON(settings);
        }
        public string ToJSON(JsonSerializerSettings settings)
        {
            string json = JsonConvert.SerializeObject(this, settings);
            return json;
        }

        public virtual string ToString() { return ToString(true); }
        public virtual string ToString(bool verbose) { return ""; }

        public virtual XElement ToXElement() { return ToXElement(true); }
        public virtual XElement ToXElement(bool verbose) { return new XElement("NotImplemented", this.GetType().ToString()); }

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
