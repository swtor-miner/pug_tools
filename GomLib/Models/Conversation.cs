using System;
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
    }
}
