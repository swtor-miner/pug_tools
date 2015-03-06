using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GomLib.Models;

namespace GomLib.ModelLoader
{
    public class AchievementLoader : IModelLoader
    {
        const long NameLookupKey = -2761358831308646330;
        const long DescLookupKey = 2806211896052149513;
        const long UnknownLookupKey = 814171245593979527;

        Dictionary<ulong, Achievement> idMap;
        Dictionary<string, Achievement> nameMap;
        Dictionary<string, Achievement> unknownMap;

        private DataObjectModel _dom;
        public AchievementLoader(DataObjectModel dom)
        {
            _dom = dom;
            if (nameMap == null)
            {
                Flush();
            }
        }

        public void Flush()
        {
            idMap = new Dictionary<ulong, Achievement>();
            nameMap = new Dictionary<string, Achievement>();
            unknownMap = new Dictionary<string, Achievement>();
        }

        public string ClassName
        {
            get { return "achAchievement"; }
        }

        public Models.Achievement Load(ulong nodeId)
        {
            Achievement result;
            if (idMap.TryGetValue(nodeId, out result))
            {
                return result;
            }

            GomObject obj = _dom.GetObject(nodeId);
            Models.Achievement ach = new Achievement();
            return Load(ach, obj);
        }


        public Models.Achievement Load(string fqn)
        {
            Achievement result;
            if (nameMap.TryGetValue(fqn, out result))
            {
                return result;
            }

            GomObject obj = _dom.GetObject(fqn);
            Models.Achievement ach = new Achievement();
            return Load(ach, obj);
        }

        public Models.Achievement Load(GomObject obj)
        {
            Models.Achievement ach = new Achievement();
            return Load(ach, obj);
        }

        public Models.GameObject CreateObject()
        {
            return new Models.Achievement();
        }

        /// <summary>Determine the string table to use for an Achievement given its FQN</summary>
        /// <param name="fqn">FQN of the Achievement</param>
        /// <returns>Possible string table FQN</returns>
        private string StringTablePath(string fqn)
        {
            string[] fqnParts = fqn.Split('.');

            // Return str.<first 3 fqn parts>
            int numParts = 3;
            if (fqnParts.Length < 3) { numParts = fqnParts.Length; }
            List<string> resultParts = new List<string>(4);
            resultParts.Add("str");
            for (var i = 0; i < numParts; i++)
            {
                resultParts.Add(fqnParts[i]);
            }

            return String.Join(".", resultParts.ToArray());
        }

