using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Newtonsoft.Json;

namespace GomLib.Models
{
    public enum Stat
    {
        Undefined = 0,
        Strength = 1,
        Aim = 2,
        Cunning = 3,
        Endurance = 4,
        Presence = 5,
        Willpower = 6,
        MaxHealth = 7,
        MaxEnergy = 8,
        MaxForce = 9,
        MaxWeaponDamage = 10,
        MinWeaponDamage = 11,
        FlurryBlowDelay = 12,
        FlurryLength = 13,
        RangeMax = 14,
        RangeMin = 15,
        DamageReductionElemental = 16,
        DamageReductionInternal = 17,
        DamageReductionKinetic = 18,
        DamageReductionEnergy = 19,
        RangedDamageBonus = 20,
        MeleeDamageBonus = 21,
        FlurryDelay = 22,
        PersuasionScore = 23,
        PersuasionCrit = 24,
        MovementSpeed = 25,
        MaxAP = 26,
        RunSpeed = 27,
        WalkSpeed = 28,
        ArmorRating = 29,
        ArenaRating = 30,
        RunBackwardsSpeed = 31,
        TurnSpeed = 32,
        EffectInitialCharges = 33,
        EffectMaxCharges = 34,
        CastingTime = 35,
        ChannelingTime = 36,
        CooldownTime = 37,
        Duration = 38,
        GlobalCooldown = 39,
        EffectTickTime = 40,
        ForceCost = 41,
        EnergyCost = 42,
        APCost = 43,
        AbilityMaxRange = 44,
        AbilityMinRange = 45,
        EffectAuraRadius = 46,
        AbsorbDamagePerHitFixed = 47,
        AbsorbDamagePerHitPercentage = 48,
        AbsorbDamageMaxFixed = 49,
        AbsorbDamageMaxPercentage = 50,
        DamageDoneModifierFixed = 51,
        DamageDoneModifierPercentage = 52,
        ThreatGeneratedModifierFixed = 53,
        ThreatGeneratedModifierPercentage = 54,
        HealingDoneModifierFixed = 55,
        HealingDoneModifierPercentage = 56,
        EffectBallisticImpulseMomentumFixed = 57,
        EffectBallisticImpulseMomentumPercentage = 58,
        EffectModifyStatFixed = 59,
        EffectModifyStatPercentage = 60,
        EffectRestoreApFixed = 61,
        EffectRestoreApPercentage = 62,
        EffectRestoreForceFixed = 63,
        EffectRestoreForcePercentage = 64,
        EffectRestoreEnergyFixed = 65,
        EffectRestoreEnergyPercentage = 66,
        EffectSpendApFixed = 67,
        EffectSpendApPercentage = 68,
        EffectSpendForceFixed = 69,
        EffectSpendForcePercentage = 70,
        EffectSpendEnergyFixed = 71,
        EffectSpendEnergyPercentage = 72,
        EffectSpendHealthFixed = 73,
        EffectSpendHealthPercentage = 74,
        EffectAoeRadiusFixed = 75,
        EffectAoeRadiusPercentage = 76,
        EffectAoeConeDistanceFixed = 77,
        EffectAoeConeDistancePercentage = 78,
        EffectAoeConeAngleFixed = 79,
        EffectAoeConeAnglePercentage = 80,
        EffectAoeCylinderDistanceFixed = 81,
        EffectAoeCylinderDistancePercentage = 82,
        EffectAoeCylinderRadiusFixed = 83,
        EffectAoeCylinderRadiusPercentage = 84,
        MeleeAccuracy = 85,
        MeleeDefense = 86,
        MeleeCriticalChance = 87,
        MeleeCriticalDamage = 88,
        MeleeShieldChance = 89,
        MeleeShieldAbsorb = 90,
        RangedAccuracy = 91,
        RangedDefense = 92,
        RangedCriticalChance = 93,
        RangedCriticalDamage = 94,
        RangedShieldChance = 95,
        RangedShieldAbsorb = 96,
        DualWieldAccuracyModifierFixed = 97,
        CoverDefenseBonus = 98,
        DualWieldDamagePenalty = 99,
        StealthLevel = 100,
        StealthDetection = 101,
        WeaponAccuracy = 102,
        GlanceRating = 103,
        ForceDamageBonus = 104,
        TechDamageBonus = 105,
        ForceCriticalChance = 106,
        ForceCriticalDamageBonus = 107,
        TechCriticalChance = 108,
        TechCriticalDamageBonus = 109,
        ForceRegenOoc = 110,
        EnergyRegen = 111,
        HealthRegenOoc = 112,
        PvpDamageBonus = 113,
        PvpDamageReduction = 114,
        PvpCriticalChance = 115,
        PvpCriticalChanceReduction = 116,
        PvpTraumaIgnore = 117,
        ExpertiseRating = 118,
        PvpCriticalDamageReduction = 119,
        AbsorptionRating = 120,
        AttackPowerRating = 121,
        ForcePowerRating = 122,
        TechPowerRating = 123,
        ForceRegenRating = 124,
        EnergyRegenRating = 125,
        AccuracyRating = 126,
        CriticalChanceRating = 127,
        ForceRegen = 128,
        PvpTrauma = 129,
        HealingReceivedModifierPercentage = 130,
        HealingReceivedModifierFixed = 131,
        DamageReceievedModifierPercentage = 132,
        DamageReceivedModifierFixed = 133,
        DefenseRating = 134,
        EffectGenerateHeatModifierFixed = 135,
        EffectGenerateHeatModifierPercentage = 136,
        ArmorPenetration = 137,
        Artifice = 138,
        Armormech = 139,
        Armstech = 140,
        Biochem = 141,
        Cybertech = 142,
        Synthweaving = 143,
        Archaeology = 144,
        Bioanalysis = 145,
        Scavenging = 146,
        Slicing = 147,
        Diplomacy = 148,
        Research = 149,
        TreasureHunting = 150,
        UnderworldTrading = 151,
        Crafting = 152,
        Harvesting = 153,
        Mission = 154,
        ArtificeEfficiency = 155,
        ArmormechEfficiency = 156,
        ArmstechEfficiency = 157,
        BiochemEfficiency = 158,
        CybertechEfficiency = 159,
        SynthweavingEfficiency = 160,
        ArchaeologyEfficiency = 161,
        BioanalysisEfficiency = 162,
        ScavengingEfficiency = 163,
        SlicingEfficiency = 164,
        DiplomacyEfficiency = 165,
        ResearchEfficiency = 166,
        TreasureHuntingEfficiency = 167,
        UnderworldTradingEfficiency = 168,
        CraftingEfficiency = 169,
        HarvestingEfficiency = 170,
        MissionEfficiency = 171,
        ArtificeCritical = 172,
        ArmormechCritical = 173,
        ArmstechCritical = 174,
        BiochemCritical = 175,
        CybertechCritical = 176,
        SynthweavingCritical = 177,
        ArchaeologyCritical = 178,
        BioanalysisCritical = 179,
        ScavengingCritical = 180,
        SlicingCritical = 181,
        DiplomacyCritical = 182,
        ResearchCritical = 183,
        TreasureHuntingCritical = 184,
        UnderworldTradingCritical = 185,
        CraftingCritical = 186,
        HarvestingCritical = 187,
        MissionCritical = 188,
        TechHealingPower = 189,
        ForceHealingPower = 190,
        TargetLowDamageModifier = 191,
        TargetBleedingDamageModifier = 192,
        TargetStunnedDamageModifier = 193,
        CasterStunnedDamageTakenModifier = 194,
        ProcRateModifier = 195,
        RateLimitModifierFixed = 196,
        RateLimitModifierPercentage = 197,
        Resolve = 198,
        ForceAccuracy = 199,
        ForceDefense = 200,
        TechAccuracy = 201,
        TechDefense = 202,
        ShipArmor = 203,
        ShipRateOfFire = 204,
        ShipBlasterDamage = 205,
        ShipMissileCount = 206,
        ShipMissileLevel = 207,
        ShipMissileRateOfFire = 208,
        ShipTorpedoCount = 209,
        ShipTorpedoLevel = 210,
        ShipTorpedoRateOfFire = 211,
        ShipShieldStrength = 212,
        ShipShieldRegen = 213,
        ShipShieldCooldown = 214,
        ShipType = 215,
        PushbackModifier = 216,
        TargetOnFireDamageModifier = 217,
        SurgeRating = 218,
        AlacrityRating = 219,
        SpellCastReductionPercentage = 220,
        SpellChannelReductionPercentage = 221,
        None = 222,
        UnusedDamageSoak = 223,
        UnusedEvasion = 224,
        UnusedDeflection = 225,
        UnusedChrenergy = 226,
        UnusedMetaModifyThreatModifierPercentage = 227,
        UnusedMetaModifyThreatModifierFixed = 228,
        UnusedCbtdeflectchance = 229,
        UnusedChraction = 230,
        UnusedCbtthreatreceivedmodifier = 231,
        UnusedChrregenerationrate = 232,
        UnusedCbtcoverdamagereduction = 233,
        UnusedCbtdamagereceivedmodifier = 234,
        UnusedCbtdamagedealtmodifier = 235,
        UnusedCbtCoverThreatGeneratedModifierPercentage = 236,
        Chrmovebonus = 237,
        Mastery = 238,
    }

