using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GomLib.Models
{
    public enum Profession
    {
        None = 0,
        Archaeology = 1,
        Bioanalysis = 2,
        Scavenging = 3,
        Artifice = 4,
        Armormech = 5,
        Armstech = 6,
        Biochem = 7,
        Cybertech = 8,
        Synthweaving = 9,
        Slicing = 10,
        Diplomacy = 11,
        Investigation = 12,
        TreasureHunting = 13,
        UnderworldTrading = 14
    }

    public static class ProfessionExtensions
    {
        public static Profession ToProfession(this ScriptEnum val)
        {
            if (val == null) { return ToProfession(String.Empty); }
            return ToProfession(val.ToString());
        }

        public static Profession ToProfession(this string str)
        {
            if (string.IsNullOrEmpty(str)) return Profession.None;
            switch (str)
            {
                case "prfProfessionNone": return Profession.None;
                case "prfProfessionArchaeology": return Profession.Archaeology;
                case "prfProfessionBioanalysis": return Profession.Bioanalysis;
                case "prfProfessionScavenging": return Profession.Scavenging;
                case "prfProfessionArtifice": return Profession.Artifice;
                case "prfProfessionArmormech": return Profession.Armormech;
                case "prfProfessionArmstech": return Profession.Armstech;
                case "prfProfessionBiochem": return Profession.Biochem;
                case "prfProfessionCybertech": return Profession.Cybertech;
                case "prfProfessionSynthweaving": return Profession.Synthweaving;
                case "prfProfessionSlicing": return Profession.Slicing;
                case "prfProfessionDiplomacy": return Profession.Diplomacy;
                case "prfProfessionResearch": return Profession.Investigation;
                case "prfProfessionTreasureHunting": return Profession.TreasureHunting;
                case "prfProfessionUnderworldTrading": return Profession.UnderworldTrading;
                default: throw new InvalidOperationException("Invalid Profession: " + str);
            }
        }
    }
}
