using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GomLib.Models;
using System.Xml.Linq;

namespace GomLib.ModelLoader
{
    public class ItemLoader : IModelLoader
    {
        const long NameLookupKey = -2761358831308646330;
        const long DescLookupKey = 2806211896052149513;
        Dictionary<object, object> itemAppearances;
        Dictionary<long, string> itemAppearanceAssets;
        Dictionary<ulong, HashSet<ulong>> questRewardRefs;
        public Dictionary<ulong, List<ulong>> schematicLookupMap;
        Dictionary<ulong, ulong> childLookupMap;
        HashSet<long> authFlags;
        readonly DataObjectModel _dom;

        public ItemLoader(DataObjectModel dom)
        {
            _dom = dom;
            Flush();
        }

        public void Flush()
        {
            itemAppearances = new Dictionary<object, object>();
            itemAppearanceAssets = new Dictionary<long, string>();
            questRewardRefs = new Dictionary<ulong, HashSet<ulong>>();
            schematicLookupMap = new Dictionary<ulong, List<ulong>>();
            childLookupMap = new Dictionary<ulong, ulong>();
            authFlags = new HashSet<long>();
        }

        public string ClassName
        {
            get { return "itmItem"; }
        }

        public GameObject CreateObject()
        {
            return new Item();
        }

