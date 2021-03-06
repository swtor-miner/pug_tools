﻿using System;
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
        internal string _FqnCategory { get; set; }
        public string FqnCategory
        {
            get
            {
                if (String.IsNullOrEmpty(_FqnCategory))
                {
                    string[] fqnParts = Fqn.Substring(4).Split('.');
                    _FqnCategory = fqnParts[0];
                    _FqnSubCategory = fqnParts[1];
                }
                return _FqnCategory;
            }
        }
        internal string _FqnSubCategory { get; set; }
        public string FqnSubCategory
        {
            get
            {
                if (String.IsNullOrEmpty(_FqnSubCategory))
                {
                    string[] fqnParts = Fqn.Substring(4).Split('.');
                    _FqnCategory = fqnParts[0];
                    _FqnSubCategory = fqnParts[1];
                }
                return _FqnSubCategory;
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

            Placeable plc = obj as Placeable;
            if (plc == null) return false;

            return Equals(plc);
        }
        public bool Equals(Placeable plc)
        {
            if (plc == null) return false;

            if (ReferenceEquals(this, plc)) return true;

            if (this.AuctionNetwork != plc.AuctionNetwork)
                return false;
            if (this.Category != plc.Category)
                return false;
            if (this.CodexId != plc.CodexId)
                return false;
            if (this.ConversationFqn != plc.ConversationFqn)
                return false;
            if (this.DifficultyFlags != plc.DifficultyFlags)
                return false;
            if (this.Faction != plc.Faction)
                return false;
            if (this.Fqn != plc.Fqn)
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

            if (this.Id != plc.Id)
                return false;
            if (this.IsAuctionHouse != plc.IsAuctionHouse)
                return false;
            if (this.IsBank != plc.IsBank)
                return false;
            if (this.IsEnhancementStation != plc.IsEnhancementStation)
                return false;
            if (this.IsMailbox != plc.IsMailbox)
                return false;

            var dComp = new DictionaryComparer<string, string>();
            if (!dComp.Equals(this.LocalizedName, plc.LocalizedName))
                return false;

            if (this.LootLevel != plc.LootLevel)
                return false;
            if (this.LootPackageId != plc.LootPackageId)
                return false;
            if (this.Name != plc.Name)
                return false;
            if (this.RequiredProfession != plc.RequiredProfession)
                return false;
            if (this.RequiredProfessionLevel != plc.RequiredProfessionLevel)
                return false;
            if (this.WonkaPackageId != plc.WonkaPackageId)
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
