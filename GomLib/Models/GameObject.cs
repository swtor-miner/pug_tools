using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Newtonsoft.Json;
using System.Xml.Linq;

namespace GomLib.Models
{
    public class GameObject : IDependencies
    {
        //[JsonConverter(typeof(Newtonsoft.Json.Converters.))]
        [JsonConverter(typeof(ULongConverter))]
        public ulong Id { get; set; }
        public string Base62Id
        {
            get
            {
                return Id.ToMaskedBase62();
            }
        }
        public string Fqn { get; set; }
        [Newtonsoft.Json.JsonIgnore]
        public DataObjectModel Dom_ { get; set; }
        [Newtonsoft.Json.JsonIgnore]
        public Dictionary<string, SortedSet<ulong>> References { get; set; }
        [Newtonsoft.Json.JsonIgnore]
        public Dictionary<string, List<string>> B62References_ { get; set; }
        public Dictionary<string, List<string>> B62References
        {
            get
            {
                if (B62References_ == null)
                {
                    if (References != null)
                    {
                        B62References_ = References.ToDictionary(x => x.Key, x => x.Value.Select(y => y.ToMaskedBase62()).ToList());
                    }
                }
                return B62References_;
            }
        }
        public Dictionary<ulong, string> FullReferences { get; set; }

        public virtual string ToJSON()
        {
            JsonSerializerSettings settings = new JsonSerializerSettings
            {
                NullValueHandling = NullValueHandling.Ignore
            };
            return ToJSON(settings);
        }
        public string ToJSON(JsonSerializerSettings settings)
        {
            string json = JsonConvert.SerializeObject(this, settings);
            return json;
        }

        public override string ToString() { return ToString(true); }
        public virtual string ToString(bool verbose) { return ""; }

        public string ToSQL(string patchVersion) //rewrote the code to allow creation of new outputs faster.
        {
            if (SQLProperties == null)
                return "Unsupported";
            else
                return SQLHelpers.ToSQL(this, SQLInfo(), patchVersion);
        }

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
        public static GameObject Load(GomObject gomItm)
        {
            return Load(gomItm, false);
        }
        public static GameObject Load(GomObject gomItm, bool classOverride)
        {
            switch (gomItm.Name.Substring(0, 4))
            {
                case "ach.": return gomItm.Dom_.achievementLoader.Load(gomItm);
                case "abl.":
                    if (!gomItm.Name.Contains("/"))
                        return gomItm.Dom_.abilityLoader.Load(gomItm);
                    return null;
                case "apn.": case "apc.": case "pkg.": return gomItm.Dom_.abilityPackageLoader.Load(gomItm);
                case "cdx.": return gomItm.Dom_.codexLoader.Load(gomItm);
                case "cnv.": return gomItm.Dom_.conversationLoader.Load(gomItm);
                case "npc.": return gomItm.Dom_.npcLoader.Load(gomItm);
                case "qst.": return gomItm.Dom_.questLoader.Load(gomItm);
                case "tal.": return gomItm.Dom_.talentLoader.Load(gomItm);
                case "sche": return gomItm.Dom_.schematicLoader.Load(gomItm);
                case "dec.": return gomItm.Dom_.decorationLoader.Load(gomItm);
                case "itm.": return gomItm.Dom_.itemLoader.Load(gomItm);
                case "apt.": return gomItm.Dom_.strongholdLoader.Load(gomItm);
                case "clas":
                    if (classOverride && gomItm.Name.StartsWith("class.pc.advanced."))
                        return gomItm.Dom_.advancedClassLoader.Load(gomItm);
                    else
                        return gomItm.Dom_.classSpecLoader.Load(gomItm);
                case "ipp.": return gomItm.Dom_.appearanceLoader.Load(gomItm);
                case "npp.": return gomItm.Dom_.appearanceLoader.Load(gomItm);
                case "nco.": return gomItm.Dom_.newCompanionLoader.Load(gomItm);
                case "spn.": return gomItm.Dom_.spawnerLoader.Load(gomItm);
                case "plc.": return gomItm.Dom_.placeableLoader.Load(gomItm);
                case "mpn.": return gomItm.Dom_.mapNoteLoader.Load(gomItm);
                default:
                    return null;
            }
        }

