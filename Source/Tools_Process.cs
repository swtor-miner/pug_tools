using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Xml.Linq;
using GomLib;
using GomLib.Models;
using TorLib;

namespace PugTools
{
    public partial class Tools
    {
        #region New Flexible Processing Functions

        public void GetObjects(string prefix, string elementName)
        {
            Clearlist2();
            ClearProgress();
            LoadData();
            Addtolist(string.Format("Getting {0}", elementName));
            switch (elementName)
            {
                case "Abilities": // This section is for exploring Ability Effects
                    ProcessGameObjects(prefix, elementName);
                    currentDom.abilityLoader.effKeys.Sort();
                    string effKeyList = string.Join(Environment.NewLine, currentDom.abilityLoader.effKeys);
                    WriteFile(effKeyList, "effKeys.txt", false);

                    currentDom.abilityLoader.effWithUnknowns = currentDom.abilityLoader.effWithUnknowns.Distinct().OrderBy(o => o).ToList();
                    string effUnknowns = string.Join(Environment.NewLine, currentDom.abilityLoader.effWithUnknowns);
                    WriteFile(effUnknowns, "effUnknowns.txt", false);

                    currentDom.abilityLoader.effWithUnknowns = new List<string>();
                    currentDom.abilityLoader.effKeys = new List<string>();
                    break;
                default:
                    ProcessGameObjects(prefix, elementName);
                    break;
            }

            FlushTempTables();
            EnableButtons();
        }
        public void GetPrototypeObjects(string xmlRoot, string prototype, string dataTable)
        {
            Clearlist2();
            ClearProgress();
            LoadData();
            Addtolist(string.Format("Getting {0}", xmlRoot));
            ProcessProtoData(xmlRoot, prototype, dataTable);

            FlushTempTables();
            EnableButtons();
        }

        public void ProcessGameObjects(string gomPrefix, string xmlRoot)
        {
            bool classOverride = xmlRoot == "AdvancedClasses";
            if (!OutputCompatible(xmlRoot))
            {
                ClearProgress();
                FlushTempTables();
                return;
            }
            IEnumerable<GomObject> curItmList = GetMatchingGomObjects(currentDom, gomPrefix);

            var curItmNames = curItmList.Select(x => x.Name).ToList();

            Dictionary<string, List<GameObject>> ObjectLists = new Dictionary<string, List<GameObject>>();
            var chaItems = new Dictionary<GameObject, GameObject>();
            var newItems = new List<GameObject>();
            var remItems = new List<GameObject>();

            int i = 0;
            int count = 0;
            if (chkBuildCompare.Checked)
            {
                var prevItmNames = GetMatchingGomObjects(previousDom, gomPrefix).Select(x => x.Name).ToList();
                var removedNames = prevItmNames.Except(curItmNames).ToList(); // Couldn't find a more elegant way to do this with linq.

                ClearProgress();

                i = 0;
                count = curItmList.Count() + removedNames.Count;
                foreach (var curObject in curItmList)
                {
                    GameObject curItm = LoadGameObject(currentDom, curObject, classOverride);

                    ProgressUpdate(i, count);
                    var prevObject = previousDom.GetObject(curObject.Name);

                    if (prevObject != null)
                    {
                        GameObject prevItm = LoadGameObject(previousDom, prevObject, classOverride);

                        if (!prevItm.Equals(curItm))
                        {
                            Addtolist2(string.Join("", "Changed: ", curItm.Fqn));
                            chaItems.Add(prevItm, curItm);
                        }
                    }
                    else
                    {
                        Addtolist2(string.Join("", "New: ", curItm.Fqn));
                        newItems.Add(curItm);
                    }
                    curObject.Unload();
                    i++;
                }

                foreach (var removedName in removedNames)
                {
                    ProgressUpdate(i, count);
                    Addtolist2(string.Join("", "Removed: ", removedName));
                    GameObject prevItm = LoadGameObject(previousDom, previousDom.GetObject(removedName), classOverride);

                    remItems.Add(prevItm);
                    i++;
                }
                ClearProgress();

                ObjectLists.Add("New", newItems);
                ObjectLists.Add("Changed", chaItems.Values.ToList());
                ObjectLists.Add("Removed", remItems);

            }
            else
            {
                i = 0;
                count = curItmList.Count();
                foreach (var curObject in curItmList)
                {
                    ProgressUpdate(i, count);
                    var obj = LoadGameObject(curObject.Dom_, curObject, classOverride);
                    if (obj.Id != 0) // Apparently if the loader passes null back, sometimes data comes, too.....
                        newItems.Add(obj);
                    i++;
                }
                ObjectLists.Add("Full", newItems);
            }

            Clearlist2();
            Addtolist2(string.Format("Generating {0} Output", outputTypeName));
            XDocument xmlDoc = new XDocument();
            XElement elements = new XElement(xmlRoot);
            count = 0;
            i = 0;
            foreach (var itmList in ObjectLists)
            {
                count += itmList.Value.Count;
            }

            foreach (var itmList in ObjectLists)
            {
                if (outputTypeName == "JSON")
                    GameObjectListAsJSON(string.Join("", itmList.Key, xmlRoot), itmList.Value);
                else if (outputTypeName == "Text")
                    GameObjectListAsText(string.Join("", itmList.Key, xmlRoot), itmList.Value);
                else if (outputTypeName == "SQL")
                    ObjectListAsSql(itmList.Key, xmlRoot, itmList.Value);
                else
                {
                    if (itmList.Key == "Changed")
                    {

                        foreach (var changedPair in chaItems)
                        {
                            ProgressUpdate(i, count);
                            XElement oldElement = ConvertToXElement(changedPair.Key);
                            XElement newElement = ConvertToXElement(changedPair.Value);

                            newElement = CompareElements(oldElement, newElement);
                            oldElement = null;


                            if (newElement != null)
                            {
                                newElement.Add(new XAttribute("Status", itmList.Key));
                                elements.Add(newElement);
                            }
                            i++;
                        }

                    }
                    else
                    {
                        if ((itmList.Key == "New" || itmList.Key == "Full") && gomPrefix == "ach.")
                        {
                            WriteFile("", "brokenAchieves.txt", false);
                        }

                        foreach (var itm in itmList.Value)
                        {
                            ProgressUpdate(i, count);
                            XElement itemElement = ConvertToXElement(itm);
                            if ((itmList.Key == "New" || itmList.Key == "Full") && gomPrefix == "ach.")
                            {
                                if (((Achievement)itm).AchId == 0)
                                {
                                    WriteFile(itm.Fqn + " : " + ((Achievement)itm).Name + Environment.NewLine, "brokenAchieves.txt", true);
                                }
                            }
                            if (itmList.Key != "Full") itemElement.Add(new XAttribute("Status", itmList.Key));
                            itemElement.Add(ReferencesToXElement(itm.References));
                            elements.Add(itemElement);
                            i++;
                        }
                    }
                }
            }

            if (outputTypeName == "XML")
            {
                var filename = string.Join("", xmlRoot, ".xml");
                string outputComment = "total";
                if (chkBuildCompare.Checked)
                {
                    filename = string.Join("", "Changed", filename);
                    outputComment = "new/changed/removed ";
                }

                elements = Sort(elements);
                Addtolist(string.Format("{0} - {1} {2}", xmlRoot, elements.Elements().Count(), outputComment));
                xmlDoc.Add(elements);
                WriteFile(xmlDoc, filename, false, false);
            }
            ClearProgress();

            FlushTempTables();
        }

