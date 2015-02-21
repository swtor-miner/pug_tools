using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GomLib.Models
{
    public enum SpawnerType
    {
        Unknown = 0,
        Placeable = 1,
        Creature = 2,
        CreatureFactory = 3,
        List = 4
    }

    public static class SpawnerTypeExtensions
    {
        public static SpawnerType ToSpawnerType(this string str)
        {
            if (String.IsNullOrEmpty(str)) { return SpawnerType.Unknown; }

            switch (str)
            {
                case "creature": return SpawnerType.Creature;
                case "placeable": return SpawnerType.Placeable;
                case "crefac": return SpawnerType.CreatureFactory;
                case "list": return SpawnerType.List;
                default: throw new InvalidOperationException("Unknown Spawner Type: " + str);
            }
        }
    }
}
