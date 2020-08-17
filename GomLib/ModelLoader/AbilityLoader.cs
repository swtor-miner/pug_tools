using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GomLib.Models;
using Newtonsoft.Json;

namespace GomLib.ModelLoader
{
    public class AbilityLoader : IModelLoader
    {
        const long NameLookupKey = -2761358831308646330;
        const long DescLookupKey = 2806211896052149513;

        //int tokenNumber = 1;

        Dictionary<ulong, Ability> idMap;
        Dictionary<string, Ability> nameMap;
        public List<string> effKeys;
        public List<string> effWithUnknowns;

        private readonly DataObjectModel _dom;

        public AbilityLoader(DataObjectModel dom)
        {
            _dom = dom;
            if (nameMap == null)
            {
                Flush();
            }
        }

        public void Flush()
        {
            nameMap = new Dictionary<string, Ability>();
            idMap = new Dictionary<ulong, Ability>();
            effKeys = new List<string>();
            effWithUnknowns = new List<string>();
        }

        public string ClassName
        {
            get { return "ablAbility"; }
        }

        public Models.Ability Load(ulong nodeId)
        {
            if (idMap.TryGetValue(nodeId, out Ability result))
            {
                return result;
            }

            GomObject obj = _dom.GetObject(nodeId);
            Models.Ability abl = new Ability();
            return Load(abl, obj);
        }


        public Models.Ability Load(string fqn)
        {
            if (nameMap.TryGetValue(fqn, out Ability result))
            {
                return result;
            }

            GomObject obj = _dom.GetObject(fqn);
            Models.Ability abl = new Ability();
            return Load(abl, obj);
        }

        public Models.Ability Load(GomObject obj)
        {
            Models.Ability abl = new Ability();
            return Load(abl, obj);
        }

        public Models.GameObject CreateObject()
        {
            return new Models.Ability();
        }