        public void ProcessProtoData(string xmlRoot, string prototype, string dataTable)
        {
            if (!OutputCompatible(xmlRoot))
            {
                ClearProgress();
                FlushTempTables();
                return;
            }

            Dictionary<object, object> currentDataProto = new Dictionary<object, object>();
            GomObject currentDataObject = currentDom.GetObject(prototype);
            if (currentDataObject != null) // Fix to ensure old game assets don't throw exceptions.
            {
                currentDataProto = currentDataObject.Data.Get<Dictionary<object, object>>(dataTable);
                currentDataObject.Unload();
            }
            else // Check replaced prototype
            {
                Dictionary<string, KeyValuePair<string, string>> replacedProtos = new Dictionary<string, KeyValuePair<string, string>>
                    {
                        { "wevConquestInfosPrototype", new KeyValuePair<string, string>("cnqConquestInfoPrototype","cnqConquestTable") }
                    };
                if (replacedProtos.TryGetValue(prototype, out KeyValuePair<string, string> protkey))
                {
                    prototype = protkey.Key;
                    dataTable = protkey.Value;
                    currentDataObject = currentDom.GetObject(prototype);
                    if (currentDataObject != null) // Fix to ensure old game assets don't throw exceptions.
                    {
                        currentDataProto = currentDataObject.Data.Get<Dictionary<object, object>>(dataTable);
                        currentDataObject.Unload();
                    }
                }
            }

            var curIds = currentDataProto.Keys;
            Dictionary<string, List<PseudoGameObject>> ObjectLists = new Dictionary<string, List<PseudoGameObject>>();
            var chaItems = new Dictionary<PseudoGameObject, PseudoGameObject>();
            var newItems = new List<PseudoGameObject>();

            int i;

            int count;
            if (chkBuildCompare.Checked)
            {
                Dictionary<object, object> previousDataProto = new Dictionary<object, object>();
                GomObject previousDataObject = previousDom.GetObject(prototype);
                if (previousDataObject != null) // Fix to ensure old game assets don't throw exceptions.
                {
                    previousDataProto = previousDataObject.Data.Get<Dictionary<object, object>>(dataTable);
                    previousDataObject.Unload();
                }


                var prevIds = previousDataProto.Keys;

                var remItems = new List<PseudoGameObject>();
                var removedIds = prevIds.Except(curIds).ToList();

                i = 0;
                count = curIds.Count() + removedIds.Count;
                ClearProgress();

                foreach (var id in curIds)
                {
                    ProgressUpdate(i, count);
                    currentDataProto.TryGetValue(id, out object curData);
                    PseudoGameObject curObj = PseudoGameObject.Load(xmlRoot, currentDom, id, curData);

                    previousDataProto.TryGetValue(id, out object prevData);
                    PseudoGameObject prevObj = PseudoGameObject.Load(xmlRoot, previousDom, id, prevData);

                    if (prevObj.Id != 0)
                    {
                        if (!prevObj.Equals(curObj))
                        {
                            Addtolist2(string.Join("", "Changed: ", curObj.Name));
                            chaItems.Add(prevObj, curObj);
                        }
                    }
                    else
                    {
                        Addtolist2(string.Join("", "New: ", curObj.Name));
                        newItems.Add(curObj);
                    }

                    i++;
                }

                foreach (var removedId in removedIds)
                {
                    ProgressUpdate(i, count);

                    previousDataProto.TryGetValue(removedId, out object prevData);
                    PseudoGameObject prevObj = PseudoGameObject.Load(xmlRoot, previousDom, removedId, prevData);

                    Addtolist2(string.Join("", "Removed: ", prevObj.Name));
                    remItems.Add(prevObj);

                    i++;
                }
                ClearProgress();

                ObjectLists.Add("New", newItems);
                ObjectLists.Add("Changed", chaItems.Values.ToList());
                ObjectLists.Add("Removed", remItems);

            }
            else
            {
                i = 0;
                count = curIds.Count();
                foreach (var id in curIds)
                {
                    ProgressUpdate(i, count);
                    currentDataProto.TryGetValue(id, out object curData);
                    PseudoGameObject curObj = PseudoGameObject.Load(xmlRoot, currentDom, id, curData);
                    newItems.Add(curObj);
                    i++;
                }
                ObjectLists.Add("Full", newItems);
            }

            Clearlist2();
            Addtolist2(string.Format("Generating {0} Output", outputTypeName));
            XDocument xmlDoc = new XDocument();
            XElement elements = new XElement(xmlRoot);
            count = 0;
            i = 0;
            foreach (var itmList in ObjectLists)
            {
                count += itmList.Value.Count;
            }


            foreach (var itmList in ObjectLists)
            {
                if (outputTypeName == "JSON")
                    PseudoGameObjectListAsJSON(string.Join("", itmList.Key, xmlRoot), itmList.Value);
                else if (outputTypeName == "Text")
                    PseudoGameObjectListAsText(string.Join("", itmList.Key, xmlRoot), itmList.Value);
                else if (outputTypeName == "SQL")
                    ObjectListAsSql(itmList.Key, xmlRoot, itmList.Value);
                else
                {
                    if (itmList.Key == "Changed")
                    {
                        foreach (var changedPair in chaItems)
                        {
                            ProgressUpdate(i, count);
                            XElement oldElement = ConvertToXElement(changedPair.Key);
                            XElement newElement = ConvertToXElement(changedPair.Value);

                            newElement = CompareElements(oldElement, newElement);
                            oldElement = null;

                            if (newElement != null)
                            {
                                newElement.Add(new XAttribute("Status", itmList.Key));
                                elements.Add(newElement);
                            }
                            i++;
                        }
                    }
                    else
                    {
                        foreach (var itm in itmList.Value)
                        {
                            ProgressUpdate(i, count);
                            XElement itemElement = itm.ToXElement();
                            if (itmList.Key != "Full") itemElement.Add(new XAttribute("Status", itmList.Key));

                            if (itmList.Key == "New")
                            {
                                if (xmlRoot == "Collections")
                                {
                                    // Named icon export.
                                    // Bleh item specific code here.
                                    if (itm is Collection colItm)
                                    {
                                        ExportIconFromPath("/resources/gfx/mtxstore/" + colItm.Icon + "_400x400.dds", colItm.Name,
                                            "/MtxImages/Named/Collections/{0}.png");
                                    }
                                }
                                else if (xmlRoot == "MtxStoreFronts")
                                {
                                    if (itm is MtxStorefrontEntry mtxItm)
                                    {
                                        ExportIconFromPath("/resources/gfx/mtxstore/" + mtxItm.Icon + "_400x400.dds", mtxItm.Name,
                                            "/MtxImages/Named/MtxStore/{0}.png");
                                    }
                                }
                            }

                            elements.Add(itemElement);
                            i++;
                        }
                    }
                }
            }

            if (outputTypeName == "XML")
            {
                var filename = string.Join("", xmlRoot, ".xml");
                string outputComment = "";
                if (chkBuildCompare.Checked)
                {
                    filename = string.Join("", "Changed", filename);
                    outputComment = "new/changed/removed ";
                }

                elements = Sort(elements);
                Addtolist(string.Format("{0} - {1} {2}", xmlRoot, elements.Elements().Count(), outputComment));
                xmlDoc.Add(elements);
                WriteFile(xmlDoc, filename, false, false);
            }
            ClearProgress();

            FlushTempTables();
        }

