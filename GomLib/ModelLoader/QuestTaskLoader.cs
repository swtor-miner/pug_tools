using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GomLib.Models;

namespace GomLib.ModelLoader
{
    public class QuestTaskLoader
    {
        public string ClassName
        {
            get { return "qstTaskDefinition"; }
        }

        DataObjectModel _dom;

        public QuestTaskLoader(DataObjectModel dom)
        {
            _dom = dom;
        }

        public QuestTask Load(GomObjectData obj, QuestStep step)
        {
            QuestTask task = new QuestTask();

            task.Step = step;
            task.Id = (int)obj.ValueOrDefault<long>("qstTaskId", 0);
            task._dom = _dom;

            var qstTaskMapNoteList = obj.ValueOrDefault<List<object>>("qstTaskMapNoteList", null);
            if (qstTaskMapNoteList != null)
                task.MapNoteFqnList = qstTaskMapNoteList.Cast<string>().ToList();
            var bonusMissions = (List<object>)obj.ValueOrDefault<List<object>>("qstBonusMissions", null);
            task.BonusMissionsIds = _dom.questLoader.LoadBonusMissions(bonusMissions);

            task.CountMax = (int)obj.ValueOrDefault<long>("qstTaskCountMax", 0);
            task.ShowTracking = obj.ValueOrDefault<bool>("qstTaskShowTracking", false);
            task.ShowCount = obj.ValueOrDefault<bool>("qstTaskShowTrackingCount", false);
            task.Hook = obj.ValueOrDefault<string>("qstHook", null);
            long stringId = 0;
            long.TryParse(obj.ValueOrDefault<string>("qstTaskStringid", null), out stringId);

            var txtLookup = step.Branch.Quest.TextLookup;
            if (txtLookup.ContainsKey(stringId))
            {
                task.String = _dom.stringTable.TryGetString(step.Branch.Quest.Fqn, (GomObjectData)txtLookup[stringId]);
                task.LocalizedString = _dom.stringTable.TryGetLocalizedStrings(step.Branch.Quest.Fqn, (GomObjectData)txtLookup[stringId]);
                task.LocalizedString = Normalize.Dictionary(task.LocalizedString, task.String);
            }

            task.TaskQuests = new List<Quest>();
            task.TaskNpcs = new List<Npc>();
            var qstTaskObjects = (Dictionary<object, object>)obj.ValueOrDefault<Dictionary<object, object>>("qstTaskTriggerIdMap", null);
            if (qstTaskObjects != null)
            {
                foreach (var taskObj in qstTaskObjects)
                {
                    GomObject taskgom = _dom.GetObject((ulong)taskObj.Key);
                    if (taskgom == null) continue;
                    string fqn = taskgom.Name;
                    switch(fqn.Substring(0, 3)) {
                        case "qst":
                            if (task.TaskQuestIds == null)
                                task.TaskQuestIds = new List<ulong>();
                            task.TaskQuestIds.Add((ulong)taskObj.Key);
                            break;
                        case "npc":
                            if (task.TaskNpcIds == null)
                                task.TaskNpcIds = new List<ulong>();
                            task.TaskNpcIds.Add((ulong)taskObj.Key);
                            break;
                        case "plc":
                            if (task.TaskPlcIds == null)
                                task.TaskPlcIds = new List<ulong>();
                            task.TaskPlcIds.Add((ulong)taskObj.Key);
                            break;
                        case "enc":
                            //defeat encounter
                            break;
                        case "mpn":
                            //reach mapnote
                            break;
                        case "cos": //overhear convo?
                        case "spn": //shouldn't be here
                        case "itm": //shouldn't be here
                            if (!fqn.Contains("test") && fqn != "spn.location.voss.class.sith_warrior.the_voice_of_darkness.crefac_sel_makor_minions")
                                break;
                            break;
                        case "TIM":
                            break; //timer
                        default:
                            break;
                    }
                }
            }

            var itemsGiven = (List<object>)obj.ValueOrDefault<List<object>>("qstItemsGivenOnCompletion", null);
            task.ItemsGiven = _dom.questLoader.LoadGivenOrTakenItems(task.Step.Branch.Quest, itemsGiven);

            var itemsTaken = (List<object>)obj.ValueOrDefault<List<object>>("qstItemsTakenOnCompletion", null);
            task.ItemsTaken = _dom.questLoader.LoadGivenOrTakenItems(task.Step.Branch.Quest, itemsTaken);

            return task;
        }
    }
}
