using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;
using System.Xml;
using Newtonsoft.Json;

namespace GomLib.Models
{
    public class MapNote : GameObject, IEquatable<MapNote>
    {
        public string Name { get; set; }
        public Dictionary<string, string> LocalizedName { get; set; }
        public string Icon { get; set; }
        [JsonConverter(typeof(Newtonsoft.Json.Converters.StringEnumConverter))]
        public MapNoteCondition Condition { get; set; }
        public long HuntingRadius { get; set; }
        public long BonusHuntingRadius { get; set; }
        public MapLink MapLink { get; set; }
        //public long WonkaPackageId { get; set; }
        //public long WonkaDestinationId { get; set; }
        public long AssetID { get; internal set; }
        public DetailedFaction Faction { get; internal set; }

        // public string MapLink { get; set; }
        // public bool IsHidden { get; set; }
        // public Placeable Placeable { get; set; }

        public override int GetHashCode()
        {
            int result = Name.GetHashCode();
            result ^= Icon.GetHashCode();
            result ^= Condition.GetHashCode();
            result ^= HuntingRadius.GetHashCode();
            result ^= BonusHuntingRadius.GetHashCode();
            if (MapLink != null)
                result ^= MapLink.GetHashCode();
            //result ^= WonkaPackageId.GetHashCode();
            //result ^= WonkaDestinationId.GetHashCode();
            if (Faction != null)
                result ^= Faction.GetHashCode();
            return result;
        }

        public override bool Equals(object obj)
        {
            if (obj == null) return false;

            if (ReferenceEquals(this, obj)) return true;

            if (!(obj is MapNote itm)) return false;

            return Equals(itm);
        }

        public bool Equals(MapNote itm)
        {
            if (itm == null) return false;

            if (ReferenceEquals(this, itm)) return true;

            if (Icon != itm.Icon)
                return false;
            if (Id != itm.Id)
                return false;

            var dComp = new DictionaryComparer<string, string>();
            if (!dComp.Equals(LocalizedName, itm.LocalizedName))
                return false;

            if (Name != itm.Name)
                return false;
            if (Condition != itm.Condition)
                return false;
            if (HuntingRadius != itm.HuntingRadius)
                return false;
            if (BonusHuntingRadius != itm.BonusHuntingRadius)
                return false;
            if (AssetID != itm.AssetID)
                return false;
            if (MapLink != null)
            {
                if (!MapLink.Equals(itm.MapLink))
                    return false;
            }
            else if (itm.MapLink != null)
                return false;

            return true;
        }
    }

    public class MapLink : IEquatable<MapLink>
    {
        public MapLink(ulong a, long m, long s)
        {
            AreaId = a;
            MapNameSId = m;
            SubmapNameSId = s;
        }

        [JsonIgnore]
        public ulong AreaId { get; internal set; }
        [JsonIgnore]
        public string AreaB62Id_ { get; set; }
        public string AreaB62Id
        {
            get
            {
                if (AreaB62Id_ == null)
                    AreaB62Id_ = AreaId.ToMaskedBase62();
                return AreaB62Id_;
            }
        }
        [JsonIgnore]
        public long MapNameSId { get; internal set; }
        [JsonIgnore]
        public string MapNameB62Id_ { get; set; }
        public string MapNameB62Id
        {
            get
            {
                if (MapNameB62Id_ == null && MapNameSId != 0)
                    MapNameB62Id_ = MapNameSId.ToMaskedBase62();
                return MapNameB62Id_;
            }
        }
        [JsonIgnore]
        public long SubmapNameSId { get; internal set; }
        [JsonIgnore]
        public string SubmapNameB62Id_ { get; set; }
        public string SubmapNameB62Id
        {
            get
            {
                if (SubmapNameB62Id_ == null && SubmapNameSId != 0)
                    SubmapNameB62Id_ = SubmapNameSId.ToMaskedBase62();
                return SubmapNameB62Id_;
            }
        }

        public override int GetHashCode()
        {
            int result = AreaId.GetHashCode();
            result ^= MapNameSId.GetHashCode();
            result ^= SubmapNameSId.GetHashCode();
            return result;
        }

        public bool Equals(MapLink itm)
        {
            if (itm == null) return false;

            if (ReferenceEquals(this, itm)) return true;

            if (AreaId != itm.AreaId)
                return false;
            if (MapNameSId != itm.MapNameSId)
                return false;
            if (SubmapNameSId != itm.SubmapNameSId)
                return false;

            return true;
        }
    }
}
