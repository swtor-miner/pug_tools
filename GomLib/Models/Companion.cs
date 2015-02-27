﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;

namespace GomLib.Models
{
    public class Companion : PseudoGameObject, IEquatable<Companion>
    {
        public List<CompanionAffectionRank> AffectionRanks { get; set; }
        public ClassSpecList Classes { get; set; }
        public float ConversationMultiplier { get; set; }
        public List<Talent> CrewAbilities { get; set; }
        public List<string> CrewPositions { get; set; }
        public string Description { get; set; }
        public string Faction { get; set; }
        public long FactionId { get; set; }
        public List<CompanionGiftInterest> GiftInterest { get; set; }
        public override long Id
        {
            get
            {
                return (long)uId;
            }
        }
        public bool IsGenderMale { get; set; }
        public bool IsRomanceable { get; set; }
        public Dictionary<string, string> LocalizedDescription { get; set; }
        //public string Name { get; set; }
        [Newtonsoft.Json.JsonIgnore]
        public Npc Npc { get; set; }
        public string Portrait { get; set; }
        public List<CompanionProfessionModifier> ProfessionModifiers { get; set; }
        [Newtonsoft.Json.JsonIgnore]
        public Ability SpaceAbility { get; set; }
        public ulong SpaceAbilityId { get; set; }
        public string SpaceIcon { get; set; }
        public ulong uId { get; set; }

        public override bool Equals(object obj)
        {
            if (obj == null) return false;

            if (ReferenceEquals(this, obj)) return true;

            Companion cmp = obj as Companion;
            if (cmp == null) return false;

            return Equals(cmp);
        }

        public bool Equals(Companion cmp)
        {
            if (cmp == null) return false;

            if (ReferenceEquals(this, cmp)) return true;

            if (this.AffectionRanks != null)
            {
                if (cmp.AffectionRanks != null)
                {
                    if (!this.AffectionRanks.SequenceEqual(cmp.AffectionRanks))
                        return false;
                }
            }
            if (!this.Classes.Equals(cmp.Classes, false))
                return false;
            if (this.ConversationMultiplier != cmp.ConversationMultiplier)
                return false;
            if (this.CrewAbilities != null)
            {
                if (cmp.CrewAbilities != null)
                {
                    if (!this.CrewAbilities.SequenceEqual(cmp.CrewAbilities))
                        return false;
                }
            }
            if (this.CrewPositions != null)
            {
                if (cmp.CrewPositions != null)
                {
                    if (!this.CrewPositions.SequenceEqual(cmp.CrewPositions))
                        return false;
                }
            }
            if (this.Description != cmp.Description)
                return false;
            if (this.Faction != cmp.Faction)
                return false;
            if (this.FactionId != cmp.FactionId)
                return false;
            if (this.GiftInterest != null)
            {
                if (cmp.GiftInterest != null)
                {
                    if (!this.GiftInterest.SequenceEqual(cmp.GiftInterest))
                        return false;
                }
            }
            if (this.Id != cmp.Id)
                return false;
            if (this.IsGenderMale != cmp.IsGenderMale)
                return false;
            if (this.IsRomanceable != cmp.IsRomanceable)
                return false;

            var ssComp = new DictionaryComparer<string, string>();
            if (!ssComp.Equals(this.LocalizedDescription, cmp.LocalizedDescription))
                return false;

            if (this.Name != cmp.Name)
                return false;
            if (!this.Npc.Equals(cmp.Npc))
                return false;
            if (this.Portrait != cmp.Portrait)
                return false;
            if (this.ProfessionModifiers != null)
            {
                if (cmp.ProfessionModifiers != null)
                {
                    if (!this.ProfessionModifiers.SequenceEqual(cmp.ProfessionModifiers))
                        return false;
                }
            }
            if (!this.SpaceAbility.Equals(cmp.SpaceAbility))
                return false;
            if (this.SpaceAbilityId != cmp.SpaceAbilityId)
                return false;
            if (this.SpaceIcon != cmp.SpaceIcon)
                return false;
            if (this.uId != cmp.uId)
                return false;
            return true;
        }

