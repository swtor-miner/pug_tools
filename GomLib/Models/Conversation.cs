﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;
using System.Xml.Linq;
using System.Xml.XPath;

namespace GomLib.Models
{
    public class Conversation : GameObject, IEquatable<Conversation>
    {
        public List<ulong> SpeakersIds { get; set; }
        public Dictionary<string, bool> AudioLanguageState { get; set; }
        internal Dictionary<ulong, GameObject> LoadedSpeakers { get; set; }
        public Dictionary<ulong, GameObject> Speakers {
            get
            {
                if (LoadedSpeakers != null)
                    return LoadedSpeakers;

                Dictionary<ulong, GameObject> speaks = new Dictionary<ulong, GameObject>();
                for (int i = 0; i < SpeakersIds.Count; i++)
                {
                    speaks.Add(SpeakersIds[i], _dom.conversationLoader.LoadSpeaker(SpeakersIds[i]));
                }
                LoadedSpeakers = speaks;
                return speaks;
            }
        }
        public List<Placeable> Placeables { get; set; }
        /*public List<Quest> QuestStarted { get; set; }
        public List<Quest> QuestEnded { get; set; }
        public List<Quest> QuestProgressed { get; set; }*/

        public List<ulong> QuestStarted { get; set; }
        public List<ulong> QuestEnded { get; set; }
        public List<ulong> QuestProgressed { get; set; }

        public Dictionary<int, long> RootNodes { get; set; }
        public List<DialogNode> DialogNodes { get; set; }
        public Dictionary<long, DialogNode> NodeLookup { get; set; }
        public Dictionary<long, long> NodeLinkList { get; set; }

        public ulong DefaultSpeakerId { get; set; }
        public GameObject DefaultSpeaker { get; set; }

        public Conversation()
        {
            SpeakersIds = new List<ulong>();
            Placeables = new List<Placeable>();
            /*QuestStarted = new List<Quest>();
            QuestEnded = new List<Quest>();
            QuestProgressed = new List<Quest>();*/
            QuestStarted = new List<ulong>();
            QuestEnded = new List<ulong>();
            QuestProgressed = new List<ulong>();
            RootNodes = new Dictionary<int,long>();
            DialogNodes = new List<DialogNode>();
            NodeLookup = new Dictionary<long, DialogNode>();
            NodeLinkList = new Dictionary<long,long>();
            AudioLanguageState = new Dictionary<string, bool>();
        }

        public override bool Equals(object obj)
        {
            if (obj == null) return false;

            if (ReferenceEquals(this, obj)) return true;

            Conversation cnv = obj as Conversation;
            if (cnv == null) return false;

            return Equals(cnv);
        }