        public Models.Ability Load(Models.GameObject obj, GomObject gom)
        {
            if (gom == null) { return (Ability)obj; }
            if (obj == null) { return null; }

            var abl = (Ability)obj;

            abl.Dom_ = _dom;
            abl.References = gom.References;

            //Hack to just return Fqn and Id for ability effects.
            var actualability = gom.Data.ValueOrDefault<Dictionary<object, object>>("locTextRetrieverMap", null);
            if (actualability == null)
            {
                abl.Fqn = gom.Name;
                abl.NodeId = gom.Id;

                return abl;
            }
            //End Hack

            abl.Fqn = gom.Name;
            abl.NodeId = gom.Id;

            // Ability Info
            abl.IsPassive = gom.Data.ValueOrDefault<bool>("ablIsPassive", false);//4611686019453829615
            abl.IsHidden = gom.Data.ValueOrDefault<bool>("ablIsHidden", false);
            abl.Icon = gom.Data.ValueOrDefault<string>("ablIconSpec", null);
            _dom._assets.icons.Add(abl.Icon);

            GomObject effectZero = gom.Data.ValueOrDefault<GomObject>("ablEffectZero", null);
            if (effectZero != null) abl.EffectZero = effectZero.Id;
            List<Object> effectIds = gom.Data.ValueOrDefault<List<Object>>("ablEffectIDs", null);
            if (effectIds != null)
            {
                abl.EffectIds = new List<ulong>();
                foreach (ulong effectId in effectIds)
                {
                    abl.EffectIds.Add(effectId);
                }
            }

            var textLookup = gom.Data.Get<Dictionary<object, object>>("locTextRetrieverMap");

            // Load Ability Name
            var nameLookupData = (GomObjectData)textLookup[NameLookupKey];
            abl.NameId = nameLookupData.Get<long>("strLocalizedTextRetrieverStringID");
            abl.LocalizedName = _dom.stringTable.TryGetLocalizedStrings(abl.Fqn, nameLookupData);
            Normalize.Dictionary(abl.LocalizedName, abl.Fqn, true);
            abl.LocalizedName = Trim.Dictionary(abl.LocalizedName);
            abl.Name = abl.LocalizedName["enMale"];

            // Load Ability Description
            var descLookupData = (GomObjectData)textLookup[DescLookupKey];
            abl.DescriptionId = descLookupData.Get<long>("strLocalizedTextRetrieverStringID");
            abl.LocalizedDescription = TryGetLocalizedDescription(abl.Fqn, descLookupData);
            abl.Description = TryGetDescription(abl.Fqn, descLookupData);

            abl.Id = gom.Id; //(ulong)(abl.NameId >> 32);

            List<object> abilityEffectList = gom.Data.Get<List<object>>("ablEffectIDs");

            // Load Talent Description Tokens
            var tokenList = new List<string>();
            var descTokenList = new Dictionary<int, Dictionary<string, object>>();
            if (gom.Data.ContainsKey("ablDescriptionTokens"))
            {
                //var tokenList = new List<string>();
                int tid = 1;
                foreach (GomObjectData tokDesc in gom.Data.Get<List<object>>("ablDescriptionTokens"))
                {
                    Dictionary<string, object> tokenInfo = LoadDescriptionToken(tokDesc, abilityEffectList);
                    descTokenList.Add(tid, tokenInfo);
                    tid++;
                }

                foreach (var token in descTokenList)
                {
                    string tokenType = ((Dictionary<string, object>)token.Value)["ablDescriptionTokenType"].ToString();
                    if (tokenType == "ablDescriptionTokenTypeRank" || tokenType == "ablDescriptionTokenTypeDuration")
                    {
                        object multiplier = ((Dictionary<string, object>)token.Value)["ablParsedDescriptionToken"];
                        /*string tokenIdentifier = "<<" + token.Key.ToString();
                        for (int i = 0; i < abl.LocalizedDescription.Count; i++)
                        {
                            abl.LocalizedDescription[abl.LocalizedDescription.ElementAt(i).Key] = abl.LocalizedDescription.ElementAt(i).Value.Replace(tokenIdentifier, multiplier.ToString());
                            
                        }
                        abl.Description = abl.Description.Replace(tokenIdentifier, multiplier.ToString());*/
                    }
                    else
                    {
                        //this should never happen with talents
                        //break;
                    }
                }
                abl.DescriptionTokens = descTokenList;
            }

            //abl.AbilityTokens =
            //LoadParamEffects(abilityEffectList); //Working on returning ability effects

            // This section of code is to aid in exploring the unknown ability effect field names and export the absorb co-efficients.
            foreach (ulong effId in abilityEffectList)
            {
                GomObject eff = _dom.GetObject(effId);
                if (eff != null)
                {
                    foreach (var key in eff.Data.Dictionary.Keys)
                    {
                        if (key.StartsWith("4611"))
                        {
                            if (eff.Data.Get<object>(key).GetType() == new List<object>().GetType())
                            {
                                if (((List<object>)eff.Data.Get<object>(key)).Count() > 0)
                                {
                                    effWithUnknowns.Add(key + " - " + eff.Data.Get<object>(key).GetType().ToString() + " - " + eff.Name);
                                }
                            }
                            else
                            {
                                effWithUnknowns.Add(key + " - " + eff.Data.Get<object>(key).GetType().ToString() + " - " + eff.Data.Get<object>(key).ToString() + " - " + eff.Name);
                            }
                        }
                        string keyType = String.Join(": ", _dom.GetStoredTypeId(key).ToString(), key, eff.Data.Dictionary[key].GetType().ToString());
                        if (!effKeys.Contains(keyType))
                        {
                            effKeys.Add(keyType);
                        }
                    }

                    foreach (var blah in eff.Data.Get<List<object>>("effSubEffects"))
                    {
                        foreach (var subKey in ((GomObjectData)blah).Dictionary.Keys)
                        {
                            if (subKey.StartsWith("4611"))
                            {
                                var blah2 = ((GomObjectData)blah).Get<object>(subKey);
                                if (blah2.GetType() == new List<object>().GetType())
                                {
                                    if (((List<object>)blah2).Count() > 0)
                                    {
                                        effWithUnknowns.Add(subKey + " - " + blah2.GetType().ToString() + " - " + eff.Name);
                                    }
                                }
                                else
                                {
                                    effWithUnknowns.Add(subKey + " - " + blah2.GetType().ToString() + " - " + blah2.ToString() + " - " + eff.Name);
                                }
                            }
                            else if (subKey == "effConditionLogic")
                            {
                                var blah2 = ((GomObjectData)blah).Get<object>(subKey);
                                if (((List<object>)blah2).Count > 1)
                                {
                                    foreach (var condition in ((List<object>)blah2).ConvertAll<GomObjectData>(x => (GomObjectData)x))
                                    {
                                        var logicOperator = condition.ValueOrDefault<object>("effConditionLogicOperator", "");
                                        /*if (logicOperator.ToString() != "" && logicOperator.ToString() != "effLogicOpAnd") //obsolete debugging code
                                        {
                                            bool complexLogicCanidate = true; //need more code here to sift out case of And + Or logic
                                        }*/
                                    }
                                }
                            }
                            else if (subKey == "effActions")
                            {
                                //Get the absorb co-efficients. Can't do this with the others because absorb doesn't have a description token.
                                var effActions = ((GomObjectData)blah).ValueOrDefault<List<object>>("effActions", null);
                                foreach (GomObjectData effAction in effActions)
                                {
                                    if (effAction.ValueOrDefault<ScriptEnum>("effActionName", null).ToString() == "effAction_AbsorbDamage")
                                    {
                                        Dictionary<string, float> absorbDetails = new Dictionary<string, float>();
                                        Dictionary<object, object> valueByParam = effAction.ValueOrDefault<Dictionary<object, object>>("effFloatParams",
                                            new Dictionary<object, object>());

                                        foreach (KeyValuePair<object, object> kvp in valueByParam)
                                        {
                                            absorbDetails.Add(((ScriptEnum)kvp.Key).ToString(), (float)kvp.Value);
                                        }

                                        abl.AbsorbParams.Add(absorbDetails);
                                    }
                                }
                            }

                            if (!effKeys.Contains("4611686039404270002: effSubEffects" + " - " + subKey))
                            {
                                effKeys.Add("4611686039404270002: effSubEffects" + " - " + subKey);
                            }
                        }
                    }
                    eff.Unload();
                }
            }

            //end of ability effect code

            // Load active ability info (energy cost, range, casting time, etc)
            abl.MinRange = gom.Data.ValueOrDefault<float>("ablMinRange", 0);//4611686019453829663
            abl.MaxRange = gom.Data.ValueOrDefault<float>("ablMaxRange", 0);//4611686019453829664
            abl.ApCost = gom.Data.ValueOrDefault<float>("ablActionPointCost", 0);//4611686019453829662
            abl.EnergyCost = gom.Data.ValueOrDefault<float>("ablEnergyCost", 0);//4611686019453829661
            abl.ForceCost = gom.Data.ValueOrDefault<float>("ablForceCost", 0);//4611686019453829660
            abl.ChannelingTime = gom.Data.ValueOrDefault<float>("ablChannelingTime", 0);//4611686019453829658
            abl.CastingTime = gom.Data.ValueOrDefault<float>("ablCastingTime", 0);//4611686019453829657
            abl.Cooldown = gom.Data.ValueOrDefault<float>("ablCooldownTime", 0);//4611686019453829631

            abl.Pushback = gom.Data.ValueOrDefault<bool>("ablUsesSpellPushback", true);
            // abl.ApType
            if (abl.ApCost > 0)
            {
                if (abl.Fqn.StartsWith("abl.jedi_knight"))
                {
                    abl.ApType = ApType.Focus;
                }
                else if (abl.Fqn.StartsWith("abl.trooper"))
                {
                    abl.ApType = ApType.Ammo;
                }
                else if (abl.Fqn.StartsWith("abl.sith_war"))
                {
                    abl.ApType = ApType.Rage;
                }
            }
            else if ((abl.ForceCost == 0) && (abl.EnergyCost == 0) && (abl.Fqn.StartsWith("abl.bounty_hunter.")))
            {
                // Find heat cost
                abl.ApCost = HeatGeneration(abilityEffectList);
                abl.ApType = (abl.ApCost > 0) ? ApType.Heat : ApType.None;
            }
            else
            {
                abl.ApType = ApType.None;
            }

            abl.IgnoreAlacrity = gom.Data.ValueOrDefault<bool>("ablIgnoreAlacrity", false);
            abl.GCD = gom.Data.ValueOrDefault<float>("ablGlobalCooldownTime", -1);
            abl.GcdOverride = abl.GCD > -0.5f;
            abl.LineOfSightCheck = gom.Data.ValueOrDefault<bool>("ablIsLineOfSightChecked", true);
            abl.ModalGroup = gom.Data.ValueOrDefault<long>("ablModalGroup", 0);

            var sharedCooldowns = gom.Data.ValueOrDefault<List<object>>("ablCooldownTimerSpecs", null);
            if (sharedCooldowns != null)
            {
                abl.CooldownTimerSpecs = sharedCooldowns.Select((s, i) => new { s, i }).ToDictionary(x => x.i, x => (ulong)x.s);
                if (sharedCooldowns.Count > 1)
                {
                    //Console.WriteLine("{0} has more than 1 shared cooldown? How does this work.", abl.Fqn);
                }
                if (sharedCooldowns.Count > 0)
                {
                    abl.SharedCooldown = (ulong)sharedCooldowns[0];
                }
            }

            abl.TargetArc = gom.Data.ValueOrDefault<float>("ablTargetArc", 0);
            abl.TargetArcOffset = gom.Data.ValueOrDefault<float>("ablTargetArcOffset", 0);
            var ablTargetRule = gom.Data.ValueOrDefault<ScriptEnum>("ablTargetRule", null);
            if (ablTargetRule != null) { abl.TargetRule = ablTargetRule.Value; } else { abl.TargetRule = 0; }


            var ablAiType = gom.Data.ValueOrDefault<ScriptEnum>("ablAiType", null);
            if (ablAiType != null) { abl.AiType = ablAiType.Value; } else { abl.AiType = 1; }
            var ablCombatMode = gom.Data.ValueOrDefault<ScriptEnum>("ablCombatMode", null);
            if (ablCombatMode != null) { abl.CombatMode = ablCombatMode.Value; } else { abl.CombatMode = 1; }
            var ablAutoAttackMode = gom.Data.ValueOrDefault<ScriptEnum>("ablAutoAttackMode", null);
            if (ablAutoAttackMode != null) { abl.AutoAttackMode = ablAutoAttackMode.Value; } else { abl.AutoAttackMode = 0; }
            abl.IsValid = gom.Data.ValueOrDefault<bool>("ablIsValid", false);
            abl.IsCustom = gom.Data.ValueOrDefault<bool>("ablIsCustom", false);
            abl.AppearanceSpec = gom.Data.ValueOrDefault<string>("ablAppearanceSpec", "");
            abl.UnknownInt = gom.Data.ValueOrDefault<long>("4611686019453829607", 0);
            abl.UnknownBool = gom.Data.ValueOrDefault<bool>("4611686025269794705", false);
            abl.UnknownInt2 = gom.Data.ValueOrDefault<ulong>("4611686028321074586", 0);

            if (gom.References != null)
            {
                string stb = "str.gui.questcategories";
                long sId = 2466269005611266; //uncategorized
                if (gom.References.ContainsKey("usedByPlayerClass"))
                {
                    stb = "str.gui.map";
                    sId = 945872057663588; //Player
                    //if (gom.References["usedByPlayerClass"].Count > 0)
                    //{
                    //    foreach (ulong cId in gom.References["usedByPlayerClass"]) {
                    //        GomObject cObj = _dom.GetObject(cId);
                    //        long subId = 0;
                    //        if (cObj.Name.StartsWith("class.pc.advanced"))
                    //            subId = cObj.Data.ValueOrDefault<long>("chrAdvancedClassDataNameId", 0);
                    //        else
                    //            subId = cObj.Data.ValueOrDefault<long>("chrClassDataNameId", 0); // Index into str.gui.classnames
                    //        StringTable subTable = _dom.stringTable.Find("str.gui.classnames");
                    //        abl.LocalizedSubCategory = subTable.GetLocalizedText(subId, "str.gui.classnames");
                    //    }
                    //}
                }
                else if (gom.References.ContainsKey("partOfApn"))
                {
                    abl.LocalizedCategory = new Dictionary<string, string>()
                    {
                        { "enMale", "Npc" },
                        { "frMale", "Pnj" },
                        { "deMale", "Nsc" },
                    };
                }
                else if (gom.References.ContainsKey("calledByItmUse") || gom.References.ContainsKey("calledByItmEquip") || gom.References.ContainsKey("taughtByItem"))
                {
                    stb = "str.gui.guildbank";
                    sId = 2813023190253604;
                }
                else if (gom.References.ContainsKey("calledByPlcUse"))
                {
                    abl.LocalizedCategory = new Dictionary<string, string>()
                    {
                        { "enMale", "Placeable Object" },
                        { "frMale", "Objet Plaçable" },
                        { "deMale", "Platzierbaren Objekt" },
                    }; ;
                }


                if (abl.LocalizedCategory == null)
                {
                    StringTable stbTable = _dom.stringTable.Find(stb);
                    if (stbTable != null)
                        abl.LocalizedCategory = stbTable.GetLocalizedText(sId, stb);
                }
            }
            gom.Unload();
            return abl;
        }

