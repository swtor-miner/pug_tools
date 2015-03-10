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

        public override string ToString() { return ToString(true); }
        public virtual string ToString(bool verbose) { return ""; }

        public virtual string ToSQL(string patchVersion) { return "Unsupported"; }

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

        public static string sqlSani(string str)
        {
            if (str == null) return "";
            return MySql.Data.MySqlClient.MySqlHelper.EscapeString(str);
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

        public override string ToString() { return ToString(true); }
        public virtual string ToString(bool verbose) { return ""; }

        public virtual string ToSQL(string patchVersion) { return "Unsupported"; }

        public virtual XElement ToXElement() { return ToXElement(true); }
        public virtual XElement ToXElement(bool verbose) { return new XElement("NotImplemented", this.GetType().ToString()); }

        public static PseudoGameObject Load(string xmlRoot, DataObjectModel dom, object id, object gomObjectData)
        {
            switch (xmlRoot)
            {
                case "MtxStoreFronts":
                    GomLib.Models.MtxStorefrontEntry mtx = new GomLib.Models.MtxStorefrontEntry();
                    dom.mtxStorefrontEntryLoader.Load(mtx, (long)id, (GomObjectData)gomObjectData);
                    return mtx;
                case "Collections":
                    GomLib.Models.Collection col = new GomLib.Models.Collection();
                    dom.collectionLoader.Load(col, (long)id, (GomObjectData)gomObjectData);
                    return col;
                case "Companions":
                    GomLib.Models.Companion cmp = new GomLib.Models.Companion();
                    dom.companionLoader.Load(cmp, (ulong)id, (GomObjectData)gomObjectData);
                    return cmp;
                case "Ships":
                    GomLib.Models.scFFShip ship = new GomLib.Models.scFFShip();
                    dom.scFFShipLoader.Load(ship, (long)id, (GomObjectData)gomObjectData);
                    return ship;
                case "Conquests":
                    GomLib.Models.Conquest cnq = new GomLib.Models.Conquest();
                    dom.conquestLoader.Load(cnq, (long)id, (GomObjectData)gomObjectData);
                    return cnq;
                /*case "AdvancedClasses":
                    GomLib.Models.PlayerClass dis = new GomLib.Models.PlayerClass();
                    dom.disciplineLoader.LoadClass(dis, (ulong)id, ((List<Object>)gomObjectData).ToList().ConvertAll(x => (GomObjectData)x));
                    return dis;*/
                case "AchCategories":
                    GomLib.Models.AchievementCategory ach = new GomLib.Models.AchievementCategory();
                    dom.achievementCategoryLoader.Load(ach, (long)id, (GomObjectData)gomObjectData);
                    return ach;
                default:
                    throw new IndexOutOfRangeException();
            }
        }

        public virtual HashSet<string> GetDependencies()
        {
            return new HashSet<string>();
        }

        public static string sqlSani(string str)
        {
            if (str == null) return "";
            return MySql.Data.MySqlClient.MySqlHelper.EscapeString(str);
        }
    }

    interface Dependencies
    {
        HashSet<string> GetDependencies();
    }
}
