using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GomLib.Models
{
    /// <summary>Represents a difficulty mode for an instance</summary>
    public enum DifficultyLevel
    {
        None = 0,
        DifficultyLevel1 = 1,   // Normal Mode Flashpoints
        DifficultyLevel2 = 2,
        DifficultyLevel3 = 4,
        DifficultyLevel4 = 8    // Hard Mode Flashpoints
    }

    public static class DifficultyLevelExtensions
    {
        public static DifficultyLevel ToDifficultyLevel(long val)
        {
            if (val == 0) { return DifficultyLevel.None; }

            switch (val)
            {
                case 1: return DifficultyLevel.DifficultyLevel1;
                case 2: return DifficultyLevel.DifficultyLevel2;
                case 4: return DifficultyLevel.DifficultyLevel3;
                case 8: return DifficultyLevel.DifficultyLevel4;
                default:
                    throw new InvalidOperationException("Unknown DifficultyLevel: " + val.ToString());
            }
        }
    }
}
