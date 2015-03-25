using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;
using System.Xml.XPath;
using System.Xml;

namespace GomLib.Models
{
    public class Decoration : GameObject, IEquatable<Decoration>
    {
        public long NameId { get; set; }
        public string Name { get; set; }
        public Dictionary<string, string> LocalizedName { get; set; }
        public float decPrevObjRotationX;
        public float decPrevObjRotationY;
        public bool a;
        public bool UseItemName { get; set; }
        public ulong UnlockingItemId { get; set; }

        [Newtonsoft.Json.JsonIgnore]
        public Item UnlockingItem { get; set; }
        public ulong DecorationId { get; set; }
        public GameObject DecorationObject { get; set; }
        public string State { get; set; }
        public string DecorationFqn { get; set; }
        public string DefaultAnimation { get; set; }
        public long MaxUnlockLimit { get; set; }
        public long F2PLimit { get; set; }
        public string FactionPlacementRestriction { get; set; }
        public long CategoryId { get; set; }
        public long SubCategoryId { get; set; }
        public Dictionary<long, bool> Hooks { get; set; }
        [Newtonsoft.Json.JsonIgnore]
        public List<string> AvailableHooks { get; set; }
        public float PrevCamHeightOff { get; set; }
        public float PrevCamDisOff { get; set; }
        public string StubType { get; set; }
        public bool RequiresAbilityUnlocked { get; set; }
        public long GuildPurchaseCost { get; set; }
        public long CategoryNameId { get; set; }
        public string CategoryName { get; set; }
        public long SubCategoryNameId { get; set; }
        public string SubCategoryName { get; set; }
        public Dictionary<long, string> SourceDict { get; set; }
        public bool UniquePerLegacy { get; set; }

        public override int GetHashCode()
        {
            int hash = Id.GetHashCode();
            if (LocalizedName != null) foreach (var x in LocalizedName) { hash ^= x.GetHashCode(); } //dictionaries need to hashed like this
            if (Hooks != null) foreach (var x in Hooks) { hash ^= x.GetHashCode(); } //dictionaries need to hashed like this
            if (AvailableHooks != null) foreach (var x in AvailableHooks) { hash ^= x.GetHashCode(); } //dictionaries need to hashed like this
            if (SourceDict != null) foreach (var x in SourceDict) { hash ^= x.GetHashCode(); } //dictionaries need to hashed like this
            hash ^= decPrevObjRotationX.GetHashCode();
            hash ^= decPrevObjRotationY.GetHashCode();
            hash ^= a.GetHashCode();
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

            Decoration itm = obj as Decoration;
            if (itm == null) return false;

            return Equals(itm);
        }


        public bool Equals(Decoration itm)
        {
            if (itm == null) return false;

            if (ReferenceEquals(this, itm)) return true;

            if (!this.AvailableHooks.SequenceEqual(itm.AvailableHooks))
                return false;
            if (this.CategoryId != itm.CategoryId)
                return false;
            if (this.CategoryName != itm.CategoryName)
                return false;
            if (this.DecorationFqn != itm.DecorationFqn)
                return false;
            if (this.DecorationId != itm.DecorationId)
                return false;
            if (this.decPrevObjRotationX != itm.decPrevObjRotationX)
                return false;
            if (this.decPrevObjRotationY != itm.decPrevObjRotationY)
                return false;
            if (this.DefaultAnimation != itm.DefaultAnimation)
                return false;
            if (this.F2PLimit != itm.F2PLimit)
                return false;
            if (this.FactionPlacementRestriction != itm.FactionPlacementRestriction)
                return false;
            if (this.Fqn != itm.Fqn)
                return false;

            var lbComp = new DictionaryComparer<long, bool>();
            if (!lbComp.Equals(this.Hooks, itm.Hooks))
                return false;

            if (this.Id != itm.Id)
                return false;

            var ssComp = new DictionaryComparer<string, string>();
            if (!ssComp.Equals(this.LocalizedName, itm.LocalizedName))
                return false; 
            
            if (this.MaxUnlockLimit != itm.MaxUnlockLimit)
                return false;
            if (this.Name != itm.Name)
                return false;
            if (this.NameId != itm.NameId)
                return false;
            if (this.Id != itm.Id)
                return false;
            if (this.RequiresAbilityUnlocked != itm.RequiresAbilityUnlocked)
                return false;

            var lsComp = new DictionaryComparer<long, string>();
            if (!lsComp.Equals(this.SourceDict, itm.SourceDict))
                return false;

            if (this.State != itm.State)
                return false;
            if (this.StubType != itm.StubType)
                return false;
            if (this.SubCategoryId != itm.SubCategoryId)
                return false;
            if (this.SubCategoryName != itm.SubCategoryName)
                return false;
            if (this.UseItemName != itm.UseItemName)
                return false;
            if (this.PrevCamHeightOff != itm.PrevCamHeightOff)
                return false;
            if (this.PrevCamDisOff != itm.PrevCamDisOff)
                return false;
            if (this.GuildPurchaseCost != itm.GuildPurchaseCost)
                return false;
            if (this.UnlockingItemId != itm.UnlockingItemId)
                return false; 
            return true;
        }

