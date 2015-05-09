using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GomLib.Models;

namespace GomLib.ModelLoader
{
    public class QuestBranchLoader
    {
        public string ClassName
        {
            get { return "qstBranchDefinition"; }
        }

        DataObjectModel _dom;

        public QuestBranchLoader(DataObjectModel dom)
        {
            _dom = dom;
        }

        public QuestBranch Load(GomObjectData obj, Quest qst)
        {
            QuestBranch branch = new QuestBranch();

            branch.Quest = qst;
            branch.Id = obj.ValueOrDefault<long>("qstBranchId", 0);

            var qstSteps = obj.ValueOrDefault<List<object>>("qstSteps", null);
            branch.Steps = new List<QuestStep>();
            if (qstSteps != null)
            {
                foreach (var step in qstSteps)
                {
                    branch.Steps.Add(_dom.questStepLoader.Load((GomObjectData)step, branch));
                }
            }

            return branch;
        }
    }
}
