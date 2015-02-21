﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GomLib.ModelLoader
{
    public class PackageAbilityLoader
    {
        StringTable strTable;

        DataObjectModel _dom;

        public PackageAbilityLoader(DataObjectModel dom)
        {
            _dom = dom;
            Flush();
        }

        public void Flush()
        {
            strTable = null;
        }
        
        public Models.PackageAbility Load(GomObjectData gomObj)
        {
            if (strTable == null)
            {
                strTable = _dom.stringTable.Find("str.abl.player.skill_trees");
            }

            Models.PackageAbility result = new Models.PackageAbility();
            result.AbilityId = gomObj.ValueOrDefault<ulong>("ablAbilityDataSpec", 0);
            result.AutoAcquire = gomObj.ValueOrDefault<bool>("ablAbilityDataAutoAcquire", false);
            // result.CategoryName = 
            // result.CategoryNameId

            // result.IsTalent
            result.PackageId = gomObj.ValueOrDefault<ulong>("ablAbilityDataPackage", 0);
            List<object> ranks = gomObj.ValueOrDefault<List<object>>("ablAbilityDataRanks", null);
            foreach (var rank in ranks)
            {
                result.Levels.Add((int)(long)rank);
            }
            if (result.Levels.Count > 0)
            {
                result.Level = result.Levels[0];
            }
            result.Scales = (result.Levels.Count == 61 - result.Level);
            // result.Rank = 
            // result.Toughness =

            result.Ability = _dom.abilityLoader.Load(result.AbilityId);

            var utilityTier = gomObj.ValueOrDefault<long>("ablUtilityTier");
            if (utilityTier != 0)
            {
                result.IsUtilityPackage = true;
                result.UtilityTier = utilityTier;
                result.UtilityPosition = gomObj.ValueOrDefault<long>("ablUtilityPosition");
            }
            return result;
        }

        public Models.PackageTalent LoadTalent(GomObjectData gomObj)
        {
            Models.PackageTalent result = new Models.PackageTalent();
            result.PackageId = gomObj.ValueOrDefault<ulong>("talTalentDataSpec", 0);
            result.Talent = _dom.talentLoader.Load(result.PackageId);
            result.Level = gomObj.ValueOrDefault<long>("talTalentLevel");
            result.UtilityTier = gomObj.ValueOrDefault<long>("talUtilityTier");
            result.UtilityPosition = gomObj.ValueOrDefault<long>("talUtilityPosition");

            return result;
        }
    }
}
