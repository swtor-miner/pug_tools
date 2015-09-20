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
    public class Ability : GameObject, IEquatable<Ability>
    {
        public ulong NodeId { get; set; }

        public long NameId { get; set; }
        public string Name { get; set; }
        public Dictionary<string, string> LocalizedName { get; set; }
        public long DescriptionId { get; set; }
        [Newtonsoft.Json.JsonIgnore]
        public string _Description
        {
            get
            {
                return System.Text.RegularExpressions.Regex.Replace(Description, @"\r\n?|\n", "<br />");
            }
        }
        public string Description { get; set; }
        public Dictionary<string, string> LocalizedDescription { get; set; }
        public string ParsedDescription
        {
            get
            {
                return SQLHelpers.sqlSani(ParseDescription(Description));
            }
        }
        [Newtonsoft.Json.JsonIgnore]
        internal Dictionary<string, string> _ParsedLocalizedDescription { get; set; }
        public Dictionary<string, string> ParsedLocalizedDescription
        {
            get
            {
                if (_ParsedLocalizedDescription == null && LocalizedDescription != null)
                {
                    _ParsedLocalizedDescription = new Dictionary<string, string>();
                    foreach (var desc in LocalizedDescription)
                    {
                        _ParsedLocalizedDescription.Add(desc.Key, ParseDescription(desc.Value));
                    }
                }
                return _ParsedLocalizedDescription;
            }
        }

        private string ParseDescription(string desc)
        {
            return ParseDescription(this, desc);
        }
        public static string ParseDescription(Ability abl, string desc)
        {
            if (abl.DescriptionTokens == null)
                return desc;

            for (var i = 0; i < abl.DescriptionTokens.Count; i++)
            {
                var id = abl.DescriptionTokens.ElementAt(i).Key;
                var value = abl.DescriptionTokens.ElementAt(i).Value["ablParsedDescriptionToken"].ToString();
                var type = abl.DescriptionTokens.ElementAt(i).Value["ablDescriptionTokenType"].ToString().Replace("ablDescriptionTokenType", "");
                var start = desc.IndexOf("<<" + id);

                if (start == -1)
                {
                    //console.log("didn't find: <<" + id);
                    continue;
                }
                //console.log("id" + id + ":" + retval);
                //console.log("Start Index: " + start);

                var end = desc.Substring(start).IndexOf(">>") + 2;

                //console.log("Length: " +length);
                var fullToken = desc.Substring(start, end);
                //console.log("Full: " + fullToken);

                var durationText = "";
                if (end > 5)
                {
                    try
                    {
                        string[] durationList = new string[] { "", "", "" };
                        var partialToken = fullToken.Substring(4, fullToken.Length - 7);
                        //console.log("Partial:" + partialToken);

                        durationList = partialToken.Replace("%d", "").Split('/').ToArray();
                        //console.log(durationList);

                        int pValue;
                        Int32.TryParse(value.ToString(), out pValue);

                        durationText = "";
                        if (pValue <= 0)
                            durationText = durationList[0];
                        else if (pValue > 1)
                            durationText = durationList[2];
                        else
                            durationText = durationList[1];
                        //console.log(pValue + durationText);
                    }
                    catch (Exception ex)
                    {
                        //this happens when the tokens are malformed
                    }
                }
                //console.log(type);
                while (desc.IndexOf(fullToken) != -1)
                { //sometimes there's multiple instance of the same token.
                    bool breakloop = false;
                    switch (type)
                    {
                        /*case "Healing":
                        case "Damage":
                            retval = retval.Replace(fullToken, generateTokenString(value));
                            break;*/
                        case "Duration":
                            desc = desc.Replace(fullToken, value + durationText);
                            break;
                        case "Talent":
                            desc = desc.Replace(fullToken, value);
                            //console.log("replaced '<<" + id + ">>' :" + retval);
                            break;
                        default:
                            //console.log(type);
                            //retval = retval.Replace(fullToken, "Unknown Token: " + type);
                            breakloop = true;
                            break;
                    }
                    if (breakloop) break;
                }

            }
            return desc;
        }

        [JsonIgnore]
        internal List<string> _Effects { get; set; }
        public List<string> EffectsB62
        {
            get
            {
                if(_Effects == null && EffectIds != null)
                {
                    _Effects = new List<string>();
                    foreach(var uId in EffectIds)
                    {
                        _Effects.Add(uId.ToMaskedBase62());
                    }
                }
                return _Effects;
            }
        }
        public List<ulong> EffectIds { get; set; }
        public int Level { get; set; }
        public string Icon { get; set; }
        public ulong EffectZero { get; set; }
        public bool IsHidden { get; set; }
        public bool IsPassive { get; set; }
        public int Version { get; set; }
        public float Cooldown { get; set; }
        public float CastingTime { get; set; }
        public float ChannelingTime { get; set; }
        public float ForceCost { get; set; }
        public float EnergyCost { get; set; }
        public float ApCost { get; set; }
        public ApType ApType { get; set; }
        public float MinRange { get; set; }
        public float MaxRange { get; set; }
        public float GCD { get; set; }
        public bool GcdOverride { get; set; }
        public long ModalGroup { get; set; }
        public ulong SharedCooldown { get; set; }
        /* What's the point?
        public string TalentTokens { get; set; }
        public string AbilityTokens { get; set; }*/
        public float TargetArc { get; set; }
        public float TargetArcOffset { get; set; }
        public int TargetRule { get; set; }
        public bool LineOfSightCheck { get; set; }
        public bool Pushback { get; set; }
        public bool IgnoreAlacrity { get; set; }

        public Dictionary<int, Dictionary<string, object>> DescriptionTokens { get; set; }
        [Newtonsoft.Json.JsonIgnore]
        internal Dictionary<int, Dictionary<string, object>> _modDescriptionTokens { get; set; }
        public Dictionary<int, Dictionary<string, object>> modDescriptionTokens
        {
            get
            {
                if (DescriptionTokens == null) return null;
                if (_modDescriptionTokens == null)
                {
                    _modDescriptionTokens = new Dictionary<int, Dictionary<string, object>>();
                    foreach (var token in DescriptionTokens)
                    {
                        _modDescriptionTokens.Add(token.Key, token.Value.ToDictionary(x => x.Key.Replace("ablDescriptionToken", "")
                            .Replace("ablParsedDescriptionToken", "Value"), x => ((x.Value.GetType() == typeof(string)) ? ((string)x.Value).Replace("ablDescriptionTokenType", "") : x.Value)));
                    }
                }
                return _modDescriptionTokens;
            }
        }
        public int AiType { get; set; }
        public int CombatMode { get; set; }
        public int AutoAttackMode { get; set; }
        public bool IsValid { get; set; }
        public bool IsCustom { get; set; }
        public string AppearanceSpec { get; set; }
        public long UnknownInt { get; set; }
        public bool UnknownBool { get; set; }
        public ulong UnknownInt2 { get; set; }
        public Dictionary<int, ulong> CooldownTimerSpecs { get; set; }
        internal List<Dictionary<string, float>> _AbsorbParams { get; set; }
        public List<Dictionary<string, float>> AbsorbParams
        {
            get
            {
                if(_AbsorbParams == null)
                {
                    _AbsorbParams = new List<Dictionary<string, float>>();
                }

                return _AbsorbParams;
            }
            
            set
            {
                _AbsorbParams = value;
            }
        }

        public string HashedIcon
        {
            get
            {
                string icon = "none";
                if (Icon != null)
                {
                    icon = Icon;
                }
                var fileId = TorLib.FileId.FromFilePath(String.Format("/resources/gfx/icons/{0}.dds", icon));
                return String.Format("{0}_{1}", fileId.ph, fileId.sh);
            }
        }

        public override int GetHashCode()  //should be fixed.
        {
            int hash = Level.GetHashCode();
            if (LocalizedDescription != null) foreach (var x in LocalizedDescription) { hash ^= x.GetHashCode(); } //dictionaries need to hashed like this
            if (LocalizedName != null) foreach (var x in LocalizedName) { hash ^= x.GetHashCode(); } //dictionaries need to hashed like this
            if (Icon != null) { hash ^= Icon.GetHashCode(); }
            if (EffectIds != null) foreach (var x in EffectIds) { hash ^= x.GetHashCode(); }
            hash ^= Level.GetHashCode();
            hash ^= EffectZero.GetHashCode();
            hash ^= IsHidden.GetHashCode();
            hash ^= IsPassive.GetHashCode();
            hash ^= Version.GetHashCode();
            hash ^= Cooldown.GetHashCode();
            hash ^= CastingTime.GetHashCode();
            hash ^= ChannelingTime.GetHashCode();
            hash ^= ForceCost.GetHashCode();
            hash ^= EnergyCost.GetHashCode();
            hash ^= ApCost.GetHashCode();
            hash ^= ApType.GetHashCode();
            hash ^= MinRange.GetHashCode();
            hash ^= MaxRange.GetHashCode();
            hash ^= GCD.GetHashCode();
            hash ^= GcdOverride.GetHashCode();
            hash ^= ModalGroup.GetHashCode();
            hash ^= SharedCooldown.GetHashCode();
            hash ^= TargetArc.GetHashCode();
            hash ^= TargetArcOffset.GetHashCode();
            hash ^= TargetRule.GetHashCode();
            hash ^= LineOfSightCheck.GetHashCode();
            hash ^= Pushback.GetHashCode();
            hash ^= IgnoreAlacrity.GetHashCode();
            if (DescriptionTokens != null)
            {
                foreach (var x in DescriptionTokens)
                {
                    hash ^= x.Key.GetHashCode();
                    object typeObj;
                    string type = string.Empty;
                    if (x.Value.TryGetValue("ablDescriptionTokenType", out typeObj))
                    {
                        type = (string)typeObj;
                    }

                    foreach(var val in x.Value)
                    {
                        hash ^= val.Key.GetHashCode();
                        if (val.Key == "ablParsedDescriptionToken" &&
                            (type == "ablDescriptionTokenTypeDamage" || type == "ablDescriptionTokenTypeHealing"))
                        {
                            List<KeyValuePair<string, List<Dictionary<string, string>>>> coeffValues
                                    = (List<KeyValuePair<string, List<Dictionary<string, string>>>>)val.Value;
   
                                    foreach (KeyValuePair<string, List<Dictionary<string, string>>> actionDetails in coeffValues)
                                    {
                                        hash ^= actionDetails.Key.GetHashCode();
                                        foreach (Dictionary<string, string> actionDict in actionDetails.Value)
                                        {
                                            foreach (KeyValuePair<string, string> detailsKvp in actionDict)
                                            {
                                                hash ^= detailsKvp.Key.GetHashCode();
                                                hash ^= detailsKvp.Value.GetHashCode();
                                            }
                                        }
                            }
                        }
                        else
                        {
                            string valueStr = val.Value as string;
                            if (valueStr != null)
                            {
                                hash ^= valueStr.GetHashCode();
                            }
                            else
                            {
                                if(val.Value is long)
                                {
                                    hash ^= ((long)val.Value).GetHashCode();
                                }
                                else if (val.Value is float)
                                {
                                    hash ^= ((float)val.Value).GetHashCode();
                                }
                            }
                            
                        }
                    }
                }
            }
            hash ^= AiType.GetHashCode();
            hash ^= CombatMode.GetHashCode();
            hash ^= AutoAttackMode.GetHashCode();
            hash ^= IsValid.GetHashCode();
            hash ^= IsCustom.GetHashCode();
            if (AppearanceSpec != null) { hash ^= AppearanceSpec.GetHashCode(); }
            hash ^= UnknownInt.GetHashCode();
            hash ^= UnknownBool.GetHashCode();
            hash ^= UnknownInt2.GetHashCode();
            if (CooldownTimerSpecs != null) foreach (var x in CooldownTimerSpecs) { hash ^= x.GetHashCode(); } //dictionaries need to hashed like this
            if(AbsorbParams.Count > 0)
            {
                foreach(Dictionary<string, float> dict in AbsorbParams)
                {
                    foreach(KeyValuePair<string, float> kvp in dict)
                    {
                        hash ^= kvp.Key.GetHashCode();
                        hash ^= kvp.Value.GetHashCode();
                    }
                }
            }

            return hash;
        }

        public override string ToString()
        {
            return string.Format("{0} {1} {2}", NameId, Name, Description);
        }

        public override bool Equals(object obj)
        {
            if (obj == null) return false;

            if (ReferenceEquals(this, obj)) return true;

            Ability abl = obj as Ability;
            if (abl == null) return false;

            return Equals(abl);
        }

        public bool Equals(Ability abl)
        {
            if (abl == null) return false;

            if (ReferenceEquals(this, abl)) return true;

            //if (this.ablEffects != abl.ablEffects)
                //return false;
            if (this.AiType != abl.AiType)
                return false;
            if (this.ApCost != abl.ApCost)
                return false;
            if (this.AppearanceSpec != abl.AppearanceSpec)
                return false;
            if (this.ApType != abl.ApType)
                return false;
            if (this.AutoAttackMode != abl.AutoAttackMode)
                return false;
            if (this.CastingTime != abl.CastingTime)
                return false;
            if (this.ChannelingTime != abl.ChannelingTime)
                return false;
            if (this.CombatMode != abl.CombatMode)
                return false;
            if (this.Cooldown != abl.Cooldown)
                return false;

            var iuComp = new DictionaryComparer<int, ulong>();
            if (!iuComp.Equals(this.CooldownTimerSpecs, abl.CooldownTimerSpecs))
                return false;

            if (this.Description != abl.Description)
                return false;
            if (this.DescriptionId != abl.DescriptionId)
                return false;

            var soComp = new DictionaryComparer<string, object>();
            if (this.DescriptionTokens != null)
            {
                if (abl.DescriptionTokens == null)
                    return false;
                foreach (var token in this.DescriptionTokens)
                {
                    var prevTok = new Dictionary<string, object>();
                    abl.DescriptionTokens.TryGetValue(token.Key, out prevTok);
                    if (!soComp.Equals(token.Value, prevTok))
                        return false;
                }
            }
            else if (abl.DescriptionTokens != null)
                return false;

            if (this.EnergyCost != abl.EnergyCost)
                return false;
            if (this.ForceCost != abl.ForceCost)
                return false;
            if (this.Fqn != abl.Fqn)
                return false;
            if (this.GCD != abl.GCD)
                return false;
            if (this.GcdOverride != abl.GcdOverride)
                return false;
            if (this.Icon != abl.Icon)
                return false;
            if (this.Id != abl.Id)
                return false;
            if (this.IgnoreAlacrity != abl.IgnoreAlacrity)
                return false;
            if (this.IsCustom != abl.IsCustom)
                return false;
            if (this.IsHidden != abl.IsHidden)
                return false;
            if (this.IsPassive != abl.IsPassive)
                return false;
            if (this.IsValid != abl.IsValid)
                return false;
            if (this.Level != abl.Level)
                return false;
            if (this.LineOfSightCheck != abl.LineOfSightCheck)
                return false;

            var ssComp = new DictionaryComparer<string, string>();
            if (!ssComp.Equals(this.LocalizedDescription, abl.LocalizedDescription))
                return false;
            if (!ssComp.Equals(this.LocalizedName, abl.LocalizedName))
                return false;

            if (this.MaxRange != abl.MaxRange)
                return false;
            if (this.MinRange != abl.MinRange)
                return false;
            if (this.ModalGroup != abl.ModalGroup)
                return false;
            if (this.Name != abl.Name)
                return false;
            if (this.NameId != abl.NameId)
                return false;
            if (this.NodeId != abl.NodeId)
                return false;
            if (this.Pushback != abl.Pushback)
                return false;
            if (this.SharedCooldown != abl.SharedCooldown)
                return false;
            if (this.TargetArc != abl.TargetArc)
                return false;
            if (this.TargetArcOffset != abl.TargetArcOffset)
                return false;
            if (this.TargetRule != abl.TargetRule)
                return false;
            if (this.UnknownBool != abl.UnknownBool)
                return false;
            if (this.UnknownInt != abl.UnknownInt)
                return false;
            if (this.UnknownInt2 != abl.UnknownInt2)
                return false;
            if (this.Version != abl.Version)
                return false;
            if (this.AbsorbParams.Count != abl.AbsorbParams.Count)
                return false;

            DictionaryComparer<string, float> absorbCompare = new DictionaryComparer<string, float>();
            for (int i = 0; i < this.AbsorbParams.Count; i++ )
            {
                if (!absorbCompare.Equals(this.AbsorbParams[i], abl.AbsorbParams[i]))
                    return false;
            }
            
            return true;
        }

        public override string ToString(bool verbose){
            var txtFile = new StringBuilder();
            string n = Environment.NewLine;

            if (this.NodeId == 0) return "";

            txtFile.Append("Name: " + Name + n);
            txtFile.Append("NodeId: " + NodeId + n);
            txtFile.Append("Id: " + Id + n);
            txtFile.Append("  Flags (Passive/Hidden): " + IsPassive + "/" + IsHidden + n);
            txtFile.Append("  Description: " + Description + " (" + DescriptionId + ")" + n);

            txtFile.Append("  Fqn: " + Fqn + n);
            txtFile.Append("  Icon: " + Icon + n);
            txtFile.Append("  level: " + Level + n);
            txtFile.Append("  Range (Min/Max): " + MinRange + "/" + MaxRange + n);
            if (ApCost != 0) { txtFile.Append("  Ammo/Heat Cost: " + ApCost + n); }
            if (EnergyCost != 0) { txtFile.Append("  Energy Cost: " + EnergyCost + n); }
            if (ForceCost != 0) { txtFile.Append("  Force Cost: " + ForceCost + n); }
            if (ChannelingTime != 0) { txtFile.Append("  Channeling Time: " + ChannelingTime + n); }
            else
            {
                if (CastingTime != 0) { txtFile.Append("  Casting Time: " + CastingTime + n); }
                else { txtFile.Append("  Casting Time: Instant" + n); }
            }
            txtFile.Append("  Cooldown Time: " + Cooldown + n);
            if (GCD != -1) { txtFile.Append("  GCD: " + GCD + n); } else { txtFile.Append("  GCD: No GCD" + n); }
            if (GcdOverride) { txtFile.Append("  Overrides GCD: " + GcdOverride + n); }
            txtFile.Append("  Uses Pushback: " + Pushback + n);
            txtFile.Append("  Ignores Alacrity: " + IgnoreAlacrity + n);
            txtFile.Append("  LoS Check: " + LineOfSightCheck + n);
            if (ModalGroup != 0) { txtFile.Append("  Modal Group: " + ModalGroup + n); }
            if (SharedCooldown != 0) { txtFile.Append("  Shared Cooldown: " + SharedCooldown + n); }
            if (TargetArc != 0 && TargetArcOffset != 0) { txtFile.Append("  Target Arc/Offset: " + TargetArc + "/" + TargetArcOffset + n); }
            txtFile.Append("  Target Rule: " + (GomLib.Models.TargetRule)TargetRule + n + n);

            txtFile.Append("  Tokens: " + n);

            if (DescriptionTokens != null && DescriptionTokens.Count > 0)
            {
                //Add all the tokens.
                foreach (KeyValuePair<int, Dictionary<string, object>> kvp in DescriptionTokens)
                {
                    //Create the Token node and give it the token ID.
                    txtFile.Append("    Token ID: " + kvp.Key + n);

                    //Lets get the token type..
                    object tokenTypeObj;
                    string tokenType = string.Empty;
                    if (kvp.Value.TryGetValue("ablDescriptionTokenType", out tokenTypeObj))
                    {
                        tokenType = (string)tokenTypeObj;
                    }

                    //Handle these differently.
                    if (tokenType == "ablDescriptionTokenTypeDamage" || tokenType == "ablDescriptionTokenTypeHealing")
                    {
                        foreach (KeyValuePair<string, object> tokenValueKvp in kvp.Value)
                        {
                            //Handle this differently.
                            if (tokenValueKvp.Key == "ablParsedDescriptionToken")
                            {
                                txtFile.Append("      Coefficient Values:" + n);
                                List<KeyValuePair<string, List<Dictionary<string, string>>>> coeffValues
                                    = (List<KeyValuePair<string, List<Dictionary<string, string>>>>)tokenValueKvp.Value;

                                foreach (KeyValuePair<string, List<Dictionary<string, string>>> actionDetails in coeffValues)
                                {
                                    txtFile.Append("        Action - " + actionDetails.Key + ":" + n);
                                    foreach (Dictionary<string, string> actionDict in actionDetails.Value)
                                    {
                                        foreach (KeyValuePair<string, string> detailsKvp in actionDict)
                                        {
                                            txtFile.Append("          " + detailsKvp.Key + " = " + detailsKvp.Value + n);
                                        }
                                    }
                                }
                            }
                            else
                            {
                                txtFile.Append("      " + tokenValueKvp.Key + " = " + tokenValueKvp.Value + n);
                            }
                        }
                    }
                    else
                    {
                        foreach (KeyValuePair<string, object> finalKvp in kvp.Value)
                        {
                            txtFile.Append("      " + finalKvp.Key + " = " + finalKvp.Value + n);
                        }
                    }
                }
            }

            return txtFile.ToString();
        }

        public override List<SQLProperty> SQLProperties
        {
            get
            {
                return new List<SQLProperty>
                    {                //(SQL Column Name, C# Property Name, SQL Column type statement, isUnique/PrimaryKey, Serialize value to json)
                        new SQLProperty("Name", "Name", "varchar(255) COLLATE utf8_unicode_ci NOT NULL"),
                        new SQLProperty("NodeId", "NodeId", "bigint(20) unsigned NOT NULL"),
                        new SQLProperty("Base62Id", "Base62Id", "varchar(7) COLLATE latin1_general_cs NOT NULL", true),
                        new SQLProperty("NameId", "NameId", "bigint(20) NOT NULL"),
                        new SQLProperty("Description", "ParsedDescription", "varchar(255) COLLATE utf8_unicode_ci NOT NULL"),
                        new SQLProperty("DescriptionId", "DescriptionId", "bigint(20) NOT NULL"),
                        new SQLProperty("Fqn", "Fqn", "varchar(255) COLLATE utf8_unicode_ci NOT NULL"),
                        new SQLProperty("Level", "Level", "int(11) NOT NULL"),
                        new SQLProperty("Icon", "HashedIcon", "varchar(255) COLLATE utf8_unicode_ci NOT NULL"),
                        new SQLProperty("IsHidden", "IsHidden", "tinyint(1) NOT NULL"),
                        new SQLProperty("IsPassive", "IsPassive", "tinyint(1) NOT NULL"),
                        new SQLProperty("Cooldown", "Cooldown", "float NOT NULL"),
                        new SQLProperty("CastingTime", "CastingTime", "float NOT NULL"),
                        new SQLProperty("ForceCost", "ForceCost", "float NOT NULL"),
                        new SQLProperty("EnergyCost", "EnergyCost", "float NOT NULL"),
                        new SQLProperty("ApCost", "ApCost", "float NOT NULL"),
                        new SQLProperty("ApType", "ApType", "varchar(25) COLLATE utf8_unicode_ci NOT NULL"),
                        new SQLProperty("MinRange", "MinRange", "float NOT NULL"),
                        new SQLProperty("MaxRange", "MaxRange", "float NOT NULL"),
                        new SQLProperty("Gcd", "GCD", "int(11) NOT NULL"),
                        new SQLProperty("GcdOverride", "GcdOverride", "tinyint(1) NOT NULL"),
                        new SQLProperty("ModalGroup", "ModalGroup", "bigint(20) NOT NULL"),
                        new SQLProperty("SharedCooldown", "SharedCooldown", "bigint(20) unsigned NOT NULL"),
                        new SQLProperty("DescriptionTokens", "modDescriptionTokens", "varchar(2000) COLLATE utf8_unicode_ci NOT NULL", false, true),
                        //new SQLProperty("AbilityTokens", "AbilityTokens", "varchar(255) COLLATE utf8_unicode_ci NOT NULL"),
                        new SQLProperty("TargetArc", "TargetArc", "float NOT NULL"),
                        new SQLProperty("TargetArcOffset", "TargetArcOffset", "float NOT NULL"),
                        new SQLProperty("TargetRule", "TargetRule", "varchar(255) COLLATE utf8_unicode_ci NOT NULL"),
                        new SQLProperty("LineOfSightCheck", "LineOfSightCheck", "tinyint(1) NOT NULL"),
                        new SQLProperty("Pushback", "Pushback", "tinyint(1) NOT NULL"),
                        new SQLProperty("IgnoreAlacrity", "IgnoreAlacrity", "tinyint(1) NOT NULL")
                    };
            }
        }

        public override XElement ToXElement(bool verbose)
        {
            XElement ability = new XElement("Ability");
            if (NodeId != 0)
            {
                /*if (!File.Exists(String.Format("{0}{1}/icons/{2}.dds", Config.ExtractPath, prefix, Icon)) && Icon != null)
                {
                    var stream = _dom._assets.FindFile(String.Format("/resources/gfx/icons/{0}.dds", Icon));
                    if (stream != null)
                        WriteFile((MemoryStream)stream.OpenCopyInMemory(), String.Format("/icons/{0}.dds", Icon));
                }*/
                if (Fqn == null) return ability;
                ability.Add(new XElement("Fqn", Fqn),
                        new XAttribute("Id", Id),
                        new XElement("Base62Id", Base62Id),
                        new XElement("Name", Name),
                        new XElement("Description", Description),
                        new XElement("DBURL", "https://torcommunity.com/database/ability/" + Base62Id + "/" + System.Web.HttpUtility.UrlEncode(Name.ToLower())));

                if (verbose)
                {
                    /*ability.Element("Name").RemoveAll(); //removes base text to replace with localized variants.
                    for (int i = 0; i < localizations.Count; i++)
                    {
                        if (LocalizedName[localizations[i]] != "")
                        {
                            ability.Element("Name").Add(new XElement(localizations[i], LocalizedName[localizations[i]]));
                        }
                    }

                    ability.Element("Description").RemoveAll();
                    for (int i = 0; i < localizations.Count; i++)
                    {
                        if (LocalizedDescription[localizations[i]] != "")
                        {
                            ability.Element("Description").Add(new XElement(localizations[i], LocalizedDescription[localizations[i]]));
                        }
                    } */
                    ability.Element("Name").Add(new XAttribute("Id", NameId));
                    ability.Element("Fqn").Add(new XAttribute("NodeId", NodeId));
                    ability.Element("Description").Add(new XAttribute("Id", DescriptionId));
                    ability.Add(new XElement("IsPassive", IsPassive),
                        new XElement("IsHidden", IsHidden),
                        new XElement("Tokens"),
                        new XElement("Icon", Icon),
                        new XElement("level", Level),
                        new XElement("MinRange", MinRange * 10),
                        new XElement("MaxRange", MaxRange * 10),
                        new XElement("AmmoHeatCost", ApCost),
                        new XElement("EnergyCost", EnergyCost),
                        new XElement("ForceCost", ForceCost),
                        new XElement("ChannelingTime", ChannelingTime),
                        new XElement("CastingTime", CastingTime),
                        new XElement("CooldownTime", Cooldown),
                        new XElement("GCD", GCD),
                        new XElement("OverridesGCD", GcdOverride),
                        new XElement("UsesPushback", Pushback),
                        new XElement("IgnoresAlacrity", IgnoreAlacrity),
                        new XElement("LoSCheck", LineOfSightCheck),
                        new XElement("ModalGroup", ModalGroup),
                        new XElement("SharedCooldown", SharedCooldown),
                        new XElement("TargetArc", TargetArc),
                        new XElement("TargetArcOffset", TargetArcOffset),
                        new XElement("TargetRule", (GomLib.Models.TargetRule)TargetRule));

                    if (DescriptionTokens != null && DescriptionTokens.Count > 0)
                    {
                        //Add all the tokens.
                        XElement tokenRootElem = ability.Element("Tokens");
                        foreach (KeyValuePair<int, Dictionary<string, object>> kvp in DescriptionTokens)
                        {
                            //Create the Token node and give it the token ID.
                            XElement tokenElem = new XElement("Token", new XAttribute("Id", kvp.Key));

                            //Lets get the token type..
                            object tokenTypeObj;
                            string tokenType = string.Empty;
                            if (kvp.Value.TryGetValue("ablDescriptionTokenType", out tokenTypeObj))
                            {
                                tokenType = (string)tokenTypeObj;
                            }

                            //Handle these differently.
                            if (tokenType == "ablDescriptionTokenTypeDamage" || tokenType == "ablDescriptionTokenTypeHealing")
                            {
                                foreach (KeyValuePair<string, object> tokenValueKvp in kvp.Value)
                                {
                                    //Handle this differently.
                                    if (tokenValueKvp.Key == "ablParsedDescriptionToken")
                                    {
                                        XElement detailsElem = new XElement("ablCoefficients");
                                        List<KeyValuePair<string, List<Dictionary<string, string>>>> coeffValues
                                            = (List<KeyValuePair<string, List<Dictionary<string, string>>>>)tokenValueKvp.Value;

                                        foreach (KeyValuePair<string, List<Dictionary<string, string>>> actionDetails in coeffValues)
                                        {
                                            XElement actionElem = new XElement("Action",
                                                new XAttribute("Name", actionDetails.Key));
                                            foreach (Dictionary<string, string> actionDict in actionDetails.Value)
                                            {
                                                foreach (KeyValuePair<string, string> detailsKvp in actionDict)
                                                {
                                                    actionElem.Add(new XElement(detailsKvp.Key, detailsKvp.Value));
                                                }
                                            }
                                            detailsElem.Add(actionElem);
                                        }
                                        tokenElem.Add(detailsElem);
                                    }
                                    else
                                    {
                                        tokenElem.Add(new XElement(tokenValueKvp.Key, tokenValueKvp.Value));
                                    }
                                }
                            }
                            else
                            {
                                foreach (KeyValuePair<string, object> finalKvp in kvp.Value)
                                {
                                    tokenElem.Add(new XElement(finalKvp.Key, finalKvp.Value));
                                }
                            }
                            tokenRootElem.Add(tokenElem);
                        }
                    }

                    if(AbsorbParams.Count > 0)
                    {
                        XElement absorbCoEffRoot = new XElement("AbsorbCoEfficients");

                        int i = 0;
                        foreach(Dictionary<string, float> absStatByParam in AbsorbParams)
                        {
                            XElement absorbRoot = new XElement("AbsorbCoEfficient", new XAttribute("Id", i));
                            i++;

                            foreach(KeyValuePair<string, float> kvp in absStatByParam)
                            {
                                absorbRoot.Add(new XElement(kvp.Key, kvp.Value));
                            }
                            absorbCoEffRoot.Add(absorbRoot);
                        }

                        ability.Add(absorbCoEffRoot);
                    }
                }

                ability.Elements().Where(x => x.Value == "0" || x.IsEmpty).Remove();
            }
            return ability;
        }

        public string generateTokenString(string value)
        {
            var retval = "";

            var splitTokens = value.Split(';');
            /*if (splitTokens.length == 2)
                retval = splitTokens[1] + " to " + splitTokens[0];
            else*/
            retval = splitTokens[0];

            var tokArray = splitTokens[0].Split(',');

            switch (tokArray[0])
            {
                case "damage":
                    if (tokArray.Count() == 7)
                    {
                        var minp = float.Parse(tokArray[5]);
                        var maxp = float.Parse(tokArray[6]);
                        if (float.Parse(tokArray[4]) == 1)
                            retval = Math.Round(float.Parse(tokArray[4]) * minp) + "-" + Math.Round(float.Parse(tokArray[4]) * maxp);
                        else
                            retval = Math.Round(float.Parse(tokArray[4]) * ((minp + maxp) / 2)).ToString();
                    }
                    else
                    {
                        switch (tokArray[4])
                        {
                            case "w":
                                var min = (float.Parse(tokArray[11]) + 1.0) * 405 + float.Parse(tokArray[6]) * 1000 + float.Parse(tokArray[7]) * 3185; /*(AmountModifierPercent + 1) * 405 * 0.3 + */
                                var max = (float.Parse(tokArray[11]) + 1.0) * 607 + float.Parse(tokArray[6]) * 1000 + float.Parse(tokArray[8]) * 3185; /*(AmountModifierPercent + 1) * 607 * 0.3 + */
                                //console.log("(" + tokArray[11] + " + 1.0) * 405 + " + tokArray[6] + " * 1000 + " + tokArray[8]  + " * 3185");
                                if (float.Parse(tokArray[5]) == 1)
                                    retval = Math.Round(float.Parse(tokArray[5]) * min) + "-" + Math.Round(float.Parse(tokArray[5]) * max);
                                else
                                    retval = Math.Round(float.Parse(tokArray[5]) * ((min + max) / 2)).ToString();
                                break;
                            case "s":
                                var mins = float.Parse(tokArray[6]) * 1000 + float.Parse(tokArray[7]) * 3185;
                                var maxs = float.Parse(tokArray[6]) * 1000 + float.Parse(tokArray[8]) * 3185;
                                if (float.Parse(tokArray[5]) == 1)
                                    retval = Math.Round(float.Parse(tokArray[5]) * mins) + "-" + Math.Round(float.Parse(tokArray[5]) * maxs);
                                else
                                    retval = Math.Round(float.Parse(tokArray[5]) * ((mins + maxs) / 2)).ToString();
                                break;
                        }
                    }
                    break;
                case "healing":
                    if (tokArray.Count() == 5)
                    {
                        var minh = float.Parse(tokArray[2]) * 1000 + float.Parse(tokArray[3]) * 14520;
                        var maxh = float.Parse(tokArray[2]) * 1000 + float.Parse(tokArray[4]) * 14520;
                        if (float.Parse(tokArray[1]) == 1)
                            retval = Math.Round(float.Parse(tokArray[1]) * minh) + "-" + Math.Round(float.Parse(tokArray[1]) * maxh);
                        else
                            retval = Math.Round(float.Parse(tokArray[1]) * ((minh + maxh) / 2)).ToString();
                    }
                    else
                    {
                        var mina = float.Parse(tokArray[2]);
                        var maxa = float.Parse(tokArray[3]);
                        if (float.Parse(tokArray[1]) == 1)
                            retval = Math.Round(float.Parse(tokArray[1]) * mina) + "-" + Math.Round(float.Parse(tokArray[1]) * maxa);
                        else
                            retval = Math.Round(float.Parse(tokArray[1]) * ((mina + maxa) / 2)).ToString();
                    }
                    break;
            }
            return retval;
        }
    }
}
