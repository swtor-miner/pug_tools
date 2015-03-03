using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GomLib
{
    public class StringTableEntry : IEquatable<StringTableEntry>
    {
        public long Id { get; internal set; }

        public Dictionary<string, string> localizedText { get; internal set; }

        public override bool Equals(object obj)
        {
            if (obj == null) return false;

            if (ReferenceEquals(this, obj)) return true;

            StringTableEntry ste = obj as StringTableEntry;
            if (ste == null) return false;

            return Equals(ste);
        }

        public bool Equals(StringTableEntry ste)
        {
            if (ste == null) return false;

            if (ReferenceEquals(this, ste)) return true;

            if (this.Id != ste.Id)
                return false;

            var ssComp = new Models.DictionaryComparer<string, string>();
            if (!ssComp.Equals(this.localizedText, ste.localizedText))
                return false;

            return true;
        }

        public override int GetHashCode()
        {
            int hash = Id.GetHashCode();
            if (localizedText != null) { hash ^= localizedText.GetHashCode(); }
            return hash;
        }
    }
}
