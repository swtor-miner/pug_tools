using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GomLib.Models;

namespace GomLib.ModelLoader
{
    public class QuestLoader
    {
        const long strOffset = 0x35D0200000000;

        Dictionary<ulong, Quest> idMap;
        Dictionary<string, Quest> nameMap;

        Dictionary<object, object> fullQuestRewardsTable;
        Dictionary<long, Dictionary<long, float>> fullCreditRewardsTable;
        Dictionary<long, long> experienceTable;
        Dictionary<QuestDifficulty, float> experienceDifficultyMultiplierTable;

        DataObjectModel _dom;

        public QuestLoader(DataObjectModel dom)
        {
            _dom = dom;
            Flush();
        }

        public void Flush()
        {
            idMap = new Dictionary<ulong, Quest>();
            nameMap = new Dictionary<string, Quest>();
            fullQuestRewardsTable = new Dictionary<object, object>();
            fullCreditRewardsTable = new Dictionary<long, Dictionary<long, float>>();
            experienceTable = new Dictionary<long, long>();
            experienceDifficultyMultiplierTable = new Dictionary<QuestDifficulty, float>();
        }

        public string ClassName
        {
            get { return "qstQuestDefinition"; }
        }

        public Models.Quest Load(ulong nodeId)
        {
            Quest result;
            if (idMap.TryGetValue(nodeId, out result))
            {
                return result;
            }

            GomObject obj = _dom.GetObject(nodeId);
            Quest qst = new Quest();
            return Load(qst, obj);
        }

        public Models.Quest Load(string fqn)
        {
            Quest result;
            if (nameMap.TryGetValue(fqn, out result))
            {
                return result;
            }

            GomObject obj = _dom.GetObject(fqn);
            Quest qst = new Quest();
            return Load(qst, obj);
        }

        public Models.Quest Load(GomObject obj)
        {
            Quest qst = new Quest();
            return Load(qst, obj);
        }

        public Models.GameObject CreateObject()
        {
            return new Models.Quest();
        }

        public Models.Quest Load(Models.GameObject obj, GomObject gom)
        {
            if (gom == null) { return (Quest)obj; }

            return Load(obj as Quest, gom);
        }

        public Models.Quest Load(Models.Quest qst, GomObject obj)
        {
            if (obj == null) { return qst; }
            if (qst == null) { return null; }

            qst.Fqn = obj.Name;
            qst.NodeId = obj.Id;
            qst._dom = _dom;
            qst.References = obj.References;
            var textMap = (Dictionary<object, object>)obj.Data.ValueOrDefault<Dictionary<object, object>>("locTextRetrieverMap", null);
            qst.TextLookup = textMap;

            long questGuid = obj.Data.ValueOrDefault<long>("qstQuestDefinitionGUID", 0);
            //qst.Id = (ulong)(questGuid >> 32);
            qst.Id = obj.Id;
            qst.RequiredLevel = (int)obj.Data.ValueOrDefault<long>("qstReqMinLevel", 0);
            qst.IsRepeatable = obj.Data.ValueOrDefault<bool>("qstIsRepeatable", false);
            qst.XpLevel = (int)obj.Data.ValueOrDefault<long>("qstXpLevel", 0);
            qst.Difficulty = QuestDifficultyExtensions.ToQuestDifficulty((ScriptEnum)obj.Data.ValueOrDefault<ScriptEnum>("qstDifficulty", null));
            qst.ReqPrivacy = obj.Data.ValueOrDefault<object>("qstReqPrivacy", (object)"").ToString().Replace("qstPrivacy", "");
            qst.CanAbandon = obj.Data.ValueOrDefault<bool>("qstAllowAbandonment", false);
            qst.Icon = obj.Data.ValueOrDefault<string>("qstMissionIcon", "").Replace(" ", "");
            qst.IsHidden = obj.Data.ValueOrDefault<bool>("qstIsHiddenQuest", false);
            qst.IsClassQuest = obj.Data.ValueOrDefault<bool>("qstIsClassQuest", false);
            qst.IsBonus = obj.Data.ValueOrDefault<bool>("qstIsBonusQuest", false);
            qst.BonusShareable = obj.Data.ValueOrDefault<bool>("qstIsBonusQuestShareable", false);
            qst.CategoryId = obj.Data.ValueOrDefault<long>("qstCategoryDisplayName", 0);
            //if (qst.CategoryId == 2466269005611293) { throw new IndexOutOfRangeException(); } // enable/disable to catch the data in the qst variable when QuestCategory.ToQuestCategory throws it's exception. To figure out which category it belongs to.
            StringTable Categories = _dom.stringTable.Find("str.gui.questcategories");

            qst.Category = Categories.GetText(qst.CategoryId, "str.gui.questcategories"); //QuestCategoryExtensions.ToQuestCategory(qst.CategoryId);

            qst.ItemMap = (List<object>)obj.Data.ValueOrDefault<List<object>>("qstItemVariableDefinition_ProtoVarList", null);
            qst.Items = LoadItems(qst.ItemMap);
            
            LoadRewards(ref qst, obj);
            LoadBranches(ref qst, obj);

            var bools = (List<object>)obj.Data.ValueOrDefault<List<object>>("qstSimpleBoolVariableDefinition_ProtoVarList", null);
            var strings = (List<object>)obj.Data.ValueOrDefault<List<object>>("qstStringIdVariableDefinition_ProtoVarList", null);
            LoadRequiredClasses(qst, obj);

            qst.NameId = questGuid + 0x58;
            var nameLookup = (GomObjectData)textMap[qst.NameId];
            qst.Name = _dom.stringTable.TryGetString(qst.Fqn, nameLookup);
            qst.LocalizedName = _dom.stringTable.TryGetLocalizedStrings(qst.Fqn, nameLookup);

/*            List<string> strings2 = new List<string>();
            foreach (var key in textMap.Keys)
            {
                var lookup = (GomObjectData)textMap[key];
                var tempstring = _dom.stringTable.TryGetString(qst.Fqn, lookup);
                strings2.Add(tempstring);
            }
 */

            if (qst.Name.StartsWith("CUT", StringComparison.InvariantCulture))
            {
                qst.IsHidden = true;
            }

            _dom._assets.icons.AddCodex(qst.Icon);

            obj.Unload(); // These GomObjects are staying loaded in memory and cause our massive memory issues.
            return qst;
        }

