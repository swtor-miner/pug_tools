﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Net;
using System.Windows;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Win32;
using MySql.Data.MySqlClient;
using System.Text.RegularExpressions;
using System.Xml;
using System.IO;
using System.Xml.Linq;
using System.Xml.Serialization;

namespace GomLib
{
    public static class Smart
    {
        //public Smart(Action<string> _MessageDelegate)
        //{
        //    MessageDelegate = _MessageDelegate;
        //}
        //public Action<string> MessageDelegate { get; set; }

        //public void InvokeMessage(String mess)
        //{
        //    if (MessageDelegate != null)
        //    {
        //        MessageDelegate(mess);
        //    }
        //}

        public static void Link(DataObjectModel dom, Action<string> MessageDelegate)
        {
           LinkAbilityPackages(dom, MessageDelegate);
           LinkAchievements(dom, MessageDelegate);
           LinkCodex(dom, MessageDelegate);
           LinkConversations(dom, MessageDelegate);
           LinkDecorations(dom, MessageDelegate);
           LinkEncounters(dom, MessageDelegate);
           LinkItems(dom, MessageDelegate);
           LinkItemAppearances(dom, MessageDelegate);
           LinkNpcs(dom, MessageDelegate);
           LinkPhases(dom, MessageDelegate);
           LinkPlaceables(dom, MessageDelegate);
           LinkQuests(dom, MessageDelegate);
           LinkQuestRewards(dom, MessageDelegate);
           LinkSchematics(dom, MessageDelegate);
           LinkSpawners(dom, MessageDelegate);
        }

