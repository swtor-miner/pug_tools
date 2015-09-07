using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;
using Newtonsoft.Json;

namespace GomLib.Models
{
    public class RoomDat : PseudoGameObject, IEquatable<RoomDat>
    {
        public RoomDat(string room, AreaDat area)
        {
            Area = area;
            Room = room;
        }
        public ulong AreaId { get; set; }
        [JsonIgnore]
        public AreaDat Area { get; set; }

        public string Room { get; set; }
        
        public Dictionary<ulong, AssetInstance> Instances { get; set; }
        
        public override int GetHashCode()  //should be fixed.
        {
            int hash = Id.GetHashCode();
            if (Name != null) hash ^= Name.GetHashCode();
            hash ^= AreaId.GetHashCode();
            if (Instances != null) foreach (var x in Instances) { hash ^= x.GetHashCode(); }

            return hash;
        }

        public override bool Equals(object obj)
        {
            if (obj == null) return false;

            if (ReferenceEquals(this, obj)) return true;

            Area itm = obj as Area;
            if (itm == null) return false;

            return Equals(itm);
        }

        public bool Equals(RoomDat itm)
        {
            if (itm == null) return false;

            if (ReferenceEquals(this, itm)) return true;

            if (this.GetHashCode() != itm.GetHashCode())
                return false;
            if (this.Id != itm.Id)
                return false;

            return true;
        }
    }
}
