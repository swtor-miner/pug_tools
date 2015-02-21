using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GomLib.Models
{
    public enum HuntingRadius
    {
        None,
        Small,
        Medium,
        Large,
        ExtraLarge,
        SuperSize,
        Husky,
        BigBoned
    }

    public static class HuntingRadiusExtensions
    {
        public static HuntingRadius ToHuntingRadius(this string str)
        {
            if (String.IsNullOrEmpty(str)) { return HuntingRadius.None; }

            switch (str)
            {
                case "None": return HuntingRadius.None;
                case "Small": return HuntingRadius.Small;
                case "Medium": return HuntingRadius.Medium;
                case "Large": return HuntingRadius.Large;
                case "Extra Large": return HuntingRadius.ExtraLarge;
                case "Super Size": return HuntingRadius.SuperSize;
                case "Husky": return HuntingRadius.Husky;
                case "Big Boned": return HuntingRadius.BigBoned;
                default: throw new InvalidOperationException("Unknown HuntingRadius: " + str);
            }
        }

        public static HuntingRadius ToHuntingRadius(long val)
        {
            switch (val)
            {
                case 5836340512931272827: return HuntingRadius.None;        // 0
                case 2959999436154067372: return HuntingRadius.Small;       // 25
                case 589686270506543030: return HuntingRadius.Medium;       // 40
                case -2493745135610518796: return HuntingRadius.Large;      // 60
                case -9157026497624578618: return HuntingRadius.ExtraLarge; // 100
                case 6196483884609695543: return HuntingRadius.SuperSize;   // 150
                case 24054240851265359: return HuntingRadius.Husky;         // 200
                case -3288063830629591179: return HuntingRadius.BigBoned;   // 250
                default: throw new InvalidOperationException("Unknown HuntingRadius: " + val.ToString());
            }
        }
    }
}
