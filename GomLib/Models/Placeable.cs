using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;
using System.Xml;
using Newtonsoft.Json;

namespace GomLib.Models
{
    public class Placeable : GameObject, IEquatable<Placeable>
    {
        public string Name { get; set; }
        public Dictionary<string, string> LocalizedName { get; set; }

        public string ConversationFqn { get; set; }
        [JsonConverter(typeof(ULongConverter))]
        public ulong CodexId { get; set; }
        [JsonConverter(typeof(Newtonsoft.Json.Converters.StringEnumConverter))]
        public Profession RequiredProfession { get; set; }
        public int RequiredProfessionLevel { get; set; }
        public bool IsBank { get; set; }
        public bool IsMailbox { get; set; }
        public bool IsAuctionHouse { get; set; }
        public bool IsEnhancementStation { get; set; }
        [JsonConverter(typeof(Newtonsoft.Json.Converters.StringEnumConverter))]
        public AuctionHouseNetwork AuctionNetwork { get; set; }
        [JsonConverter(typeof(Newtonsoft.Json.Converters.StringEnumConverter))]
        public Faction Faction { get; set; }
        public int LootLevel { get; set; }
        public long LootPackageId { get; set; }
        public long WonkaPackageId { get; set; }
        public int DifficultyFlags { get; set; }
        [JsonConverter(typeof(Newtonsoft.Json.Converters.StringEnumConverter))]
        public PlaceableCategory Category { get; set; }

        [JsonIgnore]
        public HydraScript HydraScript { get; set; }
        internal string FqnCategory_ { get; set; }
        public string FqnCategory
        {
            get
            {
                if (string.IsNullOrEmpty(FqnCategory_))
                {
                    string[] fqnParts = Fqn.Substring(4).Split('.');
                    FqnCategory_ = fqnParts[0];
                    FqnSubCategory_ = fqnParts[1];
                }
                return FqnCategory_;
            }
        }
        internal string FqnSubCategory_ { get; set; }
        public string FqnSubCategory
        {
            get
            {
                if (string.IsNullOrEmpty(FqnSubCategory_))
                {
                    string[] fqnParts = Fqn.Substring(4).Split('.');
                    FqnCategory_ = fqnParts[0];
                    FqnSubCategory_ = fqnParts[1];
                }
                return FqnSubCategory_;
            }
        }

        #region IEquatable
        public override int GetHashCode()
        {
            int result = Id.GetHashCode();
            result ^= Name.GetHashCode();
            if (ConversationFqn != null) { result ^= ConversationFqn.GetHashCode(); }
            if (CodexId != 0) { result ^= CodexId.GetHashCode(); }
            result ^= RequiredProfession.GetHashCode();
            result ^= RequiredProfessionLevel.GetHashCode();
            result ^= IsBank.GetHashCode();
            result ^= IsMailbox.GetHashCode();
            result ^= IsAuctionHouse.GetHashCode();
            result ^= IsEnhancementStation.GetHashCode();
            result ^= AuctionNetwork.GetHashCode();
            result ^= Faction.GetHashCode();
            result ^= LootLevel.GetHashCode();
            result ^= LootPackageId.GetHashCode();
            result ^= WonkaPackageId.GetHashCode();
            result ^= DifficultyFlags.GetHashCode();
            result ^= Category.GetHashCode();

            return result;
        }
        public override bool Equals(object obj)
        {
            if (obj == null) return false;

            if (ReferenceEquals(this, obj)) return true;

            if (!(obj is Placeable plc)) return false;

            return Equals(plc);
        }
        public bool Equals(Placeable plc)
        {
            if (plc == null) return false;

            if (ReferenceEquals(this, plc)) return true;

            if (AuctionNetwork != plc.AuctionNetwork)
                return false;
            if (Category != plc.Category)
                return false;
            if (CodexId != plc.CodexId)
                return false;
            if (ConversationFqn != plc.ConversationFqn)
                return false;
            if (DifficultyFlags != plc.DifficultyFlags)
                return false;
            if (Faction != plc.Faction)
                return false;
            if (Fqn != plc.Fqn)
                return false;
            /*if (this.HydraScript != null)
            {
                if (plc.HydraScript == null)
                {
                    return false;
                }
                else
                {
                    if (!this.HydraScript.Equals(plc.HydraScript))
                        return false;
                }
            }
            else if (plc.HydraScript != null)
                return false;*/

            if (Id != plc.Id)
                return false;
            if (IsAuctionHouse != plc.IsAuctionHouse)
                return false;
            if (IsBank != plc.IsBank)
                return false;
            if (IsEnhancementStation != plc.IsEnhancementStation)
                return false;
            if (IsMailbox != plc.IsMailbox)
                return false;

            var dComp = new DictionaryComparer<string, string>();
            if (!dComp.Equals(LocalizedName, plc.LocalizedName))
                return false;

            if (LootLevel != plc.LootLevel)
                return false;
            if (LootPackageId != plc.LootPackageId)
                return false;
            if (Name != plc.Name)
                return false;
            if (RequiredProfession != plc.RequiredProfession)
                return false;
            if (RequiredProfessionLevel != plc.RequiredProfessionLevel)
                return false;
            if (WonkaPackageId != plc.WonkaPackageId)
                return false;
            return true;
        }
        #endregion

        public override List<SQLProperty> SQLProperties
        {
            get
            {
                return new List<SQLProperty>
                    {                //(SQL Column Name, C# Property Name, SQL Column type statement, isUnique/PrimaryKey, Serialize value to json)
                        new SQLProperty("Name", "Name", "varchar(255) COLLATE utf8_unicode_ci NOT NULL", SQLPropSetting.AddIndex),
                        new SQLProperty("FrName", "LocalizedName[frMale]", "varchar(255) COLLATE utf8_unicode_ci NOT NULL", SQLPropSetting.AddIndex),
                        new SQLProperty("DeName", "LocalizedName[deMale]", "varchar(255) COLLATE utf8_unicode_ci NOT NULL", SQLPropSetting.AddIndex),
                        new SQLProperty("Base62Id", "Base62Id", "varchar(7) COLLATE utf8_unicode_ci NOT NULL", SQLPropSetting.PrimaryKey),
                        new SQLProperty("FqnCategory", "FqnCategory", "varchar(255) COLLATE utf8_unicode_ci NOT NULL", SQLPropSetting.AddIndex),
                        new SQLProperty("FqnSubCategory", "FqnSubCategory", "varchar(255) COLLATE utf8_unicode_ci NOT NULL", SQLPropSetting.AddIndex),
                        new SQLProperty("IsBank", "IsBank", "tinyint(1) NOT NULL", SQLPropSetting.AddIndex),
                    };
            }
        }

        public string TemplateComment { get; set; }
        public bool TemplateNoGlow { get; set; }
        public long PropState { get; set; }
        [JsonIgnore]
        public ulong AbilitySpecOnUseId { get; set; }
        public string AbilitySpecOnUseB62Id { get { return AbilitySpecOnUseId.ToMaskedBase62(); } }

        public string Model { get; internal set; }
    }
}
