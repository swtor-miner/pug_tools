using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;
using System.Xml.Linq;
using System.Xml.XPath;
using Newtonsoft.Json;

namespace GomLib.Models
{
    public class Quest : GameObject, IEquatable<Quest>
    {
        #region Properties
        internal Dictionary<object, object> TextLookup { get; set; }
        [JsonIgnore]
        public ulong NodeId { get; set; }
        public string Name { get; set; }
        [JsonConverter(typeof(LongConverter))]
        public long NameId { get; set; }
        public Dictionary<string, string> LocalizedName { get; set; }
        public string Icon { get; set; }
        public bool IsRepeatable { get; set; }
        public int RequiredLevel { get; set; }
        public int XpLevel { get; set; }
        public string Difficulty { get; set; }
        public bool CanAbandon { get; set; }
        public bool IsHidden { get; set; }
        public bool IsClassQuest { get; set; }
        public bool IsBonus { get; set; }
        public bool BonusShareable { get; set; }
        [JsonConverter(typeof(LongConverter))]
        public long CategoryId { get; set; }
        [JsonIgnore]
        public string Category { get; set; }
        public Dictionary<string, string> LocalizedCategory { get; set; }
        public List<QuestBranch> Branches { get; set; }
        public Dictionary<ulong, QuestItem> Items { get; set; }