        private IEnumerable<GomObject> GetMatchingGomObjects(DataObjectModel dom, string gomPrefix)
        {           // This function is meant to handle unique cases of loading a list of objects from the GOM
            IEnumerable<GomObject> itmList;
            switch (gomPrefix)
            {
                case "abl.":
                    itmList = dom.GetObjectsStartingWith(gomPrefix).Where(x => !x.Name.Contains("/")); // Abilities with a / in the name are Effects.
                    break;
                case "apn.":
                    itmList = dom.GetObjectsStartingWith(gomPrefix).Union(dom.GetObjectsStartingWith("apc.")); // Union APC/APN
                    break;
                case "eff.":
                    itmList = dom.GetObjectsStartingWith("abl.").Where(x => x.Name.Contains("/"));
                    break;
                default:
                    itmList = dom.GetObjectsStartingWith(gomPrefix); // No need for the extra Linq statement for non-unique cases
                    break;
            }
            return itmList;
        }

        private void ProcessGomFields()
        {
            XDocument currentFields = currentDom.ReturnTypeNames();
            if (chkBuildCompare.Checked)
            {
                XDocument previousFields = previousDom.ReturnTypeNames();
                WriteFile(new XDocument(currentFields.Root.Element("FieldUseInDomClass")), "CurrentGom_Fields.xml", false);
                WriteFile(new XDocument(previousFields.Root.Element("FieldUseInDomClass")), "PreviousGom_Fields.xml", false);
                XElement wrapper = new XElement("Wrapper");
                XElement compared = FindChangedEntries(currentFields.Root.Element("Gom_Fields"), new XDocument(previousFields.Root.Element("Gom_Fields")), "Gom_Fields", "Gom_Field");
                wrapper.Add(compared);
                XElement compared2 = FindChangedEntries(currentFields.Root.Element("FieldUseInDomClass"), new XDocument(previousFields.Root.Element("FieldUseInDomClass")), "FieldUseInDomClass", "DomClass");
                wrapper.Add(compared2);

                XDocument xmlDoc = new XDocument(wrapper);
                if (!xmlDoc.Root.IsEmpty) WriteFile(xmlDoc, "ChangedGom_Fields.xml", false);
            }
            else
                WriteFile(currentFields, "Gom_Fields.xml", false);
        }

        private bool ExportGOM = false;

        public bool ExportNPP1 { get; set; } = false;
        public bool ExportICONS1 { get; set; } = false;

        private void ProcessEffectChanges()
        {
            IEnumerable<GomObject> currentAblObjects = currentDom.GetObjectsStartingWith("abl.").Where(x => x.GetType() == typeof(GomObject));
            IEnumerable<GomObject> previousAblObjects = previousDom.GetObjectsStartingWith("abl.").Where(x => x.GetType() == typeof(GomObject));

            // Build a dictionary of effects so we can quickly look them up.
            Dictionary<string, GomObject> currentEffectByNameID = new Dictionary<string, GomObject>();
            foreach (GomObject obj in currentAblObjects)
            {
                if (!obj.Name.Contains("/"))
                {
                    // Only care about effect nodes.
                    continue;
                }

                string[] nameArray = obj.Name.Split('/');
                string name = nameArray[0] + '/' + nameArray[2];
                currentEffectByNameID.Add(name, obj);
            }

            Dictionary<string, GomObject> previousEffectByNameID = new Dictionary<string, GomObject>();
            foreach (GomObject obj in previousAblObjects)
            {
                if (!obj.Name.Contains("/"))
                {
                    // Only care about effect nodes.
                    continue;
                }

                string[] nameArray = obj.Name.Split('/');
                string name = nameArray[0] + '/' + nameArray[2];
                previousEffectByNameID.Add(name, obj);
            }

            foreach (KeyValuePair<string, GomObject> currentPair in currentEffectByNameID)
            {
                if (previousEffectByNameID.TryGetValue(currentPair.Key, out GomObject prevObj))
                {
                    if (!prevObj.Equals(currentPair.Value))
                    {
                        // Effect node not equal!
                        XElement oldElement = prevObj.Print();
                        prevObj.Unload();
                        XElement newElement = currentPair.Value.Print();
                        currentPair.Value.Unload();

                        newElement = CompareElements(oldElement, newElement);
                        oldElement = null;
                        if (newElement != null)
                        {
                            var regex = new Regex(Regex.Escape("."));
                            var newText = regex.Replace(currentPair.Key, "\\", 1);

                            WriteFile(new XDocument(newElement), string.Format("\\GOM\\ChangedEffects\\{0}.xml", newText), false);
                        }
                    }
                }
            }
        }
        private void ProcessGom()
        {
            var currentObjects = currentDom.GetObjectsStartingWith("").Where(x => x.GetType() == typeof(GomObject));

            Dictionary<string, List<GomObject>> ObjectLists = new Dictionary<string, List<GomObject>>();
            var chaObjects = new Dictionary<GomObject, GomObject>();
            var newObjects = new List<GomObject>();
            var remObjects = new List<GomObject>();

            var currentNames = currentObjects.Select(x => x.Name).ToList();

            int i = 0;
            int count = 0;
            if (chkBuildCompare.Checked)
            {
                var previousObjects = previousDom.GetObjectsStartingWith("").Where(x => x.GetType() == typeof(GomObject));
                var previousNames = previousObjects.Select(x => x.Name).ToList();
                var removedNames = previousNames.Except(currentNames).ToList(); // Couldn't find a more elegant way to do this with linq.

                ClearProgress();
                i = 0;
                count = currentObjects.Count() + removedNames.Count;
                foreach (var curObject in currentObjects)
                {
                    ProgressUpdate(i, count);
                    var prevObject = previousDom.GetObject(curObject.Name);
                    if (prevObject != null)
                    {
                        if (!prevObject.Equals(curObject))
                        {
                            chaObjects.Add(prevObject, curObject);
                        }
                        prevObject.Unload();
                    }
                    else
                    {
                        newObjects.Add(curObject);
                    }
                    curObject.Unload();
                    i++;
                }



                foreach (var removedName in removedNames)
                {
                    ProgressUpdate(i, count);
                    remObjects.Add(previousDom.GetObject(removedName));
                    i++;
                }



                ObjectLists.Add("New", newObjects);
                ObjectLists.Add("Changed", chaObjects.Values.ToList());
                ObjectLists.Add("Removed", remObjects);

            }
            else
            {
                ObjectLists.Add("", currentObjects.ToList());
            }

            Addtolist(string.Format("Generating Output {0}", outputTypeName));
            XDocument xmlDoc = new XDocument();
            XElement gomItems = new XElement("GOM_Items");
            XElement z_effects = new XElement("Z_Effects");
            ClearProgress();
            i = 0;
            count = 0;
            foreach (var kvp in ObjectLists)
            {
                count += kvp.Value.Count();
            }

            foreach (var itmList in ObjectLists)
            {
                ProgressUpdate(i, count);
                foreach (var gomItm in itmList.Value)
                {

                    XElement gomElement = new XElement("GOM_Item", new XAttribute("Id", gomItm));
                    if (itmList.Key != "") gomElement.Add(new XAttribute("Status", itmList.Key));
                    if (gomItm.References != null)
                    {
                        gomElement.Add(ReferencesToXElement(gomItm.References));
                    }
                    if (gomItm.FullReferences != null)
                    {
                        gomElement.Add(ReferencesToXElement(gomItm.FullReferences));
                    }
                    if (gomItm.Name.Contains("/"))
                        z_effects.Add(gomElement);
                    else
                        gomItems.Add(gomElement);
                    gomItm.Unload();
                }
                i++;
            }
            var filename = "GOM_Items.xml";

            if (chkBuildCompare.Checked)
            {
                filename = string.Format("{0}{1}", "Changed", filename);
                gomItems.ReplaceNodes(gomItems.Elements("GOM_Item")
                            .OrderBy(x => (string)x.Attribute("Status"))
                            .ThenBy(x => (string)x.Attribute("Id")));
                z_effects.ReplaceNodes(z_effects.Elements("GOM_Item")
                            .OrderBy(x => (string)x.Attribute("Status"))
                            .ThenBy(x => (string)x.Attribute("Id")));
            }
            else
            {
                gomItems.ReplaceNodes(gomItems.Elements("GOM_Item")
                            .OrderBy(x => (string)x.Attribute("Id")));
                z_effects.ReplaceNodes(z_effects.Elements("GOM_Item")
                            .OrderBy(x => (string)x.Attribute("Id")));
            }
            Addtolist("GOM Items - " + (gomItems.Elements("GOM_Item").Count() + z_effects.Elements("GOM_Item").Count()));
            xmlDoc.Add(new XElement("Container", gomItems, z_effects));
            WriteFile(xmlDoc, filename, false, false);

            if (chkBuildCompare.Checked)
            {
                if (ExportGOM)
                {
                    foreach (var objList in ObjectLists)
                    {
                        if (objList.Key == "Changed")
                        {
                            foreach (var changedPair in chaObjects)
                            {
                                XElement oldElement = changedPair.Key.Print();
                                changedPair.Key.Unload();
                                XElement newElement = changedPair.Value.Print();
                                changedPair.Value.Unload();

                                newElement = CompareElements(oldElement, newElement);
                                oldElement = null;
                                if (newElement != null)
                                {
                                    var regex = new Regex(Regex.Escape("."));
                                    var newText = regex.Replace(changedPair.Value.Name, "\\", 1);

                                    WriteFile(new XDocument(newElement), string.Format("\\GOM\\{0}\\{1}.xml", objList.Key, newText), false);
                                }

                            }
                        }
                        else
                        {
                            foreach (var obj in objList.Value)
                            {
                                XElement newElement = obj.Print();
                                obj.Unload();

                                var regex = new Regex(Regex.Escape("."));
                                var newText = regex.Replace(obj.Name.Replace('/', '.'), "\\", 1);

                                WriteFile(new XDocument(newElement), string.Format("\\GOM\\{0}\\{1}.xml", objList.Key, newText), false);
                            }
                        }
                    }
                }
            }
            ClearProgress();
        }

