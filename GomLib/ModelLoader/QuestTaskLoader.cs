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

            var bonusMissions = (List<object>)obj.ValueOrDefault<List<object>>("qstBonusMissions", null);
            task.BonusMissionsIds = _dom.questLoader.LoadBonusMissions(bonusMissions);

            task.CountMax = (int)obj.ValueOrDefault<long>("qstTaskCountMax", 0);
            task.ShowTracking = obj.ValueOrDefault<bool>("qstTaskShowTracking", false);
            task.ShowCount = obj.ValueOrDefault<bool>("qstTaskShowTrackingCount", false);
            task.Hook = QuestHookExtensions.ToQuestHook((string)obj.ValueOrDefault<string>("qstHook", null));
            long stringId = 0;
            long.TryParse(obj.ValueOrDefault<string>("qstTaskStringid", null), out stringId);

            var txtLookup = step.Branch.Quest.TextLookup;
            if (txtLookup.ContainsKey(stringId))
            {
                task.String = _dom.stringTable.TryGetString(step.Branch.Quest.Fqn, (GomObjectData)txtLookup[stringId]);
                task.LocalizedString = _dom.stringTable.TryGetLocalizedStrings(step.Branch.Quest.Fqn, (GomObjectData)txtLookup[stringId]);
            }

            task.TaskQuests = new List<Quest>();
            task.TaskNpcs = new List<Npc>();
            var qstTaskObjects = (Dictionary<object, object>)obj.ValueOrDefault<Dictionary<object, object>>("qstTaskObjects", null);
            if (qstTaskObjects != null)
            {
                foreach (var taskObj in qstTaskObjects)
                {
                    string fqn = (string)taskObj.Key;
                    if (fqn.StartsWith("qst."))
                    {
                        var qst = _dom.questLoader.Load(fqn);
                        task.TaskQuests.Add(qst);
                    }
                    else if (fqn.StartsWith("npc."))
                    {
                        var npc = _dom.npcLoader.Load(fqn);
                        task.TaskNpcs.Add(npc);
                    }
                    else if (fqn.StartsWith("plc."))
                    {
                        //add some code here
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
