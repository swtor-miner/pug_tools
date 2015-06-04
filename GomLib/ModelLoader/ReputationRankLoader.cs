using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GomLib.Models;

namespace GomLib.ModelLoader
{
    public class ReputationRankLoader
    {
        Dictionary<long, ReputationRank> idLookup;

        DataObjectModel _dom;

        public ReputationRankLoader(DataObjectModel dom)
        {
            _dom = dom;
            Flush();
        }

        public void Flush()
        {
            idLookup = new Dictionary<long, ReputationRank>();
        }

        public Models.ReputationRank Load(long id)
        {
            if (idLookup.Count == 0)
            {
                idLookup = _dom.GetObject("repRankInfoPrototype").Data.Get<List<object>>("repRankInfoData").ToDictionary(x=> ((GomObjectData)x).Get<long>("repRankInfoId"), x => Load(new ReputationRank(), (GomObjectData)x));
            }
            ReputationRank data = null;
            idLookup.TryGetValue(id, out data);

            return data;
        }

        public Models.ReputationRank Load(Models.ReputationRank rank, GomObjectData obj)
        {
            if (obj == null) { return rank; }
            if (rank == null) { return null; }

            rank.RankId = obj.ValueOrDefault<long>("repRankInfoId", 0);
            rank.RankTitleId = obj.ValueOrDefault<long>("repRankInfoTitle", 0);
            rank.RankTitle = _dom.stringTable.TryGetString("str.lgc.reputation", rank.RankTitleId);
            rank.LocalizedRankTitle = _dom.stringTable.TryGetLocalizedStrings("str.lgc.reputation", rank.RankTitleId);
            rank.RankPoints = obj.ValueOrDefault<long>("repRankInfoPoints", 0);

            return rank;
        }
    }
}
