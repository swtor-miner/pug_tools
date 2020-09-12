using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;
using System.Xml.XPath;
using System.Xml;
using Newtonsoft.Json;

namespace GomLib.Models
{
    public class Decoration : GameObject, IEquatable<Decoration>
    {
        [JsonConverter(typeof(LongConverter))]
        public long NameId { get; set; }
        public string Name { get; set; }
        public Dictionary<string, string> LocalizedName { get; set; }
        public float decPrevObjRotationX;
        public float decPrevObjRotationY;
        public bool UseItemName { get; set; }
        [JsonConverter(typeof(ULongConverter))]
        public ulong UnlockingItemId { get; set; }
        public string UnlockingItemB62Id { get { return UnlockingItemId.ToMaskedBase62(); } }
        [JsonIgnore]
        public Item UnlockingItem_ { get; set; }
        [JsonIgnore]
        public Item UnlockingItem
        {
            get
            {
                if (UnlockingItem_ == null)
                {
                    UnlockingItem_ = Dom_.itemLoader.Load(UnlockingItemId);
                }
                return UnlockingItem_;
            }
        }
        public string UnlockingFqn
        {
            get
            {
                var obj = Dom_.GetObject(UnlockingItemId);
                if (obj != null)
                    return obj.Name;
                return null;
            }
        }
        [JsonConverter(typeof(ULongConverter))]
        public ulong DecorationId { get; set; }
        public string DecorationB62Id { get { return DecorationId.ToMaskedBase62(); } }
        [JsonIgnore]
        public GameObject DecorationObject { get; set; }
        public string State { get; set; }
        public string DecorationFqn { get; set; }
        public string DefaultAnimation { get; set; }
        public long MaxUnlockLimit { get; set; }
        public long F2PLimit { get; set; }
        public string FactionPlacementRestriction { get; set; }
        [JsonConverter(typeof(LongConverter))]
        public long CategoryId { get; set; }
        [JsonConverter(typeof(LongConverter))]
        public long SubCategoryId { get; set; }
        public Dictionary<long, bool> Hooks { get; set; }
        [JsonIgnore]
        public List<string> AvailableHooks { get; set; }
        public float PrevCamHeightOff { get; set; }
        public float PrevCamDisOff { get; set; }
        public string StubType { get; set; }
        public bool RequiresAbilityUnlocked { get; set; }
        public long GuildPurchaseCost { get; set; }
        [JsonConverter(typeof(LongConverter))]
        public long CategoryNameId { get; set; }
        public string CategoryName { get; set; }
        public Dictionary<string, string> LocalizedCategory { get; set; }
        [JsonConverter(typeof(LongConverter))]
        public long SubCategoryNameId { get; set; }
        public string SubCategoryName { get; set; }
        public Dictionary<string, string> LocalizedSubCategory { get; set; }
        [JsonIgnore]
        public Dictionary<long, string> SourceDict { get; set; }
        public Dictionary<long, Dictionary<string, string>> LocalizedSourceDict { get; set; }
        [JsonIgnore]
        public Dictionary<string, string> SQLSources
        {
            get
            {
                //LocalizedSourceDict.Values.Union( )
                return null;
            }
        }
        public bool UniquePerLegacy { get; set; }

        #region IEquatable
        public override int GetHashCode()
        {
            int hash = Id.GetHashCode();
            if (LocalizedName != null) foreach (var x in LocalizedName) { hash ^= x.GetHashCode(); } //dictionaries need to hashed like this
            if (Hooks != null) foreach (var x in Hooks) { hash ^= x.GetHashCode(); } //dictionaries need to hashed like this
            if (AvailableHooks != null) foreach (var x in AvailableHooks) { hash ^= x.GetHashCode(); } //dictionaries need to hashed like this
            if (SourceDict != null) foreach (var x in SourceDict) { hash ^= x.GetHashCode(); } //dictionaries need to hashed like this
            hash ^= decPrevObjRotationX.GetHashCode();
            hash ^= decPrevObjRotationY.GetHashCode();
            hash ^= UseItemName.GetHashCode();
            hash ^= UnlockingItemId.GetHashCode();
            hash ^= DecorationId.GetHashCode();
            if (State != null) hash ^= State.GetHashCode();
            if (DecorationFqn != null) hash ^= DecorationFqn.GetHashCode();
            if (DefaultAnimation != null) hash ^= DefaultAnimation.GetHashCode();
            hash ^= MaxUnlockLimit.GetHashCode();
            hash ^= F2PLimit.GetHashCode();
            if (FactionPlacementRestriction != null) hash ^= FactionPlacementRestriction.GetHashCode();
            hash ^= CategoryId.GetHashCode();
            hash ^= SubCategoryId.GetHashCode();
            hash ^= PrevCamHeightOff.GetHashCode();
            hash ^= PrevCamDisOff.GetHashCode();
            if (StubType != null) hash ^= StubType.GetHashCode();
            hash ^= RequiresAbilityUnlocked.GetHashCode();
            hash ^= GuildPurchaseCost.GetHashCode();
            hash ^= CategoryNameId.GetHashCode();
            if (CategoryName != null) hash ^= CategoryName.GetHashCode();
            hash ^= SubCategoryNameId.GetHashCode();
            if (SubCategoryName != null) hash ^= SubCategoryName.GetHashCode();
            hash ^= UniquePerLegacy.GetHashCode();

            return hash;
        }