        public Item Load(GameObject obj, GomObject gom)
        {
            if (gom == null) { return (Item)obj; }
            if (gom == null) { return null; }

            if (itemAppearances.Count == 0)
            {
                var itmAppearanceDatatable = _dom.GetObject("itmAppearanceDatatable");
                itemAppearances = itmAppearanceDatatable.Data.Get<Dictionary<object, object>>("itmAppearances");
                itmAppearanceDatatable.Unload();
            }
            if (questRewardRefs.Count == 0)
            {
                var proto = _dom.GetObject("qstRewardsInfoPrototype");
                if (proto != null)
                {
                    Dictionary<object, object> table;
                    if (proto.Data.ContainsKey("qstRewardsNewInfoData"))
                    {
                        table = proto.Data.ValueOrDefault<Dictionary<object, object>>("qstRewardsNewInfoData", null);
                        if (proto.Data.ContainsKey("qstRewardsInfoData"))
                        {
                            foreach (var kvp in table = proto.Data.ValueOrDefault("qstRewardsInfoData", new Dictionary<object, object>()))
                            {
                                if (!table.ContainsKey(kvp.Key))
                                    table.Add(kvp.Key, kvp.Value);
                            }
                        }
                    }
                    else
                    {
                        table = proto.Data.ValueOrDefault<Dictionary<object, object>>("qstRewardsInfoData", null);
                    }
                    if (table != null)
                    {
                        foreach (var kvp in table)
                        {
                            ulong qstId = (ulong)kvp.Key;
                            List<GomObjectData> qRewards = ((List<object>)kvp.Value).ConvertAll(x => (GomObjectData)x);
                            foreach (GomObjectData qReward in qRewards)
                            {
                                GomObjectData rewardLookup = qReward.ValueOrDefault<GomObjectData>("qstRewardData", null);
                                if (rewardLookup == null)
                                {
                                    rewardLookup = qReward;
                                    //continue;
                                }
                                ulong rewardItemId = rewardLookup.ValueOrDefault<ulong>("qstRewardItemId", 0);
                                if (!questRewardRefs.ContainsKey(rewardItemId))
                                {
                                    HashSet<ulong> l = new HashSet<ulong>
                                    {
                                        qstId
                                    };
                                    questRewardRefs.Add(rewardItemId, l);
                                }
                                else
                                {
                                    questRewardRefs[rewardItemId].Add(qstId);
                                }
                            }
                        }
                    }
                }
            }
            var itm = (Item)obj;

            //HashSet<ulong> qRList;
            //if (questRewardRefs.TryGetValue(gom.Id, out qRList))
            //    itm.rewardedForQuests = qRList.ToList();

            itm.ShortId = gom.Id;
            itm.Fqn = gom.Name;
            itm.Dom_ = _dom;
            itm.References = gom.References;
            itm.AppearanceColor = gom.Data.ValueOrDefault<string>("itmEnhancementColor", null);
            if (itm.AppearanceColor != "None")
            {
                // string oshdfoi = "";
            }
            itm.DyeId = (int)gom.Data.ValueOrDefault<long>("itmDyeSlotIdDupe", 0);
            if (itm.DyeId != 0)
            {
                itm.DyeColor = _dom.detailedAppearanceColorLoader.Load(itm.DyeId);
            }

            itm.ArmorSpec = ArmorSpec.Load(_dom, gom.Data.ValueOrDefault<long>("itmArmorSpec", 0));
            itm.Binding = ItemBindingRuleExtensions.ToBindingRule(gom.Data.ValueOrDefault<ScriptEnum>("itmBindingRule", null));

            itm.CombinedStatModifiers = new ItemStatList();
            itm.ConsumedOnUse = gom.Data.ValueOrDefault("itmConsumedOnUse", false);

            itm.SoundType = gom.Data.ValueOrDefault<string>("itmSoundType", null);
            itm.StackCount = gom.Data.ValueOrDefault<long>("itmStackCount", 0);
            itm.MaxDurability = gom.Data.ValueOrDefault<long>("itmMaxDurability", 0);

            //itm.Conversation = new Conversation();
            itm.ConversationFqn = gom.Data.ValueOrDefault<string>("itmConversation", null);

            if (gom.Data.ContainsKey("locTextRetrieverMap"))
            {
                var descLookupData = (GomObjectData)(gom.Data.Get<Dictionary<object, object>>("locTextRetrieverMap")[DescLookupKey]);
                itm.DescriptionId = descLookupData.Get<long>("strLocalizedTextRetrieverStringID");
                itm.LocalizedDescription = _dom.stringTable.TryGetLocalizedStrings(itm.Fqn, descLookupData);
                itm.Description = _dom.stringTable.TryGetString(itm.Fqn, descLookupData);
            }

            itm.DisassembleCategory = ProfessionSubtypeExtensions.ToProfessionSubtype(gom.Data.ValueOrDefault<ScriptEnum>("prfDisassembleCategory", null));
            itm.Durability = (int)gom.Data.ValueOrDefault<long>("itmMaxDurability", 0);
            itm.EnhancementCategory = EnhancementCategoryExtensions.ToEnhancementCategory(gom.Data.ValueOrDefault<ScriptEnum>("itmEnhancementCategory", null));
            itm.EnhancementSubCategory = EnhancementSubCategoryExtensions.ToEnhancementSubCategory(gom.Data.ValueOrDefault<ScriptEnum>("itmEnhancementSubCategory", null));
            long slotId = gom.Data.ValueOrDefault<long>("itmEnhancementType", 0);
            itm.EnhancementType = EnhancementTypeExtensions.ToEnhancementType(slotId);
            itm.DetEnhancementType = _dom.enhanceData.ToEnhancement(slotId);

            // Enhancement Slots
            itm.EnhancementSlots = new ItemEnhancementList();
            var enhancementSlots = gom.Data.ValueOrDefault<List<object>>("itmEnhancementSlots", null);
            var enhancementDefaults = gom.Data.ValueOrDefault<List<object>>("itmEnhancementDefaults", null);
            if (enhancementSlots != null)
            {
                for (var i = 0; i < 5; i++)
                {
                    long slot = (long)enhancementSlots[i];
                    if (slot == 0) { continue; }

                    ItemEnhancement enh = new ItemEnhancement
                    {
                        Slot = EnhancementTypeExtensions.ToEnhancementType(slot),
                        DetailedSlot = _dom.enhanceData.ToEnhancement(slot),
                        ModificationId = (ulong)enhancementDefaults[i]
                    };
                    if (enh.ModificationId != 0)
                    {
                        enh.Modification = _dom.itemLoader.Load(enh.ModificationId);
                    }

                    // Don't add empty modulator/augment slots to gear since for some reason it seems to be on every damn modifiable item
                    if ((enh.Slot == EnhancementType.Modulator) && (enh.Modification == null))
                    {
                        continue;
                    }

                    itm.EnhancementSlots.Add(enh);
                }
            }

            itm.EquipAbilityId = gom.Data.ValueOrDefault<ulong>("itmEquipAbility", 0);
            itm.EquipAbility = _dom.abilityLoader.Load(itm.EquipAbilityId);

            itm.GiftRank = GiftRankExtensions.ToGiftRank(gom.Data.ValueOrDefault<ScriptEnum>("itmGiftAffectionRank", null));
            itm.GiftType = GiftTypeExtensions.ToGiftType(gom.Data.ValueOrDefault<ScriptEnum>("itmGiftType", null));
            if (itm.GiftType != GiftType.None && itm.GiftRank == GiftRank.None)
            {
                itm.GiftRank = GiftRank.Rank1;
            }
            itm.GiftRankNum = (int)itm.GiftRank;
            itm.Icon = gom.Data.ValueOrDefault("itmIcon", string.Empty);
            _dom._assets.icons.Add(itm.Icon);

            //string itmModifierSetID = gom.Data.ValueOrDefault<string>("itmModifierSetID", null);
            //if (!String.IsNullOrWhiteSpace(itmModifierSetID))
            //{
            //    long mId;
            //    Int64.TryParse(itmModifierSetID.Split('#').Last(), out mId);
            //    if(mId!= 0)
            //    {
            //        Dictionary<string, object> dict;
            //        _dom.data.itemModifierPackageTablePrototype.TableData.TryGetValue(mId, out dict);

            //    }
            //}

            if (gom.Data.ValueOrDefault("itmHasAppearanceSlot", false) || gom.Data.ValueOrDefault<ulong>("itmAppearanceSpec", 0) != 0)
            {
                Dictionary<object, object> itmAppearanceSpecByPlayerClass = gom.Data.ValueOrDefault<Dictionary<object, object>>("itmAppearanceSpecByPlayerClass", null);
                if (itmAppearanceSpecByPlayerClass != null)
                {
                    itm.IppRefs = itmAppearanceSpecByPlayerClass.ToDictionary(x => (ulong)x.Key, x => (ulong)x.Value);
                    Dictionary<ulong, string> classNameLookup = new Dictionary<ulong, string>()
                    {
                        { 16140902893827567561, "Warrior" },
                        { 16140912704077491401, "Smuggler" },
                        { 16140943676484767978, "Agent" },
                        { 16140973599688231714, "Trooper" },
                        { 16141010271067846579, "Inquisitor" },
                        { 16141119516274073244, "Knight" },
                        { 16141170711935532310, "Bounty_Hunter" },
                        { 16141179471541245792, "Sage" }
                    };
                    itm.ClassAppearance = new Dictionary<string, string>();
                    foreach (var appearancePair in itmAppearanceSpecByPlayerClass)
                    {
                        GomObject itemAppearance = _dom.GetObject((ulong)appearancePair.Value);
                        if (itemAppearance == null) continue;

                        itm.ClassAppearance.Add(classNameLookup[(ulong)appearancePair.Key], itemAppearance.Name);
                        if (classNameLookup[(ulong)appearancePair.Key] == "Agent")
                        {
                            itm.ImperialIcon = itemAppearance.Name;
                            itm.AppearanceImperial = itemAppearance.Id.ToMaskedBase62();
                            itm.VOModulationImperial = itemAppearance.Data.ValueOrDefault("ippVOSoundTypeOverride", "noModulation");
                            itm.ImperialAppearanceTag = AppearanceTags(itemAppearance);
                            DupeAppReferences(itm, itemAppearance, "similarBaseModel");
                            DupeAppReferences(itm, itemAppearance, "similarAppearance");
                            DupeAppReferences(itm, itemAppearance, "npcSporting", "npcWearingAppearance");
                            ScriptEnum appearanceSlotType = itemAppearance.Data.ValueOrDefault<ScriptEnum>("appAppearanceSlotType", null);
                            if (appearanceSlotType != null)
                            {
                                if (appearanceSlotType.ToString() == "appSlotChest")
                                {
                                    var ipp = _dom.appearanceLoader.LoadIpp(itemAppearance);
                                    itm.RepublicAttachedFX = ipp.GetAttachedFX();
                                }
                            }
                        }
                        if (classNameLookup[(ulong)appearancePair.Key] == "Smuggler")
                        {
                            itm.RepublicIcon = itemAppearance.Name;
                            itm.AppearanceRepublic = itemAppearance.Id.ToMaskedBase62();
                            itm.VOModulationRepublic = itemAppearance.Data.ValueOrDefault("ippVOSoundTypeOverride", "noModulation");
                            itm.RepublicAppearanceTag = AppearanceTags(itemAppearance);
                            DupeAppReferences(itm, itemAppearance, "similarBaseModel");
                            DupeAppReferences(itm, itemAppearance, "similarAppearance");
                            DupeAppReferences(itm, itemAppearance, "npcSporting", "npcWearingAppearance");
                            ScriptEnum appearanceSlotType = itemAppearance.Data.ValueOrDefault<ScriptEnum>("appAppearanceSlotType", null);
                            if (appearanceSlotType != null)
                            {
                                if (appearanceSlotType.ToString() == "appSlotChest")
                                {
                                    var ipp = _dom.appearanceLoader.LoadIpp(itemAppearance);
                                    itm.ImperialAttachedFX = ipp.GetAttachedFX();
                                }
                            }
                        }
                        itemAppearance.Unload();
                    }
                }
                else
                {
                    ulong itmAppearanceSpec = gom.Data.ValueOrDefault<ulong>("itmAppearanceSpec", 0);
                    if (itmAppearanceSpec != 0)
                    {
                        GomObject itemAppearance = _dom.GetObject(itmAppearanceSpec);
                        if (itemAppearance != null)
                        {
                            itm.VOModulationImperial = itemAppearance.Data.ValueOrDefault("ippVOSoundTypeOverride", "noModulation");
                            itm.VOModulationRepublic = itm.VOModulationImperial;
                            itm.ImperialAppearanceTag = AppearanceTags(itemAppearance);
                            itm.RepublicAppearanceTag = itm.ImperialAppearanceTag;
                            DupeAppReferences(itm, itemAppearance, "similarBaseModel");
                            DupeAppReferences(itm, itemAppearance, "similarAppearance");
                            DupeAppReferences(itm, itemAppearance, "npcSporting", "npcWearingAppearance");
                            ScriptEnum appearanceSlotType = itemAppearance.Data.ValueOrDefault<ScriptEnum>("appAppearanceSlotType", null);
                            if (appearanceSlotType != null)
                            {
                                if (appearanceSlotType.ToString() == "appSlotChest")
                                {
                                    var ipp = _dom.appearanceLoader.LoadIpp(itemAppearance);
                                    itm.ImperialAttachedFX = ipp.GetAttachedFX();
                                    itm.RepublicAttachedFX = itm.ImperialAttachedFX;
                                }
                            }
                            itemAppearance.Unload();
                        }
                    }
                }
            }

            itm.ItemLevel = (int)gom.Data.ValueOrDefault<long>("itmBaseLevel", 0);
            itm.MaxStack = (int)gom.Data.ValueOrDefault<long>("itmStackMax", 0);
            itm.ModifierSpec = gom.Data.ValueOrDefault<long>("itmModifierSetID", 0);
            itm.MountSpec = gom.Data.ValueOrDefault<long>("itmMountSpec", 0);

            if (gom.Data.ContainsKey("locTextRetrieverMap"))
            {
                var nameLookupData = (GomObjectData)(gom.Data.Get<Dictionary<object, object>>("locTextRetrieverMap")[NameLookupKey]);
                itm.NameId = nameLookupData.Get<long>("strLocalizedTextRetrieverStringID");
                itm.LocalizedName = _dom.stringTable.TryGetLocalizedStrings(itm.Fqn, nameLookupData);
                //itm.Name = _dom.stringTable.TryGetString(itm.Fqn, nameLookupData);
            }
            itm.LocalizedName = Normalize.Dictionary(itm.LocalizedName, itm.Fqn, true);
            itm.Name = itm.LocalizedName["enMale"];
            itm.Name = itm.Name.Trim();

            itm.ShortId = (ulong)(itm.NameId >> 32);
            itm.Id = gom.Id;

            itm.Quality = ItemQualityExtensions.ToItemQuality(gom.Data.ValueOrDefault<ScriptEnum>("itmBaseQuality", null));
            if ((itm.Quality == ItemQuality.Prototype) && (itm.EnhancementSlots.Count > 1))
                itm.Quality = ItemQuality.Moddable;
            //if (itm.EnhancementType != EnhancementType.None)
            //{
            itm.Rating = _dom.data.itemRating.GetRating(itm.ItemLevel, itm.Quality);
            //}
            //else
            //{
            //itm.Rating = Tables.ItemRating.GetRating(itm.ItemLevel, ItemQuality.Premium);
            //}
            itm.RequiresAlignment = gom.Data.ValueOrDefault("itmAlignmentRequired", false);
            itm.RequiredAlignmentTier = (int)gom.Data.ValueOrDefault<long>("itmRequiredAlignmentTier", 0);
            itm.RequiredAlignmentInverted = gom.Data.ValueOrDefault("itmRequiredAlignmentInverted", false);

            itm.ItmCraftedCategory = gom.Data.ValueOrDefault<long>("itmCraftedCategory", 0);

            // Required Classes
            itm.RequiredClasses = new ClassSpecList();
            var classMap = gom.Data.ValueOrDefault<Dictionary<object, object>>("itmRequiredClasses", null);
            if (classMap != null)
            {
                foreach (var kvp in classMap)
                {
                    if ((bool)kvp.Value)
                    {
                        var classSpec = _dom.classSpecLoader.Load((ulong)kvp.Key);
                        if (classSpec == null) { continue; }
                        itm.RequiredClasses.Add(classSpec);
                    }
                }
            }

            itm.RequiredGender = GenderExtensions.ToGender(gom.Data.ValueOrDefault<ScriptEnum>("itmGenderRequired", null));
            itm.AuctionCategoryId = (int)gom.Data.ValueOrDefault<long>("itmGtnMaincategory", 0);
            itm.AuctionCategory = AuctionCategory.Load(_dom, itm.AuctionCategoryId);
            itm.AuctionSubCategoryId = (int)gom.Data.ValueOrDefault<long>("itmGtnSubcategory", 0);
            itm.AuctionSubCategory = AuctionSubCategory.Load(_dom, itm.AuctionSubCategoryId);
            itm.RequiredLevel = (int)gom.Data.ValueOrDefault<long>("itmMinimumLevel", 0);
            itm.RequiredProfession = ProfessionExtensions.ToProfession(gom.Data.ValueOrDefault<ScriptEnum>("prfProfessionRequired", null));
            itm.RequiredProfessionLevel = (int)gom.Data.ValueOrDefault<long>("prfProfessionLevelRequired", 0);
            itm.RequiresSocial = gom.Data.ValueOrDefault("itmSocialScoreRequired", false);
            itm.RequiredSocialTier = (int)gom.Data.ValueOrDefault<long>("itmRequiredSocialScoreTier", 0);
            itm.RequiredValorRank = (int)gom.Data.ValueOrDefault<long>("itmRequiredValorRank", 0);
            GomObjectData repContainer = gom.Data.ValueOrDefault<GomObjectData>("itmReputationContainer", null);
            if (repContainer != null)
            {
                itm.RequiredReputationId = (int)repContainer.ValueOrDefault<long>("itmRequiredReputations", 0);
                itm.RequiredReputationLevelId = (int)repContainer.ValueOrDefault<long>("itmRequiredReputationLevel", 0);
            }
            itm.RequiredPermission = gom.Data.ValueOrDefault<long>("itmReqPermission", 0);
            itm.ReqArtEquipAuth = itm.RequiredPermission == 6;

            // Item is a recipe/schematic/mount
            object schematic = gom.Data.ValueOrDefault<object>("itmTeaches", null);
            if (schematic != null)
            {
                string teachesType = ((ScriptEnum)schematic).ToString();
                if (teachesType == "itmTeachesSchematic")
                    itm.SchematicId = gom.Data.Get<ulong>("itemTeachesRef");
                else
                {
                    itm.TeachesRef = gom.Data.Get<ulong>("itemTeachesRef");
                    itm.TeachesType = teachesType.Substring(10);
                }
            }

            itm.ShieldSpec = ArmorSpec.Load(_dom, gom.Data.ValueOrDefault<long>("itmShieldSpec", 0));

            // Valid Inventory Slots
            itm.Slots = new SlotTypeList();
            var slotList = gom.Data.ValueOrDefault<List<object>>("itmSlotTypes", null);
            if (slotList != null)
            {
                foreach (var slot in slotList)
                {
                    SlotType slotType = SlotTypeExtensions.ToSlotType((ScriptEnum)slot);
                    if (SlotTypeExtensions.IgnoreSlot(slotType)) continue;
                    itm.Slots.Add(slotType);
                }
            }

            // Item Stats
            var stats = gom.Data.ValueOrDefault<Dictionary<object, object>>("itmEquipModStats", null);
            itm.StatModifiers = new ItemStatList();
            if (stats != null)
            {
                foreach (var kvp in stats)
                {
                    ItemStat itmStat = new ItemStat
                    {
                        Stat = StatExtensions.ToStat((ScriptEnum)kvp.Key),
                        DetailedStat = _dom.statData.ToStat((ScriptEnum)kvp.Key),
                        Modifier = (int)(float)kvp.Value
                    };
                    itm.StatModifiers.Add(itmStat);
                }
            }

            // Combined Stats
            itm.CombinedStatModifiers = new ItemStatList();
            itm.CombinedRequiredLevel = itm.RequiredLevel;
            itm.CombinedRating = 0;
            foreach (var stat in itm.StatModifiers)
            {
                var itmStat = new ItemStat
                {
                    Stat = stat.Stat,
                    DetailedStat = stat.DetailedStat,
                    Modifier = stat.Modifier
                };
                itm.CombinedStatModifiers.Add(itmStat);
            }

            foreach (var mod in itm.EnhancementSlots)
            {
                if (mod.Modification != null)
                {
                    //if (mod.Slot.IsBaseMod())
                    //{
                    if (mod.Modification.RequiredLevel > itm.CombinedRequiredLevel)
                    {
                        itm.CombinedRequiredLevel = mod.Modification.RequiredLevel;
                    }
                    if (mod.Modification.Rating > itm.CombinedRating)
                    {
                        itm.CombinedRating = mod.Modification.Rating;
                    }
                    //}

                    foreach (var stat in mod.Modification.CombinedStatModifiers)
                    {
                        ItemStat itmStat = itm.CombinedStatModifiers.FirstOrDefault(s => s.Stat == stat.Stat);
                        if (itmStat == null)
                        {
                            itmStat = new ItemStat
                            {
                                Stat = stat.Stat,
                                DetailedStat = stat.DetailedStat
                            };
                            itmStat.Modifier += stat.Modifier;
                            itm.CombinedStatModifiers.Add(itmStat);
                        }
                        else
                        {
                            itmStat.Modifier += stat.Modifier;
                        }
                    }
                }
            }
            if (itm.CombinedRating == 0) itm.CombinedRating = itm.Rating;

            //itm.TreasurePackageSpec;
            itm.TreasurePackageId = gom.Data.ValueOrDefault<long>("itmTreasurePackageId", 0);
            itm.TypeBitSet = (int)gom.Data.ValueOrDefault<long>("itmTypeBitSet", 0);
            itm.UniqueLimit = (int)gom.Data.ValueOrDefault<long>("itmUniqueLimit", 0);

            itm.UseAbilityId = gom.Data.ValueOrDefault<ulong>("itmUsageAbility", 0);
            itm.UseAbility = _dom.abilityLoader.Load(itm.UseAbilityId);

            itm.Value = (int)gom.Data.ValueOrDefault<long>("itmValue", 0);
            itm.VendorStackSize = (int)gom.Data.ValueOrDefault<long>("itmStackVendor", 0);
            itm.WeaponSpec = WeaponSpec.Load(_dom, gom.Data.ValueOrDefault<ulong>("cbtWeaponSpec", 0));

            //ulong dmgType = (ulong)gom.Data.ValueOrDefault<ulong>("cbtDamageType", 0);
            if (itm.WeaponSpec != null)
                itm.DamageType = ItemDamageTypeExtensions.ToItemDamageType(itm.WeaponSpec.DamageType);
            /*switch (itm.WeaponSpec)
            {
                case WeaponSpec.Lightsaber:
                case WeaponSpec.Polesaber:
                case WeaponSpec.Pistol:
                case WeaponSpec.Rifle:
                case WeaponSpec.SniperRifle:
                case WeaponSpec.AssaultCannon:
                    { itm.DamageType = ItemDamageType.Energy; break; }
                case WeaponSpec.Vibroblade:
                case WeaponSpec.VibrobladeTech:
                case WeaponSpec.Vibroknife:
                case WeaponSpec.Shotgun:
                case WeaponSpec.Electrostaff:
                case WeaponSpec.ElectrostaffTech:
                    { itm.DamageType = ItemDamageType.Kinetic; break; }
                default:
                    { itm.DamageType = ItemDamageType.None; break; }
            }*/

            //if(itm.Quality == ItemQuality.Artifact && itm.EnhancementSlots.Count > 1)
            //{
            itm.TypeBitFlags.IsModdable = (itm.EnhancementSlots.Count > 1);
            //}
            ItemSubCategoryExtensions.SetCategory(itm);

            if (itm.WeaponSpec != null)
            {
                itm.WeaponAppSpec = gom.Data.ValueOrDefault("cbtWeaponAppearanceSpec", "");
                if (itm.WeaponAppSpec != "")
                {
                    if (itemAppearances.ContainsKey(itm.WeaponAppSpec))
                    {
                        itm.Model = ((GomObjectData)itemAppearances[itm.WeaponAppSpec]).ValueOrDefault("itmModel", "");
                    }
                }
            }

            var sourceList = gom.Data.ValueOrDefault("itmDecorationSource", new List<object>());

            itm.StrongholdSourceList = sourceList.ConvertAll(x => (long)x);

            itm.IsUnknownBool = gom.Data.ValueOrDefault("itmIsUnknownBool", false);
            itm.MTXRarity = gom.Data.ValueOrDefault("itmMTXRarity", new ScriptEnum()).ToString();

            var sourceProto = _dom.GetObject("itmSourceProto");

            if (sourceProto != null)
            {
                var sourceLookup = sourceProto.Data.Get<Dictionary<object, object>>("itmSourceNameIdTable");
                var souceNameLookup = _dom.stringTable.Find("str.gui.item_source");

                itm.StrongholdSourceNameDict = new Dictionary<long, string>();
                itm.LocalizedStrongholdSourceNameDict = new Dictionary<long, Dictionary<string, string>>();
                foreach (var source in sourceList)
                {
                    sourceLookup.TryGetValue(source, out object sourceNameId);
                    if (sourceNameId != null)
                    {
                        string name = souceNameLookup.GetText((long)sourceNameId, "str.gui.item_source");
                        itm.StrongholdSourceNameDict.Add((long)sourceNameId, name);
                        itm.LocalizedStrongholdSourceNameDict.Add((long)sourceNameId, souceNameLookup.GetLocalizedText((long)sourceNameId, "str.gui.item_source"));
                    }
                }
            }

            itm.SetBonusId = gom.Data.ValueOrDefault<long>("ItemSetBonusId", 0);


            LoadChildMap();
            childLookupMap.TryGetValue(itm.Id, out ulong cId);
            itm.ChildId = cId;

            itm.BindsToSlot = gom.Data.ValueOrDefault("itmBindsToSlot", false);

            itm.RepFactionId = (int)gom.Data.ValueOrDefault<long>("reputationFactionIndex", 0); //on trophies

            /*if (gom.References != null)
            {
                if (gom.References.ContainsKey("materialFor"))
                {
                    itm.MaterialForSchems = gom.References["materialFor"].Select(x => x.ToMaskedBase62()).ToList();
                }
                if (gom.References.ContainsKey("rewardFrom"))
                {
                    itm.RewardFromQuests = gom.References["rewardFrom"].Select(x => x.ToMaskedBase62()).ToList();
                }
                if (gom.References.ContainsKey("similarAppearance"))
                {
                    itm.SimilarAppearance = gom.References["similarAppearance"].Select(x => x.ToMaskedBase62()).ToList();
                }
            }*/
            var authFlag = gom.Data.ValueOrDefault<long>("itmAuthBitFlags", 0);
            if (!authFlags.Contains(authFlag))
                authFlags.Add(authFlag);
            itm.DisintegrateCmdXP = gom.Data.ValueOrDefault<long>("itmDisintegrateCmdXP", 0);
            gom.Unload();
            return itm;
        }

