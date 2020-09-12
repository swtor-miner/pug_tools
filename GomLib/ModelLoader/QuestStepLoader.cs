using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GomLib.Models;

namespace GomLib.ModelLoader
{
    public class QuestStepLoader
    {
        public string ClassName
        {
            get { return "qstStepDefinition"; }
        }

        readonly DataObjectModel _dom;

        public QuestStepLoader(DataObjectModel dom)
        {
            _dom = dom;
        }

        public QuestStep Load(GomObjectData obj, QuestBranch branch)
        {
            QuestStep step = new QuestStep
            {
                Dom_ = _dom,

                Id = (int)obj.ValueOrDefault<long>("qstStepId", 0),
                Branch = branch,
                IsShareable = obj.ValueOrDefault("qstStepIsShareable", false)
            };

            var bonusMissions = obj.ValueOrDefault<List<object>>("qstBonusMissions", null);
            step.BonusMissionsIds = _dom.questLoader.LoadBonusMissions(bonusMissions);

            step.Tasks = new List<QuestTask>();
            var tasks = obj.ValueOrDefault<List<object>>("qstTasks", null);
            if (tasks != null)
            {
                foreach (var taskDef in tasks)
                {
                    var tsk = _dom.questTaskLoader.Load((GomObjectData)taskDef, step);
                    step.Tasks.Add(tsk);
                }
            }

            var stringIds = obj.ValueOrDefault<List<object>>("qstStepJournalEntryStringIdList", null);
            var strings = new List<string>();
            var localizedStrings = new Dictionary<string, List<string>>();
            if (stringIds != null && branch.Quest.TextLookup != null)
            {
                var txtLookup = branch.Quest.TextLookup;
                foreach (var strId in stringIds)
                {
                    long key = (long)(ulong)strId;
                    if (txtLookup.ContainsKey(key))
                    {
                        strings.Add(_dom.stringTable.TryGetString(branch.Quest.Fqn, (GomObjectData)txtLookup[key]));
                        Dictionary<string, string> tempStrings = _dom.stringTable.TryGetLocalizedStrings(branch.Quest.Fqn, (GomObjectData)txtLookup[key]);
                        for (int i = 0; i < tempStrings.Count; i++)
                        {
                            if (!localizedStrings.ContainsKey(tempStrings.ElementAt(i).Key))
                            {
                                localizedStrings.Add(tempStrings.ElementAt(i).Key, new List<string>());
                            }
                            if (tempStrings.ElementAt(i).Value != "") { localizedStrings[tempStrings.ElementAt(i).Key].Add(tempStrings.ElementAt(i).Value); }
                        }
                    }
                    else
                    {
                        strings.Add(string.Empty);
                        if (localizedStrings.Count == 0)
                        {
                            localizedStrings.Add("enMale", new List<string>());
                            //localizedStrings.Add("enFemale", new List<string>());
                            localizedStrings.Add("frMale", new List<string>());
                            localizedStrings.Add("frFemale", new List<string>());
                            localizedStrings.Add("deMale", new List<string>());
                            localizedStrings.Add("deFemale", new List<string>());
                        }
                        localizedStrings["enMale"].Add(string.Empty);
                        //localizedStrings["enFemale"].Add(String.Empty);
                        localizedStrings["frMale"].Add(string.Empty);
                        localizedStrings["frFemale"].Add(string.Empty);
                        localizedStrings["deMale"].Add(string.Empty);
                        localizedStrings["deFemale"].Add(string.Empty);
                    }
                }
            }

            step.JournalText = string.Join(Environment.NewLine + Environment.NewLine, strings.ToArray());
            step.LocalizedJournalText = localizedStrings.ToDictionary(x => x.Key, x => string.Join(Environment.NewLine + Environment.NewLine, x.Value.ToArray()));

            var itemsGiven = obj.ValueOrDefault<List<object>>("qstItemsGivenOnCompletion", null);
            step.ItemsGiven = _dom.questLoader.LoadGivenOrTakenItems(step.Branch.Quest, itemsGiven);

            var itemsTaken = obj.ValueOrDefault<List<object>>("qstItemsTakenOnCompletion", null);
            step.ItemsTaken = _dom.questLoader.LoadGivenOrTakenItems(step.Branch.Quest, itemsTaken);

            return step;
        }
    }
}
