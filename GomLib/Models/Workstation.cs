using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GomLib.Models
{
    public enum Workstation
    {
        None = 0,
        Normal = 1,
        Advanced = 2
    }

    public static class WorkstationExtensions
    {
        public static Workstation ToWorkstation(this string str)
        {
            if (String.IsNullOrEmpty(str)) return Workstation.None;
            switch (str)
            {
                case "prfWorkstationNone": return Workstation.None;
                case "prfWorkstationNormal": return Workstation.Normal;
                case "prfWorkstationAdvanced": return Workstation.Advanced;
                default: throw new InvalidOperationException("Invalid Workstation type: " + str);
            }
        }

        public static Workstation ToWorkstation(this ScriptEnum val)
        {
            if (val == null) { return ToWorkstation(String.Empty); }
            return ToWorkstation(val.ToString());
        }
    }
}
