using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;
using System.Xml.XPath;
using System.Text.RegularExpressions;
using System.Xml;

namespace GomLib.Models
{
    public class Ability : GameObject, IEquatable<Ability>
    {
        public ulong NodeId { get; set; }

        public long NameId { get; set; }
        [Newtonsoft.Json.JsonIgnore]
        public string Name { get; set; }
        public Dictionary<string, string> LocalizedName { get; set; }
        public long DescriptionId { get; set; }
        [Newtonsoft.Json.JsonIgnore]
        public string Description { get; set; }
        public Dictionary<string, string> LocalizedDescription { get; set; }

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
        public string TalentTokens { get; set; }
        public string AbilityTokens { get; set; }
        public float TargetArc { get; set; }
        public float TargetArcOffset { get; set; }
        public int TargetRule { get; set; }
        public bool LineOfSightCheck { get; set; }
        public bool Pushback { get; set; }
        public bool IgnoreAlacrity { get; set; }

        public Dictionary<int, Dictionary<string, object>> DescriptionTokens { get; set; }
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

        public override int GetHashCode()
        {
            int hash = Level.GetHashCode();
            if (LocalizedDescription != null) { hash ^= LocalizedDescription.GetHashCode(); }
            if (LocalizedName != null) { hash ^= LocalizedName.GetHashCode(); }
            if (Icon != null) { hash ^= Icon.GetHashCode(); }
            hash ^= IsHidden.GetHashCode();
            hash ^= IsPassive.GetHashCode();
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
            if (AbilityTokens != null) { hash ^= AbilityTokens.GetHashCode(); }
            hash ^= TargetArc.GetHashCode();
            hash ^= TargetArcOffset.GetHashCode();
            hash ^= TargetRule.GetHashCode();
            hash ^= LineOfSightCheck.GetHashCode();
            hash ^= Pushback.GetHashCode();
            hash ^= IgnoreAlacrity.GetHashCode();
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

            if (this.AbilityTokens != abl.AbilityTokens)
                return false;
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
            if (this.TalentTokens != abl.TalentTokens)
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
            return true;
        }
    }
}