        private string TryGetDescription(string fqn, GomObjectData descLookupData)
        {
            string tempString = _dom.stringTable.TryGetString(fqn, descLookupData);
            /*.Replace(">>", "")
            .Replace("%d", "")
            .Replace(" / ", "")
            .Replace("//", "")
            .Replace("[]", "");

if (tempString.Contains("$d"))
{
int index = tempString.IndexOf("$d");
int endIndex = tempString.IndexOf("]", index);

string start = tempString.Substring(0, index - 1);

string tempMiddle = tempString.Substring(index, endIndex - index);
int middleIndex = tempMiddle.LastIndexOf("/$d") + 3;
string middle = tempMiddle.Substring(middleIndex, tempMiddle.Length - middleIndex);

string end = tempString.Substring(endIndex + 1);

tempString = start + middle + end;
}*/

            return tempString;
        }

        private Dictionary<string, string> TryGetLocalizedDescription(string fqn, GomObjectData descLookupData)
        {
            Dictionary<string, string> tempTable = _dom.stringTable.TryGetLocalizedStrings(fqn, descLookupData);
            /*for (int i =0; i <tempTable.Count; i++)
            {
                tempTable[tempTable.ElementAt(i).Key] = tempTable.ElementAt(i).Value.Replace(">>", "")
                            .Replace("%d", "")
                            .Replace(" / ", "")
                            .Replace("//", "")
                            .Replace("[]", "");

                if (tempTable[tempTable.ElementAt(i).Key].Contains("$d") || tempTable[tempTable.ElementAt(i).Key].Contains("["))
                {
                    string tempString = tempTable[tempTable.ElementAt(i).Key];

                    int count = tempString.Split('[').Count() - 1;
                    for (int c = 0; c < count; c++)
                    {
                        int index = tempString.IndexOf("$d");
                        if (index == -1)
                        {
                            index = tempString.IndexOf("[");
                            int endIndex = tempString.IndexOf("]", index);
                            if (endIndex == -1)
                            {
                                tempString = tempString.Replace("[%point points", "points");
                            }
                            else
                            {
                                string start = tempString.Substring(0, index - 1);

                                string tempMiddle = tempString.Substring(index, endIndex - index);
                                int middleIndex = tempMiddle.LastIndexOf("/") + 1;
                                string middle = tempMiddle.Substring(middleIndex, tempMiddle.Length - middleIndex);

                                string end = tempString.Substring(endIndex + 1);
                                tempString = start + middle + end;
                            }
                        }
                        else
                        {
                            int endIndex = tempString.IndexOf("]", index);

                            string start = tempString.Substring(0, index - 1);

                            string tempMiddle = tempString.Substring(index, endIndex - index);
                            int middleIndex = tempMiddle.LastIndexOf("/$d") + 3;
                            string middle = tempMiddle.Substring(middleIndex, tempMiddle.Length - middleIndex);

                            string end = tempString.Substring(endIndex + 1);
                            tempString = start + middle + end;
                        }
                    }
                    tempTable[tempTable.ElementAt(i).Key] = tempString.Replace("$d", "");
                }
            }*/
            return tempTable;
        }

