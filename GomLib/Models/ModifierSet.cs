using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GomLib.Models
{
    public enum ModifierSet
    {
        None = 0,
        LightForceChest,
        LightForceChestPrototype,
        LightForceFeet,
        LightForceFeetPrototype,
        LightForceHands,
        LightForceHandsPrototype,
        LightForceHead,
        LightForceHeadPrototype,
        LightForceLegs,
        LightForceLegsPrototype,
        LightForceWaist,
        LightForceWaistPrototype,
        LightForceWrists,
        LightForceWristsPrototype,
        // -- 
        MediumForceChest,
        MediumForceChestPrototype,
        MediumForceFeet,
        MediumForceFeetPrototype,
        MediumForceHands,
        MediumForceHandsPrototype,
        MediumForceHead,
        MediumForceHeadPrototype,
        MediumForceLegs,
        MediumForceLegsPrototype,
        MediumForceWaist,
        MediumForceWaistPrototype,
        MediumForceWrists,
        MediumForceWristsPrototype,
        // -- 
        MediumTechChest,
        MediumTechChestPrototype,
        MediumTechFeet,
        MediumTechFeetPrototype,
        MediumTechHands,
        MediumTechHandsPrototype,
        MediumTechHead,
        MediumTechHeadPrototype,
        MediumTechLegs,
        MediumTechLegsPrototype,
        MediumTechWaist,
        MediumTechWaistPrototype,
        MediumTechWrists,
        MediumTechWristsPrototype,
        // -- 
        HeavyForceChest,
        HeavyForceChestPrototype,
        HeavyForceFeet,
        HeavyForceFeetPrototype,
        HeavyForceHands,
        HeavyForceHandsPrototype,
        HeavyForceHead,
        HeavyForceHeadPrototype,
        HeavyForceLegs,
        HeavyForceLegsPrototype,
        HeavyForceWaist,
        HeavyForceWaistPrototype,
        HeavyForceWrists,
        HeavyForceWristsPrototype,
        // -- 
        HeavyTechChest,
        HeavyTechChestPrototype,
        HeavyTechFeet,
        HeavyTechFeetPrototype,
        HeavyTechHands,
        HeavyTechHandsPrototype,
        HeavyTechHead,
        HeavyTechHeadPrototype,
        HeavyTechLegs,
        HeavyTechLegsPrototype,
        HeavyTechWaist,
        HeavyTechWaistPrototype,
        HeavyTechWrists,
        HeavyTechWristsPrototype,
        // -- 
        ForceOffhand,
        ForceOffhandPrototype,
        ForceShield,
        ForceShieldPrototype,
        TechOffhand,
        TechOffhandPrototype,
        TechShield,
        TechShieldPrototype,
        Earpiece,
        EarpiecePrototype,
        Implant,
        ImplantPrototype,
        // -- 
        WeaponMelee, // Includes Electrostaffs and Vibroblades
        WeaponMeleePrototype,
        WeaponRanged, // Includes Rifles, Sniper Rifles, Blasters, Vibroknives, Shotguns, Assault Cannons
        WeaponRangedPrototype
    }

    public static class ModifierSetExtensions
    {
        public static ModifierSet ToModifierSet(this long modifierSetId)
        {
            if (modifierSetId == 0) { return ModifierSet.None; }

            switch (modifierSetId)
            {
                case 7167727846469441365: return ModifierSet.LightForceChest;
                case -3941045514107100987: return ModifierSet.LightForceChestPrototype;
                case -8886645456878135396: return ModifierSet.LightForceFeet;
                case -1085647114976642164: return ModifierSet.LightForceFeetPrototype;
                case -829376329659025610: return ModifierSet.LightForceHands;
                case 6539204787197322694: return ModifierSet.LightForceHandsPrototype;
                case 7943834177239367346: return ModifierSet.LightForceHead;
                case 3526883399993842818: return ModifierSet.LightForceHeadPrototype;
                case -48169345775018259: return ModifierSet.LightForceLegs;
                case -4587878397262823715: return ModifierSet.LightForceLegsPrototype;
                case -3358397899453505680: return ModifierSet.LightForceWaist;
                case -3415371702292252928: return ModifierSet.LightForceWaistPrototype;
                case 6708250933570573040: return ModifierSet.LightForceWrists;
                case 7254596178051024864: return ModifierSet.LightForceWristsPrototype;
                // --
                case -436720431543628697: return ModifierSet.MediumForceChest;
                case -3531334841593772201: return ModifierSet.MediumForceChestPrototype;
                case -7921059453127550236: return ModifierSet.MediumForceFeet;
                case -2118673664338136044: return ModifierSet.MediumForceFeetPrototype;
                case 8896477055124665808: return ModifierSet.MediumForceHands;
                case 147871168500438496: return ModifierSet.MediumForceHandsPrototype;
                case 8370214552087184550: return ModifierSet.MediumForceHead;
                case -9196626807751253002: return ModifierSet.MediumForceHeadPrototype;
                case -2510008275850943831: return ModifierSet.MediumForceLegs;
                case -2891873826938645095: return ModifierSet.MediumForceLegsPrototype;
                case -4335013788769980802: return ModifierSet.MediumForceWaist;
                case -2852738582385300210: return ModifierSet.MediumForceWaistPrototype;
                case 1558581970351433256: return ModifierSet.MediumForceWrists;
                case -2785590503383332456: return ModifierSet.MediumForceWristsPrototype;
                // -- 
                case 491118800787567026: return ModifierSet.MediumTechChest;
                case -1618727916354142334: return ModifierSet.MediumTechChestPrototype;
                case -7856521929511201911: return ModifierSet.MediumTechFeet;
                case 2296019796571062169: return ModifierSet.MediumTechFeetPrototype;
                case -2873250548138582907: return ModifierSet.MediumTechHands;
                case -4629925704620467531: return ModifierSet.MediumTechHandsPrototype;
                case 7749382675072081847: return ModifierSet.MediumTechHead;
                case 5689604749909120807: return ModifierSet.MediumTechHeadPrototype;
                case 5861350917907035032: return ModifierSet.MediumTechLegs;
                case -2432499585999224984: return ModifierSet.MediumTechLegsPrototype;
                case 2320856014545702607: return ModifierSet.MediumTechWaist;
                case -7498690028117934337: return ModifierSet.MediumTechWaistPrototype;
                case -3083014199090596707: return ModifierSet.MediumTechWrists;
                case 5766049105810175725: return ModifierSet.MediumTechWristsPrototype;
                // -- 
                case 8663018763905129649: return ModifierSet.HeavyForceChest;
                case 1612256628583920865: return ModifierSet.HeavyForceChestPrototype;
                case 8036227341628682390: return ModifierSet.HeavyForceFeet;
                case 2136604784867156870: return ModifierSet.HeavyForceFeetPrototype;
                case -9124169362340046366: return ModifierSet.HeavyForceHands;
                case -2400776178661775374: return ModifierSet.HeavyForceHandsPrototype;
                case -3005146770961837076: return ModifierSet.HeavyForceHead;
                case 6350670243392222972: return ModifierSet.HeavyForceHeadPrototype;
                case -3903565921223481545: return ModifierSet.HeavyForceLegs;
                case -3766812271195596377: return ModifierSet.HeavyForceLegsPrototype;
                case -722741346142235824: return ModifierSet.HeavyForceWaist;
                case 8107481074415434848: return ModifierSet.HeavyForceWaistPrototype;
                case -4561779744484440514: return ModifierSet.HeavyForceWrists;
                case 431844414127180846: return ModifierSet.HeavyForceWristsPrototype;
                // -- 
                case -998323921017097280: return ModifierSet.HeavyTechChest;
                case 8690348198985611536: return ModifierSet.HeavyTechChestPrototype;
                case -3968979360418491385: return ModifierSet.HeavyTechFeet;
                case -5049952211205213737: return ModifierSet.HeavyTechFeetPrototype;
                case -9022343849637589213: return ModifierSet.HeavyTechHands;
                case 6680744845747971923: return ModifierSet.HeavyTechHandsPrototype;
                case -3568687116565662191: return ModifierSet.HeavyTechHead;
                case -4649624782980281791: return ModifierSet.HeavyTechHeadPrototype;
                case 6767444716771334306: return ModifierSet.HeavyTechLegs;
                case 5686471865984611954: return ModifierSet.HeavyTechLegsPrototype;
                case 1581998544801342981: return ModifierSet.HeavyTechWaist;
                case 6570072492150043957: return ModifierSet.HeavyTechWaistPrototype;
                case 6538378173747513359: return ModifierSet.HeavyTechWrists;
                case -2990373207657130273: return ModifierSet.HeavyTechWristsPrototype;
                // -- 
                case -5123179784309879429: return ModifierSet.ForceOffhand;
                case 5229736585565630635: return ModifierSet.ForceOffhandPrototype;
                case 8306065443988410062: return ModifierSet.ForceShield;
                case 3718073456813450878: return ModifierSet.ForceShieldPrototype;
                case 4617239556880822963: return ModifierSet.TechOffhand;
                case 9141800037284063331: return ModifierSet.TechOffhandPrototype;
                case -4975657451580833421: return ModifierSet.TechShield;
                case -2799780123728815933: return ModifierSet.TechShieldPrototype;
                case -4339588451135395772: return ModifierSet.Earpiece;
                case 3235340998032607412: return ModifierSet.EarpiecePrototype;
                case 747324337974227241: return ModifierSet.Implant;
                case 5780305157222657977: return ModifierSet.ImplantPrototype;
                // -- 
                case 1027449124835981724: return ModifierSet.WeaponMelee;
                case 7483531293894126412: return ModifierSet.WeaponMeleePrototype;
                case 5791706038191817667: return ModifierSet.WeaponRanged;
                case 8022180020026237875: return ModifierSet.WeaponRangedPrototype;
                default: throw new InvalidOperationException(String.Format("Unknown ModifierSet: {0}", modifierSetId));
            }
        }
    }
}
