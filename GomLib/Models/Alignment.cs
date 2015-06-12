using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GomLib.Models
{
    public class AlignmentData
    {
        private Dictionary<string, Dictionary<int, AlignmentTier>> AlignmentTierLookup { get; set; }

        [Newtonsoft.Json.JsonIgnore]
        DataObjectModel _dom;

        public AlignmentData(DataObjectModel dom)
        {
            _dom = dom;
        }

        public AlignmentTier ToTier(int id)
        {
            if (id > 0) return ToLightTier(id);
            if (id < 0) return ToDarkTier(-id);
            return ToNeutralTier(id);
        }
        public AlignmentTier ToLightTier(int id)
        {
            if (AlignmentTierLookup == null) LoadData();

            if (!AlignmentTierLookup["Light"].ContainsKey(id)) { return null; }
            else { return AlignmentTierLookup["Light"][id]; }
        }
        public AlignmentTier ToDarkTier(int id)
        {
            if (AlignmentTierLookup == null) LoadData();

            if (!AlignmentTierLookup["Dark"].ContainsKey(id)) { return null; }
            else { return AlignmentTierLookup["Dark"][id]; }
        }
        public AlignmentTier ToNeutralTier(int id)
        {
            if (id > 0) throw new NotImplementedException();
            if (!AlignmentTierLookup["Neutral"].ContainsKey(id)) { return null; }
            else { return AlignmentTierLookup["Neutral"][id]; }
        }

        private void LoadData()
        {
            AlignmentTierLookup = new Dictionary<string, Dictionary<int, AlignmentTier>>();
            Dictionary<string, string> alignments = new Dictionary<string, string> { { "Light", "chrForceAlignmentLightTiers" }, { "Dark", "chrForceAlignmentDarkTiers" } };

            var chrForceAlignmentData = _dom.GetObject("chrForceAlignmentData");
            foreach (var align in alignments)
            {
                Dictionary<int, AlignmentTier> localDict = new Dictionary<int, AlignmentTier>();
                List<object> chrForceAlignmentTiers = chrForceAlignmentData.Data.Get<List<object>>(align.Value);
                for (int i = 0; i < chrForceAlignmentTiers.Count; i++)
                {
                    AlignmentTier socTier = LoadAlignmentTier(((GomLib.GomObjectData)chrForceAlignmentTiers[i]), i);
                    localDict.Add(i + 1, socTier);
                }
                AlignmentTierLookup.Add(align.Key, localDict);
            }

            Dictionary<int, AlignmentTier> neutralDict = new Dictionary<int, AlignmentTier>();
            GomObjectData chrForceAlignmentNeutralTier = (GomObjectData)chrForceAlignmentData.Data.Get<object>("chrForceAlignmentNeutralTier");
            AlignmentTier neutralTier = LoadAlignmentTier(chrForceAlignmentNeutralTier, 0);
            neutralDict.Add(0, neutralTier);
            AlignmentTierLookup.Add("Neutral", neutralDict);

            chrForceAlignmentData.Unload();
        }

        private AlignmentTier LoadAlignmentTier(GomObjectData gom, int i)
        {
            AlignmentTier socTier = new AlignmentTier(i);
            StringTable table = _dom.stringTable.Find("str.gui.alignment");
            socTier.Threshold = gom.ValueOrDefault<float>("chrForceAlignmentTierThreshold", 0f);
            socTier.NameId = gom.ValueOrDefault<long>("chrForceAlignmentTierName", 0) + 1331070494572544;
            socTier.LocalizedName = table.GetLocalizedText(socTier.NameId, "str.gui.alignment");
            socTier.Name = socTier.LocalizedName["enMale"];

            socTier.AchieveAppearance = gom.ValueOrDefault<ulong>("chrForceAlignmentTierAchieveAppearance", 0);
            socTier.AchieveMessageColor = gom.ValueOrDefault<string>("chrForceAlignmentTierAchieveMessageColor", "");
            socTier.AchieveMessageId = gom.ValueOrDefault<long>("chrForceAlignmentTierAchieveMessage", 0) + 1331070494572544;
            socTier.LocalizedAchieveMessage = table.GetLocalizedText(socTier.AchieveMessageId, "str.gui.tooltips");
            socTier.AchieveMessage = socTier.LocalizedAchieveMessage["enMale"];

            socTier.RelinquishAppearance = gom.ValueOrDefault<ulong>("chrForceAlignmentTierRelinquishAppearance", 0);
            socTier.RelinquishMessageColor = gom.ValueOrDefault<string>("chrForceAlignmentTierRelinquishMessageColor", "");
            socTier.RelinquishMessageId = gom.ValueOrDefault<long>("chrForceAlignmentTierRelinquishMessage", 0) + 1331070494572544;
            socTier.LocalizedRelinquishMessage = table.GetLocalizedText(socTier.RelinquishMessageId, "str.gui.tooltips");
            socTier.RelinquishMessage = socTier.LocalizedRelinquishMessage["enMale"];

            socTier.TitleId = gom.ValueOrDefault<long>("chrForceAlignmentTierTitleId", 0);
            return socTier;
        }
    }

    public class AlignmentTier
    {
        public AlignmentTier() { }
        public AlignmentTier(int id)
        {
            Id = id;
        }

        public int Id { get; set; }
        public float Threshold { get; set; }
        public string Name { get; set; }
        public long NameId { get; set; }
        public Dictionary<string, string> LocalizedName { get; set; }

        public ulong AchieveAppearance { get; set; }
        public string AchieveMessageColor { get; set; }
        public long AchieveMessageId { get; set; }
        public string AchieveMessage { get; set; }
        public Dictionary<string, string> LocalizedAchieveMessage { get; set; }

        public ulong RelinquishAppearance { get; set; }
        public string RelinquishMessageColor { get; set; }
        public long RelinquishMessageId { get; set; }
        public string RelinquishMessage { get; set; }
        public Dictionary<string, string> LocalizedRelinquishMessage { get; set; }

        public long TitleId { get; set; }

        public override string ToString()
        {
            return Name;
        }
    }
}