        public override XElement ToXElement(bool verbose)
        {
            XElement companion = new XElement("Companion");
            if (this.Id != 0)
            {
                companion.Add(new XAttribute("Id", uId),
                    new XElement("Name", Name),
                    new XElement("Description", Description),
                    new XElement("Faction", Faction));
                if (verbose)
                {
                    companion.Add(new XElement("Fqn", Npc.Fqn,
                        new XAttribute("Id", Npc.NodeId)),
                    new XElement("Potrait", Portrait));

                    int p = 1;
                    foreach (var prof in ProfessionModifiers)
                    {
                        companion.Add(new XElement("ProfessionModifier", new XAttribute("Id", p), new XElement("Name", prof.Stat), new XElement("Modifier", prof.Modifier)));
                        p++;
                    }

                    companion.Add(new XElement("ConversationMultiplier", ConversationMultiplier),
                        new XElement("IsRomanceable", IsRomanceable),
                        new XElement("IsGenderMale", IsGenderMale));

                    var giftDic = new Dictionary<string, string>();
                    foreach (var gift in GiftInterest)
                    {
                        if (giftDic.ContainsKey(gift.Reaction.ToString()))
                        {
                            string oldReactionList = giftDic[gift.Reaction.ToString()];
                            giftDic[gift.Reaction.ToString()] = oldReactionList + ", " + gift.GiftType.ToString();
                        }
                        else
                        {
                            giftDic.Add(gift.Reaction.ToString(), gift.GiftType.ToString());
                        }

                        if (giftDic.ContainsKey(gift.RomancedReaction.ToString()))
                        {
                            string oldReactionList = giftDic[gift.RomancedReaction.ToString()];
                            giftDic[gift.RomancedReaction.ToString()] = oldReactionList + ", " + gift.GiftType.ToString();
                        }
                        else
                        {
                            giftDic.Add(gift.RomancedReaction.ToString(), gift.GiftType.ToString());
                        }
                    }
                    int g = 1;
                    foreach (var reaction in giftDic)
                    {
                        companion.Add(new XElement("GiftReactions", new XAttribute("Id", reaction.Key), reaction.Value));
                        g++;
                    }

                    string affRanks = "";
                    if (AffectionRanks.Count > 0)
                    {
                        foreach (var affRank in AffectionRanks)
                        {
                            affRanks += affRank.Affection + ", ";
                        }
                        affRanks.Remove(affRanks.Length - 2);
                    }

                    XElement affectionRanks = new XElement("ConversationAffectionRanks", affRanks);
                    companion.Add(affectionRanks);

                    companion.Add(new XElement("SpaceIcon", SpaceIcon));
                    if (CrewPositions != null)
                    {
                        companion.Add(new XElement("CrewPositions", String.Join(", ", (List<string>)CrewPositions)));
                    }
                    else
                    {
                        companion.Add(new XElement("CrewPositions"));
                    }

                    string reqclasses = null;
                    if (Classes != null)
                    {
                        foreach (var reqclass in Classes)
                        {
                            reqclasses += reqclass.Name.ToString() + ", ";
                        }
                    }
                    if (reqclasses != null) { reqclasses = reqclasses.Substring(0, reqclasses.Length - 2); }
                    companion.Add(new XElement("Classes", reqclasses));
                    companion.Add(new XElement("SpaceAbility", new XAttribute("Id", 0), SpaceAbility.ToXElement(verbose)));
                    int s = 1;
                    foreach (var crwAbl in CrewAbilities)
                    {
                        companion.Add(new XElement("SpacePassive", new XAttribute("Id", s), crwAbl.ToXElement(verbose)));
                        s++;
                    }

                    companion.Add(Npc.ToXElement(verbose));
                }
            }
            return companion;
        }
    }
}