        private static void DupeAppReferences(Item itm, GomObject itemAppearance, string key, string dupekey = "givenByItem")
        {
            if (itemAppearance.References == null)
                return;
            if (itemAppearance.References.ContainsKey(key))
            {
                if (itm.References == null)
                    itm.References = new Dictionary<string, SortedSet<ulong>>();
                if (!itm.References.ContainsKey(key))
                    itm.References.Add(key, new SortedSet<ulong>());
                if (itemAppearance.References.ContainsKey(dupekey))
                    itm.References[key].UnionWith(itemAppearance.References[dupekey]);
                if (itemAppearance.References[key].Count > 0)
                {
                    foreach (var refs in itemAppearance.References[key])
                    {
                        var obj = itm.Dom_.GetObjectNoLoad(refs);
                        if (obj.References.ContainsKey(dupekey))
                            itm.References[key].UnionWith(obj.References[dupekey]);
                    }
                }
            }
        }

        public void LoadChildMap()
        {
            if (schematicLookupMap.Count == 0) //load child and parent items
            {
                var debugmap = _dom.GetObject("prfDebugSchematicMapPrototype");
                if (debugmap == null) return;
                var childmap = debugmap.Data.ValueOrDefault("schemChildMap", new Dictionary<object, object>());
                debugmap.Unload();
                childLookupMap = childmap.ToDictionary(x => (ulong)x.Key, x => (ulong)((List<object>)x.Value).First());
                foreach (var kvp in childLookupMap)
                {
                    if (!schematicLookupMap.ContainsKey(kvp.Value)) //don't know if items can have more than one parent item, they can't have more than one child.
                        schematicLookupMap.Add(kvp.Value, new List<ulong> { kvp.Key });
                    else
                        schematicLookupMap[kvp.Value].Add(kvp.Key);
                }
            }
        }