        public XElement ReferencesToXElement()
        {
            XElement references = new XElement("References");
            if (References != null)
            {
                foreach (KeyValuePair<string, SortedSet<ulong>> entry in References)
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

        public SQLData SQLInfo()
        {
            return new SQLData(SQLProperties);
        }
        [Newtonsoft.Json.JsonIgnore]
        public virtual List<SQLProperty> SQLProperties { get; set; }
    }

    public class PseudoGameObject : IDependencies
    {
        [JsonConverter(typeof(LongConverter))]
        public virtual long Id { get; set; }
        public string Base62Id
        {
            get
            {
                return ((ulong)Id).ToMaskedBase62();
            }
        }
        public virtual string Name { get; set; }
        [Newtonsoft.Json.JsonIgnore]
        public string Prototype { get; set; } //Which prototype this object is from.
        [Newtonsoft.Json.JsonIgnore]
        public string ProtoDataTable { get; set; } //which prototype field contains the object
        [Newtonsoft.Json.JsonIgnore]
        public DataObjectModel Dom_ { get; set; }
        [Newtonsoft.Json.JsonIgnore]
        public Dictionary<string, SortedSet<ulong>> References { get; set; }
        [Newtonsoft.Json.JsonIgnore]
        public Dictionary<string, List<string>> B62References_ { get; set; }
        public Dictionary<string, List<string>> B62References
        {
            get
            {
                if (B62References_ == null)
                {
                    if (References != null)
                    {
                        B62References_ = References.ToDictionary(x => x.Key, x => x.Value.Select(y => y.ToMaskedBase62()).ToList());
                    }
                }
                return B62References_;
            }
        }

        [Newtonsoft.Json.JsonIgnore]
        public HashSet<string> RequiredFiles;

        public string ToJSON()
        {
            JsonSerializerSettings settings = new JsonSerializerSettings
            {
                NullValueHandling = NullValueHandling.Ignore
            };
            return ToJSON(settings);
        }
        public string ToJSON(JsonSerializerSettings settings)
        {
            string json = JsonConvert.SerializeObject(this, settings);
            return json;
        }

        public override string ToString() { return ToString(true); }
        public virtual string ToString(bool verbose) { return ""; }

        public string ToSQL(string patchVersion) //rewrote the code to allow creation of new outputs faster.
        {
            if (SQLProperties == null)
                return "Unsupported";
            else
                return SQLHelpers.ToSQL(this, SQLInfo(), patchVersion);
        }

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
                    GomLib.Models.ScFFShip ship = new GomLib.Models.ScFFShip();
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
                case "Areas":
                    GomLib.Models.Area ara = new GomLib.Models.Area();
                    dom.areaLoader.Load(ara, (GomObjectData)gomObjectData);
                    return ara;
                case "SetBonuses":
                    GomLib.Models.SetBonusEntry setEntry = new GomLib.Models.SetBonusEntry();
                    dom.setBonusLoader.Load(setEntry, (long)id, (GomObjectData)gomObjectData);
                    return setEntry;
                case "CodexCategoryTotals":
                    GomLib.Models.CodexCatByFaction cdxCatByFaction = new GomLib.Models.CodexCatByFaction();
                    dom.cdxCatTotalsLoader.Load(cdxCatByFaction, (long)id, (Dictionary<object, object>)gomObjectData);
                    return cdxCatByFaction;
                case "SchematicVariations":
                    GomLib.Models.SchematicVariation schemVariation = new GomLib.Models.SchematicVariation();
                    dom.schemVariationLoader.Load(schemVariation, (ulong)id, (Dictionary<object, object>)gomObjectData);
                    return schemVariation;
                case "PlayerTitles":
                    GomLib.Models.PlayerTitle playerTitle = new GomLib.Models.PlayerTitle();
                    dom.playerTitleLoader.Load(playerTitle, (long)id, (GomObjectData)gomObjectData);
                    return playerTitle;
                default:
                    throw new IndexOutOfRangeException();
            }
        }

        public static PseudoGameObject LoadFromProtoName(string protoName, DataObjectModel dom, object id, object gomObjectData)
        {
            switch (protoName)
            {
                case "mtxStorefrontInfoPrototype":
                    return Load("MtxStoreFronts", dom, id, gomObjectData);
                case "colCollectionItemsPrototype":
                    return Load("Collections", dom, id, gomObjectData);
                case "chrCompanionInfo_Prototype":
                    return Load("Companions", dom, id, gomObjectData);
                case "scFFShipsDataPrototype":
                    return Load("Ships", dom, id, gomObjectData);
                case "wevConquestInfosPrototype":
                    return Load("Conquests", dom, id, gomObjectData);
                case "achCategoriesTable_Prototype":
                    return Load("AchCategories", dom, id, gomObjectData);
                //case "ablPackagePrototype":
                //    return 
                default:
                    throw new IndexOutOfRangeException();
            }
        }

        public virtual HashSet<string> GetDependencies()
        {
            return new HashSet<string>();
        }

        public SQLData SQLInfo()
        {
            return new SQLData(SQLProperties);
        }
        [Newtonsoft.Json.JsonIgnore]
        public virtual List<SQLProperty> SQLProperties { get; set; }
    }

    interface IDependencies
    {
        HashSet<string> GetDependencies();
    }
}

//public class IdToStringConverter : JsonConverter
//{
//    public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
//    {
//        JToken jt = JValue.ReadFrom(reader);

//        return jt.Value<long>();
//    }

//    public override bool CanConvert(Type objectType)
//    {
//        return typeof(System.Int64).Equals(objectType);
//    }

//    public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
//    {
//        serializer.Serialize(writer, value.ToString());
//    }
//}