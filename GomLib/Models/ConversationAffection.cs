using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GomLib.Models
{
    public enum ConversationAffection
    {
        None = 0,
        SmallIncrease = 1,
        MediumIncrease = 2,
        LargeIncrease = 3,
        SmallDecrease = 4,
        MediumDecrease = 5,
        LargeDecrease = 6
    }

    public static class ConversationAffectionExtensions
    {
        public static ConversationAffection ToConversationAffection(this long val)
        {
            switch (val)
            {
                case -5301803456931428419: return ConversationAffection.LargeDecrease;
                case -4783781478483316801: return ConversationAffection.LargeIncrease;
                case -4591313547948601546: return ConversationAffection.MediumDecrease;
                case -4560859075783160276: return ConversationAffection.MediumIncrease;
                case -1761015921176014301: return ConversationAffection.SmallIncrease;
                case -1279836118038687359: return ConversationAffection.SmallDecrease;
                default: return ConversationAffection.None;
            }
        }
    }
}
