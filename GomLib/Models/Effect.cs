using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;
using System.Xml.XPath;
using System.Text.RegularExpressions;
using System.Xml;
using Newtonsoft.Json;

namespace GomLib.Models
{
    public class Effect : GameObject
    {
        //Fields that are directly contained in the eff node, ordered by field id
        [JsonConverter(typeof(LongConverter))]
        public long Number { get; set; }
        public bool Hidden { get; set; }
        public bool Passive { get; set; }
        [JsonConverter(typeof(ULongConverter))]
        public ulong Duration { get; set; }
        public bool IsDurationRealtime { get; set; }
        [JsonConverter(typeof(ULongConverter))]
        public ulong Interval { get; set; }
        [JsonConverter(typeof(LongConverter))]
        public long InitialCharges { get; set; }
        [JsonConverter(typeof(LongConverter))]
        public long MaxCharges { get; set; }
        [JsonConverter(typeof(LongConverter))]
        public long StackLimit { get; set; }
        public bool StackLimitIsByTag { get; set; }
        public bool StackLimitIsByCaster { get; set; }
        public EffectSlot SlotType { get; set; }
        public bool DoesPersistAfterDeath { get; set; }
        [JsonConverter(typeof(ULongConverter))]
        public List<ulong> CustomIntervalPerSwing { get; set; } //4611686019692990768, duplicated values from effInitializer->effTimeIntervalParams for faster processing
        public bool StackLimitIsMultiTarget { get; set; }
        [JsonConverter(typeof(ULongConverter))]
        public ulong ProjectileTravelSpeed { get; set; }
        public bool IsReverse { get; set; }
        public List<SubEffectEppDetail> SubEffectEppDetails { get; set; }
        public Dictionary<long, bool> StackLimitRelevantTags { get; set; }
        public float AuraDistance { get; set; }
        public bool IsDurationHidden { get; set; }
        public string Icon { get; set; }
        [JsonConverter(typeof(LongConverter))]
        public long Charges { get; set; }
        public List<SubEffect> SubEffects { get; set; }
        public List<long> Tags { get; set; }
        public bool IgnoresCover { get; set; }
        [JsonConverter(typeof(ULongConverter))]
        public ulong GCD { get; set; }
        [JsonConverter(typeof(LongConverter))]
        public long unknownLong1 { get; set; } //4611686051963471065, duplicate value of effInitializer->SetStaminaCost; not used
        public bool IsDebuff { get; set; }
        [JsonConverter(typeof(ULongConverter))]
        public ulong selfReference { get; set; }
        [JsonConverter(typeof(ULongConverter))]
        public ulong AbilitySpec { get; set; }
        [JsonConverter(typeof(ULongConverter))]
        public ulong Hydra { get; set; }
        [JsonConverter(typeof(ULongConverter))]
        public ulong DurationAddedDelay { get; set; }
        public string DurationAddedDelayMaxToughness { get; set; }
        public bool HasStackLimit { get; set; }
        public bool IsInstant { get; set; }
        public bool IsInterruptible { get; set; }
        [JsonConverter(typeof(LongConverter))]
        public long NameStringId { get; set; }
        [JsonConverter(typeof(LongConverter))]
        public long DescriptionStringId { get; set; }
        public bool IsUseableOnTaxi { get; set; }
        public bool unknownBool3 { get; set; } //4611686085914561591
        public bool unknownBool1 { get; set; } //4611686297079480000 - True when it exists
        public bool unknownBool2 { get; set; } //4611686297079480001 - True when it exists
        public bool unknownBool5 { get; set; } //4611686299759854002
        public bool unknownBool6 { get; set; } //4611686299759854003
        public bool unknownBool7 { get; set; } //4611686299759854004
        public bool unknownBool4 { get; set; } //4611686300275404002

        //pseudo fields that are added by tor_tools
        //public Dictionary<string, bool> ParsedStackLimitRelevantTags { get; set; } //DEPRECATED
        public List<string> ParsedTags { get; set; }
        public string Name { get; set; }
        public Dictionary<string, string> LocalizedName { get; set; }
        public string Description { get; set; }
        public Dictionary<string, string> LocalizedDescription { get; set; }
        public bool IsRootEffect { get; set; }
        public List<long> ChildEffects { get; set; }