        private Dictionary<ulong, QuestItem> LoadItems(List<object> items)
        {
            var parsedItems = new Dictionary<ulong, QuestItem>();
            if (items != null)
            {
                items.ConvertAll(x => (GomObjectData)x);
                var parsedItem = new QuestItem();
                parsedItem._dom = _dom;
                foreach (GomObjectData item in items)
                {
                    parsedItem.Id = item.ValueOrDefault<ulong>("qstItemSpecId", 0);
                    parsedItem.Fqn = _dom.GetStoredTypeName(parsedItem.Id);

                    parsedItem.Name = item.ValueOrDefault<string>("qstVariableName", "");
                    parsedItem.GUID = item.ValueOrDefault<long>("qstVariableGUID", 0);
                    parsedItem.VariableId = item.ValueOrDefault<ulong>("qstVariableFqnId", 0);

                    parsedItem.MaxCount = item.ValueOrDefault<long>("qstMaxCount", 0);
                    parsedItem.Min = item.ValueOrDefault<long>("qstItemMin", 0);
                    parsedItem.Max = item.ValueOrDefault<long>("qstItemMax", 0);

                    parsedItems.Add(parsedItem.VariableId, parsedItem);
                    /*if (obj != null)
                    {
                        switch(obj.Name.Substring(0, 3))
                        {
                            case "itm":
                                Item itm = new Item();
                                ItemLoader.Load(itm, obj);
                                parsedItems.Add(itm);
                                break;
                            case "npc":
                                Npc npc = new Npc();
                                NpcLoader.Load(npc, obj);
                                parsedItems.Add(npc);
                                break;
                            case "abl":
                                Ability abl = new Ability();
                                AbilityLoader.Load(abl, obj);
                                parsedItems.Add(abl);
                                break;
                            case "spn":
                                break; //do nothing
                            default:
                                throw new NotImplementedException();
                        }
                    }*/
                }
            }
            return parsedItems;
        }

        private void LoadRequiredClasses(Quest qst, GomObject obj)
        {
            qst.Classes = new ClassSpecList();
            var reqClasses = (Dictionary<object, object>)obj.Data.ValueOrDefault<Dictionary<object, object>>("qstReqClasses", null);
            if (reqClasses != null)
            {
                foreach (var kvp in reqClasses)
                {
                    var spec = _dom.classSpecLoader.Load((ulong)kvp.Key);
                    var enabled = (bool)kvp.Value;
                    if (enabled)
                    {
                        qst.Classes.Add(spec);
                    }
                }
            }
        }