        public override bool Equals(object obj)
        {
            if (obj == null) return false;

            if (ReferenceEquals(this, obj)) return true;

            if (!(obj is Decoration itm)) return false;

            return Equals(itm);
        }


        public bool Equals(Decoration itm)
        {
            if (itm == null) return false;

            if (ReferenceEquals(this, itm)) return true;

            if (!AvailableHooks.SequenceEqual(itm.AvailableHooks))
                return false;
            if (CategoryId != itm.CategoryId)
                return false;
            if (CategoryName != itm.CategoryName)
                return false;
            if (DecorationFqn != itm.DecorationFqn)
                return false;
            if (DecorationId != itm.DecorationId)
                return false;
            if (decPrevObjRotationX != itm.decPrevObjRotationX)
                return false;
            if (decPrevObjRotationY != itm.decPrevObjRotationY)
                return false;
            if (DefaultAnimation != itm.DefaultAnimation)
                return false;
            if (F2PLimit != itm.F2PLimit)
                return false;
            if (FactionPlacementRestriction != itm.FactionPlacementRestriction)
                return false;
            if (Fqn != itm.Fqn)
                return false;

            var lbComp = new DictionaryComparer<long, bool>();
            if (!lbComp.Equals(Hooks, itm.Hooks))
                return false;

            if (Id != itm.Id)
                return false;

            var ssComp = new DictionaryComparer<string, string>();
            if (!ssComp.Equals(LocalizedName, itm.LocalizedName))
                return false;

            if (MaxUnlockLimit != itm.MaxUnlockLimit)
                return false;
            if (Name != itm.Name)
                return false;
            if (NameId != itm.NameId)
                return false;
            if (Id != itm.Id)
                return false;
            if (RequiresAbilityUnlocked != itm.RequiresAbilityUnlocked)
                return false;

            var lsComp = new DictionaryComparer<long, string>();
            if (!lsComp.Equals(SourceDict, itm.SourceDict))
                return false;

            if (State != itm.State)
                return false;
            if (StubType != itm.StubType)
                return false;
            if (SubCategoryId != itm.SubCategoryId)
                return false;
            if (SubCategoryName != itm.SubCategoryName)
                return false;
            if (UseItemName != itm.UseItemName)
                return false;
            if (PrevCamHeightOff != itm.PrevCamHeightOff)
                return false;
            if (PrevCamDisOff != itm.PrevCamDisOff)
                return false;
            if (GuildPurchaseCost != itm.GuildPurchaseCost)
                return false;
            if (UnlockingItemId != itm.UnlockingItemId)
                return false;
            return true;
        }
        #endregion
        public override string ToString(bool verbose)
        {
            var hookNameList = Dom_.decorationLoader.HookList.Select(x => x.Value.Name).ToList();
            hookNameList.Sort();
            string hookString = string.Join(";", hookNameList.Select(x => (AvailableHooks.Contains(x)) ? "x" : "").ToList());

            return string.Join(";",
                Name,
                string.Join(" - ", SourceDict.Values),
                UnlockingItem.Binding.ToString().Replace("None", ""),
                //String.Join(" - ", AvailableHooks)
                CategoryName.Replace("\r\n", "").Replace("\n", "").Replace("\r", ""),
                SubCategoryName,
                GuildPurchaseCost,
                StubType,
                hookString).Replace("decStubType", "");
        }

