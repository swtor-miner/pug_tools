using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GomLib.Models;

namespace GomLib.ModelLoader
{
    public class EffectLoader : IModelLoader
    {
        const long NameLookupKey = -2761358831308646330;
        const long DescLookupKey = 2806211896052149513;

        int tokenNumber = 1;

        private DataObjectModel _dom;

        public EffectLoader(DataObjectModel dom)
        {
            _dom = dom;
        }

        public void Flush()
        {
        }

        public string ClassName
        {
            get { return "effEffect"; }
        }

        public Models.Effect Load(ulong nodeId)
        {
            GomObject obj = _dom.GetObject(nodeId);
            Models.Effect model = new Effect();
            return Load(model, obj);
        }

        public Models.Effect Load(string fqn)
        {
            GomObject obj = _dom.GetObject(fqn);
            Models.Effect model = new Effect();
            return Load(model, obj);
        }

        public Models.GameObject CreateObject()
        {
            return new Models.Effect();
        }

        public Models.Effect Load(Models.GameObject obj, GomObject gom)
        {
            if (gom == null) { return (Effect)obj; }
            if (obj == null) { return null; }

            var eff = (Effect)obj;

            eff._dom = _dom;
            eff.References = gom.References;
            eff.Fqn = gom.Name;
            eff.Id = gom.Id;

            eff.Number = gom.Data.ValueOrDefault<long>("effNumber");
            eff.Hidden = gom.Data.ValueOrDefault<bool>("effIsHidden");
            eff.Passive = gom.Data.ValueOrDefault<bool>("effIsPassive");
            eff.Duration = gom.Data.ValueOrDefault<ulong>("effDuration");
            eff.IsDurationRealtime = gom.Data.ValueOrDefault<bool>("effIsDurationRealtime");
            eff.Interval = gom.Data.ValueOrDefault<ulong>("effInterval");
            eff.InitialCharges = gom.Data.ValueOrDefault<long>("effInitialCharges");
            eff.MaxCharges = gom.Data.ValueOrDefault<long>("effMaxCharges");
            eff.StackLimit = gom.Data.ValueOrDefault<long>("effStackLimit");
            eff.StackLimitIsByTag = gom.Data.ValueOrDefault<bool>("effStackLimitIsByTag");
            eff.StackLimitIsByCaster = gom.Data.ValueOrDefault<bool>("effStackLimitIsByCaster");
            eff.SlotType = (EffectSlot)gom.Data.ValueOrDefault<ScriptEnum>("effSlotType", new ScriptEnum()).Value;
            eff.DoesPersistAfterDeath = gom.Data.ValueOrDefault<bool>("effDoesPersistAfterDeath");
            List<object> customIntervalPerSwing = gom.Data.ValueOrDefault<List<object>>("4611686019692990768");
            if (customIntervalPerSwing != null)
            {
                eff.CustomIntervalPerSwing = new List<ulong>();
                foreach (ulong interval in customIntervalPerSwing)
                {
                    eff.CustomIntervalPerSwing.Add((ulong)interval);
                }
            }
            eff.StackLimitIsMultiTarget = gom.Data.ValueOrDefault<bool>("effStackLimitIsMultiTarget");
            eff.ProjectileTravelSpeed = gom.Data.ValueOrDefault<ulong>("effProjectileTravelSpeed");
            eff.IsReverse = gom.Data.ValueOrDefault<bool>("effIsReverse");
            Dictionary<object, object> effSubEffectEppDetails = gom.Data.ValueOrDefault<Dictionary<object, object>>("effSubEffectEppDetails");
            if (effSubEffectEppDetails != null)
            {
                eff.SubEffectEppDetails = new List<SubEffectEppDetail>();
                foreach (KeyValuePair<object, object> detail in effSubEffectEppDetails)
                {
                    GomObjectData tmpValue = (GomObjectData)detail.Value;
                    SubEffectEppDetail curDetail = new SubEffectEppDetail();
                    curDetail.Index = (long)detail.Key;
                    curDetail.Dependent = tmpValue.ValueOrDefault<bool>("effSubEffectEppDependent");
                    curDetail.EppSpec = tmpValue.ValueOrDefault<string>("effSubEffectEppSpec");
                    curDetail.OnApply = tmpValue.ValueOrDefault<bool>("effSubEffectEppDetails");
                    if (curDetail.EppSpec != null)
                    {
                        GomObject eppObj = _dom.GetObject(curDetail.EppSpec);
                        if (eppObj != null) curDetail.EppId = eppObj.Id;
                    }
                    eff.SubEffectEppDetails.Add(curDetail);
                }
            }
            eff.AuraDistance = gom.Data.ValueOrDefault<float>("effAuraDistance");
            eff.IsDurationHidden = gom.Data.ValueOrDefault<bool>("effIsDurationHidden");
            //var effStackLimitRelevantTags = gom.Data.ValueOrDefault<Dictionary<object,object>>("effStackLimitRelevantTags", new Dictionary<object,object>());
            //eff.StackLimitRelevantTags = effStackLimitRelevantTags.ToDictionary(k => (long)k.Key, v => (bool)v.Value);
            //DEPRECATED - effStackLimitRelevantTags is no longer used in the current client
            eff.Icon = gom.Data.ValueOrDefault<string>("effIcon");
            eff.Charges = gom.Data.ValueOrDefault<long>("effCharges");
            List<object> effSubEffects = gom.Data.ValueOrDefault<List<object>>("effSubEffects", new List<object>());
            eff.SubEffects = new List<SubEffect>();
            foreach (GomObjectData subeffect in effSubEffects)
            {
                SubEffect outSubEffect = new SubEffect();
                outSubEffect.Actions = new List<SubEffectFunction>();//-----------------------------------------------------
                List<object> actionList = subeffect.ValueOrDefault<List<object>>("effActions", new List<object>());
                foreach (GomObjectData action in actionList)
                {
                    outSubEffect.Actions.Add(LoadSubEffectFunction(action, "effActionName"));
                }
                outSubEffect.TargetOverrides = new List<SubEffectFunction>();//-----------------------------------------------------
                List<object> targetOverridesList = subeffect.ValueOrDefault<List<object>>("effTargetOverrides", new List<object>());
                foreach (GomObjectData targetOverride in targetOverridesList)
                {
                    outSubEffect.TargetOverrides.Add(LoadSubEffectFunction(targetOverride, "effTargetOverrideName"));
                }
                outSubEffect.Triggers = new List<SubEffectFunction>();//-----------------------------------------------------
                List<object> triggersList = subeffect.ValueOrDefault<List<object>>("effTriggers", new List<object>());
                foreach (GomObjectData trigger in triggersList)
                {
                    outSubEffect.Triggers.Add(LoadSubEffectFunction(trigger, "effTriggerName"));
                }
                outSubEffect.Initializers = new List<SubEffectFunction>();//-----------------------------------------------------
                List<object> initializersList = subeffect.ValueOrDefault<List<object>>("effInitializers", new List<object>());
                foreach (GomObjectData initializer in initializersList)
                {
                    outSubEffect.Initializers.Add(LoadSubEffectFunction(initializer, "effInitializerName"));
                }
                outSubEffect.Conditions = new List<SubEffectFunction>();//-----------------------------------------------------
                Dictionary<object, object> conditionsList = subeffect.ValueOrDefault<Dictionary<object, object>>("effConditions", new Dictionary<object, object>());
                foreach (KeyValuePair<object, object> kvp in conditionsList)
                {
                    outSubEffect.Conditions.Add(LoadSubEffectFunction((GomObjectData)kvp.Value, "effConditionName", (ulong)kvp.Key));
                }
                List<object> conditionLogic = subeffect.ValueOrDefault<List<object>>("effConditionLogic");
                if (conditionLogic != null)
                {
                    outSubEffect.ConditionOrder = new List<ulong>();
                    foreach (GomObjectData condLogicObj in conditionLogic)
                    {
                        //condLogicObj.ValueOrDefault<long>("effConditionLogicType"); //not necessary, can determine operator based on value
                        outSubEffect.ConditionOrder.Add(condLogicObj.ValueOrDefault<ulong>("effConditionLogicValue"));
                    }
                }
                eff.SubEffects.Add(outSubEffect);
            }
            Dictionary<object, object> effTags = gom.Data.ValueOrDefault<Dictionary<object, object>>("effTags");
            eff.Tags = new List<long>();
            if (effTags != null)
            {
                foreach (KeyValuePair<object, object> kvp in effTags)
                {
                    //if ((bool)kvp.Value != true) throw new Exception("ERROR: Expected function tag to be true, but it is false"); //not really necessary
                    eff.Tags.Add((long)kvp.Key);
                }
            }
            eff.IgnoresCover = gom.Data.ValueOrDefault<bool>("effIgnoresCover");
            eff.GCD = gom.Data.ValueOrDefault<ulong>("effGCD");
            eff.unknownLong1 = gom.Data.ValueOrDefault<long>("4611686051963471065");
            eff.IsDebuff = gom.Data.ValueOrDefault<bool>("effIsDebuff");
            eff.selfReference = gom.Data.ValueOrDefault<ulong>("conEntitySpec");
            eff.AbilitySpec = gom.Data.ValueOrDefault<ulong>("effAbilitySpec");
            eff.Hydra = gom.Data.ValueOrDefault<ulong>("effHydra");
            eff.DurationAddedDelay = gom.Data.ValueOrDefault<ulong>("effDurationAddedDelay");
            eff.DurationAddedDelayMaxToughness = gom.Data.ValueOrDefault<ScriptEnum>("effDurationAddedDelayMaxToughness", new ScriptEnum()).ToString();
            eff.HasStackLimit = gom.Data.ValueOrDefault<bool>("effHasStackLimit");
            eff.IsInstant = gom.Data.ValueOrDefault<bool>("effIsInstant");
            eff.IsInterruptible = gom.Data.ValueOrDefault<bool>("effIsInterruptible");
            eff.NameStringId = gom.Data.ValueOrDefault<long>("effNameStringId");
            eff.DescriptionStringId = gom.Data.ValueOrDefault<long>("effDescriptionStringId");
            eff.IsUseableOnTaxi = gom.Data.ValueOrDefault<bool>("effIsUseableOnTaxi");
            eff.unknownBool3 = gom.Data.ValueOrDefault<bool>("4611686085914561591");
            eff.unknownBool1 = gom.Data.ValueOrDefault<bool>("4611686297079480000");
            eff.unknownBool2 = gom.Data.ValueOrDefault<bool>("4611686297079480001");
            eff.unknownBool5 = gom.Data.ValueOrDefault<bool>("4611686299759854002");
            eff.unknownBool6 = gom.Data.ValueOrDefault<bool>("4611686299759854003");
            eff.unknownBool7 = gom.Data.ValueOrDefault<bool>("4611686299759854004");
            eff.unknownBool4 = gom.Data.ValueOrDefault<bool>("4611686300275404002");

            //Parse tag ids into tag strings
            var tagTable = _dom.GetObject("tagTablePrototype");
            var tagIdToStringMap = tagTable.Data.Get<Dictionary<object, object>>("9223371996464079193");
            eff.ParsedTags = new List<string>();
            foreach (long tagId in eff.Tags)
            {
                object tag;
                tagIdToStringMap.TryGetValue(tagId, out tag);
                if (tag == null) tag = String.Format("Unknown tag: {0}", tagId);
                eff.ParsedTags.Add((string)tag);
            }

            //Read effect name and description
            //TODO: Could also read abl node to get the locTextStringRetriever, but it only adds code that slows the program without providing additional information
            eff.LocalizedName = _dom.stringTable.TryGetLocalizedStrings("str.abl", eff.NameStringId);
            if (eff.LocalizedName != null) eff.Name = eff.LocalizedName["enMale"];
            eff.LocalizedDescription = _dom.stringTable.TryGetLocalizedStrings("str.abl", eff.DescriptionStringId);
            if (eff.LocalizedDescription != null) eff.Description = eff.LocalizedDescription["enMale"];

            //Check if effect is immediately applied upon ability use, or whether it has to be called explicitly
            eff.IsRootEffect = true;
            if (eff.SubEffects.Count > 0)
            {
                foreach (SubEffectFunction condition in eff.SubEffects[0].Conditions)
                {
                    if (condition.Type == 1L)
                    {
                        eff.IsRootEffect = false;
                        break;
                    }
                }
            }

            //Get list of child effects called by this effect
            eff.ChildEffects = new List<long>();
            if (eff.SubEffects.Count > 1)
            {
                foreach (SubEffect subEffect in eff.SubEffects)
                {
                    foreach (SubEffectFunction action in subEffect.Actions)
                    {
                        if (action.Type == 4L)
                        {
                            foreach (SubEffectFunctionParam param in action.Params)
                            {
                                if (param.Key == 14L)
                                {
                                    long childEffect = (long)param.Value;
                                    //do not allow self-loops, do not allow duplicate entries
                                    if (eff.Number != childEffect && !eff.ChildEffects.Contains(childEffect))
                                    {
                                        eff.ChildEffects.Add(childEffect);
                                    }
                                    goto nextAction;
                                }
                            }
                        }
                        nextAction: { }//
                    }
                }
            }

            gom.Unload();
            return eff;
        }

