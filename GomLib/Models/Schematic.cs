using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;

namespace GomLib.Models
{
    public class Schematic : GameObject, IEquatable<Schematic>
    {
        #region Properties
        public ulong NodeId { get; set; }
        public ulong NameId { get; set; }
        public string Name { get { return _name ?? (_name = ""); } set { if (_name != value) { _name = value; } } }
        private string _name;
        public Dictionary<string, string> LocalizedName { get; set; }
        public Profession CrewSkillId { get; set; }
        public string CrewSkillName
        {
            get
            {
                switch (CrewSkillId)
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
        }
        [Newtonsoft.Json.JsonIgnore]
        public Item Item
        {
            get
            {
                if (_Item == null)
                    _Item = _dom.itemLoader.Load(ItemId);
                return _Item;
            }
            set
            {
                _Item = value;
            }
        }
        [Newtonsoft.Json.JsonIgnore]
        private Item _Item { get; set; }
        public int SkillOrange { get; set; }
        public int SkillYellow { get; set; }
        public int SkillGreen { get; set; }
        public int SkillGrey { get; set; }

        public ulong ItemId { get; set; }
        public string ItemBase62Id { get { return ItemId.ToMaskedBase62(); } }

        public Dictionary<ulong, int> Materials { get; set; }

        [Newtonsoft.Json.JsonIgnore]
        public Dictionary<string, int> MaterialsB62
        {
            get
            {
                if (Materials != null)
                {
                    return Materials.ToDictionary(x => x.Key.ToMaskedBase62(), x => x.Value);
                }
                return null;
            }
        }

        public int CraftingTime { get; set; }
        public int CraftingTimeT1 { get; set; }
        public int CraftingTimeT2 { get; set; }
        public int CraftingTimeT3 { get; set; }

        public Workstation Workstation { get; set; }
        public ProfessionSubtype Subtype { get; set; }

        [Newtonsoft.Json.JsonIgnore]
        public Item Research1 { get; set; }
        public int ResearchQuantity1 { get; set; }
        public SchematicResearchChance ResearchChance1 { get; set; }
        [Newtonsoft.Json.JsonIgnore]
        public Item Research2 { get; set; }
        public int ResearchQuantity2 { get; set; }
        public SchematicResearchChance ResearchChance2 { get; set; }
        [Newtonsoft.Json.JsonIgnore]
        public Item Research3 { get; set; }
        public int ResearchQuantity3 { get; set; }
        public SchematicResearchChance ResearchChance3 { get; set; }

        public int MissionCost { get; set; }
        public int MissionDescriptionId { get; set; }
        public string MissionDescription { get; set; }
        public bool MissionUnlockable { get; set; }
        public int MissionLight { get; set; }
        public int MissionLightCrit { get; set; }
        public int MissionDark { get; set; }
        public int MissionDarkCrit { get; set; }
        public int TrainingCost { get; set; }
        public bool DisableDisassemble { get; set; }
        public bool DisableCritical { get; set; }
        public Faction MissionFaction { get; set; }
        public int MissionYieldDescriptionId { get; set; }
        public string MissionYieldDescription { get; set; }
        public Dictionary<string, string> LocalizedMissionYieldDescription { get; set; }
        public bool Deprecated { get; set; }
        public string Category { get; set; }
        public string SubCategory { get; set; }
        public string Quality { get; set; }
        public Dictionary<long, ModPackage> Variations { get; set; }
        public List<ulong> LearnedIds { get; set; }
        [Newtonsoft.Json.JsonIgnore]
        public string LearnedBase62Ids
        {
            get
            {
                return String.Join(", ", LearnedIds.Where(x => x != 0).Select(x => x.ToMaskedBase62()));
            }
        }
        [Newtonsoft.Json.JsonIgnore]
        public string HashedIcon
        {
            get
            {
                string icon = "none";
                if (this.Item != null)
                {
                    icon = Item.Icon;
                }
                var fileId = TorLib.FileId.FromFilePath(String.Format("/resources/gfx/icons/{0}.dds", icon));
                return String.Format("{0}_{1}", fileId.ph, fileId.sh);
            }
        }
        public bool TrainerTaught { get; set; }

        #endregion

        #region IEquatable<Schematic>
        public override int GetHashCode()
        {
            int hash = Name.GetHashCode();
            if (Item != null)
            {
                hash ^= Item.GetHashCode();
            }
            hash ^= CrewSkillId.GetHashCode();
            hash ^= SkillOrange.GetHashCode();
            hash ^= SkillYellow.GetHashCode();
            hash ^= SkillGreen.GetHashCode();
            hash ^= SkillGrey.GetHashCode();
            hash ^= CraftingTime.GetHashCode();
            hash ^= Subtype.GetHashCode();
            if (Research1 != null)
            {
                hash ^= CraftingTimeT1.GetHashCode();
                hash ^= Research1.GetHashCode();
                hash ^= ResearchChance1.GetHashCode();
                hash ^= ResearchQuantity1.GetHashCode();
            }
            if (Research2 != null)
            {
                hash ^= CraftingTimeT2.GetHashCode();
                hash ^= Research2.GetHashCode();
                hash ^= ResearchChance2.GetHashCode();
                hash ^= ResearchQuantity2.GetHashCode();
            }
            if (Research3 != null)
            {
                hash ^= CraftingTimeT3.GetHashCode();
                hash ^= Research3.GetHashCode();
                hash ^= ResearchChance3.GetHashCode();
                hash ^= ResearchQuantity3.GetHashCode();
            }
            hash ^= MissionCost.GetHashCode();
            if (MissionDescription != null)
            {
                hash ^= MissionDescription.GetHashCode();
            }
            hash ^= MissionUnlockable.GetHashCode();
            hash ^= MissionLight.GetHashCode();
            hash ^= MissionLightCrit.GetHashCode();
            hash ^= MissionDark.GetHashCode();
            hash ^= MissionDarkCrit.GetHashCode();
            hash ^= TrainingCost.GetHashCode();
            hash ^= DisableDisassemble.GetHashCode();
            hash ^= DisableCritical.GetHashCode();
            hash ^= MissionFaction.GetHashCode();
            if (MissionYieldDescription != null) hash ^= MissionYieldDescription.GetHashCode();
            hash ^= Deprecated.GetHashCode();
            return hash;
        }

        public override bool Equals(object obj)
        {
            if (obj == null) return false;

            if (ReferenceEquals(this, obj)) return true;

            Schematic sch = obj as Schematic;
            if (sch == null) return false;

            return Equals(sch);
        }

        public bool Equals(Schematic sch)
        {
            if (sch == null) return false;

            if (ReferenceEquals(this, sch)) return true;

            if (this.CraftingTime != sch.CraftingTime)
                return false;
            //if (this.ablEffects != abl.ablEffects)
            //return false;
            if (this.CraftingTimeT1 != sch.CraftingTimeT1)
                return false;
            if (this.CraftingTimeT2 != sch.CraftingTimeT2)
                return false;
            if (this.CraftingTimeT3 != sch.CraftingTimeT3)
                return false;
            if (this.CrewSkillId != sch.CrewSkillId)
                return false;
            if (this.Deprecated != sch.Deprecated)
                return false;
            if (this.DisableCritical != sch.DisableCritical)
                return false;
            if (this.DisableDisassemble != sch.DisableDisassemble)
                return false;
            if (this.Fqn != sch.Fqn)
                return false;
            if (this.Id != sch.Id)
                return false;
            if (this.ItemId != sch.ItemId)
                return false;

            var uiComp = new DictionaryComparer<ulong, int>();
            if (!uiComp.Equals(this.Materials, sch.Materials))
                return false;

            if (this.MissionCost != sch.MissionCost)
                return false;
            if (this.MissionDark != sch.MissionDark)
                return false;
            if (this.MissionDarkCrit != sch.MissionDarkCrit)
                return false;
            if (this.MissionDescription != sch.MissionDescription)
                return false;
            if (this.MissionDescriptionId != sch.MissionDescriptionId)
                return false;
            if (this.MissionLight != sch.MissionLight)
                return false;
            if (this.MissionLightCrit != sch.MissionLightCrit)
                return false;
            if (this.MissionUnlockable != sch.MissionUnlockable)
                return false;
            if (this.MissionYieldDescription != sch.MissionYieldDescription)
                return false;
            if (this.MissionYieldDescriptionId != sch.MissionYieldDescriptionId)
                return false;
            if (this.Name != sch.Name)
                return false;
            if (this.NameId != sch.NameId)
                return false;
            if (this.NodeId != sch.NodeId)
                return false;
            if (this.Research1 != null)
            {
                if (sch.Research1 == null)
                    return false;
                if (!this.Research1.Equals(sch.Research1))
                    return false;
            }
            else if (sch.Research1 != null)
                return false;
            if (this.ResearchChance1 != sch.ResearchChance1)
                return false;
            if (this.ResearchQuantity1 != sch.ResearchQuantity1)
                return false;
            if (this.Research2 != null)
            {
                if (sch.Research2 == null)
                    return false;
                if (!this.Research2.Equals(sch.Research2))
                    return false;
            }
            else if (sch.Research2 != null)
                return false;
            if (this.ResearchChance2 != sch.ResearchChance2)
                return false;
            if (this.ResearchQuantity2 != sch.ResearchQuantity2)
                return false;
            if (this.Research3 != null)
            {
                if (sch.Research3 == null)
                    return false;
                if (!this.Research3.Equals(sch.Research3))
                    return false;
            }
            else if (sch.Research3 != null)
                return false;
            if (this.ResearchChance3 != sch.ResearchChance3)
                return false;
            if (this.ResearchQuantity3 != sch.ResearchQuantity3)
                return false;
            if (this.SkillGreen != sch.SkillGreen)
                return false;
            if (this.SkillGrey != sch.SkillGrey)
                return false;
            if (this.SkillOrange != sch.SkillOrange)
                return false;
            if (this.SkillYellow != sch.SkillYellow)
                return false;
            if (this.Subtype != sch.Subtype)
                return false;
            if (this.TrainingCost != sch.TrainingCost)
                return false;
            if (this.Workstation != sch.Workstation)
                return false;
            return true;
        }
        #endregion

        public override string ToString(bool verbose)
        {
            var txt = new StringBuilder();
            string n = Environment.NewLine;

            txt.Append(String.Format("* {0}{1}", Item.Name, n)); //Item.Description.Replace("\r\n", replaceWith).Replace("\n", replaceWith).Replace("\r", replaceWith), n));
            /*List<string> reqs = new List<string>();
            foreach (var kvp in Materials)
            {
                var mat = _dom.itemLoader.Load(kvp.Key);
                reqs.Add(String.Format("{0}x {1}", kvp.Value, mat.Name)); //, mat.Description));
            }
            if (reqs.Count > 0)
                txt.Append(String.Format(" * {0}{1}", String.Join(n + " * ", reqs), n));*/
            return txt.ToString();
        }
        [Newtonsoft.Json.JsonIgnore]
        public override List<SQLProperty> SQLProperties
        {
            get
            {
                return new List<SQLProperty>
                    {                //(SQL Column Name, C# Property Name, SQL Column type statement, isUnique/PrimaryKey, Serialize value to json)
                        new SQLProperty("NodeId", "Id", "bigint(20) unsigned NOT NULL", true),
                        new SQLProperty("Base62Id", "Base62Id", "varchar(7) COLLATE latin1_general_cs NOT NULL"),
                        new SQLProperty("NameId", "NameId", "bigint(20) NOT NULL"),
                        new SQLProperty("Name", "Name", "varchar(255) COLLATE utf8_unicode_ci NOT NULL"),
                        new SQLProperty("Fqn", "Fqn", "varchar(255) COLLATE utf8_unicode_ci NOT NULL"),
                        new SQLProperty("CrewSkillId", "CrewSkillId", "int(11) NOT NULL"),
                        new SQLProperty("CrewSkillName", "CrewSkillName", "varchar(50) COLLATE utf8_unicode_ci NOT NULL"),
                        new SQLProperty("SkillOrange", "SkillOrange", "int(11) NOT NULL"),
                        new SQLProperty("SkillYellow", "SkillYellow", "int(11) NOT NULL"),
                        new SQLProperty("SkillGreen", "SkillGreen", "int(11) NOT NULL"),
                        new SQLProperty("SkillGrey", "SkillGrey", "int(11) NOT NULL"),
                        new SQLProperty("ItemBase62Id", "ItemBase62Id", "varchar(7) COLLATE utf8_unicode_ci NOT NULL"),
                        new SQLProperty("LearnedBase62Ids", "LearnedBase62Ids", "varchar(255) COLLATE utf8_unicode_ci NOT NULL"),
                        new SQLProperty("Category", "Category", "varchar(65) COLLATE utf8_unicode_ci NOT NULL"),
                        new SQLProperty("SubCategory", "SubCategory", "varchar(65) COLLATE utf8_unicode_ci NOT NULL"),
                        new SQLProperty("Quality", "Quality", "varchar(15) COLLATE utf8_unicode_ci NOT NULL"),
                        new SQLProperty("Icon", "HashedIcon", "varchar(255) COLLATE utf8_unicode_ci NOT NULL"),
                        new SQLProperty("TrainerTaught","TrainerTaught", "tinyint(1) NOT NULL"),
                        new SQLProperty("TrainingCost", "TrainingCost", "int(11) NOT NULL"),
                        new SQLProperty("Materials", "MaterialsB62", "varchar(155) COLLATE latin1_general_cs NOT NULL", false, true),
                        new SQLProperty("CraftingTime", "CraftingTime", "int(11) NOT NULL"),
                        new SQLProperty("CraftingTimeT1", "CraftingTimeT1", "int(11) NOT NULL"),
                        new SQLProperty("CraftingTimeT2", "CraftingTimeT2", "int(11) NOT NULL"),
                        new SQLProperty("CraftingTimeT3", "CraftingTimeT3", "int(11) NOT NULL"),
                        //new SQLProperty("Workstation", "Workstation",
                        //new SQLProperty("Subtype", "Subtype",
                        /*new SQLProperty("Research1", "Research1",
                        new SQLProperty("ResearchQuantity1", "ResearchQuantity1",
                        new SQLProperty("ResearchChance1", "ResearchChance1",
                        new SQLProperty("Research2", "Research2",
                        new SQLProperty("ResearchQuantity2", "ResearchQuantity2",
                        new SQLProperty("ResearchChance2", "ResearchChance2",
                        new SQLProperty("Research3", "Research3",
                        new SQLProperty("ResearchQuantity3", "ResearchQuantity3",
                        new SQLProperty("ResearchChance3", "ResearchChance3",*/
                        new SQLProperty("MissionCost", "MissionCost", "int(11) NOT NULL"),
                        new SQLProperty("MissionDescriptionId", "MissionDescriptionId", "int(11) NOT NULL"),
                        new SQLProperty("MissionDescription", "MissionDescription", "varchar(255) COLLATE utf8_unicode_ci NOT NULL"),
                        new SQLProperty("MissionUnlockable", "MissionUnlockable", "tinyint(1) NOT NULL"),
                        new SQLProperty("MissionLight", "MissionLight", "int(11) NOT NULL"),
                        new SQLProperty("MissionLightCrit", "MissionLightCrit", "int(11) NOT NULL"),
                        new SQLProperty("MissionDark", "MissionDark", "int(11) NOT NULL"),
                        new SQLProperty("MissionDarkCrit", "MissionDarkCrit", "int(11) NOT NULL"),
                        new SQLProperty("DisableDisassemble", "DisableDisassemble", "tinyint(1) NOT NULL"),
                        new SQLProperty("DisableCritical", "DisableCritical", "tinyint(1) NOT NULL"),
                        new SQLProperty("MissionFaction", "MissionFaction", "varchar(255) COLLATE utf8_unicode_ci NOT NULL"),
                        new SQLProperty("MissionYieldDescriptionId", "MissionYieldDescriptionId", "int(11) NOT NULL"),
                        new SQLProperty("MissionYieldDescription", "MissionYieldDescription", "varchar(255) COLLATE utf8_unicode_ci NOT NULL"),
                        new SQLProperty("Deprecated", "Deprecated", "tinyint(1) NOT NULL"),
                        /*new SQLProperty("Variations", "Variations*/
                    };
            }
        }
        public override XElement ToXElement(bool verbose)
        {
            XElement schem = new XElement("Schematic");
            if (this.Id == 0) return schem;

            schem.Add(new XElement("Fqn", Fqn),
                new XAttribute("Id", NodeId),
                new XElement("CrewSkill", CrewSkillId));

            if (Name == "")
            {
                if (ItemId != 0)
                {
                    var crafted = new GameObject().ToXElement(ItemId, _dom, verbose);
                    crafted.Name = "CraftedItem";
                    schem.Add(crafted);
                    /*if (ExportICONS)
                    {
                        OutputSchematicIcon(crftItm.Icon);
                    }*/
                }

                XElement mats = new XElement("RequiredMaterials");
                if (Materials != null)
                {
                    foreach (var kvp in Materials)
                    {
                        mats.Add(RequiredMatToXElement(_dom, kvp.Value, kvp.Key, verbose));
                    }
                    schem.Add(mats);
                }
                XElement resmats = new XElement("ResearchReturnedMaterials");
                if (Research1 != null)
                    resmats.Add(RequiredMatToXElement(_dom, ResearchQuantity1, Research1.Id, verbose));
                if (Research2 != null)
                    resmats.Add(RequiredMatToXElement(_dom, ResearchQuantity2, Research2.Id, verbose));
                if (Research3 != null)
                    resmats.Add(RequiredMatToXElement(_dom, ResearchQuantity3, Research3.Id, verbose));
                schem.Add(resmats);
            }
            else
            {
                schem.Add(new XElement("Name", Name),
                    new XElement("MissionDescription", MissionDescription));
            }


            if (verbose)
            {
                if (Name == "")
                {

                }
                else
                {
                    schem.Add(new XElement("Yield", MissionYieldDescription),
                        new XElement("IsUnlockable", MissionUnlockable),
                        new XElement("Cost", MissionCost),
                        new XElement("Alignment", String.Format("Light(normal/crit): {0}/{1}, Dark(normal/crit): {2}/{3}", MissionLight, MissionLightCrit, MissionDark, MissionDarkCrit)));
                }
                schem.Add(new XElement("SkillRanges",
                    new XElement("Orange", SkillOrange),
                    new XElement("Yellow", SkillYellow),
                    new XElement("Green", SkillGreen),
                    new XElement("Grey", SkillGrey)));
            }

            return schem;
        }
        private XElement RequiredMatToXElement(DataObjectModel dom, int i, ulong id, bool verbose)
        {
            var itm = dom.itemLoader.Load(id);
            var mat = new GameObject().ToXElement(id, dom, verbose);
            mat.Add(new XAttribute("Quantity", i), new XElement("Icon", itm.Icon));
            /* (ExportICONS)
            {
                OutputSchematicIcon(itm.Icon);
            }*/
            return mat;
        }
    }

