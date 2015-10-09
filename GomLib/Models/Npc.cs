using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;
using System.Xml;
using Newtonsoft.Json;

namespace GomLib.Models
{
    public class Npc : GameObject, IEquatable<Npc>
    {
        #region Properties
        public ulong parentSpecId { get; set; }
        [JsonIgnore]
        public ulong NodeId { get; set; }
        public string Name { get; set; }
        [JsonConverter(typeof(LongConverter))]
        public long NameId { get; set; }
        public Dictionary<string, string> LocalizedName { get; set; }
        public string Title { get; set; }
        public Dictionary<string, string> LocalizedTitle { get; set; }
        [JsonConverter(typeof(ULongConverter))]
        public ulong ClassId { get; set; }
        public string ClassB62Id
        {
            get
            {
                if (ClassId == 0) return "";
                return ClassId.ToMaskedBase62();
            }
        }
        public string AbilityPackageB62Id
        {
            get {
                if (ClassSpec != null) {
                    return ClassSpec.AbilityPackageB62Id;
                }
                return "";
            }
        }
        [Newtonsoft.Json.JsonIgnore]
        public ClassSpec ClassSpec { get; set; }
        public int MinLevel { get; set; }
        public int MaxLevel { get; set; }
        public Faction Faction { get; set; }
        public DetailedFaction DetFaction { get; set; }
        [JsonConverter(typeof(Newtonsoft.Json.Converters.StringEnumConverter))]
        public Toughness Toughness { get; set; }
        [JsonConverter(typeof(LongConverter))]
        public long ToughnessId { get; set; }
        public Dictionary<string, string> LocalizedToughness { get; set; }
        public int DifficultyFlags { get; set; }

        [Newtonsoft.Json.JsonIgnore]
        public Conversation Conversation { get; set; }

        public string cnvConversationName { get; set; }

        [Newtonsoft.Json.JsonIgnore]
        public Codex Codex
        {
            get {
                if (CodexId != 0)
                    return _dom.codexLoader.Load(CodexId);
                else
                    return null;
            }
        }
        [JsonConverter(typeof(ULongConverter))]
        public ulong CodexId { get; set; }
        [JsonIgnore]
        public string CodexB62Id
        {
            get
            {
                if (CodexId == 0) return "";
                return CodexId.ToMaskedBase62();
            }
        }
        public Profession ProfessionTrained { get; set; }
        [Newtonsoft.Json.JsonIgnore]
        public Npc CompanionOverride {
            get { return _dom.npcLoader.Load(CompanionOverrideId); }
            set { CompanionOverride = value; }
        }
        [JsonConverter(typeof(ULongConverter))]
        public ulong CompanionOverrideId { get; set; }
        [JsonConverter(typeof(LongConverter))]
        public long LootTableId { get; set; }

        public bool IsClassTrainer { get; set; }
        public bool IsVendor { get { return this.VendorPackages.Count > 0; } }
        public List<string> VendorPackages { get; set; }

        public string movementPackage { get; set; }
        public string coverPackage { get; set; }
        public string WanderPackage { get; set; }
        public string AggroPackage { get; set; }
        public List<NpcVisualData> VisualDataList { get; set; }

        public string charRef { get; set; }

        internal string _FqnCategory { get; set; }
        public string FqnCategory
        {
            get
            {
                if (String.IsNullOrEmpty(_FqnCategory))
                {
                    string[] fqnParts = Fqn.Substring(4).Split('.');
                    _FqnCategory = fqnParts[0];
                    _FqnSubCategory = fqnParts[1];
                }
                return _FqnCategory;
            }
        }
        internal string _FqnSubCategory { get; set; }
        public string FqnSubCategory
        {
            get
            {
                if (String.IsNullOrEmpty(_FqnSubCategory))
                {
                    string[] fqnParts = Fqn.Substring(4).Split('.');
                    _FqnCategory = fqnParts[0];
                    _FqnSubCategory = fqnParts[1];
                }
                return _FqnSubCategory;
            }
        }

