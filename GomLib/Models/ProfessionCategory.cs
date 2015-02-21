using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GomLib.Models
{
    public enum ProfessionCategory
    {
        Crafting,
        Harvesting,
        Mission
    }

    public static class ProfessionCategoryExtensions
    {
        public static ProfessionCategory ToProfessionCategory(this string str)
        {
            switch (str)
            {
                case "prfProfessionCategoryCrafting": return ProfessionCategory.Crafting;
                case "prfProfessionCategoryHarvesting": return ProfessionCategory.Harvesting;
                case "prfProfessionCategoryMission": return ProfessionCategory.Mission;
                default: throw new InvalidOperationException("Unknown ProfessionCategory: " + str);
            }
        }
    }
}
