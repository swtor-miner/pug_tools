using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GomLib.Models
{
    public enum QuestRepeatInterval
    {
        None = 0,
        Daily = 1,
        EveryOtherDay = 2,
        Biweekly = 3,
        Weekly = 4
    }

    public static class QuestRepeatIntervalExtensions
    {
        public static QuestRepeatInterval ToQuestRepeatInterval(this string str)
        {
            if (String.IsNullOrEmpty(str)) { return QuestRepeatInterval.None; }

            switch (str)
            {
                case "Daily": return QuestRepeatInterval.Daily;
                case "Weekly on Tuesday": return QuestRepeatInterval.Weekly;
                default: throw new InvalidOperationException("Unknown QuestRepeatInterval: " + str);
            }
        }
    }
}