        public Npc()
        {
            VendorPackages = new List<string>();
        }

        //public List<string> AbilityPackagesTrained { get; set; }
        #endregion

        #region IEquatable
        public override int GetHashCode()
        {
            int hash = Name.GetHashCode();
            if (Title != null) { hash ^= Title.GetHashCode(); }
            hash ^= ClassSpec.Id.GetHashCode();
            hash ^= MinLevel.GetHashCode();
            hash ^= MaxLevel.GetHashCode();
            hash ^= Faction.GetHashCode();
            hash ^= Toughness.GetHashCode();
            hash ^= DifficultyFlags.GetHashCode();
            if (Codex != null) hash ^= Codex.Id.GetHashCode();
            hash ^= ProfessionTrained.GetHashCode();
            if (cnvConversationName != null) hash ^= cnvConversationName.GetHashCode();
            return hash;
        }

        public override bool Equals(object obj)
        {
            if (obj == null) return false;

            if (ReferenceEquals(this, obj)) return true;

            Npc npc = obj as Npc;
            if (npc == null) return false;

            return Equals(npc);
        }

        public bool Equals(Npc npc)
        {
            if (npc == null) return false;

            if (ReferenceEquals(this, npc)) return true;

            if (this.AggroPackage != npc.AggroPackage)
                return false;
            if (!this.ClassSpec.Equals(npc.ClassSpec))
                return false;
            /*if (this.Codex != null)
            {
                if (!this.Codex.Equals(npc.Codex))
                    return false;
            }
            else if (npc.Codex != null)
                return false;*/
            if (this.CodexId != npc.CodexId)
                return false;
            /*if (this.CompanionOverride != null)
            {
                if (!this.CompanionOverride.Equals(npc.CompanionOverride))
                    return false;
            }
            else if (npc.CompanionOverride != null)
                return false;*/
            if (this.CompanionOverrideId != npc.CompanionOverrideId)
                return false;
            //if (!this.Conversation.Equals(npc.Conversation))
                //return false;
            if (this.cnvConversationName != npc.cnvConversationName)
                return false;
            if (this.coverPackage != npc.coverPackage)
                return false;
            if (this.DifficultyFlags != npc.DifficultyFlags)
                return false;
            if (this.Faction != npc.Faction)
                return false;
            if (this.Fqn != npc.Fqn)
                return false;
            if (this.Id != npc.Id)
                return false;
            if (this.IsClassTrainer != npc.IsClassTrainer)
                return false;
            if (this.IsVendor != npc.IsVendor)
                return false;

            var ssComp = new DictionaryComparer<string, string>();
            if (!ssComp.Equals(this.LocalizedName, npc.LocalizedName))
                return false;
            if (!ssComp.Equals(this.LocalizedTitle, npc.LocalizedTitle))
                return false;

            if (this.LootTableId != npc.LootTableId)
                return false;
            if (this.MaxLevel != npc.MaxLevel)
                return false;
            if (this.MinLevel != npc.MinLevel)
                return false;
            if (this.movementPackage != npc.movementPackage)
                return false;
            if (this.Name != npc.Name)
                return false;
            if (this.NameId != npc.NameId)
                return false;
            if (this.NodeId != npc.NodeId)
                return false;
            if (this.parentSpecId != npc.parentSpecId)
                return false;
            if (this.ProfessionTrained != npc.ProfessionTrained)
                return false;
            if (this.Title != npc.Title)
                return false;
            if (this.Toughness != npc.Toughness)
                return false;
            if (!this.VendorPackages.SequenceEqual(npc.VendorPackages))
                return false;
            if (this.VisualDataList != null)
            {
                if (npc.VisualDataList == null)
                {
                    return false;
                }
                else
                {
                    if (!Enumerable.SequenceEqual<NpcVisualData>(this.VisualDataList, npc.VisualDataList))
                        return false;
                }
            }
            if (this.WanderPackage != npc.WanderPackage)
                return false;
            return true;
        }
        #endregion

