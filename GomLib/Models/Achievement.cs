using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;
using System.Xml.XPath;
using System.Text.RegularExpressions;
using System.Xml;
using Newtonsoft.Json;
using GomLib.ModelLoader;

namespace GomLib.Models
{
    public class Achievement : GameObject, IEquatable<Achievement>
    {
        [JsonIgnore]
        public ulong NodeId { get; set; }
        [JsonIgnore]
        public string Name { get; set; }
        public Dictionary<string, string> LocalizedName { get; set; }
        [JsonConverter(typeof(LongConverter))]
        public long NameId { get; set; }
        [JsonIgnore]
        public string Description { get; set; }
        public Dictionary<string, string> LocalizedDescription { get; set; }
        [JsonConverter(typeof(LongConverter))]
        public long DescriptionId { get; set; }
        [JsonIgnore]
        public string NonSpoilerDesc { get; set; }
        public Dictionary<string, string> LocalizedNonSpoilerDesc { get; set; }
        [JsonConverter(typeof(LongConverter))]
        public long NonSpoilerId { get; set; }
        public int Level { get; set; }
        public string Icon { get; set; }
        public string HashedIcon
        {
            get
            {
                var fileId = TorLib.FileId.FromFilePath(string.Format("/resources/gfx/icons/{0}.dds", Icon));
                return string.Format("{0}_{1}", fileId.ph, fileId.sh);
            }
        }
        [JsonConverter(typeof(Newtonsoft.Json.Converters.StringEnumConverter))]
        public AchievementVisibility Visibility { get; set; }
        public long AchId { get; set; }
        public long RewardsId { get; set; }
        public Rewards Rewards { get; set; }
        public List<AchTask> Tasks { get; set; }
        public List<AchCondition> Conditions { get; set; }
        public AchievementCatData CategoryData { get; internal set; }

        #region Reward Booleans
        public bool ItemReward
        {
            get
            {
                if (Rewards != null)
                {
                    if (Rewards.ItemRewardList != null && Rewards.ItemRewardList.Count > 0)
                        return true;
                }
                return false;
            }
        }
        public bool MtxReward
        {
            get
            {
                if (Rewards != null)
                {
                    if (Rewards.CartelCoins > 0)
                        return true;
                }
                return false;
            }
        }
        public bool TitleReward
        {
            get
            {
                if (Rewards != null)
                {
                    if (!string.IsNullOrEmpty(Rewards.LegacyTitle))
                        return true;
                }
                return false;
            }
        }
        public bool GsfReward
        {
            get
            {
                if (Rewards != null)
                {
                    if (Rewards.Requisition > 0)
                        return true;
                }
                return false;
            }
        }
        #endregion 

        public Achievement()
        {
            Name = "";
            Description = "";
        }

        public override int GetHashCode() //should be fixed
        {
            int hash = Level.GetHashCode();
            if (LocalizedDescription != null) foreach (var x in LocalizedDescription) { hash ^= x.GetHashCode(); } //dictionaries need to hashed like this
            if (LocalizedNonSpoilerDesc != null) foreach (var x in LocalizedNonSpoilerDesc) { hash ^= x.GetHashCode(); } //dictionaries need to hashed like this
            if (LocalizedName != null) foreach (var x in LocalizedName) { hash ^= x.GetHashCode(); } //dictionaries need to hashed like this
            if (Icon != null) { hash ^= Icon.GetHashCode(); }
            hash ^= Visibility.GetHashCode();
            hash ^= AchId.GetHashCode();
            hash ^= RewardsId.GetHashCode();
            if (Rewards != null) { hash ^= Rewards.GetHashCode(); }
            if (Tasks != null) foreach (var x in Tasks) { hash ^= x.GetHashCode(); }
            if (Conditions != null) foreach (var x in Conditions) { hash ^= x.GetHashCode(); }
            return hash;
        }

        public override string ToString()
        {
            return string.Format("{0} {1} {2}", NameId, Name, Description);
        }

        public override bool Equals(object obj)
        {
            if (obj == null) return false;

            if (ReferenceEquals(this, obj)) return true;

            if (!(obj is Achievement ach)) return false;

            return Equals(ach);
        }

