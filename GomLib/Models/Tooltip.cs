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
        public static string language = "enMale";
        public static Dictionary<long, Dictionary<string, string>> TooltipNameMap = new Dictionary<long, Dictionary<string, string>>();
        public Tooltip() { }
        public Tooltip(ulong id, DataObjectModel dom) {
            _dom = dom;
            Id = id;
            Fqn = _dom.GetStoredTypeName(id);
        }
        public Tooltip(string fqn, DataObjectModel dom)
        {
            _dom = dom;
            Fqn = fqn;
            Id = _dom.GetStoredTypeId(Fqn);
        }
        public Tooltip(GameObject gObj)
        {
            obj = gObj;
            Id = obj.Id;
            Fqn = obj.Fqn;
        }
        public Tooltip(PseudoGameObject gObj)
        {
            pObj = gObj;
            Id = obj.Id;
            Fqn = obj.Fqn;
        }
        [Newtonsoft.Json.JsonIgnore]
        internal GameObject _obj { get; set; }
        public GameObject obj
        {
            get
            {
                if (_obj == null)
                {
                    _obj = new GameObject().Load(Id, _dom);
                }
                return _obj;
            }
            set
            {
                _obj = value;
            }
        }
        public PseudoGameObject pObj { get; set; }

        [Newtonsoft.Json.JsonIgnore]
        public string _type { get; set; }
        public string type
        {
            get
            {
                if (_type == null)
                    _type = obj.GetType().ToString();
                return _type;
            }
            set
            {
                _type = value;
            }
        }
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
                        new SQLProperty("Base62Id", "Base62Id", "varchar(7) COLLATE utf8_unicode_ci NOT NULL"),
                        new SQLProperty("Fqn", "Fqn", "varchar(255) COLLATE utf8_unicode_ci NOT NULL"),
                        new SQLProperty("Tooltip", "HTML", "varchar(10000) COLLATE utf8_unicode_ci NOT NULL")
                    };
            }
        }

        private string GetHTML()
        {
            if (obj != null)
            {
                if (obj.GetType() == typeof(Item))
                {
                    //string retu = ((Item)obj).GetHTML().ToString(SaveOptions.DisableFormatting);

                    return ((Item)obj).GetHTML().ToString(SaveOptions.DisableFormatting);
                    //return ((Item)obj).GetHTML().ToStringWithoutCharacterChecking();
                }
                if (obj.GetType() == typeof(Schematic))
                {
                    return ((Schematic)obj).GetHTML().ToString(SaveOptions.DisableFormatting);
                }
                if (obj.GetType() == typeof(Ability))
                {
                    return ((Ability)obj).GetHTML().ToString(SaveOptions.DisableFormatting);
                }
                if (obj.GetType() == typeof(Quest))
                {
                    return ((Quest)obj).GetHTML().ToString(SaveOptions.DisableFormatting);
                }
                if (obj.GetType() == typeof(Talent))
                {
                    return ((Talent)obj).GetHTML().ToString(SaveOptions.DisableFormatting);
                }
                if (obj.GetType() == typeof(Achievement))
                {
                    return ((Achievement)obj).GetHTML().ToString(SaveOptions.DisableFormatting);
                }
                return "<div>Not implemented</div>";
            }
            if (pObj == null)
            {
                //if (obj.GetType() == typeof(Discipline))
                //{
                //    return ((Discipline)obj).GetHTML().ToString(SaveOptions.DisableFormatting);
                //}
                return "<div>Not implemented</div>";
            }
            return null;
        }
    }
    public static class TooltipHelpers
    {
        #region item
        public static XElement GetHTML(this Item itm) //Behold linq!
        {
            if (Tooltip.TooltipNameMap.Count == 0)
            {
                LoadNameMap(itm._dom);
            }
            if (itm.Id == 0) return new XElement("div", "Not Found");

            XElement tooltip = new XElement("div", new XAttribute("class", "torctip_wrapper"));
            var fileId = TorLib.FileId.FromFilePath(String.Format("/resources/gfx/icons/{0}.dds", itm.Icon));

            if (itm != null)
            {
                string stringQual = ((itm.TypeBitFlags.IsModdable && (itm.Quality == ItemQuality.Prototype)) ? "moddable" : itm.Quality.ToString().ToLower());
                tooltip.Add(new XElement("div",
                    new XAttribute("class", String.Format("torctip_image torctip_image_{0}", stringQual)),
                    new XElement("img",
                        new XAttribute("src", String.Format("https://torcommunity.com/db/icons/{0}_{1}.jpg", fileId.ph, fileId.sh)),
                        new XAttribute("alt", ""))),
                    new XElement("div",
                        new XAttribute("class", "torctip_name"),
                        new XElement("a",
                            new XAttribute("href", String.Format("https://torcommunity.com/database/item/{0}", itm.Base62Id)),
                            new XAttribute("data-torc", "norestyle"),
                            itm.LocalizedName[Tooltip.language]
                            )),
                    itm.ItemInnerHTML()
                );

            }

            return tooltip;
        }
        public static XElement ItemInnerHTML(this Item itm)
        {
            string stringQual = itm.Quality.ToString().ToLower();
            string localizedRating = GetLocalizedText(836131348283473).Replace("{0}", "{1}");
            XElement tooltip = new XElement("div",
                XClass("torctip_tooltip"),
                new XElement("span",
                    XClass(String.Format("torctip_{0}", stringQual)),
                    itm.LocalizedName[Tooltip.language])
                );
            //MTX
            if (itm.TypeBitFlags.IsMtxItem)
            {
                tooltip.Add(new XElement("div",
                    XClass("torctip_val"),
                    GetLocalizedText(836131348283729) // Cartel Market Item
                    ));
            }
            //Reputation item
            if (itm.TypeBitFlags.IsRepTrophy)
            {
                string repName = "";
                if (itm.LocalizedRepFactionDictionary["Imperial"][Tooltip.language] != itm.LocalizedRepFactionDictionary["Republic"][Tooltip.language])
                {
                    repName = String.Format("{0} / {1}", itm.LocalizedRepFactionDictionary["Imperial"][Tooltip.language], itm.LocalizedRepFactionDictionary["Republic"][Tooltip.language]);
                }
                else
                    repName = itm.LocalizedRepFactionDictionary["Imperial"][Tooltip.language];
                tooltip.Add(
                    new XElement("div",
                        XClass("torctip_rep"),
                        GetLocalizedText(836131348283741) // "Reputation Trophy"
                    ),
                    new XElement("div",
                        XClass("torctip_rep"),
                        repName
                    )
                );
            }
            //gift
            if (itm.TypeBitFlags.IsGift)
            {
                if (itm.GiftType != GiftType.None)
                {
                    tooltip.Add(
                        new XElement("div",
                            GetLocalizedText(836131348283395 + (int)itm.GiftType)),
                        new XElement("div",
                            String.Format("{0} {1}", GetLocalizedText(836131348283411), itm.GiftRankNum)) //Rank
                        );

                }
            }
            //binding
            if (itm.Binding != 0)
            {
                string binding = itm.Binding.ToString(); //String.Format("Binds on {0}", itm.Binding.ToString());
                switch (itm.Binding.ToString())
                {
                    case "Equip":
                        binding = GetLocalizedText(946314439294988);
                        break;
                    case "Pickup":
                        binding = GetLocalizedText(946314439294989);
                        break;
                    case "Legacy":
                        binding = GetLocalizedText(946314439294994);
                        break;
                    case "Use":
                        binding = GetLocalizedText(946314439295248);
                        break;
                    default:
                        break;
                }
                tooltip.Add(new XElement("div",
                    XClass("torctip_main"),
                    binding
                    ));
                if (itm.BindsToSlot)
                {
                    tooltip.Add(new XElement("div",
                        XClass("torctip_main"),
                        "Binds to Slot"
                        ));
                }
            }
            //unique
            if (itm.TypeBitFlags.HasUniqueLimit)
            {
                tooltip.Add(new XElement("div",
                    XClass("torctip_main"),
                    GetLocalizedText(836131348283436) //"Unique"
                    ));
            }
            if (itm.TypeBitFlags.IsMod && itm.DyeId != 0)
            {
                string name = "";
                if(itm.DyeColor.LocalizedColorName != null){
                    name = itm.DyeColor.LocalizedColorName[Tooltip.language] ;}
                string blks = String.Format("{0} {{0}} ({1})", name, GetLocalizedText(836131348283461));
                switch (itm.EnhancementType)
                {
                    case EnhancementType.Dye:
                        blks = String.Format(blks, GetLocalizedText(1173453784744196));
                        break;
                    case EnhancementType.ColorCrystal:
                        blks = String.Format(blks, GetLocalizedText(1173453784743941));
                        break;
                }
                tooltip.Add(new XElement("div",
                    blks),
                    new XElement("div",
                        new XElement("span",
                            XClass("torctip_val"),
                            GetLocalizedText(836131348284082)),
                        GetDyeBlock(itm.DyeColor.Palette1Rep)),
                    new XElement("div",
                        new XElement("span",
                            XClass("torctip_val"),
                            GetLocalizedText(836131348284083)),
                        GetDyeBlock(itm.DyeColor.Palette2Rep))
                );
            }
            //slot
            bool isEquipable = false;
            if (itm.Slots.Count > 1) //the Any slot was removed from the item by the itemloader
            {
                tooltip.Add(new XElement("div",
                    XClass("torctip_main"),
                    String.Join(", ", itm.Slots.Select(x => x.ConvertToString(Tooltip.language)).Where(x => x != null).ToList())
                    ));
                isEquipable = true;
            }
            //armor, rating etc if gear
            float techpower = 0;
            float forcepower = 0;
            float absorbchance = 0;
            float shieldchance = 0;
            var level = itm.ItemLevel;
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
                        {
                            level = mainMod.Modification.ItemLevel;
                            qual = mainMod.Modification.Quality;
                        }
                        //else
                        //nothing premium is what we want
                    }
                }
                else
                {
                    level = itm.ItemLevel;
                    qual = itm.Quality;
                }
                float min = 0f;
                float max = 0f;
                try
                {
                    min = itm._dom.data.weaponPerLevel.GetStat(itm.WeaponSpec.Id, level, qual, Stat.MinWeaponDamage);
                    max = itm._dom.data.weaponPerLevel.GetStat(itm.WeaponSpec.Id, level, qual, Stat.MaxWeaponDamage);  //change this so items without barrels use level 1 premium numbers
                    techpower = itm._dom.data.weaponPerLevel.GetStat(itm.WeaponSpec.Id, level, qual, Stat.TechPowerRating);
                    forcepower = itm._dom.data.weaponPerLevel.GetStat(itm.WeaponSpec.Id, level, qual, Stat.ForcePowerRating);
                }
                catch (Exception e)
                {
                    string dosomething = ""; //suppress for now, break here to debug
                }
                string replaceString = String.Format(" {{0}} {0} {1}", GetLocalizedText(836131348283440), localizedRating);
                string dType = itm.WeaponSpec.DamageType;
                switch (dType)
                {
                    case "Kinetic":
                        dType = GetLocalizedText(946314439294990);
                        break;
                    case "Energy":
                        dType = GetLocalizedText(946314439294991);
                        break;
                    case "Elemental":
                        dType = GetLocalizedText(946314439294992);
                        break;
                    case "Internal":
                        dType = GetLocalizedText(946314439294993);
                        break;
                    default:
                        break;
                }
                tooltip.Add(new XElement("div",
                    XClass("torctip_main"),
                    new XElement("span",
                        XClass("torctip_minDam"),
                        min.ToString("0.0")),
                    "-",
                    new XElement("span",
                        XClass("torctip_maxDam"),
                        max.ToString("0.0")),
                    String.Format(replaceString, dType, itm.CombinedRating) // " {0} Damage (Rating {1})"
                    ));
            }
            else if (isEquipable)
            {
                ArmorSpec shield = itm.ShieldSpec;
                if (shield != null)
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
                            {
                                level = mainMod.Modification.ItemLevel;
                                qual = mainMod.Modification.Quality;
                            }
                            //else
                            //nothing premium is what we want
                        }
                    }
                    else
                    {
                        level = itm.ItemLevel;
                        qual = itm.Quality;
                    }
                    try
                    {
                        techpower = itm._dom.data.shieldPerLevel.GetShield(itm.ArmorSpec, qual, level, Stat.TechPowerRating);
                        forcepower = itm._dom.data.shieldPerLevel.GetShield(itm.ArmorSpec, qual, level, Stat.ForcePowerRating);
                        absorbchance = itm._dom.data.shieldPerLevel.GetShield(itm.ArmorSpec, qual, level, Stat.MeleeShieldAbsorb);
                        shieldchance = itm._dom.data.shieldPerLevel.GetShield(itm.ArmorSpec, qual, level, Stat.MeleeShieldChance);
                    }
                    catch (Exception e)
                    {
                        string dosomething = ""; //suppress for now, break here to debug
                    }
                    if (absorbchance > 0)
                        tooltip.Add(new XElement("div",
                            XClass("torctip_main"),
                            String.Format(GetLocalizedText(836131348283463), (absorbchance * 100).ToString("n1")) //"Shield Absorb: {0}%"
                            ));
                    if (shieldchance > 0)
                        tooltip.Add(new XElement("div",
                            XClass("torctip_main"),
                            String.Format(GetLocalizedText(836131348283464), (shieldchance * 100).ToString("n1")) //"Shield Chance: {0}%"
                            ));
                    string ratingReplace = String.Format("{{0}} {0}", localizedRating);
                    tooltip.Add(new XElement("div",
                        XClass("torctip_main"),
                        String.Format(ratingReplace, shield.LocalizedName[Tooltip.language], itm.CombinedRating) //"{0} (Rating {1})"
                        ));
                }
                ArmorSpec arm = itm.ArmorSpec;
                if (arm != null)
                {
                    if (arm.DebugSpecName == "adaptive")
                    {
                        string adapRepString = String.Format("{0} {1}", GetLocalizedText(836131348283702), localizedRating.Replace("1","0"));
                        tooltip.Add(new XElement("div",
                            XClass("torctip_main"),
                            String.Format(adapRepString, itm.CombinedRating) //"Adaptive Armor (Rating {0})"
                            ));
                    }
                    else
                    {
                        var temp = itm.EnhancementSlots.Where(x => x.Slot == EnhancementType.Harness);
                        if (temp.Count() != 0)
                            if (temp.First().ModificationId != 0)
                                level = temp.First().Modification.ItemLevel;
                        try
                        {
                            var qual = itm.Quality;
                            if (qual == ItemQuality.Moddable) qual = ItemQuality.Prototype;
                            int armor = itm._dom.data.armorPerLevel.GetArmor(arm, level, qual, itm.Slots.Where(x => x != SlotType.Any).First());

                            if (armor > 0)
                            {
                                string armRepString = String.Format("{{0}} {0} {1}", GetLocalizedText(836131348283506), localizedRating);
                                tooltip.Add(new XElement("div",
                                    XClass("torctip_main"),
                                    String.Format(armRepString, armor, itm.CombinedRating) //"{0} Armor (Rating {1})"
                                    ));
                            }
                        }
                        catch (Exception ex)
                        {
                            string sdfkljn = "";
                        }
                    }
                }
                else if (itm.CombinedRating != 0)
                    tooltip.Add(new XElement("div",
                        XClass("torctip_main"),
                        String.Format("{0} {1}", GetLocalizedText(836131348284091), itm.CombinedRating) // "Item Rating {0}"
                        ));
            }
            /*else if (itm.CombinedRating != 0)
                tooltip.Add(new XElement("div",
                    XClass("torctip_main"),
                    String.Format("Item Rating {0}", itm.CombinedRating)
                    ));*/

            if (itm.Durability > 0)
            {
                tooltip.Add(new XElement("div",
                    XClass("torctip_main"),
                    String.Format("{0}: {1}/{1}", GetLocalizedText(836131348283439), itm.MaxDurability) //"Durability: {0}/{0}"
                    ));
            }
            //stats
            //tooltip.Append("<br />");
            if (itm.CombinedStatModifiers.Count != 0 || itm.WeaponSpec != null)
            {
                XElement stats = new XElement("div",
                    XClass("torctip_stats"),
                    new XElement("span",
                        XClass("torctip_white"),
                        GetLocalizedText(836131348283465)) //"Total Stats:"
                    );
                if (!isEquipable)
                {
                    if (itm.EnhancementType != null) {
                        switch (itm.EnhancementType)
                        {
                            case EnhancementType.Harness:
                                stats.Add(new XElement("div",
                                   XClass("torctip_stat"),
                                   String.Format("{0} {1}", GetLocalizedText(836131348283474), itm.CombinedRating) //"Armor Rating {0}"
                                ));
                                break;
                            case EnhancementType.Barrel:
                            case EnhancementType.Hilt:
                            case EnhancementType.PowerCrystal:
                                stats.Add(new XElement("div",
                                   XClass("torctip_stat"),
                                   String.Format("{0} {1}", GetLocalizedText(836131348283475), itm.CombinedRating) //"Weapon Damage/Power Rating {0}"
                                ));
                                break;
                            default:
                                stats.Add(new XElement("div",
                                   XClass("torctip_stat"),
                                   String.Format("{0} {1}", GetLocalizedText(836131348284091), itm.CombinedRating) //"Item Rating {0}"
                                ));
                                break;
                        }
                    }
                }
                for (var i = 0; i < itm.CombinedStatModifiers.Count; i++)
                {
                    stats.Add(new XElement("div",
                        XClass("torctip_stat"),
                        //new XAttribute("id", String.Format("torctip_stat_{0}", i)),
                        String.Format("+{0} {1}", itm.CombinedStatModifiers[i].Modifier, itm.CombinedStatModifiers[i].DetailedStat.LocalizedDisplayName[Tooltip.language])));
                }
                if (techpower > 0)
                    stats.Add(new XElement("div",
                        XClass("torctip_stat"),
                        //new XAttribute("id", "torctip_stat_tech"),
                        String.Format("+{0} {1}", techpower, itm._dom.statData.ToStat("STAT_rtg_tech_power").LocalizedDisplayName[Tooltip.language])));
                if (forcepower > 0)
                    stats.Add(new XElement("div",
                        XClass("torctip_stat"),
                        //new XAttribute("id", "torctip_stat_force"),
                        String.Format("+{0} {1}", forcepower, itm._dom.statData.ToStat("STAT_rtg_force_power").LocalizedDisplayName[Tooltip.language])));
                tooltip.Add(stats);
            }
            //Modifications

            if (itm.EnhancementSlots.Count != 0)
            {
                XElement enhance = new XElement("div",
                    XClass("torctip_mods"),
                    new XElement("span",
                        XClass("torctip_white"),
                        String.Format("{0}:", GetLocalizedText(836131348283461))) //"Item Modifications:"
                    );

                Dictionary<string, XElement> enhancements = new Dictionary<string, XElement>();
                for (var i = 0; i < itm.EnhancementSlots.Count; i++)
                {
                    enhancements.Add(itm.EnhancementSlots[i].DetailedSlot.LocalizedDisplayName[Tooltip.language], itm.EnhancementSlots[i].ToHTML());
                }
                List<string> sortOrder = new List<string>
                    {
                        "Color Crystal",
                        "Armoring",
                        "Barrel",
                        "Hilt",
                        "Mod",
                        "Enhancement",
                        "Dye Module"
                    };
                foreach (var key in sortOrder)
                {
                    if (enhancements.ContainsKey(key))
                    {
                        enhance.Add(enhancements[key]);
                        enhancements.Remove(key);
                    }
                }
                if (enhancements.Count() > 0) //some new kind of slot?
                {
                    foreach (var kvp in enhancements)
                    {
                        enhance.Add(kvp.Value); //append them for compatibility.
                    }
                }
                string repString = GetLocalizedText(836131348283476); //"{0}: Open"
                /*enhance.Add(new XElement("div",
                    XClass("torctip_mod"),
                    new XElement("div",
                        XClass("torctip_mslot"),
                        String.Format(repString, GetLocalizedText(1173453784743948))) // "Augment: Open"
                    ));*/
                tooltip.Add(enhance);
            }

            //requirements
            string reqParanString = GetLocalizedText(836131348283395);  //"Requires {0} ({1})"
            string reqString = GetLocalizedText(836131348283394);  //"Requires {0}"
            if (itm.RequiredLevel != 0)
                tooltip.Add(new XElement("div", XClass("torctip_main"), String.Format(GetLocalizedText(836131348283393), itm.RequiredLevel))); //"Requires Level {0}"
            if (itm.RequiredClasses.Count != 0)
                tooltip.Add(new XElement("div", XClass("torctip_main"), String.Format(reqString, String.Join(",", itm.RequiredClasses.Select(x => x.Name))))); //"Requires {0}"
            if (itm.ArmorSpec != null)
                tooltip.Add(new XElement("div", XClass("torctip_main"), String.Format(reqString, itm.ArmorSpec.LocalizedName[Tooltip.language]))); //"Requires {0}"
            if (itm.WeaponSpec != null && itm.WeaponSpec.LocalizedName != null)
                tooltip.Add(new XElement("div", XClass("torctip_main"), String.Format(reqString, itm.WeaponSpec.LocalizedName[Tooltip.language]))); //"Requires {0}"
            if (itm.RequiredGender != Gender.None)
            {
                string genderString = "";
                switch(itm.RequiredGender.ToString()){
                    case "Male":
                        genderString = GetLocalizedText(836131348283503);
                        break;
                    case "Female":
                        genderString = GetLocalizedText(836131348283502);
                        break;
                }
                tooltip.Add(new XElement("div", XClass("torctip_main"), genderString));
            }
            if (itm.RequiredProfession != Profession.None)
                tooltip.Add(new XElement("div", XClass("torctip_main"), String.Format(reqParanString, itm.RequiredProfession.ConvertToString(), itm.RequiredProfessionLevel))); //"Requires {0} ({1})"
            if (itm.RequiredValorRank > 0)
            {
                tooltip.Add(new XElement("div",
                    XClass("torctip_main"),
                    String.Format(reqParanString, GetLocalizedText(836131348283505), itm.RequiredValorRank) //"Requires Valor Rank ({0})"
                    ));
            }
            if (itm.RequiresAlignment)
            {
                string alignment = GetLocalizedText(836131348283656 - Convert.ToInt32(itm.RequiredAlignmentInverted));
                AlignmentTier tier = itm._dom.alignmentData.ToTier(itm.RequiredAlignmentTier);
                if (tier != null)
                {
                    tooltip.Add(new XElement("div",
                        XClass("torctip_main"),
                        String.Format(alignment, tier.LocalizedName[Tooltip.language]) //"Requires {0} {1}{2}"
                        ));
                }
                else
                {
                    tooltip.Add(new XElement("div",
                        XClass("torctip_main"),
                        String.Format(alignment, itm.RequiredAlignmentTier) //"Requires {0} {1}{2}"
                        ));
                }
            }
            if (itm.RequiresSocial)
            {
                SocialTier soc = itm._dom.socialTierData.ToTier((int)itm.RequiredSocialTier);
                tooltip.Add(new XElement("div",
                    XClass("torctip_main"),
                    String.Format(GetLocalizedText(836131348283656), soc.LocalizedName[Tooltip.language]) //"Requires Social {0} or above"
                    ));
            }

            if (itm.RequiredReputationId != 0)
            {
                string repString = GetLocalizedText(836131348283738);
                tooltip.Add(new XElement("div",
                    XClass("torctip_main"),
                    String.Format(repString, itm.LocalizedRequiredReputationLevelName[Tooltip.language], itm.LocalizedRepFactionName[Tooltip.language]) //"Requires {1} standing with {0}"
                    ));
            }
            //decoration before abilities
            if (itm.TeachesType == "Decoration" && itm.Decoration != null)
            {
                tooltip.Add(
                    new XElement("div",
                        XClass("torctip_main"),
                        new XElement("span",
                            XClass("torctip_main"),
                            GetLocalizedText(836131348284096)), //"Stronghold Decoration: "
                        new XElement("span",
                            XClass("torctip_val"),
                            String.Format("{0} - {1}", itm.Decoration.CategoryName, itm.Decoration.SubCategoryName))
                    ),
                    new XElement("div",
                        XClass("torctip_main"),
                        new XElement("span",
                            XClass("torctip_main"),
                            GetLocalizedText(836131348284097)), // "Hook Type: "
                        new XElement("span",
                            XClass("torctip_val"),
                            String.Join(", ", itm.Decoration.AvailableHooks))
                    ),
                    new XElement("div",
                        XClass("torctip_main"),
                        new XElement("span",
                            XClass("torctip_main"),
                            GetLocalizedText(836131348284098)), //"You own: "
                        new XElement("span",
                            XClass("torctip_val"),
                            String.Format("0/{0}", itm.Decoration.MaxUnlockLimit))
                    )
                );
            }
            //abilities/description
            System.Text.RegularExpressions.Regex regex_newline = new System.Text.RegularExpressions.Regex("(\r\n|\r|\n)");
            if (itm.EquipAbilityId != 0)
            {
                if (itm.EquipAbility != null)
                {
                    string ablDesc = itm.EquipAbility.ParsedDescription ?? "";
                    if (ablDesc != "")
                    {
                        XElement desc = new XElement("div",
                            XClass("torctip_use"),
                            GetLocalizedText(836131348283442)); //"Equip: "
                        AddStringWithBreaks(ref desc, ablDesc);
                        tooltip.Add(desc);
                    }
                }
            }
            if (itm.UseAbilityId != 0)
            {
                if (itm.UseAbility != null)
                {
                    switch (itm.UseAbility.Fqn)
                    {
                        case "abl.player.learn_schematic":
                            if (itm.SchematicId != 0)
                            {
                                if (itm.Schematic.Item != null) //test for empty item
                                {
                                    tooltip.Add(new XElement("div",
                                        XClass("torctip_item"),
                                        new XElement("div",
                                            XClass("torctip_main"),
                                            String.Format(reqParanString, itm.Schematic.CrewSkillId.ToString(), itm.Schematic.SkillOrange), //"Requires {0} ({1})"
                                            ItemInnerHTML(itm.Schematic.Item))
                                        ));
                                }
                                else if (itm.Schematic.MissionDescription != "")
                                {                                       //add href here when schematics ready
                                    tooltip.Add(new XElement("div",
                                        XClass("torctip_mission"),
                                        new XElement("div",
                                            XClass("torctip_main"),
                                            String.Format(reqParanString, itm.Schematic.CrewSkillId.ToString(), itm.Schematic.SkillOrange)), //"Requires {0} ({1})"
                                        new XElement("div",
                                            XClass("torctip_mission_name"),
                                            itm.Schematic.LocalizedName[Tooltip.language]),
                                        new XElement("div",
                                            XClass("torctip_mission_yield"),
                                            itm.Schematic.LocalizedMissionYieldDescription[Tooltip.language])
                                        ));
                                }
                                else
                                    tooltip.Add(new XElement("div",
                                        XClass("torctip_use"),
                                        "Unknown Schematic"
                                        ));
                            }
                            break;
                        case "abl.player.mount.grant_mount":
                            break;
                        default:
                            if (itm.UseAbility.Fqn != null)
                                if (itm.UseAbility.Fqn.StartsWith("abl.player.") && String.IsNullOrEmpty(itm.UseAbility.LocalizedDescription[Tooltip.language])){
                                    string puasehere = "";
                                }
                            string ablDesc = "";
                            if (itm.UseAbility.ParsedLocalizedDescription != null)
                                ablDesc = itm.UseAbility.ParsedLocalizedDescription[Tooltip.language];
                            //if (ablDesc == "") break;
                            //ablDesc = System.Text.RegularExpressions.Regex.Replace(ablDesc, @"\r\n?|\n", "<br />");
                            //tooltip.Add(new XElement("div",
                            //    XClass("torctip_use"),
                            //    String.Format("Use: {0}", ablDesc)
                            //    ));
                            if (ablDesc != "")
                            {
                                XElement desc = new XElement("div",
                                    XClass("torctip_use"),
                                    String.Format("{0} ", GetLocalizedText(836131348283443))); //"Use: ");
                                AddStringWithBreaks(ref desc, ablDesc);
                                tooltip.Add(desc);
                            }
                            break;
                    }
                }
            }
            //Description
            if (itm.Description != "")
            {
                string itmDesc = itm.LocalizedDescription[Tooltip.language];
                itmDesc = System.Text.RegularExpressions.Regex.Replace(itmDesc, @"\r\n?|\n", "\n");
                XElement desc = new XElement("div",
                    XClass("torctip_desc"));
                AddStringWithBreaks(ref desc, itmDesc);
                tooltip.Add(desc);
            }
            //Decoration Source
            if (itm.TeachesType == "Decoration" || itm.StrongholdSourceList.Count > 0)
            {
                foreach (var kvp in itm.LocalizedStrongholdSourceNameDict)
                {
                    tooltip.Add(new XElement("div",
                        XClass("torctip_source"),
                        String.Format(GetLocalizedText(946314439295249), kvp.Value[Tooltip.language]) // "Source: {0}"
                    ));
                }
            }
            if (itm.SetBonusId != 0)
            {
                tooltip.Add(itm.SetBonus.ToHTML());
            }
            return tooltip;
        }

        private static XElement ToHTML(this ItemEnhancement itm)
        {
            //string slot = itm.Slot.ConvertToString();
            string slot = itm.DetailedSlot.LocalizedDisplayName[Tooltip.language];
            XElement enhancement = new XElement("div",
                XClass("torctip_mod")
                );
            if (itm.ModificationId != 0)
            {
                slot = itm.Modification.LocalizedName[Tooltip.language];
                if ((itm.Slot == EnhancementType.ColorCrystal || itm.Slot == EnhancementType.Dye) && itm.Modification.DyeColor != null)
                {
                    XElement colors = new XElement("div",
                        "[",
                        GetDyeBlock(itm.Modification.DyeColor.Palette1Rep),
                        "|",
                        GetDyeBlock(itm.Modification.DyeColor.Palette2Rep),
                        "]");
                    enhancement.Add(new XElement("div",
                        XClass("torctip_mslot"),
                        new XElement("a",
                            XClass(String.Format("torctip_{0}", itm.Modification.Quality.ToString())),
                            new XAttribute("href", String.Format("https://torcommunity.com/database/item/{0}/{1}/", itm.Modification.Base62Id, itm.Modification.LocalizedName[Tooltip.language].LinkString())),
                            new XAttribute("data-torc", "norestyle"),
                            String.Format("{0} ({1})", slot, itm.Modification.Rating.ToString()),
                            colors)
                        ));
                }
                else
                {
                    enhancement.Add(new XElement("div",
                    XClass("torctip_mslot"),
                    new XElement("a",
                        XClass(String.Format("torctip_{0}", itm.Modification.Quality.ToString())),
                        new XAttribute("href", String.Format("https://torcommunity.com/database/item/{0}/{1}/", itm.Modification.Base62Id, itm.Modification.LocalizedName[Tooltip.language].LinkString())),
                        new XAttribute("data-torc", "norestyle"),
                        String.Format("{0} ({1})", slot, itm.Modification.Rating.ToString()))
                    ));
                }

                for (var e = 0; e < itm.Modification.CombinedStatModifiers.Count; e++)
                {
                    enhancement.Add(new XElement("div",
                        XClass("torctip_mstat"),
                        String.Format("+{0} {1}", itm.Modification.CombinedStatModifiers[e].Modifier, itm.Modification.CombinedStatModifiers[e].DetailedStat.LocalizedDisplayName[Tooltip.language])
                        ));
                }
            }
            else
            {
                //empty mod
                string repString = GetLocalizedText(836131348283476); // "{0}: Open"
                enhancement.Add(new XElement("div",
                        XClass("torctip_mslot"),
                        String.Format(repString, slot)
                    ));
            }
            return enhancement;
        }

        private static XElement GetDyeBlock(System.Drawing.Color color)
        {
            if (color.Name != "0")
            {
                string color2 = String.Format("background-color: rgba({0}, {1}, {2}, {3}); box-shadow: 0px 0px 2px;",
                    (int)color.R,
                    (int)color.G,
                    (int)color.B,
                    (int)color.A);
                return new XElement("div",
                    XClass("torctip_col_block"),
                    new XAttribute("style",
                        color2),
                        " ");
            }
            else
                return new XElement("div",
                    XClass("torctip_col_block"),
                    GetLocalizedText(836131348283742)); //"No Color"
        }
        private static XElement ToHTML(this SetBonusEntry itm)
        {
            string name = "Unnamed Set Bonus";
            if (!String.IsNullOrEmpty(itm.Name))
            {
                name = itm.Name;
            }
            XElement enhancement = new XElement("div",
                XClass("torctip_set_wrapper"),
                new XElement("div",
                    XClass("torctip_set_name"),
                    new XElement("span", String.Format("{0} (", itm.Name)),
                    new XElement("span", new XAttribute("id", "set_count"), 1),
                    new XElement("span", String.Format("/{0})", itm.MaxItemCount))
                ));

            //add item list here eventually
            foreach (var kvp in itm.BonusAbilityByNum)
            {
                enhancement.Add(new XElement("div",
                    XClass("torctip_set_bonus"),
                    String.Format("({0}) {1}", kvp.Key, kvp.Value.ParsedDescription.Replace("\'", "'"))
                    ));
            }
            return enhancement;

        }
        #endregion
        #region schematic
        public static XElement GetHTML(this Schematic itm)
        {
            if (Tooltip.TooltipNameMap.Count == 0)
            {
                LoadNameMap(itm._dom);
            }
            if (itm.Id == 0) return new XElement("div", "Not Found");
            string icon = "none";
            string stringQual = "none";
            if (itm.Item != null)
            {
                stringQual = ((itm.Item.TypeBitFlags.IsModdable && (itm.Item.Quality == ItemQuality.Prototype)) ? "moddable" : itm.Item.Quality.ToString().ToLower());
                icon = itm.Item.Icon;
                if (String.IsNullOrEmpty(itm.Name))
                {
                    itm.Name = itm.Item.Name;
                }
            }

            XElement tooltip = new XElement("div", new XAttribute("class", "torctip_wrapper"));
            var fileId = TorLib.FileId.FromFilePath(String.Format("/resources/gfx/icons/{0}.dds", icon));

            if (itm != null)
            {
                tooltip.Add(new XElement("div",
                    new XAttribute("class", String.Format("torctip_image torctip_image_{0}", stringQual)),
                    new XElement("img",
                        new XAttribute("src", String.Format("https://torcommunity.com/db/icons/{0}_{1}.jpg", fileId.ph, fileId.sh)),
                        new XAttribute("alt", ""))),
                    new XElement("div",
                        new XAttribute("class", "torctip_name"),
                        new XElement("a",
                            new XAttribute("href", String.Format("https://torcommunity.com/database/item/{0}/{1}/", itm.Base62Id, itm.Name.LinkString())),
                            new XAttribute("data-torc", "norestyle"),
                            itm.Name
                            ))
                );
                XElement inner = new XElement("div",
                    XClass("torctip_tooltip"),
                    new XElement("span",
                        XClass(String.Format("torctip_{0}", stringQual)),
                        itm.Name)
                    );
                
                XElement skill = new XElement("div",
                        XClass(""),
                        new XElement("span",
                            XClass("torctip_white"),
                            "Difficulty:"),
                        new XElement("div",
                            XClass(""),
                            new XElement("span",
                                XClass("torctip_hard"),
                                String.Format("{0} ", itm.SkillOrange)),
                            new XElement("span",
                                XClass("torctip_medium"),
                                String.Format("{0} ", itm.SkillYellow)),
                            new XElement("span",
                                XClass("torctip_easy"),
                                String.Format("{0} ", itm.SkillGreen)),
                            new XElement("span",
                                XClass("torctip_trivial"),
                                String.Format("{0} ", itm.SkillGrey)))
                    );
                if (itm.MissionDescriptionId == 0)
                {
                    XElement components = new XElement("div",
                        XClass(""),
                        new XElement("span",
                            XClass("torctip_white"),
                            "Components:")
                    );
                    if (itm.Materials != null)
                    {
                        foreach (var kvp in itm.Materials)
                        {
                            var mat = (Item)new GameObject().Load(kvp.Key, itm._dom);
                            var matstringQual = ((mat.TypeBitFlags.IsModdable && (mat.Quality == ItemQuality.Prototype)) ? "moddable" : mat.Quality.ToString().ToLower());
                            var matfileId = TorLib.FileId.FromFilePath(String.Format("/resources/gfx/icons/{0}.dds", mat.Icon));
                            components.Add(new XElement("div",
                                XClass("torctip_mat"),
                                new XElement("span", String.Format("{0}x ", kvp.Value)),
                                new XElement("div",
                                    new XAttribute("class", String.Format("torctip_image torctip_image_{0} small_border", matstringQual)),
                                    new XElement("img",
                                        new XAttribute("src", String.Format("https://torcommunity.com/db/icons/{0}_{1}.jpg", matfileId.ph, matfileId.sh)),
                                        new XAttribute("alt", itm.Name),
                                        XClass("small_image"))),
                                new XElement("div",
                                    new XAttribute("class", "torctip_mat_name"),
                                    new XElement("a",
                                        XClass(String.Format("torctip_{0}", matstringQual)),
                                        new XAttribute("href", String.Format("https://torcommunity.com/database/item/{0}/{1}/", mat.Base62Id, LinkString(mat.Name))),
                                        new XAttribute("data-torc", "norestyle"),
                                        mat.Name)
                                    )
                                )
                            );
                        }
                    }
                    else
                    {
                        components.Add(new XElement("div", "Components List Empty!"));
                    }
                    if (itm.Item != null)
                    {
                        inner.Add(skill, components, itm.Item.ItemInnerHTML());
                    }
                    else
                    {
                        inner.Add(new XElement("div", "Crafted Item Missing!"));
                    }
                    inner.Add(new XElement("div",
                        XClass("torctip_main"),
                        String.Format("Requires {0} ({1})", itm.CrewSkillName, itm.SkillOrange))
                    );
                    tooltip.Add(inner);
                }
                else
                {
                    inner.Add(skill, new XElement("div",
                        XClass("torctip_mission"),
                        new XElement("div",
                            XClass("torctip_main"),
                            String.Format("Requires {0} ({1})", itm.CrewSkillId.ToString(), itm.SkillOrange)),
                        new XElement("div",
                            XClass("torctip_mission_name"),
                            itm.Name),
                        new XElement("div",
                            XClass("torctip_mission_desc"),
                            itm.MissionDescription),
                        new XElement("div",
                            XClass("torctip_mission_yield"),
                            itm.MissionYieldDescription),
                        new XElement("div",
                            XClass("torctip_mission_faction"),
                            itm.MissionFaction.ToString())
                        ));
                    tooltip.Add(inner);
                }
            }

            return tooltip;
        }
        #endregion
        #region ability
        public static XElement GetHTML(this Ability itm)
        {
            if (Tooltip.TooltipNameMap.Count == 0)
            {
                LoadNameMap(itm._dom);
            }
            if (itm.Id == 0) return new XElement("div", "Not Found");
            string icon = "none";
            string stringQual = "ability";
            icon = itm.Icon;

            XElement tooltip = new XElement("div", new XAttribute("class", "torctip_wrapper"));
            var fileId = TorLib.FileId.FromFilePath(String.Format("/resources/gfx/icons/{0}.dds", icon));

            if (itm != null)
            {
                tooltip.Add(new XElement("div",
                    new XAttribute("class", String.Format("torctip_image torctip_image_{0}", stringQual)),
                    new XElement("img",
                        new XAttribute("src", String.Format("https://torcommunity.com/db/icons/{0}_{1}.jpg", fileId.ph, fileId.sh)),
                        new XAttribute("alt", ""))),
                    new XElement("div",
                        new XAttribute("class", "torctip_name"),
                        new XElement("a",
                            new XAttribute("href", String.Format("https://torcommunity.com/database/ability/{0}/{1}/", itm.Base62Id, itm.Name.LinkString())),
                            new XAttribute("data-torc", "norestyle"),
                            itm.Name
                            ))
                );
                XElement inner = new XElement("div",
                    XClass("torctip_tooltip"),
                    new XElement("span",
                        XClass(String.Format("torctip_{0}", stringQual)),
                        itm.Name)
                    );

                XElement cast = new XElement("div");
                if (itm.IsPassive) {
                    cast.Add(new XElement("span",
                            XClass("torctip_white"),
                            "Passive")
                        );
                }
                else if(itm.CastingTime > 0){
                    cast.Add(XStat("Activation: ", String.Format("{0}s", itm.CastingTime)));
                }
                else if(itm.ChannelingTime > 0){
                    cast.Add(XStat("Channeled: ", String.Format("{0}s", itm.ChannelingTime)));
                }
                else{
                    cast.Add(new XElement("span",
                            XClass("torctip_white"),
                            "Instant")
                        );
                }
                inner.Add(cast);

                XElement playerblock = new XElement("div");
                string costType = "";
                float cost = 0;
                if(itm.ApCost > 0) {
                    costType = "Heat/Ammo: ";
                    cost = itm.ApCost;
                }
                else if(itm.ForceCost > 0) {
                    costType = "Force: ";
                    cost = itm.ForceCost;
                }
                else if(itm.EnergyCost > 0) {
                    costType = "Energy: ";
                    cost = itm.EnergyCost;
                }
                if (costType != "" && cost != 0)
                {
                    playerblock.Add(XStat(costType, cost.ToString()));
                }
                if (itm.Cooldown > 0)
                {
                    playerblock.Add(XStat("Cooldown: ", String.Format("{0}s", itm.Cooldown.ToString())));
                }
                if (itm.MaxRange > 0)
                {
                    playerblock.Add(XStat("Range: ", String.Format("{0}m", Math.Round(itm.MaxRange * 10).ToString())));
                }
                if (playerblock.HasElements)
                {
                    inner.Add(new XElement("br"), playerblock);
                }

                inner.Add(new XElement("br"),
                    new XElement("div",
                        XClass("torctip_white"),
                        itm.Description)
                    );
                tooltip.Add(inner);
            }

            return tooltip;
        }

        #endregion
        #region mission
        public static XElement GetHTML(this Quest itm)
        {
            if (Tooltip.TooltipNameMap.Count == 0)
            {
                LoadNameMap(itm._dom);
            }
            if (itm.Id == 0) return new XElement("div", "Not Found");
            string icon = "none";
            string stringQual = "mission";
            icon = itm.Icon;

            XElement tooltip = new XElement("div", new XAttribute("class", "torctip_wrapper"));
            var fileId = TorLib.FileId.FromFilePath(String.Format("/resources/gfx/icons/{0}.dds", icon));

            if (itm != null)
            {
                XElement inner = new XElement("div",
                    XClass("torctip_tooltip"),
                    new XElement("span",
                        XClass(String.Format("torctip_{0}", stringQual)),
                        itm.Name)
                    );

                XElement desc = new XElement("div",
                        XClass("torctip_white"));
                string journalText = itm.Branches.Select(x => x.Steps.Select(y => y.JournalText).FirstOrDefault(z=> !String.IsNullOrEmpty(z))).FirstOrDefault(z=> !String.IsNullOrEmpty(z));
                if (!String.IsNullOrEmpty(journalText))
                {
                    AddStringWithBreaks(ref desc, journalText);
                    inner.Add(desc);
                }
                XElement taskText = new XElement("span",
                    XClass("torctip_tsk_txt"),
                    "Tasks:");
                bool taskAdded = false;
                foreach (var branch in itm.Branches)
                {
                    int stepNum = 1;
                    XElement branchElement = new XElement("div",
                            XClass("torctip_branch"));
                    bool addElement = false;
                    foreach (var step in branch.Steps)
                    {
                        bool addTask = false;
                        if(step.Tasks.Count > 0)
                        {
                            XElement taskCont = new XElement("div",
                                XClass("torc_task_cont"),
                                new XElement("span",
                                    XClass("torc_brnch_id"),
                                    String.Format("{0}) ",stepNum))
                            );
                            XElement taskInner = new XElement("div",
                                XClass("torc_mis_tasks"));
                            
                            foreach (var task in step.Tasks)
                            {
                                if (String.IsNullOrEmpty(task.String)) continue;
                                if(task.ShowCount)
                                {
                                    taskInner.Add(new XElement("div",
                                        XClass("torc_task"),
                                        String.Format("{0}: ", task.String),
                                        new XElement("span",
                                            XClass("torctip_val"),
                                            String.Format("0/{0}", task.CountMax))
                                        )
                                    );
                                }
                                else
                                {
                                    taskInner.Add(new XElement("div",
                                        XClass("torc_task"),
                                        task.String)
                                    );
                                }
                                addTask = true;
                                addElement = true;
                            }
                            if (addTask)
                            {
                                taskCont.Add(taskInner);
                                branchElement.Add(taskCont);
                            }
                        }

                        if (addTask)
                        {
                            stepNum++;
                        }
                    }
                    if (addElement)
                    {
                        if (!taskAdded)
                        {
                            inner.Add(taskText);
                        }
                        inner.Add(branchElement);
                    }
                }
                if (itm.Classes.Count > 0)
                {
                    XElement requiredClasses = new XElement("div",
                        XClass("torctip_rqd_cls"),
                        new XElement("span",
                            "Requires: "));
                    List<ClassSpec> classes = itm.Classes.OrderBy(x => x.GetFaction()).ThenBy(x => x.Name).ToList();
                    for (int i = 0; i < classes.Count; i++)
                    {
                        string joiner = ",";
                        if (i == classes.Count - 1)
                            joiner = "";
                        requiredClasses.Add(new XElement("span",
                            XClass(String.Format("torc_cls_{0}", classes[i].GetFaction())),
                            String.Format("{0}{1} ", classes[i].Name, joiner))                            
                        );
                    }
                    inner.Add(requiredClasses);
                }
                XElement rewards = new XElement("div",
                    XClass("torctip_rewards"),
                    new XElement("span",
                        "Mission Rewards")
                    );
                XElement rewardContainer = new XElement("div",
                    XClass("torctip_rwd_inner"));
                int rewardCount = 0;
                if (itm.XP != 0 && itm.Difficulty != QuestDifficulty.NoExp)
                {
                    rewardContainer.Add(new XElement("div",
                        XClass("torctip_rwd_info"),
                        new XElement("span",
                            "Experience: "),
                        new XElement("span",
                            new XElement("span",
                                XClass("torctip_exp"),
                                String.Format("{0} (Sub)", itm.SubXP.ToString("n0"))),
                            " / ",
                            new XElement("span",
                                XClass("torctip_exp_f2p"),
                                String.Format("{0} (F2P)", itm.F2PXP.ToString("n0")))
                            )
                        )
                    );
                    rewardCount++;
                }
                if (itm.CreditsRewarded != 0)
                {
                    rewardContainer.Add(new XElement("div",
                        XClass("torctip_rwd_info"),
                        new XElement("span",
                            "Credits: "),
                        new XElement("span",
                            XClass("torctip_credits"),
                            itm.CreditsRewarded)
                        )
                    );
                    rewardCount++;
                }
                if (itm.Rewards != null)
                {
                    XElement items = new XElement("div",
                            XClass("torctip_rwd_itms"));
                    XElement providedRewards = new XElement("div",
                        XClass("torctip_rwd_items"),
                        new XElement("div",
                            "Provided Rewards:"));
                    Dictionary<string, XElement> providedClassRewards = new Dictionary<string,XElement>();
                    XElement selectOneRewards = new XElement("div",
                        XClass("torctip_rwd_items"),
                        new XElement("div",
                            "Select One Reward:"));
                    Dictionary<string, XElement> selectOneClassRewards = new Dictionary<string,XElement>();
                    HashSet<ulong> clsIds = new HashSet<ulong>();
                    clsIds.UnionWith(itm.Classes.Select(x => x.Id).ToList());
                    AddBaseClassIds(clsIds);
                    foreach (var rew in itm.Rewards)
                    {
                        var mat = rew.RewardItem;
                        if (mat == null) continue;
                        var matstringQual = ((mat.TypeBitFlags.IsModdable && (mat.Quality == ItemQuality.Prototype)) ? "moddable" : mat.Quality.ToString().ToLower());
                        var matfileId = TorLib.FileId.FromFilePath(String.Format("/resources/gfx/icons/{0}.dds", mat.Icon));
                        XElement matElement = new XElement("div",
                            XClass("torctip_rwd"),
                            new XAttribute("style", "display:inline;"),
                            new XElement("a",
                                new XAttribute("href", String.Format("https://torcommunity.com/database/item/{0}/{1}/", mat.Base62Id, LinkString(mat.Name))),
                                new XAttribute("data-torc", "norestyle"),
                                new XAttribute("class", String.Format("torctip_image torctip_image_{0}", matstringQual)),
                                new XElement("img",
                                    new XAttribute("src", String.Format("https://torcommunity.com/db/icons/{0}_{1}.jpg", matfileId.ph, matfileId.sh)),
                                    new XAttribute("alt", mat.Name),
                                    XClass("image")
                                )
                            )
                        );
                        if (rew.RewardItem.MaxStack > 1)
                        {
                            matElement.Element("a").Add(new XElement("span",
                                    XClass("torctip_rwd_overlay"),
                                    rew.NumberOfItem
                                )
                            );
                        }
                        if (rew.IsAlwaysProvided)
                        {
                            if (rew.Classes.Count > 0 && itm.Classes.Count > 1)
                            {
                                foreach (var cls in rew.Classes)
                                {
                                    if (clsIds.Count > 0 && !clsIds.Contains(cls.Id))
                                        continue;
                                    if (!providedClassRewards.ContainsKey(cls.Name))
                                    {
                                        providedClassRewards.Add(cls.Name,
                                            new XElement("div",
                                                XClass(String.Format("torctip_class_restrict torc_cls_{0}", cls.GetFaction())),
                                                new XElement("span", cls.Name)
                                            )
                                        );
                                    }
                                    providedClassRewards[cls.Name].Add(matElement);
                                }
                            }
                            else if (rew.MinLevel != 1 || rew.MaxLevel != 60)
                            {
                                string levelRestrict = String.Format("Level {0}-{1}", rew.MinLevel, rew.MaxLevel);
                                if (!providedClassRewards.ContainsKey(levelRestrict))
                                {
                                    providedClassRewards.Add(levelRestrict,
                                        new XElement("div",
                                            XClass("torctip_class_restrict torc_cls_level"),
                                            new XElement("span", levelRestrict)
                                        )
                                    );
                                }
                                providedClassRewards[levelRestrict].Add(matElement);
                            }
                            else
                            {
                                providedRewards.Add(matElement);
                            }
                        }
                        else if (rew.MinLevel != 1 || rew.MaxLevel != 60)
                        {
                            string ruhroh = "";
                        }
                        else
                        {
                            if (rew.Classes.Count > 0 && itm.Classes.Count > 1)
                            {
                                foreach (var cls in rew.Classes)
                                {
                                    if (clsIds.Count > 0 && !clsIds.Contains(cls.Id))
                                        continue;
                                    if (!selectOneClassRewards.ContainsKey(cls.Name))
                                    {
                                        selectOneClassRewards.Add(cls.Name,
                                            new XElement("div",
                                                XClass(String.Format("torctip_class_restrict torc_cls_{0}", cls.GetFaction())),
                                                new XElement("span", cls.Name)
                                            )
                                        );
                                    }
                                    selectOneClassRewards[cls.Name].Add(matElement);
                                }
                            }
                            else if (rew.MinLevel != 1 || rew.MaxLevel != 60)
                            {
                                string levelRestrict = String.Format("Level {0}-{1}", rew.MinLevel, rew.MaxLevel);
                                if (!selectOneClassRewards.ContainsKey(levelRestrict))
                                {
                                    selectOneRewards.Add(levelRestrict,
                                        new XElement("div",
                                            XClass("torctip_class_restrict torc_cls_level"),
                                            new XElement("span", levelRestrict)
                                        )
                                    );
                                }
                                selectOneClassRewards[levelRestrict].Add(matElement);
                            }
                            else
                            {
                                selectOneRewards.Add(matElement);
                            }
                        }
                        rewardCount++;
                    }
                    if (providedClassRewards.Count > 0)
                        providedRewards.Add(providedClassRewards.Values.OrderBy(x => x.Attribute("class").Value).ThenBy(x => x.Element("span").Value));
                    if (selectOneClassRewards.Count > 0)
                        selectOneRewards.Add(selectOneClassRewards.Values.OrderBy(x => x.Attribute("class").Value).ThenBy(x => x.Element("span").Value));
                    if (providedRewards.Elements().Count() > 1)
                        rewardContainer.Add(providedRewards);
                    if (selectOneRewards.Elements().Count() > 1)
                        rewardContainer.Add(selectOneRewards);
                }
                if (rewardCount > 0)
                {
                    rewards.Add(rewardContainer);
                    inner.Add(rewards);
                }

                tooltip.Add(inner);
            }

            if(itm.BranchCount > 1)
            {
                string sflsoljh = "";
            }
            if (itm.Base62Id == "xxScuD3")
            {
                string sflsoljh = "";
                tooltip.Descendants().Where(x => x.Value == null).Remove();
            }
            return tooltip;
        }

        #endregion
        #region talents
        public static XElement GetHTML(this Talent itm)
        {
            if (Tooltip.TooltipNameMap.Count == 0)
            {
                LoadNameMap(itm._dom);
            }
            if (itm.Id == 0) return new XElement("div", "Not Found");
            string icon = "none";
            string stringQual = "talent";
            icon = itm.Icon;

            XElement tooltip = new XElement("div", new XAttribute("class", "torctip_wrapper"));
            var fileId = TorLib.FileId.FromFilePath(String.Format("/resources/gfx/icons/{0}.dds", icon));

            if (itm != null)
            {
                tooltip.Add(new XElement("div",
                    new XAttribute("class", String.Format("torctip_image torctip_image_{0}", stringQual)),
                    new XElement("img",
                        new XAttribute("src", String.Format("https://torcommunity.com/db/pts/icons/{0}_{1}.jpg", fileId.ph, fileId.sh)),
                        new XAttribute("alt", ""))),
                    new XElement("div",
                        new XAttribute("class", "torctip_name"),
                        new XElement("a",
                            new XAttribute("href", String.Format("https://torcommunity.com/database/pts/talent/{0}/{1}/", itm.Base62Id, itm.Name.LinkString())),
                            new XAttribute("data-torc", "norestyle"),
                            itm.Name
                            ))
                );
                XElement inner = new XElement("div",
                    XClass("torctip_tooltip"),
                    new XElement("span",
                        XClass(String.Format("torctip_{0}", stringQual)),
                        itm.Name),
                        new XElement("span",
                            XClass("torctip_white"),
                            "Passive"
                        ),
                        new XElement("br"),
                        new XElement("div",
                            XClass("torctip_white"),
                            itm.Description)
                    );

                
                inner.Add(
                    );
                tooltip.Add(inner);
            }

            return tooltip;
        }

        #endregion
        #region achievements
        public static XElement GetHTML(this Achievement itm)
        {
            if (Tooltip.TooltipNameMap.Count == 0)
            {
                LoadNameMap(itm._dom);
            }
            if (itm.Id == 0) return new XElement("div", "Not Found");
            string icon = "none";
            string stringQual = "achievement";
            icon = itm.Icon;

            XElement tooltip = new XElement("div", new XAttribute("class", "torctip_wrapper"));
            var fileId = TorLib.FileId.FromFilePath(String.Format("/resources/gfx/icons/{0}.dds", icon));

            if (itm != null)
            {
                tooltip.Add(new XElement("div",
                    new XAttribute("class", String.Format("torctip_image torctip_image_{0}", stringQual)),
                    new XElement("img",
                        new XAttribute("src", String.Format("https://torcommunity.com/db/icons/{0}_{1}.jpg", fileId.ph, fileId.sh)),
                        new XAttribute("alt", "")),
                    new XElement("div",
                        XClass("torctip_icon_points"),
                        new XElement("span",
                            XClass("torctip_ach_star"),
                            " "
                        ),
                        itm.Rewards.AchievementPoints
                    )

                ));
                XElement inner = new XElement("div",
                    XClass("torctip_tooltip"),
                    new XElement("span",
                        XClass(String.Format("torctip_{0}", stringQual)),
                        itm.Name
                    )
                );
                bool addRewards = false;
                if(itm.Rewards != null)
                {
                    inner.Add(new XElement("span",
                        XClass("torctip_ach_points"),
                        " [ ",
                        new XElement("span",
                            XClass("torctip_ach_star"),
                            " "
                        ),
                        String.Format(" {0} ]", itm.Rewards.AchievementPoints)
                    ));
                }

                XElement desc = new XElement("div",
                    XClass("torctip_blue"),
                    itm.Description
                );
                inner.Add(desc);
                XElement taskText = new XElement("span",
                    XClass("torctip_ach_tsks")
                );
                foreach (var tsk in itm.Tasks)
                {
                    string tskName = tsk.Name;
                    if (tsk.LocalizedNames.Count != 0)
                        tskName = tsk.LocalizedNames[Tooltip.language];
                    if (tskName == "")
                    {
                        var tskSub = itm._dom.GetObject(tsk.Id);
                        if (tskSub != null)
                        {
                            GameObject obj = new GomLib.Models.GameObject().Load(tskSub);
                            switch (tskSub.Name.Substring(0, 4))
                            {
                                case "ach.":
                                    taskText.Add(new XElement("div",
                                        XClass("torctip_ach_tsk"),
                                        new XElement("span",
                                            String.Format("0/{0} ", tsk.Count)
                                        ),
                                        new XElement("a",
                                            XClass(String.Format("torctip_{0}", "achievement")),
                                            new XAttribute("href", String.Format("https://torcommunity.com/database/achievement/{0}/{1}/", obj.Base62Id, ((Achievement)obj).LocalizedName[Tooltip.language].LinkString())),
                                            new XAttribute("data-torc", "norestyle"),
                                            ((Achievement)obj).LocalizedName[Tooltip.language]
                                        )
                                    ));
                                    break;
                                case "abl.":
                                    taskText.Add(new XElement("div",
                                        XClass("torctip_ach_tsk"),
                                        new XElement("span",
                                            String.Format("0/{0} ", tsk.Count)
                                        ),
                                        new XElement("a",
                                            XClass(String.Format("torctip_{0}", "ability")),
                                            new XAttribute("href", String.Format("https://torcommunity.com/database/ability/{0}/{1}/", obj.Base62Id, ((Ability)obj).LocalizedName[Tooltip.language].LinkString())),
                                            new XAttribute("data-torc", "norestyle"),
                                            ((Ability)obj).LocalizedName[Tooltip.language]
                                        )
                                    ));
                                    break;
                                case "cdx.":
                                    taskText.Add(new XElement("div",
                                        XClass("torctip_ach_tsk"),
                                        new XElement("span",
                                            String.Format("0/{0} ", tsk.Count)
                                        ),
                                        new XElement("a",
                                            XClass(String.Format("torctip_{0}", "codex")),
                                            new XAttribute("href", String.Format("https://torcommunity.com/database/codex/{0}/{1}/", obj.Base62Id, ((Codex)obj).LocalizedName[Tooltip.language].LinkString())),
                                            new XAttribute("data-torc", "norestyle"),
                                            ((Codex)obj).LocalizedName[Tooltip.language]
                                        )
                                    ));
                                    break;
                                case "npc.":
                                    taskText.Add(new XElement("div",
                                        XClass("torctip_ach_tsk"),
                                        new XElement("span",
                                            String.Format("0/{0} ", tsk.Count)
                                        ),
                                        new XElement("a",
                                            XClass(String.Format("torctip_{0}", "npc")),
                                            new XAttribute("href", String.Format("https://torcommunity.com/database/npc/{0}/{1}/", obj.Base62Id, ((Npc)obj).LocalizedName[Tooltip.language].LinkString())),
                                            new XAttribute("data-torc", "norestyle"),
                                            ((Npc)obj).LocalizedName[Tooltip.language]
                                        )
                                    ));
                                    break;
                                case "qst.":
                                    taskText.Add(new XElement("div",
                                        XClass("torctip_ach_tsk"),
                                        new XElement("span",
                                            String.Format("0/{0} ", tsk.Count)
                                        ),
                                        new XElement("a",
                                            XClass(String.Format("torctip_{0}", "mission")),
                                            new XAttribute("href", String.Format("https://torcommunity.com/database/mission/{0}/{1}/", obj.Base62Id, ((Quest)obj).LocalizedName[Tooltip.language].LinkString())),
                                            new XAttribute("data-torc", "norestyle"),
                                            ((Quest)obj).LocalizedName[Tooltip.language]
                                        )
                                    ));
                                    break;
                                case "tal.":
                                    tskName = ((Talent)obj).LocalizedName[Tooltip.language];
                                    break;
                                case "sche":
                                    tskName = ((Schematic)obj).LocalizedName[Tooltip.language];
                                    break;
                                case "dec.":
                                    tskName = ((Decoration)obj).LocalizedName[Tooltip.language];
                                    break;
                                case "itm.":
                                    taskText.Add(new XElement("div",
                                        XClass("torctip_ach_tsk"),
                                        new XElement("span",
                                            String.Format("0/{0} ", tsk.Count)
                                        ),
                                        new XElement("a",
                                            XClass(String.Format("torctip_{0}", ((Item)obj).Quality.ToString())),
                                            new XAttribute("href", String.Format("https://torcommunity.com/database/item/{0}/{1}/", obj.Base62Id, ((Item)obj).LocalizedName[Tooltip.language].LinkString())),
                                            new XAttribute("data-torc", "norestyle"),
                                            ((Item)obj).LocalizedName[Tooltip.language]
                                        )
                                    ));
                                    break;
                                default:
                                    return null;
                            }
                        }
                    }
                    else
                    {
                        taskText.Add(new XElement("div",
                            XClass("torctip_white"),
                            String.Format("0/{0} {1}", tsk.Count, tskName)
                        ));
                    }
                }
                inner.Add(taskText);
                XElement rewards = new XElement("div",
                    XClass("torctip_rewards"),
                    new XElement("span",
                        "Rewards")
                );
                XElement rewardContainer = new XElement("div",
                    XClass("torctip_rwd_inner"));
                if (itm.Rewards != null)
                {
                    if(itm.Rewards.CartelCoins > 0)
                    {
                        addRewards = true;
                        rewardContainer.Add(new XElement("div",
                            XClass("torctip_ach_items"),
                            new XElement("div",
                                XClass("torctip_blue"),
                                "Cartel Coins:"
                            ),
                            new XElement("div",
                                XClass("torctip_mtx_coins"),
                                itm.Rewards.CartelCoins
                            )
                        ));
                    }
                    if (itm.Rewards.Requisition > 0)
                    {
                        addRewards = true;
                        rewardContainer.Add(new XElement("div",
                            XClass("torctip_ach_items"),
                            new XElement("div",
                                XClass("torctip_blue"),
                                "Fleet Requisition:"
                            ),
                            new XElement("div",
                                XClass("torctip_gsf_cur"),
                                itm.Rewards.Requisition
                            )
                        ));
                    }
                    if (itm.Rewards.LegacyTitle != null)
                    {
                        addRewards = true;
                        rewardContainer.Add(new XElement("div",
                            XClass("torctip_ach_items"),
                            new XElement("div",
                                XClass("torctip_blue"),
                                "Legacy Title:"
                            ),
                            new XElement("div",
                                XClass("torctip_lgy_tit"),
                                itm.Rewards.LocalizedLegacyTitle[Tooltip.language]  
                            )
                        ));
                    }
                    if (itm.Rewards.ItemRewardList != null)
                    {
                        XElement providedRewards = new XElement("div",
                            XClass("torctip_ach_items"),
                            new XElement("div",
                                XClass("torctip_blue"),
                                "Item Rewards:"));
                        foreach (var rew in itm.Rewards.ItemRewardList)
                        {
                            addRewards = true;
                            var mat = itm._dom.itemLoader.Load(rew.Key);
                            if (mat == null) continue;
                            var matstringQual = ((mat.TypeBitFlags.IsModdable && (mat.Quality == ItemQuality.Prototype)) ? "moddable" : mat.Quality.ToString().ToLower());
                            var matfileId = TorLib.FileId.FromFilePath(String.Format("/resources/gfx/icons/{0}.dds", mat.Icon));
                            XElement matElement = new XElement("div",
                                XClass("torctip_rwd"),
                                new XAttribute("style", "display:inline;"),
                                new XElement("a",
                                    new XAttribute("href", String.Format("https://torcommunity.com/database/item/{0}/{1}/", mat.Base62Id, LinkString(mat.Name))),
                                    new XAttribute("data-torc", "norestyle"),
                                    new XAttribute("class", String.Format("torctip_image torctip_image_{0}", matstringQual)),
                                    new XElement("img",
                                        new XAttribute("src", String.Format("https://torcommunity.com/db/icons/{0}_{1}.jpg", matfileId.ph, matfileId.sh)),
                                        new XAttribute("alt", mat.Name),
                                        XClass("image")
                                    ),
                                    new XElement("span",
                                        XClass("torctip_rwd_overlay"),
                                        rew.Value
                                    )
                                )
                            );
                            providedRewards.Add(matElement);
                        }
                        if (providedRewards.Elements().Count() > 1)
                            rewardContainer.Add(providedRewards);
                    }
                }
                if (addRewards)
                {
                    rewards.Add(rewardContainer);
                    inner.Add(rewards);
                }

                tooltip.Add(inner);
            }

            return tooltip;
        }

        #endregion

        #region HelperMethods
        /// <summary>
        /// Returns the XML string of the <paramref name="xElement"/> WITHOUT CHARACTER CHECKING.
        /// </summary>
        /// <param name="xElement"></param>
        /// <returns></returns>
        public static string ToStringWithoutCharacterChecking(this XElement xElement)
        {
            using (System.IO.StringWriter stringWriter = new System.IO.StringWriter())
            {
                XmlWriterSettings s = new XmlWriterSettings();
                s.CheckCharacters = false;
                s.Indent = false;
                s.Encoding = Encoding.Default;
                s.OmitXmlDeclaration = true;

                using (System.Xml.XmlWriter xmlTextWriter = XmlWriter.Create(stringWriter, s))
                {
                    xElement.WriteTo(xmlTextWriter);
                }
                string ret = stringWriter.ToString();
                return ret;
            }

        }
        private static void AddStringWithBreaks(ref XElement element, string desc)
        {
            if (desc.Contains('\n'))
            {
                var splits = desc.Split('\n');
                int count = splits.Count();
                for (int i = 0; i < count; i++)
                {
                    element.Add(splits[i]);
                    if (i != count)
                        element.Add(new XElement("br"));
                }
            }
            else
            {
                element.Add(desc);
            }
        }
        private static string LinkString(this string name)
        {
            string cleaned = name;
            cleaned = cleaned.Replace(".", "")
                .Replace(",", "")
                .Replace("'", "")
                .Replace("\"", "")
                .Replace("[", "")
                .Replace("]", "")
                .Replace("(", "")
                .Replace(")", "")
                .Replace(" ", "+");
            return cleaned.ToLower();
        }
        public static string ToRoman(this int number)
        {
            if ((number < 0) || (number > 3999))
                return number.ToString();
            if (number < 1) return string.Empty;
            if (number >= 1000) return "M" + ToRoman(number - 1000);
            if (number >= 900) return "CM" + ToRoman(number - 900);
            if (number >= 500) return "D" + ToRoman(number - 500);
            if (number >= 400) return "CD" + ToRoman(number - 400);
            if (number >= 100) return "C" + ToRoman(number - 100);
            if (number >= 90) return "XC" + ToRoman(number - 90);
            if (number >= 50) return "L" + ToRoman(number - 50);
            if (number >= 40) return "XL" + ToRoman(number - 40);
            if (number >= 10) return "X" + ToRoman(number - 10);
            if (number >= 9) return "IX" + ToRoman(number - 9);
            if (number >= 5) return "V" + ToRoman(number - 5);
            if (number >= 4) return "IV" + ToRoman(number - 4);
            if (number >= 1) return "I" + ToRoman(number - 1);
            throw new ArgumentOutOfRangeException("something bad happened");
        }
        internal static List<ulong> ImpClasses = new List<ulong> {
            16140902893827567561, //Sith Warrior
            16141024490216983174, //Sith Marauder
            16141180228828243745, //Sith Juggernaut
            16140943676484767978, //Imperial Agent
            16141046347418927959, //Sniper
            16140905232405801950, //Operative
            16141010271067846579, //Sith Inquisitor
            16141163438392504574, //Sith Assassin
            16141067119934185414, //Sith Sorcerer
            16141170711935532310, //Bounty Hunter
            16141007401395916385, //Powertech
            16141111589108060476, //Mercenary
        };
        internal static void AddBaseClassIds(HashSet<ulong> clsIds)
        {
            if (clsIds.Contains(16140902893827567561))//Sith Warrior
            {
                clsIds.Add(16141024490216983174); //Sith Marauder
                clsIds.Add(16141180228828243745); //Sith Juggernaut
            }
            if (clsIds.Contains(16140943676484767978)) //Imperial Agent
            {
                clsIds.Add(16141046347418927959); //Sniper
                clsIds.Add(16140905232405801950); //Operative
            }
            if (clsIds.Contains(16141010271067846579)) //Sith Inquisitor
            {
                clsIds.Add(16141163438392504574); //Sith Assassin
                clsIds.Add(16141067119934185414);  //Sith Sorcerer
            }
            if (clsIds.Contains(16141170711935532310)) //Bounty Hunter
            {
                clsIds.Add(16141007401395916385); //Powertech
                clsIds.Add(16141111589108060476); //Mercenary
            }

            if (clsIds.Contains(16141179471541245792)) //Jedi Consular
            {
                clsIds.Add(16140939761890536394); //Jedi Sage
                clsIds.Add(16141082698337403481); //Jedi Shadow
            }
            if (clsIds.Contains(16140912704077491401)) //Smuggler
            {
                clsIds.Add(16141041084185282043); //Gunslinger
                clsIds.Add(16141067128654459200); //Scoundrel
            }
            if (clsIds.Contains(16140973599688231714)) //Trooper
            {
                clsIds.Add(16141067504602942620); //Commando
                clsIds.Add(16141087184558207941); //Vanguard
            }
            if (clsIds.Contains(16141119516274073244)) //Jedi Knight
            {
                clsIds.Add(16140975849784542883); //Jedi Guardian
                clsIds.Add(16141180228828243745); //Jedi Sentinel
            }
        }
        public static string GetFaction(this ClassSpec cls)
        {
            if (ImpClasses.Contains(cls.Id)) return "Imperial";
            return "Republic";
        }
        public static XAttribute XClass(string classname)
        {
            return new XAttribute("class", classname);
        }
        private static XElement XStat(string text, string value)
        {
            return new XElement("div",
                new XElement("span",
                    XClass("torctip_val"),
                    text),
                new XElement("span",
                    XClass("torctip_white"),
                    value)
                );
        }
        #region ConvertToString
        public static void LoadNameMap(DataObjectModel dom){
            AddTableToMap(dom.stringTable.Find("str.gui.equipslot"));
            AddTableToMap(dom.stringTable.Find("str.gui.tooltips"));
            AddTableToMap(dom.stringTable.Find("str.gui.items"));
            AddTableToMap(dom.stringTable.Find("str.gui.itm.enhancement.types"));
            AddTableToMap(dom.stringTable.Find("str.prf.professions"));
        }

        private static void AddTableToMap(StringTable table)
        {
            foreach (var entry in table.data)
            {
                Dictionary<string, string> tempDict = entry.Value.localizedText.ToDictionary(x => x.Key, x => x.Value.Replace("<<1>>", "{0}").Replace("<<2>>", "{1}"));
                Tooltip.TooltipNameMap.Add(entry.Value.Id, tempDict);
            }
        }
        private static string GetLocalizedText(long id)
        {
            return GetLocalizedText(id, Tooltip.language);
        }
        public static string GetLocalizedText(long id, string language)
        {
            if (Tooltip.TooltipNameMap.ContainsKey(id))
                return Tooltip.TooltipNameMap[id][language];
            return null;
        }
        public static string GetLocalizedText(long id, string language, string defaultVal)
        {
            var returnVal = GetLocalizedText(id, language);
            if (returnVal == null) return defaultVal;
            return returnVal;
        }
        
        public static string ConvertToString(this SlotType slot, string language)
        {
            switch (slot)
            {
                case SlotType.EquipHumanMainHand: return GetLocalizedText(2073124879204376,language);
                case SlotType.EquipHumanOffHand: return GetLocalizedText(2073124879204354,language);
                case SlotType.EquipHumanWrist: return GetLocalizedText(2073124879204357,language);
                case SlotType.EquipHumanBelt: return GetLocalizedText(2073124879204358,language);
                case SlotType.EquipHumanChest: return GetLocalizedText(2073124879204355,language);
                case SlotType.EquipHumanEar: return GetLocalizedText(2073124879204362,language);
                case SlotType.EquipHumanFace: return GetLocalizedText(2073124879204361,language);
                case SlotType.EquipHumanFoot: return GetLocalizedText(2073124879204360,language);
                case SlotType.EquipHumanGlove: return GetLocalizedText(2073124879204359,language);
                case SlotType.EquipHumanImplant: return GetLocalizedText(2073124879204363,language);
                case SlotType.EquipHumanLeg: return GetLocalizedText(2073124879204356,language);
                case SlotType.EquipDroidUpper: return GetLocalizedText(2073124879204390,language);
                case SlotType.EquipDroidLower: return GetLocalizedText(2073124879204392,language);
                case SlotType.EquipDroidUtility: return GetLocalizedText(2073124879204394,language);
                case SlotType.EquipDroidSensor: return GetLocalizedText(2073124879204388,language);
                case SlotType.EquipHumanHeirloom: return GetLocalizedText(2073124879204364,language);
                case SlotType.EquipHumanRangedPrimary: return GetLocalizedText(2073124879204365,language);
                case SlotType.EquipHumanRangedSecondary: return GetLocalizedText(2073124879204375,language);
                case SlotType.EquipHumanCustomRanged: return GetLocalizedText(2073124879204366,language);
                case SlotType.EquipHumanCustomMelee: return GetLocalizedText(2073124879204367,language);
                case SlotType.EquipHumanShield: return GetLocalizedText(2073124879204358,language);
                case SlotType.EquipHumanOutfit: return GetLocalizedText(2073124879204369,language);
                case SlotType.EquipDroidLeg: return GetLocalizedText(2073124879204370,language);
                case SlotType.EquipDroidFeet: return GetLocalizedText(2073124879204371,language);
                case SlotType.EquipDroidOutfit: return GetLocalizedText(2073124879204389,language);
                case SlotType.EquipDroidChest: return GetLocalizedText(2073124879204373,language);
                case SlotType.EquipDroidHand: return GetLocalizedText(2073124879204374,language);
                case SlotType.EquipHumanRelic: return GetLocalizedText(2073124879204377,language);
                case SlotType.EquipHumanFocus: return GetLocalizedText(2073124879204368,language);
                case SlotType.EquipSpaceShipArmor: return GetLocalizedText(2073124879204381,language);
                case SlotType.EquipSpaceBeamGenerator: return GetLocalizedText(2073124879204382,language);
                case SlotType.EquipSpaceBeamCharger: return GetLocalizedText(2073124879204383,language);
                case SlotType.EquipSpaceEnergyShield: return GetLocalizedText(2073124879204384,language);
                case SlotType.EquipSpaceShieldRegenerator: return GetLocalizedText(2073124879204385,language);
                case SlotType.EquipSpaceMissileMagazine: return GetLocalizedText(2073124879204386,language);
                case SlotType.EquipSpaceProtonTorpedoes: return GetLocalizedText(2073124879204387,language);
                case SlotType.EquipSpaceAbilityDefense: return GetLocalizedText(2073124879204651,language);
                case SlotType.EquipSpaceAbilityOffense: return GetLocalizedText(2073124879204652,language);
                case SlotType.EquipSpaceAbilitySystems: return GetLocalizedText(2073124879204653,language);
                case SlotType.Any: return null;

                case SlotType.EquipDroidShield:
                case SlotType.EquipDroidGyro:
                case SlotType.EquipDroidSpecial:
                case SlotType.EquipDroidWeapon1:
                case SlotType.EquipDroidWeapon2:
                case SlotType.Upgrade:
                case SlotType.EquipHumanRanged:
                case SlotType.EquipHumanRangedTertiary:
                case SlotType.EquipHumanLightSide:
                case SlotType.EquipHumanDarkSide:
                case SlotType.EquipSpaceShipAbilityDefense:
                case SlotType.EquipSpaceShipAbilityOffense:
                case SlotType.EquipSpaceShipAbilitySystems:
                    return slot.ToString();
                default:
                    return "";
            }
        }
        public static string ConvertToString(this Profession crewSkillId)
        {
            string profession = GetLocalizedText((int)crewSkillId + 836161413054464);
            return profession;
        }
        
        #endregion
        #endregion
    }
}