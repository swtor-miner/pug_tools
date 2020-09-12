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
        [JsonConverter(typeof(ULongConverter))]
        public ulong AreaId { get; set; }
        [JsonConverter(typeof(ULongConverter))]
        public ulong AreaGuid { get; set; }

        [JsonIgnore]
        public Dictionary<ulong, string> Assets { get; set; }
        public Dictionary<string, string> AssetDefs
        {
            get
            {
                return ((Assets == null) ?
                    new Dictionary<string, string>() :
                    Assets.ToDictionary(x => x.Key.ToString(), x => x.Value));
            }
        }
        [JsonIgnore]
        public Dictionary<ulong, List<AssetInstance>> AssetInstances { get; set; }

        [JsonIgnore]
        public List<string> RoomNames { get; set; }
        [JsonIgnore]
        internal Dictionary<string, RoomDat> Rooms_ { get; set; }
        [JsonIgnore]
        public Dictionary<string, RoomDat> Rooms
        {
            get
            {
                if (Rooms_ == null)
                {
                    LoadRooms();
                }
                return Rooms_;
            }
        }
        public Dictionary<string, string> B62RoomIds
        {
            get
            {
                if (Rooms_ == null)
                {
                    LoadRooms();
                }
                return Rooms_.ToDictionary(x => x.Key, x => x.Value.Base62Id);
            }
        }

        public void LoadRooms()
        {
            if (Rooms_ == null)
            {
                Rooms_ = new Dictionary<string, RoomDat>();
                int i = 0;
                foreach (var room in RoomNames)
                {
                    i++;
                    Rooms_.Add(room, Dom_.roomDatLoader.Load(room, i, this));
                }
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

            if (!(obj is Area itm)) return false;

            return Equals(itm);
        }

        public bool Equals(AreaDat itm)
        {
            if (itm == null) return false;

            if (ReferenceEquals(this, itm)) return true;

            if (GetHashCode() != itm.GetHashCode())
                return false;
            if (Id != itm.Id)
                return false;

            return true;
        }
    }
}
