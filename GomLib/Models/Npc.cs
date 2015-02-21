using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;
using System.Xml;

namespace GomLib.Models
{
    public class Npc : GameObject, IEquatable<Npc>
    {
        public ulong parentSpecId { get; set; }
        public ulong NodeId { get; set; }
        public string Name { get; set; }
        public long NameId { get; set; }
        public Dictionary<string, string> LocalizedName { get; set; }
        public string Title { get; set; }
        public Dictionary<string, string> LocalizedTitle { get; set; }
        public ulong ClassId { get; set; }
        [Newtonsoft.Json.JsonIgnore]
        public ClassSpec ClassSpec { get; set; }
        public int MinLevel { get; set; }
        public int MaxLevel { get; set; }
        public Faction Faction { get; set; }
        public Toughness Toughness { get; set; }
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

        public ulong CodexId { get; set; }
        public Profession ProfessionTrained { get; set; }
        [Newtonsoft.Json.JsonIgnore]
        public Npc CompanionOverride {
            get { return _dom.npcLoader.Load(CompanionOverrideId); }
            set { CompanionOverride = value; }
        }
        public ulong CompanionOverrideId { get; set; }
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

        public Npc()
        {
            VendorPackages = new List<string>();
        }

        //public List<string> AbilityPackagesTrained { get; set; }

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
    }
}
