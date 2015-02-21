using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;

namespace GomLib.Models
{
    public class QuestBranch : IEquatable<QuestBranch>
    {
        public int Id { get; set; }
        public int DbId { get; set; }
        [Newtonsoft.Json.JsonIgnore]
        public Quest Quest { get; set; }

        public List<QuestStep> Steps { get; set; }

        public List<Item> RewardAll { get; set; }
        public List<Item> RewardOne { get; set; }

        public override int GetHashCode()
        {
            int hash = Id.GetHashCode();
            foreach (var x in Steps) { hash ^= x.GetHashCode(); }
            return hash;
        }

        public override bool Equals(object obj)
        {
            if (obj == null) return false;

            if (ReferenceEquals(this, obj)) return true;

            QuestBranch qsb = obj as QuestBranch;
            if (qsb == null) return false;

            return Equals(qsb);
        }

        public bool Equals(QuestBranch qsb)
        {
            if (qsb == null) return false;

            if (ReferenceEquals(this, qsb)) return true;

            if (this.DbId != qsb.DbId)
                return false;
            if (this.Id != qsb.Id)
                return false;
            if (this.RewardAll != null)
            {
                if (qsb.RewardAll == null)
                {
                    return false;
                }
                else
                {
                    if (!Enumerable.SequenceEqual<Item>(this.RewardAll, qsb.RewardAll))
                        return false;
                }
            }
            if (this.RewardOne != null)
            {
                if (qsb.RewardOne == null)
                {
                    return false;
                }
                else
                {
                    if (!Enumerable.SequenceEqual<Item>(this.RewardOne, qsb.RewardOne))
                        return false;
                }
            }
            if (this.Steps != null)
            {
                if (qsb.Steps == null)
                {
                    return false;
                }
                else
                {
                    if (!Enumerable.SequenceEqual<QuestStep>(this.Steps, qsb.Steps))
                        return false;
                }
            }

            return true;
        }
    }
}
