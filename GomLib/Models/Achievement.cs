using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;
using System.Xml.XPath;
using System.Text.RegularExpressions;
using System.Xml;
using Newtonsoft.Json;

namespace GomLib.Models
{
    public class Achievement : GameObject, IEquatable<Achievement>
    {
        public ulong NodeId { get; set; }
        public string Name { get; set; }
        public Dictionary<string, string> LocalizedName { get; set; }
        public long NameId { get; set; }
        public string Description { get; set; }
        public Dictionary<string, string> LocalizedDescription { get; set; }
        public long DescriptionId { get; set; }
        public string NonSpoilerDesc { get; set; }
        public Dictionary<string, string> LocalizedNonSpoilerDesc { get; set; }
        public long nonSpoilerId { get; set; }
        public int Level { get; set; }
        public string Icon { get; set; }
        public AchievementVisibility Visibility { get; set; }
        public long AchId { get; set; }
        public long RewardsId { get; set; }
        public Rewards Rewards { get; set; }
        public List<AchTask> Tasks { get; set; }
        public List<AchCondition> Conditions { get; set; }

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

            Achievement ach = obj as Achievement;
            if (ach == null) return false;

            return Equals(ach);
        }

        public bool Equals(Achievement ach)
        {
            if (ach == null) return false;

            if (ReferenceEquals(this, ach)) return true;

            if (this.AchId != ach.AchId)
                return false;
            if (this.Description != ach.Description)
                return false;
            if (this.DescriptionId != ach.DescriptionId)
                return false;
            if (this.Fqn != ach.Fqn)
                return false;
            if (this.Icon != ach.Icon)
                return false;
            if (this.Id != ach.Id)
                return false;
            if (this.Level != ach.Level)
                return false;

            var ssComp = new DictionaryComparer<string, string>();
            if (!ssComp.Equals(this.LocalizedDescription, ach.LocalizedDescription))
                return false;
            if (!ssComp.Equals(this.LocalizedName, ach.LocalizedName))
                return false;
            if (!ssComp.Equals(this.LocalizedNonSpoilerDesc, ach.LocalizedNonSpoilerDesc))
                return false;

            
            if (this.Name != ach.Name)
                return false;
            if (this.NameId != ach.NameId)
                return false;
            if (this.NodeId != ach.NodeId)
                return false;

            
            if (this.NonSpoilerDesc != ach.NonSpoilerDesc)
                return false;
            if (this.nonSpoilerId != ach.nonSpoilerId)
                return false;
            if (this.Rewards != null)
            {
                if (ach.Rewards == null)
                    return false;
                if (!this.Rewards.Equals(ach.Rewards))
                    return false;
            }
            else if (ach.Rewards != null)
                return false;
            if (this.RewardsId != ach.RewardsId)
                return false;
            if (this.Tasks != null)
            {
                if (ach.Tasks == null)
                    return false;
                if (!this.Tasks.SequenceEqual(ach.Tasks))
                    return false;
            }
            else if (ach.Tasks != null)
                return false;
            if (this.Conditions != null)
            {
                if (ach.Conditions == null)
                    return false;
                if (!this.Conditions.SequenceEqual(ach.Conditions))
                    return false;
            }
            else if (ach.Conditions != null)
                return false;
            if (this.Visibility != ach.Visibility)
                return false;
            return true;
        }

        public override List<SQLProperty> SQLProperties
        {
            get
            {
                return new List<SQLProperty>
                    {                //(SQL Column Name, C# Property Name, SQL Column type statement, isUnique/PrimaryKey, Serialize value to json)
                        new SQLProperty("NodeId", "NodeId", "bigint(20) unsigned NOT NULL", true),
                        new SQLProperty("Name", "Name", "varchar(255) COLLATE utf8_unicode_ci NOT NULL"),
                        new SQLProperty("NameId", "NameId", "bigint(20) NOT NULL"),
                        new SQLProperty("Description", "Description", "varchar(255) COLLATE utf8_unicode_ci NOT NULL"),
                        new SQLProperty("DescriptionId", "DescriptionId", "bigint(20) NOT NULL"),
                        new SQLProperty("AchId", "AchId", "bigint(20) NOT NULL"),
                        new SQLProperty("Visibility", "Visibility", "varchar(15) NOT NULL"),
                        new SQLProperty("Icon", "Icon", "varchar(255) COLLATE utf8_unicode_ci NOT NULL"),
                        new SQLProperty("Tasks", "Tasks", "varchar(100) COLLATE utf8_unicode_ci NOT NULL", false, true),
                        new SQLProperty("Conditions", "Conditions", "varchar(1000) COLLATE utf8_unicode_ci NOT NULL", false, true),
                        new SQLProperty("Rewards", "Rewards", "varchar(1000) COLLATE utf8_unicode_ci NOT NULL", false, true)
                    };
            }
        }

