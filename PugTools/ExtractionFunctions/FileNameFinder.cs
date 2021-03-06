﻿using System;
using System.Collections.Generic;
using System.Collections.Concurrent;
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
using Newtonsoft.Json;

namespace tor_tools
{
    public partial class Tools
    {
        #region deprecated
        /* 
        public void getFilenames()
        {
            Clearlist2();

            try
            {
                LoadData();

                ConcurrentBag<string> filenameBag = new ConcurrentBag<string>();
                string n = Environment.NewLine;

                bool append = false;
                const string filename = "Filenames.txt";
                
                int prog = 0;
                const int tasksToRun = 8;

                //Run as many extractors as we can in parallel.
                //RIP coding standards.
                System.Diagnostics.Stopwatch sw = new System.Diagnostics.Stopwatch();
                sw.Start();
                Parallel.For(0, tasksToRun, i =>
                    {
                        HashSet<string> tempList;

                        switch(i)
                        {
                            case 0:
                                {
                                    addtolist("Getting Dynamic Visuals.");
                                    var dynList = currentDom.GetObjectsStartingWith("dyn.");
                                    tempList = DynamicPlaceableVisuals(dynList);
                                    break;
                                }

                            case 1:
                                {
                                    addtolist("Getting GSF Filenames.");
                                    GomObject shipDataProto = currentDom.GetObject("scFFShipsDataPrototype");
                                    var shipData = shipDataProto.Data.Get<Dictionary<object, object>>("scFFShipsData");
                                    shipDataProto.Unload();
                                    tempList = ShipModelsFromPrototype(shipData);
                                    shipDataProto = null;
                                    break;
                                }

                            case 2:
                                {
                                    addtolist("Getting Weapon Filenames.");
                                    GomObject itemAppProto = currentDom.GetObject("itmAppearanceDatatable");
                                    var itemAppearances = itemAppProto.Data.Get<Dictionary<object, object>>("itmAppearances");
                                    itemAppProto.Unload();
                                    tempList = WeaponModelsFromPrototype(itemAppearances);
                                    itemAppearances = null;
                                    break;
                                }

                            case 3:
                                {
                                    addtolist("Getting Mount Filenames.");
                                    GomObject mountInfoProto = currentDom.GetObject("mntMountInfoPrototype");
                                    var mountAppearances = mountInfoProto.Data.Get<Dictionary<object, object>>("4611686298607484000");
                                    mountInfoProto.Unload();
                                    tempList = MountModelsFromPrototype(mountAppearances);
                                    mountAppearances = null;
                                    break;
                                }

                            case 4:
                                {
                                    addtolist("Getting Conversation STB Filenames.");
                                    var itmList = currentDom.GetObjectsStartingWith("cnv.").Where(obj => !obj.Name.StartsWith("cnv.test."));
                                    tempList = ConversationStringTables(itmList);
                                    break;
                                }

                            case 5:
                                {
                                    addtolist("Getting AMI Filenames.");
                                    var amiList = currentDom.GetObjectsStartingWith("ami.");
                                    tempList = new HashSet<string>(AmiModelsFromPrototypes(amiList));
                                    break;
                                }

                            case 6:
                                {
                                    addtolist("Getting Area Filenames.");
                                    var areaList = currentDom.GetObject("mapAreasDataProto").Data.Get<Dictionary<object, object>>("mapAreasDataObjectList");
                                    tempList = AreasFromPrototypes(areaList);
                                    break;
                                }

                            case 7:
                                {
                                    addtolist("Getting Tutorial Screen Names.");
                                    var tutorialList = currentDom.GetObject("loadingAreaLoadScreenPrototype").Data.Get<Dictionary<object, object>>("ldgAreaNameToLoadScreen");
                                    List<string> tutorials = tutorialList.Select(x => "\\resources\\gfx\\loadingscreen\\" + ((GomLib.GomObjectData)x.Value).ValueOrDefault<string>("ldgScreenName", "") + ".dds").ToList();
                                    tutorials.AddRange(tutorialList.Select(x => "\\resources\\gfx\\gfx_production\\" + ((GomLib.GomObjectData)x.Value).ValueOrDefault<string>("ldgOverlayName", "") + ".gfx").ToList());
                                    tempList = new HashSet<string>(tutorials);
                                    break;
                                }

                            default:
                                {
                                    //Thread without a job.
                                    tempList = new HashSet<string>();
                                    break;
                                }
                    }

                        //Add the resources to the bag.
                        foreach(string res in tempList)
                        {
                            filenameBag.Add(res);
                        }

                        Interlocked.Increment(ref prog);
                        progressUpdate(prog, tasksToRun);
                    });


                //Get rid of any duplicates.
                HashSet<string> filenames = new HashSet<string>(filenameBag);

               IEnumerable<string> resourceList = filenames
                    .Select(x => x
                        .Replace("\t", "")
                        .Replace("\\", "/")
                        .Replace("//", "/")
                        .Replace("/resourcesart/", "/resources/art/"));

                WriteFile(String.Join(n, resourceList), filename, append);

                if (outputTypeName == "XML")
                {
                    XElement filenamesElement = new XElement("Filenames", resourceList.Select(x => new XElement("Filename", new XAttribute("Id", x))));
                    string changed = string.Empty;
                    if (chkBuildCompare.Checked)
                    {
                        //addtolist("Comparing the Current Areas to the loaded Patch");
                        changed = "Changed";
                        filenamesElement = FindChangedEntries(filenamesElement, "Filenames", "Filename");
                        filenamesElement.ReplaceNodes(filenamesElement.Elements("Filename")
                            .OrderBy(x => (string)x.Attribute("Id")));
                    }

                    XDocument xmlContent = new XDocument(filenamesElement);
                    WriteFile(xmlContent, changed + "Filenames.xml", append);
                }
                sw.Stop();
                addtolist("Finished in " + sw.ElapsedMilliseconds / 1000 + " seconds! Found " + filenames.Count + " potential filenames.");
            }
            catch (Exception e)
            {
                // do something here 
                System.Windows.MessageBox.Show(String.Format("An error occured while loading data. ({0})", e.HResult));
            }
            EnableButtons();
        } 

        private HashSet<string> AreasFromPrototypes(Dictionary<object, object> areaList)
        {
            HashSet<string> assetList = new HashSet<string>();
            foreach (var gomItm in areaList)
            {
                ulong id = ((GomLib.GomObjectData)gomItm.Value).ValueOrDefault<ulong>("mapAreasDataAreaId", 0);

                ParseArea(assetList, id);

                string mapDataPath = String.Format("world.areas.{0}.mapdata", id);
                var mapDataObj = currentDom.GetObject(mapDataPath);
                if (mapDataObj != null)
                {
                    List<object> mapPages = (List<object>)mapDataObj.Data.ValueOrDefault<List<object>>("mapDataContainerMapDataList", null);

                    if (mapPages != null)
                    {
                        foreach (GomObjectData mapPage in mapPages)
                        {
                            string mapName = mapPage.ValueOrDefault<string>("mapName", null);
                            ParseResource(assetList, String.Format("/resources/world/areas/{0}/{1}_r.dds", id, mapName));
                        }
                    }
                }
            }
            addtolist("Found " + assetList.Count + " filenames in " + areaList.Count + " Areas");
            return assetList;
        }

        private IEnumerable<string> AmiModelsFromPrototypes(List<GomObject> amiList)
        {
            var resourceList = new HashSet<string>();

            foreach (GomObject itm in amiList)
            {
                Dictionary<object, object> appModelDetails = itm.Data.ValueOrDefault<Dictionary<object, object>>("appModelDetails", new Dictionary<object, object>());
                foreach (KeyValuePair<object, object> appModelKvp in appModelDetails)
                {
                    string appModelBaseFile = ((GomObjectData)appModelKvp.Value).ValueOrDefault<string>("appModelBaseFile", "");
                    ParseResource(resourceList, appModelBaseFile);

                    Dictionary<object, object> appModelMaterialList = ((GomObjectData)appModelKvp.Value).ValueOrDefault<Dictionary<object, object>>("appModelMaterialList", new Dictionary<object, object>());
                    foreach (KeyValuePair<object, object> appModelMaterialKvp in appModelMaterialList)
                    {
                        string resourceFilename = "/resources/art/shaders/materials/" + (string)((Dictionary<object, object>)appModelMaterialKvp.Value).First().Value + ".mat";
                        ParseResource(resourceList, resourceFilename);
                    }
                }
                itm.Unload();
            }

            addtolist("Found " + resourceList.Count + " filenames in Appearance Model Index");
            return resourceList;
        }

        private HashSet<string> ConversationStringTables(IEnumerable<GomLib.GomObject> itmList)
        {

            double i = 0;
            var stbList = new HashSet<string>();
            
            foreach (GomObject itm in itmList)
            {
                addtolist2("Conversation: " + itm.Name);
                Dictionary<object, object> dialogNodeMap = itm.Data.ValueOrDefault<Dictionary<object, object>>("cnvTreeDialogNodes_Prototype", new Dictionary<object, object>());
                foreach (KeyValuePair<object, object> dialogKvp in dialogNodeMap)
                {
                    long nodeNumber = ((GomObjectData)dialogKvp.Value).Get<long>("cnvNodeNumber");
                    Dictionary<object, object> textMap = ((GomObjectData)dialogKvp.Value).Get<Dictionary<object, object>>("locTextRetrieverMap");
                    if (textMap.ContainsKey(nodeNumber))
                    {
                        string stb = ((GomObjectData)textMap[(long)nodeNumber]).Get<string>("strLocalizedTextRetrieverBucket");
                        stb = stb.Replace(".", "/");

                        stbList.Add("/resources/en-us/" + stb + ".stb");
                        stbList.Add("/resources/de-de/" + stb + ".stb");
                        stbList.Add("/resources/fr-fr/" + stb + ".stb");
                        stb = null;
                    }
                }
                i++;
                itm.Unload();
            }
            
            addtolist("Found " + i + " conversation STBs");
            return stbList;
        }

        private HashSet<string> ShipModelsFromPrototype(Dictionary<object, object> shipProto)
        {
            addtolist2("Scanning ships (" + shipProto.Count + ")");
            double i = 0;
            HashSet<string> gr2List = new HashSet<string>();

            foreach (var shipEntry in shipProto)
            {
                GomLib.Models.scFFShip ship = new GomLib.Models.scFFShip();

                currentDom.scFFShipLoader.Load(ship, (long)shipEntry.Key, (GomObjectData)shipEntry.Value); //add a way of calling this that only models get loaded.

                if (ship.Model.Length > 0) ParseGR2(gr2List, ship.Model);

                foreach (var containerMapSlot in ship.MajorComponentSlots)
                {
                    if (ship.ComponentMap.ContainsKey(containerMapSlot.Key))
                    {
                        foreach (GomLib.Models.scFFComponent comp in ship.ComponentMap[containerMapSlot.Key])
                        {
                            if (comp.Model.Length > 0) ParseGR2(gr2List, comp.Model);
                        }
                    }
                }
                /*foreach (var containerMapSlot in ship.MinorComponentSlots)
                {
                    if (ship.ComponentMap.ContainsKey(containerMapSlot.Key))
                    {
                        foreach (GomLib.Models.scFFComponent comp in ship.ComponentMap[containerMapSlot.Key])
                        {
                            if (comp.Model != "") ParseGR2(gr2List, comp.Model);
                        }
                    }
                }///

                GomObject dynamicCollectionProto = currentDom.GetObject(ship.EppDynamicCollectionId);
                if (dynamicCollectionProto != null)
                {
                    var dynamicCollection = dynamicCollectionProto.Data.ValueOrDefault<List<object>>("4611686300175304011", null);
                    dynamicCollectionProto.Unload();

                    foreach (var entry in dynamicCollection)
                    {
                        var dynamicObjectId = ((GomObjectData)entry).Get<ulong>("4611686300175304008");
                        GomObject eppPackage = currentDom.GetObject(dynamicObjectId);
                        if (eppPackage != null)
                        {
                            Dictionary<object, object> eppPackageData = eppPackage.Data.Get<Dictionary<object, object>>("4611686300175304000");
                            foreach (KeyValuePair<object, object> eppString in eppPackageData)
                            {
                                ParseResource(gr2List, (string)eppString.Value);
                            }
                        }
                    }
                }
                i++;
            }

            GomObject scFFComponentAppearanceDataPrototype = currentDom.GetObject("scFFComponentAppearanceDataPrototype");
            var scFFComponentAppearanceData = scFFComponentAppearanceDataPrototype.Data.ValueOrDefault<Dictionary<object, object>>("scFFComponentAppearanceData", null);
            scFFComponentAppearanceDataPrototype.Unload();

            addtolist2("Scanning component appearances (" + scFFComponentAppearanceData.Count + ")");

            foreach (var entry in scFFComponentAppearanceData)
            {
                foreach (KeyValuePair<object, object> subEntry in (Dictionary<object, object>)entry.Value)
                {
                    foreach (KeyValuePair<string, object> deepEntry in ((GomObjectData)subEntry.Value).Dictionary.Skip(3))
                    {
                        ParseResource(gr2List, deepEntry.Value.ToString());
                    }
                }
            }
            
            //Add some code here to parse each gr2 file for it's texture names

            addtolist("Found " + gr2List.Count + " filenames in ships and their components");
            return gr2List;
        }

        private HashSet<string> WeaponModelsFromPrototype(Dictionary<object, object> itemAppearances)
        {
            double i = 0;
            HashSet<string> gr2List = new HashSet<string>();

            foreach (var itemEntry in itemAppearances)
            {
                string gr2 = ((GomObjectData)itemEntry.Value).ValueOrDefault<string>("itmModel", "");
                ParseGR2(gr2List, gr2);
                string fxspec = ((GomObjectData)itemEntry.Value).ValueOrDefault<string>("itmFxSpec", "");
                if (fxspec.Length > 0)
                {
                    if (!fxspec.EndsWith(".fxspec")) { fxspec += ".fxspec"; }
                    fxspec = "\\resources\\art\\fx\\fxspec\\" + fxspec;
                    gr2List.Add(fxspec);
                    ParseFXSpec(gr2List, fxspec);

                    i++;
                }
                i++;
            }

            addtolist("Found " + gr2List.Count + " filenames from weapons");
            return gr2List;
        }

        private HashSet<string> MountModelsFromPrototype(Dictionary<object, object> mountAppearances)
        {
            double i = 0;
            HashSet<string> fxSpecList = new HashSet<string>();

            foreach (var mountEntry in mountAppearances)
            {
                string vfx = ((GomObjectData)mountEntry.Value).ValueOrDefault<string>("mntDataVFX", "");
                if (vfx.Length > 0)
                {
                    if (!vfx.EndsWith(".fxspec")) { vfx += ".fxspec"; }
                    vfx = "/resources/art/fx/fxspec/" + vfx;
                    fxSpecList.Add(vfx);
                    ParseFXSpec(fxSpecList, vfx);

                    i++;
                }
            }

            HashSet<string> matList = new HashSet<string>();
            foreach (var gr2Name in fxSpecList.Where(x => x.EndsWith(".gr2")))
            {
                ParseGR2(matList, gr2Name);
            }

            fxSpecList.UnionWith(matList);

            addtolist("Found " + fxSpecList.Count + " filenames from " + i + " mounts");
            return fxSpecList;
        }

        private HashSet<string> DynamicPlaceableVisuals(IEnumerable<GomLib.GomObject> itmList)
        {
            double i = 0;
            var stbList = new HashSet<string>();

            foreach (GomObject itm in itmList)
            {
                addtolist2("Dyn: " + itm.Name);
                List<GomObjectData> dynVisualList = itm.Data.ValueOrDefault<List<object>>("dynVisualList", new List<object>()).ConvertAll<GomObjectData>(x => (GomObjectData)x);
                foreach (var obj in dynVisualList)
                {
                    string visualFqn = obj.ValueOrDefault<string>("dynVisualFqn", "");
                    if (visualFqn.Length > 0)
                    {
                        if (visualFqn.Contains("."))
                        {
                            string extension = visualFqn.Substring(visualFqn.LastIndexOf("."));
                            switch (extension)
                            {
                                case ".fxspec":
                                    string vfx = String.Format("/resources/art/fx/fxspec/{0}", visualFqn);
                                    stbList.Add(vfx);
                                    //ParseFXSpec(stbList, vfx); // took forever and didn't find any new files
                                    break;
                                case ".fxp": case ".lit": // don't know where these files go.
                                case ".gr2": case ".mag": 
                                    visualFqn = String.Format("/resources{0}/", visualFqn);
                                    stbList.Add(visualFqn);
                                    //ParseGR2(stbList, visualFqn); // took forever and didn't find any new files
                                    break;
                                default:
                                    throw new IndexOutOfRangeException();
                            }
                        }
                        //else
                            //Console.WriteLine(visualFqn);
                    }
                }
                i++;
                itm.Unload();
            }

            addtolist("Found " + i + " dynamic visuals");
            return stbList;
        }

        #region Misc
        private void Genderize(HashSet<string> resourceList, string filename)
        {
            List<string> genders = new List<string> { "m", "f", "u" }; //disable "u" to reduce noise in output for analysis, should be turned back on for file name searching

            foreach (var gender in genders)
            {
                string genderFileName = filename.Replace("[gen]", gender);
                ParseResource(resourceList, genderFileName);
            }
        }

        private void BodyType(HashSet<string> resourceList, string filename)
        {
            List<string> bodyTypeList = new List<string> { "bfa", "bfb", "bfn", "bfs", "bma", "bmf", "bmn", "bms" };
            foreach (var bodytype in bodyTypeList)
            {
                string bodyTypeFileName = filename.Replace("[bt]", bodytype);
                ParseResource(resourceList, bodyTypeFileName);
            }
        }
        #endregion

        #region File Parsers
        private void ParseResource(HashSet<string> resourceList, string filename)
        {
            if (filename.Length == 0
                || filename == "default") return;

            if (filename.Contains("[bt")) BodyType(resourceList, filename);
            else if (filename.Contains("[gen]")) Genderize(resourceList, filename);
            else
            {
                if (filename.EndsWith(".fxspec")) filename = "\\resources\\art\\fx\\fxspec\\" + filename;
                if (!filename.StartsWith("\\resources") && !filename.StartsWith("/resources")) filename = "\\resources" + filename;
                if (resourceList.Add(filename))
                {
                    if (filename.EndsWith(".gr2")) ParseGR2(resourceList, filename);
                    else if (filename.EndsWith(".mat")) ParseMaterial(resourceList, filename);
                    else if (filename.EndsWith(".fxspec")) ParseFXSpec(resourceList, filename);
                }
            }
        }

        private void ParseGR2(HashSet<string> gr2List, string rawModel)
        {
            if (rawModel.Length == 0) return;

            string slash = "/";
            if (!rawModel.Contains(slash))
            {
                slash = "\\";
            }
            string model = rawModel;
            if(!rawModel.Contains("resources")) model = "/resources" + rawModel;
            string path = model.Substring(0, model.LastIndexOf(slash) + 1);
            string gr2Name = model.Substring(model.LastIndexOf(slash) + 1);
            string weaponPrefix = string.Empty;
            if (gr2Name.Contains("_"))
            {
                weaponPrefix = model.Substring(model.LastIndexOf(slash) + 1, gr2Name.IndexOf("_"));
            }
            if (gr2List.Add(model)) addtolist2(model);
            var file = this.currentAssets.FindFile(model);
            if (file != null)
            {
                List<string> meshNames = new List<string>();
                List<string> materialNames = new List<string>();
                using (var fileStream = file.OpenCopyInMemory())
                { ReadGR2(fileStream, meshNames, materialNames); }

                foreach (string matName in materialNames)
                {
                    string fullMatName = "/resources/art/shaders/materials/" + matName + ".mat";
                    ParseMaterial(gr2List, fullMatName);
                }
            }
        }
        public void ReadGR2(Stream stream, List<string> meshNames, List<string> materialNames)
        {
            List<uint> offsetMeshNames = new List<uint>();
            List<uint> offsetMaterialNames = new List<uint>();
            byte[] buffer;

            buffer = stream.ReadBuffer(4);

            if (buffer.ToHexString() != "47415742")
            {
                //Console.WriteLine("Invalid header" + buffer.ToHexString());
                return;
            }
            else
            {
                //Console.WriteLine("Valid header");
                stream.Position = 0x10;

                buffer = stream.ReadBuffer(4);
                uint num50Offsets = BitConverter.ToUInt32(buffer, 0);
                buffer = stream.ReadBuffer(4);
                uint gr2_type = BitConverter.ToUInt32(buffer, 0);

                buffer = stream.ReadBuffer(2);
                int numMeshes = BitConverter.ToInt16(buffer, 0);

                buffer = stream.ReadBuffer(2);
                int numMaterials = BitConverter.ToInt16(buffer, 0);

                //this.parent.AddLog("Meshes: " + numMeshes);
                //this.parent.AddLog("Materials: " + numMaterials);

                stream.Position = 0x50;
                buffer = stream.ReadBuffer(4);
                uint offset50offset = BitConverter.ToUInt32(buffer, 0);

                buffer = stream.ReadBuffer(4);
                uint offsetMeshHeader = BitConverter.ToUInt32(buffer, 0);

                buffer = stream.ReadBuffer(4);
                uint offsetMaterialNameOffsets = BitConverter.ToUInt32(buffer, 0);

                if (numMeshes != 0)
                {
                    stream.Position = offsetMeshHeader;
                    for (int i = 0; i < numMeshes; i++)
                    {
                        buffer = stream.ReadBuffer(4);
                        uint offset = BitConverter.ToUInt32(buffer, 0);
                        offsetMeshNames.Add(offset);

                        buffer = stream.ReadBuffer(4);
                        float unknownFloat = BitConverter.ToSingle(buffer, 0);

                        buffer = stream.ReadBuffer(2);
                        int numPieces = BitConverter.ToInt16(buffer, 0);

                        buffer = stream.ReadBuffer(2);
                        int numUsedBones = BitConverter.ToInt16(buffer, 0);

                        buffer = stream.ReadBuffer(2);
                        int unknownInt = BitConverter.ToInt16(buffer, 0);

                        buffer = stream.ReadBuffer(2);
                        int vertexSize = BitConverter.ToInt16(buffer, 0);

                        buffer = stream.ReadBuffer(4);
                        uint numVertices = BitConverter.ToUInt32(buffer, 0);

                        buffer = stream.ReadBuffer(4);
                        uint numFaces = BitConverter.ToUInt32(buffer, 0);

                        buffer = stream.ReadBuffer(4);
                        uint offsetMeshVertices = BitConverter.ToUInt32(buffer, 0);

                        buffer = stream.ReadBuffer(4);
                        uint offsetMeshPieces = BitConverter.ToUInt32(buffer, 0);

                        buffer = stream.ReadBuffer(4);
                        uint offsetFaces = BitConverter.ToUInt32(buffer, 0);

                        buffer = stream.ReadBuffer(4);
                        uint offsetBones = BitConverter.ToUInt32(buffer, 0);
                    }

                    if (offsetMeshNames.Count() > 0)
                    {
                        int count = 0;
                        foreach (uint i in offsetMeshNames)
                        {
                            if (i < stream.Length)
                            {
                                count++;
                                string name = string.Empty;
                                string final_name = string.Empty;
                                List<byte> temp = new List<byte>();
                                stream.Position = i;
                                buffer = stream.ReadBuffer(1);
                                if (buffer[0] != 0x00)
                                {
                                    name += BitConverter.ToString(buffer, 0) + "-";
                                    while (buffer[0] != 0x00)
                                    {
                                        buffer = stream.ReadBuffer(1);
                                        name += BitConverter.ToString(buffer, 0) + "-";
                                    }
                                    name = name.Replace("-00-", "");
                                    //byte[] data = DepReader.FromHex(name);
                                    //final_name = Encoding.ASCII.GetString(data);
                                }
                                meshNames.Add(final_name);
                            }
                            else
                            {
                                addtolist("Error: Offset Mesh out of range: " + i + ">" + stream.Length);
                            }
                            //this.parent.AddLog("Mesh Name " + count + ": " + final_name);                          
                        }
                    }
                }

                if (numMaterials != 0)
                {
                    stream.Position = offsetMaterialNameOffsets;
                    for (int i = 0; i < numMaterials; i++)
                    {
                        buffer = stream.ReadBuffer(4);
                        uint offset = BitConverter.ToUInt32(buffer, 0);
                        offsetMaterialNames.Add(offset);
                    }

                    if (offsetMaterialNames.Count() > 0)
                    {
                        int count = 0;
                        foreach (uint i in offsetMaterialNames)
                        {
                            count++;
                            string name = string.Empty;
                            string final_name = string.Empty;
                            List<byte> temp = new List<byte>();
                            stream.Position = i;
                            buffer = stream.ReadBuffer(1);
                            if (buffer[0] != 0x00)
                            {
                                name += BitConverter.ToString(buffer, 0) + "-";
                                while (buffer[0] != 0x00)
                                {
                                    buffer = stream.ReadBuffer(1);
                                    name += BitConverter.ToString(buffer, 0) + "-";
                                }
                                name = name.Replace("-00-", "");
                                //byte[] data = DepReader.FromHex(name);
                                //final_name = Encoding.ASCII.GetString(data);
                            }
                            materialNames.Add(final_name);
                            //this.parent.AddLog("Material Name " + count + ": " + final_name);
                        }
                    }
                }
                //Console.Read();
            }
        }

        private void ParseMaterial(HashSet<string> matList, string fullMatName)
        {
            if (fullMatName.Length == 0 || fullMatName == "default") return;
            if (!fullMatName.StartsWith("/resources")) fullMatName = "/resources" + fullMatName;
            if (matList.Add(fullMatName)) addtolist2(fullMatName);

            var matFile = this.currentAssets.FindFile(fullMatName);
            if (matFile == null) return;
            XDocument doc = new XDocument();
            using (var fileStream = matFile.OpenCopyInMemory())
            {
                StreamReader reader = new StreamReader(fileStream);
                string text = reader.ReadToEnd();
                doc = XDocument.Parse(text);
            }
            
            foreach (var node in doc.Elements())
            {
                NodeChecker(matList, node);
            }
        }
        private void ParseFXSpec(HashSet<string> fxSpecList, string vfx)
        {
            var file = this.currentAssets.FindFile(vfx);
            if (file == null) return;

            var doc = XDocument.Load(file.OpenCopyInMemory());

            foreach (XElement name in doc.Descendants().Where(el => (string)el.Attribute("name") == "displayName"))
            {
                fxSpecList.Add("/resources/art/fx/fxspec/" + name.Value + ".fxspec");
            }
            foreach (XElement resource in doc.Descendants().Where(el => (string)el.Attribute("name") == "_fxResourceName"))
            {
                string prefix = "/resources";
                if (resource.Value.EndsWith(".prt"))
                {
                    if (!resource.Value.Replace("\\", "/").Contains("/art/fx/particles/")) { prefix += "/art/fx/particles/"; }
                    fxSpecList.Add(prefix + resource.Value);
                }
                else if (resource.Value.EndsWith(".gr2"))
                {
                    fxSpecList.Add(prefix + resource.Value);
                }
            }
        }
        private void NodeChecker(HashSet<string> fileList, XElement node)
        {
            if (node.HasElements)
            {
                foreach (var childnode in node.Elements())
                {
                    if (childnode.Name == "input" && childnode.Element("type") != null)
                    {
                        var type = childnode.Element("type").Value; //new way of searching for texture file names
                        if (type == "texture")
                        {
                            var textureName = childnode.Element("value").Value;
                            if (textureName != null && textureName.Length > 0)
                            {
                                string scrubbedName = textureName.Replace("////", "//").Replace(" #", "").Replace("#", "").Replace("+", "/").Replace(" ", "_");
                                fileList.Add("/resources/" + scrubbedName + ".dds");
                                fileList.Add("/resources/" + scrubbedName + ".tex");
                                if (fileList.Add("/resources/" + scrubbedName + ".tiny.dds")) addtolist2(scrubbedName);
                            }
                        }
                    }
                    var fxSpecList = childnode.Elements("fxSpecString");
                    if (childnode.Name == "AppearanceAction" && fxSpecList.Count() > 0) //
                    {
                        foreach (var fxSpec in fxSpecList)
                        {
                            var fxSpecName = "/resources/art/fx/fxspec/" + fxSpec.Value;
                            if (!fxSpec.Value.ToLower().EndsWith(".fxspec")) { fxSpecName += ".fxspec"; }
                            fileList.Add(fxSpecName);
                        }
                    }
                    /*if (childnode.Name == "Asset")
                    {
                        //var basefile = "\\resources" + childnode.Element("BaseFile").Value;
                        //file2.WriteLine(basefile);
                        //this.parent.AddLog(basefile);
                        var assetFilenames = AssetReader(childnode);
                        foreach (var name in assetFilenames)
                        {
                            string scrubbedName = name.Replace("////", "//").Replace(" #", "").Replace("#", "").Replace("+", "/").Replace(" ", "_");
                            fileList.Add(scrubbedName);
                        }
                    }///
                    else
                    {
                        NodeChecker(fileList, childnode);
                    }
                }
            }
        }

        private void ParseArea(HashSet<string> assetList, ulong id)
        {
            if (id == 0) return;
            string path = "\\resources\\world\\areas\\" + id.ToString() + "\\";
            string fileName = path + "area.dat";
            ParseResource(assetList, path + "mapnotes.not");

            if (assetList.Add(fileName)) addtolist2(fileName);

            var datFile = this.currentAssets.FindFile(fileName);
            if (datFile == null) return;
            XDocument doc = new XDocument();
            //List<string> lines = new List<string>();
            Dictionary<string, List<string>> blockLines = new Dictionary<string, List<string>>();
            using (var fileStream = datFile.OpenCopyInMemory())
            {
                StreamReader reader = new StreamReader(fileStream);

                string nextBlockName = string.Empty;
                while (!reader.EndOfStream)
                {
                    string line = reader.ReadLine();
                    if(line.StartsWith("[") || nextBlockName.Length > 0)
                    {
                        string blockName;
                        if(nextBlockName.Length > 0)
                        {
                            blockName = nextBlockName;
                        }
                        else
                        {
                            blockName = line;
                        }
                        List<string> currBlockLines = new List<string>();

                        bool doneBlock = false;
                        while(!doneBlock)
                        {
                            if (nextBlockName.Length > 0)
                            {
                                nextBlockName = string.Empty;
                            } else
                            {
                                line = reader.ReadLine();
                            }

                            if(line.StartsWith("["))
                            {
                                nextBlockName = line;
                                doneBlock = true;
                                break;
                            } else if (reader.EndOfStream)
                            {
                                doneBlock = true;
                            }

                            currBlockLines.Add(line);
                        }

                        blockLines.Add(blockName, currBlockLines);
                    }
                }
                reader.Close();

                foreach (KeyValuePair<string, List<string>> block in blockLines)
                {
                    string line = block.Key;

                    if (line == "[ROOMS]")
                    {
                        addtolist2("Found Rooms!");
                        foreach (string blockLine in block.Value)
                        {
                            ParseResource(assetList, path + blockLine.ToLower().Trim() + ".dat");
                        }
                    }
                    else if (line == "[ASSETS]")
                    {
                        addtolist2("Found Assets!");
                        foreach (string blockLine in block.Value)
                        {
                            string[] temp = blockLine.Split('=');
                            string temp1 = temp[1].Trim().Replace('/', '\\');
                            if (!temp1.Contains('\\') && !temp1.StartsWith("spn.")) ParseResource(assetList, "/resources/art/shaders/materials/" + temp1 + ".mat");
                            else if (!(temp1.StartsWith("\\spn\\")
                                || temp1.StartsWith("\\cos\\")
                                || temp1.StartsWith("spn.")
                                || temp1.StartsWith("\\enc\\")
                                || temp1.StartsWith("\\gamedata\\stg\\")
                                || temp1.StartsWith("\\server\\mpn\\")
                                || temp1.Contains(':'))) ParseResource(assetList, temp1);
                        }
                    }
                    else if (line == "[TERRAINTEXTURES]")
                    {
                        addtolist2("Found Terrain Textures!");
                        foreach (string blockLine in block.Value)
                        {
                            string[] temp = blockLine.Split(':');
                            if (temp.Length > 2)
                            {
                                string temp1 = temp[1].Trim().Replace('/', '\\');
                                if (!temp1.Contains('\\')) ParseResource(assetList, "/resources/art/shaders/materials/" + temp1 + ".mat");
                                else ParseResource(assetList, temp[2].Trim());
                            }
                        }
                    }
                    else if (line == "[DYDTEXTURES]")
                    {
                        addtolist2("Found Textures!");
                        foreach (string blockLine in block.Value)
                        {
                            string[] temp = blockLine.Split(':');
                            if (temp[1].Length > 0)
                            {
                                string temp1 = temp[1].Trim().Replace('/', '\\');
                                if (!temp1.Contains('\\')) ParseResource(assetList, "/resources/art/shaders/materials/" + temp1 + ".mat");
                                else ParseResource(assetList, temp[1].Trim());
                            }
                        }
                    }
                }
            }
        }

        #endregion
    }

    public static class OldFileHelpers
    {
        #region Helpers
        public static byte[] ReadBuffer(this Stream stream, int length)
        {
            byte[] buffer = new byte[length];
            stream.Read(buffer, 0, buffer.Length);

            return buffer;
        }

        public static T[] SubArray<T>(this T[] data, int start, int end)
        {
            int length = end - start + 1;
            T[] result = new T[length];
            Array.Copy(data, start, result, 0, length);
            return result;
        }

        public static string ToHexString(this byte[] array)
        {
            return BitConverter.ToString(array).Replace("-", "");
        }

        public static byte[] FromHex(string hex)
        {
            hex = hex.Replace("-", "");
            byte[] raw = new byte[hex.Length / 2];
            for (int i = 0; i < raw.Length; i++)
            {
                raw[i] = Convert.ToByte(hex.Substring(i * 2, 2), 16);
            }
            return raw;
        }
        #endregion*/
        #endregion
    }
}
