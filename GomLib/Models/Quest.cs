using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;
using System.Xml.Linq;
using System.Xml.XPath;

namespace GomLib.Models
{
    public class Quest : GameObject, IEquatable<Quest>
    {
        internal Dictionary<object,object> TextLookup { get; set; }

        public ulong NodeId { get; set; }
        public string Name { get; set; }
        public Dictionary<string, string> LocalizedName { get; set; }
        public string Icon { get; set; }
        public bool IsRepeatable { get; set; }
        public int RequiredLevel { get; set; }
        public int XpLevel { get; set; }
        public QuestDifficulty Difficulty { get; set; }
        public bool CanAbandon { get; set; }
        public bool IsHidden { get; set; }
        public bool IsClassQuest { get; set; }
        public bool IsBonus { get; set; }
        public bool BonusShareable { get; set; }
        public long CategoryId { get; set; }
        public string Category { get; set; }//QuestCategory Category { get; set; }
        public List<QuestBranch> Branches { get; set; }
        public Dictionary<ulong, QuestItem> Items { get; set; }
        public ClassSpecList Classes { get; set; }
        public List<QuestReward> Rewards { get; set; }
        public string ReqPrivacy { get; set; }
        internal List<Quest> _BonusMissions;
        [Newtonsoft.Json.JsonIgnore]
        public List<Quest> BonusMissions
        {
            get
            {
                if (_BonusMissions == null)
                {
                    _BonusMissions = new List<Quest>();
                    foreach (var bonMisId in BonusMissionsIds)
                    {
                        _BonusMissions.Add(_dom.questLoader.Load(bonMisId));
                    }
                }
                return _BonusMissions;
            }
        }
        public List<ulong> BonusMissionsIds { get; set; }
        [Newtonsoft.Json.JsonIgnore]
        public List<object> ItemMap { get; set; }

        public override int GetHashCode()
        {
            int hash = Name.GetHashCode();
            if (Icon != null) { hash ^= Icon.GetHashCode(); }
            hash ^= IsRepeatable.GetHashCode();
            hash ^= RequiredLevel.GetHashCode();
            hash ^= XpLevel.GetHashCode();
            hash ^= Difficulty.GetHashCode();
            hash ^= CanAbandon.GetHashCode();
            hash ^= IsHidden.GetHashCode();
            hash ^= IsClassQuest.GetHashCode();
            hash ^= IsBonus.GetHashCode();
            hash ^= BonusShareable.GetHashCode();
            hash ^= Category.GetHashCode();
            foreach (var x in Branches) { hash ^= x.GetHashCode(); }
            foreach (var x in Classes) { hash ^= x.Id.GetHashCode(); }
            return hash;
        }

        public override bool Equals(object obj)
        {
            if (obj == null) return false;

            if (ReferenceEquals(this, obj)) return true;

            Quest qst = obj as Quest;
            if (qst == null) return false;

            return Equals(qst);
        }

        public bool Equals(Quest qst)
        {
            if (qst == null) return false;

            if (ReferenceEquals(this, qst)) return true;

            if (this.BonusMissionsIds != null)
            {
                if (qst.BonusMissionsIds == null)
                {
                    return false;
                }
                else
                {
                    if (!Enumerable.SequenceEqual<ulong>(this.BonusMissionsIds, qst.BonusMissionsIds))
                        return false;
                }
            }
            if (this.BonusShareable != qst.BonusShareable)
                return false;
            if (this.Branches != null)
            {
                if (qst.Branches == null)
                {
                    return false;
                }
                else
                {
                    if (!Enumerable.SequenceEqual<QuestBranch>(this.Branches, qst.Branches))
                        return false;
                }
            }
            if (this.CanAbandon != qst.CanAbandon)
                return false;
            if (!this.Category.Equals(qst.Category))
            {
                return false;
            }
            if (this.CategoryId != qst.CategoryId)
                return false;
            if (this.Classes != null)
            {
                if (qst.Classes == null)
                {
                    return false;
                }
                else
                {
                    if (!this.Classes.Equals(qst.Classes, false))
                        return false;
                }
            }
            if (!this.Difficulty.Equals(qst.Difficulty))
                return false;
            if (this.Fqn != qst.Fqn)
                return false;
            if (this.Icon != qst.Icon)
                return false;
            if (this.Id != qst.Id)
                return false;
            if (this.IsBonus != qst.IsBonus)
                return false;
            if (this.IsClassQuest != qst.IsClassQuest)
                return false;
            if (this.IsHidden != qst.IsHidden)
                return false;
            if (this.IsRepeatable != qst.IsRepeatable)
                return false;

            var uQIComp = new DictionaryComparer<ulong, QuestItem>();
            if (!uQIComp.Equals(this.Items, qst.Items))
                return false;

            var ssComp = new DictionaryComparer<string, string>();
            if (!ssComp.Equals(this.LocalizedName, qst.LocalizedName))
                return false;
            
            if (this.Name != qst.Name)
                return false;
            if (this.NodeId != qst.NodeId)
                return false;
            if (this.ReqPrivacy != qst.ReqPrivacy)
                return false;
            if (this.RequiredLevel != qst.RequiredLevel)
                return false;
            if (this.Rewards != null)
            {
                if (qst.Rewards == null)
                {
                    return false;
                }
                else
                {
                    if (!Enumerable.SequenceEqual<QuestReward>(this.Rewards, qst.Rewards))
                        return false;
                }
            }
            if (this.XpLevel != qst.XpLevel)
                return false;
            return true;
        }
    }

    public class QuestReward : GameObject, IEquatable<QuestReward>
    {
        public long UnknownNum { get; set; }
        public bool IsAlwaysProvided { get; set; }
        [Newtonsoft.Json.JsonIgnore]
        public Item RewardItem {
            get { return _dom.itemLoader.Load(RewardItemId); }
            set { RewardItem = value; }
        }
        public ulong RewardItemId { get; set; }
        public ClassSpecList Classes { get; set; }
        public long NumberOfItem { get; set; }
        public long MinLevel { get; set; }
        public long MaxLevel { get; set; }

        public override bool Equals(object obj)
        {
            if (obj == null) return false;

            if (ReferenceEquals(this, obj)) return true;

            QuestReward qsr = obj as QuestReward;
            if (qsr == null) return false;

            return Equals(qsr);
        }

        public bool Equals(QuestReward qsr)
        {
            if (qsr == null) return false;

            if (ReferenceEquals(this, qsr)) return true;

            if (this.Classes != null)
            {
                if (qsr.Classes == null)
                {
                    return false;
                }
                else
                {
                    if (!this.Classes.Equals(qsr.Classes, false))
                        return false;
                }
            }
            if (this.Fqn != qsr.Fqn)
                return false;
            if (this.Id != qsr.Id)
                return false;
            if (this.IsAlwaysProvided != qsr.IsAlwaysProvided)
                return false;
            if (this.MaxLevel != qsr.MaxLevel)
                return false;
            if (this.MinLevel != qsr.MinLevel)
                return false;
            if (this.NumberOfItem != qsr.NumberOfItem)
                return false;
            //if (this.RewardItem.Equals(qsr.RewardItem))
                //return false;
            if (this.RewardItemId != qsr.RewardItemId)
                return false;
            if (this.UnknownNum != qsr.UnknownNum)
                return false;
            return true;
        }
    }
}
