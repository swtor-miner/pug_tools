using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GomLib
{
    public class DomEnum : DomType
    {
        private List<string> names;
        private List<short> vals;

        internal void AddValue(short val)
        {
            vals.Add(val);
        }

        internal void AddName(string name)
        {
            names.Add(name);
        }

        //public Dictionary<short, string> Values { get; private set; }

        //internal void AddValue(short val, string name)
        //{
        //    this.Values.Add(val, name);
        //}

        public override void Print(System.IO.TextWriter writer)
        {
            writer.WriteLine("Enum: {0} - 0x{1:X}", Name, Id);
            if (!String.IsNullOrEmpty(Description)) writer.WriteLine("\t{0}", Description);
            for (var i = 0; i < vals.Count; i++)
            {
                writer.WriteLine("\t0x{0:X}: {1} - 0x{2:X}", i + 1, names[i], vals[i]);
            }
        }

        public DomEnum()
        {
            this.names = new List<string>();
            this.vals = new List<short>();
        }

        public string ValueString(int idx)
        {
            //idx -= 1; //The DomEnum is zero-indexed, but the value that was stored to reference it wasn't. Fixed this discrepancy by making the stored value zero-indexed when read in.
            if (idx >= names.Count)
            {
                throw new ArgumentOutOfRangeException("idx");
            }

            return names[idx];
        }
    }
}