        public override string ToString(bool verbose)
        {
            var hookNameList = _dom.decorationLoader.HookList.Select(x => x.Value.Name).ToList();
            hookNameList.Sort();
            string hookString = String.Join(";", hookNameList.Select(x => (AvailableHooks.Contains(x)) ? "x" : "").ToList());

            return String.Join(";",
                Name,
                String.Join(" - ", SourceDict.Values),
                UnlockingItem.Binding.ToString().Replace("None", ""),
                //String.Join(" - ", AvailableHooks)
                CategoryName.Replace("\r\n", "").Replace("\n", "").Replace("\r", ""),
                SubCategoryName,
                GuildPurchaseCost,
                StubType,
                hookString).Replace("decStubType", "");
        }

        public override XElement ToXElement(bool verbose)
        {
            XElement decoration = new XElement("Decoration");

            decoration.Add(new XElement("Name", Name, new XAttribute("Id", NameId)),
                new XAttribute("Id", Id),
                new XElement("Fqn", Fqn, new XAttribute("Id", Id)),
                new XElement("PreviewWindowValues", String.Join(", ", decPrevObjRotationX, decPrevObjRotationY, PrevCamDisOff, PrevCamHeightOff))
                );
            if (State != null) decoration.Add(new XElement("DynamicState", State));

            decoration.Add(new XElement("DecorationFqn", DecorationFqn, new XAttribute("Id", DecorationId)),
                new XElement("IsAvailable", UseItemName));

            if (DefaultAnimation != null) decoration.Add(new XElement("DefaultAnimation", DefaultAnimation));

            decoration.Add(new XElement("FactionPlacementRestriction", FactionPlacementRestriction),
                new XElement("Category", CategoryName, new XAttribute("Id", CategoryId)),
                new XElement("SubCategory", SubCategoryName, new XAttribute("Id", SubCategoryId)));

            decoration.Add(new XElement("AvailableHooks", String.Join(", ", AvailableHooks)));
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
                new XElement("UnlockLimits", String.Join(", ", F2PLimit, MaxUnlockLimit)));
            return decoration;
        }
    }

    public class Hook: IEquatable<Hook>
    {
        public long NameId;
        public string Name;

        public static Dictionary<long, Hook> LoadHooks(DataObjectModel dom)
        {
            GomObject obj = dom.GetObject("apthookdata");

            Dictionary<long, Hook> hooks = new Dictionary<long,Hook>();

            var data = obj.Data.ValueOrDefault<Dictionary<object, object>>("apthookTable", new Dictionary<object, object>());

            var table = dom.stringTable.Find("str.ahd");

            foreach(var kvp in data)
            {
                Hook hook = new Hook();
                hook.NameId = ((GomObjectData)kvp.Value).ValueOrDefault<long>("aptHookNameId", 0);
                hook.Name = table.GetText(hook.NameId, "str.ahd");

                hooks.Add((long)kvp.Key, hook);
            }

            return hooks;
        }

        public override int GetHashCode()
        {
            int hash = NameId.GetHashCode();
            if(Name != null) hash ^= Name.GetHashCode();
            return hash;
        }

        public override bool Equals(object obj)
        {
            if (obj == null) return false;

            if (ReferenceEquals(this, obj)) return true;

            Hook itm = obj as Hook;
            if (itm == null) return false;

            return Equals(itm);
        }

        public bool Equals(Hook itm)
        {
            if (itm == null) return false;

            if (ReferenceEquals(this, itm)) return true;

            if (this.Name != itm.Name)
                return false;
            if (this.NameId != itm.NameId)
                return false;
            return true;
        }
    }

}
