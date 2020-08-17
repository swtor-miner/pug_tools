using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GomLib.Models;

namespace GomLib.ModelLoader
{
    public class ConversationLoader
    {
        Dictionary<ulong, Conversation> idMap;
        Dictionary<string, Conversation> nameMap;

        //Caching data for influence changes for faster access.
        Dictionary<ulong, long> CompanionSimpleNameByFqnID = new Dictionary<ulong, long>();
        Dictionary<long, ulong> CompanionFqnIDBySimpleName = new Dictionary<long, ulong>();
        Dictionary<long, KeyValuePair<int, string>> reactionDataByID = new Dictionary<long, KeyValuePair<int, string>>();
        Dictionary<long, KeyValuePair<int, Dictionary<string, string>>> localizedReactionDataByID = new Dictionary<long, KeyValuePair<int, Dictionary<string, string>>>();

        Dictionary<long, Npc> companionShortNameMap;
        Conversation GenericLines = null;

        private readonly DataObjectModel _dom;

        public ConversationLoader(DataObjectModel dom)
        {
            _dom = dom;
            Flush();
        }

        public void Flush()
        {
            idMap = new Dictionary<ulong, Conversation>();
            nameMap = new Dictionary<string, Conversation>();
            companionShortNameMap = new Dictionary<long, Npc>();
            GenericLines = null;
        }

        public string ClassName
        {
            get { return "cnvTree_Prototype"; }
        }

        public Models.Conversation Load(ulong nodeId)
        {
            if (idMap.TryGetValue(nodeId, out Conversation result))
            {
                return result;
            }

            GomObject obj = _dom.GetObject(nodeId);
            Conversation cnv = new Conversation();
            return Load(cnv, obj);
        }

        public Models.Conversation Load(string fqn)
        {
            if (nameMap.TryGetValue(fqn, out Conversation result))
            {
                return result;
            }

            GomObject obj = _dom.GetObject(fqn);
            Conversation cnv = new Conversation();
            return Load(cnv, obj);
        }

        public Models.Conversation Load(GomObject obj)
        {
            Conversation cnv = new Conversation();
            return Load(cnv, obj);
        }

        public Models.Conversation Load(Models.GameObject obj, GomObject gom)
        {
            if (gom == null) { return (Conversation)obj; }

            return Load(obj as Conversation, gom);
        }

