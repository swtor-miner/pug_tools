using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;
using Newtonsoft.Json;

namespace GomLib.Models
{
    public class AreaDat : PseudoGameObject, IEquatable<AreaDat>
    {
        public AreaDat(ulong id)
        {
            AreaId = id;
            Id = (long)id;
        }
        public ulong AreaId { get; set; }
        public ulong AreaGuid { get; set; }

        public Dictionary<ulong, string> Assets { get; set; }
        [JsonIgnore]
        public Dictionary<ulong, List<AssetInstance>> AssetInstances { get; set; }

        [JsonIgnore]
        public List<string> RoomNames { get; set; }
        internal Dictionary<string, RoomDat> _Rooms { get; set; }
        [JsonIgnore]
        public Dictionary<string, RoomDat> Rooms
        {
            get
            {
                if (_Rooms == null)
                {
                    _Rooms = new Dictionary<string, RoomDat>();
                    foreach (var room in RoomNames) {
                        _Rooms.Add(room, _dom.roomDatLoader.Load(room, this));
                    }
                }
                return _Rooms;
            }
        }
        
        public override int GetHashCode()  //should be fixed.
        {
            int hash = Id.GetHashCode();
            if (Name != null) hash ^= Name.GetHashCode();
            hash ^= AreaId.GetHashCode();
            if (Assets != null) foreach (var x in Assets) { hash ^= x.GetHashCode(); }

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

        public bool Equals(AreaDat itm)
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