        public override int GetHashCode()
        {
            int hash = Duration.GetHashCode();
            if (Description != null) { hash ^= Description.GetHashCode(); }
            hash ^= SlotType.GetHashCode();
            hash ^= Interval.GetHashCode();
            hash ^= StackLimit.GetHashCode();
            if (SubEffectEppDetails != null) { hash ^= (SubEffectEppDetails.GetHashCode()).GetHashCode(); }
            hash ^= (SubEffects.GetHashCode()).GetHashCode();
            hash ^= Tags.GetHashCode();
            hash ^= selfReference.GetHashCode();
            hash ^= AbilitySpec.GetHashCode();
            hash ^= NameStringId.GetHashCode();
            if (Name != null) { hash ^= Name.GetHashCode(); }
            hash ^= DescriptionStringId.GetHashCode();
            if (Description != null) { hash ^= Description.GetHashCode(); }
            return hash;
        }

        public override string ToString()
        {
            return string.Format("{0} {1} {2}", NameStringId, Name, Description);
        }
    }

    //Which container slot this effect should be placed in
    public enum EffectSlot
    {
        conSlotEffectPositive = 23,//This effect is positive and should be in the buff bar
        conSlotEffectNegative = 24,//This effect is negative and should be in the debuff bar
        conSlotEffectOther = 29//This effect is should be placed in the other effect bar that is not visible in the UI
    }

    public class SubEffectEppDetail
    {
        [JsonConverter(typeof(LongConverter))]
        public long Index { get; set; }
        public bool Dependent { get; set; }
        public string EppSpec { get; set; }
        public bool OnApply { get; set; }

        [JsonConverter(typeof(ULongConverter))]
        public ulong EppId { get; set; }

        public override int GetHashCode()
        {
            int hash = OnApply.GetHashCode();
            if (EppSpec != null) { hash ^= EppSpec.GetHashCode(); }
            return hash;
        }

        public override string ToString()
        {
            return string.Format("{0}", EppSpec);
        }
    }

    public class SubEffect
    {
        public List<SubEffectFunction> Actions { get; set; }
        public List<SubEffectFunction> TargetOverrides { get; set; }
        public List<SubEffectFunction> Triggers { get; set; }
        public List<SubEffectFunction> Initializers { get; set; }
        public List<SubEffectFunction> Conditions { get; set; }
        public List<ulong> ConditionOrder { get; set; }

        /*public override long GetHashCode() Disabled till I get time to parse all possible values
        {
            long hash = Script_NumFields.GetHashCode();
            hash ^= Script_Type.GetHashCode();
            hash ^= Script_TypeId.GetHashCode();
            //if (effSubEffectEppSpec != null) { hash ^= effSubEffectEppSpec.GetHashCode(); }
            //if (effSubEffectEppOnApply != null) { hash ^= effSubEffectEppOnApply.GetHashCode(); }
            return hash;
        }*/

        public override string ToString()
        {
            return string.Format("{0}", "");
        }
    }

    public class SubEffectFunction
    {
        [JsonConverter(typeof(LongConverter))]
        public long Type { get; set; }
        public List<SubEffectFunctionParam> Params { get; set; }
        public List<long> Tags { get; set; }
        //TODO: unknown 4611686346551820000

        //Only for conditions
        [JsonConverter(typeof(ULongConverter))]
        public ulong CondId { get; set; }
        [JsonConverter(typeof(LongConverter))]
        public long FailureStringId { get; set; }
        public string FailureString { get; set; }
        public Dictionary<string, string> FailureLocalizedString { get; set; }

        public List<string> ParsedTags { get; set; }

        public override int GetHashCode()
        {
            int hash = "".GetHashCode();
            return hash;
        }

        public override string ToString()
        {
            return string.Format("{0}", "");
        }
    }

    public class SubEffectFunctionParam
    {
        [JsonConverter(typeof(LongConverter))]
        public long Key { get; set; }
        public int Type { get; set; }
        public object Value { get; set; }
        public SubEffectFunctionParam(long key, int type, object value)
        {
            this.Key = key;
            this.Type = type;
            this.Value = value;
        }
    }
}