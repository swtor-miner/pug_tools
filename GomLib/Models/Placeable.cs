using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;
using System.Xml;

namespace GomLib.Models
{
    public class Placeable : GameObject, IEquatable<Placeable>
    {
        public string Name { get; set; }
        public Dictionary<string, string> LocalizedName { get; set; }

        public string ConversationFqn { get; set; }
        public ulong CodexId { get; set; }
        public Profession RequiredProfession { get; set; }
        public int RequiredProfessionLevel { get; set; }
        public bool IsBank { get; set; }
        public bool IsMailbox { get; set; }
        public bool IsAuctionHouse { get; set; }
        public bool IsEnhancementStation { get; set; }
        public AuctionHouseNetwork AuctionNetwork { get; set; }
        public Faction Faction { get; set; }
        public int LootLevel { get; set; }
        public long LootPackageId { get; set; }
        public long WonkaPackageId { get; set; }
        public int DifficultyFlags { get; set; }
        public PlaceableCategory Category { get; set; }

        public HydraScript HydraScript { get; set; }

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
    }
}
