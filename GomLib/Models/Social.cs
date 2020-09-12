﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GomLib.Models
{
    public class SocialTierData
    {
        private Dictionary<int, SocialTier> SocialTierLookup { get; set; }

        [Newtonsoft.Json.JsonIgnore]
        readonly DataObjectModel Dom_;

        public SocialTierData(DataObjectModel dom)
        {
            Dom_ = dom;
        }

        public SocialTier ToTier(int id)
        {
            if (SocialTierLookup == null)
            {
                SocialTierLookup = new Dictionary<int, SocialTier>();
                var chrSocialScoreTiersPrototype = Dom_.GetObject("chrSocialScoreTiersPrototype");
                List<object> chrSocialScoreTiers = chrSocialScoreTiersPrototype.Data.Get<List<object>>("chrSocialScoreTiers");
                chrSocialScoreTiersPrototype.Unload();
                StringTable table = Dom_.stringTable.Find("str.gui.tooltips");
                for (int i = 0; i < chrSocialScoreTiers.Count; i++)
                {
                    SocialTier socTier = new SocialTier(i)
                    {
                        Threshold = ((GomObjectData)chrSocialScoreTiers[i]).ValueOrDefault("chrSocialScoreTierThreshold", 0f)
                    };
                    string tierStringId = ((GomObjectData)chrSocialScoreTiers[i]).ValueOrDefault("chrSocialScoreTierName", "");
                    socTier.NameId = Convert.ToInt16(tierStringId.Split(':')[1]) + 836131348283392;
                    socTier.LocalizedName = table.GetLocalizedText(socTier.NameId, "str.gui.tooltips");
                    socTier.Name = socTier.LocalizedName["enMale"];

                    socTier.Appearance = ((GomObjectData)chrSocialScoreTiers[i]).ValueOrDefault("chrSocialScoreTierAchieveAppearance", "");
                    socTier.MessageColor = ((GomObjectData)chrSocialScoreTiers[i]).ValueOrDefault("chrSocialScoreTierAchieveMessageColor", "");

                    socTier.AchieveMessageId = ((GomObjectData)chrSocialScoreTiers[i]).ValueOrDefault<long>("chrSocialScoreTierAchieveMessageId", 0) + 836131348283566;
                    socTier.LocalizedAchieveMessage = table.GetLocalizedText(socTier.AchieveMessageId, "str.gui.tooltips");
                    socTier.AchieveMessage = socTier.LocalizedName["enMale"];

                    socTier.TitleId = ((GomObjectData)chrSocialScoreTiers[i]).ValueOrDefault<long>("chrSocialScoreTierTitleId", 0);

                    SocialTierLookup.Add(i + 1, socTier);
                }
                SocialTier outcast = new SocialTier(0)
                {
                    Threshold = -1,
                    NameId = 836131348283671
                };
                outcast.LocalizedName = table.GetLocalizedText(outcast.NameId, "str.gui.tooltips");
                outcast.Name = outcast.LocalizedName["enMale"];
                outcast.AchieveMessageId = 836131348283660;
                outcast.LocalizedAchieveMessage = table.GetLocalizedText(outcast.AchieveMessageId, "str.gui.tooltips");
                outcast.AchieveMessage = outcast.LocalizedAchieveMessage["enMale"];
                SocialTierLookup.Add(0, outcast);
            }

            if (!SocialTierLookup.ContainsKey(id)) { return null; }
            else { return SocialTierLookup[id]; }
        }
    }

    public class SocialTier
    {
        public SocialTier() { }
        public SocialTier(int id)
        {
            Id = id;
        }

        public int Id { get; set; }
        public float Threshold { get; set; }
        public string Name { get; set; }
        public long NameId { get; set; }
        public Dictionary<string, string> LocalizedName { get; set; }
        public string MessageColor { get; set; }
        public string Appearance { get; set; }
        public long AchieveMessageId { get; set; }
        public string AchieveMessage { get; set; }
        public Dictionary<string, string> LocalizedAchieveMessage { get; set; }
        public long TitleId { get; set; }

        public override string ToString()
        {
            return Name;
        }
    }
}
