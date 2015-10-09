using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace GomLib.Models
{
    public class Talent : GameObject, IEquatable<Talent>
    {
        [JsonIgnore]
        public ulong NodeId { get; set; }
        [JsonConverter(typeof(LongConverter))]
        public long NameId { get; set; }
        [Newtonsoft.Json.JsonIgnore]
        public string Name { get; set; }
        public Dictionary<string, string> LocalizedName { get; set; }
        [JsonConverter(typeof(LongConverter))]
        public long DescriptionId { get; set; }
        [Newtonsoft.Json.JsonIgnore]
        public string Description { get; set; }
        public Dictionary<string, string> LocalizedDescription { get; set; }

        public int Ranks { get; set; }
        public List<RankStatData> RankStats { get; set; }
        public ScriptEnum TalentVisibility { get; set; } //0=Needs to be selected by Discipline; 1=Base talent, always available
        public string Icon { get; set; }
        public string HashedIcon
        {
            get
            {
                string icon = "none";
                if (Icon != null)
                {
                    icon = Icon;
                }
                var fileId = TorLib.FileId.FromFilePath(String.Format("/resources/gfx/icons/{0}.dds", icon));
                return String.Format("{0}_{1}", fileId.ph, fileId.sh);
            }
        }

        public List<float> TokenList { get; set; }

        [Newtonsoft.Json.JsonIgnore]
        public string DescriptionRank2 { get; set; }
        [Newtonsoft.Json.JsonIgnore]
        public string DescriptionRank3 { get; set; }

        public static string ParseDescription(Talent tal, string desc)
        {
            if (tal.TokenList == null)
                return desc;

            for (var i = 0; i < tal.TokenList.Count; i++)
            {
                var value = tal.TokenList.ElementAt(i).ToString();
                var start = desc.IndexOf("<<" + i);

                if (start == -1)
                {
                    //console.log("didn't find: <<" + id);
                    continue;
                }
                //console.log("id" + id + ":" + retval);
                //console.log("Start Index: " + start);

                var end = desc.Substring(start).IndexOf(">>") + 2;

                //console.log("Length: " +length);
                var fullToken = desc.Substring(start, end);
                //console.log("Full: " + fullToken);

                var durationText = "";
                if (end > 5)
                {
                    try
                    {
                        string[] durationList = new string[] { "", "", "" };
                        var partialToken = fullToken.Substring(4, fullToken.Length - 7);
                        //console.log("Partial:" + partialToken);

                        durationList = partialToken.Replace("%d", "").Split('/').ToArray();
                        //console.log(durationList);

                        int pValue;
                        Int32.TryParse(value.ToString(), out pValue);

                        durationText = "";
                        if (pValue <= 0)
                            durationText = durationList[0];
                        else if (pValue > 1)
                            durationText = durationList[2];
                        else
                            durationText = durationList[1];
                        //console.log(pValue + durationText);
                    }
                    catch (Exception ex)
                    {
                        //this happens when the tokens are malformed
                    }
                }
                //console.log(type);
                while (desc.IndexOf(fullToken) != -1)
                { //sometimes there's multiple instance of the same token.
                    bool breakloop = false;
                    desc = desc.Replace(fullToken, value + durationText);
                    if (breakloop) break;
                }

            }
            return desc;
        }

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

            public override int GetHashCode()
            {
                //This verifies member data to which probably isn't ideal?
                int hash = 2;
                if(OffensiveStats != null)
                {
                    hash = hash * 3;
                    foreach(StatData std in OffensiveStats)
                    {
                        hash = hash * 31 + std.GetHashCode();
                    }
                }
                if (DefensiveStats != null)
                {
                    hash = hash * 5;
                    foreach (StatData std in DefensiveStats)
                    {
                        hash = hash * 31 + std.GetHashCode();
                    }
                }

                return hash;
            }
        }

        public class StatData : IEquatable<StatData>
        {
            internal DataObjectModel _dom;
            public StatData(DataObjectModel dom, GomObjectData gom)
            {
                _dom = dom;
                StatId = gom.ValueOrDefault<ScriptEnum>("talTalentStatName", new ScriptEnum()).Value;
                Stat = gom.ValueOrDefault<ScriptEnum>("talTalentStatName", new ScriptEnum()).ToString();
                Value = gom.ValueOrDefault<float>("talTalentStatValue");
                Modifier = gom.ValueOrDefault<ScriptEnum>("talTalentStatModifier", new ScriptEnum()).ToString().Replace("0x00", "AddToCurrent").Replace("modStatType_", "");
                Enabled = gom.ValueOrDefault<bool>("talTalentStatIsEnabled");
                AffectedNodeId = gom.ValueOrDefault<ulong>("talTalentStatTargetId");
            }
            public int StatId { get; set; }
            [Newtonsoft.Json.JsonIgnore]
            public string Stat { get; set; }
            public DetailedStat DetStat { get; set; }
            public float Value { get; set; }
            public bool Enabled { get; set; }

            [JsonConverter(typeof(ULongConverter))]
            public ulong AffectedNodeId { get; set; }
            public string AffectedNodeB62Id { get { return (AffectedNodeId != 0) ? AffectedNodeId.ToMaskedBase62() : ""; } }
            [Newtonsoft.Json.JsonIgnore]
            public Ability AffectedAbility { get { return _dom.abilityLoader.Load(AffectedNodeId); } }
            
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

            public override int GetHashCode()
            {
                int hash = StatId.GetHashCode();
                hash ^= AffectedNodeId.GetHashCode();
                hash ^= Enabled.GetHashCode();
                hash ^= Value.GetHashCode();

                if(Modifier != null)
                {
                    hash ^= Modifier.GetHashCode();
                }
                if(Stat != null)
                {
                    hash ^= Stat.GetHashCode();
                }

                return hash;
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

        public override List<SQLProperty> SQLProperties
        {
            get
            {
                return new List<SQLProperty>
                    {                //(SQL Column Name, C# Property Name, SQL Column type statement, isUnique/PrimaryKey, Serialize value to json)
                        new SQLProperty("Name", "Name", "varchar(255) COLLATE utf8_unicode_ci NOT NULL", SQLPropSetting.AddIndex),
                        new SQLProperty("FrName", "LocalizedName[frMale]", "varchar(255) COLLATE utf8_unicode_ci NOT NULL", SQLPropSetting.AddIndex),
                        new SQLProperty("DeName", "LocalizedName[deMale]", "varchar(255) COLLATE utf8_unicode_ci NOT NULL", SQLPropSetting.AddIndex),
                        new SQLProperty("Base62Id", "Base62Id", "varchar(7) COLLATE utf8_unicode_ci NOT NULL", SQLPropSetting.PrimaryKey),
                        new SQLProperty("Icon","HashedIcon", "varchar(25) COLLATE utf8_unicode_ci NOT NULL", SQLPropSetting.AddIndex),
                        new SQLProperty("Ranks", "Ranks", "int(11) NOT NULL", SQLPropSetting.AddIndex),
                    };
            }
        }
        public override XElement ToXElement(bool verbose)
        {
            XElement talent = new XElement("Talent");
            if (NodeId == 0) return talent;
            /*if (!File.Exists(String.Format("{0}{1}/icons/{2}.dds", Config.ExtractPath, prefix, Icon)))
            {
                var stream = _dom._assets.FindFile(String.Format("/resources/gfx/icons/{0}.dds", Icon));
                if(stream != null)
                    WriteFile((MemoryStream)stream.OpenCopyInMemory(), String.Format("/icons/{0}.dds", Icon));
            }*/
            if (verbose)
            {
                talent.Add(new XElement("Fqn", Fqn,
                            new XAttribute("NodeId", NodeId)),
                    new XAttribute("Base62Id", Base62Id),
                    new XElement("Name", Name, new XAttribute("Id", NameId)),
                    new XAttribute("Id", Id),
                    new XElement("Ranks", Ranks),
                    new XElement("Description", Description,
                        new XAttribute("Id", DescriptionId)),
                    new XElement("DescriptionRank2", DescriptionRank2),
                    new XElement("DescriptionRank3", DescriptionRank3),
                    new XElement("Icon", Icon),
                    new XElement("Tokens", TokenList.Select(x => new XElement("Token", new XAttribute("Id", TokenList.IndexOf(x)), x))));
            }
            else
            {
                talent.Add(new XElement("Name", Name),
                    new XElement("Ranks", Ranks),
                    new XElement("Description", Description));

            }
            if (verbose)
            {
                int r = 1;
                foreach (var blah in RankStats)
                {
                    XElement rank = new XElement("Rank", new XAttribute("Id", r));
                    string firstStatList = "{ ";
                    foreach (var stat in blah.DefensiveStats)
                    {
                        string affectedAbility = "";
                        if (stat.AffectedNodeId != 0)
                        {
                            if (stat.AffectedAbility != null)
                            {
                                if (stat.AffectedAbility.Fqn != null)
                                {
                                    affectedAbility = stat.AffectedAbility.Fqn;
                                }
                                else if (stat.AffectedNodeId != 0)
                                {
                                    affectedAbility = stat.AffectedNodeId.ToString();
                                }
                            }
                        }
                        firstStatList += String.Format("{0}, {1}, {2}, {3}, {4}; ", _dom.statData.ToStat(stat.Stat), stat.Value, stat.Modifier, stat.Enabled, affectedAbility);
                    }
                    rank.Add(new XElement("FirstStatList", firstStatList + "}"));
                    string secondStatList = "{ ";
                    foreach (var stat in blah.OffensiveStats)
                    {
                        string affectedAbility = "";
                        if (stat.AffectedNodeId != 0)
                        {
                            if (stat.AffectedAbility != null)
                            {
                                if (stat.AffectedAbility.Fqn != null)
                                {
                                    affectedAbility = stat.AffectedAbility.Fqn;
                                }
                                else if (stat.AffectedNodeId != 0)
                                {
                                    affectedAbility = stat.AffectedNodeId.ToString();
                                }
                            }
                        }
                        secondStatList += String.Format("{0}, {1}, {2}, {3}, {4}; ", _dom.statData.ToStat(stat.Stat), stat.Value, stat.Modifier, stat.Enabled, affectedAbility);
                    }

                    rank.Add(new XElement("SecondStatList", secondStatList + "}"));
                    //new XAttribute("Hash", itm.GetHashCode()),
                    talent.Add(rank);
                    r++;
                }
            }

            return talent;
        }
    }
}
