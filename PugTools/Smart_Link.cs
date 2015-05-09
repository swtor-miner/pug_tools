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
using System.Threading.Tasks;
using Microsoft.Win32;
using MySql.Data.MySqlClient;
using System.Text.RegularExpressions;
using System.Xml;
using System.IO;
using System.Xml.Linq;
using System.Xml.Serialization;
using GomLib;
using MessageBox = System.Windows.Forms.MessageBox;

namespace tor_tools
{
    public partial class Tools
    {
        private void SmartLink(DataObjectModel dom)
        {
            SmartLinkAbilities(dom);
            SmartLinkAchievements(dom);
            SmartLinkCodex(dom);
            SmartLinkDecorations(dom);
            SmartLinkEncounters(dom);
            SmartLinkItems(dom);
            SmartLinkNpcs(dom);
            SmartLinkPhases(dom);
            SmartLinkPlaceables(dom);
            SmartLinkQuests(dom);
            SmartLinkSchematics(dom);
            SmartLinkSpawners(dom);
            SmartLinkAchievements(dom);
            SmartLinkAchievements(dom);
            SmartLinkAchievements(dom);
        }

        private void SmartLinkAbilities(DataObjectModel dom)
        {
            List<GomObject> nodeList;
            addtolist2("Smart-linking ability packages (NPCs)...");//Load apns
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
            addtolist2("Smart-linking classes...");//Load classes
            nodeList = dom.GetObjectsStartingWith("class.");
            foreach (GomObject node in nodeList)
            {
                dom.AddCrossLink(node.Data.ValueOrDefault<ulong>("chrAbilityPackage", 0UL), "usedByClass", node.Id);//apn node
                node.Unload();
            }
        }
        private void SmartLinkAchievements(DataObjectModel dom)
        {
            List<GomObject> nodeList;
            addtolist2("Smart-linking achievements...");//Load ach
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
                                dom.AddCrossLink((long)subTask.Key, "requiredForAch", node.Id);//any node
                            }
                        }
                        Dictionary<object, object> events = ((GomObjectData)requirement.Value).ValueOrDefault<Dictionary<object, object>>("achTaskEvents", null);
                        if (events != null)
                        {
                            foreach (KeyValuePair<object, object> curEvent in events)
                            {
                                dom.AddCrossLink((long)curEvent.Key, "requiredForAch", node.Id);//any node
                            }
                        }
                    }
                }
                node.Unload();
            }
        }
        private void SmartLinkCodex(DataObjectModel dom)
        {
            List<GomObject> nodeList;
            addtolist2("Smart-linking codex entries...");//Load codex entries
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
        private void SmartLinkDecorations(DataObjectModel dom)
        {
            List<GomObject> nodeList;
            addtolist2("Smart-linking decorations...");//Load decorations
            nodeList = dom.GetObjectsStartingWith("dec.");
            foreach (GomObject node in nodeList)
            {
                //dom.AddCrossLink(node.Data.ValueOrDefault<ulong>("decUnlockingItemId", 0UL), "unlockingDecoration", node.Id);//itm node  //should not be necessary I think
                dom.AddCrossLink(node.Data.ValueOrDefault<ulong>("decDecorationId", 0UL), "usedByDecoration", node.Id);//dyn/npc/plc node
                node.Unload();
            }
        }
        private void SmartLinkEncounters(DataObjectModel dom)
        {
            List<GomObject> nodeList;
            addtolist2("Smart-linking encounters...");//Load enc
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
        private void SmartLinkItems(DataObjectModel dom)
        {
            List<GomObject> nodeList;
            addtolist2("Smart-linking items...");//Load items
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
        private void SmartLinkNpcs(DataObjectModel dom)
        {
            List<GomObject> nodeList;
            addtolist2("Smart-linking NPCs...");//Load NPCs
            nodeList = dom.GetObjectsStartingWith("npc.");
            foreach (GomObject node in nodeList)
            {
                dom.AddCrossLink(node.Data.ValueOrDefault<ulong>("npcClassPackage", 0UL), "npcsWithThisClass", node.Id);//class node
                dom.AddCrossLink(node.Data.ValueOrDefault<ulong>("npcParentSpecId", 0UL), "npcsWithThisBlueprint", node.Id);//npc node
                node.Unload();
            }
        }
        private void SmartLinkPhases(DataObjectModel dom)
        {
            List<GomObject> nodeList;
            addtolist2("Smart-linking phases...");//Load phases
            nodeList = dom.GetObjectsStartingWith("phs.");
            foreach (GomObject node in nodeList)
            {
                dom.AddCrossLink(node.Data.ValueOrDefault<ulong>("phsConditionalHydraScript", 0UL), "controllingPhase", node.Id);//hyd node
                node.Unload();
            }
        }
        private void SmartLinkPlaceables(DataObjectModel dom)
        {
            List<GomObject> nodeList;
            addtolist2("Smart-linking placeables...");//Load placeables
            nodeList = dom.GetObjectsStartingWith("plc.");
            foreach (GomObject node in nodeList)
            {
                dom.AddCrossLink(node.Data.ValueOrDefault<string>("plcModel", ""), "connectedToPlc", node.Id);//dyn node
                dom.AddCrossLink(node.Data.ValueOrDefault<ulong>("plcAbilitySpecOnUse", 0UL), "calledByPlcUse", node.Id);//abl node
                dom.AddCrossLink(node.Data.ValueOrDefault<ulong>("plcHydraRef", 0UL), "connectedToPlc", node.Id);//hyd node
                dom.AddCrossLink(node.Data.ValueOrDefault<string>("plcConvo", ""), "connectedToPlc", node.Id);//cnv node
                node.Unload();
            }
            addtolist2("Smart-linking dynamic placeables...");//Load dyn
            nodeList = dom.GetObjectsStartingWith("dyn.");
            foreach (GomObject node in nodeList)
            {
                dom.AddCrossLink(node.Data.ValueOrDefault<ulong>("dynRelatedHydra", 0UL), "calledByDyn", node.Id);//hyd node
                node.Unload();
            }
        }
        private void SmartLinkQuests(DataObjectModel dom)
        {
            List<GomObject> nodeList;
            addtolist2("Smart-linking quests...");//Load phases
            var proto = dom.GetObject("qstRewardsInfoPrototype");
            proto.Unload();
            if (proto != null)
            {
                var table = proto.Data.ValueOrDefault<Dictionary<object, object>>("qstRewardsInfoData", null);
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
            }
        }
        private void SmartLinkSchematics(DataObjectModel dom)
        {
            List<GomObject> nodeList;
            addtolist2("Smart-linking schematics...");//Load schematics
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
            proto.Unload();
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
            }
        }
        private void SmartLinkSpawners(DataObjectModel dom)
        {
            List<GomObject> nodeList;
            addtolist2("Smart-linking spawners...");//Load spawners
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