        #endregion

        #region Misc
        private bool OutputCompatible(string xmlRoot)
        {
            switch (outputTypeName)
            {
                case "Text":
                case "JSON":
                case "XML": return true;
                case "SQL":
                    if (initTable.ContainsKey(xmlRoot))
                        return true;
                    else
                        return false;
                default:
                    return false;
            }
        }

        public void ExportIconFromPath(string path, string name, string exportPath)
        {
            Library lib = currentAssets.libraries.Where(x => x.Name.Contains("main_gfx_assets")).Single();
            if (!lib.Loaded)
                lib.Load();

            TorLib.File iconFile = lib.FindFile(path);
            if (iconFile != null)
            {
                HashDictionaryInstance hashData = HashDictionaryInstance.Instance;
                if (!hashData.Loaded)
                {
                    hashData.Load();
                }
                hashData.dictionary.CreateHelpers();


                HashFileInfo hashInfo = new HashFileInfo(iconFile.FileInfo.PrimaryHash, iconFile.FileInfo.SecondaryHash, iconFile);
                string stateName = hashInfo.FileState.ToString();

                DevIL.ImageImporter imp = new DevIL.ImageImporter();
                DevIL.Image dds;
                using (MemoryStream iconStream = (MemoryStream)iconFile.OpenCopyInMemory())
                {
                    dds = imp.LoadImageFromStream(DevIL.ImageType.Dds, iconStream);
                }

                using (MemoryStream outputStream = new MemoryStream())
                {
                    DevIL.ImageExporter exp = new DevIL.ImageExporter();
                    exp.SaveImageToStream(dds, DevIL.ImageType.Png, outputStream); // Save DDS to stream in PNG format

                    name += "_" + stateName;
                    foreach (char character in Path.GetInvalidFileNameChars())
                    {
                        // Make sure the name doesn't contain invalid characters.
                        name = name.Replace(character, '-');
                    }

                    WriteFile(outputStream, string.Format(exportPath, name));
                }
            }
        }
        #endregion

        #region Txt functions

