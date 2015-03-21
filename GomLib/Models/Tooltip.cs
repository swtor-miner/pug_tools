using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;
using System.Xml.XPath;
using System.Xml;

namespace GomLib.Models
{
    public class Tooltip : GameObject
    {
        public Tooltip() { }
        public Tooltip(GameObject obj)
        {
            _obj = obj;
            Id = obj.Id;
            Fqn = obj.Fqn;
            type = obj.GetType().ToString();
        }

        internal GameObject _obj { get; set; }
        public string type { get; set; }
        public string HTML
        {
            get { return GetHTML(); }
        }

        public override List<SQLProperty> SQLProperties
        {
            get
            {
                return new List<SQLProperty>
                    {                //(SQL Column Name, C# Property Name, SQL Column type statement, isUnique/PrimaryKey, Serialize value to json)
                        new SQLProperty("NodeId", "Id", "bigint(20) unsigned NOT NULL", true),
                        new SQLProperty("Fqn", "Fqn", "varchar(255) COLLATE utf8_unicode_ci NOT NULL"),
                        new SQLProperty("Tooltip", "HTML", "varchar(10000) COLLATE utf8_unicode_ci NOT NULL")
                    };
            }
        }

        private string GetHTML()
        {
            if (_obj == null)
                return null;
            Item itm = null;
            if (_obj.GetType() == typeof(Item))
                itm = (Item)_obj;

            StringBuilder tooltip = new StringBuilder();
            tooltip.Append("<div class='torctip_tooltip'>");
            if (itm != null)
            {
                tooltip.Append(String.Format("<span class='torctip_{0}'>{1}</span>", itm.Quality.ToString().ToLower(), itm.Name));
                //binding
                if (itm.Binding != 0)
                {
                    tooltip.Append(String.Format("<div class='torctip_text'>Binds on {0}</div>", itm.Binding.ToString()));
                }
                //slot
                if (itm.Slots.Count > 1)
                {
                    tooltip.Append(String.Format("<div class='torctip_text'>{0}</div>", String.Join(", ", itm.Slots.Select(x => SlotToString(x)).Where(x => x != null).ToList())));
                }
                //armor, rating etc if gear
                float techpower = 0;
                float forcepower = 0;
                if (itm.WeaponSpec != null)
                {
                    List<int> mainSlots = new List<int> { 1, 3, 9 };
                    ItemEnhancement mainMod = null;
                    if (itm.EnhancementSlots != null)
                    {
                        var potentials = itm.EnhancementSlots.Where(x => x.Slot.IsBaseMod());
                        if (potentials.Count() != 0)
                            mainMod = itm.EnhancementSlots.Where(x => x.Slot.IsBaseMod()).Single();
                    }
                    //if (mainMod.Count == 0
                    ItemQuality qual = ItemQuality.Premium;
                    if (itm.EnhancementSlots.Count() != 0)
                    {
                        if (mainMod != null)
                        {
                            if (mainMod.ModificationId != 0)
                                qual = mainMod.Modification.Quality;
                            //else
                            //nothing premium is what we want
                        }
                    }
                    else
                        qual = itm.Quality;
                    float min = itm._dom.data.weaponPerLevel.GetStat(itm.WeaponSpec.Id, itm.RequiredLevel, itm.Quality, Stat.MinWeaponDamage);
                    float max = itm._dom.data.weaponPerLevel.GetStat(itm.WeaponSpec.Id, itm.RequiredLevel, itm.Quality, Stat.MaxWeaponDamage);
                    techpower = itm._dom.data.weaponPerLevel.GetStat(itm.WeaponSpec.Id, itm.RequiredLevel, itm.Quality, Stat.TechPowerRating);
                    forcepower = itm._dom.data.weaponPerLevel.GetStat(itm.WeaponSpec.Id, itm.RequiredLevel, itm.Quality, Stat.ForcePowerRating);
                    tooltip.Append(String.Format("<div class='torctip_text'><span class='torctip_minDam'>{0}</span>-<span class='torctip_maxDam'>{1}</span> {2} Damage (Rating {3})</div>", min.ToString("0.0"), max.ToString("0.0"), itm.WeaponSpec.DamageType, itm.CombinedRating)); //this needs a conditional
                }
                else
                    tooltip.Append(String.Format("<div class='torctip_text'>{0} Armor (Rating {1})</div>", 0 /*calculate this*/, itm.CombinedRating)); //this needs a conditional

                //stats
                if (itm.CombinedStatModifiers.Count != 0)
                {
                    tooltip.Append("<div class='torctip_stats'><span class='torctip_white'>Total Stats:</span>");
                    for (var i = 0; i < itm.CombinedStatModifiers.Count; i++)
                    {
                        tooltip.Append(String.Format("<div class='torctip_stat' id='torctip_stat_{0}'><span>+{1} {2}</span></div>",
                            // {0}              {1}                                     {2}
                            i, itm.CombinedStatModifiers[i].Modifier, StatToString(itm.CombinedStatModifiers[i].Stat)));
                    }
                    if (techpower > 0)
                        tooltip.Append(String.Format("<div class='torctip_stat' id='torctip_stat_tech'><span>+{0} Tech Power</span></div>", techpower));
                    if (forcepower > 0)
                        tooltip.Append(String.Format("<div class='torctip_stat' id='torctip_stat_force'><span>+{0} Force Power</span></div>", forcepower));
                    tooltip.Append("</div>");
                }
                //Modifications
                if (itm.EnhancementSlots.Count != 0)
                {
                    tooltip.Append("<div class='torctip_mods'><span class='torctip_white'>Item Modifications:</span>");
                    for (var i = 0; i < itm.EnhancementSlots.Count; i++)
                    {
                        if (itm.EnhancementSlots[i].ModificationId != 0)
                        {
                            tooltip.Append(String.Format("<div class='torctip_mod'><div class='torctip_mslot'><span class='torctip_{0}'>{1} ({2})</span></div>",
                                //                      {0}                                         {1}                                             {2}
                                itm.EnhancementSlots[i].Modification.Quality.ToString(), itm.EnhancementSlots[i].Slot.ToString(), itm.EnhancementSlots[i].Modification.Rating.ToString()));
                            for (var e = 0; e < itm.EnhancementSlots[i].Modification.CombinedStatModifiers.Count; e++)
                            {
                                tooltip.Append(String.Format("<div class='torctip_mstat'><span>+{1} {2}</span></div>", i, itm.EnhancementSlots[i].Modification.CombinedStatModifiers[e].Modifier, StatToString(itm.EnhancementSlots[i].Modification.CombinedStatModifiers[e].Stat)));
                            }
                            tooltip.Append("</div>");
                        }
                        else
                            //empty mod
                            tooltip.Append(String.Format("<div class='torctip_mod'><div class='torctip_mslot'><span>{0}: Open</span></div></div>", itm.EnhancementSlots[i].Slot.ToString()));
                    }
                    tooltip.Append("</div>");
                }

                if (itm.RequiredLevel != 0)
                    tooltip.Append(String.Format("<div>Requires Level {0}</div>", itm.RequiredLevel));
                System.Text.RegularExpressions.Regex regex_newline = new System.Text.RegularExpressions.Regex("(\r\n|\r|\n)");
                if (itm.UseAbilityId != 0)
                {
                    if (itm.UseAbility != null)
                    {
                        string ablDesc = itm.UseAbility.Description ?? "";
                        regex_newline.Replace(ablDesc, "<br />");
                        tooltip.Append(String.Format("<div class='torctip_text'>Use: </div>", ablDesc));
                    }
                }
                if (itm.Description != "")
                {
                    regex_newline.Replace(itm.Description, "<br />");
                    tooltip.Append(String.Format("<div class='torctip_text'>{0}</div>", itm.Description));
                }
            }

            tooltip.Append("</div>"); //close tooltip div
            return tooltip.ToString();
        }

