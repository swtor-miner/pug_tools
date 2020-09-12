using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GomLib.GomTypes
{
    public class Enum : GomType
    {
        public Enum() : base(GomTypeId.Enum) { }

        public ulong DomEnumId { get; internal set; }
        public DomEnum DomEnum { get; internal set; }

        internal override void Link(DataObjectModel dom)
        {
            _dom = dom;
            DomEnum = _dom.Get<DomEnum>(DomEnumId);
        }

        public override string ToString()
        {
            return string.Format("enum {0}", DomEnum);
        }

        public override object ReadData(DataObjectModel dom, GomBinaryReader reader)
        {
            ScriptEnum result = new ScriptEnum();

            int val = (int)reader.ReadNumber();
            //result.Value = val;
            result.Value = val - 1; //The DomEnum is zero-indexed, but the value that was stored to reference it wasn't. Fixed this discrepancy by making the stored value zero-indexed when read in.
            result.EnumType = DomEnum;

            return result;
        }
    }
}