        public Conversation Load(Conversation cnv, GomObject obj)
        {
            if (obj == null) { return null; }
            if (cnv == null) { return null; }

            cnv.Fqn = obj.Name;
            cnv.Id = obj.Id;
            cnv.Dom_ = _dom;
            cnv.References = obj.References;

            //Build a dictionary to get the simple name from the NPC FQN ID and reverse.
            //To maintain backwards compatability with pre 4.0 code.
            //This isn't technically correct as the same simple name can apply to different nodes.
            if (CompanionSimpleNameByFqnID == null || CompanionSimpleNameByFqnID.Count == 0)
            {
                if (CompanionSimpleNameByFqnID == null)
                {
                    CompanionSimpleNameByFqnID = new Dictionary<ulong, long>();
                    CompanionFqnIDBySimpleName = new Dictionary<long, ulong>();
                }

                GomObject compInfo = _dom.GetObject("chrCompanionInfo_Prototype");
                Dictionary<object, object> simpleNameByFqnID = compInfo.Data.ValueOrDefault<Dictionary<object, object>>("chrCompanionSpecToSimpleNameIdentifier", null);

                foreach (KeyValuePair<object, object> kvp in simpleNameByFqnID)
                {
                    //Build dictionary to keep the data.
                    long simpleName = (long)kvp.Value;
                    ulong npcFQNID = (ulong)kvp.Key;

                    CompanionSimpleNameByFqnID.Add(npcFQNID, simpleName);
                    CompanionFqnIDBySimpleName[simpleName] = npcFQNID;
                }
            }

            if (reactionDataByID == null || reactionDataByID.Count == 0)
            {
                if (reactionDataByID == null)
                {
                    //Faster to check than to reassign regardless.
                    reactionDataByID = new Dictionary<long, KeyValuePair<int, string>>();
                    localizedReactionDataByID = new Dictionary<long, KeyValuePair<int, Dictionary<string, string>>>();
                }

                //Not present pre 4.0 so have to check this exists. This could cause a slow down analayzing pre 4.0 clients.
                GomObject reactionNode = _dom.GetObject("cnvReactionsDataPrototype");
                if (reactionNode != null)
                {
                    Dictionary<object, object> reactionClassByID = reactionNode.Data.ValueOrDefault<Dictionary<object, object>>("cnvReactionsByID", null);
                    foreach (KeyValuePair<object, object> kvp in reactionClassByID)
                    {
                        long id = (long)kvp.Key;

                        GomObjectData reactionClass = kvp.Value as GomObjectData;
                        long sid = reactionClass.ValueOrDefault<long>("cnvReactionString", 0);
                        long influenceMod = reactionClass.ValueOrDefault<long>("cnvReactionInfluenceModifier", 0);
                        bool influenceNegative = reactionClass.ValueOrDefault("cnvReactionInfluenceNegative", false);

                        if (influenceNegative) influenceMod = -influenceMod;

                        //Get the string from the string tables.
                        StringTable reactionStb = _dom.stringTable.Find("str.gui.conversationreactions");
                        string reactionString = reactionStb.GetText(sid, "str.gui.conversationreactions");
                        Dictionary<string, string> localizedReactionString = reactionStb.GetLocalizedText(sid, "str.gui.conversationreactions");

                        //Store the data for quick lookup. Don't need the influence modifier to be long.
                        reactionDataByID.Add(id, new KeyValuePair<int, string>((int)influenceMod, reactionString));
                        localizedReactionDataByID.Add(id, new KeyValuePair<int, Dictionary<string, string>>((int)influenceMod, localizedReactionString));
                    }
                }
            }

            if (reactionDataByID.Count == 0)
            {
                //Pre 4.0. Fill this with our own data for backwards compatability sakes. Assumes level 60. English only.
                reactionDataByID.Add(-5301803456931428419, new KeyValuePair<int, string>(-480, "<<1>> greatly disapproves."));//Large decrease
                reactionDataByID.Add(-4783781478483316801, new KeyValuePair<int, string>(360, "<<1>> greatly approves."));//Large increase
                reactionDataByID.Add(-4591313547948601546, new KeyValuePair<int, string>(-90, "<<1>> disapproves."));//Medium decrease
                reactionDataByID.Add(-4560859075783160276, new KeyValuePair<int, string>(120, "<<1>> approves."));//Medium increase
                reactionDataByID.Add(-1761015921176014301, new KeyValuePair<int, string>(45, "<<1>> slightly approves."));//Small increase
                reactionDataByID.Add(-1279836118038687359, new KeyValuePair<int, string>(-3, "<<1>> slightly disapproves."));//Small decrease
            }

            //Try to get the audio for each language.
            string cnvPath = cnv.Fqn.Replace('.', '_');
            foreach (string fileGroup in _dom._assets.loadedFileGroups)
            {
                //Don't want to check for main.
                if (fileGroup == "main")
                {
                    continue;
                }

                cnv.AudioLanguageState.Add(fileGroup,
                    _dom._assets.HasFile(String.Format("/resources/{0}/bnk2/" + cnvPath + ".acb", fileGroup)));
            }

            cnv.IsKOTORStyle = obj.Data.ValueOrDefault<bool>("cnvIsKOTORStyle", false);

            var dialogNodeMap = obj.Data.ValueOrDefault<Dictionary<object, object>>("cnvTreeDialogNodes_Prototype", new Dictionary<object, object>());
            foreach (var dialogKvp in dialogNodeMap)
            {
                var dialogNode = LoadDialogNode(cnv, (GomObjectData)dialogKvp.Value);
                if (dialogNode.Stb != null)
                    if (cnv.Stb == null)
                        cnv.Stb = dialogNode.Stb;
                //else if (cnv.stb != dialogNode.stb && dialogNode.stb != "str.cnv.misc.generic_lines")
                //{
                //    throw new IndexOutOfRangeException();
                //}
                cnv.DialogNodes.Add(dialogNode);
                cnv.NodeLookup[dialogNode.NodeId] = dialogNode;
                cnv.QuestStarted.AddRange(dialogNode.QuestsGranted);
                cnv.QuestEnded.AddRange(dialogNode.QuestsEnded);
                cnv.QuestProgressed.AddRange(dialogNode.QuestsProgressed);
            }

            var treeRootNodeMap = obj.Data.ValueOrDefault<object>("cnvTreeRootNode_Prototype", new object());
            var rootNodeList = ((GomObjectData)treeRootNodeMap).ValueOrDefault<List<object>>("cnvChildNodes", new List<object>());
            for (int i = 0; i < rootNodeList.Count; i++)
            {
                long childNodeId = (long)rootNodeList[i];
                cnv.RootNodes.Add(i, childNodeId);
            }

            var treeLinkNodeMap = obj.Data.ValueOrDefault<Dictionary<object, object>>("cnvTreeLinkNodes_Prototype", new Dictionary<object, object>());
            foreach (var nodeLinkKvp in treeLinkNodeMap)
            {
                long target = (long)((GomObjectData)nodeLinkKvp.Value).ValueOrDefault<long>("cnvLinkTarget", 0);
                cnv.NodeLinkList.Add((long)nodeLinkKvp.Key, target);
            }

            cnv.DefaultSpeakerId = obj.Data.ValueOrDefault<ulong>("cnvDefaultSpeaker", 0);
            if (!cnv.SpeakersIds.Contains(cnv.DefaultSpeakerId) && cnv.DefaultSpeakerId != 0)
            {
                cnv.SpeakersIds.Add(cnv.DefaultSpeakerId); //, LoadSpeaker(cnv.DefaultSpeakerId));
            }

            cnv.LocalizedName = new Dictionary<string, string>() {
                { "enMale", obj.Name },
                { "frMale", obj.Name },
                { "deMale", obj.Name }
            };
            obj.Unload();
            return cnv;
        }