    public class StatData
    {
        public Dictionary<string, DetailedStat> StatLookup { get; set; }

        [Newtonsoft.Json.JsonIgnore]
        readonly DataObjectModel _dom;

        public StatData(DataObjectModel dom)
        {
            _dom = dom;
        }

        public DetailedStat ToStat(string str)
        {
            if (String.IsNullOrEmpty(str)) { return new DetailedStat("Undefined"); }

            if (StatLookup == null)
            {
                StatLookup = new Dictionary<string, DetailedStat>();
                var modStatData = _dom.GetObject("modStatData");
                Dictionary<object, object> modStatDescriptions = modStatData.Data.Get<Dictionary<object, object>>("modStatDescriptions");
                modStatData.Unload();
                StringTable table = _dom.stringTable.Find("str.gui.stats");
                foreach (var stat in modStatDescriptions)
                {
                    DetailedStat detStat = new DetailedStat();
                    string lookupName = stat.Key.ToString();
                    detStat.Minimum = ((GomLib.GomObjectData)stat.Value).ValueOrDefault<float>("modStatMinimum", 0f);
                    detStat.DisplayName = ((GomLib.GomObjectData)stat.Value).ValueOrDefault<string>("modStatDisplayName", stat.Key.ToString());
                    detStat.Maximum = ((GomLib.GomObjectData)stat.Value).ValueOrDefault<float>("modStatMaximum", 0f);
                    detStat.IsMeta = ((GomLib.GomObjectData)stat.Value).ValueOrDefault<bool>("modStatIsMetaStat", false);
                    detStat.NumFormat = ((GomLib.GomObjectData)stat.Value).ValueOrDefault<ScriptEnum>("modStatDisplayNumberFormat", new ScriptEnum()).ToString();
                    detStat.Id = ((GomLib.GomObjectData)stat.Value).ValueOrDefault<long>("modStatDisplayNameStringID", 0);
                    detStat.StringId = detStat.Id + 972058473267200;
                    detStat.LocalizedDisplayName = table.GetLocalizedText(detStat.StringId, "str.gui.stats");
                    StatLookup.Add(lookupName, detStat);
                }
            }

            if (!StatLookup.ContainsKey(str)) { return null; }
            else { return StatLookup[str]; }
        }

