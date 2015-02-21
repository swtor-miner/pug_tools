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
        private string ConversationToText(GomLib.Models.Conversation itm, bool overrideVerbose)
        {
            StringBuilder bld = new StringBuilder();
            string n = Environment.NewLine;
            bld.Append(String.Format("Conversation - {0}{1}", itm.Fqn, n));
            foreach (var dialogNode in itm.DialogNodes)
            {
                if (itm.SpeakersIds.Contains(dialogNode.SpeakerId))
                {
                    ulong speakerId = dialogNode.SpeakerId;
                    if (speakerId == 0)
                    {
                        speakerId = itm.DefaultSpeakerId;
                    }
                    if (itm.Speakers[speakerId] != null)
                    {
                        switch (itm.Speakers[speakerId].Fqn.Substring(0, 3))
                        {
                            case "npc":
                                string name = ((GomLib.Models.Npc)itm.Speakers[speakerId]).Name;
                                if (name.Length == 0) { name = itm.Speakers[speakerId].Fqn; }
                                bld.Append(String.Format("{0}: {1} - {2}{3}", dialogNode.NodeId, name, dialogNode.Text, n));
                                continue;
                            case "plc":
                                string plcName = ((GomLib.Models.Placeable)itm.Speakers[speakerId]).Name;
                                if (plcName.Length == 0) { plcName = itm.Speakers[speakerId].Fqn; }
                                bld.Append(String.Format("{0}: {1} - {2}{3}", dialogNode.NodeId, plcName, dialogNode.Text, n));
                                continue;
                            default:
                                throw new Exception("Unaccounted for speaker type");
                        }
                    }

                }
                bld.Append(String.Format("{0}: {1}{2}", dialogNode.NodeId, dialogNode.Text, n));
            }
            return bld.ToString();
        }

        public XElement ConversationToXElement(GomObject gomItm)
        {
            return ConversationToXElement(gomItm, false);
        }
        public XElement ConversationToXElement(GomObject gomItm, bool overrideVerbose)
        {
            if (gomItm != null)
            {
                GomLib.Models.Conversation itm = new GomLib.Models.Conversation();
                currentDom.conversationLoader.Load(itm, gomItm);
                gomItm.Unload(); // These GomObjects are staying loaded in memory and cause our massive memory issues.
                return ConversationToXElement(itm, overrideVerbose);
            }
            return null;
        }
        private XElement ConversationToXElement(GomLib.Models.Conversation itm)
        {
            return ConversationToXElement(itm, false);
        }
        private XElement ConversationToXElement(GomLib.Models.Conversation itm, bool overrideVerbose)
        {
            addtolist2("Conversation Name: " + itm.Fqn);
            XElement conversation = new XElement("Conversation", new XAttribute("Id", itm.Id));
            conversation.Add(new XElement("Fqn", itm.Fqn));
            if (!overrideVerbose)
            {
                XElement dialogNodes = new XElement("DialogNodes");
                if (itm.RootNodes != null)
                {
                    foreach (var rootNodeKvp in itm.RootNodes)
                    {
                        var childs = new List<int>();
                        dialogNodes.Add(DialogNodeToXElement(ref itm, ref childs, rootNodeKvp.Value));
                    }
                }
                conversation.Add(dialogNodes); //add classes to codex
                /*XElement speakers = new XElement("Speakers");
                foreach (var speaker in itm.Speakers)
                {
                    if (speaker.Value != null)
                    {
                        switch (speaker.Value.Fqn.Substring(0, 3))
                        {
                            case "npc":
                                speakers.Add(new XElement("Speaker", new XAttribute("Id", speaker.Key >> 32), ((GomLib.Models.Npc)speaker.Value).Name + " - " + speaker.Value.Fqn));
                                break;
                            case "plc":
                                speakers.Add(new XElement("Speaker", new XAttribute("Id", speaker.Key >> 32), ((GomLib.Models.Placeable)speaker.Value).Name + " - " + speaker.Value.Fqn));
                                break;
                            default:
                                throw new Exception("Unaccounted for speaker type");
                        }
                    }
                    else
                    {
                        speakers.Add(new XElement("Speaker", new XAttribute("Id", speaker.Key)));
                    }
                }
                conversation.Add(speakers);
                XElement questStarted = new XElement("QuestStarted");
                foreach (var quest in itm.QuestStarted)
                {
                    questStarted.Add(new XElement("Quest", new XElement("Name", quest.Name),
                                new XElement("Fqn", quest.Fqn,
                                new XAttribute("NodeId", quest.NodeId)),
                                new XAttribute("Id", quest.Id)));
                }
                conversation.Add(questStarted);

                XElement questProgressed = new XElement("QuestProgressed");
                foreach (var quest in itm.QuestProgressed)
                {
                    questProgressed.Add(new XElement("Quest", new XElement("Name", quest.Name),
                                new XElement("Fqn", quest.Fqn,
                                new XAttribute("NodeId", quest.NodeId)),
                                new XAttribute("Id", quest.Id)));
                }
                conversation.Add(questProgressed);

                XElement questEnded = new XElement("QuestEnded");
                foreach (var quest in itm.QuestEnded)
                {
                    questEnded.Add(new XElement("Quest", new XElement("Name", quest.Name),
                                new XElement("Fqn", quest.Fqn,
                                new XAttribute("NodeId", quest.NodeId)),
                                new XAttribute("Id", quest.Id)));
                }
                conversation.Add(questEnded);*/
                //conversation.Add(new XElement("QuestStarted", itm.QuestStarted));
                //sqlexec("INSERT INTO `conversations` (`conversation_idc`, `quest_name`, `quest_nodeid`, `quest_id`, `IsBonus`, `BonusShareable`, `Branches`, `CanAbandon`, `Category`, `CategoryId`, `Classes`, `Difficulty`, `Fqn`, `Icon`, `IsClassQuest`, `IsHidden`, `IsRepeatable`, `Items`, `RequiredLevel`, `XpLevel`) VALUES (NULL, '" + name + "', '" + itm.NodeId + "', '" + itm.Id + "', '" + itm.IsBonus + "', '" + itm.BonusShareable + "', '" + itm.Branches + "', '" + itm.CanAbandon + "', '" + itm.Category + "', '" + itm.CategoryId + "', '" + itm.Classes + "', '" + itm.Difficulty + "', '" + itm.Fqn + "', '" + itm.Icon + "', '" + itm.IsClassQuest + "', '" + itm.IsHidden + "', '" + itm.IsRepeatable + "', '" + itm.Items + "', '" + itm.RequiredLevel + "', '" + itm.XpLevel + "');");
            }
            return conversation;

        }

        private XElement DialogNodeToXElement(ref GomLib.Models.Conversation itm, ref List<int> childs, long id)
        {
            if (itm.NodeLinkList.ContainsKey(id))
            {
                return new XElement("LinkToDialogNode", new XAttribute("Id", id), itm.NodeLinkList[id]);
            }
            else if (itm.NodeLookup.ContainsKey(id))
            {
                XElement dNode = new XElement("DialogNode", new XAttribute("Id", id));
                GomLib.Models.DialogNode dialogNode = itm.NodeLookup[id];
                if (true) //!childs.Contains((int)id))
                {
                    if (itm.SpeakersIds.Contains(dialogNode.SpeakerId))
                    {
                        ulong speakerId = dialogNode.SpeakerId;
                        if (speakerId == 0)
                        {
                            speakerId = itm.DefaultSpeakerId;
                        }
                        if (itm.Speakers[speakerId] != null)
                        {
                            switch (itm.Speakers[speakerId].Fqn.Substring(0, 3))
                            {
                                case "npc":
                                    string name = ((GomLib.Models.Npc)itm.Speakers[speakerId]).Name;
                                    if (name.Length == 0) { name = itm.Speakers[speakerId].Fqn; }
                                    dNode.Add(new XElement("Speaker", new XAttribute("Id", ((GomLib.Models.Npc)itm.Speakers[speakerId]).NodeId >> 32), name));
                                    break;
                                case "plc":
                                    string plcName = ((GomLib.Models.Placeable)itm.Speakers[speakerId]).Name;
                                    if (plcName.Length == 0) { plcName = itm.Speakers[speakerId].Fqn; }
                                    dNode.Add(new XElement("Speaker", new XAttribute("Id", ((GomLib.Models.Placeable)itm.Speakers[speakerId]).NodeId >> 32), plcName));
                                    break;
                                default:
                                    throw new Exception("Unaccounted for speaker type");
                            }
                        }

                    }
                    dNode.Add(new XElement("Text", dialogNode.Text));
                    if (dialogNode.MinLevel != -1) { dNode.Add(new XElement("MinLevel", dialogNode.MinLevel)); }
                    if (dialogNode.MaxLevel != -1) { dNode.Add(new XElement("MaxLevel", dialogNode.MaxLevel)); }
                    if (dialogNode.IsEmpty) { dNode.Add(new XElement("IsEmpty", dialogNode.IsEmpty)); }
                    if (dialogNode.IsAmbient) { dNode.Add(new XElement("IsAmbient", dialogNode.IsAmbient)); }
                    if (dialogNode.IsPlayerNode) { dNode.Add(new XElement("IsPlayerNode", dialogNode.IsPlayerNode)); }
                    if (dialogNode.JoinDisabledForHolocom) { dNode.Add(new XElement("JoinDisabledForHolocom", dialogNode.JoinDisabledForHolocom)); }
                    if (dialogNode.ChoiceDisabledForHolocom) { dNode.Add(new XElement("ChoiceDisabledForHolocom", dialogNode.ChoiceDisabledForHolocom)); }
                    if (dialogNode.AbortsConversation) { dNode.Add(new XElement("AbortsConversation", dialogNode.AbortsConversation)); }
                    if (dialogNode.ActionHook.ToString() != "None") { dNode.Add(new XElement("ActionHook", dialogNode.ActionHook.ToString())); }
                    if (dialogNode.ActionQuest != 0)
                    {
                        /*dNode.Add(new XElement("ActionQuest", new XElement("Name", dialogNode.ActionQuest.Name),
                            new XElement("Fqn", dialogNode.ActionQuest.Fqn,
                            new XAttribute("NodeId", dialogNode.ActionQuest.NodeId)),
                            new XAttribute("Id", dialogNode.ActionQuest.Id)));*/
                        dNode.Add(new XElement("ActionQuest", new XAttribute("Id", dialogNode.ActionQuest)));
                    }
                    if (dialogNode.QuestReward != 0)
                    {
                        /*XElement rewardNode = new XElement("QuestReward", new XElement("Name", dialogNode.QuestReward.Name),
                            new XElement("Fqn", dialogNode.QuestReward.Fqn,
                            new XAttribute("NodeId", dialogNode.QuestReward.NodeId)),
                            new XAttribute("Id", dialogNode.QuestReward.Id));
                        dNode.Add(rewardNode);*/
                        dNode.Add(new XElement("QuestReward", new XAttribute("Id", dialogNode.QuestReward)));
                    }
                    if (dialogNode.QuestsGranted.Count != 0)
                    {
                        XElement questsGranted = new XElement("QuestsGranted", new XAttribute("Id",dialogNode.QuestsGranted.Count));
                        for (int i = 0; i < dialogNode.QuestsGranted.Count; i++)
                        {
                            /*questsGranted.Add(new XElement("Quest", new XElement("Name", dialogNode.QuestsGranted[i].Name),
                                new XElement("Fqn", dialogNode.QuestsGranted[i].Fqn,
                                new XAttribute("NodeId", dialogNode.QuestsGranted[i].NodeId)),
                                new XAttribute("Id", dialogNode.QuestsGranted[i].Id)));*/
                            questsGranted.Add(new XElement("Quest", new XAttribute("Id", dialogNode.QuestsGranted[i])));
                        }
                        dNode.Add(questsGranted);
                    }
                    if (dialogNode.QuestsEnded.Count != 0)
                    {
                        XElement questsEnded = new XElement("QuestsEnded", new XAttribute("Id", dialogNode.QuestsEnded.Count));
                        for (int i = 0; i < dialogNode.QuestsEnded.Count; i++)
                        {
                            /*questsEnded.Add(new XElement("Quest", new XElement("Name", dialogNode.QuestsEnded[i].Name),
                                new XElement("Fqn", dialogNode.QuestsEnded[i].Fqn,
                                new XAttribute("NodeId", dialogNode.QuestsEnded[i].NodeId)),
                                new XAttribute("Id", dialogNode.QuestsEnded[i].Id)));*/
                            questsEnded.Add(new XElement("Quest", new XAttribute("Id", dialogNode.QuestsEnded[i])));
                        }
                        dNode.Add(questsEnded);
                    }
                    if (dialogNode.QuestsProgressed.Count != 0)
                    {
                        XElement questsProgressed = new XElement("QuestsProgressed", new XAttribute("Id", dialogNode.QuestsProgressed.Count));
                        for (int i = 0; i < dialogNode.QuestsProgressed.Count; i++)
                        {
                            /*questsProgressed.Add(new XElement("Quest", new XElement("Name", dialogNode.QuestsProgressed[i].Name),
                                new XElement("Fqn", dialogNode.QuestsProgressed[i].Fqn,
                                new XAttribute("NodeId", dialogNode.QuestsProgressed[i].NodeId)),
                                new XAttribute("Id", dialogNode.QuestsProgressed[i].Id)));*/
                            questsProgressed.Add(new XElement("Quest", new XAttribute("Id", dialogNode.QuestsProgressed[i])));
                        }
                        dNode.Add(questsProgressed);
                    }
                    if (dialogNode.AlignmentGain.ToString() != "None") { dNode.Add(new XElement("AlignmentGain", dialogNode.AlignmentGain.ToString().Replace("Small","+50 ")
                                                                                                                                       .Replace("Medium","+100 ")
                                                                                                                                       .Replace("Large","+150 ")
                                                                                                                                       .Replace("Mega","+200 ")));}
                    for (int i = 0; i < dialogNode.AffectionRewards.Count; i++)
                    {
                        dNode.Add(new XElement("AffectionGain", new XAttribute("Id", i), dialogNode.AffectionRewards.ElementAt(i).Key.Fqn + " - " + dialogNode.AffectionRewards.ElementAt(i).Value.ToString()));
                    }

                    if (dialogNode.ChildIds != null)
                    {
                        foreach (var child in dialogNode.ChildIds)
                        {
                            dNode.Add(new XElement(DialogNodeToXElement(ref itm, ref childs, child)));
                        }
                    }
                    childs.Add((int)id);
                }
                return dNode;
            }
            else
            {
                return new XElement("DialogNode", new XAttribute("Id", "notFound" + id));
            }
        }

        private XElement SortConversations(XElement items)
        {
            //addtolist("Sorting Items");
            items.ReplaceNodes(items.Elements("Conversation")
                .OrderBy(x => (string)x.Attribute("Fqn")));
            return items;
        }
    }
}
