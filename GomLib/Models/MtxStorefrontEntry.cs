using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;

namespace GomLib.Models
{
    public class MtxStorefrontEntry : PseudoGameObject, IEquatable<MtxStorefrontEntry>
    {
        public Dictionary<string, string> LocalizedName { get; set; }
        public string Icon { get; set; }
        public long UnknowntextId { get; set; }
        public string Unknowntext { get; set; }
        public Dictionary<string, string> Localizedunknowntext { get; set; }
        public long RarityDescId { get; set; }
        public string RarityDesc { get; set; }
        public Dictionary<string, string> LocalizedRarityDesc { get; set; }
        public List<string> BulletPoints { get; set; }
        public List<Dictionary<string, string>> LocalizedBulletPoints { get; set; }
        public long UnknownNumber { get; set; }
        public Dictionary<object, object> Categories { get; set; }
        public long DiscountCost { get; set; }
        public long FullPriceCost { get; set; }
        public long CategoryId { get; set; }
        internal List<Item> LoadedItemList { get; set; }
        public List<Item> ItemList
        {
            get
            {
                if (LoadedItemList != null)
                    return LoadedItemList;
                LoadedItemList = new List<Item>();
                if (ItemIdsList != null)
                    foreach (var item in ItemIdsList)
                    {
                        LoadedItemList.Add(Dom_.itemLoader.Load(item));
                    }
                return LoadedItemList;
            }
        }
        public List<ulong> ItemIdsList { get; set; }
        public bool IsAccountUnlock { get; set; }
        public bool UnknownBool2 { get; set; }
        public long LinkedMTXEntryId { get; set; }

        public override int GetHashCode()
        {
            int hash = Id.GetHashCode();
            hash ^= UnknowntextId.GetHashCode();
            if (Icon != null) hash ^= Icon.GetHashCode();
            hash ^= RarityDescId.GetHashCode();
            hash ^= UnknownNumber.GetHashCode();
            hash ^= DiscountCost.GetHashCode();
            hash ^= FullPriceCost.GetHashCode();
            hash ^= CategoryId.GetHashCode();
            hash ^= IsAccountUnlock.GetHashCode();
            hash ^= UnknownBool2.GetHashCode();
            hash ^= LinkedMTXEntryId.GetHashCode();
            if (LocalizedName != null) foreach (var x in LocalizedName) { hash ^= x.GetHashCode(); } //dictionaries need to hashed like this
            if (Localizedunknowntext != null) foreach (var x in Localizedunknowntext) { hash ^= x.GetHashCode(); }
            if (LocalizedRarityDesc != null) foreach (var x in LocalizedRarityDesc) { hash ^= x.GetHashCode(); }
            if (LocalizedBulletPoints != null) foreach (var x in LocalizedBulletPoints) if (x != null) foreach (var y in x) { hash ^= y.GetHashCode(); }
            if (Categories != null) foreach (var x in Categories) { hash ^= x.GetHashCode(); }
            if (ItemIdsList != null) foreach (var x in ItemIdsList) { hash ^= x.GetHashCode(); }

            return hash;
        }

        public override bool Equals(object obj)
        {
            if (obj == null) return false;

            if (ReferenceEquals(this, obj)) return true;

            if (!(obj is MtxStorefrontEntry mtx)) return false;

            return Equals(mtx);
        }

        public bool Equals(MtxStorefrontEntry mtx)
        {
            if (mtx == null) return false;

            if (ReferenceEquals(this, mtx)) return true;

            if (BulletPoints != null)
            {
                if (mtx.BulletPoints != null)
                {
                    if (!BulletPoints.SequenceEqual(mtx.BulletPoints))
                        return false;
                }
            }
            //if (this.categories != mtx.categories) //this doesn't appear to be used right now.
            //return false;
            if (CategoryId != mtx.CategoryId)
                return false;
            if (DiscountCost != mtx.DiscountCost)
                return false;
            if (FullPriceCost != mtx.FullPriceCost)
                return false;
            if (Icon != mtx.Icon)
                return false;
            if (Id != mtx.Id)
                return false;
            if (IsAccountUnlock != mtx.IsAccountUnlock)
                return false;
            if (ItemIdsList != null)
            {
                if (mtx.ItemIdsList != null)
                {
                    if (!ItemIdsList.SequenceEqual(mtx.ItemIdsList))
                        return false;
                }
            }
            if (LinkedMTXEntryId != mtx.LinkedMTXEntryId)
                return false;

            var ssComp = new DictionaryComparer<string, string>();
            if (LocalizedBulletPoints != null)
            {
                if (mtx.LocalizedBulletPoints != null)
                {
                    if (mtx.LocalizedBulletPoints.Count != mtx.LocalizedBulletPoints.Count)
                        return false;
                    for (int i = 0; i < mtx.LocalizedBulletPoints.Count; i++)
                    {
                        if (!ssComp.Equals(LocalizedBulletPoints[i], mtx.LocalizedBulletPoints[i]))
                            return false;
                    }
                }
            }
            if (!ssComp.Equals(LocalizedName, mtx.LocalizedName))
                return false;
            if (!ssComp.Equals(LocalizedRarityDesc, mtx.LocalizedRarityDesc))
                return false;
            if (!ssComp.Equals(Localizedunknowntext, mtx.Localizedunknowntext))
                return false;

            if (Name != mtx.Name)
                return false;
            if (RarityDesc != mtx.RarityDesc)
                return false;
            if (RarityDescId != mtx.RarityDescId)
                return false;
            if (UnknownBool2 != mtx.UnknownBool2)
                return false;
            if (UnknownNumber != mtx.UnknownNumber)
                return false;
            if (Unknowntext != mtx.Unknowntext)
                return false;
            if (UnknowntextId != mtx.UnknowntextId)
                return false;
            return true;
        }

        public override XElement ToXElement(bool verbose)
        {
            XElement mtxStorefrontEntryXE = new XElement("MtxStoreFront");

            mtxStorefrontEntryXE.Add(new XElement("Name", Name),
                new XElement("RarityDescription", RarityDesc),
                new XAttribute("Id", Id),
                new XElement("Cost", FullPriceCost),
                new XElement("DiscountCost", DiscountCost),
                new XElement("IsAccountUnlock", IsAccountUnlock),
                new XElement("UnknownBool", UnknownBool2));

            if (Unknowntext != "") { mtxStorefrontEntryXE.Add(new XElement("Unknown", Unknowntext)); }
            else { mtxStorefrontEntryXE.Add(new XElement("Unknown")); }

            mtxStorefrontEntryXE.Add(new XElement("Requirements"), new XElement("Icon", Icon));

            int b = 0;
            if (BulletPoints != null)
            {
                foreach (var bullet in BulletPoints)
                {
                    mtxStorefrontEntryXE.Add(new XElement("Info", bullet, new XAttribute("Id", b)));
                    b++;
                }
            }

            return mtxStorefrontEntryXE;
        }
    }
}
