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
        [JsonConverter(typeof(ULongConverter))]
        public ulong InstanceId { get; set; }
        [JsonConverter(typeof(ULongConverter))]
        public ulong AssetId { get; set; }
        internal string AssetFqn_ { get; set; }
        public string AssetFqn
        {
            get
            {
                if (AssetFqn_ == null)
                {
                    Room.Area.Assets.TryGetValue(AssetId, out string assetString);
                    string parsedAssetString = assetString;
                    if (parsedAssetString.StartsWith("\\"))
                        parsedAssetString = parsedAssetString.Substring(1);
                    if (parsedAssetString.Contains(':'))
                    {
                        var splits = parsedAssetString.Split(':');
                        EncounterComponent = splits[0];
                        parsedAssetString = splits[1];
                    }
                    string assType = parsedAssetString.Substring(0, 3);
                    switch (assType)
                    {
                        case "ser":
                            parsedAssetString = parsedAssetString.Substring(7, parsedAssetString.Length - 11);
                            break;
                        case "spn":
                        case "enc":
                            if (parsedAssetString.Contains(".spn_"))
                            {
                                SpawnType = parsedAssetString.Substring(parsedAssetString.IndexOf(".spn_") + 5);
                            }
                            parsedAssetString = parsedAssetString.Replace(".spn_" + SpawnType, "");
                            if (parsedAssetString.EndsWith(".enc"))
                                parsedAssetString = parsedAssetString.Substring(0, parsedAssetString.Length - 4);
                            break;
                        default:
                            break;
                    }
                    parsedAssetString = parsedAssetString.Replace('\\', '.');
                    var obj = Room.Dom_.GetObject(parsedAssetString);
                    if (obj == null)
                    {

                        Failed = true;
                        if (SpawnType != null)
                        {
                            // string poisn = "s";
                        }
                    }
                    else
                    {
                        AssetFqn_ = parsedAssetString;
                        GomId = obj.Id;
                        GomType = parsedAssetString.Substring(0, 3);
                    }
                }
                return AssetFqn_;
            }
        }
        [JsonConverter(typeof(ULongConverter))]
        public ulong GomId { get; set; }
        public string GomB62Id { get { return GomId.ToMaskedBase62(); } }
        public string GomType { get; set; }
        public bool Failed { get; set; }
        public string EncounterComponent { get; set; }
        public string SpawnType { get; set; }
        [JsonIgnore]
        public AreaDat Area { get; set; }

        internal ulong ParentInstance_ { get; set; }
        [JsonConverter(typeof(ULongConverter))]
        public ulong ParentInstance
        {
            get
            {
                if (RawProperties != null)
                {
                    if (RawProperties.ContainsKey(1082547839))
                        ParentInstance_ = (ulong)RawProperties[1082547839];
                }
                return ParentInstance_;
            }
        }

        internal Vec3 Position_ { get; set; }
        public Vec3 Position
        {
            get
            {
                if (RawProperties != null)
                {
                    if (RawProperties.ContainsKey(1333256809))
                        Position_ = (Vec3)RawProperties[1333256809];
                }
                return Position_;
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

            if (!(obj is Area itm)) return false;

            return Equals(itm);
        }

        public bool Equals(AssetInstance itm)
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

            if (!(obj is Vec3 itm)) return false;

            return Equals(itm);
        }

        public bool Equals(Vec3 itm)
        {
            if (itm == null) return false;

            if (ReferenceEquals(this, itm)) return true;

            if (GetHashCode() != itm.GetHashCode())
                return false;

            return true;
        }

    }
}