        public Models.Achievement Load(Models.GameObject obj, GomObject gom)
        {
            if (gom == null) { return (Achievement)obj; }
            if (obj == null) { return null; }

            var ach = obj as Achievement;

            ach.Fqn = gom.Name;
            ach.NodeId = gom.Id;
            ach._dom = _dom;
            ach.References = gom.References;

            // Achievement Info
            ach.Icon = gom.Data.ValueOrDefault<string>("achIcon", null);
            _dom._assets.icons.Add(ach.Icon);

            //Wrong Way - When you read a value with Get or ValueOrDefault, you have to use the actual <type> the value is stored in
            //ach.Visibility = obj.Data.ValueOrDefault<AchievementVisibility>("achVisibility", AchievementVisibility.Always);//4611686344448990000

            //Right Way - Then once read you can cast to your own type like so:
            ach.Visibility = (AchievementVisibility)gom.Data.ValueOrDefault<ScriptEnum>("achVisibility", new ScriptEnum()).Value; //4611686344448990000
            //Or you could store the ScriptEnum in ach.Visibility and cast the value to your custom enum at output time.

            ach.AchId = gom.Data.ValueOrDefault<long>("achId", 0);

            ach.Rewards = new Rewards();
            if (gom.Data.ContainsKey("achRewardId"))
            {
                ach.RewardsId = gom.Data.Get<long>("achRewardId");
                GomObject rewardsTable = _dom.GetObject("achRewardsTable_Prototype");
                var rewardsLookupList = rewardsTable.Data.Get<Dictionary<object, object>>("achRewardsData"); //fix this to be a reference to somehwere so we don't have to load it each time to read one value.
                var rawRewards = (GomObjectData)rewardsLookupList[ach.RewardsId];
                var achievementPoints = rawRewards.Get<long>("achRewardPoints");
                ach.Rewards.AchievementPoints = achievementPoints;
                var cartelCoins = rawRewards.ValueOrDefault<long>("achRewardCartelCoins", 0);
                ach.Rewards.CartelCoins = cartelCoins;
                
                var legacyTitleField = rawRewards.ValueOrDefault<long>("achRewardLegacyTitleId", 0);
                ach.Rewards.LocalizedLegacyTitle = new Dictionary<string, string>();
                if ((long)legacyTitleField != 0)
                {
                        ach.Rewards.LocalizedLegacyTitle = LegacyTitleLookup(legacyTitleField);
                        ach.Rewards.LegacyTitle = ach.Rewards.LocalizedLegacyTitle["enMale"];
                }
                

                long requisition = rawRewards.ValueOrDefault<long>("achRewardFleetRequisition", 0);
                ach.Rewards.Requisition = requisition;
                /*string title = "";
                string codexFqn = "";
                bool prefix = false;
                if (titleField > 0 && titleField < 1000)
                {
                    GomObject titleTable = _dom.GetObject("chrPlayerTitlesTablePrototype");
                    var titleLookupList = titleTable.Data.Get<List<object>>("chrPlayerTitlesMapping");
                    var titleTextLookup = (GomObjectData)titleLookupList[Convert.ToInt32(titleField) - 1];
                    var titleId = titleTextLookup.ValueOrDefault<long>("titleDetailStringID", -1);
                    title = _dom.stringTable.TryGetString("str.pc.title", titleId);
                    var titleCodexId = titleTextLookup.ValueOrDefault<ulong>("titleCodex", 0);
                    GomObject codex = _dom.GetObject(titleCodexId);
                    prefix = titleTextLookup.ValueOrDefault<bool>("titleDetailLegacyPrefix", false);
                }
                else if (titleField > 1000)
                {
                    GomObject titleTable = _dom.GetObject("chrPlayerTitlesTablePrototype");
                    var titleLookupList = titleTable.Data.Get<List<object>>("chrPlayerTitlesMapping");
                    var titleTextLookup = (GomObjectData)titleLookupList[Convert.ToInt32(titleField)];
                    var titleId = titleTextLookup.ValueOrDefault<long>("titleDetailStringID", -1);
                    title = _dom.stringTable.TryGetString("str.pc.title", titleId);
                    var titleCodexId = titleTextLookup.ValueOrDefault<ulong>("titleCodex", 0);
                    GomObject codex = _dom.GetObject(titleCodexId);
                    prefix = titleTextLookup.ValueOrDefault<bool>("titleDetailLegacyPrefix", false);
                }*/

                //TODO: This is not working, no items are being read.
                var itemRew = rawRewards.Get<List<object>>("achRewardItems");
                ach.Rewards.ItemRewardList = new Dictionary<ulong, long>();
                foreach (var gomDat in itemRew)
                {
                    var quant = ((GomObjectData)gomDat).Get<long>("achRewardItemQty");
                    var itemId = ((GomObjectData)gomDat).Get<ulong>("achRewardItemId");
                    GomObject rew = _dom.GetObject(itemId);
                    /*if (rew.Name.Contains("itm.stronghold.") && !rew.Name.Contains(".trophy.") && !rew.Name.Contains("datacron_master_display")) //obsolete debugging code
                    {
                        string paushere = "";
                    }*/
                    ach.Rewards.ItemRewardList.Add(itemId, quant);
                }

                rewardsTable.Unload();
                rawRewards = null;
            }
            
            var textLookup = gom.Data.Get<Dictionary<object, object>>("locTextRetrieverMap");

            // Load Achievement Name
            var nameLookupData = (GomObjectData)textLookup[NameLookupKey];
            ach.NameId = nameLookupData.Get<long>("strLocalizedTextRetrieverStringID");
            ach.LocalizedName = _dom.stringTable.TryGetLocalizedStrings(ach.Fqn, nameLookupData);
            ach.Name = _dom.stringTable.TryGetString(ach.Fqn, nameLookupData);

            // Load Achievement Description
            var descLookupData = (GomObjectData)textLookup[DescLookupKey];
            ach.DescriptionId = descLookupData.Get<long>("strLocalizedTextRetrieverStringID");
            ach.LocalizedDescription = _dom.stringTable.TryGetLocalizedStrings(ach.Fqn, descLookupData);
            ach.Description = _dom.stringTable.TryGetString(ach.Fqn, descLookupData);

            var nonSpoilerData = (GomObjectData)textLookup[UnknownLookupKey];
            ach.nonSpoilerId = nonSpoilerData.Get<long>("strLocalizedTextRetrieverStringID");
            ach.LocalizedNonSpoilerDesc = _dom.stringTable.TryGetLocalizedStrings(ach.Fqn, nonSpoilerData);
            ach.NonSpoilerDesc = _dom.stringTable.TryGetString(ach.Fqn, nonSpoilerData);

            ach.Id = (ulong)(ach.NameId >> 32);

            //Conditions
            var conditionLookup = gom.Data.ValueOrDefault<List<object>>("achConditions", null);
            ach.Conditions = new List<AchCondition>();
            if (conditionLookup != null) 
            {
                foreach (var cond in conditionLookup)
                {
                    var condLookupData = (GomObjectData)cond;
                    var tmpCondition = new AchCondition();
                    tmpCondition.UnknownBoolean = condLookupData.ValueOrDefault<bool>("4611686294605190001", false); // is only set true on kill achievements
                                                                                                                     // All Unknown13 type achievements (except a test one) has this set true
                                                                                                                     // Player Faction restricted kill achievements have this set false

                    tmpCondition.Type = (AchConditionType)condLookupData.ValueOrDefault<ScriptEnum>("achConditionType", new ScriptEnum()).Value;
                                                                                                                    // 13 - player/non-faction npc kills
                                                                                                                    // 
                    tmpCondition.Target = (AchConditionTarget)condLookupData.ValueOrDefault<ScriptEnum>("achConditionTarget", new ScriptEnum()).Value;

                    //if (tmpCondition.Type == AchConditionType.Unknown13 || tmpCondition.Type == AchConditionType.Faction || tmpCondition.UnknownBoolean)
                        //break;
                    //TODO: read all the other condition fields

                    ach.Conditions.Add(tmpCondition);
                    //Console.WriteLine(ach.Fqn);
                }
            }

            // Initialize the Tasks
            var tasksLookup = gom.Data.Get<Dictionary<object, object>>("achTasks"); //add a task loader
            ach.Tasks = new List<AchTask>();

            foreach (var task in tasksLookup.Keys)
            {
                var taskLookupData = (GomObjectData)tasksLookup[task];
                var subtasks = taskLookupData.Get<Dictionary<object, object>>("achTaskSubtasks");
                var events = taskLookupData.Get<Dictionary<object, object>>("achTaskEvents");

                var taskName = "";
                var localizedTaskName = new Dictionary<string, string>();
                if (textLookup.ContainsKey(task))
                {
                    var taskNameObj = (GomObjectData)textLookup[task];
                     localizedTaskName = _dom.stringTable.TryGetLocalizedStrings(ach.Fqn, taskNameObj);
                     taskName = _dom.stringTable.TryGetString(ach.Fqn, taskNameObj);
                }

                //When achTaskSubtasks is empty:
                //- This task consists of only one task
                //- This task needs to be completed as many times as given in achTaskTotal
                //- Completing any of the events in achTaskEvents will count toward this task
                if (subtasks.Count == 0)
                {
                    var tmpTask = new AchTask();
                    tmpTask.Index = (long)task;
                    tmpTask.Count = taskLookupData.Get<long>("achTaskTotal");
                    tmpTask.Events = new List<AchEvent>();
                    foreach (var curEvent in events)
                    {
                        var tmpEvent = new AchEvent();
                        tmpEvent.NodeRef = (ulong)(long)curEvent.Key;
                        tmpEvent.checkNodeRef(_dom);
                        tmpEvent.Value = (long)curEvent.Value;
                        tmpTask.Events.Add(tmpEvent);
                    }
                    tmpTask.Name = taskName;
                    tmpTask.LocalizedNames = localizedTaskName;
                    ach.Tasks.Add(tmpTask);
                }
                //When achTaskSubtasks is not empty:
                //- This task consists of multiple subtasks
                //- Each entry in achTaskSubtasks stands for one subtask
                //- Each subtask also has an entry in achTaskEvents
                //- The values of achTaskSubtasks indicate the order of the subtasks
                //- Each entry in achTaskSubtasks needs to be completed exactly once
                //- achTaskTotal is a bitflag used by the game to check whether the achievement is completed
                //- achTaskObjectives may have more information to describe the subtask
                else
                {
                    foreach (var curSubtask in subtasks)
                    {
                        var tmpTask = new AchTask();
                        tmpTask.Index = (long)task;
                        tmpTask.Index2 = (long)curSubtask.Value;
                        tmpTask.Count = 1L;
                        tmpTask.Events = new List<AchEvent>();
                        var tmpEvent = new AchEvent();
                        tmpEvent.NodeRef = (ulong)(long)curSubtask.Key;
                        tmpEvent.checkNodeRef(_dom);
                        foreach (var curEvent in events)
                        {
                            if (curEvent.Key == curSubtask.Key)
                            {
                                tmpEvent.Value = (long)curEvent.Value;
                                break;
                            }
                        }
                        tmpTask.Events.Add(tmpEvent);
                        tmpTask.Name = taskName;
                        tmpTask.LocalizedNames = localizedTaskName;
                        ach.Tasks.Add(tmpTask);
                    }
                }
            }
            ach.Tasks = ach.Tasks.OrderBy(x => x.Index).ThenBy(x => x.Index2).ToList();

            gom.Unload();
            return ach;
        }

        private Dictionary<string, string> LegacyTitleLookup(long legacyTitleField)
        {
            GomObject titleTable = _dom.GetObject("lgcLegacyTitlesTablePrototype");
            var titleLookupList = titleTable.Data.Get<Dictionary<object, object>>("lgcLegacyTitlesData");
            object titleTextLookup;
            titleLookupList.TryGetValue(legacyTitleField, out titleTextLookup);
            if (titleTextLookup != null)
            {
                var titleId = ((GomObjectData)titleTextLookup).ValueOrDefault<long>("lgcLegacyTitleString");
                return _dom.stringTable.TryGetLocalizedStrings("str.pc.legacytitle", titleId);
            }
            else
                return new Dictionary<string, string> {
                                { "enMale", "" },
                                { "enFemale", "" },
                                { "frMale", "" },
                                { "frFemale", "" },
                                { "deMale", "" },
                                { "deFemale", "" },
                            };
        }

        public void LoadObject(Models.GameObject loadMe, GomObject obj)
        {
            GomLib.Models.Achievement ach = (Models.Achievement)loadMe;
            Load(ach, obj);
        }

        public void LoadReferences(Models.GameObject obj, GomObject gom)
        {
            // No references to load
        }
    }
}
