using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GomLib.Models
{
    /*public enum ArmorSpec
    {
        Undefined = 0,
        Light = 1,  // light
        Medium = 2, // medium
        Heavy = 3, // heavy
        Generator = 4, // generator
        ShieldForce = 5, // shield_force
        Focus = 6, // focus
        Shield = 7, // shield
        ArmorDroid = 8,
        LightFemale = 9,
        LightMale = 10,
        Adaptive = 11
    }

    public static class ArmorSpecExtensions
    {
        public static ArmorSpec ToArmorSpec(this long val)
        {
            // THESE VALUES ARE DEFINED IN chrArmorTablePrototype
            switch (val)
            {
                case 0: return ArmorSpec.Undefined;
                case 5763611092890301551: return ArmorSpec.Light;
                case 589686270506543030: return ArmorSpec.Medium;
                case -8622546409652942944: return ArmorSpec.Heavy;
                case 3927728356064183411: return ArmorSpec.Focus;
                case -8489117329102589124: return ArmorSpec.ShieldForce;
                case 6944519570668593218: return ArmorSpec.Generator;
                case -3325415850905959312: return ArmorSpec.Shield;
                case 1138086775958122321: return ArmorSpec.ArmorDroid;
                case 8799400953706039056: return ArmorSpec.LightFemale;
                case -7804289248234247763: return ArmorSpec.LightMale;
                case -335583520315957829: return ArmorSpec.Adaptive;
                default: throw new InvalidOperationException("Unknown ArmorSpec: " + val);
            }
        }

        public static bool HasStats(this ArmorSpec spec)
        {
            switch (spec)
            {
                case ArmorSpec.Generator:
                case ArmorSpec.Focus:
                case ArmorSpec.Shield:
                case ArmorSpec.ShieldForce:
                    return true;
                default:
                    return false;
            }
        }
    }*/

    public class ArmorSpec : PseudoGameObject, IEquatable<ArmorSpec>
    {
        ArmorSpec(DataObjectModel dom, long id)
        {
            Dom_ = dom;
            Id = id;
            Prototype = "cbtArmorTablePrototype";
            ProtoDataTable = "cbtArmorDetailsBySpec";
        }

        //public string Name { get; set; } //str.gui.tooltips 836131348283392
        public long NameId { get; set; }
        public Dictionary<string, string> LocalizedName { get; set; }
        public ulong ReqAbilityId { get; set; }
        //[Newtonsoft.Json.JsonIgnore]
        //public Ability ReqAbility { get; set; }
        public string DebugSpecName { get; set; }

        public static Dictionary<long, ArmorSpec> ArmorSpecList;
        public static void Flush()
        {
            ArmorSpecList = null;
        }

        public override string ToString()
        {
            return Name;
        }

        public override int GetHashCode()
        {
            int hash = Id.GetHashCode();
            hash ^= NameId.GetHashCode();
            if (LocalizedName != null) foreach (var x in LocalizedName) { hash ^= x.GetHashCode(); }
            hash ^= ReqAbilityId.GetHashCode();
            hash ^= DebugSpecName.GetHashCode();
            return hash;
        }

        public override bool Equals(object obj)
        {
            if (obj == null) return false;
            if (ReferenceEquals(this, obj)) return true;

            if (!(obj is ArmorSpec obj2)) return false;

            return Equals(obj2);
        }

        public bool Equals(ArmorSpec obj)
        {
            if (obj == null) return false;
            if (ReferenceEquals(this, obj)) return true;

            if (GetHashCode() != obj.GetHashCode())
                return false;
            return true;
        }
        public static void Load(DataObjectModel dom)
        {
            if (ArmorSpecList == null)
            {
                Dictionary<object, object> cbtWeaponData = dom.GetObject("cbtArmorTablePrototype").Data.Get<Dictionary<object, object>>("cbtArmorDetailsBySpec");
                ArmorSpecList = new Dictionary<long, ArmorSpec>();
                foreach (var kvp in cbtWeaponData)
                {
                    ArmorSpec itm = new ArmorSpec(dom, (long)kvp.Key);
                    Load(itm, (GomObjectData)kvp.Value);
                    ArmorSpecList.Add((long)kvp.Key, itm);
                }
            }
        }
        public static ArmorSpec Load(DataObjectModel dom, long id)
        {
            if (dom == null || id == 0) return null;
            Load(dom);
            ArmorSpecList.TryGetValue(id, out ArmorSpec ret);

            return ret;
        }
        internal static ArmorSpec Load(ArmorSpec itm, GomObjectData gom)
        {
            itm.NameId = gom.ValueOrDefault<long>("cbtArmorNameID", 0) + 836131348283392;
            itm.LocalizedName = itm.Dom_.stringTable.TryGetLocalizedStrings("str.gui.tooltips", itm.NameId);
            itm.Name = itm.Dom_.stringTable.TryGetString("str.gui.tooltips", itm.NameId);

            itm.ReqAbilityId = gom.ValueOrDefault<ulong>("cbtArmorRequiredAbility", 0);
            itm.DebugSpecName = gom.ValueOrDefault("cbtArmorDebugSpecName", "");
            return itm;
        }
    }
}
