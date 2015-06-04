using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GomLib.Models;

namespace GomLib.ModelLoader
{
    public class ReputationGroupLoader
    {
        Dictionary<long, ReputationGroup> idLookup;

        DataObjectModel _dom;

        public ReputationGroupLoader(DataObjectModel dom)
        {
            _dom = dom;
            Flush();
        }

        public void Flush()
        {
            idLookup = new Dictionary<long, ReputationGroup>();
        }

        public Models.ReputationGroup Load(long id)
        {
            if (idLookup.Count == 0)
            {
                idLookup = _dom.GetObject("repGroupInfosPrototype").Data.Get<Dictionary<object, object>>("repGroupInfoData").ToDictionary(x => Convert.ToInt64(x.Key), x => Load(new ReputationGroup(), (GomObjectData)x.Value));
            }
            ReputationGroup data = null;
            idLookup.TryGetValue(id, out data);

            return data;
        }

        public Models.ReputationGroup Load(Models.ReputationGroup rank, GomObjectData obj)
        {
            if (obj == null) { return rank; }
            if (rank == null) { return null; }

            //General Fields
            rank.GroupId = obj.ValueOrDefault<long>("repGroupInfoId", 0);
            rank.GroupFactionAvailability = obj.Get<ScriptEnum>("repGroupInfoFactionAvailablilityEnum").ToString();
            rank.GroupReputationWeeklyLimit = obj.ValueOrDefault<long>("repGroupInfoWeeklyRepLimit", 0);
            
            //Republic Sepecific Fields
            rank.GroupRepublicDescriptionId = obj.ValueOrDefault<long>("repGroupInfoRepublicDescription", 0);
            rank.GroupRepublicTitleId = obj.ValueOrDefault<long>("repGroupInfoRepublicTitle", 0);
            rank.GroupRepublicDescription = _dom.stringTable.TryGetString("str.lgc.reputation", rank.GroupRepublicDescriptionId);
            rank.GroupRepublicTitle = _dom.stringTable.TryGetString("str.lgc.reputation", rank.GroupRepublicTitleId);
            StringTable repTable = _dom.stringTable.Find("str.lgc.reputation");
            rank.LocalizedGroupRepublicTitle = repTable.GetLocalizedText(rank.GroupRepublicTitleId, "str.lgc.reputation");
            rank.GroupRepublicRankTitles = obj.Get<List<object>>("repGroupInfoRepublicRankTitles");
            rank.GroupRepublicIcon = obj.ValueOrDefault<string>("repGroupInfoIconRepublic", "");
            rank.GroupRepublicRankLegacyTitles = new Dictionary<object, object>();
            
            //Empire Specific Fields
            rank.GroupEmpireDescriptionId = obj.ValueOrDefault<long>("repGroupInfoEmpireDescription", 0);
            rank.GroupEmpireTitleId = obj.ValueOrDefault<long>("repGroupInfoEmpireTitle", 0);
            rank.GroupEmpireDescription = _dom.stringTable.TryGetString("str.lgc.reputation", rank.GroupEmpireDescriptionId);
            rank.GroupEmpireTitle = _dom.stringTable.TryGetString("str.lgc.reputation", rank.GroupEmpireTitleId);
            rank.LocalizedGroupEmpireTitle = repTable.GetLocalizedText(rank.GroupEmpireTitleId, "str.lgc.reputation");
            rank.GroupEmpireRankTitles = obj.Get<List<object>>("repGroupInfoEmpireRankTitles");
            rank.GroupEmpireIcon = obj.ValueOrDefault<string>("repGroupInfoIconEmpire", "");
            rank.GroupEmpireRankLegacyTitles = new Dictionary<object, object>();

            rank.LocalizedGroupleTitle = rank.LocalizedGroupEmpireTitle.ToDictionary(x => x.Key, x => String.Format("{0} / {1}", rank.LocalizedGroupRepublicTitle[x.Key], x.Value));

            List<object> repRankData = _dom.GetObject("repRankInfoPrototype").Data.Get<List<object>>("repRankInfoData");
            Dictionary<object, object> lgcTitleData = _dom.GetObject("lgcLegacyTitlesTablePrototype").Data.Get<Dictionary<object, object>>("lgcLegacyTitlesData");

            int intCount = 0;
            foreach (long rankId in rank.GroupRepublicRankTitles)
            {
                GomLib.Models.ReputationRank currentRank = new GomLib.Models.ReputationRank();
                _dom.reputationRankLoader.Load(currentRank, (GomObjectData)repRankData[intCount]);

                if (rankId != 0)
                {
                    object titleData = new object();
                    lgcTitleData.TryGetValue(rankId, out titleData);

                    GomLib.Models.LegacyTitle title = new GomLib.Models.LegacyTitle();
                    _dom.legacyTitleLoader.Load(title, (GomObjectData)titleData);
                    rank.GroupRepublicRankLegacyTitles.Add(currentRank.RankTitle, title.LegacyTitleString);
                }
                else
                {
                    rank.GroupRepublicRankLegacyTitles.Add(currentRank.RankTitle, "");
                }
                intCount++;
            }

            intCount = 0;
            foreach (long rankId in rank.GroupEmpireRankTitles)
            { 
                GomLib.Models.ReputationRank currentRank = new GomLib.Models.ReputationRank();
                _dom.reputationRankLoader.Load(currentRank, (GomObjectData)repRankData[intCount]);

                if (rankId != 0)
                {
                    object titleData = new object();
                    lgcTitleData.TryGetValue(rankId, out titleData);

                    GomLib.Models.LegacyTitle title = new GomLib.Models.LegacyTitle();
                    _dom.legacyTitleLoader.Load(title, (GomObjectData)titleData);
                    rank.GroupEmpireRankLegacyTitles.Add(currentRank.RankTitle, title.LegacyTitleString);
                }
                else
                {
                    rank.GroupEmpireRankLegacyTitles.Add(currentRank.RankTitle, "");
                }
                intCount++;
            }

            return rank;
        }
    }
}