        public bool Equals(Conversation cnv)
        {
            if (cnv == null) return false;

            if (ReferenceEquals(this, cnv)) return true;

            if (this.DefaultSpeakerId != cnv.DefaultSpeakerId)
                return false;
            if (this.DialogNodes != null)
            {
                if (cnv.DialogNodes == null)
                {
                    return false;
                }
                else
                {
                    if (!Enumerable.SequenceEqual<DialogNode>(this.DialogNodes, cnv.DialogNodes))
                        return false;
                }
            }
            else if (cnv.DialogNodes != null)
                return false;

            if (this.Fqn != cnv.Fqn)
                return false;
            if (this.Id != cnv.Id)
                return false;

            var llComp = new DictionaryComparer<long, long>();
            if (!llComp.Equals(this.NodeLinkList, cnv.NodeLinkList))
                return false;

            var lDNComp = new DictionaryComparer<long, DialogNode>();
            if (!lDNComp.Equals(this.NodeLookup, cnv.NodeLookup)) //dunno if this will work
                return false; 

            if (this.NodeLookup != null)
            {
                if (cnv.NodeLookup == null)
                    return false;
                if (this.NodeLookup.Count != cnv.NodeLookup.Count || !Enumerable.SequenceEqual<long>(this.NodeLookup.Keys, cnv.NodeLookup.Keys))
                    return false;
                foreach (var kvp in this.NodeLookup)
                {
                    var prevReq = new DialogNode();
                    cnv.NodeLookup.TryGetValue(kvp.Key, out prevReq);
                    if (!kvp.Value.Equals(prevReq))
                        return false;
                }
            }
            else if (cnv.NodeLookup != null)
                return false;

            if (this.Placeables != null)
            {
                if (cnv.Placeables == null)
                {
                    return false;
                }
                else
                {
                    if (!Enumerable.SequenceEqual<Placeable>(this.Placeables, cnv.Placeables))
                        return false;
                }
            }
            else if (cnv.Placeables != null)
                return false;

            if (this.QuestEnded != null)
            {
                if (cnv.QuestEnded == null)
                {
                    return false;
                }
                else
                {
                    if (!Enumerable.SequenceEqual<ulong>(this.QuestEnded, cnv.QuestEnded))
                        return false;
                }
            }
            else if (cnv.QuestEnded != null)
                return false;

            if (this.QuestProgressed != null)
            {
                if (cnv.QuestProgressed == null)
                {
                    return false;
                }
                else
                {
                    if (!Enumerable.SequenceEqual<ulong>(this.QuestProgressed, cnv.QuestProgressed))
                        return false;
                }
            }
            else if (cnv.QuestProgressed != null)
                return false;

            if (this.QuestStarted != null)
            {
                if (cnv.QuestStarted == null)
                {
                    return false;
                }
                else
                {
                    if (!Enumerable.SequenceEqual<ulong>(this.QuestStarted, cnv.QuestStarted))
                        return false;
                }
            }
            else if (cnv.QuestStarted != null)
                return false;

            var ilComp = new DictionaryComparer<int, long>();
            if (!ilComp.Equals(this.RootNodes, cnv.RootNodes))
                return false;

            if (this.SpeakersIds != null)
            {
                if (cnv.SpeakersIds == null)
                {
                    return false;
                }
                else
                {
                    if (!Enumerable.SequenceEqual<ulong>(this.SpeakersIds, cnv.SpeakersIds))
                        return false;
                }
            }
            else if (cnv.SpeakersIds != null)
                return false;

            return true;
        }

        public string stb { get; set; }

        public string ToString(bool verbose)
        {
            StringBuilder bld = new StringBuilder();
            string n = Environment.NewLine;
            bld.Append(String.Format("Conversation - {0}{1}", Fqn, n));
            foreach (var dialogNode in DialogNodes)
            {
                if (SpeakersIds.Contains(dialogNode.SpeakerId))
                {
                    ulong speakerId = dialogNode.SpeakerId;
                    if (speakerId == 0)
                    {
                        speakerId = DefaultSpeakerId;
                    }
                    if (Speakers[speakerId] != null)
                    {
                        switch (Speakers[speakerId].Fqn.Substring(0, 3))
                        {
                            case "npc":
                                string name = ((GomLib.Models.Npc)Speakers[speakerId]).Name;
                                if (name.Length == 0) { name = Speakers[speakerId].Fqn; }
                                bld.Append(String.Format("{0}: {1} - {2}{3}", dialogNode.NodeId, name, dialogNode.Text, n));
                                continue;
                            case "plc":
                                string plcName = ((GomLib.Models.Placeable)Speakers[speakerId]).Name;
                                if (plcName.Length == 0) { plcName = Speakers[speakerId].Fqn; }
                                bld.Append(String.Format("{0}: {1} - {2}{3}", dialogNode.NodeId, plcName, dialogNode.Text, n));
                                continue;
                            default:
                                throw new Exception("Unaccounted for speaker type");
                        }
                    }

                }
                bld.Append(String.Format("{0}: {1}{2}", dialogNode.NodeId, dialogNode.Text, n));
            }
            return bld.ToString();
        }

