using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;

namespace GomLib.Models
{
    public class DialogNode : IEquatable<DialogNode>
    {
        public Conversation Conversation { get; set; }
        public long NodeId { get; set; }

        /*public List<Quest> QuestsGranted { get; set; }
        public List<Quest> QuestsEnded { get; set; }
        public List<Quest> QuestsProgressed { get; set; }
        public Quest QuestReward { get; set; }

        public Quest ActionQuest { get; set; }*/

        public List<ulong> QuestsGranted { get; set; }
        public List<ulong> QuestsEnded { get; set; }
        public List<ulong> QuestsProgressed { get; set; }
        public ulong QuestReward { get; set; }

        public ulong ActionQuest { get; set; }
        public QuestHook ActionHook { get; set; }

        public int MinLevel { get; set; }
        public int MaxLevel { get; set; }
        public ConversationAlignment AlignmentGain { get; set; }
        public int CreditsGained { get; set; }
        public bool IsEmpty { get; set; }
        public bool JoinDisabledForHolocom { get; set; }
        public bool ChoiceDisabledForHolocom { get; set; }
        public bool AbortsConversation { get; set; }
        public bool IsPlayerNode { get; set; }

        public ulong SpeakerId { get; set; }

        public string Text { get; set; }
        public Dictionary<string, string> LocalizedText { get; set; }

        public Dictionary<Npc, ConversationAffection> AffectionRewards { get; set; }
        public Dictionary<long, ConversationAffection> AffectionRewardsIds { get; set; }

        /// <summary>Doesn't trigger conversation mode - just speaks dialog and prints text to chat tab</summary>
        public bool IsAmbient { get; set; }

        public List<int> ChildIds { get; set; }
        public List<DialogNode> ChildNodes { get; set; }

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

            if (this.AffectionRewardsIds != null)
            {
                if (dln.AffectionRewardsIds == null)
                    return false;
                if (this.AffectionRewardsIds.Count != dln.AffectionRewardsIds.Count || !this.AffectionRewardsIds.Keys.SequenceEqual(dln.AffectionRewardsIds.Keys))
                    return false;
                foreach (var kvp in this.AffectionRewardsIds)
                {
                    ConversationAffection prevAff;
                    dln.AffectionRewardsIds.TryGetValue(kvp.Key, out prevAff);
                    if (prevAff == null)
                        return false;
                    if (!kvp.Value.Equals(prevAff))
                        return false;
                }
            }
            else if (dln.AffectionRewardsIds != null)
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

        public string stb { get; set; }

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
                                    dNode.Add(new XElement("Speaker", new XAttribute("Id", ((GomLib.Models.Placeable)Conversation.Speakers[speakerId]).NodeId >> 32), plcName));
                                    break;
                                default:
                                    throw new Exception("Unaccounted for speaker type");
                            }
                        }

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
                    if (ActionHook.ToString() != "None") { dNode.Add(new XElement("ActionHook", ActionHook.ToString())); }
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
                        dNode.Add(new XElement("AffectionGain", new XAttribute("Id", i), AffectionRewards.ElementAt(i).Key.Fqn + " - " + AffectionRewards.ElementAt(i).Value.ToString()));
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
