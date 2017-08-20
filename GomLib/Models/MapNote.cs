using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;
using System.Xml;
using Newtonsoft.Json;

namespace GomLib.Models
{
    public class MapNote : GameObject
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
            result ^= MapLink.GetHashCode();
            //result ^= WonkaPackageId.GetHashCode();
            //result ^= WonkaDestinationId.GetHashCode();
            result ^= Faction.GetHashCode();
            return result;
        }
    }

    public class MapLink
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
        public string _AreaB62Id { get; set; }
        public string AreaB62Id
        {
            get
            {
                if (_AreaB62Id == null)
                    _AreaB62Id = AreaId.ToMaskedBase62();
                return _AreaB62Id;
            }
        }
        [JsonIgnore]
        public long MapNameSId { get; internal set; }
        [JsonIgnore]
        public string _MapNameB62Id { get; set; }
        public string MapNameB62Id
        {
            get
            {
                if (_MapNameB62Id == null && MapNameSId != 0)
                    _MapNameB62Id = MapNameSId.ToMaskedBase62();
                return _MapNameB62Id;
            }
        }
        [JsonIgnore]
        public long SubmapNameSId { get; internal set; }
        [JsonIgnore]
        public string _SubmapNameB62Id { get; set; }
        public string SubmapNameB62Id
        {
            get
            {
                if (_SubmapNameB62Id == null && SubmapNameSId != 0)
                    _SubmapNameB62Id = SubmapNameSId.ToMaskedBase62();
                return _SubmapNameB62Id;
            }
        }

        public override int GetHashCode()
        {
            int result = AreaId.GetHashCode();
            result ^= MapNameSId.GetHashCode();
            result ^= MapNameSId.GetHashCode();
            return result;
        }
    }
}
