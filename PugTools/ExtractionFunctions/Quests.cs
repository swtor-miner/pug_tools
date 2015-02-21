using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Net;
using System.Windows;
using System.Threading;
using Microsoft.Win32;
using MySql.Data.MySqlClient;
using System.Text.RegularExpressions;
using System.Xml;
using System.IO;
using System.Xml.Linq;
using System.Xml.Serialization;
using GomLib;

namespace tor_tools
{
    public partial class Tools
    {
        public void getQuest()
        {
            Clearlist2();

            LoadData();
            var itmList = currentDom.GetObjectsStartingWith("qst."); //.Where(obj => !obj.Name.StartsWith("qst.test."));
            double ttl = itmList.Count();

            bool append = false;
            bool addedChanged = false;
            string changed = "";
            if(chkBuildCompare.Checked)
            {
                addedChanged = true;
                changed = "Changed";
            }
            var filename = changed + "Quests.xml";
            if(outputTypeName == "Text")
            {
                filename = changed + "Quests.txt";
                var questContent = QuestDataFromFqnList(itmList);
                WriteFile(questContent, filename, append);
            }
            else
            {
                if (addedChanged)
                {
                    ProcessGameObjects("qst.", "Quests");
                }
                else
                {
                    var xmlContent = new XDocument(QuestDataFromFqnListAsXElement(itmList, addedChanged));
                    WriteFile(xmlContent, filename, append);
                }
            }

            //addtolist("The Quest lists has been generated there are " + ttl + " Quests");
            //MessageBox.Show("the Quest lists has been generated there are " + ttl + " Quests");
            EnableButtons();
        }

        private string QuestDataFromFqnList(IEnumerable<GomLib.GomObject> itmList)
        {
            double i = 0;
            string n = Environment.NewLine;
            var txtFile = new StringBuilder();
            foreach (var gomItm in itmList)
            {
                GomLib.Models.Quest itm = new GomLib.Models.Quest();
                currentDom.questLoader.Load(itm, gomItm);
                addtolist2("Quest Name: " + itm.Name);

                txtFile.Append("------------------------------------------------------------" + n);
                txtFile.Append("Quest Name: " + itm.Name + n);
                txtFile.Append("Quest NodeId: " + itm.NodeId + n);
                txtFile.Append("Quest Id: " + itm.Id + n);
                txtFile.Append("------------------------------------------------------------" + n);
                txtFile.Append("Quest INFO" + n);
                txtFile.Append("  IsBonus: " + itm.IsBonus + n);
                txtFile.Append("  BonusShareable: " + itm.BonusShareable + n);
                txtFile.Append("  Branches: " + itm.Branches.ToList().ToString() + n);
                txtFile.Append("  CanAbandon: " + itm.CanAbandon + n);
                txtFile.Append("  Category: " + itm.Category + n);
                txtFile.Append("  CategoryId: " + itm.CategoryId + n);
                txtFile.Append("  Classes: " + itm.Classes.ToList().ToString() + n);
                txtFile.Append("  Difficulty: " + itm.Difficulty + n);
                txtFile.Append("  Fqn: " + itm.Fqn + n);
                txtFile.Append("  Icon: " + itm.Icon + n);
                txtFile.Append("  IsClassQuest: " + itm.IsClassQuest + n);
                txtFile.Append("  IsHidden: " + itm.IsHidden + n);
                txtFile.Append("  IsRepeatable: " + itm.IsRepeatable + n);
                txtFile.Append("  Items: " + itm.Items + n);
                txtFile.Append("  RequiredLevel: " + itm.RequiredLevel + n);
                txtFile.Append("  XpLevel: " + itm.XpLevel + n);
                txtFile.Append("------------------------------------------------------------" + n + n);

                AddQuestToSQL(itm);
                i++;
            }
            addtolist("The Quest list has been generated there are " + i + " Quests");
            return txtFile.ToString();
        }

