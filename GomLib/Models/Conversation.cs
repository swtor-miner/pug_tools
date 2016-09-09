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
    public class Conversation : GameObject, IEquatable<Conversation>
    {
        [JsonIgnore]
        public List<ulong> SpeakersIds { get; set; }
        internal Dictionary<string, string> _SpeakersB62Ids;
        public Dictionary<string, string> SpeakersB62Ids
        {
            get {
                if (_SpeakersB62Ids == null)
                {
                    if (SpeakersIds == null) return new Dictionary<string, string>();
                    _SpeakersB62Ids = SpeakersIds.ToDictionary(x => x.ToMaskedBase62(), x => (_dom.GetObject(x) ?? new GomObject()).Name);
                }
                return _SpeakersB62Ids;
            }
        }
        public Dictionary<string, bool> AudioLanguageState { get; set; }
        internal Dictionary<ulong, GameObject> LoadedSpeakers { get; set; }
        [JsonIgnore]
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
        [JsonIgnore]
        public List<Placeable> Placeables { get; set; }
        /*public List<Quest> QuestStarted { get; set; }
        public List<Quest> QuestEnded { get; set; }
        public List<Quest> QuestProgressed { get; set; }*/

        [JsonIgnore]
        public List<ulong> QuestStarted { get; set; }
        [JsonIgnore]
        public List<string> _QuestB62Started;
        public List<string> QuestB62Started { get { if (_QuestB62Started == null) _QuestB62Started = QuestStarted.ToMaskedBase62(); return _QuestB62Started; } }
        [JsonIgnore]
        public List<ulong> QuestEnded { get; set; }
        [JsonIgnore]
        public List<string> _QuestB62Ended;
        public List<string> QuestB62Ended { get { if (_QuestB62Ended == null) _QuestB62Ended = QuestEnded.ToMaskedBase62(); return _QuestB62Ended; } }
        [JsonIgnore]
        public List<ulong> QuestProgressed { get; set; }
        [JsonIgnore]
        public List<string> _QuestB62Progressed;
        public List<string> QuestB62Progressed { get { if (_QuestB62Progressed == null) _QuestB62Progressed = QuestProgressed.ToMaskedBase62(); return _QuestB62Progressed; } }

        public Dictionary<int, long> RootNodes { get; set; }
        [JsonIgnore]
        public List<DialogNode> DialogNodes { get; set; }
        public Dictionary<long, DialogNode> NodeLookup { get; set; }
        public Dictionary<long, long> NodeLinkList { get; set; }

        public ulong DefaultSpeakerId { get; set; }
        [JsonIgnore]
        public GameObject DefaultSpeaker { get; set; }

        public string stb { get; set; }

        public bool IsKOTORStyle { get; set; }

        public HashSet<string> AffectionNpcB62 { get; set; }
        public HashSet<string> AffectionNcoB62 { get; set; }
        public Dictionary<string, string> LocalizedName { get; set; }

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

        public override int GetHashCode()
        {
            int hash = DefaultSpeakerId.GetHashCode();
            hash ^= IsKOTORStyle.GetHashCode();
            if (stb != null) hash ^= stb.GetHashCode();
            if (AudioLanguageState != null) foreach (var x in AudioLanguageState) { hash ^= x.GetHashCode(); } //dictionaries need to hashed like this
            if (SpeakersIds != null) foreach (var x in SpeakersIds) { hash ^= x.GetHashCode(); } //dictionaries need to hashed like this
            if (Placeables != null) foreach (var x in Placeables) { hash ^= x.GetHashCode(); } //dictionaries need to hashed like this
            if (QuestStarted != null) foreach (var x in QuestStarted) { hash ^= x.GetHashCode(); } //dictionaries need to hashed like this
            if (QuestEnded != null) foreach (var x in QuestEnded) { hash ^= x.GetHashCode(); } //dictionaries need to hashed like this
            if (QuestProgressed != null) foreach (var x in QuestProgressed) { hash ^= x.GetHashCode(); } //dictionaries need to hashed like this
            if (RootNodes != null) foreach (var x in RootNodes) { hash ^= x.GetHashCode(); } //dictionaries need to hashed like this
            if (NodeLookup != null) foreach (var x in NodeLookup) { hash ^= x.GetHashCode(); } //dictionaries need to hashed like this
            if (NodeLinkList != null) foreach (var x in NodeLinkList) { hash ^= x.GetHashCode(); } //dictionaries need to hashed like this
            return hash;
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
            /*if (this.DialogNodes != null) //these are the same nodes as on the NodeLookup list
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
                return false;*/

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

            /*if (this.NodeLookup != null) //this was being done right above this
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
                return false;*/ 

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

            if(this.IsKOTORStyle != cnv.IsKOTORStyle)
            {
                return false;
            }

            return true;
        }

        public override string ToString(bool verbose)
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

                conversation.Add(new XElement("IsKOTORStyle", IsKOTORStyle));

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
