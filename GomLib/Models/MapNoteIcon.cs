using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GomLib.Models
{
    public enum MapNoteIcon
    {
        Unknown = 0,
        DefaultMapLink = 1,
        QuestCircle = 2,
        Wonkavator = 3,
        DefaultIcon = 4,
        LargeText = 5,
        SmallText = 6,
        Taxi = 7,
        BindPoint = 8,
        CheckPoint = 9,
        WarzonePOI = 10,
        PvpRepublicLarge = 11,
        PvpEmpireLarge = 12,
        ResPoint = 13
    }

    public static class MapNoteIconExtensions
    {
        public static MapNoteIcon ToMapNoteIcon(this string str)
        {
            if (String.IsNullOrEmpty(str)) return MapNoteIcon.Unknown;

            switch (str)
            {
                case "DefaultMapLink": return MapNoteIcon.DefaultMapLink;
                case "QuestCircle": return MapNoteIcon.QuestCircle;
                case "Wonkavator": return MapNoteIcon.Wonkavator;
                case "DefaultIcon": return MapNoteIcon.DefaultIcon;
                case "LargeText": return MapNoteIcon.LargeText;
                case "SmallText": return MapNoteIcon.SmallText;
                case "Taxi": return MapNoteIcon.Taxi;
                case "BindPoint": return MapNoteIcon.BindPoint;
                case "CheckPoint": return MapNoteIcon.CheckPoint;
                case "WarzonePOI": return MapNoteIcon.WarzonePOI;
                case "PvpRepublicLarge": return MapNoteIcon.PvpRepublicLarge;
                case "PvpEmpireLarge": return MapNoteIcon.PvpEmpireLarge;                
                case "ResPoint": return MapNoteIcon.ResPoint;
                default: throw new InvalidOperationException("Unknown MapNoteIcon: " + str);
            }
        }
    }
}
