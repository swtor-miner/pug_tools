using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GomLib.Models
{
    public enum QuestDifficulty
    {
        NoExp = 0,
        ChainStep = 1,
        Easy = 2,
        Normal = 3,
        Hard = 4,
        VeryHard = 5
    }

    public static class QuestDifficultyExtensions
    {
        public static QuestDifficulty ToQuestDifficulty(this string str)
        {
            if (String.IsNullOrEmpty(str)) { return QuestDifficulty.NoExp; }
            str = str.ToLower();
            switch (str)
            {
                case "qstdifficultynoexp":
                case "noexp": return QuestDifficulty.NoExp;
                case "qstdifficultychainstep":
                case "chainstep": return QuestDifficulty.ChainStep;
                case "qstdifficultyeasy":
                case "easy": return QuestDifficulty.Easy;
                case "qstdifficultynormal":
                case "normal": return QuestDifficulty.Normal;
                case "qstdifficultyhard":
                case "hard": return QuestDifficulty.Hard;
                case "qstdifficultyveryhard":
                case "veryhard": return QuestDifficulty.VeryHard;
                default: throw new InvalidOperationException("Unknown Quest Difficulty: " + str);
            }
        }

        public static QuestDifficulty ToQuestDifficulty(this ScriptEnum val)
        {
            if (val == null) { return ToQuestDifficulty(String.Empty); }
            return ToQuestDifficulty(val.ToString());
        }
    }
}