        private XElement QuestDataFromFqnListAsXElement(IEnumerable<GomLib.GomObject> itmList, bool addedChangedOnly)
        {
            double i = 0;

            XElement quests = new XElement("Quests");

            foreach (var gomItm in itmList)
            {
                GomLib.Models.Quest itm = new GomLib.Models.Quest();
                currentDom.questLoader.Load(itm, gomItm);

                addtolist2("Quest Name: " + itm.Name);
                if (itm.Fqn != null)
                {
                    //Console.WriteLine(gomItm.Name);
                    XElement questNode = QuestToXElement(itm);
                    questNode.Add(ReferencesToXElement(gomItm.References));
                    quests.Add(questNode); //add quest to quests
                    i++;
                }
                else
                {
                    continue;
                }
                
            }

            if (addedChangedOnly)
            {
                //addtolist("Comparing the Current Items to the loaded Patch");
                
                XElement addedItems = FindChangedEntries(quests, "Quests", "Quest");
                addedItems = SortQuests(addedItems);
                addtolist("The Quest list has been generated there are " + addedItems.Elements("Quest").Count() + " new/changed Quests");
                quests = null;
                return addedItems;
            }

            quests = SortQuests(quests);
            addtolist("The Quest list has been generated there are " + i + " Quests");
            return quests;
        }

        public static Dictionary<string, XElement> LoadedNpcs;
        public static Dictionary<string, XElement> LoadedQuests;

        public XElement QuestToXElement(GomObject gomItm)
        {
            return QuestToXElement(gomItm, false);
        }

        public XElement QuestToXElement(GomObject gomItm, bool overrideVerbose)
        {
            if (gomItm != null)
            {
                GomLib.Models.Quest itm = new GomLib.Models.Quest();
                currentDom.questLoader.Load(itm, gomItm);
                return QuestToXElement(itm, overrideVerbose);
            }
            return null;
        }

        private XElement QuestToXElement(GomLib.Models.Quest qst)
        {
            return QuestToXElement(qst, false);
        }

        private XElement QuestToXElement(GomLib.Models.Quest qst, bool overrideVerbose)
        {
            var questNode = new XElement("Quest", new XElement("Name", qst.Name),
                //new XAttribute("Name", itm.Name),
                new XElement("Fqn", qst.Fqn,
                    new XAttribute("NodeId", qst.NodeId)),
                new XAttribute("Id", qst.Id),
                new XElement("Category", qst.Category,
                    new XAttribute("Id", qst.CategoryId)),
                new XElement("RequiredLevel", qst.RequiredLevel),
                new XElement("XpLevel", qst.XpLevel));
            if (verbose && !overrideVerbose)
            {
                //Intialize our repeat XElement holders for this quest.
                LoadedNpcs = new Dictionary<string, XElement>();
                LoadedQuests = new Dictionary<string, XElement>();

                questNode.Add(
                    //new XAttribute("Hash", itm.GetHashCode()),
                new XElement("IsBonus", qst.IsBonus),
                new XElement("BonusShareable", qst.BonusShareable),
                new XElement("CanAbandon", qst.CanAbandon),
                new XElement("IsClassQuest", qst.IsClassQuest),
                new XElement("IsHidden", qst.IsHidden),
                new XElement("IsRepeatable", qst.IsRepeatable),
                new XElement("Difficulty", qst.Difficulty),
                new XElement("Icon", qst.Icon),
                new XElement("RequiredPrivacy", qst.ReqPrivacy));
                string classString = null;
                if (qst.Classes != null)
                {
                    foreach (var classy in qst.Classes)
                    {
                        classString += classy.Name + ", ";
                    }
                    if (classString != null) { classString = classString.Substring(0, classString.Length - 2); }
                }
                questNode.Add(new XElement("Classes", classString));

                XElement questItems = new XElement("Items");
                if (qst.Items != null)
                {
                    questItems.Add(new XAttribute("id", qst.Items.Count));
                    foreach (var item in qst.Items)
                    {
                        if (item.Value != null)
                        {
                            questItems.Add(ConvertToXElement(item.Value, true));
                        }
                    }
                }
                questNode.Add(questItems);


                //XElement rewards = new XElement("Rewards");
                //int r = 1;
                if (qst.Rewards != null)
                {
                    foreach (var rewardEntry in qst.Rewards.OrderBy(x => x.RewardItemId))
                    {
                        XElement reward = new XElement("Reward", new XAttribute("Id", rewardEntry.Id));
                        if (verbose)
                        {
                            reward.Add(new XElement("IsAlwaysProvided", rewardEntry.IsAlwaysProvided),
                            new XElement("UnknownNum", rewardEntry.UnknownNum),
                            new XElement("MinLevel", rewardEntry.MinLevel),
                            new XElement("MaxLevel", rewardEntry.MaxLevel));

                            XElement clas = new XElement("Classes");
                            foreach (var c in rewardEntry.Classes)
                            {
                                clas.Add(new XElement("Class", c.Name, new XAttribute("Id", c.Id)));
                            }
                            reward.Add(clas);
                        }
                        reward.Add(ConvertToXElement(rewardEntry.RewardItem, true));
                        questNode.Add(reward);
                        //r++;
                    }
                }
                //questNode.Add(rewards);

                foreach (var branch in qst.Branches)
                {
                    XElement branchNode = QuestBranchToXElement(branch);
                    questNode.Add(branchNode); //add branch to branches

                }
                //Trash our repeat XElement holders
                LoadedNpcs = null;
                LoadedQuests = null;
            }
            return questNode;
        }

