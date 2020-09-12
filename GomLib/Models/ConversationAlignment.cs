using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GomLib.Models
{
    public enum ConversationAlignment
    {
        None = 0,
        SmallLight = 1,
        MediumLight = 2,
        LargeLight = 3,
        MegaLight = 4,
        SmallDark = 5,
        MediumDark = 6,
        LargeDark = 7,
        MegaDark = 8
    }

    public static class ConversationAlignmentExtensions
    {
        public static ConversationAlignment ToConversationAlignment(long amount, string forceType)
        {
            switch (forceType)
            {
                case "cnvForceRewardTypeDarkSide":
                    {
                        switch (amount)
                        {
                            case 2959999436154067372: return ConversationAlignment.SmallDark;   // 50 DSP
                            case 589686270506543030: return ConversationAlignment.MediumDark;   // 100 DSP
                            case -2493745135610518796: return ConversationAlignment.LargeDark;  // 150 DSP
                            case 5787183427540372334: return ConversationAlignment.MegaDark;    // 200 DSP
                        }
                    }
                    break;
                case "cnvForceRewardTypeLightSide":
                    {
                        switch (amount)
                        {
                            case 2959999436154067372: return ConversationAlignment.SmallLight;  // 50 LSP
                            case 589686270506543030: return ConversationAlignment.MediumLight;  // 100 LSP
                            case -2493745135610518796: return ConversationAlignment.LargeLight; // 150 LSP
                            case 5787183427540372334: return ConversationAlignment.MegaLight;   // 200 LSP
                        }
                    }
                    break;
                case "0x00":
                    {
                        switch (amount)
                        {
                            case 2959999436154067372: return ConversationAlignment.SmallLight;  // 50 LSP
                            case 589686270506543030: return ConversationAlignment.MediumLight;  // 100 LSP
                            case -2493745135610518796: return ConversationAlignment.LargeLight; // 150 LSP
                            case 5787183427540372334: return ConversationAlignment.MegaLight;   // 200 LSP
                        }
                    }
                    break;
            }
            throw new InvalidOperationException(string.Format("Unknown ConversationAlignment: forceType = {0}, amount = {1}", forceType, amount));
        }
    }
}
