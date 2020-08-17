using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GomLib.Models
{
    public enum ItemPlot
    {
        None = 0,
        QuestItem = 1,
        QuestRewardItem = 2
    }

    public static class ItemPlotExtensions
    {
        public static ItemPlot ToItemPlot(this string str)
        {
            if (String.IsNullOrEmpty(str)) return ItemPlot.None;

            switch (str)
            {
                case "None": return ItemPlot.None;
                case "QuestItem": return ItemPlot.QuestItem;
                case "QuestRewardItem": return ItemPlot.QuestRewardItem;
                default: throw new InvalidOperationException("Invalid ItemPlot: " + str);
            }
        }
    }
}
