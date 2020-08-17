using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GomLib.Models
{
    public enum QuestCredits
    {
        None = 0,
        Low = 1,
        Medium = 2,
        High = 3,
        VeryHigh = 4
    }

    public static class QuestCreditsExtensions
    {
        public static QuestCredits ToQuestCredits(this string str)
        {
            if (String.IsNullOrEmpty(str)) { return QuestCredits.None; }
            str = str.ToLower();

            switch (str)
            {
                case "low": return QuestCredits.Low;
                case "medium": return QuestCredits.Medium;
                case "high": return QuestCredits.High;
                case "veryhigh": return QuestCredits.VeryHigh;
                default: throw new InvalidOperationException("Unknown QuestCredits: " + str);
            }
        }
    }
}
