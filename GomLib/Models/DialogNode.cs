using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;
using Newtonsoft.Json;

namespace GomLib.Models
{
    public class DialogNode : IEquatable<DialogNode>
    {
        [JsonIgnore]
        public Conversation Conversation { get; set; }
        public long NodeId { get; set; }

        /*public List<Quest> QuestsGranted { get; set; }
        public List<Quest> QuestsEnded { get; set; }
        public List<Quest> QuestsProgressed { get; set; }
        public Quest QuestReward { get; set; }

        public Quest ActionQuest { get; set; }*/

        [JsonIgnore]
        public List<ulong> QuestsGranted { get; set; }
        public List<string> QuestsGrantedB62 { get { return QuestsGranted.ToMaskedBase62(); } }
        [JsonIgnore]
        public List<ulong> QuestsEnded { get; set; }
        public List<string> QuestsEndedB62 { get { return QuestsEnded.ToMaskedBase62(); } }
        [JsonIgnore]
        public List<ulong> QuestsProgressed { get; set; }
        public List<string> QuestsProgressedB62 { get { return QuestsProgressed.ToMaskedBase62(); } }
        [JsonIgnore]
        public ulong QuestReward { get; set; }
        public string QuestRewardB62 { get { return QuestReward.ToMaskedBase62(); } }

        public ulong ActionQuest { get; set; }
        public string ActionHook { get; set; }

        public int MinLevel { get; set; }
        public int MaxLevel { get; set; }
        [JsonConverter(typeof(Newtonsoft.Json.Converters.StringEnumConverter))]
        public ConversationAlignment AlignmentGain { get; set; }
        public int CreditsGained { get; set; }
        public bool IsEmpty { get; set; }
        public bool JoinDisabledForHolocom { get; set; }
        public bool ChoiceDisabledForHolocom { get; set; }
        public bool AbortsConversation { get; set; }
        public bool IsPlayerNode { get; set; }

        [JsonIgnore]
        public ulong SpeakerId { get; set; }
        public string SpeakerB62Id { get { return SpeakerId.ToMaskedBase62(); } }

        public string Text { get; set; }
        public Dictionary<string, string> LocalizedText { get; set; }
        public Dictionary<string, string> LocalizedOptionText { get; set; }

        //To display what alien VO we should be using for this.
        public long cnvAlienVONode { get; set; }
        public string cnvAlienVOFQN { get; set; }

        internal Dictionary<Npc, KeyValuePair<int, string>> _AffectionRewards { get; set; }
        [JsonIgnore]
        public Dictionary<Npc, KeyValuePair<int, string>> AffectionRewards
        {
            get
            {
                if (_AffectionRewards == null)
                {
                    _AffectionRewards = new Dictionary<Npc, KeyValuePair<int, string>>();
                    foreach (var kvp in AffectionRewardEvents)
                    {
                        Npc companion = Conversation._dom.conversationLoader.CompanionBySimpleNameId(kvp.Key);
                        if (companion != null)
                            _AffectionRewards[companion] = kvp.Value;
                    }
                }
                return _AffectionRewards;
            }
        }
        [JsonIgnore]
        public Dictionary<long, KeyValuePair<int, string>> AffectionRewardEvents { get; set; }
        public Dictionary<string, KeyValuePair<int, Dictionary<string, string>>> AffectionRewardEventsB62 { get; set; }

        /// <summary>Doesn't trigger conversation mode - just speaks dialog and prints text to chat tab</summary>
        public bool IsAmbient { get; set; }

        public List<int> ChildIds { get; set; }
        [JsonIgnore]
        public List<DialogNode> ChildNodes { get; set; }

        public string stb { get; set; }