        public Dictionary<string, object> LoadDescriptionToken(GomObjectData tokDesc, List<object> abilityEffectList)
        {
            var tokenInfo = new Dictionary<string, object>();

            string tokType = tokDesc.ValueOrDefault<object>("ablDescriptionTokenType", "ablDescriptionTokenTypeRank").ToString();
            object multiplier = tokDesc.ValueOrDefault<object>("ablDescriptionTokenMultiplier", 1);
            object tEff = tokDesc.ValueOrDefault<object>("ablDescriptionTokenEffect", -1);
            object subEff = tokDesc.ValueOrDefault<object>("ablDescriptionTokenSubEffect", -1);

            switch (tokType)
            {
                case "ablDescriptionTokenTypeRank":
                    tokenInfo.Add("ablParsedDescriptionToken", multiplier);
                    break;
                case "ablDescriptionTokenTypeDamage":
                    var damage = LoadParamDamage(tokDesc, abilityEffectList);
                    tokenInfo.Add("ablParsedDescriptionToken", damage);
                    break;
                case "ablDescriptionTokenTypeHealing":
                    var healing = LoadParamHealing(tokDesc, abilityEffectList);
                    tokenInfo.Add("ablParsedDescriptionToken", healing);
                    break;
                case "ablDescriptionTokenTypeDuration":
                    string duration = LoadParamDuration(tokDesc, abilityEffectList);
                    tokenInfo.Add("ablParsedDescriptionToken", float.Parse(duration.Substring(duration.IndexOf(",") + 1)));
                    break;
                case "ablDescriptionTokenTypeBindpoint":
                    tokenInfo.Add("ablParsedDescriptionToken", "bindpoint");
                    break;
            }

            tokenInfo.Add("ablDescriptionTokenMultiplier", multiplier);

            tokenInfo.Add("ablDescriptionTokenType", tokType);
            tokenInfo.Add("ablDescriptionTokenEffect", tEff);
            tokenInfo.Add("ablDescriptionTokenSubEffect", subEff);
            return tokenInfo;
        }