        private void GameObjectListAsText(string prefix, IEnumerable<GameObject> itmList)
        {
            int i = 0;
            short e = 0;
            _ = Environment.NewLine;
            var txtFile = new StringBuilder();
            string filename = string.Join("", prefix, ".txt");
            if (itmList.Count() == 0) return;
            string headerRow = GetHeaderRow(
    prefix
        .Replace("Changed", "")
        .Replace("Full", "")
        .Replace("New", "")
        .Replace("Removed", ""),
    itmList.First().Dom_);
            WriteFile(headerRow, filename, false);
            var count = itmList.Count();
            foreach (var itm in itmList)
            {
                ProgressUpdate(i, count);
                if (e % 1000 == 1)
                {
                    WriteFile(txtFile.ToString(), filename, true);
                    txtFile.Clear();
                    e = 0;
                }

                Addtolist2(string.Format("{0}: {1}", prefix, itm.Fqn));

                string textString = ConvertToText(itm);
                txtFile.Append(textString + Environment.NewLine); // Append it with a newline to the output.
                i++;
                e++;
            }
            Addtolist(string.Join("", "The ", prefix, " text file has been generated; there are ", i, " ", prefix));
            WriteFile(txtFile.ToString(), filename, true);
            DeleteEmptyFile(filename, i);
            ClearProgress();
        }
        private void PseudoGameObjectListAsText(string prefix, IEnumerable<PseudoGameObject> itmList)
        {
            int i = 0;
            short e = 0;
            string n = Environment.NewLine;
            var txtFile = new StringBuilder();
            string filename = string.Format("{0}{1}", prefix, ".txt");
            WriteFile("", filename, false);
            var count = itmList.Count();
            Dictionary<long, string> conqOrder = null;
            if (prefix.Contains("Conquest"))
            {
                WriteFile("", "ConquestSCSV.txt", false);
                WriteFile("", "ConquestOrder.txt", false);
            }

            foreach (var itm in itmList)
            {
                ProgressUpdate(i, count);
                if (e % 1000 == 1)
                {
                    WriteFile(txtFile.ToString(), filename, true);
                    txtFile.Clear();
                    e = 0;
                }

                Addtolist2(string.Format("{0}: {1}", prefix, itm.Name));

                string jsonString = ConvertToText(itm);
                txtFile.Append(jsonString + Environment.NewLine); // Append it with a newline to the output.
                i++;
                e++;
                if (prefix.Contains("Conquest"))
                {
                    if (conqOrder == null)
                        conqOrder = new Dictionary<long, string>();
                    if (((Conquest)itm).ActiveData != null)
                        foreach (var actDat in ((Conquest)itm).ActiveData)
                        {
                            conqOrder.Add(actDat.ActualOrderNum, string.Format("{0}: {1} EST - {2}", actDat.ActualOrderNum, actDat.StartTime.ToString(), ((Conquest)itm).Name));
                        }
                    else if (((Conquest)itm).NewActiveData != null)
                        foreach (var actDat in ((Conquest)itm).NewActiveData)
                        {
                            conqOrder.Add(actDat.Ticks, string.Format("{0} EST - {1}  ", actDat.ToString(), ((Conquest)itm).Name));
                        }
                    WriteFile(((Conquest)itm).ConquestToSCSV(), "ConquestSCSV.txt", true);
                }
            }
            if (prefix.Contains("Conquest"))
                WriteFile(string.Join(Environment.NewLine, conqOrder.OrderBy(x => x.Key).Select(x => x.Value)), "ConquestOrder.txt", true);
            Addtolist(string.Join("", "The ", prefix, " text file has been generated; there are ", i, " ", prefix));
            WriteFile(txtFile.ToString(), filename, true);
            DeleteEmptyFile(filename, i);
            ClearProgress();
        }

        private static string GetHeaderRow(string prefix, DataObjectModel dom)
        {
            switch (prefix)
            {
                case "Decorations":
                    var hookNameList = dom.decorationLoader.HookList.Select(x => x.Value.Name).ToList();
                    hookNameList.Sort();
                    string hookString = string.Join(";", hookNameList);
                    return string.Format("Name;Sources;Binding;Category;SubCategory;Purchase for Guild Cost;Stub Type;{0}{1}", hookString, Environment.NewLine);
                default: // Don't have a predefined header row for this type
                    break;
            }
            return "";
        }

        public string ConvertToText(ulong itemId, DataObjectModel dom)
        {
            return ConvertToText(itemId, dom, false);
        }
        public string ConvertToText(ulong itemId, DataObjectModel dom, bool overrideVerbose)
        {
            var gomItm = dom.GetObject(itemId);
            return ConvertToText(gomItm, overrideVerbose);
        }
        public string ConvertToText(string fqn, DataObjectModel dom)
        {
            if (dom is null)
            {
                throw new ArgumentNullException(nameof(dom));
            }

            return ConvertToText(fqn, false);
        }
        public string ConvertToText(string fqn, DataObjectModel dom, bool overrideVerbose)
        {
            var gomItm = dom.GetObject(fqn);
            return ConvertToText(gomItm, overrideVerbose);
        }
        public string ConvertToText(GomObject gomItm)
        {
            return ConvertToText(gomItm, false);
        }
        public string ConvertToText(GomObject gomItm, bool overrideVerbose)
        {
            if (gomItm != null)
            {
                if (gomItm.Name.Contains("/"))
                    return null;
                return ConvertToText(LoadGameObject(gomItm.Dom_, gomItm, false), overrideVerbose);
            }
            return null;
        }
        public string ConvertToText(object item) //was tired of writing code that duplicated this functionality
        {
            return ConvertToText(item, false);
        }
        public string ConvertToText(object item, bool overrideVerbose) //was tired of writing code that duplicated this functionality
        {
            if (item == null) return null;
            string type = item.GetType().ToString();
            switch (type)
            {
                case "GomLib.Models.Conversation":
                case "GomLib.Models.Decoration":
                case "GomLib.Models.Conquest": return ((PseudoGameObject)item).ToString(!overrideVerbose);
                default: break;
            }
            return null;
        }
        #endregion
        #region XML functions
        #region Xml Comparison Functions

