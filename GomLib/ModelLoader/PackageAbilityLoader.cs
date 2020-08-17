using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GomLib.ModelLoader
{
    public class PackageAbilityLoader
    {
        StringTable strTable;
        readonly DataObjectModel _dom;

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

            Models.PackageAbility result = new Models.PackageAbility
            {
                _dom = _dom,
                AbilityId = gomObj.ValueOrDefault<ulong>("ablAbilityDataSpec", 0),
                AutoAcquire = gomObj.ValueOrDefault<bool>("ablAbilityDataAutoAcquire", false),
                PackageId = gomObj.ValueOrDefault<ulong>("ablAbilityDataPackage", 0)
            };
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
            result.Toughness = gomObj.ValueOrDefault<string>("apnCbtToughness", "");
            result.AiUsePriority = gomObj.ValueOrDefault<long>("AiUsagePriority", 0);

            //result.Ability = _dom.abilityLoader.Load(result.AbilityId);

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
            Models.PackageTalent result = new Models.PackageTalent
            {
                _dom = _dom,
                PackageId = gomObj.ValueOrDefault<ulong>("talTalentDataSpec", 0),
                //result.Talent = _dom.talentLoader.Load(result.PackageId);
                Level = gomObj.ValueOrDefault<long>("talTalentLevel"),
                UtilityTier = gomObj.ValueOrDefault<long>("talUtilityTier"),
                UtilityPosition = gomObj.ValueOrDefault<long>("talUtilityPosition")
            };

            return result;
        }
    }
}
