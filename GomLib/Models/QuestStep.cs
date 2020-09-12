using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;

namespace GomLib.Models
{
    public class QuestStep : IEquatable<QuestStep>
    {
        [Newtonsoft.Json.JsonIgnore]
        public QuestBranch Branch { get; set; }
        public int Id { get; set; }
        public int DbId { get; set; }
        [Newtonsoft.Json.JsonIgnore]
        public DataObjectModel Dom_ { get; set; }
        public bool IsShareable { get; set; }
        public string JournalText { get; set; }
        public Dictionary<string, string> LocalizedJournalText { get; set; }
        public List<QuestTask> Tasks { get; set; }
        //public List<string> Strings { get; set; }
        [Newtonsoft.Json.JsonIgnore]
        public List<Quest> BonusMissions
        {
            get
            {
                var bMissions = new List<Quest>();
                foreach (var bonMisId in BonusMissionsIds)
                {
                    bMissions.Add(Dom_.questLoader.Load(bonMisId));
                }
                return bMissions;
            }
        }
        public List<ulong> BonusMissionsIds { get; set; }
        public List<QuestItem> ItemsGiven { get; set; }
        public List<QuestItem> ItemsTaken { get; set; }

        public override int GetHashCode()
        {
            int hash = Id.GetHashCode();
            hash ^= IsShareable.GetHashCode();
            if (JournalText != null) { hash ^= JournalText.GetHashCode(); }
            foreach (var x in Tasks) { hash ^= x.GetHashCode(); }
            return hash;
        }

        public override bool Equals(object obj)
        {
            if (obj == null) return false;

            if (ReferenceEquals(this, obj)) return true;

            if (!(obj is QuestStep qss)) return false;

            return Equals(qss);
        }

        public bool Equals(QuestStep qss)
        {
            if (qss == null) return false;

            if (ReferenceEquals(this, qss)) return true;

            if (BonusMissions != null)
            {
                if (qss.BonusMissions == null)
                {
                    return false;
                }
                else
                {
                    if (!Enumerable.SequenceEqual(BonusMissions, qss.BonusMissions))
                        return false;
                }
            }
            if (DbId != qss.DbId)
                return false;
            if (Id != qss.Id)
                return false;
            if (IsShareable != qss.IsShareable)
                return false;
            if (ItemsGiven != null)
            {
                if (qss.ItemsGiven == null)
                {
                    return false;
                }
                else
                {
                    if (!Enumerable.SequenceEqual(ItemsGiven, qss.ItemsGiven))
                        return false;
                }
            }
            if (ItemsTaken != null)
            {
                if (qss.ItemsTaken == null)
                {
                    return false;
                }
                else
                {
                    if (!Enumerable.SequenceEqual(ItemsTaken, qss.ItemsTaken))
                        return false;
                }
            }
            if (JournalText != qss.JournalText)
                return false;
            if (Tasks != null)
            {
                if (qss.Tasks == null)
                {
                    return false;
                }
                else
                {
                    if (!Enumerable.SequenceEqual(Tasks, qss.Tasks))
                        return false;
                }
            }

            return true;
        }

        public XElement ToXElement(bool verbose)
        {
            XElement stepNode = new XElement("Step", new XElement("JournalText", JournalText.Replace(Environment.NewLine, "<br>")),
                new XAttribute("Id", Id),
                new XElement("Shareable", IsShareable),
                new XElement("ItemsTaken"));
            //new XAttribute("DBId", DbId)); //this is always 0
            foreach (var task in Tasks)
            {
                XElement taskNode = task.ToXElement(verbose);
                stepNode.Add(taskNode); //add task to tasks
            }

            if (verbose)
            {
                new Quest().QuestItemsGivenOrTakenToXElement(stepNode, ItemsGiven, ItemsTaken);
            }

            stepNode.Add(new XElement("BonusMissions"));
            if (BonusMissions.Count > 0)
            {
                foreach (var bonus in BonusMissions)
                {
                    stepNode.Element("BonusMissions").Add(bonus.ToXElement(true));
                }
            }

            return stepNode;
        }

    }
}