        private XElement QuestBranchToXElement(GomLib.Models.QuestBranch branch)
        {
            XElement branchNode = new XElement("Branch",
                new XAttribute("Id", branch.Id)); //,
            //new XAttribute("Hash", branch.GetHashCode()));
            /*if (verbose)
            {
                XElement rewardAll = new XElement("RewardAll"); //need to fix the questloader to actually load these from the questrewards prototypes.
                if (branch.RewardAll != null)
                {
                    foreach (var reward in branch.RewardAll) { rewardAll.Add(new XElement("Reward", new XAttribute("Name", reward.Name))); }
                }

                XElement rewardOne = new XElement("RewardOne"); //need to fix the questloader to actually load these from the questrewards prototypes.
                if (branch.RewardOne != null)
                {
                    foreach (var reward in branch.RewardOne) { rewardOne.Add(new XElement("Reward", new XAttribute("Name", reward.Name))); }
                }

                branchNode.Add(rewardAll, rewardOne);
            }*/
            foreach (var step in branch.Steps)
            {
                XElement stepNode = QuestStepToXElement(step);
                branchNode.Add(stepNode); //add step to steps
            }
            return branchNode;
        }

        private XElement QuestStepToXElement(GomLib.Models.QuestStep step)
        {
            XElement stepNode = new XElement("Step", new XElement("JournalText", step.JournalText.Replace(Environment.NewLine, "<br>")),
                new XAttribute("Id", step.Id),
                new XElement("Shareable", step.IsShareable),
                new XElement("ItemsTaken"));
            //new XAttribute("DBId", step.DbId)); //this is always 0
            foreach (var task in step.Tasks)
            {
                XElement taskNode = QuestTaskToXElement(task);
                stepNode.Add(taskNode); //add task to tasks
            }

            if (verbose)
            {
                QuestItemsGivenOrTakenToXElement(stepNode, step.ItemsGiven, step.ItemsTaken);
            }

            stepNode.Add(new XElement("BonusMissions"));
            if (step.BonusMissions.Count > 0)
            {
                foreach (var bonus in step.BonusMissions)
                {
                    stepNode.Element("BonusMissions").Add(QuestToXElement(bonus));
                }
            }

            return stepNode;
        }

        private void QuestItemsGivenOrTakenToXElement(XElement questNode, List<GomLib.Models.QuestItem> givenItems, List<GomLib.Models.QuestItem> takenItems)
        {
            XElement itemsGiven = QuestItemListToXElement("ItemsGiven", givenItems);
            XElement itemsTaken = QuestItemListToXElement("itemsTaken", takenItems);

            questNode.Add(itemsGiven, itemsTaken);
        }

        private XElement QuestItemListToXElement(string elementName, List<GomLib.Models.QuestItem> Items)
        {
            XElement itemsElement = new XElement(elementName);
            if (Items != null)
            {
                if (Items.Count != 0)
                {
                    itemsElement.Add(new XAttribute("id", Items.Count));
                    for (var i = 0; i < Items.Count; i++)
                    {
                        var item = Items.ElementAt(i);
                        XElement questItem = ConvertToXElement(item, true);
                        itemsElement.Add(questItem);
                    }
                }
            }
            return itemsElement;
        }

