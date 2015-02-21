using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GomLib.Models
{
    public class QuestTask : IEquatable<QuestTask>
    {
        [Newtonsoft.Json.JsonIgnore]
        public QuestStep Step { get; set; }
        public int Id { get; set; }
        public int DbId { get; set; }
        [Newtonsoft.Json.JsonIgnore]
        public DataObjectModel _dom { get; set; }

        public string String { get; set; }
        public Dictionary<string, string> LocalizedString { get; set; }
        public QuestHook Hook { get; set; }

        public bool ShowTracking { get; set; }
        public bool ShowCount { get; set; }
        public int CountMax { get; set; }

        public List<Quest> TaskQuests { get; set; }
        public List<Npc> TaskNpcs { get; set; }

        [Newtonsoft.Json.JsonIgnore]
        public List<Quest> BonusMissions
        {
            get
            {
                var bMissions = new List<Quest>();
                foreach (var bonMisId in BonusMissionsIds)
                {
                    if (bonMisId != 0)
                    {
                        var qst = _dom.questLoader.Load(bonMisId);
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
            hash ^= Hook.GetHashCode();
            hash ^= ShowTracking.GetHashCode();
            hash ^= ShowCount.GetHashCode();
            hash ^= CountMax.GetHashCode();
            return hash;
        }

        public override bool Equals(object obj)
        {
            if (obj == null) return false;

            if (ReferenceEquals(this, obj)) return true;

            QuestTask qts = obj as QuestTask;
            if (qts == null) return false;

            return Equals(qts);
        }

        public bool Equals(QuestTask qts)
        {
            if (qts == null) return false;

            if (ReferenceEquals(this, qts)) return true;

            if (this.BonusMissionsIds != null)
            {
                if (qts.BonusMissionsIds == null)
                {
                    return false;
                }
                else
                {
                    if (!Enumerable.SequenceEqual<ulong>(this.BonusMissionsIds, qts.BonusMissionsIds))
                        return false;
                }
            }
            if (this.CountMax != qts.CountMax)
                return false;
            if (this.DbId != qts.DbId)
                return false;
            if (!this.Hook.Equals(qts.Hook))
                return false;
            if (this.Id != qts.Id)
                return false;
            if (this.ItemsGiven != null)
            {
                if (qts.ItemsGiven == null)
                {
                    return false;
                }
                else
                {
                    if (!Enumerable.SequenceEqual<QuestItem>(this.ItemsGiven, qts.ItemsGiven))
                        return false;
                }
            }
            if (this.ItemsTaken != null)
            {
                if (qts.ItemsTaken == null)
                {
                    return false;
                }
                else
                {
                    if (!Enumerable.SequenceEqual<QuestItem>(this.ItemsTaken, qts.ItemsTaken))
                        return false;
                }
            }

            var ssComp = new DictionaryComparer<string, string>();
            if (!ssComp.Equals(this.LocalizedString, qts.LocalizedString))
                return false;

            if (this.ShowCount != qts.ShowCount)
                return false;
            if (this.ShowTracking != qts.ShowTracking)
                return false;
            if (this.String != qts.String)
                return false;

            if (this.TaskNpcs != null)
            {
                if (qts.TaskNpcs == null)
                {
                    return false;
                }
                else
                {
                    if (!Enumerable.SequenceEqual<Npc>(this.TaskNpcs, qts.TaskNpcs))
                        return false;
                }
            }

            if (this.TaskQuests != null)
            {
                if (qts.TaskQuests == null)
                {
                    return false;
                }
                else
                {
                    if (!Enumerable.SequenceEqual<Quest>(this.TaskQuests, qts.TaskQuests))
                        return false;
                }
            }

            return true;
        }
    }
}
