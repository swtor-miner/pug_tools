using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;

namespace GomLib.Models
{
    public class Collection : PseudoGameObject, IEquatable<Collection>
    {
        //public string Name { get; set; }
        public Dictionary<string, string> LocalizedName { get; set; }
        public string Icon { get; set; }
        //public long Id { get; set; }
        public long CreationIndex { get; set; }
        public long linkedId { get; set; }
        public long CategoryId { get; set; }
        public long RequiredLevel { get; set; }
        public string RarityDesc { get; set; }
        public Dictionary<string, string> LocalizedRarityDesc { get; set; }
        public string unknowntext { get; set; }
        public Dictionary<string, string> Localizedunknowntext { get; set; }
        internal List<Ability> LoadedAbilityList { get; set; }
        public List<Ability> AbilityList
        {
            get
            {
                if (LoadedAbilityList != null)
                    return LoadedAbilityList;
                LoadedAbilityList = new List<Ability>();
                foreach (var ability in AbilityIdsList)
                {
                    LoadedAbilityList.Add(_dom.abilityLoader.Load(ability));
                }
                return LoadedAbilityList;
            }
        }
        public List<ulong> AbilityIdsList { get; set; }
        public List<string> BulletPoints { get; set; }
        public List<Dictionary<string, string>> LocalizedBulletPoints { get; set; }
        public bool IsFoundInPacks { get; set; }
        internal List<Item> LoadedItemList { get; set; }
        public List<Item> ItemList
        {
            get
            {
                if (LoadedItemList != null)
                    return LoadedItemList;
                LoadedItemList = new List<Item>();
                foreach (var item in ItemIdsList)
                {
                    LoadedItemList.Add(_dom.itemLoader.Load(item));
                }
                return LoadedItemList;
            }
        }
        public List<ulong> ItemIdsList { get; set; }
        public long unknowntextId { get; set; }
        public long RarityDescId { get; set; }
        public bool HasAlternateUnlocks { get; set; }
        public Dictionary<ulong, List<ulong>> AlternateUnlocksMap { get; set; }

        public override int GetHashCode()
        {
            int hash = Id.GetHashCode();
            if (Icon != null) hash ^= Icon.GetHashCode();
            hash ^= CreationIndex.GetHashCode();
            hash ^= linkedId.GetHashCode();
            hash ^= CategoryId.GetHashCode();
            hash ^= RequiredLevel.GetHashCode();
            hash ^= IsFoundInPacks.GetHashCode();
            hash ^= unknowntextId.GetHashCode();
            hash ^= RarityDescId.GetHashCode();
            hash ^= HasAlternateUnlocks.GetHashCode();
            if (LocalizedName != null) foreach (var x in LocalizedName) { hash ^= x.GetHashCode(); } //dictionaries need to hashed like this
            if (LocalizedRarityDesc != null) foreach (var x in LocalizedRarityDesc) { hash ^= x.GetHashCode(); }
            if (Localizedunknowntext != null) foreach (var x in Localizedunknowntext) { hash ^= x.GetHashCode(); }
            if (AbilityIdsList != null) foreach (var x in AbilityIdsList) { hash ^= x.GetHashCode(); }
            if (LocalizedBulletPoints != null) foreach (var x in LocalizedBulletPoints) foreach (var y in x) { hash ^= y.GetHashCode(); }
            if (ItemIdsList != null) foreach (var x in ItemIdsList) { hash ^= x.GetHashCode(); }
            if (AlternateUnlocksMap != null) foreach (var x in AlternateUnlocksMap)
            {
                hash ^= x.Key.GetHashCode();
                foreach (var y in x.Value) { hash ^= y.GetHashCode(); }
            }

            return hash;
        }

        public override bool Equals(object obj)
        {
            if (obj == null) return false;

            if (ReferenceEquals(this, obj)) return true;

            Collection col = obj as Collection;
            if (col == null) return false;

            return Equals(col);
        }

