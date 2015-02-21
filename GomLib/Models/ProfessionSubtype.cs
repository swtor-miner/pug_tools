using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GomLib.Models
{
    public enum ProfessionSubtype
    {
        None,
        ArmormechMedium,
        ArmormechHeavy,
        ArmstechBlasterPistol,
        ArmstechBlasterRifle,
        ArmstechSniperRifle,
        ArmstechAssaultCannon,
        ArtificeResonanceCrystal,
        ArtificeFocusCrystal,
        ArtificeGadget,
        BiochemStimulant,
        BiochemAdrenal,
        BiochemMedKit,
        BiochemImplant,
        BiochemRepairKit,
        CybertechDroid,
        CybertechElectroWeapon,
        CybertechEar,
        CybertechGadget,
        SynthweavingLight,
        SynthweavingMedium,
        SynthweavingHeavy,
        ArmstechBlasterModBarrel,
        ArmstechBlasterModScope,
        ArmstechBlasterModTrigger,
        ArmstechBlasterModColorCartridge,
        ArtificeEmitterMatrix,
        ArtificeFocusLens,
        ArtificeArmorModOverlay,
        ArtificeArmorModCircuitry,
        ArtificeArmorModUnderlay,
        ArtificeArmorModHarness,
        CybertechArmorModOverlay,
        CybertechArmorModCircuitry,
        CybertechArmorModUnderlay,
        CybertechArmorModHarness,
        ArmormechMisc,
        ArmstechMisc,
        ArtificeMisc,
        BiochemMisc,
        CybertechMisc,
        SynthweavingMisc,
        ArtificeHilt,
        ArtificeLight,
        ArtificeMod,
        ArtificeMedium,
        ArtificeEnhancement,
        ArtificeArmoring,
        CybertechMod,
        CybertechMedium,
        CybertechEnhancement,
        CybertechArmoring,
        ArtificeHeavy,
        CybertechHeavy,
        CybertechGrenade,
        CybertechGenerator,
        CybertechSpace,
        Strongholds,
        ArmormechAdaptive,
        SynthweavingAdaptive
    }

    public static class ProfessionSubtypeExtensions
    {
        public static ProfessionSubtype ToProfessionSubtype(this ScriptEnum val)
        {
            if (val == null) { return ToProfessionSubtype(String.Empty); }
            return ToProfessionSubtype(val.ToString());
        }

        public static ProfessionSubtype ToProfessionSubtype(this string str)
        {
            if (String.IsNullOrEmpty(str)) return ProfessionSubtype.None;

            switch (str)
            {
                case "prfProfessionSubtypeNone": return ProfessionSubtype.None;
                case "prfProfessionArmormechMedium": return ProfessionSubtype.ArmormechMedium;
                case "prfProfessionArmormechHeavy": return ProfessionSubtype.ArmormechHeavy;
                case "prfProfessionArmstechBlasterPistol": return ProfessionSubtype.ArmstechBlasterPistol;
                case "prfProfessionArmstechBlasterRifle": return ProfessionSubtype.ArmstechBlasterRifle;
                case "prfProfessionArmstechSniperRifle": return ProfessionSubtype.ArmstechSniperRifle;
                case "prfProfessionArmstechAssaultCannon": return ProfessionSubtype.ArmstechAssaultCannon;
                case "prfProfessionArtificeResonanceCrystal": return ProfessionSubtype.ArtificeResonanceCrystal;
                case "prfProfessionArtificeFocusCrystal": return ProfessionSubtype.ArtificeFocusCrystal;
                case "prfProfessionArtificeGadget": return ProfessionSubtype.ArtificeGadget;
                case "prfProfessionBiochemStimulant": return ProfessionSubtype.BiochemStimulant;
                case "prfProfessionBiochemAdrenal": return ProfessionSubtype.BiochemAdrenal;
                case "prfProfessionBiochemMedKit": return ProfessionSubtype.BiochemMedKit;
                case "prfProfessionBiochemImplant": return ProfessionSubtype.BiochemImplant;
                case "prfProfessionBiochemRepairKit": return ProfessionSubtype.BiochemRepairKit;
                case "prfProfessionCybertechDroid": return ProfessionSubtype.CybertechDroid;
                case "prfProfessionCybertechElectroWeapon": return ProfessionSubtype.CybertechElectroWeapon;
                case "prfProfessionCybertechEar": return ProfessionSubtype.CybertechEar;
                case "prfProfessionCybertechGadget": return ProfessionSubtype.CybertechGadget;
                case "prfProfessionSynthweavingLight": return ProfessionSubtype.SynthweavingLight;
                case "prfProfessionSynthweavingMedium": return ProfessionSubtype.SynthweavingMedium;
                case "prfProfessionSynthweavingHeavy": return ProfessionSubtype.SynthweavingHeavy;
                case "prfProfessionArmstechBlasterModBarrel": return ProfessionSubtype.ArmstechBlasterModBarrel;
                case "prfProfessionArmstechBlasterModScope": return ProfessionSubtype.ArmstechBlasterModScope;
                case "prfProfessionArmstechBlasterModTrigger": return ProfessionSubtype.ArmstechBlasterModTrigger;
                case "prfProfessionArmstechBlasterModColorCartridge": return ProfessionSubtype.ArmstechBlasterModColorCartridge;
                case "prfProfessionArtificeEmitterMatrix": return ProfessionSubtype.ArtificeEmitterMatrix;
                case "prfProfessionArtificeFocusLens": return ProfessionSubtype.ArtificeFocusLens;
                case "prfProfessionArtificeArmorModOverlay": return ProfessionSubtype.ArtificeArmorModOverlay;
                case "prfProfessionArtificeArmorModCircuitry": return ProfessionSubtype.ArtificeArmorModCircuitry;
                case "prfProfessionArtificeArmorModUnderlay": return ProfessionSubtype.ArtificeArmorModUnderlay;
                case "prfProfessionArtificeArmorModHarness": return ProfessionSubtype.ArtificeArmorModHarness;
                case "prfProfessionCybertechArmorModOverlay": return ProfessionSubtype.CybertechArmorModOverlay;
                case "prfProfessionCybertechArmorModCircuitry": return ProfessionSubtype.CybertechArmorModCircuitry;
                case "prfProfessionCybertechArmorModUnderlay": return ProfessionSubtype.CybertechArmorModUnderlay;
                case "prfProfessionCybertechArmorModHarness": return ProfessionSubtype.CybertechArmorModHarness;
                case "prfProfessionArmormechMisc": return ProfessionSubtype.ArmormechMisc;
                case "prfProfessionArmstechMisc": return ProfessionSubtype.ArmstechMisc;
                case "prfProfessionArtificeMisc": return ProfessionSubtype.ArtificeMisc;
                case "prfProfessionBiochemMisc": return ProfessionSubtype.BiochemMisc;
                case "prfProfessionCybertechMisc": return ProfessionSubtype.CybertechMisc;
                case "prfProfessionSynthweavingMisc": return ProfessionSubtype.SynthweavingMisc;
                case "prfProfessionArtificeHilt": return ProfessionSubtype.ArtificeHilt;
                case "prfProfessionArtificeLight": return ProfessionSubtype.ArtificeLight;
                case "prfProfessionArtificeMod": return ProfessionSubtype.ArtificeMod;
                case "prfProfessionArtificeMedium": return ProfessionSubtype.ArtificeMedium;
                case "prfProfessionArtificeEnhancement": return ProfessionSubtype.ArtificeEnhancement;
                case "prfProfessionArtificeArmoring": return ProfessionSubtype.ArtificeArmoring;
                case "prfProfessionCybertechMod": return ProfessionSubtype.CybertechMod;
                case "prfProfessionCybertechMedium": return ProfessionSubtype.CybertechMedium;
                case "prfProfessionCybertechEnhancement": return ProfessionSubtype.CybertechEnhancement;
                case "prfProfessionCybertechArmoring": return ProfessionSubtype.CybertechArmoring;
                case "prfProfessionArtificeHeavy": return ProfessionSubtype.ArtificeHeavy;
                case "prfProfessionCybertechHeavy": return ProfessionSubtype.CybertechHeavy;
                case "prfProfessionCybertechGrenade": return ProfessionSubtype.CybertechGrenade;
                case "prfProfessionCybertechGenerator": return ProfessionSubtype.CybertechGenerator;
                case "prfProfessionCybertechSpace": return ProfessionSubtype.CybertechSpace;
                case "prfProfessionStrongholds": return ProfessionSubtype.Strongholds;
                case "prfProfessionArmormechAdaptive": return ProfessionSubtype.ArmormechAdaptive;
                case "prfProfessionSynthweavingAdaptive": return ProfessionSubtype.SynthweavingAdaptive;
                
                default: throw new InvalidOperationException("Unknown ProfessionSubtype: " + str);
            }
        }
    }
}
