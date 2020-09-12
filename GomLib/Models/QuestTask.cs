using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;
using Newtonsoft.Json;

namespace GomLib.Models
{
    public class QuestTask : IEquatable<QuestTask>
    {
        [JsonIgnore]
        public QuestStep Step { get; set; }
        public int Id { get; set; }
        public int DbId { get; set; }
        [JsonIgnore]
        public DataObjectModel Dom_ { get; set; }

        public string String { get; set; }
        public Dictionary<string, string> LocalizedString { get; set; }
        public string Hook { get; set; }

        public bool ShowTracking { get; set; }
        public bool ShowCount { get; set; }
        public int CountMax { get; set; }

        public List<ulong> TaskQuestIds { get; set; }
        public List<string> TaskQuestB62Ids
        {
            get
            {
                if (TaskQuestIds == null) return new List<string>();
                return TaskQuestIds.Select(x => x.ToMaskedBase62()).ToList();
            }
        }
        public List<ulong> TaskNpcIds { get; set; }
        public List<string> TaskNpcB62Ids
        {
            get
            {
                if (TaskNpcIds == null) return new List<string>();
                return TaskNpcIds.Select(x => x.ToMaskedBase62()).ToList();
            }
        }
        public List<ulong> TaskPlcIds { get; set; }
        public List<string> TaskPlcB62Ids
        {
            get
            {
                if (TaskPlcIds == null) return new List<string>();
                return TaskPlcIds.Select(x => x.ToMaskedBase62()).ToList();
            }
        }
        [JsonIgnore]
        public List<Quest> TaskQuests { get; set; }
        [JsonIgnore]
        public List<Npc> TaskNpcs { get; set; }

        [JsonIgnore]
        public List<string> MapNoteFqnList { get; set; }
        public List<string> MapNoteB62Ids
        {
            get
            {
                if (MapNoteFqnList == null) return new List<string>();
                return MapNoteFqnList.Select(x => Dom_.GetObjectId(x).ToMaskedBase62()).ToList();
            }
        }

        [JsonIgnore]
        public List<Quest> BonusMissions
        {
            get
            {
                var bMissions = new List<Quest>();
                foreach (var bonMisId in BonusMissionsIds)
                {
                    if (bonMisId != 0)
                    {
                        var qst = Dom_.questLoader.Load(bonMisId);
                        if (qst.Fqn != null)
                            bMissions.Add(qst);
                    }
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
            if (String != null) { hash ^= String.GetHashCode(); }
            if (Hook != null) { hash ^= Hook.GetHashCode(); }
            if (MapNoteFqnList != null) foreach (var mpn in MapNoteFqnList) { hash ^= mpn.GetHashCode(); }
            hash ^= ShowTracking.GetHashCode();
            hash ^= ShowCount.GetHashCode();
            hash ^= CountMax.GetHashCode();
            return hash;
        }

        public override bool Equals(object obj)
        {
            if (obj == null) return false;

            if (ReferenceEquals(this, obj)) return true;

            if (!(obj is QuestTask qts)) return false;

            return Equals(qts);
        }

        public bool Equals(QuestTask qts)
        {
            if (qts == null) return false;

            if (ReferenceEquals(this, qts)) return true;

            if (BonusMissionsIds != null)
            {
                if (qts.BonusMissionsIds == null)
                {
                    return false;
                }
                else
                {
                    if (!Enumerable.SequenceEqual(BonusMissionsIds, qts.BonusMissionsIds))
                        return false;
                }
            }
            if (CountMax != qts.CountMax)
                return false;
            if (DbId != qts.DbId)
                return false;
            if (Hook != null)
            {
                if (qts.Hook == null)
                    return false;
                else
                {
                    if (!Hook.Equals(qts.Hook))
                        return false;
                }
            }
            if (Id != qts.Id)
                return false;
            if (ItemsGiven != null)
            {
                if (qts.ItemsGiven == null)
                {
                    return false;
                }
                else
                {
                    if (!Enumerable.SequenceEqual(ItemsGiven, qts.ItemsGiven))
                        return false;
                }
            }
            if (ItemsTaken != null)
            {
                if (qts.ItemsTaken == null)
                {
                    return false;
                }
                else
                {
                    if (!Enumerable.SequenceEqual(ItemsTaken, qts.ItemsTaken))
                        return false;
                }
            }

            var ssComp = new DictionaryComparer<string, string>();
            if (!ssComp.Equals(LocalizedString, qts.LocalizedString))
                return false;

            if (ShowCount != qts.ShowCount)
                return false;
            if (ShowTracking != qts.ShowTracking)
                return false;
            if (String != qts.String)
                return false;

            if (TaskNpcs != null)
            {
                if (qts.TaskNpcs == null)
                {
                    return false;
                }
                else
                {
                    if (!Enumerable.SequenceEqual(TaskNpcs, qts.TaskNpcs))
                        return false;
                }
            }

            if (TaskQuests != null)
            {
                if (qts.TaskQuests == null)
                {
                    return false;
                }
                else
                {
                    if (!Enumerable.SequenceEqual(TaskQuests, qts.TaskQuests))
                        return false;
                }
            }

            return true;
        }

        public XElement ToXElement(bool verbose)
        {
            XElement taskNode = new XElement("Task",
                new XAttribute("Id", Id));
            //new XAttribute("DBId", DbId)); //this is always 0
            if (String != null)
                taskNode.Add(new XElement("String", String));
            else
                taskNode.Add(new XElement("String"));

            taskNode.Add(new XElement("CountMax", CountMax));

            new Quest().QuestItemsGivenOrTakenToXElement(taskNode, ItemsGiven, ItemsTaken);

            taskNode.Add(new XElement("BonusMissions"));
            if (BonusMissions != null)
            {
                foreach (var bonus in BonusMissions)
                {
                    taskNode.Element("BonusMissions").Add(bonus.ToXElement(verbose));
                }
            }

            if (verbose)
            {
                taskNode.Add(new XElement("Hook", Hook),
                    new XElement("ShowCount", ShowCount),
                    new XElement("ShowTracking", ShowTracking));
                XElement taskNpcs = new XElement("TaskNpcs");
                foreach (var npc in TaskNpcs)
                {
                    if (Step.Branch.Quest.LoadedNpcs.ContainsKey(npc.Fqn))
                    {
                        taskNpcs.Add(Step.Branch.Quest.LoadedNpcs[npc.Fqn]);
                    }
                    else
                    {
                        XElement taskNpc = npc.ToXElement(verbose);
                        taskNpcs.Add(taskNpc); //add task npc to task npcs
                    }
                }
                taskNode.Add(taskNpcs); //add task npcs to task
                XElement taskQuests = new XElement("TaskQuests");
                foreach (var quest in TaskQuests)
                {
                    XElement taskQuest = quest.ToXElement(verbose);
                    taskQuests.Add(taskQuest); //add task quest to task quests
                }
                taskNode.Add(taskQuests); //add task quests to task
            }
            return taskNode;
        }
    }
}
