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

        public override int GetHashCode()  //needs fixed, it's changing for the same data
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

        public override string ToString(bool verbose){
            var txtFile = new StringBuilder();
            string n = Environment.NewLine;

            if (this.NodeId == 0) return "";

            txtFile.Append("Name: " + Name + n);
            txtFile.Append("NodeId: " + NodeId + n);
            txtFile.Append("Id: " + Id + n);
            txtFile.Append("  Flags (Passive/Hidden): " + IsPassive + "/" + IsHidden + n);
            txtFile.Append("  Description: " + Description + " (" + DescriptionId + ")" + n);
            txtFile.Append("  Tokens: " + TalentTokens + n);
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

            return txtFile.ToString();
        }

        public override List<SQLProperty> SQLProperties
        {
            get
            {
                return new List<SQLProperty>
                    {                //(SQL Column Name, C# Property Name, SQL Column type statement, isUnique/PrimaryKey, Serialize value to json)
                        new SQLProperty("Name", "Name", "varchar(255) COLLATE utf8_unicode_ci NOT NULL"),
                        new SQLProperty("NodeId", "NodeId", "bigint(20) unsigned NOT NULL", true),
                        new SQLProperty("NameId", "NameId", "bigint(20) NOT NULL"),
                        new SQLProperty("Description", "Description", "varchar(255) COLLATE utf8_unicode_ci NOT NULL"),
                        new SQLProperty("DescriptionId", "DescriptionId", "bigint(20) NOT NULL"),
                        new SQLProperty("Fqn", "Fqn", "varchar(255) COLLATE utf8_unicode_ci NOT NULL"),
                        new SQLProperty("Level", "Level", "int(11) NOT NULL"),
                        new SQLProperty("Icon", "Icon", "varchar(255) COLLATE utf8_unicode_ci NOT NULL"),
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
                        new SQLProperty("TalentTokens", "TalentTokens", "varchar(255) COLLATE utf8_unicode_ci NOT NULL"),
                        new SQLProperty("AbilityTokens", "AbilityTokens", "varchar(255) COLLATE utf8_unicode_ci NOT NULL"),
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
                        new XElement("Name", Name),
                        new XElement("Description", Description));

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
                        new XElement("Tokens", (DescriptionTokens ?? new Dictionary<int, Dictionary<string, object>>()).Select(x => TokenToXElement(x))),
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
                }
                ability.Elements().Where(x => x.Value == "0" || x.IsEmpty).Remove();
            }
            return ability;
        }
        public static XElement TokenToXElement(KeyValuePair<int, Dictionary<string, object>> data)
        {
            return new XElement("Token", new XAttribute("Id", data.Key),
                data.Value.Select(x => new XElement(x.Key, x.Value)));
        }
    }
}