        public override List<SQLProperty> SQLProperties
        {
            get
            {
                return new List<SQLProperty>
                    {                //(SQL Column Name, C# Property Name, SQL Column type statement, IsUnique/PrimaryKey, Serialize value to json)
                        new SQLProperty("Name", "LocalizedName[enMale]", "varchar(255) COLLATE utf8_unicode_ci NOT NULL", SQLPropSetting.AddIndex),
                        new SQLProperty("FrName", "LocalizedName[frMale]", "varchar(255) COLLATE utf8_unicode_ci NOT NULL", SQLPropSetting.AddIndex),
                        new SQLProperty("DeName", "LocalizedName[deMale]", "varchar(255) COLLATE utf8_unicode_ci NOT NULL", SQLPropSetting.AddIndex),
                        new SQLProperty("Base62Id", "Base62Id", "varchar(7) COLLATE latin1_general_cs NOT NULL", SQLPropSetting.PrimaryKey),
                        new SQLProperty("UnlockingItemB62Id", "UnlockingItemB62Id", "varchar(7) COLLATE latin1_general_cs", SQLPropSetting.AddIndex),
                        new SQLProperty("DecorationB62Id", "DecorationB62Id", "varchar(7) COLLATE latin1_general_cs", SQLPropSetting.AddIndex),
                        new SQLProperty("MaxUnlockLimit", "MaxUnlockLimit", "int(11) NOT NULL", SQLPropSetting.AddIndex),
                        new SQLProperty("F2PLimit", "F2PLimit", "int(11) NOT NULL", SQLPropSetting.AddIndex),
                        new SQLProperty("GuildPurchaseCost", "GuildPurchaseCost", "int(11) NOT NULL", SQLPropSetting.AddIndex),
                        new SQLProperty("FactionPlacementRestriction", "FactionPlacementRestriction", "varchar(255) COLLATE utf8_unicode_ci NOT NULL", SQLPropSetting.AddIndex),
                        new SQLProperty("Category", "LocalizedCategory[enMale]", "varchar(255) COLLATE utf8_unicode_ci NOT NULL", SQLPropSetting.AddIndex),
                        new SQLProperty("FrCategory", "LocalizedCategory[frMale]", "varchar(255) COLLATE utf8_unicode_ci NOT NULL", SQLPropSetting.AddIndex),
                        new SQLProperty("DeCategory", "LocalizedCategory[deMale]", "varchar(255) COLLATE utf8_unicode_ci NOT NULL", SQLPropSetting.AddIndex),
                        new SQLProperty("SubCategory", "LocalizedSubCategory[enMale]", "varchar(255) COLLATE utf8_unicode_ci NOT NULL", SQLPropSetting.AddIndex),
                        new SQLProperty("FrSubCategory", "LocalizedSubCategory[frMale]", "varchar(255) COLLATE utf8_unicode_ci NOT NULL", SQLPropSetting.AddIndex),
                        new SQLProperty("DeSubCategory", "LocalizedSubCategory[deMale]", "varchar(255) COLLATE utf8_unicode_ci NOT NULL", SQLPropSetting.AddIndex),
                    };
            }
        }

        public override XElement ToXElement(bool verbose)
        {
            XElement decoration = new XElement("Decoration");

            decoration.Add(new XElement("Name", Name, new XAttribute("Id", NameId)),
                new XAttribute("Id", Id),
                new XElement("Fqn", Fqn, new XAttribute("Id", Id)),
                new XElement("PreviewWindowValues", string.Join(", ", decPrevObjRotationX, decPrevObjRotationY, PrevCamDisOff, PrevCamHeightOff))
                );
            if (State != null) decoration.Add(new XElement("DynamicState", State));

            decoration.Add(new XElement("DecorationFqn", DecorationFqn, new XAttribute("Id", DecorationId)),
                new XElement("IsAvailable", UseItemName));

            if (DefaultAnimation != null) decoration.Add(new XElement("DefaultAnimation", DefaultAnimation));

            decoration.Add(new XElement("FactionPlacementRestriction", FactionPlacementRestriction),
                new XElement("Category", CategoryName, new XAttribute("Id", CategoryId)),
                new XElement("SubCategory", SubCategoryName, new XAttribute("Id", SubCategoryId)));

            decoration.Add(new XElement("AvailableHooks", string.Join(", ", AvailableHooks)));
            decoration.Add(new XElement("GuildPurchaseCost", GuildPurchaseCost));
            if (StubType != null) decoration.Add(new XElement("RequiredAbilityType", StubType));
            decoration.Add(new XElement("RequiresAbilityUnlocked", RequiresAbilityUnlocked));

            XElement sources = new XElement("Sources", new XAttribute("Num", SourceDict.Count));
            foreach (var kvp in SourceDict)
            {
                sources.Add(new XElement("Source", kvp.Value, new XAttribute("Id", kvp.Key)));
            }
            decoration.Add(sources);
            decoration.Add(new XElement("UnlockingItem", UnlockingItem.ToXElement(false)),
                new XElement("UnlockLimits", string.Join(", ", F2PLimit, MaxUnlockLimit)));
            return decoration;
        }
    }

    public class Hook : IEquatable<Hook>
    {
        public long NameId;
        public string Name;

        public static Dictionary<long, Hook> LoadHooks(DataObjectModel dom)
        {
            GomObject obj = dom.GetObject("apthookdata");

            Dictionary<long, Hook> hooks = new Dictionary<long, Hook>();

            var data = obj.Data.ValueOrDefault("apthookTable", new Dictionary<object, object>());

            var table = dom.stringTable.Find("str.ahd");

            foreach (var kvp in data)
            {
                Hook hook = new Hook
                {
                    NameId = ((GomObjectData)kvp.Value).ValueOrDefault<long>("aptHookNameId", 0)
                };
                hook.Name = table.GetText(hook.NameId, "str.ahd");

                hooks.Add((long)kvp.Key, hook);
            }

            return hooks;
        }

        public override int GetHashCode()
        {
            int hash = NameId.GetHashCode();
            if (Name != null) hash ^= Name.GetHashCode();
            return hash;
        }

        public override bool Equals(object obj)
        {
            if (obj == null) return false;

            if (ReferenceEquals(this, obj)) return true;

            if (!(obj is Hook itm)) return false;

            return Equals(itm);
        }

        public bool Equals(Hook itm)
        {
            if (itm == null) return false;

            if (ReferenceEquals(this, itm)) return true;

            if (Name != itm.Name)
                return false;
            if (NameId != itm.NameId)
                return false;
            return true;
        }
    }

}
