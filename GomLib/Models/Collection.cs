using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GomLib.Models
{
    public class Collection : PseudoGameObject, IEquatable<Collection>
    {
        [Newtonsoft.Json.JsonIgnore]
        public DataObjectModel _dom;
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
    }
}
