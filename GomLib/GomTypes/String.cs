using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GomLib.GomTypes
{
    public class String : GomType
    {
        public String() : base(GomTypeId.String) { }

        public override string ToString()
        {
            return "string";
        }

        public override object ReadData(DataObjectModel dom, GomBinaryReader reader)
        {
            return reader.ReadLengthPrefixString(Encoding.UTF8);
        }
    }
}