    public class ModPackage : PseudoGameObject
    {
        public override string ToString()
        {
            return string.Format("{0} {1}: {2}",
                Id, Name, AtrributePercentages);
        }

        public override XElement ToXElement()
        {
            XElement packageElem = new XElement("Package", new XAttribute("Id", Id));
            packageElem.Add(new XElement("Name", Name));
            XElement statsElem = new XElement("Stats");
            foreach(KeyValuePair<Stat, float> kvp in AtrributePercentages)
            {
                statsElem.Add(new XElement("Stat", new XAttribute("Id", kvp.Key), kvp.Value));
            }
            packageElem.Add(statsElem);

            return packageElem;
        }

        public override int GetHashCode()
        {
            int hash = NameId.GetHashCode();
            hash ^= Name.GetHashCode();
            hash ^= Id.GetHashCode();
            foreach(KeyValuePair<Stat, float> kvp in AtrributePercentages)
            {
                hash ^= kvp.Key.GetHashCode();
                hash ^= kvp.Value.GetHashCode();
            }

            return hash;
        }

        public override bool Equals(object obj)
        {
            if (obj == null)
                return false;
            if (ReferenceEquals(this, obj))
                return true;

            return Equals(obj as ModPackage);
        }

        public bool Equals(ModPackage obj)
        {
            if (obj == null)
                return false;
            if (ReferenceEquals(this, obj))
                return true;

            if (this.Id != obj.Id)
                return false;
            if (this.NameId != obj.NameId)
                return false;
            if (this.AtrributePercentages.Count != obj.AtrributePercentages.Count)
                return false;
            if (!this.AtrributePercentages.SequenceEqual(obj.AtrributePercentages))
                return false;

            return true;
        }

        public ModPackage(long id, long nId, string n, Dictionary<Stat, float> modStats)
        {
            NameId = nId;
            Id = id;
            Name = n;
            AtrributePercentages = modStats;
        }
        public long NameId { get; set; }
        public Dictionary<Stat, float> AtrributePercentages { get; set; }
    }
}