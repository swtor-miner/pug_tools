using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GomLib.GomTypes
{
    public class UInt64 : GomType
    {
        public UInt64() : base(GomTypeId.UInt64) { }

        public override string ToString()
        {
            return "ulong";
        }

        public override object ReadData(DataObjectModel dom, GomBinaryReader reader)
        {
            ulong val = reader.ReadNumber();

            return val;
        }

        public override bool ConfirmType(GomBinaryReader reader)
        {
            byte typeByte = reader.ReadByte();

            return typeByte == (byte)this.TypeId;
        }
    }
}