        private void LoadRewards(ref Quest qst, GomObject obj)
        {
            if (fullQuestRewardsTable.Count == 0)
            {
                fullQuestRewardsTable = _dom.GetObject("qstRewardsInfoPrototype").Data.Get<Dictionary<object, object>>("qstRewardsInfoData");
                fullCreditRewardsTable = _dom.GetObject("qstrewardscreditsData").Data.Get<Dictionary<object, object>>("qstRewardsPerLevelData")
                    .ToDictionary(x=> (long)x.Key, x => ((Dictionary<object, object>)x.Value).ToDictionary(y => (long)y.Key, y => (float)y.Value));
                experienceTable = _dom.GetObject("qstExperiencePrototype").Data.Get<Dictionary<object, object>>("qstExperienceTable")
                    .ToDictionary(x => (long)x.Key, x => (long)x.Value);
                var qstExperienceMultiplierPrototype = _dom.GetObject("qstExperienceMultiplierPrototype");
                experienceDifficultyMultiplierTable = qstExperienceMultiplierPrototype.Data.ValueOrDefault<Dictionary<object, object>>("qstExperienceMultiplierTable", new Dictionary<object, object>())
                    .ToDictionary(x=> QuestDifficultyExtensions.ToQuestDifficulty((ScriptEnum)x.Key), x=> (float)x.Value);
                // Subscriber XP: base xp * difficulty multiplier * (1.2853 - level * .0012)
                // F2P XP: base xp * difficulty multiplier * (1.2573 - level * .0012)
            }
            //try
            //{
            var questRewardsTable = new List<object>();

            if (fullQuestRewardsTable.ContainsKey(obj.Id))
            {
                questRewardsTable = (List<object>)fullQuestRewardsTable[obj.Id];
            }
            else
            {
                return;
            }
            qst.Rewards = new List<QuestReward>();

            foreach (GomObjectData qReward in questRewardsTable)
            {
                var reward = new QuestReward();
                reward._dom = _dom;

                long unknownNum = qReward.Get<long>("4611686090803470052");
                /*if (unknownNum != 1) //obsolete debugging code
                {
                    var t = ""; //checking if always 1. Order of appearance in listing?!?
                }*/
                reward.UnknownNum = unknownNum;

                bool isAlwaysProvided = qReward.ValueOrDefault<bool>("qstRewardIsAlwaysProvided", false);
                reward.IsAlwaysProvided = isAlwaysProvided;

                GomObjectData rewardLookup = qReward.Get<GomObjectData>("qstRewardData");

                /*Item rewardItem = new Item(); 
                rewardItem = _dom.itemLoader.Load(rewardLookup.ValueOrDefault<ulong>("qstRewardItemId", 0));
                reward.RewardItem = rewardItem;*/
                reward.RewardItemId = rewardLookup.ValueOrDefault<ulong>("qstRewardItemId", 0);

                Dictionary<object, object> classes = rewardLookup.ValueOrDefault<Dictionary<object, object>>("qstRewardRequiredClasses", null);
                reward.Classes = new ClassSpecList();
                foreach (var classLookup in classes)
                {
                    ClassSpec classy = _dom.classSpecLoader.Load((ulong)classLookup.Key);
                    reward.Classes.Add(classy);
                    /*if (!(bool)classLookup.Value)//obsolete debugging code
                    {
                        var t = "";
                    }*/
                }

                long numberOfItem = rewardLookup.Get<long>("qstRewardItemQty");
                reward.NumberOfItem = numberOfItem;

                long minLevel = rewardLookup.Get<long>("qstRewardMinLevel");
                reward.MinLevel = minLevel;

                long maxLevel = rewardLookup.Get<long>("qstRewardMaxLevel");
                reward.MaxLevel = maxLevel;

                reward.Id = Convert.ToUInt64(reward.IsAlwaysProvided) + reward.RewardItemId + Convert.ToUInt64(reward.NumberOfItem);
                qst.Rewards.Add(reward);
            }
            qst.CreditRewardType = obj.Data.ValueOrDefault<long>("creditRewardType", 0);
            if (qst.CreditRewardType != 0)
            {
                qst.CreditsRewarded = fullCreditRewardsTable[qst.CreditRewardType][qst.XpLevel];
            }
            if (qst.XpLevel != 0)
            {
                // Subscriber XP: base xp * difficulty multiplier * (1.2853 - level * .0012)
                // F2P XP: base xp * difficulty multiplier * (1.2573 - level * .0012)
                float diffMulti = experienceDifficultyMultiplierTable[qst.Difficulty];
                if(qst.IsClassQuest)
                    qst.SubXP = (long)Math.Round(12 * experienceTable[qst.XpLevel] * diffMulti * (1.2853 - qst.XpLevel * .0012));
                else
                    qst.SubXP = (long)Math.Round(experienceTable[qst.XpLevel] * diffMulti * (1.2853 - qst.XpLevel * .0012));
                qst.F2PXP = (long)Math.Round(experienceTable[qst.XpLevel] * diffMulti * (1.2573 - qst.XpLevel * .0012));
                qst.XP = (long)Math.Round(experienceTable[qst.XpLevel] * diffMulti);
            }
        }