        [JsonIgnore]
        public ClassSpecList Classes { get; set; }
        public List<QuestReward> Rewards { get; set; }
        public string ReqPrivacy { get; set; }
        internal List<Quest> _BonusMissions;
        [JsonIgnore]
        public List<Quest> BonusMissions
        {
            get
            {
                if (_BonusMissions == null)
                {
                    _BonusMissions = new List<Quest>();
                    foreach (var bonMisId in BonusMissionsIds)
                    {
                        _BonusMissions.Add(Dom_.questLoader.Load(bonMisId));
                    }
                }
                return _BonusMissions;
            }
        }
        [JsonIgnore]
        public List<ulong> BonusMissionsIds { get; set; }
        [JsonIgnore]
        public List<object> ItemMap { get; set; }
        [JsonConverter(typeof(LongConverter))]
        public long CreditRewardType { get; set; }
        public float CreditsRewarded { get; set; }
        public long XP { get; set; }
        public long SubXP { get; set; }
        public long F2PXP { get; set; }
        public long CommandXP { get; set; }
        #endregion
        #region FlatQuestProperties
        [JsonIgnore]
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
        public string HashedIcon
        {
            get
            {
                var fileId = TorLib.FileId.FromFilePath(string.Format("/resources/gfx/codex/{0}.dds", Icon));
                return string.Format("{0}_{1}", fileId.ph, fileId.sh);
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
        [JsonIgnore]
        internal List<string> BranchIds_ { get; set; }
        [JsonIgnore]
        public List<string> BranchIds
        {
            get
            {
                if (BranchIds_ == null)
                {
                    BranchIds_ = Branches.Select(x => x.Id.ToMaskedBase62()).ToList();
                }
                return BranchIds_;
            }
        }
        #endregion
        #region Rewards
        [JsonIgnore]
        internal List<string> RewardIds_ { get; set; }
        [JsonIgnore]
        public List<string> RewardIds
        {
            get
            {
                if (RewardIds_ == null)
                {
                    if (Rewards == null) return new List<string>();
                    RewardIds_ = Rewards.Select(x => x.Id.ToMaskedBase62()).ToList();
                }
                return RewardIds_;
            }
        }
        #endregion
        #region Bonus Missions
        [JsonIgnore]
        internal List<string> BonusMissionsB62Ids_ { get; set; }
        [JsonIgnore]
        public List<string> BonusMissionsB62Ids
        {
            get
            {
                if (BonusMissionsB62Ids_ == null)
                {
                    if (BonusMissionsIds == null) return new List<string>();
                    BonusMissionsB62Ids_ = BonusMissionsIds.Select(x => x.ToMaskedBase62()).ToList();
                }
                return BonusMissionsB62Ids_;
            }
        }
        #endregion
        #region Classes
        [JsonIgnore]
        internal List<string> ClassesB62_ { get; set; }
        public List<string> ClassesB62
        {
            get
            {
                if (ClassesB62_ == null)
                {
                    if (Classes == null) return new List<string>();
                    ClassesB62_ = Classes.Select(x => x.Base62Id).ToList();
                }
                return ClassesB62_;
            }
        }
        [JsonIgnore]
        public string AllowedClasses
        {
            get
            {
                if (Classes == null) return "";
                return string.Join(",", Classes.Select(x => x.Name).ToList());
            }
        }
        #endregion
        #region Conversation Rewards
        [JsonIgnore]
        public QuestAffectionGainTable ConversationGains_ { get; set; }
        public QuestAffectionGainTable ConversationGains
        {
            get
            {
                List<ulong> ConvosParsed = new List<ulong>();
                ConversationGains_ = null;
                if (ConversationGains_ == null)
                {
                    ConversationGains_ = new QuestAffectionGainTable();
                    if (References != null)
                    {
                        if (References.ContainsKey("conversationEnds"))
                        {
                            AddQuestAffectionGains("conversationEnds", ConvosParsed, this);
                        }
                        if (References.ContainsKey("conversationProgresses"))
                        {
                            AddQuestAffectionGains("conversationProgresses", ConvosParsed, this);
                        }
                        if (References.ContainsKey("conversationStarts"))
                        {
                            AddQuestAffectionGains("conversationStarts", ConvosParsed, this);
                        }
                    }
                }
                return ConversationGains_;
            }
        }

        private static void AddQuestAffectionGains(string reference, List<ulong> ConvosParsed, Quest qst)
        {
            foreach (var convoKey in qst.References[reference])
            {
                if (ConvosParsed.Contains(convoKey)) { continue; }
                var convo = qst.Dom_.conversationLoader.Load(convoKey);
                if (convo == null) { continue; }
                ConvosParsed.Add(convoKey);
                var dNodes = convo.DialogNodes.Where(x => x.IsPlayerNode).Where(x => x.AffectionRewardEvents.Count > 0);
                foreach (var dNode in dNodes)
                {
                    var affects = dNode.AffectionRewards;
                    string NodeLookupId = string.Format("{0}_{1}", convo.Base62Id, dNode.NodeId);
                    qst.ConversationGains_.NodeText.Add(NodeLookupId, dNode.LocalizedText);
                    qst.ConversationGains_.AffectionGainTable.Add(NodeLookupId, new List<QuestAffectionGain>());
                    foreach (var kvp in affects)
                    {
                        if (kvp.Key == null) continue;
                        if (!qst.ConversationGains_.Companions.ContainsKey(kvp.Key.Base62Id))
                        {
                            var tempc = kvp.Key;
                            if (tempc.LocalizedName["enMale"] == "Jaesa Willsaam")
                            {
                                if (kvp.Key.Fqn == "npc.companion.sith_warrior.jaesa_dark")
                                {
                                    tempc.LocalizedName["enMale"] = "Jaesa Willsaam (Dark)";
                                }
                                else
                                {
                                    tempc.LocalizedName["enMale"] = "Jaesa Willsaam (Light)";
                                }
                            }
                            qst.ConversationGains_.Companions.Add(kvp.Key.Base62Id, tempc);
                        }
                        QuestAffectionGain qag = new QuestAffectionGain(kvp.Key.Base62Id, kvp.Value.Key);
                        qst.ConversationGains_.AffectionGainTable[NodeLookupId].Add(qag);
                    }
                }
            }
        }
        #endregion

        #region Quest Links
        internal SortedSet<ulong> QuestsNext_ { get; set; }
        internal SortedSet<ulong> QuestsNext
        {
            get
            {
                if (QuestsNext_ == null)
                {
                    QuestsNext_ = new SortedSet<ulong>();
                    if (References != null)
                    {
                        if (References.ContainsKey("conversationEnds"))
                        {
                            foreach (var cnvId in References["conversationEnds"])
                            {
                                var cnvObj = Dom_.GetObject(cnvId);
                                if (cnvObj.References != null)
                                {
                                    if (cnvObj.References.ContainsKey("startsQuest"))
                                    {
                                        QuestsNext_ = cnvObj.References["startsQuest"];
                                    }
                                }
                            }
                        }
                    }
                }
                return QuestsNext_;
            }
        }
        internal List<string> QuestsNextB62_ { get; set; }
        public List<string> QuestsNextB62
        {
            get
            {
                if (QuestsNextB62_ == null)
                {
                    if (Classes == null) return new List<string>();
                    QuestsNextB62_ = QuestsNext.Select(x => x.ToMaskedBase62()).ToList();
                }
                return QuestsNextB62_;
            }
        }

        internal SortedSet<ulong> QuestsPrevious_ { get; set; }
        internal SortedSet<ulong> QuestsPrevious
        {
            get
            {
                if (QuestsPrevious_ == null)
                {
                    QuestsPrevious_ = new SortedSet<ulong>();
                    if (References != null)
                    {
                        if (References.ContainsKey("conversationStarts"))
                        {
                            foreach (var cnvId in References["conversationStarts"])
                            {
                                var cnvObj = Dom_.GetObject(cnvId);
                                if (cnvObj.References != null)
                                {
                                    if (cnvObj.References.ContainsKey("endsQuest"))
                                    {
                                        QuestsPrevious_ = cnvObj.References["endsQuest"];
                                    }
                                }
                            }
                        }
                    }
                }
                return QuestsPrevious_;
            }
        }
        internal List<string> QuestsPreviousB62_ { get; set; }
        public List<string> QuestsPreviousB62
        {
            get
            {
                if (QuestsPreviousB62_ == null)
                {
                    if (Classes == null) return new List<string>();
                    QuestsPreviousB62_ = QuestsPrevious.Select(x => x.ToMaskedBase62()).ToList();
                }
                return QuestsPreviousB62_;
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

            if (!(obj is Quest qst)) return false;

            return Equals(qst);
        }
        public bool Equals(Quest qst)
        {
            if (qst == null) return false;

            if (ReferenceEquals(this, qst)) return true;

            if (BonusMissionsIds != null)
            {
                if (qst.BonusMissionsIds == null)
                {
                    return false;
                }
                else
                {
                    if (!Enumerable.SequenceEqual(BonusMissionsIds, qst.BonusMissionsIds))
                        return false;
                }
            }
            if (BonusShareable != qst.BonusShareable)
                return false;
            if (Branches != null)
            {
                if (qst.Branches == null)
                {
                    return false;
                }
                else
                {
                    if (!Enumerable.SequenceEqual(Branches, qst.Branches))
                        return false;
                }
            }
            if (CanAbandon != qst.CanAbandon)
                return false;
            if (!Category.Equals(qst.Category))
            {
                return false;
            }
            if (CategoryId != qst.CategoryId)
                return false;
            if (Classes != null)
            {
                if (qst.Classes == null)
                {
                    return false;
                }
                else
                {
                    if (!Classes.Equals(qst.Classes, false))
                        return false;
                }
            }
            if (!Difficulty.Equals(qst.Difficulty))
                return false;
            if (Fqn != qst.Fqn)
                return false;
            if (Icon != qst.Icon)
                return false;
            if (Id != qst.Id)
                return false;
            if (IsBonus != qst.IsBonus)
                return false;
            if (IsClassQuest != qst.IsClassQuest)
                return false;
            if (IsHidden != qst.IsHidden)
                return false;
            if (IsRepeatable != qst.IsRepeatable)
                return false;

            var uQIComp = new DictionaryComparer<ulong, QuestItem>();
            if (!uQIComp.Equals(Items, qst.Items))
                return false;

            var ssComp = new DictionaryComparer<string, string>();
            if (!ssComp.Equals(LocalizedName, qst.LocalizedName))
                return false;

            if (Name != qst.Name)
                return false;
            if (NodeId != qst.NodeId)
                return false;
            if (ReqPrivacy != qst.ReqPrivacy)
                return false;
            if (RequiredLevel != qst.RequiredLevel)
                return false;
            if (Rewards != null)
            {
                if (qst.Rewards == null)
                {
                    return false;
                }
                else
                {
                    if (!Enumerable.SequenceEqual(Rewards, qst.Rewards))
                        return false;
                }
            }
            if (XpLevel != qst.XpLevel)
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
                        new SQLProperty("Name", "Name", "varchar(255) COLLATE utf8_unicode_ci NOT NULL", SQLPropSetting.AddIndex),
                        new SQLProperty("FrName", "LocalizedName[frMale]", "varchar(255) COLLATE utf8_unicode_ci NOT NULL", SQLPropSetting.AddIndex),
                        new SQLProperty("DeName", "LocalizedName[deMale]", "varchar(255) COLLATE utf8_unicode_ci NOT NULL", SQLPropSetting.AddIndex),
                        new SQLProperty("CleanName", "CleanName", "varchar(255) COLLATE utf8_unicode_ci NOT NULL", SQLPropSetting.AddIndex),
                        new SQLProperty("Base62Id", "Base62Id", "varchar(7) COLLATE latin1_general_cs NOT NULL", SQLPropSetting.PrimaryKey),
                        new SQLProperty("Fqn", "Fqn", "varchar(255) COLLATE utf8_unicode_ci NOT NULL"),
                        new SQLProperty("Icon", "HashedIcon", "varchar(255) COLLATE utf8_unicode_ci NOT NULL"),
                        new SQLProperty("IsRepeatable", "IsRepeatable", "tinyint(1) NOT NULL", SQLPropSetting.AddIndex),
                        new SQLProperty("RequiredLevel", "RequiredLevel", "int(11) NOT NULL", SQLPropSetting.AddIndex),
                        new SQLProperty("XpLevel", "XpLevel", "int(11) NOT NULL", SQLPropSetting.AddIndex),
                        new SQLProperty("XP", "XP", "int(11) NOT NULL"),
                        new SQLProperty("Difficulty", "Difficulty", "varchar(255) COLLATE utf8_unicode_ci NOT NULL", SQLPropSetting.AddIndex),
                        new SQLProperty("CanAbandon", "CanAbandon", "tinyint(1) NOT NULL", SQLPropSetting.AddIndex),
                        new SQLProperty("IsHidden", "IsHidden", "tinyint(1) NOT NULL", SQLPropSetting.AddIndex),
                        new SQLProperty("IsClassQuest", "IsClassQuest", "tinyint(1) NOT NULL", SQLPropSetting.AddIndex),
                        new SQLProperty("IsBonus", "IsBonus", "tinyint(1) NOT NULL", SQLPropSetting.AddIndex),
                        new SQLProperty("BonusShareable", "BonusShareable", "tinyint(1) NOT NULL", SQLPropSetting.AddIndex),
                        new SQLProperty("Category", "Category", "varchar(255) COLLATE latin1_general_cs NOT NULL", SQLPropSetting.AddIndex),
                        new SQLProperty("FrCategory", "LocalizedCategory[frMale]", "varchar(255) COLLATE utf8_unicode_ci NOT NULL", SQLPropSetting.AddIndex),
                        new SQLProperty("DeCategory", "LocalizedCategory[deMale]", "varchar(255) COLLATE utf8_unicode_ci NOT NULL", SQLPropSetting.AddIndex),
                        new SQLProperty("BranchCount", "BranchCount", "int(11) NOT NULL"),
                        new SQLProperty("Branches", "Branches", "TEXT NOT NULL", SQLPropSetting.JsonSerialize),
                        //new SQLProperty("Items", "Items", "TEXT NOT NULL", false, true),
                        new SQLProperty("Classes", "ClassesB62", "varchar(255) COLLATE latin1_general_cs NOT NULL", SQLPropSetting.JsonSerialize),
                        new SQLProperty("RewardIds", "RewardIds", "varchar(255) COLLATE latin1_general_cs NOT NULL", SQLPropSetting.JsonSerialize),
                        new SQLProperty("Rewards", "Rewards", "TEXT NOT NULL", SQLPropSetting.JsonSerialize),
                        new SQLProperty("CreditsRewarded", "CreditsRewarded", "int(11) NOT NULL"),
                        new SQLProperty("ReqPrivacy", "ReqPrivacy", "varchar(255) COLLATE latin1_general_cs NOT NULL"),
                        new SQLProperty("BonusMissionsIds", "BonusMissionsB62Ids", "TEXT NOT NULL", SQLPropSetting.JsonSerialize),
                        new SQLProperty("ConversationGains","ConversationGains", "TEXT NOT NULL", SQLPropSetting.JsonSerialize),
                        new SQLProperty("AllowedClasses", "AllowedClasses", "varchar(505) NOT NULL", SQLPropSetting.JsonSerialize, SQLPropSetting.AddFullTextIndex),
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
        [JsonIgnore]
        public Dictionary<string, XElement> LoadedNpcs;
        [JsonIgnore]
        public Dictionary<string, XElement> LoadedQuests;
        public override XElement ToXElement(bool verbose)
        {
            var questNode = new XElement("Quest", new XElement("Name", Name),
                //new XAttribute("Name", itm.Name),
                new XElement("Fqn", Fqn,
                    new XAttribute("Id", NodeId)),
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
                    questItems.Add(new XAttribute("Id", Items.Count));
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
                        if (rewardEntry.RewardItem != null)
                        {
                            questNode.Add(rewardEntry.ToXElement(verbose));
                        }
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
        public void QuestItemsGivenOrTakenToXElement(XElement questNode, List<QuestItem> givenItems, List<QuestItem> takenItems)
        {
            XElement itemsGiven = QuestItemListToXElement("ItemsGiven", givenItems);
            XElement itemsTaken = QuestItemListToXElement("itemsTaken", takenItems);

            questNode.Add(itemsGiven, itemsTaken);
        }
        private XElement QuestItemListToXElement(string elementName, List<QuestItem> Items)
        {
            XElement itemsElement = new XElement(elementName);
            if (Items != null)
            {
                if (Items.Count != 0)
                {
                    itemsElement.Add(new XAttribute("Id", Items.Count));
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
        [JsonIgnore]
        public Item RewardItem
        {
            get { return Dom_.itemLoader.Load(RewardItemId); }
            set { RewardItem = value; }
        }
        public new string Base62Id => RewardItemId.ToMaskedBase62();
        [JsonIgnore]
        public ulong RewardItemId { get; set; }
        #region Classes
        [JsonIgnore]
        public ClassSpecList Classes { get; set; }
        [JsonIgnore]
        internal List<string> ClassesB62_ { get; set; }
        public List<string> ClassesB62
        {
            get
            {
                if (ClassesB62_ == null)
                {
                    if (Classes == null) return new List<string>();
                    ClassesB62_ = Classes.Select(x => x.Base62Id).ToList();
                }
                return ClassesB62_;
            }
        }
        #endregion
        public long NumberOfItem { get; set; }
        public long MinLevel { get; set; }
        public long MaxLevel { get; set; }
        public override List<SQLProperty> SQLProperties { get => base.SQLProperties; set => base.SQLProperties = value; }

        public override bool Equals(object obj)
        {
            if (obj == null) return false;

            if (ReferenceEquals(this, obj)) return true;

            if (!(obj is QuestReward qsr)) return false;

            return Equals(qsr);
        }

        public bool Equals(QuestReward qsr)
        {
            if (qsr == null) return false;

            if (ReferenceEquals(this, qsr)) return true;

            if (Classes != null)
            {
                if (qsr.Classes == null)
                {
                    return false;
                }
                else
                {
                    if (!Classes.Equals(qsr.Classes, false))
                        return false;
                }
            }
            if (Fqn != qsr.Fqn)
                return false;
            if (Id != qsr.Id)
                return false;
            if (IsAlwaysProvided != qsr.IsAlwaysProvided)
                return false;
            if (MaxLevel != qsr.MaxLevel)
                return false;
            if (MinLevel != qsr.MinLevel)
                return false;
            if (NumberOfItem != qsr.NumberOfItem)
                return false;
            //if (this.RewardItem.Equals(qsr.RewardItem))
            //return false;
            if (RewardItemId != qsr.RewardItemId)
                return false;
            if (UnknownNum != qsr.UnknownNum)
                return false;
            return true;
        }

        public override XElement ToXElement(bool verbose)
        {
            XElement reward = new XElement("Reward", new XAttribute("Id", Id));
            if (verbose)
            {
                reward.Add(new XElement("IsAlwaysProvided", IsAlwaysProvided),
                new XElement("NumberProvided", NumberOfItem),
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

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }

        public override string ToJSON()
        {
            return base.ToJSON();
        }

        public override string ToString()
        {
            return base.ToString();
        }

        public override string ToString(bool verbose)
        {
            return base.ToString(verbose);
        }

        public override XElement ToXElement(GomObject gomItm)
        {
            return base.ToXElement(gomItm);
        }

        public override XElement ToXElement(GomObject gomItm, bool verbose)
        {
            return base.ToXElement(gomItm, verbose);
        }

        public override XElement ToXElement(ulong nodeId, DataObjectModel dom)
        {
            return base.ToXElement(nodeId, dom);
        }

        public override XElement ToXElement(ulong nodeId, DataObjectModel dom, bool verbose)
        {
            return base.ToXElement(nodeId, dom, verbose);
        }

        public override XElement ToXElement(string fqn, DataObjectModel dom)
        {
            return base.ToXElement(fqn, dom);
        }

        public override XElement ToXElement(string fqn, DataObjectModel dom, bool verbose)
        {
            return base.ToXElement(fqn, dom, verbose);
        }

        public override XElement ToXElement()
        {
            return base.ToXElement();
        }

        public override HashSet<string> GetDependencies()
        {
            return base.GetDependencies();
        }
    }
    public class QuestAffectionGain
    {
        public QuestAffectionGain(string compId, int gain)
        {
            CompanionId = compId;
            AffectionGainType = gain;
        }
        public string CompanionId { get; set; }
        public int AffectionGainType { get; set; }
    }
    public class QuestAffectionGainTable
    {
        public QuestAffectionGainTable()
        {
            Companions = new Dictionary<string, Npc>();
            NodeText = new Dictionary<string, Dictionary<string, string>>();
            AffectionGainTable = new Dictionary<string, List<QuestAffectionGain>>();
        }

        [JsonIgnore]
        public Dictionary<string, Npc> Companions { get; set; }
        [JsonIgnore]
        internal Dictionary<string, Dictionary<string, string>> CompanionsParsed_ { get; set; }
        public Dictionary<string, Dictionary<string, string>> CompanionsParsed
        {
            get
            {
                if (CompanionsParsed_ == null)
                {
                    CompanionsParsed_ = new Dictionary<string, Dictionary<string, string>>();
                    if (Companions != null)
                    {
                        foreach (var comp in Companions)
                        {
                            CompanionsParsed_.Add(comp.Key, comp.Value.LocalizedName);
                        }
                    }
                }
                return CompanionsParsed_;
            }
        }
        public Dictionary<string, Dictionary<string, string>> NodeText { get; set; }
        public Dictionary<string, List<QuestAffectionGain>> AffectionGainTable { get; set; }
    }
}