        public bool Equals(Collection col)
        {
            if (col == null) return false;

            if (ReferenceEquals(this, col)) return true;

            if (this.AbilityIdsList != null)
            {
                if (col.AbilityIdsList != null)
                {
                    if (!this.AbilityIdsList.SequenceEqual(col.AbilityIdsList))
                        return false;
                }
            }
            if (this.AlternateUnlocksMap != null)
            {
                if (col.AlternateUnlocksMap != null)
                {
                    foreach (var altUnlock in this.AlternateUnlocksMap)
                    {
                        var prevUnlock = new List<ulong>();
                        col.AlternateUnlocksMap.TryGetValue(altUnlock.Key, out prevUnlock);
                        if (prevUnlock == null)
                            return false;
                        if (!altUnlock.Value.SequenceEqual(prevUnlock))
                            return false;
                    }
                }
            }
            if (this.BulletPoints != null)
            {
                if (col.BulletPoints != null)
                {
                    if (!this.BulletPoints.SequenceEqual(col.BulletPoints))
                        return false;
                }
            }
            if (this.CategoryId != col.CategoryId)
                return false;
            if (this.CreationIndex != col.CreationIndex)
                return false;
            if (this.HasAlternateUnlocks != col.HasAlternateUnlocks)
                return false;
            if (this.Icon != col.Icon)
                return false;
            if (this.Id != col.Id)
                return false;
            if (this.IsFoundInPacks != col.IsFoundInPacks)
                return false;
            if (this.ItemIdsList != null)
            {
                if (col.ItemIdsList != null)
                {
                    if (!this.ItemIdsList.SequenceEqual(col.ItemIdsList))
                        return false;
                }
            }
            if (this.linkedId != col.linkedId)
                return false;

            var ssComp = new DictionaryComparer<string, string>();
            if (this.LocalizedBulletPoints != null)
            {
                if (col.LocalizedBulletPoints != null)
                {
                    if (col.LocalizedBulletPoints.Count != col.LocalizedBulletPoints.Count)
                        return false;
                    for(int i = 0; i < col.LocalizedBulletPoints.Count; i++)
                    {
                        if (!ssComp.Equals(this.LocalizedBulletPoints[i], col.LocalizedBulletPoints[i]))
                            return false;
                    }
                }
            }
            if (!ssComp.Equals(this.LocalizedName, col.LocalizedName))
                return false;
            if (!ssComp.Equals(this.LocalizedRarityDesc, col.LocalizedRarityDesc))
                return false;
            if (!ssComp.Equals(this.Localizedunknowntext, col.Localizedunknowntext))
                return false;

            if (this.Name != col.Name)
                return false;
            if (this.RarityDesc != col.RarityDesc)
                return false;
            if (this.RarityDescId != col.RarityDescId)
                return false;
            if (this.RequiredLevel != col.RequiredLevel)
                return false;
            if (this.unknowntext != col.unknowntext)
                return false;
            if (this.unknowntextId != col.unknowntextId)
                return false;
            return true;
        }

        public override XElement ToXElement(bool verbose)
        {
            XElement collectionXE = new XElement("Collection");

            collectionXE.Add(new XElement("Name", Name),
                new XElement("RarityDescription", RarityDesc),
                new XAttribute("Id", Id));

            if (unknowntext != "") { collectionXE.Add(new XElement("Unknown", unknowntext)); }
            else { collectionXE.Add(new XElement("Unknown")); }

            collectionXE.Add(new XElement("Requirements"),
                new XElement("Icon", Icon));
            int b = 0;
            foreach (var bullet in BulletPoints)
            {
                collectionXE.Add(new XElement("Info", bullet, new XAttribute("Id", b)));
                b++;
            }

            XElement grantedItems = new XElement("GrantedItems");
            foreach (var grantedItem in ItemList)
            {
                grantedItems.Add(grantedItem.ToXElement(false));
            }
            collectionXE.Add(grantedItems);

            XElement grantedAbilities = new XElement("GrantedAbilities");
            foreach (var grantedAbility in AbilityList)
            {
                grantedAbilities.Add(grantedAbility.ToXElement(false));
            }
            collectionXE.Add(grantedAbilities);

            XElement alternateUnlocks = new XElement("AlternateUnlocks");
            foreach (var altUnlock in AlternateUnlocksMap)
            {
                XElement alternate = new GameObject().ToXElement(altUnlock.Key, _dom, false);
                foreach (var baseAlternate in altUnlock.Value)
                {
                    alternate.Add(new XElement("AlternateUnlockFor", new GameObject().ToXElement(baseAlternate, _dom, false)));
                }
                alternateUnlocks.Add(alternate);
            }
            collectionXE.Add(alternateUnlocks);

            return collectionXE;
        }
    }
}
