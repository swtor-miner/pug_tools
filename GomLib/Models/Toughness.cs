using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GomLib.Models
{
    public enum Toughness
    {
        None = 0,
        Weak = 1,
        Standard = 2,
        Strong = 3,
        Boss1 = 4,
        BossRaid = 5,
        Player = 6,
        Companion = 7,
        Boss2 = 8,
        Boss3 = 9,
        Boss4 = 10
    }

    public static class ToughnessExtensions
    {
        public static Toughness ToToughness(this string str)
        {
            if (String.IsNullOrEmpty(str)) { return Toughness.None; }

            switch (str)
            {
                case "cbtToughness_none": return Toughness.None;
                case "cbtToughness_weak": return Toughness.Weak;
                case "cbtToughness_standard": return Toughness.Standard;
                case "cbtToughness_strong": return Toughness.Strong;
                case "cbtToughness_boss_1": return Toughness.Boss1;
                case "cbtToughness_boss_raid": return Toughness.BossRaid;
                case "cbtToughness_player": return Toughness.Player;
                case "cbtToughness_companion": return Toughness.Companion;
                case "cbtToughness_boss_2": return Toughness.Boss2;
                case "cbtToughness_boss_3": return Toughness.Boss3;
                case "cbtToughness_boss_4": return Toughness.Boss4;
                default: throw new InvalidOperationException("Unknown Toughness: " + str);
            }
        }

        public static Toughness ToToughness(int val)
        {
            if ((val > 11) || (val < 1)) throw new InvalidOperationException("Unknown Toughness: " + val);

            return (Toughness)val;
        }

        public static Toughness ToToughness(this ScriptEnum val)
        {
            if (val == null) { return Toughness.None; }
            return ToToughness((int)val.Value);
        }
    }
}
