using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GomLib.Models
{
    public enum EnhancementStationType
    {
        None = 0,
        General = 1,
        Armor = 2,
        Lightsaber = 3,
        Blaster = 4
    }

    public class EnhancementStationTypeExtensions
    {
        public static EnhancementStationType ToEnhancementStationType(ScriptEnum val)
        {
            if (val == null) { return EnhancementStationType.None; }

            switch (val.ToString())
            {
                case "itmEnhancementStationTypeNone": return EnhancementStationType.None;
                case "itmEnhancementStationTypeGeneral": return EnhancementStationType.General;
                case "itmEnhancementStationTypeArmor": return EnhancementStationType.Armor;
                case "itmEnhancementStationTypeLightsaber": return EnhancementStationType.Lightsaber;
                case "itmEnhancementStationTypeBlaster": return EnhancementStationType.Blaster;
                default: throw new InvalidOperationException("Unknown EnhancementStationType: " + val.ToString());
            }
        }
    }
}
