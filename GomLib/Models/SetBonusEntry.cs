using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using System.Text;
using Newtonsoft.Json;

namespace GomLib.Models
{
    public class SetBonusEntry : PseudoGameObject, IEquatable<SetBonusEntry>
    {
        public Dictionary<string, string> LocalizedNameStrings { get; set; }
        public long MaxItemCount { get; set; }
        #region Sources
        [JsonIgnore]
        public List<ulong> SourcesIds { get; set; }
        internal List<string> SourcesB62Ids_ { get; set; }
        public List<string> SourcesB62Ids
        {
            get
            {
                if (SourcesB62Ids_ == null)
                {
                    if (SourcesIds == null) return new List<string>();
                    SourcesB62Ids_ = SourcesIds.Select(x => x.ToMaskedBase62()).ToList();
                }
                return SourcesB62Ids_;
            }
        }
        internal List<Item> Sources_ { get; set; }
        [JsonIgnore]
        public List<Item> Sources
        {
            get
            {
                if (Sources_ == null)
                {
                    if (SourcesIds == null) return new List<Item>();
                    Sources_ = SourcesIds.Select(x => Dom_.itemLoader.Load(x)).ToList();
                }
                return Sources_;
            }
        }
        #endregion
        #region Bonus Abilities
        [JsonIgnore]
        public Dictionary<long, ulong> BonusAbilityIdsByNum { get; set; }
        internal Dictionary<long, string> BonusAbilityIdsB62Ids_ { get; set; }
        public Dictionary<long, string> BonusAbilityIdsB62Ids
        {
            get
            {
                if (BonusAbilityIdsB62Ids_ == null)
                {
                    if (BonusAbilityIdsByNum == null) return new Dictionary<long, string>();
                    BonusAbilityIdsB62Ids_ = BonusAbilityIdsByNum.ToDictionary(x => x.Key, x => x.Value.ToMaskedBase62());
                }
                return BonusAbilityIdsB62Ids_;
            }
        }
        internal Dictionary<long, Ability> BonusAbilityByNum_ { get; set; }
        [JsonIgnore]
        public Dictionary<long, Ability> BonusAbilityByNum
        {
            get
            {
                if (BonusAbilityByNum_ == null)
                {
                    if (BonusAbilityIdsByNum == null) return new Dictionary<long, Ability>();
                    BonusAbilityByNum_ = BonusAbilityIdsByNum.ToDictionary(x => x.Key, x => Dom_.abilityLoader.Load(x.Value));
                }
                return BonusAbilityByNum_;
            }
        }
        #endregion
        #region Parsed Descriptions
        public Dictionary<long, Dictionary<string, string>> LocalizedBonusDescriptions
        {
            get
            {
                if (BonusAbilityIdsByNum == null) return null;
                return BonusAbilityIdsByNum.ToDictionary(x => x.Key, x => (Dom_.abilityLoader.Load(x.Value) ?? new Ability()).ParsedLocalizedDescription);
            }
        }
        #endregion

        public override int GetHashCode()
        {
            int hash = Id.GetHashCode();
            hash ^= MaxItemCount.GetHashCode();

            foreach (Item srcItm in Sources)
            {
                hash ^= srcItm.GetHashCode();
            }

            if (LocalizedNameStrings != null)
            {
                foreach (KeyValuePair<string, string> kvp in LocalizedNameStrings)
                {
                    hash ^= kvp.Key.GetHashCode();
                    hash ^= kvp.Value.GetHashCode();
                }
            }

            foreach (KeyValuePair<long, Ability> kvp in BonusAbilityByNum)
            {
                hash ^= kvp.Key.GetHashCode();
                hash ^= kvp.Value.GetHashCode();
            }

            return hash;
        }

        public override bool Equals(object entry)
        {
            if (entry == null)
                return false;
            if (ReferenceEquals(this, entry))
                return true;

            SetBonusEntry entryObj = entry as SetBonusEntry;
            return Equals(entryObj);
        }

        public bool Equals(SetBonusEntry entry)
        {
            if (entry == null)
                return false;
            if (ReferenceEquals(this, entry))
                return true;
            if (this.Id != entry.Id)
                return false;
            if (this.MaxItemCount != entry.MaxItemCount)
                return false;
            if (this.Sources.Count != entry.Sources.Count)
                return false;
            if (this.LocalizedNameStrings != null && entry.LocalizedNameStrings != null)
            {
                if (!this.LocalizedNameStrings.SequenceEqual(entry.LocalizedNameStrings))
                {
                    return false;
                }
            }
            if (!this.Sources.SequenceEqual(entry.Sources))
            {
                return false;
            }
            if (!this.BonusAbilityByNum.SequenceEqual(entry.BonusAbilityByNum))
            {
                return false;
            }

            return true;
        }

        public override XElement ToXElement(bool verbose)
        {
            XElement entryElem = new XElement("SetBonusEntry", new XAttribute("Id", Id));
            entryElem.Add(new XElement("Name", Name),
                new XElement("MaxCount", MaxItemCount));

            XElement itmListElem = new XElement("SourceItems");
            foreach (Item itm in Sources)
            {
                itmListElem.Add(new XElement("Item", new XAttribute("Id", itm.Id), itm.Name));
            }
            entryElem.Add(itmListElem);

            XElement ablBonusElem = new XElement("BonusAbilities");
            foreach (KeyValuePair<long, Ability> ablKvp in BonusAbilityByNum)
            {
                ablBonusElem.Add(new XElement("SetAbility", new XAttribute("Id", ablKvp.Key), ablKvp.Value.Description));
            }
            entryElem.Add(ablBonusElem);

            return entryElem;
        }
    }
}