        public DetailedStat ToStat(ScriptEnum val)
        {
            if (val == null) { return ToStat(String.Empty); }
            return ToStat(val.ToString());
        }
    }

    public class DetailedStat
    {
        public DetailedStat() { }
        public DetailedStat(string name)
        {
            DisplayName = name;
        }

        public float Minimum { get; set; }
        public float Maximum { get; set; }
        public string DisplayName { get; set; }
        public bool IsMeta { get; set; }
        public string NumFormat { get; set; }
        [JsonConverter(typeof(LongConverter))]
        public long StringId { get; set; }
        [JsonConverter(typeof(LongConverter))]
        public long Id { get; set; }
        public Dictionary<string, string> LocalizedDisplayName { get; set; }

        public override string ToString()
        {
            if (LocalizedDisplayName != null)
                return LocalizedDisplayName["enMale"];
            else
                return DisplayName;
        }
    }

    public static class StatExtensions
    {
        public static Stat ToStat(string str)
        {
            if (String.IsNullOrEmpty(str)) { return Stat.Undefined; }

            str = str.ToLower();
            str = str.Replace('.', '_');

            switch (str)
            {
                case "stat_att_strength": return Stat.Strength;
                case "stat_att_agility": return Stat.Aim;
                case "stat_att_cunning": return Stat.Cunning;
                case "stat_att_endurance": return Stat.Endurance;
                case "stat_att_presence": return Stat.Presence;
                case "stat_att_willpower": return Stat.Willpower;
                case "stat_cbt_health_max": return Stat.MaxHealth;
                case "stat_cbt_tech_energy_max": return Stat.MaxEnergy;
                case "stat_cbt_force_force_max": return Stat.MaxForce;
                case "stat_cbtdamagemax": return Stat.MaxWeaponDamage;
                case "stat_cbtdamagemin": return Stat.MinWeaponDamage;
                case "stat_cbtflurryblowdelay": return Stat.FlurryBlowDelay;
                case "stat_cbtflurrylength": return Stat.FlurryLength;
                case "stat_cbtrangemax": return Stat.RangeMax;
                case "stat_cbtrangemin": return Stat.RangeMin;
                case "stat_cbt_damage_reduction_elemental": return Stat.DamageReductionElemental;
                case "stat_cbt_damage_reduction_internal": return Stat.DamageReductionInternal;
                case "stat_cbt_damage_reduction_kinetic": return Stat.DamageReductionKinetic;
                case "stat_cbt_damage_reduction_energy": return Stat.DamageReductionEnergy;
                case "stat_cbt_ranged_damage_bonus": return Stat.RangedDamageBonus;
                case "stat_cbt_melee_damage_bonus": return Stat.MeleeDamageBonus;
                case "stat_cbtflurrydelay": return Stat.FlurryDelay;
                case "stat_cnv_persuasion_score": return Stat.PersuasionScore;
                case "stat_cnv_persuasion_crit": return Stat.PersuasionCrit;
                case "stat_aimovespeedmodifier": return Stat.MovementSpeed;
                case "stat_cbt_action_points_max": return Stat.MaxAP;
                case "stat_airunspeed": return Stat.RunSpeed;
                case "stat_aiwalkspeed": return Stat.WalkSpeed;
                case "stat_rtg_armor": return Stat.ArmorRating;
                case "stat_cbtarenarating": return Stat.ArenaRating;
                case "stat_airunbackspeed": return Stat.RunBackwardsSpeed;
                case "stat_aiturnspeed": return Stat.TurnSpeed;
                case "stat_eff_charges_initial": return Stat.EffectInitialCharges;
                case "stat_eff_charges_max": return Stat.EffectMaxCharges;
                case "stat_abl_time_casting": return Stat.CastingTime;
                case "stat_abl_time_channeling": return Stat.ChannelingTime;
                case "stat_abl_time_cooldown": return Stat.CooldownTime;
                case "stat_eff_time_duration": return Stat.Duration;
                case "stat_abl_time_global_cooldown": return Stat.GlobalCooldown;
                case "stat_eff_time_tick": return Stat.EffectTickTime;
                case "stat_abl_cost_force": return Stat.ForceCost;
                case "stat_abl_cost_energy": return Stat.EnergyCost;
                case "stat_abl_cost_action_points": return Stat.APCost;
                case "stat_abl_range_max": return Stat.AbilityMaxRange;
                case "stat_abl_range_min": return Stat.AbilityMinRange;
                case "stat_eff_aura_radius": return Stat.EffectAuraRadius;
                case "stat_eff_action_absorb_damage_per_hit_modifier_fixed": return Stat.AbsorbDamagePerHitFixed;
                case "stat_eff_action_absorb_damage_per_hit_modifier_percentage": return Stat.AbsorbDamagePerHitPercentage;
                case "stat_eff_action_absorb_damage_max_modifier_fixed": return Stat.AbsorbDamageMaxFixed;
                case "stat_eff_action_absorb_damage_max_modifier_percentage": return Stat.AbsorbDamageMaxPercentage;
                case "stat_cbt_damage_done_modifier_fixed": return Stat.DamageDoneModifierFixed;
                case "stat_cbt_damage_done_modifier_percentage": return Stat.DamageDoneModifierPercentage;
                case "stat_cbt_threat_generated_modifier_fixed": return Stat.ThreatGeneratedModifierFixed;
                case "stat_cbt_threat_generated_modifier_percentage": return Stat.ThreatGeneratedModifierPercentage;
                case "stat_cbt_healing_done_modifier_fixed": return Stat.HealingDoneModifierFixed;
                case "stat_cbt_healing_done_modifier_percentage": return Stat.HealingDoneModifierPercentage;
                case "stat_eff_action_ballistic_impulse_momentum_modifier_fixed": return Stat.EffectBallisticImpulseMomentumFixed;
                case "stat_eff_action_ballistic_impulse_momentum_modifier_percentage": return Stat.EffectBallisticImpulseMomentumPercentage;
                case "stat_eff_action_modify_stat_modifier_fixed": return Stat.EffectModifyStatFixed;
                case "stat_eff_action_modify_stat_modifier_percentage": return Stat.EffectModifyStatPercentage;
                case "stat_eff_action_restore_action_points_modifier_fixed": return Stat.EffectRestoreApFixed;
                case "stat_eff_action_restore_action_points_modifier_percentage": return Stat.EffectRestoreApPercentage;
                case "stat_eff_action_restore_force_modifier_fixed": return Stat.EffectRestoreForceFixed;
                case "stat_eff_action_restore_force_modifier_percentage": return Stat.EffectRestoreForcePercentage;
                case "stat_eff_action_restore_energy_modifier_fixed": return Stat.EffectRestoreEnergyFixed;
                case "stat_eff_action_restore_energy_modifier_percentage": return Stat.EffectRestoreEnergyPercentage;
                case "stat_eff_action_spend_action_points_modifier_fixed": return Stat.EffectSpendApFixed;
                case "stat_eff_action_spend_action_points_modifier_percentage": return Stat.EffectSpendApPercentage;
                case "stat_eff_action_spend_force_modifier_fixed": return Stat.EffectSpendForceFixed;
                case "stat_eff_action_spend_force_modifier_percentage": return Stat.EffectSpendForcePercentage;
                case "stat_eff_action_spend_energy_modifier_fixed": return Stat.EffectSpendEnergyFixed;
                case "stat_eff_action_spend_energy_modifier_percentage": return Stat.EffectSpendEnergyPercentage;
                case "stat_eff_action_spend_health_modifier_fixed": return Stat.EffectSpendHealthFixed;
                case "stat_eff_action_spend_health_modifier_percentage": return Stat.EffectSpendHealthPercentage;
                case "stat_eff_target_aoe_sphere_radius_modifier_fixed": return Stat.EffectAoeRadiusFixed;
                case "stat_eff_target_aoe_sphere_radius_modifier_percentage": return Stat.EffectAoeRadiusPercentage;
                case "stat_eff_target_aoe_cone_distance_modifier_fixed": return Stat.EffectAoeConeDistanceFixed;
                case "stat_eff_target_aoe_cone_distance_modifier_percentage": return Stat.EffectAoeConeDistancePercentage;
                case "stat_eff_target_aoe_cone_angle_modifier_fixed": return Stat.EffectAoeConeAngleFixed;
                case "stat_eff_target_aoe_cone_angle_modifier_percentage": return Stat.EffectAoeConeAnglePercentage;
                case "stat_eff_target_aoe_cylinder_distance_modifier_fixed": return Stat.EffectAoeCylinderDistanceFixed;
                case "stat_eff_target_aoe_cylinder_distance_modifier_percentage": return Stat.EffectAoeCylinderDistancePercentage;
                case "stat_eff_target_aoe_cylinder_radius_modifier_fixed": return Stat.EffectAoeCylinderRadiusFixed;
                case "stat_eff_target_aoe_cylinder_radius_modifier_percentage": return Stat.EffectAoeCylinderRadiusPercentage;
                case "stat_cbt_melee_accuracy": return Stat.MeleeAccuracy;
                case "stat_cbt_melee_defense": return Stat.MeleeDefense;
                case "stat_cbt_melee_critical_chance": return Stat.MeleeCriticalChance;
                case "stat_cbt_melee_critical_damage": return Stat.MeleeCriticalDamage;
                case "stat_cbt_melee_glance_chance": return Stat.MeleeShieldChance;
                case "stat_cbt_melee_glance_absorb": return Stat.MeleeShieldAbsorb;
                case "stat_cbt_ranged_accuracy": return Stat.RangedAccuracy;
                case "stat_cbt_ranged_defense": return Stat.RangedDefense;
                case "stat_cbt_ranged_critical_chance": return Stat.RangedCriticalChance;
                case "stat_cbt_ranged_critical_damage": return Stat.RangedCriticalDamage;
                case "stat_cbt_ranged_glance_chance": return Stat.RangedShieldChance;
                case "stat_cbt_ranged_glance_absorb": return Stat.RangedShieldAbsorb;
                case "stat_cbt_dual_wield_accuracy_modifier_fixed": return Stat.DualWieldAccuracyModifierFixed;
                case "stat_cbt_cover_defense_modifier_fixed": return Stat.CoverDefenseBonus;
                case "stat_cbt_dual_wield_damage_done_modifier_percentage": return Stat.DualWieldDamagePenalty;
                case "stat_cbtstealtheffectivelevel": return Stat.StealthLevel;
                case "stat_cbtstealthdetecteffectivelevel": return Stat.StealthDetection;
                case "stat_cbtaccuracy": return Stat.WeaponAccuracy;
                case "stat_rtg_glance_chance": return Stat.GlanceRating;
                case "stat_cbt_force_damage_bonus": return Stat.ForceDamageBonus;
                case "stat_cbt_tech_damage_bonus": return Stat.TechDamageBonus;
                case "stat_cbt_force_critical_chance": return Stat.ForceCriticalChance;
                case "stat_cbt_force_critical_damage": return Stat.ForceCriticalDamageBonus;
                case "stat_cbt_tech_critical_chance": return Stat.TechCriticalChance;
                case "stat_cbt_tech_critical_damage": return Stat.TechCriticalDamageBonus;
                case "stat_cbt_force_force_regen_rate_ooc": return Stat.ForceRegenOoc;
                case "stat_cbt_tech_energy_regen_rate": return Stat.EnergyRegen;
                case "stat_cbt_health_regen_rate_ooc": return Stat.HealthRegenOoc;
                case "stat_cbt_pvp_damage_bonus": return Stat.PvpDamageBonus;
                case "stat_cbt_pvp_damage_reduction": return Stat.PvpDamageReduction;
                case "stat_cbt_pvp_critical_chance": return Stat.PvpCriticalChance;
                case "stat_cbt_pvp_critical_chance_reduction": return Stat.PvpCriticalChanceReduction;
                case "stat_cbt_pvp_trauma_ignore": return Stat.PvpTraumaIgnore;
                case "stat_rtg_pvp_expertise": return Stat.ExpertiseRating;
                case "stat_cbt_pvp_critical_damage_reduction": return Stat.PvpCriticalDamageReduction;
                case "stat_rtg_glance_absorb": return Stat.AbsorptionRating;
                case "stat_rtg_attack_power": return Stat.AttackPowerRating;
                case "stat_rtg_force_power": return Stat.ForcePowerRating;
                case "stat_rtg_tech_power": return Stat.TechPowerRating;
                case "stat_rtg_force_regen": return Stat.ForceRegenRating;
                case "stat_rtg_energy_regen": return Stat.EnergyRegenRating;
                case "stat_rtg_accuracy": return Stat.AccuracyRating;
                case "stat_rtg_critical_chance": return Stat.CriticalChanceRating;
                case "stat_cbt_force_force_regen_rate": return Stat.ForceRegen;
                case "stat_cbt_pvp_trauma": return Stat.PvpTrauma;
                case "stat_cbt_healing_received_modifier_percentage": return Stat.HealingReceivedModifierPercentage;
                case "stat_cbt_healing_received_modifier_fixed": return Stat.HealingReceivedModifierFixed;
                case "stat_cbt_damage_received_modifier_percentage": return Stat.DamageReceievedModifierPercentage;
                case "stat_cbt_damage_received_modifier_fixed": return Stat.DamageReceivedModifierFixed;
                case "stat_rtg_defense": return Stat.DefenseRating;
                case "stat_eff_action_generate_heat_modifier_fixed": return Stat.EffectGenerateHeatModifierFixed;
                case "stat_eff_action_generate_heat_modifier_percentage": return Stat.EffectGenerateHeatModifierPercentage;
                case "stat_cbt_armor_penetration": return Stat.ArmorPenetration;
                case "stat_prf_skill_artifice": return Stat.Artifice;
                case "stat_prf_skill_armormech": return Stat.Armormech;
                case "stat_prf_skill_armstech": return Stat.Armstech;
                case "stat_prf_skill_biochem": return Stat.Biochem;
                case "stat_prf_skill_cybertech": return Stat.Cybertech;
                case "stat_prf_skill_synthweaving": return Stat.Synthweaving;
                case "stat_prf_skill_archaeology": return Stat.Archaeology;
                case "stat_prf_skill_bioanalysis": return Stat.Bioanalysis;
                case "stat_prf_skill_scavenging": return Stat.Scavenging;
                case "stat_prf_skill_slicing": return Stat.Slicing;
                case "stat_prf_skill_diplomacy": return Stat.Diplomacy;
                case "stat_prf_skill_research": return Stat.Research;
                case "stat_prf_skill_treasure_hunting": return Stat.TreasureHunting;
                case "stat_prf_skill_underworld_trading": return Stat.UnderworldTrading;
                case "stat_prf_skill_crafting": return Stat.Crafting;
                case "stat_prf_skill_harvesting": return Stat.Harvesting;
                case "stat_prf_skill_mission": return Stat.Mission;
                case "stat_prf_efficiency_artifice": return Stat.ArtificeEfficiency;
                case "stat_prf_efficiency_armormech": return Stat.ArmormechEfficiency;
                case "stat_prf_efficiency_armstech": return Stat.ArmstechEfficiency;
                case "stat_prf_efficiency_biochem": return Stat.BiochemEfficiency;
                case "stat_prf_efficiency_cybertech": return Stat.CybertechEfficiency;
                case "stat_prf_efficiency_synthweaving": return Stat.SynthweavingEfficiency;
                case "stat_prf_efficiency_archaeology": return Stat.ArchaeologyEfficiency;
                case "stat_prf_efficiency_bioanalysis": return Stat.BioanalysisEfficiency;
                case "stat_prf_efficiency_scavenging": return Stat.ScavengingEfficiency;
                case "stat_prf_efficiency_slicing": return Stat.SlicingEfficiency;
                case "stat_prf_efficiency_diplomacy": return Stat.DiplomacyEfficiency;
                case "stat_prf_efficiency_research": return Stat.ResearchEfficiency;
                case "stat_prf_efficiency_treasure_hunting": return Stat.TreasureHuntingEfficiency;
                case "stat_prf_efficiency_underworld_trading": return Stat.UnderworldTradingEfficiency;
                case "stat_prf_efficiency_crafting": return Stat.CraftingEfficiency;
                case "stat_prf_efficiency_harvesting": return Stat.HarvestingEfficiency;
                case "stat_prf_efficiency_mission": return Stat.MissionEfficiency;
                case "stat_prf_critical_artifice": return Stat.ArtificeCritical;
                case "stat_prf_critical_armormech": return Stat.ArmormechCritical;
                case "stat_prf_critical_armstech": return Stat.ArmstechCritical;
                case "stat_prf_critical_biochem": return Stat.BiochemCritical;
                case "stat_prf_critical_cybertech": return Stat.CybertechCritical;
                case "stat_prf_critical_synthweaving": return Stat.SynthweavingCritical;
                case "stat_prf_critical_archaeology": return Stat.ArchaeologyCritical;
                case "stat_prf_critical_bioanalysis": return Stat.BioanalysisCritical;
                case "stat_prf_critical_scavenging": return Stat.ScavengingCritical;
                case "stat_prf_critical_slicing": return Stat.SlicingCritical;
                case "stat_prf_critical_diplomacy": return Stat.DiplomacyCritical;
                case "stat_prf_critical_research": return Stat.ResearchCritical;
                case "stat_prf_critical_treasure_hunting": return Stat.TreasureHuntingCritical;
                case "stat_prf_critical_underworld_trading": return Stat.UnderworldTradingCritical;
                case "stat_prf_critical_crafting": return Stat.CraftingCritical;
                case "stat_prf_critical_harvesting": return Stat.HarvestingCritical;
                case "stat_prf_critical_mission": return Stat.MissionCritical;
                case "stat_cbt_healing_power_tech": return Stat.TechHealingPower;
                case "stat_cbt_healing_power_force": return Stat.ForceHealingPower;
                case "stat_cbt_damage_done_if_target_health_low_modifier_percentage": return Stat.TargetLowDamageModifier;
                case "stat_cbt_damage_done_if_target_bleeding_modifier_percentage": return Stat.TargetBleedingDamageModifier;
                case "stat_cbt_damage_done_if_target_stunned_modifier_percentage": return Stat.TargetStunnedDamageModifier;
                case "stat_cbt_damage_received_if_target_stunned_modifier_percentage": return Stat.CasterStunnedDamageTakenModifier;
                case "stat_eff_trigger_proc_chance_percent_modifier_fixed": return Stat.ProcRateModifier;
                case "stat_eff_trigger_rate_limit_modifier_fixed": return Stat.RateLimitModifierFixed;
                case "stat_eff_trigger_rate_limit_modifier_percentage": return Stat.RateLimitModifierPercentage;
                case "stat_cbt_pvp_stamina_max": return Stat.Resolve;
                case "stat_cbt_force_accuracy": return Stat.ForceAccuracy;
                case "stat_cbt_force_defense": return Stat.ForceDefense;
                case "stat_cbt_tech_accuracy": return Stat.TechAccuracy;
                case "stat_cbt_tech_defense": return Stat.TechDefense;
                case "stat_spc_armor": return Stat.ShipArmor;
                case "stat_spc_weapon_rof": return Stat.ShipRateOfFire;
                case "stat_spc_weapon_damage": return Stat.ShipBlasterDamage;
                case "stat_spc_weapon_missile_count": return Stat.ShipMissileCount;
                case "stat_spc_weapon_missile_level": return Stat.ShipMissileLevel;
                case "stat_spc_weapon_missile_rof": return Stat.ShipMissileRateOfFire;
                case "stat_spc_weapon_torpedo_count": return Stat.ShipTorpedoCount;
                case "stat_spc_weapon_torpedo_level": return Stat.ShipTorpedoLevel;
                case "stat_spc_weapon_torpedo_rof": return Stat.ShipTorpedoRateOfFire;
                case "stat_spc_shields_strength": return Stat.ShipShieldStrength;
                case "stat_spc_shields_regen": return Stat.ShipShieldRegen;
                case "stat_spc_shields_cooldown": return Stat.ShipShieldCooldown;
                case "stat_spc_ship_type": return Stat.ShipType;
                case "stat_abl_time_pushback_modifier_percentage": return Stat.PushbackModifier;
                case "stat_cbt_damage_done_if_target_on_fire_modifier_percentage": return Stat.TargetOnFireDamageModifier;
                case "stat_rtg_critical_damage": return Stat.SurgeRating;
                case "stat_rtg_spell_haste": return Stat.AlacrityRating;
                case "stat_cbt_spell_casting_reduction_percentage": return Stat.SpellCastReductionPercentage;
                case "stat_cbt_spell_channeling_reduction_percentage": return Stat.SpellChannelReductionPercentage;
                case "stat_none": return Stat.None;
                case "_unused_stat_cbtdamagesoak": return Stat.UnusedDamageSoak;
                case "_unused_stat_cbtevasion": return Stat.UnusedEvasion;
                case "_unused_stat_cbtdeflection": return Stat.UnusedDeflection;
                case "_unused_stat_chrenergy": return Stat.UnusedChrenergy;
                case "_unused_stat_meta_modify_threat_modifier_percentage": return Stat.UnusedMetaModifyThreatModifierPercentage;
                case "_unused_stat_meta_modify_threat_modifier_fixed": return Stat.UnusedMetaModifyThreatModifierFixed;
                case "_unused_stat_cbtdeflectchance": return Stat.UnusedCbtdeflectchance;
                case "_unused_stat_chraction": return Stat.UnusedChraction;
                case "_unused_stat_cbtthreatreceivedmodifier": return Stat.UnusedCbtthreatreceivedmodifier;
                case "_unused_stat_chrregenerationrate": return Stat.UnusedChrregenerationrate;
                case "_unused_stat_cbtcoverdamagereduction": return Stat.UnusedCbtcoverdamagereduction;
                case "_unused_stat_cbtdamagereceivedmodifier": return Stat.UnusedCbtdamagereceivedmodifier;
                case "_unused_stat_cbtdamagedealtmodifier": return Stat.UnusedCbtdamagedealtmodifier;
                case "_unused_stat_cbt_cover_threat_generated_modifier_percentage": return Stat.UnusedCbtCoverThreatGeneratedModifierPercentage;
                case "stat_chrmovebonus": return Stat.Chrmovebonus;
                case "stat_att_mastery": return Stat.Mastery;
                default: throw new InvalidOperationException("Invalid Stat: " + str);
            }
        }

        public static Stat ToStat(ScriptEnum val)
        {
            if (val == null) { return ToStat(String.Empty); }
            return ToStat(val.ToString());
        }
    }
}
