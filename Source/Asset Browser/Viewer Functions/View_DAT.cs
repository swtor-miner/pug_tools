using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Threading;
using System.Diagnostics;

namespace PugTools
{
    public class View_DAT
    {
        public ArrayList rootList = new ArrayList();

        public ArrayList ParseDAT(StreamReader reader)
        {
            string line;
            List<string> lines = new List<string>();
            while ((line = reader.ReadLine()) != null)
            {
                lines.Add(line);
            }

            if (lines[0].StartsWith("Version="))
            {
                //parse dyc
            }
            else if (lines[1].StartsWith("! Area Specification"))
            {
                List<string> rooms = new List<string>();
                Dictionary<string, string> assets = new Dictionary<string, string>();
                List<string> paths = new List<string>();
                Dictionary<string, string> schemes = new Dictionary<string, string>();
                List<string> terrain = new List<string>();
                List<string> dydtex = new List<string>();
                List<string> dydchannel = new List<string>();
                Dictionary<string, string> settings = new Dictionary<string, string>();

                //parse area dat
                string area_name = lines[1].Split(new string[] { "for " }, StringSplitOptions.None).Last();
                _ = lines[3].Split('=').Last();
                lines.RemoveAt(0);
                lines.RemoveAt(0);
                lines.RemoveAt(0);
                lines.RemoveAt(0);
                string allLines = String.Join("\n", lines.ToArray());
                string[] sections = allLines.Split('[');
                foreach (string section in sections)
                {
                    string[] data = section.Split(']');

                    switch (data[0])
                    {
                        case "":
                            break;
                        case "ROOMS":
                            string[] roomsData = data[1].Split('\n');
                            foreach (string roomData in roomsData)
                            {
                                if (roomData != "")
                                {
                                    rooms.Add(roomData.Replace(" ", ""));
                                }
                            }
                            break;
                        case "ASSETS":
                            string[] assetsData = data[1].Split('\n');
                            foreach (string assetData in assetsData)
                            {
                                if (assetData != "")
                                {
                                    string[] assetSplitData = assetData.Split('=');
                                    assets.Add(assetSplitData[0], assetSplitData[1]);
                                }
                            }
                            break;
                        case "PATHS":
                            string[] pathsData = data[1].Split('\n');
                            foreach (string pathData in pathsData)
                            {
                                if (pathData != "")
                                {
                                    paths.Add(pathData);
                                }
                            }
                            break;
                        case "SCHEMES":
                            string[] schemesData = data[1].Split('\n');
                            foreach (string schemeData in schemesData)
                            {
                                if (schemeData != "")
                                {
                                    string[] schemeSplitData = schemeData.Split('~');
                                    schemes.Add(schemeSplitData[0], "~" + schemeSplitData[1]);
                                }
                            }
                            break;
                        case "TERRAINTEXTURES":
                            string[] terrianTexsData = data[1].Split('\n');
                            foreach (string terTexData in terrianTexsData)
                            {
                                if (terTexData != "")
                                {
                                    terrain.Add(terTexData);
                                }
                            }
                            break;
                        case "DYDTEXTURES":
                            string[] dydTexsData = data[1].Split('\n');
                            foreach (string dydTexData in dydTexsData)
                            {
                                if (dydTexData != "")
                                {
                                    dydtex.Add(dydTexData);
                                }
                            }
                            break;
                        case "DYDCHANNELPARAMS":
                            string[] dydChansData = data[1].Split('\n');
                            foreach (string dydChanData in dydChansData)
                            {
                                if (dydChanData != "")
                                {
                                    dydchannel.Add(dydChanData);
                                }
                            }
                            break;
                        case "SETTINGS":
                            string[] settingsData = data[1].Split('\n');
                            settings.Add("Area Name", area_name);
                            foreach (string settingData in settingsData)
                            {
                                if (settingData != "")
                                {
                                    string[] settingSplitData = settingData.Split('=');
                                    settings.Add(settingSplitData[0].Replace("  ", ""), settingSplitData[1]);
                                }
                            }
                            break;
                        default:
                            throw new ArgumentException("Unhandled Area DAT Section: " + data[0]);
                    }
                }
                NodeListItem roomItems = new NodeListItem("ROOMS", rooms);
                NodeListItem assetItems = new NodeListItem("ASSETS", assets);
                NodeListItem pathItems = new NodeListItem("PATHS", paths);
                NodeListItem schemeItems = new NodeListItem("SCHEMES", schemes);
                NodeListItem terrainItems = new NodeListItem("TERRAINTEXTURES", terrain);
                NodeListItem dydtexItems = new NodeListItem("DYDTEXTURES", dydtex);
                NodeListItem dydchanItems = new NodeListItem("DYDCHANNELPARAMS", dydchannel);
                NodeListItem settingItems = new NodeListItem("SETTINGS", settings);
                rootList.Add(roomItems);
                rootList.Add(assetItems);
                rootList.Add(pathItems);
                rootList.Add(schemeItems);
                rootList.Add(terrainItems);
                rootList.Add(dydtexItems);
                rootList.Add(dydchanItems);
                rootList.Add(settingItems);
            }
            else if (lines[1].StartsWith("! Room Specification"))
            {
                NodeListItem instanceItems = new NodeListItem("INSTANCES", new List<NodeListItem>())
                {
                    displayValue = ""
                };
                _ = lines[1].Split(new string[] { "for " }, StringSplitOptions.None).Last();
                _ = lines[3].Split('=').Last();
                lines.RemoveAt(0);
                lines.RemoveAt(0);
                lines.RemoveAt(0);
                lines.RemoveAt(0);
                string allLines = String.Join("\n", lines.ToArray());
                string[] sections = allLines.Split('[');
                foreach (string section in sections)
                {
                    string[] data = section.Split(']');
                    switch (data[0])
                    {
                        case "":
                            break;
                        case "INSTANCES":
                            string[] instancesData = data[1].Split('\n');
                            NodeListItem currentRoom = new NodeListItem("", "");
                            //List<NodeListItem> childData = new List<NodeListItem>();
                            bool firstRun = true;
                            foreach (string instanceData in instancesData)
                            {
                                if (instanceData == "")
                                    continue;
                                if (instanceData.StartsWith("    ."))
                                {
                                    string[] instanceRoomData = instanceData.Replace("    .", "").Split('=');
                                    NodeListItem childData = new NodeListItem(instanceRoomData[0], instanceRoomData[1]);
                                    currentRoom.children.Add(childData);
                                }
                                else
                                {
                                    if (firstRun)
                                    {
                                        firstRun = false;
                                        string[] instanceRoomData = instanceData.Split('=');
                                        currentRoom.name = instanceRoomData[0].Replace(" ", "");
                                        currentRoom.displayValue = instanceRoomData[1];
                                    }
                                    else
                                    {
                                        instanceItems.children.Add(currentRoom);
                                        string[] instanceSplitData = instanceData.Split('=');
                                        currentRoom.name = instanceSplitData[0].Replace(" ", "");
                                        currentRoom.displayValue = instanceSplitData[1];
                                        currentRoom.children.Clear();
                                    }
                                }
                            }
                            rootList.Add(instanceItems);
                            break;
                        default:
                            throw new ArgumentException("Unhandled Room DAT Type: " + data[0]);
                    }
                }

                //parse room dat
                //NodeListItem instanceItems = new NodeListItem("INSTANCES", instances);
                //NodeListItem assetItems = new NodeListItem("VISIBLE", visible);
                //NodeListItem pathItems = new NodeListItem("SETTINGS", settings);

                //rootList.Add(assetItems);
                //rootList.Add(pathItems);



            }
            else if (lines[1].StartsWith("! Character Specification"))
            {
                //parse character dat
            }
            else if (lines[1].StartsWith("! Particle Specification"))
            {
                //parse particle prt
            }

            return rootList;
        }
    }
}
