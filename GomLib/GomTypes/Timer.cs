using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GomLib.GomTypes
{
    public class Timer : GomType
    {
        public Timer() : base(GomTypeId.Timer) { }

        public override string ToString()
        {
            return "timer";
        }

        public override object ReadData(DataObjectModel dom, GomBinaryReader reader)
        {
            reader.ReadBytes(0x22);
            return null;
        }
    }
}
