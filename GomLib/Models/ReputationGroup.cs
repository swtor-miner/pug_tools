using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Newtonsoft.Json;

namespace GomLib.Models
{
    public class ReputationGroup : PseudoGameObject, IEquatable<ReputationGroup>
    {
        //repGroupInfosPrototype - repGroupInfoData - class lookup

        //General Fields
        [JsonConverter(typeof(LongConverter))]
        public long GroupId { get; set; }                                    //repGroupInfoId - long : 4611686297436250001
        public string GroupFactionAvailability { get; set; }                 //repGroupInfoFactionAvailibilityEnum - enum : 4611686297436250007 - repFactionBoth
        public long GroupReputationWeeklyLimit { get; set; }                 //repGroupInfoWeeklyRepLimit - long : 4611686297719284000 - 12000

        //Republic Specific Fields
        [JsonConverter(typeof(LongConverter))]
        public long GroupRepublicDescriptionId { get; set; }                 //repGroupInfoRepublicDescription - long lookup : 4611686297436250005 - str.lgc.reputation - 3171541290320167
        [JsonConverter(typeof(LongConverter))]
        public long GroupRepublicTitleId { get; set; }                       //repRankInfoTitleId - long lookup : 4611686297436250003 - str.lgc.reputation - 3171541290320165
        public string GroupRepublicDescription { get; set; }                 //repRankInfoTitle - "Outsider"
        public string GroupRepublicTitle { get; set; }                       //repRankInfoTitle - "Outsider"        
        public Dictionary<string, string> LocalizedGroupRepublicTitle { get; set; }
        public List<object> GroupRepublicRankTitles { get; set; }            //repGroupInfoRepublicRankTitles - 4611686297436250010 - repRankInfoId => lgcTitleLookupId    
        public Dictionary<object, object> GroupRepublicRankLegacyTitles { get; set; }
        public string GroupRepublicIcon { get; set; }                        //repGroupInfoIconRepublic - string : 4611686297701794046 - "rep.makeb.republic"        

        //Empire Specific Fields
        [JsonConverter(typeof(LongConverter))]
        public long GroupEmpireDescriptionId { get; set; }                   //repGroupInfoEmpireDescriptionId - long lookup : 4611686297436250004 - str.lgc.reputation - 3171541290320167
        [JsonConverter(typeof(LongConverter))]
        public long GroupEmpireTitleId { get; set; }                         //repGroupInfoEmpireTitleId - long lookup : 4611686297436250002 - str.lgc.reputation - 3171541290320165
        public string GroupEmpireDescription { get; set; }                   //repGroupInfoEmpireDescription - "Outsider"
        public string GroupEmpireTitle { get; set; }                         //repGroupInfoEmpireTitle - "Outsider"      
        public Dictionary<string, string> LocalizedGroupEmpireTitle { get; set; }
        public List<object> GroupEmpireRankTitles { get; set; }              //repGroupInfoEmpireRankTitles - repRankInfoId => lgcTitleLookupId    
        public Dictionary<object, object> GroupEmpireRankLegacyTitles { get; set; }
        public string GroupEmpireIcon { get; set; }                          //repGroupInfoIconEmpire - string : 4611686297701794046 - "rep.makeb.empire"                
        public Dictionary<string, string> LocalizedGroupleTitle { get; set; }

        public override bool Equals(object obj)
        {            
            if (obj == null) return false;

            if (ReferenceEquals(this, obj)) return true;

            ReputationGroup rank = obj as ReputationGroup;
            if (rank == null) return false;

            return Equals(rank);
        }

        public bool Equals(ReputationGroup group)
        {
            return true;

            if (group == null) return false;

            if (ReferenceEquals(this, group)) return true;

            if (this.GroupId != group.GroupId)
                return false;
            if (this.GroupFactionAvailability != group.GroupFactionAvailability)
                return false;
            if (this.GroupReputationWeeklyLimit != group.GroupReputationWeeklyLimit)
                return false;
            if (this.GroupRepublicDescriptionId != group.GroupRepublicDescriptionId)
                return false;
            if (this.GroupRepublicTitleId != group.GroupRepublicTitleId)
                return false;
            if (this.GroupRepublicDescription != group.GroupRepublicDescription)
                return false;
            if (this.GroupRepublicTitle != group.GroupRepublicTitle)
                return false;
            if (this.GroupRepublicRankTitles != group.GroupRepublicRankTitles)
                return false;
            if (this.GroupRepublicIcon != group.GroupRepublicIcon)
                return false;
            if (this.GroupEmpireDescriptionId != group.GroupEmpireDescriptionId)
                return false;
            if (this.GroupEmpireTitleId != group.GroupEmpireTitleId)
                return false;
            if (this.GroupEmpireDescription != group.GroupEmpireDescription)
                return false;
            if (this.GroupEmpireTitle != group.GroupEmpireTitle)
                return false;
            if (this.GroupEmpireRankTitles != group.GroupEmpireRankTitles)
                return false;
            if (this.GroupEmpireIcon != group.GroupEmpireIcon)
                return false;

            return true;             
        }
    }
}