        public GameObject LoadSpeaker(ulong id)
        {
            GomObject speakerObject = _dom.GetObject(id);
            if (speakerObject != null)
            {
                switch (speakerObject.Name.Substring(0, 3))
                {
                    case "npc":
                        Npc defNpc = new Npc();
                        return _dom.npcLoader.Load(defNpc, speakerObject);
                    case "plc":
                        Placeable defPlc = new Placeable();
                        return _dom.placeableLoader.Load(defPlc, speakerObject);
                    default:
                        throw new Exception("Unaccounted for speaker type");
                }
            }
            return null;
        }

        public Npc CompanionBySimpleNameId(long nameId)
        {
            LoadcompanionShortNameMap();
            if (companionShortNameMap.ContainsKey(nameId))
                return companionShortNameMap[nameId];
            else
                return null;
        }

        public string CompanionB62BySimpleNameId(long nameId)
        {
            LoadcompanionShortNameMap();
            if (companionShortNameMap.ContainsKey(nameId))
                return companionShortNameMap[nameId].Id.ToMaskedBase62();
            else
                return null;
        }

        private void LoadcompanionShortNameMap()
        {
            if (companionShortNameMap.Count == 0)
            {
                foreach (var kvp in CompanionFqnIDBySimpleName)
                {
                    var simpleNameId = (long)kvp.Key;
                    Npc npc = _dom.npcLoader.Load((ulong)kvp.Value);
                    companionShortNameMap[simpleNameId] = npc;
                }
            }
        }