        public void LoadObject(Models.GameObject loadMe, GomObject obj)
        {
            GomLib.Models.Ability abl = (Models.Ability)loadMe;
            Load(abl, obj);
        }

        private float HeatGeneration(List<object> effectIds)
        {
            if (effectIds == null) { return 0; }

            foreach (ulong effId in effectIds)
            {
                var eff = _dom.GetObject(effId);
                if (!String.Equals(eff.Data.ValueOrDefault<ScriptEnum>("effSlotType", null).ToString(), "conSlotEffectOther")) { continue; } // GenerateHeat function only appears in this type of effect
                var subEffects = eff.Data.ValueOrDefault<List<object>>("effSubEffects", null);
                foreach (GomObjectData subEff in subEffects)
                {
                    var effActions = subEff.ValueOrDefault<List<object>>("effActions", null);
                    foreach (GomObjectData effAction in effActions)
                    {
                        if (effAction.ValueOrDefault<ScriptEnum>("effActionName", null).ToString() == "effAction_GenerateHeat")
                        {
                            // Heat = effAction.effFloatParams[effParam_Amount]
                            return (float)((IDictionary<object, object>)effAction.ValueOrDefault<Dictionary<object, object>>("effFloatParams", null)).First(kvp => ((ScriptEnum)kvp.Key).ToString() == "effParam_Amount").Value;
                        }
                    }
                }
            }
            return 0;
        }

