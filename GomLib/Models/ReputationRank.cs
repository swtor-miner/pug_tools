using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GomLib.Models
{
    public class ReputationRank : PseudoGameObject, IEquatable<ReputationRank>
    {
        //repRankInfoPrototype - repRankInfoData - class lookup

        public long RankId { get; set; }       //repRankInfoId - long : 4611686297441920001
        public long RankPoints { get; set; }   //repRankInfoPoints - long : 4611686297441920002        
        public long RankTitleId { get; set; }  //repRankInfoTitleId - long lookup : 4611686297441920004 - str.lgc.reputation - 3171541290320128
        public string RankTitle { get; set; }  //repRankInfoTitle - "Outsider"
        public Dictionary<string, string> LocalizedRankTitle { get; set; }
       
        public override bool Equals(object obj)
        {
            if (obj == null) return false;

            if (ReferenceEquals(this, obj)) return true;

            ReputationRank rank = obj as ReputationRank;
            if (rank == null) return false;

            return Equals(rank);
        }

        public bool Equals(ReputationRank rank)
        {
            if (rank == null) return false;

            if (ReferenceEquals(this, rank)) return true;

            if (this.RankPoints != rank.RankPoints)
                return false;
            if (this.RankTitle != rank.RankTitle)
                return false;
            if (this.RankId != rank.RankId)
                return false;
            if (this.RankTitleId != rank.RankTitleId)
                return false;

            return true;
        }
    }
}