        private string AppearanceTags(GomObject itemAppearance)
        {
            ScriptEnum appearanceSlotType = itemAppearance.Data.ValueOrDefault<ScriptEnum>("appAppearanceSlotType", null);
            if (appearanceSlotType != null)
            {
                if (appearanceSlotType.ToString() == "appSlotFace")
                {
                    if (itemAppearanceAssets.Count == 0)
                    {
                        var ami = _dom.ami.GetAMI("ami.face"); //Apparently they removed these from the client. Luckily I had implemented the AMI Loading.
                        if (ami != null)
                        {
                            foreach (var kvp in ami.data)
                            {
                                long id = kvp.Key;
                                string name = kvp.Value.BaseFile;
                                if (name.Contains('_'))
                                {
                                    name = name.Split('_')[1];
                                }
                                itemAppearanceAssets.Add(id, name);
                            }
                        }
                    }
                    long modelId = itemAppearance.Data.ValueOrDefault<long>("appAppearanceSlotModelID", 0);
                    if (itemAppearanceAssets.ContainsKey(modelId)) return itemAppearanceAssets[modelId];
                }
            }
            return "";
        }

        public void LoadReferences(GameObject obj, GomObject gom)
        {
        }

        public void LoadObject(GameObject obj, GomObject gom)
        {
            Load(obj, gom);
        }

        readonly Dictionary<ulong, Item> idMap = new Dictionary<ulong, Item>();
        readonly Dictionary<string, Item> nameMap = new Dictionary<string, Item>();

        public Item Load(ulong nodeId)
        {
            if (idMap.TryGetValue(nodeId, out Item result))
            {
                return result;
            }

            GomObject obj = _dom.GetObject(nodeId);
            if (obj == null) { return null; }
            Item itm = new Item();
            return Load(itm, obj);
        }

        public Item Load(string fqn)
        {
            if (nameMap.TryGetValue(fqn, out Item result))
            {
                return result;
            }

            GomObject obj = _dom.GetObject(fqn);
            if (obj == null) { return null; }
            Item itm = new Item();
            return Load(itm, obj);
        }

        public Item Load(GomObject obj)
        {
            if (obj == null) { return null; }
            Item itm = new Item();
            return Load(itm, obj);
        }
    }
}
