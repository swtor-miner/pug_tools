using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;

namespace GomLib.Models
{
    public enum AppearanceColor
    {
        None = 0,
        Purple = 1,
	    Yellow = 2,
	    Blue = 3,
	    Green = 4,
	    Red = 5,
	    Orange = 6,
	    White = 7,
        LightBlue = 8,
        LightRed = 9,
        BlackRed = 10,
        BlackYellow = 11,
        BlackBlue = 12,
        Black = 13,
        RazerGreen = 14
    }

    public static class AppearanceColorExtensions
    {
        public static AppearanceColor ToAppearanceColor(this string str)
        {
            if (string.IsNullOrEmpty(str)) return AppearanceColor.None;
            switch (str)
            {
                case "PURPLE": return AppearanceColor.Purple;
                case "BLK_PURPLE": return AppearanceColor.Purple;
                case "YELLOW": return AppearanceColor.Yellow;
                case "BLUE": return AppearanceColor.Blue;
                case "GREEN": return AppearanceColor.Green;
                case "BLK_GREEN": return AppearanceColor.Green;
                case "RED": return AppearanceColor.Red;
                case "ORANGE": return AppearanceColor.Orange;
                case "BLK_ORANGE": return AppearanceColor.Orange;
                case "WHITE": return AppearanceColor.White;
                case "BLK_WHITE": return AppearanceColor.White;
                case "LTBLUE": return AppearanceColor.LightBlue;
                case "BLK_LTBLUE": return AppearanceColor.LightBlue;
                case "BLK_BLUE": return AppearanceColor.BlackBlue;
                case "BLK_RED": return AppearanceColor.BlackRed;
                case "BLK_YELLOW": return AppearanceColor.BlackYellow;
                case "LTRED": return AppearanceColor.LightRed;
                case "RAZER_GREEN": return AppearanceColor.RazerGreen;
                case "BLACK": return AppearanceColor.Black;
                default: throw new InvalidOperationException("Unknown AppearanceColor: " + str);
            }
        }
    }

    public class DetailedAppearanceColor
    {
        public long ColorNameId { get; set; }
        public string ColorName { get; set; }
        public Dictionary<string, string> LocalizedColorName { get; set; }
        public Color Palette1Rep { get; set; }
        public Color Palette2Rep { get; set; }
        public bool UnknownBool1 { get; set; }
        public bool UnknownBool2 { get; set; }
        public long ColorSchemeId { get; set; }
        public long ColorId { get; set; }
        public long ShortId { get; set; }
        public string HueName { get; set; }
    }

}
