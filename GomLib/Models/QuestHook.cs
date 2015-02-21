using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GomLib.Models
{
    public enum QuestHook
    {
        None,
        OnDeath,
        OnLoot,
        OnStart,
        OnEnd,
        OnQuestComplete,
        OnHydra,
        OnTimerExpired,
        OnUse,
        OnSurrender,
        ForceConversation,
        OnStartAbandoned,
        OnCrefacCleared,
        Progress1,
        Progress2,
        Progress3,
        Progress4,
        Progress5,
        Progress6,
        Progress7,
        Progress8,
        Progress9,
        Progress10,
        Progress11,
        Progress12,
        Progress13,
        Progress14,
        Progress15,
        Progress16,
        Progress17,
        Progress18,
        Progress19,
        Progress20,
        Progress22,
        Progress23,
        Progress31
    }

    public static class QuestHookExtensions
    {
        public static QuestHook ToQuestHook(this string str)
        {
            if (String.IsNullOrEmpty(str)) { return QuestHook.None; }

            switch (str)
            {
                case "On Death": return QuestHook.OnDeath;
                case "On Start": return QuestHook.OnStart;
                case "On Loot": return QuestHook.OnLoot;
                case "On End": return QuestHook.OnEnd;
                case "On Quest Complete": return QuestHook.OnQuestComplete;
                case "On Hydra": return QuestHook.OnHydra;
                case "On Timer Expired": return QuestHook.OnTimerExpired;
                case "On Use": return QuestHook.OnUse;
                case "On Surrender": return QuestHook.OnSurrender;
                case "ForceConversation": return QuestHook.ForceConversation;
                case "On Start Abandoned": return QuestHook.OnStartAbandoned;
                case "On Crefac Cleared": return QuestHook.OnCrefacCleared;
                case "Progress 1": return QuestHook.Progress1;
                case "Progress 2": return QuestHook.Progress2;
                case "Progress 3": return QuestHook.Progress3;
                case "Progress 4": return QuestHook.Progress4;
                case "Progress 5": return QuestHook.Progress5;
                case "Progress 6": return QuestHook.Progress6;
                case "Progress 7": return QuestHook.Progress7;
                case "Progress 8": return QuestHook.Progress8;
                case "Progress 9": return QuestHook.Progress9;
                case "Progress 10": return QuestHook.Progress10;
                case "Progress 11": return QuestHook.Progress11;
                case "Progress 12": return QuestHook.Progress12;
                case "Progress 13": return QuestHook.Progress13;
                case "Progress 14": return QuestHook.Progress14;
                case "Progress 15": return QuestHook.Progress15;
                case "Progress 16": return QuestHook.Progress16;
                case "Progress 17": return QuestHook.Progress17;
                case "Progress 18": return QuestHook.Progress18;
                case "Progress 19": return QuestHook.Progress19;
                case "Progress 20": return QuestHook.Progress20;
                case "Progress 22": return QuestHook.Progress22;
                case "Progress 23": return QuestHook.Progress23;
                case "Progress 31": return QuestHook.Progress31;
                default: throw new InvalidOperationException("Unknown QuestHook: " + str);
            }
        }
    }
}