        private bool HasDamageAction(GomObject effect)
        {
            var tokSubEffects = (List<object>)effect.Data.ValueOrDefault<List<object>>("effSubEffects", null);
            foreach (GomObjectData subEff in tokSubEffects)
            {
                foreach (GomObjectData a in subEff.ValueOrDefault<List<object>>("effActions", null))
                {
                    switch (((ScriptEnum)a.ValueOrDefault<ScriptEnum>("effActionName", null)).ToString())
                    {
                        case "effAction_WeaponDamage":
                        case "effAction_SpellDamage":
                            return true;
                    }
                }
            }

            return false;
        }

        private List<KeyValuePair<string, List<Dictionary<string, string>>>> LoadParamDamage(GomObjectData tokDesc, List<object> abilityEffectList)
        {
            int tokEffIndex = (int)tokDesc.ValueOrDefault<long>("ablDescriptionTokenEffect", 0);
            int tokSubEffIndex = (int)tokDesc.ValueOrDefault<long>("ablDescriptionTokenSubEffect", 0);
            float multi = (float)tokDesc.ValueOrDefault<float>("ablDescriptionTokenMultiplier", 0f);
            if (!(tokEffIndex < abilityEffectList.Count)) { return new List<KeyValuePair<string, List<Dictionary<string, string>>>>(); }

            // Effect is in-range, find the coefficients for dmg formula
            GomObject tokEff = null;
            if (tokEffIndex >= 0)
            {
                tokEff = _dom.GetObject((ulong)abilityEffectList[tokEffIndex]);
            }
            else
            {
                foreach (ulong ablEffId in abilityEffectList)
                {
                    tokEff = _dom.GetObject(ablEffId);
                    if (tokEff == null) { continue; }
                    if (HasDamageAction(tokEff))
                    {
                        break;
                    }
                    else
                    {
                        tokEff = null;
                    }
                }
            }
            if (tokEff == null) { return new List<KeyValuePair<string, List<Dictionary<string, string>>>>(); }

            var tokSubEffects = (List<object>)tokEff.Data.ValueOrDefault<List<object>>("effSubEffects", null);
            List<GomObjectData> actions = new List<GomObjectData>();
            List<string> actionNames = new List<string>();

            // Loop through subEffects
            if ((tokSubEffIndex >= 0) && (tokSubEffIndex < tokSubEffects.Count))
            {
                GomObjectData subEff = (GomObjectData)tokSubEffects[tokSubEffIndex];
                foreach (GomObjectData a in subEff.ValueOrDefault<List<object>>("effActions", null))
                {
                    string actionName = ((ScriptEnum)a.ValueOrDefault<ScriptEnum>("effActionName", null)).ToString();
                    if (actionName == "effAction_WeaponDamage" || actionName == "effAction_SpellDamage"
                        || actionName == "effAction_GodDamage")
                    {
                        actions.Add(a);
                        actionNames.Add(actionName);
                    }
                }
            }
            else
            {
                foreach (GomObjectData subEff in tokSubEffects)
                {
                    var effActions = (List<object>)subEff.Get<List<object>>("effActions");
                    foreach (GomObjectData a in effActions)
                    {
                        string actionName = ((ScriptEnum)a.Get<ScriptEnum>("effActionName")).ToString();
                        if (actionName == "effAction_WeaponDamage" || actionName == "effAction_SpellDamage"
                            || actionName == "effAction_GodDamage")
                        {
                            actions.Add(a);
                            actionNames.Add(actionName);
                        }
                    }
                    //if (actions != null) { break; }
                }
            }

            if (actions == null || actions.Count <= 0) { return new List<KeyValuePair<string, List<Dictionary<string, string>>>>(); }
            var retVal = new List<KeyValuePair<string, List<Dictionary<string, string>>>>();

            for (int i = 0; i < actions.Count; i++)
            {
                KeyValuePair<string, List<Dictionary<string, string>>> kvp
                    = new KeyValuePair<string, List<Dictionary<string, string>>>(actionNames[i], LoadActionParams(actions[i]));
                kvp.Value.Add(new Dictionary<string, string> { { "ablDescriptionTokenMultiplier", multi.ToString() } });
                retVal.Add(kvp);
            }

            tokEff.Unload();
            return retVal;
        }

