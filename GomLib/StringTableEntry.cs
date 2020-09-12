using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GomLib
{
    public class StringTableEntry : IEquatable<StringTableEntry>
    {
        public long Id { get; internal set; }

        public Dictionary<string, string> LocalizedText { get; internal set; }
        public Dictionary<string, string> OptionText { get; internal set; }
        public bool HasOptionText { get; set; }

        public override bool Equals(object obj)
        {
            if (obj == null) return false;

            if (ReferenceEquals(this, obj)) return true;

            if (!(obj is StringTableEntry ste)) return false;

            return Equals(ste);
        }

        public bool Equals(StringTableEntry ste)
        {
            if (ste == null) return false;

            if (ReferenceEquals(this, ste)) return true;

            if (Id != ste.Id)
                return false;

            var ssComp = new Models.DictionaryComparer<string, string>();
            if (!ssComp.Equals(LocalizedText, ste.LocalizedText))
                return false;

            return true;
        }

        public override int GetHashCode()
        {
            int hash = Id.GetHashCode();
            if (LocalizedText != null) { hash ^= LocalizedText.GetHashCode(); }
            return hash;
        }
    }
}