        public override XElement ToXElement(bool verbose)
        {
            XElement conversation = new XElement("Conversation", new XAttribute("Id", Id));
            conversation.Add(new XElement("Fqn", Fqn));
            if (verbose)
            {
                XElement audioStateElem = new XElement("HasAudio");
                foreach(KeyValuePair<string, bool> audKvp in AudioLanguageState)
                {
                    audioStateElem.SetAttributeValue(audKvp.Key, audKvp.Value);
                }
                conversation.Add(audioStateElem);

                XElement dialogNodes = new XElement("DialogNodes");
                if (RootNodes != null)
                {
                    foreach (var rootNodeKvp in RootNodes)
                    {
                        var childs = new List<int>();
                        if (NodeLinkList.ContainsKey(rootNodeKvp.Value))
                        {
                            dialogNodes.Add(new XElement("LinkToDialogNode", new XAttribute("Id", rootNodeKvp.Value), NodeLinkList[rootNodeKvp.Value]));
                        }
                        else if (NodeLookup.ContainsKey(rootNodeKvp.Value))
                        {
                            dialogNodes.Add(NodeLookup[rootNodeKvp.Value].ToXElement(ref childs, rootNodeKvp.Value, verbose));
                        }
                        else
                        {
                            dialogNodes.Add(new XElement("DialogNode", new XAttribute("Id", "notFound" + rootNodeKvp.Value)));
                        }
                    }
                }
                conversation.Add(dialogNodes); //add classes to codex
                /*XElement speakers = new XElement("Speakers");
                foreach (var speaker in Speakers)
                {
                    if (speaker.Value != null)
                    {
                        switch (speaker.Value.Fqn.Substring(0, 3))
                        {
                            case "npc":
                                speakers.Add(new XElement("Speaker", new XAttribute("Id", speaker.Key >> 32), ((GomLib.Models.Npc)speaker.Value).Name + " - " + speaker.Value.Fqn));
                                break;
                            case "plc":
                                speakers.Add(new XElement("Speaker", new XAttribute("Id", speaker.Key >> 32), ((GomLib.Models.Placeable)speaker.Value).Name + " - " + speaker.Value.Fqn));
                                break;
                            default:
                                throw new Exception("Unaccounted for speaker type");
                        }
                    }
                    else
                    {
                        speakers.Add(new XElement("Speaker", new XAttribute("Id", speaker.Key)));
                    }
                }
                conversation.Add(speakers);
                XElement questStarted = new XElement("QuestStarted");
                foreach (var quest in QuestStarted)
                {
                    questStarted.Add(new XElement("Quest", new XElement("Name", quest.Name),
                                new XElement("Fqn", quest.Fqn,
                                new XAttribute("NodeId", quest.NodeId)),
                                new XAttribute("Id", quest.Id)));
                }
                conversation.Add(questStarted);

                XElement questProgressed = new XElement("QuestProgressed");
                foreach (var quest in QuestProgressed)
                {
                    questProgressed.Add(new XElement("Quest", new XElement("Name", quest.Name),
                                new XElement("Fqn", quest.Fqn,
                                new XAttribute("NodeId", quest.NodeId)),
                                new XAttribute("Id", quest.Id)));
                }
                conversation.Add(questProgressed);

                XElement questEnded = new XElement("QuestEnded");
                foreach (var quest in QuestEnded)
                {
                    questEnded.Add(new XElement("Quest", new XElement("Name", quest.Name),
                                new XElement("Fqn", quest.Fqn,
                                new XAttribute("NodeId", quest.NodeId)),
                                new XAttribute("Id", quest.Id)));
                }
                conversation.Add(questEnded);*/
                //conversation.Add(new XElement("QuestStarted", QuestStarted));
                //sqlexec("INSERT INTO `conversations` (`conversation_idc`, `quest_name`, `quest_nodeid`, `quest_id`, `IsBonus`, `BonusShareable`, `Branches`, `CanAbandon`, `Category`, `CategoryId`, `Classes`, `Difficulty`, `Fqn`, `Icon`, `IsClassQuest`, `IsHidden`, `IsRepeatable`, `Items`, `RequiredLevel`, `XpLevel`) VALUES (NULL, '" + name + "', '" + NodeId + "', '" + Id + "', '" + IsBonus + "', '" + BonusShareable + "', '" + Branches + "', '" + CanAbandon + "', '" + Category + "', '" + CategoryId + "', '" + Classes + "', '" + Difficulty + "', '" + Fqn + "', '" + Icon + "', '" + IsClassQuest + "', '" + IsHidden + "', '" + IsRepeatable + "', '" + Items + "', '" + RequiredLevel + "', '" + XpLevel + "');");
            }
            return conversation;

        }
    }
}