        private XElement FindChangedEntries(XElement items, XDocument previousPatch, string containerName, string subContainerName)
        {
            Addtolist("Comparing to Previous Version.");
            Clearlist2();
            Addtolist2("No Output will appear here.");
            Addtolist2("Is this really necessary?");
            string filename = prefix + containerName + ".xml";

            XElement addedChangedItems = new XElement(containerName, items.Elements(subContainerName).Cast<XNode>()
                .Except(previousPatch.Element(containerName).Elements(subContainerName).Cast<XNode>(),
                new XNodeEqualityComparer()));

            // This section should add a OldVersion element to every changed item which contains the old version of it.
            if (containerName == "GOM_Items")
            {
                if (items.Descendants("References") != null) // OldValue doesn't apply to Gom_Items
                {
                    List<string> changedItemIds = addedChangedItems.Elements(subContainerName)
                           .Select(i => i.Attribute("Id").Value)
                       .Intersect(previousPatch.Element(containerName).Elements(subContainerName)
                           .Select(i => i.Attribute("Id").Value))
                       .ToList();
                    var count = changedItemIds.Count();
                    foreach (var changedItemId in changedItemIds)
                    {
                        var previousElement = previousPatch.Element(containerName).Elements(subContainerName).First(p => p.Attribute("Id").Value == changedItemId);

                        var newElement = addedChangedItems.Elements(subContainerName).Where(x => x.Attribute("Id").Value == changedItemId).First();

                        var changedItem = addedChangedItems.Elements().First(x => x.Attribute("Id").Value == changedItemId);
                        List<string> toRemove = previousElement.Element("References").Elements()
                                .Select(i => i.Attribute("Id").Value)
                            .Intersect(newElement.Element("References").Elements()
                                .Select(i => i.Attribute("Id").Value))
                            .ToList();
                        foreach (var removedItemId in toRemove)
                        {
                            XElement itemToRemove = changedItem.Element("References").Elements().First(x => x.Attribute("Id").Value == removedItemId);
                            if (itemToRemove != null) itemToRemove.Remove();
                        }
                        if (changedItem.Attribute("Status") == null)
                        {
                            changedItem.Add(new XAttribute("Status", "Changed"));
                        }
                    }
                }
            }
            else if (containerName != "GOM_Fields")
            {
                List<string> changedItemIds = addedChangedItems.Elements(subContainerName)
                       .Select(i => i.Attribute("Id").Value)
                       .Intersect(previousPatch.Element(containerName).Elements(subContainerName)
                       .Select(i => i.Attribute("Id").Value))
                       .ToList();
                var count = changedItemIds.Count();
                foreach (var changedItemId in changedItemIds)
                {
                    var previousElement = previousPatch.Element(containerName).Elements(subContainerName).First(p => p.Attribute("Id").Value == changedItemId);

                    var newElement = addedChangedItems.Elements(subContainerName).Where(x => x.Attribute("Id").Value == changedItemId).First();

                    var changedItem = addedChangedItems.Elements().First(x => x.Attribute("Id").Value == changedItemId);
                    changedItem.ReplaceWith(CompareElements(previousElement, newElement));
                    if (changedItem != null && changedItem.Attribute("Status") == null)
                    {
                        changedItem.Add(new XAttribute("Status", "Changed"));
                    }
                }
            }

            List<string> newItemIds = addedChangedItems.Elements(subContainerName) // Separate out the new elements and tag them with the status attribute
                       .Select(i => i.Attribute("Id").Value)
                   .Except(previousPatch.Element(containerName).Elements(subContainerName)
                       .Select(i => i.Attribute("Id").Value))
                   .ToList();
            foreach (var newItemId in newItemIds)
            {
                var newItem = addedChangedItems.Elements().First(x => x.Attribute("Id").Value == newItemId);
                newItem.Add(new XAttribute("Status", "New"));
            }

            List<string> removedItemIds = previousPatch.Element(containerName).Elements(subContainerName)  //Separate out the removed elements and tag them with the status attribute
                       .Select(i => i.Attribute("Id").Value)
                   .Except(items.Elements(subContainerName)
                       .Select(i => i.Attribute("Id").Value))
                   .ToList();
            foreach (var removedItemId in removedItemIds)
            {
                var removedItem = previousPatch.Element(containerName).Elements().First(x => x.Attribute("Id").Value == removedItemId);
                removedItem.Add(new XAttribute("Status", "Removed"));
                addedChangedItems.Add(removedItem); // Add removed elements to the return value
            }

            previousPatch = null; // Trashing this
            return addedChangedItems;
        }

        private XElement CompareElements(XElement previousElement, XElement newElement)
        {
            List<XElement> elementsToRemove = new List<XElement>();
            int unmodifiedBaseElemCount = 0;
            if (previousElement != null)
            {
                IEnumerable<XNode> changedElements;
                List<string> removedItemIds = new List<string>();
                IEnumerable<XNode> removedElements = previousElement.Elements().Where(x => !x.HasAttributes).Cast<XNode>()  //separate out the removed elements and remove them
                        .Except(newElement.Elements().Where(x => !x.HasAttributes).Cast<XNode>(),
                        new XNodeEqualityComparer());

                removedItemIds = previousElement.Elements().Where(x => x.HasAttributes)  //separate out the removed elements and tag them with the status attribute
                       .Where(x => x.Attributes().Any(a => a.Name == "Id"))
                       .Select(i => i.Attribute("Id").Value)
                   .Except(newElement.Elements().Where(x => x.HasAttributes)
                       .Where(x => x.Attributes().Any(a => a.Name == "Id"))
                       .Select(i => i.Attribute("Id").Value))
                   .ToList();

                if (newElement.Elements().Count() > 200) // Have to handle string tables separately, as they can have tens of thousands of sub-elements with the same name.
                {
                    changedElements = newElement.Elements().Cast<XNode>()
                       .Except(previousElement.Elements().Cast<XNode>(),
                       new XNodeEqualityComparer());

                    if (removeUnchangedElements)
                    {
                        newElement.Elements().Cast<XNode>()
                        .Except(changedElements.Cast<XNode>(),
                        new XNodeEqualityComparer()).Remove();
                    }

                    // Remove the unchanged, and non-removed entries from the previous element to speed up enumeration of it when loading changed values
                    var changedItemIds = changedElements
                        .Where(x => (x as XElement).HasAttributes)
                       .Select(i => ((XElement)i).Attribute("Id").Value)
                       .ToList();

                    previousElement.Elements().Cast<XNode>()
                        .Except(previousElement.Elements().Where(x => x.HasAttributes)
                            .Where(x => changedItemIds.Contains(x.Attribute("Id").Value)))
                        .Except(previousElement.Elements().Where(x => x.HasAttributes)
                        .Where(x => x.Attribute("Id") != null)
                            .Where(x => removedItemIds.Contains(x.Attribute("Id").Value)));

                }
                else
                {
                    changedElements = newElement.Elements();
                }

                for (int n = 0; n < changedElements.Count(); n++)
                {
                    XElement subElement = changedElements.ElementAt(n) as XElement;

                    XElement prevSubElement = null;
                    IEnumerable<XElement> previousSubElements = previousElement.Elements(subElement.Name);

                    var prevSubEleCount = previousSubElements.Count();
                    if (prevSubEleCount > 0)
                    {
                        if (subElement.Attribute("Id") != null)
                        {
                            if (prevSubEleCount > 1)
                            {
                                var potentialElements = previousSubElements.Where(x => x.Attributes("Id").Count() != 0).Where(x => x.Attribute("Id").Value == subElement.Attribute("Id").Value);

                                int potEleCount = potentialElements.Count();
                                if (potEleCount == 0)
                                {
                                    prevSubElement = null;
                                }
                                else
                                {
                                    if (potEleCount > 1) { Addtolist2("Multiple potential matching elements for " + subElement.Name + " found"); }
                                    prevSubElement = potentialElements.First();
                                }
                            }
                            else
                            {
                                prevSubElement = previousSubElements.First();
                                if (prevSubElement.Attribute("Id") != null && subElement.Attribute("Id") != null)
                                {
                                    if (prevSubElement.Attribute("Id").Value != subElement.Attribute("Id").Value)
                                    {
                                        removedItemIds.Remove(prevSubElement.Attribute("Id").Value);
                                    }
                                }
                            }
                        }
                        else { prevSubElement = previousSubElements.First(); }
                    }

                    if (prevSubElement != null)
                    {
                        if (!XNode.DeepEquals(prevSubElement, subElement)) // Need to normalize the xml elements and compare them again to catch ordering of subelement changes. 
                        {
                            if (prevSubElement.HasElements)
                            {
                                if (subElement.HasElements)
                                {
                                    subElement.ReplaceWith(CompareElements(prevSubElement, subElement));
                                }
                                else
                                {
                                    try
                                    {
                                        newElement.Add(new XAttribute("OldValue", prevSubElement.Value.ToString()));
                                    }
                                    catch (Exception ex)
                                    {
                                        Console.WriteLine(ex.ToString());
                                    }

                                }
                            }
                            else
                            {
                                if (subElement.HasElements) subElement.Add(new XAttribute("Status", "New"));
                                else subElement.Add(new XAttribute("OldValue", prevSubElement.Value));
                            }
                        }
                        else
                        {
                            if (!(subElement.Name == "Name" || subElement.Name == "Description" || subElement.Name == "Fqn" || subElement.Name == "enMale" || subElement.Name == "Speaker"))
                            {
                                elementsToRemove.Add(subElement); // Element didn't change and it's clogging up the file, saving a shallow copy to remove it later. Because removing it now fucks with the foreach looping
                            }
                            else if (subElement.Attribute("OldValue") == null)
                            {
                                // If this is a "base" element that we want to keep but its unchanged then increment the counter.
                                unmodifiedBaseElemCount++;
                            }
                        }
                    }
                    else
                    {
                        subElement.Add(new XAttribute("Status", "New"));
                    }
                }

                for (int i = 0; i < removedItemIds.Count; i++)
                {
                    var removedItem = previousElement.Elements()
                        .Where(x => x.HasAttributes)
                        .Where(x => x.Attributes().Any(a => a.Name == "Id"))
                        .First(x => x.Attribute("Id").Value == removedItemIds[i]);
                    if (!removedItem.Attributes().Any(a => a.Name == "Status"))
                    {
                        removedItem.Add(new XAttribute("Status", "Removed"));
                        newElement.Add(removedItem); // Add removed elements to the return value
                    }
                }

                var newElementNames = newElement.Elements().Select(x => x.Name).ToList();
                foreach (var remElement in removedElements)
                {
                    var remX = remElement as XElement;
                    if (!remX.HasElements && !newElementNames.Contains(remX.Name))
                    {
                        ((XElement)remElement).Add(new XAttribute("Status", "Removed"));
                        newElement.Add(remElement);
                    }
                }
            }
            else
            {
                newElement.Add(new XAttribute("NewElement", "true"));
            }

            if (removeUnchangedElements) // Added this check so we can maintain the old functionality with the aid of a checkbox
            {
                for (int i = elementsToRemove.Count() - 1; i >= 0; i--)
                {
                    elementsToRemove[i].Remove(); // Removing elements we saved shallow copies off earlier
                }

                if (newElement.Elements().Count() == unmodifiedBaseElemCount)
                {
                    // If all we have are the "base" elements and none are changed then just delete the whole thing.
                    newElement = null;
                }
            }
            previousElement = null;

            return newElement;
        }

