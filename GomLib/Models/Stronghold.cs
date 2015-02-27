using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;
using System.Xml.XPath;
using System.Xml;

namespace GomLib.Models
{
    public class Stronghold : GameObject, IEquatable<Stronghold>
    {
        public ulong NodeId { get; set; }
        public long NameId { get; set; }
        public string Name { get; set; }
        public Dictionary<string, string> LocalizedName { get; set; }
        public long DescId { get; set; }
        public string Description { get; set; }
        public Dictionary<string, string> LocalizedDescription { get; set; }
        public string PublicIcon { get; set; }
        public string Icon { get; set; }
        public long DefaultOccupancy { get; set; }
        public long DefaultHooks { get; set; }
        public ulong PhsId { get; set; }
        public long MaxHooks { get; set; }
        public Dictionary<long, Room> RoomTable { get; set; }
        public long DiscountMtxSFId { get; set; }
        [Newtonsoft.Json.JsonIgnore]
        internal MtxStorefrontEntry _DiscountMtxSF { get; set; }
        public MtxStorefrontEntry DiscountMtxSF
        {
            get
            {
                if (_DiscountMtxSF == null)
                    _DiscountMtxSF = _dom.mtxStorefrontEntryLoader.Load(DiscountMtxSFId);
                return _DiscountMtxSF;
            }
        }
        public long MtxStoreFrontId { get; set; }
        [Newtonsoft.Json.JsonIgnore]
        internal MtxStorefrontEntry _MtxStoreFront { get; set; }
        public MtxStorefrontEntry MtxStoreFront
        {
            get
            {
                if (_MtxStoreFront == null)
                    _MtxStoreFront = _dom.mtxStorefrontEntryLoader.Load(MtxStoreFrontId);
                return _MtxStoreFront;
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

            Stronghold itm = obj as Stronghold;
            if (itm == null) return false;

            return Equals(itm);
        }

        public bool Equals(Stronghold itm)
        {
            if (itm == null) return false;

            if (ReferenceEquals(this, itm)) return true;

            if (this.DefaultOccupancy != itm.DefaultOccupancy)
                return false;
            if (this.DefGuildShOcc != itm.DefGuildShOcc)
                return false;
            if (this.DescId != itm.DescId)
                return false;
            if (this.Description != itm.Description)
                return false;
            if (!this.DiscountMtxSF.Equals(itm.DiscountMtxSF))
                return false;
            if (this.DiscountMtxSFId != itm.DiscountMtxSFId)
                return false;
            if (this.FactionPurchaseRestriction != itm.FactionPurchaseRestriction)
                return false;
            if (this.Fqn != itm.Fqn)
                return false;
            if (this.GuildShCost != itm.GuildShCost)
                return false;
            if (this.Icon != itm.Icon)
                return false;
            if (this.Id != itm.Id)
                return false;

            var ssComp = new DictionaryComparer<string, string>();
            if (!ssComp.Equals(this.LocalizedDescription, itm.LocalizedDescription))
                return false; 
            if (!ssComp.Equals(this.LocalizedName, itm.LocalizedName))
                return false; 
            
            if (this.MaxHooks != itm.MaxHooks)
                return false;
            if (this.DefaultHooks != itm.DefaultHooks)
                return false;
            if (!this.MtxStoreFront.Equals(itm.MtxStoreFront))
                return false;
            if (this.MtxStoreFrontId != itm.MtxStoreFrontId)
                return false;
            if (this.Name != itm.Name)
                return false;
            if (this.NameId != itm.NameId)
                return false;
            if (this.NodeId != itm.NodeId)
                return false;
            if (this.PhsId != itm.PhsId)
                return false;
            if (this.PlayerShCost != itm.PlayerShCost)
                return false;
            if (this.PublicIcon != itm.PublicIcon)
                return false;
            
            if (this.RoomTable != null)
            {
                if (itm.RoomTable == null)
                    return false;
                foreach (var kvp in this.RoomTable)
                {
                    var prevRoom = new Room();
                    itm.RoomTable.TryGetValue(kvp.Key, out prevRoom);
                    if (!kvp.Value.Equals(prevRoom))
                        return false;
                }
            }

            if (this.Type != itm.Type)
                return false;
            return true;
        }

        public override string ToString(bool verbose)
        {
            return String.Join("; ",
                Name,
                String.Join(" - ", RoomTable.Values.Select(x => x.Name).ToList()),
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

    public class Room: IEquatable<Room>
    {
        [Newtonsoft.Json.JsonIgnore]
        public DataObjectModel _dom;
        public long NameId { get; set; }
        public string Name { get; set; }
        public Dictionary<string, string> LocalizedName { get; set; }
        public long DescId { get; set; }
        public string Description { get; set; }
        public Dictionary<string, string> LocalizedDescription { get; set; }
        public long PlayerShCost { get; set; }
        public long DiscountMtxSFId { get; set; }
        internal MtxStorefrontEntry _DiscountMtxSF { get; set; }
        public MtxStorefrontEntry DiscountMtxSF
        {
            get
            {
                if (_DiscountMtxSF == null)
                    _DiscountMtxSF = _dom.mtxStorefrontEntryLoader.Load(DiscountMtxSFId);
                return _DiscountMtxSF;
            }
        }

        public long PlyrShIncDecs { get; set; }
        public long PlyrShIncOcc { get; set; }
        public long MtxStoreFrontId { get; set; }
        [Newtonsoft.Json.JsonIgnore]
        internal MtxStorefrontEntry _MtxStoreFront { get; set; }
        public MtxStorefrontEntry MtxStoreFront
        {
            get
            {
                if (_MtxStoreFront == null)
                    _MtxStoreFront = _dom.mtxStorefrontEntryLoader.Load(MtxStoreFrontId);
                return _MtxStoreFront;
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

            Room itm = obj as Room;
            if (itm == null) return false;

            return Equals(itm);
        }

        public bool Equals(Room itm)
        {
            if (itm == null) return false;

            if (ReferenceEquals(this, itm)) return true;

            if (this.DescId != itm.DescId)
                return false;
            if (this.Description != itm.Description)
                return false;
            if (!this.DiscountMtxSF.Equals(itm.DiscountMtxSF))
                return false;
            if (this.DiscountMtxSFId != itm.DiscountMtxSFId)
                return false;
            if (this.GldShCost != itm.GldShCost)
                return false;
            if (this.GldShIncDecs != itm.GldShIncDecs)
                return false;
            if (this.GldShIncOcc != itm.GldShIncOcc)
                return false;
            if (this.Idx != itm.Idx)
                return false;

            var ssComp = new DictionaryComparer<string, string>();
            if (!ssComp.Equals(this.LocalizedDescription, itm.LocalizedDescription))
                return false;
            if (!ssComp.Equals(this.LocalizedName, itm.LocalizedName))
                return false;

            if (!this.MtxStoreFront.Equals(itm.MtxStoreFront))
                return false;
            if (this.MtxStoreFrontId != itm.MtxStoreFrontId)
                return false;
            if (this.Name != itm.Name)
                return false;
            if (this.NameId != itm.NameId)
                return false;
            if (this.PlayerShCost != itm.PlayerShCost)
                return false;
            if (this.PlyrShIncDecs != itm.PlyrShIncDecs)
                return false;
            if (this.PlyrShIncOcc != itm.PlyrShIncOcc)
                return false;
            if (this.ReqItmToUnlockId != itm.ReqItmToUnlockId)
                return false;
            if (this.ReqQty != itm.ReqQty)
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
    }

}
