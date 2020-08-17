using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;
using Newtonsoft.Json;

namespace GomLib.Models
{
    public class QuestBranch : IEquatable<QuestBranch>
    {
        [JsonConverter(typeof(LongConverter))]
        public long Id { get; set; }
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

            if (!(obj is QuestBranch qsb)) return false;

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

        public XElement ToXElement(bool verbose)
        {
            XElement branchNode = new XElement("Branch",
                new XAttribute("Id", Id)); //,
            //new XAttribute("Hash", GetHashCode()));
            /*if (verbose)
            {
                XElement rewardAll = new XElement("RewardAll"); //need to fix the questloader to actually load these from the questrewards prototypes.
                if (RewardAll != null)
                {
                    foreach (var reward in RewardAll) { rewardAll.Add(new XElement("Reward", new XAttribute("Name", reward.Name))); }
                }

                XElement rewardOne = new XElement("RewardOne"); //need to fix the questloader to actually load these from the questrewards prototypes.
                if (RewardOne != null)
                {
                    foreach (var reward in RewardOne) { rewardOne.Add(new XElement("Reward", new XAttribute("Name", reward.Name))); }
                }

                branchNode.Add(rewardAll, rewardOne);
            }*/
            foreach (var step in Steps)
            {
                XElement stepNode = step.ToXElement(verbose);
                branchNode.Add(stepNode); //add step to steps
            }
            return branchNode;
        }
    }
}
