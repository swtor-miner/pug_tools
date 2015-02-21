using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GomLib.Models
{
    public class Talent : GameObject, IEquatable<Talent>
    {
        public ulong NodeId { get; set; }

        public long NameId { get; set; }
        [Newtonsoft.Json.JsonIgnore]
        public string Name { get; set; }
        public Dictionary<string, string> LocalizedName { get; set; }
        public long DescriptionId { get; set; }
        [Newtonsoft.Json.JsonIgnore]
        public string Description { get; set; }
        public Dictionary<string, string> LocalizedDescription { get; set; }

        public int Ranks { get; set; }
        public List<RankStatData> RankStats { get; set; }
        public int UnknownEnum { get; set; } //0=Needs to be selected by Discipline; 1=Base talent, always available
        public string Icon { get; set; }

        public List<float> TokenList { get; set; }

        [Newtonsoft.Json.JsonIgnore]
        public string DescriptionRank2 { get; set; }
        [Newtonsoft.Json.JsonIgnore]
        public string DescriptionRank3 { get; set; }

        public class RankStatData : IEquatable<RankStatData>
        {
            public List<StatData> OffensiveStats { get; set; }
            public List<StatData> DefensiveStats { get; set; }

            public override bool Equals(object obj)
            {
                if (obj == null) return false;
                if (ReferenceEquals(this, obj)) return true;
                RankStatData std = obj as RankStatData;
                if (std == null) return false;
                return Equals(std);
            }

            public bool Equals(RankStatData std)
            {
                if (std == null) return false;

                if (ReferenceEquals(this, std)) return true;

                if (this.OffensiveStats != std.OffensiveStats)
                    return false;
                if (this.DefensiveStats != std.DefensiveStats)
                    return false;
                return true;
            }
        }

        public class StatData : IEquatable<StatData>
        {
            public int StatId { get; set; }
            [Newtonsoft.Json.JsonIgnore]
            public string Stat { get; set; }
            public float Value { get; set; }
            public bool Enabled { get; set; }
            public ulong AffectedNodeId { get; set; }
            [Newtonsoft.Json.JsonIgnore]
            public Ability AffectedAbility { get; set; }
            
            public string Modifier { get; set; }

            public override bool Equals(object obj)
            {
                if (obj == null) return false;

                if (ReferenceEquals(this, obj)) return true;

                StatData std = obj as StatData;
                if (std == null) return false;

                return Equals(std);
            }

            public bool Equals(StatData std)
            {
                if (std == null) return false;

                if (ReferenceEquals(this, std)) return true;

                if (this.AffectedNodeId != std.AffectedNodeId)
                    return false;
                if (this.Enabled != std.Enabled)
                    return false;
                if (this.Modifier != std.Modifier)
                    return false;
                if (this.Stat != std.Stat)
                    return false;
                if (this.Value != std.Value)
                    return false;
                return true;
            }
        }


        public override int GetHashCode()
        {
            int hash = Name.GetHashCode();
            hash ^= Description.GetHashCode();
            hash ^= this.Icon.GetHashCode();
            return hash;
        }

        public override bool Equals(object obj)
        {
            if (obj == null) return false;

            if (ReferenceEquals(this, obj)) return true;

            Talent tal = obj as Talent;
            if (tal == null) return false;

            return Equals(tal);
        }

        public bool Equals(Talent tal)
        {
            if (tal == null) return false;

            if (ReferenceEquals(this, tal)) return true;

            if (this.Description != tal.Description)
                return false;
            if (this.DescriptionId != tal.DescriptionId)
                return false;
            if (this.DescriptionRank2 != tal.DescriptionRank2)
                return false;
            if (this.DescriptionRank3 != tal.DescriptionRank3)
                return false;
            if (this.Fqn != tal.Fqn)
                return false;
            if (this.Icon != tal.Icon)
                return false;
            if (this.Id != tal.Id)
                return false;
            if (this.Name != tal.Name)
                return false;
            if (this.NameId != tal.NameId)
                return false;
            if (this.NodeId != tal.NodeId)
                return false;
            if (this.Ranks != tal.Ranks)
                return false;

            if (this.RankStats.Count != tal.RankStats.Count)
                return false;
            else
            {
                for(int i = 0; i< this.RankStats.Count; i++)
                {
                    if (!Enumerable.SequenceEqual<StatData>(this.RankStats[i].DefensiveStats, tal.RankStats[i].DefensiveStats))
                        return false;
                    if (!Enumerable.SequenceEqual<StatData>(this.RankStats[i].OffensiveStats, tal.RankStats[i].OffensiveStats))
                        return false;
                }
            }

            if (this.TokenList != null)
            {
                if (tal.TokenList == null)
                {
                    return false;
                }
                else
                {
                    if (!Enumerable.SequenceEqual<float>(this.TokenList, tal.TokenList))
                        return false;
                }
            }
            return true;
        }
    }
}
