using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GomLib.Models
{
    public enum MapNoteCondition
    {
        Unknown = 0,
        AlwaysOn = 1,
        BindPoint = 2,
        CheckPoint = 3,
        DevsOnly = 4,
        GroupContent = 5,
        MapLink = 6,
        Phase = 7,
        Quest = 8,
        ResPoint = 9,
        TaxiTerminal = 10,
        AlwaysOff = 11,
        Explored = 12,
        POI = 13,
        Wonkavator = 14,
        PvpPoi = 15
    }

    public static class MapNoteConditionExtensions
    {
        public static MapNoteCondition ToMapNoteCondition(this string str)
        {
            if (String.IsNullOrEmpty(str)) return MapNoteCondition.Unknown;

            switch (str)
            {
                case "AlwaysOn": return MapNoteCondition.AlwaysOn;
                case "BindPoint": return MapNoteCondition.BindPoint;
                case "CheckPoint": return MapNoteCondition.CheckPoint;
                case "DevsOnly": return MapNoteCondition.DevsOnly;
                case "GroupContent": return MapNoteCondition.GroupContent;
                case "MapLink": return MapNoteCondition.MapLink;
                case "Phase": return MapNoteCondition.Phase;
                case "Quest": return MapNoteCondition.Quest;
                case "ResPoint": return MapNoteCondition.ResPoint;
                case "TaxiTerminal": return MapNoteCondition.TaxiTerminal;
                case "AlwaysOff": return MapNoteCondition.AlwaysOff;
                case "Explored": return MapNoteCondition.Explored;
                case "POI": return MapNoteCondition.POI;
                case "Wonkavator": return MapNoteCondition.Wonkavator;
                case "PvpPoi": return MapNoteCondition.PvpPoi;
                default: throw new InvalidOperationException("Unknown MapNoteCondition: " + str);
            }
        }
    }
}
