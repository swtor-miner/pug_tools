using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;
using Newtonsoft.Json;

namespace GomLib.Models
{
    public class AssetInstance : PseudoGameObject, IEquatable<AssetInstance>
    {
        public AssetInstance(RoomDat room)
        {
            Room = room;
        }
        [JsonIgnore]
        public RoomDat Room { get; set; }

        public string RoomName
        {
            get
            {
                return Room.Name;
            }
        }

        public ulong InstanceId { get; set; }
        public ulong AssetId { get; set; }
        [JsonIgnore]
        public AreaDat Area { get; set; }

        internal ulong _ParentInstance { get; set; }
        public ulong ParentInstance
        {
            get
            {
                if(RawProperties != null)
                {
                    if(RawProperties.ContainsKey(1082547839))
                        _ParentInstance = (ulong)RawProperties[1082547839];
                }
                return _ParentInstance;
            }
        }

        internal Vec3 _Position { get; set; }
        public Vec3 Position
        {
            get
            {
                if (RawProperties != null)
                {
                    if (RawProperties.ContainsKey(1333256809))
                        _Position = (Vec3)RawProperties[1333256809];
                }
                return _Position;
            }
        }

        [JsonIgnore]
        public Dictionary<uint, object> RawProperties { get; set; }
        
        public override int GetHashCode()  //should be fixed.
        {
            int hash = Id.GetHashCode();
            if (Name != null) hash ^= Name.GetHashCode();
            hash ^= AssetId.GetHashCode();
            if (RawProperties != null) foreach (var x in RawProperties) { hash ^= x.GetHashCode(); }

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

        public bool Equals(AssetInstance itm)
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

    public class Vec3 : IEquatable<Vec3>
    {
        public Vec3(float _x, float _y, float _z)
        {
            x = _x;
            y = _y;
            z = _z;
        }
        public float x;
        public float y;
        public float z;

        public override int GetHashCode()  //should be fixed.
        {
            int hash = x.GetHashCode();
            hash ^= y.GetHashCode();
            hash ^= z.GetHashCode();

            return hash;
        }

        public override bool Equals(object obj)
        {
            if (obj == null) return false;

            if (ReferenceEquals(this, obj)) return true;

            Vec3 itm = obj as Vec3;
            if (itm == null) return false;

            return Equals(itm);
        }

        public bool Equals(Vec3 itm)
        {
            if (itm == null) return false;

            if (ReferenceEquals(this, itm)) return true;

            if (this.GetHashCode() != itm.GetHashCode())
                return false;

            return true;
        }

    }
}
