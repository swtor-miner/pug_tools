using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GomLib.GomTypes
{
    public class Time : GomType
    {
        public Time() : base(GomTypeId.Time) { }

        public override string ToString()
        {
            return "time";
        }

        public override object ReadData(DataObjectModel dom, GomBinaryReader reader)
        {
            ulong val = reader.ReadNumber();
            return val;
        }
    }
}