        public override int GetHashCode()
        {
            int hash = QuestReward.GetHashCode();
            if (stb != null) hash ^= stb.GetHashCode();
            if (QuestsGranted != null) foreach (var x in QuestsGranted) { hash ^= x.GetHashCode(); } //dictionaries need to hashed like this
            if (QuestsEnded != null) foreach (var x in QuestsEnded) { hash ^= x.GetHashCode(); } //dictionaries need to hashed like this
            if (QuestsProgressed != null) foreach (var x in QuestsProgressed) { hash ^= x.GetHashCode(); } //dictionaries need to hashed like this
            hash ^= ActionQuest.GetHashCode();
            hash ^= ActionHook.GetHashCode();
            hash ^= MinLevel.GetHashCode();
            hash ^= MaxLevel.GetHashCode();
            hash ^= AlignmentGain.GetHashCode();
            hash ^= CreditsGained.GetHashCode();
            hash ^= IsEmpty.GetHashCode();
            hash ^= JoinDisabledForHolocom.GetHashCode();
            hash ^= ChoiceDisabledForHolocom.GetHashCode();
            hash ^= AbortsConversation.GetHashCode();
            hash ^= IsPlayerNode.GetHashCode();
            hash ^= SpeakerId.GetHashCode();
            if (Text != null) hash ^= Text.GetHashCode();
            if (LocalizedText != null) foreach (var x in LocalizedText) { hash ^= x.GetHashCode(); } //dictionaries need to hashed like this
            if (AffectionRewardEvents != null) foreach (var x in AffectionRewardEvents) { hash ^= x.Key.GetHashCode(); hash ^= x.Value.GetHashCode(); }
            hash ^= IsAmbient.GetHashCode();
            if (ChildIds != null) foreach (var x in ChildIds) { hash ^= x.GetHashCode(); } //dictionaries need to hashed like this
            return hash;
        }

        public override bool Equals(object obj)
        {
            if (obj == null) return false;

            if (ReferenceEquals(this, obj)) return true;

            DialogNode dln = obj as DialogNode;
            if (dln == null) return false;

            return Equals(dln);
        }

        public bool Equals(DialogNode dln)
        {
            if (dln == null) return false;

            if (ReferenceEquals(this, dln)) return true;

            if (this.AbortsConversation != dln.AbortsConversation)
                return false;
            if (this.ActionHook != dln.ActionHook)
                return false;
            if (this.ActionQuest != dln.ActionQuest)
                return false;

            if (this.AffectionRewardEvents != null)
            {
                if (dln.AffectionRewardEvents == null)
                    return false;
                if (this.AffectionRewardEvents.Count != dln.AffectionRewardEvents.Count || !this.AffectionRewardEvents.Keys.SequenceEqual(dln.AffectionRewardEvents.Keys))
                    return false;
                foreach (var kvp in this.AffectionRewardEvents)
                {
                    KeyValuePair<int, string> prevAff;
                    if (!dln.AffectionRewardEvents.TryGetValue(kvp.Key, out prevAff))
                        return false;
                    if (!kvp.Value.Equals(prevAff))
                        return false;
                }
            }
            else if (dln.AffectionRewardEvents != null)
                return false;

            if (this.AlignmentGain != dln.AlignmentGain)
                return false;
            if (this.ChildIds != null)
            {
                if (dln.ChildIds == null)
                {
                    return false;
                }
                else
                {
                    if (!Enumerable.SequenceEqual<int>(this.ChildIds, dln.ChildIds))
                        return false;
                }
            }
            else if (dln.ChildIds != null)
                return false;

            if (this.ChildNodes != null)
            {
                if (dln.ChildNodes == null)
                {
                    return false;
                }
                else
                {
                    if (!Enumerable.SequenceEqual<DialogNode>(this.ChildNodes, dln.ChildNodes))
                        return false;
                }
            }
            else if (dln.ChildNodes != null)
                return false;

            if (this.ChoiceDisabledForHolocom != dln.ChoiceDisabledForHolocom)
                return false;
            if (this.CreditsGained != dln.CreditsGained)
                return false;
            if (this.IsAmbient != dln.IsAmbient)
                return false;
            if (this.IsEmpty != dln.IsEmpty)
                return false;
            if (this.IsPlayerNode != dln.IsPlayerNode)
                return false;
            if (this.JoinDisabledForHolocom != dln.JoinDisabledForHolocom)
                return false;

            var ssComp = new DictionaryComparer<string, string>();
            if (!ssComp.Equals(this.LocalizedText, dln.LocalizedText))
                return false;

            if (this.MaxLevel != dln.MaxLevel)
                return false;
            if (this.MinLevel != dln.MinLevel)
                return false;
            if (this.NodeId != dln.NodeId)
                return false;
            if (this.QuestReward != dln.QuestReward)
                return false;
            if (this.QuestsEnded != null)
            {
                if (dln.QuestsEnded == null)
                {
                    return false;
                }
                else
                {
                    if (!Enumerable.SequenceEqual<ulong>(this.QuestsEnded, dln.QuestsEnded))
                        return false;
                }
            }
            else if (dln.QuestsEnded != null)
                return false;

            if (this.QuestsGranted != null)
            {
                if (dln.QuestsGranted == null)
                {
                    return false;
                }
                else
                {
                    if (!Enumerable.SequenceEqual<ulong>(this.QuestsGranted, dln.QuestsGranted))
                        return false;
                }
            }
            else if (dln.QuestsGranted != null)
                return false;

            if (this.QuestsProgressed != null)
            {
                if (dln.QuestsProgressed == null)
                {
                    return false;
                }
                else
                {
                    if (!Enumerable.SequenceEqual<ulong>(this.QuestsProgressed, dln.QuestsProgressed))
                        return false;
                }
            }
            else if (dln.QuestsProgressed != null)
                return false;

            if (this.SpeakerId != dln.SpeakerId)
                return false;
            if (this.Text != dln.Text)
                return false;
            return true;
        }

