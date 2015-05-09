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
        #region Properties
        internal Dictionary<object,object> TextLookup { get; set; }

        public ulong NodeId { get; set; }
        public string Name { get; set; }
        public long NameId { get; set; }
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
        public string Category { get; set; }
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
        public long CreditRewardType { get; set; }
        public float CreditsRewarded { get; set; }
        public long XP { get; set; }
        #endregion
        #region FlatQuestProperties
            [Newtonsoft.Json.JsonIgnore]
            public string CleanName
        {
            get
            {
                if (Name == null || Name == "")
                    return "Unnamed_Quest";
                //return Name.Replace("'", "");
                return System.IO.Path.GetInvalidFileNameChars().Aggregate(Name, (current, c) => current.Replace(c.ToString(), string.Empty)).Replace("'", "").Replace(" ", "_");
            }
        }
            [Newtonsoft.Json.JsonIgnore]
            public string HashedIcon
            {
                get
                {
                    var fileId = TorLib.FileId.FromFilePath(String.Format("/resources/gfx/icons/{0}.dds", this.Icon));
                    return String.Format("{0}_{1}", fileId.ph, fileId.sh);
                }
            }
            #region Branches
            public int BranchCount
            {
                get
                {
                    return Branches.Count;
                }
            }
            [Newtonsoft.Json.JsonIgnore]
            internal List<string> _BranchIds { get; set; }
            [Newtonsoft.Json.JsonIgnore]
            public List<string> BranchIds
            {
                get
                {
                    if (_BranchIds == null)
                    {
                        _BranchIds = Branches.Select(x => x.Id.ToMaskedBase62()).ToList();
                    }
                    return _BranchIds;
                }
            }
            #endregion
            #region Rewards
            [Newtonsoft.Json.JsonIgnore]
            internal List<string> _RewardIds { get; set; }
            [Newtonsoft.Json.JsonIgnore]
            public List<string> RewardIds
            {
                get
                {
                    if (_RewardIds == null)
                    {
                        if (Rewards == null) return new List<string>();
                        _RewardIds = Rewards.Select(x => x.Id.ToMaskedBase62()).ToList();
                    }
                    return _RewardIds;
                }
            }
            #endregion
            #region Bonus Missions
            [Newtonsoft.Json.JsonIgnore]
            internal List<string> _BonusMissionsB62Ids { get; set; }
            [Newtonsoft.Json.JsonIgnore]
            public List<string> BonusMissionsB62Ids
            {
                get
                {
                    if(_BonusMissionsB62Ids == null)
                    {
                        if (BonusMissionsIds == null) return new List<string>();
                        _BonusMissionsB62Ids = BonusMissionsIds.Select(x => x.ToMaskedBase62()).ToList();
                    }
                    return _BonusMissionsB62Ids;
                }
            }
            #endregion
            #region Classes
            [Newtonsoft.Json.JsonIgnore]
            internal List<string> _ClassesB62 { get; set; }
            [Newtonsoft.Json.JsonIgnore]
            public List<string> ClassesB62
            {
                get
                {
                    if (_ClassesB62 == null)
                    {
                        if (Classes == null) return new List<string>();
                        _ClassesB62 = Classes.Select(x => x.Base62Id).ToList();
                    }
                    return _ClassesB62;
                }
            }
            #endregion
        #endregion

        #region IEquatable<Quest>
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
        #endregion

        #region Output Function
        public override List<SQLProperty> SQLProperties
        {
            get
            {
                return new List<SQLProperty>
                    {                //(SQL Column Name, C# Property Name, SQL Column type statement, isUnique/PrimaryKey, Serialize value to json)
                        new SQLProperty("Name", "Name", "varchar(255) COLLATE utf8_unicode_ci NOT NULL"),
                        new SQLProperty("CleanName", "CleanName", "varchar(255) COLLATE utf8_unicode_ci NOT NULL"),
                        new SQLProperty("Base62Id", "Base62Id", "varchar(7) COLLATE latin1_general_cs NOT NULL", true),
                        new SQLProperty("NameId", "NameId", "bigint(20) NOT NULL"),
                        new SQLProperty("Fqn", "Fqn", "varchar(255) COLLATE utf8_unicode_ci NOT NULL"),
                        new SQLProperty("LocalizedName", "LocalizedName", "varchar(255) COLLATE utf8_unicode_ci NOT NULL", false, true),
                        new SQLProperty("Icon", "HashedIcon", "varchar(255) COLLATE utf8_unicode_ci NOT NULL"),
                        new SQLProperty("IsRepeatable", "IsRepeatable", "tinyint(1) NOT NULL"),
                        new SQLProperty("RequiredLevel", "RequiredLevel", "int(11) NOT NULL"),
                        new SQLProperty("XpLevel", "XpLevel", "int(11) NOT NULL"),
                        new SQLProperty("XP", "XP", "int(11) NOT NULL"),
                        new SQLProperty("Difficulty", "Difficulty", "varchar(255) COLLATE utf8_unicode_ci NOT NULL"),
                        new SQLProperty("CanAbandon", "CanAbandon", "tinyint(1) NOT NULL"),
                        new SQLProperty("IsHidden", "IsHidden", "tinyint(1) NOT NULL"),
                        new SQLProperty("IsClassQuest", "IsClassQuest", "tinyint(1) NOT NULL"),
                        new SQLProperty("IsBonus", "IsBonus", "tinyint(1) NOT NULL"),
                        new SQLProperty("BonusShareable", "BonusShareable", "tinyint(1) NOT NULL"),
                        new SQLProperty("CategoryId", "CategoryId", "bigint(20) NOT NULL"),
                        new SQLProperty("Category", "Category", "varchar(255) COLLATE latin1_general_cs NOT NULL"),
                        new SQLProperty("BranchCount", "BranchCount", "int(11) NOT NULL"),
                        new SQLProperty("Branches", "Branches", "TEXT NOT NULL", false, true),
                        //new SQLProperty("Items", "Items", "TEXT NOT NULL", false, true),
                        new SQLProperty("Classes", "ClassesB62", "varchar(255) COLLATE latin1_general_cs NOT NULL", false, true),
                        new SQLProperty("RewardIds", "RewardIds", "varchar(255) COLLATE latin1_general_cs NOT NULL", false, true),
                        new SQLProperty("Rewards", "Rewards", "TEXT NOT NULL", false, true),
                        new SQLProperty("CreditsRewarded", "CreditsRewarded", "int(11) NOT NULL"),
                        new SQLProperty("ReqPrivacy", "ReqPrivacy", "varchar(255) COLLATE latin1_general_cs NOT NULL"),
                        new SQLProperty("BonusMissionsIds", "BonusMissionsB62Ids", "TEXT NOT NULL", false, true),
                        //new SQLProperty("ItemMap", "ItemMap", "TEXT NOT NULL", false, true),

                    };
            }
        }
        public override string ToString(bool verbose)
        {
            string n = Environment.NewLine;
            var txtFile = new StringBuilder();

            txtFile.Append("------------------------------------------------------------" + n);
            txtFile.Append("Quest Name: " + Name + n);
            txtFile.Append("Quest NodeId: " + NodeId + n);
            txtFile.Append("Quest Id: " + Id + n);
            txtFile.Append("------------------------------------------------------------" + n);
            txtFile.Append("Quest INFO" + n);
            txtFile.Append("  IsBonus: " + IsBonus + n);
            txtFile.Append("  BonusShareable: " + BonusShareable + n);
            txtFile.Append("  Branches: " + Branches.ToList().ToString() + n);
            txtFile.Append("  CanAbandon: " + CanAbandon + n);
            txtFile.Append("  Category: " + Category + n);
            txtFile.Append("  CategoryId: " + CategoryId + n);
            txtFile.Append("  Classes: " + Classes.ToList().ToString() + n);
            txtFile.Append("  Difficulty: " + Difficulty + n);
            txtFile.Append("  Fqn: " + Fqn + n);
            txtFile.Append("  Icon: " + Icon + n);
            txtFile.Append("  IsClassQuest: " + IsClassQuest + n);
            txtFile.Append("  IsHidden: " + IsHidden + n);
            txtFile.Append("  IsRepeatable: " + IsRepeatable + n);
            txtFile.Append("  Items: " + Items + n);
            txtFile.Append("  RequiredLevel: " + RequiredLevel + n);
            txtFile.Append("  XpLevel: " + XpLevel + n);
            txtFile.Append("------------------------------------------------------------" + n + n);

            return txtFile.ToString();
        }
        #region XML
        [Newtonsoft.Json.JsonIgnore]
        public Dictionary<string, XElement> LoadedNpcs;
        [Newtonsoft.Json.JsonIgnore]
        public Dictionary<string, XElement> LoadedQuests;
        public override XElement ToXElement(bool verbose)
        {
            var questNode = new XElement("Quest", new XElement("Name", Name),
                //new XAttribute("Name", itm.Name),
                new XElement("Fqn", Fqn,
                    new XAttribute("NodeId", NodeId)),
                new XAttribute("Id", Id),
                new XElement("Category", Category,
                    new XAttribute("Id", CategoryId)),
                new XElement("RequiredLevel", RequiredLevel),
                new XElement("XpLevel", XpLevel));
            if (verbose)
            {
                //Intialize our repeat XElement holders for this quest.
                LoadedNpcs = new Dictionary<string, XElement>();
                LoadedQuests = new Dictionary<string, XElement>();

                questNode.Add(
                    //new XAttribute("Hash", itm.GetHashCode()),
                new XElement("IsBonus", IsBonus),
                new XElement("BonusShareable", BonusShareable),
                new XElement("CanAbandon", CanAbandon),
                new XElement("IsClassQuest", IsClassQuest),
                new XElement("IsHidden", IsHidden),
                new XElement("IsRepeatable", IsRepeatable),
                new XElement("Difficulty", Difficulty),
                new XElement("Icon", Icon),
                new XElement("RequiredPrivacy", ReqPrivacy));
                string classString = null;
                if (Classes != null)
                {
                    foreach (var classy in Classes)
                    {
                        classString += classy.Name + ", ";
                    }
                    if (classString != null) { classString = classString.Substring(0, classString.Length - 2); }
                }
                questNode.Add(new XElement("Classes", classString));

                XElement questItems = new XElement("Items");
                if (Items != null)
                {
                    questItems.Add(new XAttribute("id", Items.Count));
                    foreach (var item in Items)
                    {
                        if (item.Value != null)
                        {
                            questItems.Add(item.Value.ToXElement(true));
                        }
                    }
                }
                questNode.Add(questItems);


                //XElement rewards = new XElement("Rewards");
                //int r = 1;
                if (Rewards != null)
                {
                    foreach (var rewardEntry in Rewards.OrderBy(x => x.RewardItemId))
                    {
                        questNode.Add(rewardEntry.ToXElement(verbose));
                        //r++;
                    }
                }
                //questNode.Add(rewards);

                foreach (var branch in Branches)
                {
                    XElement branchNode = branch.ToXElement(verbose);
                    questNode.Add(branchNode); //add branch to branches

                }
                //Trash our repeat XElement holders
                LoadedNpcs = null;
                LoadedQuests = null;
            }
            return questNode;
        }
        public void QuestItemsGivenOrTakenToXElement(XElement questNode, List<GomLib.Models.QuestItem> givenItems, List<GomLib.Models.QuestItem> takenItems)
        {
            XElement itemsGiven = QuestItemListToXElement("ItemsGiven", givenItems);
            XElement itemsTaken = QuestItemListToXElement("itemsTaken", takenItems);

            questNode.Add(itemsGiven, itemsTaken);
        }
        private XElement QuestItemListToXElement(string elementName, List<GomLib.Models.QuestItem> Items)
        {
            XElement itemsElement = new XElement(elementName);
            if (Items != null)
            {
                if (Items.Count != 0)
                {
                    itemsElement.Add(new XAttribute("id", Items.Count));
                    for (var i = 0; i < Items.Count; i++)
                    {
                        var item = Items.ElementAt(i);
                        XElement questItem = item.ToXElement(true);
                        itemsElement.Add(questItem);
                    }
                }
            }
            return itemsElement;
        }
        #endregion
        #endregion
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
        public string Base62Id
        {
            get
            {
                return RewardItemId.ToMaskedBase62();
            }
        }
        [Newtonsoft.Json.JsonIgnore]
        public ulong RewardItemId { get; set; }
        #region Classes
        [Newtonsoft.Json.JsonIgnore]
        public ClassSpecList Classes { get; set; }
        [Newtonsoft.Json.JsonIgnore]
        internal List<string> _ClassesB62 { get; set; }
        public List<string> ClassesB62
        {
            get
            {
                if (_ClassesB62 == null)
                {
                    if (Classes == null) return new List<string>();
                    _ClassesB62 = Classes.Select(x => x.Base62Id).ToList();
                }
                return _ClassesB62;
            }
        }
        #endregion
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

        public override XElement ToXElement(bool verbose)
        {
            XElement reward = new XElement("Reward", new XAttribute("Id", Id));
            if (verbose)
            {
                reward.Add(new XElement("IsAlwaysProvided", IsAlwaysProvided),
                new XElement("UnknownNum", UnknownNum),
                new XElement("MinLevel", MinLevel),
                new XElement("MaxLevel", MaxLevel));

                XElement clas = new XElement("Classes");
                foreach (var c in Classes)
                {
                    clas.Add(new XElement("Class", c.Name, new XAttribute("Id", c.Id)));
                }
                reward.Add(clas);
            }
            reward.Add(RewardItem.ToXElement(true));
            return reward;
        }
    }
}
