using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GomLib.Models
{
    public enum EnhancementType
    {
        None = 0,
        Hilt = 1,
        PowerCell = 2,
        Harness = 3,
        Overlay = 4,
        Underlay = 5,
        Support = 6,
        FocusLens = 7,
        Trigger = 8,
        Barrel = 9,
        PowerCrystal = 10,
        Scope = 11,
        Circuitry = 12,
        EmitterMatrix = 13,
        ColorCrystal = 14,
        ColorCartridge = 15,
        Modulator = 16,
        Dye = 17
    }

    public static class EnhancementTypeExtensions
    {
        public static bool IsBaseMod(this EnhancementType modType)
        {
            switch (modType)
            {
                case EnhancementType.Barrel:
                case EnhancementType.PowerCrystal:
                case EnhancementType.Harness:
                case EnhancementType.Underlay:
                    return true;
                default: return false;
            }
        }

        // Deprecated - Unused
        //public static EnhancementType ToEnhancementType(this string str)
        //{
        //    if (String.IsNullOrEmpty(str)) return EnhancementType.None;

        //    switch (str)
        //    {
        //        case "itmEnhancementTypeNone": return EnhancementType.None;
        //        case "itmEnhancementTypeHilt": return EnhancementType.Hilt;
        //        case "itmEnhancementTypePowerCell": return EnhancementType.PowerCell;
        //        case "itmEnhancementTypeHarness": return EnhancementType.Harness;
        //        case "itmEnhancementTypeOverlay": return EnhancementType.Overlay;
        //        case "itmEnhancementTypeUnderlay": return EnhancementType.Underlay;
        //        case "itmEnhancementTypeSupport": return EnhancementType.Support;
        //        case "itmEnhancementTypeFocusLens": return EnhancementType.FocusLens;
        //        case "itmEnhancementTypeTrigger": return EnhancementType.Trigger;
        //        case "itmEnhancementTypeBarrel": return EnhancementType.Barrel;
        //        case "itmEnhancementTypePowerCrystal": return EnhancementType.PowerCrystal;
        //        case "itmEnhancementTypeScope": return EnhancementType.Scope;
        //        case "itmEnhancementTypeCircuitry": return EnhancementType.Circuitry;
        //        case "itmEnhancementTypeEmitterMatrix": return EnhancementType.EmitterMatrix;
        //        case "itmEnhancementTypeColorCrystal": return EnhancementType.ColorCrystal;
        //        case "itmEnhancementTypeColorCartridge": return EnhancementType.ColorCartridge;
        //        case "itmEnhancementTypeModulator": return EnhancementType.Modulator;
        //        default: throw new InvalidOperationException("Unknown EnhancementType: " + str);
        //    }
        //}

        public static EnhancementType ToEnhancementType(this long val)
        {
            switch (val)
            {
                case 0: return EnhancementType.None;
                case 5896642495383363362: return EnhancementType.Support; // Enhancement
                case 5314498280388707818: return EnhancementType.PowerCrystal; // Hilt
                case 2809304092246332557: return EnhancementType.Overlay; // Mod
                case 1774827397081270696: return EnhancementType.Modulator; // Augment
                case -146341075757061383: return EnhancementType.Harness; //Armoring
                case -8643996337002403560: return EnhancementType.ColorCrystal;
                case -3078326126619797673: return EnhancementType.ColorCartridge;
                case 1578998821196777783: return EnhancementType.Barrel;
                case 7738793343379667311: return EnhancementType.Circuitry;
                case 3038083514834894874: return EnhancementType.Dye;
                default: throw new InvalidOperationException("Unknown EnhancementType: " + val);
            }
        }
    }

    public class EnhancementData
    {
        public Dictionary<long, DetailedEnhancementType> SlotLookup { get; set; }

        [Newtonsoft.Json.JsonIgnore]
        DataObjectModel _dom;

        public EnhancementData(DataObjectModel dom)
        {
            _dom = dom;
        }

        public DetailedEnhancementType ToEnhancement(long id)
        {
            if (id == 0) { return null; } // new DetailedEnhancementType("Undefined"); }

            if (SlotLookup == null)
            {
                SlotLookup = new Dictionary<long, DetailedEnhancementType>();
                var itmEnhancementInfo = _dom.GetObject("itmEnhancementInfoPrototype");
                Dictionary<object, object> itmEnhancementTypes = itmEnhancementInfo.Data.Get<Dictionary<object, object>>("itmEnhancementTypes");
                itmEnhancementInfo.Unload();
                StringTable table = _dom.stringTable.Find("str.gui.itm.enhancement.types");
                foreach (var enh in itmEnhancementTypes)
                {
                    DetailedEnhancementType detEnhance = new DetailedEnhancementType();
                    detEnhance.IsArmorMod = ((GomLib.GomObjectData)enh.Value).ValueOrDefault<bool>("itmEnhancementTypeIsArmor", false);
                    detEnhance.Id = ((GomLib.GomObjectData)enh.Value).ValueOrDefault<long>("itmEnhancementTypeDisplayStringID", 0);
                    detEnhance.StringId =  detEnhance.Id + 1173453784743936;
                    detEnhance.DisplayName = table.GetText(detEnhance.StringId, "str.gui.stats");
                    detEnhance.LocalizedDisplayName = table.GetLocalizedText(detEnhance.StringId, "str.gui.stats");
                    detEnhance.Mount = ((GomLib.GomObjectData)enh.Value).ValueOrDefault<string>("itmEnhancementTypeMount", "");
                    detEnhance.Icon = ((GomLib.GomObjectData)enh.Value).ValueOrDefault<string>("itmEnhancementTypeIconAsset", "");
                    detEnhance.ModType = ((GomLib.GomObjectData)enh.Value).ValueOrDefault<ScriptEnum>("itmEnhancementModificationType", new ScriptEnum());
                    detEnhance.IsDestroyed = ((GomLib.GomObjectData)enh.Value).ValueOrDefault<bool>("itmEnhancementDestroyOnRemove", false);
                    SlotLookup.Add((long)enh.Key, detEnhance);
                }
                itmEnhancementTypes = null;
            }

            if (!SlotLookup.ContainsKey(id)) { return null; }
            else { return SlotLookup[id]; }
        }
    }
    public class DetailedEnhancementType
    {
        public DetailedEnhancementType() { }
        public DetailedEnhancementType(string name)
        {
            DisplayName = name;
        }

        public string DisplayName { get; set; }
        public string Icon { get; set; }
        public string Mount { get; set; } //"The mounting string that the GUI uses to link and item to a mounting point on an image"
        public ScriptEnum ModType { get; set; }
        public bool IsArmorMod { get; set; }
        public bool IsDestroyed { get; set; }
        public long StringId { get; set; }
        public long Id { get; set; }
        public Dictionary<string, string> LocalizedDisplayName { get; set; }

        public override string ToString()
        {
            return DisplayName;
        }
    }
}