        private List<Dictionary<string, string>> LoadActionParams(GomObjectData action)
        {
            List<Dictionary<string, string>> effectDetails = new List<Dictionary<string, string>>();

            Dictionary<object, object> boolParams = action.Get<Dictionary<object, object>>("effBoolParams");
            Dictionary<string, string> boolValues = new Dictionary<string, string>();

            foreach (KeyValuePair<object, object> kvp in boolParams)
            {
                ScriptEnum key = (ScriptEnum)kvp.Key;
                string keyStr = key.ToString();
                string curVal = ((bool)kvp.Value).ToString();

                boolValues.Add(keyStr, curVal);
            }
            effectDetails.Add(boolValues);


            Dictionary<object, object> intParams = action.Get<Dictionary<object, object>>("effIntParams");
            Dictionary<string, string> intValues = new Dictionary<string, string>();
            foreach (KeyValuePair<object, object> kvp in intParams)
            {
                ScriptEnum key = (ScriptEnum)kvp.Key;
                string keyStr = key.ToString();
                string curVal = ((long)kvp.Value).ToString();

                intValues.Add(keyStr, curVal);
            }
            effectDetails.Add(intValues);


            Dictionary<object, object> floatParams = action.Get<Dictionary<object, object>>("effFloatParams");
            Dictionary<string, string> floatValues = new Dictionary<string, string>();
            foreach (KeyValuePair<object, object> kvp in floatParams)
            {
                ScriptEnum key = (ScriptEnum)kvp.Key;
                string keyStr = key.ToString();
                string curVal = ((float)kvp.Value).ToString();

                floatValues.Add(keyStr, curVal);
            }
            effectDetails.Add(floatValues);

            return effectDetails;
        }