        public XElement ToXElement(ref List<int> childs, long id, bool verbose)
        {
            XElement dNode = new XElement("DialogNode", new XAttribute("Id", id));

            if (true) //!childs.Contains((int)id))
            {
                if (Conversation.SpeakersIds.Contains(SpeakerId))
                {
                    ulong speakerId = SpeakerId;
                    if (speakerId == 0)
                    {
                        speakerId = Conversation.DefaultSpeakerId;
                    }
                    if (Conversation.Speakers[speakerId] != null)
                    {
                        switch (Conversation.Speakers[speakerId].Fqn.Substring(0, 3))
                        {
                            case "npc":
                                string name = ((GomLib.Models.Npc)Conversation.Speakers[speakerId]).Name;
                                if (name.Length == 0) { name = Conversation.Speakers[speakerId].Fqn; }
                                dNode.Add(new XElement("Speaker", new XAttribute("Id", ((GomLib.Models.Npc)Conversation.Speakers[speakerId]).NodeId >> 32), name));
                                break;
                            case "plc":
                                string plcName = ((GomLib.Models.Placeable)Conversation.Speakers[speakerId]).Name;
                                if (plcName.Length == 0) { plcName = Conversation.Speakers[speakerId].Fqn; }
                                dNode.Add(new XElement("Speaker", new XAttribute("Id", ((GomLib.Models.Placeable)Conversation.Speakers[speakerId]).Id >> 32), plcName));
                                break;
                            default:
                                throw new Exception("Unaccounted for speaker type");
                        }
                    }

                }

                //Output alien data.
                if (cnvAlienVOFQN.Length > 0)
                {
                    dNode.Add(new XElement("AlienVO", new XAttribute("FQN", cnvAlienVOFQN), cnvAlienVONode));
                }

                dNode.Add(new XElement("Text", Text));
                if (MinLevel != -1) { dNode.Add(new XElement("MinLevel", MinLevel)); }
                if (MaxLevel != -1) { dNode.Add(new XElement("MaxLevel", MaxLevel)); }
                if (IsEmpty) { dNode.Add(new XElement("IsEmpty", IsEmpty)); }
                if (IsAmbient) { dNode.Add(new XElement("IsAmbient", IsAmbient)); }
                if (IsPlayerNode) { dNode.Add(new XElement("IsPlayerNode", IsPlayerNode)); }
                if (JoinDisabledForHolocom) { dNode.Add(new XElement("JoinDisabledForHolocom", JoinDisabledForHolocom)); }
                if (ChoiceDisabledForHolocom) { dNode.Add(new XElement("ChoiceDisabledForHolocom", ChoiceDisabledForHolocom)); }
                if (AbortsConversation) { dNode.Add(new XElement("AbortsConversation", AbortsConversation)); }
                if (ActionHook != null && ActionHook.ToString() != "None") { dNode.Add(new XElement("ActionHook", ActionHook.ToString())); }
                if (ActionQuest != 0)
                {
                    /*dNode.Add(new XElement("ActionQuest", new XElement("Name", ActionQuest.Name),
                        new XElement("Fqn", ActionQuest.Fqn,
                        new XAttribute("NodeId", ActionQuest.NodeId)),
                        new XAttribute("Id", ActionQuest.Id)));*/
                    dNode.Add(new XElement("ActionQuest", new XAttribute("Id", ActionQuest)));
                }
                if (QuestReward != 0)
                {
                    /*XElement rewardNode = new XElement("QuestReward", new XElement("Name", QuestReward.Name),
                        new XElement("Fqn", QuestReward.Fqn,
                        new XAttribute("NodeId", QuestReward.NodeId)),
                        new XAttribute("Id", QuestReward.Id));
                    dNode.Add(rewardNode);*/
                    dNode.Add(new XElement("QuestReward", new XAttribute("Id", QuestReward)));
                }
                if (QuestsGranted.Count != 0)
                {
                    XElement questsGranted = new XElement("QuestsGranted", new XAttribute("Id", QuestsGranted.Count));
                    for (int i = 0; i < QuestsGranted.Count; i++)
                    {
                        /*questsGranted.Add(new XElement("Quest", new XElement("Name", QuestsGranted[i].Name),
                            new XElement("Fqn", QuestsGranted[i].Fqn,
                            new XAttribute("NodeId", QuestsGranted[i].NodeId)),
                            new XAttribute("Id", QuestsGranted[i].Id)));*/
                        questsGranted.Add(new XElement("Quest", new XAttribute("Id", QuestsGranted[i])));
                    }
                    dNode.Add(questsGranted);
                }
                if (QuestsEnded.Count != 0)
                {
                    XElement questsEnded = new XElement("QuestsEnded", new XAttribute("Id", QuestsEnded.Count));
                    for (int i = 0; i < QuestsEnded.Count; i++)
                    {
                        /*questsEnded.Add(new XElement("Quest", new XElement("Name", QuestsEnded[i].Name),
                            new XElement("Fqn", QuestsEnded[i].Fqn,
                            new XAttribute("NodeId", QuestsEnded[i].NodeId)),
                            new XAttribute("Id", QuestsEnded[i].Id)));*/
                        questsEnded.Add(new XElement("Quest", new XAttribute("Id", QuestsEnded[i])));
                    }
                    dNode.Add(questsEnded);
                }
                if (QuestsProgressed.Count != 0)
                {
                    XElement questsProgressed = new XElement("QuestsProgressed", new XAttribute("Id", QuestsProgressed.Count));
                    for (int i = 0; i < QuestsProgressed.Count; i++)
                    {
                        /*questsProgressed.Add(new XElement("Quest", new XElement("Name", QuestsProgressed[i].Name),
                            new XElement("Fqn", QuestsProgressed[i].Fqn,
                            new XAttribute("NodeId", QuestsProgressed[i].NodeId)),
                            new XAttribute("Id", QuestsProgressed[i].Id)));*/
                        questsProgressed.Add(new XElement("Quest", new XAttribute("Id", QuestsProgressed[i])));
                    }
                    dNode.Add(questsProgressed);
                }
                if (AlignmentGain.ToString() != "None")
                {
                    dNode.Add(new XElement("AlignmentGain", AlignmentGain.ToString().Replace("Small", "+50 ")
                                                                                  .Replace("Medium", "+100 ")
                                                                                  .Replace("Large", "+150 ")
                                                                                  .Replace("Mega", "+200 ")));
                }
                for (int i = 0; i < AffectionRewards.Count; i++)
                {
                    KeyValuePair<Npc, KeyValuePair<int, string>> influenceDetails = AffectionRewards.ElementAt(i);
                    dNode.Add(new XElement("AffectionGain", new XAttribute("Id", influenceDetails.Key.Base62Id), new XElement("RewardReceiver",
                        influenceDetails.Key.Name), new XElement("InfluenceChange", influenceDetails.Value.Key),
                        new XElement("RewardReaction", influenceDetails.Value.Value)));
                }

                if (ChildIds != null)
                {
                    foreach (var child in ChildIds)
                    {
                        if (Conversation.NodeLinkList.ContainsKey(child))
                        {
                            dNode.Add(new XElement("LinkToDialogNode", new XAttribute("Id", child), Conversation.NodeLinkList[child]));
                        }
                        else if (Conversation.NodeLookup.ContainsKey(child))
                        {
                            dNode.Add(Conversation.NodeLookup[child].ToXElement(ref childs, child, verbose));
                        }
                        else
                        {
                            dNode.Add(new XElement("DialogNode", new XAttribute("Id", "notFound" + child)));
                        }
                    }
                }
                childs.Add((int)id);
            }
            return dNode;

        }
    }
}
