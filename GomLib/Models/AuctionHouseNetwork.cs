using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GomLib.Models
{
    public enum AuctionHouseNetwork
    {
        None = 0,
        Empire = 2,
        Republic = 3,
        Neutral = 4
    }

    public static class AuctionHouseNetworkExtensions
    {
        public static AuctionHouseNetwork ToAuctionHouseNetwork(this string str)
        {
            if (String.IsNullOrEmpty(str)) return AuctionHouseNetwork.None;

            switch (str)
            {
                case "ahNetworkNone": return AuctionHouseNetwork.None;
                case "ahNetworkEmpire": return AuctionHouseNetwork.Empire;
                case "ahNetworkRepublic": return AuctionHouseNetwork.Republic;
                case "ahNetworkNeutral": return AuctionHouseNetwork.Neutral;
                default:
                    throw new InvalidOperationException("Unknown AuctionHouseNetwork: " + str);
            }
        }

        public static AuctionHouseNetwork ToAuctionHouseNetwork(ScriptEnum val)
        {
            if (val == null) { return AuctionHouseNetwork.None; }

            switch (val.Value)
            {
                case 0: return AuctionHouseNetwork.None;
                case 2: return AuctionHouseNetwork.Empire;
                case 3: return AuctionHouseNetwork.Republic;
                case 4: return AuctionHouseNetwork.Neutral;
                default:
                    throw new InvalidOperationException("Unknown AuctionHouseNetwork Value: " + val.Value);
            }
        }
    }
}
