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
        public ulong NodeId { get; set; }
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
            if (this.NodeId != itm.NodeId)
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
            /*
        public override int GetHashCode()
        {
            int hash = LocalizedName.GetHashCode();
            hash ^= LocalizedDescription.GetHashCode();
            hash ^= this.AppearanceColor.GetHashCode();
            hash ^= this.ArmorSpec.GetHashCode();
            hash ^= this.Binding.GetHashCode();
            hash ^= this.Category.GetHashCode();
            hash ^= this.CombinedRating.GetHashCode();
            hash ^= this.CombinedRequiredLevel.GetHashCode();
            hash ^= this.ConsumedOnUse.GetHashCode();
            hash ^= this.DisassembleCategory.GetHashCode();
            hash ^= this.Durability.GetHashCode();
            hash ^= this.EnhancementCategory.GetHashCode();
            hash ^= this.EnhancementSubCategory.GetHashCode();
            hash ^= this.EnhancementType.GetHashCode();
            hash ^= this.EquipAbility.GetHashCode();
            hash ^= this.GiftRank.GetHashCode();
            hash ^= this.GiftType.GetHashCode();
            hash ^= this.Icon.GetHashCode();
            hash ^= this.IsModdable.GetHashCode();
            hash ^= this.ItemLevel.GetHashCode();
            hash ^= this.MaxStack.GetHashCode();
            hash ^= this.ModifierSpec.GetHashCode();
            hash ^= this.MountSpec.GetHashCode();
            hash ^= this.Quality.GetHashCode();
            hash ^= this.Rating.GetHashCode();
            hash ^= this.RequiredAlignmentInverted.GetHashCode();
            hash ^= this.RequiredAlignmentTier.GetHashCode();
            hash ^= this.RequiredGender.GetHashCode();
            hash ^= this.RequiredLevel.GetHashCode();
            hash ^= this.RequiredProfession.GetHashCode();
            hash ^= this.RequiredProfessionLevel.GetHashCode();
            hash ^= this.RequiredSocialTier.GetHashCode();
            hash ^= this.RequiredValorRank.GetHashCode();
            hash ^= this.RequiresAlignment.GetHashCode();
            hash ^= this.RequiresSocial.GetHashCode();
            hash ^= this.SchematicId.GetHashCode();
            hash ^= this.ShieldSpec.GetHashCode();
            hash ^= this.SubCategory.GetHashCode();
            hash ^= this.TypeBitSet.GetHashCode();
            hash ^= this.UniqueLimit.GetHashCode();
            hash ^= this.UseAbility.GetHashCode();
            hash ^= this.Value.GetHashCode();
            hash ^= this.VendorStackSize.GetHashCode();
            hash ^= this.WeaponSpec.GetHashCode();
            foreach (var x in this.CombinedStatModifiers) { hash ^= x.GetHashCode(); }
            foreach (var x in this.EnhancementSlots) { hash ^= x.GetHashCode(); }
            foreach (var x in this.RequiredClasses) { hash ^= x.Id.GetHashCode(); }
            foreach (var x in this.Slots) { hash ^= x.GetHashCode(); }
            foreach (var x in this.StatModifiers) { hash ^= x.GetHashCode(); }
            hash ^= this.StackCount.GetHashCode();
            hash ^= this.MaxDurability.GetHashCode();
            if (this.WeaponAppSpec != null) hash ^= this.WeaponAppSpec.GetHashCode();
            if (this.Model != null) hash ^= this.Model.GetHashCode();
            if (this.ImperialVOModulation != null) hash ^= this.ImperialVOModulation.GetHashCode();
            if (this.RepublicVOModulation != null) hash ^= this.RepublicVOModulation.GetHashCode();
            return hash;
        }*/




        public long CategoryNameId { get; set; }

        public string CategoryName { get; set; }

        public long SubCategoryNameId { get; set; }

        public string SubCategoryName { get; set; }

        public Dictionary<long, string> SourceDict { get; set; }

        public bool UniquePerLegacy { get; set; }

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
                new XElement("Fqn", Fqn, new XAttribute("Id", NodeId)),
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