        DialogNode LoadDialogNode(Conversation cnv, GomObjectData data)
        {
            DialogNode result = new DialogNode
            {
                Conversation = cnv,
                NodeId = data.Get<long>("cnvNodeNumber"),
                MinLevel = (int)data.ValueOrDefault<long>("cnvLevelConditionMin", -1),
                MaxLevel = (int)data.ValueOrDefault<long>("cnvLevelConditionMax", -1),
                IsEmpty = data.ValueOrDefault<bool>("cnvIsEmpty", false),
                IsAmbient = data.ValueOrDefault<bool>("cnvIsAmbient", false),
                JoinDisabledForHolocom = data.ValueOrDefault<bool>("cnvIsJoinDisabledForHolocom", false),
                ChoiceDisabledForHolocom = data.ValueOrDefault<bool>("cnvIsVoteWinDisabledForHolocom", false),
                AbortsConversation = data.ValueOrDefault<bool>("cnvAbortConversation", false),
                IsPlayerNode = data.ValueOrDefault<bool>("cnvIsPcNode", false),
                GenericNodeNumber = data.ValueOrDefault<long>("cnvGenericNodeNumber", 0),
                CnvAlienVOFQN = data.ValueOrDefault<string>("cnvAlienVOConvoFQN", string.Empty),
                CnvAlienVONode = data.ValueOrDefault<long>("cnvAlienVONodeNumber", -1),

                ActionHook = data.ValueOrDefault<string>("cnvActionHook", null)
            };

            // Load Alignment Results
            long alignmentAmount = data.ValueOrDefault<long>("cnvRewardForceAmount", 0);
            if (alignmentAmount != 0)
            {
                string forceType = data.ValueOrDefault<ScriptEnum>("cnvRewardForceType", new ScriptEnum()).ToString();
                result.AlignmentGain = ConversationAlignmentExtensions.ToConversationAlignment(alignmentAmount, forceType);
            }

            // Load Companion Affection Results pre 4.0 system.
            var affectionGains = data.ValueOrDefault<Dictionary<object, object>>("cnvRewardAffectionRewards", null);
            result.AffectionRewardEvents = new Dictionary<long, KeyValuePair<int, string>>();
            result.AffectionRewardEventsB62 = new Dictionary<string, KeyValuePair<int, Dictionary<string, string>>>();
            if (result.Conversation.AffectionNpcB62 == null)
                result.Conversation.AffectionNpcB62 = new HashSet<string>();
            if (result.Conversation.AffectionNcoB62 == null)
                result.Conversation.AffectionNcoB62 = new HashSet<string>();
            if (affectionGains != null)
            {
                foreach (var companionGain in affectionGains)
                {
                    //Get NPC object for companion.
                    long companionShortNameId = (long)companionGain.Key;
                    Npc companion = CompanionBySimpleNameId(companionShortNameId);
                    if (companion != null)
                    {
                        //Build reaction string and make sure the reaction id is valid.
                        if (reactionDataByID.TryGetValue((long)companionGain.Value, out KeyValuePair<int, string> reactionData))
                        {
                            string reactionString = reactionData.Value.Replace("<<1>>", companion.Name);

                            result.AffectionRewardEvents[companionShortNameId] = new KeyValuePair<int, string>(reactionData.Key, reactionString);
                            string b62 = companion.Id.ToMaskedBase62();
                            result.Conversation.AffectionNpcB62.Add(b62);
                            if (localizedReactionDataByID.Count > 0)
                                result.AffectionRewardEventsB62[b62] = new KeyValuePair<int, Dictionary<string, string>>(reactionData.Key, localizedReactionDataByID[(long)companionGain.Value].Value);
                        }
                    }
                }
            }

            //Load Companion affection results post 4.0 system.
            List<object> affectionGainList = data.ValueOrDefault<List<object>>("cnvRewardAffectionRewardsList", null);
            if (affectionGainList != null)
            {
                foreach (object companionRewardObj in affectionGainList)
                {
                    //Get the data.
                    GomObjectData companionRewardData = companionRewardObj as GomObjectData;
                    ulong ncoFQN = companionRewardData.ValueOrDefault<ulong>("cnvRewardAffectionRewardNCOID", 0);
                    long rewardID = companionRewardData.ValueOrDefault<long>("cnvRewardAffectionRewardID", 0);

                    GomObject companionObj = _dom.GetObject(ncoFQN);
                    if (companionObj != null)
                    {
                        if (companionObj.Name.StartsWith("nco."))
                        {
                            //Load the NCO data.
                            NewCompanion nco = _dom.newCompanionLoader.Load(companionObj);

                            //Make sure the reward id is valid.
                            if (reactionDataByID.TryGetValue(rewardID, out KeyValuePair<int, string> rewardData))
                            {
                                //Replace the token with companion name.
                                string reactionString = rewardData.Value.Replace("<<1>>", nco.Name);

                                //Use simple name to maintain pre 4.0 compatability.
                                if (CompanionSimpleNameByFqnID.TryGetValue(nco.NpcId, out long simpleName))
                                {
                                    result.AffectionRewardEvents[simpleName] = new KeyValuePair<int, string>(rewardData.Key, reactionString);
                                    string b62 = nco.Id.ToMaskedBase62();
                                    result.Conversation.AffectionNcoB62.Add(b62);
                                    result.AffectionRewardEventsB62[b62] = new KeyValuePair<int, Dictionary<string, string>>(Math.Abs(rewardData.Key), localizedReactionDataByID[rewardID].Value);
                                }
                            }
                        }
                        else
                        {
                            //NPC node.. wtf Bioware?
                            //Load the NPC node.
                            Npc npc = _dom.npcLoader.Load(companionObj);

                            //Make sure the reward id is valid.
                            if (reactionDataByID.TryGetValue(rewardID, out KeyValuePair<int, string> rewardData))
                            {
                                //Replace the token with companion name.
                                string reactionString = rewardData.Value.Replace("<<1>>", npc.Name);

                                //Use simple name to maintain pre 4.0 compatability.
                                if (CompanionSimpleNameByFqnID.TryGetValue(npc.Id, out long simpleName))
                                {
                                    result.AffectionRewardEvents[simpleName] = new KeyValuePair<int, string>(rewardData.Key, reactionString);
                                    string b62 = npc.Id.ToMaskedBase62();
                                    result.Conversation.AffectionNpcB62.Add(b62);
                                    result.AffectionRewardEventsB62[b62] = new KeyValuePair<int, Dictionary<string, string>>(Math.Abs(rewardData.Key), localizedReactionDataByID[rewardID].Value);
                                }
                                else
                                {
                                    result.AffectionRewardEvents[0] = new KeyValuePair<int, string>(rewardData.Key, reactionString);
                                    string b62 = npc.Id.ToMaskedBase62();
                                    result.Conversation.AffectionNpcB62.Add(b62);
                                    result.AffectionRewardEventsB62[b62] = new KeyValuePair<int, Dictionary<string, string>>(Math.Abs(rewardData.Key), localizedReactionDataByID[rewardID].Value);
                                }

                            }
                        }
                    }
                }
            }

            // Get Speaker
            result.SpeakerId = data.ValueOrDefault<ulong>("cnvSpeaker", 0);
            if (!cnv.SpeakersIds.Contains(result.SpeakerId) && result.SpeakerId != 0)
            {
                cnv.SpeakersIds.Add(result.SpeakerId); //, LoadSpeaker(result.SpeakerId));
            }

            // Get Text
            var textMap = data.Get<Dictionary<object, object>>("locTextRetrieverMap");
            if (!textMap.ContainsKey(result.NodeId)) { result.Text = null; }
            //if (result.IsEmpty) { result.Text = null; }
            else
            {
                GomObjectData txtData = (GomObjectData)textMap[(long)result.NodeId];
                result.Stb = txtData.Get<string>("strLocalizedTextRetrieverBucket");
                result.Text = _dom.stringTable.TryGetString(cnv.Fqn, txtData);
                result.LocalizedText = _dom.stringTable.TryGetLocalizedStrings(cnv.Fqn, txtData);
                result.LocalizedOptionText = _dom.stringTable.TryGetLocalizedOptionStrings(cnv.Fqn, txtData);
            }

            if (result.GenericNodeNumber != 0)
            {
                if (GenericLines == null)
                    GenericLines = _dom.conversationLoader.Load("cnv.misc.generic_lines");
                if (GenericLines != null)
                {
                    if (GenericLines.NodeLookup.TryGetValue(result.GenericNodeNumber, out DialogNode d))
                    {
                        result.Stb = d.Stb;
                        result.Text = d.Text;
                        result.LocalizedText = d.LocalizedText;
                        //result.LocalizedOptionText = d.LocalizedOptionText;
                    }
                }
            }

            result.ChildIds = new List<int>();
            foreach (long childId in data.Get<List<object>>("cnvChildNodes"))
            {
                result.ChildIds.Add((int)childId);
            }

            // Load Quests
            var actionQuest = data.ValueOrDefault<ulong>("cnvActionQuest", 0);
            if (actionQuest > 0)
            {
                //result.ActionQuest = QuestLoader.Load(actionQuest);
                result.ActionQuest = actionQuest;
            }

            var questReward = data.ValueOrDefault<ulong>("cnvRewardQuest", 0);
            if (questReward > 0)
            {
                //result.QuestReward = QuestLoader.Load(questReward);
                result.QuestReward = questReward;
            }

            var questGrants = data.Get<Dictionary<object, object>>("cnvNodeQuestGrants");
            //result.QuestsGranted = new List<Quest>();
            result.QuestsGranted = new List<ulong>();
            foreach (var grant in questGrants)
            {
                if ((bool)grant.Value)
                {
                    //Quest q = QuestLoader.Load((ulong)grant.Key);
                    //result.QuestsGranted.Add(q);
                    result.QuestsGranted.Add((ulong)grant.Key);
                }
            }

            var questEnds = data.Get<Dictionary<object, object>>("cnvNodeQuestEnds");
            //result.QuestsEnded = new List<Quest>();
            result.QuestsEnded = new List<ulong>();
            foreach (var ends in questEnds)
            {
                if ((bool)ends.Value)
                {
                    //Quest q = QuestLoader.Load((ulong)ends.Key);
                    //result.QuestsEnded.Add(q);
                    result.QuestsEnded.Add((ulong)ends.Key);
                }
            }

            var questProgress = data.Get<Dictionary<object, object>>("cnvNodeQuestProgress");
            //result.QuestsProgressed = new List<Quest>();
            result.QuestsProgressed = new List<ulong>();
            foreach (var prog in questProgress)
            {
                if ((bool)prog.Value)
                {
                    //Quest q = QuestLoader.Load((ulong)prog.Key);
                    //result.QuestsProgressed.Add(q);
                    result.QuestsProgressed.Add((ulong)prog.Key);
                }
            }

            return result;
        }
    }
}