        #region Output Related
        public override string ToString()
        {
            var txtFile = new StringBuilder();
            string n = Environment.NewLine;

            txtFile.Append("------------------------------------------------------------" + n);
            txtFile.Append("Name: " + Name + n);
            txtFile.Append("NodeId: " + NodeId + n);
            txtFile.Append("Id: " + Id + n);
            txtFile.Append("  Title: " + Title + n);
            txtFile.Append("  Level (Min/Max): " + MinLevel + "/" + MaxLevel + n);
            txtFile.Append("  Toughness: " + Toughness + n);
            txtFile.Append("  Faction: " + Faction + n);
            txtFile.Append("  Fqn: " + Fqn + n);
            txtFile.Append("  ClassSpec: " + n);
            txtFile.Append("    Name: " + ClassSpec.Name + " (" + n); // + ClassSpec.NameId + ")" + n );
            txtFile.Append("    NodeId: " + ClassSpec.Id + n);
            txtFile.Append("    ID: " + ClassSpec.Id + n);
            txtFile.Append("    Fqn: " + ClassSpec.Fqn + n);
            //txtFile.Append("    Player Class?: " + ClassSpec.IsPlayerClass + n); //Always false
            //txtFile.Append("    Alignment (Dark/Light): " + ClassSpec.AlignmentDark + "/" + ClassSpec.AlignmentLight + n); //Always 0
            txtFile.Append("    Ability Package Id: " + ClassSpec.AbilityPackageId + n);
            txtFile.Append("    Ability Package:" + n);
            txtFile.Append("    --------------------" + n);
            int p = 1;
            if (ClassSpec.AbilityPackage != null)
            {
                foreach (var ablpack in ClassSpec.AbilityPackage.PackageAbilities)
                {
                    txtFile.Append("     Package " + p + ":" + n);
                    var abl = ablpack.Ability;
                    txtFile.Append("      Ability Name: " + abl.Name + n);
                    txtFile.Append("      Ability NodeId: " + abl.NodeId + n);
                    //txtFile.Append("      Ability Id: " + abl.Id + n);
                    //txtFile.Append("      Flags (Passive/Hidden): " + abl.IsPassive + "/" + abl.IsHidden + n);
                    txtFile.Append("      Description: " + abl.Description + " (" + abl.DescriptionId + ")" + n);
                    //txtFile.Append("      Tokens: " + abl.TalentTokens + n);
                    /*if (abl.ablEffects != null) // May have gone overboard here on the text output. Add check for Verbose flag and radiobutton to set it.
                    {
                        txtFile.Append("      Ability Effects:" + abl.ablEffects.ToArray().ToString() + n);
                    }
                    txtFile.Append("      Fqn: " + abl.Fqn + n);
                    txtFile.Append("      Icon: " + abl.Icon + n);
                    txtFile.Append("      level: " + ablpack.Level + n);
                    if (ablpack.Scales) { txtFile.Append("      levels: " + String.Join(", ", ablpack.Levels.ToArray()) + n); }
                    txtFile.Append("      Range (Min/Max): " + abl.MinRange + "/" + abl.MaxRange + n);
                    if (abl.ChannelingTime != 0) { txtFile.Append("      Channeling Time: " + abl.ChannelingTime + n); }
                    else
                    {
                        if (abl.CastingTime != 0) { txtFile.Append("      Casting Time: " + abl.CastingTime + n); }
                        else { txtFile.Append("      Casting Time: Instant" + n); }
                    }
                    txtFile.Append("      Cooldown Time: " + abl.Cooldown + n);
                    if (abl.GCD != -1) { txtFile.Append("      GCD: " + abl.GCD + n); }
                    if (abl.GcdOverride) { txtFile.Append("      Overrides GCD: " + abl.GcdOverride + n); }
                    txtFile.Append("      LoS Check: " + abl.LineOfSightCheck + n);
                    if (abl.ModalGroup != 0) { txtFile.Append("      Modal Group: " + abl.ModalGroup + n); }
                    if (abl.SharedCooldown != 0) { txtFile.Append("      Shared Cooldown: " + abl.SharedCooldown + n); }
                    if (abl.TargetArc != 0 && abl.TargetArcOffset != 0) { txtFile.Append("      Target Arc/Offset: " + abl.TargetArc + "/" + abl.TargetArcOffset + n); }
                    txtFile.Append("      Target Rule: " + abl.TargetRule + n);*/
                    txtFile.Append("    --------------------" + n);
                    p++;
                }
            }
            if (Codex != null)
            {
                txtFile.Append("  Codex: " + Codex.Fqn.ToString() + ": " + Codex.LocalizedName["enMale"] + n);
            }
            /* txtFile.Append("  CompanionOverride: " + CompanionOverride + n); //Always Empty
            txtFile.Append("  Conversation: " + Conversation + n);
            txtFile.Append("  ConversationFqn: " + ConversationFqn + n); */
            txtFile.Append("  DifficultyFlags: " + DifficultyFlags + n);
            if (IsClassTrainer) { txtFile.Append("  ProfessionTrained: " + ProfessionTrained + n); }
            if (IsVendor) { txtFile.Append("  VendorPackages: { " + String.Join(", ", VendorPackages.ToArray<string>()) + " }" + n); }
            txtFile.Append("  LootTableId: " + LootTableId + n);
            txtFile.Append("------------------------------------------------------------" + n);
            txtFile.Append(Environment.NewLine + n);

            return txtFile.ToString();
        }
        public override List<SQLProperty> SQLProperties
        {
            get
            {
                return new List<SQLProperty>
                    {                //(SQL Column Name, C# Property Name, SQL Column type statement, IsUnique/PrimaryKey, Serialize value to json)
                        new SQLProperty("Name", "Name", "varchar(255) COLLATE utf8_unicode_ci NOT NULL", SQLPropSetting.AddFullTextIndex),
                        new SQLProperty("FrName", "LocalizedName[frMale]", "varchar(255) COLLATE utf8_unicode_ci NOT NULL", SQLPropSetting.AddFullTextIndex),
                        new SQLProperty("DeName", "LocalizedName[deMale]", "varchar(255) COLLATE utf8_unicode_ci NOT NULL", SQLPropSetting.AddFullTextIndex),
                        new SQLProperty("Title", "Title", "varchar(255) COLLATE utf8_unicode_ci NOT NULL", SQLPropSetting.AddIndex),
                        new SQLProperty("Base62Id", "Base62Id", "varchar(7) COLLATE latin1_general_cs NOT NULL", SQLPropSetting.PrimaryKey),
                        new SQLProperty("MinLevel", "MinLevel", "int(11) NOT NULL", SQLPropSetting.AddIndex),
                        new SQLProperty("MaxLevel", "MaxLevel", "int(11) NOT NULL", SQLPropSetting.AddIndex),
                        new SQLProperty("FactionString", "DetFaction.FactionString", "varchar(255) COLLATE utf8_unicode_ci NOT NULL", SQLPropSetting.AddIndex),
                        new SQLProperty("Faction", "DetFaction.LocalizedName[enMale]", "varchar(255) COLLATE utf8_unicode_ci NOT NULL", SQLPropSetting.AddIndex),
                        new SQLProperty("FrFaction", "DetFaction.LocalizedName[frMale]", "varchar(255) COLLATE utf8_unicode_ci NOT NULL", SQLPropSetting.AddIndex),
                        new SQLProperty("DeFaction", "DetFaction.LocalizedName[deMale]", "varchar(255) COLLATE utf8_unicode_ci NOT NULL", SQLPropSetting.AddIndex),
                        new SQLProperty("Toughness", "LocalizedToughness[enMale]", "varchar(255) COLLATE utf8_unicode_ci NOT NULL", SQLPropSetting.AddIndex),
                        new SQLProperty("FrToughness", "LocalizedToughness[frMale]", "varchar(255) COLLATE utf8_unicode_ci NOT NULL", SQLPropSetting.AddIndex),
                        new SQLProperty("DeToughness", "LocalizedToughness[deMale]", "varchar(255) COLLATE utf8_unicode_ci NOT NULL", SQLPropSetting.AddIndex),
                        new SQLProperty("FqnCategory", "FqnCategory", "varchar(255) COLLATE utf8_unicode_ci NOT NULL", SQLPropSetting.AddIndex),
                        new SQLProperty("FqnSubCategory", "FqnSubCategory", "varchar(255) COLLATE utf8_unicode_ci NOT NULL", SQLPropSetting.AddIndex),
                        new SQLProperty("CodexB62Id", "CodexB62Id", "varchar(7) COLLATE utf8_unicode_ci NOT NULL", SQLPropSetting.AddIndex)

                    };
            }
        }
        public override XElement ToXElement(bool verbose)
        {
            if (NodeId == 0) return null;

            /* Missing Element Fixers */
            string codexFQN = "none";
            string codexTitle = "none";
            if (Codex != null)
            {
                codexFQN = Codex.Fqn;
                codexTitle = Codex.LocalizedName["enMale"];
            }
            string conversation = null;
            string conversationFQN = null;
            if (Conversation != null)
            {
                conversation = Conversation.Id.ToString();
                conversationFQN = cnvConversationName;
            }

            XElement npcNode = new XElement("Npc",
                new XElement("Fqn", Fqn,
                    new XAttribute("NodeId", NodeId)),
                new XAttribute("Id", NodeId), //this is a hack otherwise the id would always be "376543"
                //new XAttribute("Hash", GetHashCode()),
                new XElement("Name", Name),
                new XElement("MinLevel", MinLevel),
                new XElement("MaxLevel", MaxLevel),
                new XElement("Toughness", Toughness));
            if (verbose)
            {
                if (VisualDataList != null)
                {
                    for (var x = 0; x < VisualDataList.Count; x++)
                    {
                        npcNode.Add(VisualDataList[x].ToXElement(x));
                    }
                }
                npcNode.Add(new XElement("Faction", Faction),
                new XElement("Codex", new XElement("Title", codexTitle), //add codex section here
                    new XAttribute("Id", codexFQN)),
                new XElement("CompanionOverride", CompanionOverride.ToXElement(false)),
                    /*new XElement("Conversation", new XElement("String", conversation), //add code here to cycle through nodes
                        new XElement("Fqn", conversationFQN)), */
                new XElement("DifficultyFlags", DifficultyFlags),
                new XElement("ProfessionTrained", ProfessionTrained),
                new XElement("LootTableId", LootTableId));

                XElement vendorPackages = new XElement("VendorPackages");
                foreach (var venPack in VendorPackages)
                {
                    vendorPackages.Add(new XElement("VendorPackage", venPack));
                }
                npcNode.Add(vendorPackages); //add VendorPackages to NPC

                XElement classSpec = new XElement("ClassSpec",
                    new XElement("Name", ClassSpec.Name,
                        new XAttribute("Id", ClassSpec.NameId)),
                    new XAttribute("Id", ClassSpec.Id),
                    //new XAttribute("Hash", ClassSpec.GetHashCode()),
                    new XElement("Fqn", ClassSpec.Fqn,
                        new XAttribute("Id", ClassSpec.Id)));

                classSpec.Add(new XElement("PlayerClass", ClassSpec.IsPlayerClass),
                        new XElement("AlignmentDark", ClassSpec.AlignmentDark),
                        new XElement("AlignmentLight", ClassSpec.AlignmentLight));

                classSpec.Add((ClassSpec.AbilityPackage ?? new AbilityPackage()).ToXElement());

                npcNode.Add(classSpec); //add ClassSpec to NPC
            }

            return npcNode;
        }
        #endregion
    }
}
