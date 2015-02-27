using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;

namespace GomLib.Models
{
    public class MtxStorefrontEntry : PseudoGameObject, IEquatable<MtxStorefrontEntry>
    {
        [Newtonsoft.Json.JsonIgnore]
        public DataObjectModel _dom;
        //public string Name { get; set; }
        public Dictionary<string, string> LocalizedName { get; set; }
        public string Icon { get; set; }
        //public long Id { get; set; }
        public long unknowntextId { get; set; }
        public string unknowntext { get; set; }
        public Dictionary<string, string> Localizedunknowntext { get; set; }
        public long RarityDescId { get; set; }
        public string RarityDesc { get; set; }
        public Dictionary<string, string> LocalizedRarityDesc { get; set; }
        public List<string> BulletPoints { get; set; }
        public List<Dictionary<string, string>> LocalizedBulletPoints { get; set; }
        public long unknownNumber { get; set; }
        public Dictionary<object, object> categories { get; set; }
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
                foreach (var item in ItemIdsList)
                {
                    LoadedItemList.Add(_dom.itemLoader.Load(item));
                }
                return LoadedItemList;
            }
        }
        public List<ulong> ItemIdsList { get; set; }
        public bool IsAccountUnlock { get; set; }
        public bool unknownBool2 { get; set; }
        public long LinkedMTXEntryId { get; set; }

        public override bool Equals(object obj)
        {
            if (obj == null) return false;

            if (ReferenceEquals(this, obj)) return true;

            MtxStorefrontEntry mtx = obj as MtxStorefrontEntry;
            if (mtx == null) return false;

            return Equals(mtx);
        }

        public bool Equals(MtxStorefrontEntry mtx)
        {
            if (mtx == null) return false;

            if (ReferenceEquals(this, mtx)) return true;

            if (this.BulletPoints != null)
            {
                if (mtx.BulletPoints != null)
                {
                    if (!this.BulletPoints.SequenceEqual(mtx.BulletPoints))
                        return false;
                }
            }
            //if (this.categories != mtx.categories) //this doesn't appear to be used right now.
                //return false;
            if (this.CategoryId != mtx.CategoryId)
                return false;
            if (this.DiscountCost != mtx.DiscountCost)
                return false;
            if (this.FullPriceCost != mtx.FullPriceCost)
                return false;
            if (this.Icon != mtx.Icon)
                return false;
            if (this.Id != mtx.Id)
                return false;
            if (this.IsAccountUnlock != mtx.IsAccountUnlock)
                return false;
            if (this.ItemIdsList != null)
            {
                if (mtx.ItemIdsList != null)
                {
                    if (!this.ItemIdsList.SequenceEqual(mtx.ItemIdsList))
                        return false;
                }
            }
            if (this.LinkedMTXEntryId != mtx.LinkedMTXEntryId)
                return false;

            var ssComp = new DictionaryComparer<string, string>();
            if (this.LocalizedBulletPoints != null)
            {
                if (mtx.LocalizedBulletPoints != null)
                {
                    if (mtx.LocalizedBulletPoints.Count != mtx.LocalizedBulletPoints.Count)
                        return false;
                    for (int i = 0; i < mtx.LocalizedBulletPoints.Count; i++)
                    {
                        if (!ssComp.Equals(this.LocalizedBulletPoints[i], mtx.LocalizedBulletPoints[i]))
                            return false;
                    }
                }
            }
            if (!ssComp.Equals(this.LocalizedName, mtx.LocalizedName))
                return false;
            if (!ssComp.Equals(this.LocalizedRarityDesc, mtx.LocalizedRarityDesc))
                return false;
            if (!ssComp.Equals(this.Localizedunknowntext, mtx.Localizedunknowntext))
                return false;

            if (this.Name != mtx.Name)
                return false;
            if (this.RarityDesc != mtx.RarityDesc)
                return false;
            if (this.RarityDescId != mtx.RarityDescId)
                return false;
            if (this.unknownBool2 != mtx.unknownBool2)
                return false;
            if (this.unknownNumber != mtx.unknownNumber)
                return false;
            if (this.unknowntext != mtx.unknowntext)
                return false;
            if (this.unknowntextId != mtx.unknowntextId)
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
                new XElement("UnknownBool", unknownBool2));

            if (unknowntext != "") { mtxStorefrontEntryXE.Add(new XElement("Unknown", unknowntext)); }
            else { mtxStorefrontEntryXE.Add(new XElement("Unknown")); }

            mtxStorefrontEntryXE.Add(new XElement("Requirements"),
                new XElement("Icon", Icon));
            int b = 0;
            foreach (var bullet in BulletPoints)
            {
                mtxStorefrontEntryXE.Add(new XElement("Info", bullet, new XAttribute("Id", b)));
                b++;
            }

            return mtxStorefrontEntryXE;
        }
    }
}
