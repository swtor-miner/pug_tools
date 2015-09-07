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
using GomLib.Models;
using TorLib;

namespace tor_tools
{
    public partial class Tools
    {
        #region New Flexible Processing Functions

        public void getObjects(string prefix, string elementName)
        {
            Clearlist2();
            ClearProgress();
            LoadData();
            addtolist(String.Format("Getting {0}", elementName));
            switch (elementName)
            {
                case "Abilities": //This section is for exploring Ability Effects
                    ProcessGameObjects(prefix, elementName);
                    currentDom.abilityLoader.effKeys.Sort();
                    string effKeyList = String.Join(Environment.NewLine, currentDom.abilityLoader.effKeys);
                    WriteFile(effKeyList, "effKeys.txt", false);

                    currentDom.abilityLoader.effWithUnknowns = currentDom.abilityLoader.effWithUnknowns.Distinct().OrderBy(o => o).ToList();
                    string effUnknowns = String.Join(Environment.NewLine, currentDom.abilityLoader.effWithUnknowns);
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
        public void getPrototypeObjects(string xmlRoot, string prototype, string dataTable)
        {
            Clearlist2();
            ClearProgress();
            LoadData();
            addtolist(String.Format("Getting {0}", xmlRoot));
            ProcessProtoData(xmlRoot, prototype, dataTable);

            FlushTempTables();
            EnableButtons();
        }

        public void ProcessGameObjects(string gomPrefix, string xmlRoot)
        {
            if (!OutputCompatible(xmlRoot))
            {
                ClearProgress();
                FlushTempTables();
                return;
            }
            IEnumerable<GomObject> curItmList = getMatchingGomObjects(currentDom, gomPrefix);

            var curItmNames = curItmList.Select(x => x.Name).ToList();

            Dictionary<string, List<GomLib.Models.GameObject>> ObjectLists = new Dictionary<string, List<GomLib.Models.GameObject>>();
            var chaItems = new Dictionary<GomLib.Models.GameObject, GomLib.Models.GameObject>();
            var newItems = new List<GomLib.Models.GameObject>();
            var remItems = new List<GomLib.Models.GameObject>();

            int i = 0;
            int count = 0;
            if (chkBuildCompare.Checked)
            {
                var prevItmNames = getMatchingGomObjects(previousDom, gomPrefix).Select(x => x.Name).ToList();
                var removedNames = prevItmNames.Except(curItmNames).ToList(); //couldn't find a more elegant way to do this with linq.

                ClearProgress();

                i = 0;
                count = curItmList.Count() + removedNames.Count;
                foreach (var curObject in curItmList)
                {
                    GomLib.Models.GameObject curItm = LoadGameObject(currentDom, curObject);

                    progressUpdate(i, count);
                    var prevObject = previousDom.GetObject(((GomObject)curObject).Name);

                    if (prevObject != null)
                    {
                        GomLib.Models.GameObject prevItm = LoadGameObject(previousDom, prevObject);

                        if (!prevItm.Equals(curItm))
                        {
                            addtolist2(String.Join("", "Changed: ", curItm.Fqn));
                            chaItems.Add(prevItm, curItm);
                        }
                    }
                    else
                    {
                        addtolist2(String.Join("", "New: ", curItm.Fqn));
                        newItems.Add(curItm);
                    }
                    curObject.Unload();
                    i++;
                }

                foreach (var removedName in removedNames)
                {
                    progressUpdate(i, count);
                    addtolist2(String.Join("", "Removed: ", removedName));
                    GomLib.Models.GameObject prevItm = LoadGameObject(previousDom, previousDom.GetObject(removedName));

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
                    progressUpdate(i, count);
                    newItems.Add(LoadGameObject(curObject._dom, curObject));
                    i++;
                }
                ObjectLists.Add("Full", newItems);
            }

            Clearlist2();
            addtolist2(String.Format("Generating {0} Output", outputTypeName));
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
                    GameObjectListAsJSON(String.Join("", itmList.Key, xmlRoot), itmList.Value);
                else if (outputTypeName == "Text")
                    GameObjectListAsText(String.Join("", itmList.Key, xmlRoot), itmList.Value);
                else if (outputTypeName == "SQL")
                    ObjectListAsSql(itmList.Key, xmlRoot, itmList.Value);
                else
                {
                    if (itmList.Key == "Changed")
                    {

                        foreach (var changedPair in chaItems)
                        {
                            progressUpdate(i, count);
                            XElement oldElement = ConvertToXElement(changedPair.Key);
                            XElement newElement = ConvertToXElement(changedPair.Value);

                            newElement = CompareElements(oldElement, newElement);
                            oldElement = null;


                            if (newElement != null)
                            {
                                newElement.Add(new XAttribute("Status", itmList.Key));
                                /*switch (xmlRoot)
                                {
                                    case "Decorations":
                                        if (newElement.Elements().Count() == 2)
                                            i++;
                                            continue;
                                }*/
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
                            progressUpdate(i, count);
                            XElement itemElement = ConvertToXElement(itm);
                            if ((itmList.Key == "New" || itmList.Key == "Full") && gomPrefix == "ach.")
                            {
                                if (((GomLib.Models.Achievement)itm).AchId == 0)
                                {
                                    WriteFile(itm.Fqn + " : " + ((GomLib.Models.Achievement)itm).Name + Environment.NewLine, "brokenAchieves.txt", true);
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
                var filename = String.Join("", xmlRoot, ".xml");
                string outputComment = "total";
                if (chkBuildCompare.Checked)
                {
                    filename = String.Join("", "Changed", filename);
                    outputComment = "new/changed/removed ";
                }

                elements = Sort(elements);
                addtolist(String.Format("{0} - {1} {2}", xmlRoot, elements.Elements().Count(), outputComment));
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
            int i = 0;
            int count = 0;

            Dictionary<object, object> currentDataProto = new Dictionary<object, object>();
            GomObject currentDataObject = currentDom.GetObject(prototype);
            if (currentDataObject != null) //fix to ensure old game assets don't throw exceptions.
            {
                currentDataProto = currentDataObject.Data.Get<Dictionary<object, object>>(dataTable);
                currentDataObject.Unload();
            }

            var curIds = currentDataProto.Keys;
            Dictionary<string, List<GomLib.Models.PseudoGameObject>> ObjectLists = new Dictionary<string, List<GomLib.Models.PseudoGameObject>>();
            var chaItems = new Dictionary<GomLib.Models.PseudoGameObject, GomLib.Models.PseudoGameObject>();
            var newItems = new List<GomLib.Models.PseudoGameObject>();

            if (chkBuildCompare.Checked)
            {
                Dictionary<object, object> previousDataProto = new Dictionary<object, object>();
                GomObject previousDataObject = previousDom.GetObject(prototype);
                if (previousDataObject != null) //fix to ensure old game assets don't throw exceptions.
                {
                    previousDataProto = previousDataObject.Data.Get<Dictionary<object, object>>(dataTable);
                    previousDataObject.Unload();
                }


                var prevIds = previousDataProto.Keys;

                var remItems = new List<GomLib.Models.PseudoGameObject>();
                var removedIds = prevIds.Except(curIds).ToList();

                i = 0;
                count = curIds.Count() + removedIds.Count;
                ClearProgress();
                //int percent = count / 10;
                foreach (var id in curIds)
                {
                    progressUpdate(i, count);
                    object curData;
                    currentDataProto.TryGetValue(id, out curData);
                    GomLib.Models.PseudoGameObject curObj = PseudoGameObject.Load(xmlRoot, currentDom, id, curData);

                    object prevData;
                    previousDataProto.TryGetValue(id, out prevData);
                    GomLib.Models.PseudoGameObject prevObj = PseudoGameObject.Load(xmlRoot, previousDom, id, prevData);

                    if (prevObj.Id != 0)
                    {
                        if (!prevObj.Equals(curObj))
                        {
                            addtolist2(String.Join("", "Changed: ", curObj.Name));
                            chaItems.Add(prevObj, curObj);
                        }
                    }
                    else
                    {
                        addtolist2(String.Join("", "New: ", curObj.Name));
                        newItems.Add(curObj);
                    }

                    i++;
                }

                foreach (var removedId in removedIds)
                {
                    progressUpdate(i, count);

                    object prevData;
                    previousDataProto.TryGetValue(removedId, out prevData);
                    GomLib.Models.PseudoGameObject prevObj = PseudoGameObject.Load(xmlRoot, previousDom, removedId, prevData);

                    addtolist2(String.Join("", "Removed: ", prevObj.Name));
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
                    progressUpdate(i, count);
                    object curData;
                    currentDataProto.TryGetValue(id, out curData);
                    GomLib.Models.PseudoGameObject curObj = PseudoGameObject.Load(xmlRoot, currentDom, id, curData);
                    newItems.Add(curObj);
                    i++;
                }
                ObjectLists.Add("Full", newItems);
            }

            Clearlist2();
            addtolist2(String.Format("Generating {0} Output", outputTypeName));
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
                    PseudoGameObjectListAsJSON(String.Join("", itmList.Key, xmlRoot), itmList.Value);
                else if (outputTypeName == "Text")
                    PseudoGameObjectListAsText(String.Join("", itmList.Key, xmlRoot), itmList.Value);
                else if (outputTypeName == "SQL")
                    ObjectListAsSql(itmList.Key, xmlRoot, itmList.Value);
                else
                {
                    if (itmList.Key == "Changed")
                    {
                        foreach (var changedPair in chaItems)
                        {
                            progressUpdate(i, count);
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
                            progressUpdate(i, count);
                            XElement itemElement = itm.ToXElement();
                            if (itmList.Key != "Full") itemElement.Add(new XAttribute("Status", itmList.Key));
                            //itemElement.Add(ReferencesToXElement(itm.References));

                            if(itmList.Key == "New")
                            {
                                if (xmlRoot == "Collections")
                                {
                                    //Named icon export.
                                    //Bleh item specific code here.
                                    Collection colItm = itm as Collection;
                                    if (colItm != null)
                                    {
                                        ExportIconFromPath("/resources/gfx/mtxstore/" + colItm.Icon + "_400x400.dds", colItm.Name,
                                            "/MtxImages/Named/Collections/{0}.png");
                                    }
                                }
                                else if (xmlRoot == "MtxStoreFronts")
                                {
                                    MtxStorefrontEntry mtxItm = itm as MtxStorefrontEntry;
                                    if (mtxItm != null)
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
                var filename = String.Join("", xmlRoot, ".xml");
                string outputComment = "";
                if (chkBuildCompare.Checked)
                {
                    filename = String.Join("", "Changed", filename);
                    outputComment = "new/changed/removed ";
                }

                elements = Sort(elements);
                addtolist(String.Format("{0} - {1} {2}", xmlRoot, elements.Elements().Count(), outputComment));
                xmlDoc.Add(elements);
                WriteFile(xmlDoc, filename, false, false);
            }
            ClearProgress();

            FlushTempTables();
        }

        private IEnumerable<GomObject> getMatchingGomObjects(DataObjectModel dom, string gomPrefix)
        {           //This function is meant to handle unique cases of loading a list of objects from the GOM
            IEnumerable<GomObject> itmList;
            switch (gomPrefix)
            {
                case "abl.":
                    itmList = dom.GetObjectsStartingWith(gomPrefix).Where(x => !x.Name.Contains("/")); //Abilities with a / in the name are Effects.
                    break;
                case "eff.":
                    itmList = dom.GetObjectsStartingWith("abl.").Where(x => x.Name.Contains("/"));
                    break;
                default:
                    itmList = dom.GetObjectsStartingWith(gomPrefix); //No need for the extra Linq statement for non-unique cases
                    break;
            }
            return itmList;
        }

        private void ProcessGomFields()
        {
            XDocument currentFields = currentDom.ReturnTypeNames(); //XDocument.Load(Config.ExtractPath + prefix + "Gom_Fields.xml");            
            if (chkBuildCompare.Checked)
            {
                XDocument previousFields = previousDom.ReturnTypeNames(); //XDocument.Load(Config.ExtractPath + prefix + "Gom_Fields.xml");
                //XElement addedItems = FindChangedEntries(currentFields.Root, "Gom_Fields", "Gom_Field");
                WriteFile(new XDocument(currentFields.Root.Element("FieldUseInDomClass")), "CurrentGom_Fields.xml", false);
                WriteFile(new XDocument(previousFields.Root.Element("FieldUseInDomClass")), "PreviousGom_Fields.xml", false);
                XElement wrapper = new XElement("Wrapper");
                XElement compared = FindChangedEntries(currentFields.Root.Element("Gom_Fields"), new XDocument(previousFields.Root.Element("Gom_Fields")), "Gom_Fields", "Gom_Field");
                wrapper.Add(compared);
                /*compared.ReplaceNodes(compared.Elements("Gom_Field")
                    .OrderBy(x => (string)x.Attribute("Status"))
                    .ThenBy(x => (string)x.Attribute("Id")));*/
                XElement compared2 = FindChangedEntries(currentFields.Root.Element("FieldUseInDomClass"), new XDocument(previousFields.Root.Element("FieldUseInDomClass")), "FieldUseInDomClass", "DomClass");
                //compared = CompareElements(previousFields.Root.Element("FieldUseInDomClass"), currentFields.Root.Element("FieldUseInDomClass"));
                /*compared2.ReplaceNodes(compared.Elements("DomClass")
                    .OrderBy(x => (string)x.Attribute("Status"))
                    .ThenBy(x => (string)x.Attribute("Id")));*/
                wrapper.Add(compared2);

                XDocument xmlDoc = new XDocument(wrapper);
                if (!xmlDoc.Root.IsEmpty) WriteFile(xmlDoc, "ChangedGom_Fields.xml", false);
            }
            else
                WriteFile(currentFields, "Gom_Fields.xml", false);
        }

        private bool ExportGOM = false;
        private bool ExportNPP = false;
        private bool ExportICONS = false;
        private void ProcessEffectChanges()
        {
            IEnumerable<GomObject> currentAblObjects = currentDom.GetObjectsStartingWith("abl.").Where(x => x.GetType() == typeof(GomObject));
            IEnumerable<GomObject> previousAblObjects = previousDom.GetObjectsStartingWith("abl.").Where(x => x.GetType() == typeof(GomObject));

            //Build a dictionary of effects so we can quickly look them up.
            Dictionary<string, GomObject> currentEffectByNameID = new Dictionary<string, GomObject>();
            foreach(GomObject obj in currentAblObjects)
            {
                if(!obj.Name.Contains("/"))
                {
                    //Only care about effect nodes.
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
                    //Only care about effect nodes.
                    continue;
                }

                string[] nameArray = obj.Name.Split('/');
                string name = nameArray[0] + '/' + nameArray[2];
                previousEffectByNameID.Add(name, obj);
            }

            foreach(KeyValuePair<string, GomObject> currentPair in currentEffectByNameID)
            {
                GomObject prevObj;
                if(previousEffectByNameID.TryGetValue(currentPair.Key, out prevObj))
                {
                    if(!prevObj.Equals(currentPair.Value))
                    {
                        //Effect node not equal!
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

                            WriteFile(new XDocument(newElement), String.Format("\\GOM\\ChangedEffects\\{0}.xml", newText), false);
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
                var removedNames = previousNames.Except(currentNames).ToList(); //couldn't find a more elegant way to do this with linq.

                ClearProgress();
                i = 0;
                count = currentObjects.Count() + removedNames.Count;
                foreach (var curObject in currentObjects)
                {
                    progressUpdate(i, count);
                    var prevObject = previousDom.GetObject(((GomObject)curObject).Name);
                    if (prevObject != null)
                    {
                        if (!prevObject.Equals(curObject))
                        {
                            //Outputting these changes is a huge time sink for something that isn't really useful.
                            //addtolist2(String.Format("Changed: {0}", curObject.Name));
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
                    progressUpdate(i, count);
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

            addtolist(String.Format("Generating Output {0}", outputTypeName));
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
                progressUpdate(i, count);
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
                filename = String.Format("{0}{1}", "Changed", filename);
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
            addtolist("GOM Items - " + (gomItems.Elements("GOM_Item").Count() + z_effects.Elements("GOM_Item").Count()));
            xmlDoc.Add(new XElement("Container", gomItems, z_effects));
            WriteFile(xmlDoc, filename, false, false);

            if (chkBuildCompare.Checked)
            {
                if (ExportGOM) //MessageBox.Show("Export changed GOM Objects for anaylsis?", "Select an Option", MessageBoxButtons.YesNo) == System.Windows.Forms.DialogResult.Yes)
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

                                    WriteFile(new XDocument(newElement), String.Format("\\GOM\\{0}\\{1}.xml", objList.Key, newText), false);
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

                                WriteFile(new XDocument(newElement), String.Format("\\GOM\\{0}\\{1}.xml", objList.Key, newText), false);
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


                HashFileInfo hashInfo = new HashFileInfo(iconFile.FileInfo.ph, iconFile.FileInfo.sh, iconFile);
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
                    exp.SaveImageToStream(dds, DevIL.ImageType.Png, outputStream); //save DDS to stream in PNG format

                    name += "_" + stateName;
                    foreach(char character in System.IO.Path.GetInvalidFileNameChars())
                    {
                        //Make sure the name doesn't contain invalid characters.
                        name = name.Replace(character, '-');
                    }

                    WriteFile(outputStream, String.Format(exportPath, name));
                }
            }
        }
        #endregion

        #region Txt functions

        private void GameObjectListAsText(string prefix, IEnumerable<GomLib.Models.GameObject> itmList)
        {
            int i = 0;
            short e = 0;
            string n = Environment.NewLine;
            var txtFile = new StringBuilder();
            string filename = String.Join("", prefix, ".txt");
            string headerRow = "";
            if (itmList.Count() == 0) return;
            headerRow = GetHeaderRow(
                prefix
                    .Replace("Changed", "")
                    .Replace("Full", "")
                    .Replace("New", "")
                    .Replace("Removed", ""),
                itmList.First()._dom);
            WriteFile(headerRow, filename, false);
            var count = itmList.Count();
            foreach (var itm in itmList)
            {
                progressUpdate(i, count);
                if (e % 1000 == 1)
                {
                    WriteFile(txtFile.ToString(), filename, true);
                    txtFile.Clear();
                    e = 0;
                }

                addtolist2(String.Format("{0}: {1}", prefix, itm.Fqn));

                string textString = ConvertToText(itm); // ConvertToJson(itm); //added method in Tools.cs
                txtFile.Append(textString + Environment.NewLine); //Append it with a newline to the output.
                i++;
                e++;
            }
            addtolist(String.Join("", "The ", prefix, " text file has been generated; there are ", i, " ", prefix));
            WriteFile(txtFile.ToString(), filename, true);
            DeleteEmptyFile(filename);
            ClearProgress();
        }
        private void PseudoGameObjectListAsText(string prefix, IEnumerable<GomLib.Models.PseudoGameObject> itmList)
        {
            int i = 0;
            short e = 0;
            string n = Environment.NewLine;
            var txtFile = new StringBuilder();
            string filename = String.Format("{0}{1}", prefix, ".txt");
            WriteFile("", filename, false);
            var count = itmList.Count();
            Dictionary<int, string> conqOrder = null;
            if (prefix.Contains("Conquest"))
            {
                WriteFile("", "ConquestSCSV.txt", false);
                WriteFile("", "ConquestOrder.txt", false);
            }

            foreach (var itm in itmList)
            {
                progressUpdate(i, count);
                if (e % 1000 == 1)
                {
                    WriteFile(txtFile.ToString(), filename, true);
                    txtFile.Clear();
                    e = 0;
                }

                addtolist2(String.Format("{0}: {1}", prefix, itm.Name));

                string jsonString = ConvertToText(itm);
                txtFile.Append(jsonString + Environment.NewLine); //Append it with a newline to the output.
                i++;
                e++;
                if (prefix.Contains("Conquest"))
                {
                    if (conqOrder == null)
                        conqOrder = new Dictionary<int, string>();
                    if (((GomLib.Models.Conquest)itm).ActiveData != null)
                        foreach (var actDat in ((GomLib.Models.Conquest)itm).ActiveData)
                        {
                            conqOrder.Add(actDat.ActualOrderNum, String.Format("{0}: {1} EST - {2}", actDat.ActualOrderNum, actDat.StartTime.ToString(), ((GomLib.Models.Conquest)itm).Name));
                        }
                    WriteFile(((GomLib.Models.Conquest)itm).ConquestToSCSV(), "ConquestSCSV.txt", true);
                }
            }
            if (prefix.Contains("Conquest"))
                WriteFile(String.Join(Environment.NewLine, conqOrder.OrderBy(x => x.Key).Select(x => x.Value)), "ConquestOrder.txt", true);
            addtolist(String.Join("", "The ", prefix, " text file has been generated; there are ", i, " ", prefix));
            WriteFile(txtFile.ToString(), filename, true);
            DeleteEmptyFile(filename);
            ClearProgress();
        }
        private void GomObjectListAsText(string prefix, IEnumerable<GomLib.GomObject> itmList)
        {
            int i = 0;
            short e = 0;
            string n = Environment.NewLine;
            var txtFile = new StringBuilder();
            string filename = String.Join("", prefix, ".txt");
            WriteFile("", filename, false);
            var count = itmList.Count();
            foreach (var itm in itmList)
            {
                progressUpdate(i, count);
                if (e % 1000 == 1)
                {
                    WriteFile(txtFile.ToString(), filename, true);
                    txtFile.Clear();
                    e = 0;
                }

                addtolist2(String.Format("{0}: {1}", prefix, itm.Name));

                string jsonString = ConvertToText(itm); // ConvertToJson(itm); //added method in Tools.cs
                txtFile.Append(jsonString + Environment.NewLine); //Append it with a newline to the output.
                i++;
                e++;
            }
            addtolist(String.Join("", "The ", prefix, " text file has been generated; there are ", i, " ", prefix));
            WriteFile(txtFile.ToString(), filename, true);
            DeleteEmptyFile(filename);
            ClearProgress();
        }

        private static string GetHeaderRow(string prefix, DataObjectModel dom)
        {
            switch (prefix)
            {
                case "Decorations":
                    var hookNameList = dom.decorationLoader.HookList.Select(x => x.Value.Name).ToList();
                    hookNameList.Sort();
                    string hookString = String.Join(";", hookNameList);
                    return String.Format("Name;Sources;Binding;Category;SubCategory;Purchase for Guild Cost;Stub Type;{0}{1}", hookString, Environment.NewLine);
                default: //don't have a predefined header row for this type
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
            return ConvertToText(fqn, false);
        }
        public string ConvertToText(string fqn, DataObjectModel dom, bool overrideVerbose)
        {
            var gomItm = dom.GetObject(fqn);
            return ConvertToText(gomItm, overrideVerbose);
        }
        public string ConvertToText(GomLib.GomObject gomItm)
        {
            return ConvertToText(gomItm, false);
        }
        public string ConvertToText(GomLib.GomObject gomItm, bool overrideVerbose)
        {
            if (gomItm != null)
            {
                if (gomItm.Name.Contains("/"))
                    return null;
                return ConvertToText(LoadGameObject(gomItm._dom, gomItm), overrideVerbose);
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
                //case "GomLib.Models.Item": return ItemToText((GomLib.Models.Item)item, overrideVerbose);
                /*case "GomLib.Models.Npc": return NpcToXElement((GomLib.Models.Npc)item, overrideVerbose);
                case "GomLib.Models.Ability": return AbilityToXElement((GomLib.Models.Ability)item, overrideVerbose);
                case "GomLib.Models.Quest": return QuestToXElement((GomLib.Models.Quest)item, overrideVerbose);
                case "GomLib.Models.QuestItem": return QuestItemToXElement((GomLib.Models.QuestItem)item, overrideVerbose);
                case "GomLib.Models.Codex": return CodexToXElement((GomLib.Models.Codex)item, overrideVerbose);*/
                case "GomLib.Models.Conversation":
                /*case "GomLib.Models.Achievement": return AchievementToXElement((GomLib.Models.Achievement)item, overrideVerbose);
                case "GomLib.Models.Talent": return TalentToXElement((GomLib.Models.Talent)item, overrideVerbose);
                case "GomLib.Models.Collection": return CollectionToXElement((GomLib.Models.Collection)item);
                case "GomLib.Models.MtxStorefrontEntry": return MtxStoreFrontToXElement((GomLib.Models.MtxStorefrontEntry)item);
                case "GomLib.Models.Companion": return CompanionToXElement((GomLib.Models.Companion)item);
                case "GomLib.Models.scFFShip": return ShipToXElement((GomLib.Models.scFFShip)item);
                case "GomLib.Models.Schematic": return SchematicToXElement((GomLib.Models.Schematic)item);*/
                case "GomLib.Models.Decoration": 
                //case "GomLib.Models.ItemAppearance": return ItemAppearanceToXElement((GomLib.Models.ItemAppearance)item);*/
                case "GomLib.Models.Conquest": return ((GomLib.Models.PseudoGameObject)item).ToString(!overrideVerbose);
                default: break;
            }
            return null;
        }
        #endregion
        #region XML functions
        #region Xml Comparison Functions

        #region Deprecated
        /*private XElement FindChangedEntries(XElement items, string containerName, string subContainerName)
        {
            string filename = prefix + containerName + ".xml";
            WriteFile(new XDocument(items), containerName + ".xml", false); //output new data while we have it.
            if (!System.IO.File.Exists(textBoxPrevXMLFolder.Text + filename))
            {
                addtolist("Previous Data missing!!!");
                return items;
            }
            XDocument previousPatch = XDocument.Load(textBoxPrevXMLFolder.Text + filename);

            return FindChangedEntries(items, previousPatch, containerName, subContainerName);
        }*/
        #endregion

        private XElement FindChangedEntries(XElement items, XDocument previousPatch, string containerName, string subContainerName)
        {
            addtolist("Comparing to Previous Version.");
            Clearlist2();
            addtolist2("No Output will appear here.");
            addtolist2("Is this really necessary?");
            string filename = prefix + containerName + ".xml";

            XElement addedChangedItems = new XElement(containerName, items.Elements(subContainerName).Cast<XNode>()
                .Except(previousPatch.Element(containerName).Elements(subContainerName).Cast<XNode>(),
                new XNodeEqualityComparer()));

            // Secondary Comparison needed for the PTS that went backwards and forwards a patch. //
            /*previousPatch = XDocument.Load("c:\\swtor\\2.4.1\\" + filename);
            XElement secondaryChangedItems = new XElement(containerName, addedChangedItems.Elements(subContainerName).Cast<XNode>()
                .Except(previousPatch.Element(containerName).Elements(subContainerName).Cast<XNode>(),
                new XNodeEqualityComparer()));
            addedChangedItems = secondaryChangedItems;*/

            //This section should add a OldVersion element to every changed item which contains the old version of it.
            if (containerName == "GOM_Items")
            {
                if (items.Descendants("References") != null) //OldValue doesn't apply to Gom_Items
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

            List<string> newItemIds = addedChangedItems.Elements(subContainerName) //separate out the new elements and tag them with the status attribute
                       .Select(i => i.Attribute("Id").Value)
                   .Except(previousPatch.Element(containerName).Elements(subContainerName)
                       .Select(i => i.Attribute("Id").Value))
                   .ToList();
            foreach (var newItemId in newItemIds)
            {
                var newItem = addedChangedItems.Elements().First(x => x.Attribute("Id").Value == newItemId);
                newItem.Add(new XAttribute("Status", "New"));
            }

            List<string> removedItemIds = previousPatch.Element(containerName).Elements(subContainerName)  //separate out the removed elements and tag them with the status attribute
                       .Select(i => i.Attribute("Id").Value)
                   .Except(items.Elements(subContainerName)
                       .Select(i => i.Attribute("Id").Value))
                   .ToList();
            foreach (var removedItemId in removedItemIds)
            {
                var removedItem = previousPatch.Element(containerName).Elements().First(x => x.Attribute("Id").Value == removedItemId);
                removedItem.Add(new XAttribute("Status", "Removed"));
                addedChangedItems.Add(removedItem); //add removed elements to the return value
            }

            previousPatch = null; //trashing this
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

                if (newElement.Elements().Count() > 200)//newElement.Name == "StringTable" || newElement.Name == "Filename") //have to handle string tables separately, as they can have tens of thousands of sub-elements with the same name.
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

                    //remove the unchanged, and non-removed entries from the previous element to speed up enumeration of it when loading changed values
                    var changedItemIds = changedElements
                        .Where(x => (x as XElement).HasAttributes)
                       .Select(i => ((XElement)i).Attribute("Id").Value)
                       .ToList();

                    previousElement.Elements().Cast<XNode>()
                        .Except(previousElement.Elements().Where(x => x.HasAttributes)
                            .Where(x => changedItemIds.Contains(x.Attribute("Id").Value)))
                        .Except(previousElement.Elements().Where(x => x.HasAttributes)
                        .Where(x=> x.Attribute("Id") != null)
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
                                    prevSubElement = null; // new XElement(subElement.Name);
                                }
                                else
                                {
                                    if (potEleCount > 1) { addtolist2("Multiple potential matching elements for " + subElement.Name + " found"); }
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
                    /*else
                    {
                        prevSubElement = new XElement(subElement.Name);
                    }*/
                    if (prevSubElement != null)
                    {
                        if (!XNode.DeepEquals(prevSubElement, subElement)) //Need to normalize the xml elements and compare them again to catch ordering of subelement changes. 
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
                                //else subElement.Add(new XAttribute("Status", "New"));
                            }
                        }
                        else
                        {
                            if (!(subElement.Name == "Name" || subElement.Name == "Description" || subElement.Name == "Fqn" || subElement.Name == "enMale" || subElement.Name == "Speaker"))
                            {
                                elementsToRemove.Add(subElement); //Element didn't change and it's clogging up the file, saving a shallow copy to remove it later. Because removing it now fucks with the foreach looping
                            }
                            else if (subElement.Attribute("OldValue") == null)
                            {
                                //If this is a "base" element that we want to keep but its unchanged then increment the counter.
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
                        newElement.Add(removedItem); //add removed elements to the return value
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

            if (removeUnchangedElements) // added this check so we can maintain the old functionality with the aid of a checkbox
            {
                for (int i = elementsToRemove.Count() - 1; i >= 0; i--)
                {
                    elementsToRemove[i].Remove(); //removing elements we saved shallow copies off earlier
                }

                if (newElement.Elements().Count() == unmodifiedBaseElemCount)
                {
                    //If all we have are the "base" elements and none are changed then just delete the whole thing.
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
            return ConvertToXElement(fqn, false);
        }

        public XElement ConvertToXElement(string fqn, DataObjectModel dom, bool overrideVerbose)
        {
            var gomItm = dom.GetObject(fqn);
            return ConvertToXElement(gomItm, overrideVerbose);
        }

        public XElement ConvertToXElement(GomLib.GomObject gomItm)
        {
            return ConvertToXElement(gomItm, false);
        }

        public XElement ConvertToXElement(GomLib.GomObject gomItm, bool overrideVerbose)
        {
            if (gomItm != null)
            {
                if (gomItm.Name.Contains("/"))
                    return null;
                switch (gomItm.Name.Substring(0, 3))
                {
                    case "abl":
                        if (!gomItm.Name.Contains("/"))
                            return new GomLib.Models.GameObject().ToXElement(gomItm, !overrideVerbose);
                        break;
                    case "itm":
                    case "npc":
                    case "qst":
                    case "cdx":
                    case "cnv":
                    case "ach":
                    case "tal":
                    case "sch":
                    case "dec": return new GomLib.Models.GameObject().ToXElement(gomItm, !overrideVerbose);
                    default: break;
                }
            }
            return null;
        }

        public XElement ConvertToXElement(object item) //was tired of writing code that duplicated this functionality
        {
            return ConvertToXElement(item, false);
        }

        public XElement ConvertToXElement(object item, bool overrideVerbose) //was tired of writing code that duplicated this functionality
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
                case "GomLib.Models.AbilityPackage": return ((GomLib.Models.GameObject)item).ToXElement(!overrideVerbose);
                case "GomLib.Models.Conquest":
                case "GomLib.Models.scFFShip":
                case "GomLib.Models.Companion":
                case "GomLib.Models.Collection":
                case "GomLib.Models.MtxStorefrontEntry": return ((GomLib.Models.PseudoGameObject)item).ToXElement(!overrideVerbose);
                case "GomLib.Models.ConquestObjective": return ((GomLib.Models.ConquestObjective)item).ToXElement(!overrideVerbose);
                case "GomLib.Models.ConquestData": return ((GomLib.Models.ConquestData)item).ToXElement(!overrideVerbose);
                default:
                    if (item is GomLib.Models.GameObject)
                        return ((GomLib.Models.GameObject)item).ToXElement(!overrideVerbose);
                    else if (item is GomLib.Models.PseudoGameObject)
                        return ((GomLib.Models.PseudoGameObject)item).ToXElement(!overrideVerbose);
                    break;
            }
            return null;
        }

        private static XElement ReferencesToXElement(Dictionary<string, List<ulong>> refs)
        {
            XElement references = new XElement("References");
            if (refs != null)
            {
                foreach (KeyValuePair<string, List<ulong>> entry in refs)
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

        public string ConvertToJson(object itm) //Obsolete
        {
            XElement element = ConvertToXElement(itm); //convert the achievement to XElement

            element.DescendantsAndSelf() //Descendants() grabs all the XElements at every depth, Elements() only grabs the immediate child XElements, and I threw in Self to get the parent XElement, too
                .Where(x => x.HasAttributes) //look for only the descendant XElements with XAttributes so we don't try to execute an object method of a null object
                .Where(x => x.Attribute("Id") != null) //second check to make sure that the XAttribute "Id" is present
                .Attributes("Id") //suck up the "Id" in an IEnumerable<XAttribute> so we can use the Remove() method on them all at once
                .Remove(); //remove the "Id" XAttributes that are used for comparison purposes as they clog up the resulting Json.

            /*
             * XDocument is the newer LINQ to XML format while XmlDocument is the older and harder to manipulate format.
             * You can build and manipulate XDocuments and the related XNode types like I did above for the XAttributes with common LINQ constructors
             * 
             * But the Json serializer only works on XmlDocuments
             * So I put a helper method ToXmlDocument in Tools.cs to handle the conversion.
             */

            string jsonString = Newtonsoft.Json.JsonConvert.SerializeXmlNode(
                new XDocument(element) //wrap the achievement in an XDocument
                    .ToXmlDocument(), //convert to XmlDocument 
                Newtonsoft.Json.Formatting.None, //set the output formatting option
                true //omit the root object
                );

            return jsonString;
        }

        private void GameObjectListAsJSON(string prefix, IEnumerable<GomLib.Models.GameObject> itmList)
        {
            int i = 0;
            short e = 0;
            string n = Environment.NewLine;
            var txtFile = new StringBuilder();
            string filename = String.Format("{0}{1}", prefix, ".json");
            WriteFile("", filename, false);
            var count = itmList.Count();
            foreach (var itm in itmList)
            {
                progressUpdate(i, count);
                if (e % 1000 == 1)
                {
                    WriteFile(txtFile.ToString(), filename, true);
                    txtFile.Clear();
                    e = 0;
                }

                addtolist2(String.Format("{0}: {1}", prefix, itm.Fqn));

                string jsonString = itm.ToJSON(); // ConvertToJson(itm); //added method in Tools.cs
                txtFile.Append(jsonString + Environment.NewLine); //Append it with a newline to the output.
                i++;
                e++;
            }
            addtolist(String.Format("The {0} json file has been generated; there were {1} {0}", prefix, i));
            WriteFile(txtFile.ToString(), filename, true);
            DeleteEmptyFile(filename);
            itmList = null;
            GC.Collect();
            CreateGzip(filename);
            ClearProgress();
        }

        private void PseudoGameObjectListAsJSON(string prefix, List<GomLib.Models.PseudoGameObject> itmList)
        {
            int i = 0;
            short e = 0;
            string n = Environment.NewLine;
            var txtFile = new StringBuilder();
            string filename = String.Format("{0}{1}", prefix, ".json");
            WriteFile("", filename, false);
            var count = itmList.Count();
            for(int c = 0; c < count; c++)
            {
                progressUpdate(i, count);
                if (e % 1000 == 1)
                {
                    WriteFile(txtFile.ToString(), filename, true);
                    txtFile.Clear();
                    e = 0;
                }

                addtolist2(String.Format("{0}: {1}", prefix, itmList[c].Name));

                string jsonString = itmList[c].ToJSON();
                itmList[c] = null;
                txtFile.Append(jsonString + Environment.NewLine); //Append it with a newline to the output.
                i++;
                e++;
            }
            addtolist(String.Format("The {0} json file has been generated; there were {1} {0}", prefix, i));
            WriteFile(txtFile.ToString(), filename, true);
            DeleteEmptyFile(filename);
            itmList = null;
            GC.Collect();
            CreateGzip(filename);
            ClearProgress();
        }

        private void GomObjectListAsJSON(string prefix, IEnumerable<GomLib.GomObject> itmList)
        {
            int i = 0;
            short e = 0;
            string n = Environment.NewLine;
            var txtFile = new StringBuilder();
            string filename = String.Join("", prefix, ".json");
            WriteFile("", filename, false);
            var count = itmList.Count();
            foreach (var itm in itmList)
            {
                progressUpdate(i, count);
                if (e % 1000 == 1)
                {
                    WriteFile(txtFile.ToString(), filename, true);
                    txtFile.Clear();
                    e = 0;
                }

                addtolist2(String.Format("{0}: {1}", prefix, itm.Name));

                string jsonString = LoadGameObject(itm._dom, itm).ToJSON(); // ConvertToJson(itm); //added method in Tools.cs
                txtFile.Append(jsonString + Environment.NewLine); //Append it with a newline to the output.
                i++;
                e++;
            }
            addtolist(String.Format("The {0} json file has been generated; there were {1} {0}", prefix, i));
            WriteFile(txtFile.ToString(), filename, true);
            DeleteEmptyFile(filename);
            itmList = null;
            GC.Collect();
            CreateGzip(filename);
            ClearProgress();
        }
        #endregion
        #region SQL Functions
        private void ObjectListAsSql(string prefix, string xmlRoot, IEnumerable<object> itmList)
        {
            if (prefix == "Removed") return; //not supported as of yet.
            int i = 0;
            short e = 0;
            int f = 1;
            string n = Environment.NewLine;
            var txtFile = new StringBuilder();
            string filename = String.Format("{0}{1}_", prefix, xmlRoot);
            string frs = "{0}{1}.sql";
            WriteFile("", filename, false);
            var count = itmList.Count();
            
            SQLInitStore transInit;
            string transQuery;
            if (initTable.TryGetValue(xmlRoot, out transInit)) //verify that there is an SQL Transaction Query for this object type
            {
                //WriteFile(transInit.InitBegin + n, String.Format("{0}_{1}", filename, f), false);
                transQuery = transInit.InitBegin + n;
                if (sql)
                    sqlTransactionsInitialize(transInit.InitBegin, transInit.InitEnd); //initialize the SQL tranaction if direct SQL output is enabled.
            }
            else
            {
                addtolist2(String.Format("Output type not supported for: {1}", xmlRoot));
                return;
            }
            string joiner = ",";
            foreach (var itm in itmList)
            {
                if (i == count - 1)
                    joiner = "";
                progressUpdate(i, count);

                if (false) //e > 2000)
                {
                    WriteFile(transQuery, String.Format(frs, filename, f), false);
                    string output = txtFile.ToString();
                    output = output.Substring(0, output.Length-3); //trim end characters
                    WriteFile(output, String.Format(frs, filename, f), true);
                    WriteFile(transInit.InitEnd, String.Format(frs, filename, f), true);
                    txtFile.Clear();
                    e = 0;
                    f++;
                }

                addtolist2(String.Format("{0}: {1}", prefix, GetObjectText(itm)));

                string sqlString = ToSQL(itm);
                if (sql)
                    sqlAddTransactionValue(sqlString); //Add to current SQL Transaction if direct SQL output is enabled.
                txtFile.Append(String.Join(joiner, sqlString, n)); //Append it with a newline to the output.
                i++;
                e++;
            }
            txtFile.Append(transInit.InitEnd);
            addtolist(String.Format("The {0} sql file has been generated; there were {1} {0}", prefix, i));
            WriteFile(transQuery, String.Format(frs, filename, f), false);
            //string outputf = txtFile.ToString();
            //outputf = outputf.Substring(0, outputf.Length - 3); //trim end characters
            //WriteFile(outputf, String.Format(frs, filename, f), true);
            WriteFile(txtFile.ToString(), String.Format(frs, filename, f), true);
            //WriteFile(transInit.InitEnd, String.Format(frs, filename, f), true);
            initTable[xmlRoot].OutputCreationSQL(); //output the creation sql file for this table
            sqlTransactionsFlush(); //flush the transaction queue
            DeleteEmptyFile(filename);
            itmList = null;
            GC.Collect();
            for (int j = 1; j <= f; j++)
            {
                CreateGzip(String.Format(frs, filename, j)); //compresses output for upload
            }
            ClearProgress();
        }

        private string GetObjectText(object obj)
        {
            if(obj is GomLib.Models.GameObject)
            {
                return ((GomLib.Models.GameObject)obj).Fqn;
            }
            else if (obj is GomLib.Models.PseudoGameObject)
            {
                return ((GomLib.Models.PseudoGameObject)obj).Name;
            }

            return "";
        }
        
        #endregion
    }
}
