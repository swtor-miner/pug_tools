using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GomLib.Models
{
    public enum ArmorSpec
    {
        Undefined = 0,
        Light = 1,  // light
        Medium = 2, // medium
        Heavy = 3, // heavy
        Generator = 4, // generator
        ShieldForce = 5, // shield_force
        Focus = 6, // focus
        Shield = 7, // shield
        ArmorDroid = 8,
        LightFemale = 9,
        LightMale = 10,
        Adaptive = 11
    }

    public static class ArmorSpecExtensions
    {
        public static ArmorSpec ToArmorSpec(this long val)
        {
            // THESE VALUES ARE DEFINED IN chrArmorTablePrototype
            switch (val)
            {
                case 0: return ArmorSpec.Undefined;
                case 5763611092890301551: return ArmorSpec.Light;
                case 589686270506543030: return ArmorSpec.Medium;
                case -8622546409652942944: return ArmorSpec.Heavy;
                case 3927728356064183411: return ArmorSpec.Focus;
                case -8489117329102589124: return ArmorSpec.ShieldForce;
                case 6944519570668593218: return ArmorSpec.Generator;
                case -3325415850905959312: return ArmorSpec.Shield;
                case 1138086775958122321: return ArmorSpec.ArmorDroid;
                case 8799400953706039056: return ArmorSpec.LightFemale;
                case -7804289248234247763: return ArmorSpec.LightMale;
                case -335583520315957829: return ArmorSpec.Adaptive;
                default: throw new InvalidOperationException("Unknown ArmorSpec: " + val);
            }
        }

        public static bool HasStats(this ArmorSpec spec)
        {
            switch (spec)
            {
                case ArmorSpec.Generator:
                case ArmorSpec.Focus:
                case ArmorSpec.Shield:
                case ArmorSpec.ShieldForce:
                    return true;
                default:
                    return false;
            }
        }
    }
}
