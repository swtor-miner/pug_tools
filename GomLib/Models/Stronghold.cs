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
    public class Stronghold : GameObject, IEquatable<Stronghold>
    {
        [JsonIgnore]
        public ulong NodeId { get; set; }
        [JsonConverter(typeof(LongConverter))]
        public long NameId { get; set; }
        public string Name { get; set; }
        public Dictionary<string, string> LocalizedName { get; set; }
        [JsonConverter(typeof(LongConverter))]
        public long DescId { get; set; }
        public string Description { get; set; }
        public Dictionary<string, string> LocalizedDescription { get; set; }
        public string PublicIcon { get; set; }
        public string Icon { get; set; }
        public long DefaultOccupancy { get; set; }
        public long DefaultHooks { get; set; }
        [JsonConverter(typeof(ULongConverter))]
        public ulong PhsId { get; set; }
        public long MaxHooks { get; set; }
        public Dictionary<long, Room> RoomTable { get; set; }
        public long DiscountMtxSFId { get; set; }
        [JsonIgnore]
        internal MtxStorefrontEntry DiscountMtxSF_ { get; set; }
        public MtxStorefrontEntry DiscountMtxSF
        {
            get
            {
                if (DiscountMtxSF_ == null)
                    DiscountMtxSF_ = Dom_.mtxStorefrontEntryLoader.Load(DiscountMtxSFId);
                return DiscountMtxSF_;
            }
        }
        public long MtxStoreFrontId { get; set; }
        [JsonIgnore]
        internal MtxStorefrontEntry MtxStoreFront_ { get; set; }
        public MtxStorefrontEntry MtxStoreFront
        {
            get
            {
                if (MtxStoreFront_ == null)
                    MtxStoreFront_ = Dom_.mtxStorefrontEntryLoader.Load(MtxStoreFrontId);
                return MtxStoreFront_;
            }
        }
        public long DefGuildShOcc { get; set; }
        public long PlayerShCost { get; set; }
        public long GuildShCost { get; set; }
        public string FactionPurchaseRestriction { get; set; }
        public string Type { get; set; }

        public override bool Equals(object obj)
        {
            if (obj == null) return false;

            if (ReferenceEquals(this, obj)) return true;

            if (!(obj is Stronghold itm)) return false;

            return Equals(itm);
        }

        public bool Equals(Stronghold itm)
        {
            if (itm == null) return false;

            if (ReferenceEquals(this, itm)) return true;

            if (DefaultOccupancy != itm.DefaultOccupancy)
                return false;
            if (DefGuildShOcc != itm.DefGuildShOcc)
                return false;
            if (DescId != itm.DescId)
                return false;
            if (Description != itm.Description)
                return false;
            if (!DiscountMtxSF.Equals(itm.DiscountMtxSF))
                return false;
            if (DiscountMtxSFId != itm.DiscountMtxSFId)
                return false;
            if (FactionPurchaseRestriction != itm.FactionPurchaseRestriction)
                return false;
            if (Fqn != itm.Fqn)
                return false;
            if (GuildShCost != itm.GuildShCost)
                return false;
            if (Icon != itm.Icon)
                return false;
            if (Id != itm.Id)
                return false;

            var ssComp = new DictionaryComparer<string, string>();
            if (!ssComp.Equals(LocalizedDescription, itm.LocalizedDescription))
                return false;
            if (!ssComp.Equals(LocalizedName, itm.LocalizedName))
                return false;

            if (MaxHooks != itm.MaxHooks)
                return false;
            if (DefaultHooks != itm.DefaultHooks)
                return false;
            if (!MtxStoreFront.Equals(itm.MtxStoreFront))
                return false;
            if (MtxStoreFrontId != itm.MtxStoreFrontId)
                return false;
            if (Name != itm.Name)
                return false;
            if (NameId != itm.NameId)
                return false;
            if (NodeId != itm.NodeId)
                return false;
            if (PhsId != itm.PhsId)
                return false;
            if (PlayerShCost != itm.PlayerShCost)
                return false;
            if (PublicIcon != itm.PublicIcon)
                return false;

            if (RoomTable != null)
            {
                if (itm.RoomTable == null)
                    return false;
                foreach (var kvp in RoomTable)
                {
                    if (itm.RoomTable.TryGetValue(kvp.Key, out Room prevRoom))
                    {
                        if (!kvp.Value.Equals(prevRoom))
                        {
                            return false;
                        }
                    }
                    else
                    {
                        return false;
                    }
                }
            }

            if (Type != itm.Type)
                return false;
            return true;
        }

        public override int GetHashCode()
        {
            int hash = Name.GetHashCode();
            hash ^= NameId.GetHashCode();
            hash ^= Id.GetHashCode();
            hash ^= Description.GetHashCode();
            hash ^= DescId.GetHashCode();
            hash ^= Fqn.GetHashCode();
            hash ^= NodeId.GetHashCode();
            hash ^= DefaultOccupancy.GetHashCode();
            hash ^= DefGuildShOcc.GetHashCode();
            hash ^= GuildShCost.GetHashCode();
            hash ^= PlayerShCost.GetHashCode();
            hash ^= DiscountMtxSF.GetHashCode();
            hash ^= MtxStoreFront.GetHashCode();
            hash ^= FactionPurchaseRestriction.GetHashCode();
            hash ^= Icon.GetHashCode();
            hash ^= PublicIcon.GetHashCode();
            hash ^= MaxHooks.GetHashCode();
            hash ^= DefaultHooks.GetHashCode();
            hash ^= PhsId.GetHashCode();
            hash ^= Type.GetHashCode();
            if (RoomTable != null)
            {
                foreach (KeyValuePair<long, Room> kvp in RoomTable)
                {
                    hash = hash * 31 + kvp.Key.GetHashCode();
                    hash = hash * 31 + kvp.Value.GetHashCode();
                }
            }

            return hash;
        }

        public override string ToString(bool verbose)
        {
            return string.Join("; ",
                Name,
                string.Join(" - ", RoomTable.Values.Select(x => x.Name).ToList()),
                GuildShCost,
                PlayerShCost);
        }

        public override XElement ToXElement(bool verbose)
        {
            XElement stronghold = new XElement("Stronghold");

            stronghold.Add(new XElement("Name", Name, new XAttribute("Id", NameId)),
                new XAttribute("Id", Id),
                new XElement("Description", Description, new XAttribute("Id", DescId)),
                new XElement("Fqn", Fqn, new XAttribute("Id", NodeId)),
                new XElement("DefaultOccupancy", DefaultOccupancy),
                new XElement("DefaultGuildSHOccupancy", DefGuildShOcc),
                new XElement("GuildSHCost", GuildShCost),
                new XElement("PlayerShCost", PlayerShCost),
                new XElement("CartelMarketEntries",
                    new XElement("DataminingNote", "I haven't confirmed what the second entry is for yet. They could be Subscriber/F2P costs, or a Player Stronghold cost (lower cost one) and a disabled Guild Stronghold cost."),
                    DiscountMtxSF.ToXElement(verbose),
                    MtxStoreFront.ToXElement(verbose)),
                new XElement("FactionPurchaseRestriction", FactionPurchaseRestriction),
                new XElement("Icon", Icon),
                new XElement("PublicIcon", PublicIcon),
                new XElement("MaxHooks", MaxHooks),
                new XElement("DefaultHooks", DefaultHooks),
                new XAttribute("PhaseId", PhsId),
                new XElement("Type", Type)
                );
            XElement rooms = new XElement("Rooms", new XAttribute("Num", RoomTable.Count));
            foreach (var kvp in RoomTable)
            {
                rooms.Add(kvp.Value.ToXElement(verbose));
            }
            stronghold.Add(rooms);

            return stronghold;
        }
    }

    public class Room : IEquatable<Room>
    {
        [JsonIgnore]
        public DataObjectModel _dom;
        public long NameId { get; set; }
        public string Name { get; set; }
        public Dictionary<string, string> LocalizedName { get; set; }
        public long DescId { get; set; }
        public string Description { get; set; }
        public Dictionary<string, string> LocalizedDescription { get; set; }
        public long PlayerShCost { get; set; }
        public long DiscountMtxSFId { get; set; }
        internal MtxStorefrontEntry DiscountMtxSF_ { get; set; }
        public MtxStorefrontEntry DiscountMtxSF
        {
            get
            {
                if (DiscountMtxSF_ == null)
                    DiscountMtxSF_ = _dom.mtxStorefrontEntryLoader.Load(DiscountMtxSFId);
                return DiscountMtxSF_;
            }
        }

        public long PlyrShIncDecs { get; set; }
        public long PlyrShIncOcc { get; set; }
        public long MtxStoreFrontId { get; set; }
        [JsonIgnore]
        internal MtxStorefrontEntry MtxStoreFront_ { get; set; }
        public MtxStorefrontEntry MtxStoreFront
        {
            get
            {
                if (MtxStoreFront_ == null)
                    MtxStoreFront_ = _dom.mtxStorefrontEntryLoader.Load(MtxStoreFrontId);
                return MtxStoreFront_;
            }
        }
        public long GldShIncDecs { get; set; }
        public long GldShIncOcc { get; set; }
        public long GldShCost { get; set; }
        public long Idx { get; set; }
        public ulong ReqItmToUnlockId { get; set; }
        public long ReqQty { get; set; }

        public override bool Equals(object obj)
        {
            if (obj == null) return false;

            if (ReferenceEquals(this, obj)) return true;

            if (!(obj is Room itm)) return false;

            return Equals(itm);
        }

        public bool Equals(Room itm)
        {
            if (itm == null) return false;

            if (ReferenceEquals(this, itm)) return true;

            if (DescId != itm.DescId)
                return false;
            if (Description != itm.Description)
                return false;
            if (!DiscountMtxSF.Equals(itm.DiscountMtxSF))
                return false;
            if (DiscountMtxSFId != itm.DiscountMtxSFId)
                return false;
            if (GldShCost != itm.GldShCost)
                return false;
            if (GldShIncDecs != itm.GldShIncDecs)
                return false;
            if (GldShIncOcc != itm.GldShIncOcc)
                return false;
            if (Idx != itm.Idx)
                return false;

            var ssComp = new DictionaryComparer<string, string>();
            if (!ssComp.Equals(LocalizedDescription, itm.LocalizedDescription))
                return false;
            if (!ssComp.Equals(LocalizedName, itm.LocalizedName))
                return false;

            if (!MtxStoreFront.Equals(itm.MtxStoreFront))
                return false;
            if (MtxStoreFrontId != itm.MtxStoreFrontId)
                return false;
            if (Name != itm.Name)
                return false;
            if (NameId != itm.NameId)
                return false;
            if (PlayerShCost != itm.PlayerShCost)
                return false;
            if (PlyrShIncDecs != itm.PlyrShIncDecs)
                return false;
            if (PlyrShIncOcc != itm.PlyrShIncOcc)
                return false;
            if (ReqItmToUnlockId != itm.ReqItmToUnlockId)
                return false;
            if (ReqQty != itm.ReqQty)
                return false;

            return true;
        }

        public XElement ToXElement(bool verbose)
        {
            XElement room = new XElement("Room");

            room.Add(new XElement("Name", Name, new XAttribute("Id", NameId)),
                new XAttribute("Id", Idx),
                new XElement("Description", Description, new XAttribute("Id", DescId)),
                new XElement("PlayerShCost", PlayerShCost),
                new XElement("IncPlayerSHOccupancy", PlyrShIncOcc),
                new XElement("IncPlayerSHDecorations", PlyrShIncDecs),
                new XElement("GuildSHCost", GldShCost),
                new XElement("IncGuildSHOccupancy", GldShIncOcc),
                new XElement("IncGuildSHDecorations", GldShIncDecs),
                new XElement(DiscountMtxSF.ToXElement(verbose)),
                new XElement(MtxStoreFront.ToXElement(verbose)),
                new XElement("RequiredItemToUnlock", new GameObject().ToXElement(ReqItmToUnlockId, _dom, true),
                    new XAttribute("Qty", ReqQty))
                );

            return room;
        }

        public override int GetHashCode()
        {
            int hash = DescId.GetHashCode();
            hash ^= Description.GetHashCode();
            hash ^= DiscountMtxSFId.GetHashCode();
            hash ^= GldShCost.GetHashCode();
            hash ^= GldShIncDecs.GetHashCode();
            hash ^= GldShIncOcc.GetHashCode();
            hash ^= Idx.GetHashCode();
            hash ^= Name.GetHashCode();
            hash ^= NameId.GetHashCode();
            hash ^= PlayerShCost.GetHashCode();
            hash ^= PlyrShIncDecs.GetHashCode();
            hash ^= PlyrShIncOcc.GetHashCode();
            hash ^= ReqItmToUnlockId.GetHashCode();
            hash ^= ReqQty.GetHashCode();

            if (LocalizedName != null && LocalizedName.Count > 0)
            {
                hash *= 3;
                foreach (KeyValuePair<string, string> kvp in LocalizedName)
                {
                    hash = hash * 31 + kvp.Key.GetHashCode();
                    hash = hash * 31 + kvp.Value.GetHashCode();
                }
            }
            if (LocalizedDescription != null && LocalizedDescription.Count > 0)
            {
                hash *= 5;
                foreach (KeyValuePair<string, string> kvp in LocalizedDescription)
                {
                    hash = hash * 31 + kvp.Key.GetHashCode();
                    hash = hash * 31 + kvp.Value.GetHashCode();
                }
            }

            return hash;
        }
    }

}
