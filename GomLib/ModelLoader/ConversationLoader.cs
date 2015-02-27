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

        Dictionary<long, Npc> companionShortNameMap;

        private DataObjectModel _dom;

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
        }

        public string ClassName
        {
            get { return "cnvTree_Prototype"; }
        }

        public Models.Conversation Load(ulong nodeId)
        {
            Conversation result;
            if (idMap.TryGetValue(nodeId, out result))
            {
                return result;
            }

            GomObject obj = _dom.GetObject(nodeId);
            Conversation cnv = new Conversation();
            return Load(cnv, obj);
        }

        public Models.Conversation Load(string fqn)
        {
            Conversation result;
            if (nameMap.TryGetValue(fqn, out result))
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
            cnv._dom = _dom;
            cnv.References = obj.References;

            var dialogNodeMap = obj.Data.ValueOrDefault<Dictionary<object, object>>("cnvTreeDialogNodes_Prototype", new Dictionary<object,object>());
            foreach (var dialogKvp in dialogNodeMap)
            {
                var dialogNode = LoadDialogNode(cnv, (GomObjectData)dialogKvp.Value);
                if (dialogNode.stb != null)
                    if (cnv.stb == null)
                        cnv.stb = dialogNode.stb;
                    else if (cnv.stb != dialogNode.stb)
                    {
                        throw new IndexOutOfRangeException();
                    }
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

        Npc CompanionBySimpleNameId(long nameId)
        {
            if (companionShortNameMap.Count == 0)
            {
                var cmpInfo = _dom.GetObject("chrCompanionInfo_Prototype");
                var chrCompanionSimpleNameToSpec = cmpInfo.Data.Get<Dictionary<object, object>>("chrCompanionSimpleNameToSpec");
                foreach (var kvp in chrCompanionSimpleNameToSpec)
                {
                    var simpleNameId = (long)kvp.Key;
                    Npc npc = _dom.npcLoader.Load((ulong)kvp.Value);
                    companionShortNameMap[simpleNameId] = npc;
                }
            }
            if (companionShortNameMap.ContainsKey(nameId))
                return companionShortNameMap[nameId];
            else
                return null;
        }

        DialogNode LoadDialogNode(Conversation cnv, GomObjectData data)
        {
            DialogNode result = new DialogNode();

            result.Conversation = cnv;
            result.NodeId = data.Get<long>("cnvNodeNumber");
            result.MinLevel = (int)data.ValueOrDefault<long>("cnvLevelConditionMin", -1);
            result.MaxLevel = (int)data.ValueOrDefault<long>("cnvLevelConditionMax", -1);
            result.IsEmpty = data.ValueOrDefault<bool>("cnvIsEmpty", false);
            result.IsAmbient = data.ValueOrDefault<bool>("cnvIsAmbient", false);
            result.JoinDisabledForHolocom = data.ValueOrDefault<bool>("cnvIsJoinDisabledForHolocom", false);
            result.ChoiceDisabledForHolocom = data.ValueOrDefault<bool>("cnvIsVoteWinDisabledForHolocom", false);
            result.AbortsConversation = data.ValueOrDefault<bool>("cnvAbortConversation", false);
            result.IsPlayerNode = data.ValueOrDefault<bool>("cnvIsPcNode", false);

            result.ActionHook = QuestHookExtensions.ToQuestHook(data.ValueOrDefault<string>("cnvActionHook", null));

            // Load Alignment Results
            long alignmentAmount = data.ValueOrDefault<long>("cnvRewardForceAmount", 0);
            if (alignmentAmount != 0)
            {
                string forceType = data.ValueOrDefault<ScriptEnum>("cnvRewardForceType", new ScriptEnum()).ToString();
                result.AlignmentGain = ConversationAlignmentExtensions.ToConversationAlignment(alignmentAmount, forceType);
            }

            // Load Companion Affection Results
            var affectionGains = data.ValueOrDefault<Dictionary<object, object>>("cnvRewardAffectionRewards", null);
            result.AffectionRewards = new Dictionary<Npc, ConversationAffection>();
            result.AffectionRewardsIds = new Dictionary<long, ConversationAffection>();
            if (affectionGains != null)
            {
                foreach (var companionGain in affectionGains)
                {
                    long companionShortNameId = (long)companionGain.Key;
                    ConversationAffection affectionGain = ConversationAffectionExtensions.ToConversationAffection((long)companionGain.Value);
                    result.AffectionRewardsIds[companionShortNameId] = affectionGain; 
                    Npc companion = CompanionBySimpleNameId(companionShortNameId);
                    if (companion != null)
                        result.AffectionRewards[companion] = affectionGain;
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
                result.stb = txtData.Get<string>("strLocalizedTextRetrieverBucket");
                result.Text = _dom.stringTable.TryGetString(cnv.Fqn, txtData);
                result.LocalizedText = _dom.stringTable.TryGetLocalizedStrings(cnv.Fqn, txtData);
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
