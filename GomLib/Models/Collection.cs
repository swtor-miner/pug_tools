using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;
using Newtonsoft.Json;

namespace GomLib.Models
{
    public class Collection : PseudoGameObject, IEquatable<Collection>
    {
        //public string Name { get; set; }
        public Dictionary<string, string> LocalizedName { get; set; }
        public string Icon { get; set; }
        //public long Id { get; set; }
        public long CreationIndex { get; set; }
        public long LinkedId { get; set; }
        public long CategoryId { get; set; }
        public long RequiredLevel { get; set; }
        [JsonIgnore]
        public string RarityDesc { get; set; }
        public Dictionary<string, string> LocalizedRarityDesc { get; set; }
        public string Unknowntext { get; set; }
        public Dictionary<string, string> Localizedunknowntext { get; set; }
        internal List<Ability> LoadedAbilityList { get; set; }
        [JsonIgnore]
        public List<Ability> AbilityList
        {
            get
            {
                if (LoadedAbilityList != null)
                    return LoadedAbilityList;
                LoadedAbilityList = new List<Ability>();
                foreach (var ability in AbilityIdsList)
                {
                    LoadedAbilityList.Add(Dom_.abilityLoader.Load(ability));
                }
                return LoadedAbilityList;
            }
        }
        public List<ulong> AbilityIdsList { get; set; }
        public List<string> AbilityB62IdsList { get { return AbilityIdsList.ToMaskedBase62(); } }
        [JsonIgnore]
        public List<string> BulletPoints { get; set; }
        public List<Dictionary<string, string>> LocalizedBulletPoints { get; set; }
        public bool IsFoundInPacks { get; set; }
        internal List<Item> LoadedItemList { get; set; }
        [JsonIgnore]
        public List<Item> ItemList
        {
            get
            {
                if (LoadedItemList != null)
                    return LoadedItemList;
                LoadedItemList = new List<Item>();
                foreach (var item in ItemIdsList)
                {
                    LoadedItemList.Add(Dom_.itemLoader.Load(item));
                }
                return LoadedItemList;
            }
        }
        [JsonIgnore]
        public List<ulong> ItemIdsList { get; set; }
        public List<string> ItemB62IdsList { get { return ItemIdsList.ToMaskedBase62(); } }
        public long UnknowntextId { get; set; }
        public long RarityDescId { get; set; }
        public bool HasAlternateUnlocks { get; set; }
        public Dictionary<ulong, List<ulong>> AlternateUnlocksMap { get; set; }

        public override int GetHashCode()
        {
            int hash = Id.GetHashCode();
            if (Icon != null) hash ^= Icon.GetHashCode();
            hash ^= CreationIndex.GetHashCode();
            hash ^= LinkedId.GetHashCode();
            hash ^= CategoryId.GetHashCode();
            hash ^= RequiredLevel.GetHashCode();
            hash ^= IsFoundInPacks.GetHashCode();
            hash ^= UnknowntextId.GetHashCode();
            hash ^= RarityDescId.GetHashCode();
            hash ^= HasAlternateUnlocks.GetHashCode();
            if (LocalizedName != null) foreach (var x in LocalizedName) { hash ^= x.GetHashCode(); } //dictionaries need to hashed like this
            if (LocalizedRarityDesc != null) foreach (var x in LocalizedRarityDesc) { hash ^= x.GetHashCode(); }
            if (Localizedunknowntext != null) foreach (var x in Localizedunknowntext) { hash ^= x.GetHashCode(); }
            if (AbilityIdsList != null) foreach (var x in AbilityIdsList) { hash ^= x.GetHashCode(); }
            if (LocalizedBulletPoints != null) foreach (var x in LocalizedBulletPoints) if (x != null) foreach (var y in x) { hash ^= y.GetHashCode(); }
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

            if (!(obj is Collection col)) return false;

            return Equals(col);
        }

        public bool Equals(Collection col)
        {
            if (col == null) return false;

            if (ReferenceEquals(this, col)) return true;

            if (AbilityIdsList != null)
            {
                if (col.AbilityIdsList != null)
                {
                    if (!AbilityIdsList.SequenceEqual(col.AbilityIdsList))
                        return false;
                }
            }
            if (AlternateUnlocksMap != null)
            {
                if (col.AlternateUnlocksMap != null)
                {
                    foreach (var altUnlock in AlternateUnlocksMap)
                    {
                        _ = new List<ulong>();
                        col.AlternateUnlocksMap.TryGetValue(altUnlock.Key, out List<ulong> prevUnlock);
                        if (prevUnlock == null)
                            return false;
                        if (!altUnlock.Value.SequenceEqual(prevUnlock))
                            return false;
                    }
                }
            }
            if (BulletPoints != null)
            {
                if (col.BulletPoints != null)
                {
                    if (!BulletPoints.SequenceEqual(col.BulletPoints))
                        return false;
                }
            }
            if (CategoryId != col.CategoryId)
                return false;
            if (CreationIndex != col.CreationIndex)
                return false;
            if (HasAlternateUnlocks != col.HasAlternateUnlocks)
                return false;
            if (Icon != col.Icon)
                return false;
            if (Id != col.Id)
                return false;
            if (IsFoundInPacks != col.IsFoundInPacks)
                return false;
            if (ItemIdsList != null)
            {
                if (col.ItemIdsList != null)
                {
                    if (!ItemIdsList.SequenceEqual(col.ItemIdsList))
                        return false;
                }
            }
            if (LinkedId != col.LinkedId)
                return false;

            var ssComp = new DictionaryComparer<string, string>();
            if (LocalizedBulletPoints != null)
            {
                if (col.LocalizedBulletPoints != null)
                {
                    if (col.LocalizedBulletPoints.Count != col.LocalizedBulletPoints.Count)
                        return false;
                    for (int i = 0; i < col.LocalizedBulletPoints.Count; i++)
                    {
                        if (!ssComp.Equals(LocalizedBulletPoints[i], col.LocalizedBulletPoints[i]))
                            return false;
                    }
                }
            }
            if (!ssComp.Equals(LocalizedName, col.LocalizedName))
                return false;
            if (!ssComp.Equals(LocalizedRarityDesc, col.LocalizedRarityDesc))
                return false;
            if (!ssComp.Equals(Localizedunknowntext, col.Localizedunknowntext))
                return false;

            if (Name != col.Name)
                return false;
            if (RarityDesc != col.RarityDesc)
                return false;
            if (RarityDescId != col.RarityDescId)
                return false;
            if (RequiredLevel != col.RequiredLevel)
                return false;
            if (Unknowntext != col.Unknowntext)
                return false;
            if (UnknowntextId != col.UnknowntextId)
                return false;
            return true;
        }

        public override XElement ToXElement(bool verbose)
        {
            XElement collectionXE = new XElement("Collection");

            collectionXE.Add(new XElement("Name", Name),
                new XElement("RarityDescription", RarityDesc),
                new XAttribute("Id", Id));

            if (Unknowntext != "") { collectionXE.Add(new XElement("Unknown", Unknowntext)); }
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
                XElement alternate = new GameObject().ToXElement(altUnlock.Key, Dom_, false);
                foreach (var baseAlternate in altUnlock.Value)
                {
                    alternate.Add(new XElement("AlternateUnlockFor", new GameObject().ToXElement(baseAlternate, Dom_, false)));
                }
                alternateUnlocks.Add(alternate);
            }
            collectionXE.Add(alternateUnlocks);

            return collectionXE;
        }
    }
}