        private List<KeyValuePair<string, List<Dictionary<string, string>>>> LoadParamHealing(GomObjectData tokDesc, List<object> abilityEffectList)
        {
            int tokEffIndex = (int)tokDesc.ValueOrDefault<long>("ablDescriptionTokenEffect", 0);
            int tokSubEffIndex = (int)tokDesc.ValueOrDefault<long>("ablDescriptionTokenSubEffect", 0);
            float multi = (float)tokDesc.ValueOrDefault<float>("ablDescriptionTokenMultiplier", 0f);

            if (tokEffIndex < abilityEffectList.Count)
            {
                // Effect is in-range, find the coefficients for healing formula
                var tokEff = _dom.GetObject((ulong)abilityEffectList[tokEffIndex]);
                if (tokEff == null) { return new List<KeyValuePair<string, List<Dictionary<string, string>>>>(); }

                var tokSubEffects = (List<object>)tokEff.Data.ValueOrDefault<List<object>>("effSubEffects", null);
                tokEff.Unload();
                GomObjectData action = null;
                // Loop through subEffects
                if ((tokSubEffIndex >= 0) && (tokSubEffIndex < tokSubEffects.Count))
                {
                    GomObjectData subEff = (GomObjectData)tokSubEffects[tokSubEffIndex];
                    foreach (GomObjectData a in subEff.ValueOrDefault<List<object>>("effActions", null))
                    {
                        if (((ScriptEnum)a.ValueOrDefault<ScriptEnum>("effActionName", null)).ToString() == "effAction_Heal")
                        {
                            action = a;
                            break;
                        }
                    }
                }
                else
                {
                    foreach (GomObjectData subEff in tokSubEffects)
                    {
                        var effActions = (List<object>)subEff.ValueOrDefault<List<object>>("effActions", null);
                        foreach (GomObjectData a in effActions)
                        {
                            if (((ScriptEnum)a.ValueOrDefault<ScriptEnum>("effActionName", null)).ToString() == "effAction_Heal")
                            {
                                action = a;
                                break;
                            }
                        }
                        if (action != null) { break; }
                    }
                }

                if (action == null) { return new List<KeyValuePair<string, List<Dictionary<string, string>>>>(); }

                //Get the action details and return it in a list. We do this to maintain the same structure between dmg and heals.
                KeyValuePair<string, List<Dictionary<string, string>>> kvp =
                    new KeyValuePair<string, List<Dictionary<string, string>>>("effAction_Heal", LoadActionParams(action));
                kvp.Value.Add(new Dictionary<string, string> { { "ablDescriptionTokenMultiplier", multi.ToString() } });
                var retVal = new List<KeyValuePair<string, List<Dictionary<string, string>>>>
                {
                    kvp
                };

                return retVal;
            }

            return new List<KeyValuePair<string, List<Dictionary<string, string>>>>();
        }

        private string LoadParamDuration(GomObjectData tokDesc, List<object> abilityEffectList)
        {
            float duration = 1;
            int tokEffIndex = (int)tokDesc.Get<long>("ablDescriptionTokenEffect");
            int tokSubEffIndex = (int)tokDesc.Get<long>("ablDescriptionTokenSubEffect");
            if (tokEffIndex >= abilityEffectList.Count)
            {
                // Effect is out of range, so just put a 0 here for the duration.
                duration = 0;
            }
            else
            {
                var tokEff = _dom.GetObject((ulong)abilityEffectList[tokEffIndex]);
                if (tokEff == null)
                {
                    duration = 0;
                }
                else
                {
                    if (tokSubEffIndex < 1)
                    {
                        duration = (float)((ulong)tokEff.Data.ValueOrDefault<ulong>("effDuration", 0)) / 1000;
                    }
                    else
                    {
                        var tokSubEffInitializers = ((GomObjectData)tokEff.Data.Get<List<object>>("effSubEffects")[tokSubEffIndex - 1]).Get<List<object>>("effInitializers");
                        foreach (GomObjectData effInit in tokSubEffInitializers)
                        {
                            if (effInit.Get<object>("effInitializerName").ToString() == "effInitializer_SetDuration")
                            {
                                var durationMap = effInit.Get<Dictionary<object, object>>("effTimeIntervalParams");
                                foreach (var durKvp in durationMap)
                                {
                                    if (((ScriptEnum)durKvp.Key).Value == 0xA2)
                                    {
                                        duration = (float)durKvp.Value / 1000;
                                        break;
                                    }
                                }
                                break;
                            }
                        }
                    }
                    tokEff.Unload();
                }
            }

            return String.Format("duration,{0}", duration);
        }

        public void LoadReferences(Models.GameObject obj, GomObject gom)
        {
            // No references to load
        }
    }
}