        public static void LinkAbilityPackages(DataObjectModel dom, Action<string> MessageDelegate)
        {
            List<GomObject> nodeList;
            MessageDelegate("Smart-linking ability packages (NPCs)...");//Load apns
            nodeList = dom.GetObjectsStartingWith("apn.");
            foreach (GomObject node in nodeList)
            {
                Dictionary<object, object> abilities = node.Data.ValueOrDefault<Dictionary<object, object>>("ablPackageAbilitiesList", null);
                if (abilities != null)
                {
                    foreach (KeyValuePair<object, object> ability in abilities)
                    {
                        dom.AddCrossLink((ulong)ability.Key, "partOfApn", node.Id);//abl node
                    }
                }
                node.Unload();
            }
            //add apc code here
            MessageDelegate("Smart-linking classes...");//Load classes
            nodeList = dom.GetObjectsStartingWith("class.");
            Dictionary<ulong, GomLib.Models.AdvancedClass> LoadedACs = new Dictionary<ulong, GomLib.Models.AdvancedClass>();
            foreach (GomObject node in nodeList)
            {
                ulong apcId = node.Data.ValueOrDefault<ulong>("chrAbilityPackage", 0UL);
                dom.AddCrossLink(apcId, "usedByClass", node.Id);//apn node
                if (node.Name.StartsWith("class.pc."))
                {
                    if (apcId != 0)
                    {
                        GomObject apc = dom.GetObject(apcId);
                        if (apc != null)
                        {
                            Dictionary<object, object> abilities = apc.Data.ValueOrDefault<Dictionary<object, object>>("ablPackageAbilitiesList", null);
                            if (abilities != null)
                            {
                                foreach (KeyValuePair<object, object> ability in abilities)
                                {
                                    dom.AddCrossLink((ulong)ability.Key, "partOfApc", apc.Id);//abl node
                                    dom.AddCrossLink((ulong)ability.Key, "usedByPlayerClass", node.Id);//abl node
                                }
                            }
                            apc.Unload();
                        }
                    }
                    else
                    {
                        GomLib.Models.AdvancedClass ac;
                        LoadedACs.TryGetValue(node.Id, out ac);
                        if (ac == null)
                        {
                            ac = dom.advancedClassLoader.Load(node);
                            LoadedACs.Add(node.Id, ac);
                        }
                        if(ac != null && ac.BaseClassPkgIds != null)
                        {
                            apcId = ac.BaseClassPkgIds[1];
                            GomObject apc = dom.GetObject(apcId);
                            if (apc != null)
                            {
                                Dictionary<object, object> abilities = apc.Data.ValueOrDefault<Dictionary<object, object>>("ablPackageAbilitiesList", null);
                                if (abilities != null)
                                {
                                    foreach (KeyValuePair<object, object> ability in abilities)
                                    {
                                        dom.AddCrossLink((ulong)ability.Key, "partOfApc", apc.Id);//abl node
                                        dom.AddCrossLink((ulong)ability.Key, "usedByPlayerClass", node.Id);//abl node
                                    }
                                }
                                apc.Unload();
                            }
                        }
                    }
                }
                node.Unload();
            }
        }
        public static void LinkAchievements(DataObjectModel dom, Action<string> MessageDelegate)
        {
            List<GomObject> nodeList;
            MessageDelegate("Smart-linking achievements...");//Load ach
            nodeList = dom.GetObjectsStartingWith("ach.");
            foreach (GomObject node in nodeList)
            {
                Dictionary<object, object> requirements = node.Data.ValueOrDefault<Dictionary<object, object>>("achTasks", null);
                if (requirements != null)
                {
                    foreach (KeyValuePair<object, object> requirement in requirements)
                    {
                        Dictionary<object, object> subTasks = ((GomObjectData)requirement.Value).ValueOrDefault<Dictionary<object, object>>("achTaskSubtasks", null);
                        if (subTasks != null)
                        {
                            foreach (KeyValuePair<object, object> subTask in subTasks)
                            {
                                dom.AddCrossLink((ulong)(long)subTask.Key, "requiredForAch", node.Id);//any node
                            }
                        }
                        Dictionary<object, object> events = ((GomObjectData)requirement.Value).ValueOrDefault<Dictionary<object, object>>("achTaskEvents", null);
                        if (events != null)
                        {
                            foreach (KeyValuePair<object, object> curEvent in events)
                            {
                                dom.AddCrossLink((ulong)(long)curEvent.Key, "requiredForAch", node.Id);//any node
                            }
                        }
                    }
                }
                node.Unload();
            }
        }
        public static void LinkCodex(DataObjectModel dom, Action<string> MessageDelegate)
        {
            List<GomObject> nodeList;
            MessageDelegate("Smart-linking codex entries...");//Load codex entries
            nodeList = dom.GetObjectsStartingWith("cdx.");
            foreach (GomObject node in nodeList)
            {
                List<object> cdxPlanets = node.Data.ValueOrDefault<List<object>>("cdxPlanets", null);
                if (cdxPlanets != null)
                {
                    foreach (ulong cdxPlanet in cdxPlanets)
                    {
                        dom.AddCrossLink(cdxPlanet, "cdxOnThisPlanet", node.Id);//cdx node
                    }
                }
                node.Unload();
            }
        }
        public static void LinkConversations(DataObjectModel dom, Action<string> MessageDelegate)
        {
            List<GomObject> nodeList;
            MessageDelegate("Smart-linking conversations...");//Load cnv
            nodeList = dom.GetObjectsStartingWith("cnv.");
            foreach (GomObject node in nodeList)
            {
                Dictionary<object, object> dialogNodes = node.Data.ValueOrDefault<Dictionary<object, object>>("cnvTreeDialogNodes_Prototype", null);
                if (dialogNodes != null)
                {
                    foreach (KeyValuePair<object, object> dNode in dialogNodes)
                    {
                        Dictionary<object, object> questGrants = ((GomObjectData)dNode.Value).ValueOrDefault<Dictionary<object, object>>("cnvNodeQuestGrants", null);
                        if (questGrants != null)
                        {
                            foreach (KeyValuePair<object, object> questPair in questGrants)
                            {
                                if (!(bool)questPair.Value)
                                {
                                    string paus = "";
                                }
                                dom.AddCrossLink((ulong)questPair.Key, "conversationStarts", node.Id);
                                dom.AddCrossLink(node.Id, "startsQuest", (ulong)questPair.Key);
                            }
                        }
                        Dictionary<object, object> questEnds = ((GomObjectData)dNode.Value).ValueOrDefault<Dictionary<object, object>>("cnvNodeQuestEnds", null);
                        if (questEnds != null)
                        {
                            foreach (KeyValuePair<object, object> questPair in questEnds)
                            {
                                if (!(bool)questPair.Value)
                                {
                                    string paus = "";
                                }
                                dom.AddCrossLink((ulong)questPair.Key, "conversationEnds", node.Id);
                                dom.AddCrossLink(node.Id, "endsQuest", (ulong)questPair.Key);
                            }
                        }
                        Dictionary<object, object> questProgress = ((GomObjectData)dNode.Value).ValueOrDefault<Dictionary<object, object>>("cnvNodeQuestProgress", null);
                        if (questProgress != null)
                        {
                            foreach (KeyValuePair<object, object> questPair in questProgress)
                            {
                                if (!(bool)questPair.Value)
                                {
                                    string paus = "";
                                }
                                dom.AddCrossLink((ulong)questPair.Key, "conversationProgresses", node.Id);
                                dom.AddCrossLink(node.Id, "ProgressesQuest", (ulong)questPair.Key);
                            }
                        }
                    }
                }
                node.Unload();
            }
        }
        public static void LinkDecorations(DataObjectModel dom, Action<string> MessageDelegate)
        {
            List<GomObject> nodeList;
            MessageDelegate("Smart-linking decorations...");//Load decorations
            nodeList = dom.GetObjectsStartingWith("dec.");
            foreach (GomObject node in nodeList)
            {
                //dom.AddCrossLink(node.Data.ValueOrDefault<ulong>("decUnlockingItemId", 0UL), "unlockingDecoration", node.Id);//itm node  //should not be necessary I think
                dom.AddCrossLink(node.Data.ValueOrDefault<ulong>("decDecorationId", 0UL), "usedByDecoration", node.Id);//dyn/npc/plc node
                node.Unload();
            }
        }
        public static void LinkEncounters(DataObjectModel dom, Action<string> MessageDelegate)
        {
            List<GomObject> nodeList;
            MessageDelegate("Smart-linking encounters...");//Load enc
            nodeList = dom.GetObjectsStartingWith("enc.");
            foreach (GomObject node in nodeList)
            {
                Dictionary<object, object> spawners = node.Data.ValueOrDefault<Dictionary<object, object>>("spnEncounterSpawnerIdsToFqns", null);
                if (spawners != null)
                {
                    foreach (KeyValuePair<object, object> spawner in spawners)
                    {
                        dom.AddCrossLink((string)spawner.Value, "partOfEnc", node.Id);//spn node
                    }
                }
                dom.AddCrossLink(node.Data.ValueOrDefault<ulong>("plcHydraId", 0UL), "controllingEnc", node.Id);//hyd node
                node.Unload();
            }
        }
        public static void LinkItems(DataObjectModel dom, Action<string> MessageDelegate)
        {
            List<GomObject> nodeList;
            MessageDelegate("Smart-linking items...");//Load items
            nodeList = dom.GetObjectsStartingWith("itm.");
            foreach (GomObject node in nodeList)
            {
                dom.AddCrossLink(node.Data.ValueOrDefault<ulong>("itmEquipAbility", 0UL), "calledByItmEquip", node.Id);//abl node
                dom.AddCrossLink(node.Data.ValueOrDefault<ulong>("itmUsageAbility", 0UL), "calledByItmUse", node.Id);//abl node
                dom.AddCrossLink(node.Data.ValueOrDefault<ulong>("itmUniformNppSpec", 0UL), "givenByItem", node.Id);//npp node
                dom.AddCrossLink(node.Data.ValueOrDefault<ulong>("itmAppearanceSpec", 0UL), "givenByItem", node.Id);//ipp node
                dom.AddCrossLink(node.Data.ValueOrDefault<ulong>("itemTeachesRef", 0UL), "taughtByItem", node.Id);//any node
                node.Unload();
            }
        }
        public static void LinkItemAppearances(DataObjectModel dom, Action<string> MessageDelegate)
        {
            List<GomObject> nodeList;
            MessageDelegate("Smart-linking item apperances...");//Load items
            nodeList = dom.GetObjectsStartingWith("ipp.");
            Dictionary<long, List<ulong>> appList = new Dictionary<long, List<ulong>>();
            foreach (GomObject node in nodeList)
            {
                long modelId = node.Data.ValueOrDefault<long>("appAppearanceSlotModelID", 0L);
                if(!appList.ContainsKey(modelId))
                {
                    appList.Add(modelId, new List<ulong>());
                }
                appList[modelId].Add(node.Id);
                node.Unload();
            }
            foreach (List<ulong> apps in appList.Values)
            {
                for (int i = 0; i < apps.Count; i++)
                {
                    HashSet<ulong> similarItemIds = new HashSet<ulong>();
                    for (int e = 0; e < apps.Count; e++)
                    {
                        if (i == e) continue;
                        dom.AddCrossLink(apps[e], "similarAppearance", apps[i]);
                        
                        GomObject ipp = dom.GetObject(apps[e]);
                        if (ipp.References.ContainsKey("givenByItem"))
                        {
                            var similar = ipp.References["givenByItem"];
                            similarItemIds.UnionWith(similar);
                        }
                    }
                    List<ulong> similarList = similarItemIds.ToList();
                    for (int g = 0; g < similarList.Count; g++)
                    {
                        for (int h = 0; h < similarList.Count; h++)
                        {
                            if (g == h) continue;
                            dom.AddCrossLink(similarList[h], "similarAppearance", similarList[g]);
                        }
                    }
                }
                
                
            }
        }
        public static void LinkNpcs(DataObjectModel dom, Action<string> MessageDelegate)
        {
            List<GomObject> nodeList;
            MessageDelegate("Smart-linking NPCs...");//Load NPCs
            nodeList = dom.GetObjectsStartingWith("npc.");
            foreach (GomObject node in nodeList)
            {
                ulong npcCodexSpec = node.Data.ValueOrDefault<ulong>("npcCodexSpec", 0UL);
                if (npcCodexSpec != 0)
                {
                    dom.AddCrossLink(npcCodexSpec, "npcsGrantThisCodex", node.Id);//cdx node
                    dom.AddCrossLink(node.Id, "cdxGrantedByThisNpc", npcCodexSpec);//npc node
                }
                dom.AddCrossLink(node.Data.ValueOrDefault<ulong>("npcClassPackage", 0UL), "npcsWithThisClass", node.Id);//class node
                dom.AddCrossLink(node.Data.ValueOrDefault<ulong>("npcParentSpecId", 0UL), "npcsWithThisBlueprint", node.Id);//npc node

                string npcCnvName = node.Data.ValueOrDefault<string>("cnvConversationName", "");
                if (npcCnvName == "") {
                    ulong ParentSpecId = node.Data.ValueOrDefault<ulong>("npcParentSpecId", 0);
                    if(ParentSpecId != 0)
                    {
                        GomObject baseNpc = dom.GetObject(ParentSpecId);
                        npcCnvName = baseNpc.Data.ValueOrDefault<string>("cnvConversationName", "");
                    }
                }
                if (!String.IsNullOrEmpty(npcCnvName))
                {
                    ulong cnvId = dom.GetObjectId(npcCnvName);
                    if (cnvId != 0)
                        dom.AddCrossLink(cnvId, "npcStartsCnv", node.Id);
                }
                node.Unload();
            }
        }
        public static void LinkPhases(DataObjectModel dom, Action<string> MessageDelegate)
        {
            List<GomObject> nodeList;
            MessageDelegate("Smart-linking phases...");//Load phases
            nodeList = dom.GetObjectsStartingWith("phs.");
            foreach (GomObject node in nodeList)
            {
                dom.AddCrossLink(node.Data.ValueOrDefault<ulong>("phsConditionalHydraScript", 0UL), "controllingPhase", node.Id);//hyd node
                node.Unload();
            }
        }
        public static void LinkPlaceables(DataObjectModel dom, Action<string> MessageDelegate)
        {
            List<GomObject> nodeList;
            MessageDelegate("Smart-linking placeables...");//Load placeables
            nodeList = dom.GetObjectsStartingWith("plc.");
            foreach (GomObject node in nodeList)
            {
                dom.AddCrossLink(node.Data.ValueOrDefault<string>("plcModel", ""), "connectedToPlc", node.Id);//dyn node
                dom.AddCrossLink(node.Data.ValueOrDefault<ulong>("plcAbilitySpecOnUse", 0UL), "calledByPlcUse", node.Id);//abl node
                dom.AddCrossLink(node.Data.ValueOrDefault<ulong>("plcHydraRef", 0UL), "connectedToPlc", node.Id);//hyd node
                dom.AddCrossLink(node.Data.ValueOrDefault<string>("plcConvo", ""), "connectedToPlc", node.Id);//cnv node
                node.Unload();
            }
            MessageDelegate("Smart-linking dynamic placeables...");//Load dyn
            nodeList = dom.GetObjectsStartingWith("dyn.");
            foreach (GomObject node in nodeList)
            {
                dom.AddCrossLink(node.Data.ValueOrDefault<ulong>("dynRelatedHydra", 0UL), "calledByDyn", node.Id);//hyd node
                node.Unload();
            }
        }
        public static void LinkQuests(DataObjectModel dom, Action<string> MessageDelegate)
        {
            List<GomObject> nodeList;
            MessageDelegate("Smart-linking quests...");//Load cnv
            nodeList = dom.GetObjectsStartingWith("qst.");
            foreach (GomObject node in nodeList)
            {
                List<object> qstBranches = node.Data.ValueOrDefault<List<object>>("qstBranches", null);
                if (qstBranches != null)
                {
                    foreach (object branch in qstBranches)
                    {
                        List<object> qstSteps = ((GomObjectData)branch).ValueOrDefault<List<object>>("qstSteps", null);
                        if (qstSteps != null)
                        {
                            foreach (object step in qstSteps)
                            {
                                GomObjectData qstHydraScriptOnSuccess = ((GomObjectData)step).ValueOrDefault<GomObjectData>("qstHydraScriptOnSuccess", null);
                                if (qstHydraScriptOnSuccess != null)
                                {
                                    string hydraEvent = ((GomObjectData)qstHydraScriptOnSuccess).ValueOrDefault<string>("hydraEvent", null);
                                    if (hydraEvent == "codexGrantEntry")
                                    {
                                        ulong hydId = ((GomObjectData)qstHydraScriptOnSuccess).ValueOrDefault<ulong>("hydraScriptNodeId", 0);
                                        GomObject hydra = dom.GetObject(hydId);
                                        if (hydra != null)
                                        {
                                            Dictionary<object, object> hydScriptMap = hydra.Data.ValueOrDefault<Dictionary<object, object>>("hydScriptMap", null);
                                            if (hydScriptMap.ContainsKey("On Custom:codexGrantEntry"))
                                            {
                                                GomObjectData hydraScript = (GomObjectData)hydScriptMap["On Custom:codexGrantEntry"];
                                                List<object> hydScriptBlocks = hydraScript.ValueOrDefault<List<object>>("hydScriptBlocks", null);
                                                if (hydScriptBlocks != null)
                                                {
                                                    foreach (object block in hydScriptBlocks)
                                                    {
                                                        List<object> hydActionBlocks = ((GomObjectData)block).ValueOrDefault<List<object>>("hydActionBlocks", null);
                                                        if (hydActionBlocks != null)
                                                        {
                                                            foreach (object actionBlock in hydActionBlocks)
                                                            {
                                                                List<object> hydActions = ((GomObjectData)actionBlock).ValueOrDefault<List<object>>("hydActions", null);
                                                                if (hydActions != null)
                                                                {
                                                                    foreach (object action in hydActions)
                                                                    {
                                                                        string hydAction = ((GomObjectData)action).ValueOrDefault<string>("hydAction", null);
                                                                        if (hydAction == "Grant Codex")
                                                                        {
                                                                            string hydValue = ((GomObjectData)action).ValueOrDefault<string>("hydValue", "");
                                                                            if (hydValue != null)
                                                                            {
                                                                                GomObject cdxObj = dom.GetObject(hydValue);
                                                                                if (cdxObj != null)
                                                                                {
                                                                                    dom.AddCrossLink(cdxObj.Id, "grantedByQst", node.Id);
                                                                                    dom.AddCrossLink(node.Id, "grantsCdx", cdxObj.Id);
                                                                                }
                                                                            }
                                                                        }
                                                                    }
                                                                }
                                                            }
                                                        }
                                                    }
                                                }
                                            }
                                        }
                                    }
                                }
                                List<object> bonusMissions = ((GomObjectData)step).ValueOrDefault<List<object>>("qstBonusMissions", null);
                                if (bonusMissions != null)
                                {
                                    foreach (object bonus in bonusMissions)
                                    {
                                        ulong qstTaskBonusMissionNodeId = ((GomObjectData)bonus).ValueOrDefault<ulong>("qstTaskBonusMissionNodeId", 0);
                                        dom.AddCrossLink(qstTaskBonusMissionNodeId, "parentQuest", node.Id);
                                        dom.AddCrossLink(node.Id, "bonusQsts", qstTaskBonusMissionNodeId);
                                    }
                                }
                                List<object> qstTasks = ((GomObjectData)step).ValueOrDefault<List<object>>("qstTasks", null);
                                if (qstSteps != null)
                                {
                                    foreach (object task in qstTasks)
                                    {
                                        bonusMissions = ((GomObjectData)task).ValueOrDefault<List<object>>("qstBonusMissions", null);
                                        if (bonusMissions != null)
                                        {
                                            foreach (object bonus in bonusMissions)
                                            {
                                                ulong qstTaskBonusMissionNodeId = ((GomObjectData)bonus).ValueOrDefault<ulong>("qstTaskBonusMissionNodeId", 0);
                                                dom.AddCrossLink(qstTaskBonusMissionNodeId, "parentQuest", node.Id);
                                                dom.AddCrossLink(node.Id, "stagedBonusQsts", qstTaskBonusMissionNodeId);
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
                node.Unload();
            }
        }
        public static void LinkQuestRewards(DataObjectModel dom, Action<string> MessageDelegate)
        {
            List<GomObject> nodeList;
            MessageDelegate("Smart-linking quest rewards...");//Load phases
            var proto = dom.GetObject("qstRewardsInfoPrototype");
            if (proto != null)
            {
                Dictionary<object, object> table;
                if (proto.Data.ContainsKey(""))
                {
                    table = proto.Data.Get<Dictionary<object, object>>("qstRewardsInfoData");
                }
                else
                {
                    table = proto.Data.Get<Dictionary<object, object>>("qstRewardsNewInfoData");
                }
                if (table != null)
                {
                    foreach (KeyValuePair<object, object> node in table)
                    {
                        foreach (GomObjectData rewardcontainer in ((List<object>)node.Value))
                        {
                            GomObjectData reward = rewardcontainer.ValueOrDefault<GomObjectData>("qstRewardData", null);
                            if (reward == null) continue;
                            ulong itemId = reward.ValueOrDefault<ulong>("qstRewardItemId", 0);
                            if (itemId == 0) continue;
                            dom.AddCrossLink(itemId, "rewardFrom", (ulong)node.Key);//qst node
                        }
                    }
                }
                proto.Unload();
            }

        }
        public static void LinkSchematics(DataObjectModel dom, Action<string> MessageDelegate)
        {
            List<GomObject> nodeList;
            MessageDelegate("Smart-linking schematics...");//Load schematics
            nodeList = dom.GetObjectsStartingWith("schem.");
            foreach (GomObject node in nodeList)
            {
                Dictionary<object, object> materials = node.Data.ValueOrDefault<Dictionary<object, object>>("prfSchematicMaterials", null);
                if (materials != null)
                {
                    foreach (KeyValuePair<object, object> material in materials)
                    {
                        dom.AddCrossLink((ulong)material.Key, "materialFor", node.Id);//itm node
                    }
                }
                dom.AddCrossLink(node.Data.ValueOrDefault<ulong>("prfSchematicItemSpec", 0UL), "createdBy", node.Id);//itm node
                node.Unload();
            }
            nodeList = dom.GetObjectsStartingWith("pkg.profession_trainer");
            foreach (GomObject node in nodeList)
            {
                List<object> schematics = node.Data.ValueOrDefault<List<object>>("prfTrainerSchematicList", null);
                if (schematics != null)
                {
                    foreach (var schematic in schematics)
                    {
                        dom.AddCrossLink((ulong)schematic, "trainerTaught", node.Id);//trainer package node
                    }
                }
                node.Unload();
            }
            var proto = dom.GetObject("prfBundlesTablePrototype");
            if (proto != null)
            {
                var table = proto.Data.ValueOrDefault<List<object>>("prfBundlesTable", null);
                if (table != null)
                {
                    foreach (GomObjectData node in table)
                    {
                        List<object> schematics = node.ValueOrDefault<List<object>>("prfBundleSchemList", null);
                        if (schematics != null)
                        {
                            foreach (var schematic in schematics)
                            {
                                dom.AddCrossLink((ulong)schematic, "trainerTaught", proto.Id);//trainer package node
                            }
                        }
                    }
                }
                proto.Unload();
            }
        }
        public static void LinkSpawners(DataObjectModel dom, Action<string> MessageDelegate)
        {
            List<GomObject> nodeList;
            MessageDelegate("Smart-linking spawners...");//Load spawners
            nodeList = dom.GetObjectsStartingWith("spn.");
            foreach (GomObject node in nodeList)
            {
                List<object> spawnedNpcs = node.Data.ValueOrDefault<List<object>>("4611686029875809984", null);
                if (spawnedNpcs != null)
                {
                    foreach (GomObjectData spawnedNpc in spawnedNpcs)
                    {
                        dom.AddCrossLink(spawnedNpc.ValueOrDefault<ulong>("spnHydraRef", 0UL), "controllingSpawner", node.Id);//hyd node
                        dom.AddCrossLink(spawnedNpc.ValueOrDefault<ulong>("spawnedId", 0UL), "spawnedBy", node.Id);//npc node
                        dom.AddCrossLink(spawnedNpc.ValueOrDefault<ulong>("4611686198161500153", 0UL), "affectingSpawner", node.Id);//epp node
                    }
                }
                node.Unload();
            }
        }

    }
}