        private SubEffectFunction LoadSubEffectFunction(GomObjectData obj, string nameId, ulong conditionId = 0UL)
        {
            SubEffectFunction output = new SubEffectFunction();
            output.Type = obj.ValueOrDefault<ScriptEnum>(nameId, new ScriptEnum()).Value;
            if (conditionId != 0UL)
            {
                output.CondId = conditionId;
                //output.Id = (ulong)obj.ValueOrDefault<long>("effConditionId"); //not really necessary
                output.FailureStringId = obj.ValueOrDefault<long>("effConditionFailureStringId");
                output.FailureLocalizedString = _dom.stringTable.TryGetLocalizedStrings("str.abl", output.FailureStringId);
                if (output.FailureLocalizedString != null) output.FailureString = output.FailureLocalizedString["enMale"];
            }
            output.Params = new List<SubEffectFunctionParam>();
            Dictionary<object, object> boolParams = obj.ValueOrDefault<Dictionary<object, object>>("effBoolParams");
            foreach (KeyValuePair<object, object> kvp in boolParams)
            {
                output.Params.Add(new SubEffectFunctionParam((long)((ScriptEnum)kvp.Key).Value, 1, (bool)kvp.Value));
            }
            Dictionary<object, object> stringParams = obj.ValueOrDefault<Dictionary<object, object>>("effStringParams");
            foreach (KeyValuePair<object, object> kvp in stringParams)
            {
                output.Params.Add(new SubEffectFunctionParam((long)((ScriptEnum)kvp.Key).Value, 2, (string)kvp.Value));
            }
            Dictionary<object, object> intParams = obj.ValueOrDefault<Dictionary<object, object>>("effIntParams");
            foreach (KeyValuePair<object, object> kvp in intParams)
            {
                output.Params.Add(new SubEffectFunctionParam((long)((ScriptEnum)kvp.Key).Value, 3, (long)kvp.Value));
            }
            Dictionary<object, object> floatParams = obj.ValueOrDefault<Dictionary<object, object>>("effFloatParams");
            foreach (KeyValuePair<object, object> kvp in floatParams)
            {
                output.Params.Add(new SubEffectFunctionParam((long)((ScriptEnum)kvp.Key).Value, 4, (float)kvp.Value));
            }
            Dictionary<object, object> functionTags = obj.ValueOrDefault<Dictionary<object, object>>("effFunctionTags");
            output.Tags = new List<long>();
            foreach (KeyValuePair<object, object> kvp in functionTags)
            {
                //if ((bool)kvp.Value != true) throw new Exception("ERROR: Expected function tag to be true, but it is false"); //not really necessary
                output.Tags.Add((long)kvp.Key);
            }
            Dictionary<object, object> timeParams = obj.ValueOrDefault<Dictionary<object, object>>("effTimeIntervalParams");
            foreach (KeyValuePair<object, object> kvp in timeParams)
            {
                output.Params.Add(new SubEffectFunctionParam((long)((ScriptEnum)kvp.Key).Value, 5, (ulong)kvp.Value));
            }
            Dictionary<object, object> floatListParams = obj.ValueOrDefault<Dictionary<object, object>>("effFloatListParams");
            foreach (KeyValuePair<object, object> kvp in floatListParams)
            {
                List<float> floatList = new List<float>();
                foreach (float entry in (List<object>)kvp.Value)
                {
                    floatList.Add(entry);
                }
                output.Params.Add(new SubEffectFunctionParam((long)((ScriptEnum)kvp.Key).Value, 6, floatList));
            }
            Dictionary<object, object> intListParams = obj.ValueOrDefault<Dictionary<object, object>>("effIntListParams");
            foreach (KeyValuePair<object, object> kvp in intListParams)
            {
                List<long> intList = new List<long>();
                foreach (long entry in (List<object>)kvp.Value)
                {
                    intList.Add(entry);
                }
                output.Params.Add(new SubEffectFunctionParam((long)((ScriptEnum)kvp.Key).Value, 7, intList));
            }
            Dictionary<object, object> idListParams = obj.ValueOrDefault<Dictionary<object, object>>("effIdListParams");
            foreach (KeyValuePair<object, object> kvp in idListParams)
            {
                List<ulong> idList = new List<ulong>();
                foreach (ulong entry in (List<object>)kvp.Value)
                {
                    idList.Add(entry);
                }
                output.Params.Add(new SubEffectFunctionParam((long)((ScriptEnum)kvp.Key).Value, 8, idList));
            }
            //TODO unknown: 4611686346551820000

            //Parse tag ids into tag strings
            var tagTable = _dom.GetObject("tagTablePrototype");
            var tagIdToStringMap = tagTable.Data.Get<Dictionary<object, object>>("9223371996464079193");
            output.ParsedTags = new List<string>();
            foreach (long tagId in output.Tags)
            {
                object tag;
                tagIdToStringMap.TryGetValue(tagId, out tag);
                if (tag == null) tag = String.Format("Unknown tag: {0}", tagId);
                output.ParsedTags.Add((string)tag);
            }

            return output;
        }

        public void LoadObject(Models.GameObject loadMe, GomObject obj)
        {
            GomLib.Models.Ability abl = (Models.Ability)loadMe;
            Load(abl, obj);
        }

        public void LoadReferences(Models.GameObject obj, GomObject gom)
        {
            // No references to load
        }
    }
}