        public override XElement ToXElement(bool verbose)
        {
            XElement achievement = new XElement("Achievement");

            achievement.Add(
                new XElement("Name", Name),
                new XElement("Description", Description),
                new XElement("Fqn", Fqn),
                new XAttribute("Id", Id),
                new XElement("Icon", Icon));

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
                
                achievement.Add(new XElement("Rewards"),
                new XElement("NonSpoilerDescription", NonSpoilerDesc,
                    new XAttribute("Id", nonSpoilerId)),
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
                if (RewardsId != 0)
                {
                    string cC = Rewards.CartelCoins.ToString();
                    if (cC == "0") cC = null;
                    string fR = Rewards.Requisition.ToString();
                    if (fR == "0") fR = null;
                    achievement.Element("Rewards").Add(new XAttribute("Id", RewardsId),
                        new XElement("AchievementPoints", Rewards.AchievementPoints),
                        new XElement("CartelCoins", cC),
                        new XElement("FleetRequisition", fR),
                        new XElement("LegacyTitle", Rewards.LegacyTitle));
                    
                    XElement itemRew = new XElement("Items", new XAttribute("Num", Rewards.ItemRewardList.Count));
                    foreach (var kvp in Rewards.ItemRewardList)
                    {
                        var item = new GameObject().ToXElement(kvp.Key, _dom, true);
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
        public Dictionary<ulong, long> ItemRewardList { get; set; }

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

            Rewards rwd = obj as Rewards;
            if (rwd == null) return false;

            return Equals(rwd);
        }

        public bool Equals(Rewards rwd)
        {
            if (rwd == null) return false;

            if (ReferenceEquals(this, rwd)) return true;

            if (this.AchievementPoints != rwd.AchievementPoints)
                return false;
            if (this.CartelCoins != rwd.CartelCoins)
                return false;
            if (this.Id != rwd.Id)
                return false;
            if (this.LegacyTitle != rwd.LegacyTitle)
                return false;

            var ssComp = new DictionaryComparer<string, string>();
            if (!ssComp.Equals(this.LocalizedLegacyTitle, rwd.LocalizedLegacyTitle))
                return false;

            if (this.Requisition != rwd.Requisition)
                return false;

            var ulComp = new DictionaryComparer<ulong, long>();
            if (!ulComp.Equals(this.ItemRewardList, rwd.ItemRewardList))
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

            AchTask rwd = obj as AchTask;
            if (rwd == null) return false;

            return Equals(rwd);
        }

        public bool Equals(AchTask rwd)
        {
            if (rwd == null) return false;

            if (ReferenceEquals(this, rwd)) return true;

            if (this.Index != rwd.Index)
                return false;
            if (this.Index2 != rwd.Index2)
                return false;
            if (this.Count != rwd.Count)
                return false;
            if (this.Name != rwd.Name)
                return false;

            var ssComp = new DictionaryComparer<string, string>();
            if (!ssComp.Equals(this.LocalizedNames, rwd.LocalizedNames))
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
        public ulong NodeRef { get; set; }
        public string NodePrefix { get; set; }
        public long Value { get; set; }

        public void checkNodeRef(DataObjectModel _dom) {
            var tmpNode = _dom.GetObject(NodeRef);
            if (tmpNode != null)
            {
                NodePrefix = tmpNode.Name.Substring(0, tmpNode.Name.IndexOf("."));
                tmpNode.Unload();
            }
        }

        public override int GetHashCode()
        {
            int hash = NodeRef.GetHashCode();
            hash ^= Value.GetHashCode();
            return hash;
        }

        public override string ToString()
        {
            return string.Format("{0} ({1})", NodeRef, Value);
        }

        public override bool Equals(object obj)
        {
            if (obj == null) return false;

            if (ReferenceEquals(this, obj)) return true;

            AchEvent rwd = obj as AchEvent;
            if (rwd == null) return false;

            return Equals(rwd);
        }

        public bool Equals(AchEvent rwd)
        {
            if (rwd == null) return false;

            if (ReferenceEquals(this, rwd)) return true;

            if (this.NodeRef != rwd.NodeRef)
                return false;
            if (this.Value != rwd.Value)
                return false;

            return true;
        }
    }

    //Each achievement consists of one or multiple tasks
    public class AchCondition : GameObject, IEquatable<AchCondition>
    {
        public bool UnknownBoolean { get; set; }
        public AchConditionType Type { get; set; }
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

            AchCondition rwd = obj as AchCondition;
            if (rwd == null) return false;

            return Equals(rwd);
        }

        public bool Equals(AchCondition rwd)
        {
            if (rwd == null) return false;

            if (ReferenceEquals(this, rwd)) return true;

            if (this.UnknownBoolean != rwd.UnknownBoolean)
                return false;
            if (this.Type != rwd.Type)
                return false;
            if (this.Target != rwd.Target)
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
