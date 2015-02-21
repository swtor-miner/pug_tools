using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GomLib.GomTypes
{
    class Integer : GomType
    {
        public Integer() : base(GomTypeId.Int64) { }
        public override string ToString()
        {
            return "long";
        }

        public override object ReadData(DataObjectModel dom, GomBinaryReader reader)
        {
            long val = reader.ReadSignedNumber();

            return val;
        }
    }
}
