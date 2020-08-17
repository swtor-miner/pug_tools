using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GomLib.Models
{
    /// <summary>Enumeration of the values in str.gui.questcategories</summary>
    public enum QuestCategory
    {
        Crafting = 1,
        Uncategorized = 2,
        Quesh = 3,
        Voss = 4,
        Tython = 5,
        Tatooine = 6,
        Taris = 7,
        OrdMantell = 8,
        NarShaddaa = 9,
        Korriban = 10,
        Ilum = 11,
        Hutta = 12,
        Hoth = 13,
        DromundKaas = 14,
        Coruscant = 15,
        Corellia = 16,
        Belsavis = 17,
        Balmorra = 18,
        Alderaan = 19,
        Operation = 20,
        Flashpoint = 21,
        PvP = 22,
        Space = 23,
        Companion = 24,
        Class = 25,
        Tutorial = 26,
        Achievement = 27, // Add some code to parse these from resources\en-us\str\gui\questcategories.stb
        Makeb = 28,
        Event = 29,
        Macrobinoculars = 30,
        Seeker_Droid = 31,
        Galactic_Solutions_Industries = 32,
        Czerka = 33,
        Holofile = 34,
        Oricon = 35
    }

    public static class QuestCategoryExtensions
    {
        public static QuestCategory ToQuestCategory(this long val)
        {
            switch (val)
            {
                case 0: return QuestCategory.Uncategorized;
                case 2466269005611264: return QuestCategory.Tutorial; 
                case 2466269005611265: return QuestCategory.Crafting;
                case 2466269005611266: return QuestCategory.Uncategorized;
                case 2466269005611267: return QuestCategory.Quesh;
                case 2466269005611268: return QuestCategory.Voss;
                case 2466269005611269: return QuestCategory.Tython;
                case 2466269005611270: return QuestCategory.Tatooine;
                case 2466269005611271: return QuestCategory.Taris;
                case 2466269005611272: return QuestCategory.OrdMantell;
                case 2466269005611273: return QuestCategory.NarShaddaa;
                case 2466269005611274: return QuestCategory.Korriban;
                case 2466269005611275: return QuestCategory.Ilum;
                case 2466269005611276: return QuestCategory.Hutta;
                case 2466269005611277: return QuestCategory.Hoth;
                case 2466269005611278: return QuestCategory.DromundKaas;
                case 2466269005611279: return QuestCategory.Coruscant;
                case 2466269005611280: return QuestCategory.Corellia;
                case 2466269005611281: return QuestCategory.Belsavis;
                case 2466269005611282: return QuestCategory.Balmorra;
                case 2466269005611283: return QuestCategory.Alderaan;
                case 2466269005611284: return QuestCategory.Operation;
                case 2466269005611285: return QuestCategory.Flashpoint;
                case 2466269005611286: return QuestCategory.PvP;
                case 2466269005611287: return QuestCategory.Space;
                case 2466269005611288: return QuestCategory.Companion;
                case 2466269005611289: return QuestCategory.Class;
                case 2466269005611290: return QuestCategory.Tutorial;
                case 2466269005611291: return QuestCategory.Achievement; // Added new ids by hand using exception catching code in questloader.cs, noticed pattern after a few scans, should be right.
                case 2466269005611292: return QuestCategory.Makeb;
                case 2466269005611293: return QuestCategory.Event;
                case 2466269005611294: return QuestCategory.Macrobinoculars;
                case 2466269005611295: return QuestCategory.Seeker_Droid;
                case 2466269005611296: return QuestCategory.Galactic_Solutions_Industries;
                case 2466269005611297: return QuestCategory.Czerka;
                case 2466269005611298: return QuestCategory.Holofile;
                case 2466269005611299: return QuestCategory.Oricon;
                //case 2466269005611302: return QuestCategory.Priority; //suspect Conquest

                default: throw new InvalidOperationException("Unknown QuestCategory: " + val);
            }
        }
    }
}
