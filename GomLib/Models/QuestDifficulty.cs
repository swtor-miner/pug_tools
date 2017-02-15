using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GomLib.Models
{
    public class QuestDifficulty {
        private DataObjectModel _dom;

        public QuestDifficulty(DataObjectModel dom)
        {
            _dom = dom;
            LoadData();
        }

        private Dictionary<string, float> table_data;
        string tablePath = "qstExperienceMultiplierPrototype";
        private bool disposed = false;

        private void LoadData()
        {
            GomObject table = _dom.GetObject(tablePath);
            if (table == null) return;
            Dictionary<object, object> tableData = table.Data.ValueOrDefault<Dictionary<object, object>>("qstExperienceMultiplierTable", new Dictionary<object, object>());

            table_data = tableData.ToDictionary(x => ((ScriptEnum)x.Key).ToString(), x => (float)x.Value);
        }

        public float GetMultiplier(string str)
        {
            float ret;
            table_data.TryGetValue(str, out ret);
            return ret;
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (disposed)
                return;
            if (disposing)
            {
                if(table_data != null) table_data.Clear();
                table_data = null;
            }
            disposed = true;
        }


        ~QuestDifficulty()
        {
            Dispose(false);
        }
    }
    //public enum QuestDifficulty
    //{
    //    NoExp = 0,
    //    ChainStep = 1,
    //    Easy = 2,
    //    Normal = 3,
    //    Hard = 4,
    //    VeryHard = 5,
    //    SeasonOne = 6
    //}

    //public static class QuestDifficultyExtensions
    //{
    //    public static QuestDifficulty ToQuestDifficulty(this string str)
    //    {
    //        if (String.IsNullOrEmpty(str)) { return QuestDifficulty.NoExp; }
    //        str = str.ToLower();
    //        switch (str)
    //        {
    //            case "qstdifficultynoexp":
    //            case "noexp": return QuestDifficulty.NoExp;
    //            case "qstdifficultychainstep":
    //            case "chainstep": return QuestDifficulty.ChainStep;
    //            case "qstdifficultyeasy":
    //            case "easy": return QuestDifficulty.Easy;
    //            case "qstdifficultynormal":
    //            case "normal": return QuestDifficulty.Normal;
    //            case "qstdifficultyhard":
    //            case "hard": return QuestDifficulty.Hard;
    //            case "qstdifficultyveryhard":
    //            case "veryhard": return QuestDifficulty.VeryHard;
    //            case "qstdifficultyseasonone":return QuestDifficulty.SeasonOne;
    //            default: throw new InvalidOperationException("Unknown Quest Difficulty: " + str);
    //        }
    //    }

    //    public static QuestDifficulty ToQuestDifficulty(this ScriptEnum val)
    //    {
    //        if (val == null) { return ToQuestDifficulty(String.Empty); }
    //        return ToQuestDifficulty(val.ToString());
    //    }
//}
}
