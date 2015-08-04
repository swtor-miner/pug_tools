using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using System.Text;

namespace GomLib.Models
{
    public class SetBonusEntry: PseudoGameObject, IEquatable<SetBonusEntry>
    {
        public Dictionary<string, string> LocalizedNameStrings { get; set; }
        public long MaxItemCount { get; set; }
        public List<Item> Sources { get; set; }
        public Dictionary<long, Ability> BonusAbilityByNum { get; set; }

        public override int GetHashCode()
        {
            int hash = Id.GetHashCode();
            hash ^= MaxItemCount.GetHashCode();
            
            foreach(Item srcItm in Sources)
            {
                hash ^= srcItm.GetHashCode();
            }

            foreach(KeyValuePair<string, string> kvp in LocalizedNameStrings)
            {
                hash ^= kvp.Key.GetHashCode();
                hash ^= kvp.Value.GetHashCode();
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
            if(this.Sources.Count != entry.Sources.Count)
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
            foreach(Item itm in Sources)
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