        public bool Equals(Achievement ach)
        {
            if (ach == null) return false;

            if (ReferenceEquals(this, ach)) return true;

            if (AchId != ach.AchId)
                return false;
            if (Description != ach.Description)
                return false;
            if (DescriptionId != ach.DescriptionId)
                return false;
            if (Fqn != ach.Fqn)
                return false;
            if (Icon != ach.Icon)
                return false;
            if (Id != ach.Id)
                return false;
            if (Level != ach.Level)
                return false;

            var ssComp = new DictionaryComparer<string, string>();
            if (!ssComp.Equals(LocalizedDescription, ach.LocalizedDescription))
                return false;
            if (!ssComp.Equals(LocalizedName, ach.LocalizedName))
                return false;
            if (!ssComp.Equals(LocalizedNonSpoilerDesc, ach.LocalizedNonSpoilerDesc))
                return false;


            if (Name != ach.Name)
                return false;
            if (NameId != ach.NameId)
                return false;
            if (NodeId != ach.NodeId)
                return false;


            if (NonSpoilerDesc != ach.NonSpoilerDesc)
                return false;
            if (NonSpoilerId != ach.NonSpoilerId)
                return false;
            if (Rewards != null)
            {
                if (ach.Rewards == null)
                    return false;
                if (!Rewards.Equals(ach.Rewards))
                    return false;
            }
            else if (ach.Rewards != null)
                return false;
            if (RewardsId != ach.RewardsId)
                return false;
            if (Tasks != null)
            {
                if (ach.Tasks == null)
                    return false;
                if (!Tasks.SequenceEqual(ach.Tasks))
                    return false;
            }
            else if (ach.Tasks != null)
                return false;
            if (Conditions != null)
            {
                if (ach.Conditions == null)
                    return false;
                if (!Conditions.SequenceEqual(ach.Conditions))
                    return false;
            }
            else if (ach.Conditions != null)
                return false;
            if (Visibility != ach.Visibility)
                return false;
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
                        new SQLProperty("AchId", "AchId", "bigint(20) NOT NULL", SQLPropSetting.AddIndex),
                        new SQLProperty("Visibility", "Visibility", "varchar(15) NOT NULL", SQLPropSetting.AddIndex),
                        new SQLProperty("Icon", "HashedIcon", "varchar(255) COLLATE utf8_unicode_ci NOT NULL", SQLPropSetting.AddIndex),
                        new SQLProperty("Category","CategoryData.Category.LocalizedName[enMale]", "varchar(255) COLLATE utf8_unicode_ci NOT NULL", SQLPropSetting.AddIndex),
                        new SQLProperty("FrCategory","CategoryData.Category.LocalizedName[frMale]", "varchar(255) COLLATE utf8_unicode_ci NOT NULL", SQLPropSetting.AddIndex),
                        new SQLProperty("DeCategory","CategoryData.Category.LocalizedName[deMale]", "varchar(255) COLLATE utf8_unicode_ci NOT NULL", SQLPropSetting.AddIndex),
                        new SQLProperty("SubCategory","CategoryData.SubCategory.LocalizedName[enMale]", "varchar(255) COLLATE utf8_unicode_ci NOT NULL", SQLPropSetting.AddIndex),
                        new SQLProperty("FrSubCategory","CategoryData.SubCategory.LocalizedName[frMale]", "varchar(255) COLLATE utf8_unicode_ci NOT NULL", SQLPropSetting.AddIndex),
                        new SQLProperty("DeSubCategory","CategoryData.SubCategory.LocalizedName[deMale]", "varchar(255) COLLATE utf8_unicode_ci NOT NULL", SQLPropSetting.AddIndex),
                        new SQLProperty("TertiaryCategory","CategoryData.TertiaryCategory.LocalizedName[enMale]", "varchar(255) COLLATE utf8_unicode_ci NOT NULL", SQLPropSetting.AddIndex),
                        new SQLProperty("FrTertiaryCategory","CategoryData.TertiaryCategory.LocalizedName[frMale]", "varchar(255) COLLATE utf8_unicode_ci NOT NULL", SQLPropSetting.AddIndex),
                        new SQLProperty("DeTertiaryCategory","CategoryData.TertiaryCategory.LocalizedName[deMale]", "varchar(255) COLLATE utf8_unicode_ci NOT NULL", SQLPropSetting.AddIndex),
                        new SQLProperty("ItemReward", "ItemReward", "tinyint(1) NOT NULL", SQLPropSetting.AddIndex),
                        new SQLProperty("MtxReward", "MtxReward", "tinyint(1) NOT NULL", SQLPropSetting.AddIndex),
                        new SQLProperty("TitleReward", "TitleReward", "tinyint(1) NOT NULL", SQLPropSetting.AddIndex),
                        new SQLProperty("GsfReward", "GsfReward", "tinyint(1) NOT NULL", SQLPropSetting.AddIndex),
                    };
            }
        }

        public override XElement ToXElement(bool verbose)
        {
            XElement achievement = new XElement("Achievement");

            string visibility;
            if (Visibility == AchievementVisibility.Completion)
            {
                visibility = "On Completion";
            }
            else if (Visibility == AchievementVisibility.Progress)
            {
                visibility = "On task started";
            }
            else
            {
                visibility = "Always";
            }

            achievement.Add(
                new XElement("Name", Name),
                new XElement("Description", Description),
                new XElement("Fqn", Fqn),
                new XAttribute("Id", Id),
                new XElement("Icon", Icon),
                new XElement("Visbility", visibility));

            if (verbose)
            {
                /*achievement.Element("Name").RemoveAll();
                for (int i = 0; i < localizations.Count; i++)
                {
                    if (LocalizedName[localizations[i]] != "")
                    {
                        achievement.Element("Name").Add(new XElement(localizations[i], LocalizedName[localizations[i]]));
                    }
                }

                achievement.Element("Description").RemoveAll();
                for (int i = 0; i < localizations.Count; i++)
                {
                    if (LocalizedDescription[localizations[i]] != "")
                    {
                        achievement.Element("Description").Add(new XElement(localizations[i], LocalizedDescription[localizations[i]]));
                    }
                } */

                achievement.Element("Name").Add(new XAttribute("Id", NameId));
                achievement.Element("Description").Add(new XAttribute("Id", DescriptionId));
                achievement.Element("Fqn").Add(new XAttribute("Id", NodeId));

                //new XAttribute("Hash", GetHashCode()),
                if (AchId != 0)
                {
                    achievement.Add(new XElement("AchID", AchId));
                }
                else
                {
                    //Achivement broken. Bring this to the attention of people so it hopefully gets fixed before live.
                    achievement.Add(new XElement("AchID", "Broken Achievement."));
                }
                achievement.Add(
                new XElement("NonSpoilerDescription", NonSpoilerDesc,
                    new XAttribute("Id", NonSpoilerId)),
                new XElement("Requirements"),
                new XElement("Amounts"));


                /*for (int i = 0; i < localizations.Count; i++)
                {
                    if (LocalizedNonSpoilerDesc[localizations[i]] != "")
                    {
                        achievement.Element("NonSpoilerDescription").Add(new XElement(localizations[i], LocalizedNonSpoilerDesc[localizations[i]]));
                    }
                }*/

                //string rewards = "{ " + String.Join(", ", RewardsList) + " }";
            }
            achievement.Add(new XElement("Rewards"));
            if (RewardsId != 0 && Rewards != null)
            {
                string cC = Rewards.CartelCoins.ToString();
                if (cC == "0") cC = null;
                string fR = Rewards.Requisition.ToString();
                if (fR == "0") fR = null;
                achievement.Element("Rewards").Add(new XAttribute("Id", RewardsId),
                    new XElement("AchievementPoints", Rewards.AchievementPoints));
                if (cC != null) achievement.Element("Rewards").Add(new XElement("CartelCoins", cC));
                if (fR != null) achievement.Element("Rewards").Add(new XElement("FleetRequisition", fR));
                if (Rewards.LegacyTitle != null) achievement.Element("Rewards").Add(new XElement("LegacyTitle", Rewards.LegacyTitle));

                XElement itemRew = new XElement("Items", new XAttribute("Num", Rewards.ItemRewardList.Count));
                foreach (var kvp in Rewards.ItemRewardList)
                {
                    var item = new GameObject().ToXElement(kvp.Key, Dom_, true);
                    if (item != null)
                    {
                        item.Add(new XAttribute("Quantity", kvp.Value));
                        itemRew.Add(item);
                    }
                }
                if (itemRew.HasElements)
                    achievement.Element("Rewards").Add(itemRew);
                /*if (Rewards.LocalizedLegacyTitle.Count > 0)
                {
                    for (int i = 0; i < localizations.Count; i++)
                    {
                        if (Rewards.LocalizedLegacyTitle[localizations[i]] != "")
                        {
                            achievement.Element("Rewards").Element("LegacyTitle").Add(new XElement(localizations[i], Rewards.LocalizedLegacyTitle[localizations[i]]));
                        }
                    }
                }*/
            }
            if (verbose)
            {
                //TODO: Add code to output tasks
                if (References != null)
                    achievement.Add(ReferencesToXElement());
            }
            return achievement;
        }
    }

    public class Rewards : PseudoGameObject, IEquatable<Rewards>
    {
        public Dictionary<string, string> LocalizedLegacyTitle { get; set; }
        public string LegacyTitle { get; set; }
        //public string Title { get; set; }
        public long CartelCoins { get; set; }
        public long AchievementPoints { get; set; }
        public long Requisition { get; set; }
        [JsonIgnore]
        public Dictionary<ulong, long> ItemRewardList { get; set; }
        internal Dictionary<string, long> ItemRewardListB62_ { get; set; }
        public Dictionary<string, long> ItemRewardListB62
        {
            get
            {
                if (ItemRewardListB62_ == null && ItemRewardList != null)
                {
                    ItemRewardListB62_ = ItemRewardList.ToDictionary(x => x.Key.ToMaskedBase62(), x => x.Value);
                }
                return ItemRewardListB62_;
            }
        }

        public override int GetHashCode()
        {
            int hash = CartelCoins.GetHashCode();
            hash ^= AchievementPoints.GetHashCode();
            hash ^= Requisition.GetHashCode();
            if (LocalizedLegacyTitle != null) foreach (var x in LocalizedLegacyTitle) { hash ^= x.GetHashCode(); } //dictionaries need to hashed like this
            if (ItemRewardList != null) foreach (var x in ItemRewardList) { hash ^= x.GetHashCode(); } //dictionaries need to hashed like this
            return hash;
        }

        public override string ToString()
        {
            return string.Format("{0} {1} {2} {3}", LegacyTitle, CartelCoins, AchievementPoints, Requisition);
        }

        public override bool Equals(object obj)
        {
            if (obj == null) return false;

            if (ReferenceEquals(this, obj)) return true;

            if (!(obj is Rewards rwd)) return false;

            return Equals(rwd);
        }

        public bool Equals(Rewards rwd)
        {
            if (rwd == null) return false;

            if (ReferenceEquals(this, rwd)) return true;

            if (AchievementPoints != rwd.AchievementPoints)
                return false;
            if (CartelCoins != rwd.CartelCoins)
                return false;
            if (Id != rwd.Id)
                return false;
            if (LegacyTitle != rwd.LegacyTitle)
                return false;

            var ssComp = new DictionaryComparer<string, string>();
            if (!ssComp.Equals(LocalizedLegacyTitle, rwd.LocalizedLegacyTitle))
                return false;

            if (Requisition != rwd.Requisition)
                return false;

            var ulComp = new DictionaryComparer<ulong, long>();
            if (!ulComp.Equals(ItemRewardList, rwd.ItemRewardList))
                return false;
            return true;
        }
    }

    //Each achievement consists of one or multiple tasks
    public class AchTask : GameObject, IEquatable<AchTask>
    {
        public long Index { get; set; }
        public long Index2 { get; set; }
        public long Count { get; set; }
        public string Name { get; set; }
        public Dictionary<string, string> LocalizedNames { get; set; }
        public List<AchEvent> Events { get; set; }

        public override int GetHashCode()
        {
            int hash = Index.GetHashCode();
            hash ^= Index2.GetHashCode();
            hash ^= Count.GetHashCode();
            hash ^= Name.GetHashCode();
            foreach (var x in LocalizedNames) { hash ^= x.GetHashCode(); } //dictionaries need to hashed like this
            foreach (var x in Events) { hash ^= x.GetHashCode(); }
            return hash;
        }

        public override string ToString()
        {
            return string.Format("{0}x {1}", Count, Name);
        }

        public override bool Equals(object obj)
        {
            if (obj == null) return false;

            if (ReferenceEquals(this, obj)) return true;

            if (!(obj is AchTask rwd)) return false;

            return Equals(rwd);
        }

        public bool Equals(AchTask rwd)
        {
            if (rwd == null) return false;

            if (ReferenceEquals(this, rwd)) return true;

            if (Index != rwd.Index)
                return false;
            if (Index2 != rwd.Index2)
                return false;
            if (Count != rwd.Count)
                return false;
            if (Name != rwd.Name)
                return false;

            var ssComp = new DictionaryComparer<string, string>();
            if (!ssComp.Equals(LocalizedNames, rwd.LocalizedNames))
                return false;

            //var ulComp = new DictionaryComparer<AchEvent, AchEvent>();
            //if (!ulComp.Equals(this.Events, rwd.Events))
            //  return false;
            return true;
        }
    }

    //A list of events that each can complete an achievement task
    public class AchEvent : GameObject, IEquatable<AchEvent>
    {
        //public ulong Id { get; set; }
        public string NodePrefix { get; set; }
        public long Value { get; set; }

        public void CheckNodeRef(DataObjectModel _dom)
        {
            var tmpNode = _dom.GetObject(Id);
            if (tmpNode != null)
            {
                NodePrefix = tmpNode.Name.Substring(0, tmpNode.Name.IndexOf("."));
                if (NodePrefix != "npc")
                {
                    // string sioin = "";
                }
                tmpNode.Unload();
            }
        }

        public override int GetHashCode()
        {
            int hash = Id.GetHashCode();
            hash ^= Value.GetHashCode();
            return hash;
        }

        public override string ToString()
        {
            return string.Format("{0} ({1})", Id, Value);
        }

        public override bool Equals(object obj)
        {
            if (obj == null) return false;

            if (ReferenceEquals(this, obj)) return true;

            if (!(obj is AchEvent rwd)) return false;

            return Equals(rwd);
        }

        public bool Equals(AchEvent rwd)
        {
            if (rwd == null) return false;

            if (ReferenceEquals(this, rwd)) return true;

            if (Id != rwd.Id)
                return false;
            if (Value != rwd.Value)
                return false;

            return true;
        }
    }

    //Each achievement consists of one or multiple tasks
    public class AchCondition : GameObject, IEquatable<AchCondition>
    {
        public bool UnknownBoolean { get; set; }
        [JsonConverter(typeof(Newtonsoft.Json.Converters.StringEnumConverter))]
        public AchConditionType Type { get; set; }
        [JsonConverter(typeof(Newtonsoft.Json.Converters.StringEnumConverter))]
        public AchConditionTarget Target { get; set; }

        public override int GetHashCode()
        {
            int hash = UnknownBoolean.GetHashCode();
            hash ^= Type.GetHashCode();
            hash ^= Target.GetHashCode();
            return hash;
        }

        public override string ToString()
        {
            return string.Format("{0}", Type.ToString());
        }

        public override bool Equals(object obj)
        {
            if (obj == null) return false;

            if (ReferenceEquals(this, obj)) return true;

            if (!(obj is AchCondition rwd)) return false;

            return Equals(rwd);
        }

        public bool Equals(AchCondition rwd)
        {
            if (rwd == null) return false;

            if (ReferenceEquals(this, rwd)) return true;

            if (UnknownBoolean != rwd.UnknownBoolean)
                return false;
            if (Type != rwd.Type)
                return false;
            if (Target != rwd.Target)
                return false;

            return true;
        }
    }

    //Whether you can see this achievement in the achievement window
    public enum AchievementVisibility
    {
        Always = 0,//This achievement is always visible
        Progress = 1,//This achievement becomes visible when a player starts completing one of the tasks
        Completion = 2//This achievement will only become visible once it has been completed
    }

    public enum AchConditionType
    {
        Area = 0,
        InPhase = 1,
        SurvivedNightmare = 2,
        IsClass = 3,
        Unknown4 = 4,
        CompletedInTime = 8,
        IsNightmare = 10,
        Faction = 11,
        Unknown13 = 13,
        Companion = 15,
        PvPScoreboard = 16,
        RaidDifficulty = 18,
        HasEffect = 19
    }

    public enum AchConditionTarget
    {
        Player = 0,
        Enemy = 2
    }

}
