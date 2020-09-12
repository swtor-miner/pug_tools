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
using Newtonsoft.Json;

namespace PugTools
{
    public partial class Tools
    {
        public void GetStrings()
        {
            Clearlist2();

            LoadData();

            //string generatedContent = ConversationStringTables(itmList);

            XElement stringTables;
            int i;
            HashSet<string> foundStringTables = DiscoverStringTables(currentDom);

            if (chkBuildCompare.Checked)
            {
                /*prevfoundStringTables = DiscoverStringTables(previousDom);
                if (foundStringTables.Count != prevfoundStringTables.Count)
                {
                    string pausehere = "";
                }*/
            }

            stringTables = new XElement("StringTables");
            Clearlist2();
            ClearProgress();
            Addtolist2("Loading String Tables.");
            int count = foundStringTables.Count;
            i = 0;
            foreach (string stb in foundStringTables)
            {
                if (chkBuildCompare.Checked)
                {
                    var curTbl = currentDom.stringTable.Find(stb);

                    ProgressUpdate(i, count);
                    var prevTbl = previousDom.stringTable.Find(stb);

                    if (curTbl != null)
                    {
                        if (prevTbl != null)
                        {
                            if (!curTbl.Equals(prevTbl))
                            {
                                Addtolist2(string.Format("Changed: {0}", curTbl.Fqn));
                                XElement oldElement = StbToXElement(prevTbl);
                                XElement newElement = StbToXElement(curTbl);

                                newElement = CompareElements(oldElement, newElement);

                                if (newElement != null)
                                {
                                    newElement.Add(new XAttribute("Status", "Changed"));
                                    if (newElement.Elements().Count() > 0) { stringTables.Add(newElement); }
                                }
                            }
                        }
                        else
                        {
                            Addtolist2(string.Format("New: {0}", curTbl.Fqn));
                            XElement newElement = StbToXElement(curTbl);
                            newElement.Add(new XAttribute("Status", "New"));
                            if (newElement.Elements().Count() > 0) { stringTables.Add(newElement); }
                        }
                    }
                    else if (prevTbl != null)
                    {
                        Addtolist2(string.Format("Removed: {0}", prevTbl.Fqn));
                        XElement remElement = StbToXElement(prevTbl);
                        remElement.Add(new XAttribute("Status", "Removed"));
                        if (remElement.Elements().Count() > 0) { stringTables.Add(remElement); }
                    }
                }
                else
                {
                    Addtolist2("String Table: " + stb);
                    XElement stringTable = StbToXElement(currentDom, stb);
                    //if (stringTable.Elements().Count() > 0)
                    //{
                    stringTables.Add(stringTable);
                    //}
                }
                currentDom.stringTable.Flush(); //Seeing if this helps memory issues
                if (previousDom != null)
                    previousDom.stringTable.Flush();
                i++;
            }

            if (chkBuildCompare.Checked)
            {
                //addtolist("Comparing the Current Abilities to the loaded Patch");

                //XElement addedItems = FindChangedEntries(stringTables, "StringTables", "StringTable");
                XElement addedItems = stringTables;
                Addtolist("The String Tables has been generated there are " + addedItems.Elements("StringTable").Count() + " new/changed String Tables");
                WriteFile(new XDocument(addedItems), "ChangedStringTables.xml", false);
            }
            else
            {
                WriteFile(new XDocument(stringTables), "StringTables.xml", false);
            }
            EnableButtons();
        }

        private HashSet<string> DiscoverStringTables(DataObjectModel dom)
        {
            var itmList = dom.GetObjectsStartingWith("cnv.");
            var doc = XDocument.Load(dom._assets.FindFile("\\resources\\gamedata\\str\\stb.manifest").OpenCopyInMemory());

            dom.stringTable.Flush(); //flushing out any loaded string tables that might have been altered.
            HashSet<string> foundStringTables = new HashSet<string>();
            foreach (XElement element in doc.Element("manifest").Elements("file"))
            {
                foundStringTables.Add(element.Attribute("val").Value);
                //XElement stringTable = StbToXElement(element.Attribute("val").Value);
                //if (stringTable.Elements().Count() > 0) { stringTables.Add(stringTable); }
            }

            foreach (GomObject itm in itmList)
            {
                /*Dictionary<object, object> dialogNodeMap = itm.Data.ValueOrDefault<Dictionary<object, object>>("cnvTreeDialogNodes_Prototype", new Dictionary<object, object>());
                foreach (KeyValuePair<object, object> dialogKvp in dialogNodeMap)
                {
                    long nodeNumber = ((GomObjectData)dialogKvp.Value).Get<long>("cnvNodeNumber");
                    Dictionary<object, object> textMap = ((GomObjectData)dialogKvp.Value).Get<Dictionary<object, object>>("locTextRetrieverMap");
                    if (textMap.ContainsKey(nodeNumber))
                    {
                        string stb = ((GomObjectData)textMap[(long)nodeNumber]).Get<string>("strLocalizedTextRetrieverBucket");
                        foundStringTables.Add(stb);
                    }
                }
                 This is always equal to the conversation node name. We can save a ton of time by just looking at that.
                 */
                string potentialStb = "str." + itm.Name;
                foundStringTables.Add(potentialStb);
                itm.Unload();
            }
            return foundStringTables;
        }

        private XElement StbToXElement(DataObjectModel dom, string stb)
        {
            var stbTable = dom.stringTable.Find(stb);
            //Console.WriteLine(stb);
            if (stbTable == null)
            {
                //Console.WriteLine("Couldn't find " + stb + " string table.");
                return new XElement("StringTable", new XAttribute("Id", stb), new XAttribute("Notfound", "true"));
            }
            return StbToXElement(stbTable);
        }

        private XElement StbToXElement(StringTable stb)
        {
            XElement stringTable = new XElement("StringTable", new XAttribute("Id", stb.Fqn));
            //string stb = "/resources/en-us/" + .Replace(".", "/") + ".stb";
            try
            {
                foreach (var entry in stb.data)
                {
                    //If we are doing a compare build then strip the entries without any values.
                    if (chkBuildCompare.Checked)
                    {
                        string value = entry.Value.LocalizedText["enMale"];
                        if (value != null && value.Length > 0)
                        {
                            stringTable.Add(new XElement("Entry", new XAttribute("Id", entry.Key), value));
                        }
                    }
                    else
                    {
                        if (!string.IsNullOrEmpty(entry.Value.LocalizedText["enMale"]))
                            stringTable.Add(new XElement("Entry", new XAttribute("Id", entry.Key),
                                new XElement("en", entry.Value.LocalizedText["enMale"]),
                                new XElement("fr", entry.Value.LocalizedText["frMale"]),
                                new XElement("de", entry.Value.LocalizedText["deMale"])
                                ));
                    }
                }

            }
            catch
            {
                //Console.WriteLine("Couldn't find " + stb + " string table.");
            }
            return stringTable;
        }
    }
}
