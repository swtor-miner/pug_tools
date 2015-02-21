using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GomLib.Models
{
    public enum TargetRule
    {
        //None = -1,
        Self = 0,
        Attackable = 1,
        Friendly = 2,
        Grouped = 3,
        FriendlyDead = 4,
        SelfDead = 5,
        Companion = 6,
        PBAoE = 7,
        Ground = 8,
        CoverObject = 9,
        Any = 10,
        LootHopper = 11,
        CoverObjectLeft = 12,
        CoverObjectRight = 13,
        SelfStunned = 14,
        Vendor = 15,
        Vehicle = 16,
        CcMaster = 17,
        VehicleOwner = 18,
        SelfDeadorAlive = 19,
        GroundSelf = 20
    }

    public static class TargetRuleExtensions
    {
        public static TargetRule ToTargetRule(this ScriptEnum val)
        {
            if (val == null) { return TargetRule.Self; }
            return val.ToString().ToTargetRule();
        }

        public static TargetRule ToTargetRule(this string str)
        {
            if (string.IsNullOrEmpty(str)) return TargetRule.Self;
            switch (str)
            {
                case "tgtRuleSelf": return TargetRule.Self;
                case "tgtRuleAttackable": return TargetRule.Attackable;
                case "tgtRuleFriendly": return TargetRule.Friendly;
                case "tgtRuleGrouped": return TargetRule.Grouped;
                case "tgtRuleFriendlyDead": return TargetRule.FriendlyDead;
                case "tgtRuleSelfDead": return TargetRule.SelfDead;
                case "tgtRuleCompanion": return TargetRule.Companion;
                case "tgtRulePBAoE": return TargetRule.PBAoE;
                case "tgtRuleGround": return TargetRule.Ground;
                case "tgtRuleCoverObject": return TargetRule.CoverObject;
                case "tgtRuleAny": return TargetRule.Any;
                case "tgtRuleLootHopper": return TargetRule.LootHopper;
                case "tgtRuleCoverObjectLeft": return TargetRule.CoverObjectLeft;
                case "tgtRuleCoverObjectRight": return TargetRule.CoverObjectRight;
                case "tgtRuleSelfStunned": return TargetRule.SelfStunned;
                case "tgtRuleVendor": return TargetRule.Vendor;
                case "tgtRuleVehicle": return TargetRule.Vehicle;
                case "tgtRuleCCMaster": return TargetRule.CcMaster;
                case "tgtRuleVehicleOwner": return TargetRule.VehicleOwner;
                case "tgtRuleSelfDeadOrAlive": return TargetRule.SelfDeadorAlive;
                case "tgtRuleGroundSelf": return TargetRule.GroundSelf;
                default: throw new InvalidOperationException("Unknown TargetRule: " + str);
            }
        }
    }
}