        private void LoadBranches(ref Quest qst, GomObject obj)
        {
            var branches = (List<object>)obj.Data.ValueOrDefault<List<object>>("qstBranches", null);
            qst.Branches = new List<QuestBranch>();
            if (branches != null)
            {
                foreach (var br in branches)
                {
                    var branch = _dom.questBranchLoader.Load((GomObjectData)br, qst);
                    branch.Quest = qst;
                    qst.Branches.Add(branch);
                }
            }
        }

        public List<ulong> LoadBonusMissions(List<object> bonuses)
        {
            var bonusMissions = new List<ulong>();
            if (bonuses != null)
            {
                foreach (var bonus in bonuses)
                {
                    var bonusMissionId = ((GomObjectData)bonus).ValueOrDefault<ulong>("qstTaskBonusMissionNodeId", 0);
                    /*var bonusMission = _dom.questLoader.Load(bonusMissionId);
                    if (bonusMission.Fqn != null)
                    {
                        bonusMissions.Add(bonusMission);
                    }*/
                    bonusMissions.Add(bonusMissionId);
                }
            }
            return bonusMissions;
        }

        public List<QuestItem> LoadGivenOrTakenItems(Quest qst, List<object> items)
        {
            var itemsGivenOrTaken = new List<QuestItem>();

            if (items.Count > 0)
            {
                foreach (var item in items)
                {
                    //var itemObj = DataObjectModel.GetObject((ulong)item);
                    if (qst.Items.ContainsKey((ulong)item)) //(itemObj == null)
                    {
                        if (qst.ItemMap == null)
                        {
                            Console.WriteLine(qst.Fqn + " 's item Map is null. But the quest has items. Test Quest?");
                            continue;
                        }
                        itemsGivenOrTaken.Add(qst.Items[(ulong)item]);
                        /*var itemId = qst.ItemMap.Where(x => (ulong)((GomObjectData)x).Dictionary["qstVariableFqnId"] == (ulong)item)
                            .Select(y => ((GomObjectData)y).ValueOrDefault<ulong>("qstItemSpecId", 0));

                        if (itemId.Count() == 1 && itemId.Single() != 0 && qst.Items.Count > 0)
                        {
                            var potentialItems = qst.Items.Where(x => x.GetType() == typeof(GomLib.Models.Item))
                                .Where(x => ((Item)x).NodeId == itemId.Single());
                            if (potentialItems.Count() == 1)
                            {
                                var parsedItem = (Item)potentialItems.Single();
                                itemsGivenOrTaken.Add(parsedItem);
                            }
                            else if (itemId.Count() > 1)
                            {
                                throw new IndexOutOfRangeException();
                            }

                            var potentialNpcs = qst.Items.Where(x => x.GetType() == typeof(GomLib.Models.Npc))
                                .Where(x => ((Npc)x).NodeId == itemId.Single());
                            if (potentialNpcs.Count() == 1)
                            {
                                var parsedNpc = (Npc)potentialNpcs.Single();
                                itemsGivenOrTaken.Add(parsedNpc);
                            }
                            else if (itemId.Count() > 1)
                            {
                                throw new IndexOutOfRangeException();
                            }

                            var potentialAbilities = qst.Items.Where(x => x.GetType() == typeof(GomLib.Models.Ability))
                                .Where(x => ((Ability)x).NodeId == itemId.Single());
                            if (potentialAbilities.Count() == 1)
                            {
                                var parsedAbility = (Ability)potentialAbilities.Single();
                                itemsGivenOrTaken.Add(parsedAbility);
                            }
                            else if (itemId.Count() > 1)
                            {
                                throw new IndexOutOfRangeException();
                            }
                        }
                        else if (itemId.Count() > 1)
                        {
                            throw new IndexOutOfRangeException();
                        }
                        else
                        {
                            Console.WriteLine("Test Quest?");
                        }*/
                    }
                    else
                    {
                        /*var itemObj = DataObjectModel.GetObject((ulong)item);
                        Item itm = new Item();
                        ItemLoader.Load(itm, itemObj);*/
                        var questItem = new QuestItem();
                        questItem._dom = _dom;
                        questItem.Id = (ulong)item;
                        questItem.Fqn = _dom.GetStoredTypeName(questItem.Id);

                        questItem.Name = "item";
                        questItem.VariableId = (ulong)item; ;

                        questItem.MaxCount = 1;
                        questItem.Min = 1;
                        questItem.Max = 1;
                        itemsGivenOrTaken.Add(questItem);
                    }
                }
            }
            return itemsGivenOrTaken;
        }

        public void LoadObject(Models.GameObject loadMe, GomObject obj)
        {
            GomLib.Models.Quest qst = (Models.Quest)loadMe;
            Load(qst, obj);
        }

        public void LoadReferences(Models.GameObject obj, GomObject gom)
        {
            // No references to load
        }
    }
}