        public static string SlotToString(SlotType slot) //replace these with friendly names
        {
            switch (slot)
            {
                case SlotType.EquipHumanMainHand: return "Main Hand (Melee)";
                case SlotType.EquipHumanOffHand: return "Off-Hand (Melee)";
                case SlotType.EquipHumanWrist: return "Wrist";
                case SlotType.EquipHumanBelt: return "Belt";
                case SlotType.EquipHumanChest: return "Chest";
                case SlotType.EquipHumanEar: return "Ear";
                case SlotType.EquipHumanFace: return "Face";
                case SlotType.EquipHumanFoot: return "Foot";
                case SlotType.EquipHumanGlove: return "Glove";
                case SlotType.EquipHumanImplant: return "Implant";
                case SlotType.EquipHumanLeg: return "Leg";
                case SlotType.EquipDroidUpper: return "DroidUpper";
                case SlotType.EquipDroidLower: return "DroidLower";
                case SlotType.EquipDroidShield: return "DroidShield";
                case SlotType.EquipDroidGyro: return "DroidGyro";
                case SlotType.EquipDroidUtility: return "DroidUtility";
                case SlotType.EquipDroidSensor: return "DroidSensor";
                case SlotType.EquipDroidSpecial: return "DroidSpecial";
                case SlotType.EquipDroidWeapon1: return "DroidWeapon1";
                case SlotType.EquipDroidWeapon2: return "DroidWeapon2";
                case SlotType.Upgrade: return "Upgrade";
                case SlotType.EquipHumanRanged: return "Ranged";
                case SlotType.EquipHumanHeirloom: return "Heirloom";
                case SlotType.EquipHumanRangedPrimary: return "Main Hand (Ranged)";
                case SlotType.EquipHumanRangedSecondary: return "Off-Hand (Ranged)";
                case SlotType.EquipHumanRangedTertiary: return "RangedTertiary";
                case SlotType.EquipHumanCustomRanged: return "CustomRanged";
                case SlotType.EquipHumanCustomMelee: return "CustomMelee";
                case SlotType.EquipHumanShield: return "Shield";
                case SlotType.EquipHumanOutfit: return "Outfit";
                case SlotType.EquipDroidLeg: return "DroidLeg";
                case SlotType.EquipDroidFeet: return "DroidFeet";
                case SlotType.EquipDroidOutfit: return "DroidOutfit";
                case SlotType.EquipDroidChest: return "DroidChest";
                case SlotType.EquipDroidHand: return "DroidHand";
                case SlotType.EquipHumanLightSide: return "LightSide";
                case SlotType.EquipHumanDarkSide: return "DarkSide";
                case SlotType.EquipHumanRelic: return "Relic";
                case SlotType.EquipHumanFocus: return "Focus";
                case SlotType.EquipSpaceShipArmor: return "SpaceShipArmor";
                case SlotType.EquipSpaceBeamGenerator: return "SpaceBeamGenerator";
                case SlotType.EquipSpaceBeamCharger: return "SpaceBeamCharger";
                case SlotType.EquipSpaceEnergyShield: return "SpaceEnergyShield";
                case SlotType.EquipSpaceShieldRegenerator: return "SpaceShieldRegenerator";
                case SlotType.EquipSpaceMissileMagazine: return "SpaceMissileMagazine";
                case SlotType.EquipSpaceProtonTorpedoes: return "SpaceProtonTorpedoes";
                case SlotType.EquipSpaceAbilityDefense: return "SpaceAbilityDefense";
                case SlotType.EquipSpaceAbilityOffense: return "SpaceAbilityOffense";
                case SlotType.EquipSpaceAbilitySystems: return "SpaceAbilitySystems";
                case SlotType.EquipSpaceShipAbilityDefense: return "SpaceShipAbilityDefense";
                case SlotType.EquipSpaceShipAbilityOffense: return "SpaceShipAbilityOffense";
                case SlotType.EquipSpaceShipAbilitySystems: return "SpaceShipAbilitySystems";
                case SlotType.Any: return null;
                default:
                    return "";
            }
        }

