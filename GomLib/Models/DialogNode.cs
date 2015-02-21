using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

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
    }
}
