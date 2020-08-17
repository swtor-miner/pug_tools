using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GomLib.GomTypes
{
    public class Boolean : GomType
    {
        public Boolean() : base(GomTypeId.Boolean) { }
        public override string ToString()
        {
            return "bool";
        }

        public override object ReadData(DataObjectModel dom, GomBinaryReader reader)
        {
            byte val = reader.ReadByte();
            return (val != 0);
        }
    }
}