        public XElement QuestItemToXElement(GomLib.Models.QuestItem item)
        {
            return QuestItemToXElement(item, false);
        }

        public XElement QuestItemToXElement(GomLib.Models.QuestItem item, bool overrideVerbose)
        {
            XElement questItem = new XElement("QuestItem",
                new XElement("Name", item.Name),
                new XElement("MaxCount", item.MaxCount),
                new XElement("Min", item.Min),
                new XElement("Max", item.Max),
                new XElement("GUID", item.GUID),
                new XAttribute("Id", item.VariableId),
                new XElement("UnknownLong", item.UnknownLong));
            questItem.Add(ConvertToXElement(item.Id, item._dom, overrideVerbose));
            return questItem;
        }

        private XElement QuestTaskToXElement(GomLib.Models.QuestTask task)
        {
            XElement taskNode = new XElement("Task",
                new XAttribute("Id", task.Id));
            //new XAttribute("DBId", task.DbId)); //this is always 0
            if (task.String != null)
                taskNode.Add(new XElement("String", task.String));
            else
                taskNode.Add(new XElement("String"));

            taskNode.Add(new XElement("CountMax", task.CountMax));

            QuestItemsGivenOrTakenToXElement(taskNode, task.ItemsGiven, task.ItemsTaken);

            taskNode.Add(new XElement("BonusMissions"));
            if (task.BonusMissions != null)
            {
                foreach (var bonus in task.BonusMissions)
                {
                    taskNode.Element("BonusMissions").Add(QuestToXElement(bonus));
                }
            }

            if (verbose)
            {
                taskNode.Add(new XElement("Hook", task.Hook),
                    new XElement("ShowCount", task.ShowCount),
                    new XElement("ShowTracking", task.ShowTracking));
                XElement taskNpcs = new XElement("TaskNpcs");
                foreach (var npc in task.TaskNpcs)
                {
                    if (LoadedNpcs.ContainsKey(npc.Fqn))
                    {
                        taskNpcs.Add(LoadedNpcs[npc.Fqn]);
                    }
                    else
                    {
                        XElement taskNpc = ConvertToXElement(npc, true);
                        taskNpcs.Add(taskNpc); //add task npc to task npcs
                    }
                }
                taskNode.Add(taskNpcs); //add task npcs to task
                XElement taskQuests = new XElement("TaskQuests");
                foreach (var quest in task.TaskQuests)
                {
                    XElement taskQuest = ConvertToXElement(quest, true);
                    taskQuests.Add(taskQuest); //add task quest to task quests
                }
                taskNode.Add(taskQuests); //add task quests to task
            }
            return taskNode;
        }

        private XElement SortQuests(XElement quests)
        {
            //addtolist("Sorting Quests");
            quests.ReplaceNodes(quests.Elements("Quest")
                .OrderBy(x => (string)x.Attribute("Status"))
                .ThenBy(x => (string)x.Element("Fqn"))
                .ThenBy(x => (string)x.Attribute("Id")));
            return quests;
        }

        private void AddQuestToSQL(GomLib.Models.Quest itm)
        {
            string name = itm.Name.Replace("'", "''");
            sqlexec("INSERT INTO `quest` (`quest_name`, `quest_nodeid`, `quest_id`, `IsBonus`, `BonusShareable`, `Branches`, `CanAbandon`, `Category`, `CategoryId`, `Classes`, `Difficulty`, `Fqn`, `Icon`, `IsClassQuest`, `IsHidden`, `IsRepeatable`, `Items`, `RequiredLevel`, `XpLevel`) VALUES ('" + name + "', '" + itm.NodeId + "', '" + itm.Id + "', '" + itm.IsBonus + "', '" + itm.BonusShareable + "', '" + itm.Branches + "', '" + itm.CanAbandon + "', '" + itm.Category + "', '" + itm.CategoryId + "', '" + itm.Classes + "', '" + itm.Difficulty + "', '" + itm.Fqn + "', '" + itm.Icon + "', '" + itm.IsClassQuest + "', '" + itm.IsHidden + "', '" + itm.IsRepeatable + "', '" + itm.Items + "', '" + itm.RequiredLevel + "', '" + itm.XpLevel + "');");
        }
    }
}