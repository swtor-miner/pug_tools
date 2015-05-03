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
            if (obj == null)
                return null;
            if (obj.GetType() == typeof(Item))
            {
                return ((Item)obj).GetHTML().ToString(SaveOptions.DisableFormatting);
            }
            if (obj.GetType() == typeof(Schematic))
            {
                return ((Schematic)obj).GetHTML().ToString(SaveOptions.DisableFormatting);
            }
            if (obj.GetType() == typeof(Ability))
            {
                return ((Ability)obj).GetHTML().ToString(SaveOptions.DisableFormatting);
            }
            return "<div>Not implemented</div>";
        }
    }
    public static class TooltipHelpers
    {
        #region item
        public static XElement GetHTML(this Item itm) //Behold linq!
        {
            if (itm.Id == 0) return new XElement("div", "Not Found");

            XElement tooltip = new XElement("div", new XAttribute("class", "torctip_wrapper"));
            var fileId = TorLib.FileId.FromFilePath(String.Format("/resources/gfx/icons/{0}.dds", itm.Icon));

            if (itm != null)
            {
                string stringQual = ((itm.IsModdable && (itm.Quality == ItemQuality.Prototype)) ? "moddable" : itm.Quality.ToString().ToLower());
                tooltip.Add(new XElement("div",
                    new XAttribute("class", String.Format("torctip_image torctip_image_{0}", stringQual)),
                    new XElement("img",
                        new XAttribute("src", String.Format("http://torcommunity.com/db/icons/{0}_{1}.png", fileId.ph, fileId.sh)),
                        new XAttribute("alt", ""))),
                    new XElement("div",
                        new XAttribute("class", "torctip_name"),
                        new XElement("a",
                            new XAttribute("href", String.Format("http://torcommunity.com/database/item/{0}", itm.Base62Id)),
                            new XAttribute("data-torc", "norestyle"),
                            itm.Name
                            )),
                    itm.ItemInnerHTML()
                );

            }

            return tooltip;
        }
        public static XElement ItemInnerHTML(this Item itm)
        {
            string stringQual = ((itm.IsModdable && (itm.Quality == ItemQuality.Prototype)) ? "moddable" : itm.Quality.ToString().ToLower());
            XElement tooltip = new XElement("div",
                XClass("torctip_tooltip"),
                new XElement("span",
                    XClass(String.Format("torctip_{0}", stringQual)),
                    itm.Name)
                );
            //MTX
            if (itm.IsMtxItem)
            {
                tooltip.Add(new XElement("div",
                    XClass("torctip_val"),
                    "Cartel Market Item"
                    ));
            }
            //binding
            if (itm.Binding != 0)
            {
                tooltip.Add(new XElement("div",
                    XClass("torctip_main"),
                    String.Format("Binds on {0}", itm.Binding.ToString())
                    ));
            }
            //unique
            if (itm.UniqueLimit > 0)
            {
                tooltip.Add(new XElement("div",
                    XClass("torctip_main"),
                    "Unique"
                    ));
            }
            //slot
            bool isEquipable = false;
            if (itm.Slots.Count > 1) //the Any slot was removed from the item by the itemloader
            {
                tooltip.Add(new XElement("div",
                    XClass("torctip_main"),
                    String.Join(", ", itm.Slots.Select(x => x.ConvertToString()).Where(x => x != null).ToList())
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
                tooltip.Add(new XElement("div",
                    XClass("torctip_main"),
                    new XElement("span",
                        XClass("torctip_minDam"),
                        min.ToString("0.0")),
                    "-",
                    new XElement("span",
                        XClass("torctip_maxDam"),
                        max.ToString("0.0")),
                    String.Format(" {0} Damage (Rating {1})", itm.WeaponSpec.DamageType, itm.CombinedRating)
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
                            String.Format("Shield Absorb: {0}%", (absorbchance * 100).ToString("n1"))
                            ));
                    if (shieldchance > 0)
                        tooltip.Add(new XElement("div",
                            XClass("torctip_main"),
                            String.Format("Shield Chance: {0}%", (shieldchance * 100).ToString("n1"))
                            ));
                    tooltip.Add(new XElement("div",
                        XClass("torctip_main"),
                        String.Format("{0} (Rating {1})", shield.Name, itm.CombinedRating)
                        ));
                }
                ArmorSpec arm = itm.ArmorSpec;
                if (arm != null)
                {
                    if (arm.DebugSpecName == "adaptive")
                        tooltip.Add(new XElement("div",
                            XClass("torctip_main"),
                            String.Format("Adaptive Armor (Rating {0})", itm.CombinedRating)
                            ));
                    else
                    {
                        var temp = itm.EnhancementSlots.Where(x => x.Slot == EnhancementType.Harness);
                        if (temp.Count() != 0)
                            if (temp.First().ModificationId != 0)
                                level = temp.First().Modification.ItemLevel;
                        int armor = itm._dom.data.armorPerLevel.GetArmor(arm, level, itm.Quality, itm.Slots.Where(x => x != SlotType.Any).First());
                        if (armor > 0)
                            tooltip.Add(new XElement("div",
                                XClass("torctip_main"),
                                String.Format("{0} Armor (Rating {1})", armor, itm.CombinedRating)
                                ));
                    }
                }
                else if (itm.CombinedRating != 0)
                    tooltip.Add(new XElement("div",
                        XClass("torctip_main"),
                        String.Format("Item Rating {0}", itm.CombinedRating)
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
                    String.Format("Durability: {0}/{0}", itm.MaxDurability)
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
                        "Total Stats:")
                    );
                if (!isEquipable)
                {
                    stats.Add(new XElement("div",
                       XClass("torctip_stat"),
                       String.Format("Armor Rating {0}", itm.CombinedRating)
                       ));
                }
                for (var i = 0; i < itm.CombinedStatModifiers.Count; i++)
                {
                    stats.Add(new XElement("div",
                        XClass("torctip_stat"),
                        //new XAttribute("id", String.Format("torctip_stat_{0}", i)),
                        String.Format("+{0} {1}", itm.CombinedStatModifiers[i].Modifier, itm.CombinedStatModifiers[i].Stat.ConvertToString())));
                }
                if (techpower > 0)
                    stats.Add(new XElement("div",
                        XClass("torctip_stat"),
                        //new XAttribute("id", "torctip_stat_tech"),
                        String.Format("+{0} Tech Power", techpower)));
                if (forcepower > 0)
                    stats.Add(new XElement("div",
                        XClass("torctip_stat"),
                        //new XAttribute("id", "torctip_stat_force"),
                        String.Format("+{0} Force Power", forcepower)));
                tooltip.Add(stats);
            }
            //Modifications

            if (itm.EnhancementSlots.Count != 0)
            {
                XElement enhance = new XElement("div",
                    XClass("torctip_mods"),
                    new XElement("span",
                        XClass("torctip_white"),
                        "Item Modifications:")
                    );

                Dictionary<string, XElement> enhancements = new Dictionary<string, XElement>();
                for (var i = 0; i < itm.EnhancementSlots.Count; i++)
                {
                    enhancements.Add(itm.EnhancementSlots[i].Slot.ConvertToString(), itm.EnhancementSlots[i].ToHTML());
                }
                List<string> sortOrder = new List<string>
                    {
                        "Color Crystal",
                        "Armoring",
                        "Barrel",
                        "Hilt",
                        "Modification",
                        "Enhancement",
                        "Dye"
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
                enhance.Add(new XElement("div",
                    XClass("torctip_mod"),
                    new XElement("div",
                        XClass("torctip_mslot"),
                        "Augment: Open")
                    ));
                tooltip.Add(enhance);
            }

            //requirements
            if (itm.RequiredLevel != 0)
                tooltip.Add(new XElement("div", XClass("torctip_main"), String.Format("Requires Level {0}", itm.RequiredLevel)));
            if (itm.RequiredClasses.Count != 0)
                tooltip.Add(new XElement("div", XClass("torctip_main"), String.Format("Requires {0}", String.Join(",",itm.RequiredClasses.Select(x => x.Name)))));
            if (itm.ArmorSpec != null)
                tooltip.Add(new XElement("div", XClass("torctip_main"), String.Format("Requires {0}", itm.ArmorSpec.Name)));
            if (itm.WeaponSpec != null)
                tooltip.Add(new XElement("div", XClass("torctip_main"), String.Format("Requires {0}", itm.WeaponSpec.Name)));
            if (itm.RequiredGender != Gender.None)
                tooltip.Add(new XElement("div", XClass("torctip_main"), String.Format("{0} Clothing", itm.RequiredGender.ToString())));
            if (itm.RequiredProfession != Profession.None)
                tooltip.Add(new XElement("div", XClass("torctip_main"), String.Format("Requires {0} ({1})", itm.RequiredProfession.ConvertToString(), itm.RequiredProfessionLevel)));
            if (itm.RequiredValorRank > 0)
            {
                tooltip.Add(new XElement("div",
                    XClass("torctip_main"),
                    String.Format("Requires Valor Rank ({0})", itm.RequiredValorRank)
                    ));
            }
            if (itm.RequiresAlignment)
            {
                string alignment = "";
                string reqAlignType = (itm.RequiredAlignmentInverted) ? " or below" : "or above";
                string tier = "";
                switch(Math.Sign(itm.RequiredAlignmentTier)){
                    case -1:
                        alignment = "Dark";
                        tier = String.Format("{0} ", (-itm.RequiredAlignmentTier).ToRoman());
                        break;
                    case 1:
                        alignment = "Light";
                        tier = String.Format("{0} ", itm.RequiredAlignmentTier.ToRoman());
                        break;
                    default:
                        alignment = "Neutral";
                        break;
                }
                
                tooltip.Add(new XElement("div",
                    XClass("torctip_main"),
                    String.Format("Requires {0} {1}{2}", alignment, tier, reqAlignType)
                    ));
            }
            if (itm.RequiresSocial)
            {
                tooltip.Add(new XElement("div",
                    XClass("torctip_main"),
                    String.Format("Requires Social {0} or above", itm.RequiredSocialTier.ToRoman())
                    ));
            }

            if (itm.RequiredReputationId != 0)
            {
                tooltip.Add(new XElement("div",
                    XClass("torctip_main"),
                    String.Format("Requires {1} standing with {0}", itm.RequiredReputationName, itm.RequiredReputationLevelName)
                    ));
            }
            //decoration before abilities
            if (itm.IsDecoration && itm.Decoration != null)
            {
                tooltip.Add(
                    new XElement("div",
                        XClass("torctip_main"),
                        new XElement("span",
                            XClass("torctip_main"),
                            "Stronghold Decoration: "),
                        new XElement("span",
                            XClass("torctip_val"),
                            String.Format("{0} - {1}", itm.Decoration.CategoryName, itm.Decoration.SubCategoryName))
                    ),
                    new XElement("div",
                        XClass("torctip_main"),
                        new XElement("span",
                            XClass("torctip_main"),
                            "Hook Type: "),
                        new XElement("span",
                            XClass("torctip_val"),
                            String.Join(", ", itm.Decoration.AvailableHooks))
                    ),
                    new XElement("div",
                        XClass("torctip_main"),
                        new XElement("span",
                            XClass("torctip_main"),
                            "You own: "),
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
                            "Equip: ");
                        if (ablDesc.Contains('\n'))
                        {
                            var splits = ablDesc.Split('\n');
                            foreach (var split in splits)
                            {
                                desc.Add(split, new XElement("br"));
                            }
                        }
                        else
                        {
                            desc.Add(ablDesc);
                        }
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
                                            String.Format("Requires {0} ({1})", itm.Schematic.CrewSkillId.ToString(), itm.Schematic.SkillOrange),
                                            ItemInnerHTML(itm.Schematic.Item))
                                        ));
                                }
                                else if (itm.Schematic.MissionDescription != "")
                                {                                       //add href here when schematics ready
                                    tooltip.Add(new XElement("div",
                                        XClass("torctip_mission"),
                                        new XElement("div",
                                            XClass("torctip_main"),
                                            String.Format("Requires {0} ({1})", itm.Schematic.CrewSkillId.ToString(), itm.Schematic.SkillOrange)),
                                        new XElement("div",
                                            XClass("torctip_mission_name"),
                                            itm.Schematic.Name),
                                        new XElement("div",
                                            XClass("torctip_mission_yield"),
                                            itm.Schematic.MissionYieldDescription)
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
                                if (itm.UseAbility.Fqn.StartsWith("abl.player.") && String.IsNullOrEmpty(itm.UseAbility.Description)){
                                    string puasehere = "";
                                }
                            string ablDesc = itm.UseAbility.ParsedDescription ?? "";
                            if (ablDesc == "") break;
                            ablDesc = System.Text.RegularExpressions.Regex.Replace(ablDesc, @"\r\n?|\n", "<br />");
                            tooltip.Add(new XElement("div",
                                XClass("torctip_use"),
                                String.Format("Use: {0}", ablDesc)
                                ));
                            break;
                    }
                }
            }
            //Decoration Source
            if (itm.IsDecoration || itm.StrongholdSourceList.Count > 0)
            {
                if (!itm.IsDecoration) {
                    string pusfgs = "";
                }
                tooltip.Add(new XElement("br"));
                foreach (var kvp in itm.StrongholdSourceNameDict)
                {
                    tooltip.Add(new XElement("div",
                        XClass("torctip_val"),
                        String.Format("Source: {0}", kvp.Value)
                    ));
                }
            }
            if (itm.Description != "")
            {
                string itmDesc = itm.Description;
                //regex_newline.Replace(itmDesc, "<br />");
                itmDesc = System.Text.RegularExpressions.Regex.Replace(itmDesc, @"\r\n?|\n", "\n");
                //itmDesc = itmDesc.Replace("<br />", "\n");
                XElement desc = new XElement("div",
                    XClass("torctip_desc"));
                if (itmDesc.Contains('\n'))
                {
                    var splits = itmDesc.Split('\n');
                    foreach (var split in splits)
                    {
                        desc.Add(split, new XElement("br"));
                    }
                }
                else
                {
                    desc.Add(itmDesc);
                }
                tooltip.Add(desc);
            }
            if (itm.SetBonusId != 0)
            {
                tooltip.Add(itm.SetBonus.ToHTML());
            }
            return tooltip;
        }
        private static XElement ToHTML(this ItemEnhancement itm)
        {
            string slot = itm.Slot.ConvertToString();
            //StringBuilder enhancement = new StringBuilder();
            XElement enhancement = new XElement("div",
                XClass("torctip_mod")
                );
            if (itm.ModificationId != 0)
            {
                //if (itm.Slot == EnhancementType.ColorCrystal)
                slot = itm.Modification.Name;
                enhancement.Add(new XElement("div",
                    XClass("torctip_mslot"),
                    new XElement("a",
                        XClass(String.Format("torctip_{0}", itm.Modification.Quality.ToString())),
                        new XAttribute("href", String.Format("http://torcommunity.com/database/item/{0}/{1}/", itm.Modification.Base62Id, itm.Modification.Name.LinkString())),
                        new XAttribute("data-torc", "norestyle"),
                        String.Format("{0} ({1})", slot, itm.Modification.Rating.ToString()))
                    ));
                for (var e = 0; e < itm.Modification.CombinedStatModifiers.Count; e++)
                {
                    enhancement.Add(new XElement("div",
                        XClass("torctip_mstat"),
                        String.Format("+{0} {1}", itm.Modification.CombinedStatModifiers[e].Modifier, itm.Modification.CombinedStatModifiers[e].Stat.ConvertToString())
                        ));
                }
            }
            else
                //empty mod
                enhancement.Add(new XElement("div",
                        XClass("torctip_mslot"),
                        String.Format("{0}: Open", slot)
                    ));
            return enhancement;
        }
        private static XElement ToHTML(this SetBonusEntry itm)
        {
            if (!String.IsNullOrEmpty(itm.Name))
            {
                XElement enhancement = new XElement("div",
                    XClass("torctip_set_wrapper"),
                    new XElement("div",
                        XClass("torctip_set_name"),
                        new XElement("span", String.Format("{0} (", itm.Name)),
                        new XElement("span", new XAttribute("id", "set_count"), 1),
                        new XElement("span", String.Format("/{0})", itm.MaxItemCount))
                    ));

                //add item list here eventually
                foreach(var kvp in itm.BonusAbilityByNum)
                {
                    enhancement.Add(new XElement("div",
                        XClass("torctip_set_bonus"),
                        String.Format("({0}) {1}", kvp.Key, kvp.Value.ParsedDescription)
                        ));
                }
                return enhancement;
            }
            else
                return null;
        }
        #endregion

        #region schematic
        public static XElement GetHTML(this Schematic itm)
        {
            if (itm.Id == 0) return new XElement("div", "Not Found");
            string icon = "none";
            string stringQual = "none";
            if (itm.Item != null)
            {
                stringQual = ((itm.Item.IsModdable && (itm.Item.Quality == ItemQuality.Prototype)) ? "moddable" : itm.Item.Quality.ToString().ToLower());
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
                        new XAttribute("src", String.Format("http://torcommunity.com/db/icons/{0}_{1}.png", fileId.ph, fileId.sh)),
                        new XAttribute("alt", ""))),
                    new XElement("div",
                        new XAttribute("class", "torctip_name"),
                        new XElement("a",
                            new XAttribute("href", String.Format("http://torcommunity.com/database/item/{0}/{1}/", itm.Base62Id, itm.Name.LinkString())),
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
                            var matstringQual = ((mat.IsModdable && (mat.Quality == ItemQuality.Prototype)) ? "moddable" : mat.Quality.ToString().ToLower());
                            var matfileId = TorLib.FileId.FromFilePath(String.Format("/resources/gfx/icons/{0}.dds", mat.Icon));
                            components.Add(new XElement("div",
                                XClass("torctip_mat"),
                                new XElement("span", String.Format("{0}x ", kvp.Value)),
                                new XElement("div",
                                    new XAttribute("class", String.Format("torctip_image torctip_image_{0} small_border", matstringQual)),
                                    new XElement("img",
                                        new XAttribute("src", String.Format("http://torcommunity.com/db/icons/{0}_{1}.png", matfileId.ph, matfileId.sh)),
                                        new XAttribute("alt", itm.Name),
                                        XClass("small_image"))),
                                new XElement("div",
                                    new XAttribute("class", "torctip_mat_name"),
                                    new XElement("a",
                                        XClass(String.Format("torctip_{0}", matstringQual)),
                                        new XAttribute("href", String.Format("http://torcommunity.com/database/item/{0}/{1}/", mat.Base62Id, LinkString(mat.Name))),
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
                        new XAttribute("src", String.Format("http://torcommunity.com/db/icons/{0}_{1}.png", fileId.ph, fileId.sh)),
                        new XAttribute("alt", ""))),
                    new XElement("div",
                        new XAttribute("class", "torctip_name"),
                        new XElement("a",
                            new XAttribute("href", String.Format("http://torcommunity.com/database/ability/{0}/{1}/", itm.Base62Id, itm.Name.LinkString())),
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
                    cast.Add(new XElement("span",
                            XClass("torctip_stat"),
                            "Activation: "),
                        new XElement("span",
                            XClass("torctip_white"),
                            String.Format("{0}s", itm.CastingTime)
                        )
                    );
                }
                else if(itm.ChannelingTime > 0){
                        cast.Add(new XElement("span",
                            XClass("torctip_stat"),
                            "Channeled: "),
                        new XElement("span",
                            XClass("torctip_white"),
                            String.Format("{0}s", itm.ChannelingTime)
                        )
                    );
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
            if (number >= 900) return "CM" + ToRoman(number - 900); //EDIT: i've typed 400 instead 900
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
        private static XAttribute XClass(string classname)
        {
            return new XAttribute("class", classname);
        }
        private static XElement XStat(string text, string value)
        {
            return new XElement("div",
                new XElement("span",
                    XClass("torctip_stat"),
                    text),
                new XElement("span",
                    XClass("torctip_white"),
                    value)
                );
        }
        #region ConvertToString
        public static string ConvertToString(this SlotType slot) //replace these with friendly names
        {
            switch (slot)
            {
                case SlotType.EquipHumanMainHand: return "Main Hand (Melee)";
                case SlotType.EquipHumanOffHand: return "Off-Hand (Melee)";
                case SlotType.EquipHumanWrist: return "Wrist";
                case SlotType.EquipHumanBelt: return "Waist";
                case SlotType.EquipHumanChest: return "Chest";
                case SlotType.EquipHumanEar: return "Ear";
                case SlotType.EquipHumanFace: return "Head";
                case SlotType.EquipHumanFoot: return "Feet";
                case SlotType.EquipHumanGlove: return "Hands";
                case SlotType.EquipHumanImplant: return "Implant";
                case SlotType.EquipHumanLeg: return "Legs";
                case SlotType.EquipDroidUpper: return "Core";
                case SlotType.EquipDroidLower: return "Motor";
                case SlotType.EquipDroidShield: return "DroidShield";
                case SlotType.EquipDroidGyro: return "DroidGyro";
                case SlotType.EquipDroidUtility: return "Parts";
                case SlotType.EquipDroidSensor: return "Sensor Unit";
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
                case SlotType.EquipHumanShield: return "Offhand (General)";
                case SlotType.EquipHumanOutfit: return "Customization";
                case SlotType.EquipDroidLeg: return "DroidLeg";
                case SlotType.EquipDroidFeet: return "DroidFeet";
                case SlotType.EquipDroidOutfit: return "DroidOutfit";
                case SlotType.EquipDroidChest: return "DroidChest";
                case SlotType.EquipDroidHand: return "DroidHand";
                case SlotType.EquipHumanLightSide: return "LightSide";
                case SlotType.EquipHumanDarkSide: return "DarkSide";
                case SlotType.EquipHumanRelic: return "Relic";
                case SlotType.EquipHumanFocus: return "Offhand (General)";
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
        public static string ConvertToString(this Stat slot) //replace these with friendly names
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
                case Stat.GlanceRating: return "Shield Rating";
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
                case Stat.DefenseRating: return "Defense Rating";
                case Stat.EffectGenerateHeatModifierFixed: return "debug_EffectGenerateHeatModifierFixed";
                case Stat.EffectGenerateHeatModifierPercentage: return "debug_EffectGenerateHeatModifierPercentage";
                case Stat.ArmorPenetration: return "debug_ArmorPenetration";
                case Stat.Artifice: return "Artifice";
                case Stat.Armormech: return "Armormech";
                case Stat.Armstech: return "Armstech";
                case Stat.Biochem: return "Biochem";
                case Stat.Cybertech: return "Cybertech";
                case Stat.Synthweaving: return "Synthweaving";
                case Stat.Archaeology: return "Archaeology";
                case Stat.Bioanalysis: return "Bioanalysis";
                case Stat.Scavenging: return "Scavenging";
                case Stat.Slicing: return "Slicing";
                case Stat.Diplomacy: return "Diplomacy";
                case Stat.Research: return "Research";
                case Stat.TreasureHunting: return "TreasureHunting";
                case Stat.UnderworldTrading: return "UnderworldTrading";
                case Stat.Crafting: return "Crafting";
                case Stat.Harvesting: return "Harvesting";
                case Stat.Mission: return "Mission";
                case Stat.ArtificeEfficiency: return "Artifice Efficiency";
                case Stat.ArmormechEfficiency: return "Armormech Efficiency";
                case Stat.ArmstechEfficiency: return "Armstech Efficiency";
                case Stat.BiochemEfficiency: return "Biochem Efficiency";
                case Stat.CybertechEfficiency: return "Cybertech Efficiency";
                case Stat.SynthweavingEfficiency: return "Synthweaving Efficiency";
                case Stat.ArchaeologyEfficiency: return "Archaeology Efficiency";
                case Stat.BioanalysisEfficiency: return "Bioanalysis Efficiency";
                case Stat.ScavengingEfficiency: return "Scavenging Efficiency";
                case Stat.SlicingEfficiency: return "Slicing Efficiency";
                case Stat.DiplomacyEfficiency: return "Diplomacy  Efficiency";
                case Stat.ResearchEfficiency: return "Research Efficiency";
                case Stat.TreasureHuntingEfficiency: return "TreasureHunting Efficiency";
                case Stat.UnderworldTradingEfficiency: return "UnderworldTrading Efficiency";
                case Stat.CraftingEfficiency: return "Crafting Efficiency";
                case Stat.HarvestingEfficiency: return "Harvesting Efficiency";
                case Stat.MissionEfficiency: return "Mission Efficiency";
                case Stat.ArtificeCritical: return "Artifice Critical";
                case Stat.ArmormechCritical: return "Armormech Critical";
                case Stat.ArmstechCritical: return "Armstech Critical";
                case Stat.BiochemCritical: return "Biochem Critical";
                case Stat.CybertechCritical: return "Cybertech Critical";
                case Stat.SynthweavingCritical: return "Synthweaving Critical";
                case Stat.ArchaeologyCritical: return "Archaeology Critical";
                case Stat.BioanalysisCritical: return "Bioanalysis Critical";
                case Stat.ScavengingCritical: return "Scavenging Critical";
                case Stat.SlicingCritical: return "Slicing Critical";
                case Stat.DiplomacyCritical: return "Diplomacy  Critical";
                case Stat.ResearchCritical: return "Research Critical";
                case Stat.TreasureHuntingCritical: return "TreasureHunting Critical";
                case Stat.UnderworldTradingCritical: return "UnderworldTrading Critical";
                case Stat.CraftingCritical: return "Crafting Critical";
                case Stat.HarvestingCritical: return "Harvesting Critical";
                case Stat.MissionCritical: return "Mission Critical";
                case Stat.TechHealingPower: return "Tech Healing Power";
                case Stat.ForceHealingPower: return "Force Healing Power";
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
                case Stat.ShipArmor: return "Ship Armor";
                case Stat.ShipRateOfFire: return "Ship Blaster ROF";
                case Stat.ShipBlasterDamage: return "Ship Blaster Damage";
                case Stat.ShipMissileCount: return "Ship Missile Count";
                case Stat.ShipMissileLevel: return "Ship Missile Level";
                case Stat.ShipMissileRateOfFire: return "Ship Missile ROF";
                case Stat.ShipTorpedoCount: return "Ship Torpedo Count";
                case Stat.ShipTorpedoLevel: return "Ship Torpedo Level";
                case Stat.ShipTorpedoRateOfFire: return "Ship Torpedo ROF";
                case Stat.ShipShieldStrength: return "Ship Shield Strength";
                case Stat.ShipShieldRegen: return "Ship Shield Regen";
                case Stat.ShipShieldCooldown: return "Ship Shield Cooldown";
                case Stat.ShipType: return "Ship Type";
                case Stat.PushbackModifier: return "debug_PushbackModifier";
                case Stat.TargetOnFireDamageModifier: return "debug_TargetOnFireDamageModifier";
                case Stat.SurgeRating: return "Surge Rating";
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
        public static string ConvertToString(this EnhancementType enhancement) //replace these with friendly names
        {
            switch (enhancement)
            {
                case EnhancementType.None: return "None";
                case EnhancementType.Hilt: return "Hilt";
                case EnhancementType.PowerCell: return "debug_PowerCell";
                case EnhancementType.Harness: return "Armoring";
                case EnhancementType.Overlay: return "Modification";
                case EnhancementType.Underlay: return "debug_Underlay";
                case EnhancementType.Support: return "Enhancement";
                case EnhancementType.FocusLens: return "debug_FocusLens";
                case EnhancementType.Trigger: return "debug_Trigger";
                case EnhancementType.Barrel: return "Barrel";
                case EnhancementType.PowerCrystal: return "Hilt"; //lightsaber hilt
                case EnhancementType.Scope: return "debug_Scope";
                case EnhancementType.Circuitry: return "debug_Circuitry";
                case EnhancementType.EmitterMatrix: return "debug_EmitterMatrix";
                case EnhancementType.ColorCrystal: return "Color Crystal";
                case EnhancementType.ColorCartridge: return "debug_ColorCartridge";
                case EnhancementType.Modulator: return "debug_Modulator";
                case EnhancementType.Dye: return "Dye";
                default: return "Unknown";
            }
        }
        public static string ConvertToString(this Profession crewSkillId)
        {
            switch (crewSkillId)
            {
                case Profession.Archaeology: return "Archaeology";
                case Profession.Bioanalysis: return "Bioanalysis";
                case Profession.Scavenging: return "Scavenging";
                case Profession.Artifice: return "Artifice";
                case Profession.Armormech: return "Armormech";
                case Profession.Armstech: return "Armstech";
                case Profession.Biochem: return "Biochem";
                case Profession.Cybertech: return "Cybertech";
                case Profession.Synthweaving: return "Synthweaving";
                case Profession.Slicing: return "Slicing";
                case Profession.Diplomacy: return "Diplomacy";
                case Profession.Investigation: return "Investigation";
                case Profession.TreasureHunting: return "Treasure Hunting";
                case Profession.UnderworldTrading: return "Underworld Trading";
                default: return "None";
            }
        }
        #endregion
    }
}