        #endregion
        public XElement ConvertToXElement(ulong itemId, DataObjectModel dom)
        {
            return ConvertToXElement(itemId, dom, false);
        }

        public XElement ConvertToXElement(ulong itemId, DataObjectModel dom, bool overrideVerbose)
        {
            var gomItm = dom.GetObject(itemId);
            return ConvertToXElement(gomItm, overrideVerbose);
        }

        public XElement ConvertToXElement(string fqn, DataObjectModel dom)
        {
            if (dom is null)
            {
                throw new ArgumentNullException(nameof(dom));
            }

            return ConvertToXElement(fqn, false);
        }

        public XElement ConvertToXElement(string fqn, DataObjectModel dom, bool overrideVerbose)
        {
            var gomItm = dom.GetObject(fqn);
            return ConvertToXElement(gomItm, overrideVerbose);
        }

        public XElement ConvertToXElement(GomObject gomItm)
        {
            return ConvertToXElement(gomItm, false);
        }

        public XElement ConvertToXElement(GomObject gomItm, bool overrideVerbose)
        {
            if (gomItm != null)
            {
                if (gomItm.Name.Contains("/"))
                    return null;
                switch (gomItm.Name.Substring(0, 3))
                {
                    case "abl":
                        if (!gomItm.Name.Contains("/"))
                            return new GameObject().ToXElement(gomItm, !overrideVerbose);
                        break;
                    case "itm":
                    case "npc":
                    case "qst":
                    case "cdx":
                    case "cnv":
                    case "ach":
                    case "tal":
                    case "sch":
                    case "dec": return new GameObject().ToXElement(gomItm, !overrideVerbose);
                    default: break;
                }
            }
            return null;
        }

        public XElement ConvertToXElement(object item) // Was tired of writing code that duplicated this functionality
        {
            return ConvertToXElement(item, false);
        }

        public XElement ConvertToXElement(object item, bool overrideVerbose) // Was tired of writing code that duplicated this functionality
        {
            if (item == null) return null;
            string type = item.GetType().ToString();
            switch (type)
            {
                case "GomLib.Models.Item":
                case "GomLib.Models.Npc":
                case "GomLib.Models.Ability":
                case "GomLib.Models.Quest":
                case "GomLib.Models.QuestItem":
                case "GomLib.Models.Codex":
                case "GomLib.Models.Conversation":
                case "GomLib.Models.Achievement":
                case "GomLib.Models.Talent":
                case "GomLib.Models.Schematic":
                case "GomLib.Models.Decoration":
                case "GomLib.Models.ItemAppearance":
                case "GomLib.Models.Stronghold":
                case "GomLib.Models.Room":
                case "GomLib.Models.Planet":
                case "GomLib.Models.AdvancedClass":
                case "GomLib.Models.Discipline":
                // Broken
                // case "GomLib.Models.AbilityPackage": return ((GomLib.Models.GameObject)item).ToXElement(!overrideVerbose);
                case "GomLib.Models.Conquest":
                case "GomLib.Models.scFFShip":
                case "GomLib.Models.Companion":
                case "GomLib.Models.Collection":
                // Broken
                // case "GomLib.Models.MtxStorefrontEntry": return ((GomLib.Models.PseudoGameObject)item).ToXElement(!overrideVerbose);
                // Broken
                // case "GomLib.Models.ConquestObjective": return ((GomLib.Models.ConquestObjective)item).ToXElement(!overrideVerbose);
                // Broken
                // case "GomLib.Models.ConquestData": return ((GomLib.Models.ConquestData)item).ToXElement(!overrideVerbose);
                default:
                    if (item is GameObject @object)
                        return @object.ToXElement(!overrideVerbose);
                    else if (item is PseudoGameObject object1)
                        return object1.ToXElement(!overrideVerbose);
                    break;
            }
            return null;
        }

        private static XElement ReferencesToXElement(Dictionary<string, SortedSet<ulong>> refs)
        {
            XElement references = new XElement("References");
            if (refs != null)
            {
                foreach (KeyValuePair<string, SortedSet<ulong>> entry in refs)
                {
                    XElement tmpEle = new XElement(entry.Key);
                    foreach (ulong ele in entry.Value)
                    {
                        tmpEle.Add(new XElement("Ref", ele));
                    }
                    references.Add(tmpEle);
                }
            }
            return references;
        }

        private static XElement ReferencesToXElement(Dictionary<ulong, string> refs)
        {
            XElement references = new XElement("References");
            if (refs != null)
            {
                foreach (var entry in refs)
                {
                    XElement tmpEle = new XElement("Ref", entry.Value, new XAttribute("Id", entry.Key));
                    references.Add(tmpEle);
                }
            }
            return references;
        }

        #endregion

        #region Json Functions

