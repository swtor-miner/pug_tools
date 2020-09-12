using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GomLib.Models
{
    public class GroupFinderContentData
    {
        public Dictionary<long, GroupFinderContent> GroupFinderLookup { get; set; }

        [Newtonsoft.Json.JsonIgnore]
        readonly DataObjectModel _dom;

        public GroupFinderContentData(DataObjectModel dom)
        {
            _dom = dom;
        }

        public GroupFinderContent Load(int id)
        {
            if (GroupFinderLookup == null || GroupFinderLookup.Count == 0)
            {
                GroupFinderLookup = new Dictionary<long, GroupFinderContent>();
                var groupFinderContentDataPrototype = _dom.GetObject("groupFinderContentDataPrototype");
                Dictionary<object, object> grpFindTable = groupFinderContentDataPrototype.Data.Get<Dictionary<object, object>>("grpFindTable");
                groupFinderContentDataPrototype.Unload();
                var grpDailyQuestRewardsPrototype = _dom.GetObject("grpDailyQuestRewardsPrototype");
                Dictionary<object, object> grpFindRewardQuests = grpDailyQuestRewardsPrototype.Data.Get<Dictionary<object, object>>("grpFindRewardQuests");
                foreach (var kvp in grpFindTable)
                {
                    GroupFinderContent gfCont = new GroupFinderContent((long)kvp.Key);
                    var gom = kvp.Value as GomObjectData;
                    gfCont.SortIdx = (int)gom.ValueOrDefault<long>("grpFindSortIdx", 0); //sort index
                    var grpFindTime = gom.ValueOrDefault<List<object>>("grpFindTimes", null);
                    if (grpFindTime != null)
                    {
                        gfCont.Times = new List<GroupFinderTime>();
                        foreach (var subGom in grpFindTime)
                        {
                            var gommy = subGom as GomObjectData;
                            GroupFinderTime gfTime = new GroupFinderTime
                            {
                                RawStartTime = Convert.ToInt64(gommy.ValueOrDefault<object>("grpFindStartTime", 0))
                            };
                            gfTime.StartTime = GetTime(gfTime.RawStartTime);
                            gfTime.RawEndTime = Convert.ToInt64(gommy.ValueOrDefault<object>("grpFindEndTime", 0));
                            gfTime.EndTime = GetTime(gfTime.RawEndTime);
                            gfCont.Times.Add(gfTime);
                        }
                    }
                    var gfType = gom.ValueOrDefault("grpFindTypeEnum", new ScriptEnum());
                    gfCont.TypeId = gfType.Value;
                    StringTable table = _dom.stringTable.Find("str.sys.worldmap");
                    gfCont.NameId = gom.ValueOrDefault<long>("grpFindAreaName", 0);
                    gfCont.LocalizedName = table.GetLocalizedText(gfCont.NameId, "str.rdd");
                    gfCont.Name = table.GetText(gfCont.NameId, "str.rdd");
                    if (gfCont.LocalizedName == null)
                    {
                        table = _dom.stringTable.Find("str.rdd");
                        gfCont.LocalizedName = table.GetLocalizedText(gfCont.NameId, "str.rdd");
                        gfCont.Name = table.GetText(gfCont.NameId, "str.rdd");
                    }

                    gfCont.PhsId = gom.ValueOrDefault<long>("grpFindPhsIdLookup", 0);
                    gfCont.QuestLevel = (int)gom.ValueOrDefault<long>("grpFindQuestLevel", 0);

                    gfCont.IsImperialAvailable = gom.ValueOrDefault("grpFindImperialAvailable", false); //available imperial
                    gfCont.IsRepublicAvailable = gom.ValueOrDefault("grpFindRepublicAvailable", false); //available republic
                    gfCont.GroupSize = (int)gom.ValueOrDefault<long>("grpFindGroupSize", 0);
                    gfCont.MinLevel = (int)gom.ValueOrDefault<long>("grpFindMinLevel", 0);
                    gfCont.MaxLevel = (int)gom.ValueOrDefault<long>("grpFindMaxLevel", 0);
                    gfCont.MaxHealers = (int)gom.ValueOrDefault<long>("grpFindMaxNumHealers", 0);
                    gfCont.MaxTanks = (int)gom.ValueOrDefault<long>("grpFindMaxNumTanks", 0);
                    gfCont.MaxDPS = (int)gom.ValueOrDefault<long>("grpFindMaxNumDPS", 0);

                    gfCont.RewardQuestId = grpFindRewardQuests.Where(x => ((ScriptEnum)x.Key).Value == gfCont.TypeId).Select(x => (ulong)x.Value).FirstOrDefault();
                    if (gfCont.RewardQuestId != 0)
                    {
                        gfCont.RewardQuest = _dom.questLoader.Load(gfCont.RewardQuestId);
                    }
                    GroupFinderLookup.Add((long)kvp.Key, gfCont);
                }
            }

            if (!GroupFinderLookup.ContainsKey(id)) { return null; }
            else { return GroupFinderLookup[id]; }
        }

        private static DateTime GetTime(long milliseconds)
        {
            DateTime time = new DateTime(1601, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc);
            time = time.AddSeconds(milliseconds / 1000).ToLocalTime();
            return time;
        }
    }

    public class GroupFinderContent
    {
        public GroupFinderContent() { }
        public GroupFinderContent(long id)
        {
            Id = id;
        }

        public int SortIdx { get; set; }
        public long Id { get; set; }
        public long PhsId { get; set; }
        public int QuestLevel { get; set; }
        public int TypeId { get; set; }
        public bool IsImperialAvailable { get; set; }
        public bool IsRepublicAvailable { get; set; }
        public int MinLevel { get; set; }
        public int MaxLevel { get; set; }
        public int MaxHealers { get; set; }
        public int MaxTanks { get; set; }
        public int MaxDPS { get; set; }
        public int GroupSize { get; set; }
        public string Name { get; set; }
        public long NameId { get; set; }
        public Dictionary<string, string> LocalizedName { get; set; }
        public List<GroupFinderTime> Times { get; set; }
        public ulong RewardQuestId { get; set; }
        public Quest RewardQuest { get; set; }

        public override string ToString()
        {
            return Name;
        }
    }

    public class GroupFinderTime
    {
        public long RawStartTime { get; set; }
        public DateTime StartTime { get; set; }
        public long RawEndTime { get; set; }
        public DateTime EndTime { get; set; }
    }
}
