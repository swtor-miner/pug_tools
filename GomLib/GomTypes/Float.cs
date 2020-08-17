using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GomLib.GomTypes
{
    public class Float : GomType
    {
        public Float() : base(GomTypeId.Float) { }
        public override string ToString()
        {
            return "float";
        }

        public override object ReadData(DataObjectModel dom, GomBinaryReader reader)
        {
            float val = reader.ReadSingle();
            return val;
        }
    }
}