        public string ConvertToJson(object itm) // Obsolete
        {
            XElement element = ConvertToXElement(itm); // Convert the achievement to XElement

            element.DescendantsAndSelf() // Descendants() grabs all the XElements at every depth, Elements() only grabs the immediate child XElements, and I threw in Self to get the parent XElement, too
                .Where(x => x.HasAttributes) // Look for only the descendant XElements with XAttributes so we don't try to execute an object method of a null object
                .Where(x => x.Attribute("Id") != null) // Second check to make sure that the XAttribute "Id" is present
                .Attributes("Id") // Suck up the "Id" in an IEnumerable<XAttribute> so we can use the Remove() method on them all at once
                .Remove(); // Remove the "Id" XAttributes that are used for comparison purposes as they clog up the resulting Json.

            /*
             * XDocument is the newer LINQ to XML format while XmlDocument is the older and harder to manipulate format.
             * You can build and manipulate XDocuments and the related XNode types like I did above for the XAttributes with common LINQ constructors
             * 
             * But the Json serializer only works on XmlDocuments
             * So I put a helper method ToXmlDocument in Tools.cs to handle the conversion.
             */

            string jsonString = Newtonsoft.Json.JsonConvert.SerializeXmlNode(
                new XDocument(element) // Wrap the achievement in an XDocument
                    .ToXmlDocument(), // Convert to XmlDocument 
                Newtonsoft.Json.Formatting.None, // Set the output formatting option
                true // Omit the root object
                );

            return jsonString;
        }

        private void GameObjectListAsJSON(string prefix, List<GameObject> itmList)
        {
            int i = 0;
            short e = 0;
            string n = Environment.NewLine;
            var txtFile = new StringBuilder();
            string filename = string.Format("\\json\\{0}{1}", prefix, ".json");
            WriteFile(string.Format("{0}{1}", patchVersion, n), filename, false);
            var count = itmList.Count();
            HashTableHashing.MurmurHash2Unsafe jsonHasher = new HashTableHashing.MurmurHash2Unsafe();

            for (int b = itmList.Count - 1; b >= 0; b--) // Go backwards so we can delete values
            {
                ProgressUpdate(i, count);
                if (e % 1000 == 1)
                {
                    WriteFile(txtFile.ToString(), filename, true);
                    txtFile.Clear();
                    e = 0;
                }

                Addtolist2(string.Format("{0}: {1}", prefix, itmList[b].Fqn));

                string jsonString = itmList[b].ToJSON();
                uint hash = jsonHasher.Hash(Encoding.ASCII.GetBytes(jsonString));
                txtFile.Append(string.Format("{0},{1},{2}{3}", itmList[b].Base62Id, hash, jsonString, Environment.NewLine)); // Append it with a newline to the output.
                itmList[b] = null;
                i++;
                e++;
            }

            Addtolist(string.Format("The {0} json file has been generated; there were {1} {0}", prefix, i));
            WriteFile(txtFile.ToString(), filename, true);
            DeleteEmptyFile(filename, i);
            GC.Collect();
            CreateGzip(filename);
            ClearProgress();
        }

        private void PseudoGameObjectListAsJSON(string prefix, List<PseudoGameObject> itmList)
        {
            int i = 0;
            short e = 0;
            string n = Environment.NewLine;
            var txtFile = new StringBuilder();
            string filename = string.Format("\\json\\{0}{1}", prefix, ".json");
            WriteFile(string.Format("{0}{1}", patchVersion, n), filename, false);
            var count = itmList.Count();
            HashTableHashing.MurmurHash2Unsafe jsonHasher = new HashTableHashing.MurmurHash2Unsafe();
            for (int c = 0; c < count; c++)
            {
                ProgressUpdate(i, count);
                if (e % 1000 == 1)
                {
                    WriteFile(txtFile.ToString(), filename, true);
                    txtFile.Clear();
                    e = 0;
                }

                Addtolist2(string.Format("{0}: {1}", prefix, itmList[c].Name));

                string jsonString = itmList[c].ToJSON();
                uint hash = jsonHasher.Hash(Encoding.ASCII.GetBytes(jsonString));
                txtFile.Append(string.Format("{0},{1},{2}{3}", itmList[c].Base62Id, hash, jsonString, Environment.NewLine)); //Append it with a newline to the output.
                itmList[c] = null;
                i++;
                e++;
            }
            Addtolist(string.Format("The {0} json file has been generated; there were {1} {0}", prefix, i));
            WriteFile(txtFile.ToString(), filename, true);
            DeleteEmptyFile(filename, i);
            GC.Collect();
            CreateGzip(filename);
            ClearProgress();
        }

        #endregion

        #region SQL Functions
        private void ObjectListAsSql(string prefix, string xmlRoot, IEnumerable<object> itmList)
        {
            if (prefix == "Removed") return; //not supported as of yet.
            if (itmList.Count() == 0) return;
            int i = 0;
            short e = 0;
            int f = 1;
            string n = Environment.NewLine;
            var txtFile = new StringBuilder();
            string filename = string.Format("\\sql\\{0}{1}", prefix, xmlRoot);
            string frs = "{0}.sql";
            WriteFile("", string.Format(frs, filename, f), false);
            var count = itmList.Count();

            string transQuery;
            if (initTable.TryGetValue(xmlRoot, out SQLInitStore transInit)) //verify that there is an SQL Transaction Query for this object type
            {
                transQuery = transInit.InitBegin + n;
                if (sql)
                    SqlTransactionsInitialize(transInit.InitBegin, transInit.InitEnd); //initialize the SQL tranaction if direct SQL output is enabled.
            }
            else
            {
                Addtolist2(string.Format("Output type not supported for: {0}", xmlRoot));
                return;
            }
            string joiner = ",";
            foreach (var itm in itmList)
            {
                if (i == count - 1)
                    joiner = "";
                ProgressUpdate(i, count);

                if (false)
                {
                }

                Addtolist2(string.Format("{0}: {1}", prefix, GetObjectText(itm)));

                string sqlString = ToSQL(itm);
                if (sql)
                    SqlAddTransactionValue(sqlString); //Add to current SQL Transaction if direct SQL output is enabled.
                txtFile.Append(string.Join(joiner, sqlString, n)); //Append it with a newline to the output.
                i++;
                e++;
            }
            txtFile.Append(transInit.InitEnd);
            Addtolist(string.Format("The {0} sql file has been generated; there were {1} {0}", prefix, i));
            WriteFile(transQuery, string.Format(frs, filename, f), false);
            WriteFile(txtFile.ToString(), string.Format(frs, filename, f), true);
            initTable[xmlRoot].OutputCreationSQL(); //output the creation sql file for this table
            SqlTransactionsFlush(); //flush the transaction queue
            DeleteEmptyFile(string.Format(frs, filename, f), i);
            GC.Collect();
            for (int j = 1; j <= f; j++)
            {
                CreateGzip(string.Format(frs, filename, j)); //compresses output for upload
            }
            ClearProgress();
        }

        private string GetObjectText(object obj)
        {
            if (obj is GameObject @object)
            {
                return @object.Fqn;
            }
            else if (obj is PseudoGameObject object1)
            {
                return object1.Name;
            }

            return "";
        }

        #endregion
    }
}
