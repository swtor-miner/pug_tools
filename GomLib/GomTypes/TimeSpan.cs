using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GomLib.GomTypes
{
    public class TimeSpan : GomType
    {
        public TimeSpan() : base(GomTypeId.TimeSpan) { }

        public override string ToString()
        {
            return "timespan";
        }

        public override object ReadData(DataObjectModel dom, GomBinaryReader reader)
        {
            return reader.ReadNumber();
        }
    }
}
