using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GomLib
{
    public class ScriptEnum
    {
        public DomEnum EnumType { get; set; }
        public int Value { get; set; }

        public override string ToString()
        {
            if (EnumType != null)
            {
                return EnumType.ValueString(Value);
            }
            else
            {
                return string.Format("0x{0:X2}", Value);
            }
        }
    }
}