        public static string StatToString(Stat slot) //replace these with friendly names
        {
            switch (slot)
            {
                case Stat.Undefined: return "Undefined";
                case Stat.Strength: return "Strength";
                case Stat.Aim: return "Aim";
                case Stat.Cunning: return "Cunning";
                case Stat.Endurance: return "Endurance";
                case Stat.Presence: return "Presence";
                case Stat.Willpower: return "Willpower";
                case Stat.MaxHealth: return "MaxHealth";
                case Stat.MaxEnergy: return "MaxEnergy";
                case Stat.MaxForce: return "MaxForce";
                case Stat.MaxWeaponDamage: return "debug_MaxWeaponDamage";
                case Stat.MinWeaponDamage: return "debug_MinWeaponDamage";
                case Stat.FlurryBlowDelay: return "debug_FlurryBlowDelay";
                case Stat.FlurryLength: return "debug_FlurryLength";
                case Stat.RangeMax: return "debug_RangeMax";
                case Stat.RangeMin: return "debug_RangeMin";
                case Stat.DamageReductionElemental: return "debug_DamageReductionElemental";
                case Stat.DamageReductionInternal: return "debug_DamageReductionInternal";
                case Stat.DamageReductionKinetic: return "debug_DamageReductionKinetic";
                case Stat.DamageReductionEnergy: return "debug_DamageReductionEnergy";
                case Stat.RangedDamageBonus: return "debug_RangedDamageBonus";
                case Stat.MeleeDamageBonus: return "debug_MeleeDamageBonus";
                case Stat.FlurryDelay: return "debug_FlurryDelay";
                case Stat.PersuasionScore: return "debug_PersuasionScore";
                case Stat.PersuasionCrit: return "debug_PersuasionCrit";
                case Stat.MovementSpeed: return "debug_MovementSpeed";
                case Stat.MaxAP: return "debug_MaxAP";
                case Stat.RunSpeed: return "debug_RunSpeed";
                case Stat.WalkSpeed: return "debug_WalkSpeed";
                case Stat.ArmorRating: return "debug_ArmorRating";
                case Stat.ArenaRating: return "debug_ArenaRating";
                case Stat.RunBackwardsSpeed: return "debug_RunBackwardsSpeed";
                case Stat.TurnSpeed: return "debug_TurnSpeed";
                case Stat.EffectInitialCharges: return "debug_EffectInitialCharges";
                case Stat.EffectMaxCharges: return "debug_EffectMaxCharges";
                case Stat.CastingTime: return "debug_Casting Time";
                case Stat.ChannelingTime: return "debug_Channeling Time";
                case Stat.CooldownTime: return "debug_Cooldown Time";
                case Stat.Duration: return "debug_Duration";
                case Stat.GlobalCooldown: return "debug_GlobalCooldown";
                case Stat.EffectTickTime: return "debug_EffectTickTime";
                case Stat.ForceCost: return "debug_ForceCost";
                case Stat.EnergyCost: return "debug_EnergyCost";
                case Stat.APCost: return "debug_APCost";
                case Stat.AbilityMaxRange: return "debug_AbilityMaxRange";
                case Stat.AbilityMinRange: return "debug_AbilityMinRange";
                case Stat.EffectAuraRadius: return "debug_EffectAuraRadius";
                case Stat.AbsorbDamagePerHitFixed: return "debug_AbsorbDamagePerHitFixed";
                case Stat.AbsorbDamagePerHitPercentage: return "debug_AbsorbDamagePerHitPercentage";
                case Stat.AbsorbDamageMaxFixed: return "debug_AbsorbDamageMaxFixed";
                case Stat.AbsorbDamageMaxPercentage: return "debug_AbsorbDamageMaxPercentage";
                case Stat.DamageDoneModifierFixed: return "debug_DamageDoneModifierFixed";
                case Stat.DamageDoneModifierPercentage: return "debug_DamageDoneModifierPercentage";
                case Stat.ThreatGeneratedModifierFixed: return "debug_ThreatGeneratedModifierFixed";
                case Stat.ThreatGeneratedModifierPercentage: return "debug_ThreatGeneratedModifierPercentage";
                case Stat.HealingDoneModifierFixed: return "debug_HealingDoneModifierFixed";
                case Stat.HealingDoneModifierPercentage: return "debug_HealingDoneModifierPercentage";
                case Stat.EffectBallisticImpulseMomentumFixed: return "debug_EffectBallisticImpulseMomentumFixed";
                case Stat.EffectBallisticImpulseMomentumPercentage: return "debug_EffectBallisticImpulseMomentumPercentage";
                case Stat.EffectModifyStatFixed: return "debug_EffectModifyStatFixed";
                case Stat.EffectModifyStatPercentage: return "debug_EffectModifyStatPercentage";
                case Stat.EffectRestoreApFixed: return "debug_EffectRestoreApFixed";
                case Stat.EffectRestoreApPercentage: return "debug_EffectRestoreApPercentage";
                case Stat.EffectRestoreForceFixed: return "debug_EffectRestoreForceFixed";
                case Stat.EffectRestoreForcePercentage: return "debug_EffectRestoreForcePercentage";
                case Stat.EffectRestoreEnergyFixed: return "debug_EffectRestoreEnergyFixed";
                case Stat.EffectRestoreEnergyPercentage: return "debug_EffectRestoreEnergyPercentage";
                case Stat.EffectSpendApFixed: return "debug_EffectSpendApFixed";
                case Stat.EffectSpendApPercentage: return "debug_EffectSpendApPercentage";
                case Stat.EffectSpendForceFixed: return "debug_EffectSpendForceFixed";
                case Stat.EffectSpendForcePercentage: return "debug_EffectSpendForcePercentage";
                case Stat.EffectSpendEnergyFixed: return "debug_EffectSpendEnergyFixed";
                case Stat.EffectSpendEnergyPercentage: return "debug_EffectSpendEnergyPercentage";
                case Stat.EffectSpendHealthFixed: return "debug_EffectSpendHealthFixed";
                case Stat.EffectSpendHealthPercentage: return "debug_EffectSpendHealthPercentage";
                case Stat.EffectAoeRadiusFixed: return "debug_EffectAoeRadiusFixed";
                case Stat.EffectAoeRadiusPercentage: return "debug_EffectAoeRadiusPercentage";
                case Stat.EffectAoeConeDistanceFixed: return "debug_EffectAoeConeDistanceFixed";
                case Stat.EffectAoeConeDistancePercentage: return "debug_EffectAoeConeDistancePercentage";
                case Stat.EffectAoeConeAngleFixed: return "debug_EffectAoeConeAngleFixed";
                case Stat.EffectAoeConeAnglePercentage: return "debug_EffectAoeConeAnglePercentage";
                case Stat.EffectAoeCylinderDistanceFixed: return "debug_EffectAoeCylinderDistanceFixed";
                case Stat.EffectAoeCylinderDistancePercentage: return "debug_EffectAoeCylinderDistancePercentage";
                case Stat.EffectAoeCylinderRadiusFixed: return "debug_EffectAoeCylinderRadiusFixed";
                case Stat.EffectAoeCylinderRadiusPercentage: return "debug_EffectAoeCylinderRadiusPercentage";
                case Stat.MeleeAccuracy: return "debug_Accuracy";
                case Stat.MeleeDefense: return "debug_MeleeDefense";
                case Stat.MeleeCriticalChance: return "debug_MeleeCriticalChance";
                case Stat.MeleeCriticalDamage: return "debug_MeleeCriticalDamage";
                case Stat.MeleeShieldChance: return "debug_MeleeShieldChance";
                case Stat.MeleeShieldAbsorb: return "debug_MeleeShieldAbsorb";
                case Stat.RangedAccuracy: return "debug_Accuracy";
                case Stat.RangedDefense: return "debug_RangedDefense";
                case Stat.RangedCriticalChance: return "debug_RangedCriticalChance";
                case Stat.RangedCriticalDamage: return "debug_RangedCriticalDamage";
                case Stat.RangedShieldChance: return "debug_RangedShieldChance";
                case Stat.RangedShieldAbsorb: return "debug_RangedShieldAbsorb";
                case Stat.DualWieldAccuracyModifierFixed: return "debug_DualWieldAccuracyModifierFixed";
                case Stat.CoverDefenseBonus: return "debug_CoverDefenseBonus";
                case Stat.DualWieldDamagePenalty: return "debug_DualWieldDamagePenalty";
                case Stat.StealthLevel: return "debug_StealthLevel";
                case Stat.StealthDetection: return "debug_StealthDetection";
                case Stat.WeaponAccuracy: return "debug_WeaponAccuracy";
                case Stat.GlanceRating: return "debug_GlanceRating";
                case Stat.ForceDamageBonus: return "debug_ForceDamageBonus";
                case Stat.TechDamageBonus: return "debug_TechDamageBonus";
                case Stat.ForceCriticalChance: return "debug_ForceCriticalChance";
                case Stat.ForceCriticalDamageBonus: return "debug_ForceCriticalDamageBonus";
                case Stat.TechCriticalChance: return "debug_TechCriticalChance";
                case Stat.TechCriticalDamageBonus: return "debug_TechCriticalDamageBonus";
                case Stat.ForceRegenOoc: return "debug_ForceRegenOoc";
                case Stat.EnergyRegen: return "debug_EnergyRegen";
                case Stat.HealthRegenOoc: return "debug_HealthRegenOoc";
                case Stat.PvpDamageBonus: return "debug_PvpDamageBonus";
                case Stat.PvpDamageReduction: return "debug_PvpDamageReduction";
                case Stat.PvpCriticalChance: return "debug_PvpCriticalChance";
                case Stat.PvpCriticalChanceReduction: return "debug_PvpCriticalChanceReduction";
                case Stat.PvpTraumaIgnore: return "debug_PvpTraumaIgnore";
                case Stat.ExpertiseRating: return "Expertise Rating";
                case Stat.PvpCriticalDamageReduction: return "debug_PvpCriticalDamageReduction";
                case Stat.AbsorptionRating: return "Absorption Rating";
                case Stat.AttackPowerRating: return "Power";
                case Stat.ForcePowerRating: return "Force Power";
                case Stat.TechPowerRating: return "Tech Power";
                case Stat.ForceRegenRating: return "debug_ForceRegenRating";
                case Stat.EnergyRegenRating: return "debug_EnergyRegenRating";
                case Stat.AccuracyRating: return "Accuracy Rating";
                case Stat.CriticalChanceRating: return "Critical Rating";
                case Stat.ForceRegen: return "debug_ForceRegen";
                case Stat.PvpTrauma: return "debug_PvpTrauma";
                case Stat.HealingReceivedModifierPercentage: return "debug_HealingReceivedModifierPercentage";
                case Stat.HealingReceivedModifierFixed: return "debug_HealingReceivedModifierFixed";
                case Stat.DamageReceievedModifierPercentage: return "debug_DamageReceievedModifierPercentage";
                case Stat.DamageReceivedModifierFixed: return "debug_DamageReceivedModifierFixed";
                case Stat.DefenseRating: return "debug_DefenseRating";
                case Stat.EffectGenerateHeatModifierFixed: return "debug_EffectGenerateHeatModifierFixed";
                case Stat.EffectGenerateHeatModifierPercentage: return "debug_EffectGenerateHeatModifierPercentage";
                case Stat.ArmorPenetration: return "debug_ArmorPenetration";
                case Stat.Artifice: return "debug_Artifice";
                case Stat.Armormech: return "debug_Armormech";
                case Stat.Armstech: return "debug_Armstech";
                case Stat.Biochem: return "debug_Biochem";
                case Stat.Cybertech: return "debug_Cybertech";
                case Stat.Synthweaving: return "debug_Synthweaving";
                case Stat.Archaeology: return "debug_Archaeology";
                case Stat.Bioanalysis: return "debug_Bioanalysis";
                case Stat.Scavenging: return "debug_Scavenging";
                case Stat.Slicing: return "debug_Slicing";
                case Stat.Diplomacy: return "debug_Diplomacy";
                case Stat.Research: return "debug_Research";
                case Stat.TreasureHunting: return "debug_TreasureHunting";
                case Stat.UnderworldTrading: return "debug_UnderworldTrading";
                case Stat.Crafting: return "debug_Crafting";
                case Stat.Harvesting: return "debug_Harvesting";
                case Stat.Mission: return "debug_Mission";
                case Stat.ArtificeEfficiency: return "debug_ArtificeEfficiency";
                case Stat.ArmormechEfficiency: return "debug_ArmormechEfficiency";
                case Stat.ArmstechEfficiency: return "debug_ArmstechEfficiency";
                case Stat.BiochemEfficiency: return "debug_BiochemEfficiency";
                case Stat.CybertechEfficiency: return "debug_CybertechEfficiency";
                case Stat.SynthweavingEfficiency: return "debug_SynthweavingEfficiency";
                case Stat.ArchaeologyEfficiency: return "debug_ArchaeologyEfficiency";
                case Stat.BioanalysisEfficiency: return "debug_BioanalysisEfficiency";
                case Stat.ScavengingEfficiency: return "debug_ScavengingEfficiency";
                case Stat.SlicingEfficiency: return "debug_SlicingEfficiency";
                case Stat.DiplomacyEfficiency: return "debug_DiplomacyEfficiency";
                case Stat.ResearchEfficiency: return "debug_ResearchEfficiency";
                case Stat.TreasureHuntingEfficiency: return "debug_TreasureHuntingEfficiency";
                case Stat.UnderworldTradingEfficiency: return "debug_UnderworldTradingEfficiency";
                case Stat.CraftingEfficiency: return "debug_CraftingEfficiency";
                case Stat.HarvestingEfficiency: return "debug_HarvestingEfficiency";
                case Stat.MissionEfficiency: return "debug_MissionEfficiency";
                case Stat.ArtificeCritical: return "debug_ArtificeCritical";
                case Stat.ArmormechCritical: return "debug_ArmormechCritical";
                case Stat.ArmstechCritical: return "debug_ArmstechCritical";
                case Stat.BiochemCritical: return "debug_BiochemCritical";
                case Stat.CybertechCritical: return "debug_CybertechCritical";
                case Stat.SynthweavingCritical: return "debug_SynthweavingCritical";
                case Stat.ArchaeologyCritical: return "debug_ArchaeologyCritical";
                case Stat.BioanalysisCritical: return "debug_BioanalysisCritical";
                case Stat.ScavengingCritical: return "debug_ScavengingCritical";
                case Stat.SlicingCritical: return "debug_SlicingCritical";
                case Stat.DiplomacyCritical: return "debug_DiplomacyCritical";
                case Stat.ResearchCritical: return "debug_ResearchCritical";
                case Stat.TreasureHuntingCritical: return "debug_TreasureHuntingCritical";
                case Stat.UnderworldTradingCritical: return "debug_UnderworldTradingCritical";
                case Stat.CraftingCritical: return "debug_CraftingCritical";
                case Stat.HarvestingCritical: return "debug_HarvestingCritical";
                case Stat.MissionCritical: return "debug_MissionCritical";
                case Stat.TechHealingPower: return "debug_TechHealingPower";
                case Stat.ForceHealingPower: return "debug_ForceHealingPower";
                case Stat.TargetLowDamageModifier: return "debug_TargetLowDamageModifier";
                case Stat.TargetBleedingDamageModifier: return "debug_TargetBleedingDamageModifier";
                case Stat.TargetStunnedDamageModifier: return "debug_TargetStunnedDamageModifier";
                case Stat.CasterStunnedDamageTakenModifier: return "debug_CasterStunnedDamageTakenModifier";
                case Stat.ProcRateModifier: return "debug_ProcRateModifier";
                case Stat.RateLimitModifierFixed: return "debug_RateLimitModifierFixed";
                case Stat.RateLimitModifierPercentage: return "debug_RateLimitModifierPercentage";
                case Stat.Resolve: return "debug_Resolve";
                case Stat.ForceAccuracy: return "debug_ForceAccuracy";
                case Stat.ForceDefense: return "debug_ForceDefense";
                case Stat.TechAccuracy: return "debug_TechAccuracy";
                case Stat.TechDefense: return "debug_TechDefense";
                case Stat.ShipArmor: return "debug_ShipArmor";
                case Stat.ShipRateOfFire: return "debug_ShipRateOfFire";
                case Stat.ShipBlasterDamage: return "debug_ShipBlasterDamage";
                case Stat.ShipMissileCount: return "debug_ShipMissileCount";
                case Stat.ShipMissileLevel: return "debug_ShipMissileLevel";
                case Stat.ShipMissileRateOfFire: return "debug_ShipMissileRateOfFire";
                case Stat.ShipTorpedoCount: return "debug_ShipTorpedoCount";
                case Stat.ShipTorpedoLevel: return "debug_ShipTorpedoLevel";
                case Stat.ShipTorpedoRateOfFire: return "debug_ShipTorpedoRateOfFire";
                case Stat.ShipShieldStrength: return "debug_ShipShieldStrength";
                case Stat.ShipShieldRegen: return "debug_ShipShieldRegen";
                case Stat.ShipShieldCooldown: return "debug_ShipShieldCooldown";
                case Stat.ShipType: return "debug_ShipType";
                case Stat.PushbackModifier: return "debug_PushbackModifier";
                case Stat.TargetOnFireDamageModifier: return "debug_TargetOnFireDamageModifier";
                case Stat.SurgeRating: return "debug_SurgeRating";
                case Stat.AlacrityRating: return "Alacrity Rating";
                case Stat.SpellCastReductionPercentage: return "debug_SpellCastReductionPercentage";
                case Stat.SpellChannelReductionPercentage: return "debug_SpellChannelReductionPercentage";
                case Stat.None: return "debug_None";
                case Stat.UnusedDamageSoak: return "debug_UnusedDamageSoak";
                case Stat.UnusedEvasion: return "debug_UnusedEvasion";
                case Stat.UnusedDeflection: return "debug_UnusedDeflection";
                case Stat.UnusedChrenergy: return "debug_UnusedChrenergy";
                case Stat.UnusedMetaModifyThreatModifierPercentage: return "debug_UnusedMetaModifyThreatModifierPercentage";
                case Stat.UnusedMetaModifyThreatModifierFixed: return "debug_UnusedMetaModifyThreatModifierFixed";
                case Stat.UnusedCbtdeflectchance: return "debug_UnusedCbtdeflectchance";
                case Stat.UnusedChraction: return "debug_UnusedChraction";
                case Stat.UnusedCbtthreatreceivedmodifier: return "debug_UnusedCbtthreatreceivedmodifier";
                case Stat.UnusedChrregenerationrate: return "debug_UnusedChrregenerationrate";
                case Stat.UnusedCbtcoverdamagereduction: return "debug_UnusedCbtcoverdamagereduction";
                case Stat.UnusedCbtdamagereceivedmodifier: return "debug_UnusedCbtdamagereceivedmodifier";
                case Stat.UnusedCbtdamagedealtmodifier: return "debug_UnusedCbtdamagedealtmodifier";
                case Stat.UnusedCbtCoverThreatGeneratedModifierPercentage: return "debug_UnusedCbtCoverThreatGeneratedModifierPercentage";
                case Stat.Chrmovebonus: return "debug_Chrmovebonus";
                default:
                    return "";
            }
        }
    }
}